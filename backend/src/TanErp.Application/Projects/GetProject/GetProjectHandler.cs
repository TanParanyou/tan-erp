using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Projects.GetProject;

public sealed record GetProjectQuery(string FirebaseUid, Guid MembershipId, Guid ProjectId);

public class GetProjectHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProjectStore _store;

    public GetProjectHandler(IRequestAccessResolver accessResolver, IProjectStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ProjectDetailProjection>> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(query.FirebaseUid, query.MembershipId, "projects.read", cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ProjectDetailProjection>.Failure(accessResult.Error);
        }

        var project = await _store.GetAsync(accessResult.Value!.OrganizationId, query.ProjectId, cancellationToken);
        return project is null
            ? Result<ProjectDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Project not found."))
            : Result<ProjectDetailProjection>.Success(project);
    }
}
