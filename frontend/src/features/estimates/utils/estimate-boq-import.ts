export type BoqImportField = "code" | "description" | "quantity" | "unitCost" | "margin";

export interface BoqImportRow {
  code: string;
  descriptionTh: string;
  descriptionEn: string;
  quantity: number;
  unitCode: string;
  unitCost: number;
  /** Selling margin in percent, as typed in the workspace form. */
  marginPercent: number;
}

export interface BoqImportError {
  row: number;
  field: BoqImportField;
}

/** Column order of the import sheet; row 1 is the header and is skipped. */
export const BOQ_IMPORT_COLUMNS = ["code", "descriptionTh", "descriptionEn", "quantity", "unitCode", "unitCost", "marginPercent"] as const;

const DEFAULT_MARGIN_PERCENT = 25;
const MAX_MARGIN_PERCENT = 95;

function text(value: unknown): string {
  return value === null || value === undefined ? "" : String(value).trim();
}

function number(value: unknown): number {
  if (typeof value === "number") return value;
  const raw = text(value).replace(/,/g, "");
  return raw === "" ? Number.NaN : Number(raw);
}

/** Validates raw sheet rows (index 0 = header). Blank rows are ignored; any invalid cell is reported, never guessed. */
export function parseBoqImportRows(sheet: unknown[][]): { rows: BoqImportRow[]; errors: BoqImportError[] } {
  const rows: BoqImportRow[] = [];
  const errors: BoqImportError[] = [];
  sheet.slice(1).forEach((cells, offset) => {
    if (cells.every((cell) => text(cell) === "")) return;
    const row = offset + 2;
    const [code, descriptionTh, descriptionEn, quantity, unitCode, unitCost, margin] = cells;
    const before = errors.length;
    if (text(code) === "") errors.push({ row, field: "code" });
    if (text(descriptionTh) === "") errors.push({ row, field: "description" });
    const qty = number(quantity);
    if (!(qty > 0)) errors.push({ row, field: "quantity" });
    const cost = number(unitCost);
    if (!(cost >= 0)) errors.push({ row, field: "unitCost" });
    const marginPercent = text(margin) === "" ? DEFAULT_MARGIN_PERCENT : number(margin);
    if (!(marginPercent >= 0 && marginPercent <= MAX_MARGIN_PERCENT)) errors.push({ row, field: "margin" });
    if (errors.length > before) return;
    rows.push({ code: text(code), descriptionTh: text(descriptionTh), descriptionEn: text(descriptionEn), quantity: qty, unitCode: text(unitCode) || "lot", unitCost: cost, marginPercent });
  });
  return { rows, errors };
}
