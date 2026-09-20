namespace TanErp.Application.DocumentNumbering.PreviewDocumentSequence;

public record PreviewDocumentSequenceQuery(
    string FirebaseUid,
    Guid MembershipId,
    string Prefix,
    string FormatPattern,
    string? BranchCode,
    int Padding,
    long SampleSequence);
