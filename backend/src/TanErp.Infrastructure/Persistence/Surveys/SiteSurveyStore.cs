using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Surveys;
using TanErp.Application.Surveys.CreateSiteSurvey;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Surveys;

public class SiteSurveyStore : ISiteSurveyStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public SiteSurveyStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<SiteSurveyProjection>> CreateAsync(
        RequestAccessContext access,
        CreateSiteSurveyCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "surveys.create";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency record first
            var existingReplay = await TryLoadReplayAsync(orgId, operation, keyHash, payloadHash, cancellationToken);
            if (existingReplay is not null)
            {
                return existingReplay;
            }

            // 2. Resource scope: load Opportunity constrained by organization_id
            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.Id == command.OpportunityId && o.OrganizationId == orgId, cancellationToken);

            if (opp == null)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // Validate Opportunity stage: must be qualified
            if (opp.Stage != OpportunityStage.Qualified)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Opportunity must be in 'qualified' stage to schedule a survey."));
            }

            // Validate ExpectedOpportunityVersion
            if (opp.RowVersion != command.ExpectedOpportunityVersion)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            // Validate Customer exists, belongs to org, and is Active
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == opp.CustomerId && c.OrganizationId == orgId, cancellationToken);

            if (customer == null || customer.Status != CustomerStatus.Active)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active customer not found."));
            }

            // Validate Site exists, belongs to same Customer and same Organization, and is Active
            var site = await _db.Sites
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == command.SiteId && s.CustomerId == opp.CustomerId && s.OrganizationId == orgId, cancellationToken);

            if (site == null || site.Status != SiteStatus.Active)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active site not found for this customer."));
            }

            // Validate Branch exists, belongs to org, and is Active
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == opp.BranchId && b.OrganizationId == orgId, cancellationToken);

            if (branch == null || !branch.IsActive)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active branch not found."));
            }

            // Validate Assigned Surveyor has active Membership in the same branch/org
            var now = _clock.UtcNow;
            var surveyorMembership = await _db.Memberships
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == command.AssignedSurveyorId && m.BranchId == opp.BranchId && m.OrganizationId == orgId, cancellationToken);

            if (surveyorMembership == null || !surveyorMembership.IsActiveAt(now))
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active surveyor membership not found in this branch."));
            }

            // 3. Create SiteSurvey aggregate and Baseline Revision 1
            SiteSurvey survey;
            try
            {
                survey = SiteSurvey.CreateAppointment(
                    orgId,
                    opp.BranchId,
                    opp.Id,
                    site.Id,
                    command.AssignedSurveyorId,
                    access.ActorUserId,
                    command.ScheduledStartUtc,
                    command.ScheduledEndUtc,
                    now);
            }
            catch (ArgumentException ex)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("SURVEY_SCHEDULE_INVALID", ex.Message));
            }

            var revision = SiteSurveyRevision.CreateBaseline(
                orgId,
                survey.Id,
                access.ActorUserId,
                now);

            _db.SiteSurveys.Add(survey);
            _db.SiteSurveyRevisions.Add(revision);

            // 4. Transition Opportunity to Surveying and record history
            var previousStage = opp.Stage;
            try
            {
                opp.EnterSurveying(command.ExpectedOpportunityVersion, site.Id);
            }
            catch (OpportunityVersionException)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (OpportunityTransitionException)
            {
                return Result<SiteSurveyProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Opportunity cannot transition to surveying."));
            }

            var history = new OpportunityStageHistory(
                Guid.NewGuid(),
                orgId,
                opp.Id,
                previousStage,
                OpportunityStage.Surveying,
                reasonCode: null,
                note: $"Survey appointment {survey.SurveyNumber} created.",
                access.ActorUserId,
                now,
                OpportunityStagePolicy.Version,
                command.TraceId);
            _db.OpportunityStageHistories.Add(history);

            // 5. Add Audit Events
            var oppAuditChanges = FormattableString.Invariant(
                $"{{\"changedFields\":[\"stage\",\"primarySiteId\"],\"fromStage\":\"{previousStage}\",\"toStage\":\"{OpportunityStage.Surveying}\",\"surveyNumber\":\"{survey.SurveyNumber}\"}}");
            var oppAuditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.stage-changed",
                "Opportunity",
                opp.Id.ToString(),
                now,
                command.TraceId,
                oppAuditChanges);
            _db.AddAuditEvent(oppAuditEvent);

            var surveyAuditChanges = FormattableString.Invariant(
                $"{{\"changedFields\":[\"surveyNumber\",\"status\",\"scheduledStartUtc\",\"scheduledEndUtc\",\"assignedSurveyorId\"],\"surveyNumber\":\"{survey.SurveyNumber}\"}}");
            var surveyAuditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "survey.created",
                "SiteSurvey",
                survey.Id.ToString(),
                now,
                command.TraceId,
                surveyAuditChanges);
            _db.AddAuditEvent(surveyAuditEvent);

            // 6. Add IdempotencyRecord
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                survey.Id.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 7. Save and Commit
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<SiteSurveyProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();
                var replay = await TryLoadReplayAsync(
                    orgId, operation, keyHash, payloadHash, cancellationToken);
                if (replay is not null) return replay;
                throw;
            }

            var surveyorUser = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == command.AssignedSurveyorId)
                .Select(u => new SurveyorSummaryProjection(u.Id, u.DisplayName, u.Email))
                .FirstOrDefaultAsync(cancellationToken);

            var siteSummary = new SiteSummaryProjection(site.Id, site.Label, site.AddressLine1);

            return Result<SiteSurveyProjection>.Success(ToProjection(survey, revision, surveyorUser, siteSummary));
        });
    }

    public async Task<SiteSurveyProjection?> GetByOpportunityIdAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default)
    {
        var survey = await _db.SiteSurveys
            .AsNoTracking()
            .Include(s => s.Revisions)
            .FirstOrDefaultAsync(s => s.OpportunityId == opportunityId && s.OrganizationId == organizationId, cancellationToken);

        if (survey == null) return null;

        var currentRevision = survey.Revisions.OrderByDescending(r => r.RevisionNumber).FirstOrDefault();

        var surveyor = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == survey.AssignedSurveyorId)
            .Select(u => new SurveyorSummaryProjection(u.Id, u.DisplayName, u.Email))
            .FirstOrDefaultAsync(cancellationToken);

        var site = await _db.Sites
            .AsNoTracking()
            .Where(s => s.Id == survey.SiteId && s.OrganizationId == organizationId)
            .Select(s => new SiteSummaryProjection(s.Id, s.Label, s.AddressLine1))
            .FirstOrDefaultAsync(cancellationToken);

        return ToProjection(survey, currentRevision, surveyor, site);
    }

    private async Task<Result<SiteSurveyProjection>?> TryLoadReplayAsync(
        Guid organizationId,
        string operation,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken)
    {
        var record = await _db.IdempotencyRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(row =>
                row.OrganizationId == organizationId &&
                row.Operation == operation &&
                row.KeyHash == keyHash,
                cancellationToken);

        if (record is null) return null;
        if (record.PayloadHash != payloadHash)
            return Result<SiteSurveyProjection>.Failure(new Error(
                "IDEMPOTENCY_KEY_REUSED",
                "The idempotency key has already been used with a different payload."));

        if (!Guid.TryParse(record.ResourceId, out var surveyId)) return null;
        var survey = await _db.SiteSurveys
            .AsNoTracking()
            .Include(s => s.Revisions)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == surveyId && candidate.OrganizationId == organizationId,
                cancellationToken);

        if (survey is null) return null;
        var currentRevision = survey.Revisions.OrderByDescending(r => r.RevisionNumber).FirstOrDefault();
        return Result<SiteSurveyProjection>.Success(ToProjection(survey, currentRevision));
    }

    private static SiteSurveyProjection ToProjection(
        SiteSurvey s,
        SiteSurveyRevision? r,
        SurveyorSummaryProjection? surveyor = null,
        SiteSummaryProjection? site = null)
    {
        SiteSurveyRevisionProjection? revProj = null;
        if (r != null)
        {
            revProj = new SiteSurveyRevisionProjection(
                r.Id,
                r.SiteSurveyId,
                r.RevisionNumber,
                r.SurveyTemplateVersion,
                r.VisitedAtUtc,
                r.ScopeSummary,
                r.Assumptions.ToList(),
                r.Constraints.ToList(),
                r.MissingDetails.ToList(),
                r.Readiness,
                r.Status,
                r.ReadyAtUtc,
                r.ReadyByUserId,
                r.SnapshotHash,
                r.RowVersion,
                r.CreatedAtUtc,
                r.CreatedByUserId);
        }

        return new SiteSurveyProjection(
            s.Id,
            s.OrganizationId,
            s.BranchId,
            s.OpportunityId,
            s.SiteId,
            s.SurveyNumber,
            s.AssignedSurveyorId,
            s.ScheduledStartUtc,
            s.ScheduledEndUtc,
            s.Status,
            s.RowVersion,
            s.CreatedAtUtc,
            s.CreatedByUserId,
            revProj,
            surveyor,
            site);
    }
}
