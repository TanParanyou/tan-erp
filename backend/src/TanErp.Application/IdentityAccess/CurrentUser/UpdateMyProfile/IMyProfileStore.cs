using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

namespace TanErp.Application.IdentityAccess.CurrentUser.UpdateMyProfile;

public interface IMyProfileStore
{
    /// <summary>Updates only the caller's own user row; writes an audit event inside the caller's organization.</summary>
    Task<Result<UserContext>> UpdateAsync(
        Guid organizationId, AdminActor actor, UpdateMyProfileInput input, string traceId, CancellationToken cancellationToken = default);
}
