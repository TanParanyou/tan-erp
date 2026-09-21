using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Files;

public enum FileAccessOperation
{
    Upload,
    Read,
    Bind
}

public sealed record FileParentAccess(
    string ParentType,
    Guid? ParentId,
    Guid? CreationIntentId,
    Guid OrganizationId);

public interface IFileParentAccessResolver
{
    Task<Result<FileParentAccess>> ResolveAsync(
        RequestAccessContext access,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        FileAccessOperation operation,
        CancellationToken cancellationToken = default);
}
