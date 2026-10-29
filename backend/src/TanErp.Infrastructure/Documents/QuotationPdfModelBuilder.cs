using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Results;
using TanErp.Domain.Commercial;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Estimates;

namespace TanErp.Infrastructure.Documents;

/// <summary>
/// Projects one quotation (possibly not saved yet) into the allowlisted PDF model. Only selling-side columns are read:
/// the cost, margin and markup columns of the estimate revision are never touched.
/// </summary>
internal static class QuotationPdfModelBuilder
{
    private static Result<QuotationPdfModel> Fail(string code, string message) => Result<QuotationPdfModel>.Failure(new Error(code, message));

    public static async Task<Result<QuotationPdfModel>> BuildAsync(
        AppDbContext db,
        Quotation quotation,
        CancellationToken ct,
        TanErp.Application.Files.IFileStorageProvider? storage = null)
    {
        var organizationId = quotation.OrganizationId;
        var revision = await db.EstimateRevisions.AsNoTracking().Include(r => r.Sections).ThenInclude(s => s.WorkItems)
            .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.Id == quotation.EstimateRevisionId, ct);
        var organization = await db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId, ct);
        var branch = await db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.Id == quotation.BranchId, ct);
        if (revision is null || organization is null || branch is null) return Fail("RESOURCE_NOT_FOUND", "Estimate revision, organization or branch not found.");

        QuotationBillingSnapshotData? snapshot = null;
        try
        {
            snapshot = string.IsNullOrWhiteSpace(quotation.CustomerBillingSnapshotJson)
                ? null
                : JsonSerializer.Deserialize<QuotationBillingSnapshotData>(quotation.CustomerBillingSnapshotJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            // Unreadable stored data is reported below without logging its (customer) content.
        }

        if (snapshot is null) return Fail("ESTIMATE_INVALID_STATE", "Quotation billing snapshot is missing or corrupted.");

        string? supersedesNumber = null;
        if (quotation.SupersedesQuotationId is { } supersededId)
            supersedesNumber = await db.Quotations.AsNoTracking().Where(q => q.OrganizationId == organizationId && q.Id == supersededId).Select(q => q.Number).FirstOrDefaultAsync(ct);

        byte[]? logoBytes = null;
        if (organization.BrandLogoFileId is { } logoFileId && storage is not null)
        {
            var logoFile = await db.UploadedFiles.AsNoTracking().FirstOrDefaultAsync(f => f.OrganizationId == organizationId && f.Id == logoFileId, ct);
            if (logoFile is not null)
            {
                try
                {
                    using var stream = await storage.OpenReadStreamAsync(logoFile.StoragePath, ct);
                    if (stream is not null)
                    {
                        using var ms = new MemoryStream();
                        await stream.CopyToAsync(ms, ct);
                        logoBytes = ms.ToArray();
                    }
                }
                catch
                {
                    // File storage read error should not prevent quotation rendering; logo is omitted.
                }
            }
        }

        // Business rule (ADR 0023): a branch with its own address (Thai or English) uses that address pair and phone as a whole;
        // otherwise the organization's pair is used. The two are never mixed line by line.
        var branchOwnsAddress = !string.IsNullOrWhiteSpace(branch.AddressTh) || !string.IsNullOrWhiteSpace(branch.AddressEn);
        var issuer = new QuotationPdfIssuer(
            organization.Name, organization.NameEn, branch.Name, branch.NameEn, organization.TaxIdentifier, branch.TaxBranchCode,
            branchOwnsAddress ? branch.AddressTh : organization.AddressTh, branchOwnsAddress ? branch.AddressEn : organization.AddressEn,
            branchOwnsAddress ? branch.Phone : organization.Phone,
            logoBytes);
        var customer = new QuotationPdfCustomer(
            snapshot.DisplayNameTh, snapshot.DisplayNameEn, snapshot.LegalName, snapshot.TaxIdentifier, snapshot.BranchCode,
            snapshot.Address is null ? null : new QuotationPdfAddress(snapshot.Address.AddressLine1, snapshot.Address.Subdistrict, snapshot.Address.District, snapshot.Address.Province, snapshot.Address.PostalCode));
        var sections = revision.Sections.OrderBy(s => s.SortOrder).Select(section => new QuotationPdfSection(
            section.Code, section.NameTh, section.NameEn, section.SubtotalSellingPrice,
            section.WorkItems.OrderBy(w => w.SortOrder).Select(item => new QuotationPdfWorkItem(
                item.Code, item.DescriptionTh, item.DescriptionEn, item.Quantity, item.UnitCode, item.UnitSellingPrice, item.TotalSellingPrice)).ToList())).ToList();
        var totals = new QuotationPdfTotals(revision.SellingBeforeDiscount, revision.DiscountAmount, revision.NetBeforeTax, revision.TaxAmount, revision.GrandTotal);

        return Result<QuotationPdfModel>.Success(new QuotationPdfModel(
            quotation.Number, quotation.IssuedAtUtc, revision.Currency, supersedesNumber, issuer, customer, sections, totals));
    }
}
