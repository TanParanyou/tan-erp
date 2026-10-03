using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Persistence.Estimates;

public sealed class EstimateReviewQueueReader : IEstimateReviewQueueReader
{
    private readonly AppDbContext _db;

    public EstimateReviewQueueReader(AppDbContext db) => _db = db;

    public async Task<EstimateReviewQueuePage> ListAsync(
        Guid organizationId,
        Guid reviewerMembershipId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = from step in _db.EstimateApprovalSteps.AsNoTracking()
                    where step.OrganizationId == organizationId &&
                          step.ReviewerMembershipId == reviewerMembershipId &&
                          step.Status == EstimateApprovalStepStatus.Pending &&
                          !_db.EstimateApprovalSteps.Any(previous => previous.OrganizationId == step.OrganizationId &&
                              previous.EstimateApprovalRequestId == step.EstimateApprovalRequestId &&
                              previous.Status == EstimateApprovalStepStatus.Pending && previous.Sequence < step.Sequence)
                    join request in _db.EstimateApprovalRequests.AsNoTracking()
                        on new { Id = step.EstimateApprovalRequestId, step.OrganizationId }
                        equals new { request.Id, request.OrganizationId }
                    where request.Status == EstimateApprovalRequestStatus.Open
                    join estimate in _db.Estimates.AsNoTracking()
                        on new { Id = request.EstimateId, request.OrganizationId }
                        equals new { estimate.Id, estimate.OrganizationId }
                    join revision in _db.EstimateRevisions.AsNoTracking()
                        on new { Id = request.EstimateRevisionId, request.OrganizationId }
                        equals new { revision.Id, revision.OrganizationId }
                    join opportunity in _db.Opportunities.AsNoTracking()
                        on new { Id = estimate.OpportunityId, estimate.OrganizationId }
                        equals new { opportunity.Id, opportunity.OrganizationId }
                    join customer in _db.Customers.AsNoTracking()
                        on new { Id = estimate.CustomerId, estimate.OrganizationId }
                        equals new { customer.Id, customer.OrganizationId }
                    join branch in _db.Branches.AsNoTracking()
                        on new { Id = estimate.BranchId, estimate.OrganizationId }
                        equals new { branch.Id, branch.OrganizationId }
                    join requester in _db.Users.AsNoTracking()
                        on request.RequestedByUserId equals requester.Id
                    join reviewer in _db.Users.AsNoTracking()
                        on step.ReviewerUserId equals reviewer.Id
                    select new
                    {
                        Step = step,
                        Request = request,
                        Estimate = estimate,
                        Revision = revision,
                        Opportunity = opportunity,
                        Customer = customer,
                        Branch = branch,
                        Requester = requester,
                        Reviewer = reviewer
                    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(row =>
                row.Estimate.Number.ToLower().Contains(term) ||
                row.Opportunity.NormalizedTitle.Contains(term) ||
                row.Customer.NormalizedDisplayName.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(row => row.Request.RequestedAtUtc)
            .ThenByDescending(row => row.Request.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(row => new ReviewRow(
                row.Step,
                row.Request,
                row.Estimate,
                row.Revision,
                row.Opportunity,
                row.Customer,
                row.Branch,
                row.Requester,
                row.Reviewer))
            .ToListAsync(cancellationToken);

        var revisionIds = rows.Select(row => row.Revision.Id).ToArray();
        var revisions = revisionIds.Length == 0
            ? new Dictionary<Guid, EstimateRevision>()
            : await _db.EstimateRevisions.AsNoTracking()
                .Where(revision => revision.OrganizationId == organizationId && revisionIds.Contains(revision.Id))
                .Include(revision => revision.Sections)
                    .ThenInclude(section => section.WorkItems)
                        .ThenInclude(workItem => workItem.CostComponents)
                .ToDictionaryAsync(revision => revision.Id, cancellationToken);
        var estimateRevisionPairs = rows
            .Where(row => row.Revision.RevisionNo > 1)
            .Select(row => new { row.Revision.EstimateId, PreviousRevisionNo = row.Revision.RevisionNo - 1 })
            .Distinct()
            .ToArray();
        var previousEstimateIds = estimateRevisionPairs.Select(pair => pair.EstimateId).Distinct().ToArray();
        var previousRevisions = estimateRevisionPairs.Length == 0
            ? new Dictionary<string, EstimateRevision>()
            : await _db.EstimateRevisions.AsNoTracking()
                .Where(revision => revision.OrganizationId == organizationId && previousEstimateIds.Contains(revision.EstimateId))
                .Include(revision => revision.Sections)
                    .ThenInclude(section => section.WorkItems)
                        .ThenInclude(workItem => workItem.CostComponents)
                .ToDictionaryAsync(revision => $"{revision.EstimateId:N}:{revision.RevisionNo}", cancellationToken);

        var items = rows.Select(row =>
        {
            var revision = revisions[row.Revision.Id];
            var readiness = revision.EvaluateReadiness();
            var provisionalCostCount = revision.Sections
                .SelectMany(section => section.WorkItems)
                .SelectMany(workItem => workItem.CostComponents)
                .Count(component => component.IsProvisional);
            var costEvidence = revision.Sections
                .SelectMany(section => section.WorkItems)
                .SelectMany(workItem => workItem.CostComponents.Select(component => new EstimateReviewCostEvidence(
                    workItem.Code, workItem.DescriptionTh, component.Type, component.Description,
                    component.Quantity, component.UnitCode, component.UnitCost, component.Currency,
                    component.TotalCost, component.CostOrigin, component.ItemCodeSnapshot,
                    component.CostRecordVersion, component.CostSourceCodeSnapshot,
                    component.CostSourceReferenceSnapshot, component.CostEvidenceFileIdSnapshot,
                    component.CostRecordReasonSnapshot, component.ProvisionalReasonCode,
                    component.ProvisionalNote, component.IsProvisional)))
                .ToList();
            var priceOverrides = revision.Sections
                .SelectMany(section => section.WorkItems)
                .Where(workItem => workItem.SellingRuleType == SellingRuleType.FixedPrice)
                .Select(workItem => new EstimateReviewPriceOverride(
                    workItem.Code,
                    workItem.DescriptionTh,
                    workItem.SellingRuleValue,
                    workItem.SellingRuleReasonCode))
                .ToList();
            var previousRevisionKey = $"{revision.EstimateId:N}:{revision.RevisionNo - 1}";
            var revisionDiff = previousRevisions.TryGetValue(previousRevisionKey, out var previousRevision)
                ? CreateRevisionDiff(previousRevision, revision)
                : null;

            var routeDetails = ReadRouteDetails(row.Request.RouteSnapshotJson);
            return new EstimateReviewQueueItem(
                row.Request.Id,
                row.Estimate.Id,
                row.Estimate.Number,
                row.Estimate.RowVersion,
                revision.Id,
                revision.RevisionNo,
                revision.Status,
                revision.CalculationVersion,
                revision.Currency,
                revision.GrandTotal,
                revision.MarginRate,
                row.Opportunity.Id,
                new EstimateReviewQueueOpportunity(row.Opportunity.Id, row.Opportunity.Title),
                new EstimateReviewQueueCustomer(row.Customer.Id, row.Customer.DisplayNameTh, row.Customer.DisplayNameEn),
                new EstimateReviewQueueBranch(row.Branch.Id, row.Branch.Code, row.Branch.Name),
                new EstimateReviewQueuePerson(row.Requester.Id, row.Requester.DisplayName),
                row.Request.RequestedAtUtc,
                row.Request.SubmissionNote,
                row.Request.CalculationInputHash,
                row.Request.CalculationSnapshotHash,
                new EstimateReviewFrozenRoute(
                    row.Request.PolicyCode,
                    row.Request.PolicyVersion,
                    row.Request.RouteHash,
                    row.Step.ScopeType,
                    row.Step.ScopeId,
                    row.Step.PermissionKey,
                    new EstimateReviewQueueReviewer(row.Reviewer.Id, row.Reviewer.DisplayName, row.Step.ReviewerMembershipId),
                    routeDetails.Triggers,
                    routeDetails.Thresholds),
                readiness.Status,
                readiness.Reasons,
                priceOverrides,
                costEvidence,
                revisionDiff,
                provisionalCostCount);
        }).ToList();

        return new EstimateReviewQueuePage(items, totalCount, pageNumber, pageSize);
    }

    private static (IReadOnlyList<EstimateReviewApprovalTrigger> Triggers, EstimateReviewApprovalThresholds? Thresholds)
        ReadRouteDetails(string routeSnapshotJson)
    {
        using var document = JsonDocument.Parse(routeSnapshotJson);
        var root = document.RootElement;
        var triggers = new List<EstimateReviewApprovalTrigger>();
        if (root.TryGetProperty("triggerSnapshot", out var triggerArray) && triggerArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var trigger in triggerArray.EnumerateArray())
            {
                if (!trigger.TryGetProperty("code", out var code) || code.ValueKind != JsonValueKind.String)
                    continue;
                triggers.Add(new EstimateReviewApprovalTrigger(
                    code.GetString()!, ReadNullableDecimal(trigger, "actualValue"),
                    ReadNullableDecimal(trigger, "thresholdValue"), ReadNullableString(trigger, "unit")));
            }
        }

        EstimateReviewApprovalThresholds? thresholds = null;
        if (root.TryGetProperty("thresholdSnapshot", out var threshold) && threshold.ValueKind == JsonValueKind.Object)
        {
            thresholds = new EstimateReviewApprovalThresholds(
                ReadNullableDecimal(threshold, "managerAmountLimit"),
                ReadNullableDecimal(threshold, "financialAmountLimit"),
                ReadNullableDecimal(threshold, "directorAmountLimit"),
                ReadNullableDecimal(threshold, "checkerMinimumMarginRate"),
                ReadNullableDecimal(threshold, "directorMinimumMarginRate"),
                ReadNullableDecimal(threshold, "discountLimitRate"));
        }
        return (triggers, thresholds);
    }

    private static decimal? ReadNullableDecimal(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
            ? value.GetDecimal()
            : null;

    private static string? ReadNullableString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static EstimateReviewRevisionDiff CreateRevisionDiff(EstimateRevision previous, EstimateRevision current)
    {
        var previousItems = WorkItemSignatures(previous);
        var currentItems = WorkItemSignatures(current);
        var added = currentItems.Keys.Except(previousItems.Keys, StringComparer.Ordinal).Count();
        var removed = previousItems.Keys.Except(currentItems.Keys, StringComparer.Ordinal).Count();
        var changed = currentItems.Keys.Intersect(previousItems.Keys, StringComparer.Ordinal)
            .Count(code => previousItems[code] != currentItems[code]);
        return new EstimateReviewRevisionDiff(previous.RevisionNo, previous.GrandTotal,
            current.GrandTotal - previous.GrandTotal, added, removed, changed);
    }

    private static Dictionary<string, string> WorkItemSignatures(EstimateRevision revision) => revision.Sections
        .SelectMany(section => section.WorkItems)
        .GroupBy(item => item.Code, StringComparer.Ordinal)
        .ToDictionary(group => group.Key, group => string.Join(";", group.OrderBy(item => item.SortOrder).Select(item =>
            $"{item.DescriptionTh}|{item.DescriptionEn}|{item.Quantity}|{item.UnitCode}|{item.TotalCost}|{item.TotalSellingPrice}")), StringComparer.Ordinal);

    private sealed record ReviewRow(
        EstimateApprovalStep Step,
        EstimateApprovalRequest Request,
        Estimate Estimate,
        EstimateRevision Revision,
        TanErp.Domain.Crm.Opportunities.Opportunity Opportunity,
        TanErp.Domain.Crm.Customers.Customer Customer,
        TanErp.Domain.Organization.Branch Branch,
        TanErp.Domain.IdentityAccess.User Requester,
        TanErp.Domain.IdentityAccess.User Reviewer);
}
