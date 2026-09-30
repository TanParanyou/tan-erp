using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;

namespace TanErp.Infrastructure.Persistence;

public sealed class OrganizationBranchReader : IOrganizationBranchReader
{
    private readonly AppDbContext _db;

    public OrganizationBranchReader(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<OrganizationBranchProjection>> ListActiveAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _db.Branches.AsNoTracking()
            .Where(branch => branch.OrganizationId == organizationId && branch.IsActive)
            .OrderBy(branch => branch.Code)
            .Select(branch => new OrganizationBranchProjection(branch.Id, branch.Code, branch.Name))
            .ToListAsync(cancellationToken);
    }
}
