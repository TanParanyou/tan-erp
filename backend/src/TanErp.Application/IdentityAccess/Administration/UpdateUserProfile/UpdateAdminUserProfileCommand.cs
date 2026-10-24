namespace TanErp.Application.IdentityAccess.Administration.UpdateUserProfile;

public sealed record UpdateAdminUserProfileCommand(
    AdminCaller Caller,
    Guid UserId,
    Guid ExpectedRowVersion,
    string DisplayName,
    string? PhoneNumber,
    string? EmployeeCode,
    Guid? DepartmentId,
    Guid? JobTitleId,
    Guid? AvatarFileId,
    string? Notes,
    string TraceId);
