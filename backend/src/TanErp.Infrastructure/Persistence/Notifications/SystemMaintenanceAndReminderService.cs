using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Files;
using TanErp.Application.Notifications;
using TanErp.Domain.Files;
using TanErp.Domain.Notifications;
using TanErp.Domain.Service;

namespace TanErp.Infrastructure.Persistence.Notifications;

public class SystemMaintenanceAndReminderService : ISystemMaintenanceAndReminderService
{
    public const long AdvisoryLockKey = 2_110_000_022L;

    private readonly AppDbContext _db;
    private readonly INotificationPublisher _publisher;
    private readonly IFileStorageProvider _fileStorage;
    private readonly IClock _clock;
    private readonly SystemMaintenanceAndReminderOptions _options;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SystemMaintenanceAndReminderService> _logger;

    public SystemMaintenanceAndReminderService(
        AppDbContext db,
        INotificationPublisher publisher,
        IFileStorageProvider fileStorage,
        IClock clock,
        SystemMaintenanceAndReminderOptions options,
        IConfiguration configuration,
        ILogger<SystemMaintenanceAndReminderService> logger)
    {
        _db = db;
        _publisher = publisher;
        _fileStorage = fileStorage;
        _clock = clock;
        _options = options;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SystemMaintenanceAndReminderRunResult> RunOnceAsync(CancellationToken ct = default)
    {
        var now = _clock.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var warrantyThresholds = SystemMaintenanceAndReminderPlanner.NormalizeWarrantyThresholds(_options.WarrantyThresholdDays);
        var warrantyHorizon = today.AddDays(warrantyThresholds[^1]);

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var locked = await _db.Database
            .SqlQuery<bool>($"SELECT pg_try_advisory_xact_lock({AdvisoryLockKey}) AS \"Value\"")
            .SingleAsync(ct);
        if (!locked)
        {
            return new SystemMaintenanceAndReminderRunResult(0, 0, 0, 0, 0, 0, 0, true);
        }

        // 1. Service SLA Warning Scan
        var openRequests = await _db.ServiceRequests.AsNoTracking()
            .Where(r => r.Status != ServiceRequestStatus.Resolved && r.Status != ServiceRequestStatus.Closed)
            .OrderBy(r => r.CreatedAtUtc)
            .Take(_options.EffectiveMaxItems)
            .ToListAsync(ct);

        var orgIds = openRequests.Select(r => r.OrganizationId).Distinct().ToList();
        var holidaysByOrg = new Dictionary<Guid, HashSet<DateOnly>>();
        if (orgIds.Count > 0)
        {
            var holidays = await _db.WorkingCalendarHolidays.AsNoTracking()
                .Where(h => orgIds.Contains(h.OrganizationId))
                .Select(h => new { h.OrganizationId, h.HolidayDate })
                .ToListAsync(ct);
            foreach (var group in holidays.GroupBy(h => h.OrganizationId))
            {
                holidaysByOrg[group.Key] = group.Select(x => x.HolidayDate).ToHashSet();
            }
        }

        var notificationsBeforeSla = _db.ChangeTracker.Entries<Notification>().Count(e => e.State == EntityState.Added);
        foreach (var req in openRequests)
        {
            var orgHolidays = holidaysByOrg.TryGetValue(req.OrganizationId, out var set) ? set : (IReadOnlySet<DateOnly>)new HashSet<DateOnly>();
            var eval = ServiceSlaCalculator.Evaluate(req, now, orgHolidays);
            if (eval.SlaStatus is SlaStatus.Warning or SlaStatus.Breached)
            {
                var transitionId = SystemMaintenanceAndReminderPlanner.ServiceSlaTransitionId(req.Id, eval.SlaStatus);
                var evt = NotificationEvents.ServiceRequestSlaWarning(
                    req.OrganizationId,
                    req.BranchId,
                    transitionId,
                    req.CreatedByUserId,
                    req.Id,
                    req.Number);
                try
                {
                    await _publisher.PublishAsync(evt, ct);
                }
                catch (NotificationDomainException ex)
                {
                    _logger.LogWarning(ex, "Service SLA reminder skipped for service request {ServiceRequestId}: {Code}", req.Id, ex.Code);
                }
            }
        }
        var slaNotificationsCreated = _db.ChangeTracker.Entries<Notification>().Count(e => e.State == EntityState.Added) - notificationsBeforeSla;

        // 2. Warranty Expiry Reminder Scan
        var warranties = await (
            from w in _db.Warranties.AsNoTracking()
            join p in _db.Projects.AsNoTracking() on new { w.ProjectId, w.OrganizationId } equals new { ProjectId = p.Id, p.OrganizationId }
            where w.EndDate >= today && w.EndDate <= warrantyHorizon
            orderby w.EndDate, w.Id
            select new { w.Id, w.OrganizationId, p.BranchId, w.ProjectId, w.Number, w.EndDate })
            .Take(_options.EffectiveMaxItems)
            .ToListAsync(ct);

        var notificationsBeforeWarranty = _db.ChangeTracker.Entries<Notification>().Count(e => e.State == EntityState.Added);
        foreach (var warranty in warranties)
        {
            var threshold = SystemMaintenanceAndReminderPlanner.SelectWarrantyThreshold(today, warranty.EndDate, warrantyThresholds);
            if (threshold is null) continue;

            var transitionId = SystemMaintenanceAndReminderPlanner.WarrantyTransitionId(warranty.Id, threshold.Value, warranty.EndDate);
            var evt = NotificationEvents.WarrantyExpiring(
                warranty.OrganizationId,
                warranty.BranchId,
                transitionId,
                warranty.Id,
                warranty.ProjectId,
                warranty.Number,
                threshold.Value);
            try
            {
                await _publisher.PublishAsync(evt, ct);
            }
            catch (NotificationDomainException ex)
            {
                _logger.LogWarning(ex, "Warranty expiry reminder skipped for warranty {WarrantyId}: {Code}", warranty.Id, ex.Code);
            }
        }
        var warrantyNotificationsCreated = _db.ChangeTracker.Entries<Notification>().Count(e => e.State == EntityState.Added) - notificationsBeforeWarranty;

        // 3. Expired FileUploadSessions & Orphan UploadedFiles Cleanup
        var expiredSessions = await _db.FileUploadSessions
            .Where(s => (s.Status == FileUploadSessionStatus.Pending || s.Status == FileUploadSessionStatus.Expired) && s.ExpiresAtUtc < now)
            .OrderBy(s => s.ExpiresAtUtc)
            .Take(_options.EffectiveMaxItems)
            .ToListAsync(ct);
        if (expiredSessions.Count > 0)
        {
            var expiredSessionIds = expiredSessions.Select(s => s.Id).ToList();
            var slots = await _db.FileUploadSlots
                .Where(slot => expiredSessionIds.Contains(slot.SessionId))
                .ToListAsync(ct);
            if (slots.Count > 0)
            {
                _db.FileUploadSlots.RemoveRange(slots);
            }
            _db.FileUploadSessions.RemoveRange(expiredSessions);
        }

        var orphanCutoff = now - _options.EffectiveOrphanGrace;
        var candidateFiles = await _db.UploadedFiles
            .Where(f => f.CreatedAtUtc < orphanCutoff && f.UploadSessionId != "quotation-document")
            .OrderBy(f => f.CreatedAtUtc)
            .Take(_options.EffectiveMaxItems)
            .ToListAsync(ct);

        var orphanFilesDeleted = 0;
        if (candidateFiles.Count > 0)
        {
            var candidateIds = candidateFiles.Select(f => f.Id).ToList();
            var linkedIds = new HashSet<Guid>();

            linkedIds.UnionWith(await _db.AttachmentLinks.AsNoTracking().Where(x => candidateIds.Contains(x.FileId)).Select(x => x.FileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.OpportunityWorkImages.AsNoTracking().Where(x => candidateIds.Contains(x.FileId)).Select(x => x.FileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.SiteImages.AsNoTracking().Where(x => candidateIds.Contains(x.FileId)).Select(x => x.FileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.Customers.AsNoTracking().Where(x => x.ImageFileId.HasValue && candidateIds.Contains(x.ImageFileId.Value)).Select(x => x.ImageFileId!.Value).ToListAsync(ct));
            linkedIds.UnionWith(await _db.SiteSurveyEvidence.AsNoTracking().Where(x => candidateIds.Contains(x.FileId)).Select(x => x.FileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.SurveyDesignStyles.AsNoTracking().Where(x => x.ImageFileId.HasValue && candidateIds.Contains(x.ImageFileId.Value)).Select(x => x.ImageFileId!.Value).ToListAsync(ct));
            linkedIds.UnionWith(await _db.ItemImages.AsNoTracking().Where(x => candidateIds.Contains(x.FileId)).Select(x => x.FileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.ItemCategories.AsNoTracking().Where(x => x.ImageFileId.HasValue && candidateIds.Contains(x.ImageFileId.Value)).Select(x => x.ImageFileId!.Value).ToListAsync(ct));
            linkedIds.UnionWith(await _db.ItemBrands.AsNoTracking().Where(x => x.ImageFileId.HasValue && candidateIds.Contains(x.ImageFileId.Value)).Select(x => x.ImageFileId!.Value).ToListAsync(ct));
            linkedIds.UnionWith(await _db.CostRecords.AsNoTracking().Where(x => x.EvidenceFileId.HasValue && candidateIds.Contains(x.EvidenceFileId.Value)).Select(x => x.EvidenceFileId!.Value).ToListAsync(ct));
            linkedIds.UnionWith(await _db.SignatureCaptures.AsNoTracking().Where(x => candidateIds.Contains(x.ImageFileId)).Select(x => x.ImageFileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.QuotationDocuments.AsNoTracking().Where(x => candidateIds.Contains(x.FileId)).Select(x => x.FileId).ToListAsync(ct));
            linkedIds.UnionWith(await _db.Users.AsNoTracking().Where(x => x.AvatarFileId.HasValue && candidateIds.Contains(x.AvatarFileId.Value)).Select(x => x.AvatarFileId!.Value).ToListAsync(ct));
            linkedIds.UnionWith(await _db.Organizations.AsNoTracking().Where(x => x.BrandLogoFileId.HasValue && candidateIds.Contains(x.BrandLogoFileId.Value)).Select(x => x.BrandLogoFileId!.Value).ToListAsync(ct));

            var unlinkedFiles = candidateFiles.Where(f => !linkedIds.Contains(f.Id)).ToList();
            if (unlinkedFiles.Count > 0)
            {
                var unlinkedIds = unlinkedFiles.Select(f => f.Id).ToList();
                var referencingSlots = await _db.FileUploadSlots
                    .Where(s => s.UploadedFileId.HasValue && unlinkedIds.Contains(s.UploadedFileId.Value))
                    .ToListAsync(ct);
                if (referencingSlots.Count > 0)
                {
                    _db.FileUploadSlots.RemoveRange(referencingSlots);
                }

                foreach (var orphan in unlinkedFiles)
                {
                    await _fileStorage.DeleteAsync(orphan.StoragePath, ct);
                }
                _db.UploadedFiles.RemoveRange(unlinkedFiles);
                orphanFilesDeleted = unlinkedFiles.Count;
            }
        }

        // 4. Audit Retention Cleanup (when Audit:RetentionDays > 0)
        var expiredAuditEventsDeleted = 0;
        var retentionDays = int.TryParse(_configuration["Audit:RetentionDays"], out var parsedRetention) ? parsedRetention : 0;
        if (retentionDays > 0)
        {
            var auditCutoff = now.AddDays(-retentionDays);
            var expiredAudits = await _db.AuditEvents
                .Where(a => a.OccurredAtUtc < auditCutoff)
                .OrderBy(a => a.OccurredAtUtc)
                .Take(_options.EffectiveMaxItems)
                .ToListAsync(ct);
            if (expiredAudits.Count > 0)
            {
                _db.AuditEvents.RemoveRange(expiredAudits);
                expiredAuditEventsDeleted = expiredAudits.Count;
            }
        }

        try
        {
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
        {
            await transaction.RollbackAsync(ct);
            _db.ChangeTracker.Clear();
            return new SystemMaintenanceAndReminderRunResult(
                openRequests.Count, 0, warranties.Count, 0, 0, 0, 0, false);
        }

        return new SystemMaintenanceAndReminderRunResult(
            openRequests.Count,
            slaNotificationsCreated,
            warranties.Count,
            warrantyNotificationsCreated,
            expiredSessions.Count,
            orphanFilesDeleted,
            expiredAuditEventsDeleted,
            false);
    }
}
