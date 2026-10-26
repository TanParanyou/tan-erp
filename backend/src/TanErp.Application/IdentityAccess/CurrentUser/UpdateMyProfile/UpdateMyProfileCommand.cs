namespace TanErp.Application.IdentityAccess.CurrentUser.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
    string FirebaseUid,
    Guid MembershipId,
    string DisplayName,
    string? PhoneNumber,
    string? PreferredLocale,
    Guid? AvatarFileId,
    string TraceId);

public sealed record UpdateMyProfileInput(
    string DisplayName,
    string? PhoneNumber,
    string? PreferredLocale,
    Guid? AvatarFileId);
