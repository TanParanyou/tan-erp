namespace TanErp.Api.Contracts.DocumentTemplates;

public sealed class DocumentTemplateResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? BranchId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string LayoutConfigJson { get; set; } = string.Empty;
    public Guid? LogoFileId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public Guid RowVersion { get; set; }
}

public sealed class CreateDocumentTemplateRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string LayoutConfigJson { get; set; } = string.Empty;
    public Guid? LogoFileId { get; set; }
    public Guid? BranchId { get; set; }
}

public sealed class UpdateDocumentTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string LayoutConfigJson { get; set; } = string.Empty;
    public Guid? LogoFileId { get; set; }
}
