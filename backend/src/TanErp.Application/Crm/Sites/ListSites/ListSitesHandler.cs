using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Sites.ListSites;

public class ListSitesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteStore _store;

    public ListSitesHandler(
        IRequestAccessResolver accessResolver,
        ISiteStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<IReadOnlyList<SiteProjection>>> Handle(
        ListSitesQuery query,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required sites.read permission
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "sites.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<IReadOnlyList<SiteProjection>>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Fetch list from store (scoped to organization and customer)
        var sites = await _store.ListByCustomerAsync(
            access.OrganizationId,
            query.CustomerId,
            cancellationToken);

        if (sites == null)
        {
            return Result<IReadOnlyList<SiteProjection>>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Customer not found or access denied."));
        }

        return Result<IReadOnlyList<SiteProjection>>.Success(sites);
    }
}
