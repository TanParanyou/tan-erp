namespace TanErp.Application.Estimates.GetQuotationDocument;

public sealed record GetQuotationDocumentQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    string? Locale);
