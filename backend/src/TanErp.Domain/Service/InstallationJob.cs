using TanErp.Domain.Common;

namespace TanErp.Domain.Service;

/// <summary>An installation at a customer site for one Project, with its checklist, punch list and handover.</summary>
public class InstallationJob : Entity
{
    public const int MaxChecklistItems = 100;

    private readonly List<InstallationChecklistItem> _checklist = new();
    private readonly List<InstallationDefect> _defects = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Status { get; private set; } = InstallationStatus.Planned;
    public DateOnly ScheduledStart { get; private set; }
    public DateOnly ScheduledEnd { get; private set; }
    public string? CrewName { get; private set; }
    public string? Note { get; private set; }
    public string? CancelReason { get; private set; }
    public DateTimeOffset? ReadyAtUtc { get; private set; }
    public DateOnly? HandoverDate { get; private set; }
    public string? HandoverSignerName { get; private set; }
    public string? HandoverNote { get; private set; }
    public int DisputeCount { get; private set; }
    public string? LastDisputeNote { get; private set; }
    public int? WarrantyMonths { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    public IReadOnlyCollection<InstallationChecklistItem> Checklist => _checklist.AsReadOnly();
    public IReadOnlyCollection<InstallationDefect> Defects => _defects.AsReadOnly();

    protected InstallationJob() { }

    public InstallationJob(Guid id, Guid organizationId, Guid branchId, Guid projectId, string number, DateOnly scheduledStart, DateOnly scheduledEnd, string? crewName, string? note, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || projectId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, branch, project and actor are required.");
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number cannot be blank.", nameof(number));
        if (scheduledEnd < scheduledStart) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "The end date cannot be before the start date.");

        OrganizationId = organizationId;
        BranchId = branchId;
        ProjectId = projectId;
        Number = number.Trim();
        ScheduledStart = scheduledStart;
        ScheduledEnd = scheduledEnd;
        CrewName = Clean(crewName, 200);
        Note = Clean(note, 500);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    private static string? Clean(string? value, int max)
    {
        var trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (trimmed is not null && trimmed.Length > max) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", $"A value cannot exceed {max} characters.");
        return trimmed;
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    private void Require(params string[] statuses)
    {
        if (!statuses.Contains(Status))
        {
            throw new ServiceDomainException("INSTALLATION_INVALID_STATE", $"This action is not allowed while the installation is '{Status}'.");
        }
    }

    public void AddChecklistItem(InstallationChecklistItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_checklist.Count >= MaxChecklistItems) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", $"A checklist can have at most {MaxChecklistItems} items.");
        _checklist.Add(item);
    }

    public void AddDefectRecord(InstallationDefect defect) => _defects.Add(defect);

    public void Start(DateTimeOffset now)
    {
        Require(InstallationStatus.Planned);
        Status = InstallationStatus.InProgress;
        Touch(now);
    }

    public void SetChecklistDone(Guid itemId, bool done, Guid actorUserId, DateTimeOffset now)
    {
        Require(InstallationStatus.InProgress);
        var item = _checklist.FirstOrDefault(c => c.Id == itemId) ?? throw new ServiceDomainException("INSTALLATION_ITEM_NOT_FOUND", "Checklist item not found.");
        item.SetDone(done, actorUserId, now);
        Touch(now);
    }

    /// <summary>Reporting a defect on a job that was ready for handover sends it back to in-progress work.</summary>
    public InstallationDefect ReportDefect(string description, string severity, Guid actorUserId, DateTimeOffset now)
    {
        Require(InstallationStatus.InProgress, InstallationStatus.ReadyForHandover);
        var defect = new InstallationDefect(Guid.NewGuid(), OrganizationId, Id, _defects.Count == 0 ? 1 : _defects.Max(d => d.No) + 1, description, severity, actorUserId, now);
        _defects.Add(defect);
        if (Status == InstallationStatus.ReadyForHandover)
        {
            Status = InstallationStatus.InProgress;
            ReadyAtUtc = null;
        }

        Touch(now);
        return defect;
    }

    public void ResolveDefect(Guid defectId, string note, Guid actorUserId, DateTimeOffset now)
    {
        Require(InstallationStatus.InProgress);
        FindDefect(defectId).Resolve(note, actorUserId, now);
        Touch(now);
    }

    public void VerifyDefect(Guid defectId, Guid actorUserId, DateTimeOffset now)
    {
        Require(InstallationStatus.InProgress);
        FindDefect(defectId).Verify(actorUserId, now);
        Touch(now);
    }

    public void ReopenDefect(Guid defectId, string reason, Guid actorUserId, DateTimeOffset now)
    {
        Require(InstallationStatus.InProgress, InstallationStatus.ReadyForHandover);
        FindDefect(defectId).Reopen(reason, actorUserId, now);
        if (Status == InstallationStatus.ReadyForHandover)
        {
            Status = InstallationStatus.InProgress;
            ReadyAtUtc = null;
        }

        Touch(now);
    }

    private InstallationDefect FindDefect(Guid id) =>
        _defects.FirstOrDefault(d => d.Id == id) ?? throw new ServiceDomainException("INSTALLATION_ITEM_NOT_FOUND", "Defect not found.");

    /// <summary>Ready for handover needs every required checklist item done and every defect verified.</summary>
    public void MarkReady(DateTimeOffset now)
    {
        Require(InstallationStatus.InProgress);
        if (_checklist.Any(c => c.Required && !c.Done))
        {
            throw new ServiceDomainException("INSTALLATION_CHECKLIST_INCOMPLETE", "Every required checklist item must be done before handover.");
        }

        if (_defects.Any(d => d.Status != DefectStatus.Verified))
        {
            throw new ServiceDomainException("INSTALLATION_DEFECTS_OPEN", "Every defect must be resolved and verified before handover.");
        }

        Status = InstallationStatus.ReadyForHandover;
        ReadyAtUtc = now.ToUniversalTime();
        Touch(now);
    }

    /// <summary>
    /// Records the customer's decision. Accepted ends the installation and fixes the warranty start; a dispute returns the job to
    /// in-progress with the reason kept, and the job must be made ready again.
    /// </summary>
    public void Handover(string outcome, string signerName, string? note, int? warrantyMonths, DateOnly handoverDate, DateOnly today, DateTimeOffset now)
    {
        Require(InstallationStatus.ReadyForHandover);
        var name = string.IsNullOrWhiteSpace(signerName) ? throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "The customer signer name is required.") : signerName.Trim();
        if (name.Length > 200) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "The signer name cannot exceed 200 characters.");
        var cleanNote = Clean(note, 500);

        if (outcome == HandoverOutcome.Disputed)
        {
            if (cleanNote is null) throw new ServiceDomainException("INSTALLATION_REASON_REQUIRED", "A reason is required when the customer disputes the handover.");
            DisputeCount++;
            LastDisputeNote = cleanNote;
            Status = InstallationStatus.InProgress;
            ReadyAtUtc = null;
            Touch(now);
            return;
        }

        if (outcome != HandoverOutcome.Accepted) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "The handover outcome must be accepted or disputed.");
        if (warrantyMonths is null or < 0 or > 120) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "Warranty months (0-120) must be stated explicitly; use 0 for no warranty.");
        if (handoverDate > today) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "The handover date cannot be in the future.");

        HandoverDate = handoverDate;
        HandoverSignerName = name;
        HandoverNote = cleanNote;
        WarrantyMonths = warrantyMonths;
        Status = InstallationStatus.HandedOver;
        Touch(now);
    }

    public void Cancel(string reason, DateTimeOffset now)
    {
        Require(InstallationStatus.Planned, InstallationStatus.InProgress, InstallationStatus.ReadyForHandover);
        if (string.IsNullOrWhiteSpace(reason)) throw new ServiceDomainException("INSTALLATION_REASON_REQUIRED", "A reason is required to cancel an installation.");
        CancelReason = Clean(reason, 500);
        Status = InstallationStatus.Cancelled;
        Touch(now);
    }
}

public class InstallationChecklistItem : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid InstallationJobId { get; private set; }
    public int SortOrder { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool Required { get; private set; }
    public bool Done { get; private set; }
    public Guid? DoneByUserId { get; private set; }
    public DateTimeOffset? DoneAtUtc { get; private set; }

    protected InstallationChecklistItem() { }

    public InstallationChecklistItem(Guid id, Guid organizationId, Guid jobId, int sortOrder, string title, bool required) : base(id)
    {
        var trimmed = title?.Trim() ?? string.Empty;
        if (trimmed.Length is 0 or > 200) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "A checklist title must be 1-200 characters.");
        OrganizationId = organizationId;
        InstallationJobId = jobId;
        SortOrder = sortOrder;
        Title = trimmed;
        Required = required;
    }

    public void SetDone(bool done, Guid actorUserId, DateTimeOffset now)
    {
        Done = done;
        DoneByUserId = done ? actorUserId : null;
        DoneAtUtc = done ? now.ToUniversalTime() : null;
    }
}

public class InstallationDefect : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid InstallationJobId { get; private set; }
    public int No { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Severity { get; private set; } = DefectSeverity.Minor;
    public string Status { get; private set; } = DefectStatus.Open;
    public Guid ReportedByUserId { get; private set; }
    public DateTimeOffset ReportedAtUtc { get; private set; }
    public Guid? ResolvedByUserId { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }
    public string? ResolutionNote { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }
    public DateTimeOffset? VerifiedAtUtc { get; private set; }
    public int ReopenCount { get; private set; }
    public string? LastReopenReason { get; private set; }

    protected InstallationDefect() { }

    public InstallationDefect(Guid id, Guid organizationId, Guid jobId, int no, string description, string severity, Guid reportedBy, DateTimeOffset now) : base(id)
    {
        var text = description?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "A defect description must be 1-500 characters.");
        if (!DefectSeverity.All.Contains(severity)) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "The defect severity is invalid.");
        OrganizationId = organizationId;
        InstallationJobId = jobId;
        No = no;
        Description = text;
        Severity = severity;
        ReportedByUserId = reportedBy;
        ReportedAtUtc = now.ToUniversalTime();
    }

    public void Resolve(string note, Guid actorUserId, DateTimeOffset now)
    {
        if (Status is not (DefectStatus.Open or DefectStatus.Reopened)) throw new ServiceDomainException("INSTALLATION_DEFECT_INVALID_STATE", $"A defect in status '{Status}' cannot be resolved.");
        var text = note?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new ServiceDomainException("INSTALLATION_REASON_REQUIRED", "A resolution note of 1-500 characters is required.");
        Status = DefectStatus.Resolved;
        ResolvedByUserId = actorUserId;
        ResolvedAtUtc = now.ToUniversalTime();
        ResolutionNote = text;
        VerifiedByUserId = null;
        VerifiedAtUtc = null;
    }

    /// <summary>Maker–checker: whoever fixed a defect cannot also verify the fix.</summary>
    public void Verify(Guid actorUserId, DateTimeOffset now)
    {
        if (Status != DefectStatus.Resolved) throw new ServiceDomainException("INSTALLATION_DEFECT_INVALID_STATE", $"A defect in status '{Status}' cannot be verified.");
        if (ResolvedByUserId == actorUserId) throw new ServiceDomainException("INSTALLATION_SELF_VERIFICATION", "The person who resolved a defect cannot verify it.");
        Status = DefectStatus.Verified;
        VerifiedByUserId = actorUserId;
        VerifiedAtUtc = now.ToUniversalTime();
    }

    public void Reopen(string reason, Guid actorUserId, DateTimeOffset now)
    {
        if (Status is not (DefectStatus.Resolved or DefectStatus.Verified)) throw new ServiceDomainException("INSTALLATION_DEFECT_INVALID_STATE", $"A defect in status '{Status}' cannot be reopened.");
        var text = reason?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new ServiceDomainException("INSTALLATION_REASON_REQUIRED", "A reason of 1-500 characters is required to reopen a defect.");
        Status = DefectStatus.Reopened;
        ReopenCount++;
        LastReopenReason = text;
        VerifiedByUserId = null;
        VerifiedAtUtc = null;
    }
}
