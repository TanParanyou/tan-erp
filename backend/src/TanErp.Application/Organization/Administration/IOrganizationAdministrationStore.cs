using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Organization.Administration;

public interface IOrganizationAdministrationStore
{
    Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct);
    Task<Result<OrganizationProfile>> UpdateProfileAsync(
        Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);
}
