import { ApiError } from "@/lib/api/api-error";

const KNOWN_CODES = [
  "PROJECT_VERSION_CONFLICT",
  "PROJECT_INVALID_STATE",
  "PROJECT_BUDGET_FROZEN",
  "PROJECT_INVALID_TRANSITION",
  "PROJECT_NOT_READY",
  "PROJECT_OPEN_INSTALLATION",
  "PROJECT_MILESTONE_COMPLETED",
  "PROJECT_MILESTONE_VERSION_CONFLICT",
  "PROJECT_CHANGE_ORDER_INVALID_STATE",
  "PROJECT_CHANGE_ORDER_VERSION_CONFLICT",
  "PROJECT_PLAN_INVALID",
  "PROJECT_BUDGET_INVALID",
  "PROJECT_BUDGET_NEGATIVE",
  "PROJECT_REASON_REQUIRED",
  "PROJECT_MILESTONE_INVALID",
  "PROJECT_CHANGE_ORDER_INVALID",
  "PROJECT_CHANGE_ORDER_SELF_APPROVAL",
] as const;

export type ProjectControlErrorCode = (typeof KNOWN_CODES)[number];

/** The backend code when it is one this module has a translation for; otherwise null (generic failure message). */
export function projectControlErrorCode(error: unknown): ProjectControlErrorCode | null {
  if (!(error instanceof ApiError)) return null;
  return KNOWN_CODES.find((code) => code === error.code) ?? null;
}
