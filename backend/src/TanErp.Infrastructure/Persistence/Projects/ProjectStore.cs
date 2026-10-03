using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Projects;
using TanErp.Application.Projects.CreateProjectFromHandover;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Projects;

public class ProjectStore : IProjectStore
{
    private const string Operation = "projects.create-from-handover";

    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;

    public ProjectStore(AppDbContext db, IClock clock, IDocumentNumberGenerator documentNumberGenerator)
    {
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
    }

    public async Task<Result<ProjectDetailProjection>> CreateFromHandoverAsync(
        RequestAccessContext access,
        CreateProjectFromHandoverCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // Replay precedes every state check so a retried request returns the original project.
            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == orgId && r.Operation == Operation && r.KeyHash == keyHash, cancellationToken);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash)
                {
                    return Result<ProjectDetailProjection>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                }

                var replayed = Guid.TryParse(replay.ResourceId, out var replayId) ? await GetAsync(orgId, replayId, cancellationToken) : null;
                if (replayed is not null)
                {
                    return Result<ProjectDetailProjection>.Success(replayed);
                }
            }

            var quotation = await _db.Quotations.AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == command.QuotationId && q.OrganizationId == orgId, cancellationToken);
            if (quotation is null)
            {
                return Result<ProjectDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Quotation not found."));
            }

            if (quotation.RowVersion != command.ExpectedQuotationVersion)
            {
                return Result<ProjectDetailProjection>.Failure(new Error("QUOTATION_VERSION_CONFLICT", "Quotation version conflict."));
            }

            if (quotation.Status != "accepted")
            {
                return Result<ProjectDetailProjection>.Failure(
                    new Error("PROJECT_HANDOVER_NOT_ALLOWED", "Only an accepted quotation can be handed over to a project."));
            }

            var opportunity = await _db.Opportunities.AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == quotation.OpportunityId && o.OrganizationId == orgId, cancellationToken);
            if (opportunity is null)
            {
                return Result<ProjectDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            if (opportunity.Stage != OpportunityStage.Won)
            {
                return Result<ProjectDetailProjection>.Failure(
                    new Error("PROJECT_HANDOVER_NOT_ALLOWED", "Only a won opportunity can be handed over to a project."));
            }

            if (await _db.Projects.AsNoTracking().AnyAsync(p => p.OrganizationId == orgId && p.QuotationId == quotation.Id, cancellationToken))
            {
                return Result<ProjectDetailProjection>.Failure(
                    new Error("PROJECT_ALREADY_EXISTS", "A project already exists for this quotation."));
            }

            var estimate = await _db.Estimates.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == quotation.EstimateId && e.OrganizationId == orgId, cancellationToken);
            if (estimate is null)
            {
                return Result<ProjectDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Estimate not found."));
            }

            var now = _clock.UtcNow;
            var ownerMembership = await _db.Memberships.AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == command.OwnerUserId && m.BranchId == quotation.BranchId && m.OrganizationId == orgId, cancellationToken);
            if (ownerMembership is null || !ownerMembership.IsActiveAt(now))
            {
                return Result<ProjectDetailProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active project owner membership not found in the quotation's branch."));
            }

            string code;
            try
            {
                code = await _documentNumberGenerator.GenerateAsync(orgId, DocumentTypes.Projects, quotation.BranchId, now, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Result<ProjectDetailProjection>.Failure(
                    new Error("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate project document number."));
            }

            var baselineHash = Sha256Hex.Compute(JsonSerializer.Serialize(new
            {
                quotationId = quotation.Id,
                quotationNumber = quotation.Number,
                contractAmount = quotation.TotalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                quotationSnapshotHash = quotation.SnapshotHash,
                estimateId = quotation.EstimateId,
                estimateRevisionId = quotation.EstimateRevisionId,
                siteSurveyRevisionId = estimate.SiteSurveyRevisionId,
                siteSurveySnapshotHash = estimate.SiteSurveySnapshotHash
            }));

            Project project;
            try
            {
                project = Project.CreateFromHandover(
                    Guid.NewGuid(), orgId, quotation.BranchId, quotation.CustomerId, opportunity.PrimarySiteId, opportunity.Id,
                    quotation.Id, quotation.EstimateId, quotation.EstimateRevisionId, estimate.SiteSurveyRevisionId,
                    code, command.Name ?? opportunity.Title, command.OwnerUserId, command.PlannedStartDate,
                    quotation.Number, quotation.TotalAmount, quotation.SnapshotHash, estimate.SiteSurveySnapshotHash,
                    baselineHash, access.ActorUserId, now);
            }
            catch (ProjectDomainException ex)
            {
                return Result<ProjectDetailProjection>.Failure(new Error(ex.Code, ex.Message));
            }

            _db.Projects.Add(project);
            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(), orgId, access.ActorUserId, "project.created-from-handover", "Project", project.Id.ToString(), now,
                command.TraceId,
                JsonSerializer.Serialize(new { code = project.Code, quotationId = quotation.Id, quotationNumber = quotation.Number, ownerUserId = project.OwnerUserId, baselineHash }),
                branchId: access.BranchId,
                actorMembershipId: access.MembershipId,
                rowVersionAfter: project.RowVersion));
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, Operation, keyHash, payloadHash, project.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                // A concurrent handover of the same quotation won the unique (organization, quotation) slot.
                return Result<ProjectDetailProjection>.Failure(
                    new Error("PROJECT_ALREADY_EXISTS", "A project already exists for this quotation."));
            }

            var created = await GetAsync(orgId, project.Id, cancellationToken);
            return Result<ProjectDetailProjection>.Success(created!);
        });
    }

    public async Task<ProjectHandoverSourceProjection?> GetHandoverSourceAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from q in _db.Quotations.AsNoTracking()
            where q.OrganizationId == organizationId && q.OpportunityId == opportunityId && q.Status == "accepted"
            join o in _db.Opportunities.AsNoTracking() on q.OpportunityId equals o.Id
            join p in _db.Projects.AsNoTracking() on q.Id equals p.QuotationId into projects
            from p in projects.DefaultIfEmpty()
            orderby q.IssuedAtUtc descending
            select new { Quotation = q, o.Stage, Project = p })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : new ProjectHandoverSourceProjection(
                row.Quotation.Id, row.Quotation.Number, row.Quotation.Status, row.Quotation.RowVersion,
                row.Quotation.TotalAmount, row.Stage, row.Project?.Id, row.Project?.Code);
    }

    public async Task<ProjectDetailProjection?> GetAsync(Guid organizationId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var row = await (
            from p in _db.Projects.AsNoTracking()
            where p.Id == projectId && p.OrganizationId == organizationId
            join owner in _db.Users.AsNoTracking() on p.OwnerUserId equals owner.Id
            join customer in _db.Customers.AsNoTracking() on p.CustomerId equals customer.Id
            join opportunity in _db.Opportunities.AsNoTracking() on p.OpportunityId equals opportunity.Id
            join site in _db.Sites.AsNoTracking() on p.SiteId equals (Guid?)site.Id into sites
            from site in sites.DefaultIfEmpty()
            select new { Project = p, Owner = owner, Customer = customer, Opportunity = opportunity, Site = site })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null) return null;

        var project = row.Project;
        return new ProjectDetailProjection(
            project.Id,
            project.BranchId,
            project.Code,
            project.Name,
            project.Status,
            project.PlannedStartDate,
            new ProjectPersonProjection(row.Owner.Id, row.Owner.DisplayName, row.Owner.Email),
            new ProjectCustomerProjection(row.Customer.Id, row.Customer.Code, row.Customer.DisplayNameTh, row.Customer.DisplayNameEn),
            row.Site is null ? null : new ProjectSiteProjection(row.Site.Id, row.Site.Label),
            new ProjectOpportunityProjection(row.Opportunity.Id, row.Opportunity.Code, row.Opportunity.Title),
            new ProjectBaselineProjection(
                project.QuotationId, project.BaselineQuotationNumber, project.BaselineContractAmount, project.BaselineQuotationSnapshotHash,
                project.EstimateId, project.EstimateRevisionId, project.SiteSurveyRevisionId, project.BaselineSurveySnapshotHash, project.BaselineHash),
            project.RowVersion,
            project.CreatedAtUtc);
    }

    public async Task<PagedProjectsProjection> ListAsync(Guid organizationId, ProjectListQuery query, CancellationToken cancellationToken = default)
    {
        var projects = _db.Projects.AsNoTracking().Where(p => p.OrganizationId == organizationId);

        if (query.Status is not null)
        {
            projects = projects.Where(p => p.Status == query.Status);
        }

        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            projects = projects.Where(p => p.Code.ToLower().Contains(needle) || p.Name.ToLower().Contains(needle)
                || p.BaselineQuotationNumber.ToLower().Contains(needle));
        }

        var total = await projects.CountAsync(cancellationToken);
        var items = await (
            from p in projects
            join owner in _db.Users.AsNoTracking() on p.OwnerUserId equals owner.Id
            join customer in _db.Customers.AsNoTracking() on p.CustomerId equals customer.Id
            orderby p.CreatedAtUtc descending, p.Id descending
            select new ProjectListItemProjection(
                p.Id, p.Code, p.Name, p.Status, p.PlannedStartDate,
                new ProjectPersonProjection(owner.Id, owner.DisplayName, owner.Email),
                new ProjectCustomerProjection(customer.Id, customer.Code, customer.DisplayNameTh, customer.DisplayNameEn),
                p.BaselineContractAmount, p.CreatedAtUtc))
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedProjectsProjection(items, total, query.Page, query.PageSize);
    }
}
