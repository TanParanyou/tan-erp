using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Reporting;
using Xunit;

namespace TanErp.UnitTests.Reporting;

public class ReportingHandlerTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchA = Guid.NewGuid();
    private static readonly Guid BranchB = Guid.NewGuid();
    private static readonly Guid ActorUserId = Guid.NewGuid();
    private static readonly ReportingCaller Caller = new("uid-test", Guid.NewGuid(), "trace-1");

    private sealed class FakeAccessResolver : IRequestAccessResolver
    {
        public bool MembershipActive { get; set; } = true;
        public Guid? MembershipBranchId { get; set; }
        public HashSet<string> GrantedPermissions { get; } = new(StringComparer.Ordinal);
        public List<string> CheckedPermissions { get; } = new();

        public Task<Result<RequestAccessContext>> ResolveMembershipAsync(string firebaseUid, Guid membershipId, CancellationToken cancellationToken = default)
        {
            if (!MembershipActive)
            {
                return Task.FromResult(Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "Active membership required.")));
            }
            return Task.FromResult(Result<RequestAccessContext>.Success(new RequestAccessContext(
                ActorUserId, membershipId, OrgId, MembershipBranchId, string.Empty, "organization")));
        }

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            CheckedPermissions.Add(permissionKey);
            if (!MembershipActive)
            {
                return Task.FromResult(Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "Active membership required.")));
            }

            if (GrantedPermissions.Contains(permissionKey))
            {
                return Task.FromResult(Result<RequestAccessContext>.Success(new RequestAccessContext(
                    ActorUserId, membershipId, OrgId, MembershipBranchId, permissionKey, "organization")));
            }

            return Task.FromResult(Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "Permission denied.")));
        }
    }

    private sealed class FakeReportingStore : IReportingStore
    {
        public Guid? LastPassedBranchId { get; private set; }
        public Guid? LastPassedOrgId { get; private set; }
        public DateOnly? LastPassedAsOfDate { get; private set; }

        public Task<PipelineReportProjection> GetPipelineReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new PipelineReportProjection(
                new PipelineSummaryMetrics(5, 50000m, 3, 30000m, 2, 20000m, 0, 100m),
                new List<PipelineStageMetrics> { new("won", 2, 20000m) }));
        }

        public Task<PipelineSummaryMetrics> GetPipelineSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new PipelineSummaryMetrics(5, 50000m, 3, 30000m, 2, 20000m, 0, 100m));
        }

        public Task<QuotationReportProjection> GetQuotationReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new QuotationReportProjection(
                new QuotationSummaryMetrics(2, 2, 20000m, 1, 10000m, 50m),
                new List<QuotationReportRow>()));
        }

        public Task<QuotationSummaryMetrics> GetQuotationSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new QuotationSummaryMetrics(2, 2, 20000m, 1, 10000m, 50m));
        }

        public Task<ProjectBudgetReportProjection> GetProjectBudgetReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new ProjectBudgetReportProjection(
                new ProjectBudgetSummaryMetrics(1, 0, 1000m, 0m, 1000m, 200m, 300m, 500m),
                new List<ProjectBudgetReportRow>()));
        }

        public Task<ProjectBudgetSummaryMetrics> GetProjectBudgetSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new ProjectBudgetSummaryMetrics(1, 0, 1000m, 0m, 1000m, 200m, 300m, 500m));
        }

        public Task<StockValueReportProjection> GetStockValueReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new StockValueReportProjection(
                new StockValueSummaryMetrics(1, 2, 10m, 2m, 5000m),
                new List<StockValueReportRow>()));
        }

        public Task<StockValueSummaryMetrics> GetStockValueSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new StockValueSummaryMetrics(1, 2, 10m, 2m, 5000m));
        }

        public Task<ArAgingReportProjection> GetArAgingReportAsync(Guid organizationId, Guid? branchId, DateOnly asOfDate, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            LastPassedAsOfDate = asOfDate;
            return Task.FromResult(new ArAgingReportProjection(
                new ArAgingSummaryMetrics(asOfDate, 1, 100m, 0m, 0m, 0m, 0m, 100m),
                new List<ArAgingReportRow>()));
        }

        public Task<ArAgingSummaryMetrics> GetArAgingSummaryAsync(Guid organizationId, Guid? branchId, DateOnly asOfDate, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            LastPassedAsOfDate = asOfDate;
            return Task.FromResult(new ArAgingSummaryMetrics(asOfDate, 1, 100m, 0m, 0m, 0m, 0m, 100m));
        }

        public Task<ServiceSlaReportProjection> GetServiceSlaReportAsync(Guid organizationId, Guid? branchId, DateTimeOffset nowUtc, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new ServiceSlaReportProjection(
                new ServiceSlaSummaryMetrics(2, 1, 0, 0, 0, 0m),
                new List<ServiceSlaReportRow>()));
        }

        public Task<ServiceSlaSummaryMetrics> GetServiceSlaSummaryAsync(Guid organizationId, Guid? branchId, DateTimeOffset nowUtc, CancellationToken ct = default)
        {
            LastPassedOrgId = organizationId;
            LastPassedBranchId = branchId;
            return Task.FromResult(new ServiceSlaSummaryMetrics(2, 1, 0, 0, 0, 0m));
        }
    }

    [Fact]
    public async Task GetPipelineReport_WhenPermissionDenied_ReturnsFailure()
    {
        var access = new FakeAccessResolver();
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetPipelineReportAsync(Caller, null);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task GetPipelineReport_WhenBranchScopedAndRequestingDifferentBranch_ReturnsPermissionDenied()
    {
        var access = new FakeAccessResolver
        {
            MembershipBranchId = BranchA
        };
        access.GrantedPermissions.Add("opportunities.read");
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetPipelineReportAsync(Caller, BranchB);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task GetPipelineReport_WhenBranchScopedAndNoBranchRequested_ClampsToCallerBranch()
    {
        var access = new FakeAccessResolver
        {
            MembershipBranchId = BranchA
        };
        access.GrantedPermissions.Add("opportunities.read");
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetPipelineReportAsync(Caller, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(BranchA, store.LastPassedBranchId);
        Assert.Equal(OrgId, store.LastPassedOrgId);
    }

    [Fact]
    public async Task GetPipelineReport_WhenOrgWideCallerRequestsBranch_PassesRequestedBranch()
    {
        var access = new FakeAccessResolver
        {
            MembershipBranchId = null
        };
        access.GrantedPermissions.Add("opportunities.read");
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetPipelineReportAsync(Caller, BranchB);

        Assert.True(result.IsSuccess);
        Assert.Equal(BranchB, store.LastPassedBranchId);
    }

    [Fact]
    public async Task GetDashboardSummary_WhenMembershipInactive_ReturnsActiveMembershipRequired()
    {
        var access = new FakeAccessResolver
        {
            MembershipActive = false
        };
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetDashboardSummaryAsync(Caller, null, null);

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task GetDashboardSummary_WhenBranchScopedAndRequestingDifferentBranch_ReturnsPermissionDenied()
    {
        var access = new FakeAccessResolver
        {
            MembershipBranchId = BranchA
        };
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetDashboardSummaryAsync(Caller, BranchB, null);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task GetDashboardSummary_EvaluatesPermissionsIndependently_ReturnsPartialSections()
    {
        var access = new FakeAccessResolver
        {
            MembershipBranchId = null
        };
        // Grant only opportunities.read and inventory.read
        access.GrantedPermissions.Add("opportunities.read");
        access.GrantedPermissions.Add("inventory.read");

        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var result = await handler.GetDashboardSummaryAsync(Caller, null, new DateOnly(2026, 10, 31));

        Assert.True(result.IsSuccess);
        var dash = result.Value!;

        // Pipeline should be permitted
        Assert.True(dash.Pipeline.Permitted);
        Assert.NotNull(dash.Pipeline.Summary);
        Assert.Equal(5, dash.Pipeline.Summary.TotalCount);

        // StockValue should be permitted
        Assert.True(dash.StockValue.Permitted);
        Assert.NotNull(dash.StockValue.Summary);

        // Others should be redacted
        Assert.False(dash.Quotations.Permitted);
        Assert.Null(dash.Quotations.Summary);

        Assert.False(dash.ProjectsBudget.Permitted);
        Assert.Null(dash.ProjectsBudget.Summary);

        Assert.False(dash.ArAging.Permitted);
        Assert.Null(dash.ArAging.Summary);

        Assert.False(dash.ServiceSla.Permitted);
        Assert.Null(dash.ServiceSla.Summary);
    }

    [Fact]
    public async Task GetArAgingReport_PassesAsOfDateAndFilters()
    {
        var access = new FakeAccessResolver();
        access.GrantedPermissions.Add("billings.read");
        var store = new FakeReportingStore();
        var handler = new ReportingHandler(access, store);

        var asOf = new DateOnly(2026, 10, 31);
        var result = await handler.GetArAgingReportAsync(Caller, null, asOf);

        Assert.True(result.IsSuccess);
        Assert.Equal(asOf, store.LastPassedAsOfDate);
    }
}
