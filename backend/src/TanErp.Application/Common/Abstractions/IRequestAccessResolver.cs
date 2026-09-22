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

    Task<Result<RequestAccessContext>> ResolveBranchAccessAsync(
        string firebaseUid,
        Guid membershipId,
        string permissionKey,
        Guid targetBranchId,
        CancellationToken cancellationToken = default)
        => ResolveAsync(firebaseUid, membershipId, permissionKey, cancellationToken);

    async Task<Result<RequestAccessContext>> ResolveAnyAsync(
        string firebaseUid,
        Guid membershipId,
        IReadOnlyCollection<string> permissionKeys,
        CancellationToken cancellationToken = default)
    {
        foreach (var key in permissionKeys)
        {
            var result = await ResolveAsync(firebaseUid, membershipId, key, cancellationToken);
            if (result.IsSuccess) return result;
        }
        return Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
    }
}
