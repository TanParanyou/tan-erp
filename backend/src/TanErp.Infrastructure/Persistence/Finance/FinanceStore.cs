using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Finance;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Finance;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Finance;

public class FinanceStore : IFinanceStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly IAccountingConnector _connector;

    public FinanceStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, IAccountingConnector connector)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _connector = connector;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> Fail<T>(FinanceDomainException ex) => Fail<T>(ex.Code, ex.Message);

    private DateOnly Today => DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime);

    private void Audit(RequestAccessContext access, string action, string resourceType, Guid resourceId, string traceId, object changes, Guid? rowVersionAfter, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, resourceType, resourceId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            rowVersionAfter: rowVersionAfter));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    private void Enqueue(Guid orgId, string dedupeKey, string kind, Guid resourceId, string number, decimal amount, object payload, DateTimeOffset now) =>
        _db.AccountingOutbox.Add(new AccountingOutboxMessage(Guid.NewGuid(), orgId, dedupeKey, kind, resourceId, number, amount, JsonSerializer.Serialize(payload), now));

    private async Task<Dictionary<Guid, FinancePerson>> PeopleAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var list = ids.Distinct().ToList();
        return await _db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new FinancePerson(u.Id, u.DisplayName), ct);
    }

    // ===== billing ===================================================================================

    public async Task<Result<BillingProjection>> CreateBillingAsync(
        RequestAccessContext access, BillingInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "finance.billing.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<BillingProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetBillingAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<BillingProjection>.Success(replayed);
            }

            // Serialise billing of one project so the contract cap cannot be exceeded by two requests at once.
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM projects.projects WHERE id = {input.ProjectId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ProjectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status == ProjectStatus.Cancelled) return Fail<BillingProjection>("BILLING_PROJECT_NOT_BILLABLE", "A cancelled project cannot be billed.");

            var billed = await _db.BillingDocuments.Where(b => b.OrganizationId == orgId && b.ProjectId == project.Id && b.Status != BillingStatus.Voided).SumAsync(b => (decimal?)b.Amount, ct) ?? 0m;
            if (billed + input.Amount > project.BaselineContractAmount)
            {
                return Fail<BillingProjection>("BILLING_EXCEEDS_CONTRACT", "The billed total would exceed the project's contract amount.");
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.BillingDocuments, project.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<BillingProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate billing document number.");
            }

            try
            {
                var billing = new BillingDocument(Guid.NewGuid(), orgId, project.BranchId, project.Id, number, input.Kind, input.Description, input.Amount, input.DueDate, access.ActorUserId, now);
                _db.BillingDocuments.Add(billing);
                Enqueue(orgId, $"billing.issued:{billing.Id}", OutboxKind.BillingIssued, billing.Id, number, billing.Amount,
                    new { kind = OutboxKind.BillingIssued, number, project = project.Code, billingKind = billing.Kind, billing.Amount, billing.Currency, billing.ReferenceHash, issuedAtUtc = billing.IssuedAtUtc }, now);
                Audit(access, "billing.issued", "BillingDocument", billing.Id, traceId, new { number, amount = billing.Amount, project = project.Code }, billing.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, billing.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<BillingProjection>.Success((await GetBillingAsync(orgId, billing.Id, ct))!);
            }
            catch (FinanceDomainException ex)
            {
                return Fail<BillingProjection>(ex);
            }
        });
    }

    private async Task<Result<BillingProjection>> MutateBillingAsync(
        RequestAccessContext access, Guid billingId, Guid expectedVersion, Func<BillingDocument, DateTimeOffset, Task<Result<BillingProjection>?>> mutate, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM finance.billing_documents WHERE id = {billingId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
            var billing = await _db.BillingDocuments.Include(b => b.Payments).FirstOrDefaultAsync(b => b.Id == billingId && b.OrganizationId == orgId, ct);
            if (billing is null) return Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Billing not found.");
            if (billing.RowVersion != expectedVersion) return Fail<BillingProjection>("BILLING_VERSION_CONFLICT", "Billing version conflict.");

            var now = _clock.UtcNow;
            try
            {
                var early = await mutate(billing, now);
                if (early is not null) return early;
            }
            catch (FinanceDomainException ex)
            {
                return Fail<BillingProjection>(ex);
            }

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<BillingProjection>("BILLING_VERSION_CONFLICT", "Billing version conflict.");
            }

            return Result<BillingProjection>.Success((await GetBillingAsync(orgId, billing.Id, ct))!);
        });
    }

    public Task<Result<BillingProjection>> VoidBillingAsync(RequestAccessContext access, Guid billingId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default) =>
        MutateBillingAsync(access, billingId, expectedVersion, (billing, now) =>
        {
            billing.Void(reason, access.ActorUserId, now);
            Enqueue(access.OrganizationId, $"billing.voided:{billing.Id}", OutboxKind.BillingVoided, billing.Id, billing.Number, billing.Amount,
                new { kind = OutboxKind.BillingVoided, number = billing.Number, billing.Amount, billing.Currency, billing.ReferenceHash, voidedAtUtc = now }, now);
            // The reason stays on the billing; the audit trail carries identifiers only.
            Audit(access, "billing.voided", "BillingDocument", billing.Id, traceId, new { number = billing.Number }, billing.RowVersion, now);
            return Task.FromResult<Result<BillingProjection>?>(null);
        }, ct);

    public async Task<Result<BillingProjection>> RecordPaymentAsync(
        RequestAccessContext access, Guid billingId, PaymentInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "finance.payment.record";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<BillingProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = await GetBillingAsync(orgId, billingId, ct);
                if (replayed is not null) return Result<BillingProjection>.Success(replayed);
            }

            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM finance.billing_documents WHERE id = {billingId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
            var billing = await _db.BillingDocuments.Include(b => b.Payments).FirstOrDefaultAsync(b => b.Id == billingId && b.OrganizationId == orgId, ct);
            if (billing is null) return Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Billing not found.");

            var reference = input.Reference.Trim();
            if (await _db.Payments.AnyAsync(p => p.OrganizationId == orgId && p.Reference == reference, ct))
            {
                return Fail<BillingProjection>("PAYMENT_DUPLICATE_REFERENCE", "A payment with this reference has already been recorded.");
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.Payments, billing.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<BillingProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate payment document number.");
            }

            try
            {
                var payment = new Payment(Guid.NewGuid(), orgId, billing.Id, number, input.Amount, input.Method, reference, input.ReceivedDate, Today, access.ActorUserId, now);
                billing.ApplyPayment(input.Amount, now);
                _db.Payments.Add(payment);
                Enqueue(orgId, $"payment.recorded:{payment.Id}", OutboxKind.PaymentRecorded, payment.Id, number, payment.Amount,
                    new { kind = OutboxKind.PaymentRecorded, number, billing = billing.Number, payment.Amount, billing.Currency, payment.Method, payment.Reference, receivedDate = payment.ReceivedDate }, now);
                Audit(access, "payment.recorded", "Payment", payment.Id, traceId, new { number, billing = billing.Number, amount = payment.Amount }, billing.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, billing.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (FinanceDomainException ex)
            {
                return Fail<BillingProjection>(ex);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return Fail<BillingProjection>("PAYMENT_DUPLICATE_REFERENCE", "A payment with this reference has already been recorded.");
            }

            return Result<BillingProjection>.Success((await GetBillingAsync(orgId, billing.Id, ct))!);
        });
    }

    public Task<Result<BillingProjection>> ReversePaymentAsync(RequestAccessContext access, Guid billingId, Guid paymentId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default) =>
        MutateBillingAsync(access, billingId, expectedVersion, (billing, now) =>
        {
            var payment = billing.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null) return Task.FromResult<Result<BillingProjection>?>(Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Payment not found."));
            payment.Reverse(reason, access.ActorUserId, now);
            billing.RemovePayment(payment.Amount, now);
            Enqueue(access.OrganizationId, $"payment.reversed:{payment.Id}", OutboxKind.PaymentReversed, payment.Id, payment.Number, payment.Amount,
                new { kind = OutboxKind.PaymentReversed, number = payment.Number, billing = billing.Number, payment.Amount, billing.Currency, payment.Reference, reversedAtUtc = now }, now);
            Audit(access, "payment.reversed", "Payment", payment.Id, traceId, new { number = payment.Number, billing = billing.Number }, billing.RowVersion, now);
            return Task.FromResult<Result<BillingProjection>?>(null);
        }, ct);

    public async Task<BillingProjection?> GetBillingAsync(Guid organizationId, Guid billingId, CancellationToken ct = default)
    {
        var b = await _db.BillingDocuments.AsNoTracking().Include(x => x.Payments).FirstOrDefaultAsync(x => x.Id == billingId && x.OrganizationId == organizationId, ct);
        if (b is null) return null;
        var project = await _db.Projects.AsNoTracking().Where(p => p.Id == b.ProjectId).Select(p => new FinanceProjectRef(p.Id, p.Code, p.Name)).FirstAsync(ct);
        var people = await PeopleAsync(b.Payments.Select(p => p.RecordedByUserId).Append(b.CreatedByUserId), ct);
        FinancePerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new FinancePerson(id, string.Empty);
        return new BillingProjection(
            b.Id, b.BranchId, b.Number, project, b.Kind, b.Description, b.Amount, b.PaidAmount, b.Outstanding, b.Currency, b.DueDate, b.Status, b.ReferenceHash,
            b.VoidReason, b.VoidedAtUtc, Person(b.CreatedByUserId), b.IssuedAtUtc, b.RowVersion,
            b.Payments.OrderBy(p => p.RecordedAtUtc).ThenBy(p => p.Id).Select(p => new PaymentProjection(
                p.Id, p.Number, p.Amount, p.Method, p.Reference, p.ReceivedDate, p.Status, p.ReversalReason, p.ReversedAtUtc, Person(p.RecordedByUserId), p.RecordedAtUtc)).ToList());
    }

    public async Task<PagedBillings> ListBillingsAsync(Guid organizationId, BillingListQuery query, CancellationToken ct = default)
    {
        var billings = _db.BillingDocuments.AsNoTracking().Where(b => b.OrganizationId == organizationId);
        if (query.Status is not null) billings = billings.Where(b => b.Status == query.Status);
        if (query.ProjectId.HasValue) billings = billings.Where(b => b.ProjectId == query.ProjectId);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            billings = billings.Where(b => b.Number.ToUpper().Contains(needle) || b.Description.ToUpper().Contains(needle) || _db.Projects.Any(p => p.Id == b.ProjectId && p.Code.ToUpper().Contains(needle)));
        }

        var total = await billings.CountAsync(ct);
        var rows = await billings.OrderByDescending(b => b.IssuedAtUtc).ThenBy(b => b.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var projectIds = rows.Select(r => r.ProjectId).Distinct().ToList();
        var projects = await _db.Projects.AsNoTracking().Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Code, ct);
        return new PagedBillings(rows.Select(b => new BillingListItemProjection(b.Id, b.Number, projects[b.ProjectId], b.Kind, b.Amount, b.PaidAmount, b.Status, b.DueDate, b.IssuedAtUtc)).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<ProjectBillingSummary?> GetProjectSummaryAsync(Guid organizationId, Guid projectId, CancellationToken ct = default)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == organizationId, ct);
        if (project is null) return null;
        var live = await _db.BillingDocuments.AsNoTracking().Where(b => b.OrganizationId == organizationId && b.ProjectId == projectId && b.Status != BillingStatus.Voided).ToListAsync(ct);
        var billed = live.Sum(b => b.Amount);
        var paid = live.Sum(b => b.PaidAmount);
        return new ProjectBillingSummary(new FinanceProjectRef(project.Id, project.Code, project.Name), project.BaselineContractAmount, billed, paid, billed - paid, project.BaselineContractAmount - billed);
    }

    // ===== accounting sync ===========================================================================

    private static OutboxMessageProjection ToProjection(AccountingOutboxMessage m) => new(
        m.Id, m.Kind, m.ResourceNumber, m.Amount, m.Status, m.Attempts, m.NextAttemptAtUtc, m.LastError, m.ExternalRef, m.ExternalAmount, m.ConfirmedAtUtc, m.CreatedAtUtc, m.SentAtUtc, m.RowVersion);

    public async Task<PagedOutbox> ListOutboxAsync(Guid organizationId, OutboxListQuery query, CancellationToken ct = default)
    {
        var messages = _db.AccountingOutbox.AsNoTracking().Where(m => m.OrganizationId == organizationId);
        if (query.Status is not null) messages = messages.Where(m => m.Status == query.Status);
        if (query.Kind is not null) messages = messages.Where(m => m.Kind == query.Kind);
        var total = await messages.CountAsync(ct);
        var rows = await messages.OrderByDescending(m => m.CreatedAtUtc).ThenBy(m => m.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedOutbox(rows.Select(ToProjection).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<Result<DispatchResult>> DispatchAsync(RequestAccessContext access, int batchSize, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var now = _clock.UtcNow;
            // SKIP LOCKED: two dispatchers running at once never pick up the same message.
            var due = await _db.AccountingOutbox
                .FromSqlInterpolated($@"SELECT * FROM finance.accounting_outbox WHERE organization_id = {orgId} AND status IN ('pending', 'failed') AND next_attempt_at_utc <= {now} ORDER BY created_at_utc, id LIMIT {batchSize} FOR UPDATE SKIP LOCKED")
                .ToListAsync(ct);

            int sent = 0, failed = 0, dead = 0;
            foreach (var message in due)
            {
                ConnectorResult result;
                try
                {
                    result = await _connector.SendAsync(new AccountingEnvelope(message.Id, message.DedupeKey, message.Kind, message.ResourceNumber, message.Amount, message.PayloadJson, message.Attempts + 1), ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Only the exception type is stored; messages from a remote system may carry customer data.
                    result = new ConnectorResult(false, null, $"CONNECTOR_EXCEPTION:{ex.GetType().Name}");
                }

                if (result.Success)
                {
                    message.MarkSent(result.ExternalRef, now);
                    sent++;
                }
                else
                {
                    message.MarkFailed(result.Error ?? "CONNECTOR_FAILED", now);
                    if (message.Status == OutboxStatus.Dead) dead++;
                    else failed++;
                }
            }

            if (due.Count > 0) Audit(access, "finance.outbox.dispatched", "AccountingOutbox", Guid.Empty, traceId, new { processed = due.Count, sent, failed, dead }, null, now);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<DispatchResult>.Success(new DispatchResult(due.Count, sent, failed, dead));
        });
    }

    public async Task<Result<OutboxMessageProjection>> ConfirmAsync(RequestAccessContext access, Guid messageId, string externalRef, decimal externalAmount, string traceId, CancellationToken ct = default)
    {
        var message = await _db.AccountingOutbox.FirstOrDefaultAsync(m => m.Id == messageId && m.OrganizationId == access.OrganizationId, ct);
        if (message is null) return Fail<OutboxMessageProjection>("RESOURCE_NOT_FOUND", "Outbox message not found.");
        var alreadyConfirmed = message.ConfirmedAtUtc.HasValue;
        var now = _clock.UtcNow;
        try
        {
            message.Confirm(externalRef, externalAmount, now);
        }
        catch (FinanceDomainException ex)
        {
            return Fail<OutboxMessageProjection>(ex);
        }

        // A repeated identical callback changes nothing and is not audited twice.
        if (!alreadyConfirmed) Audit(access, "finance.outbox.confirmed", "AccountingOutbox", message.Id, traceId, new { kind = message.Kind, number = message.ResourceNumber }, message.RowVersion, now);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<OutboxMessageProjection>("OUTBOX_CONFIRMATION_CONFLICT", "The message was confirmed concurrently; retry.");
        }

        return Result<OutboxMessageProjection>.Success(ToProjection(message));
    }

    public async Task<Result<OutboxMessageProjection>> RequeueAsync(RequestAccessContext access, Guid messageId, string traceId, CancellationToken ct = default)
    {
        var message = await _db.AccountingOutbox.FirstOrDefaultAsync(m => m.Id == messageId && m.OrganizationId == access.OrganizationId, ct);
        if (message is null) return Fail<OutboxMessageProjection>("RESOURCE_NOT_FOUND", "Outbox message not found.");
        var now = _clock.UtcNow;
        try
        {
            message.Requeue(now);
        }
        catch (FinanceDomainException ex)
        {
            return Fail<OutboxMessageProjection>(ex);
        }

        Audit(access, "finance.outbox.requeued", "AccountingOutbox", message.Id, traceId, new { kind = message.Kind, number = message.ResourceNumber }, message.RowVersion, now);
        await _db.SaveChangesAsync(ct);
        return Result<OutboxMessageProjection>.Success(ToProjection(message));
    }

    public async Task<ReconciliationProjection> ReconcileAsync(Guid organizationId, CancellationToken ct = default)
    {
        var live = await _db.BillingDocuments.AsNoTracking().Where(b => b.OrganizationId == organizationId && b.Status != BillingStatus.Voided).ToListAsync(ct);
        var messages = await _db.AccountingOutbox.AsNoTracking().Where(m => m.OrganizationId == organizationId).ToListAsync(ct);

        decimal Confirmed(string add, string subtract) =>
            messages.Where(m => m.ConfirmedAtUtc.HasValue && m.Kind == add).Sum(m => m.ExternalAmount ?? 0m)
            - messages.Where(m => m.ConfirmedAtUtc.HasValue && m.Kind == subtract).Sum(m => m.ExternalAmount ?? 0m);

        var rows = new List<ReconciliationRow>();
        foreach (var m in messages.OrderBy(m => m.CreatedAtUtc).ThenBy(m => m.Id))
        {
            if (m.Status is OutboxStatus.Pending or OutboxStatus.Failed or OutboxStatus.Dead) rows.Add(new ReconciliationRow("unsynced", m.Kind, m.ResourceNumber, m.Id, m.Amount, null));
            else if (!m.ConfirmedAtUtc.HasValue) rows.Add(new ReconciliationRow("awaiting_confirmation", m.Kind, m.ResourceNumber, m.Id, m.Amount, null));
            else if (m.ExternalAmount != m.Amount) rows.Add(new ReconciliationRow("amount_mismatch", m.Kind, m.ResourceNumber, m.Id, m.Amount, m.ExternalAmount));
        }

        // Every billing and payment event must have been queued; a gap means something bypassed the outbox.
        var queued = messages.Select(m => m.DedupeKey).ToHashSet(StringComparer.Ordinal);
        var allBillings = await _db.BillingDocuments.AsNoTracking().Include(b => b.Payments).Where(b => b.OrganizationId == organizationId).ToListAsync(ct);
        foreach (var b in allBillings)
        {
            if (!queued.Contains($"billing.issued:{b.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.BillingIssued, b.Number, null, b.Amount, null));
            if (b.Status == BillingStatus.Voided && !queued.Contains($"billing.voided:{b.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.BillingVoided, b.Number, null, b.Amount, null));
            foreach (var p in b.Payments)
            {
                if (!queued.Contains($"payment.recorded:{p.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.PaymentRecorded, p.Number, null, p.Amount, null));
                if (p.Status == PaymentStatus.Reversed && !queued.Contains($"payment.reversed:{p.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.PaymentReversed, p.Number, null, p.Amount, null));
            }
        }

        return new ReconciliationProjection(
            live.Sum(b => b.Amount), live.Sum(b => b.PaidAmount),
            Confirmed(OutboxKind.BillingIssued, OutboxKind.BillingVoided), Confirmed(OutboxKind.PaymentRecorded, OutboxKind.PaymentReversed),
            messages.Count(m => m.Status == OutboxStatus.Pending), messages.Count(m => m.Status == OutboxStatus.Failed), messages.Count(m => m.Status == OutboxStatus.Dead),
            rows.Take(200).ToList());
    }
}
