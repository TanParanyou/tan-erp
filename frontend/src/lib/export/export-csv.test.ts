import { describe, it, expect } from "vitest";
import { escapeCsvCell, generateCsvContent, type CsvColumn } from "./export-csv";

describe("export-csv utils", () => {
  it("escapeCsvCell properly handles commas, quotes, formula injections, and newlines", () => {
    expect(escapeCsvCell("simple")).toBe("simple");
    expect(escapeCsvCell("hello, world")).toBe('"hello, world"');
    expect(escapeCsvCell('with "quotes"')).toBe('"with ""quotes"""');
    expect(escapeCsvCell("line1\nline2")).toBe('"line1\nline2"');
    expect(escapeCsvCell("=SUM(A1:A5)")).toBe("'=SUM(A1:A5)");
    expect(escapeCsvCell(null)).toBe("");
    expect(escapeCsvCell(undefined)).toBe("");
    expect(escapeCsvCell(123.45)).toBe("123.45");
    expect(escapeCsvCell(true)).toBe("true");
  });

  it("generateCsvContent generates valid CSV rows with UTF-8 content", () => {
    interface CustomerData {
      name: string;
      taxId: string;
      notes: string;
    }

    const columns: CsvColumn<CustomerData>[] = [
      { header: "ชื่อลูกค้า", accessor: (d) => d.name },
      { header: "เลขประจำตัวผู้เสียภาษี", accessor: (d) => d.taxId },
      { header: "หมายเหตุ", accessor: (d) => d.notes },
    ];

    const data: CustomerData[] = [
      { name: "บริษัท สยาม จำกัด", taxId: "0105550000000", notes: "เครดิต 30 วัน, ส่งของเช้า" },
      { name: "Somchai, J.", taxId: "1234567890123", notes: "ลูกค้ารายย่อย" },
    ];

    const csv = generateCsvContent(columns, data);
    const rows = csv.split("\r\n");

    expect(rows.length).toBe(3);
    expect(rows[0]).toBe("ชื่อลูกค้า,เลขประจำตัวผู้เสียภาษี,หมายเหตุ");
    expect(rows[1]).toBe('บริษัท สยาม จำกัด,0105550000000,"เครดิต 30 วัน, ส่งของเช้า"');
    expect(rows[2]).toBe('"Somchai, J.",1234567890123,ลูกค้ารายย่อย');
  });
});
