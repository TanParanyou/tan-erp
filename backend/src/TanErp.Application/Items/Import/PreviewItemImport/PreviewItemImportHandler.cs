using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Items.Import.PreviewItemImport;

public sealed record PreviewItemImportCommand(
    string FirebaseUid,
    Guid MembershipId,
    string? Content,
    string Kind = ItemImportKind.Items,
    string Mode = ItemImportMode.Create);

public class PreviewItemImportHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportStore _store;
    private readonly ICostImportStore _costStore;

    public PreviewItemImportHandler(IRequestAccessResolver accessResolver, IItemImportStore store, ICostImportStore costStore)
    {
        _accessResolver = accessResolver;
        _store = store;
        _costStore = costStore;
    }

    public async Task<Result<ItemImportPreview>> Handle(PreviewItemImportCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await ItemImportAccess.ResolveAsync(
            _accessResolver, command.FirebaseUid, command.MembershipId, command.Kind, command.Mode, cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportPreview>.Failure(accessResult.Error);
        }

        IReadOnlyList<ItemImportRowResult> results;
        if (command.Kind == ItemImportKind.Costs)
        {
            var parsed = CostImportCsv.Parse(command.Content);
            if (parsed.IsFailure) return Result<ItemImportPreview>.Failure(parsed.Error);
            results = await _costStore.ValidateAsync(accessResult.Value!, parsed.Value!, cancellationToken);
        }
        else
        {
            var parsed = ItemImportCsv.Parse(command.Content);
            if (parsed.IsFailure) return Result<ItemImportPreview>.Failure(parsed.Error);
            results = await _store.ValidateAsync(accessResult.Value!, parsed.Value!, command.Mode, cancellationToken);
        }

        var valid = results.Count(r => r.IsValid);
        return Result<ItemImportPreview>.Success(new ItemImportPreview(
            Sha256Hex.Compute(command.Content!),
            results.Count,
            valid,
            results.Count - valid,
            results,
            results.Count(r => r.Outcome == ItemImportOutcomes.Created),
            results.Count(r => r.Outcome == ItemImportOutcomes.Updated),
            results.Count(r => r.Outcome == ItemImportOutcomes.Unchanged)));
    }
}
