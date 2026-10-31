using System.Data;
using System.Globalization;
using Dapper;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Reporting;
using TanErp.Domain.Finance;
using TanErp.Domain.Service;

namespace TanErp.Infrastructure.Persistence.Reporting;

public class ReportingStore : IReportingStore
{
    private readonly AppDbContext _db;

    public ReportingStore(AppDbContext db)
    {
        _db = db;
    }

    private async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
        {
            await _db.Database.OpenConnectionAsync(ct);
        }
        return conn;
    }

    // ---------------------------------------------------------------------------------------------
    // 1. Pipeline & Win Rate
    // ---------------------------------------------------------------------------------------------

    private sealed class PipelineStageRow
    {
        public string Stage { get; init; } = string.Empty;
        public int OpportunityCount { get; init; }
        public decimal ExpectedBudgetTotal { get; init; }
    }

    public async Task<PipelineReportProjection> GetPipelineReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var stages = await QueryPipelineStagesAsync(organizationId, branchId, ct);
        var summary = CalculatePipelineSummary(stages);
        var stageProjections = stages.Select(s => new PipelineStageMetrics(s.Stage, s.OpportunityCount, s.ExpectedBudgetTotal)).ToList();
        return new PipelineReportProjection(summary, stageProjections);
    }

    public async Task<PipelineSummaryMetrics> GetPipelineSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var stages = await QueryPipelineStagesAsync(organizationId, branchId, ct);
        return CalculatePipelineSummary(stages);
    }

    private async Task<IReadOnlyList<PipelineStageRow>> QueryPipelineStagesAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT
                stage AS "Stage",
                COUNT(*)::int AS "OpportunityCount",
                COALESCE(SUM(expected_budget), 0) AS "ExpectedBudgetTotal"
            FROM crm.opportunities
            WHERE organization_id = @OrgId
              AND (@BranchId::uuid IS NULL OR branch_id = @BranchId)
            GROUP BY stage
            ORDER BY stage ASC
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var result = await conn.QueryAsync<PipelineStageRow>(cmd);
        return result.ToList();
    }

    private static PipelineSummaryMetrics CalculatePipelineSummary(IReadOnlyList<PipelineStageRow> stages)
    {
        var totalCount = stages.Sum(s => s.OpportunityCount);
        var totalBudget = stages.Sum(s => s.ExpectedBudgetTotal);

        var wonStage = stages.FirstOrDefault(s => string.Equals(s.Stage, "won", StringComparison.OrdinalIgnoreCase));
        var lostStage = stages.FirstOrDefault(s => string.Equals(s.Stage, "lost", StringComparison.OrdinalIgnoreCase));

        var wonCount = wonStage?.OpportunityCount ?? 0;
        var wonBudget = wonStage?.ExpectedBudgetTotal ?? 0m;
        var lostCount = lostStage?.OpportunityCount ?? 0;

        var openStages = stages.Where(s =>
            !string.Equals(s.Stage, "won", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(s.Stage, "lost", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(s.Stage, "cancelled", StringComparison.OrdinalIgnoreCase)).ToList();

        var openCount = openStages.Sum(s => s.OpportunityCount);
        var openBudget = openStages.Sum(s => s.ExpectedBudgetTotal);

        var closedWonLost = wonCount + lostCount;
        var winRatePercent = closedWonLost == 0
            ? 0m
            : Math.Round((decimal)wonCount * 100m / closedWonLost, 2, MidpointRounding.AwayFromZero);

        return new PipelineSummaryMetrics(
            totalCount,
            totalBudget,
            openCount,
            openBudget,
            wonCount,
            wonBudget,
            lostCount,
            winRatePercent);
    }

    // ---------------------------------------------------------------------------------------------
    // 2. Quotation Value
    // ---------------------------------------------------------------------------------------------

    private sealed class QuotationRowRecord
    {
        public Guid QuotationId { get; init; }
        public string Number { get; init; } = string.Empty;
        public string CustomerCode { get; init; } = string.Empty;
        public string CustomerName { get; init; } = string.Empty;
        public string BranchCode { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
        public DateTime? IssuedAtUtc { get; init; }
        public DateTime? AcceptedAtUtc { get; init; }

        public DateTimeOffset? IssuedAtDto => IssuedAtUtc.HasValue
            ? new DateTimeOffset(DateTime.SpecifyKind(IssuedAtUtc.Value, DateTimeKind.Utc))
            : null;

        public DateTimeOffset? AcceptedAtDto => AcceptedAtUtc.HasValue
            ? new DateTimeOffset(DateTime.SpecifyKind(AcceptedAtUtc.Value, DateTimeKind.Utc))
            : null;
    }

    public async Task<QuotationReportProjection> GetQuotationReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var rows = await QueryQuotationRowsAsync(organizationId, branchId, ct);
        var summary = CalculateQuotationSummary(rows);
        var rowProjections = rows.Select(r => new QuotationReportRow(
            r.QuotationId,
            r.Number,
            r.CustomerCode,
            r.CustomerName,
            r.BranchCode,
            r.Status,
            r.TotalAmount,
            r.IssuedAtDto,
            r.AcceptedAtDto)).ToList();

        return new QuotationReportProjection(summary, rowProjections);
    }

    public async Task<QuotationSummaryMetrics> GetQuotationSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var rows = await QueryQuotationRowsAsync(organizationId, branchId, ct);
        return CalculateQuotationSummary(rows);
    }

    private async Task<IReadOnlyList<QuotationRowRecord>> QueryQuotationRowsAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT
                q.id AS "QuotationId",
                q.number AS "Number",
                COALESCE(c.code, '') AS "CustomerCode",
                COALESCE(c.display_name_th, c.display_name_en, '') AS "CustomerName",
                COALESCE(b.branch_code, '') AS "BranchCode",
                q.status AS "Status",
                COALESCE(q.total_amount, 0) AS "TotalAmount",
                q.issued_at_utc AS "IssuedAtUtc",
                q.accepted_at_utc AS "AcceptedAtUtc"
            FROM commercial.quotations q
            LEFT JOIN crm.customers c ON c.organization_id = q.organization_id AND c.id = q.customer_id
            LEFT JOIN organization.branches b ON b.organization_id = q.organization_id AND b.id = q.branch_id
            WHERE q.organization_id = @OrgId
              AND (@BranchId::uuid IS NULL OR q.branch_id = @BranchId)
            ORDER BY q.issued_at_utc DESC NULLS LAST, q.id ASC
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var result = await conn.QueryAsync<QuotationRowRecord>(cmd);
        return result.ToList();
    }

    private static QuotationSummaryMetrics CalculateQuotationSummary(IReadOnlyList<QuotationRowRecord> rows)
    {
        var totalCount = rows.Count;

        var issuedRows = rows.Where(r =>
            !string.Equals(r.Status, "draft", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(r.Status, "voided", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(r.Status, "superseded", StringComparison.OrdinalIgnoreCase)).ToList();
        var issuedCount = issuedRows.Count;
        var issuedAmount = issuedRows.Sum(r => r.TotalAmount);

        var acceptedRows = rows.Where(r => string.Equals(r.Status, "accepted", StringComparison.OrdinalIgnoreCase) || r.AcceptedAtUtc.HasValue).ToList();
        var acceptedCount = acceptedRows.Count;
        var acceptedAmount = acceptedRows.Sum(r => r.TotalAmount);

        var acceptanceRatePercent = issuedCount == 0
            ? 0m
            : Math.Round((decimal)acceptedCount * 100m / issuedCount, 2, MidpointRounding.AwayFromZero);

        return new QuotationSummaryMetrics(
            totalCount,
            issuedCount,
            issuedAmount,
            acceptedCount,
            acceptedAmount,
            acceptanceRatePercent);
    }

    // ---------------------------------------------------------------------------------------------
    // 3. Project Budget vs Actual
    // ---------------------------------------------------------------------------------------------

    private sealed class ProjectBudgetRowRecord
    {
        public Guid ProjectId { get; init; }
        public string ProjectCode { get; init; } = string.Empty;
        public string ProjectName { get; init; } = string.Empty;
        public string BranchCode { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public decimal BaselineContractAmount { get; init; }
        public decimal BaselineBudget { get; init; }
        public decimal ApprovedChangeOrderBudget { get; init; }
        public decimal CommittedAmount { get; init; }
        public decimal ActualAmount { get; init; }
    }

    public async Task<ProjectBudgetReportProjection> GetProjectBudgetReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var rows = await QueryProjectBudgetRowsAsync(organizationId, branchId, ct);
        var calculatedRows = CalculateProjectBudgetRows(rows);
        var summary = CalculateProjectBudgetSummary(calculatedRows);
        return new ProjectBudgetReportProjection(summary, calculatedRows);
    }

    public async Task<ProjectBudgetSummaryMetrics> GetProjectBudgetSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var rows = await QueryProjectBudgetRowsAsync(organizationId, branchId, ct);
        var calculatedRows = CalculateProjectBudgetRows(rows);
        return CalculateProjectBudgetSummary(calculatedRows);
    }

    private async Task<IReadOnlyList<ProjectBudgetRowRecord>> QueryProjectBudgetRowsAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT
                p.id AS "ProjectId",
                p.code AS "ProjectCode",
                p.name AS "ProjectName",
                COALESCE(b.branch_code, '') AS "BranchCode",
                p.status AS "Status",
                COALESCE(p.baseline_contract_amount, 0) AS "BaselineContractAmount",
                COALESCE(p.baseline_budget_total, (
                    SELECT COALESCE(SUM(bl.amount), 0)
                    FROM projects.project_budget_lines bl
                    WHERE bl.organization_id = p.organization_id AND bl.project_id = p.id
                ), 0) AS "BaselineBudget",
                COALESCE((
                    SELECT SUM(co.budget_delta)
                    FROM projects.project_change_orders co
                    WHERE co.organization_id = p.organization_id AND co.project_id = p.id AND co.status = 'approved'
                ), 0) AS "ApprovedChangeOrderBudget",
                COALESCE((
                    SELECT SUM(cle.amount)
                    FROM projects.project_cost_ledger_entries cle
                    WHERE cle.organization_id = p.organization_id AND cle.project_id = p.id AND cle.entry_type = 'commitment'
                ), 0) AS "CommittedAmount",
                COALESCE((
                    SELECT SUM(cle.amount)
                    FROM projects.project_cost_ledger_entries cle
                    WHERE cle.organization_id = p.organization_id AND cle.project_id = p.id AND cle.entry_type = 'actual'
                ), 0) AS "ActualAmount"
            FROM projects.projects p
            LEFT JOIN organization.branches b ON b.organization_id = p.organization_id AND b.id = p.branch_id
            WHERE p.organization_id = @OrgId
              AND (@BranchId::uuid IS NULL OR p.branch_id = @BranchId)
              AND p.status <> 'cancelled'
            ORDER BY p.code ASC, p.id ASC
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var result = await conn.QueryAsync<ProjectBudgetRowRecord>(cmd);
        return result.ToList();
    }

    private static List<ProjectBudgetReportRow> CalculateProjectBudgetRows(IReadOnlyList<ProjectBudgetRowRecord> records)
    {
        var rows = new List<ProjectBudgetReportRow>(records.Count);
        foreach (var r in records)
        {
            var currentBudget = r.BaselineBudget + r.ApprovedChangeOrderBudget;
            var totalConsumed = r.CommittedAmount + r.ActualAmount;
            var varianceAmount = currentBudget - totalConsumed;
            var isOverBudget = totalConsumed > currentBudget;

            rows.Add(new ProjectBudgetReportRow(
                r.ProjectId,
                r.ProjectCode,
                r.ProjectName,
                r.BranchCode,
                r.Status,
                r.BaselineContractAmount,
                r.BaselineBudget,
                r.ApprovedChangeOrderBudget,
                currentBudget,
                r.CommittedAmount,
                r.ActualAmount,
                varianceAmount,
                isOverBudget));
        }
        return rows;
    }

    private static ProjectBudgetSummaryMetrics CalculateProjectBudgetSummary(IReadOnlyList<ProjectBudgetReportRow> rows)
    {
        var projectCount = rows.Count;
        var overBudgetCount = rows.Count(r => r.IsOverBudget);
        var totalBaselineBudget = rows.Sum(r => r.BaselineBudget);
        var totalApprovedChangeOrderBudget = rows.Sum(r => r.ApprovedChangeOrderBudget);
        var totalCurrentBudget = rows.Sum(r => r.CurrentBudget);
        var totalCommittedAmount = rows.Sum(r => r.CommittedAmount);
        var totalActualAmount = rows.Sum(r => r.ActualAmount);
        var totalVarianceAmount = rows.Sum(r => r.VarianceAmount);

        return new ProjectBudgetSummaryMetrics(
            projectCount,
            overBudgetCount,
            totalBaselineBudget,
            totalApprovedChangeOrderBudget,
            totalCurrentBudget,
            totalCommittedAmount,
            totalActualAmount,
            totalVarianceAmount);
    }

    // ---------------------------------------------------------------------------------------------
    // 4. Stock Value
    // ---------------------------------------------------------------------------------------------

    private sealed class StockValueRowRecord
    {
        public Guid WarehouseId { get; init; }
        public string WarehouseCode { get; init; } = string.Empty;
        public string WarehouseName { get; init; } = string.Empty;
        public string BranchCode { get; init; } = string.Empty;
        public Guid ItemId { get; init; }
        public string ItemCode { get; init; } = string.Empty;
        public string ItemNameTh { get; init; } = string.Empty;
        public string ItemNameEn { get; init; } = string.Empty;
        public decimal OnHand { get; init; }
        public decimal Reserved { get; init; }
        public decimal Available { get; init; }
        public decimal TotalValue { get; init; }
    }

    public async Task<StockValueReportProjection> GetStockValueReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var rows = await QueryStockValueRowsAsync(organizationId, branchId, ct);
        var summary = CalculateStockValueSummary(rows);
        var rowProjections = rows.Select(r => new StockValueReportRow(
            r.WarehouseId,
            r.WarehouseCode,
            r.WarehouseName,
            r.BranchCode,
            r.ItemId,
            r.ItemCode,
            r.ItemNameTh,
            r.ItemNameEn,
            r.OnHand,
            r.Reserved,
            r.Available,
            r.TotalValue)).ToList();

        return new StockValueReportProjection(summary, rowProjections);
    }

    public async Task<StockValueSummaryMetrics> GetStockValueSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var rows = await QueryStockValueRowsAsync(organizationId, branchId, ct);
        return CalculateStockValueSummary(rows);
    }

    private async Task<IReadOnlyList<StockValueRowRecord>> QueryStockValueRowsAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT
                sb.warehouse_id AS "WarehouseId",
                COALESCE(w.code, '') AS "WarehouseCode",
                COALESCE(w.name, '') AS "WarehouseName",
                COALESCE(b.branch_code, '') AS "BranchCode",
                sb.item_id AS "ItemId",
                COALESCE(i.code, '') AS "ItemCode",
                COALESCE(i.name->>'th', '') AS "ItemNameTh",
                COALESCE(i.name->>'en', '') AS "ItemNameEn",
                sb.on_hand AS "OnHand",
                sb.reserved AS "Reserved",
                (sb.on_hand - sb.reserved) AS "Available",
                sb.total_value AS "TotalValue"
            FROM inventory.stock_balances sb
            INNER JOIN inventory.warehouses w ON w.organization_id = sb.organization_id AND w.id = sb.warehouse_id
            INNER JOIN item_master.items i ON i.organization_id = sb.organization_id AND i.id = sb.item_id
            LEFT JOIN organization.branches b ON b.organization_id = w.organization_id AND b.id = w.branch_id
            WHERE sb.organization_id = @OrgId
              AND (@BranchId::uuid IS NULL OR w.branch_id = @BranchId)
            ORDER BY w.code ASC, i.code ASC, sb.id ASC
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var result = await conn.QueryAsync<StockValueRowRecord>(cmd);
        return result.ToList();
    }

    private static StockValueSummaryMetrics CalculateStockValueSummary(IReadOnlyList<StockValueRowRecord> rows)
    {
        var warehouseCount = rows.Select(r => r.WarehouseId).Distinct().Count();
        var skuCount = rows.Select(r => r.ItemId).Distinct().Count();
        var totalOnHand = rows.Sum(r => r.OnHand);
        var totalReserved = rows.Sum(r => r.Reserved);
        var totalValue = rows.Sum(r => r.TotalValue);

        return new StockValueSummaryMetrics(
            warehouseCount,
            skuCount,
            totalOnHand,
            totalReserved,
            totalValue);
    }

    // ---------------------------------------------------------------------------------------------
    // 5. AR Aging
    // ---------------------------------------------------------------------------------------------

    private sealed class ArAgingRawRowRecord
    {
        public Guid BillingDocumentId { get; init; }
        public string Number { get; init; } = string.Empty;
        public string ProjectCode { get; init; } = string.Empty;
        public string CustomerCode { get; init; } = string.Empty;
        public string CustomerName { get; init; } = string.Empty;
        public string BranchCode { get; init; } = string.Empty;
        public string? DueDateStr { get; init; }
        public DateTime IssuedAtUtc { get; init; }
        public int CreditTermDays { get; init; }
        public string Status { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public decimal VatAmount { get; init; }
        public decimal DebitNoteBaseAmount { get; init; }
        public decimal DebitNoteVatAmount { get; init; }
        public decimal CreditNoteBaseAmount { get; init; }
        public decimal CreditNoteVatAmount { get; init; }
        public decimal RetentionAmount { get; init; }
        public string RetentionStatus { get; init; } = string.Empty;
        public decimal PaidAmount { get; init; }

        public DateTimeOffset IssuedAtDto => new(DateTime.SpecifyKind(IssuedAtUtc, DateTimeKind.Utc));
    }

    public async Task<ArAgingReportProjection> GetArAgingReportAsync(Guid organizationId, Guid? branchId, DateOnly asOfDate, CancellationToken ct = default)
    {
        var rawRows = await QueryArAgingRawRowsAsync(organizationId, branchId, ct);
        var rows = ProcessArAgingRows(rawRows, asOfDate);
        var summary = CalculateArAgingSummary(rows, asOfDate);
        return new ArAgingReportProjection(summary, rows);
    }

    public async Task<ArAgingSummaryMetrics> GetArAgingSummaryAsync(Guid organizationId, Guid? branchId, DateOnly asOfDate, CancellationToken ct = default)
    {
        var rawRows = await QueryArAgingRawRowsAsync(organizationId, branchId, ct);
        var rows = ProcessArAgingRows(rawRows, asOfDate);
        return CalculateArAgingSummary(rows, asOfDate);
    }

    private async Task<IReadOnlyList<ArAgingRawRowRecord>> QueryArAgingRawRowsAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT
                b.id AS "BillingDocumentId",
                b.number AS "Number",
                COALESCE(p.code, '') AS "ProjectCode",
                COALESCE(c.code, '') AS "CustomerCode",
                COALESCE(c.display_name_th, c.display_name_en, '') AS "CustomerName",
                COALESCE(br.branch_code, '') AS "BranchCode",
                to_char(b.due_date, 'YYYY-MM-DD') AS "DueDateStr",
                b.issued_at_utc AS "IssuedAtUtc",
                COALESCE(c.credit_term_days, 0) AS "CreditTermDays",
                b.status AS "Status",
                b.amount AS "Amount",
                b.vat_amount AS "VatAmount",
                b.debit_note_base_amount AS "DebitNoteBaseAmount",
                b.debit_note_vat_amount AS "DebitNoteVatAmount",
                b.credit_note_base_amount AS "CreditNoteBaseAmount",
                b.credit_note_vat_amount AS "CreditNoteVatAmount",
                b.retention_amount AS "RetentionAmount",
                b.retention_status AS "RetentionStatus",
                b.paid_amount AS "PaidAmount"
            FROM finance.billing_documents b
            INNER JOIN projects.projects p ON p.organization_id = b.organization_id AND p.id = b.project_id
            LEFT JOIN crm.customers c ON c.organization_id = p.organization_id AND c.id = p.customer_id
            LEFT JOIN organization.branches br ON br.organization_id = b.organization_id AND br.id = b.branch_id
            WHERE b.organization_id = @OrgId
              AND b.status <> 'voided'
              AND (@BranchId::uuid IS NULL OR b.branch_id = @BranchId)
            ORDER BY b.due_date ASC NULLS LAST, b.id ASC
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var result = await conn.QueryAsync<ArAgingRawRowRecord>(cmd);
        return result.ToList();
    }

    private static List<ArAgingReportRow> ProcessArAgingRows(IReadOnlyList<ArAgingRawRowRecord> rawRows, DateOnly asOfDate)
    {
        var processed = new List<ArAgingReportRow>();
        foreach (var r in rawRows)
        {
            var grossTotal = r.Amount + r.VatAmount + r.DebitNoteBaseAmount + r.DebitNoteVatAmount - r.CreditNoteBaseAmount - r.CreditNoteVatAmount;
            var isRetentionHeld = string.Equals(r.RetentionStatus, RetentionHoldingStatus.Held, StringComparison.OrdinalIgnoreCase);
            var effectiveReceivable = grossTotal - (isRetentionHeld ? r.RetentionAmount : 0m);
            var outstanding = string.Equals(r.Status, BillingStatus.Voided, StringComparison.OrdinalIgnoreCase)
                ? 0m
                : Math.Max(0m, effectiveReceivable - r.PaidAmount);

            if (outstanding <= 0m)
            {
                continue;
            }

            DateOnly? dueDate = null;
            if (!string.IsNullOrWhiteSpace(r.DueDateStr) &&
                DateOnly.TryParseExact(r.DueDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                dueDate = parsedDate;
            }

            var daysOverdue = ArAgingCalculator.CalculateDaysOverdue(dueDate, r.IssuedAtDto, r.CreditTermDays, asOfDate);
            var bucket = ArAgingCalculator.ClassifyAgingBucket(daysOverdue);

            processed.Add(new ArAgingReportRow(
                r.BillingDocumentId,
                r.Number,
                r.ProjectCode,
                r.CustomerCode,
                r.CustomerName,
                r.BranchCode,
                dueDate,
                daysOverdue,
                bucket,
                grossTotal,
                r.PaidAmount,
                outstanding));
        }
        return processed;
    }

    private static ArAgingSummaryMetrics CalculateArAgingSummary(IReadOnlyList<ArAgingReportRow> rows, DateOnly asOfDate)
    {
        var current = rows.Where(r => r.AgingBucket == ArAgingBucket.Current).Sum(r => r.OutstandingAmount);
        var b1_30 = rows.Where(r => r.AgingBucket == ArAgingBucket.Bucket1To30).Sum(r => r.OutstandingAmount);
        var b31_60 = rows.Where(r => r.AgingBucket == ArAgingBucket.Bucket31To60).Sum(r => r.OutstandingAmount);
        var b61_90 = rows.Where(r => r.AgingBucket == ArAgingBucket.Bucket61To90).Sum(r => r.OutstandingAmount);
        var bOver90 = rows.Where(r => r.AgingBucket == ArAgingBucket.BucketOver90).Sum(r => r.OutstandingAmount);
        var totalOutstanding = current + b1_30 + b31_60 + b61_90 + bOver90;

        return new ArAgingSummaryMetrics(
            asOfDate,
            rows.Count,
            current,
            b1_30,
            b31_60,
            b61_90,
            bOver90,
            totalOutstanding);
    }

    // ---------------------------------------------------------------------------------------------
    // 6. Service Backlog & SLA
    // ---------------------------------------------------------------------------------------------

    private sealed class ServiceSlaRawRowRecord
    {
        public Guid ServiceRequestId { get; init; }
        public string Number { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string ProjectCode { get; init; } = string.Empty;
        public string BranchCode { get; init; } = string.Empty;
        public string Priority { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public bool InWarranty { get; init; }
        public DateTime? PausedAtUtc { get; init; }
        public long PausedSeconds { get; init; }
        public DateTime? ResolvedAtUtc { get; init; }
        public decimal ServiceChargeAmount { get; init; }
        public DateTime CreatedAtUtc { get; init; }

        public DateTimeOffset? PausedAtDto => PausedAtUtc.HasValue
            ? new DateTimeOffset(DateTime.SpecifyKind(PausedAtUtc.Value, DateTimeKind.Utc))
            : null;

        public DateTimeOffset? ResolvedAtDto => ResolvedAtUtc.HasValue
            ? new DateTimeOffset(DateTime.SpecifyKind(ResolvedAtUtc.Value, DateTimeKind.Utc))
            : null;

        public DateTimeOffset CreatedAtDto => new(DateTime.SpecifyKind(CreatedAtUtc, DateTimeKind.Utc));
    }

    public async Task<ServiceSlaReportProjection> GetServiceSlaReportAsync(Guid organizationId, Guid? branchId, DateTimeOffset nowUtc, CancellationToken ct = default)
    {
        var holidays = await QueryHolidaysAsync(organizationId, branchId, ct);
        var rawRows = await QueryServiceSlaRawRowsAsync(organizationId, branchId, ct);
        var rows = ProcessServiceSlaRows(rawRows, holidays, nowUtc);
        var summary = CalculateServiceSlaSummary(rows);
        return new ServiceSlaReportProjection(summary, rows);
    }

    public async Task<ServiceSlaSummaryMetrics> GetServiceSlaSummaryAsync(Guid organizationId, Guid? branchId, DateTimeOffset nowUtc, CancellationToken ct = default)
    {
        var holidays = await QueryHolidaysAsync(organizationId, branchId, ct);
        var rawRows = await QueryServiceSlaRawRowsAsync(organizationId, branchId, ct);
        var rows = ProcessServiceSlaRows(rawRows, holidays, nowUtc);
        return CalculateServiceSlaSummary(rows);
    }

    private async Task<IReadOnlySet<DateOnly>> QueryHolidaysAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT DISTINCT to_char(holiday_date, 'YYYY-MM-DD') AS "HolidayDateStr"
            FROM mrp.working_calendar_holidays
            WHERE organization_id = @OrgId
              AND (@BranchId::uuid IS NULL OR branch_id IS NULL OR branch_id = @BranchId)
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var holidayDateStrs = await conn.QueryAsync<string>(cmd);

        var holidaySet = new HashSet<DateOnly>();
        foreach (var s in holidayDateStrs)
        {
            if (!string.IsNullOrWhiteSpace(s) &&
                DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            {
                holidaySet.Add(d);
            }
        }
        return holidaySet;
    }

    private async Task<IReadOnlyList<ServiceSlaRawRowRecord>> QueryServiceSlaRawRowsAsync(Guid organizationId, Guid? branchId, CancellationToken ct)
    {
        const string sql = """
            SELECT
                sr.id AS "ServiceRequestId",
                sr.number AS "Number",
                sr.title AS "Title",
                COALESCE(p.code, '') AS "ProjectCode",
                COALESCE(b.branch_code, '') AS "BranchCode",
                sr.priority AS "Priority",
                sr.status AS "Status",
                sr.in_warranty AS "InWarranty",
                sr.paused_at_utc AS "PausedAtUtc",
                sr.paused_seconds AS "PausedSeconds",
                sr.resolved_at_utc AS "ResolvedAtUtc",
                COALESCE(sr.service_charge_amount, 0) AS "ServiceChargeAmount",
                sr.created_at_utc AS "CreatedAtUtc"
            FROM service.service_requests sr
            LEFT JOIN projects.projects p ON p.organization_id = sr.organization_id AND p.id = sr.project_id
            LEFT JOIN organization.branches b ON b.organization_id = sr.organization_id AND b.id = sr.branch_id
            WHERE sr.organization_id = @OrgId
              AND (@BranchId::uuid IS NULL OR sr.branch_id = @BranchId)
            ORDER BY sr.created_at_utc DESC, sr.id ASC
            """;

        var conn = await GetOpenConnectionAsync(ct);
        var cmd = new CommandDefinition(sql, new { OrgId = organizationId, BranchId = branchId }, commandTimeout: 15, cancellationToken: ct);
        var result = await conn.QueryAsync<ServiceSlaRawRowRecord>(cmd);
        return result.ToList();
    }

    private static List<ServiceSlaReportRow> ProcessServiceSlaRows(
        IReadOnlyList<ServiceSlaRawRowRecord> rawRows,
        IReadOnlySet<DateOnly> holidays,
        DateTimeOffset nowUtc)
    {
        var rows = new List<ServiceSlaReportRow>(rawRows.Count);
        foreach (var r in rawRows)
        {
            var eval = ServiceSlaCalculator.Evaluate(
                r.Priority,
                r.Status,
                r.CreatedAtDto,
                r.ResolvedAtDto,
                r.PausedAtDto,
                r.PausedSeconds,
                nowUtc,
                holidays);

            rows.Add(new ServiceSlaReportRow(
                r.ServiceRequestId,
                r.Number,
                r.Title,
                r.ProjectCode,
                r.BranchCode,
                r.Priority,
                r.Status,
                r.InWarranty,
                eval.SlaStatus,
                eval.ElapsedHours,
                eval.ResolveHours,
                r.ServiceChargeAmount,
                r.CreatedAtDto));
        }
        return rows;
    }

    private static ServiceSlaSummaryMetrics CalculateServiceSlaSummary(IReadOnlyList<ServiceSlaReportRow> rows)
    {
        var totalCount = rows.Count;
        var openBacklogRows = rows.Where(r =>
            !string.Equals(r.Status, ServiceRequestStatus.Resolved, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(r.Status, ServiceRequestStatus.Closed, StringComparison.OrdinalIgnoreCase)).ToList();

        var openBacklogCount = openBacklogRows.Count;
        var waitingCustomerCount = rows.Count(r => string.Equals(r.Status, ServiceRequestStatus.WaitingCustomer, StringComparison.OrdinalIgnoreCase));

        var slaWarningCount = openBacklogRows.Count(r => string.Equals(r.SlaStatus, SlaStatus.Warning, StringComparison.OrdinalIgnoreCase));
        var slaBreachedCount = openBacklogRows.Count(r => string.Equals(r.SlaStatus, SlaStatus.Breached, StringComparison.OrdinalIgnoreCase));

        var outOfWarrantyChargedTotal = rows.Where(r => !r.InWarranty).Sum(r => r.ServiceChargeAmount);

        return new ServiceSlaSummaryMetrics(
            totalCount,
            openBacklogCount,
            waitingCustomerCount,
            slaWarningCount,
            slaBreachedCount,
            outOfWarrantyChargedTotal);
    }
}
