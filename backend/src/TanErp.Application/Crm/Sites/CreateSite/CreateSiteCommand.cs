namespace TanErp.Application.Crm.Sites.CreateSite;

public sealed record CreateSiteImageInput(
    Guid FileId,
    string? Caption = null);

public sealed record CreateSiteCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid CustomerId,
    string IdempotencyKey,
    string Label,
    string AddressLine1,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode,
    string CountryCode,
    decimal? Latitude,
    decimal? Longitude,
    string? AccessNote,
    string TraceId,
    IReadOnlyList<CreateSiteImageInput>? Images = null,
    Guid? FileUploadIntentId = null);
