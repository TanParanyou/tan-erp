using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Application.Crm.Opportunities.QualifyOpportunity;

using TanErp.Application.Crm.Opportunities.UpdateDraftQGate;

namespace TanErp.Application.Crm.Opportunities;

public sealed record OpportunityListFilter(
    string? Search,
    Guid? CustomerId,
    string? Stage,
    string? SortBy = null,
    string? SortOrder = null,
    int? Page = null,
    int Limit = 25,
    string? Cursor = null);

public sealed record OpportunityPage(
    IReadOnlyList<OpportunityProjection> Items,
    string? NextCursor,
    int TotalCount = 0,
    int Page = 1,
    int PageSize = 25);

public interface IOpportunityStore
{
    Task<Result<OpportunityProjection>> CreateAsync(
        RequestAccessContext access,
        CreateOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<OpportunityProjection>> QualifyAsync(
        RequestAccessContext access,
        QualifyOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<OpportunityProjection>> UpdateDraftQGateAsync(
        RequestAccessContext access,
        UpdateDraftQGateCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<OpportunityProjection>> UpdateOpenAsync(
        RequestAccessContext access,
        UpdateOpenOpportunity.UpdateOpenOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<OpportunityProjection>> ReassignOwnerAsync(
        RequestAccessContext access,
        ReassignOpportunityOwner.ReassignOpportunityOwnerCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<OpportunityPage> ListAsync(
        Guid organizationId,
        OpportunityListFilter filter,
        CancellationToken cancellationToken = default);

    Task<OpportunityProjection?> GetAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpportunityStageHistoryProjection>> GetStageHistoryAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default);
}
