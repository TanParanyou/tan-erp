using System.Buffers.Text;
using System.Security.Cryptography;
using TanErp.Application.Common.Security;

namespace TanErp.Application.IdentityAccess.Authentication;

public sealed record GeneratedRefreshToken(string WireToken, string SecretHash);

public sealed record ParsedRefreshToken(Guid SessionId, string SecretHash);

public static class RefreshTokenFormat
{
    public static GeneratedRefreshToken Generate(Guid sessionId)
    {
        Span<byte> secretBytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(secretBytes);
        var secret = Base64Url.EncodeToString(secretBytes);
        var secretHash = Sha256Hex.Compute(secret);
        return new GeneratedRefreshToken($"{sessionId:N}.{secret}", secretHash);
    }

    public static ParsedRefreshToken? TryParse(string? wireToken)
    {
        if (string.IsNullOrWhiteSpace(wireToken))
        {
            return null;
        }

        var trimmed = wireToken.Trim();
        var dotIndex = trimmed.IndexOf('.');
        if (dotIndex <= 0 || dotIndex >= trimmed.Length - 1)
        {
            return null;
        }

        var sessionPart = trimmed[..dotIndex];
        var secretPart = trimmed[(dotIndex + 1)..];
        if (secretPart.Length < 16 || secretPart.Length > 256 || secretPart.Contains('.'))
        {
            return null;
        }

        if (!Guid.TryParse(sessionPart, out var sessionId) || sessionId == Guid.Empty)
        {
            return null;
        }

        return new ParsedRefreshToken(sessionId, Sha256Hex.Compute(secretPart));
    }
}
