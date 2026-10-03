namespace TanErp.Domain.Service;

public class ServiceDomainException : Exception
{
    public string Code { get; }

    public ServiceDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class InstallationStatus
{
    public const string Planned = "planned";
    public const string InProgress = "in_progress";
    public const string ReadyForHandover = "ready_for_handover";
    public const string HandedOver = "handed_over";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Planned, InProgress, ReadyForHandover, HandedOver, Cancelled };

    public static bool IsTerminal(string status) => status is HandedOver or Cancelled;
}

public static class DefectSeverity
{
    public const string Minor = "minor";
    public const string Major = "major";
    public const string Critical = "critical";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Minor, Major, Critical };
}

public static class DefectStatus
{
    public const string Open = "open";
    public const string Resolved = "resolved";
    public const string Verified = "verified";
    public const string Reopened = "reopened";
}

public static class HandoverOutcome
{
    public const string Accepted = "accepted";
    public const string Disputed = "disputed";
}

public static class ServiceRequestStatus
{
    public const string Open = "open";
    public const string Scheduled = "scheduled";
    public const string InProgress = "in_progress";
    public const string Resolved = "resolved";
    public const string Closed = "closed";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Open, Scheduled, InProgress, Resolved, Closed };
}

public static class ServicePriority
{
    public const string Low = "low";
    public const string Normal = "normal";
    public const string High = "high";
    public const string Urgent = "urgent";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Low, Normal, High, Urgent };
}
