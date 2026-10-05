using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Attachments;

/// <summary>Finds an owner record in the caller's organization. Implemented per owner type in Infrastructure.</summary>
public interface IAttachmentOwnerScopeReader
{
    Task<AttachmentOwnerScope?> FindAsync(string ownerType, Guid ownerId, Guid organizationId, CancellationToken ct = default);
}

public interface IAttachmentStore
{
    /// <summary>
    /// Active (not removed) links of exactly one owner: organization + ownerType + ownerId. Implementations must never return links of
    /// another owner, because the 50-per-owner limit (AttachmentLink.AssertCanAdd) counts every link it is given.
    /// </summary>
    Task<IReadOnlyList<AttachmentLinkProjection>> ListLinksAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default);

    /// <summary>
    /// Authoritative attach inside one transaction. The store must load the active links of exactly this owner (organization +
    /// ownerType + ownerId) and pass only those to AttachmentLink.AssertCanAdd.
    /// </summary>
    Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, AttachFilesInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);

    Task<Result<bool>> UnlinkAsync(RequestAccessContext access, string ownerType, Guid ownerId, Guid linkId, string traceId, CancellationToken ct = default);

    Task<IReadOnlyList<SignatureCaptureProjection>> ListSignaturesAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default);

    Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, SignatureCaptureCommand command, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
}
