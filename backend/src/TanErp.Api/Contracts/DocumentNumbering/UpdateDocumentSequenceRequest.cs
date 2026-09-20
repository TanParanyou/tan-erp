using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.DocumentNumbering;

public class UpdateDocumentSequenceRequest
{
    public string Prefix { get; set; } = string.Empty;
    [Required]
    public string FormatPattern { get; set; } = string.Empty;
    [Required]
    public string ResetPeriod { get; set; } = "Yearly";
    [Range(1, 10)]
    public int Padding { get; set; } = 4;
    public bool IsBranchSpecific { get; set; }
}
