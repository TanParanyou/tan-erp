namespace TanErp.Application.Mrp;

public sealed record MrpPerson(Guid Id, string DisplayName, string? Email);

public sealed record MrpItemRef(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record MrpDemandRequestInput(Guid ItemId, decimal Quantity, DateOnly NeedBy, string? Reference);

public sealed record MrpRunInput(
    DateOnly AsOfDate,
    int PurchaseLeadTimeDays,
    int ProductionLeadTimeDays,
    bool IncludeOpenWorkOrders,
    IReadOnlyList<MrpDemandRequestInput> Demands);

public sealed record MrpReasonProjection(string SourceType, string SourceRef, decimal Quantity, DateOnly NeedBy);

public sealed record MrpConvertedProjection(string Type, Guid Id, string Number);

public sealed record MrpRecommendationProjection(
    Guid Id,
    int LineNo,
    MrpItemRef Item,
    string Action,
    decimal Quantity,
    DateOnly NeedBy,
    DateOnly OrderBy,
    int Level,
    decimal GrossRequirement,
    decimal StockUsed,
    decimal ScheduledReceiptsUsed,
    IReadOnlyList<MrpReasonProjection> Reasons,
    string Status,
    MrpPerson? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    MrpConvertedProjection? Converted,
    Guid RowVersion);

public sealed record MrpSnapshotSummary(int DemandCount, int SupplyCount, int BomCount, int StockItemCount);

public sealed record MrpRunProjection(
    Guid Id,
    string Number,
    DateOnly AsOfDate,
    int PurchaseLeadTimeDays,
    int ProductionLeadTimeDays,
    string InputHash,
    MrpSnapshotSummary Snapshot,
    MrpPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<MrpRecommendationProjection> Recommendations);

public sealed record MrpRunListItemProjection(
    Guid Id,
    string Number,
    DateOnly AsOfDate,
    int RecommendationCount,
    int ShortageCount,
    int OpenCount,
    DateTimeOffset CreatedAtUtc);

public sealed record MrpRunListQuery(string? Search, int Page, int PageSize);

public sealed record PagedMrpRuns(IReadOnlyList<MrpRunListItemProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record MrpConvertInput(Guid? SupplierId, decimal? UnitPrice, Guid? WarehouseId);

/// <summary>What the handler needs to convert an approved recommendation, without exposing persistence types.</summary>
public sealed record MrpConvertTarget(Guid RecommendationId, Guid RunId, string RunNumber, int LineNo, Guid ItemId, string Action, decimal Quantity, DateOnly NeedBy, string Status, Guid RowVersion);
