namespace TanErp.Application.DocumentNumbering;

public record DocumentSequenceProjection(
    Guid? Id,
    string DocumentType,
    string Prefix,
    string FormatPattern,
    string ResetPeriod,
    int Padding,
    bool IsBranchSpecific,
    bool IsActive,
    string SamplePreview);
