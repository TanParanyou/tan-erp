using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Finance;

namespace TanErp.Infrastructure.Documents;

public sealed class ETaxSigningOptions
{
    public const string SectionName = "ETax";

    public string? SigningCertificatePath { get; init; }
    public string? SigningCertificatePassword { get; init; }
}

/// <summary>
/// ETDA e-Tax Invoice XML builder (UN/CEFACT Cross Industry Invoice profile of ETDA Recommendation 3-2560) for a tax invoice.
/// Populates Seller and Buyer structured PostalTradeAddress, references approved Credit/Debit Adjustment Notes (type codes 81/80),
/// and supports optional enveloped RSA-SHA256 XML Digital Signature (<c>ds:Signature</c>).
/// </summary>
public static class ETaxInvoiceXmlBuilder
{
    private static readonly XNamespace Rsm = "urn:etda:uncefact:data:standard:TaxInvoice_CrossIndustryInvoice:2";
    private static readonly XNamespace Ram = "urn:etda:uncefact:data:standard:TaxInvoice_ReusableAggregateBusinessInformationEntity:2";
    private static readonly XNamespace Udt = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:16";
    private static readonly XNamespace Ds = "http://www.w3.org/2000/09/xmldsig#";

    private const string GuidelineId = "ER3-2560";
    private const string TaxInvoiceTypeCode = "T02";
    private const string DraftNotice = " DRAFT: not validated against the ETDA XSD and not digitally signed. Do not submit. ";

    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public sealed record AddressData(
        string LineOne,
        string? CitySubDivisionName = null,
        string? CityName = null,
        string? CountrySubDivisionName = null,
        string? PostalCode = null,
        string? CountryId = "TH");

    private static string Money(decimal value) => value.ToString("0.00", Invariant);

    // Stored as a ratio (0.07); the e-Tax profile expects a percent (7).
    private static string Rate(decimal value) => (value * 100m).ToString("0.##", Invariant);

    private static XElement Party(string name, string? taxId, string? branchCode, AddressData? address, XNamespace ram)
    {
        var party = new XElement(ram + "Name", name);
        var children = new List<XElement> { party };
        if (!string.IsNullOrWhiteSpace(taxId))
        {
            children.Add(new XElement(ram + "SpecifiedTaxRegistration", new XElement(ram + "ID", new XAttribute("schemeID", "TXID"), taxId.Trim())));
        }

        if (!string.IsNullOrWhiteSpace(branchCode))
        {
            children.Add(new XElement(ram + "ID", branchCode.Trim()));
        }

        if (address is not null && !string.IsNullOrWhiteSpace(address.LineOne))
        {
            var addressElements = new List<XElement>();
            if (!string.IsNullOrWhiteSpace(address.PostalCode))
            {
                addressElements.Add(new XElement(ram + "PostcodeCode", address.PostalCode.Trim()));
            }

            addressElements.Add(new XElement(ram + "LineOne", address.LineOne.Trim()));

            if (!string.IsNullOrWhiteSpace(address.CityName))
            {
                addressElements.Add(new XElement(ram + "CityName", address.CityName.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(address.CitySubDivisionName))
            {
                addressElements.Add(new XElement(ram + "CitySubDivisionName", address.CitySubDivisionName.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(address.CountryId))
            {
                addressElements.Add(new XElement(ram + "CountryID", address.CountryId.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(address.CountrySubDivisionName))
            {
                addressElements.Add(new XElement(ram + "CountrySubDivisionName", address.CountrySubDivisionName.Trim()));
            }

            children.Add(new XElement(ram + "PostalTradeAddress", addressElements));
        }

        return new XElement(ram + "Party", children);
    }

    private static AddressData? ResolveBuyerAddress(Customer? customer)
    {
        if (customer?.Addresses is null || customer.Addresses.Count == 0)
        {
            return null;
        }

        var selected = customer.Addresses.FirstOrDefault(a => a.Status == "active" && a.IsPrimary && a.AddressType == "billing")
            ?? customer.Addresses.FirstOrDefault(a => a.Status == "active" && a.IsPrimary)
            ?? customer.Addresses.FirstOrDefault(a => a.Status == "active")
            ?? customer.Addresses.First();

        if (string.IsNullOrWhiteSpace(selected.AddressLine1))
        {
            return null;
        }

        return new AddressData(
            selected.AddressLine1.Trim(),
            string.IsNullOrWhiteSpace(selected.Subdistrict) ? null : selected.Subdistrict.Trim(),
            string.IsNullOrWhiteSpace(selected.District) ? null : selected.District.Trim(),
            string.IsNullOrWhiteSpace(selected.Province) ? null : selected.Province.Trim(),
            string.IsNullOrWhiteSpace(selected.PostalCode) ? null : selected.PostalCode.Trim(),
            string.IsNullOrWhiteSpace(selected.CountryCode) ? "TH" : selected.CountryCode.Trim().ToUpperInvariant());
    }

    public static string Build(TaxDocumentPdfModel model, ETaxSigningOptions? signingOptions = null)
    {
        var b = model.Billing;
        var issued = b.IssuedAtUtc.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-dd'T'HH:mm:ss", Invariant);
        var sellerAddressStr = model.Branch.AddressTh ?? model.Organization.AddressTh;
        AddressData? sellerAddress = string.IsNullOrWhiteSpace(sellerAddressStr)
            ? null
            : new AddressData(sellerAddressStr.Trim());
        var buyerName = model.Customer is null ? string.Empty : (model.Customer.LegalName ?? model.Customer.DisplayNameTh);
        var buyerAddress = ResolveBuyerAddress(model.Customer);

        XElement Retag(XElement party, string name) => new(Ram + name, party.Elements());

        var agreementChildren = new List<XElement>
        {
            Retag(Party(model.Organization.Name, model.Organization.TaxIdentifier, model.Branch.TaxBranchCode ?? "00000", sellerAddress, Ram), "SellerTradeParty"),
            Retag(Party(buyerName, model.Customer?.TaxIdentifier, model.Customer?.BranchCode, buyerAddress, Ram), "BuyerTradeParty")
        };

        foreach (var note in model.ApprovedNotes)
        {
            var noteIssued = (note.DecidedAtUtc ?? note.CreatedAtUtc).ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-dd'T'HH:mm:ss", Invariant);
            var typeCode = note.NoteType is BillingNoteType.Credit or "credit_note" ? "81" : "80";
            agreementChildren.Add(new XElement(Ram + "AdditionalReferencedDocument",
                new XElement(Ram + "IssuerAssignedID", note.Number),
                new XElement(Ram + "IssueDateTime", noteIssued),
                new XElement(Ram + "TypeCode", typeCode),
                new XElement(Ram + "Name", note.Reason)));
        }

        var lines = model.Lines.Select(l => new XElement(Ram + "IncludedSupplyChainTradeLineItem",
            new XElement(Ram + "AssociatedDocumentLineDocument", new XElement(Ram + "LineID", l.LineNumber.ToString(Invariant))),
            new XElement(Ram + "SpecifiedTradeProduct", new XElement(Ram + "Name", l.Description)),
            new XElement(Ram + "SpecifiedLineTradeSettlement",
                new XElement(Ram + "ApplicableTradeTax",
                    new XElement(Ram + "TypeCode", "VAT"),
                    new XElement(Ram + "CalculatedRate", Rate(l.VatRate))),
                new XElement(Ram + "SpecifiedTradeSettlementLineMonetarySummation",
                    new XElement(Ram + "LineTotalAmount", Money(l.TaxBaseAmount))))));

        var lineTaxBase = model.Lines.Sum(l => l.TaxBaseAmount);
        var taxBase = lineTaxBase + b.DebitNoteBaseAmount - b.CreditNoteBaseAmount;
        var vat = model.Lines.Sum(l => l.VatAmount) + b.DebitNoteVatAmount - b.CreditNoteVatAmount;
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
                    new XElement(Ram + "ApplicableHeaderTradeAgreement", agreementChildren),
                    new XElement(Ram + "ApplicableHeaderTradeDelivery"),
                    new XElement(Ram + "ApplicableHeaderTradeSettlement",
                        new XElement(Ram + "InvoiceCurrencyCode", b.Currency),
                        new XElement(Ram + "ApplicableTradeTax",
                            new XElement(Ram + "CalculatedAmount", Money(vat)),
                            new XElement(Ram + "TypeCode", "VAT"),
                            new XElement(Ram + "BasisAmount", Money(taxBase)),
                            new XElement(Ram + "CalculatedRate", Rate(b.VatRate))),
                        new XElement(Ram + "SpecifiedTradeSettlementHeaderMonetarySummation",
                            new XElement(Ram + "LineTotalAmount", Money(lineTaxBase)),
                            new XElement(Ram + "TaxBasisTotalAmount", Money(taxBase)),
                            new XElement(Ram + "TaxTotalAmount", new XAttribute("currencyID", b.Currency), Money(vat)),
                            new XElement(Ram + "GrandTotalAmount", Money(taxBase + vat)))),
                    lines)));

        var xml = SerializeUtf8(doc);

        if (signingOptions is not null &&
            !string.IsNullOrWhiteSpace(signingOptions.SigningCertificatePath) &&
            File.Exists(signingOptions.SigningCertificatePath))
        {
            using var cert = X509CertificateLoader.LoadPkcs12FromFile(
                signingOptions.SigningCertificatePath,
                signingOptions.SigningCertificatePassword);
            using var rsa = cert.GetRSAPrivateKey();
            if (rsa is not null)
            {
                return SignXml(xml, rsa, cert);
            }
        }

        return xml;
    }

    /// <summary>
    /// Signs an e-Tax XML document with an enveloped RSA-SHA256 XML Digital Signature (<c>ds:Signature</c>)
    /// using SHA-256 digest and appends <c>&lt;ds:Signature&gt;</c> to the root element.
    /// </summary>
    public static string SignXml(string xml, RSA rsaKey, X509Certificate2? cert = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);
        ArgumentNullException.ThrowIfNull(rsaKey);

        var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
        if (doc.Root is null)
        {
            throw new InvalidOperationException("XML document has no root element.");
        }

        // Remove any existing ds:Signature before computing the enveloped digest.
        doc.Root.Elements(Ds + "Signature").Remove();

        var canonicalPayload = doc.Root.ToString(SaveOptions.DisableFormatting);
        var digestBytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalPayload));
        var digestBase64 = Convert.ToBase64String(digestBytes);

        var signedInfo = new XElement(Ds + "SignedInfo",
            new XAttribute(XNamespace.Xmlns + "ds", Ds),
            new XElement(Ds + "CanonicalizationMethod",
                new XAttribute("Algorithm", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315")),
            new XElement(Ds + "SignatureMethod",
                new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")),
            new XElement(Ds + "Reference",
                new XAttribute("URI", string.Empty),
                new XElement(Ds + "Transforms",
                    new XElement(Ds + "Transform",
                        new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#enveloped-signature"))),
                new XElement(Ds + "DigestMethod",
                    new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256")),
                new XElement(Ds + "DigestValue", digestBase64)));

        var signedInfoBytes = Encoding.UTF8.GetBytes(signedInfo.ToString(SaveOptions.DisableFormatting));
        var signatureBytes = rsaKey.SignData(signedInfoBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var signatureBase64 = Convert.ToBase64String(signatureBytes);

        var keyInfoChildren = new List<XElement>();
        if (cert is not null)
        {
            keyInfoChildren.Add(new XElement(Ds + "X509Data",
                new XElement(Ds + "X509Certificate", Convert.ToBase64String(cert.RawData))));
        }

        var rsaParams = rsaKey.ExportParameters(false);
        if (rsaParams.Modulus is not null && rsaParams.Exponent is not null)
        {
            keyInfoChildren.Add(new XElement(Ds + "KeyValue",
                new XElement(Ds + "RSAKeyValue",
                    new XElement(Ds + "Modulus", Convert.ToBase64String(rsaParams.Modulus)),
                    new XElement(Ds + "Exponent", Convert.ToBase64String(rsaParams.Exponent)))));
        }

        var signatureElement = new XElement(Ds + "Signature",
            new XAttribute(XNamespace.Xmlns + "ds", Ds),
            signedInfo,
            new XElement(Ds + "SignatureValue", signatureBase64),
            new XElement(Ds + "KeyInfo", keyInfoChildren));

        doc.Root.Add(signatureElement);
        return SerializeUtf8(doc);
    }

    public static bool VerifyXmlSignature(string signedXml, RSA rsaKey)
    {
        if (string.IsNullOrWhiteSpace(signedXml)) return false;
        var doc = XDocument.Parse(signedXml, LoadOptions.PreserveWhitespace);
        var root = doc.Root;
        var sig = root?.Element(Ds + "Signature");
        if (root is null || sig is null) return false;

        var signedInfo = sig.Element(Ds + "SignedInfo");
        var sigValueText = sig.Element(Ds + "SignatureValue")?.Value;
        var digestValueText = signedInfo?.Element(Ds + "Reference")?.Element(Ds + "DigestValue")?.Value;
        if (signedInfo is null || string.IsNullOrWhiteSpace(sigValueText) || string.IsNullOrWhiteSpace(digestValueText))
        {
            return false;
        }

        sig.Remove();
        var canonicalPayload = root.ToString(SaveOptions.DisableFormatting);
        var expectedDigest = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalPayload)));
        if (!string.Equals(expectedDigest, digestValueText.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        var signedInfoForVerify = new XElement(signedInfo);
        if (signedInfoForVerify.Attribute(XNamespace.Xmlns + "ds") is null)
        {
            signedInfoForVerify.SetAttributeValue(XNamespace.Xmlns + "ds", Ds);
        }

        var signedInfoBytes = Encoding.UTF8.GetBytes(signedInfoForVerify.ToString(SaveOptions.DisableFormatting));
        var signatureBytes = Convert.FromBase64String(sigValueText.Trim());
        return rsaKey.VerifyData(signedInfoBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    private static string SerializeUtf8(XDocument doc)
    {
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
