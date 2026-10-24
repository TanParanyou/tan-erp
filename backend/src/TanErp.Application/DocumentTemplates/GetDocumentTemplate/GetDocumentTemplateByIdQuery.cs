namespace TanErp.Application.DocumentTemplates.GetDocumentTemplate;

public sealed record GetDocumentTemplateByIdQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid TemplateId);
