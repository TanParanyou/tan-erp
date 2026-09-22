using System.ComponentModel.DataAnnotations;
using TanErp.Application.Items;

namespace TanErp.Api.Contracts.Items;

public sealed record CreateCostRecordRequest
{
    [Required]
    public string Scope { get; init; } = "organization";

    public Guid? BranchId { get; init; }

    [Required]
    public Guid UnitId { get; init; }

    [Required]
    [MaxLength(3)]
    public string Currency { get; init; } = "THB";

    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Amount { get; init; }

    public decimal MinimumQuantity { get; init; } = 0m;

    public decimal? MaximumQuantity { get; init; }

    [Required]
    public DateTimeOffset EffectiveFromUtc { get; init; }

    public DateTimeOffset? EffectiveToUtc { get; init; }

    public Guid? CostSourceId { get; init; }

    [MaxLength(128)]
    public string? SourceReference { get; init; }

    [MaxLength(500)]
    public string? Reason { get; init; }

    public Guid? EvidenceFileId { get; init; }
}

public sealed record UpdateCostRecordRequest
{
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Amount { get; init; }

    [Required]
    [MaxLength(3)]
    public string Currency { get; init; } = "THB";

    [Required]
    public Guid UnitId { get; init; }

    public decimal MinimumQuantity { get; init; } = 0m;

    public decimal? MaximumQuantity { get; init; }

    [Required]
    public DateTimeOffset EffectiveFromUtc { get; init; }

    public DateTimeOffset? EffectiveToUtc { get; init; }

    public Guid? CostSourceId { get; init; }

    [MaxLength(128)]
    public string? SourceReference { get; init; }

    [MaxLength(500)]
    public string? Reason { get; init; }

    public Guid? EvidenceFileId { get; init; }
}

public sealed record ReturnCostRecordRequest
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;
}

public sealed record DisableCostRecordRequest
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;
}

public sealed record CostRecordResponse(
    Guid Id,
    Guid OrganizationId,
    Guid ItemId,
    string Scope,
    Guid? BranchId,
    Guid UnitId,
    LocalizedTextDto UnitName,
    string Currency,
    decimal Amount,
    decimal MinimumQuantity,
    decimal? MaximumQuantity,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    string Status,
    int Version,
    Guid? CostSourceId,
    LocalizedTextDto? CostSourceName,
    string? SourceReference,
    string? Reason,
    Guid? EvidenceFileId,
    Guid CreatedByUserId,
    Guid LastFinancialEditorId,
    Guid? ApprovedByUserId,
    Guid? PublishedByUserId,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
