using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Files;
using TanErp.Application.Files.CreateUploadSession;
using TanErp.Application.Files.GetFileContent;
using TanErp.Domain.Files;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Files;

/// <summary>
/// Persists and retrieves UploadedFile and FileUploadSession metadata via EF Core,
/// and validates tenant + parent access when serving file content.
/// </summary>
public class FileStore : IFileStore
{
    private readonly AppDbContext _db;
    private readonly IFileStorageProvider _storageProvider;
    private readonly IFileParentAccessResolver _parentAccessResolver;

    public FileStore(
        AppDbContext db,
        IFileStorageProvider storageProvider,
        IFileParentAccessResolver parentAccessResolver)
    {
        _db = db;
        _storageProvider = storageProvider;
        _parentAccessResolver = parentAccessResolver;
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
                new Error("RESOURCE_NOT_FOUND", $"File '{fileId}' not found."));
        }

        return Result<UploadedFile>.Success(file);
    }

    public async Task SaveAsync(UploadedFile file, CancellationToken cancellationToken = default)
    {
        _db.UploadedFiles.Add(file);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<FileUploadSession>> CreateSessionAsync(
        RequestAccessContext access,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        IReadOnlyList<FileSlotInput> files,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var existing = await _db.FileUploadSessions
            .Include(s => s.Slots)
            .FirstOrDefaultAsync(
                s => s.OrganizationId == access.OrganizationId
                    && s.CreatedByUserId == access.ActorUserId
                    && s.IdempotencyKeyHash == keyHash,
                cancellationToken);

        if (existing != null)
        {
            if (existing.RequestPayloadHash != payloadHash)
            {
                return Result<FileUploadSession>.Failure(
                    new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different request payload."));
            }

            return Result<FileUploadSession>.Success(existing);
        }

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(15);
        var sessionId = Guid.NewGuid();

        var session = new FileUploadSession(
            sessionId,
            access.OrganizationId,
            parentType,
            parentId,
            creationIntentId,
            access.ActorUserId,
            now,
            expiresAt,
            keyHash,
            payloadHash);

        for (var i = 0; i < files.Count; i++)
        {
            var f = files[i];
            var slot = new FileUploadSlot(
                Guid.NewGuid(),
                sessionId,
                i,
                f.Filename,
                f.MediaType,
                f.FileSizeBytes);
            session.AddSlot(slot);
        }

        _db.FileUploadSessions.Add(session);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<FileUploadSession>.Success(session);
    }

    public async Task<Result<FileUploadSession>> GetCompletableSessionAsync(
        Guid organizationId,
        Guid actorUserId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _db.FileUploadSessions
            .Include(s => s.Slots)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session is null)
        {
            return Result<FileUploadSession>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", "Upload session was not found."));
        }

        if (session.OrganizationId != organizationId)
        {
            return Result<FileUploadSession>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Upload session does not belong to the requested organization."));
        }

        if (session.CreatedByUserId != actorUserId)
        {
            return Result<FileUploadSession>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", "Upload session was created by a different user."));
        }

        if (session.Status != FileUploadSessionStatus.Pending)
        {
            return Result<FileUploadSession>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", $"Upload session is already {session.Status}."));
        }

        if (session.IsExpired(DateTimeOffset.UtcNow))
        {
            return Result<FileUploadSession>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", "Upload session has expired."));
        }

        return Result<FileUploadSession>.Success(session);
    }

    public async Task<Result<IReadOnlyList<UploadedFile>>> CompleteSessionAsync(
        Guid organizationId,
        Guid actorUserId,
        Guid sessionId,
        IReadOnlyList<UploadedFile> files,
        CancellationToken cancellationToken = default)
    {
        var session = await _db.FileUploadSessions
            .Include(s => s.Slots)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session is null || session.OrganizationId != organizationId || session.CreatedByUserId != actorUserId || session.Status != FileUploadSessionStatus.Pending)
        {
            return Result<IReadOnlyList<UploadedFile>>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", "Upload session cannot be completed."));
        }

        session.MarkConsumed();

        // Link files to slots
        var slotList = session.Slots.OrderBy(s => s.SlotIndex).ToList();
        for (var i = 0; i < Math.Min(slotList.Count, files.Count); i++)
        {
            slotList[i].LinkUploadedFile(files[i].Id);
        }

        _db.UploadedFiles.AddRange(files);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<UploadedFile>>.Success(files);
    }

    public async Task<Result<FileContentResult>> GetAuthorizedFileContentAsync(
        RequestAccessContext access,
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var file = await _db.UploadedFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                f => f.Id == fileId && f.OrganizationId == access.OrganizationId,
                cancellationToken);

        if (file is null)
        {
            return Result<FileContentResult>.Failure(
                new Error("RESOURCE_NOT_FOUND", "File was not found."));
        }

        var parentAuthorized = false;

        if (Guid.TryParse(file.UploadSessionId, out var sessionId))
        {
            var session = await _db.FileUploadSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    s => s.Id == sessionId && s.OrganizationId == access.OrganizationId,
                    cancellationToken);

            if (session != null)
            {
                if (!session.ParentId.HasValue && session.CreatedByUserId != access.ActorUserId)
                {
                    return Result<FileContentResult>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "File was not found."));
                }

                var parentAccessResult = await _parentAccessResolver.ResolveAsync(
                    access,
                    session.ParentType,
                    session.ParentId,
                    session.CreationIntentId,
                    FileAccessOperation.Read,
                    cancellationToken);

                if (parentAccessResult.IsSuccess)
                {
                    parentAuthorized = true;
                }
            }
        }

        if (!parentAuthorized)
        {
            var oppWorkImage = await _db.OpportunityWorkImages
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.FileId == file.Id, cancellationToken);

            if (oppWorkImage != null)
            {
                var parentAccessResult = await _parentAccessResolver.ResolveAsync(
                    access,
                    FileParentTypes.Opportunity,
                    oppWorkImage.OpportunityId,
                    null,
                    FileAccessOperation.Read,
                    cancellationToken);

                if (parentAccessResult.IsSuccess)
                {
                    parentAuthorized = true;
                }
            }
        }

        if (!parentAuthorized)
        {
            var siteImage = await _db.SiteImages
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.FileId == file.Id, cancellationToken);

            if (siteImage != null)
            {
                var parentAccessResult = await _parentAccessResolver.ResolveAsync(
                    access,
                    FileParentTypes.Site,
                    siteImage.SiteId,
                    null,
                    FileAccessOperation.Read,
                    cancellationToken);

                if (parentAccessResult.IsSuccess)
                {
                    parentAuthorized = true;
                }
            }
        }

        if (!parentAuthorized)
        {
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrganizationId == access.OrganizationId && c.ImageFileId == file.Id, cancellationToken);

            if (customer != null)
            {
                var parentAccessResult = await _parentAccessResolver.ResolveAsync(
                    access,
                    FileParentTypes.Customer,
                    customer.Id,
                    null,
                    FileAccessOperation.Read,
                    cancellationToken);

                if (parentAccessResult.IsSuccess)
                {
                    parentAuthorized = true;
                }
            }
        }

        if (!parentAuthorized)
        {
            return Result<FileContentResult>.Failure(
                new Error("RESOURCE_NOT_FOUND", "File was not found."));
        }

        var stream = await _storageProvider.OpenReadStreamAsync(file.StoragePath, cancellationToken);
        if (stream is null)
        {
            return Result<FileContentResult>.Failure(
                new Error("RESOURCE_NOT_FOUND", "File content was not found."));
        }

        var mediaType = string.IsNullOrWhiteSpace(file.MediaType) ? "application/octet-stream" : file.MediaType;
        return Result<FileContentResult>.Success(
            new FileContentResult(stream, mediaType, file.OriginalFilename));
    }

    public async Task<Result<bool>> ValidateVerifiedFilesForParentAsync(
        Guid organizationId,
        Guid actorUserId,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        IReadOnlyCollection<Guid> fileIds,
        CancellationToken cancellationToken = default)
    {
        if (fileIds == null || fileIds.Count == 0)
        {
            return Result<bool>.Success(true);
        }

        var distinctIds = fileIds.Distinct().ToList();

        var files = await _db.UploadedFiles
            .AsNoTracking()
            .Where(f => distinctIds.Contains(f.Id) && f.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

        if (files.Count != distinctIds.Count)
        {
            return Result<bool>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", "One or more files do not exist or belong to a different organization."));
        }

        foreach (var file in files)
        {
            if (file.Status != UploadedFileStatus.Verified)
            {
                return Result<bool>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID", $"File '{file.Id}' is not verified."));
            }

            if (!Guid.TryParse(file.UploadSessionId, out var sessionId))
            {
                return Result<bool>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID", $"File '{file.Id}' does not have a valid upload session link."));
            }

            var session = await _db.FileUploadSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.OrganizationId == organizationId, cancellationToken);

            if (session == null)
            {
                return Result<bool>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID", $"Upload session for file '{file.Id}' was not found."));
            }

            if (!string.Equals(session.ParentType, parentType, StringComparison.OrdinalIgnoreCase))
            {
                return Result<bool>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"File '{file.Id}' was uploaded for parent type '{session.ParentType}', cannot bind to '{parentType}'."));
            }

            if (parentId.HasValue)
            {
                if (!session.ParentId.HasValue || session.ParentId.Value != parentId.Value)
                {
                    return Result<bool>.Failure(
                        new Error("FILE_UPLOAD_SESSION_INVALID",
                            $"File '{file.Id}' was uploaded for a different parent ID."));
                }
            }
            else if (creationIntentId.HasValue)
            {
                if (!session.CreationIntentId.HasValue || session.CreationIntentId.Value != creationIntentId.Value)
                {
                    return Result<bool>.Failure(
                        new Error("FILE_UPLOAD_SESSION_INVALID",
                            $"File '{file.Id}' was uploaded with a different creation intent ID."));
                }

                if (session.CreatedByUserId != actorUserId)
                {
                    return Result<bool>.Failure(
                        new Error("FILE_UPLOAD_SESSION_INVALID",
                            $"File '{file.Id}' was uploaded by a different user."));
                }
            }
        }

        return Result<bool>.Success(true);
    }

    public async Task BindFilesToParentAsync(
        Guid organizationId,
        Guid? creationIntentId,
        Guid actualParentId,
        CancellationToken cancellationToken = default)
    {
        if (!creationIntentId.HasValue) return;

        var sessions = await _db.FileUploadSessions
            .Where(s => s.OrganizationId == organizationId && s.CreationIntentId == creationIntentId.Value)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.BindActualParentId(actualParentId);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
