namespace TanErp.Domain.Mrp;

public class MrpDomainException : Exception
{
    public string Code { get; }

    public MrpDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class MrpAction
{
    public const string Buy = "buy";
    public const string Make = "make";
    /// <summary>Short item that has neither an approved BOM nor a purchasable flag; a planner must decide.</summary>
    public const string Shortage = "shortage";
}

public static class MrpRecommendationStatus
{
    public const string Proposed = "proposed";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Converted = "converted";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Proposed, Approved, Rejected, Converted };
}

public static class MrpDemandSource
{
    public const string Manual = "manual";
    public const string WorkOrder = "work_order";
    public const string Dependent = "dependent";
}
