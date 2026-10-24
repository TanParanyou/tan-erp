using System.Security.Cryptography;
using System.Text;
using TanErp.Domain.Common;

namespace TanErp.Domain.DocumentTemplates;

public class DocumentTemplate : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public string LayoutConfigJson { get; private set; } = string.Empty;
    public Guid? LogoFileId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public Guid RowVersion => ComputeRowVersion(OrganizationId, Id, UpdatedAtUtc);

    public static Guid ComputeRowVersion(Guid orgId, Guid id, DateTimeOffset updatedAtUtc)
    {
        var key = $"doctpl:{orgId}:{id}:{updatedAtUtc.ToUnixTimeMilliseconds()}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        Span<byte> guidBytes = stackalloc byte[16];
        hash.AsSpan(0, 16).CopyTo(guidBytes);
        return new Guid(guidBytes);
    }

    private DocumentTemplate() { }

    public DocumentTemplate(
        Guid id,
        Guid organizationId,
        Guid? branchId,
        string documentType,
        string name,
        bool isDefault,
        string layoutConfigJson,
        Guid? logoFileId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty) throw new ArgumentException("OrganizationId cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(documentType)) throw new ArgumentException("DocumentType cannot be empty.", nameof(documentType));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(layoutConfigJson)) throw new ArgumentException("LayoutConfigJson cannot be empty.", nameof(layoutConfigJson));

        OrganizationId = organizationId;
        BranchId = branchId;
        DocumentType = documentType.Trim().ToLowerInvariant();
        Name = name.Trim();
        IsDefault = isDefault;
        LayoutConfigJson = layoutConfigJson.Trim();
        LogoFileId = logoFileId;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public void Update(
        string name,
        bool isDefault,
        string layoutConfigJson,
        Guid? logoFileId,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(layoutConfigJson)) throw new ArgumentException("LayoutConfigJson cannot be empty.", nameof(layoutConfigJson));

        Name = name.Trim();
        IsDefault = isDefault;
        LayoutConfigJson = layoutConfigJson.Trim();
        LogoFileId = logoFileId;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetDefault(bool isDefault, DateTimeOffset updatedAtUtc)
    {
        IsDefault = isDefault;
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
