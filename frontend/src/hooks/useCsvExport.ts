"use client";

import { useState, useCallback } from "react";
import { exportToCsv, type CsvColumn } from "@/lib/export/export-csv";

export interface UseCsvExportOptions<T, TId = string | number> {
  filename: string;
  columns: readonly CsvColumn<T>[];
  data?: readonly T[];
  selectedIds?: ReadonlySet<TId>;
  getId?: (item: T) => TId | undefined;
  fetchAll?: () => Promise<readonly T[]>;
}

export interface UseCsvExportResult {
  isExporting: boolean;
  exportAll: () => Promise<void>;
  exportSelected: () => void;
}

export function useCsvExport<T, TId = string | number>({
  filename,
  columns,
  data = [],
  selectedIds,
  getId,
  fetchAll,
}: UseCsvExportOptions<T, TId>): UseCsvExportResult {
  const [isExporting, setIsExporting] = useState(false);

  const getDatedFilename = useCallback(
    (suffix: string) => {
      const dateStr = new Date().toISOString().slice(0, 10);
      return `${filename}_${suffix}_${dateStr}`;
    },
    [filename]
  );

  const exportAll = useCallback(async () => {
    if (isExporting) return;

    try {
      setIsExporting(true);
      let exportData: readonly T[] = data;
      if (fetchAll) {
        exportData = await fetchAll();
      }

      if (exportData.length === 0) return;

      exportToCsv({
        filename: getDatedFilename("all"),
        columns,
        data: exportData,
      });
    } finally {
      setIsExporting(false);
    }
  }, [columns, data, fetchAll, getDatedFilename, isExporting]);

  const exportSelected = useCallback(() => {
    if (!selectedIds || selectedIds.size === 0) return;

    const selectedData = data.filter((item) => {
      const id = getId ? getId(item) : (item as Record<string, unknown>).id;
      return id !== undefined && selectedIds.has(id as TId);
    });

    if (selectedData.length === 0) return;

    exportToCsv({
      filename: getDatedFilename("selected"),
      columns,
      data: selectedData,
    });
  }, [columns, data, getDatedFilename, getId, selectedIds]);

  return {
    isExporting,
    exportAll,
    exportSelected,
  };
}
