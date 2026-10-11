using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.RoleManagement;

public static class RoleManagementPolicy
{
    public const int MaxGrants = 200;

    private static Error Invalid(string detail) => new("REQUEST_VALIDATION_FAILED", detail);

    /// <summary>Shape-only validation (name, scopes, limits). The store validates the catalog and branches.</summary>
    public static Error? ValidateShape(RoleInput input)
    {
        try
        {
            RoleRules.Normalize(input.Name, input.Description);
        }
        catch (RoleDomainException ex)
        {
            return new Error(ex.Code, ex.Message);
        }

        if (input.Permissions.Count > MaxGrants) return Invalid($"A role cannot have more than {MaxGrants} permissions.");

        foreach (var grant in input.Permissions)
        {
            if (string.IsNullOrWhiteSpace(grant.PermissionKey)) return Invalid("Permission key is required.");

            if (grant.Scope == PermissionScope.Branch)
            {
                if (grant.BranchId is null || grant.BranchId == Guid.Empty) return Invalid("Branch scope requires a branch.");
            }
            else if (grant.Scope == PermissionScope.Organization || grant.Scope == PermissionScope.Own)
            {
                if (grant.BranchId is not null) return Invalid("Only branch scope accepts a branch.");
            }
            else
            {
                return Invalid("Unsupported permission scope.");
            }
        }

        return null;
    }

    /// <summary>Trims keys, drops duplicates and sorts so equal requests hash equally.</summary>
    public static RoleInput Canonicalize(RoleInput input) => input with
    {
        Permissions = input.Permissions
            .Select(g => g with { PermissionKey = g.PermissionKey.Trim() })
            .Distinct()
            .OrderBy(g => g.PermissionKey, StringComparer.Ordinal)
            .ThenBy(g => g.Scope, StringComparer.Ordinal)
            .ThenBy(g => g.BranchId)
            .ToList()
    };

    public static PermissionGrant ToGrant(Guid organizationId, string key, string scope, Guid? branchId) =>
        new(key, scope, scope == PermissionScope.Organization ? organizationId : scope == PermissionScope.Branch ? branchId : null);

    public static (IReadOnlyList<PermissionGrant> Added, IReadOnlyList<PermissionGrant> Removed) Diff(
        IReadOnlyCollection<PermissionGrant> before, IReadOnlyCollection<PermissionGrant> after) =>
        (after.Except(before).ToList(), before.Except(after).ToList());

    /// <summary>
    /// ADR 0020 decision 3: a change that adds grants to a Role people already hold, where the resulting Role carries an
    /// approval permission, needs an independent checker. Unheld Roles are covered later by the assignment gate (CP-02).
    /// </summary>
    public static bool NeedsChecker(bool hasHolders, IEnumerable<string> resultingKeys, IReadOnlyCollection<PermissionGrant> added) =>
        hasHolders && added.Count > 0 && AdministrationPolicy.RequiresApproval(resultingKeys);
}
