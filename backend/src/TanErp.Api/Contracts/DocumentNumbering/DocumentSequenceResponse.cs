namespace TanErp.Api.Contracts.DocumentNumbering;

public class DocumentSequenceResponse
{
    public Guid? Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string FormatPattern { get; set; } = string.Empty;
    public string ResetPeriod { get; set; } = string.Empty;
    public int Padding { get; set; }
    public bool IsBranchSpecific { get; set; }
    public bool IsActive { get; set; }
    public string SamplePreview { get; set; } = string.Empty;
    public Guid RowVersion { get; set; }
}
