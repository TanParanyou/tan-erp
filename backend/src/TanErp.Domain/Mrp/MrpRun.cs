using TanErp.Domain.Common;

namespace TanErp.Domain.Mrp;

/// <summary>One planning run. The snapshot and plan are written once and never recomputed; a new plan needs a new run.</summary>
public class MrpRun : Entity
{
    private readonly List<MrpRecommendation> _recommendations = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateOnly AsOfDate { get; private set; }
    public int PurchaseLeadTimeDays { get; private set; }
    public int ProductionLeadTimeDays { get; private set; }
    public string InputHash { get; private set; } = string.Empty;
    public string SnapshotJson { get; private set; } = "{}";
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<MrpRecommendation> Recommendations => _recommendations.AsReadOnly();

    protected MrpRun() { }

    public MrpRun(Guid id, Guid organizationId, Guid branchId, string number, MrpParameters parameters, string inputHash, string snapshotJson, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, branch and actor are required.");
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Run number cannot be blank.", nameof(number));

        OrganizationId = organizationId;
        BranchId = branchId;
        Number = number.Trim();
        AsOfDate = parameters.AsOfDate;
        PurchaseLeadTimeDays = parameters.PurchaseLeadTimeDays;
        ProductionLeadTimeDays = parameters.ProductionLeadTimeDays;
        InputHash = inputHash;
        SnapshotJson = snapshotJson;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    public void AddRecommendation(MrpRecommendation recommendation)
    {
        ArgumentNullException.ThrowIfNull(recommendation);
        _recommendations.Add(recommendation);
    }
}

public class MrpRecommendation : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid RunId { get; private set; }
    public int LineNo { get; private set; }
    public Guid ItemId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public DateOnly NeedBy { get; private set; }
    public DateOnly OrderBy { get; private set; }
    public int Level { get; private set; }
    public decimal GrossRequirement { get; private set; }
    public decimal StockUsed { get; private set; }
    public decimal ScheduledReceiptsUsed { get; private set; }
    public string ReasonsJson { get; private set; } = "[]";
    public string Status { get; private set; } = MrpRecommendationStatus.Proposed;
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? ConvertedType { get; private set; }
    public Guid? ConvertedId { get; private set; }
    public string? ConvertedNumber { get; private set; }
    public Guid RowVersion { get; private set; }

    protected MrpRecommendation() { }

    public MrpRecommendation(Guid id, Guid organizationId, Guid runId, int lineNo, MrpPlannedOrder order, string reasonsJson) : base(id)
    {
        OrganizationId = organizationId;
        RunId = runId;
        LineNo = lineNo;
        ItemId = order.ItemId;
        Action = order.Action;
        Quantity = order.Quantity;
        NeedBy = order.NeedBy;
        OrderBy = order.OrderBy;
        Level = order.Level;
        GrossRequirement = order.GrossRequirement;
        StockUsed = order.StockUsed;
        ScheduledReceiptsUsed = order.ScheduledReceiptsUsed;
        ReasonsJson = reasonsJson;
        Status = MrpRecommendationStatus.Proposed;
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Maker–checker: the run's creator cannot decide its recommendations.</summary>
    public void Decide(bool approve, Guid deciderUserId, Guid runCreatorUserId, DateTimeOffset now)
    {
        if (Status != MrpRecommendationStatus.Proposed)
        {
            throw new MrpDomainException("MRP_INVALID_STATE", $"Only a proposed recommendation can be decided; current status is '{Status}'.");
        }

        if (deciderUserId == runCreatorUserId)
        {
            throw new MrpDomainException("MRP_SELF_APPROVAL", "The creator of a planning run cannot decide its recommendations.");
        }

        if (approve && Action == MrpAction.Shortage)
        {
            throw new MrpDomainException("MRP_NOT_CONVERTIBLE", "A shortage with no sourcing route needs a planner decision; it cannot be approved for conversion.");
        }

        Status = approve ? MrpRecommendationStatus.Approved : MrpRecommendationStatus.Rejected;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void MarkConverted(string type, Guid id, string number)
    {
        if (Status != MrpRecommendationStatus.Approved)
        {
            throw new MrpDomainException("MRP_INVALID_STATE", $"Only an approved recommendation can be converted; current status is '{Status}'.");
        }

        Status = MrpRecommendationStatus.Converted;
        ConvertedType = type;
        ConvertedId = id;
        ConvertedNumber = number;
        RowVersion = Guid.NewGuid();
    }
}
