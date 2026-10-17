namespace TanErp.Application.Items.Import;

/// <summary>Worker settings (config section ItemImportJobs). The batch size is the commit unit; see ADR 0026.</summary>
public sealed class ItemImportJobOptions
{
    public const string SectionName = "ItemImportJobs";

    public bool Enabled { get; set; } = true;
    public int BatchSize { get; set; } = ItemImportLimits.DefaultBatchSize;
    public int PollIntervalSeconds { get; set; } = 5;
    public int LeaseMinutes { get; set; } = 10;

    public int EffectiveBatchSize => Math.Clamp(BatchSize, ItemImportLimits.MinBatchSize, ItemImportLimits.MaxBatchSize);
    public TimeSpan EffectivePollInterval => TimeSpan.FromSeconds(Math.Clamp(PollIntervalSeconds, 1, 300));
    public TimeSpan EffectiveLease => TimeSpan.FromMinutes(Math.Clamp(LeaseMinutes, 1, 120));
}

public sealed record ItemImportJobSnapshot(
    Guid Id,
    string Kind,
    string Mode,
    string Status,
    string ContentSha256,
    int TotalRows,
    int ProcessedRows,
    int BatchSize,
    int TotalBatches,
    int CompletedBatches,
    int RejectedBatches,
    int CreatedCount,
    int UpdatedCount,
    int UnchangedCount,
    int ErrorRowCount,
    bool ErrorsTruncated,
    IReadOnlyList<ItemImportRowResult> Errors,
    string? FailureCode,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    bool Replayed);
