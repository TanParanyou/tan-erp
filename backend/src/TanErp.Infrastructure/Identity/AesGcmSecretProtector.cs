using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Identity;

public sealed class AesGcmSecretProtector : ISecretProtector
{
    private const string FormatVersion = "v1";
    private const int NonceByteLength = 12;
    private const int TagByteLength = 16;

    private readonly byte[] _key;

    public AesGcmSecretProtector(SelfHostedAuthOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(options.EncryptionKey));
    }

    public string Protect(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            throw new ArgumentException("Plaintext secret cannot be empty.", nameof(plaintext));
        }

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        Span<byte> nonce = stackalloc byte[NonceByteLength];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintextBytes.Length];
        Span<byte> tag = stackalloc byte[TagByteLength];

        using var aes = new AesGcm(_key, TagByteLength);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        return $"{FormatVersion}.{Base64Url.EncodeToString(nonce)}.{Base64Url.EncodeToString(ciphertext)}.{Base64Url.EncodeToString(tag)}";
    }

    public string Unprotect(string protectedPayload)
    {
        if (string.IsNullOrWhiteSpace(protectedPayload))
        {
            throw new CryptographicException("Protected secret payload cannot be empty.");
        }

        var parts = protectedPayload.Split('.');
        if (parts.Length != 4 || !string.Equals(parts[0], FormatVersion, StringComparison.Ordinal))
        {
            throw new CryptographicException("Unsupported protected secret format.");
        }

        byte[] nonce;
        byte[] ciphertext;
        byte[] tag;
        try
        {
            nonce = Base64Url.DecodeFromChars(parts[1]);
            ciphertext = Base64Url.DecodeFromChars(parts[2]);
            tag = Base64Url.DecodeFromChars(parts[3]);
        }
        catch (FormatException ex)
        {
            throw new CryptographicException("Malformed base64url in protected secret.", ex);
        }

        if (nonce.Length != NonceByteLength || tag.Length != TagByteLength || ciphertext.Length == 0)
        {
            throw new CryptographicException("Invalid nonce, tag, or ciphertext length.");
        }

        var plaintextBytes = new byte[ciphertext.Length];
        using var aes = new AesGcm(_key, TagByteLength);
        aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
