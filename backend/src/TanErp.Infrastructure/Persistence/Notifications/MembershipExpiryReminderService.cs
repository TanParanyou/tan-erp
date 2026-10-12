using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// One scan for memberships that will expire within the largest reminder threshold. Everything happens in one transaction that first
/// takes a PostgreSQL transaction-scoped advisory lock: an instance that cannot get it skips the run, and the lock is released by commit,
/// rollback or a dropped connection. Reminders go through the shared INotificationPublisher, whose dedupe key (type + deterministic
/// transition id) and unique index make a repeated scan, a restart or a second instance harmless. A single bad row is logged and skipped.
/// </summary>
public class MembershipExpiryReminderService : IMembershipExpiryReminderService
{
    /// <summary>Arbitrary but fixed: every instance must use the same number for the lock to mean anything.</summary>
    public const long AdvisoryLockKey = 2_110_000_021L;

    private readonly AppDbContext _db;
    private readonly INotificationPublisher _publisher;
    private readonly IClock _clock;
    private readonly MembershipExpiryReminderOptions _options;
    private readonly ILogger<MembershipExpiryReminderService> _logger;

    public MembershipExpiryReminderService(
        AppDbContext db, INotificationPublisher publisher, IClock clock, MembershipExpiryReminderOptions options,
        ILogger<MembershipExpiryReminderService> logger)
    {
        _db = db;
        _publisher = publisher;
        _clock = clock;
        _options = options;
        _logger = logger;
    }

    private sealed record Candidate(Guid Id, Guid OrganizationId, Guid? BranchId, Guid UserId, string DisplayName, DateTimeOffset ExpiresAtUtc);

    public async Task<MembershipExpiryReminderRunResult> RunOnceAsync(CancellationToken ct = default)
    {
        var thresholds = MembershipExpiryReminderPlanner.NormalizeThresholds(_options.ThresholdDays);
        var now = _clock.UtcNow;
        var horizon = now.AddDays(thresholds[^1]);

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var locked = await _db.Database
            .SqlQuery<bool>($"SELECT pg_try_advisory_xact_lock({AdvisoryLockKey}) AS \"Value\"")
            .SingleAsync(ct);
        if (!locked) return new MembershipExpiryReminderRunResult(0, 0, 0, true);

        // Same "usable membership" rule as NotificationMembershipFilter / RequestAccessResolver, plus: it expires inside the horizon.
        var candidates = await _db.Memberships.AsNoTracking()
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc != null && m.ExpiresAtUtc > now && m.ExpiresAtUtc <= horizon)
            .OrderBy(m => m.ExpiresAtUtc).ThenBy(m => m.Id)
            .Take(_options.EffectiveMaxMemberships)
            .Select(m => new Candidate(m.Id, m.OrganizationId, m.BranchId, m.UserId, m.User!.DisplayName, m.ExpiresAtUtc!.Value))
            .ToListAsync(ct);

        var failed = 0;
        foreach (var candidate in candidates)
        {
            var threshold = MembershipExpiryReminderPlanner.SelectThreshold(now, candidate.ExpiresAtUtc, thresholds);
            if (threshold is null) continue;

            var evt = NotificationEvents.MembershipExpiring(
                candidate.OrganizationId, candidate.BranchId,
                MembershipExpiryReminderPlanner.TransitionId(candidate.Id, threshold.Value, candidate.ExpiresAtUtc),
                candidate.Id, candidate.UserId, candidate.DisplayName, threshold.Value);
            try
            {
                await _publisher.PublishAsync(evt, ct);
            }
            catch (NotificationDomainException ex)
            {
                // A programming/data error for one membership must not block everyone else's reminder. No names or emails are logged.
                failed++;
                _logger.LogWarning(ex, "Membership expiry reminder skipped for membership {MembershipId}: {Code}", candidate.Id, ex.Code);
            }
        }

        var created = _db.ChangeTracker.Entries<Notification>().Count(e => e.State == EntityState.Added);
        try
        {
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
        {
            // Another writer stored the same (recipient, reminder) first. Nothing is lost: the next scan sees it as already sent.
            await transaction.RollbackAsync(ct);
            _db.ChangeTracker.Clear();
            return new MembershipExpiryReminderRunResult(candidates.Count, 0, failed, false);
        }

        return new MembershipExpiryReminderRunResult(candidates.Count, created, failed, false);
    }
}
