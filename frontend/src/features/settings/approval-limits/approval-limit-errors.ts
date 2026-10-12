import { ApiError } from "@/lib/api/api-error";

/** Backend codes that have a dedicated message in `approvalLimitAdmin.errors`. Anything else shows the generic message. */
export const KNOWN_APPROVAL_LIMIT_ERROR_CODES = [
  "ADMIN_VERSION_CONFLICT",
  "APPROVAL_LIMIT_ALREADY_EXISTS",
  "APPROVAL_LIMIT_ROLE_NOT_FOUND",
  "BRANCH_NOT_FOUND",
  "REQUEST_VALIDATION_FAILED",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

const KNOWN = new Set<string>(KNOWN_APPROVAL_LIMIT_ERROR_CODES);

/** Returns the key under `approvalLimitAdmin.errors` for a failed approval limit call. */
export function approvalLimitErrorKey(error: unknown): string {
  return error instanceof ApiError && KNOWN.has(error.code) ? error.code : "GENERIC";
}
