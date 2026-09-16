namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record AttachWorkImageItemRequest(
    Guid FileId,
    string? Caption);

public sealed record AttachWorkImagesRequest(
    IReadOnlyList<AttachWorkImageItemRequest> Images);

public sealed record WorkImageUserSummaryResponse(
    Guid Id,
    string DisplayName);

public sealed record OpportunityWorkImageResponse(
    Guid Id,
    Guid FileId,
    string StageAtAttach,
    string? Caption,
    int DisplayOrder,
    DateTimeOffset CreatedAtUtc,
    WorkImageUserSummaryResponse CreatedBy);

public sealed record AttachWorkImagesResponse(
    IReadOnlyList<OpportunityWorkImageResponse> Items,
    Guid OpportunityRowVersion);

public sealed record OpportunityWorkImageListResponse(
    IReadOnlyList<OpportunityWorkImageResponse> Items);
