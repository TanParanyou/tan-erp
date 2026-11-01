using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Identity;

public sealed class SelfHostedJwtService : ISelfHostedJwtService
{
    private static readonly string EncodedHeader = Base64Url.EncodeToString(
        Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));

    private readonly byte[] _signingKeyBytes;

    public SelfHostedJwtService(SelfHostedAuthOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _signingKeyBytes = Encoding.UTF8.GetBytes(options.JwtSigningKey);
    }

    public AccessTokenIssueResult CreateAccessToken(
        Guid userId,
        string subjectUid,
        Guid sessionId,
        string email,
        DateTimeOffset now)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User ID is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(subjectUid)) throw new ArgumentException("Subject UID is required.", nameof(subjectUid));
        if (sessionId == Guid.Empty) throw new ArgumentException("Session ID is required.", nameof(sessionId));

        var expiresAt = now.Add(SelfHostedJwtConstants.AccessTokenLifetime);
        var payload = new Dictionary<string, object>
        {
            ["iss"] = SelfHostedJwtConstants.Issuer,
            ["aud"] = SelfHostedJwtConstants.ApiAudience,
            ["sub"] = subjectUid.Trim(),
            ["uid"] = userId.ToString(),
            ["sid"] = sessionId.ToString(),
            ["email"] = email ?? string.Empty,
            ["email_verified"] = true,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds()
        };

        var token = SignPayload(payload);
        return new AccessTokenIssueResult(token, expiresAt);
    }

    public AccessTokenClaims? VerifyAccessToken(string token, DateTimeOffset now)
    {
        using var payloadDoc = VerifyAndReadPayload(token, SelfHostedJwtConstants.ApiAudience, now);
        if (payloadDoc is null)
        {
            return null;
        }

        var root = payloadDoc.RootElement;
        if (!TryGetString(root, "sub", out var sub) || string.IsNullOrWhiteSpace(sub))
        {
            return null;
        }

        if (!TryGetGuid(root, "uid", out var userId) || userId == Guid.Empty)
        {
            return null;
        }

        if (!TryGetGuid(root, "sid", out var sessionId) || sessionId == Guid.Empty)
        {
            return null;
        }

        if (!TryGetUnixSeconds(root, "exp", out var expSeconds))
        {
            return null;
        }

        TryGetString(root, "email", out var email);
        var emailVerified = root.TryGetProperty("email_verified", out var evProp) && evProp.ValueKind == JsonValueKind.True;

        return new AccessTokenClaims(
            userId,
            sub,
            sessionId,
            email ?? string.Empty,
            emailVerified,
            DateTimeOffset.FromUnixTimeSeconds(expSeconds));
    }

    public string CreateTwoFactorChallengeToken(
        Guid userId,
        string subjectUid,
        string purpose,
        DateTimeOffset now)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User ID is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(subjectUid)) throw new ArgumentException("Subject UID is required.", nameof(subjectUid));
        if (purpose is not SelfHostedJwtConstants.PurposeTwoFactorVerify and not SelfHostedJwtConstants.PurposeTwoFactorSetup)
        {
            throw new ArgumentException("Unsupported two-factor token purpose.", nameof(purpose));
        }

        var lifetime = purpose == SelfHostedJwtConstants.PurposeTwoFactorSetup
            ? SelfHostedJwtConstants.TwoFactorSetupLifetime
            : SelfHostedJwtConstants.TwoFactorVerifyLifetime;

        var expiresAt = now.Add(lifetime);
        var payload = new Dictionary<string, object>
        {
            ["iss"] = SelfHostedJwtConstants.Issuer,
            ["aud"] = SelfHostedJwtConstants.TwoFactorAudience,
            ["sub"] = subjectUid.Trim(),
            ["uid"] = userId.ToString(),
            ["purpose"] = purpose,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds()
        };

        return SignPayload(payload);
    }

    public TwoFactorChallengeClaims? VerifyTwoFactorChallengeToken(string token, DateTimeOffset now)
    {
        using var payloadDoc = VerifyAndReadPayload(token, SelfHostedJwtConstants.TwoFactorAudience, now);
        if (payloadDoc is null)
        {
            return null;
        }

        var root = payloadDoc.RootElement;
        if (!TryGetGuid(root, "uid", out var userId) || userId == Guid.Empty)
        {
            return null;
        }

        if (!TryGetString(root, "sub", out var sub) || string.IsNullOrWhiteSpace(sub))
        {
            return null;
        }

        if (!TryGetString(root, "purpose", out var purpose)
            || purpose is not SelfHostedJwtConstants.PurposeTwoFactorVerify and not SelfHostedJwtConstants.PurposeTwoFactorSetup)
        {
            return null;
        }

        if (!TryGetUnixSeconds(root, "exp", out var expSeconds))
        {
            return null;
        }

        return new TwoFactorChallengeClaims(
            userId,
            sub,
            purpose,
            DateTimeOffset.FromUnixTimeSeconds(expSeconds));
    }

    private string SignPayload(Dictionary<string, object> payload)
    {
        var payloadJson = JsonSerializer.SerializeToUtf8Bytes(payload);
        var encodedPayload = Base64Url.EncodeToString(payloadJson);
        var signingInput = $"{EncodedHeader}.{encodedPayload}";

        Span<byte> signature = stackalloc byte[32];
        HMACSHA256.HashData(_signingKeyBytes, Encoding.ASCII.GetBytes(signingInput), signature);
        var encodedSignature = Base64Url.EncodeToString(signature);

        return $"{signingInput}.{encodedSignature}";
    }

    private JsonDocument? VerifyAndReadPayload(string? token, string expectedAudience, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var parts = token.Trim().Split('.');
        if (parts.Length != 3 || parts[0].Length == 0 || parts[1].Length == 0 || parts[2].Length == 0)
        {
            return null;
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
            return null;
        }

        if (signatureBytes.Length != 32)
        {
            return null;
        }

        try
        {
            using var headerDoc = JsonDocument.Parse(headerBytes);
            if (!TryGetString(headerDoc.RootElement, "alg", out var alg)
                || !string.Equals(alg, "HS256", StringComparison.Ordinal))
            {
                return null;
            }
        }
        catch (JsonException)
        {
            return null;
        }

        var signingInputBytes = Encoding.ASCII.GetBytes($"{parts[0]}.{parts[1]}");
        Span<byte> expectedSignature = stackalloc byte[32];
        HMACSHA256.HashData(_signingKeyBytes, signingInputBytes, expectedSignature);

        if (!CryptographicOperations.FixedTimeEquals(signatureBytes, expectedSignature))
        {
            return null;
        }

        JsonDocument payloadDoc;
        try
        {
            payloadDoc = JsonDocument.Parse(payloadBytes);
        }
        catch (JsonException)
        {
            return null;
        }

        var root = payloadDoc.RootElement;
        if (!TryGetString(root, "aud", out var aud) || !string.Equals(aud, expectedAudience, StringComparison.Ordinal))
        {
            payloadDoc.Dispose();
            return null;
        }

        if (!TryGetUnixSeconds(root, "exp", out var expSeconds) || expSeconds <= now.ToUnixTimeSeconds())
        {
            payloadDoc.Dispose();
            return null;
        }

        return payloadDoc;
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

    private static bool TryGetGuid(JsonElement element, string propertyName, out Guid value)
    {
        if (TryGetString(element, propertyName, out var raw) && Guid.TryParse(raw, out value))
        {
            return true;
        }

        value = Guid.Empty;
        return false;
    }

    private static bool TryGetUnixSeconds(JsonElement element, string propertyName, out long seconds)
    {
        if (element.TryGetProperty(propertyName, out var prop)
            && prop.ValueKind == JsonValueKind.Number
            && prop.TryGetInt64(out seconds))
        {
            return true;
        }

        seconds = 0;
        return false;
    }
}
