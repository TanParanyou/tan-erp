using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.CurrentUser.UpdateMyProfile;

/// <summary>Self-service profile edit. Access rule is "this row is mine": an active membership, no permission key.</summary>
public class UpdateMyProfileHandler
{
    private const int MaxDisplayNameLength = 255;
    private const int MaxPhoneLength = 50;

    private readonly IRequestAccessResolver _access;
    private readonly IMyProfileStore _store;

    public UpdateMyProfileHandler(IRequestAccessResolver access, IMyProfileStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<UserContext>> Handle(UpdateMyProfileCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveMembershipAsync(command.FirebaseUid, command.MembershipId, cancellationToken);
        if (access.IsFailure) return Result<UserContext>.Failure(access.Error);

        var name = command.DisplayName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > MaxDisplayNameLength)
            return Validation("Display name is required (max 255 characters).");

        var phone = command.PhoneNumber?.Trim();
        if (phone is { Length: > MaxPhoneLength })
            return Validation("Phone number must not exceed 50 characters.");

        var locale = string.IsNullOrWhiteSpace(command.PreferredLocale) ? null : command.PreferredLocale.Trim().ToLowerInvariant();
        if (locale is not null && !User.SupportedLocales.Contains(locale))
            return Validation("Preferred locale must be 'th' or 'en'.");

        return await _store.UpdateAsync(
            access.Value!.OrganizationId,
            new AdminActor(access.Value.ActorUserId, access.Value.MembershipId),
            new UpdateMyProfileInput(name, string.IsNullOrEmpty(phone) ? null : phone, locale, command.AvatarFileId),
            command.TraceId,
            cancellationToken);
    }

    private static Result<UserContext> Validation(string message) =>
        Result<UserContext>.Failure(new Error("REQUEST_VALIDATION_FAILED", message));
}
