using TanErp.Application.Common.Results;
using TanErp.Domain.Files;

namespace TanErp.Application.Files;

/// <summary>
/// Persistence contract for file metadata operations.
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
}
