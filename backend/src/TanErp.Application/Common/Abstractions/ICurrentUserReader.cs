using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

namespace TanErp.Application.Common.Abstractions;

public interface ICurrentUserReader
{
    Task<Result<GetCurrentUserResult>> GetAsync(string firebaseUid, CancellationToken cancellationToken = default);
}
