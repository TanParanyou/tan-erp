import { ApiError } from "@/lib/api/api-error";

/** Backend codes that have a dedicated message in `organizationAdmin.errors`. Anything else shows the generic message. */
export const KNOWN_ORGANIZATION_ADMIN_ERROR_CODES = [
  "ADMIN_VERSION_CONFLICT",
  "ORGANIZATION_TAX_ID_INVALID",
  "BRANCH_CODE_INVALID",
  "BRANCH_TAX_CODE_INVALID",
  "BRANCH_CODE_ALREADY_EXISTS",
  "BRANCH_TAX_CODE_ALREADY_EXISTS",
  "BRANCH_HAS_OPEN_DOCUMENTS",
  "BRANCH_HAS_ACTIVE_MEMBERSHIPS",
  "BRANCH_LAST_ACTIVE",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

const KNOWN = new Set<string>(KNOWN_ORGANIZATION_ADMIN_ERROR_CODES);

/** Returns the key under `organizationAdmin.errors` for a failed organization administration call. */
export function organizationAdminErrorKey(error: unknown): string {
  return error instanceof ApiError && KNOWN.has(error.code) ? error.code : "GENERIC";
}
