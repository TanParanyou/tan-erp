using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TanErp.Application.Common.Security;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Identity;

public sealed class Rfc6238TotpService : ITotpService
{
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    private const string BackupCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int SecretByteLength = 20;
    private const int TimeStepSeconds = 30;
    private const int Digits = 6;

    public string GenerateSecretKey()
    {
        Span<byte> bytes = stackalloc byte[SecretByteLength];
        RandomNumberGenerator.Fill(bytes);
        return EncodeBase32(bytes);
    }

    public string BuildOtpAuthUri(string email, string base32Secret)
    {
        var escapedEmail = Uri.EscapeDataString((email ?? string.Empty).Trim());
        var cleanSecret = NormalizeBase32(base32Secret);
        return $"otpauth://totp/tan-erp:{escapedEmail}?secret={cleanSecret}&issuer=tan-erp&algorithm=SHA1&digits={Digits}&period={TimeStepSeconds}";
    }

    public string ComputeCode(string base32Secret, DateTimeOffset utcNow)
    {
        var secretBytes = DecodeBase32(base32Secret);
        var timeStep = utcNow.ToUnixTimeSeconds() / TimeStepSeconds;
        return ComputeHotp(secretBytes, timeStep);
    }

    public bool VerifyCode(string base32Secret, string code, DateTimeOffset utcNow)
    {
        if (!IsSixDigitTotp(code) || string.IsNullOrWhiteSpace(base32Secret))
        {
            return false;
        }

        byte[] secretBytes;
        try
        {
            secretBytes = DecodeBase32(base32Secret);
        }
        catch (FormatException)
        {
            return false;
        }

        if (secretBytes.Length == 0)
        {
            return false;
        }

        var trimmedCodeBytes = Encoding.ASCII.GetBytes(code.Trim());
        var currentStep = utcNow.ToUnixTimeSeconds() / TimeStepSeconds;

        var matched = false;
        for (var offset = -1; offset <= 1; offset++)
        {
            var candidate = ComputeHotp(secretBytes, currentStep + offset);
            var candidateBytes = Encoding.ASCII.GetBytes(candidate);
            if (CryptographicOperations.FixedTimeEquals(trimmedCodeBytes, candidateBytes))
            {
                matched = true;
            }
        }

        return matched;
    }

    public IReadOnlyList<string> GenerateBackupCodes(int count = 8)
    {
        if (count <= 0 || count > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Backup code count must be between 1 and 32.");
        }

        var codes = new List<string>(count);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        Span<char> chars = stackalloc char[9];

        while (codes.Count < count)
        {
            for (var i = 0; i < 9; i++)
            {
                if (i == 4)
                {
                    chars[i] = '-';
                    continue;
                }

                chars[i] = BackupCodeAlphabet[RandomNumberGenerator.GetInt32(BackupCodeAlphabet.Length)];
            }

            var code = new string(chars);
            if (seen.Add(code))
            {
                codes.Add(code);
            }
        }

        return codes;
    }

    public string NormalizeBackupCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(code.Length);
        foreach (var ch in code)
        {
            if (ch == '-' || char.IsWhiteSpace(ch))
            {
                continue;
            }

            sb.Append(char.ToUpperInvariant(ch));
        }

        return sb.ToString();
    }

    public string HashBackupCode(string code) =>
        Sha256Hex.Compute(NormalizeBackupCode(code));

    public bool IsSixDigitTotp(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var trimmed = code.Trim();
        if (trimmed.Length != Digits)
        {
            return false;
        }

        foreach (var ch in trimmed)
        {
            if (!char.IsAsciiDigit(ch))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsBackupCodeCandidate(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var normalized = NormalizeBackupCode(code);
        if (normalized.Length != 8)
        {
            return false;
        }

        foreach (var ch in normalized)
        {
            if (!BackupCodeAlphabet.Contains(ch))
            {
                return false;
            }
        }

        return true;
    }

    private static string ComputeHotp(byte[] secret, long counter)
    {
        Span<byte> counterBytes = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(counterBytes, counter);

        Span<byte> hash = stackalloc byte[20];
        HMACSHA1.HashData(secret, counterBytes, hash);

        var offset = hash[19] & 0x0F;
        var binaryCode =
            ((hash[offset] & 0x7F) << 24)
            | ((hash[offset + 1] & 0xFF) << 16)
            | ((hash[offset + 2] & 0xFF) << 8)
            | (hash[offset + 3] & 0xFF);

        var otp = binaryCode % 1_000_000;
        return otp.ToString("D6", CultureInfo.InvariantCulture);
    }

    public static string EncodeBase32(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return string.Empty;
        }

        var outputLength = (data.Length * 8 + 4) / 5;
        var sb = new StringBuilder(outputLength);
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var b in data)
        {
            buffer = (buffer << 8) | b;
            bitsLeft += 8;
            while (bitsLeft >= 5)
            {
                var index = (buffer >> (bitsLeft - 5)) & 0x1F;
                bitsLeft -= 5;
                sb.Append(Base32Alphabet[index]);
            }
        }

        if (bitsLeft > 0)
        {
            var index = (buffer << (5 - bitsLeft)) & 0x1F;
            sb.Append(Base32Alphabet[index]);
        }

        return sb.ToString();
    }

    public static byte[] DecodeBase32(string base32)
    {
        var clean = NormalizeBase32(base32);
        if (clean.Length == 0)
        {
            return Array.Empty<byte>();
        }

        var output = new List<byte>(clean.Length * 5 / 8);
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var ch in clean)
        {
            var val = Base32Alphabet.IndexOf(ch);
            if (val < 0)
            {
                throw new FormatException($"Invalid Base32 character '{ch}'.");
            }

            buffer = (buffer << 5) | val;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                output.Add((byte)((buffer >> (bitsLeft - 8)) & 0xFF));
                bitsLeft -= 8;
            }
        }

        return output.ToArray();
    }

    private static string NormalizeBase32(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if (ch == '=' || ch == '-' || char.IsWhiteSpace(ch))
            {
                continue;
            }

            sb.Append(char.ToUpperInvariant(ch));
        }

        return sb.ToString();
    }
}
