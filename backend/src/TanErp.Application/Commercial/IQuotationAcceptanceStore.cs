using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Commercial;

public interface IQuotationAcceptanceStore
{
    Task<Result<AcceptanceLinkProjection>> CreateLinkAsync(RequestAccessContext access, Guid quotationId, string tokenHash, string? signerHint, int lifetimeDays, string traceId, CancellationToken ct = default);
    Task<Result<AcceptanceLinkProjection>> RevokeLinkAsync(RequestAccessContext access, Guid linkId, string traceId, CancellationToken ct = default);
    Task<IReadOnlyList<AcceptanceLinkProjection>?> ListLinksAsync(Guid organizationId, Guid quotationId, CancellationToken ct = default);

    /// <summary>Public lookup by token hash. No organization filter: the token is the credential.</summary>
    Task<AcceptanceContext?> FindContextAsync(string tokenHash, CancellationToken ct = default);

    /// <summary>Marks the link accepted and stores the evidence once; a repeat returns the stored evidence.</summary>
    Task<Result<AcceptanceEvidenceSummary>> RecordAcceptanceAsync(AcceptanceContext context, AcceptanceSubmission submission, string signatureHash, string clientAddressHash, string? userAgent, string traceId, CancellationToken ct = default);
}
