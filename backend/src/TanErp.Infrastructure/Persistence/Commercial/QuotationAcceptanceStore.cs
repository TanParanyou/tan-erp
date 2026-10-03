using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Commercial;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;

namespace TanErp.Infrastructure.Persistence.Commercial;

public class QuotationAcceptanceStore : IQuotationAcceptanceStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public QuotationAcceptanceStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private void Audit(Guid orgId, Guid actorUserId, Guid? membershipId, Guid? branchId, string action, Guid resourceId, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), orgId, actorUserId, action, "QuotationAcceptanceLink", resourceId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: branchId,
            actorMembershipId: membershipId));
    }

    private async Task<AcceptanceLinkProjection> ProjectAsync(QuotationAcceptanceLink link, CancellationToken ct)
    {
        var quotation = await _db.Quotations.AsNoTracking().FirstAsync(q => q.Id == link.QuotationId, ct);
        var creator = await _db.Users.AsNoTracking().Where(u => u.Id == link.CreatedByUserId).Select(u => u.DisplayName).FirstOrDefaultAsync(ct);
        var evidence = await _db.QuotationAcceptanceEvidences.AsNoTracking().FirstOrDefaultAsync(e => e.LinkId == link.Id, ct);
        var now = _clock.UtcNow;
        return new AcceptanceLinkProjection(
            link.Id, link.QuotationId, quotation.Number, link.Status, quotation.Status,
            link.IsUsable(now) && quotation.Status == QuotationStatus.Issued, link.SignerHint, link.ExpiresAtUtc, link.CreatedAtUtc,
            new QuotationPersonRef(link.CreatedByUserId, creator ?? string.Empty), link.RevokedAtUtc, link.AcceptedAtUtc,
            evidence is null ? null : Summarize(evidence));
    }

    private static AcceptanceEvidenceSummary Summarize(QuotationAcceptanceEvidence e) =>
        new(e.SignerName, e.SignerRole, e.ConsentVersion, !string.IsNullOrEmpty(e.SignatureImage), e.SignatureHash, e.AcceptedAtUtc);

    public async Task<Result<AcceptanceLinkProjection>> CreateLinkAsync(
        RequestAccessContext access, Guid quotationId, string tokenHash, string? signerHint, int lifetimeDays, string traceId, CancellationToken ct = default)
    {
        var quotation = await _db.Quotations.AsNoTracking().FirstOrDefaultAsync(q => q.Id == quotationId && q.OrganizationId == access.OrganizationId, ct);
        if (quotation is null) return Fail<AcceptanceLinkProjection>("RESOURCE_NOT_FOUND", "Quotation not found.");
        if (quotation.Status != QuotationStatus.Issued) return Fail<AcceptanceLinkProjection>("QUOTATION_INVALID_STATE", $"A link can only be created for an issued quotation; current status is '{quotation.Status}'.");

        var now = _clock.UtcNow;
        QuotationAcceptanceLink link;
        try
        {
            link = new QuotationAcceptanceLink(Guid.NewGuid(), access.OrganizationId, quotationId, tokenHash, signerHint, now.AddDays(lifetimeDays), access.ActorUserId, now);
        }
        catch (AcceptanceLinkException ex)
        {
            return Fail<AcceptanceLinkProjection>(ex.Code, ex.Message);
        }

        _db.QuotationAcceptanceLinks.Add(link);
        // The token and the signer hint are never written to the audit trail.
        Audit(access.OrganizationId, access.ActorUserId, access.MembershipId, access.BranchId, "quotation-acceptance-link.created", link.Id, traceId, new { quotation = quotation.Number, expiresAtUtc = link.ExpiresAtUtc }, now);
        await _db.SaveChangesAsync(ct);
        return Result<AcceptanceLinkProjection>.Success(await ProjectAsync(link, ct));
    }

    public async Task<Result<AcceptanceLinkProjection>> RevokeLinkAsync(RequestAccessContext access, Guid linkId, string traceId, CancellationToken ct = default)
    {
        var link = await _db.QuotationAcceptanceLinks.FirstOrDefaultAsync(l => l.Id == linkId && l.OrganizationId == access.OrganizationId, ct);
        if (link is null) return Fail<AcceptanceLinkProjection>("RESOURCE_NOT_FOUND", "Acceptance link not found.");

        var now = _clock.UtcNow;
        try
        {
            link.Revoke(access.ActorUserId, now);
        }
        catch (AcceptanceLinkException ex)
        {
            return Fail<AcceptanceLinkProjection>(ex.Code, ex.Message);
        }

        Audit(access.OrganizationId, access.ActorUserId, access.MembershipId, access.BranchId, "quotation-acceptance-link.revoked", link.Id, traceId, new { }, now);
        await _db.SaveChangesAsync(ct);
        return Result<AcceptanceLinkProjection>.Success(await ProjectAsync(link, ct));
    }

    public async Task<IReadOnlyList<AcceptanceLinkProjection>?> ListLinksAsync(Guid organizationId, Guid quotationId, CancellationToken ct = default)
    {
        if (!await _db.Quotations.AnyAsync(q => q.Id == quotationId && q.OrganizationId == organizationId, ct)) return null;
        var links = await _db.QuotationAcceptanceLinks.AsNoTracking().Where(l => l.OrganizationId == organizationId && l.QuotationId == quotationId)
            .OrderByDescending(l => l.CreatedAtUtc).ToListAsync(ct);
        var result = new List<AcceptanceLinkProjection>();
        foreach (var link in links) result.Add(await ProjectAsync(link, ct));
        return result;
    }

    public async Task<AcceptanceContext?> FindContextAsync(string tokenHash, CancellationToken ct = default)
    {
        var row = await (from l in _db.QuotationAcceptanceLinks.AsNoTracking()
                         join q in _db.Quotations.AsNoTracking() on l.QuotationId equals q.Id
                         join o in _db.Opportunities.AsNoTracking() on q.OpportunityId equals o.Id
                         where l.TokenHash == tokenHash
                         select new { l, q, o }).FirstOrDefaultAsync(ct);
        if (row is null) return null;

        var evidence = await _db.QuotationAcceptanceEvidences.AsNoTracking().FirstOrDefaultAsync(e => e.LinkId == row.l.Id, ct);
        return new AcceptanceContext(
            row.l.Id, row.l.OrganizationId, row.q.Id, row.q.Number, row.q.Status, row.q.EstimateId, row.o.RowVersion, row.l.CreatedByUserId,
            row.l.Status, row.l.ExpiresAtUtc, row.l.SignerHint, row.l.AcceptedAtUtc, evidence is null ? null : Summarize(evidence));
    }

    public async Task<Result<AcceptanceEvidenceSummary>> RecordAcceptanceAsync(
        AcceptanceContext context, AcceptanceSubmission submission, string signatureHash, string clientAddressHash, string? userAgent, string traceId, CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var existing = await _db.QuotationAcceptanceEvidences.AsNoTracking().FirstOrDefaultAsync(e => e.LinkId == context.LinkId, ct);
            if (existing is not null) return Result<AcceptanceEvidenceSummary>.Success(Summarize(existing));

            var link = await _db.QuotationAcceptanceLinks.FirstOrDefaultAsync(l => l.Id == context.LinkId, ct);
            if (link is null) return Fail<AcceptanceEvidenceSummary>("ACCEPTANCE_LINK_UNAVAILABLE", "This acceptance link is not available.");

            var now = _clock.UtcNow;
            try
            {
                link.MarkAccepted(now);
            }
            catch (AcceptanceLinkException)
            {
                return Fail<AcceptanceEvidenceSummary>("ACCEPTANCE_LINK_UNAVAILABLE", "This acceptance link is not available.");
            }

            var evidence = new QuotationAcceptanceEvidence(
                Guid.NewGuid(), context.OrganizationId, link.Id, context.QuotationId, submission.SignerName!, submission.SignerRole, AcceptanceConsent.CurrentVersion,
                string.IsNullOrEmpty(submission.SignatureImage) ? null : submission.SignatureImage, string.IsNullOrEmpty(signatureHash) ? null : signatureHash,
                clientAddressHash, userAgent, now);
            _db.QuotationAcceptanceEvidences.Add(evidence);
            // The signer's name is evidence, not audit payload.
            Audit(context.OrganizationId, link.CreatedByUserId, null, null, "quotation-acceptance-link.accepted", link.Id, traceId, new { quotation = context.QuotationNumber }, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                // A concurrent submission already stored the evidence.
                _db.ChangeTracker.Clear();
                var stored = await _db.QuotationAcceptanceEvidences.AsNoTracking().FirstAsync(e => e.LinkId == context.LinkId, ct);
                return Result<AcceptanceEvidenceSummary>.Success(Summarize(stored));
            }

            return Result<AcceptanceEvidenceSummary>.Success(Summarize(evidence));
        });
    }
}
