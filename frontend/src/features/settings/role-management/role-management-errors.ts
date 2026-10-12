import { ApiError } from "@/lib/api/api-error";

/** Backend codes that have a dedicated message in `roleAdmin.errors`. Anything else shows the generic message. */
export const KNOWN_ROLE_ADMIN_ERROR_CODES = [
  "ADMIN_VERSION_CONFLICT",
  "ROLE_NAME_ALREADY_EXISTS",
  "ROLE_NAME_RESERVED",
  "ROLE_PERMISSION_UNKNOWN",
  "ROLE_ESCALATION_DENIED",
  "ROLE_SYSTEM_IMMUTABLE",
  "ROLE_IN_USE",
  "ROLE_CHANGE_REQUEST_PENDING",
  "ROLE_CHANGE_REQUEST_NOT_PENDING",
  "ROLE_CHANGE_REQUEST_STALE",
  "ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED",
  "LAST_ADMINISTRATOR_REQUIRED",
  "BRANCH_NOT_FOUND",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

const KNOWN = new Set<string>(KNOWN_ROLE_ADMIN_ERROR_CODES);

/** Returns the key under `roleAdmin.errors` for a failed role administration call. */
export function roleAdminErrorKey(error: unknown): string {
  return error instanceof ApiError && KNOWN.has(error.code) ? error.code : "GENERIC";
}
