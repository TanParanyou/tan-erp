using Microsoft.EntityFrameworkCore;
using TanErp.Application.Organization.Administration;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Items;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Estimates;
using TanErp.Domain.Finance;
using TanErp.Domain.Inventory;
using TanErp.Domain.Mrp;
using TanErp.Domain.Procurement;
using TanErp.Domain.Production;
using TanErp.Domain.Projects;
using TanErp.Domain.QuickEstimates;
using TanErp.Domain.Service;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.OrganizationAdministration;

/// <summary>
/// The single definition of "open" work that keeps a branch from being deactivated (ADR 0019). Used by the
/// deactivation-check endpoint and by the guard inside the deactivation transaction, so the UI and the server cannot disagree.
/// </summary>
public sealed class BranchDependencyInspector
{
    private const string QuotationIssued = QuotationStatus.Issued;

    /// <summary>Entities whose open lifecycle blocks deactivation. Keys must match the queries in <see cref="InspectAsync"/>.</summary>
    public static readonly IReadOnlyDictionary<Type, string> CountedEntities = new Dictionary<Type, string>
    {
        [typeof(Estimate)] = BranchBlockerTypes.Estimates,
        [typeof(Quotation)] = BranchBlockerTypes.Quotations,
        [typeof(PurchaseOrder)] = BranchBlockerTypes.PurchaseOrders,
        [typeof(BillingDocument)] = BranchBlockerTypes.Billings,
        [typeof(WorkOrder)] = BranchBlockerTypes.WorkOrders,
        [typeof(Project)] = BranchBlockerTypes.Projects,
        [typeof(InstallationJob)] = BranchBlockerTypes.Installations,
        [typeof(ServiceRequest)] = BranchBlockerTypes.ServiceRequests,
        [typeof(SiteSurvey)] = BranchBlockerTypes.SiteSurveys,
        [typeof(Opportunity)] = BranchBlockerTypes.Opportunities,
        [typeof(QuickEstimate)] = BranchBlockerTypes.QuickEstimates,
        [typeof(MrpRun)] = BranchBlockerTypes.MrpRuns,
        [typeof(Warehouse)] = BranchBlockerTypes.Warehouses,
        [typeof(TanErp.Domain.Organization.Membership)] = BranchBlockerTypes.Memberships,
    };

    /// <summary>Types that carry BranchId but are not open work. Every entry needs a reason; the coverage test enforces it.</summary>
    public static readonly IReadOnlyDictionary<Type, string> ExcludedEntities = new Dictionary<Type, string>
    {
        [typeof(GoodsReceipt)] = "Posted immutable receipt with no status lifecycle.",
        [typeof(StockDocument)] = "Posted immutable stock document with no status lifecycle.",
        [typeof(CostRecord)] = "Item cost master data (optional BranchId) with no cancel transition; counting drafts could leave a branch undeactivatable. Business decision pending.",
        [typeof(ItemBranchAvailability)] = "Item availability configuration per branch, not an open document.",
        [typeof(CalculationPolicyVersion)] = "Versioned calculation policy configuration, not an open document.",
        [typeof(TaxPolicyVersion)] = "Versioned tax policy configuration, not an open document.",
        [typeof(DocumentSequenceCounter)] = "Document numbering counter, not a document.",
        [typeof(RolePermission)] = "Role grant configuration, not an open document.",
        [typeof(AuditEvent)] = "Append-only audit log entry, not an open document.",
    };

    private readonly AppDbContext _db;

    public BranchDependencyInspector(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<BranchBlocker>> InspectAsync(Guid organizationId, Guid branchId, CancellationToken ct)
    {
        var o = organizationId;
        var b = branchId;
        var found = new List<BranchBlocker>();

        // Queries run one after another on purpose: a DbContext is not safe for concurrent use.
        async Task Add(string type, Task<int> count)
        {
            var n = await count;
            if (n > 0) found.Add(new BranchBlocker(type, n));
        }

        string[] openPo = [PurchaseOrderStatus.Draft, PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Approved, PurchaseOrderStatus.PartiallyReceived];
        string[] openBilling = [BillingStatus.Issued, BillingStatus.PartiallyPaid];
        string[] openWorkOrder = [WorkOrderStatus.Draft, WorkOrderStatus.Released, WorkOrderStatus.InProgress];
        string[] openProject = [ProjectStatus.Planned, ProjectStatus.Active, ProjectStatus.OnHold, ProjectStatus.ReadyForHandover];
        string[] openInstallation = [InstallationStatus.Planned, InstallationStatus.InProgress, InstallationStatus.ReadyForHandover];
        string[] openServiceRequest = [ServiceRequestStatus.Open, ServiceRequestStatus.Scheduled, ServiceRequestStatus.InProgress, ServiceRequestStatus.Resolved];
        string[] openSurvey = [SiteSurveyStatus.Scheduled, SiteSurveyStatus.InProgress];
        string[] closedOpportunity = [OpportunityStage.Won, OpportunityStage.Lost, OpportunityStage.Cancelled];
        string[] closedEstimate = [EstimateStatus.Quoted, EstimateStatus.Cancelled];

        await Add(BranchBlockerTypes.Estimates, _db.Estimates.CountAsync(x => x.OrganizationId == o && x.BranchId == b && !closedEstimate.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Quotations, _db.Quotations.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.Status == QuotationIssued, ct));
        await Add(BranchBlockerTypes.PurchaseOrders, _db.PurchaseOrders.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openPo.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Billings, _db.BillingDocuments.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openBilling.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.WorkOrders, _db.WorkOrders.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openWorkOrder.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Projects, _db.Projects.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openProject.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Installations, _db.InstallationJobs.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openInstallation.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.ServiceRequests, _db.ServiceRequests.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openServiceRequest.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.SiteSurveys, _db.SiteSurveys.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openSurvey.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Opportunities, _db.Opportunities.CountAsync(x => x.OrganizationId == o && x.BranchId == b && !closedOpportunity.Contains(x.Stage), ct));
        await Add(BranchBlockerTypes.QuickEstimates, _db.QuickEstimates.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.Status != QuickEstimateStatus.Converted, ct));
        await Add(BranchBlockerTypes.MrpRuns, _db.MrpRecommendations
            .Where(r => r.OrganizationId == o && r.Status == MrpRecommendationStatus.Proposed && _db.MrpRuns.Any(run => run.Id == r.RunId && run.BranchId == b))
            .Select(r => r.RunId).Distinct().CountAsync(ct));
        await Add(BranchBlockerTypes.Warehouses, _db.Warehouses.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.Status == WarehouseStatus.Active, ct));
        await Add(BranchBlockerTypes.Memberships, _db.Memberships.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.IsActive, ct));

        return found;
    }
}
