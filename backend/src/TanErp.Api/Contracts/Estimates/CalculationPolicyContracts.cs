using System.ComponentModel.DataAnnotations;
using TanErp.Domain.Estimates;

namespace TanErp.Api.Contracts.Estimates;

public sealed record CalculationPolicyVersionResponse(
    Guid Id,
    Guid OrganizationId,
    Guid? BranchId,
    string PolicyCode,
    int Version,
    string Status,
    string OverheadMethod,
    decimal OverheadValue,
    string RoundingMode,
    string UnitConversionMode,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    Guid CreatedByUserId,
    Guid? PublishedByUserId,
    DateTimeOffset? PublishedAtUtc,
    string? ContentHash)
{
    public static CalculationPolicyVersionResponse From(CalculationPolicyVersion p) =>
        new(
            p.Id,
            p.OrganizationId,
            p.BranchId,
            p.PolicyCode,
            p.Version,
            p.Status,
            p.OverheadMethod,
            p.OverheadValue,
            p.RoundingMode,
            p.UnitConversionMode,
            p.EffectiveFromUtc,
            p.EffectiveToUtc,
            p.CreatedByUserId,
            p.PublishedByUserId,
            p.PublishedAtUtc,
            p.ContentHash);
}

public sealed record CalculationPolicyListResponse(
    IReadOnlyList<CalculationPolicyVersionResponse> Items);

public sealed record CreateCalculationPolicyRequest
{
    public Guid? BranchId { get; init; }

    [Required]
    [StringLength(64, MinimumLength = 1)]
    public string PolicyCode { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Version { get; init; }

    [Required]
    public string OverheadMethod { get; init; } = "none";

    [Range(0, double.MaxValue)]
    public decimal OverheadValue { get; init; }

    [Required]
    public string RoundingMode { get; init; } = "away-from-zero";

    [Required]
    public string UnitConversionMode { get; init; } = UnitConversionModes.None;

    public DateTimeOffset EffectiveFromUtc { get; init; }

    public DateTimeOffset? EffectiveToUtc { get; init; }
}
