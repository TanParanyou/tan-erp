namespace TanErp.Application.DocumentTemplates.UpdateDocumentTemplate;

public sealed record UpdateDocumentTemplateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid IfMatchRowVersion,
    Guid TemplateId,
    string Name,
    bool IsDefault,
    string LayoutConfigJson,
    Guid? LogoFileId);
