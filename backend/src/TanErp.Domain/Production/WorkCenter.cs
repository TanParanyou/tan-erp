using TanErp.Domain.Common;

namespace TanErp.Domain.Production;

public class WorkCenter : Entity
{
    public const decimal DefaultDailyCapacityHours = 8.00m;
    public const decimal MinDailyCapacityHours = 0.25m;
    public const decimal MaxDailyCapacityHours = 24.00m;

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal DailyCapacityHours { get; private set; } = DefaultDailyCapacityHours;
    public decimal HourlyRate { get; private set; }
    public string Status { get; private set; } = WorkCenterStatus.Active;
    public Guid RowVersion { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected WorkCenter() { }

    public WorkCenter(
        Guid id,
        Guid organizationId,
        Guid branchId,
        string code,
        string name,
        decimal dailyCapacityHours,
        decimal hourlyRate,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));

        var trimmedCode = code?.Trim() ?? string.Empty;
        if (trimmedCode.Length == 0 || trimmedCode.Length > 64)
        {
            throw new ProductionDomainException("PRODUCTION_WORK_CENTER_INVALID", "Work center code is required and cannot exceed 64 characters.");
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        Code = trimmedCode;
        NormalizedCode = trimmedCode.ToUpperInvariant();
        Apply(name, dailyCapacityHours, hourlyRate);
        Status = WorkCenterStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void Update(string name, decimal dailyCapacityHours, decimal hourlyRate, DateTimeOffset now)
    {
        Apply(name, dailyCapacityHours, hourlyRate);
        Touch(now);
    }

    public void Activate(DateTimeOffset now)
    {
        if (Status == WorkCenterStatus.Active)
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", "The work center is already active.");
        }

        Status = WorkCenterStatus.Active;
        Touch(now);
    }

    public void Deactivate(DateTimeOffset now)
    {
        if (Status == WorkCenterStatus.Inactive)
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", "The work center is already inactive.");
        }

        Status = WorkCenterStatus.Inactive;
        Touch(now);
    }

    public void SetActive(bool active, DateTimeOffset now)
    {
        if (active) Activate(now);
        else Deactivate(now);
    }

    private void Apply(string name, decimal dailyCapacityHours, decimal hourlyRate)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length == 0 || trimmedName.Length > 200)
        {
            throw new ProductionDomainException("PRODUCTION_WORK_CENTER_INVALID", "Work center name is required and cannot exceed 200 characters.");
        }

        if (dailyCapacityHours < MinDailyCapacityHours || dailyCapacityHours > MaxDailyCapacityHours)
        {
            throw new ProductionDomainException("PRODUCTION_WORK_CENTER_INVALID", "Daily capacity hours must be between 0.25 and 24.00.");
        }

        if (hourlyRate < 0m || hourlyRate > 999_999m)
        {
            throw new ProductionDomainException("PRODUCTION_WORK_CENTER_INVALID", "Hourly rate cannot be negative.");
        }

        Name = trimmedName;
        DailyCapacityHours = decimal.Round(dailyCapacityHours, 2);
        HourlyRate = decimal.Round(hourlyRate, 2);
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
