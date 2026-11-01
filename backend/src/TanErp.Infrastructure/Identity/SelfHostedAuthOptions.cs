using Microsoft.Extensions.Configuration;

namespace TanErp.Infrastructure.Identity;

public sealed class SelfHostedAuthOptions
{
    public const string SectionName = "Authentication";
    public const int MinimumKeyLength = 32;

    private const string NonProductionFallbackJwtKey = "tan-erp-dev-test-only-jwt-signing-key-minimum-32-chars!";
    private const string NonProductionFallbackEncryptionKey = "tan-erp-dev-test-only-aes-gcm-encryption-key-32-chars!";

    public string JwtSigningKey { get; }
    public string EncryptionKey { get; }
    public string? GoogleClientId { get; }

    public SelfHostedAuthOptions(string jwtSigningKey, string encryptionKey, string? googleClientId = null)
    {
        if (string.IsNullOrWhiteSpace(jwtSigningKey) || jwtSigningKey.Trim().Length < MinimumKeyLength)
        {
            throw new InvalidOperationException(
                $"Required configuration 'Authentication:JwtSigningKey' must be at least {MinimumKeyLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(encryptionKey) || encryptionKey.Trim().Length < MinimumKeyLength)
        {
            throw new InvalidOperationException(
                $"Required configuration 'Authentication:EncryptionKey' must be at least {MinimumKeyLength} characters.");
        }

        JwtSigningKey = jwtSigningKey.Trim();
        EncryptionKey = encryptionKey.Trim();
        GoogleClientId = string.IsNullOrWhiteSpace(googleClientId) ? null : googleClientId.Trim();
    }

    public static SelfHostedAuthOptions FromConfiguration(IConfiguration configuration, string? environmentName)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var env = environmentName
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Production";
        var isProduction = string.Equals(env, "Production", StringComparison.OrdinalIgnoreCase);

        var rawJwtKey = configuration["Authentication:JwtSigningKey"];
        var rawEncryptionKey = configuration["Authentication:EncryptionKey"];
        var googleClientId = configuration["Authentication:Google:ClientId"];

        if (isProduction)
        {
            return new SelfHostedAuthOptions(rawJwtKey ?? string.Empty, rawEncryptionKey ?? string.Empty, googleClientId);
        }

        var resolvedJwtKey = !string.IsNullOrWhiteSpace(rawJwtKey) && rawJwtKey.Trim().Length >= MinimumKeyLength
            ? rawJwtKey.Trim()
            : NonProductionFallbackJwtKey;

        var resolvedEncryptionKey = !string.IsNullOrWhiteSpace(rawEncryptionKey) && rawEncryptionKey.Trim().Length >= MinimumKeyLength
            ? rawEncryptionKey.Trim()
            : NonProductionFallbackEncryptionKey;

        return new SelfHostedAuthOptions(resolvedJwtKey, resolvedEncryptionKey, googleClientId);
    }
}
