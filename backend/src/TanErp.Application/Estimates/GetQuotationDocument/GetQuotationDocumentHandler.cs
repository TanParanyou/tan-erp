using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Estimates.GetQuotationDocument;

public class GetQuotationDocumentHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public GetQuotationDocumentHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<QuotationDocumentProjection>> HandleAsync(
        GetQuotationDocumentQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "quotations.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<QuotationDocumentProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var normalizedLocale = NormalizeLocale(query.Locale);

        var docResult = await _store.GetQuotationDocumentAsync(
            access.OrganizationId,
            query.EstimateId,
            normalizedLocale,
            cancellationToken);

        if (docResult.IsFailure)
        {
            return docResult;
        }

        var doc = docResult.Value!;
        if (!access.HasBranchAccess(doc.BranchId))
        {
            return Result<QuotationDocumentProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Quotation for estimate '{query.EstimateId}' was not found."));
        }

        return docResult;
    }

    private static string NormalizeLocale(string? locale)
    {
        if (string.IsNullOrWhiteSpace(locale))
            return "th";

        var trimmed = locale.Trim().ToLowerInvariant();
        return trimmed == "en" ? "en" : "th";
    }
}
