/** Column order of the Item import CSV; must match the backend header check exactly. */
export const ITEM_IMPORT_COLUMNS = [
  "code",
  "itemType",
  "categoryCode",
  "brandCode",
  "baseUnitCode",
  "nameTh",
  "nameEn",
  "descriptionTh",
  "descriptionEn",
  "taxCategoryCode",
  "canSell",
  "canCost",
  "canPurchase",
  "canStock",
  "canProduce",
] as const;

export type ItemImportColumn = (typeof ITEM_IMPORT_COLUMNS)[number];

/** Cost import CSV columns; must match the backend header check (CostImportCsv.Columns) exactly. */
export const COST_IMPORT_COLUMNS = [
  "itemCode",
  "scope",
  "branchCode",
  "unitCode",
  "currency",
  "amount",
  "minimumQuantity",
  "maximumQuantity",
  "effectiveFromUtc",
  "effectiveToUtc",
  "costSourceCode",
  "sourceReference",
  "reason",
] as const;

export type CostImportColumn = (typeof COST_IMPORT_COLUMNS)[number];

/** Limits of the immediate preview/commit path (ItemImportLimits.MaxContentLength / MaxRows). Larger files run as a background job. */
export const ITEM_IMPORT_MAX_FILE_BYTES = 1_000_000;
export const ITEM_IMPORT_MAX_SYNC_ROWS = 500;
/** Limit of the background job path (ItemImportLimits.MaxJobContentLength). */
export const ITEM_IMPORT_JOB_MAX_FILE_BYTES = 10_000_000;
/** How often the job status is polled while the job is not finished. */
export const ITEM_IMPORT_JOB_POLL_MS = 2_000;

export const ITEM_IMPORT_KINDS = ["items", "costs"] as const;
export type ItemImportKind = (typeof ITEM_IMPORT_KINDS)[number];

export const ITEM_IMPORT_MODES = ["create", "upsert"] as const;
export type ItemImportMode = (typeof ITEM_IMPORT_MODES)[number];

export const ITEM_IMPORT_OUTCOMES = ["created", "updated", "unchanged"] as const;
export type ItemImportOutcome = (typeof ITEM_IMPORT_OUTCOMES)[number];

export const ITEM_IMPORT_JOB_STATUSES = ["queued", "running", "completed", "completed_with_errors", "failed"] as const;
export type ItemImportJobStatus = (typeof ITEM_IMPORT_JOB_STATUSES)[number];

/** Failure codes a job can end with (backend error codes surfaced as failureCode). */
export const ITEM_IMPORT_JOB_FAILURE_CODES = [
  "ITEM_IMPORT_JOB_FAILED",
  "ITEM_IMPORT_BATCH_CONFLICT",
  "ITEM_IMPORT_FILE_INVALID",
  "PERMISSION_DENIED",
] as const;
export type ItemImportJobFailureCode = (typeof ITEM_IMPORT_JOB_FAILURE_CODES)[number];

/** Backend per-cell error codes (ItemImportErrorCodes). */
export const ITEM_IMPORT_ERROR_CODES = [
  "REQUIRED",
  "INVALID",
  "TOO_LONG",
  "NOT_FOUND",
  "INACTIVE",
  "DUPLICATE_IN_FILE",
  "ALREADY_EXISTS",
  "MISMATCH",
  "LOCKED",
  "BRANCH_ACCESS_DENIED",
  "DUPLICATE_COST",
] as const;

export type ItemImportErrorCode = (typeof ITEM_IMPORT_ERROR_CODES)[number];

export function isItemImportErrorCode(value: string | null | undefined): value is ItemImportErrorCode {
  return ITEM_IMPORT_ERROR_CODES.some((code) => code === value);
}

/** Header-only CSV template; example values are deliberately not included so no sample data can be imported by accident. */
export function buildItemImportTemplate(): string {
  return `${ITEM_IMPORT_COLUMNS.join(",")}\n`;
}

export function buildCostImportTemplate(): string {
  return `${COST_IMPORT_COLUMNS.join(",")}\n`;
}

export function isItemImportKind(value: string | null | undefined): value is ItemImportKind {
  return ITEM_IMPORT_KINDS.some((kind) => kind === value);
}

export function isItemImportMode(value: string | null | undefined): value is ItemImportMode {
  return ITEM_IMPORT_MODES.some((mode) => mode === value);
}

export function isItemImportOutcome(value: string | null | undefined): value is ItemImportOutcome {
  return ITEM_IMPORT_OUTCOMES.some((outcome) => outcome === value);
}

export function isItemImportJobStatus(value: string | null | undefined): value is ItemImportJobStatus {
  return ITEM_IMPORT_JOB_STATUSES.some((status) => status === value);
}

export function isItemImportJobFailureCode(value: string | null | undefined): value is ItemImportJobFailureCode {
  return ITEM_IMPORT_JOB_FAILURE_CODES.some((code) => code === value);
}

/** True once the job will not change any more, so polling can stop. */
export function isItemImportJobFinished(status: ItemImportJobStatus): boolean {
  return status === "completed" || status === "completed_with_errors" || status === "failed";
}

/**
 * A file the immediate preview cannot take (over 1 MB or more than 500 data lines) is sent as a background job instead.
 * Line counting is a cheap estimate (quoted line breaks count too); the backend re-checks the real row count.
 */
export function needsBackgroundJob(content: string, byteLength: number): boolean {
  if (byteLength > ITEM_IMPORT_MAX_FILE_BYTES) return true;
  const lineCount = content.split(/\r\n|\n|\r/).filter((line) => line.trim().length > 0).length;
  return lineCount - 1 > ITEM_IMPORT_MAX_SYNC_ROWS;
}
