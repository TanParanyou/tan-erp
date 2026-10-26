using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Application.IdentityAccess.CurrentUser.UpdateMyPreferences;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public class MyPreferencesStore : IMyPreferencesStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public MyPreferencesStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<UserContext>> UpdateAsync(
        Guid organizationId, AdminActor actor, UpdateMyPreferencesInput input, string traceId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == actor.UserId, cancellationToken);
        if (user is null)
            return Result<UserContext>.Failure(new Error("RESOURCE_NOT_FOUND", "User was not found."));

        var before = user.RowVersion;
        user.UpdateThemePreferences(input.ThemeMode, input.Density, input.TextScale, input.PreferredLocale);

        IdentityAdministrationSupport.AddAudit(
            _db, organizationId, actor, "users.own_preferences_updated", "User", user.Id.ToString(), traceId, _clock.UtcNow,
            new { themeMode = input.ThemeMode, density = input.Density, textScale = input.TextScale, preferredLocale = user.PreferredLocale },
            before, user.RowVersion);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<UserContext>.Success(new UserContext(
            user.Id, user.DisplayName, user.Email, user.PhoneNumber, user.AvatarFileId, user.PreferredLocale,
            new UserPreferencesContext(user.ThemeMode, user.Density, user.TextScale)));
    }
}
