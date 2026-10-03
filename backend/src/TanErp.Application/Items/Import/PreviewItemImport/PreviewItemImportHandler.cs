using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Items.Import.PreviewItemImport;

public sealed record PreviewItemImportCommand(string FirebaseUid, Guid MembershipId, string? Content);

public class PreviewItemImportHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportStore _store;

    public PreviewItemImportHandler(IRequestAccessResolver accessResolver, IItemImportStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ItemImportPreview>> Handle(PreviewItemImportCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "items.create", cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportPreview>.Failure(accessResult.Error);
        }

        var parsed = ItemImportCsv.Parse(command.Content);
        if (parsed.IsFailure)
        {
            return Result<ItemImportPreview>.Failure(parsed.Error);
        }

        var results = await _store.ValidateAsync(accessResult.Value!, parsed.Value!, cancellationToken);
        var valid = results.Count(r => r.IsValid);

        return Result<ItemImportPreview>.Success(new ItemImportPreview(
            Sha256Hex.Compute(command.Content!),
            results.Count,
            valid,
            results.Count - valid,
            results));
    }
}
