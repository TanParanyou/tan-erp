using TanErp.Domain.Common;

namespace TanErp.Domain.DocumentNumbering;

public class DocumentSequenceDefinition : Entity
{
    public Guid OrganizationId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string Prefix { get; private set; } = string.Empty;
    public string FormatPattern { get; private set; } = string.Empty;
    public ResetPeriod ResetPeriod { get; private set; }
    public int Padding { get; private set; } = 4;
    public bool IsBranchSpecific { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public Guid RowVersion => ComputeRowVersion(OrganizationId, DocumentType, UpdatedAtUtc);

    public static Guid ComputeRowVersion(Guid orgId, string docType, DateTimeOffset updatedAtUtc)
    {
        var key = $"docseq:{orgId}:{docType.Trim().ToLowerInvariant()}:{updatedAtUtc.ToUnixTimeMilliseconds()}";
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key));
        Span<byte> guidBytes = stackalloc byte[16];
        hash.AsSpan(0, 16).CopyTo(guidBytes);
        return new Guid(guidBytes);
    }

    public static Guid ComputeDefaultRowVersion(Guid orgId, string docType)
    {
        var key = $"docseq:{orgId}:{docType.Trim().ToLowerInvariant()}:default";
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key));
        Span<byte> guidBytes = stackalloc byte[16];
        hash.AsSpan(0, 16).CopyTo(guidBytes);
        return new Guid(guidBytes);
    }

    private DocumentSequenceDefinition() { }

    public DocumentSequenceDefinition(
        Guid id,
        Guid organizationId,
        string documentType,
        string prefix,
        string formatPattern,
        ResetPeriod resetPeriod,
        int padding,
        bool isBranchSpecific,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty) throw new ArgumentException("OrganizationId cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(documentType)) throw new ArgumentException("DocumentType cannot be empty.", nameof(documentType));
        if (string.IsNullOrWhiteSpace(formatPattern)) throw new ArgumentException("FormatPattern cannot be empty.", nameof(formatPattern));
        if (padding < 1 || padding > 10) throw new ArgumentException("Padding must be between 1 and 10.", nameof(padding));

        OrganizationId = organizationId;
        DocumentType = documentType.Trim().ToLowerInvariant();
        Prefix = prefix?.Trim() ?? string.Empty;
        FormatPattern = formatPattern.Trim();
        ResetPeriod = resetPeriod;
        Padding = padding;
        IsBranchSpecific = isBranchSpecific;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public void UpdateFormat(
        string prefix,
        string formatPattern,
        ResetPeriod resetPeriod,
        int padding,
        bool isBranchSpecific,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(formatPattern)) throw new ArgumentException("FormatPattern cannot be empty.", nameof(formatPattern));
        if (padding < 1 || padding > 10) throw new ArgumentException("Padding must be between 1 and 10.", nameof(padding));

        Prefix = prefix?.Trim() ?? string.Empty;
        FormatPattern = formatPattern.Trim();
        ResetPeriod = resetPeriod;
        Padding = padding;
        IsBranchSpecific = isBranchSpecific;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Activate(DateTimeOffset updatedAtUtc)
    {
        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTimeOffset updatedAtUtc)
    {
        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }
}
