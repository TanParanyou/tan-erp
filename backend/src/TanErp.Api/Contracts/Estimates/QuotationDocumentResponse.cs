namespace TanErp.Api.Contracts.Estimates;

public sealed record QuotationAddressResponse(
    string? Label,
    string AddressLine1,
    string? Subdistrict,
    string? District,
    string? Province,
    string? PostalCode,
    string? CountryCode);

public sealed record QuotationCustomerResponse(
    string CustomerType,
    string? DisplayName,
    string DisplayNameTh,
    string? DisplayNameEn,
    string? LegalName,
    string? TaxIdentifier,
    string? BranchCode,
    QuotationAddressResponse? Address);

public sealed record QuotationWorkItemResponse(
    string Code,
    string? Description,
    string DescriptionTh,
    string? DescriptionEn,
    decimal Quantity,
    string UnitCode,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record QuotationSectionResponse(
    string Code,
    string? Name,
    string NameTh,
    string? NameEn,
    decimal Subtotal,
    IReadOnlyList<QuotationWorkItemResponse> WorkItems);

public sealed record QuotationTotalsResponse(
    decimal Subtotal,
    string DiscountType,
    decimal DiscountValue,
    decimal DiscountAmount,
    decimal NetBeforeTax,
    decimal TaxAmount,
    decimal GrandTotal);

public sealed record QuotationDocumentResponse(
    string Number,
    DateTimeOffset IssuedAtUtc,
    string Currency,
    string Locale,
    bool HasIncompleteTranslations,
    QuotationCustomerResponse Customer,
    IReadOnlyList<QuotationSectionResponse> Sections,
    QuotationTotalsResponse Totals)
{
    public static QuotationDocumentResponse FromProjection(Application.Estimates.GetQuotationDocument.QuotationDocumentProjection p) =>
        new(
            p.Number,
            p.IssuedAtUtc,
            p.Currency,
            p.Locale,
            p.HasIncompleteTranslations,
            new QuotationCustomerResponse(
                p.Customer.CustomerType,
                p.Customer.DisplayName,
                p.Customer.DisplayNameTh,
                p.Customer.DisplayNameEn,
                p.Customer.LegalName,
                p.Customer.TaxIdentifier,
                p.Customer.BranchCode,
                p.Customer.Address is null
                    ? null
                    : new QuotationAddressResponse(
                        p.Customer.Address.Label,
                        p.Customer.Address.AddressLine1,
                        p.Customer.Address.Subdistrict,
                        p.Customer.Address.District,
                        p.Customer.Address.Province,
                        p.Customer.Address.PostalCode,
                        p.Customer.Address.CountryCode)),
            p.Sections.Select(s => new QuotationSectionResponse(
                s.Code,
                s.Name,
                s.NameTh,
                s.NameEn,
                s.Subtotal,
                s.WorkItems.Select(w => new QuotationWorkItemResponse(
                    w.Code,
                    w.Description,
                    w.DescriptionTh,
                    w.DescriptionEn,
                    w.Quantity,
                    w.UnitCode,
                    w.UnitPrice,
                    w.LineTotal)).ToList())).ToList(),
            new QuotationTotalsResponse(
                p.Totals.Subtotal,
                p.Totals.DiscountType,
                p.Totals.DiscountValue,
                p.Totals.DiscountAmount,
                p.Totals.NetBeforeTax,
                p.Totals.TaxAmount,
                p.Totals.GrandTotal));
}
