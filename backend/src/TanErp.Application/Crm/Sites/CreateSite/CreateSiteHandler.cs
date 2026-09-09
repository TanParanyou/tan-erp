using System.Security.Cryptography;
using System.Text;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
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

        // 3. Compute key hash and canonical payload hash
        var keyHash = ComputeSha256Hex(command.IdempotencyKey);
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

        var canonicalPayload = $"{command.CustomerId}|{normLabel}|{normAddr}|{normSub}|{normDist}|{normProv}|{normPost}|{normCountry}|{latStr}|{lngStr}|{normNote}";
        var payloadHash = ComputeSha256Hex(canonicalPayload);

        // 4. Delegate to atomic store
        return await _store.CreateAsync(access, command, keyHash, payloadHash, cancellationToken);
    }

    private static string ComputeSha256Hex(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
