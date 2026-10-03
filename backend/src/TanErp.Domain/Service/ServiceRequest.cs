using TanErp.Domain.Common;

namespace TanErp.Domain.Service;

/// <summary>After-sales request for a Project. It records whether a warranty covered the report date, but never blocks the request.</summary>
public class ServiceRequest : Entity
{
    private readonly List<ServiceRequestEvent> _events = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? WarrantyId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Priority { get; private set; } = ServicePriority.Normal;
    public string Status { get; private set; } = ServiceRequestStatus.Open;
    public bool InWarranty { get; private set; }
    public DateOnly? ScheduledDate { get; private set; }
    public string? ResolutionNote { get; private set; }
    public int ReopenCount { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    public IReadOnlyCollection<ServiceRequestEvent> Events => _events.AsReadOnly();

    protected ServiceRequest() { }

    public ServiceRequest(Guid id, Guid organizationId, Guid branchId, Guid projectId, Guid? warrantyId, string number, string title, string description, string priority, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        var t = title?.Trim() ?? string.Empty;
        var d = description?.Trim() ?? string.Empty;
        if (t.Length is 0 or > 200 || d.Length is 0 or > 1000) throw new ServiceDomainException("SERVICE_FIELD_INVALID", "A title (1-200) and description (1-1000 characters) are required.");
        if (!ServicePriority.All.Contains(priority)) throw new ServiceDomainException("SERVICE_FIELD_INVALID", "The priority is invalid.");

        OrganizationId = organizationId;
        BranchId = branchId;
        ProjectId = projectId;
        WarrantyId = warrantyId;
        InWarranty = warrantyId.HasValue;
        Number = number;
        Title = t;
        Description = d;
        Priority = priority;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
        _events.Add(new ServiceRequestEvent(Guid.NewGuid(), organizationId, Id, null, ServiceRequestStatus.Open, null, createdByUserId, now));
    }

    private void Move(string to, string? note, Guid actor, DateTimeOffset now)
    {
        _events.Add(new ServiceRequestEvent(Guid.NewGuid(), OrganizationId, Id, Status, to, string.IsNullOrWhiteSpace(note) ? null : note.Trim(), actor, now));
        Status = to;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    private void Require(string action, params string[] statuses)
    {
        if (!statuses.Contains(Status)) throw new ServiceDomainException("SERVICE_INVALID_STATE", $"Cannot {action} a request that is '{Status}'.");
    }

    private static string RequireNote(string? note)
    {
        var text = note?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new ServiceDomainException("SERVICE_REASON_REQUIRED", "A note of 1-500 characters is required.");
        return text;
    }

    public void Schedule(DateOnly date, DateOnly today, Guid actor, DateTimeOffset now)
    {
        Require("schedule", ServiceRequestStatus.Open, ServiceRequestStatus.Scheduled);
        if (date < today) throw new ServiceDomainException("SERVICE_FIELD_INVALID", "The visit date cannot be in the past.");
        ScheduledDate = date;
        Move(ServiceRequestStatus.Scheduled, $"Scheduled for {date:yyyy-MM-dd}", actor, now);
    }

    public void Start(Guid actor, DateTimeOffset now)
    {
        Require("start", ServiceRequestStatus.Open, ServiceRequestStatus.Scheduled);
        Move(ServiceRequestStatus.InProgress, null, actor, now);
    }

    public void Resolve(string note, Guid actor, DateTimeOffset now)
    {
        Require("resolve", ServiceRequestStatus.InProgress);
        ResolutionNote = RequireNote(note);
        Move(ServiceRequestStatus.Resolved, ResolutionNote, actor, now);
    }

    public void Close(Guid actor, DateTimeOffset now)
    {
        Require("close", ServiceRequestStatus.Resolved);
        Move(ServiceRequestStatus.Closed, null, actor, now);
    }

    public void Reopen(string reason, Guid actor, DateTimeOffset now)
    {
        Require("reopen", ServiceRequestStatus.Resolved, ServiceRequestStatus.Closed);
        var text = RequireNote(reason);
        ReopenCount++;
        Move(ServiceRequestStatus.InProgress, text, actor, now);
    }
}

public class ServiceRequestEvent : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ServiceRequestId { get; private set; }
    public string? FromStatus { get; private set; }
    public string ToStatus { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public Guid ActorUserId { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }

    protected ServiceRequestEvent() { }

    public ServiceRequestEvent(Guid id, Guid organizationId, Guid requestId, string? from, string to, string? note, Guid actor, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        ServiceRequestId = requestId;
        FromStatus = from;
        ToStatus = to;
        Note = note;
        ActorUserId = actor;
        OccurredAtUtc = now.ToUniversalTime();
    }
}
