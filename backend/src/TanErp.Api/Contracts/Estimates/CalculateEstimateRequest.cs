using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record CalculateEstimateRequest(
    [Required] Guid ExpectedRevisionVersion,
    decimal DiscountAmount = 0,
    string? DiscountType = null,
    decimal? DiscountValue = null,
    [MaxLength(64)] string? DiscountReasonCode = null);
