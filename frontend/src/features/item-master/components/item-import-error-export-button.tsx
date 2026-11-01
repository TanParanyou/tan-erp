"use client";

import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { IconDownload } from "@/components/common/Icons";
import {
  buildImportErrorReportRows,
  downloadImportErrorReport,
  type ImportErrorSource,
} from "../item-import-error-report";
import {
  COST_IMPORT_COLUMNS,
  isItemImportErrorCode,
  ITEM_IMPORT_COLUMNS,
  type CostImportColumn,
  type ItemImportColumn,
} from "../item-import-constants";

export type ImportField = ItemImportColumn | CostImportColumn | "capabilities" | "row";

export function isImportField(value: string | null | undefined): value is ImportField {
  return (
    value === "capabilities" ||
    value === "row" ||
    ITEM_IMPORT_COLUMNS.some((column) => column === value) ||
    COST_IMPORT_COLUMNS.some((column) => column === value)
  );
}

interface ItemImportErrorExportButtonProps {
  sources: readonly ImportErrorSource[];
  className?: string;
}

/** Downloads every row error as a CSV (row, column, cause, fix) so the user knows exactly what to correct in the file. */
export function ItemImportErrorExportButton({ sources, className }: ItemImportErrorExportButtonProps) {
  const t = useTranslations("itemMaster.import");

  function handleExport(): void {
    const rows = buildImportErrorReportRows(sources, (field, code) => ({
      column: isImportField(field) ? t(`fields.${field}`) : "-",
      reason: isItemImportErrorCode(code) ? t(`errors.${code}`) : "-",
      fix: isItemImportErrorCode(code) ? t(`errorHints.${code}`) : "-",
    }));
    downloadImportErrorReport(
      rows,
      {
        row: t("reportRow"),
        code: t("reportCode"),
        name: t("reportName"),
        column: t("reportColumn"),
        errorCode: t("reportErrorCode"),
        reason: t("reportReason"),
        fix: t("reportFix"),
      },
      t("reportFilePrefix"),
    );
  }

  return (
    <Button type="button" variant="outline" size="sm" icon={<IconDownload size={14} />} onClick={handleExport} className={className} title={t("exportErrorsHint")}>
      {t("exportErrors")}
    </Button>
  );
}
