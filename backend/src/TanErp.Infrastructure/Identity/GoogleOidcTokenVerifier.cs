using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Identity;

public sealed class GoogleOidcTokenVerifier : IGoogleTokenVerifier, IDisposable
{
    public const string DefaultJwksEndpoint = "https://www.googleapis.com/oauth2/v3/certs";
    private static readonly TimeSpan JwksCacheDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan MinKeyRefreshInterval = TimeSpan.FromMinutes(1);

    private readonly Func<HttpClient> _httpClientFactory;
    private readonly SelfHostedAuthOptions _options;
    private readonly IClock _clock;
    private readonly string _jwksEndpoint;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private IReadOnlyDictionary<string, RsaJwkKey>? _cachedKeys;
    private DateTimeOffset _cachedAtUtc;
    private DateTimeOffset _expiresAtUtc;

    public GoogleOidcTokenVerifier(
        Func<HttpClient> httpClientFactory,
        SelfHostedAuthOptions options,
        IClock clock,
        string jwksEndpoint = DefaultJwksEndpoint)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _jwksEndpoint = jwksEndpoint;
    }

    public async Task<Result<GoogleVerifiedIdentity>> VerifyIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_options.GoogleClientId))
        {
            return Result<GoogleVerifiedIdentity>.Failure(
                new Error("GOOGLE_AUTH_NOT_CONFIGURED", "Google Sign-In is not configured."));
        }

        if (string.IsNullOrWhiteSpace(idToken))
        {
            return InvalidToken();
        }

        var parts = idToken.Trim().Split('.');
        if (parts.Length != 3 || parts[0].Length == 0 || parts[1].Length == 0 || parts[2].Length == 0)
        {
            return InvalidToken();
        }

        byte[] headerBytes;
        byte[] payloadBytes;
        byte[] signatureBytes;
        try
        {
            headerBytes = Base64Url.DecodeFromChars(parts[0]);
            payloadBytes = Base64Url.DecodeFromChars(parts[1]);
            signatureBytes = Base64Url.DecodeFromChars(parts[2]);
        }
        catch (FormatException)
        {
            return InvalidToken();
        }

        string? kid;
        try
        {
            using var headerDoc = JsonDocument.Parse(headerBytes);
            var hRoot = headerDoc.RootElement;
            if (!TryGetString(hRoot, "alg", out var alg) || !string.Equals(alg, "RS256", StringComparison.Ordinal))
            {
                return InvalidToken();
            }

            if (!TryGetString(hRoot, "kid", out kid) || string.IsNullOrWhiteSpace(kid))
            {
                return InvalidToken();
            }
        }
        catch (JsonException)
        {
            return InvalidToken();
        }

        string? sub;
        string? email;
        string? displayName;
        var now = _clock.UtcNow;

        try
        {
            using var payloadDoc = JsonDocument.Parse(payloadBytes);
            var pRoot = payloadDoc.RootElement;

            if (!TryGetString(pRoot, "iss", out var iss)
                || (!string.Equals(iss, "https://accounts.google.com", StringComparison.Ordinal)
                    && !string.Equals(iss, "accounts.google.com", StringComparison.Ordinal)))
            {
                return InvalidToken();
            }

            if (!MatchesAudience(pRoot, _options.GoogleClientId))
            {
                return InvalidToken();
            }

            if (!pRoot.TryGetProperty("exp", out var expProp)
                || expProp.ValueKind != JsonValueKind.Number
                || !expProp.TryGetInt64(out var expSeconds)
                || expSeconds <= now.ToUnixTimeSeconds())
            {
                return InvalidToken();
            }

            if (!pRoot.TryGetProperty("email_verified", out var evProp)
                || (evProp.ValueKind != JsonValueKind.True
                    && !(evProp.ValueKind == JsonValueKind.String && string.Equals(evProp.GetString(), "true", StringComparison.OrdinalIgnoreCase))))
            {
                return InvalidToken();
            }

            if (!TryGetString(pRoot, "sub", out sub) || string.IsNullOrWhiteSpace(sub))
            {
                return InvalidToken();
            }

            if (!TryGetString(pRoot, "email", out email) || string.IsNullOrWhiteSpace(email))
            {
                return InvalidToken();
            }

            TryGetString(pRoot, "name", out displayName);
        }
        catch (JsonException)
        {
            return InvalidToken();
        }

        var jwk = await ResolveSigningKeyAsync(kid, now, cancellationToken);
        if (jwk is null)
        {
            return InvalidToken();
        }

        try
        {
            using var rsa = RSA.Create();
            rsa.ImportParameters(new RSAParameters
            {
                Modulus = jwk.Modulus,
                Exponent = jwk.Exponent
            });

            var signedBytes = Encoding.ASCII.GetBytes($"{parts[0]}.{parts[1]}");
            var valid = rsa.VerifyData(signedBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            if (!valid)
            {
                return InvalidToken();
            }
        }
        catch (CryptographicException)
        {
            return InvalidToken();
        }

        return Result<GoogleVerifiedIdentity>.Success(new GoogleVerifiedIdentity(
            sub.Trim(),
            email.Trim(),
            string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim()));
    }

    private async Task<RsaJwkKey?> ResolveSigningKeyAsync(string kid, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var snapshot = _cachedKeys;
        if (snapshot is not null && now < _expiresAtUtc && snapshot.TryGetValue(kid, out var hit))
        {
            return hit;
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedKeys is not null && now < _expiresAtUtc && _cachedKeys.TryGetValue(kid, out var recheckedHit))
            {
                return recheckedHit;
            }

            if (_cachedKeys is not null && now < _cachedAtUtc.Add(MinKeyRefreshInterval) && now < _expiresAtUtc)
            {
                return null;
            }

            using var client = _httpClientFactory();
            using var response = await client.GetAsync(_jwksEndpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var parsed = new Dictionary<string, RsaJwkKey>(StringComparer.Ordinal);
            if (doc.RootElement.TryGetProperty("keys", out var keysArray) && keysArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var keyEl in keysArray.EnumerateArray())
                {
                    if (!TryGetString(keyEl, "kty", out var kty) || !string.Equals(kty, "RSA", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!TryGetString(keyEl, "kid", out var keyId) || string.IsNullOrWhiteSpace(keyId))
                    {
                        continue;
                    }

                    if (!TryGetString(keyEl, "n", out var nB64) || string.IsNullOrWhiteSpace(nB64)
                        || !TryGetString(keyEl, "e", out var eB64) || string.IsNullOrWhiteSpace(eB64))
                    {
                        continue;
                    }

                    try
                    {
                        var modulus = Base64Url.DecodeFromChars(nB64);
                        var exponent = Base64Url.DecodeFromChars(eB64);
                        parsed[keyId] = new RsaJwkKey(modulus, exponent);
                    }
                    catch (FormatException)
                    {
                        // Ignore malformed JWK entry
                    }
                }
            }

            _cachedKeys = parsed;
            _cachedAtUtc = now;
            _expiresAtUtc = now.Add(JwksCacheDuration);

            return parsed.TryGetValue(kid, out var resolved) ? resolved : null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private static bool MatchesAudience(JsonElement payload, string expectedClientId)
    {
        if (!payload.TryGetProperty("aud", out var audProp))
        {
            return false;
        }

        if (audProp.ValueKind == JsonValueKind.String)
        {
            return string.Equals(audProp.GetString(), expectedClientId, StringComparison.Ordinal);
        }

        if (audProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in audProp.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String
                    && string.Equals(item.GetString(), expectedClientId, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryGetString(JsonElement element, string propertyName, out string? value)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            value = prop.GetString();
            return true;
        }

        value = null;
        return false;
    }

    private static Result<GoogleVerifiedIdentity> InvalidToken() =>
        Result<GoogleVerifiedIdentity>.Failure(new Error("GOOGLE_TOKEN_INVALID", "Google identity token is invalid or expired."));

    public void Dispose()
    {
        _cacheLock.Dispose();
    }

    private sealed record RsaJwkKey(byte[] Modulus, byte[] Exponent);
}
