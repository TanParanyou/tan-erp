"use client";

import { useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { IconDownload, IconUpload } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { ItemImportPreviewResponse, ItemImportRowResponse } from "@/lib/api/api-client";
import { useItemImport } from "../api/item-master-queries";
import {
  buildItemImportTemplate,
  isItemImportErrorCode,
  ITEM_IMPORT_COLUMNS,
  ITEM_IMPORT_MAX_FILE_BYTES,
  type ItemImportColumn,
} from "../item-import-constants";

interface ItemImportModalProps {
  isOpen: boolean;
  onClose: () => void;
}

const MAX_VISIBLE_ROWS = 50;

interface StrictPreview {
  contentSha256: string;
  totalRows: number;
  validRows: number;
  invalidRows: number;
  rows: ItemImportRowResponse[];
}

class IncompleteImportResponseError extends Error {}

/** The generated response type marks every field optional; reject a response that lacks the fields the UI depends on. */
function toStrictPreview(response: ItemImportPreviewResponse): StrictPreview {
  const { contentSha256, totalRows, validRows, invalidRows, rows } = response;
  if (
    typeof contentSha256 !== "string" ||
    typeof totalRows !== "number" ||
    typeof validRows !== "number" ||
    typeof invalidRows !== "number" ||
    !Array.isArray(rows)
  ) {
    throw new IncompleteImportResponseError("Item import preview response is incomplete");
  }
  return { contentSha256, totalRows, validRows, invalidRows, rows };
}

function isImportColumn(value: string | null | undefined): value is ItemImportColumn | "capabilities" {
  return value === "capabilities" || ITEM_IMPORT_COLUMNS.some((column) => column === value);
}

/**
 * Item CSV import: pick a file, preview per-row validation (nothing is written), then commit all rows at once.
 * Commit is only offered when every row is valid, mirroring the backend's all-or-nothing rule.
 */
export function ItemImportModal({ isOpen, onClose }: ItemImportModalProps) {
  const t = useTranslations("itemMaster.import");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { preview, commit } = useItemImport();
  const inputRef = useRef<HTMLInputElement>(null);
  const [content, setContent] = useState<string | null>(null);
  const [fileName, setFileName] = useState<string | null>(null);
  const [result, setResult] = useState<StrictPreview | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  // One idempotency key per previewed content, so a retried commit of the same file replays instead of duplicating.
  const commitKeyRef = useRef<{ hash: string; key: string } | null>(null);

  const isBusy = preview.isPending || commit.isPending;

  function reset(): void {
    setContent(null);
    setFileName(null);
    setResult(null);
    setErrorMessage(null);
    commitKeyRef.current = null;
    if (inputRef.current) inputRef.current.value = "";
  }

  function close(): void {
    if (isBusy) return;
    reset();
    onClose();
  }

  function describeError(error: unknown): string {
    if (error instanceof ApiError) {
      if (error.code === "ITEM_IMPORT_FILE_INVALID") return t("fileInvalid");
      if (error.code === "ITEM_IMPORT_VALIDATION_FAILED") return t("validationFailed");
      if (error.code === "ITEM_IMPORT_CONTENT_CHANGED") return t("contentChanged");
      if (error.code === "ITEM_CODE_CONFLICT") return t("codeConflict");
    }
    return t("failed");
  }

  async function handleFile(file: File | undefined): Promise<void> {
    reset();
    if (!file) return;
    if (file.size > ITEM_IMPORT_MAX_FILE_BYTES) {
      setErrorMessage(t("fileTooLarge"));
      return;
    }

    setFileName(file.name);
    try {
      const text = await file.text();
      setContent(text);
      setResult(toStrictPreview(await preview.mutateAsync(text)));
    } catch (error: unknown) {
      setErrorMessage(describeError(error));
    }
  }

  async function handleCommit(): Promise<void> {
    if (!content || !result || result.invalidRows !== 0 || result.validRows === 0) return;
    if (commitKeyRef.current?.hash !== result.contentSha256) {
      commitKeyRef.current = { hash: result.contentSha256, key: crypto.randomUUID() };
    }
    const idempotencyKey = commitKeyRef.current.key;

    try {
      const committed = await commit.mutateAsync({
        content,
        expectedContentSha256: result.contentSha256,
        idempotencyKey,
      });
      if (typeof committed.createdCount !== "number") throw new IncompleteImportResponseError("Item import commit response is incomplete");
      toast.success(t("success", { count: committed.createdCount }));
      reset();
      onClose();
    } catch (error: unknown) {
      setErrorMessage(describeError(error));
    }
  }

  function downloadTemplate(): void {
    const url = URL.createObjectURL(new Blob([buildItemImportTemplate()], { type: "text/csv;charset=utf-8" }));
    const link = document.createElement("a");
    link.href = url;
    link.download = "item-import-template.csv";
    link.click();
    URL.revokeObjectURL(url);
  }

  function describeCell(field: string | null | undefined, code: string | null | undefined): string {
    const fieldLabel = isImportColumn(field) ? t(`fields.${field}`) : "-";
    const codeLabel = isItemImportErrorCode(code) ? t(`errors.${code}`) : "-";
    return `${fieldLabel}: ${codeLabel}`;
  }

  const rows = result?.rows ?? [];
  // Invalid rows first so the problems are visible without scrolling a long valid list.
  const visibleRows = [...rows].sort((a, b) => Number(a.isValid) - Number(b.isValid)).slice(0, MAX_VISIBLE_ROWS);
  const canCommit = Boolean(result && result.invalidRows === 0 && result.validRows > 0);

  return (
    <Modal
      isOpen={isOpen}
      onClose={close}
      title={t("title")}
      description={t("description")}
      size="lg"
      closeDisabled={isBusy}
      closeOnOverlayClick={!isBusy}
      closeOnEscape={!isBusy}
      footer={(
        <div className="flex justify-end gap-3">
          <Button type="button" variant="outline" onClick={close} disabled={isBusy} className="min-h-11">
            {tCommon("actions.cancel")}
          </Button>
          <Button
            type="button"
            variant="primary"
            onClick={() => void handleCommit()}
            isLoading={commit.isPending}
            disabled={!canCommit || isBusy}
            className="min-h-11"
          >
            {t("commitAction", { count: result?.validRows ?? 0 })}
          </Button>
        </div>
      )}
    >
      <div className="space-y-4">
        <p className="text-xs text-erp-text-muted">{t("rules")}</p>

        <div className="flex flex-wrap items-center gap-3">
          <input
            ref={inputRef}
            type="file"
            accept=".csv,text/csv"
            className="sr-only"
            aria-label={t("chooseFile")}
            data-testid="item-import-file"
            disabled={isBusy}
            onChange={(event) => void handleFile(event.currentTarget.files?.[0])}
          />
          <Button type="button" variant="secondary" icon={<IconUpload size={15} />} disabled={isBusy} isLoading={preview.isPending} onClick={() => inputRef.current?.click()} className="min-h-11">
            {t("chooseFile")}
          </Button>
          <Button type="button" variant="outline" icon={<IconDownload size={15} />} onClick={downloadTemplate} className="min-h-11">
            {t("downloadTemplate")}
          </Button>
          {fileName && <span className="font-mono text-xs text-erp-text-muted">{fileName}</span>}
        </div>

        {errorMessage && <Alert variant="danger" onClose={() => setErrorMessage(null)}>{errorMessage}</Alert>}

        {result && (
          <div className="space-y-3">
            <p className="text-sm font-semibold text-erp-navy">
              {t("summary", { total: result.totalRows, valid: result.validRows, invalid: result.invalidRows })}
            </p>
            {result.invalidRows > 0 && <Alert variant="warning">{t("fixBeforeCommit")}</Alert>}
            <div className="max-h-72 overflow-auto border border-erp-border">
              <table className="w-full text-left text-xs">
                <thead className="sticky top-0 bg-erp-surface-subtle">
                  <tr>
                    <th className="px-2 py-1.5">{t("columnRow")}</th>
                    <th className="px-2 py-1.5">{t("columnCode")}</th>
                    <th className="px-2 py-1.5">{t("columnName")}</th>
                    <th className="px-2 py-1.5">{t("columnResult")}</th>
                  </tr>
                </thead>
                <tbody>
                  {visibleRows.map((row) => (
                    <tr key={row.rowNumber} className="border-t border-erp-border align-top">
                      <td className="px-2 py-1.5 font-mono">{row.rowNumber}</td>
                      <td className="px-2 py-1.5 font-mono">{row.code ?? "-"}</td>
                      <td className="px-2 py-1.5">{row.nameTh ?? "-"}</td>
                      <td className="px-2 py-1.5">
                        {row.isValid ? (
                          <span className="text-erp-success">{t("rowValid")}</span>
                        ) : (
                          <ul className="space-y-0.5 text-erp-danger">
                            {(row.errors ?? []).map((error) => (
                              <li key={`${error.field}-${error.code}`}>{describeCell(error.field, error.code)}</li>
                            ))}
                          </ul>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {rows.length > MAX_VISIBLE_ROWS && (
              <p className="text-xs text-erp-text-muted">{t("truncated", { shown: MAX_VISIBLE_ROWS, total: rows.length })}</p>
            )}
          </div>
        )}
      </div>
    </Modal>
  );
}
