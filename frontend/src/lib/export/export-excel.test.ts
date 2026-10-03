import { describe, it, expect, vi } from "vitest";
import { buildExcelWorkbook, exportToExcel } from "./export-excel";
import type { ExportColumn } from "./export-types";

describe("export-excel utils", () => {
  interface SampleData {
    code: string;
    name: string;
    amount: number;
    active: boolean;
  }

  const columns: ExportColumn<SampleData>[] = [
    { header: "รหัส", accessor: (d) => d.code },
    { header: "ชื่อลูกค้า", accessor: (d) => d.name },
    { header: "ยอดเงิน", accessor: (d) => d.amount },
    { header: "สถานะ", accessor: (d) => d.active },
  ];

  const data: SampleData[] = [
    { code: "C001", name: "บริษัท สยาม จำกัด", amount: 15000, active: true },
    { code: "C002", name: "นาย สมชาย ใจดี", amount: 2500, active: false },
  ];

  it("buildExcelWorkbook creates a valid workbook with sheets, headers and data rows", () => {
    const workbook = buildExcelWorkbook(columns, data, "ลูกค้า");
    const worksheet = workbook.getWorksheet("ลูกค้า");

    expect(worksheet).toBeDefined();
    if (!worksheet) return;

    // Header row verification
    const headerRow = worksheet.getRow(1);
    expect(headerRow.getCell(1).value).toBe("รหัส");
    expect(headerRow.getCell(2).value).toBe("ชื่อลูกค้า");
    expect(headerRow.getCell(3).value).toBe("ยอดเงิน");
    expect(headerRow.getCell(4).value).toBe("สถานะ");

    // Header styling check (Atelier Solid Navy #0B3056)
    expect(headerRow.fill).toEqual(
      expect.objectContaining({
        type: "pattern",
        pattern: "solid",
        fgColor: { argb: "FF0B3056" },
      })
    );

    // Data rows verification
    const row1 = worksheet.getRow(2);
    expect(row1.getCell(1).value).toBe("C001");
    expect(row1.getCell(2).value).toBe("บริษัท สยาม จำกัด");
    expect(row1.getCell(3).value).toBe(15000);
    expect(row1.getCell(4).value).toBe(true);

    const row2 = worksheet.getRow(3);
    expect(row2.getCell(1).value).toBe("C002");
    expect(row2.getCell(2).value).toBe("นาย สมชาย ใจดี");
    expect(row2.getCell(3).value).toBe(2500);
    expect(row2.getCell(4).value).toBe(false);

    expect(worksheet.rowCount).toBe(3);
  });

  it("exportToExcel generates buffer and creates download link in DOM", async () => {
    const createObjectURLSpy = vi.fn().mockReturnValue("blob:mock-url");
    const revokeObjectURLSpy = vi.fn();
    const clickSpy = vi.fn();

    global.URL.createObjectURL = createObjectURLSpy;
    global.URL.revokeObjectURL = revokeObjectURLSpy;

    const mockAnchor = {
      setAttribute: vi.fn(),
      click: clickSpy,
      style: {},
      href: "",
      download: "",
    };

    vi.spyOn(document, "createElement").mockImplementation((tag) => {
      if (tag === "a") return mockAnchor as unknown as HTMLElement;
      return document.createElement(tag);
    });
    vi.spyOn(document.body, "appendChild").mockImplementation((node) => node);
    vi.spyOn(document.body, "removeChild").mockImplementation((node) => node);

    await exportToExcel({
      filename: "customers_export",
      columns,
      data,
    });

    expect(createObjectURLSpy).toHaveBeenCalled();
    expect(mockAnchor.setAttribute).toHaveBeenCalledWith("download", "customers_export.xlsx");
    expect(clickSpy).toHaveBeenCalled();
    expect(revokeObjectURLSpy).toHaveBeenCalledWith("blob:mock-url");
  });
});
