using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.ApprovalLimits;

/// <param name="MembershipId">The approver's membership (its active roles are the source of authority).</param>
/// <param name="DocumentType">One of <c>ApprovalDocumentType</c>.</param>
/// <param name="BranchId">Branch of the document being decided.</param>
/// <param name="Amount">Document amount at decision time; the sign is ignored.</param>
public sealed record ApprovalAuthorityCheck(
    Guid OrganizationId, Guid MembershipId, string DocumentType, Guid BranchId, decimal Amount, string Currency);

/// <summary>
/// Checks the approver's Approval Limits. Implementations share the caller's <c>DbContext</c> so the check runs inside the
/// approval transaction. Returns <c>null</c> when allowed, otherwise <c>APPROVAL_AUTHORITY_NOT_CONFIGURED</c> or <c>APPROVAL_LIMIT_EXCEEDED</c>.
/// </summary>
public interface IApprovalAuthority
{
    Task<Error?> EnsureAsync(ApprovalAuthorityCheck check, CancellationToken cancellationToken);
}
