using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Commercial.Documents;

/// <summary>Download of the stored quotation PDF needs quotations.read.</summary>
public class QuotationDocumentHandler(IRequestAccessResolver accessResolver, IQuotationDocumentStore store)
{
    public async Task<Result<QuotationDocumentFile>> DownloadAsync(QuotationLifecycleCaller caller, Guid quotationId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.read", ct);
        return access.IsFailure ? Result<QuotationDocumentFile>.Failure(access.Error) : await store.OpenAsync(access.Value!, quotationId, ct);
    }
}
