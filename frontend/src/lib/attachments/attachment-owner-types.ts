/**
 * Owner types and enums of the shared attachment API. These mirror the backend whitelist
 * (AttachmentOwnerTypes / AttachmentPurposes); register a new owner type in the backend first.
 */
export const ATTACHMENT_OWNER_TYPES = ["installation-job"] as const;
export type AttachmentOwnerType = (typeof ATTACHMENT_OWNER_TYPES)[number];

export const ATTACHMENT_PURPOSES = ["general", "evidence", "handover", "defect"] as const;
export type AttachmentPurpose = (typeof ATTACHMENT_PURPOSES)[number];

export function isAttachmentPurpose(value: string): value is AttachmentPurpose {
  return ATTACHMENT_PURPOSES.some((purpose) => purpose === value);
}

export const SIGNATURE_PURPOSES = ["handover"] as const;
export type SignaturePurpose = (typeof SIGNATURE_PURPOSES)[number];

/** Consent wording version the server currently accepts per signature purpose (the wording itself is in messages). */
export const SIGNATURE_CONSENT_VERSIONS = {
  handover: "handover-2026-10-v1",
} as const satisfies Record<SignaturePurpose, string>;

/** Files module limits (TEST_ONLY): JPEG/PNG/WebP up to 10 MB each. */
export const ATTACHMENT_ACCEPTED_MEDIA_TYPES = ["image/jpeg", "image/png", "image/webp"] as const;
export const ATTACHMENT_MAX_FILE_BYTES = 10 * 1024 * 1024;

export const ATTACHMENT_ERROR_CODES = [
  "ATTACHMENT_OWNER_TYPE_INVALID",
  "ATTACHMENT_PURPOSE_INVALID",
  "ATTACHMENT_FIELD_INVALID",
  "ATTACHMENT_FILE_NOT_READY",
  "ATTACHMENT_FILE_SCOPE_MISMATCH",
  "ATTACHMENT_DUPLICATE",
  "ATTACHMENT_LIMIT_EXCEEDED",
  "ATTACHMENT_OWNER_LOCKED",
  "SIGNATURE_SUBMISSION_INVALID",
  "SIGNATURE_CONSENT_REQUIRED",
  "SIGNATURE_IMAGE_INVALID",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

export type AttachmentErrorCode = (typeof ATTACHMENT_ERROR_CODES)[number];

export function attachmentErrorCode(code: string | null | undefined): AttachmentErrorCode | null {
  return ATTACHMENT_ERROR_CODES.find((known) => known === code) ?? null;
}
