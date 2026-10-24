namespace TanErp.Application.DocumentTemplates.ListDocumentTemplates;

public sealed record ListDocumentTemplatesQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? DocumentType = null,
    Guid? BranchId = null);
