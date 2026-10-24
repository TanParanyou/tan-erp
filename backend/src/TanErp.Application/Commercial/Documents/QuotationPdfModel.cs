namespace TanErp.Application.Commercial.Documents;

// The renderer sees ONLY these types. They are the allowlist: no cost, margin, markup, supplier or internal id exists here.
// QuotationPdfModelAllowlistTests reflects over this namespace and fails when a property is added without review.

public sealed record QuotationPdfAddress(string AddressLine1, string? Subdistrict, string? District, string? Province, string? PostalCode);

public sealed record QuotationPdfIssuer(
    string NameTh, string? NameEn, string? BranchNameTh, string? BranchNameEn,
    string? TaxIdentifier, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

public sealed record QuotationPdfCustomer(
    string DisplayNameTh, string? DisplayNameEn, string? LegalName, string? TaxIdentifier, string? BranchCode, QuotationPdfAddress? Address);

public sealed record QuotationPdfWorkItem(
    string Code, string DescriptionTh, string? DescriptionEn, decimal Quantity, string UnitCode, decimal UnitPrice, decimal LineTotal);

public sealed record QuotationPdfSection(
    string Code, string NameTh, string? NameEn, decimal Subtotal, IReadOnlyList<QuotationPdfWorkItem> WorkItems);

public sealed record QuotationPdfTotals(decimal Subtotal, decimal DiscountAmount, decimal NetBeforeTax, decimal TaxAmount, decimal GrandTotal);

public sealed record QuotationPdfModel(
    string Number,
    DateTimeOffset IssuedAtUtc,
    string Currency,
    string? SupersedesNumber,
    QuotationPdfIssuer Issuer,
    QuotationPdfCustomer Customer,
    IReadOnlyList<QuotationPdfSection> Sections,
    QuotationPdfTotals Totals);

/// <summary>Turns the allowlisted model into PDF bytes. Pure and synchronous: no I/O, no clock, no database.</summary>
public interface IQuotationDocumentRenderer
{
    /// <summary>Recorded with the stored artifact, e.g. "questpdf-2026.9.1/quotation-v1".</summary>
    string Version { get; }

    byte[] Render(QuotationPdfModel model);

    byte[] Render(QuotationPdfModel model, TanErp.Domain.DocumentTemplates.DocumentTemplateLayoutConfig? layoutConfig) =>
        Render(model);
}
