using TanErp.Domain.Common;

namespace TanErp.Domain.Commercial;

/// <summary>
/// The immutable PDF artifact of one quotation: which stored file, its SHA-256 and which renderer made it. There is no mutator on purpose;
/// a changed quotation is a new quotation (amendment) with its own document. The file bytes live in the Files module (UploadedFile).
/// </summary>
public class QuotationDocument : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid QuotationId { get; private set; }
    public Guid FileId { get; private set; }
    public string Sha256 { get; private set; } = string.Empty;
    public long ByteCount { get; private set; }
    public string RendererVersion { get; private set; } = string.Empty;
    public DateTimeOffset RenderedAtUtc { get; private set; }
    public Guid RenderedByUserId { get; private set; }

    protected QuotationDocument() { }

    public QuotationDocument(
        Guid id, Guid organizationId, Guid quotationId, Guid fileId, string sha256, long byteCount,
        string rendererVersion, DateTimeOffset renderedAtUtc, Guid renderedByUserId) : base(id)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || quotationId == Guid.Empty || fileId == Guid.Empty || renderedByUserId == Guid.Empty)
            throw new ArgumentException("Document, organization, quotation, file and actor identifiers are required.");
        if (sha256 is null || sha256.Length != 64 || !sha256.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'))
            throw new ArgumentException("SHA-256 must be 64 lowercase hexadecimal characters.", nameof(sha256));
        if (byteCount <= 0) throw new ArgumentOutOfRangeException(nameof(byteCount), "A document cannot be empty.");
        if (string.IsNullOrWhiteSpace(rendererVersion)) throw new ArgumentException("Renderer version is required.", nameof(rendererVersion));

        OrganizationId = organizationId;
        QuotationId = quotationId;
        FileId = fileId;
        Sha256 = sha256;
        ByteCount = byteCount;
        RendererVersion = rendererVersion.Trim();
        RenderedAtUtc = renderedAtUtc.ToUniversalTime();
        RenderedByUserId = renderedByUserId;
    }
}
