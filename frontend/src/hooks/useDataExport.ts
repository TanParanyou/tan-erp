"use client";

import { useState, useCallback } from "react";
import { exportToCsv } from "@/lib/export/export-csv";
import { exportToExcel } from "@/lib/export/export-excel";
import type { ExportColumn, ExportFormat } from "@/lib/export/export-types";

export interface UseDataExportOptions<T, TId = string | number> {
  filename: string;
  sheetName?: string;
  columns: readonly ExportColumn<T>[];
  data?: readonly T[];
  selectedIds?: ReadonlySet<TId>;
  getId?: (item: T) => TId | undefined;
  fetchAll?: () => Promise<readonly T[]>;
}

export interface UseDataExportResult {
  isExporting: boolean;
  exportAll: (format?: ExportFormat) => Promise<void>;
  exportSelected: (format?: ExportFormat) => Promise<void>;
}

export function useDataExport<T, TId = string | number>({
  filename,
  sheetName,
  columns,
  data = [],
  selectedIds,
  getId,
  fetchAll,
}: UseDataExportOptions<T, TId>): UseDataExportResult {
  const [isExporting, setIsExporting] = useState(false);

  const getDatedFilename = useCallback(
    (suffix: string) => {
      const dateStr = new Date().toISOString().slice(0, 10);
      return `${filename}_${suffix}_${dateStr}`;
    },
    [filename]
  );

  const exportAll = useCallback(
    async (format: ExportFormat = "csv") => {
      if (isExporting) return;

      try {
        setIsExporting(true);
        let exportData: readonly T[] = data;
        if (fetchAll) {
          exportData = await fetchAll();
        }

        if (exportData.length === 0) return;

        const datedName = getDatedFilename("all");
        if (format === "xlsx") {
          await exportToExcel({
            filename: datedName,
            sheetName,
            columns,
            data: exportData,
          });
        } else {
          exportToCsv({
            filename: datedName,
            columns,
            data: exportData,
          });
        }
      } finally {
        setIsExporting(false);
      }
    },
    [columns, data, fetchAll, getDatedFilename, isExporting, sheetName]
  );

  const exportSelected = useCallback(
    async (format: ExportFormat = "csv") => {
      if (!selectedIds || selectedIds.size === 0) return;

      const selectedData = data.filter((item) => {
        const id = getId ? getId(item) : (item as Record<string, unknown>).id;
        return id !== undefined && selectedIds.has(id as TId);
      });

      if (selectedData.length === 0) return;

      const datedName = getDatedFilename("selected");
      if (format === "xlsx") {
        await exportToExcel({
          filename: datedName,
          sheetName,
          columns,
          data: selectedData,
        });
      } else {
        exportToCsv({
          filename: datedName,
          columns,
          data: selectedData,
        });
      }
    },
    [columns, data, getDatedFilename, getId, selectedIds, sheetName]
  );

  return {
    isExporting,
    exportAll,
    exportSelected,
  };
}
