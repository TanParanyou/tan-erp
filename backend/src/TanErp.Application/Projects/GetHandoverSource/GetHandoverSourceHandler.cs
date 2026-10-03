using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Projects.GetHandoverSource;

public sealed record GetHandoverSourceQuery(string FirebaseUid, Guid MembershipId, Guid OpportunityId);

public class GetHandoverSourceHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProjectStore _store;

    public GetHandoverSourceHandler(IRequestAccessResolver accessResolver, IProjectStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ProjectHandoverSourceProjection>> Handle(GetHandoverSourceQuery query, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(query.FirebaseUid, query.MembershipId, "projects.read", cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ProjectHandoverSourceProjection>.Failure(accessResult.Error);
        }

        var source = await _store.GetHandoverSourceAsync(accessResult.Value!.OrganizationId, query.OpportunityId, cancellationToken);
        return source is null
            ? Result<ProjectHandoverSourceProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "No accepted quotation found for this opportunity."))
            : Result<ProjectHandoverSourceProjection>.Success(source);
    }
}
