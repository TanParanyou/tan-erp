namespace TanErp.Domain.DocumentNumbering;

public class DocumentSequenceCounter
{
    public Guid OrganizationId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public string PeriodKey { get; private set; } = string.Empty;
    public long CurrentValue { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private DocumentSequenceCounter() { }

    public DocumentSequenceCounter(
        Guid organizationId,
        string documentType,
        Guid branchId,
        string periodKey,
        long currentValue,
        DateTimeOffset updatedAtUtc)
    {
        OrganizationId = organizationId;
        DocumentType = documentType.Trim().ToLowerInvariant();
        BranchId = branchId;
        PeriodKey = string.IsNullOrWhiteSpace(periodKey) ? "ALL" : periodKey.Trim();
        CurrentValue = currentValue;
        UpdatedAtUtc = updatedAtUtc;
    }
}
