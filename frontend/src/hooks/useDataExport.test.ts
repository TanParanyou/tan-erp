import { describe, it, expect, vi } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useDataExport } from "./useDataExport";
import * as csvExportUtils from "@/lib/export/export-csv";
import * as excelExportUtils from "@/lib/export/export-excel";
import type { ExportColumn } from "@/lib/export/export-types";

describe("useDataExport hook", () => {
  interface Item {
    id: number;
    name: string;
    code: string;
  }

  const columns: ExportColumn<Item>[] = [
    { header: "Code", accessor: (item) => item.code },
    { header: "Name", accessor: (item) => item.name },
  ];

  const data: Item[] = [
    { id: 1, code: "C001", name: "Alpha" },
    { id: 2, code: "C002", name: "Beta" },
  ];

  it("exports CSV by default for exportAll", async () => {
    const csvSpy = vi.spyOn(csvExportUtils, "exportToCsv").mockImplementation(() => {});
    const excelSpy = vi.spyOn(excelExportUtils, "exportToExcel").mockResolvedValue();

    const { result } = renderHook(() =>
      useDataExport({
        filename: "test_export",
        columns,
        data,
      })
    );

    await act(async () => {
      await result.current.exportAll();
    });

    expect(csvSpy).toHaveBeenCalledTimes(1);
    expect(excelSpy).not.toHaveBeenCalled();

    csvSpy.mockRestore();
    excelSpy.mockRestore();
  });

  it("exports Excel when 'xlsx' format is passed to exportAll", async () => {
    const csvSpy = vi.spyOn(csvExportUtils, "exportToCsv").mockImplementation(() => {});
    const excelSpy = vi.spyOn(excelExportUtils, "exportToExcel").mockResolvedValue();

    const { result } = renderHook(() =>
      useDataExport({
        filename: "test_export",
        columns,
        data,
      })
    );

    await act(async () => {
      await result.current.exportAll("xlsx");
    });

    expect(excelSpy).toHaveBeenCalledTimes(1);
    expect(csvSpy).not.toHaveBeenCalled();
    expect(excelSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        columns,
        data,
      })
    );

    csvSpy.mockRestore();
    excelSpy.mockRestore();
  });

  it("exports selected rows as Excel when format is 'xlsx'", async () => {
    const excelSpy = vi.spyOn(excelExportUtils, "exportToExcel").mockResolvedValue();
    const selectedIds = new Set([2]);

    const { result } = renderHook(() =>
      useDataExport({
        filename: "test_export",
        columns,
        data,
        selectedIds,
        getId: (item) => item.id,
      })
    );

    await act(async () => {
      await result.current.exportSelected("xlsx");
    });

    expect(excelSpy).toHaveBeenCalledTimes(1);
    expect(excelSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        columns,
        data: [{ id: 2, code: "C002", name: "Beta" }],
      })
    );

    excelSpy.mockRestore();
  });
});
