using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Sites;

namespace TanErp.Application.Crm.Sites;

public sealed class ManageCustomerSiteHandler(IRequestAccessResolver accessResolver, ISiteStore store)
{
    public async Task<Result<SiteProjection>> UpdateAsync(string uid, Guid membershipId, Guid customerId, Guid siteId, Guid expectedRowVersion, UpdateSiteData data, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "sites.manage", ct);
        if (access.IsFailure) return Result<SiteProjection>.Failure(access.Error);
        if (string.IsNullOrWhiteSpace(data.Label) || string.IsNullOrWhiteSpace(data.AddressLine1))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Required site fields are missing."));
        if (!AddressLocationValidator.IsValid(data.Subdistrict, data.District, data.Province, data.PostalCode, data.CountryCode) || data.AccessNote?.Length > 1000)
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Site address or access note is invalid."));
        if ((data.Latitude.HasValue != data.Longitude.HasValue) || data.Latitude is < -90 or > 90 || data.Longitude is < -180 or > 180)
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Site coordinates are invalid."));
        return await store.UpdateAsync(access.Value!, customerId, siteId, expectedRowVersion, data, traceId, ct);
    }

    public async Task<Result<SiteProjection>> DeactivateAsync(string uid, Guid membershipId, Guid customerId, Guid siteId, Guid expectedRowVersion, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "sites.manage", ct);
        return access.IsFailure ? Result<SiteProjection>.Failure(access.Error) : await store.DeactivateAsync(access.Value!, customerId, siteId, expectedRowVersion, traceId, ct);
    }
}
