namespace TanErp.Application.Estimates.GetQuotationDocument;

public sealed record QuotationAddressProjection(
    string? Label,
    string AddressLine1,
    string? Subdistrict,
    string? District,
    string? Province,
    string? PostalCode,
    string? CountryCode);

public sealed record QuotationCustomerProjection(
    string CustomerType,
    string? DisplayName,
    string DisplayNameTh,
    string? DisplayNameEn,
    string? LegalName,
    string? TaxIdentifier,
    string? BranchCode,
    QuotationAddressProjection? Address);

public sealed record QuotationWorkItemProjection(
    string Code,
    string? Description,
    string DescriptionTh,
    string? DescriptionEn,
    decimal Quantity,
    string UnitCode,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record QuotationSectionProjection(
    string Code,
    string? Name,
    string NameTh,
    string? NameEn,
    decimal Subtotal,
    IReadOnlyList<QuotationWorkItemProjection> WorkItems);

public sealed record QuotationTotalsProjection(
    decimal Subtotal,
    string DiscountType,
    decimal DiscountValue,
    decimal DiscountAmount,
    decimal NetBeforeTax,
    decimal TaxAmount,
    decimal GrandTotal);

public sealed record QuotationDocumentProjection(
    Guid BranchId,
    string Number,
    DateTimeOffset IssuedAtUtc,
    string Currency,
    string Locale,
    bool HasIncompleteTranslations,
    QuotationCustomerProjection Customer,
    IReadOnlyList<QuotationSectionProjection> Sections,
    QuotationTotalsProjection Totals);
