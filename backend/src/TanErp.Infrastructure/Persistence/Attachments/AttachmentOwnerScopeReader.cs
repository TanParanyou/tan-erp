using Microsoft.EntityFrameworkCore;
using TanErp.Application.Attachments;
using TanErp.Domain.Attachments;

namespace TanErp.Infrastructure.Persistence.Attachments;

/// <summary>
/// Finds an owner record inside one organization. Owner types are matched against a fixed set; the value is never used to build SQL.
/// Add one case (and one entry in SupportedOwnerTypes) per registered owner type.
/// </summary>
public class AttachmentOwnerScopeReader : IAttachmentOwnerScopeReader
{
    public static readonly IReadOnlySet<string> SupportedOwnerTypes = new HashSet<string>(StringComparer.Ordinal) { AttachmentOwnerTypes.InstallationJob };

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
            default:
                return null;
        }
    }
}
