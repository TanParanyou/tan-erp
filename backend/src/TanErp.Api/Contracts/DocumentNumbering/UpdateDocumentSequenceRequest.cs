namespace TanErp.Api.Contracts.DocumentNumbering;

public class UpdateDocumentSequenceRequest
{
    public string Prefix { get; set; } = string.Empty;
    public string FormatPattern { get; set; } = string.Empty;
    public string ResetPeriod { get; set; } = "Yearly";
    public int Padding { get; set; } = 4;
    public bool IsBranchSpecific { get; set; }
}
