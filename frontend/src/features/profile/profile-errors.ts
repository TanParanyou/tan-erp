import { ApiError } from "@/lib/api/api-error";

export type ProfileErrorKey =
  | "GENERIC"
  | "REQUEST_VALIDATION_FAILED"
  | "FILE_NOT_FOUND"
  | "WRONG_PASSWORD"
  | "WEAK_PASSWORD"
  | "REQUIRES_RECENT_LOGIN"
  | "TOO_MANY_REQUESTS";

const API_CODES: ReadonlySet<string> = new Set(["REQUEST_VALIDATION_FAILED", "FILE_NOT_FOUND"]);

/** Maps an API error to a `myProfile.errors.*` message key; unknown errors fall back to GENERIC. */
export function profileErrorKey(error: unknown): ProfileErrorKey {
  if (error instanceof ApiError && error.code && API_CODES.has(error.code)) {
    return error.code as ProfileErrorKey;
  }
  return "GENERIC";
}

/** Maps a Firebase Auth error (`auth/...`) raised while changing the password to a `myProfile.errors.*` key. */
export function passwordErrorKey(error: unknown): ProfileErrorKey {
  const code = error && typeof error === "object" && "code" in error ? (error as { code?: unknown }).code : undefined;
  switch (code) {
    case "auth/wrong-password":
    case "auth/invalid-credential":
      return "WRONG_PASSWORD";
    case "auth/weak-password":
      return "WEAK_PASSWORD";
    case "auth/requires-recent-login":
      return "REQUIRES_RECENT_LOGIN";
    case "auth/too-many-requests":
      return "TOO_MANY_REQUESTS";
    default:
      return "GENERIC";
  }
}
