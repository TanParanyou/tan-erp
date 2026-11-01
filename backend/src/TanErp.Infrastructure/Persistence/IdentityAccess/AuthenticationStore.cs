using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.IdentityAccess.Authentication;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public sealed class AuthenticationStore : IAuthenticationStore
{
    private readonly AppDbContext _db;

    public AuthenticationStore(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> FindUserByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<User?> FindUserByGoogleSubjectIdAsync(string googleSubjectId, CancellationToken cancellationToken = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.GoogleSubjectId == googleSubjectId, cancellationToken);

    public Task<User?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<User?> FindUserBySubjectUidAsync(string subjectUid, CancellationToken cancellationToken = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == subjectUid, cancellationToken);

    public Task<int> CountRemainingBackupCodesAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _db.UserBackupCodes.AsNoTracking()
            .CountAsync(c => c.UserId == userId && c.UsedAtUtc == null, cancellationToken);

    public async Task ReplaceBackupCodesAsync(
        Guid userId,
        IReadOnlyCollection<UserBackupCode> backupCodes,
        CancellationToken cancellationToken = default)
    {
        var existing = await _db.UserBackupCodes
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            _db.UserBackupCodes.RemoveRange(existing);
        }

        if (backupCodes.Count > 0)
        {
            _db.UserBackupCodes.AddRange(backupCodes);
        }
    }

    public async Task DeleteAllBackupCodesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await _db.UserBackupCodes
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            _db.UserBackupCodes.RemoveRange(existing);
        }
    }

    public async Task<bool> TryConsumeBackupCodeAsync(
        Guid userId,
        string codeHash,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var row = await _db.UserBackupCodes
            .FirstOrDefaultAsync(
                c => c.UserId == userId && c.UsedAtUtc == null && c.CodeHash == codeHash,
                cancellationToken);
        if (row is null)
        {
            return false;
        }

        return row.TryMarkUsed(usedAtUtc);
    }

    public void AddSession(UserSession session)
    {
        _db.UserSessions.Add(session);
    }

    public Task<UserSession?> FindSessionWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        _db.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

    public async Task<IReadOnlyList<UserSession>> ListActiveSessionsAsync(
        Guid userId,
        DateTimeOffset now,
        CancellationToken cancellationToken = default) =>
        await _db.UserSessions.AsNoTracking()
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null && s.ExpiresAtUtc > now)
            .OrderByDescending(s => s.LastUsedAtUtc)
            .ThenByDescending(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<UserSession?> FindUserSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken = default) =>
        _db.UserSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, cancellationToken);

    public async Task<int> RevokeOtherActiveSessionsAsync(
        Guid userId,
        Guid? excludeSessionId,
        string reason,
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
    {
        var query = _db.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null && s.ExpiresAtUtc > now);

        if (excludeSessionId.HasValue)
        {
            var excluded = excludeSessionId.Value;
            query = query.Where(s => s.Id != excluded);
        }

        var activeSessions = await query.ToListAsync(cancellationToken);
        foreach (var session in activeSessions)
        {
            session.Revoke(reason, now);
        }

        return activeSessions.Count;
    }

    public async Task RecordGoogleLinkedAuditAsync(
        Guid userId,
        string traceId,
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
    {
        var organizationIds = await _db.Memberships.AsNoTracking()
            .Where(m => m.UserId == userId)
            .Select(m => m.OrganizationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var changesJson = JsonSerializer.Serialize(new { provider = "google" });
        foreach (var organizationId in organizationIds)
        {
            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                userId,
                "users.google-linked",
                "User",
                userId.ToString(),
                now,
                traceId,
                changesJson));
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
