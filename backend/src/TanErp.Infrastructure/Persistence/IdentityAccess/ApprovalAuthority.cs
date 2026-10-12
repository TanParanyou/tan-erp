using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.ApprovalLimits;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public sealed class ApprovalAuthority : IApprovalAuthority
{
    private readonly AppDbContext _db;

    public ApprovalAuthority(AppDbContext db) => _db = db;

    public async Task<Error?> EnsureAsync(ApprovalAuthorityCheck check, CancellationToken cancellationToken)
    {
        // Only the approver's ACTIVE roles count (same filter as RequestAccessResolver); inactive limit rows are ignored.
        var rows = await (
            from membershipRole in _db.MembershipRoles.AsNoTracking()
            join limit in _db.ApprovalLimits.AsNoTracking() on membershipRole.RoleId equals limit.RoleId
            where membershipRole.OrganizationId == check.OrganizationId
                  && membershipRole.MembershipId == check.MembershipId
                  && membershipRole.Role!.IsActive
                  && limit.OrganizationId == check.OrganizationId
                  && limit.IsActive
                  && limit.DocumentType == check.DocumentType
                  && limit.Currency == check.Currency
                  && (limit.BranchId == null || limit.BranchId == check.BranchId)
            select new { limit.RoleId, limit.BranchId, limit.IsUnlimited, limit.MaxAmount })
            .ToListAsync(cancellationToken);

        var decision = ApprovalAuthorityPolicy.Evaluate(
            rows.Select(r => new ApprovalLimitCandidate(r.RoleId, r.BranchId, r.IsUnlimited, r.MaxAmount)),
            check.BranchId,
            check.Amount);

        return decision.Outcome switch
        {
            ApprovalAuthorityOutcome.Allowed => null,
            ApprovalAuthorityOutcome.LimitExceeded => new Error("APPROVAL_LIMIT_EXCEEDED", "The document amount exceeds the approver's approval limit."),
            _ => new Error("APPROVAL_AUTHORITY_NOT_CONFIGURED", "No active approval limit is configured for this approver, document type and branch.")
        };
    }
}
