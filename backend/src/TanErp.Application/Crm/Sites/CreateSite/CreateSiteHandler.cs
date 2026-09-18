using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Crm.Sites;

namespace TanErp.Application.Crm.Sites.CreateSite;

public class CreateSiteHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteStore _store;

    public CreateSiteHandler(
        IRequestAccessResolver accessResolver,
        ISiteStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteProjection>> Handle(
        CreateSiteCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required sites.manage permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "sites.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Validate required fields
        if (string.IsNullOrWhiteSpace(command.Label))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Site label is required."));
        if (string.IsNullOrWhiteSpace(command.AddressLine1))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Address line 1 is required."));
        if (string.IsNullOrWhiteSpace(command.Subdistrict))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Subdistrict is required."));
        if (string.IsNullOrWhiteSpace(command.District))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "District is required."));
        if (string.IsNullOrWhiteSpace(command.Province))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Province is required."));
        if (string.IsNullOrWhiteSpace(command.PostalCode))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Postal code is required."));
        if (string.IsNullOrWhiteSpace(command.CountryCode))
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Country code is required."));

        // Validate coordinate pairing & ranges
        if ((command.Latitude.HasValue && !command.Longitude.HasValue) || (!command.Latitude.HasValue && command.Longitude.HasValue))
        {
            return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Latitude and longitude must both be provided or both be empty."));
        }

        if (command.Latitude.HasValue)
        {
            if (command.Latitude.Value < -90m || command.Latitude.Value > 90m)
                return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Latitude must be between -90 and 90."));
            if (command.Longitude!.Value < -180m || command.Longitude.Value > 180m)
                return Result<SiteProjection>.Failure(new Error("SITE_FIELD_REQUIRED", "Longitude must be between -180 and 180."));
        }

        if (command.Images != null)
        {
            foreach (var img in command.Images)
            {
                if (img.FileId == Guid.Empty)
                    return Result<SiteProjection>.Failure(new Error("SITE_IMAGE_INVALID", "Image File ID cannot be empty."));
                if (img.Caption != null && img.Caption.Length > 500)
                    return Result<SiteProjection>.Failure(new Error("SITE_IMAGE_INVALID", "Image caption cannot exceed 500 characters."));
            }
        }

        // 3. Compute key hash and canonical payload hash
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var normLabel = SiteNormalizer.CollapseWhitespace(command.Label);
        var normAddr = SiteNormalizer.CollapseWhitespace(command.AddressLine1);
        var normSub = SiteNormalizer.CollapseWhitespace(command.Subdistrict);
        var normDist = SiteNormalizer.CollapseWhitespace(command.District);
        var normProv = SiteNormalizer.CollapseWhitespace(command.Province);
        var normPost = SiteNormalizer.CollapseWhitespace(command.PostalCode);
        var normCountry = command.CountryCode.Trim().ToUpperInvariant();
        var latStr = command.Latitude?.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) ?? "";
        var lngStr = command.Longitude?.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) ?? "";
        var normNote = command.AccessNote != null ? SiteNormalizer.CollapseWhitespace(command.AccessNote) : "";
        var imagesStr = command.Images != null && command.Images.Count > 0
            ? string.Join(";", command.Images.Select(i => $"{i.FileId}:{i.Caption?.Trim() ?? ""}"))
            : "";

        var canonicalPayload = $"{command.CustomerId}|{normLabel}|{normAddr}|{normSub}|{normDist}|{normProv}|{normPost}|{normCountry}|{latStr}|{lngStr}|{normNote}|{imagesStr}";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        // 4. Delegate to atomic store
        return await _store.CreateAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
