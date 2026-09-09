using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Sites.CreateSite;

namespace TanErp.Application.Crm.Sites;

public interface ISiteStore
{
    Task<Result<SiteProjection>> CreateAsync(
        RequestAccessContext access,
        CreateSiteCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SiteProjection>?> ListByCustomerAsync(
        Guid organizationId,
        Guid customerId,
        CancellationToken cancellationToken = default);
}
