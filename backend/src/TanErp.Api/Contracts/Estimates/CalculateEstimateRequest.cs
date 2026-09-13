using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record CalculateEstimateRequest(
    [Required] Guid ExpectedRevisionVersion,
    decimal DiscountAmount = 0);
