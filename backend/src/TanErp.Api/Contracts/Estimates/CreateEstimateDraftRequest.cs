using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record CreateEstimateDraftRequest(
    [Required] Guid CustomerId,
    [Required] Guid OpportunityId,
    [Required] Guid BranchId,
    Guid? SiteSurveyRevisionId,
    string? SiteSurveySnapshotHash,
    string? Currency);
