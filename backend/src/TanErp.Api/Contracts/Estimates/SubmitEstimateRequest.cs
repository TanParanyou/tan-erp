using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record SubmitEstimateRequest(
    [Range(1, int.MaxValue)] int RevisionNo,
    [Range(1, int.MaxValue)] int CalculationVersion,
    [MaxLength(2000)] string? Note = null);
