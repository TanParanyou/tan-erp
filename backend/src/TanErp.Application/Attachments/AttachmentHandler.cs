using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Attachments;

namespace TanErp.Application.Attachments;

/// <summary>
/// Shared attachment and signature use cases. Order of checks: owner type (422) → permission (403) → owner visible in the caller's
/// organization/branch (404) → input (422) → owner state (409) → store.
/// </summary>
public class AttachmentHandler
{
    public const int MaxFilesPerRequest = 20;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IAttachmentOwnerScopeReader _scopes;
    private readonly IAttachmentStore _store;

    public AttachmentHandler(IRequestAccessResolver accessResolver, IAttachmentOwnerScopeReader scopes, IAttachmentStore store)
    {
        _accessResolver = accessResolver;
        _scopes = scopes;
        _store = store;
    }

    private enum OwnerOperation { Read, Manage, Sign }

    private sealed record ResolvedOwner(RequestAccessContext Access, AttachmentOwnerDescriptor Descriptor, AttachmentOwnerScope Scope);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private async Task<Result<ResolvedOwner>> ResolveOwnerAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, OwnerOperation operation, CancellationToken ct)
    {
        var descriptor = AttachmentOwnerRegistry.Find(AttachmentOwnerTypes.Normalize(ownerType));
        if (descriptor is null) return Fail<ResolvedOwner>("ATTACHMENT_OWNER_TYPE_INVALID", $"Owner type '{ownerType}' is not registered.");

        var permission = operation switch
        {
            OwnerOperation.Read => descriptor.ReadPermission,
            OwnerOperation.Manage => descriptor.ManagePermission,
            _ => descriptor.SignPermission
        };

        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);
        if (access.IsFailure) return Result<ResolvedOwner>.Failure(access.Error);

        var context = access.Value!;
        var scope = await _scopes.FindAsync(descriptor.OwnerType, ownerId, context.OrganizationId, ct);
        if (scope is null
            || scope.OrganizationId != context.OrganizationId
            || (scope.BranchId is { } branchId && !context.HasBranchAccess(branchId)))
        {
            return Fail<ResolvedOwner>("RESOURCE_NOT_FOUND", "Owner not found.");
        }

        return Result<ResolvedOwner>.Success(new ResolvedOwner(context, descriptor, scope));
    }

    public async Task<Result<IReadOnlyList<AttachmentLinkProjection>>> ListAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Read, ct);
        if (owner.IsFailure) return Result<IReadOnlyList<AttachmentLinkProjection>>.Failure(owner.Error);
        var items = await _store.ListLinksAsync(owner.Value!.Access.OrganizationId, owner.Value.Descriptor.OwnerType, ownerId, ct);
        return Result<IReadOnlyList<AttachmentLinkProjection>>.Success(items);
    }

    public async Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(
        AttachmentCaller caller, string? ownerType, Guid ownerId, string idempotencyKey, AttachFilesInput? input, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Manage, ct);
        if (owner.IsFailure) return Result<IReadOnlyList<AttachmentLinkProjection>>.Failure(owner.Error);

        var fileIds = input?.FileIds;
        if (input is null || fileIds is null || fileIds.Count == 0 || fileIds.Count > MaxFilesPerRequest || fileIds.Any(id => id == Guid.Empty))
        {
            return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_FIELD_INVALID", $"Between 1 and {MaxFilesPerRequest} files are required.");
        }

        var purpose = AttachmentPurposes.Normalize(input.Purpose);
        if (purpose is null) return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_PURPOSE_INVALID", $"Purpose '{input.Purpose}' is not supported.");
        if (fileIds.Distinct().Count() != fileIds.Count) return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_DUPLICATE", "The same file was listed more than once.");

        var resolved = owner.Value!;
        if (!resolved.Descriptor.ManageStates.Contains(resolved.Scope.State))
        {
            return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_OWNER_LOCKED", "The record's status does not allow attachment changes.");
        }

        // Fast pre-check against the active links of exactly this owner; the store re-checks authoritatively inside its transaction.
        var existing = await _store.ListLinksAsync(resolved.Access.OrganizationId, resolved.Descriptor.OwnerType, ownerId, ct);
        var newLinks = fileIds.Count(id => !existing.Any(l => l.FileId == id && l.Purpose == purpose));
        if (existing.Count + newLinks > AttachmentLink.MaxActiveLinksPerOwner)
        {
            return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_LIMIT_EXCEEDED", $"A record can have at most {AttachmentLink.MaxActiveLinksPerOwner} attachments.");
        }

        var normalized = new AttachFilesInput(purpose, fileIds.ToList());
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(FormattableString.Invariant($"{resolved.Descriptor.OwnerType}|{ownerId:D}|{purpose}|{string.Join(",", fileIds.Select(id => id.ToString("D")))}"));
        return await _store.AttachAsync(resolved.Access, resolved.Descriptor.OwnerType, ownerId, normalized, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<bool>> UnlinkAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, Guid linkId, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Manage, ct);
        if (owner.IsFailure) return Result<bool>.Failure(owner.Error);

        var resolved = owner.Value!;
        if (!resolved.Descriptor.ManageStates.Contains(resolved.Scope.State))
        {
            return Fail<bool>("ATTACHMENT_OWNER_LOCKED", "The record's status does not allow attachment changes.");
        }

        return await _store.UnlinkAsync(resolved.Access, resolved.Descriptor.OwnerType, ownerId, linkId, caller.TraceId, ct);
    }

    public async Task<Result<IReadOnlyList<SignatureCaptureProjection>>> ListSignaturesAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Read, ct);
        if (owner.IsFailure) return Result<IReadOnlyList<SignatureCaptureProjection>>.Failure(owner.Error);
        var items = await _store.ListSignaturesAsync(owner.Value!.Access.OrganizationId, owner.Value.Descriptor.OwnerType, ownerId, ct);
        return Result<IReadOnlyList<SignatureCaptureProjection>>.Success(items);
    }

    public async Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(
        AttachmentCaller caller, string? ownerType, Guid ownerId, string idempotencyKey, SignatureCaptureInput? input, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Sign, ct);
        if (owner.IsFailure) return Result<SignatureCaptureProjection>.Failure(owner.Error);

        if (input is null || input.ImageFileId == Guid.Empty
            || !SignatureEvidenceRules.TryNormalizeSigner(input.SignerName, input.SignerRole, out var signerName, out var signerRole))
        {
            return Fail<SignatureCaptureProjection>("SIGNATURE_SUBMISSION_INVALID", "A signer name of 2-200 characters and a signature image are required.");
        }

        var purpose = AttachmentPurposes.Normalize(input.Purpose);
        var consentVersion = purpose is null ? null : SignatureConsentVersions.Current(purpose);
        if (purpose is null || consentVersion is null)
        {
            return Fail<SignatureCaptureProjection>("ATTACHMENT_PURPOSE_INVALID", "Signatures are supported only for the handover purpose.");
        }

        if (!input.ConsentAccepted || input.ConsentTextVersion != consentVersion)
        {
            return Fail<SignatureCaptureProjection>("SIGNATURE_CONSENT_REQUIRED", "The current consent statement must be accepted.");
        }

        var resolved = owner.Value!;
        if (!resolved.Descriptor.SignStates.Contains(resolved.Scope.State))
        {
            return Fail<SignatureCaptureProjection>("ATTACHMENT_OWNER_LOCKED", "The record's status does not allow signatures.");
        }

        var command = new SignatureCaptureCommand(purpose, signerName, signerRole, input.ImageFileId, consentVersion);
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(FormattableString.Invariant(
            $"{resolved.Descriptor.OwnerType}|{ownerId:D}|{purpose}|{signerName}|{signerRole}|{input.ImageFileId:D}|{consentVersion}"));
        return await _store.CaptureSignatureAsync(resolved.Access, resolved.Descriptor.OwnerType, ownerId, command, keyHash, payloadHash, caller.TraceId, ct);
    }
}
