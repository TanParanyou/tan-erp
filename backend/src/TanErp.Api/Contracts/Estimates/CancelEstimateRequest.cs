using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record CancelEstimateRequest([Required, MaxLength(2000)] string Reason);
