import { ApiError } from "@/lib/api/api-error";

/** Backend error codes that have a dedicated message in `userAdmin.errors`. Anything else uses the generic message. */
const KNOWN_ADMIN_ERROR_CODES = new Set<string>([
  "USER_EMAIL_ALREADY_EXISTS",
  "ADMIN_VERSION_CONFLICT",
  "LAST_ADMINISTRATOR_REQUIRED",
  "ROLE_ESCALATION_DENIED",
  "SELF_ROLE_CHANGE_FORBIDDEN",
  "ROLE_ALREADY_ASSIGNED",
  "ROLE_ASSIGNMENT_REQUEST_PENDING",
  "ROLE_ASSIGNMENT_REQUEST_NOT_PENDING",
  "ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED",
  "ROLE_NOT_ASSIGNED",
  "USER_SHARED_ACROSS_ORGANIZATIONS",
  "BRANCH_NOT_FOUND",
  "BRANCH_INACTIVE",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
]);

/** Returns the key under `userAdmin.errors` for a failed administration call. */
export function adminErrorKey(error: unknown): string {
  if (error instanceof ApiError && KNOWN_ADMIN_ERROR_CODES.has(error.code)) {
    return error.code;
  }
  return "GENERIC";
}
