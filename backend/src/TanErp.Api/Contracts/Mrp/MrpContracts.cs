namespace TanErp.Api.Contracts.Mrp;

public sealed record MrpDemandRequest(Guid ItemId, decimal Quantity, DateOnly NeedBy, string? Reference);

public sealed record MrpRunRequest(
    DateOnly AsOfDate,
    int PurchaseLeadTimeDays,
    int ProductionLeadTimeDays,
    bool IncludeOpenWorkOrders,
    List<MrpDemandRequest>? Demands);

public sealed record MrpConvertRequest(Guid? SupplierId, decimal? UnitPrice, Guid? WarehouseId);

public sealed record MrpPersonResponse(Guid Id, string DisplayName, string? Email);

public sealed record MrpItemResponse(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record MrpPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record MrpReasonResponse(string SourceType, string SourceRef, decimal Quantity, DateOnly NeedBy);

public sealed record MrpConvertedResponse(string Type, Guid Id, string Number);

public sealed record MrpRecommendationResponse(
    Guid Id,
    int LineNo,
    MrpItemResponse Item,
    string Action,
    decimal Quantity,
    DateOnly NeedBy,
    DateOnly OrderBy,
    int Level,
    decimal GrossRequirement,
    decimal StockUsed,
    decimal ScheduledReceiptsUsed,
    IReadOnlyList<MrpReasonResponse> Reasons,
    string Status,
    MrpPersonResponse? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    MrpConvertedResponse? Converted,
    Guid RowVersion);

public sealed record MrpSnapshotSummaryResponse(int DemandCount, int SupplyCount, int BomCount, int StockItemCount);

public sealed record MrpRunResponse(
    Guid Id,
    string Number,
    DateOnly AsOfDate,
    int PurchaseLeadTimeDays,
    int ProductionLeadTimeDays,
    string InputHash,
    MrpSnapshotSummaryResponse Snapshot,
    MrpPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<MrpRecommendationResponse> Recommendations);

public sealed record MrpRunListItemResponse(Guid Id, string Number, DateOnly AsOfDate, int RecommendationCount, int ShortageCount, int OpenCount, DateTimeOffset CreatedAtUtc);

public sealed record MrpRunListResponse(IReadOnlyList<MrpRunListItemResponse> Items, MrpPaginationResponse Pagination);
