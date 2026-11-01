import { exportToCsv, type CsvColumn } from "@/lib/export/export-csv";

/** One row of a preview or job response; only the fields the report needs. */
export interface ImportErrorSource {
  rowNumber?: number;
  code?: string | null;
  nameTh?: string | null;
  errors?: ReadonlyArray<{ field?: string | null; code?: string | null }> | null;
}

export interface ImportErrorReportRow {
  rowNumber: number | null;
  code: string | null;
  name: string | null;
  column: string;
  errorCode: string;
  reason: string;
  fix: string;
}

export interface ImportErrorDescription {
  column: string;
  reason: string;
  fix: string;
}

export interface ImportErrorReportHeaders {
  row: string;
  code: string;
  name: string;
  column: string;
  errorCode: string;
  reason: string;
  fix: string;
}

const MISSING = "-";

/** One report line per error (not per row) so the file can be filtered and worked through in a spreadsheet. */
export function buildImportErrorReportRows(
  sources: readonly ImportErrorSource[],
  describe: (field: string | null | undefined, code: string | null | undefined) => ImportErrorDescription,
): ImportErrorReportRow[] {
  return sources.flatMap((source) =>
    (source.errors ?? []).map((error) => {
      const description = describe(error.field, error.code);
      return {
        rowNumber: typeof source.rowNumber === "number" ? source.rowNumber : null,
        code: source.code ?? null,
        name: source.nameTh ?? null,
        column: description.column,
        errorCode: error.code ?? MISSING,
        reason: description.reason,
        fix: description.fix,
      };
    }),
  );
}

export function downloadImportErrorReport(
  rows: readonly ImportErrorReportRow[],
  headers: ImportErrorReportHeaders,
  filenamePrefix: string,
): void {
  const columns: CsvColumn<ImportErrorReportRow>[] = [
    { header: headers.row, accessor: (row) => row.rowNumber ?? MISSING },
    { header: headers.code, accessor: (row) => row.code ?? MISSING },
    { header: headers.name, accessor: (row) => row.name ?? MISSING },
    { header: headers.column, accessor: (row) => row.column },
    { header: headers.errorCode, accessor: (row) => row.errorCode },
    { header: headers.reason, accessor: (row) => row.reason },
    { header: headers.fix, accessor: (row) => row.fix },
  ];
  const stamp = new Date().toISOString().slice(0, 16).replace(/\D/g, "");
  exportToCsv({ filename: `${filenamePrefix}-${stamp}`, columns, data: rows });
}
