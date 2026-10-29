using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>
/// Organization-scoped standard hourly labor rate for a worker used to cost labor timesheets.
/// </summary>
public class LaborRate : Entity
{
    public const decimal DefaultTestOnlyHourlyRate = 150.00m;
    public const decimal MaxHourlyRate = 100_000m;

    public Guid OrganizationId { get; private set; }
    public Guid WorkerUserId { get; private set; }
    public decimal HourlyRate { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public Guid RowVersion { get; private set; }

    protected LaborRate() { }

    public LaborRate(
        Guid id,
        Guid organizationId,
        Guid workerUserId,
        decimal hourlyRate,
        Guid actorUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (workerUserId == Guid.Empty) throw new ArgumentException("Worker user ID cannot be empty.", nameof(workerUserId));
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor user ID cannot be empty.", nameof(actorUserId));

        OrganizationId = organizationId;
        WorkerUserId = workerUserId;
        Update(hourlyRate, actorUserId, now);
    }

    public void Update(decimal hourlyRate, Guid actorUserId, DateTimeOffset now)
    {
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor user ID cannot be empty.", nameof(actorUserId));
        var rounded = decimal.Round(hourlyRate, 2);
        if (rounded <= 0m || rounded > MaxHourlyRate)
        {
            throw new ProjectDomainException("LABOR_RATE_INVALID", $"Hourly rate must be greater than 0 and at most {MaxHourlyRate:0.00}.");
        }

        HourlyRate = rounded;
        UpdatedByUserId = actorUserId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }
}
