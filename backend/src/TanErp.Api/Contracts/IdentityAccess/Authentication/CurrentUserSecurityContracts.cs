using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Api.Contracts.IdentityAccess.Authentication;

public sealed record MySecurityStatusResponse(
    bool HasPassword,
    bool HasGoogleLinked,
    bool IsTotpEnabled,
    bool RequireTwoFactor,
    int RemainingBackupCodesCount,
    DateTimeOffset? TotpEnabledAtUtc,
    DateTimeOffset? PasswordChangedAtUtc)
{
    public static MySecurityStatusResponse From(MySecurityStatus status) => new(
        status.HasPassword,
        status.HasGoogleLinked,
        status.IsTotpEnabled,
        status.RequireTwoFactor,
        status.RemainingBackupCodesCount,
        status.TotpEnabledAtUtc,
        status.PasswordChangedAtUtc);
}

public sealed record ChangeMyPasswordRequest(
    string? CurrentPassword,
    string? NewPassword);

public sealed record EnableTwoFactorRequest(string? Code);

public sealed record DisableTwoFactorRequest(string? Code);

public sealed record RegenerateBackupCodesRequest(string? Code);

public sealed record TwoFactorBackupCodesResponse(IReadOnlyList<string> BackupCodes);

public sealed record ActiveSessionItemDto(
    Guid Id,
    string DeviceLabel,
    string ClientPlatform,
    string IpAddress,
    string UserAgent,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastUsedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    bool IsCurrent)
{
    public static ActiveSessionItemDto From(ActiveSessionItem item) => new(
        item.Id,
        item.DeviceLabel,
        item.ClientPlatform,
        item.IpAddress,
        item.UserAgent,
        item.CreatedAtUtc,
        item.LastUsedAtUtc,
        item.ExpiresAtUtc,
        item.IsCurrent);
}

public sealed record ActiveSessionListResponse(IReadOnlyList<ActiveSessionItemDto> Items);

public sealed record RevokeSessionsResponse(int RevokedCount);
