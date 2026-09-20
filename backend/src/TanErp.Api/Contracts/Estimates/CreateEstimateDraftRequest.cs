using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record CreateEstimateDraftRequest(
    [Required] Guid OpportunityId,
    [Required] Guid SiteSurveyRevisionId,
    string? Currency);
