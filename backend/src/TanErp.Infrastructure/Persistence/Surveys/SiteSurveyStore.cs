using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Files;
using TanErp.Application.Surveys;
using TanErp.Application.Surveys.CloneSurveyRevision;
using TanErp.Application.Surveys.CreateSiteSurvey;
using TanErp.Application.Surveys.MarkSurveyReady;
using TanErp.Application.Surveys.UpdateSurveyDraft;
using TanErp.Application.Surveys.VoidSurveyRevision;
using TanErp.Domain.Common;
using TanErp.Domain.Files;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Surveys;


public class SiteSurveyStore : ISiteSurveyStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IFileStore _fileStore;

    public SiteSurveyStore(AppDbContext db, IClock clock, IFileStore fileStore)
    {
        _db = db;
        _clock = clock;
        _fileStore = fileStore;
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
                now,
                SurveyDefaults.CurrentTemplateVersion);

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
                .ThenInclude(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
            .Include(s => s.Revisions)
                .ThenInclude(r => r.ChecklistResults)
            .Include(s => s.Revisions)
                .ThenInclude(r => r.Evidence)
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

    public async Task<Result<SiteSurveyRevisionProjection>> UpdateDraftAsync(
        RequestAccessContext access,
        UpdateSurveyDraftCommand command,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var survey = await _db.SiteSurveys
                .FirstOrDefaultAsync(s => s.Id == command.SiteSurveyId && s.OrganizationId == orgId, cancellationToken);

            if (survey == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey not found."));
            }

            var revision = await _db.SiteSurveyRevisions
                .Include(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
                .Include(r => r.ChecklistResults)
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.Id == command.RevisionId && r.SiteSurveyId == survey.Id && r.OrganizationId == orgId, cancellationToken);

            if (revision == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey revision not found."));
            }

            if (revision.RowVersion != command.ExpectedRevisionVersion)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_VERSION_CONFLICT", "Survey revision version conflict."));
            }

            if (revision.Status != SurveyRevisionStatus.Draft)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_INVALID_STATE", $"Cannot edit revision in status '{revision.Status}'."));
            }

            revision.UpdateDraft(
                command.VisitedAtUtc,
                command.ScopeSummary,
                command.Assumptions,
                command.Constraints,
                command.MissingDetails);

            // Update Areas & Measurements
            var existingAreas = revision.Areas.ToList();
            var commandAreaIds = command.Areas.Where(a => a.Id.HasValue).Select(a => a.Id!.Value).ToHashSet();

            foreach (var ea in existingAreas.Where(a => !commandAreaIds.Contains(a.Id)))
            {
                _db.SiteSurveyAreas.Remove(ea);
            }

            foreach (var aInput in command.Areas)
            {
                SiteSurveyArea area;
                if (aInput.Id.HasValue && existingAreas.FirstOrDefault(a => a.Id == aInput.Id.Value) is { } existingArea)
                {
                    area = existingArea;
                    _db.Entry(area).Property(p => p.Code).CurrentValue = aInput.Code.Trim().ToUpperInvariant();
                    _db.Entry(area).Property(p => p.Name).CurrentValue = aInput.Name.Trim();
                    _db.Entry(area).Property(p => p.Description).CurrentValue = string.IsNullOrWhiteSpace(aInput.Description) ? null : aInput.Description.Trim();
                    _db.Entry(area).Property(p => p.SortOrder).CurrentValue = aInput.SortOrder;
                }
                else
                {
                    area = new SiteSurveyArea(
                        aInput.Id ?? Guid.NewGuid(),
                        orgId,
                        revision.Id,
                        aInput.Code,
                        aInput.Name,
                        aInput.Description,
                        aInput.SortOrder);
                    _db.SiteSurveyAreas.Add(area);
                }

                var existingMeasurements = area.Measurements.ToList();
                var commandMeasurementIds = aInput.Measurements.Where(m => m.Id.HasValue).Select(m => m.Id!.Value).ToHashSet();

                foreach (var em in existingMeasurements.Where(m => !commandMeasurementIds.Contains(m.Id)))
                {
                    _db.SiteSurveyMeasurements.Remove(em);
                }

                foreach (var mInput in aInput.Measurements)
                {
                    if (mInput.Value <= 0)
                    {
                        return Result<SiteSurveyRevisionProjection>.Failure(
                            new Error("SURVEY_MEASUREMENT_INVALID", $"Measurement value for '{mInput.MeasurementType}' must be positive."));
                    }

                    if (!MeasurementType.IsValid(mInput.MeasurementType))
                    {
                        return Result<SiteSurveyRevisionProjection>.Failure(
                            new Error("SURVEY_MEASUREMENT_INVALID", $"Invalid measurement type: '{mInput.MeasurementType}'."));
                    }

                    if (!MeasurementUnit.IsValid(mInput.UnitCode))
                    {
                        return Result<SiteSurveyRevisionProjection>.Failure(
                            new Error("SURVEY_MEASUREMENT_INVALID", $"Invalid measurement unit: '{mInput.UnitCode}'."));
                    }

                    if (mInput.Id.HasValue && existingMeasurements.FirstOrDefault(m => m.Id == mInput.Id.Value) is { } existingMeasurement)
                    {
                        _db.Entry(existingMeasurement).Property(p => p.MeasurementType).CurrentValue = mInput.MeasurementType.Trim().ToLowerInvariant();
                        _db.Entry(existingMeasurement).Property(p => p.Value).CurrentValue = mInput.Value;
                        _db.Entry(existingMeasurement).Property(p => p.UnitCode).CurrentValue = mInput.UnitCode.Trim().ToLowerInvariant();
                        _db.Entry(existingMeasurement).Property(p => p.CaptureMethod).CurrentValue = mInput.CaptureMethod.Trim().ToLowerInvariant();
                        _db.Entry(existingMeasurement).Property(p => p.Notes).CurrentValue = string.IsNullOrWhiteSpace(mInput.Notes) ? null : mInput.Notes.Trim();
                        _db.Entry(existingMeasurement).Property(p => p.SortOrder).CurrentValue = mInput.SortOrder;
                    }
                    else
                    {
                        var newM = new SiteSurveyMeasurement(
                            mInput.Id ?? Guid.NewGuid(),
                            orgId,
                            area.Id,
                            mInput.MeasurementType,
                            mInput.Value,
                            mInput.UnitCode,
                            mInput.CaptureMethod,
                            mInput.Notes,
                            mInput.SortOrder);
                        _db.SiteSurveyMeasurements.Add(newM);
                    }
                }
            }

            var changedFields = new List<string> { "visitedAtUtc", "scopeSummary", "areas" };

            if (command.Checklist is not null)
            {
                var checklistError = ApplyChecklist(revision, orgId, command.Checklist);
                if (checklistError is not null)
                {
                    return Result<SiteSurveyRevisionProjection>.Failure(checklistError);
                }

                changedFields.Add("checklist");
            }

            if (command.Evidence is not null)
            {
                var evidenceError = await ApplyEvidenceAsync(revision, survey, orgId, access.ActorUserId, command.Evidence, cancellationToken);
                if (evidenceError is not null)
                {
                    return Result<SiteSurveyRevisionProjection>.Failure(evidenceError);
                }

                changedFields.Add("evidence");
            }

            var now = _clock.UtcNow;
            var changedFieldsJson = string.Join(",", changedFields.Select(f => $"\"{f}\""));
            var auditChanges = FormattableString.Invariant(
                $"{{\"changedFields\":[{changedFieldsJson}],\"revisionNumber\":{revision.RevisionNumber}}}");
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "survey.revision-updated",
                "SiteSurveyRevision",
                revision.Id.ToString(),
                now,
                command.TraceId,
                auditChanges);
            _db.AddAuditEvent(auditEvent);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            var refreshedRevision = await _db.SiteSurveyRevisions
                .AsNoTracking()
                .Include(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
                .Include(r => r.ChecklistResults)
                .Include(r => r.Evidence)
                .SingleAsync(r => r.Id == revision.Id, cancellationToken);

            return Result<SiteSurveyRevisionProjection>.Success(ToRevisionProjection(refreshedRevision, refreshedRevision.Areas.ToList()));
        });
    }

    public async Task<Result<SiteSurveyRevisionProjection>> MarkReadyAsync(
        RequestAccessContext access,
        MarkSurveyReadyCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // Replay check
            var existingRecord = await _db.IdempotencyRecords
                .SingleOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == "site_survey_revision.mark_ready" && r.KeyHash == keyHash, cancellationToken);

            if (existingRecord != null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<SiteSurveyRevisionProjection>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key has already been used with different payload."));
                }

                var cachedRevision = await _db.SiteSurveyRevisions
                    .AsNoTracking()
                    .Include(r => r.Areas)
                        .ThenInclude(a => a.Measurements)
                    .Include(r => r.ChecklistResults)
                    .Include(r => r.Evidence)
                    .SingleOrDefaultAsync(r => r.Id == command.RevisionId && r.OrganizationId == orgId, cancellationToken);

                if (cachedRevision != null)
                {
                    return Result<SiteSurveyRevisionProjection>.Success(ToRevisionProjection(cachedRevision, cachedRevision.Areas.ToList()));
                }
            }

            var survey = await _db.SiteSurveys
                .FirstOrDefaultAsync(s => s.Id == command.SiteSurveyId && s.OrganizationId == orgId, cancellationToken);

            if (survey == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey not found."));
            }

            var revision = await _db.SiteSurveyRevisions
                .Include(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
                .Include(r => r.ChecklistResults)
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.Id == command.RevisionId && r.SiteSurveyId == survey.Id && r.OrganizationId == orgId, cancellationToken);

            if (revision == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey revision not found."));
            }

            if (revision.RowVersion != command.ExpectedRevisionVersion)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_VERSION_CONFLICT", "Survey revision version conflict."));
            }

            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.Id == survey.OpportunityId && o.OrganizationId == orgId, cancellationToken);

            if (opp == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Associated opportunity not found."));
            }

            if (opp.RowVersion != command.ExpectedOpportunityVersion)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            var previousReadyRevisions = await _db.SiteSurveyRevisions
                .Where(r => r.SiteSurveyId == survey.Id
                    && r.OrganizationId == orgId
                    && r.Id != revision.Id
                    && r.Status == SurveyRevisionStatus.Ready)
                .ToListAsync(cancellationToken);

            // A clone re-marked ready while the opportunity is already estimating keeps the stage;
            // only the first ready revision advances surveying -> estimating.
            var advancesStage = opp.Stage == OpportunityStage.Surveying;
            var isLaterReadyRevision = opp.Stage == OpportunityStage.Estimating && revision.RevisionNumber > 1;

            if (!advancesStage && !isLaterReadyRevision)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", $"Opportunity must be in 'surveying' stage to enter estimating. Current stage: '{opp.Stage}'."));
            }

            var now = _clock.UtcNow;

            var evidenceFileIds = revision.Evidence.Select(e => e.FileId).ToList();
            var evidenceFiles = evidenceFileIds.Count == 0
                ? new Dictionary<Guid, string?>()
                : await _db.UploadedFiles
                    .AsNoTracking()
                    .Where(f => evidenceFileIds.Contains(f.Id) && f.OrganizationId == orgId && f.Status == UploadedFileStatus.Verified)
                    .ToDictionaryAsync(f => f.Id, f => f.ContentSha256, cancellationToken);

            if (evidenceFileIds.Any(id => !evidenceFiles.ContainsKey(id)))
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_NOT_READY", "Every evidence file must be uploaded and verified before the revision is ready."));
            }

            var snapshotHash = SurveySnapshotHasher.Compute(survey.SurveyNumber, revision, evidenceFiles);

            try
            {
                revision.MarkReady(access.ActorUserId, now, snapshotHash);
            }
            catch (SurveyReadinessException ex)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_NOT_READY", ex.Message));
            }
            catch (SurveyInvalidStateException ex)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_INVALID_STATE", ex.Message));
            }

            if (advancesStage)
            {
                // Milestone: Advance Opportunity to estimating
                var previousOppStage = opp.Stage;
                opp.EnterEstimating(command.ExpectedOpportunityVersion);

                var history = new OpportunityStageHistory(
                    Guid.NewGuid(),
                    orgId,
                    opp.Id,
                    previousOppStage,
                    OpportunityStage.Estimating,
                    reasonCode: null,
                    note: $"Survey {survey.SurveyNumber} revision {revision.RevisionNumber} marked ready.",
                    access.ActorUserId,
                    now,
                    OpportunityStagePolicy.Version,
                    command.TraceId);
                _db.OpportunityStageHistories.Add(history);

                // Audit
                var oppAuditChanges = FormattableString.Invariant(
                    $"{{\"changedFields\":[\"stage\"],\"fromStage\":\"{previousOppStage}\",\"toStage\":\"{OpportunityStage.Estimating}\",\"surveyNumber\":\"{survey.SurveyNumber}\",\"revisionNumber\":{revision.RevisionNumber}}}");
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
            }

            foreach (var previous in previousReadyRevisions)
            {
                previous.Supersede();
                _db.AddAuditEvent(new AuditEvent(
                    Guid.NewGuid(),
                    orgId,
                    access.ActorUserId,
                    "survey.revision-superseded",
                    "SiteSurveyRevision",
                    previous.Id.ToString(),
                    now,
                    command.TraceId,
                    FormattableString.Invariant(
                        $"{{\"changedFields\":[\"status\"],\"revisionNumber\":{previous.RevisionNumber},\"supersededByRevisionNumber\":{revision.RevisionNumber}}}")));
            }

            var surveyAuditChanges = FormattableString.Invariant(
                $"{{\"changedFields\":[\"status\",\"readiness\",\"snapshotHash\"],\"revisionNumber\":{revision.RevisionNumber},\"snapshotHash\":\"{snapshotHash}\"}}");
            var surveyAuditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "survey.marked-ready",
                "SiteSurveyRevision",
                revision.Id.ToString(),
                now,
                command.TraceId,
                surveyAuditChanges);
            _db.AddAuditEvent(surveyAuditEvent);

            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                "site_survey_revision.mark_ready",
                keyHash,
                payloadHash,
                revision.Id.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);


            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result<SiteSurveyRevisionProjection>.Success(ToRevisionProjection(revision, revision.Areas.ToList()));
        });
    }

    public async Task<Result<SiteSurveyRevisionProjection>> CloneRevisionAsync(
        RequestAccessContext access,
        CloneSurveyRevisionCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "site_survey_revision.clone";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var replay = await TryLoadRevisionReplayAsync(orgId, operation, keyHash, payloadHash, cancellationToken);
            if (replay is not null)
            {
                return replay;
            }

            var survey = await _db.SiteSurveys
                .FirstOrDefaultAsync(s => s.Id == command.SiteSurveyId && s.OpportunityId == command.OpportunityId && s.OrganizationId == orgId, cancellationToken);

            if (survey == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey not found."));
            }

            var revisions = await _db.SiteSurveyRevisions
                .Include(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
                .Include(r => r.ChecklistResults)
                .Include(r => r.Evidence)
                .Where(r => r.SiteSurveyId == survey.Id && r.OrganizationId == orgId)
                .ToListAsync(cancellationToken);

            var source = revisions.FirstOrDefault(r => r.Id == command.SourceRevisionId);
            if (source == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey revision not found."));
            }

            var opp = await _db.Opportunities
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == survey.OpportunityId && o.OrganizationId == orgId, cancellationToken);

            if (opp == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Associated opportunity not found."));
            }

            if (!IsRevisableStage(opp.Stage))
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", $"Survey revisions cannot be cloned while the opportunity is '{opp.Stage}'."));
            }

            if (revisions.Any(r => r.Status == SurveyRevisionStatus.Draft))
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_DRAFT_EXISTS", "A draft revision already exists for this survey."));
            }

            var now = _clock.UtcNow;
            SiteSurveyRevision clone;
            try
            {
                clone = SiteSurveyRevision.CloneFrom(
                    source,
                    revisions.Max(r => r.RevisionNumber) + 1,
                    command.Reason,
                    access.ActorUserId,
                    now);
            }
            catch (SurveyInvalidStateException ex)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_INVALID_STATE", ex.Message));
            }

            _db.SiteSurveyRevisions.Add(clone);

            _db.AddAuditEvent(new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "survey.revision-cloned",
                "SiteSurveyRevision",
                clone.Id.ToString(),
                now,
                command.TraceId,
                FormattableString.Invariant(
                    $"{{\"changedFields\":[\"revision\"],\"revisionNumber\":{clone.RevisionNumber},\"sourceRevisionNumber\":{source.RevisionNumber}}}")));

            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                clone.Id.ToString(),
                now));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // Concurrent clone won the unique (survey, revision_number) slot.
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_VERSION_CONFLICT", "Survey revision number was taken by a concurrent request."));
            }

            await tx.CommitAsync(cancellationToken);

            return Result<SiteSurveyRevisionProjection>.Success(ToRevisionProjection(clone, clone.Areas.ToList()));
        });
    }

    public async Task<Result<SiteSurveyRevisionProjection>> VoidRevisionAsync(
        RequestAccessContext access,
        VoidSurveyRevisionCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "site_survey_revision.void";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var replay = await TryLoadRevisionReplayAsync(orgId, operation, keyHash, payloadHash, cancellationToken);
            if (replay is not null)
            {
                return replay;
            }

            var survey = await _db.SiteSurveys
                .FirstOrDefaultAsync(s => s.Id == command.SiteSurveyId && s.OpportunityId == command.OpportunityId && s.OrganizationId == orgId, cancellationToken);

            if (survey == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey not found."));
            }

            var revision = await _db.SiteSurveyRevisions
                .Include(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
                .Include(r => r.ChecklistResults)
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.Id == command.RevisionId && r.SiteSurveyId == survey.Id && r.OrganizationId == orgId, cancellationToken);

            if (revision == null)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey revision not found."));
            }

            if (revision.RowVersion != command.ExpectedRevisionVersion)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_VERSION_CONFLICT", "Survey revision version conflict."));
            }

            // Voiding never deletes or re-points an Estimate; it only records the risk in the audit trail.
            var referencedByEstimate = await _db.Estimates
                .AsNoTracking()
                .AnyAsync(e => e.OrganizationId == orgId && e.SiteSurveyRevisionId == revision.Id, cancellationToken);

            var now = _clock.UtcNow;
            var previousStatus = revision.Status;
            try
            {
                revision.Void(access.ActorUserId, now, command.Reason);
            }
            catch (SurveyInvalidStateException ex)
            {
                return Result<SiteSurveyRevisionProjection>.Failure(
                    new Error("SURVEY_INVALID_STATE", ex.Message));
            }

            _db.AddAuditEvent(new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "survey.revision-voided",
                "SiteSurveyRevision",
                revision.Id.ToString(),
                now,
                command.TraceId,
                FormattableString.Invariant(
                    $"{{\"changedFields\":[\"status\"],\"revisionNumber\":{revision.RevisionNumber},\"fromStatus\":\"{previousStatus}\",\"referencedByEstimate\":{(referencedByEstimate ? "true" : "false")}}}")));

            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                revision.Id.ToString(),
                now));

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result<SiteSurveyRevisionProjection>.Success(ToRevisionProjection(revision, revision.Areas.ToList()));
        });
    }

    private Error? ApplyChecklist(
        SiteSurveyRevision revision,
        Guid orgId,
        IReadOnlyList<ChecklistInput> inputs)
    {
        var template = SurveyTemplates.Find(revision.SurveyTemplateVersion);
        if (template is null)
        {
            return new Error("SURVEY_TEMPLATE_UNAVAILABLE", $"Survey template '{revision.SurveyTemplateVersion}' is not available.");
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var input in inputs)
        {
            var code = input.ItemCode?.Trim() ?? string.Empty;
            if (!template.RequiredChecklistItems.Contains(code, StringComparer.Ordinal))
            {
                return new Error("SURVEY_CHECKLIST_INVALID", $"Checklist item '{code}' does not belong to template '{template.Code}'.");
            }

            if (!ChecklistResultValue.IsValid(input.Result) || !seen.Add(code))
            {
                return new Error("SURVEY_CHECKLIST_INVALID", $"Checklist item '{code}' has an invalid or duplicate result.");
            }

            if (input.Note is { Length: > 500 })
            {
                return new Error("SURVEY_CHECKLIST_INVALID", $"Checklist item '{code}' note is too long.");
            }
        }

        var existing = revision.ChecklistResults.ToList();
        foreach (var removed in existing.Where(e => !seen.Contains(e.ItemCode)))
        {
            _db.SiteSurveyChecklistResults.Remove(removed);
        }

        foreach (var input in inputs)
        {
            var code = input.ItemCode.Trim();
            var note = string.IsNullOrWhiteSpace(input.Note) ? null : input.Note.Trim();
            var current = existing.FirstOrDefault(e => e.ItemCode == code);
            if (current is null)
            {
                _db.SiteSurveyChecklistResults.Add(new SiteSurveyChecklistResult(
                    Guid.NewGuid(), orgId, revision.Id, code, input.Result, note));
            }
            else
            {
                _db.Entry(current).Property(p => p.Result).CurrentValue = input.Result.Trim();
                _db.Entry(current).Property(p => p.Note).CurrentValue = note;
            }
        }

        return null;
    }

    private async Task<Error?> ApplyEvidenceAsync(
        SiteSurveyRevision revision,
        SiteSurvey survey,
        Guid orgId,
        Guid actorUserId,
        IReadOnlyList<EvidenceInput> inputs,
        CancellationToken cancellationToken)
    {
        var fileIds = new HashSet<Guid>();
        foreach (var input in inputs)
        {
            if (input.FileId == Guid.Empty || !EvidenceKind.IsValid(input.Kind) || !fileIds.Add(input.FileId))
            {
                return new Error("SURVEY_EVIDENCE_INVALID", "Evidence entries need a unique file ID and a valid kind.");
            }

            if (input.Caption is { Length: > 500 })
            {
                return new Error("SURVEY_EVIDENCE_INVALID", "Evidence caption is too long.");
            }
        }

        var existing = revision.Evidence.ToList();
        // Files already attached to this revision stay valid; only newly added ones must come from the opportunity's upload sessions.
        var newFileIds = fileIds.Where(id => existing.All(e => e.FileId != id)).ToList();
        if (newFileIds.Count > 0)
        {
            var validation = await _fileStore.ValidateVerifiedFilesForParentAsync(
                orgId,
                actorUserId,
                FileParentTypes.Opportunity,
                survey.OpportunityId,
                null,
                newFileIds,
                cancellationToken);

            if (validation.IsFailure)
            {
                return new Error("SURVEY_EVIDENCE_NOT_READY", validation.Error.Message ?? "One or more evidence files are not verified.");
            }
        }

        foreach (var removed in existing.Where(e => !fileIds.Contains(e.FileId)))
        {
            _db.SiteSurveyEvidence.Remove(removed);
        }

        foreach (var input in inputs)
        {
            var caption = string.IsNullOrWhiteSpace(input.Caption) ? null : input.Caption.Trim();
            var current = existing.FirstOrDefault(e => e.FileId == input.FileId);
            if (current is null)
            {
                _db.SiteSurveyEvidence.Add(new SiteSurveyEvidence(
                    Guid.NewGuid(), orgId, revision.Id, input.FileId, input.Kind, caption, input.SortOrder));
            }
            else
            {
                _db.Entry(current).Property(p => p.Kind).CurrentValue = input.Kind.Trim();
                _db.Entry(current).Property(p => p.Caption).CurrentValue = caption;
                _db.Entry(current).Property(p => p.SortOrder).CurrentValue = input.SortOrder;
            }
        }

        return null;
    }

    private static bool IsRevisableStage(string stage) =>
        stage is OpportunityStage.Surveying or OpportunityStage.Estimating or OpportunityStage.Proposed;

    private async Task<Result<SiteSurveyRevisionProjection>?> TryLoadRevisionReplayAsync(
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
        {
            return Result<SiteSurveyRevisionProjection>.Failure(new Error(
                "IDEMPOTENCY_KEY_REUSED",
                "The idempotency key has already been used with a different payload."));
        }

        if (!Guid.TryParse(record.ResourceId, out var revisionId)) return null;
        var revision = await _db.SiteSurveyRevisions
            .AsNoTracking()
            .Include(r => r.Areas)
                .ThenInclude(a => a.Measurements)
            .Include(r => r.ChecklistResults)
            .Include(r => r.Evidence)
            .SingleOrDefaultAsync(r => r.Id == revisionId && r.OrganizationId == organizationId, cancellationToken);

        return revision is null
            ? null
            : Result<SiteSurveyRevisionProjection>.Success(ToRevisionProjection(revision, revision.Areas.ToList()));
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
                .ThenInclude(r => r.Areas)
                    .ThenInclude(a => a.Measurements)
            .Include(s => s.Revisions)
                .ThenInclude(r => r.ChecklistResults)
            .Include(s => s.Revisions)
                .ThenInclude(r => r.Evidence)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == surveyId && candidate.OrganizationId == organizationId,
                cancellationToken);

        if (survey is null) return null;
        var currentRevision = survey.Revisions.OrderByDescending(r => r.RevisionNumber).FirstOrDefault();
        return Result<SiteSurveyProjection>.Success(ToProjection(survey, currentRevision));
    }

    private static SiteSurveyRevisionProjection ToRevisionProjection(
        SiteSurveyRevision r,
        IReadOnlyList<SiteSurveyArea>? areas = null)
    {
        var areaList = areas?.OrderBy(a => a.SortOrder).Select(a => new SiteSurveyAreaProjection(
            a.Id,
            a.SiteSurveyRevisionId,
            a.Code,
            a.Name,
            a.Description,
            a.SortOrder,
            a.Measurements.OrderBy(m => m.SortOrder).Select(m => new SiteSurveyMeasurementProjection(
                m.Id,
                m.SiteSurveyAreaId,
                m.MeasurementType,
                m.Value,
                m.UnitCode,
                m.CaptureMethod,
                m.Notes,
                m.SortOrder)).ToList())).ToList();

        return new SiteSurveyRevisionProjection(
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
            r.CreatedByUserId,
            areaList,
            r.ChecklistResults
                .OrderBy(c => c.ItemCode, StringComparer.Ordinal)
                .Select(c => new SiteSurveyChecklistResultProjection(c.Id, c.ItemCode, c.Result, c.Note))
                .ToList(),
            r.Evidence
                .OrderBy(e => e.SortOrder)
                .ThenBy(e => e.FileId)
                .Select(e => new SiteSurveyEvidenceProjection(e.Id, e.FileId, e.Kind, e.Caption, e.SortOrder))
                .ToList());
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
            revProj = ToRevisionProjection(r, r.Areas.ToList());
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
            site,
            s.Revisions
                .Where(x => x.Status == SurveyRevisionStatus.Ready)
                .OrderByDescending(x => x.RevisionNumber)
                .Select(x => new SiteSurveyReadyRevisionProjection(x.Id, x.RevisionNumber, x.SnapshotHash))
                .FirstOrDefault());
    }
}
