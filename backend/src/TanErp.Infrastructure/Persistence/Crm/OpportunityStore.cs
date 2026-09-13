using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Application.Crm.Opportunities.QualifyOpportunity;
using TanErp.Application.Crm.Opportunities.UpdateDraftQGate;
using TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity;
using TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Crm;

public class OpportunityStore : IOpportunityStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public OpportunityStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<OpportunityProjection>> CreateAsync(
        RequestAccessContext access,
        CreateOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.create";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency record
            var existingReplay = await TryLoadReplayAsync(orgId, operation, keyHash, payloadHash, cancellationToken);
            if (existingReplay is not null)
            {
                return existingReplay;
            }

            // 2. Validate Customer exists, belongs to org, and is Active
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == command.CustomerId && c.OrganizationId == orgId, cancellationToken);

            if (customer == null)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            }

            if (customer.Status != CustomerStatus.Active)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("CUSTOMER_INVALID_STATE", "Opportunities can only be created for active customers."));
            }

            // 3. Validate Branch exists, belongs to org, and is Active
            var branchId = access.BranchId!.Value;
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == orgId, cancellationToken);

            if (branch == null || !branch.IsActive)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active branch not found."));
            }

            // 4. Validate PrimarySiteId if provided: must belong to the exact same customer and organization, and be Active
            if (command.PrimarySiteId.HasValue)
            {
                var site = await _db.Sites
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == command.PrimarySiteId.Value && s.CustomerId == command.CustomerId && s.OrganizationId == orgId, cancellationToken);

                if (site == null)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Primary site not found for this customer."));
                }

                if (site.Status != SiteStatus.Active)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Primary site is not active."));
                }
            }

            // 5. Create Opportunity aggregate
            var now = _clock.UtcNow;
            var oppId = Guid.NewGuid();

            var opp = Opportunity.CreateDraft(
                oppId,
                orgId,
                branchId,
                customer.Id,
                command.PrimarySiteId,
                access.ActorUserId,
                access.ActorUserId,
                command.Title,
                command.ScopeSummary,
                command.WorkTypes,
                command.SourceCode,
                command.ExpectedBudget,
                command.CurrencyCode,
                command.TargetDecisionDate,
                command.NextActionAtUtc,
                command.NextActionNote,
                now);

            _db.Opportunities.Add(opp);

            // 6. Record idempotency
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                oppId.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 7. Record audit event (no raw PII)
            const string auditChanges = "{\"changedFields\":[\"branchId\",\"customerId\",\"primarySiteId\",\"ownerUserId\",\"title\",\"scopeSummary\",\"workTypes\",\"sourceCode\",\"expectedBudget\",\"currencyCode\",\"targetDecisionDate\",\"nextActionAtUtc\",\"nextActionNote\",\"stage\"]}";
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.created",
                "Opportunity",
                oppId.ToString(),
                now,
                command.TraceId,
                auditChanges);
            _db.AddAuditEvent(auditEvent);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
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

            return Result<OpportunityProjection>.Success(ToProjection(opp));
        });
    }

    public async Task<Result<OpportunityProjection>> QualifyAsync(
        RequestAccessContext access,
        QualifyOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.qualify";
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
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // Validate Customer exists, belongs to org, and is Active
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == opp.CustomerId && c.OrganizationId == orgId, cancellationToken);

            if (customer == null)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            }

            // Validate Branch exists, belongs to org, and is Active
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == opp.BranchId && b.OrganizationId == orgId, cancellationToken);

            if (branch == null || !branch.IsActive)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active branch not found."));
            }

            // Validate active owner Membership in the same branch
            var now = _clock.UtcNow;
            var ownerMembership = await _db.Memberships
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == opp.OwnerUserId && m.BranchId == opp.BranchId && m.OrganizationId == orgId, cancellationToken);

            if (ownerMembership == null || !ownerMembership.IsActiveAt(now))
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active owner membership not found in this branch."));
            }

            // 3. Expected version
            if (opp.RowVersion != command.ExpectedVersion)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            // 4. Current state / target check & domain execution
            var previousStage = opp.Stage;
            var targetStage = command.TargetStage;

            if (targetStage == OpportunityStage.Qualified && previousStage == OpportunityStage.Draft)
            {
                if (customer.Status != CustomerStatus.Active)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("CUSTOMER_INVALID_STATE", "Customer is not active."));
                }

                try
                {
                    opp.Qualify(command.ExpectedVersion);
                }
                catch (OpportunityVersionException)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
                }
                catch (OpportunityTransitionException)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_INVALID_TRANSITION", "Cannot transition opportunity to the requested stage."));
                }
                catch (OpportunityQualificationException ex)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_FIELD_REQUIRED", $"Qualification gate failed for field '{ex.MissingField}'."));
                }
            }
            else if (targetStage == OpportunityStage.Lost || targetStage == OpportunityStage.Cancelled)
            {
                if (string.IsNullOrWhiteSpace(command.ReasonCode))
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_FIELD_REQUIRED", "Reason code is required to close an opportunity."));
                }

                try
                {
                    opp.Close(command.ExpectedVersion, targetStage, command.ReasonCode, command.Note);
                }
                catch (OpportunityVersionException)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
                }
                catch (OpportunityTransitionException)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_INVALID_TRANSITION", "Cannot transition opportunity to the requested stage."));
                }
                catch (ArgumentException ex)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_FIELD_REQUIRED", ex.Message));
                }
            }
            else if ((previousStage == OpportunityStage.Lost || previousStage == OpportunityStage.Cancelled) &&
                     (targetStage == OpportunityStage.Draft || targetStage == OpportunityStage.Qualified))
            {
                if (string.IsNullOrWhiteSpace(command.ReasonCode))
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_FIELD_REQUIRED", "Reason code is required to reopen an opportunity."));
                }

                try
                {
                    opp.Reopen(command.ExpectedVersion, targetStage, command.ReasonCode, command.Note);
                }
                catch (OpportunityVersionException)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
                }
                catch (OpportunityTransitionException)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_INVALID_TRANSITION", "Cannot transition opportunity to the requested stage."));
                }
                catch (ArgumentException ex)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("OPPORTUNITY_FIELD_REQUIRED", ex.Message));
                }
            }
            else
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Cannot transition opportunity to the requested stage."));
            }

            // 6. Add OpportunityStageHistory
            var history = new OpportunityStageHistory(
                Guid.NewGuid(),
                orgId,
                opp.Id,
                previousStage,
                targetStage,
                command.ReasonCode,
                command.Note,
                access.ActorUserId,
                now,
                OpportunityStagePolicy.Version,
                command.TraceId);
            _db.OpportunityStageHistories.Add(history);

            // 7. Add AuditEvent (stage change only, no PII, metadata only)
            var reasonCodeJson = string.IsNullOrWhiteSpace(command.ReasonCode) ? "null" : $"\"{command.ReasonCode}\"";
            var auditChanges = FormattableString.Invariant(
                $"{{\"changedFields\":[\"stage\"],\"fromStage\":\"{previousStage}\",\"toStage\":\"{targetStage}\",\"reasonCode\":{reasonCodeJson}}}");
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.stage-changed",
                "Opportunity",
                opp.Id.ToString(),
                now,
                command.TraceId,
                auditChanges);
            _db.AddAuditEvent(auditEvent);

            // 8. Add IdempotencyRecord
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                opp.Id.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 9. Save and commit
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<OpportunityProjection>.Failure(
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

            return Result<OpportunityProjection>.Success(ToProjection(opp));
        });
    }

    public async Task<Result<OpportunityProjection>> UpdateDraftQGateAsync(
        RequestAccessContext access,
        UpdateDraftQGateCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.update-draft-q-gate";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency replay
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
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // 3. Expected version
            if (opp.RowVersion != command.ExpectedVersion)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            // 4. Current state
            if (opp.Stage != OpportunityStage.Draft)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Only draft opportunities can be updated."));
            }

            // 5. Track changed fields for privacy-safe audit
            var changedFields = new List<string>();
            if (opp.ScopeSummary != command.ScopeSummary) changedFields.Add("scopeSummary");
            if (!opp.WorkTypes.SequenceEqual(command.WorkTypes)) changedFields.Add("workTypes");
            if (opp.NextActionAtUtc != command.NextActionAtUtc) changedFields.Add("nextActionAtUtc");
            if (opp.NextActionNote != command.NextActionNote) changedFields.Add("nextActionNote");

            // 6. Aggregate mutation
            try
            {
                opp.EditDraftQGate(
                    command.ExpectedVersion,
                    command.ScopeSummary,
                    command.WorkTypes,
                    command.NextActionAtUtc,
                    command.NextActionNote);
            }
            catch (OpportunityVersionException)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (OpportunityTransitionException)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Only draft opportunities can be updated."));
            }
            catch (ArgumentException ex)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", ex.Message));
            }

            var now = _clock.UtcNow;

            // 7. Audit event (changed field names only, no values/PII)
            var changedFieldsJson = "{\"changedFields\":[" + string.Join(",", changedFields.Select(f => $"\"{f}\"")) + "]}";
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.updated",
                "Opportunity",
                opp.Id.ToString(),
                now,
                command.TraceId,
                changedFieldsJson);
            _db.AuditEvents.Add(auditEvent);

            // 8. Idempotency record
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                opp.Id.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 9. Commit transaction with optimistic concurrency
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<OpportunityProjection>.Failure(
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

            return Result<OpportunityProjection>.Success(ToProjection(opp));
        });
    }

    public async Task<Result<OpportunityProjection>> UpdateOpenAsync(
        RequestAccessContext access,
        UpdateOpenOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.update-open";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency replay
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
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // 3. Expected version
            if (opp.RowVersion != command.ExpectedVersion)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            // 4. Current state must be open
            if (opp.Stage == OpportunityStage.Won || opp.Stage == OpportunityStage.Lost || opp.Stage == OpportunityStage.Cancelled)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Closed opportunities cannot be updated."));
            }

            // 5. If primarySiteId provided, validate it belongs to same customer and org and is Active
            if (command.PrimarySiteId.HasValue)
            {
                var site = await _db.Sites
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == command.PrimarySiteId.Value && s.CustomerId == opp.CustomerId && s.OrganizationId == orgId, cancellationToken);

                if (site == null)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Primary site not found for this customer."));
                }

                if (site.Status != SiteStatus.Active)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Primary site is not active."));
                }
            }

            // 6. Track changed fields for privacy-safe audit
            var changedFields = new List<string>();
            if (opp.Title != command.Title) changedFields.Add("title");
            if (opp.PrimarySiteId != command.PrimarySiteId) changedFields.Add("primarySiteId");
            if (opp.ScopeSummary != command.ScopeSummary) changedFields.Add("scopeSummary");
            if (!opp.WorkTypes.SequenceEqual(command.WorkTypes)) changedFields.Add("workTypes");
            if (opp.SourceCode != command.SourceCode) changedFields.Add("sourceCode");
            if (opp.ExpectedBudget != command.ExpectedBudget) changedFields.Add("expectedBudget");
            if (opp.CurrencyCode != command.CurrencyCode) changedFields.Add("currencyCode");
            if (opp.TargetDecisionDate != command.TargetDecisionDate) changedFields.Add("targetDecisionDate");
            if (opp.NextActionAtUtc != command.NextActionAtUtc) changedFields.Add("nextActionAtUtc");
            if (opp.NextActionNote != command.NextActionNote) changedFields.Add("nextActionNote");

            // 7. Aggregate mutation
            try
            {
                opp.EditOpen(
                    command.ExpectedVersion,
                    command.Title,
                    command.PrimarySiteId,
                    command.ScopeSummary,
                    command.WorkTypes,
                    command.SourceCode,
                    command.ExpectedBudget,
                    command.CurrencyCode,
                    command.TargetDecisionDate,
                    command.NextActionAtUtc,
                    command.NextActionNote);
            }
            catch (OpportunityVersionException)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (OpportunityTransitionException)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Closed opportunities cannot be updated."));
            }
            catch (ArgumentException ex)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", ex.Message));
            }

            var now = _clock.UtcNow;

            // 8. Audit event (changed field names only, no values/PII)
            var changedFieldsJson = "{\"changedFields\":[" + string.Join(",", changedFields.Select(f => $"\"{f}\"")) + "]}";
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.updated",
                "Opportunity",
                opp.Id.ToString(),
                now,
                command.TraceId,
                changedFieldsJson);
            _db.AuditEvents.Add(auditEvent);

            // 9. Idempotency record
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                opp.Id.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 10. Commit transaction with optimistic concurrency
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<OpportunityProjection>.Failure(
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

            return Result<OpportunityProjection>.Success(ToProjection(opp));
        });
    }

    public async Task<Result<OpportunityProjection>> ReassignOwnerAsync(
        RequestAccessContext access,
        ReassignOpportunityOwnerCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.reassign-owner";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency replay
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
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // 3. Expected version
            if (opp.RowVersion != command.ExpectedVersion)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            // 4. Current state must be open
            if (opp.Stage == OpportunityStage.Won || opp.Stage == OpportunityStage.Lost || opp.Stage == OpportunityStage.Cancelled)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Closed opportunities cannot be reassigned."));
            }

            // 5. Target owner must have active membership in the same branch
            var now = _clock.UtcNow;
            var targetMembership = await _db.Memberships
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == command.TargetOwnerUserId && m.BranchId == opp.BranchId && m.OrganizationId == orgId, cancellationToken);

            if (targetMembership == null || !targetMembership.IsActiveAt(now))
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active target owner membership not found in this branch."));
            }

            var previousOwnerUserId = opp.OwnerUserId;

            // 6. Aggregate mutation
            try
            {
                opp.ReassignOwner(command.ExpectedVersion, command.TargetOwnerUserId);
            }
            catch (OpportunityVersionException)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (OpportunityTransitionException)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Closed opportunities cannot be reassigned."));
            }
            catch (ArgumentException ex)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", ex.Message));
            }

            // 7. Audit event: opportunity.owner-changed with IDs only (no PII)
            var auditPayload = FormattableString.Invariant(
                $"{{\"previousOwnerUserId\":\"{previousOwnerUserId:D}\",\"newOwnerUserId\":\"{command.TargetOwnerUserId:D}\"}}");
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.owner-changed",
                "Opportunity",
                opp.Id.ToString(),
                now,
                command.TraceId,
                auditPayload);
            _db.AuditEvents.Add(auditEvent);

            // 8. Idempotency record
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                opp.Id.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 9. Commit transaction with optimistic concurrency
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<OpportunityProjection>.Failure(
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

            return Result<OpportunityProjection>.Success(ToProjection(opp));
        });
    }

    private async Task<Result<OpportunityProjection>?> TryLoadReplayAsync(
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
            return Result<OpportunityProjection>.Failure(new Error(
                "IDEMPOTENCY_KEY_REUSED",
                "The idempotency key has already been used with a different payload."));

        if (!Guid.TryParse(record.ResourceId, out var opportunityId)) return null;
        var opportunity = await _db.Opportunities.AsNoTracking().SingleOrDefaultAsync(
            candidate => candidate.Id == opportunityId && candidate.OrganizationId == organizationId,
            cancellationToken);
        return opportunity is null
            ? null
            : Result<OpportunityProjection>.Success(ToProjection(opportunity));
    }

    public async Task<OpportunityPage> ListAsync(
        Guid organizationId,
        OpportunityListFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Opportunities
            .AsNoTracking()
            .Where(o => o.OrganizationId == organizationId);

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == filter.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Stage))
        {
            var stage = filter.Stage.Trim().ToLowerInvariant();
            query = query.Where(o => o.Stage == stage);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var normSearch = OpportunityNormalizer.CollapseWhitespace(filter.Search);
            query = query.Where(o => o.NormalizedTitle.Contains(normSearch) || o.Code.Contains(normSearch));
        }

        // Total count before cursor/page slice
        var totalCount = await query.CountAsync(cancellationToken);

        // Dynamic Sorting
        var isDesc = string.Equals(filter.SortOrder, OpportunitySortOrder.Desc, StringComparison.OrdinalIgnoreCase);
        query = filter.SortBy?.ToLowerInvariant() switch
        {
            OpportunitySortKey.Code => isDesc
                ? query.OrderByDescending(o => o.Code).ThenBy(o => o.Id)
                : query.OrderBy(o => o.Code).ThenBy(o => o.Id),
            OpportunitySortKey.Title => isDesc
                ? query.OrderByDescending(o => o.NormalizedTitle).ThenBy(o => o.Id)
                : query.OrderBy(o => o.NormalizedTitle).ThenBy(o => o.Id),
            OpportunitySortKey.Stage => isDesc
                ? query.OrderByDescending(o => o.Stage).ThenBy(o => o.Id)
                : query.OrderBy(o => o.Stage).ThenBy(o => o.Id),
            OpportunitySortKey.ExpectedBudget => isDesc
                ? query.OrderByDescending(o => o.ExpectedBudget).ThenBy(o => o.Id)
                : query.OrderBy(o => o.ExpectedBudget).ThenBy(o => o.Id),
            OpportunitySortKey.CreatedAt => isDesc
                ? query.OrderByDescending(o => o.CreatedAtUtc).ThenBy(o => o.Id)
                : query.OrderBy(o => o.CreatedAtUtc).ThenBy(o => o.Id),
            _ => isDesc
                ? query.OrderByDescending(o => o.NextActionAtUtc.HasValue ? 1 : 0)
                    .ThenByDescending(o => o.NextActionAtUtc)
                    .ThenBy(o => o.Id)
                : query.OrderBy(o => o.NextActionAtUtc.HasValue ? 0 : 1)
                    .ThenBy(o => o.NextActionAtUtc)
                    .ThenBy(o => o.Id)
        };

        var limit = filter.Limit;
        List<Opportunity> items;
        string? nextCursor = null;
        int currentPage = filter.Page ?? 1;

        if (filter.Page.HasValue)
        {
            var skip = (currentPage - 1) * limit;
            items = await query
                .Skip(skip)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        else
        {
            // Keyset Pagination cursor: NextActionAtUtc ASC NULLS LAST, Id ASC
            if (!string.IsNullOrWhiteSpace(filter.Cursor))
            {
                var decoded = OpportunityCursor.TryDecode(filter.Cursor);
                if (decoded != null)
                {
                    if (decoded.NextActionAtUtc.HasValue)
                    {
                        var cursorTime = decoded.NextActionAtUtc.Value;
                        var cursorId = decoded.Id;
                        query = query.Where(o =>
                            o.NextActionAtUtc == null ||
                            o.NextActionAtUtc > cursorTime ||
                            (o.NextActionAtUtc == cursorTime && o.Id > cursorId));
                    }
                    else
                    {
                        // cursor was on a null next_action_at_utc row: all previous rows with non-null were before it
                        var cursorId = decoded.Id;
                        query = query.Where(o => o.NextActionAtUtc == null && o.Id > cursorId);
                    }
                }
            }

            // Query limit + 1 items to determine if there's a next page
            items = await query
                .Take(limit + 1)
                .ToListAsync(cancellationToken);

            if (items.Count > limit)
            {
                var lastItem = items[limit - 1];
                nextCursor = OpportunityCursor.Encode(lastItem.NextActionAtUtc, lastItem.Id);
                items.RemoveAt(limit);
            }
        }

        var projections = items.Select(ToProjection).ToList();
        return new OpportunityPage(projections, nextCursor, totalCount, currentPage, limit);
    }

    public async Task<OpportunityProjection?> GetAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default)
    {
        var opp = await _db.Opportunities
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == opportunityId && o.OrganizationId == organizationId, cancellationToken);

        return opp != null ? ToProjection(opp) : null;
    }

    public async Task<IReadOnlyList<OpportunityStageHistoryProjection>> GetStageHistoryAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default)
    {
        var histories = await _db.OpportunityStageHistories
            .AsNoTracking()
            .Where(h => h.OpportunityId == opportunityId && h.OrganizationId == organizationId)
            .OrderByDescending(h => h.OccurredAtUtc)
            .ThenByDescending(h => h.Id)
            .Select(h => new OpportunityStageHistoryProjection(
                h.Id,
                h.OpportunityId,
                h.FromStage,
                h.ToStage,
                h.ReasonCode,
                h.Note,
                h.ActorUserId,
                h.OccurredAtUtc,
                h.PolicyVersion,
                h.TraceId))
            .ToListAsync(cancellationToken);

        return histories;
    }

    private static OpportunityProjection ToProjection(Opportunity o) => new(
        o.Id,
        o.Code,
        o.CustomerId,
        o.PrimarySiteId,
        o.BranchId,
        o.OwnerUserId,
        o.Title,
        o.ScopeSummary,
        o.WorkTypes,
        o.SourceCode,
        o.ExpectedBudget,
        o.CurrencyCode,
        o.TargetDecisionDate,
        o.NextActionAtUtc,
        o.NextActionNote,
        o.Stage,
        o.RowVersion,
        o.CreatedAtUtc);
}
