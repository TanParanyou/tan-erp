namespace TanErp.Api.Contracts.DocumentNumbering;

public class PreviewDocumentSequenceRequest
{
    public string Prefix { get; set; } = string.Empty;
    public string FormatPattern { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public int Padding { get; set; } = 4;
    public long SampleSequence { get; set; } = 1;
}
