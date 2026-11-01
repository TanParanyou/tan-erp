using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Identity;

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string AlgorithmPrefix = "pbkdf2-sha512";
    public const int DefaultIterations = 210_000;
    private const int SaltByteLength = 16;
    private const int HashByteLength = 32;
    private const int MinAcceptedIterations = 10_000;
    private const int MaxAcceptedIterations = 5_000_000;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        Span<byte> salt = stackalloc byte[SaltByteLength];
        RandomNumberGenerator.Fill(salt);

        Span<byte> hash = stackalloc byte[HashByteLength];
        Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            hash,
            DefaultIterations,
            HashAlgorithmName.SHA512);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{AlgorithmPrefix}${DefaultIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}");
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var parts = passwordHash.Split('$');
        if (parts.Length != 4 || !string.Equals(parts[0], AlgorithmPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var iterations)
            || iterations < MinAcceptedIterations
            || iterations > MaxAcceptedIterations)
        {
            return false;
        }

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (salt.Length < 8 || expectedHash.Length < 16)
        {
            return false;
        }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA512,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
