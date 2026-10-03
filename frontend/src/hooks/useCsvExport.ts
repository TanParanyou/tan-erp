"use client";

import { useCallback } from "react";
import type { CsvColumn } from "@/lib/export/export-csv";
import { useDataExport, type UseDataExportOptions } from "./useDataExport";

export type UseCsvExportOptions<T, TId = string | number> = UseDataExportOptions<T, TId> & {
  columns: readonly CsvColumn<T>[];
};

export interface UseCsvExportResult {
  isExporting: boolean;
  exportAll: () => Promise<void>;
  exportSelected: () => void;
}

export function useCsvExport<T, TId = string | number>(
  options: UseCsvExportOptions<T, TId>
): UseCsvExportResult {
  const { isExporting, exportAll: baseExportAll, exportSelected: baseExportSelected } =
    useDataExport<T, TId>(options);

  const exportAll = useCallback(async () => {
    await baseExportAll("csv");
  }, [baseExportAll]);

  const exportSelected = useCallback(() => {
    void baseExportSelected("csv");
  }, [baseExportSelected]);

  return {
    isExporting,
    exportAll,
    exportSelected,
  };
}
