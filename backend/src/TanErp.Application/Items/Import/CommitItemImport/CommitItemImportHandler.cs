using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Items.Import.CommitItemImport;

public sealed record CommitItemImportCommand(
    string FirebaseUid,
    Guid MembershipId,
    string? Content,
    string? ExpectedContentSha256,
    string IdempotencyKey,
    string TraceId);

public class CommitItemImportHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportStore _store;

    public CommitItemImportHandler(IRequestAccessResolver accessResolver, IItemImportStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ItemImportCommitResult>> Handle(CommitItemImportCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "items.create", cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportCommitResult>.Failure(accessResult.Error);
        }

        var parsed = ItemImportCsv.Parse(command.Content);
        if (parsed.IsFailure)
        {
            return Result<ItemImportCommitResult>.Failure(parsed.Error);
        }

        var contentHash = Sha256Hex.Compute(command.Content!);
        if (!string.IsNullOrWhiteSpace(command.ExpectedContentSha256)
            && !string.Equals(command.ExpectedContentSha256, contentHash, StringComparison.OrdinalIgnoreCase))
        {
            return Result<ItemImportCommitResult>.Failure(
                new Error("ITEM_IMPORT_CONTENT_CHANGED", "The file differs from the one that was previewed."));
        }

        return await _store.CommitAsync(
            accessResult.Value!, parsed.Value!, contentHash, command.IdempotencyKey, command.TraceId, cancellationToken);
    }
}
