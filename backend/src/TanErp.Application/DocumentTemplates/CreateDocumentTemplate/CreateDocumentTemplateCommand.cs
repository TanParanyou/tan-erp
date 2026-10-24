namespace TanErp.Application.DocumentTemplates.CreateDocumentTemplate;

public sealed record CreateDocumentTemplateCommand(
    string FirebaseUid,
    Guid MembershipId,
    string DocumentType,
    string Name,
    bool IsDefault,
    string LayoutConfigJson,
    Guid? LogoFileId,
    Guid? BranchId = null);
