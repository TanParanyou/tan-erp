using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Files;
using TanErp.Domain.Files;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Files;

/// <summary>
/// Persists and retrieves UploadedFile metadata via EF Core.
/// Binary storage is handled separately by IFileStorageProvider.
/// </summary>
public class FileStore : IFileStore
{
    private readonly AppDbContext _db;

    public FileStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<UploadedFile>> GetByIdAsync(
        Guid fileId,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var file = await _db.UploadedFiles
            .FirstOrDefaultAsync(
                f => f.Id == fileId && f.OrganizationId == organizationId,
                cancellationToken);

        if (file is null)
        {
            return Result<UploadedFile>.Failure(
                new Error("FILE_NOT_FOUND", $"File '{fileId}' not found."));
        }

        return Result<UploadedFile>.Success(file);
    }

    public async Task SaveAsync(UploadedFile file, CancellationToken cancellationToken = default)
    {
        _db.UploadedFiles.Add(file);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
