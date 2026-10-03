using TanErp.Domain.Common;

namespace TanErp.Domain.Service;

/// <summary>Created when a customer accepts an installation handover with a stated warranty term. Dates are fixed at creation.</summary>
public class Warranty : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid InstallationJobId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public int Months { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected Warranty() { }

    public Warranty(Guid id, Guid organizationId, Guid projectId, Guid installationJobId, string number, DateOnly startDate, int months, DateTimeOffset now) : base(id)
    {
        if (months is <= 0 or > 120) throw new ServiceDomainException("INSTALLATION_FIELD_INVALID", "Warranty months must be between 1 and 120.");
        OrganizationId = organizationId;
        ProjectId = projectId;
        InstallationJobId = installationJobId;
        Number = number;
        StartDate = startDate;
        Months = months;
        // Covers up to the day before the anniversary. When the start day does not exist in the final month (e.g. 31 Jan + 1 month),
        // the term ends on that month's last day instead of a day short.
        var anniversary = startDate.AddMonths(months);
        EndDate = anniversary.Day < startDate.Day ? anniversary : anniversary.AddDays(-1);
        CreatedAtUtc = now.ToUniversalTime();
    }

    public bool Covers(DateOnly date) => date >= StartDate && date <= EndDate;
}
