using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Common.Abstractions;

public interface IRequestAccessResolver
{
    Task<Result<RequestAccessContext>> ResolveAsync(
        string firebaseUid,
        Guid membershipId,
        string permissionKey,
        CancellationToken cancellationToken = default);
}
