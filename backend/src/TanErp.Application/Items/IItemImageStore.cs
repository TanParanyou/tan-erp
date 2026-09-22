using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

public sealed record AttachItemImageData(
    Guid ItemId,
    Guid FileId,
    string Role,
    bool IsPrimary,
    LocalizedTextDto AltText,
    LocalizedTextDto? Caption);

public sealed record ItemImageDetailProjection(
    Guid Id,
    Guid OrganizationId,
    Guid ItemId,
    Guid FileId,
    string Role,
    bool IsPrimary,
    int DisplayOrder,
    LocalizedTextDto AltText,
    LocalizedTextDto? Caption,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    Guid CreatedByUserId,
    string FileName,
    string ContentType,
    long ByteSize,
    int? Width,
    int? Height);

public interface IItemImageStore
{
    Task<Result<ItemImageDetailProjection>> AttachImageAsync(
        AttachItemImageData data,
        RequestAccessContext access,
        CancellationToken ct);

    Task<IReadOnlyList<ItemImageDetailProjection>> ListImagesAsync(
        Guid organizationId,
        Guid itemId,
        CancellationToken ct);

    Task<Result<ItemImageDetailProjection>> SetPrimaryImageAsync(
        Guid organizationId,
        Guid itemId,
        Guid imageId,
        RequestAccessContext access,
        CancellationToken ct);

    Task<Result<IReadOnlyList<ItemImageDetailProjection>>> ReorderImagesAsync(
        Guid organizationId,
        Guid itemId,
        IReadOnlyList<Guid> orderedImageIds,
        RequestAccessContext access,
        CancellationToken ct);

    Task<Result<bool>> DetachImageAsync(
        Guid organizationId,
        Guid itemId,
        Guid imageId,
        RequestAccessContext access,
        CancellationToken ct);
}
