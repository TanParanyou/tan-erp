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

    Task<Result<SiteProjection>> UpdateAsync(RequestAccessContext access, Guid customerId, Guid siteId, Guid expectedRowVersion, UpdateSiteData data, string traceId, CancellationToken cancellationToken = default);
    Task<Result<SiteProjection>> DeactivateAsync(RequestAccessContext access, Guid customerId, Guid siteId, Guid expectedRowVersion, string traceId, CancellationToken cancellationToken = default);
}

public sealed record UpdateSiteData(string Label, string AddressLine1, string Subdistrict, string District, string Province, string PostalCode, string CountryCode, decimal? Latitude, decimal? Longitude, string? AccessNote);
