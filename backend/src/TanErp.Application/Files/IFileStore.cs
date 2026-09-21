using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Files.CreateUploadSession;
using TanErp.Application.Files.GetFileContent;
using TanErp.Domain.Files;

namespace TanErp.Application.Files;

/// <summary>
/// Persistence contract for file metadata, upload sessions, and parent-bound verification.
/// </summary>
public interface IFileStore
{
    Task<Result<UploadedFile>> GetByIdAsync(
        Guid fileId,
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        UploadedFile file,
        CancellationToken cancellationToken = default);

    Task<Result<FileUploadSession>> CreateSessionAsync(
        RequestAccessContext access,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        IReadOnlyList<FileSlotInput> files,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<FileUploadSession>> GetCompletableSessionAsync(
        Guid organizationId,
        Guid actorUserId,
        Guid sessionId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<UploadedFile>>> CompleteSessionAsync(
        Guid organizationId,
        Guid actorUserId,
        Guid sessionId,
        IReadOnlyList<UploadedFile> files,
        CancellationToken cancellationToken = default);

    Task<Result<FileContentResult>> GetAuthorizedFileContentAsync(
        RequestAccessContext access,
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> ValidateVerifiedFilesForParentAsync(
        Guid organizationId,
        Guid actorUserId,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        IReadOnlyCollection<Guid> fileIds,
        CancellationToken cancellationToken = default);

    Task BindFilesToParentAsync(
        Guid organizationId,
        Guid? creationIntentId,
        Guid actualParentId,
        CancellationToken cancellationToken = default);
}
