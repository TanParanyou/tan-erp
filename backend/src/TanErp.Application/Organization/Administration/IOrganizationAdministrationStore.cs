using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Organization.Administration;

public interface IOrganizationAdministrationStore
{
    Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct);
    Task<Result<OrganizationProfile>> UpdateProfileAsync(
        Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);
    Task<IReadOnlyList<BranchDetail>> ListBranchesAsync(Guid organizationId, BranchStatusFilter filter, CancellationToken ct);
    Task<Result<BranchDetail>> GetBranchAsync(Guid organizationId, Guid branchId, CancellationToken ct);
    Task<Result<BranchDetail>> CreateBranchAsync(
        Guid organizationId, CreateBranchInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct);
    Task<Result<BranchDetail>> UpdateBranchAsync(
        Guid organizationId, Guid branchId, BranchInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);
}
