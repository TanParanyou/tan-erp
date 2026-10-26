using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

namespace TanErp.Application.IdentityAccess.CurrentUser.UpdateMyPreferences;

public interface IMyPreferencesStore
{
    /// <summary>Updates only the caller's own theme preferences; writes an audit event inside the caller's organization.</summary>
    Task<Result<UserContext>> UpdateAsync(
        Guid organizationId, AdminActor actor, UpdateMyPreferencesInput input, string traceId, CancellationToken cancellationToken = default);
}
