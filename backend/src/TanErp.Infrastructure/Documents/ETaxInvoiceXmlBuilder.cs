using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace TanErp.Infrastructure.Documents;

/// <summary>
/// DRAFT e-Tax Invoice XML (UN/CEFACT Cross Industry Invoice profile of ETDA Recommendation 3-2560) for a tax invoice.
/// NOT validated against the official ETDA XSD and NOT digitally signed: namespaces, element names and type codes below come from
/// the author's recollection of the standard, so every item marked UNVERIFIED must be checked against the official schema before
/// any real submission. Credit/debit notes are separate documents and are not generated here.
/// </summary>
public static class ETaxInvoiceXmlBuilder
{
    // UNVERIFIED: namespace URIs and version suffixes.
    private static readonly XNamespace Rsm = "urn:etda:uncefact:data:standard:TaxInvoice_CrossIndustryInvoice:2";
    private static readonly XNamespace Ram = "urn:etda:uncefact:data:standard:TaxInvoice_ReusableAggregateBusinessInformationEntity:2";
    private static readonly XNamespace Udt = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:16";

    // UNVERIFIED: guideline id and tax invoice type code (T02).
    private const string GuidelineId = "ER3-2560";
    private const string TaxInvoiceTypeCode = "T02";
    private const string DraftNotice = " DRAFT: not validated against the ETDA XSD and not digitally signed. Do not submit. ";

    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private static string Money(decimal value) => value.ToString("0.00", Invariant);

    // Stored as a ratio (0.07); the e-Tax profile expects a percent (7).
    private static string Rate(decimal value) => (value * 100m).ToString("0.##", Invariant);

    private static XElement Party(string name, string? taxId, string? branchCode, string? address, XNamespace ram)
    {
        var party = new XElement(ram + "Name", name);
        var children = new List<XElement> { party };
        if (!string.IsNullOrWhiteSpace(taxId))
        {
            // UNVERIFIED: schemeID "TXID" for the tax identification number.
            children.Add(new XElement(ram + "SpecifiedTaxRegistration", new XElement(ram + "ID", new XAttribute("schemeID", "TXID"), taxId.Trim())));
        }

        if (!string.IsNullOrWhiteSpace(branchCode))
        {
            // UNVERIFIED: where the 5-digit branch code is carried.
            children.Add(new XElement(ram + "ID", branchCode.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(address))
        {
            children.Add(new XElement(ram + "PostalTradeAddress", new XElement(ram + "LineOne", address.Trim())));
        }

        return new XElement(ram + "Party", children);
    }

    public static string Build(TaxDocumentPdfModel model)
    {
        var b = model.Billing;
        var issued = b.IssuedAtUtc.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-dd'T'HH:mm:ss", Invariant);
        var sellerAddress = model.Branch.AddressTh ?? model.Organization.AddressTh;
        var buyerName = model.Customer is null ? string.Empty : (model.Customer.LegalName ?? model.Customer.DisplayNameTh);

        XElement Retag(XElement party, string name) => new(Ram + name, party.Elements());

        var lines = model.Lines.Select(l => new XElement(Ram + "IncludedSupplyChainTradeLineItem",
            new XElement(Ram + "AssociatedDocumentLineDocument", new XElement(Ram + "LineID", l.LineNumber.ToString(Invariant))),
            new XElement(Ram + "SpecifiedTradeProduct", new XElement(Ram + "Name", l.Description)),
            new XElement(Ram + "SpecifiedLineTradeSettlement",
                new XElement(Ram + "ApplicableTradeTax",
                    new XElement(Ram + "TypeCode", "VAT"),
                    new XElement(Ram + "CalculatedRate", Rate(l.VatRate))),
                new XElement(Ram + "SpecifiedTradeSettlementLineMonetarySummation",
                    new XElement(Ram + "LineTotalAmount", Money(l.TaxBaseAmount))))));

        var taxBase = model.Lines.Sum(l => l.TaxBaseAmount);
        var vat = model.Lines.Sum(l => l.VatAmount);
        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XComment(DraftNotice),
            new XElement(Rsm + "TaxInvoice_CrossIndustryInvoice",
                new XAttribute(XNamespace.Xmlns + "rsm", Rsm),
                new XAttribute(XNamespace.Xmlns + "ram", Ram),
                new XAttribute(XNamespace.Xmlns + "udt", Udt),
                new XElement(Rsm + "ExchangedDocumentContext",
                    new XElement(Ram + "GuidelineSpecifiedDocumentContextParameter", new XElement(Ram + "ID", GuidelineId))),
                new XElement(Rsm + "ExchangedDocument",
                    new XElement(Ram + "ID", b.TaxInvoiceNumber),
                    new XElement(Ram + "Name", "ใบกำกับภาษี"),
                    new XElement(Ram + "TypeCode", TaxInvoiceTypeCode),
                    new XElement(Ram + "IssueDateTime", issued),
                    new XElement(Ram + "CreationDateTime", issued)),
                new XElement(Rsm + "SupplyChainTradeTransaction",
                    new XElement(Ram + "ApplicableHeaderTradeAgreement",
                        Retag(Party(model.Organization.Name, model.Organization.TaxIdentifier, model.Branch.TaxBranchCode ?? "00000", sellerAddress, Ram), "SellerTradeParty"),
                        Retag(Party(buyerName, model.Customer?.TaxIdentifier, model.Customer?.BranchCode, null, Ram), "BuyerTradeParty")),
                    new XElement(Ram + "ApplicableHeaderTradeDelivery"),
                    new XElement(Ram + "ApplicableHeaderTradeSettlement",
                        new XElement(Ram + "InvoiceCurrencyCode", b.Currency),
                        new XElement(Ram + "ApplicableTradeTax",
                            new XElement(Ram + "CalculatedAmount", Money(vat)),
                            new XElement(Ram + "TypeCode", "VAT"),
                            new XElement(Ram + "BasisAmount", Money(taxBase)),
                            new XElement(Ram + "CalculatedRate", Rate(b.VatRate))),
                        new XElement(Ram + "SpecifiedTradeSettlementHeaderMonetarySummation",
                            new XElement(Ram + "LineTotalAmount", Money(taxBase)),
                            new XElement(Ram + "TaxBasisTotalAmount", Money(taxBase)),
                            new XElement(Ram + "TaxTotalAmount", new XAttribute("currencyID", b.Currency), Money(vat)),
                            new XElement(Ram + "GrandTotalAmount", Money(taxBase + vat)))),
                    lines)));

        var sb = new StringBuilder();
        using (var writer = new StringWriterUtf8(sb))
        {
            doc.Save(writer);
        }

        return sb.ToString();
    }

    private sealed class StringWriterUtf8 : StringWriter
    {
        public StringWriterUtf8(StringBuilder sb) : base(sb, Invariant) { }

        public override Encoding Encoding => Encoding.UTF8;
    }
}
