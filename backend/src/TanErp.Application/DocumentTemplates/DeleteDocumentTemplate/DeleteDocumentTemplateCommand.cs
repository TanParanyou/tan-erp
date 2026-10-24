namespace TanErp.Application.DocumentTemplates.DeleteDocumentTemplate;

public sealed record DeleteDocumentTemplateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid TemplateId);
