using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Attachments;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Files;
using TanErp.Domain.Attachments;
using TanErp.Domain.Common;

namespace TanErp.Infrastructure.Persistence.Attachments;

public class AttachmentStore : IAttachmentStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IFileStore _files;

    public AttachmentStore(AppDbContext db, IClock clock, IFileStore files)
    {
        _db = db;
        _clock = clock;
        _files = files;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static string ServingUrl(Guid fileId) => $"/api/v1/files/{fileId}/content";

    private void Audit(RequestAccessContext access, string action, Guid ownerId, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "AttachmentOwner", ownerId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            rowVersionAfter: null));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    /// <summary>
    /// Serializes attaches to the same owner for the rest of the transaction so the 50-link limit and duplicate checks are authoritative.
    /// Same pg_advisory_xact_lock technique the repo already uses for idempotency keys (FindReplayAsync); it needs no owner-level row,
    /// works for every owner type and is released automatically on commit/rollback. Always taken after the idempotency lock (fixed order).
    /// </summary>
    private async Task LockOwnerAsync(Guid orgId, string ownerType, Guid ownerId, CancellationToken ct)
    {
        var lockKey = $"attachments:{orgId:N}:{ownerType}:{ownerId:N}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
    }

    public const string ActiveLinkIndex = "ux_attachment_links_active_owner_file_purpose";
    public const string SignatureImageIndex = "ux_signature_captures_image_file";

    /// <summary>
    /// Matches one specific unique index. Any other unique violation (for example the idempotency record) is not a duplicate attachment
    /// and propagates; same-key races cannot reach it because FindReplayAsync holds an advisory lock on the key.
    /// </summary>
    internal static bool IsUniqueViolation(DbUpdateException ex, string constraintName) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg && pg.ConstraintName == constraintName;

    // ===== attachments =============================================================================

    public async Task<IReadOnlyList<AttachmentLinkProjection>> ListLinksAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var rows = await (
            from l in _db.AttachmentLinks.AsNoTracking()
            where l.OrganizationId == organizationId && l.OwnerType == ownerType && l.OwnerId == ownerId && l.RemovedAtUtc == null
            join f in _db.UploadedFiles.AsNoTracking() on new { Id = l.FileId, l.OrganizationId } equals new { f.Id, f.OrganizationId }
            join u in _db.Users.AsNoTracking() on l.CreatedByUserId equals u.Id
            orderby l.CreatedAtUtc, l.Id
            select new { Link = l, f.OriginalFilename, f.MediaType, f.FileSizeBytes, UserId = u.Id, u.DisplayName }).ToListAsync(ct);

        return rows.Select(r => new AttachmentLinkProjection(
            r.Link.Id, r.Link.OwnerType, r.Link.OwnerId, r.Link.FileId, r.Link.Purpose, r.OriginalFilename, r.MediaType, r.FileSizeBytes,
            ServingUrl(r.Link.FileId), new AttachmentPerson(r.UserId, r.DisplayName), r.Link.CreatedAtUtc)).ToList();
    }

    public async Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, AttachFilesInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var operation = $"attachments.attach.{ownerType}";
        var orgId = access.OrganizationId;
        var fileIds = input.FileIds!;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<IReadOnlyList<AttachmentLinkProjection>>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                return Result<IReadOnlyList<AttachmentLinkProjection>>.Success(await ListLinksAsync(orgId, ownerType, ownerId, ct));
            }

            // Verified, same organization, and uploaded through a session bound to this very owner.
            var validation = await _files.ValidateVerifiedFilesForParentAsync(orgId, access.ActorUserId, ownerType, ownerId, null, fileIds, ct);
            if (validation.IsFailure)
            {
                return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_FILE_NOT_READY", "A file is not verified or was not uploaded for this record.");
            }

            // Authoritative limit/duplicate check: serialize per owner, then read the owner's active links.
            await LockOwnerAsync(orgId, ownerType, ownerId, ct);

            var files = await _db.UploadedFiles.AsNoTracking().Where(f => fileIds.Contains(f.Id) && f.OrganizationId == orgId).ToListAsync(ct);
            var active = await _db.AttachmentLinks
                .Where(l => l.OrganizationId == orgId && l.OwnerType == ownerType && l.OwnerId == ownerId && l.RemovedAtUtc == null)
                .ToListAsync(ct);

            var now = _clock.UtcNow;
            try
            {
                foreach (var fileId in fileIds)
                {
                    var file = files.Single(f => f.Id == fileId);
                    var link = new AttachmentLink(Guid.NewGuid(), orgId, ownerType, ownerId, file.OrganizationId, file.Id, input.Purpose!, access.ActorUserId, now);
                    AttachmentLink.AssertCanAdd(active, link);
                    active.Add(link);
                    _db.AttachmentLinks.Add(link);
                }
            }
            catch (AttachmentDomainException ex)
            {
                // All-or-nothing: drop anything already staged in this request so no partial link can be saved.
                _db.ChangeTracker.Clear();
                return Fail<IReadOnlyList<AttachmentLinkProjection>>(ex.Code, ex.Message);
            }

            Audit(access, "attachment.linked", ownerId, traceId, new { ownerType, purpose = input.Purpose, count = fileIds.Count, fileIds }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, ownerId.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex, ActiveLinkIndex))
            {
                await tx.RollbackAsync(ct);
                _db.ChangeTracker.Clear();
                return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_DUPLICATE", "This file is already attached to this record for the same purpose.");
            }

            return Result<IReadOnlyList<AttachmentLinkProjection>>.Success(await ListLinksAsync(orgId, ownerType, ownerId, ct));
        });
    }

    public async Task<Result<bool>> UnlinkAsync(RequestAccessContext access, string ownerType, Guid ownerId, Guid linkId, string traceId, CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            // Owner lock serializes concurrent unlinks (and attaches) of the same owner; the second unlink then sees the link as removed.
            await LockOwnerAsync(access.OrganizationId, ownerType, ownerId, ct);

            var link = await _db.AttachmentLinks.FirstOrDefaultAsync(l =>
                l.Id == linkId && l.OrganizationId == access.OrganizationId && l.OwnerType == ownerType && l.OwnerId == ownerId && l.RemovedAtUtc == null, ct);
            if (link is null) return Fail<bool>("RESOURCE_NOT_FOUND", "Attachment not found.");

            var now = _clock.UtcNow;
            link.Remove(access.ActorUserId, now);
            Audit(access, "attachment.unlinked", ownerId, traceId, new { ownerType, purpose = link.Purpose, fileId = link.FileId }, now);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<bool>.Success(true);
        });
    }

    // ===== signatures ==============================================================================

    public async Task<IReadOnlyList<SignatureCaptureProjection>> ListSignaturesAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var rows = await (
            from s in _db.SignatureCaptures.AsNoTracking()
            where s.OrganizationId == organizationId && s.OwnerType == ownerType && s.OwnerId == ownerId
            join u in _db.Users.AsNoTracking() on s.CapturedByUserId equals u.Id
            orderby s.SignedAtUtc, s.Id
            select new { Signature = s, UserId = u.Id, u.DisplayName }).ToListAsync(ct);

        return rows.Select(r => ToProjection(r.Signature, new AttachmentPerson(r.UserId, r.DisplayName))).ToList();
    }

    private static SignatureCaptureProjection ToProjection(SignatureCapture s, AttachmentPerson capturedBy) => new(
        s.Id, s.OwnerType, s.OwnerId, s.Purpose, s.SignerName, s.SignerRole, s.SignedAtUtc, s.ImageFileId, ServingUrl(s.ImageFileId),
        s.ConsentTextVersion, s.ContentHash, capturedBy);

    public async Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, SignatureCaptureCommand command, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var operation = $"attachments.signature.{ownerType}";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<SignatureCaptureProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id)
                    ? (await ListSignaturesAsync(orgId, ownerType, ownerId, ct)).FirstOrDefault(s => s.Id == id)
                    : null;
                return replayed is not null
                    ? Result<SignatureCaptureProjection>.Success(replayed)
                    : Fail<SignatureCaptureProjection>("RESOURCE_NOT_FOUND", "Signature not found.");
            }

            var validation = await _files.ValidateVerifiedFilesForParentAsync(orgId, access.ActorUserId, ownerType, ownerId, null, [command.ImageFileId], ct);
            if (validation.IsFailure) return Fail<SignatureCaptureProjection>("ATTACHMENT_FILE_NOT_READY", "The image is not verified or was not uploaded for this record.");

            var file = await _db.UploadedFiles.AsNoTracking().FirstAsync(f => f.Id == command.ImageFileId && f.OrganizationId == orgId, ct);
            if (file.MediaType != "image/png" || !SignatureEvidenceRules.IsSha256Hex(file.ContentSha256))
            {
                return Fail<SignatureCaptureProjection>("SIGNATURE_IMAGE_INVALID", "The signature image must be a verified PNG.");
            }

            var now = _clock.UtcNow;
            SignatureCapture capture;
            try
            {
                capture = new SignatureCapture(
                    Guid.NewGuid(), orgId, ownerType, ownerId, command.Purpose, command.SignerName, command.SignerRole, now,
                    file.OrganizationId, file.Id, command.ConsentTextVersion, file.ContentSha256!, access.ActorUserId);
            }
            catch (AttachmentDomainException ex)
            {
                return Fail<SignatureCaptureProjection>(ex.Code, ex.Message);
            }

            _db.SignatureCaptures.Add(capture);
            // No signer name or role in the audit trail (personal data stays in the capture row only).
            Audit(access, "signature.captured", ownerId, traceId,
                new { ownerType, purpose = capture.Purpose, consentTextVersion = capture.ConsentTextVersion, contentHash = capture.ContentHash, imageFileId = capture.ImageFileId }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, capture.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex, SignatureImageIndex))
            {
                await tx.RollbackAsync(ct);
                _db.ChangeTracker.Clear();
                return Fail<SignatureCaptureProjection>("ATTACHMENT_DUPLICATE", "This image is already used as a signature.");
            }

            var actor = await _db.Users.AsNoTracking().Where(u => u.Id == access.ActorUserId).Select(u => new AttachmentPerson(u.Id, u.DisplayName)).FirstAsync(ct);
            return Result<SignatureCaptureProjection>.Success(ToProjection(capture, actor));
        });
    }
}
