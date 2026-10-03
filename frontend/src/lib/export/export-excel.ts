import ExcelJS from "exceljs";
import type { ExportColumn } from "./export-types";

export interface ExcelExportOptions<T> {
  filename: string;
  sheetName?: string;
  columns: readonly ExportColumn<T>[];
  data: readonly T[];
}

export function buildExcelWorkbook<T>(
  columns: readonly ExportColumn<T>[],
  data: readonly T[],
  sheetName = "Sheet1"
): ExcelJS.Workbook {
  const workbook = new ExcelJS.Workbook();
  workbook.creator = "Project ERP";
  workbook.created = new Date();

  const worksheet = workbook.addWorksheet(sheetName, {
    views: [{ state: "frozen", ySplit: 1 }],
  });

  // Calculate column widths based on content
  worksheet.columns = columns.map((col) => {
    let maxLen = col.header.length;
    data.forEach((item) => {
      const val = col.accessor(item);
      if (val !== null && val !== undefined) {
        const str = String(val);
        if (str.length > maxLen) {
          maxLen = str.length;
        }
      }
    });
    return {
      header: col.header,
      key: col.header,
      width: Math.min(Math.max(maxLen + 4, 14), 50),
    };
  });

  // Header styling: Atelier Solid Navy with white text
  const headerRow = worksheet.getRow(1);
  headerRow.height = 26;
  headerRow.font = { name: "Segoe UI", bold: true, color: { argb: "FFFFFFFF" }, size: 10 };
  headerRow.fill = {
    type: "pattern",
    pattern: "solid",
    fgColor: { argb: "FF0B3056" }, // #0B3056 Solid Navy
  };
  headerRow.alignment = { vertical: "middle", horizontal: "left" };

  // Add data rows with clean styling
  data.forEach((item) => {
    const rowValues = columns.map((col) => {
      const val = col.accessor(item);
      if (val === undefined || val === null) return "";
      return val;
    });
    const row = worksheet.addRow(rowValues);
    row.height = 20;
    row.alignment = { vertical: "middle" };
    row.font = { name: "Segoe UI", size: 10 };
  });

  return workbook;
}

export async function exportToExcel<T>({
  filename,
  sheetName = "Sheet1",
  columns,
  data,
}: ExcelExportOptions<T>): Promise<void> {
  const workbook = buildExcelWorkbook(columns, data, sheetName);
  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], {
    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
  });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.setAttribute("href", url);
  link.setAttribute("download", filename.endsWith(".xlsx") ? filename : `${filename}.xlsx`);
  link.style.visibility = "hidden";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}
