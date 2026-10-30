using TanErp.Domain.Common;

namespace TanErp.Domain.Mrp;

public class WorkingCalendarHoliday : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid? BranchId { get; private set; }
    public DateOnly HolidayDate { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected WorkingCalendarHoliday() { }

    public WorkingCalendarHoliday(
        Guid id,
        Guid organizationId,
        Guid? branchId,
        DateOnly holidayDate,
        string name,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization and actor are required.");
        }

        if (branchId.HasValue && branchId.Value == Guid.Empty)
        {
            branchId = null;
        }

        var trimmed = name?.Trim() ?? string.Empty;
        if (trimmed.Length == 0 || trimmed.Length > 200)
        {
            throw new MrpDomainException("MRP_HOLIDAY_INVALID", "Holiday name is required and cannot exceed 200 characters.");
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        HolidayDate = holidayDate;
        Name = trimmed;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }
}
