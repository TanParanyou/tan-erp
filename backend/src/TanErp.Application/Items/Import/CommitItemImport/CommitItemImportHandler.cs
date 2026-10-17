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
    string TraceId,
    string Kind = ItemImportKind.Items,
    string Mode = ItemImportMode.Create);

public class CommitItemImportHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportStore _store;
    private readonly ICostImportStore _costStore;

    public CommitItemImportHandler(IRequestAccessResolver accessResolver, IItemImportStore store, ICostImportStore costStore)
    {
        _accessResolver = accessResolver;
        _store = store;
        _costStore = costStore;
    }

    public async Task<Result<ItemImportCommitResult>> Handle(CommitItemImportCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await ItemImportAccess.ResolveAsync(
            _accessResolver, command.FirebaseUid, command.MembershipId, command.Kind, command.Mode, cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportCommitResult>.Failure(accessResult.Error);
        }

        var contentHash = Sha256Hex.Compute(command.Content ?? string.Empty);
        if (command.Kind == ItemImportKind.Costs)
        {
            var parsedCosts = CostImportCsv.Parse(command.Content);
            if (parsedCosts.IsFailure) return Result<ItemImportCommitResult>.Failure(parsedCosts.Error);
            if (ContentChanged(command.ExpectedContentSha256, contentHash)) return ContentChangedFailure();
            return await _costStore.CommitAsync(
                accessResult.Value!, parsedCosts.Value!, contentHash, command.IdempotencyKey, command.TraceId, cancellationToken);
        }

        var parsed = ItemImportCsv.Parse(command.Content);
        if (parsed.IsFailure)
        {
            return Result<ItemImportCommitResult>.Failure(parsed.Error);
        }

        if (ContentChanged(command.ExpectedContentSha256, contentHash)) return ContentChangedFailure();

        return await _store.CommitAsync(
            accessResult.Value!, parsed.Value!, command.Mode, contentHash, command.IdempotencyKey, command.TraceId, cancellationToken);
    }

    private static bool ContentChanged(string? expected, string actual) =>
        !string.IsNullOrWhiteSpace(expected) && !string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase);

    private static Result<ItemImportCommitResult> ContentChangedFailure() =>
        Result<ItemImportCommitResult>.Failure(
            new Error("ITEM_IMPORT_CONTENT_CHANGED", "The file differs from the one that was previewed."));
}
