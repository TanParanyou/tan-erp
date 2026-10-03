using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record UpdateEstimateCostComponentDto(
    Guid? Id,
    [Required] string Type,
    [Required] string Description,
    [Range(0.0001, 999999999)] decimal Quantity,
    [Required] string UnitCode,
    [Range(0, 999999999)] decimal UnitCost,
    string? Currency,
    int SortOrder = 1,
    Guid? ItemId = null,
    Guid? CostRecordId = null,
    int? CostRecordVersion = null,
    [MaxLength(64)] string? ProvisionalReasonCode = null,
    [MaxLength(1000)] string? ProvisionalNote = null);

public sealed record UpdateEstimateWorkItemDto(
    Guid? Id,
    [Required] string Code,
    [Required] string DescriptionTh,
    string? DescriptionEn,
    [Range(0.0001, 999999999)] decimal Quantity,
    [Required] string UnitCode,
    [Required] string SellingRuleType,
    decimal SellingRuleValue,
    int SortOrder = 1,
    IReadOnlyList<UpdateEstimateCostComponentDto>? CostComponents = null,
    [MaxLength(64)] string? SellingRuleReasonCode = null,
    Guid? ItemId = null,
    [MaxLength(64)] string? OverrideReasonCode = null,
    [MaxLength(500)] string? OverrideReason = null);

public sealed record UpdateEstimateSectionDto(
    Guid? Id,
    [Required] string Code,
    [Required] string NameTh,
    string? NameEn,
    int SortOrder = 1,
    IReadOnlyList<UpdateEstimateWorkItemDto>? WorkItems = null);

public sealed record UpdateEstimateDraftRequest(
    [Required] Guid ExpectedRevisionVersion,
    [Required] IReadOnlyList<UpdateEstimateSectionDto> Sections);
