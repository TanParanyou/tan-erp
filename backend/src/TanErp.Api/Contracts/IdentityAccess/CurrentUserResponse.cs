namespace TanErp.Api.Contracts.IdentityAccess;

public sealed record CurrentUserResponse(
    UserDto User,
    IReadOnlyList<MembershipDto> Memberships);

public sealed record UserDto(
    Guid Id,
    string DisplayName,
    string Email,
    string? PhoneNumber = null,
    Guid? AvatarFileId = null,
    string? PreferredLocale = null,
    UserPreferencesDto? Preferences = null);

public sealed record UserPreferencesDto(string ThemeMode, string Density, int TextScale);

public sealed record UpdateMyPreferencesRequest(string? ThemeMode, string? Density, int? TextScale, string? PreferredLocale = null);

public sealed record UpdateMyProfileRequest(
    string? DisplayName,
    string? PhoneNumber = null,
    string? PreferredLocale = null,
    Guid? AvatarFileId = null);

public sealed record MembershipDto(
    Guid Id,
    OrganizationDto Organization,
    BranchDto? Branch,
    IReadOnlyList<PermissionDto> Permissions);

public sealed record OrganizationDto(
    Guid Id,
    string Name,
    OrganizationBrandingDto? Branding = null);

/// <summary>Null PrimaryColor means the default Atelier Navy; null LogoUrl means no logo.</summary>
public sealed record OrganizationBrandingDto(string? PrimaryColor, string? LogoUrl, Guid? LogoFileId);

public sealed record BranchDto(
    Guid Id,
    string Name);

public sealed record PermissionDto(
    string Key,
    string Scope,
    Guid? ScopeId);
