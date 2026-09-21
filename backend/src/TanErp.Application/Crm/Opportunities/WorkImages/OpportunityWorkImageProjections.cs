namespace TanErp.Application.Crm.Opportunities.WorkImages;

public sealed record WorkImageUserSummaryProjection(
    Guid Id,
    string DisplayName);

public sealed record OpportunityWorkImageProjection(
    Guid Id,
    Guid FileId,
    string StageAtAttach,
    string? Caption,
    int DisplayOrder,
    DateTimeOffset CreatedAtUtc,
    WorkImageUserSummaryProjection CreatedBy);

public sealed record AttachWorkImagesResultProjection(
    IReadOnlyList<OpportunityWorkImageProjection> Items,
    Guid OpportunityRowVersion);

public sealed record OpportunityWorkImagePageProjection(
    IReadOnlyList<OpportunityWorkImageProjection> Items,
    string? NextCursor);
