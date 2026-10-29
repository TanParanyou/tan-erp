using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>
/// A daily labor timesheet entry logged against an active project (and optionally a WBS node and/or a production Work Order).
/// Once approved by an independent checker, the entry is immutable and posts actual labor cost to the project cost ledger.
/// </summary>
public class LaborTimesheetEntry : Entity
{
    public const decimal MinEntryHours = 0.25m;
    public const decimal MaxDailyHours = 24.00m;
    public const decimal StandardDailyHours = 8.00m;

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? WbsNodeId { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Guid WorkerUserId { get; private set; }
    public DateOnly WorkDate { get; private set; }
    public decimal Hours { get; private set; }
    public decimal HourlyRate { get; private set; }
    public decimal CostAmount { get; private set; }
    public string TaskDescription { get; private set; } = string.Empty;
    public string Status { get; private set; } = TimesheetStatus.Draft;
    public bool IsOvertimeWarning { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }
    public Guid RowVersion { get; private set; }

    protected LaborTimesheetEntry() { }

    public LaborTimesheetEntry(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid projectId,
        Guid? wbsNodeId,
        Guid? workOrderId,
        string number,
        Guid workerUserId,
        DateOnly workDate,
        decimal hours,
        decimal hourlyRate,
        decimal existingDayHours,
        string taskDescription,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
        if (workerUserId == Guid.Empty) throw new ArgumentException("Worker user ID cannot be empty.", nameof(workerUserId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Timesheet number cannot be blank.", nameof(number));

        var roundedHours = decimal.Round(hours, 2);
        if (roundedHours < MinEntryHours || roundedHours > MaxDailyHours)
        {
            throw new ProjectDomainException("TIMESHEET_HOURS_INVALID", $"Hours must be between {MinEntryHours:0.00} and {MaxDailyHours:0.00}.");
        }

        var roundedExisting = decimal.Round(existingDayHours, 2);
        if (roundedExisting < 0m || roundedExisting + roundedHours > MaxDailyHours)
        {
            throw new ProjectDomainException("TIMESHEET_DAILY_HOURS_EXCEEDED", $"Total hours for a worker on {workDate:yyyy-MM-dd} cannot exceed {MaxDailyHours:0.00}.");
        }

        var roundedRate = decimal.Round(hourlyRate, 2);
        if (roundedRate <= 0m || roundedRate > LaborRate.MaxHourlyRate)
        {
            throw new ProjectDomainException("LABOR_RATE_INVALID", "Hourly rate must be greater than 0.");
        }

        var trimmedTask = taskDescription?.Trim() ?? string.Empty;
        if (trimmedTask.Length == 0 || trimmedTask.Length > 500)
        {
            throw new ProjectDomainException("TIMESHEET_FIELD_INVALID", "Task description is required and cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        ProjectId = projectId;
        WbsNodeId = wbsNodeId;
        WorkOrderId = workOrderId;
        Number = number.Trim();
        WorkerUserId = workerUserId;
        WorkDate = workDate;
        Hours = roundedHours;
        HourlyRate = roundedRate;
        CostAmount = decimal.Round(roundedHours * roundedRate, 2);
        TaskDescription = trimmedTask;
        Status = TimesheetStatus.Draft;
        IsOvertimeWarning = roundedExisting + roundedHours > StandardDailyHours;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void Submit(DateTimeOffset now)
    {
        if (Status == TimesheetStatus.Approved)
        {
            throw new ProjectDomainException("TIMESHEET_IMMUTABLE", "An approved timesheet cannot be modified.");
        }

        if (Status != TimesheetStatus.Draft)
        {
            throw new ProjectDomainException("TIMESHEET_INVALID_STATE", $"Only a draft timesheet can be submitted; current status is '{Status}'.");
        }

        Status = TimesheetStatus.Submitted;
        SubmittedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Approve or reject. The decider must differ from both the creator and the worker (maker-checker).</summary>
    public void Decide(bool approve, Guid deciderUserId, string? note, DateTimeOffset now)
    {
        if (Status == TimesheetStatus.Approved)
        {
            throw new ProjectDomainException("TIMESHEET_IMMUTABLE", "An approved timesheet is immutable.");
        }

        if (Status != TimesheetStatus.Submitted)
        {
            throw new ProjectDomainException("TIMESHEET_INVALID_STATE", $"Only a submitted timesheet can be decided; current status is '{Status}'.");
        }

        if (deciderUserId == CreatedByUserId || deciderUserId == WorkerUserId)
        {
            throw new ProjectDomainException("TIMESHEET_SELF_APPROVAL", "A timesheet cannot be decided by its creator or worker.");
        }

        if (!approve && string.IsNullOrWhiteSpace(note))
        {
            throw new ProjectDomainException("PROJECT_REASON_REQUIRED", "A note is required when rejecting a timesheet.");
        }

        Status = approve ? TimesheetStatus.Approved : TimesheetStatus.Rejected;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
        DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RowVersion = Guid.NewGuid();
    }

    public void Cancel()
    {
        if (Status == TimesheetStatus.Approved)
        {
            throw new ProjectDomainException("TIMESHEET_IMMUTABLE", "An approved timesheet cannot be cancelled.");
        }

        if (Status is not (TimesheetStatus.Draft or TimesheetStatus.Submitted))
        {
            throw new ProjectDomainException("TIMESHEET_INVALID_STATE", $"A '{Status}' timesheet cannot be cancelled.");
        }

        Status = TimesheetStatus.Cancelled;
        RowVersion = Guid.NewGuid();
    }
}
