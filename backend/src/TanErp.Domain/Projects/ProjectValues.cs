namespace TanErp.Domain.Projects;

public static class ProjectStatus
{
    public const string Planned = "planned";
    public const string Active = "active";
    public const string OnHold = "on_hold";
    public const string ReadyForHandover = "ready_for_handover";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Planned, Active, OnHold, ReadyForHandover, Completed, Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public class ProjectDomainException : Exception
{
    public string Code { get; }

    public ProjectDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}
