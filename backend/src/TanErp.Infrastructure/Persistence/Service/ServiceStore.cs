using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Service;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Projects;
using TanErp.Domain.Service;

namespace TanErp.Infrastructure.Persistence.Service;

public class ServiceStore : IServiceStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;

    public ServiceStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> Fail<T>(ServiceDomainException ex) => Fail<T>(ex.Code, ex.Message);

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

    private async Task<Dictionary<Guid, ServicePerson>> PeopleAsync(IEnumerable<Guid?> ids, CancellationToken ct)
    {
        var list = ids.Where(i => i.HasValue).Select(i => i!.Value).Distinct().ToList();
        return await _db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new ServicePerson(u.Id, u.DisplayName), ct);
    }

    private async Task<ServiceProjectRef?> ProjectRefAsync(Guid orgId, Guid projectId, CancellationToken ct) =>
        await _db.Projects.AsNoTracking().Where(p => p.Id == projectId && p.OrganizationId == orgId).Select(p => new ServiceProjectRef(p.Id, p.Code, p.Name)).FirstOrDefaultAsync(ct);

    // ===== installation ==============================================================================

    public async Task<Result<InstallationProjection>> CreateInstallationAsync(
        RequestAccessContext access, InstallationInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "service.installation.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<InstallationProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<InstallationProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetInstallationAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<InstallationProjection>.Success(replayed);
            }

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ProjectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail<InstallationProjection>("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
            {
                return Fail<InstallationProjection>("INSTALLATION_PROJECT_NOT_ACTIVE", "Installations cannot be planned for a completed or cancelled project.");
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.InstallationJobs, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<InstallationProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate installation document number.");
            }

            try
            {
                var job = new InstallationJob(Guid.NewGuid(), orgId, access.BranchId.Value, input.ProjectId, number, input.ScheduledStart, input.ScheduledEnd, input.CrewName, input.Note, access.ActorUserId, now);
                var sort = 1;
                foreach (var item in input.Checklist) job.AddChecklistItem(new InstallationChecklistItem(Guid.NewGuid(), orgId, job.Id, sort++, item.Title, item.Required));
                _db.InstallationJobs.Add(job);
                Audit(access, "installation.created", "InstallationJob", job.Id, traceId, new { number, project = project.Code }, job.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, job.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<InstallationProjection>.Success((await GetInstallationAsync(orgId, job.Id, ct))!);
            }
            catch (ServiceDomainException ex)
            {
                return Fail<InstallationProjection>(ex);
            }
        });
    }

    public async Task<Result<InstallationProjection>> InstallationCommandAsync(
        RequestAccessContext access, Guid installationId, Guid expectedVersion, InstallationCommand command, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var job = await _db.InstallationJobs.Include(j => j.Checklist).Include(j => j.Defects).FirstOrDefaultAsync(j => j.Id == installationId && j.OrganizationId == orgId, ct);
            if (job is null) return Fail<InstallationProjection>("RESOURCE_NOT_FOUND", "Installation not found.");
            if (job.RowVersion != expectedVersion) return Fail<InstallationProjection>("INSTALLATION_VERSION_CONFLICT", "Installation version conflict.");

            var now = _clock.UtcNow;
            string audit;
            try
            {
                switch (command)
                {
                    case StartInstallation:
                        job.Start(now);
                        audit = "installation.started";
                        break;
                    case SetChecklistItem c:
                        job.SetChecklistDone(c.ItemId, c.Done, access.ActorUserId, now);
                        audit = "installation.checklist-updated";
                        break;
                    case ReportDefect d:
                        var defect = job.ReportDefect(d.Description, d.Severity, access.ActorUserId, now);
                        _db.Entry(defect).State = EntityState.Added;
                        audit = "installation.defect-reported";
                        break;
                    case ResolveDefect r:
                        job.ResolveDefect(r.DefectId, r.Note, access.ActorUserId, now);
                        audit = "installation.defect-resolved";
                        break;
                    case VerifyDefect v:
                        job.VerifyDefect(v.DefectId, access.ActorUserId, now);
                        audit = "installation.defect-verified";
                        break;
                    case ReopenDefect o:
                        job.ReopenDefect(o.DefectId, o.Reason, access.ActorUserId, now);
                        audit = "installation.defect-reopened";
                        break;
                    case MarkInstallationReady:
                        job.MarkReady(now);
                        audit = "installation.ready-for-handover";
                        break;
                    case CancelInstallation x:
                        job.Cancel(x.Reason, now);
                        audit = "installation.cancelled";
                        break;
                    case RecordHandover h:
                    {
                        var date = h.HandoverDate ?? Today;
                        job.Handover(h.Outcome, h.SignerName, h.Note, h.WarrantyMonths, date, Today, now);
                        audit = h.Outcome == HandoverOutcome.Accepted ? "installation.handed-over" : "installation.handover-disputed";
                        if (job.Status == InstallationStatus.HandedOver && job.WarrantyMonths is > 0)
                        {
                            string warrantyNumber;
                            try
                            {
                                warrantyNumber = await _numbers.GenerateAsync(orgId, DocumentTypes.Warranties, job.BranchId, now, ct);
                            }
                            catch (Exception ex) when (ex is not OperationCanceledException)
                            {
                                return Fail<InstallationProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate warranty document number.");
                            }

                            _db.Warranties.Add(new Warranty(Guid.NewGuid(), orgId, job.ProjectId, job.Id, warrantyNumber, date, job.WarrantyMonths.Value, now));
                        }

                        break;
                    }
                    default:
                        return Fail<InstallationProjection>("INSTALLATION_FIELD_INVALID", "Unknown command.");
                }
            }
            catch (ServiceDomainException ex)
            {
                return Fail<InstallationProjection>(ex);
            }

            // Names, notes and reasons stay on the records themselves; the audit trail carries identifiers and status only.
            Audit(access, audit, "InstallationJob", job.Id, traceId, new { number = job.Number, status = job.Status }, job.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<InstallationProjection>("INSTALLATION_VERSION_CONFLICT", "Installation version conflict.");
            }

            return Result<InstallationProjection>.Success((await GetInstallationAsync(orgId, job.Id, ct))!);
        });
    }

    public async Task<InstallationProjection?> GetInstallationAsync(Guid organizationId, Guid installationId, CancellationToken ct = default)
    {
        var job = await _db.InstallationJobs.AsNoTracking().Include(j => j.Checklist).Include(j => j.Defects)
            .FirstOrDefaultAsync(j => j.Id == installationId && j.OrganizationId == organizationId, ct);
        if (job is null) return null;

        var project = await ProjectRefAsync(organizationId, job.ProjectId, ct);
        var warranty = await _db.Warranties.AsNoTracking().Where(w => w.InstallationJobId == job.Id).Select(w => new WarrantyRef(w.Id, w.Number, w.StartDate, w.EndDate)).FirstOrDefaultAsync(ct);
        var people = await PeopleAsync(
            job.Defects.SelectMany(d => new Guid?[] { d.ReportedByUserId, d.ResolvedByUserId, d.VerifiedByUserId })
                .Concat(job.Checklist.Select(c => c.DoneByUserId)).Append(job.CreatedByUserId), ct);
        ServicePerson? Person(Guid? id) => id.HasValue && people.TryGetValue(id.Value, out var p) ? p : null;

        return new InstallationProjection(
            job.Id, job.BranchId, job.Number, job.Status, project!, job.ScheduledStart, job.ScheduledEnd, job.CrewName, job.Note, job.CancelReason, job.ReadyAtUtc,
            job.HandoverDate.HasValue ? new HandoverProjection(job.HandoverDate.Value, job.HandoverSignerName ?? string.Empty, job.HandoverNote, job.WarrantyMonths ?? 0) : null,
            job.DisputeCount, job.LastDisputeNote, warranty, Person(job.CreatedByUserId)!, job.CreatedAtUtc, job.RowVersion,
            job.Checklist.OrderBy(c => c.SortOrder).Select(c => new ChecklistItemProjection(c.Id, c.SortOrder, c.Title, c.Required, c.Done, Person(c.DoneByUserId), c.DoneAtUtc)).ToList(),
            job.Defects.OrderBy(d => d.No).Select(d => new DefectProjection(
                d.Id, d.No, d.Description, d.Severity, d.Status, Person(d.ReportedByUserId)!, d.ReportedAtUtc, Person(d.ResolvedByUserId), d.ResolvedAtUtc, d.ResolutionNote,
                Person(d.VerifiedByUserId), d.VerifiedAtUtc, d.ReopenCount, d.LastReopenReason)).ToList());
    }

    public async Task<PagedInstallations> ListInstallationsAsync(Guid organizationId, InstallationListQuery query, CancellationToken ct = default)
    {
        var jobs = _db.InstallationJobs.AsNoTracking().Where(j => j.OrganizationId == organizationId);
        if (query.Status is not null) jobs = jobs.Where(j => j.Status == query.Status);
        if (query.ProjectId.HasValue) jobs = jobs.Where(j => j.ProjectId == query.ProjectId);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            jobs = jobs.Where(j => j.Number.ToUpper().Contains(needle) || _db.Projects.Any(p => p.Id == j.ProjectId && (p.Code.ToUpper().Contains(needle) || p.Name.ToUpper().Contains(needle))));
        }

        var total = await jobs.CountAsync(ct);
        var rows = await jobs.OrderByDescending(j => j.CreatedAtUtc).ThenBy(j => j.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(j => new
            {
                j.Id, j.Number, j.Status, j.ProjectId, j.ScheduledStart, j.ScheduledEnd, j.CreatedAtUtc,
                Done = j.Checklist.Count(c => c.Done), Total = j.Checklist.Count, Open = j.Defects.Count(d => d.Status != DefectStatus.Verified)
            }).ToListAsync(ct);
        var projectIds = rows.Select(r => r.ProjectId).Distinct().ToList();
        var projects = await _db.Projects.AsNoTracking().Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name }, ct);
        return new PagedInstallations(rows.Select(r => new InstallationListItemProjection(
            r.Id, r.Number, r.Status, projects[r.ProjectId].Code, projects[r.ProjectId].Name, r.ScheduledStart, r.ScheduledEnd, r.Done, r.Total, r.Open, r.CreatedAtUtc)).ToList(), total, query.Page, query.PageSize);
    }

    // ===== warranties ================================================================================

    private WarrantyProjection ToProjection(Warranty w, string projectCode, string projectName, string installationNumber, int requests) =>
        new(w.Id, w.Number, new ServiceProjectRef(w.ProjectId, projectCode, projectName), w.InstallationJobId, installationNumber, w.StartDate, w.EndDate, w.Months, w.Covers(Today), requests);

    public async Task<WarrantyProjection?> GetWarrantyAsync(Guid organizationId, Guid warrantyId, CancellationToken ct = default)
    {
        var w = await _db.Warranties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == warrantyId && x.OrganizationId == organizationId, ct);
        if (w is null) return null;
        var project = await ProjectRefAsync(organizationId, w.ProjectId, ct);
        var installation = await _db.InstallationJobs.AsNoTracking().Where(j => j.Id == w.InstallationJobId).Select(j => j.Number).FirstAsync(ct);
        var requests = await _db.ServiceRequests.CountAsync(r => r.WarrantyId == w.Id, ct);
        return ToProjection(w, project!.Code, project.Name, installation, requests);
    }

    public async Task<PagedWarranties> ListWarrantiesAsync(Guid organizationId, WarrantyListQuery query, CancellationToken ct = default)
    {
        var warranties = _db.Warranties.AsNoTracking().Where(w => w.OrganizationId == organizationId);
        var today = Today;
        if (query.ProjectId.HasValue) warranties = warranties.Where(w => w.ProjectId == query.ProjectId);
        if (query.State == "active") warranties = warranties.Where(w => w.StartDate <= today && w.EndDate >= today);
        if (query.State == "expired") warranties = warranties.Where(w => w.EndDate < today);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            warranties = warranties.Where(w => w.Number.ToUpper().Contains(needle) || _db.Projects.Any(p => p.Id == w.ProjectId && (p.Code.ToUpper().Contains(needle) || p.Name.ToUpper().Contains(needle))));
        }

        var total = await warranties.CountAsync(ct);
        var rows = await warranties.OrderByDescending(w => w.EndDate).ThenBy(w => w.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var projectIds = rows.Select(r => r.ProjectId).Distinct().ToList();
        var projects = await _db.Projects.AsNoTracking().Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name }, ct);
        var jobIds = rows.Select(r => r.InstallationJobId).ToList();
        var jobs = await _db.InstallationJobs.AsNoTracking().Where(j => jobIds.Contains(j.Id)).ToDictionaryAsync(j => j.Id, j => j.Number, ct);
        var warrantyIds = rows.Select(r => r.Id).ToList();
        var counts = await _db.ServiceRequests.AsNoTracking().Where(r => r.WarrantyId != null && warrantyIds.Contains(r.WarrantyId.Value)).GroupBy(r => r.WarrantyId!.Value).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(g => g.Key, g => g.Count, ct);
        return new PagedWarranties(rows.Select(w => ToProjection(w, projects[w.ProjectId].Code, projects[w.ProjectId].Name, jobs[w.InstallationJobId], counts.GetValueOrDefault(w.Id))).ToList(), total, query.Page, query.PageSize);
    }

    // ===== service requests ==========================================================================

    public async Task<Result<ServiceRequestProjection>> CreateServiceRequestAsync(
        RequestAccessContext access, ServiceRequestInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "service.request.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<ServiceRequestProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<ServiceRequestProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetServiceRequestAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<ServiceRequestProjection>.Success(replayed);
            }

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ProjectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail<ServiceRequestProjection>("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status == ProjectStatus.Cancelled) return Fail<ServiceRequestProjection>("SERVICE_PROJECT_INVALID", "Service requests cannot be raised for a cancelled project.");

            // The warranty that covers today (latest end first) decides the in-warranty flag; it never blocks the request.
            var today = Today;
            var warranty = await _db.Warranties.AsNoTracking()
                .Where(w => w.OrganizationId == orgId && w.ProjectId == input.ProjectId && w.StartDate <= today && w.EndDate >= today)
                .OrderByDescending(w => w.EndDate).FirstOrDefaultAsync(ct);

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.ServiceRequests, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<ServiceRequestProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate service request document number.");
            }

            try
            {
                var request = new ServiceRequest(Guid.NewGuid(), orgId, access.BranchId.Value, input.ProjectId, warranty?.Id, number, input.Title, input.Description, input.Priority, access.ActorUserId, now);
                _db.ServiceRequests.Add(request);
                Audit(access, "service-request.created", "ServiceRequest", request.Id, traceId, new { number, inWarranty = request.InWarranty }, request.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, request.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<ServiceRequestProjection>.Success((await GetServiceRequestAsync(orgId, request.Id, ct))!);
            }
            catch (ServiceDomainException ex)
            {
                return Fail<ServiceRequestProjection>(ex);
            }
        });
    }

    public async Task<Result<ServiceRequestProjection>> ServiceRequestCommandAsync(
        RequestAccessContext access, Guid requestId, Guid expectedVersion, ServiceRequestCommand command, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var request = await _db.ServiceRequests.Include(r => r.Events).FirstOrDefaultAsync(r => r.Id == requestId && r.OrganizationId == orgId, ct);
            if (request is null) return Fail<ServiceRequestProjection>("RESOURCE_NOT_FOUND", "Service request not found.");
            if (request.RowVersion != expectedVersion) return Fail<ServiceRequestProjection>("SERVICE_VERSION_CONFLICT", "Service request version conflict.");

            var now = _clock.UtcNow;
            var before = request.Events.Count;
            try
            {
                switch (command)
                {
                    case ScheduleServiceRequest s: request.Schedule(s.Date, Today, access.ActorUserId, now); break;
                    case StartServiceRequest: request.Start(access.ActorUserId, now); break;
                    case ResolveServiceRequest r: request.Resolve(r.Note, access.ActorUserId, now); break;
                    case CloseServiceRequest: request.Close(access.ActorUserId, now); break;
                    case ReopenServiceRequest o: request.Reopen(o.Reason, access.ActorUserId, now); break;
                    default: return Fail<ServiceRequestProjection>("SERVICE_FIELD_INVALID", "Unknown command.");
                }
            }
            catch (ServiceDomainException ex)
            {
                return Fail<ServiceRequestProjection>(ex);
            }

            // EF only discovers events added through the private field by tracking; mark the new ones explicitly.
            foreach (var added in request.Events.Skip(before)) _db.Entry(added).State = EntityState.Added;

            Audit(access, "service-request.status-changed", "ServiceRequest", request.Id, traceId, new { number = request.Number, status = request.Status }, request.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<ServiceRequestProjection>("SERVICE_VERSION_CONFLICT", "Service request version conflict.");
            }

            return Result<ServiceRequestProjection>.Success((await GetServiceRequestAsync(orgId, request.Id, ct))!);
        });
    }

    public async Task<ServiceRequestProjection?> GetServiceRequestAsync(Guid organizationId, Guid requestId, CancellationToken ct = default)
    {
        var r = await _db.ServiceRequests.AsNoTracking().Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == requestId && x.OrganizationId == organizationId, ct);
        if (r is null) return null;
        var project = await ProjectRefAsync(organizationId, r.ProjectId, ct);
        var warranty = r.WarrantyId.HasValue
            ? await _db.Warranties.AsNoTracking().Where(w => w.Id == r.WarrantyId).Select(w => new WarrantyRef(w.Id, w.Number, w.StartDate, w.EndDate)).FirstOrDefaultAsync(ct)
            : null;
        var people = await PeopleAsync(r.Events.Select(e => (Guid?)e.ActorUserId).Append(r.CreatedByUserId), ct);
        ServicePerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ServicePerson(id, string.Empty);

        return new ServiceRequestProjection(
            r.Id, r.BranchId, r.Number, r.Title, r.Description, r.Priority, r.Status, r.InWarranty, warranty, project!, r.ScheduledDate, r.ResolutionNote, r.ReopenCount,
            Person(r.CreatedByUserId), r.CreatedAtUtc, r.RowVersion,
            r.Events.OrderBy(e => e.OccurredAtUtc).ThenBy(e => e.Id).Select(e => new ServiceRequestEventProjection(e.FromStatus, e.ToStatus, e.Note, Person(e.ActorUserId), e.OccurredAtUtc)).ToList());
    }

    public async Task<PagedServiceRequests> ListServiceRequestsAsync(Guid organizationId, ServiceRequestListQuery query, CancellationToken ct = default)
    {
        var requests = _db.ServiceRequests.AsNoTracking().Where(r => r.OrganizationId == organizationId);
        if (query.Status is not null) requests = requests.Where(r => r.Status == query.Status);
        if (query.ProjectId.HasValue) requests = requests.Where(r => r.ProjectId == query.ProjectId);
        if (query.InWarranty.HasValue) requests = requests.Where(r => r.InWarranty == query.InWarranty);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            requests = requests.Where(r => r.Number.ToUpper().Contains(needle) || r.Title.ToUpper().Contains(needle));
        }

        var total = await requests.CountAsync(ct);
        var rows = await requests.OrderByDescending(r => r.CreatedAtUtc).ThenBy(r => r.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var projectIds = rows.Select(r => r.ProjectId).Distinct().ToList();
        var projects = await _db.Projects.AsNoTracking().Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Code, ct);
        return new PagedServiceRequests(rows.Select(r => new ServiceRequestListItemProjection(r.Id, r.Number, r.Title, r.Priority, r.Status, r.InWarranty, projects[r.ProjectId], r.CreatedAtUtc)).ToList(), total, query.Page, query.PageSize);
    }
}
