import type { components } from "@/generated/api/tan-erp.v1";

export type MembershipDto = components["schemas"]["MembershipDto"];
export type PermissionDto = components["schemas"]["PermissionDto"];

/**
 * Checks if the given membership has the specified permission key.
 *
 * Rules:
 * - Returns true ONLY if the permission key matches AND the scope is "organization".
 * - Scopes of "branch" or "own" must return false in this foundation phase.
 * - Missing or undefined membership/permissions return false.
 */
export function can(
  membership: MembershipDto | null | undefined,
  requiredPermission: string
): boolean {
  if (!membership || !membership.permissions || !Array.isArray(membership.permissions)) {
    return false;
  }

  return membership.permissions.some((perm) => {
    if (!perm || perm.key !== requiredPermission) {
      return false;
    }

    // Organization scope is the only authorized scope in this slice
    return perm.scope === "organization";
  });
}
