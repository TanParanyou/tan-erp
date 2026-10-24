namespace TanErp.Application.DocumentTemplates;

public sealed record DocumentTemplateProjection(
    Guid Id,
    Guid OrganizationId,
    Guid? BranchId,
    string DocumentType,
    string Name,
    bool IsDefault,
    string LayoutConfigJson,
    Guid? LogoFileId,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    Guid RowVersion);
