"use client";

import { useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Badge, type BadgeVariant } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import type { ItemImportJobResponse, ItemImportJobRowResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";
import { useItemImportJobStatus, useStartItemImportJob } from "../api/item-master-queries";
import {
  isItemImportErrorCode,
  isItemImportJobFailureCode,
  isItemImportJobStatus,
  type ItemImportJobStatus,
  type ItemImportKind,
  type ItemImportMode,
} from "../item-import-constants";

interface ItemImportJobPanelProps {
  content: string;
  kind: ItemImportKind;
  mode: ItemImportMode;
  /** Maps a column/field name to its label; the modal owns the field translations. */
  describeField: (field: string | null | undefined, code: string | null | undefined) => string;
}

const MAX_VISIBLE_ERROR_ROWS = 50;

class IncompleteImportJobError extends Error {}

interface StrictJob {
  id: string;
  status: ItemImportJobStatus;
  totalRows: number;
  processedRows: number;
  totalBatches: number;
  completedBatches: number;
  rejectedBatches: number;
  createdCount: number;
  updatedCount: number;
  unchangedCount: number;
  errorRowCount: number;
  errorsTruncated: boolean;
  errors: ItemImportJobRowResponse[];
  failureCode: string | null;
}

/** The generated type marks every field optional; reject a response that lacks what the progress view needs. */
function toStrictJob(response: ItemImportJobResponse): StrictJob {
  const {
    id, status, totalRows, processedRows, totalBatches, completedBatches, rejectedBatches, createdCount, updatedCount,
    unchangedCount, errorRowCount, errorsTruncated, errors, failureCode,
  } = response;
  if (
    typeof id !== "string" ||
    !isItemImportJobStatus(status) ||
    typeof totalRows !== "number" ||
    typeof processedRows !== "number" ||
    typeof totalBatches !== "number" ||
    typeof completedBatches !== "number" ||
    typeof rejectedBatches !== "number" ||
    typeof createdCount !== "number" ||
    typeof updatedCount !== "number" ||
    typeof unchangedCount !== "number" ||
    typeof errorRowCount !== "number" ||
    typeof errorsTruncated !== "boolean" ||
    !Array.isArray(errors)
  ) {
    throw new IncompleteImportJobError("Item import job response is incomplete");
  }
  return {
    id, status, totalRows, processedRows, totalBatches, completedBatches, rejectedBatches, createdCount, updatedCount,
    unchangedCount, errorRowCount, errorsTruncated, errors, failureCode: failureCode ?? null,
  };
}

const STATUS_VARIANT: Record<ItemImportJobStatus, BadgeVariant> = {
  queued: "neutral",
  running: "info",
  completed: "success",
  completed_with_errors: "warning",
  failed: "danger",
};

/**
 * Starts a large import as a background job and shows its progress. Each batch is saved all-or-nothing by the backend, so a
 * batch with a wrong row is skipped and listed here while the other batches still import. The job keeps running if the drawer is closed.
 */
export function ItemImportJobPanel({ content, kind, mode, describeField }: ItemImportJobPanelProps) {
  const t = useTranslations("itemMaster.import");
  const start = useStartItemImportJob();
  const [jobId, setJobId] = useState<string | undefined>(undefined);
  const [startFailed, setStartFailed] = useState(false);
  // One key per file/option choice, so a retried click replays the same job instead of queueing a second one.
  const keyRef = useRef<string>(crypto.randomUUID());
  const status = useItemImportJobStatus(jobId);

  async function handleStart(): Promise<void> {
    setStartFailed(false);
    try {
      const created = await start.mutateAsync({ content, kind, mode, idempotencyKey: keyRef.current });
      if (typeof created.id !== "string") throw new IncompleteImportJobError("Item import job response is incomplete");
      setJobId(created.id);
    } catch (error: unknown) {
      if (error instanceof ApiError || error instanceof IncompleteImportJobError) setStartFailed(true);
      else throw error;
    }
  }

  let job: StrictJob | null = null;
  let loadFailed = false;
  if (status.data) {
    try {
      job = toStrictJob(status.data);
    } catch (error: unknown) {
      if (!(error instanceof IncompleteImportJobError)) throw error;
      loadFailed = true;
    }
  }
  if (status.isError) loadFailed = true;

  return (
    <div className="space-y-3 border border-erp-border p-3" data-testid="item-import-job-panel">
      <p className="text-xs text-erp-text-muted">{t("job.largeFile")}</p>

      {!jobId && (
        <Button type="button" variant="primary" onClick={() => void handleStart()} isLoading={start.isPending} disabled={start.isPending} className="min-h-11">
          {t("job.start")}
        </Button>
      )}
      {startFailed && <Alert variant="danger">{t("job.startFailed")}</Alert>}
      {loadFailed && <Alert variant="warning">{t("job.loadFailed")}</Alert>}

      {job && (
        <div className="space-y-3">
          <div className="flex flex-wrap items-center gap-2">
            <Badge variant={STATUS_VARIANT[job.status]}>{t(`job.status.${job.status}`)}</Badge>
            <span className="text-sm font-semibold text-erp-navy">
              {t("job.progress", { processed: job.processedRows, total: job.totalRows })}
            </span>
          </div>
          <progress className="h-2 w-full accent-erp-navy" value={job.processedRows} max={job.totalRows} aria-label={t("job.progressLabel")} />
          <p className="text-xs text-erp-text-muted">
            {t("job.batches", { done: job.completedBatches, total: job.totalBatches, rejected: job.rejectedBatches })}
          </p>
          <p className="text-xs text-erp-text-muted">
            {t("summaryOutcome", { created: job.createdCount, updated: job.updatedCount, unchanged: job.unchangedCount })}
          </p>
          {job.status === "running" || job.status === "queued" ? (
            <p className="text-xs text-erp-text-muted">{t("job.closeHint")}</p>
          ) : null}
          {job.status === "failed" && (
            <Alert variant="danger">
              {isItemImportJobFailureCode(job.failureCode) ? t(`job.failureCodes.${job.failureCode}`) : t("job.failureUnknown")}
            </Alert>
          )}
          {job.errorRowCount > 0 && (
            <div className="space-y-2">
              <Alert variant="warning">{t("job.rejectedNotice", { batches: job.rejectedBatches, rows: job.errorRowCount })}</Alert>
              <div className="max-h-60 overflow-auto border border-erp-border">
                <table className="w-full text-left text-xs">
                  <thead className="sticky top-0 bg-erp-surface-subtle">
                    <tr>
                      <th className="px-2 py-1.5">{t("columnRow")}</th>
                      <th className="px-2 py-1.5">{t("columnCode")}</th>
                      <th className="px-2 py-1.5">{t("columnResult")}</th>
                    </tr>
                  </thead>
                  <tbody>
                    {job.errors.slice(0, MAX_VISIBLE_ERROR_ROWS).map((row) => (
                      <tr key={row.rowNumber} className="border-t border-erp-border align-top">
                        <td className="px-2 py-1.5 font-mono">{row.rowNumber}</td>
                        <td className="px-2 py-1.5 font-mono">{row.code ?? "-"}</td>
                        <td className="px-2 py-1.5">
                          <ul className="space-y-0.5 text-erp-danger">
                            {(row.errors ?? []).map((error) => (
                              <li key={`${error.field}-${error.code}`}>
                                {isItemImportErrorCode(error.code) ? describeField(error.field, error.code) : "-"}
                              </li>
                            ))}
                          </ul>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              {(job.errorsTruncated || job.errors.length > MAX_VISIBLE_ERROR_ROWS) && (
                <p className="text-xs text-erp-text-muted">
                  {t("job.errorsTruncated", { shown: Math.min(job.errors.length, MAX_VISIBLE_ERROR_ROWS), total: job.errorRowCount })}
                </p>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
