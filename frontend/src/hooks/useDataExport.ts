"use client";

import { useState, useCallback } from "react";
import { exportToCsv } from "@/lib/export/export-csv";
import { exportToExcel } from "@/lib/export/export-excel";
import type { ExportColumn, ExportFormat } from "@/lib/export/export-types";
import { apiClient } from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";

export interface UseDataExportOptions<T, TId = string | number> {
  filename: string;
  sheetName?: string;
  resourceType?: string;
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
  resourceType,
  columns,
  data = [],
  selectedIds,
  getId,
  fetchAll,
}: UseDataExportOptions<T, TId>): UseDataExportResult {
  const [isExporting, setIsExporting] = useState(false);
  const context = useApiRequestContext();

  const recordAudit = useCallback(
    async (rowCount: number, format: ExportFormat, actualFilename: string) => {
      if (!context.membershipId) return;
      try {
        const options = await context.buildOptions();
        await apiClient.recordExportAudit(
          {
            resourceType: resourceType || filename,
            rowCount,
            format,
            filename: actualFilename,
          },
          options
        );
      } catch (err) {
        // Logging failure does not disrupt the user download
        console.warn("Audit recording for data export failed", err);
      }
    },
    [context, filename, resourceType]
  );

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
        void recordAudit(exportData.length, format, datedName);
      } finally {
        setIsExporting(false);
      }
    },
    [columns, data, fetchAll, getDatedFilename, isExporting, recordAudit, sheetName]
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
      void recordAudit(selectedData.length, format, datedName);
    },
    [columns, data, getDatedFilename, getId, recordAudit, selectedIds, sheetName]
  );

  return {
    isExporting,
    exportAll,
    exportSelected,
  };
}
