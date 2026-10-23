using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Commercial;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;

namespace TanErp.Infrastructure.Persistence.Commercial;

public class AcceptanceOtpStore : IAcceptanceOtpStore
{
    private readonly AppDbContext _db;

    public AcceptanceOtpStore(AppDbContext db)
    {
        _db = db;
    }

    // Audit rows carry the challenge id and channel only, never the code, its hash or the recipient address.
    private void Audit(AcceptanceContext context, string action, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), context.OrganizationId, context.CreatedByUserId, action, "QuotationAcceptanceLink", context.LinkId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes)));
    }

    public async Task<OtpSendStats> GetSendStatsAsync(Guid linkId, DateTimeOffset since, CancellationToken ct = default)
    {
        var times = await _db.AcceptanceOtpChallenges.AsNoTracking().Where(c => c.LinkId == linkId && c.CreatedAtUtc > since).Select(c => c.CreatedAtUtc).ToListAsync(ct);
        return new OtpSendStats(times.Count, times.Count == 0 ? null : times.Max());
    }

    public async Task<Guid> CreateChallengeAsync(AcceptanceContext context, string channel, string codeHash, DateTimeOffset now, DateTimeOffset expiresAtUtc, string traceId, CancellationToken ct = default)
    {
        var challenge = new AcceptanceOtpChallenge(Guid.NewGuid(), context.OrganizationId, context.LinkId, channel, codeHash, now, expiresAtUtc);
        _db.AcceptanceOtpChallenges.Add(challenge);
        Audit(context, "quotation-acceptance-link.otp-issued", traceId, new { challengeId = challenge.Id, channel }, now);
        await _db.SaveChangesAsync(ct);
        return challenge.Id;
    }

    public async Task<OtpChallengeRecord?> FindActiveChallengeAsync(Guid linkId, DateTimeOffset now, CancellationToken ct = default) =>
        await _db.AcceptanceOtpChallenges.AsNoTracking()
            .Where(c => c.LinkId == linkId && c.VerifiedAtUtc == null && c.ConsumedAtUtc == null && c.ExpiresAtUtc > now)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Select(c => new OtpChallengeRecord(c.Id, c.CodeHash, c.FailedAttempts))
            .FirstOrDefaultAsync(ct);

    public async Task<bool> RegisterFailureAsync(AcceptanceContext context, Guid challengeId, int maxAttempts, DateTimeOffset now, string traceId, CancellationToken ct = default)
    {
        // Atomic increment so parallel guesses cannot slip past the limit.
        await _db.AcceptanceOtpChallenges.Where(c => c.Id == challengeId).ExecuteUpdateAsync(u => u.SetProperty(c => c.FailedAttempts, c => c.FailedAttempts + 1), ct);
        var attempts = await _db.AcceptanceOtpChallenges.AsNoTracking().Where(c => c.Id == challengeId).Select(c => c.FailedAttempts).FirstAsync(ct);
        if (attempts < maxAttempts) return false;

        var locked = await _db.QuotationAcceptanceLinks.Where(l => l.Id == context.LinkId && l.OtpLockedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(l => l.OtpLockedAtUtc, now), ct);
        if (locked > 0)
        {
            Audit(context, "quotation-acceptance-link.otp-locked", traceId, new { challengeId }, now);
            await _db.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task MarkVerifiedAsync(AcceptanceContext context, Guid challengeId, string verificationTokenHash, DateTimeOffset now, string traceId, CancellationToken ct = default)
    {
        var challenge = await _db.AcceptanceOtpChallenges.FirstAsync(c => c.Id == challengeId && c.LinkId == context.LinkId, ct);
        challenge.MarkVerified(verificationTokenHash, now);
        Audit(context, "quotation-acceptance-link.otp-verified", traceId, new { challengeId, channel = challenge.Channel }, now);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<OtpVerificationRecord?> FindVerificationAsync(Guid linkId, string verificationTokenHash, CancellationToken ct = default) =>
        await _db.AcceptanceOtpChallenges.AsNoTracking()
            .Where(c => c.LinkId == linkId && c.VerificationTokenHash == verificationTokenHash && c.VerifiedAtUtc != null && c.ConsumedAtUtc == null)
            .Select(c => new OtpVerificationRecord(c.Id, c.VerifiedAtUtc!.Value, c.Channel))
            .FirstOrDefaultAsync(ct);
}
