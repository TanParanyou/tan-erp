using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.CurrentUser.UpdateMyPreferences;

/// <summary>Self-service theme preferences. Access rule is "this row is mine": an active membership, no permission key.</summary>
public class UpdateMyPreferencesHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IMyPreferencesStore _store;

    public UpdateMyPreferencesHandler(IRequestAccessResolver access, IMyPreferencesStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<UserContext>> Handle(UpdateMyPreferencesCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveMembershipAsync(command.FirebaseUid, command.MembershipId, cancellationToken);
        if (access.IsFailure) return Result<UserContext>.Failure(access.Error);

        var themeMode = command.ThemeMode?.Trim().ToLowerInvariant();
        if (themeMode is null || !User.SupportedThemeModes.Contains(themeMode))
            return Validation("Theme mode must be 'light', 'dark' or 'system'.");

        var density = command.Density?.Trim().ToLowerInvariant();
        if (density is null || !User.SupportedDensities.Contains(density))
            return Validation("Density must be 'comfortable' or 'compact'.");

        if (command.TextScale is not { } textScale || textScale < User.MinTextScale || textScale > User.MaxTextScale)
            return Validation("Text scale must be between 90 and 130.");

        var locale = string.IsNullOrWhiteSpace(command.PreferredLocale) ? null : command.PreferredLocale.Trim().ToLowerInvariant();
        if (locale is not null && !User.SupportedLocales.Contains(locale))
            return Validation("Preferred locale must be 'th' or 'en'.");

        return await _store.UpdateAsync(
            access.Value!.OrganizationId,
            new AdminActor(access.Value.ActorUserId, access.Value.MembershipId),
            new UpdateMyPreferencesInput(themeMode, density, textScale, locale),
            command.TraceId,
            cancellationToken);
    }

    private static Result<UserContext> Validation(string message) =>
        Result<UserContext>.Failure(new Error("REQUEST_VALIDATION_FAILED", message));
}
