using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence;

public class FirebaseIdentityLinker : IFirebaseIdentityLinker
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly ILogger<FirebaseIdentityLinker> _logger;

    public FirebaseIdentityLinker(AppDbContext db, IClock clock, ILogger<FirebaseIdentityLinker> logger)
    {
        _db = db;
        _clock = clock;
        _logger = logger;
    }

    public async Task<bool> TryLinkAsync(string firebaseUid, string verifiedEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid) || string.IsNullOrWhiteSpace(verifiedEmail))
        {
            return false;
        }

        // A UID that is already known never links again (no re-linking, no takeover of another user).
        if (await _db.Users.AsNoTracking().AnyAsync(u => u.FirebaseUid == firebaseUid, cancellationToken))
        {
            return false;
        }

        var normalizedEmail = User.NormalizeEmail(verifiedEmail);
        var pending = await _db.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.FirebaseUid == null && u.IsActive, cancellationToken);
        if (pending is null)
        {
            return false;
        }

        pending.LinkFirebaseIdentity(firebaseUid);

        var now = _clock.UtcNow;
        var organizationIds = await _db.Memberships.AsNoTracking()
            .Where(m => m.UserId == pending.Id)
            .Select(m => m.OrganizationId)
            .Distinct()
            .ToListAsync(cancellationToken);
        foreach (var organizationId in organizationIds)
        {
            // Audit stores identifiers only; the email is not copied into the audit trail.
            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                pending.Id,
                "users.identity-linked",
                "User",
                pending.Id.ToString(),
                now,
                traceId: string.Empty,
                changesJson: JsonSerializer.Serialize(new { linked = true })));
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
        {
            // Lost a race with another request that linked the same user or UID; the caller falls back to normal lookup.
            _logger.LogWarning(ex, "Linking Firebase identity to pending user {UserId} failed.", pending.Id);
            _db.ChangeTracker.Clear();
            return false;
        }
    }
}
