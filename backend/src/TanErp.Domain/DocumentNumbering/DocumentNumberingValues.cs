namespace TanErp.Domain.DocumentNumbering;

public enum ResetPeriod
{
    Never = 0,
    Yearly = 1,
    Monthly = 2,
    Daily = 3
}

public static class DocumentTypes
{
    public const string Estimates = "estimates";
    public const string Surveys = "surveys";
    public const string Opportunities = "opportunities";
    public const string Quotations = "quotations";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Estimates,
        Surveys,
        Opportunities,
        Quotations
    };

    public static bool IsValid(string documentType) =>
        All.Contains(documentType, StringComparer.OrdinalIgnoreCase);
}

public static class DocumentNumberTokens
{
    public const string Prefix = "{PREFIX}";
    public const string Branch = "{BRANCH}";
    public const string YearAD = "{YYYY}";
    public const string ShortYearAD = "{YY}";
    public const string YearBE = "{BBBB}";
    public const string ShortYearBE = "{BB}";
    public const string Month = "{MM}";
    public const string Day = "{DD}";
    public const string Sequence = "{SEQ:";
}
