using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Reporting;

public class ReportingHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IReportingStore _store;

    public ReportingHandler(IRequestAccessResolver accessResolver, IReportingStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private async Task<Result<(RequestAccessContext Access, Guid? EffectiveBranchId)>> ResolveReportAccessAsync(
        ReportingCaller caller,
        string permissionKey,
        Guid? requestedBranchId,
        CancellationToken ct)
    {
        var accessResult = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permissionKey, ct);
        if (accessResult.IsFailure)
        {
            return Result<(RequestAccessContext Access, Guid? EffectiveBranchId)>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        if (access.BranchId.HasValue)
        {
            if (requestedBranchId.HasValue && requestedBranchId.Value != access.BranchId.Value)
            {
                return Fail<(RequestAccessContext Access, Guid? EffectiveBranchId)>(
                    "PERMISSION_DENIED", "Access is denied for the requested branch.");
            }
            return Result<(RequestAccessContext Access, Guid? EffectiveBranchId)>.Success((access, access.BranchId.Value));
        }

        return Result<(RequestAccessContext Access, Guid? EffectiveBranchId)>.Success((access, requestedBranchId));
    }

    public async Task<Result<DashboardSummaryProjection>> GetDashboardSummaryAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        DateOnly? asOfDate,
        CancellationToken ct = default)
    {
        var membershipResult = await _accessResolver.ResolveMembershipAsync(caller.FirebaseUid, caller.MembershipId, ct);
        if (membershipResult.IsFailure)
        {
            return Result<DashboardSummaryProjection>.Failure(membershipResult.Error);
        }

        var access = membershipResult.Value!;
        Guid? effectiveBranchId;
        if (access.BranchId.HasValue)
        {
            if (requestedBranchId.HasValue && requestedBranchId.Value != access.BranchId.Value)
            {
                return Fail<DashboardSummaryProjection>("PERMISSION_DENIED", "Access is denied for the requested branch.");
            }
            effectiveBranchId = access.BranchId.Value;
        }
        else
        {
            effectiveBranchId = requestedBranchId;
        }

        var orgId = access.OrganizationId;
        var effectiveAsOf = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var nowUtc = DateTimeOffset.UtcNow;

        // Check permissions independently and query store sequentially to share the DbConnection
        ReportSectionProjection<PipelineSummaryMetrics> pipelineSection;
        var pAccess = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "opportunities.read", ct);
        if (pAccess.IsSuccess)
        {
            var summary = await _store.GetPipelineSummaryAsync(orgId, effectiveBranchId, ct);
            pipelineSection = new(true, summary);
        }
        else
        {
            pipelineSection = new(false, null);
        }

        ReportSectionProjection<QuotationSummaryMetrics> quotationSection;
        var qAccess = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.read", ct);
        if (qAccess.IsSuccess)
        {
            var summary = await _store.GetQuotationSummaryAsync(orgId, effectiveBranchId, ct);
            quotationSection = new(true, summary);
        }
        else
        {
            quotationSection = new(false, null);
        }

        ReportSectionProjection<ProjectBudgetSummaryMetrics> projectBudgetSection;
        var pbAccess = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "projects.read", ct);
        if (pbAccess.IsSuccess)
        {
            var summary = await _store.GetProjectBudgetSummaryAsync(orgId, effectiveBranchId, ct);
            projectBudgetSection = new(true, summary);
        }
        else
        {
            projectBudgetSection = new(false, null);
        }

        ReportSectionProjection<StockValueSummaryMetrics> stockValueSection;
        var svAccess = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "inventory.read", ct);
        if (svAccess.IsSuccess)
        {
            var summary = await _store.GetStockValueSummaryAsync(orgId, effectiveBranchId, ct);
            stockValueSection = new(true, summary);
        }
        else
        {
            stockValueSection = new(false, null);
        }

        ReportSectionProjection<ArAgingSummaryMetrics> arAgingSection;
        var arAccess = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "billings.read", ct);
        if (arAccess.IsSuccess)
        {
            var summary = await _store.GetArAgingSummaryAsync(orgId, effectiveBranchId, effectiveAsOf, ct);
            arAgingSection = new(true, summary);
        }
        else
        {
            arAgingSection = new(false, null);
        }

        ReportSectionProjection<ServiceSlaSummaryMetrics> serviceSlaSection;
        var ssAccess = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "service-requests.read", ct);
        if (ssAccess.IsSuccess)
        {
            var summary = await _store.GetServiceSlaSummaryAsync(orgId, effectiveBranchId, nowUtc, ct);
            serviceSlaSection = new(true, summary);
        }
        else
        {
            serviceSlaSection = new(false, null);
        }

        return Result<DashboardSummaryProjection>.Success(new DashboardSummaryProjection(
            nowUtc,
            effectiveBranchId,
            pipelineSection,
            quotationSection,
            projectBudgetSection,
            stockValueSection,
            arAgingSection,
            serviceSlaSection));
    }

    public async Task<Result<PipelineReportProjection>> GetPipelineReportAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        CancellationToken ct = default)
    {
        var auth = await ResolveReportAccessAsync(caller, "opportunities.read", requestedBranchId, ct);
        if (auth.IsFailure) return Result<PipelineReportProjection>.Failure(auth.Error);
        var report = await _store.GetPipelineReportAsync(auth.Value.Access.OrganizationId, auth.Value.EffectiveBranchId, ct);
        return Result<PipelineReportProjection>.Success(report);
    }

    public async Task<Result<QuotationReportProjection>> GetQuotationReportAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        CancellationToken ct = default)
    {
        var auth = await ResolveReportAccessAsync(caller, "quotations.read", requestedBranchId, ct);
        if (auth.IsFailure) return Result<QuotationReportProjection>.Failure(auth.Error);
        var report = await _store.GetQuotationReportAsync(auth.Value.Access.OrganizationId, auth.Value.EffectiveBranchId, ct);
        return Result<QuotationReportProjection>.Success(report);
    }

    public async Task<Result<ProjectBudgetReportProjection>> GetProjectBudgetReportAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        CancellationToken ct = default)
    {
        var auth = await ResolveReportAccessAsync(caller, "projects.read", requestedBranchId, ct);
        if (auth.IsFailure) return Result<ProjectBudgetReportProjection>.Failure(auth.Error);
        var report = await _store.GetProjectBudgetReportAsync(auth.Value.Access.OrganizationId, auth.Value.EffectiveBranchId, ct);
        return Result<ProjectBudgetReportProjection>.Success(report);
    }

    public async Task<Result<StockValueReportProjection>> GetStockValueReportAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        CancellationToken ct = default)
    {
        var auth = await ResolveReportAccessAsync(caller, "inventory.read", requestedBranchId, ct);
        if (auth.IsFailure) return Result<StockValueReportProjection>.Failure(auth.Error);
        var report = await _store.GetStockValueReportAsync(auth.Value.Access.OrganizationId, auth.Value.EffectiveBranchId, ct);
        return Result<StockValueReportProjection>.Success(report);
    }

    public async Task<Result<ArAgingReportProjection>> GetArAgingReportAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        DateOnly? asOfDate,
        CancellationToken ct = default)
    {
        var auth = await ResolveReportAccessAsync(caller, "billings.read", requestedBranchId, ct);
        if (auth.IsFailure) return Result<ArAgingReportProjection>.Failure(auth.Error);
        var effectiveAsOf = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var report = await _store.GetArAgingReportAsync(auth.Value.Access.OrganizationId, auth.Value.EffectiveBranchId, effectiveAsOf, ct);
        return Result<ArAgingReportProjection>.Success(report);
    }

    public async Task<Result<ServiceSlaReportProjection>> GetServiceSlaReportAsync(
        ReportingCaller caller,
        Guid? requestedBranchId,
        CancellationToken ct = default)
    {
        var auth = await ResolveReportAccessAsync(caller, "service-requests.read", requestedBranchId, ct);
        if (auth.IsFailure) return Result<ServiceSlaReportProjection>.Failure(auth.Error);
        var report = await _store.GetServiceSlaReportAsync(auth.Value.Access.OrganizationId, auth.Value.EffectiveBranchId, DateTimeOffset.UtcNow, ct);
        return Result<ServiceSlaReportProjection>.Success(report);
    }
}
