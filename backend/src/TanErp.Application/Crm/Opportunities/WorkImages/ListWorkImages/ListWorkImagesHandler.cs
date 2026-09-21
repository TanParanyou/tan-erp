using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Opportunities.WorkImages.ListWorkImages;

public class ListWorkImagesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public ListWorkImagesHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityWorkImagePageProjection>> Handle(
        ListWorkImagesQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "opportunities.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<OpportunityWorkImagePageProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var limit = query.Limit is < 1 or > 100 ? 25 : query.Limit;

        return await _store.ListWorkImagesAsync(
            access.OrganizationId,
            query.OpportunityId,
            query.Stage,
            limit,
            query.Cursor,
            cancellationToken);
    }
}
