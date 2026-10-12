namespace TanErp.Application.IdentityAccess.ApprovalLimits;

/// <summary>One active limit row that applies to the approver (already filtered by document type, currency and role).</summary>
public sealed record ApprovalLimitCandidate(Guid RoleId, Guid? BranchId, bool IsUnlimited, decimal? MaxAmount);

public enum ApprovalAuthorityOutcome { Allowed, NotConfigured, LimitExceeded }

public sealed record ApprovalAuthorityDecision(ApprovalAuthorityOutcome Outcome, bool IsUnlimited, decimal? Ceiling);

/// <summary>
/// Per role a branch-specific row beats the organization-wide row; across roles the highest ceiling wins
/// (unlimited beats any number). No applicable row is never "allowed" (fail-closed).
/// </summary>
public static class ApprovalAuthorityPolicy
{
    public static ApprovalAuthorityDecision Evaluate(IEnumerable<ApprovalLimitCandidate> candidates, Guid documentBranchId, decimal amount)
    {
        var effective = candidates
            .Where(c => c.BranchId is null || c.BranchId == documentBranchId)
            .GroupBy(c => c.RoleId)
            .Select(g => g.FirstOrDefault(c => c.BranchId == documentBranchId) ?? g.First(c => c.BranchId is null))
            .ToList();

        if (effective.Count == 0) return new ApprovalAuthorityDecision(ApprovalAuthorityOutcome.NotConfigured, false, null);
        if (effective.Any(c => c.IsUnlimited)) return new ApprovalAuthorityDecision(ApprovalAuthorityOutcome.Allowed, true, null);

        var ceiling = effective.Max(c => c.MaxAmount ?? 0m);
        return Math.Abs(amount) <= ceiling
            ? new ApprovalAuthorityDecision(ApprovalAuthorityOutcome.Allowed, false, ceiling)
            : new ApprovalAuthorityDecision(ApprovalAuthorityOutcome.LimitExceeded, false, ceiling);
    }
}
