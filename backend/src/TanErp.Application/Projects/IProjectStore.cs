using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Projects.CreateProjectFromHandover;

namespace TanErp.Application.Projects;

public interface IProjectStore
{
    Task<Result<ProjectDetailProjection>> CreateFromHandoverAsync(
        RequestAccessContext access,
        CreateProjectFromHandoverCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<ProjectDetailProjection?> GetAsync(
        Guid organizationId,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<PagedProjectsProjection> ListAsync(
        Guid organizationId,
        ProjectListQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>The accepted quotation of an opportunity, plus the project already created from it (if any).</summary>
    Task<ProjectHandoverSourceProjection?> GetHandoverSourceAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default);
}
