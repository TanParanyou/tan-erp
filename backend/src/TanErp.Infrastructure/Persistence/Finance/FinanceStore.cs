using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Finance;
using TanErp.Application.IdentityAccess.ApprovalLimits;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Finance;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Documents;

namespace TanErp.Infrastructure.Persistence.Finance;

public class FinanceStore : IFinanceStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly IAccountingConnector _connector;
    private readonly IApprovalAuthority _approvalAuthority;

    public FinanceStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, IAccountingConnector connector, IApprovalAuthority approvalAuthority)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _connector = connector;
        _approvalAuthority = approvalAuthority;
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

            decimal vatRate = 0m;
            string? taxInvoiceNumber = null;
            if (input.ApplyVat || (input.VatRate.HasValue && input.VatRate.Value > 0))
            {
                if (input.VatRate.HasValue)
                {
                    vatRate = input.VatRate.Value;
                }
                else
                {
                    var publishedPolicies = await _db.TaxPolicyVersions.AsNoTracking()
                        .Where(row => row.OrganizationId == orgId && row.Status == "published" &&
                            row.EffectiveFromUtc <= now && (row.EffectiveToUtc == null || row.EffectiveToUtc > now) &&
                            (row.BranchId == project.BranchId || row.BranchId == null))
                        .ToListAsync(ct);
                    var branchPolicy = publishedPolicies.FirstOrDefault(p => p.BranchId == project.BranchId);
                    var orgPolicy = publishedPolicies.FirstOrDefault(p => p.BranchId == null);
                    vatRate = (branchPolicy ?? orgPolicy)?.TaxRate ?? 0.07m;
                }

                try
                {
                    taxInvoiceNumber = await _numbers.GenerateAsync(orgId, DocumentTypes.TaxInvoices, project.BranchId, now, ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Fail<BillingProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate tax invoice document number.");
                }
            }

            // Wave D G-16 Credit Limit Gate
            var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == project.CustomerId && c.OrganizationId == orgId, ct);
            if (customer is not null && customer.CreditLimit.HasValue)
            {
                var overrides = await _db.CreditLimitOverrides.AsNoTracking()
                    .Where(o => o.OrganizationId == orgId && o.CustomerId == customer.Id && o.Status == CreditOverrideStatus.Approved)
                    .ToListAsync(ct);

                var effectiveCeiling = ArAgingCalculator.ComputeEffectiveCeiling(customer.CreditLimit, overrides, Today);

                var (_, _, currentTotalExposure) = await ComputeCustomerExposureAsync(orgId, customer.Id, ct);
                var currentUnbilledOnProject = Math.Max(0m, project.BaselineContractAmount - billed);
                var newUnbilledOnProject = Math.Max(0m, project.BaselineContractAmount - (billed + input.Amount));
                decimal estimatedGross;
                if (input.Lines != null && input.Lines.Count > 0)
                {
                    var linesVat = input.Lines.Sum(l => decimal.Round(l.TaxBaseAmount * (l.VatRate ?? vatRate), 2, MidpointRounding.AwayFromZero));
                    estimatedGross = input.Lines.Sum(l => l.TaxBaseAmount) + linesVat;
                }
                else
                {
                    var estimatedVat = vatRate > 0m ? decimal.Round(input.Amount * vatRate, 2, MidpointRounding.AwayFromZero) : 0m;
                    estimatedGross = input.Amount + estimatedVat;
                }

                var projectedExposure = currentTotalExposure - (currentUnbilledOnProject - newUnbilledOnProject) + estimatedGross;

                if (effectiveCeiling.HasValue && projectedExposure > effectiveCeiling.Value)
                {
                    return Fail<BillingProjection>("CREDIT_LIMIT_EXCEEDED",
                        $"Projected exposure of {projectedExposure:N2} THB exceeds effective credit ceiling of {effectiveCeiling.Value:N2} THB.");
                }
            }

            try
            {
                var billingId = Guid.NewGuid();
                List<BillingLine>? lines = null;
                if (input.Lines != null && input.Lines.Count > 0)
                {
                    lines = input.Lines.Select((l, idx) => new BillingLine(Guid.NewGuid(), orgId, billingId, idx + 1, l.Description, l.TaxBaseAmount, l.VatRate ?? vatRate)).ToList();
                }

                var billing = new BillingDocument(billingId, orgId, project.BranchId, project.Id, number, input.Kind, input.Description, input.Amount, input.DueDate, access.ActorUserId, now, vatRate, input.RetentionRate, taxInvoiceNumber, lines);
                _db.BillingDocuments.Add(billing);
                Enqueue(orgId, $"billing.issued:{billing.Id}", OutboxKind.BillingIssued, billing.Id, number, billing.Amount,
                    new
                    {
                        kind = OutboxKind.BillingIssued,
                        number,
                        project = project.Code,
                        billingKind = billing.Kind,
                        billing.Amount,
                        billing.Currency,
                        billing.ReferenceHash,
                        issuedAtUtc = billing.IssuedAtUtc,
                        taxInvoiceNumber = billing.TaxInvoiceNumber,
                        vatRate = billing.VatRate,
                        vatAmount = billing.VatAmount,
                        retentionRate = billing.RetentionRate,
                        retentionAmount = billing.RetentionAmount,
                        grossTotal = billing.GrossTotal
                    }, now);
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
            var billing = await _db.BillingDocuments
                .Include(b => b.Payments)
                .Include(b => b.Lines)
                .Include(b => b.AdjustmentNotes)
                .FirstOrDefaultAsync(b => b.Id == billingId && b.OrganizationId == orgId, ct);
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

    public Task<Result<BillingProjection>> ReleaseRetentionAsync(RequestAccessContext access, Guid billingId, Guid expectedVersion, string traceId, CancellationToken ct = default) =>
        MutateBillingAsync(access, billingId, expectedVersion, async (billing, now) =>
        {
            var warranties = await _db.Warranties.AsNoTracking()
                .Where(w => w.OrganizationId == access.OrganizationId && w.ProjectId == billing.ProjectId)
                .ToListAsync(ct);
            if (warranties.Count == 0 || warranties.Any(w => w.EndDate >= Today))
            {
                return Fail<BillingProjection>("RETENTION_WARRANTY_NOT_EXPIRED", "Retention can only be released after the project warranty has expired.");
            }

            billing.ReleaseRetention(access.ActorUserId, now);
            Audit(access, "billing.retention-released", "BillingDocument", billing.Id, traceId, new { number = billing.Number, retentionAmount = billing.RetentionAmount }, billing.RowVersion, now);
            return null;
        }, ct);

    public async Task<Result<BillingProjection>> CreateBillingNoteAsync(
        RequestAccessContext access, Guid billingId, BillingNoteInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "finance.billing-note.create";
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
            var billing = await _db.BillingDocuments.Include(b => b.Payments).Include(b => b.AdjustmentNotes).FirstOrDefaultAsync(b => b.Id == billingId && b.OrganizationId == orgId, ct);
            if (billing is null) return Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Billing not found.");
            if (billing.Status == BillingStatus.Voided) return Fail<BillingProjection>("BILLING_INVALID_STATE", "A voided billing cannot receive adjustment notes.");

            if (!BillingNoteType.All.Contains(input.NoteType)) return Fail<BillingProjection>("BILLING_FIELD_INVALID", "The note type is invalid.");
            var reason = input.Reason?.Trim() ?? string.Empty;
            if (reason.Length is 0 or > 500) return Fail<BillingProjection>("BILLING_FIELD_INVALID", "A reason of 1-500 characters is required.");
            if (input.TaxBaseDelta <= 0 || decimal.Round(input.TaxBaseDelta, 2) != input.TaxBaseDelta) return Fail<BillingProjection>("BILLING_FIELD_INVALID", "The tax base delta must be above zero with at most two decimals.");

            var now = _clock.UtcNow;
            if (input.NoteType == BillingNoteType.Credit)
            {
                var vatDelta = decimal.Round(input.TaxBaseDelta * billing.VatRate, 2, MidpointRounding.AwayFromZero);
                var totalDelta = input.TaxBaseDelta + vatDelta;
                if (input.TaxBaseDelta > (billing.Amount + billing.DebitNoteBaseAmount - billing.CreditNoteBaseAmount) ||
                    (billing.GrossTotal - totalDelta) < billing.PaidAmount)
                {
                    return Fail<BillingProjection>("BILLING_NOTE_EXCEEDS_ORIGINAL", "Credit note exceeds remaining billing amount.");
                }
            }

            var docType = input.NoteType == BillingNoteType.Credit ? DocumentTypes.CreditNotes : DocumentTypes.DebitNotes;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, docType, billing.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<BillingProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate billing note document number.");
            }

            try
            {
                var note = new BillingAdjustmentNote(Guid.NewGuid(), orgId, billing.BranchId, billing.Id, number, input.NoteType, reason, input.TaxBaseDelta, billing.VatRate, access.ActorUserId, now);
                _db.BillingAdjustmentNotes.Add(note);
                billing.AddAdjustmentNoteRecord(note);
                Audit(access, "billing-note.created", "BillingAdjustmentNote", note.Id, traceId, new { number = note.Number, noteType = note.NoteType, billing = billing.Number, totalDelta = note.TotalDelta }, billing.RowVersion, now);
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

    public async Task<Result<BillingProjection>> ApproveBillingNoteAsync(
        RequestAccessContext access, Guid billingId, Guid noteId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM finance.billing_documents WHERE id = {billingId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
            var billing = await _db.BillingDocuments
                .Include(b => b.Payments)
                .Include(b => b.AdjustmentNotes)
                .FirstOrDefaultAsync(b => b.Id == billingId && b.OrganizationId == orgId, ct);
            if (billing is null) return Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Billing not found.");
            if (billing.RowVersion != expectedVersion) return Fail<BillingProjection>("BILLING_VERSION_CONFLICT", "Billing version conflict.");

            var note = billing.AdjustmentNotes.FirstOrDefault(n => n.Id == noteId);
            if (note is null) return Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Billing note not found.");
            if (note.Status != BillingNoteStatus.PendingApproval) return Fail<BillingProjection>("BILLING_NOTE_INVALID_STATE", "Only pending notes can be approved.");
            if (note.CreatedByUserId == access.ActorUserId) return Fail<BillingProjection>("BILLING_NOTE_SELF_APPROVAL", "The creator cannot approve their own note.");

            var check = new ApprovalAuthorityCheck(orgId, access.MembershipId, TanErp.Domain.IdentityAccess.ApprovalDocumentType.ChangeOrder, billing.BranchId, note.TotalDelta, billing.Currency);
            var authError = await _approvalAuthority.EnsureAsync(check, ct);
            if (authError is not null) return Fail<BillingProjection>(authError.Code, authError.Message);

            var now = _clock.UtcNow;
            try
            {
                note.Approve(access.ActorUserId, now);
                billing.ApplyApprovedNote(note, now);
                Enqueue(orgId, $"billing-note.approved:{note.Id}", OutboxKind.BillingNoteApproved, note.Id, note.Number, note.TotalDelta,
                    new
                    {
                        kind = OutboxKind.BillingNoteApproved,
                        number = note.Number,
                        billing = billing.Number,
                        noteType = note.NoteType,
                        taxBaseDelta = note.TaxBaseDelta,
                        vatRate = note.VatRate,
                        vatDelta = note.VatDelta,
                        totalDelta = note.TotalDelta,
                        reason = note.Reason,
                        decidedAtUtc = note.DecidedAtUtc
                    }, now);
                Audit(access, "billing-note.approved", "BillingAdjustmentNote", note.Id, traceId, new { number = note.Number, billing = billing.Number, totalDelta = note.TotalDelta }, billing.RowVersion, now);
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

            string receiptNumber;
            try
            {
                receiptNumber = await _numbers.GenerateAsync(orgId, DocumentTypes.Receipts, billing.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<BillingProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate receipt document number.");
            }

            try
            {
                var payment = new Payment(Guid.NewGuid(), orgId, billing.Id, number, input.Amount, input.Method, reference, input.ReceivedDate, Today, access.ActorUserId, now, input.WhtRate, input.WhtType, input.WhtCertificateNumber, receiptNumber);
                billing.ApplyPayment(input.Amount, now);
                _db.Payments.Add(payment);
                Enqueue(orgId, $"payment.recorded:{payment.Id}", OutboxKind.PaymentRecorded, payment.Id, number, payment.Amount,
                    new
                    {
                        kind = OutboxKind.PaymentRecorded,
                        number,
                        billing = billing.Number,
                        payment.Amount,
                        billing.Currency,
                        payment.Method,
                        payment.Reference,
                        receivedDate = payment.ReceivedDate,
                        receiptNumber = payment.ReceiptNumber,
                        whtRate = payment.WhtRate,
                        whtAmount = payment.WhtAmount,
                        netCashReceived = payment.NetCashReceived,
                        whtType = payment.WhtType,
                        whtCertificateNumber = payment.WhtCertificateNumber
                    }, now);
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
        var b = await _db.BillingDocuments.AsNoTracking()
            .Include(x => x.Payments)
            .Include(x => x.Lines)
            .Include(x => x.AdjustmentNotes)
            .FirstOrDefaultAsync(x => x.Id == billingId && x.OrganizationId == organizationId, ct);
        if (b is null) return null;
        var project = await _db.Projects.AsNoTracking().Where(p => p.Id == b.ProjectId).Select(p => new FinanceProjectRef(p.Id, p.Code, p.Name)).FirstAsync(ct);
        var userIds = b.Payments.Select(p => p.RecordedByUserId)
            .Append(b.CreatedByUserId)
            .Concat(b.AdjustmentNotes.Select(n => n.CreatedByUserId))
            .Concat(b.AdjustmentNotes.Where(n => n.DecidedByUserId.HasValue).Select(n => n.DecidedByUserId!.Value));
        if (b.ReleasedRetentionByUserId.HasValue) userIds = userIds.Append(b.ReleasedRetentionByUserId.Value);
        var people = await PeopleAsync(userIds, ct);
        FinancePerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new FinancePerson(id, string.Empty);

        return new BillingProjection(
            b.Id, b.BranchId, b.Number, project, b.Kind, b.Description, b.Amount, b.PaidAmount, b.Outstanding, b.Currency, b.DueDate, b.Status, b.ReferenceHash,
            b.VoidReason, b.VoidedAtUtc, Person(b.CreatedByUserId), b.IssuedAtUtc, b.RowVersion,
            b.Payments.OrderBy(p => p.RecordedAtUtc).ThenBy(p => p.Id).Select(p => new PaymentProjection(
                p.Id, p.Number, p.Amount, p.Method, p.Reference, p.ReceivedDate, p.Status, p.ReversalReason, p.ReversedAtUtc, Person(p.RecordedByUserId), p.RecordedAtUtc,
                p.ReceiptNumber, p.WhtRate, p.WhtAmount, p.NetCashReceived, p.WhtType, p.WhtCertificateNumber)).ToList(),
            b.TaxInvoiceNumber, b.VatRate, b.VatAmount, b.GrossTotal, b.EffectiveReceivable, b.RetentionRate, b.RetentionAmount, b.RetentionStatus,
            b.ReleasedRetentionAtUtc, b.ReleasedRetentionByUserId.HasValue ? Person(b.ReleasedRetentionByUserId.Value) : null,
            b.CreditNoteBaseAmount, b.CreditNoteVatAmount, b.DebitNoteBaseAmount, b.DebitNoteVatAmount,
            b.Lines.OrderBy(l => l.LineNumber).Select(l => new BillingLineProjection(l.Id, l.LineNumber, l.Description, l.TaxBaseAmount, l.VatRate, l.VatAmount, l.TotalAmount)).ToList(),
            b.AdjustmentNotes.OrderBy(n => n.CreatedAtUtc).Select(n => new BillingAdjustmentNoteProjection(
                n.Id, n.Number, n.NoteType, n.Reason, n.TaxBaseDelta, n.VatRate, n.VatDelta, n.TotalDelta, n.Status,
                Person(n.CreatedByUserId), n.DecidedByUserId.HasValue ? Person(n.DecidedByUserId.Value) : null,
                n.CreatedAtUtc, n.DecidedAtUtc)).ToList());
    }

    public async Task<byte[]?> RenderTaxDocumentPdfAsync(Guid organizationId, Guid billingId, CancellationToken ct = default)
    {
        var b = await _db.BillingDocuments.AsNoTracking()
            .Include(x => x.Payments)
            .Include(x => x.Lines)
            .Include(x => x.AdjustmentNotes)
            .FirstOrDefaultAsync(x => x.Id == billingId && x.OrganizationId == organizationId, ct);
        if (b is null) return null;

        var org = await _db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId, ct);
        var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(br => br.Id == b.BranchId && br.OrganizationId == organizationId, ct);
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == b.ProjectId && p.OrganizationId == organizationId, ct);
        if (org is null || branch is null || project is null) return null;

        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == project.CustomerId && c.OrganizationId == organizationId, ct);

        var model = new TaxDocumentPdfModel(
            org, branch, project, customer, b,
            b.Lines.OrderBy(l => l.LineNumber).ToList(),
            b.AdjustmentNotes.OrderBy(n => n.CreatedAtUtc).ToList(),
            b.Payments.OrderBy(p => p.RecordedAtUtc).ToList());

        return QuestPdfTaxDocumentRenderer.Render(model);
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
        var allBillings = await _db.BillingDocuments.AsNoTracking()
            .Include(b => b.Payments)
            .Include(b => b.AdjustmentNotes)
            .Where(b => b.OrganizationId == organizationId)
            .ToListAsync(ct);
        foreach (var b in allBillings)
        {
            if (!queued.Contains($"billing.issued:{b.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.BillingIssued, b.Number, null, b.Amount, null));
            if (b.Status == BillingStatus.Voided && !queued.Contains($"billing.voided:{b.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.BillingVoided, b.Number, null, b.Amount, null));
            foreach (var note in b.AdjustmentNotes)
            {
                if (note.Status == BillingNoteStatus.Approved && !queued.Contains($"billing-note.approved:{note.Id}"))
                {
                    rows.Add(new ReconciliationRow("missing_message", OutboxKind.BillingNoteApproved, note.Number, null, note.TotalDelta, null));
                }
            }
            foreach (var p in b.Payments)
            {
                if (!queued.Contains($"payment.recorded:{p.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.PaymentRecorded, p.Number, null, p.Amount, null));
                if (p.Status == PaymentStatus.Reversed && !queued.Contains($"payment.reversed:{p.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.PaymentReversed, p.Number, null, p.Amount, null));
            }
        }

        var postedInvoices = await _db.SupplierInvoices.AsNoTracking()
            .Where(i => i.OrganizationId == organizationId
                && (i.Status == SupplierInvoiceStatus.Matched || i.Status == SupplierInvoiceStatus.Approved || i.Status == SupplierInvoiceStatus.Voided))
            .ToListAsync(ct);
        foreach (var inv in postedInvoices)
        {
            if (!queued.Contains($"supplier-invoice.posted:{inv.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.SupplierInvoicePosted, inv.Number, null, inv.TotalAmount, null));
            if (inv.Status == SupplierInvoiceStatus.Voided && !queued.Contains($"supplier-invoice.voided:{inv.Id}")) rows.Add(new ReconciliationRow("missing_message", OutboxKind.SupplierInvoiceVoided, inv.Number, null, inv.TotalAmount, null));
        }

        return new ReconciliationProjection(
            live.Sum(b => b.Amount), live.Sum(b => b.PaidAmount),
            Confirmed(OutboxKind.BillingIssued, OutboxKind.BillingVoided), Confirmed(OutboxKind.PaymentRecorded, OutboxKind.PaymentReversed),
            messages.Count(m => m.Status == OutboxStatus.Pending), messages.Count(m => m.Status == OutboxStatus.Failed), messages.Count(m => m.Status == OutboxStatus.Dead),
            rows.Take(200).ToList());
    }

    // ===== Wave D G-16 Credit Exposure & AR Aging ====================================================

    private async Task<(decimal OutstandingReceivables, decimal UnbilledAcceptedAmount, decimal TotalExposure)> ComputeCustomerExposureAsync(
        Guid organizationId, Guid customerId, CancellationToken ct)
    {
        var projects = await _db.Projects.AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && p.CustomerId == customerId && p.Status != ProjectStatus.Cancelled)
            .ToListAsync(ct);

        var projectIds = projects.Select(p => p.Id).ToList();

        var billings = await _db.BillingDocuments.AsNoTracking()
            .Where(b => b.OrganizationId == organizationId && projectIds.Contains(b.ProjectId) && b.Status != BillingStatus.Voided)
            .ToListAsync(ct);

        var outstandingReceivables = billings.Sum(b => b.Outstanding);

        var billingsByProject = billings.GroupBy(b => b.ProjectId).ToDictionary(g => g.Key, g => g.Sum(b => b.Amount));
        var unbilledOnProjects = projects.Sum(p =>
        {
            var billedAmount = billingsByProject.TryGetValue(p.Id, out var sum) ? sum : 0m;
            return Math.Max(0m, p.BaselineContractAmount - billedAmount);
        });

        var projectQuotationIds = projects.Select(p => p.QuotationId).ToHashSet();
        var acceptedQuotations = await _db.Quotations.AsNoTracking()
            .Where(q => q.OrganizationId == organizationId && q.CustomerId == customerId && q.Status == QuotationStatus.Accepted)
            .ToListAsync(ct);

        var unbilledAcceptedQuotations = acceptedQuotations
            .Where(q => !projectQuotationIds.Contains(q.Id))
            .Sum(q => q.TotalAmount);

        var unbilledAcceptedAmount = unbilledOnProjects + unbilledAcceptedQuotations;
        var totalExposure = outstandingReceivables + unbilledAcceptedAmount;

        return (outstandingReceivables, unbilledAcceptedAmount, totalExposure);
    }

    private static CreditOverrideProjection ToOverrideProjection(
        CreditLimitOverrideRequest o, string customerName, IReadOnlyDictionary<Guid, FinancePerson> people)
    {
        var createdBy = people.TryGetValue(o.CreatedByUserId, out var c) ? c : new FinancePerson(o.CreatedByUserId, string.Empty);
        FinancePerson? decidedBy = o.DecidedByUserId.HasValue && people.TryGetValue(o.DecidedByUserId.Value, out var d) ? d : null;
        return new CreditOverrideProjection(
            o.Id,
            o.OrganizationId,
            o.BranchId,
            o.CustomerId,
            customerName,
            o.RequestedCeilingAmount,
            o.Reason,
            o.ExpiresOn,
            o.Status,
            createdBy,
            o.CreatedAtUtc,
            decidedBy,
            o.DecidedAtUtc,
            o.DecisionNote,
            o.RowVersion);
    }

    public async Task<CustomerCreditExposureProjection?> GetCustomerCreditExposureAsync(Guid organizationId, Guid customerId, CancellationToken ct = default)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == customerId && c.OrganizationId == organizationId, ct);
        if (customer is null) return null;

        var (outstandingReceivables, unbilledAcceptedAmount, totalExposure) = await ComputeCustomerExposureAsync(organizationId, customerId, ct);

        var overrides = await _db.CreditLimitOverrides.AsNoTracking()
            .Where(o => o.OrganizationId == organizationId && o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(ct);

        var userIds = overrides.Select(o => o.CreatedByUserId)
            .Concat(overrides.Where(o => o.DecidedByUserId.HasValue).Select(o => o.DecidedByUserId!.Value))
            .Distinct();
        var people = await PeopleAsync(userIds, ct);

        var overrideProjections = overrides.Select(o => ToOverrideProjection(o, customer.DisplayNameTh, people)).ToList();
        var activeOverrides = overrides.Where(o => o.IsEffectiveOn(Today)).ToList();

        var effectiveCeiling = ArAgingCalculator.ComputeEffectiveCeiling(customer.CreditLimit, activeOverrides, Today);
        decimal? availableCredit = effectiveCeiling.HasValue ? Math.Max(0m, effectiveCeiling.Value - totalExposure) : null;
        var isOverLimit = effectiveCeiling.HasValue && totalExposure > effectiveCeiling.Value;

        return new CustomerCreditExposureProjection(
            customer.Id,
            customer.DisplayNameTh,
            customer.CreditLimit,
            customer.CreditTermDays,
            outstandingReceivables,
            unbilledAcceptedAmount,
            totalExposure,
            effectiveCeiling,
            availableCredit,
            isOverLimit,
            overrideProjections.Where(o => o.Status == CreditOverrideStatus.Approved && (o.ExpiresOn == null || o.ExpiresOn >= Today)).ToList()
        );
    }

    public async Task<ArAgingReportProjection> GetArAgingAsync(Guid organizationId, DateOnly? asOfDate, Guid? customerId, CancellationToken ct = default)
    {
        var asOf = asOfDate ?? Today;

        var projectsQuery = _db.Projects.AsNoTracking().Where(p => p.OrganizationId == organizationId);
        if (customerId.HasValue)
        {
            projectsQuery = projectsQuery.Where(p => p.CustomerId == customerId.Value);
        }
        var projects = await projectsQuery.ToListAsync(ct);
        var projectIds = projects.Select(p => p.Id).ToList();
        var projectMap = projects.ToDictionary(p => p.Id);

        var customersQuery = _db.Customers.AsNoTracking().Where(c => c.OrganizationId == organizationId);
        if (customerId.HasValue)
        {
            customersQuery = customersQuery.Where(c => c.Id == customerId.Value);
        }
        var customers = await customersQuery.ToDictionaryAsync(c => c.Id, ct);

        var billings = await _db.BillingDocuments.AsNoTracking()
            .Where(b => b.OrganizationId == organizationId && projectIds.Contains(b.ProjectId) && b.Status != BillingStatus.Voided)
            .ToListAsync(ct);

        var customerBuckets = new Dictionary<Guid, (decimal Current, decimal B1_30, decimal B31_60, decimal B61_90, decimal BOver90, decimal Total)>();

        foreach (var b in billings)
        {
            if (b.Outstanding <= 0m) continue;
            if (!projectMap.TryGetValue(b.ProjectId, out var project)) continue;
            if (!customers.TryGetValue(project.CustomerId, out var customer)) continue;

            var daysOverdue = ArAgingCalculator.CalculateDaysOverdue(b.DueDate, b.IssuedAtUtc, customer.CreditTermDays, asOf);
            var bucket = ArAgingCalculator.ClassifyAgingBucket(daysOverdue);

            if (!customerBuckets.TryGetValue(customer.Id, out var acc))
            {
                acc = (0m, 0m, 0m, 0m, 0m, 0m);
            }

            acc.Total += b.Outstanding;
            switch (bucket)
            {
                case ArAgingBucket.Current:
                    acc.Current += b.Outstanding;
                    break;
                case ArAgingBucket.Bucket1To30:
                    acc.B1_30 += b.Outstanding;
                    break;
                case ArAgingBucket.Bucket31To60:
                    acc.B31_60 += b.Outstanding;
                    break;
                case ArAgingBucket.Bucket61To90:
                    acc.B61_90 += b.Outstanding;
                    break;
                case ArAgingBucket.BucketOver90:
                    acc.BOver90 += b.Outstanding;
                    break;
            }
            customerBuckets[customer.Id] = acc;
        }

        var customerRows = customerBuckets.Select(kv =>
        {
            var cust = customers[kv.Key];
            var val = kv.Value;
            return new ArAgingCustomerBucketProjection(
                cust.Id,
                cust.Code,
                cust.DisplayNameTh,
                val.Current,
                val.B1_30,
                val.B31_60,
                val.B61_90,
                val.BOver90,
                val.Total
            );
        }).OrderByDescending(r => r.TotalOutstanding).ThenBy(r => r.CustomerCode).ToList();

        var totalCurrent = customerRows.Sum(r => r.Current);
        var total1_30 = customerRows.Sum(r => r.Bucket1To30);
        var total31_60 = customerRows.Sum(r => r.Bucket31To60);
        var total61_90 = customerRows.Sum(r => r.Bucket61To90);
        var totalOver90 = customerRows.Sum(r => r.BucketOver90);
        var grandTotal = customerRows.Sum(r => r.TotalOutstanding);

        return new ArAgingReportProjection(
            asOf,
            totalCurrent,
            total1_30,
            total31_60,
            total61_90,
            totalOver90,
            grandTotal,
            customerRows
        );
    }

    public async Task<IReadOnlyList<CreditOverrideProjection>> ListCreditOverridesAsync(Guid organizationId, CreditOverrideListQuery query, CancellationToken ct = default)
    {
        var q = _db.CreditLimitOverrides.AsNoTracking().Where(o => o.OrganizationId == organizationId);
        if (query.CustomerId.HasValue) q = q.Where(o => o.CustomerId == query.CustomerId.Value);
        if (!string.IsNullOrWhiteSpace(query.Status)) q = q.Where(o => o.Status == query.Status);

        var rows = await q.OrderByDescending(o => o.CreatedAtUtc).ToListAsync(ct);
        if (rows.Count == 0) return Array.Empty<CreditOverrideProjection>();

        var custIds = rows.Select(r => r.CustomerId).Distinct().ToList();
        var customers = await _db.Customers.AsNoTracking().Where(c => custIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, c => c.DisplayNameTh, ct);

        var userIds = rows.Select(r => r.CreatedByUserId)
            .Concat(rows.Where(r => r.DecidedByUserId.HasValue).Select(r => r.DecidedByUserId!.Value))
            .Distinct();
        var people = await PeopleAsync(userIds, ct);

        return rows.Select(r => ToOverrideProjection(r, customers.TryGetValue(r.CustomerId, out var name) ? name : string.Empty, people)).ToList();
    }

    public async Task<Result<CreditOverrideProjection>> CreateCreditOverrideAsync(
        RequestAccessContext access, CreditOverrideInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "finance.credit_override.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash)
                    return Fail<CreditOverrideProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                if (Guid.TryParse(replay.ResourceId, out var replayedId))
                {
                    var existing = await _db.CreditLimitOverrides.AsNoTracking().FirstOrDefaultAsync(o => o.Id == replayedId && o.OrganizationId == orgId, ct);
                    if (existing is not null)
                    {
                        var cust = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == existing.CustomerId, ct);
                        var ppl = await PeopleAsync(new[] { existing.CreatedByUserId }, ct);
                        return Result<CreditOverrideProjection>.Success(ToOverrideProjection(existing, cust?.DisplayNameTh ?? string.Empty, ppl));
                    }
                }
            }

            var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == input.CustomerId && c.OrganizationId == orgId, ct);
            if (customer is null) return Fail<CreditOverrideProjection>("RESOURCE_NOT_FOUND", "Customer not found.");

            var branchId = access.BranchId ?? await _db.Branches.AsNoTracking()
                .Where(b => b.OrganizationId == orgId && b.IsActive)
                .Select(b => b.Id)
                .FirstOrDefaultAsync(ct);
            if (branchId == Guid.Empty)
            {
                return Fail<CreditOverrideProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");
            }

            var now = _clock.UtcNow;
            try
            {
                var overrideReq = new CreditLimitOverrideRequest(
                    Guid.NewGuid(),
                    orgId,
                    branchId,
                    input.CustomerId,
                    input.RequestedCeilingAmount,
                    input.Reason,
                    input.ExpiresOn,
                    access.ActorUserId,
                    now);

                _db.CreditLimitOverrides.Add(overrideReq);
                Audit(access, "credit_override.requested", "CreditLimitOverrideRequest", overrideReq.Id, traceId,
                    new { customerId = input.CustomerId, requestedCeiling = input.RequestedCeilingAmount, reason = input.Reason },
                    overrideReq.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, overrideReq.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                var ppl = await PeopleAsync(new[] { access.ActorUserId }, ct);
                return Result<CreditOverrideProjection>.Success(ToOverrideProjection(overrideReq, customer.DisplayNameTh, ppl));
            }
            catch (FinanceDomainException ex)
            {
                return Fail<CreditOverrideProjection>(ex);
            }
        });
    }

    public async Task<Result<CreditOverrideProjection>> ApproveCreditOverrideAsync(
        RequestAccessContext access, Guid overrideId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM finance.credit_limit_overrides WHERE id = {overrideId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);

            var overrideReq = await _db.CreditLimitOverrides.FirstOrDefaultAsync(o => o.Id == overrideId && o.OrganizationId == orgId, ct);
            if (overrideReq is null) return Fail<CreditOverrideProjection>("RESOURCE_NOT_FOUND", "Credit override request not found.");
            if (overrideReq.RowVersion != expectedVersion) return Fail<CreditOverrideProjection>("CONCURRENCY_CONFLICT", "Credit override version conflict.");

            if (overrideReq.Status != CreditOverrideStatus.PendingApproval)
                return Fail<CreditOverrideProjection>("CREDIT_OVERRIDE_INVALID_STATE", "Only pending credit override requests can be approved.");
            if (overrideReq.CreatedByUserId == access.ActorUserId)
                return Fail<CreditOverrideProjection>("CREDIT_OVERRIDE_SELF_APPROVAL", "Creator cannot approve their own credit override request.");

            var check = new ApprovalAuthorityCheck(
                orgId, access.MembershipId, TanErp.Domain.IdentityAccess.ApprovalDocumentType.ChangeOrder,
                overrideReq.BranchId, overrideReq.RequestedCeilingAmount, "THB");
            var authError = await _approvalAuthority.EnsureAsync(check, ct);
            if (authError is not null) return Fail<CreditOverrideProjection>(authError.Code, authError.Message);

            var now = _clock.UtcNow;
            try
            {
                overrideReq.Approve(access.ActorUserId, now);
                Audit(access, "credit_override.approved", "CreditLimitOverrideRequest", overrideReq.Id, traceId,
                    new { customerId = overrideReq.CustomerId, requestedCeiling = overrideReq.RequestedCeilingAmount },
                    overrideReq.RowVersion, now);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == overrideReq.CustomerId, ct);
                var people = await PeopleAsync(new[] { overrideReq.CreatedByUserId, access.ActorUserId }, ct);
                return Result<CreditOverrideProjection>.Success(ToOverrideProjection(overrideReq, customer?.DisplayNameTh ?? string.Empty, people));
            }
            catch (FinanceDomainException ex)
            {
                return Fail<CreditOverrideProjection>(ex);
            }
        });
    }

    // ===== Wave D G-17 Journal Export & COA Mapping ===================================================

    public async Task<IReadOnlyList<AccountMappingProjection>> ListAccountMappingsAsync(Guid organizationId, CancellationToken ct = default)
    {
        var mappings = await _db.AccountMappings.AsNoTracking()
            .Where(m => m.OrganizationId == organizationId)
            .OrderBy(m => m.Kind)
            .ToListAsync(ct);

        var people = await PeopleAsync(mappings.Select(m => m.UpdatedByUserId), ct);
        return mappings.Select(m => new AccountMappingProjection(
            m.Id,
            m.Kind,
            m.DebitAccountCode,
            m.DebitAccountName,
            m.CreditAccountCode,
            m.CreditAccountName,
            people.TryGetValue(m.UpdatedByUserId, out var p) ? p : new FinancePerson(m.UpdatedByUserId, string.Empty),
            m.UpdatedAtUtc,
            m.RowVersion)).ToList();
    }

    public async Task<Result<AccountMappingProjection>> UpsertAccountMappingAsync(
        RequestAccessContext access, AccountMappingInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var kind = input.Kind.Trim();
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM finance.account_mappings WHERE organization_id = {orgId} AND kind = {kind} FOR UPDATE").ToListAsync(ct);

            var mapping = await _db.AccountMappings.FirstOrDefaultAsync(m => m.OrganizationId == orgId && m.Kind == kind, ct);
            var now = _clock.UtcNow;

            try
            {
                if (mapping is null)
                {
                    mapping = new AccountMapping(
                        Guid.NewGuid(),
                        orgId,
                        kind,
                        input.DebitAccountCode,
                        input.DebitAccountName,
                        input.CreditAccountCode,
                        input.CreditAccountName,
                        access.ActorUserId,
                        now);
                    _db.AccountMappings.Add(mapping);
                }
                else
                {
                    mapping.Update(
                        input.DebitAccountCode,
                        input.DebitAccountName,
                        input.CreditAccountCode,
                        input.CreditAccountName,
                        access.ActorUserId,
                        now);
                }

                Audit(access, "finance.account_mapping.upserted", "AccountMapping", mapping.Id, traceId,
                    new { kind = mapping.Kind, debit = mapping.DebitAccountCode, credit = mapping.CreditAccountCode },
                    mapping.RowVersion, now);

                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                var people = await PeopleAsync(new[] { access.ActorUserId }, ct);
                var person = people.TryGetValue(access.ActorUserId, out var p) ? p : new FinancePerson(access.ActorUserId, string.Empty);

                return Result<AccountMappingProjection>.Success(new AccountMappingProjection(
                    mapping.Id,
                    mapping.Kind,
                    mapping.DebitAccountCode,
                    mapping.DebitAccountName,
                    mapping.CreditAccountCode,
                    mapping.CreditAccountName,
                    person,
                    mapping.UpdatedAtUtc,
                    mapping.RowVersion));
            }
            catch (FinanceDomainException ex)
            {
                return Fail<AccountMappingProjection>(ex);
            }
        });
    }

    public async Task<Result<JournalExportResult>> ExportJournalAsync(
        RequestAccessContext access, int batchSize, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var now = _clock.UtcNow;

            var batch = await _db.AccountingOutbox
                .FromSqlInterpolated($@"SELECT * FROM finance.accounting_outbox WHERE organization_id = {orgId} AND status IN ('pending', 'failed') ORDER BY created_at_utc, id LIMIT {batchSize} FOR UPDATE SKIP LOCKED")
                .ToListAsync(ct);

            var mappings = await _db.AccountMappings.AsNoTracking()
                .Where(m => m.OrganizationId == orgId)
                .ToDictionaryAsync(m => m.Kind, ct);

            var batchRef = $"JRN-{now.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}-{Guid.NewGuid():N}"[..20].ToUpperInvariant();
            var lines = new List<JournalExportLineProjection>();
            int exportedCount = 0;
            int deadCount = 0;

            foreach (var message in batch)
            {
                if (mappings.TryGetValue(message.Kind, out var mapping))
                {
                    message.MarkExported(batchRef, now);
                    exportedCount++;
                    lines.Add(new JournalExportLineProjection(
                        batchRef,
                        message.Id,
                        message.DedupeKey,
                        message.Kind,
                        message.ResourceNumber,
                        mapping.DebitAccountCode,
                        mapping.DebitAccountName,
                        mapping.CreditAccountCode,
                        mapping.CreditAccountName,
                        message.Amount,
                        "THB",
                        message.CreatedAtUtc));
                }
                else
                {
                    message.MarkDeadMissingMapping(message.Kind, now);
                    deadCount++;
                }
            }

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("BatchRef,DateUtc,DedupeKey,Kind,DocumentNumber,DebitAccountCode,DebitAccountName,CreditAccountCode,CreditAccountName,Amount,Currency");
            foreach (var l in lines)
            {
                csvBuilder.AppendLine(
                    $"{EscapeCsv(l.BatchRef)},{l.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)},{EscapeCsv(l.DedupeKey)},{EscapeCsv(l.Kind)},{EscapeCsv(l.ResourceNumber)},{EscapeCsv(l.DebitAccountCode)},{EscapeCsv(l.DebitAccountName)},{EscapeCsv(l.CreditAccountCode)},{EscapeCsv(l.CreditAccountName)},{l.Amount.ToString("F2", CultureInfo.InvariantCulture)},{l.Currency}");
            }

            if (batch.Count > 0)
            {
                Audit(access, "finance.outbox.journal-exported", "AccountingOutbox", Guid.Empty, traceId,
                    new { batchRef, exportedCount, deadCount }, null, now);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<JournalExportResult>.Success(new JournalExportResult(
                batchRef,
                exportedCount,
                deadCount,
                lines,
                csvBuilder.ToString()));
        });
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
