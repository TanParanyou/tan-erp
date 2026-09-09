import { describe, it, expect, vi } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useCsvExport } from "./useCsvExport";
import * as exportUtils from "@/lib/export/export-csv";

describe("useCsvExport hook", () => {
  interface Item {
    id: number;
    name: string;
    code: string;
  }

  const columns = [
    { header: "Code", accessor: (item: Item) => item.code },
    { header: "Name", accessor: (item: Item) => item.name },
  ];

  const data: Item[] = [
    { id: 1, code: "C001", name: "Alpha" },
    { id: 2, code: "C002", name: "Beta" },
  ];

  it("exports all data directly when fetchAll is not provided", async () => {
    const exportSpy = vi.spyOn(exportUtils, "exportToCsv").mockImplementation(() => {});

    const { result } = renderHook(() =>
      useCsvExport({
        filename: "test_export",
        columns,
        data,
      })
    );

    await act(async () => {
      await result.current.exportAll();
    });

    expect(exportSpy).toHaveBeenCalledTimes(1);
    expect(exportSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        columns,
        data,
      })
    );

    exportSpy.mockRestore();
  });

  it("exports data fetched via fetchAll", async () => {
    const exportSpy = vi.spyOn(exportUtils, "exportToCsv").mockImplementation(() => {});
    const fetchedItems: Item[] = [
      { id: 3, code: "C003", name: "Gamma" },
      { id: 4, code: "C004", name: "Delta" },
    ];
    const fetchAll = vi.fn().mockResolvedValue(fetchedItems);

    const { result } = renderHook(() =>
      useCsvExport({
        filename: "test_export",
        columns,
        data,
        fetchAll,
      })
    );

    await act(async () => {
      await result.current.exportAll();
    });

    expect(fetchAll).toHaveBeenCalledTimes(1);
    expect(exportSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        columns,
        data: fetchedItems,
      })
    );

    exportSpy.mockRestore();
  });

  it("exports only selected rows", () => {
    const exportSpy = vi.spyOn(exportUtils, "exportToCsv").mockImplementation(() => {});
    const selectedIds = new Set([2]);

    const { result } = renderHook(() =>
      useCsvExport({
        filename: "test_export",
        columns,
        data,
        selectedIds,
        getId: (item) => item.id,
      })
    );

    act(() => {
      result.current.exportSelected();
    });

    expect(exportSpy).toHaveBeenCalledTimes(1);
    expect(exportSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        columns,
        data: [{ id: 2, code: "C002", name: "Beta" }],
      })
    );

    exportSpy.mockRestore();
  });
});
