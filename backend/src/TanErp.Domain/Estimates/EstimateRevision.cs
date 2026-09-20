using System.Text.Json;

namespace TanErp.Domain.Estimates;

public class EstimateRevision
{
    private readonly List<EstimateSection> _sections = new();

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateId { get; private set; }
    public int RevisionNo { get; private set; }
    public string Status { get; private set; } = EstimateRevisionStatus.Draft;
    public string Currency { get; private set; } = EstimateDefaults.DefaultCurrency;
    public int CalculationVersion { get; private set; }
    public string CalculationPolicyVersion { get; private set; } = EstimateDefaults.DefaultCalculationPolicyVersion;
    public string TaxPolicyVersion { get; private set; } = EstimateDefaults.DefaultTaxPolicyVersion;
    public decimal NetCost { get; private set; }
    public decimal SellingBeforeDiscount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal NetBeforeTax { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public decimal MarginAmount { get; private set; }
    public decimal MarginRate { get; private set; }
    public decimal MarkupRate { get; private set; }
    public string? CalculationSnapshotJson { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<EstimateSection> Sections => _sections.AsReadOnly();

    private EstimateRevision() { }

    public EstimateRevision(
        Guid id,
        Guid organizationId,
        Guid estimateId,
        int revisionNo,
        string status = EstimateRevisionStatus.Draft,
        string currency = EstimateDefaults.DefaultCurrency)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Revision ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (estimateId == Guid.Empty)
            throw new ArgumentException("Estimate ID cannot be empty.", nameof(estimateId));
        if (revisionNo <= 0)
            throw new ArgumentException("Revision number must be positive.", nameof(revisionNo));

        Id = id;
        OrganizationId = organizationId;
        EstimateId = estimateId;
        RevisionNo = revisionNo;
        Status = status;
        Currency = string.IsNullOrWhiteSpace(currency) ? EstimateDefaults.DefaultCurrency : currency.Trim();
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void AddSection(EstimateSection section)
    {
        ArgumentNullException.ThrowIfNull(section);
        _sections.Add(section);
    }

    public void ClearSections()
    {
        _sections.Clear();
    }

    public void Calculate(decimal discountAmount = 0)
    {
        if (Status != EstimateRevisionStatus.Draft && Status != EstimateRevisionStatus.Returned)
            throw new EstimateInvalidStateException($"Cannot calculate estimate in '{Status}' status.");

        if (discountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative.", nameof(discountAmount));

        foreach (var section in _sections)
        {
            section.Recalculate();
        }

        NetCost = decimal.Round(_sections.Sum(s => s.SubtotalCost), 2, MidpointRounding.AwayFromZero);
        SellingBeforeDiscount = decimal.Round(_sections.Sum(s => s.SubtotalSellingPrice), 2, MidpointRounding.AwayFromZero);
        DiscountAmount = decimal.Round(discountAmount, 2, MidpointRounding.AwayFromZero);
        NetBeforeTax = decimal.Round(Math.Max(0, SellingBeforeDiscount - DiscountAmount), 2, MidpointRounding.AwayFromZero);
        TaxAmount = decimal.Round(NetBeforeTax * EstimateDefaults.DefaultTaxRate, 2, MidpointRounding.AwayFromZero);
        GrandTotal = decimal.Round(NetBeforeTax + TaxAmount, 2, MidpointRounding.AwayFromZero);

        MarginAmount = decimal.Round(NetBeforeTax - NetCost, 2, MidpointRounding.AwayFromZero);
        MarginRate = NetBeforeTax > 0
            ? decimal.Round(MarginAmount / NetBeforeTax, 4, MidpointRounding.AwayFromZero)
            : 0;
        MarkupRate = NetCost > 0
            ? decimal.Round(MarginAmount / NetCost, 4, MidpointRounding.AwayFromZero)
            : 0;

        CalculationVersion++;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        var snapshotObj = new
        {
            calculationVersion = CalculationVersion,
            calculatedAtUtc = UpdatedAtUtc,
            currency = Currency,
            netCost = NetCost,
            sellingBeforeDiscount = SellingBeforeDiscount,
            discountAmount = DiscountAmount,
            netBeforeTax = NetBeforeTax,
            taxAmount = TaxAmount,
            grandTotal = GrandTotal,
            marginAmount = MarginAmount,
            marginRate = MarginRate,
            markupRate = MarkupRate,
            sectionsCount = _sections.Count,
            workItemsCount = _sections.Sum(s => s.WorkItems.Count),
            costComponentsCount = _sections.Sum(s => s.WorkItems.Sum(w => w.CostComponents.Count))
        };

        CalculationSnapshotJson = JsonSerializer.Serialize(snapshotObj);
    }

    public void MarkQuoted()
    {
        if (Status != EstimateRevisionStatus.Draft && Status != EstimateRevisionStatus.Approved)
            throw new EstimateInvalidStateException($"Cannot quote estimate revision in status '{Status}'.");

        Status = EstimateRevisionStatus.Quoted;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
