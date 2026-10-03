export const INSTALLATION_STATUSES = ["planned", "in_progress", "ready_for_handover", "handed_over", "cancelled"] as const;
export type InstallationStatusValue = (typeof INSTALLATION_STATUSES)[number];
export function isInstallationStatus(value: string | null | undefined): value is InstallationStatusValue {
  return INSTALLATION_STATUSES.some((status) => status === value);
}

export const DEFECT_SEVERITIES = ["minor", "major", "critical"] as const;
export type DefectSeverityValue = (typeof DEFECT_SEVERITIES)[number];
export function isDefectSeverity(value: string | null | undefined): value is DefectSeverityValue {
  return DEFECT_SEVERITIES.some((severity) => severity === value);
}

export const DEFECT_STATUSES = ["open", "resolved", "verified", "reopened"] as const;
export type DefectStatusValue = (typeof DEFECT_STATUSES)[number];
export function isDefectStatus(value: string | null | undefined): value is DefectStatusValue {
  return DEFECT_STATUSES.some((status) => status === value);
}

export const SERVICE_REQUEST_STATUSES = ["open", "scheduled", "in_progress", "resolved", "closed"] as const;
export type ServiceRequestStatusValue = (typeof SERVICE_REQUEST_STATUSES)[number];
export function isServiceRequestStatus(value: string | null | undefined): value is ServiceRequestStatusValue {
  return SERVICE_REQUEST_STATUSES.some((status) => status === value);
}

export const SERVICE_PRIORITIES = ["low", "normal", "high", "urgent"] as const;
export type ServicePriorityValue = (typeof SERVICE_PRIORITIES)[number];
export function isServicePriority(value: string | null | undefined): value is ServicePriorityValue {
  return SERVICE_PRIORITIES.some((priority) => priority === value);
}

export type ServiceBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function installationStatusVariant(status: string | null | undefined): ServiceBadgeVariant {
  switch (status) {
    case "handed_over":
      return "success";
    case "in_progress":
    case "ready_for_handover":
      return "info";
    case "cancelled":
      return "danger";
    default:
      return "neutral";
  }
}

export function defectStatusVariant(status: string | null | undefined): ServiceBadgeVariant {
  switch (status) {
    case "verified":
      return "success";
    case "resolved":
      return "info";
    case "open":
    case "reopened":
      return "danger";
    default:
      return "neutral";
  }
}

export function serviceRequestStatusVariant(status: string | null | undefined): ServiceBadgeVariant {
  switch (status) {
    case "closed":
    case "resolved":
      return "success";
    case "in_progress":
    case "scheduled":
      return "info";
    case "open":
      return "warning";
    default:
      return "neutral";
  }
}

const KNOWN_ERROR_CODES = [
  "INSTALLATION_VERSION_CONFLICT",
  "INSTALLATION_INVALID_STATE",
  "INSTALLATION_PROJECT_NOT_ACTIVE",
  "INSTALLATION_CHECKLIST_INCOMPLETE",
  "INSTALLATION_DEFECTS_OPEN",
  "INSTALLATION_SELF_VERIFICATION",
  "INSTALLATION_DEFECT_INVALID_STATE",
  "INSTALLATION_ITEM_NOT_FOUND",
  "INSTALLATION_REASON_REQUIRED",
  "INSTALLATION_FIELD_REQUIRED",
  "INSTALLATION_FIELD_INVALID",
  "SERVICE_VERSION_CONFLICT",
  "SERVICE_INVALID_STATE",
  "SERVICE_PROJECT_INVALID",
  "SERVICE_REASON_REQUIRED",
  "SERVICE_FIELD_REQUIRED",
  "SERVICE_FIELD_INVALID",
] as const;

export type ServiceErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function serviceErrorCode(code: string | null | undefined): ServiceErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
