/**
 * Notification types the UI can render. Mirrors the backend registry (NotificationTypes / NotificationTypeRegistry):
 * register a new type in the backend first, then here and in messages (th + en). A type that is not listed is never
 * rendered from guesses; callers fall back to the generic "unknown" message.
 */
export type NotificationPayloadField = "documentNumber" | "actorDisplayName" | "subjectDisplayName" | "roleName";

/** Key under `notifications.types` in the messages (JSON keys cannot contain dots). */
export type NotificationMessageKey =
  | "estimateApprovalRequested"
  | "costRecordApprovalRequested"
  | "purchaseOrderApprovalRequested"
  | "changeOrderApprovalRequested"
  | "mrpRunApprovalRequested"
  | "roleAssignmentApprovalRequested";

export interface NotificationTypeDescriptor {
  messageKey: NotificationMessageKey;
  /** Payload fields the message uses; all other payload fields are ignored. */
  fields: readonly NotificationPayloadField[];
}

export const NOTIFICATION_TYPES = {
  "estimate.approval-requested": {
    messageKey: "estimateApprovalRequested",
    fields: ["documentNumber", "actorDisplayName"],
  },
  "cost-record.approval-requested": {
    messageKey: "costRecordApprovalRequested",
    fields: ["documentNumber", "actorDisplayName"],
  },
  "purchase-order.approval-requested": {
    messageKey: "purchaseOrderApprovalRequested",
    fields: ["documentNumber", "actorDisplayName"],
  },
  "change-order.approval-requested": {
    messageKey: "changeOrderApprovalRequested",
    fields: ["documentNumber", "actorDisplayName"],
  },
  "mrp-run.approval-requested": {
    messageKey: "mrpRunApprovalRequested",
    fields: ["documentNumber", "actorDisplayName"],
  },
  "role-assignment.approval-requested": {
    messageKey: "roleAssignmentApprovalRequested",
    fields: ["subjectDisplayName", "roleName", "actorDisplayName"],
  },
} as const satisfies Record<string, NotificationTypeDescriptor>;

export type NotificationType = keyof typeof NOTIFICATION_TYPES;

export function isNotificationType(value: string): value is NotificationType {
  return Object.prototype.hasOwnProperty.call(NOTIFICATION_TYPES, value);
}

export function notificationMessageKey(type: string): NotificationMessageKey | null {
  return isNotificationType(type) ? NOTIFICATION_TYPES[type].messageKey : null;
}

/** Values for the type's message: only declared fields, and "-" for a field the payload does not carry. */
export function notificationMessageValues(
  type: NotificationType,
  payload: Readonly<Record<string, string>>,
): Record<string, string> {
  const values: Record<string, string> = {};
  for (const field of NOTIFICATION_TYPES[type].fields) {
    values[field] = payload[field] ?? "-";
  }
  return values;
}
