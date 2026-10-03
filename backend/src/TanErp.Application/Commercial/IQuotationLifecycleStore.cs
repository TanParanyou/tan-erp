using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Commercial;

public interface IQuotationLifecycleStore
{
    Task<Result<QuotationHistoryProjection>> VoidAsync(RequestAccessContext access, Guid quotationId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default);
    Task<Result<QuotationHistoryProjection>> AmendAsync(RequestAccessContext access, Guid quotationId, Guid expectedVersion, string reason, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<QuotationHistoryProjection?> GetHistoryAsync(Guid organizationId, Guid estimateId, CancellationToken ct = default);
}
