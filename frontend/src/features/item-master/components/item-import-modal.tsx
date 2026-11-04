"use client";

import { useEffect, useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { Select } from "@/components/ui/Select";
import { Badge } from "@/components/ui/Badge";
import { SegmentedControl } from "@/components/ui/SegmentedControl";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table";
import { cn } from "@/lib/utils/cn";
import { IconDownload, IconFileText } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { ItemImportPreviewResponse, ItemImportRowResponse } from "@/lib/api/api-client";
import { useCancelItemImportJob, useItemImport } from "../api/item-master-queries";
import {
  buildCostImportTemplate,
  buildItemImportTemplate,
  isItemImportErrorCode,
  isItemImportKind,
  isItemImportMode,
  isItemImportOutcome,
  ITEM_IMPORT_JOB_MAX_FILE_BYTES,
  needsBackgroundJob,
  type ItemImportKind,
  type ItemImportMode,
} from "../item-import-constants";
import { ItemImportJobPanel } from "./item-import-job-panel";
import { ItemImportColumnGuide } from "./item-import-column-guide";
import { ItemImportDropzone } from "./item-import-dropzone";
import { ItemImportErrorExportButton, isImportField } from "./item-import-error-export-button";
import { ItemImportStepper } from "./item-import-stepper";

interface ItemImportModalProps {
  isOpen: boolean;
  onClose: () => void;
  /** Offered only with items.create. */
  canImportItems: boolean;
  /** Offered only with cost-records.create. */
  canImportCosts: boolean;
  /** Offered only with items.update (upsert changes existing Items). */
  canUpsert?: boolean;
  /** Default import kind ("items" | "costs"). */
  defaultKind?: ItemImportKind;
}

type RowFilter = "all" | "invalid" | "valid";

function SummaryCard({ label, value, tone }: { label: string; value: number; tone: "neutral" | "success" | "danger" }) {
  return (
    <div
      className={cn(
        "border px-3 py-2",
        tone === "neutral" && "border-erp-border bg-erp-surface-subtle",
        tone === "success" && "border-erp-success-border bg-erp-success-bg text-erp-success-text",
        tone === "danger" && "border-erp-danger-border bg-erp-danger-bg text-erp-danger",
      )}
    >
      <p className="text-xs">{label}</p>
      <p className="text-2xl font-semibold">{value}</p>
    </div>
  );
}

interface StrictPreview {
  contentSha256: string;
  totalRows: number;
  validRows: number;
  invalidRows: number;
  rows: ItemImportRowResponse[];
  createCount: number;
  updateCount: number;
  unchangedCount: number;
}

class IncompleteImportResponseError extends Error {}

/** The generated response type marks every field optional; reject a response that lacks the fields the UI depends on. */
function toStrictPreview(response: ItemImportPreviewResponse): StrictPreview {
  const { contentSha256, totalRows, validRows, invalidRows, rows, createCount, updateCount, unchangedCount } = response;
  if (
    typeof contentSha256 !== "string" ||
    typeof totalRows !== "number" ||
    typeof validRows !== "number" ||
    typeof invalidRows !== "number" ||
    !Array.isArray(rows)
  ) {
    throw new IncompleteImportResponseError("Item import preview response is incomplete");
  }
  // The outcome counters are absent from older responses; they are display-only, so absence reads as zero rather than failing the preview.
  return {
    contentSha256,
    totalRows,
    validRows,
    invalidRows,
    rows,
    createCount: typeof createCount === "number" ? createCount : 0,
    updateCount: typeof updateCount === "number" ? updateCount : 0,
    unchangedCount: typeof unchangedCount === "number" ? unchangedCount : 0,
  };
}

/**
 * Item CSV import: pick a file, preview per-row validation (nothing is written), then commit all rows at once.
 * Commit is only offered when every row is valid, mirroring the backend's all-or-nothing rule.
 */
export function ItemImportModal({ isOpen, onClose, canImportItems, canImportCosts, canUpsert = false, defaultKind }: ItemImportModalProps) {
  const t = useTranslations("itemMaster.import");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { preview, commit } = useItemImport();
  const cancelJob = useCancelItemImportJob();
  const inputRef = useRef<HTMLInputElement>(null);
  const [content, setContent] = useState<string | null>(null);
  const [fileName, setFileName] = useState<string | null>(null);
  const [result, setResult] = useState<StrictPreview | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const resolvedDefaultKind: ItemImportKind = defaultKind ?? (canImportItems ? "items" : "costs");
  const [kind, setKind] = useState<ItemImportKind>(resolvedDefaultKind);
  const [mode, setMode] = useState<ItemImportMode>("create");
  const [rowFilter, setRowFilter] = useState<RowFilter>("all");
  // A file too big for the immediate preview is sent as a background job instead (no preview).
  const [isLargeFile, setIsLargeFile] = useState(false);
  // One idempotency key per previewed content, so a retried commit of the same file replays instead of duplicating.
  const commitKeyRef = useRef<{ hash: string; key: string } | null>(null);

  const prevDefaultKindRef = useRef(defaultKind);
  useEffect(() => {
    if (defaultKind !== undefined && defaultKind !== prevDefaultKindRef.current) {
      prevDefaultKindRef.current = defaultKind;
      setKind(defaultKind);
    }
  }, [defaultKind]);

  const kindOptions = [
    ...(canImportItems ? [{ value: "items", label: t("kindItems") }] : []),
    ...(canImportCosts ? [{ value: "costs", label: t("kindCosts") }] : []),
  ];
  const modeOptions = [
    { value: "create", label: t("modeCreate") },
    ...(canUpsert && kind === "items" ? [{ value: "upsert", label: t("modeUpsert") }] : []),
  ];

  const isBusy = preview.isPending || commit.isPending;

  function reset(): void {
    setContent(null);
    setFileName(null);
    setResult(null);
    setErrorMessage(null);
    setIsLargeFile(false);
    setRowFilter("all");
    commitKeyRef.current = null;
    if (inputRef.current) inputRef.current.value = "";
  }

  function close(): void {
    if (isBusy) return;
    reset();
    setKind(resolvedDefaultKind);
    setMode("create");
    onClose();
  }

  function changeKind(value: string): void {
    if (!isItemImportKind(value)) return;
    reset();
    setKind(value);
    // Cost import has no upsert; it always creates Draft cost records.
    setMode("create");
  }

  function changeMode(value: string): void {
    if (!isItemImportMode(value)) return;
    reset();
    setMode(value);
  }

  function describeError(error: unknown): string {
    if (error instanceof ApiError) {
      if (error.code === "ITEM_IMPORT_OPTION_INVALID") return t("optionInvalid");
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
    if (file.size > ITEM_IMPORT_JOB_MAX_FILE_BYTES) {
      setErrorMessage(t("fileTooLarge"));
      return;
    }

    setFileName(file.name);
    try {
      const text = await file.text();
      setContent(text);
      if (needsBackgroundJob(text, file.size)) {
        setIsLargeFile(true);
        return;
      }
      setResult(toStrictPreview(await preview.mutateAsync({ content: text, kind, mode })));
    } catch (error: unknown) {
      setErrorMessage(describeError(error));
    }
  }

  async function handleCommit(): Promise<void> {
    if (!content || !result || result.invalidRows !== 0 || result.validRows === 0) return;
    const keyScope = `${result.contentSha256}:${kind}:${mode}`;
    if (commitKeyRef.current?.hash !== keyScope) {
      commitKeyRef.current = { hash: keyScope, key: crypto.randomUUID() };
    }
    const idempotencyKey = commitKeyRef.current.key;

    try {
      const committed = await commit.mutateAsync({
        content,
        expectedContentSha256: result.contentSha256,
        idempotencyKey,
        kind,
        mode,
      });
      if (typeof committed.createdCount !== "number") throw new IncompleteImportResponseError("Item import commit response is incomplete");
      if (kind === "costs") {
        toast.success(t("successCosts", { count: committed.createdCount }));
      } else if (mode === "upsert") {
        toast.success(t("successUpsert", {
          created: committed.createdCount,
          updated: committed.updatedCount ?? 0,
          unchanged: committed.unchangedCount ?? 0,
        }));
      } else {
        toast.success(t("success", { count: committed.createdCount }));
      }
      reset();
      onClose();
    } catch (error: unknown) {
      setErrorMessage(describeError(error));
    }
  }

  function downloadTemplate(): void {
    const template = kind === "costs" ? buildCostImportTemplate() : buildItemImportTemplate();
    const url = URL.createObjectURL(new Blob([template], { type: "text/csv;charset=utf-8" }));
    const link = document.createElement("a");
    link.href = url;
    link.download = kind === "costs" ? "cost-import-template.csv" : "item-import-template.csv";
    link.click();
    URL.revokeObjectURL(url);
  }

  function describeCell(field: string | null | undefined, code: string | null | undefined): string {
    const fieldLabel = isImportField(field) ? t(`fields.${field}`) : "-";
    const codeLabel = isItemImportErrorCode(code) ? t(`errors.${code}`) : "-";
    return `${fieldLabel}: ${codeLabel}`;
  }

  const rows = result?.rows ?? [];
  // Invalid rows first so the problems are visible without scrolling a long valid list.
  const sortedRows = [...rows].sort((a, b) => Number(a.isValid) - Number(b.isValid));
  const visibleRows = sortedRows.filter((row) => rowFilter === "all" || (rowFilter === "invalid" ? !row.isValid : row.isValid));
  const canCommit = Boolean(result && result.invalidRows === 0 && result.validRows > 0);
  const currentStep = commit.isPending ? 3 : result || isLargeFile ? 2 : 1;
  const filterOptions = result
    ? [
        { value: "all" as const, label: t("filterAll", { count: result.totalRows }) },
        { value: "invalid" as const, label: t("filterInvalid", { count: result.invalidRows }) },
        { value: "valid" as const, label: t("filterValid", { count: result.validRows }) },
      ]
    : [];

  const footerActions = (
    <div className="flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3">
      <Button
        type="button"
        variant="outline"
        onClick={close}
        disabled={isBusy}
        className="w-full sm:w-auto min-h-11"
      >
        {tCommon("actions.cancel")}
      </Button>
      <Button
        type="button"
        variant="primary"
        onClick={() => void handleCommit()}
        isLoading={commit.isPending}
        disabled={!canCommit || isBusy}
        className="w-full sm:w-auto min-h-11"
      >
        {kind === "costs"
          ? t("commitActionCosts", { count: result?.validRows ?? 0 })
          : mode === "upsert"
            ? t("commitActionUpsert", { count: result?.validRows ?? 0 })
            : t("commitAction", { count: result?.validRows ?? 0 })}
      </Button>
    </div>
  );

  return (
    <Drawer
      isOpen={isOpen}
      onClose={close}
      title={t("title")}
      description={t("description")}
      size="xl"
      closeDisabled={isBusy}
      closeOnOverlayClick={!isBusy}
      footer={footerActions}
    >
      <div className="space-y-4">
        <ItemImportStepper ariaLabel={t("stepsLabel")} labels={[t("stepChoose"), t("stepReview"), t("stepConfirm")]} current={currentStep} />

        {(kindOptions.length > 1 || modeOptions.length > 1) && (
          <div className="grid gap-3 sm:grid-cols-2">
            {kindOptions.length > 1 && (
              <Select id="item-import-kind" label={t("kindLabel")} value={kind} onChange={(event) => changeKind(event.target.value)} options={kindOptions} disabled={isBusy} />
            )}
            {modeOptions.length > 1 && (
              <Select id="item-import-mode" label={t("modeLabel")} value={mode} onChange={(event) => changeMode(event.target.value)} options={modeOptions} disabled={isBusy} />
            )}
          </div>
        )}
        {kind === "costs" && <p className="text-xs text-erp-text-muted">{t("costHint")}</p>}
        {mode === "upsert" && <p className="text-xs text-erp-text-muted">{t("upsertHint")}</p>}

        <ItemImportDropzone
          inputRef={inputRef}
          disabled={isBusy}
          isLoading={preview.isPending}
          onFile={(file) => void handleFile(file)}
          onReject={() => { reset(); setErrorMessage(t("notCsv")); }}
        />
        <div className="flex flex-wrap items-center gap-3">
          <Button type="button" variant="outline" size="sm" icon={<IconDownload size={14} />} onClick={downloadTemplate}>
            {t("downloadTemplate")}
          </Button>
          {fileName && (
            <span className="inline-flex items-center gap-1.5 font-mono text-xs text-erp-text-muted">
              <IconFileText size={14} />
              {fileName}
            </span>
          )}
        </div>
        <ItemImportColumnGuide kind={kind} />

        {errorMessage && <Alert variant="danger" onClose={() => setErrorMessage(null)}>{errorMessage}</Alert>}

        {isLargeFile && content !== null && (
          <ItemImportJobPanel
            key={`${fileName}:${kind}:${mode}`}
            content={content}
            kind={kind}
            mode={mode}
            describeField={describeCell}
            onCancelJob={async (jobId) => {
              await cancelJob.mutateAsync(jobId);
              toast.success(t("job.cancelSuccess"));
            }}
          />
        )}

        {result && (
          <div className="space-y-3">
            <p className="sr-only" aria-live="polite">
              {t("summary", { total: result.totalRows, valid: result.validRows, invalid: result.invalidRows })}
            </p>
            <div className="grid grid-cols-3 gap-2">
              <SummaryCard label={t("cardTotal")} value={result.totalRows} tone="neutral" />
              <SummaryCard label={t("cardValid")} value={result.validRows} tone="success" />
              <SummaryCard label={t("cardInvalid")} value={result.invalidRows} tone={result.invalidRows > 0 ? "danger" : "neutral"} />
            </div>
            {(mode === "upsert" || kind === "costs") && result.invalidRows === 0 && (
              <p className="text-xs text-erp-text-muted">
                {t("summaryOutcome", { created: result.createCount, updated: result.updateCount, unchanged: result.unchangedCount })}
              </p>
            )}
            {mode === "upsert" && kind === "items" && result.invalidRows === 0 && result.updateCount > 0 && (
              <Alert variant="info">{t("upsertImpact", { count: result.updateCount })}</Alert>
            )}
            {result.invalidRows > 0 && (
              <Alert variant="warning">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <span>{t("fixBeforeCommit")}</span>
                  <ItemImportErrorExportButton sources={rows.filter((row) => !row.isValid)} />
                </div>
              </Alert>
            )}
            <SegmentedControl aria-label={t("filterLabel")} value={rowFilter} onChange={setRowFilter} options={filterOptions} />
            <Table wrapperClassName="max-h-72 overflow-auto border border-erp-border">
              <TableHeader className="sticky top-0 bg-erp-surface-subtle">
                <TableRow>
                  <TableHead>{t("columnRow")}</TableHead>
                  <TableHead>{t("columnCode")}</TableHead>
                  <TableHead>{t("columnName")}</TableHead>
                  <TableHead>{t("columnResult")}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {visibleRows.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={4} className="text-center text-xs text-erp-text-muted">{t("filterEmpty")}</TableCell>
                  </TableRow>
                ) : visibleRows.map((row) => (
                  <TableRow key={row.rowNumber} className="align-top">
                    <TableCell className="font-mono text-xs">{row.rowNumber}</TableCell>
                    <TableCell className="font-mono text-xs">{row.code ?? "-"}</TableCell>
                    <TableCell className="text-xs">{row.nameTh ?? "-"}</TableCell>
                    <TableCell className="text-xs">
                      {row.isValid ? (
                        <Badge variant="success" size="sm">
                          {isItemImportOutcome(row.outcome) ? t(`outcome.${row.outcome}`) : t("rowValid")}
                        </Badge>
                      ) : (
                        <ul className="space-y-1 text-erp-danger">
                          {(row.errors ?? []).map((error) => (
                            <li key={`${error.field}-${error.code}`}>
                              <span>{describeCell(error.field, error.code)}</span>
                              {isItemImportErrorCode(error.code) && (
                                <span className="block text-erp-text-muted">{t(`errorHints.${error.code}`)}</span>
                              )}
                            </li>
                          ))}
                        </ul>
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </div>
    </Drawer>
  );
}

export { ItemImportModal as ItemImportDrawer };
