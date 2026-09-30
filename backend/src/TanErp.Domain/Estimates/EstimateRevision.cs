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
    public string CalculationPolicyVersion { get; private set; } = EstimateDefaults.UnresolvedPolicyVersion;
    public string TaxPolicyVersion { get; private set; } = EstimateDefaults.UnresolvedPolicyVersion;
    public bool CalculationOutdated { get; private set; } = true;
    public Guid? LastFinancialEditorUserId { get; private set; }
    public Guid? SubmittedByUserId { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTimeOffset? ApprovedAtUtc { get; private set; }
    public string? ApprovalSnapshotJson { get; private set; }
    public decimal NetCost { get; private set; }
    public decimal SellingBeforeDiscount { get; private set; }
    public string DiscountType { get; private set; } = EstimateDiscount.None;
    public decimal DiscountValue { get; private set; }
    public string? DiscountReasonCode { get; private set; }
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

    public EstimateReadinessResult EvaluateReadiness(bool calculationSnapshotBeingWritten = false)
    {
        var reasons = new List<EstimateReadinessReason>();
        var workItems = _sections.SelectMany(section => section.WorkItems).ToArray();

        if (CalculationOutdated || CalculationVersion <= 0 ||
            (!calculationSnapshotBeingWritten && string.IsNullOrWhiteSpace(CalculationSnapshotJson)))
            reasons.Add(new EstimateReadinessReason("ESTIMATE_CALCULATION_OUTDATED", "revision", Id, "calculation"));
        if (_sections.Count == 0)
            reasons.Add(new EstimateReadinessReason("ESTIMATE_FIELD_REQUIRED", "revision", Id, "sections"));
        foreach (var section in _sections.Where(section => section.WorkItems.Count == 0))
            reasons.Add(new EstimateReadinessReason("ESTIMATE_FIELD_REQUIRED", "section", section.Id, "workItems"));
        foreach (var workItem in workItems.Where(workItem => workItem.CostComponents.Count == 0))
            reasons.Add(new EstimateReadinessReason("ESTIMATE_COST_INCOMPLETE", "workItem", workItem.Id, "costComponents"));
        foreach (var workItem in workItems.Where(workItem =>
                     workItem.SellingRuleType == SellingRuleType.FixedPrice &&
                     string.IsNullOrWhiteSpace(workItem.SellingRuleReasonCode)))
            reasons.Add(new EstimateReadinessReason("ESTIMATE_FIXED_PRICE_REASON_REQUIRED", "workItem", workItem.Id, "sellingRuleReasonCode"));
        foreach (var workItem in workItems.Where(workItem => workItem.ItemId is null &&
                     (string.IsNullOrWhiteSpace(workItem.OverrideReasonCode) || string.IsNullOrWhiteSpace(workItem.OverrideReason))))
        {
            if (string.IsNullOrWhiteSpace(workItem.OverrideReasonCode))
                reasons.Add(new EstimateReadinessReason("ESTIMATE_CUSTOM_WORK_ITEM_REASON_CODE_REQUIRED", "workItem", workItem.Id, "overrideReasonCode"));
            if (string.IsNullOrWhiteSpace(workItem.OverrideReason))
                reasons.Add(new EstimateReadinessReason("ESTIMATE_CUSTOM_WORK_ITEM_REASON_REQUIRED", "workItem", workItem.Id, "overrideReason"));
        }
        foreach (var component in workItems.SelectMany(workItem => workItem.CostComponents).Where(component => component.IsProvisional))
        {
            if (string.IsNullOrWhiteSpace(component.ProvisionalReasonCode))
                reasons.Add(new EstimateReadinessReason("ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED", "costComponent", component.Id, "provisionalReasonCode"));
        }

        if (reasons.Count > 0)
            return new EstimateReadinessResult(EstimateReadinessStatus.Blocked, reasons);

        var provisionalComponents = workItems.SelectMany(workItem => workItem.CostComponents)
            .Where(component => component.IsProvisional).ToArray();
        foreach (var component in provisionalComponents)
            reasons.Add(new EstimateReadinessReason("ESTIMATE_PROVISIONAL_COST", "costComponent", component.Id, "provisionalReasonCode"));
        foreach (var workItem in workItems.Where(workItem =>
                     workItem.SellingRuleType == SellingRuleType.FixedPrice &&
                     !string.IsNullOrWhiteSpace(workItem.SellingRuleReasonCode)))
            reasons.Add(new EstimateReadinessReason("ESTIMATE_FIXED_PRICE_OVERRIDE", "workItem", workItem.Id, "sellingRuleReasonCode"));
        foreach (var workItem in workItems.Where(workItem => workItem.ItemId is null &&
                     !string.IsNullOrWhiteSpace(workItem.OverrideReasonCode) && !string.IsNullOrWhiteSpace(workItem.OverrideReason)))
            reasons.Add(new EstimateReadinessReason("CUSTOM_WORK_ITEM", "workItem", workItem.Id, "overrideReason"));

        if (NetCost == 0m || NetBeforeTax == 0m)
        {
            reasons.Add(new EstimateReadinessReason("ESTIMATE_ZERO_DENOMINATOR", "revision", Id, "marginRate"));
        }

        if (reasons.Count > 0)
            return new EstimateReadinessResult(EstimateReadinessStatus.RequiresAttention, reasons);

        return new EstimateReadinessResult(EstimateReadinessStatus.Ready, reasons);
    }

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

    public static EstimateRevision CloneAsDraft(Guid id, int revisionNo, EstimateRevision source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var clone = new EstimateRevision(id, source.OrganizationId, source.EstimateId, revisionNo, EstimateRevisionStatus.Draft, source.Currency);
        clone.DiscountType = source.DiscountType;
        clone.DiscountValue = source.DiscountValue;
        clone.DiscountReasonCode = source.DiscountReasonCode;
        foreach (var sourceSection in source.Sections.OrderBy(section => section.SortOrder))
        {
            var section = new EstimateSection(Guid.NewGuid(), source.OrganizationId, clone.Id,
                sourceSection.Code, sourceSection.NameTh, sourceSection.NameEn, sourceSection.SortOrder);
            foreach (var sourceItem in sourceSection.WorkItems.OrderBy(item => item.SortOrder))
            {
                var item = new EstimateWorkItem(Guid.NewGuid(), source.OrganizationId, section.Id,
                    sourceItem.Code, sourceItem.DescriptionTh, sourceItem.DescriptionEn, sourceItem.Quantity,
                    sourceItem.UnitCode, sourceItem.SellingRuleType, sourceItem.SellingRuleValue, sourceItem.SortOrder,
                    sourceItem.SellingRuleReasonCode);
                item.SetItemMasterLink(sourceItem.ItemId, sourceItem.ItemCodeSnapshot,
                    sourceItem.ItemNameThSnapshot, sourceItem.ItemNameEnSnapshot);
                item.SetCustomWorkItemReason(sourceItem.OverrideReasonCode, sourceItem.OverrideReason);
                foreach (var sourceCost in sourceItem.CostComponents.OrderBy(component => component.SortOrder))
                {
                    var cost = new EstimateCostComponent(Guid.NewGuid(), source.OrganizationId, item.Id,
                        sourceCost.Type, sourceCost.Description, sourceCost.Quantity, sourceCost.UnitCode,
                        sourceCost.UnitCost, sourceCost.Currency, sourceCost.SortOrder);
                    if (sourceCost.ItemId.HasValue && sourceCost.CostRecordId.HasValue && sourceCost.CostRecordVersion.HasValue &&
                        sourceCost.ItemCodeSnapshot is not null && sourceCost.ItemNameSnapshot is not null &&
                        sourceCost.UnitSnapshot is not null && sourceCost.UnitCostSnapshot.HasValue &&
                        sourceCost.CurrencySnapshot is not null)
                    {
                        cost.SetCatalogCostSnapshot(sourceCost.ItemId.Value, sourceCost.CostRecordId.Value,
                            sourceCost.CostRecordVersion.Value, sourceCost.ItemCodeSnapshot, sourceCost.ItemNameSnapshot,
                            sourceCost.UnitSnapshot, sourceCost.UnitCostSnapshot.Value, sourceCost.CurrencySnapshot,
                            sourceCost.CostScopeSnapshot ?? string.Empty, sourceCost.CostEffectiveFromUtc,
                            sourceCost.CostPolicyVersion, sourceCost.ResolvedAtUtc ?? source.UpdatedAtUtc,
                            sourceCost.CostSourceIdSnapshot, sourceCost.CostSourceCodeSnapshot,
                            sourceCost.CostSourceReferenceSnapshot, sourceCost.CostEvidenceFileIdSnapshot,
                            sourceCost.CostRecordReasonSnapshot);
                    }
                    cost.SetProvisionalReason(sourceCost.ProvisionalReasonCode, sourceCost.ProvisionalNote);
                    item.AddCostComponent(cost);
                }
                section.AddWorkItem(item);
            }
            clone.AddSection(section);
        }
        return clone;
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

    public void MarkCalculationOutdated()
    {
        if (Status != EstimateRevisionStatus.Draft && Status != EstimateRevisionStatus.Returned)
            throw new EstimateInvalidStateException($"Cannot change financial inputs in '{Status}' status.");

        CalculationOutdated = true;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkFinancialInputChanged(Guid actorUserId)
    {
        if (Status != EstimateRevisionStatus.Draft && Status != EstimateRevisionStatus.Returned)
            throw new EstimateInvalidStateException($"Cannot change financial inputs in '{Status}' status.");
        if (actorUserId == Guid.Empty)
            throw new ArgumentException("Financial editor user ID must be provided.", nameof(actorUserId));

        CalculationOutdated = true;
        LastFinancialEditorUserId = actorUserId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Submit(Guid submittedByUserId, DateTimeOffset submittedAtUtc)
    {
        if (Status != EstimateRevisionStatus.Draft && Status != EstimateRevisionStatus.Returned)
            throw new EstimateInvalidStateException($"Cannot submit estimate revision in status '{Status}'.");
        if (submittedByUserId == Guid.Empty)
            throw new ArgumentException("Submitter user ID must be provided.", nameof(submittedByUserId));
        if (CalculationOutdated || CalculationVersion <= 0 || string.IsNullOrWhiteSpace(CalculationSnapshotJson))
            throw new EstimateInvalidStateException("A current calculation snapshot is required before submit.");

        Status = EstimateRevisionStatus.Submitted;
        SubmittedByUserId = submittedByUserId;
        SubmittedAtUtc = submittedAtUtc;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = submittedAtUtc;
    }

    public void Approve(Guid reviewerUserId, DateTimeOffset approvedAtUtc, string approvalSnapshotJson)
    {
        if (Status != EstimateRevisionStatus.Submitted)
            throw new EstimateInvalidStateException($"Cannot approve estimate revision in status '{Status}'.");
        if (reviewerUserId == Guid.Empty || reviewerUserId == SubmittedByUserId || reviewerUserId == LastFinancialEditorUserId)
            throw new EstimateInvalidStateException("The submitter and last financial editor cannot approve this estimate revision.");
        if (string.IsNullOrWhiteSpace(approvalSnapshotJson))
            throw new ArgumentException("Approval snapshot is required.", nameof(approvalSnapshotJson));

        Status = EstimateRevisionStatus.Approved;
        ApprovedByUserId = reviewerUserId;
        ApprovedAtUtc = approvedAtUtc;
        ApprovalSnapshotJson = approvalSnapshotJson;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = approvedAtUtc;
    }

    public void Return(Guid reviewerUserId, DateTimeOffset returnedAtUtc, string reasonCode, string note)
    {
        if (Status != EstimateRevisionStatus.Submitted)
            throw new EstimateInvalidStateException($"Cannot return estimate revision in status '{Status}'.");
        if (reviewerUserId == Guid.Empty || reviewerUserId == SubmittedByUserId || reviewerUserId == LastFinancialEditorUserId)
            throw new EstimateInvalidStateException("The submitter and last financial editor cannot return this estimate revision.");
        if (string.IsNullOrWhiteSpace(reasonCode) || string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("A reason code and note are required to return an estimate revision.");

        Status = EstimateRevisionStatus.Returned;
        CalculationOutdated = true;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = returnedAtUtc;
    }

    public void Cancel(DateTimeOffset cancelledAtUtc, string reason)
    {
        if (Status is not (EstimateRevisionStatus.Draft or EstimateRevisionStatus.Returned or EstimateRevisionStatus.Submitted))
            throw new EstimateInvalidStateException($"Cannot cancel estimate revision in status '{Status}'.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A cancellation reason is required.", nameof(reason));

        Status = EstimateRevisionStatus.Cancelled;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = cancelledAtUtc;
    }

    public void Calculate(
        decimal discountAmount,
        CalculationPolicyVersion calculationPolicy,
        TaxPolicyVersion taxPolicy,
        DateTimeOffset calculatedAtUtc) =>
        Calculate(EstimateDiscount.LegacyFixedAmount(discountAmount), calculationPolicy, taxPolicy, calculatedAtUtc);

    public void Calculate(
        EstimateDiscount discount,
        CalculationPolicyVersion calculationPolicy,
        TaxPolicyVersion taxPolicy,
        DateTimeOffset calculatedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(discount);
        ArgumentNullException.ThrowIfNull(calculationPolicy);
        ArgumentNullException.ThrowIfNull(taxPolicy);
        if (Status != EstimateRevisionStatus.Draft && Status != EstimateRevisionStatus.Returned)
            throw new EstimateInvalidStateException($"Cannot calculate estimate in '{Status}' status.");

        if (calculationPolicy.OrganizationId != OrganizationId || taxPolicy.OrganizationId != OrganizationId)
            throw new ArgumentException("Resolved policies must belong to the estimate organization.");
        if (!calculationPolicy.IsEffectiveAt(calculatedAtUtc) || !taxPolicy.IsEffectiveAt(calculatedAtUtc))
            throw new EstimateInvalidStateException("Resolved estimate policies are not currently effective.");

        foreach (var section in _sections)
        {
            section.Recalculate();
        }

        var rounding = calculationPolicy.RoundingMode == "away-from-zero"
            ? MidpointRounding.AwayFromZero
            : throw new ArgumentException("Calculation policy rounding mode is unsupported.", nameof(calculationPolicy));
        var directCost = decimal.Round(_sections.Sum(section => section.SubtotalCost), 2, rounding);
        var overheadAmount = calculationPolicy.OverheadMethod switch
        {
            "none" => 0m,
            "percent-direct-cost" => decimal.Round(directCost * calculationPolicy.OverheadValue, 2, rounding),
            "fixed-amount" => decimal.Round(calculationPolicy.OverheadValue, 2, rounding),
            _ => throw new ArgumentException("Calculation policy overhead method is unsupported.", nameof(calculationPolicy))
        };
        var workItems = _sections.OrderBy(section => section.SortOrder)
            .SelectMany(section => section.WorkItems.OrderBy(workItem => workItem.SortOrder))
            .ToArray();
        if (overheadAmount > 0m && directCost <= 0m)
            throw new ArgumentException("Overhead cannot be allocated without direct cost.", nameof(calculationPolicy));

        var costBases = new Dictionary<Guid, decimal>();
        var overheadAllocated = 0m;
        for (var index = 0; index < workItems.Length; index++)
        {
            var item = workItems[index];
            var allocation = index == workItems.Length - 1
                ? overheadAmount - overheadAllocated
                : decimal.Round(overheadAmount * item.TotalCost / directCost, 2, rounding);
            overheadAllocated += allocation;
            costBases.Add(item.Id, item.TotalCost + allocation);
        }

        foreach (var section in _sections)
            section.RecalculateWithCostBases(costBases);

        var netCost = decimal.Round(_sections.Sum(s => s.SubtotalCost), 2, MidpointRounding.AwayFromZero);
        var sellingBeforeDiscount = decimal.Round(_sections.Sum(s => s.SubtotalSellingPrice), 2, MidpointRounding.AwayFromZero);
        var roundedDiscount = discount.ResolveAmount(sellingBeforeDiscount, rounding);

        NetCost = netCost;
        SellingBeforeDiscount = sellingBeforeDiscount;
        DiscountType = discount.Type;
        DiscountValue = discount.Value;
        DiscountReasonCode = string.IsNullOrWhiteSpace(discount.ReasonCode) ? null : discount.ReasonCode.Trim();
        DiscountAmount = roundedDiscount;
        var discountedSelling = decimal.Round(SellingBeforeDiscount - DiscountAmount, 2, MidpointRounding.AwayFromZero);
        switch (taxPolicy.TaxMode)
        {
            case "exclusive":
                NetBeforeTax = discountedSelling;
                TaxAmount = decimal.Round(NetBeforeTax * taxPolicy.TaxRate, 2, rounding);
                GrandTotal = decimal.Round(NetBeforeTax + TaxAmount, 2, rounding);
                break;
            case "inclusive":
                GrandTotal = discountedSelling;
                NetBeforeTax = decimal.Round(GrandTotal / (1m + taxPolicy.TaxRate), 2, rounding);
                TaxAmount = decimal.Round(GrandTotal - NetBeforeTax, 2, rounding);
                break;
            case "exempt":
                NetBeforeTax = discountedSelling;
                TaxAmount = 0m;
                GrandTotal = NetBeforeTax;
                break;
            default:
                throw new ArgumentException("Tax policy mode is unsupported.", nameof(taxPolicy));
        }

        MarginAmount = decimal.Round(NetBeforeTax - NetCost, 2, MidpointRounding.AwayFromZero);
        MarginRate = NetBeforeTax > 0
            ? decimal.Round(MarginAmount / NetBeforeTax, 4, MidpointRounding.AwayFromZero)
            : 0;
        MarkupRate = NetCost > 0
            ? decimal.Round(MarginAmount / NetCost, 4, MidpointRounding.AwayFromZero)
            : 0;

        CalculationPolicyVersion = $"{calculationPolicy.PolicyCode}-v{calculationPolicy.Version}";
        TaxPolicyVersion = $"{taxPolicy.PolicyCode}-v{taxPolicy.Version}";

        CalculationVersion++;
        CalculationOutdated = false;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        var readiness = EvaluateReadiness(calculationSnapshotBeingWritten: true);
        var snapshotObj = new
        {
            calculationVersion = CalculationVersion,
            calculatedAtUtc = UpdatedAtUtc,
            currency = Currency,
            directCost,
            overheadMethod = calculationPolicy.OverheadMethod,
            overheadValue = calculationPolicy.OverheadValue,
            overheadAmount,
            calculationPolicyVersionId = calculationPolicy.Id,
            calculationPolicyVersion = CalculationPolicyVersion,
            calculationPolicyHash = calculationPolicy.ContentHash,
            taxPolicyVersionId = taxPolicy.Id,
            taxPolicyVersion = TaxPolicyVersion,
            taxPolicyHash = taxPolicy.ContentHash,
            taxMode = taxPolicy.TaxMode,
            taxRate = taxPolicy.TaxRate,
            taxCode = taxPolicy.TaxCode,
            netCost = NetCost,
            sellingBeforeDiscount = SellingBeforeDiscount,
            discountType = discount.Type,
            discountValue = discount.Value,
            discountReasonCode = discount.ReasonCode,
            discountAmount = DiscountAmount,
            netBeforeTax = NetBeforeTax,
            taxAmount = TaxAmount,
            grandTotal = GrandTotal,
            marginAmount = MarginAmount,
            marginRate = MarginRate,
            markupRate = MarkupRate,
            workItemPricing = _sections.OrderBy(section => section.SortOrder)
                .SelectMany(section => section.WorkItems.OrderBy(item => item.SortOrder)
                    .Select(item => new
                    {
                        item.Id,
                        item.SellingRuleType,
                        item.SellingRuleValue,
                        item.SellingRuleReasonCode,
                        item.ItemId,
                        item.ItemCodeSnapshot,
                        item.ItemNameThSnapshot,
                        item.ItemNameEnSnapshot,
                        item.OverrideReasonCode,
                        item.OverrideReason
                    })),
            sectionsCount = _sections.Count,
            workItemsCount = _sections.Sum(s => s.WorkItems.Count),
            costComponentsCount = _sections.Sum(s => s.WorkItems.Sum(w => w.CostComponents.Count)),
            costEvidence = _sections.OrderBy(section => section.SortOrder)
                .SelectMany(section => section.WorkItems.OrderBy(item => item.SortOrder)
                    .SelectMany(item => item.CostComponents.OrderBy(component => component.SortOrder)))
                .Select(component => new
                {
                    component.Id,
                    component.CostOrigin,
                    component.ItemId,
                    component.CostRecordId,
                    component.CostRecordVersion,
                    component.CostSourceIdSnapshot,
                    component.CostSourceCodeSnapshot,
                    component.CostSourceReferenceSnapshot,
                    component.CostEvidenceFileIdSnapshot,
                    component.CostRecordReasonSnapshot,
                    component.ProvisionalReasonCode,
                    component.ProvisionalNote,
                    component.IsProvisional
                }),
            readiness = readiness.Status,
            readinessReasons = readiness.Reasons
        };

        CalculationSnapshotJson = JsonSerializer.Serialize(snapshotObj);
    }

    public void MarkQuoted()
    {
        if (Status != EstimateRevisionStatus.Approved)
            throw new EstimateInvalidStateException($"Cannot quote estimate revision in status '{Status}'.");
        if (CalculationOutdated || string.IsNullOrWhiteSpace(CalculationSnapshotJson) || string.IsNullOrWhiteSpace(ApprovalSnapshotJson))
            throw new EstimateInvalidStateException("Quotation requires current calculation and frozen approval snapshots.");

        Status = EstimateRevisionStatus.Quoted;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
