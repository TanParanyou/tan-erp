using System.Reflection;
using TanErp.Application.Commercial.Documents;
using Xunit;

namespace TanErp.UnitTests.Commercial;

/// <summary>The renderer input is a typed allowlist: adding a property is a conscious change here too, and no name may suggest internal pricing data.</summary>
public class QuotationPdfModelAllowlistTests
{
    private static readonly IReadOnlyDictionary<string, string[]> Allowed = new Dictionary<string, string[]>
    {
        [nameof(QuotationPdfModel)] = ["Number", "IssuedAtUtc", "Currency", "SupersedesNumber", "Issuer", "Customer", "Sections", "Totals"],
        [nameof(QuotationPdfIssuer)] = ["NameTh", "NameEn", "BranchNameTh", "BranchNameEn", "TaxIdentifier", "TaxBranchCode", "AddressTh", "AddressEn", "Phone", "LogoBytes"],
        [nameof(QuotationPdfCustomer)] = ["DisplayNameTh", "DisplayNameEn", "LegalName", "TaxIdentifier", "BranchCode", "Address"],
        [nameof(QuotationPdfAddress)] = ["AddressLine1", "Subdistrict", "District", "Province", "PostalCode"],
        [nameof(QuotationPdfSection)] = ["Code", "NameTh", "NameEn", "Subtotal", "WorkItems"],
        [nameof(QuotationPdfWorkItem)] = ["Code", "DescriptionTh", "DescriptionEn", "Quantity", "UnitCode", "UnitPrice", "LineTotal"],
        [nameof(QuotationPdfTotals)] = ["Subtotal", "DiscountAmount", "NetBeforeTax", "TaxAmount", "GrandTotal"]
    };

    private static readonly string[] Forbidden =
        ["cost", "margin", "markup", "profit", "supplier", "vendor", "rule", "reason", "approval", "userid", "organizationid", "estimateid", "revisionid", "snapshot", "hash", "note"];

    [Fact]
    public void EveryModelType_ExposesExactlyTheReviewedProperties_AndNoInternalPricingNames()
    {
        var types = typeof(QuotationPdfModel).Assembly.GetTypes()
            .Where(type => type.Namespace == typeof(QuotationPdfModel).Namespace && type.Name.StartsWith("QuotationPdf", StringComparison.Ordinal) && type.IsClass).ToList();
        Assert.Equal(Allowed.Keys.OrderBy(name => name), types.Select(type => type.Name).OrderBy(name => name));
        foreach (var type in types)
        {
            var names = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.Name != "EqualityContract").Select(p => p.Name).ToList();
            Assert.Equal(Allowed[type.Name].OrderBy(name => name), names.OrderBy(name => name));
            Assert.DoesNotContain(names, name => Forbidden.Any(fragment => name.Contains(fragment, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
