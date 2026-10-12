/** Document types an approval limit can apply to. Values match the backend approval document types. */
export const APPROVAL_DOCUMENT_TYPES = ["estimate", "purchase_order", "change_order"] as const;

export type ApprovalDocumentType = (typeof APPROVAL_DOCUMENT_TYPES)[number];

/** Narrows an untyped API value to a known approval document type. */
export const isApprovalDocumentType = (value: string): value is ApprovalDocumentType =>
  APPROVAL_DOCUMENT_TYPES.some((type) => type === value);
