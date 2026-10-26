using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Application.IdentityAccess.CurrentUser.UpdateMyProfile;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public class MyProfileStore : IMyProfileStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public MyProfileStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<UserContext>> UpdateAsync(
        Guid organizationId, AdminActor actor, UpdateMyProfileInput input, string traceId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == actor.UserId, cancellationToken);
        if (user is null)
            return Result<UserContext>.Failure(new Error("RESOURCE_NOT_FOUND", "User was not found."));

        if (input.AvatarFileId.HasValue)
        {
            var fileExists = await _db.UploadedFiles.AsNoTracking()
                .AnyAsync(f => f.Id == input.AvatarFileId.Value && f.OrganizationId == organizationId, cancellationToken);
            if (!fileExists)
                return Result<UserContext>.Failure(new Error("FILE_NOT_FOUND", "Avatar file not found."));
        }

        var before = user.RowVersion;
        user.UpdateOwnProfile(input.DisplayName, input.PhoneNumber, input.PreferredLocale, input.AvatarFileId);

        IdentityAdministrationSupport.AddAudit(
            _db, organizationId, actor, "users.own_profile_updated", "User", user.Id.ToString(), traceId, _clock.UtcNow,
            new
            {
                displayName = input.DisplayName,
                phoneNumber = input.PhoneNumber,
                preferredLocale = input.PreferredLocale,
                avatarFileId = input.AvatarFileId
            },
            before, user.RowVersion);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<UserContext>.Success(new UserContext(
            user.Id, user.DisplayName, user.Email, user.PhoneNumber, user.AvatarFileId, user.PreferredLocale));
    }
}
