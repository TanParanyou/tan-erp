using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Projects;

namespace TanErp.Application.Projects.ListProjects;

public sealed record ListProjectsQuery(string FirebaseUid, Guid MembershipId, string? Search, string? Status, int Page, int PageSize);

public class ListProjectsHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProjectStore _store;

    public ListProjectsHandler(IRequestAccessResolver accessResolver, IProjectStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<PagedProjectsProjection>> Handle(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(query.FirebaseUid, query.MembershipId, "projects.read", cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<PagedProjectsProjection>.Failure(accessResult.Error);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && !ProjectStatus.IsValid(query.Status))
        {
            return Result<PagedProjectsProjection>.Failure(new Error("PROJECT_FIELD_INVALID", "Project status filter is invalid."));
        }

        var page = Math.Max(1, query.Page);
        var pageSize = query.PageSize <= 0 ? DefaultPageSize : Math.Min(query.PageSize, MaxPageSize);
        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
        var status = string.IsNullOrWhiteSpace(query.Status) ? null : query.Status.Trim();

        return Result<PagedProjectsProjection>.Success(await _store.ListAsync(
            accessResult.Value!.OrganizationId, new ProjectListQuery(search, status, page, pageSize), cancellationToken));
    }
}
