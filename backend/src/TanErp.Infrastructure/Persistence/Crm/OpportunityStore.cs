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
using TanErp.Application.Crm.Opportunities.WorkImages;
using TanErp.Application.Crm.Opportunities.WorkImages.AttachWorkImages;
using TanErp.Application.Crm.Opportunities.WorkImages.DetachWorkImage;

namespace TanErp.Infrastructure.Persistence.Crm;

public class OpportunityStore : IOpportunityStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly TanErp.Application.Files.IFileStore _fileStore;

    public OpportunityStore(
        AppDbContext db,
        IClock clock,
        TanErp.Application.Files.IFileStore fileStore)
    {
        _db = db;
        _clock = clock;
        _fileStore = fileStore;
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

            return Result<OpportunityProjection>.Success(await LoadFullProjectionAsync(opp, orgId, cancellationToken));
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

            return Result<OpportunityProjection>.Success(await LoadFullProjectionAsync(opp, orgId, cancellationToken));
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

            return Result<OpportunityProjection>.Success(await LoadFullProjectionAsync(opp, orgId, cancellationToken));
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

            return Result<OpportunityProjection>.Success(await LoadFullProjectionAsync(opp, orgId, cancellationToken));
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

            return Result<OpportunityProjection>.Success(await LoadFullProjectionAsync(opp, orgId, cancellationToken));
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
            : Result<OpportunityProjection>.Success(await LoadFullProjectionAsync(opportunity, organizationId, cancellationToken));
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

        var customerIds = items.Select(i => i.CustomerId).Distinct().ToList();
        var ownerIds = items.Select(i => i.OwnerUserId).Distinct().ToList();

        var customerMap = await _db.Customers
            .AsNoTracking()
            .Where(c => customerIds.Contains(c.Id) && c.OrganizationId == organizationId)
            .Select(c => new CustomerSummaryProjection(c.Id, c.Code, c.DisplayNameTh, c.DisplayNameEn, c.Status))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var ownerMap = await _db.Users
            .AsNoTracking()
            .Where(u => ownerIds.Contains(u.Id))
            .Select(u => new OwnerSummaryProjection(u.Id, u.DisplayName, u.Email))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var projections = items.Select(o => ToProjection(
            o,
            ownerMap.GetValueOrDefault(o.OwnerUserId),
            null,
            customerMap.GetValueOrDefault(o.CustomerId),
            null)).ToList();

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

        if (opp == null) return null;
        return await LoadFullProjectionAsync(opp, organizationId, cancellationToken);
    }

    private async Task<OpportunityProjection> LoadFullProjectionAsync(
        Opportunity opp,
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var owner = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == opp.OwnerUserId)
            .Select(u => new OwnerSummaryProjection(u.Id, u.DisplayName, u.Email))
            .FirstOrDefaultAsync(cancellationToken);

        var branch = await _db.Branches
            .AsNoTracking()
            .Where(b => b.Id == opp.BranchId && b.OrganizationId == organizationId)
            .Select(b => new BranchSummaryProjection(b.Id, b.Name))
            .FirstOrDefaultAsync(cancellationToken);

        var customer = await _db.Customers
            .AsNoTracking()
            .Where(c => c.Id == opp.CustomerId && c.OrganizationId == organizationId)
            .Select(c => new CustomerSummaryProjection(c.Id, c.Code, c.DisplayNameTh, c.DisplayNameEn, c.Status))
            .FirstOrDefaultAsync(cancellationToken);

        SiteSummaryProjection? primarySite = null;
        if (opp.PrimarySiteId.HasValue)
        {
            primarySite = await _db.Sites
                .AsNoTracking()
                .Where(s => s.Id == opp.PrimarySiteId.Value && s.OrganizationId == organizationId)
                .Select(s => new SiteSummaryProjection(s.Id, s.Label, s.AddressLine1, s.Subdistrict, s.District, s.Province, s.PostalCode))
                .FirstOrDefaultAsync(cancellationToken);
        }

        return ToProjection(opp, owner, branch, customer, primarySite);
    }

    public async Task<IReadOnlyList<OpportunityStageHistoryProjection>> GetStageHistoryAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default)
    {
        var rawHistories = await _db.OpportunityStageHistories
            .AsNoTracking()
            .Where(h => h.OpportunityId == opportunityId && h.OrganizationId == organizationId)
            .OrderByDescending(h => h.OccurredAtUtc)
            .ThenByDescending(h => h.Id)
            .ToListAsync(cancellationToken);

        var actorIds = rawHistories.Select(h => h.ActorUserId).Distinct().ToList();
        var actorMap = await _db.Users
            .AsNoTracking()
            .Where(u => actorIds.Contains(u.Id))
            .Select(u => new ActorSummaryProjection(u.Id, u.DisplayName))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var histories = rawHistories.Select(h => new OpportunityStageHistoryProjection(
            h.Id,
            h.OpportunityId,
            h.FromStage,
            h.ToStage,
            h.ReasonCode,
            h.Note,
            h.ActorUserId,
            h.OccurredAtUtc,
            h.PolicyVersion,
            h.TraceId,
            actorMap.GetValueOrDefault(h.ActorUserId))).ToList();

        return histories;
    }

    public async Task<Result<AttachWorkImagesResultProjection>> AttachWorkImagesAsync(
        RequestAccessContext access,
        AttachWorkImagesCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.work-images.attach";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Idempotency replay check
            var existingRecord = await _db.IdempotencyRecords
                .FirstOrDefaultAsync(
                    r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord != null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<AttachWorkImagesResultProjection>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different request payload."));
                }

                // Return existing attached items
                var reloadedOpp = await _db.Opportunities.AsNoTracking().FirstOrDefaultAsync(o => o.Id == command.OpportunityId && o.OrganizationId == orgId, cancellationToken);
                var existingItemsResult = await ListWorkImagesAsync(orgId, command.OpportunityId, null, 100, null, cancellationToken);
                return Result<AttachWorkImagesResultProjection>.Success(new AttachWorkImagesResultProjection(existingItemsResult.Value?.Items ?? [], reloadedOpp?.RowVersion ?? command.ExpectedVersion));
            }

            // 2. Load Opportunity
            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.Id == command.OpportunityId && o.OrganizationId == orgId, cancellationToken);

            if (opp == null)
            {
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // 3. Concurrency check & Open stage assertion
            try
            {
                opp.AssertCanAttachWorkImages(command.ExpectedVersion);
            }
            catch (OpportunityVersionException)
            {
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (OpportunityTransitionException)
            {
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_STATE", "Work images cannot be attached to a closed opportunity."));
            }

            // 4. Verify all files exist in files schema, belong to same org, match parent intent, and have verified status
            var fileIds = command.Images.Select(i => i.FileId).Distinct().ToList();
            var validationResult = await _fileStore.ValidateVerifiedFilesForParentAsync(
                orgId,
                access.ActorUserId,
                TanErp.Domain.Files.FileParentTypes.Opportunity,
                opp.Id,
                null,
                fileIds,
                cancellationToken);

            if (validationResult.IsFailure)
            {
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("OPPORTUNITY_IMAGE_NOT_READY", validationResult.Error.Message ?? "One or more images are not verified or ready to be attached."));
            }

            // 5. Determine display order baseline
            var currentMaxOrder = await _db.OpportunityWorkImages
                .Where(w => w.OrganizationId == orgId && w.OpportunityId == opp.Id && !w.IsDeleted)
                .Select(w => (int?)w.DisplayOrder)
                .MaxAsync(cancellationToken) ?? 0;

            var now = _clock.UtcNow;
            var createdItems = new List<OpportunityWorkImage>();
            for (var i = 0; i < command.Images.Count; i++)
            {
                var img = command.Images[i];
                var entity = new OpportunityWorkImage(
                    Guid.NewGuid(),
                    orgId,
                    opp.Id,
                    img.FileId,
                    opp.Stage,
                    img.Caption,
                    currentMaxOrder + i + 1,
                    now,
                    access.ActorUserId);

                _db.OpportunityWorkImages.Add(entity);
                createdItems.Add(entity);
            }

            // 6. Rotate Opportunity rowVersion
            opp.RotateRowVersion(command.ExpectedVersion);

            // 7. Add AuditEvent (IDs and stage only, no caption or URL)
            var attachedFileIdsJson = string.Join(",", createdItems.Select(c => $"\"{c.FileId:D}\""));
            var auditChanges = FormattableString.Invariant(
                $"{{\"stage\":\"{opp.Stage}\",\"attachedCount\":{createdItems.Count},\"fileIds\":[{attachedFileIdsJson}]}}");

            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.work-images-added",
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
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            // 10. Query actor display name for structured projection
            var actorUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == access.ActorUserId, cancellationToken);
            var actorSummary = new WorkImageUserSummaryProjection(access.ActorUserId, actorUser?.DisplayName ?? "System User");

            var projections = createdItems.Select(c => new OpportunityWorkImageProjection(
                c.Id,
                c.FileId,
                c.StageAtAttach,
                c.Caption,
                c.DisplayOrder,
                c.CreatedAtUtc,
                actorSummary)).ToList();

            return Result<AttachWorkImagesResultProjection>.Success(
                new AttachWorkImagesResultProjection(projections, opp.RowVersion));
        });
    }

    public async Task<Result<OpportunityWorkImagePageProjection>> ListWorkImagesAsync(
        Guid organizationId,
        Guid opportunityId,
        string? stage,
        int limit,
        string? cursor,
        CancellationToken cancellationToken = default)
    {
        var decodeResult = OpportunityWorkImageCursor.TryDecode(cursor);
        if (decodeResult.IsFailure)
        {
            return Result<OpportunityWorkImagePageProjection>.Failure(decodeResult.Error);
        }

        var cursorValue = decodeResult.Value;

        var query = _db.OpportunityWorkImages
            .AsNoTracking()
            .Where(w => w.OrganizationId == organizationId && w.OpportunityId == opportunityId && !w.IsDeleted);

        if (!string.IsNullOrWhiteSpace(stage))
        {
            var normalizedStage = stage.Trim().ToLowerInvariant();
            query = query.Where(w => w.StageAtAttach == normalizedStage);
        }

        if (cursorValue.HasValue)
        {
            var (cursorCreatedAt, cursorId) = cursorValue.Value;
            query = query.Where(w => w.CreatedAtUtc < cursorCreatedAt || (w.CreatedAtUtc == cursorCreatedAt && w.Id < cursorId));
        }

        var fetchLimit = Math.Clamp(limit, 1, 100);
        var images = await query
            .OrderByDescending(w => w.CreatedAtUtc)
            .ThenByDescending(w => w.Id)
            .Take(fetchLimit + 1)
            .ToListAsync(cancellationToken);

        string? nextCursor = null;
        if (images.Count > fetchLimit)
        {
            var lastItem = images[fetchLimit - 1];
            nextCursor = OpportunityWorkImageCursor.Encode(lastItem.CreatedAtUtc, lastItem.Id);
            images = images.Take(fetchLimit).ToList();
        }

        var userIds = images.Select(w => w.CreatedByUserId).Distinct().ToList();
        var userMap = await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

        var projections = images.Select(w => new OpportunityWorkImageProjection(
            w.Id,
            w.FileId,
            w.StageAtAttach,
            w.Caption,
            w.DisplayOrder,
            w.CreatedAtUtc,
            new WorkImageUserSummaryProjection(w.CreatedByUserId, userMap.GetValueOrDefault(w.CreatedByUserId) ?? "Unknown User"))).ToList();

        return Result<OpportunityWorkImagePageProjection>.Success(
            new OpportunityWorkImagePageProjection(projections, nextCursor));
    }

    public async Task<Result<Guid>> DetachWorkImageAsync(
        RequestAccessContext access,
        DetachWorkImageCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.work-images.detach";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency
            var existingRecord = await _db.IdempotencyRecords
                .FirstOrDefaultAsync(
                    r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord != null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<Guid>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different request payload."));
                }
                var reloadedOpp = await _db.Opportunities.AsNoTracking().FirstOrDefaultAsync(o => o.Id == command.OpportunityId && o.OrganizationId == orgId, cancellationToken);
                return Result<Guid>.Success(reloadedOpp?.RowVersion ?? command.ExpectedVersion);
            }

            // 2. Load Opportunity
            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.Id == command.OpportunityId && o.OrganizationId == orgId, cancellationToken);

            if (opp == null)
            {
                return Result<Guid>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            // 3. Concurrency check
            try
            {
                opp.AssertCanAttachWorkImages(command.ExpectedVersion);
            }
            catch (OpportunityVersionException)
            {
                return Result<Guid>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }
            catch (OpportunityTransitionException)
            {
                return Result<Guid>.Failure(
                    new Error("OPPORTUNITY_INVALID_STATE", "Work images cannot be detached from a closed opportunity."));
            }

            // 4. Load WorkImage
            var workImage = await _db.OpportunityWorkImages
                .FirstOrDefaultAsync(
                    w => w.Id == command.WorkImageId && w.OpportunityId == opp.Id && w.OrganizationId == orgId && !w.IsDeleted,
                    cancellationToken);

            if (workImage == null)
            {
                return Result<Guid>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Work image not found."));
            }

            // 5. Soft delete (do not delete physical file)
            workImage.SoftDelete();

            // 6. Rotate Opportunity rowVersion
            opp.RotateRowVersion(command.ExpectedVersion);

            // 7. Add AuditEvent
            var now = _clock.UtcNow;
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.work-image-detached",
                "Opportunity",
                opp.Id.ToString(),
                now,
                command.TraceId,
                $"{{\"workImageId\":\"{workImage.Id:D}\",\"fileId\":\"{workImage.FileId:D}\"}}");
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
                return Result<Guid>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            return Result<Guid>.Success(opp.RowVersion);
        });
    }

    private static OpportunityProjection ToProjection(
        Opportunity o,
        OwnerSummaryProjection? owner = null,
        BranchSummaryProjection? branch = null,
        CustomerSummaryProjection? customer = null,
        SiteSummaryProjection? primarySite = null) => new(
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
        o.CreatedAtUtc,
        owner,
        branch,
        customer,
        primarySite);
}
