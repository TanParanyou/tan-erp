namespace TanErp.Domain.Estimates;

public sealed record EstimateReadinessReason(
    string Code,
    string TargetType,
    Guid? TargetId,
    string TargetField);

public sealed record EstimateReadinessResult(
    string Status,
    IReadOnlyList<EstimateReadinessReason> Reasons);

public static class EstimateReadinessStatus
{
    public const string Blocked = "blocked";
    public const string RequiresAttention = "requiresAttention";
    public const string Ready = "ready";
}
