import { ApiError } from "@/lib/api/api-error";

export type ProfileErrorKey =
  | "GENERIC"
  | "REQUEST_VALIDATION_FAILED"
  | "FILE_NOT_FOUND"
  | "WRONG_PASSWORD"
  | "WEAK_PASSWORD"
  | "REQUIRES_RECENT_LOGIN"
  | "TOO_MANY_REQUESTS"
  | "CURRENT_PASSWORD_INVALID"
  | "PASSWORD_TOO_SHORT"
  | "PASSWORD_SAME_AS_CURRENT";

const API_CODES: ReadonlySet<string> = new Set([
  "REQUEST_VALIDATION_FAILED",
  "FILE_NOT_FOUND",
  "CURRENT_PASSWORD_INVALID",
  "PASSWORD_TOO_SHORT",
  "PASSWORD_SAME_AS_CURRENT",
  "WRONG_PASSWORD",
  "WEAK_PASSWORD",
]);

/** Maps an API error to a `myProfile.errors.*` message key; unknown errors fall back to GENERIC. */
export function profileErrorKey(error: unknown): ProfileErrorKey {
  if (error instanceof ApiError && error.code && API_CODES.has(error.code)) {
    return error.code as ProfileErrorKey;
  }
  return "GENERIC";
}

/** Maps an API or Auth error raised while changing the password to a `myProfile.errors.*` key. */
export function passwordErrorKey(error: unknown): ProfileErrorKey {
  if (error instanceof ApiError && error.code) {
    switch (error.code) {
      case "CURRENT_PASSWORD_INVALID":
        return "CURRENT_PASSWORD_INVALID";
      case "PASSWORD_TOO_SHORT":
        return "PASSWORD_TOO_SHORT";
      case "PASSWORD_SAME_AS_CURRENT":
        return "PASSWORD_SAME_AS_CURRENT";
      case "WRONG_PASSWORD":
        return "WRONG_PASSWORD";
      case "WEAK_PASSWORD":
        return "WEAK_PASSWORD";
      case "REQUEST_VALIDATION_FAILED":
        return "REQUEST_VALIDATION_FAILED";
      default:
        break;
    }
  }

  const code = error && typeof error === "object" && "code" in error ? (error as { code?: unknown }).code : undefined;
  switch (code) {
    case "CURRENT_PASSWORD_INVALID":
    case "auth/wrong-password":
    case "auth/invalid-credential":
      return "WRONG_PASSWORD";
    case "PASSWORD_TOO_SHORT":
    case "auth/weak-password":
      return "WEAK_PASSWORD";
    case "PASSWORD_SAME_AS_CURRENT":
      return "PASSWORD_SAME_AS_CURRENT";
    case "auth/requires-recent-login":
      return "REQUIRES_RECENT_LOGIN";
    case "auth/too-many-requests":
      return "TOO_MANY_REQUESTS";
    default:
      return "GENERIC";
  }
}
