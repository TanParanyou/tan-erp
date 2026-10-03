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

export const ITEM_IMPORT_MAX_FILE_BYTES = 1_000_000;

/** Backend per-cell error codes (ItemImportErrorCodes). */
export const ITEM_IMPORT_ERROR_CODES = [
  "REQUIRED",
  "INVALID",
  "TOO_LONG",
  "NOT_FOUND",
  "INACTIVE",
  "DUPLICATE_IN_FILE",
  "ALREADY_EXISTS",
] as const;

export type ItemImportErrorCode = (typeof ITEM_IMPORT_ERROR_CODES)[number];

export function isItemImportErrorCode(value: string | null | undefined): value is ItemImportErrorCode {
  return ITEM_IMPORT_ERROR_CODES.some((code) => code === value);
}

/** Header-only CSV template; example values are deliberately not included so no sample data can be imported by accident. */
export function buildItemImportTemplate(): string {
  return `${ITEM_IMPORT_COLUMNS.join(",")}\n`;
}
