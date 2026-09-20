namespace TanErp.Application.DocumentNumbering.UpdateDocumentSequence;

public record UpdateDocumentSequenceCommand(
    string FirebaseUid,
    Guid MembershipId,
    string DocumentType,
    string Prefix,
    string FormatPattern,
    string ResetPeriod,
    int Padding,
    bool IsBranchSpecific);
