namespace TanErp.Application.Items.Import;

/// <summary>Raw (untrusted) CSV cells for one Cost Record row. Nothing is interpreted until validation.</summary>
public sealed record CostImportRowDraft(
    int RowNumber,
    string? ItemCode,
    string? Scope,
    string? BranchCode,
    string? UnitCode,
    string? Currency,
    string? Amount,
    string? MinimumQuantity,
    string? MaximumQuantity,
    string? EffectiveFromUtc,
    string? EffectiveToUtc,
    string? CostSourceCode,
    string? SourceReference,
    string? Reason);

/// <summary>A cost row whose syntax passed validation; item/branch/unit/source codes are still unresolved.</summary>
public sealed record CostImportRow(
    int RowNumber,
    string ItemCode,
    string Scope,
    string? BranchCode,
    string UnitCode,
    string Currency,
    decimal Amount,
    decimal MinimumQuantity,
    decimal? MaximumQuantity,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    string CostSourceCode,
    string SourceReference,
    string Reason);
