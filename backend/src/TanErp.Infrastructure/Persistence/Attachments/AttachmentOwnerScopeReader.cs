using Microsoft.EntityFrameworkCore;
using TanErp.Application.Attachments;
using TanErp.Domain.Attachments;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Attachments;

/// <summary>
/// Finds an owner record inside one organization. Owner types are matched against a fixed set; the value is never used to build SQL.
/// Add one case (and one entry in SupportedOwnerTypes) per registered owner type.
/// </summary>
public class AttachmentOwnerScopeReader : IAttachmentOwnerScopeReader
{
    public static readonly IReadOnlySet<string> SupportedOwnerTypes = new HashSet<string>(StringComparer.Ordinal) { AttachmentOwnerTypes.InstallationJob, AttachmentOwnerTypes.SurveyArea, AttachmentOwnerTypes.QuickEstimate };

    private readonly AppDbContext _db;

    public AttachmentOwnerScopeReader(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AttachmentOwnerScope?> FindAsync(string ownerType, Guid ownerId, Guid organizationId, CancellationToken ct = default)
    {
        switch (ownerType)
        {
            case AttachmentOwnerTypes.InstallationJob:
                return await _db.InstallationJobs.AsNoTracking()
                    .Where(j => j.Id == ownerId && j.OrganizationId == organizationId)
                    .Select(j => new AttachmentOwnerScope(j.OrganizationId, j.BranchId, j.Status))
                    .FirstOrDefaultAsync(ct);
            case AttachmentOwnerTypes.SurveyArea:
                // Evidence can be edited only on a draft revision whose template hashes area evidence (v4); any other
                // state is reported as a non-manageable one so read stays possible but upload is rejected.
                return await (
                    from area in _db.SiteSurveyAreas.AsNoTracking()
                    join revision in _db.SiteSurveyRevisions.AsNoTracking() on area.SiteSurveyRevisionId equals revision.Id
                    join survey in _db.SiteSurveys.AsNoTracking() on revision.SiteSurveyId equals survey.Id
                    join template in _db.SurveyTemplateVersions.AsNoTracking() on revision.SurveyTemplateVersion equals template.Code
                    where area.Id == ownerId && area.OrganizationId == organizationId && survey.OrganizationId == organizationId
                    select new AttachmentOwnerScope(
                        survey.OrganizationId,
                        survey.BranchId,
                        template.SnapshotHashVersion == SurveyTemplateVersion.AreaEvidenceHashVersion ? revision.Status : "area-evidence-unsupported"))
                    .FirstOrDefaultAsync(ct);
            case AttachmentOwnerTypes.QuickEstimate:
                return await _db.QuickEstimates.AsNoTracking()
                    .Where(q => q.Id == ownerId && q.OrganizationId == organizationId)
                    .Select(q => new AttachmentOwnerScope(q.OrganizationId, q.BranchId, q.Status))
                    .FirstOrDefaultAsync(ct);
            default:
                return null;
        }
    }
}
