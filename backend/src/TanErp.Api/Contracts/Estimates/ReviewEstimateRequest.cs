using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record ReviewEstimateRequest(
    [Range(1, int.MaxValue)] int RevisionNo,
    [Required] string Decision,
    [MaxLength(64)] string? ReasonCode = null,
    [MaxLength(2000)] string? Note = null);
