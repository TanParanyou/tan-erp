using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Items.Import.CreateItemImportJob;

public sealed record CreateItemImportJobCommand(
    string FirebaseUid,
    Guid MembershipId,
    string Kind,
    string Mode,
    string? Content,
    string? ExpectedContentSha256,
    string IdempotencyKey,
    string TraceId);

/// <summary>
/// Accepts a large file and queues it. Only the file-level syntax (header, size, row count) is checked here, so the caller gets an
/// immediate 422 for a broken file; row-level problems are found per batch by the worker and reported on the job.
/// </summary>
public class CreateItemImportJobHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportJobStore _jobStore;
    private readonly ItemImportJobOptions _options;

    public CreateItemImportJobHandler(IRequestAccessResolver accessResolver, IItemImportJobStore jobStore, ItemImportJobOptions options)
    {
        _accessResolver = accessResolver;
        _jobStore = jobStore;
        _options = options;
    }

    public async Task<Result<ItemImportJobSnapshot>> Handle(CreateItemImportJobCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await ItemImportAccess.ResolveAsync(
            _accessResolver, command.FirebaseUid, command.MembershipId, command.Kind, command.Mode, cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportJobSnapshot>.Failure(accessResult.Error);
        }

        int totalRows;
        if (command.Kind == ItemImportKind.Costs)
        {
            var parsed = CostImportCsv.Parse(command.Content, ItemImportLimits.MaxJobRows, ItemImportLimits.MaxJobContentLength);
            if (parsed.IsFailure) return Result<ItemImportJobSnapshot>.Failure(parsed.Error);
            totalRows = parsed.Value!.Count;
        }
        else
        {
            var parsed = ItemImportCsv.Parse(command.Content, ItemImportLimits.MaxJobRows, ItemImportLimits.MaxJobContentLength);
            if (parsed.IsFailure) return Result<ItemImportJobSnapshot>.Failure(parsed.Error);
            totalRows = parsed.Value!.Count;
        }

        var contentHash = Sha256Hex.Compute(command.Content!);
        if (!string.IsNullOrWhiteSpace(command.ExpectedContentSha256)
            && !string.Equals(command.ExpectedContentSha256, contentHash, StringComparison.OrdinalIgnoreCase))
        {
            return Result<ItemImportJobSnapshot>.Failure(
                new Error("ITEM_IMPORT_CONTENT_CHANGED", "The file differs from the one that was previewed."));
        }

        return await _jobStore.EnqueueAsync(
            accessResult.Value!,
            new EnqueueItemImportJobData(
                command.Kind, command.Mode, command.Content!, contentHash, totalRows, _options.EffectiveBatchSize,
                command.FirebaseUid, command.IdempotencyKey, command.TraceId),
            cancellationToken);
    }
}
