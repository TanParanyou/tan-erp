"use client";

import React, { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import type { AccountMappingResponse, JournalExportResultResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useAccountMappings, useOutboxList, useReconciliation, useSyncMutations } from "../api/finance-queries";
import {
  OUTBOX_KINDS,
  OUTBOX_STATUSES,
  financeErrorCode,
  isOutboxKind,
  isOutboxStatus,
  isReconciliationIssue,
  outboxStatusVariant,
} from "../finance-status";

const PAGE_SIZE = 25;

export function FinanceSyncPage() {
  const t = useTranslations("finance.sync");
  const tErrors = useTranslations("finance.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canRun = can(selectedMembership, PERMISSIONS.FINANCE_SYNC_RUN);

  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const outbox = useOutboxList({ status: status || undefined, page, pageSize: PAGE_SIZE });
  const reconciliation = useReconciliation();
  const accountMappings = useAccountMappings();
  const mutations = useSyncMutations();

  const [message, setMessage] = useState<string | null>(null);
  const [lastExportResult, setLastExportResult] = useState<JournalExportResultResponse | null>(null);

  // Form state for COA mapping upsert
  const [mappingKind, setMappingKind] = useState<string>("billing.issued");
  const [debitCode, setDebitCode] = useState<string>("");
  const [debitName, setDebitName] = useState<string>("");
  const [creditCode, setCreditCode] = useState<string>("");
  const [creditName, setCreditName] = useState<string>("");
  const [mappingError, setMappingError] = useState<string | null>(null);

  const busy =
    mutations.dispatch.isPending ||
    mutations.requeue.isPending ||
    mutations.exportJournal.isPending ||
    mutations.upsertMapping.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? financeErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function dispatch(): Promise<void> {
    setMessage(null);
    try {
      const result = await mutations.dispatch.mutateAsync();
      toast.success(
        t("dispatched", {
          processed: result.processed ?? 0,
          sent: result.sent ?? 0,
          failed: result.failed ?? 0,
          dead: result.dead ?? 0,
        })
      );
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function exportJournal(): Promise<void> {
    setMessage(null);
    try {
      const result = await mutations.exportJournal.mutateAsync();
      setLastExportResult(result);
      toast.success(
        t("journalExported", {
          batchRef: result.batchRef ?? "-",
          exported: result.exportedCount ?? 0,
          dead: result.deadCount ?? 0,
        })
      );
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function requeue(id: string): Promise<void> {
    setMessage(null);
    try {
      await mutations.requeue.mutateAsync({ id });
      toast.success(t("requeued"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function handleUpsertMapping(e: React.FormEvent): Promise<void> {
    e.preventDefault();
    setMappingError(null);
    if (!mappingKind || !debitCode.trim() || !debitName.trim() || !creditCode.trim() || !creditName.trim()) {
      setMappingError(t("coa.invalidForm"));
      return;
    }
    try {
      await mutations.upsertMapping.mutateAsync({
        kind: mappingKind,
        debitAccountCode: debitCode.trim(),
        debitAccountName: debitName.trim(),
        creditAccountCode: creditCode.trim(),
        creditAccountName: creditName.trim(),
      });
      toast.success(t("coa.mappingSaved"));
      setDebitCode("");
      setDebitName("");
      setCreditCode("");
      setCreditName("");
    } catch (err: unknown) {
      setMappingError(describe(err));
    }
  }

  function handleEditMapping(row: AccountMappingResponse) {
    if (row.kind) setMappingKind(row.kind);
    setDebitCode(row.debitAccountCode ?? "");
    setDebitName(row.debitAccountName ?? "");
    setCreditCode(row.creditAccountCode ?? "");
    setCreditName(row.creditAccountName ?? "");
  }

  function handleDownloadCsv() {
    if (!lastExportResult?.csvContent) return;
    const blob = new Blob([lastExportResult.csvContent], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `journal-${lastExportResult.batchRef ?? "export"}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  }

  const recon = reconciliation.data;
  const items = outbox.data?.items ?? [];
  const mappings = accountMappings.data?.items ?? [];
  const totalPages = outbox.data?.pagination?.totalPages ?? 0;

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={
          canRun ? (
            <div className="flex flex-wrap items-center gap-2">
              <Button
                type="button"
                variant="outline"
                className="min-h-11"
                isLoading={mutations.exportJournal.isPending}
                disabled={busy}
                onClick={() => void exportJournal()}
              >
                {t("exportJournal")}
              </Button>
              <Button
                type="button"
                variant="primary"
                className="min-h-11"
                isLoading={mutations.dispatch.isPending}
                disabled={busy}
                onClick={() => void dispatch()}
              >
                {t("dispatch")}
              </Button>
            </div>
          ) : undefined
        }
      />
      <Alert variant="warning">{t("notConfigured")}</Alert>
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      {/* Latest Journal Export Result Card */}
      {lastExportResult && (
        <div className="erp-card space-y-3 p-5" role="region" aria-label={t("lastExportTitle")}>
          <div className="flex flex-wrap items-center justify-between gap-3">
            <div>
              <h3 className="text-sm font-bold text-erp-navy">{t("lastExportTitle")}</h3>
              <p className="text-xs text-erp-text-muted">
                {t("lastExportSubtitle", {
                  batchRef: lastExportResult.batchRef ?? "-",
                  exported: lastExportResult.exportedCount ?? 0,
                  dead: lastExportResult.deadCount ?? 0,
                })}
              </p>
            </div>
            {lastExportResult.csvContent && (
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="min-h-11"
                onClick={handleDownloadCsv}
              >
                {t("downloadCsv", { batchRef: lastExportResult.batchRef ?? "-" })}
              </Button>
            )}
          </div>
          {lastExportResult.lines && lastExportResult.lines.length > 0 && (
            <div className="overflow-x-auto">
              <table className="w-full text-left text-xs">
                <thead>
                  <tr className="border-b border-erp-border">
                    <th className="py-2 pr-2">{t("document")}</th>
                    <th className="py-2 pr-2">{t("coa.kind")}</th>
                    <th className="py-2 pr-2">{t("coa.debitAccount")}</th>
                    <th className="py-2 pr-2">{t("coa.creditAccount")}</th>
                    <th className="py-2 pr-2 text-right">{t("amount")}</th>
                  </tr>
                </thead>
                <tbody>
                  {lastExportResult.lines.map((line, idx) => (
                    <tr key={`${line.messageId ?? idx}`} className="border-b border-erp-border/60">
                      <td className="py-2 pr-2 font-mono">{line.resourceNumber ?? "-"}</td>
                      <td className="py-2 pr-2">
                        {line.kind ? (isOutboxKind(line.kind) ? t(`kinds.${line.kind}`) : line.kind) : "-"}
                      </td>
                      <td className="py-2 pr-2 font-mono">
                        {line.debitAccountCode} {line.debitAccountName ? `· ${line.debitAccountName}` : ""}
                      </td>
                      <td className="py-2 pr-2 font-mono">
                        {line.creditAccountCode} {line.creditAccountName ? `· ${line.creditAccountName}` : ""}
                      </td>
                      <td className="py-2 pr-2 text-right font-mono font-semibold">
                        {formatCurrency(line.amount, line.currency ?? "THB", locale)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* Chart of Accounts Mapping Card */}
      <div className="erp-card space-y-4 p-5" role="region" aria-label={t("coa.title")}>
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("coa.title")}</h3>
          <p className="text-xs text-erp-text-muted">{t("coa.subtitle")}</p>
        </div>

        {accountMappings.isLoading ? (
          <MonoSpinner size="sm" />
        ) : mappings.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("coa.emptyMappings")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("coa.kind")}</th>
                  <th className="py-2 pr-2">{t("coa.debitAccount")}</th>
                  <th className="py-2 pr-2">{t("coa.creditAccount")}</th>
                  <th className="py-2 pr-2">{t("coa.updatedAt")}</th>
                  {canRun && <th className="py-2 text-right">{t("actions")}</th>}
                </tr>
              </thead>
              <tbody>
                {mappings.map((row) => (
                  <tr key={row.id ?? row.kind} className="border-b border-erp-border/60 align-middle">
                    <td className="py-2 pr-2 font-medium">
                      {isOutboxKind(row.kind) ? t(`kinds.${row.kind}`) : row.kind ?? "-"}
                    </td>
                    <td className="py-2 pr-2 font-mono">
                      <span className="font-semibold text-erp-navy">{row.debitAccountCode}</span>
                      {row.debitAccountName ? ` · ${row.debitAccountName}` : ""}
                    </td>
                    <td className="py-2 pr-2 font-mono">
                      <span className="font-semibold text-erp-navy">{row.creditAccountCode}</span>
                      {row.creditAccountName ? ` · ${row.creditAccountName}` : ""}
                    </td>
                    <td className="py-2 pr-2 text-erp-text-muted">
                      {row.updatedAtUtc ? formatDateTime(row.updatedAtUtc, locale) : "-"}
                    </td>
                    {canRun && (
                      <td className="py-2 text-right">
                        <Button
                          type="button"
                          size="sm"
                          variant="ghost"
                          className="min-h-11"
                          onClick={() => handleEditMapping(row)}
                        >
                          {tCommon("actions.edit")}
                        </Button>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Upsert Mapping Form */}
        {canRun && (
          <form
            onSubmit={(e) => void handleUpsertMapping(e)}
            className="border border-erp-border bg-erp-surface-subtle p-4 space-y-3"
          >
            <h5 className="text-xs font-bold text-erp-navy uppercase tracking-wider">
              {t("coa.saveMapping")}
            </h5>
            {mappingError && (
              <Alert variant="danger" onClose={() => setMappingError(null)}>
                {mappingError}
              </Alert>
            )}
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-5">
              <Select
                id="coa-kind"
                label={t("coa.kind")}
                required
                value={mappingKind}
                placeholder={t("coa.selectKindPlaceholder")}
                disabled={busy}
                className="h-11 min-h-[44px]"
                options={OUTBOX_KINDS.map((kind) => ({
                  value: kind,
                  label: t(`kinds.${kind}`),
                }))}
                onChange={(e) => setMappingKind(e.target.value)}
              />
              <Input
                id="coa-debit-code"
                label={t("coa.debitAccountCode")}
                required
                value={debitCode}
                placeholder={t("coa.debitAccountCodePlaceholder")}
                disabled={busy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setDebitCode(e.target.value)}
              />
              <Input
                id="coa-debit-name"
                label={t("coa.debitAccountName")}
                required
                value={debitName}
                placeholder={t("coa.debitAccountNamePlaceholder")}
                disabled={busy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setDebitName(e.target.value)}
              />
              <Input
                id="coa-credit-code"
                label={t("coa.creditAccountCode")}
                required
                value={creditCode}
                placeholder={t("coa.creditAccountCodePlaceholder")}
                disabled={busy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setCreditCode(e.target.value)}
              />
              <Input
                id="coa-credit-name"
                label={t("coa.creditAccountName")}
                required
                value={creditName}
                placeholder={t("coa.creditAccountNamePlaceholder")}
                disabled={busy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setCreditName(e.target.value)}
              />
            </div>
            <div className="flex justify-end pt-2">
              <Button
                type="submit"
                variant="primary"
                className="min-h-11"
                isLoading={mutations.upsertMapping.isPending}
                disabled={busy}
              >
                {t("coa.saveMapping")}
              </Button>
            </div>
          </form>
        )}
      </div>

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("reconciliationTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("reconciliationTitle")}</h3>
        {reconciliation.isLoading ? (
          <MonoSpinner size="sm" />
        ) : reconciliation.isError || !recon ? (
          <p className="text-xs text-erp-text-muted">{t("loadError")}</p>
        ) : (
          <>
            <dl className="grid grid-cols-2 gap-3 text-xs md:grid-cols-4">
              <div>
                <dt className="text-erp-text-muted">{t("erpBilled")}</dt>
                <dd className="font-mono font-semibold">{formatCurrency(recon.erpBilled, "THB", locale)}</dd>
              </div>
              <div>
                <dt className="text-erp-text-muted">{t("confirmedBilled")}</dt>
                <dd className="font-mono font-semibold">{formatCurrency(recon.confirmedBilled, "THB", locale)}</dd>
              </div>
              <div>
                <dt className="text-erp-text-muted">{t("erpPaid")}</dt>
                <dd className="font-mono font-semibold">{formatCurrency(recon.erpPaid, "THB", locale)}</dd>
              </div>
              <div>
                <dt className="text-erp-text-muted">{t("confirmedPaid")}</dt>
                <dd className="font-mono font-semibold">{formatCurrency(recon.confirmedPaid, "THB", locale)}</dd>
              </div>
              <div>
                <dt className="text-erp-text-muted">{t("statuses.pending")}</dt>
                <dd className="font-mono font-semibold">{recon.pendingCount ?? 0}</dd>
              </div>
              <div>
                <dt className="text-erp-text-muted">{t("statuses.failed")}</dt>
                <dd className="font-mono font-semibold">{recon.failedCount ?? 0}</dd>
              </div>
              <div>
                <dt className="text-erp-text-muted">{t("statuses.dead")}</dt>
                <dd className="font-mono font-semibold">{recon.deadCount ?? 0}</dd>
              </div>
            </dl>
            {(recon.rows ?? []).length === 0 ? (
              <p className="text-xs text-erp-success">{t("reconciled")}</p>
            ) : (
              <ul className="space-y-1 text-xs">
                {(recon.rows ?? []).map((row, index) => (
                  <li
                    key={`${row.messageId ?? row.resourceNumber}-${index}`}
                    className="border-l-4 border-erp-danger bg-erp-surface-subtle px-3 py-1"
                  >
                    <span className="font-semibold">{isReconciliationIssue(row.issue) ? t(`issues.${row.issue}`) : "-"}</span>
                    {" · "}
                    {isOutboxKind(row.kind) ? t(`kinds.${row.kind}`) : "-"} <span className="font-mono">{row.resourceNumber}</span>
                    {" · "}
                    {formatCurrency(row.erpAmount, "THB", locale)}
                    {row.externalAmount !== null && row.externalAmount !== undefined && (
                      <> → {formatCurrency(row.externalAmount, "THB", locale)}</>
                    )}
                  </li>
                ))}
              </ul>
            )}
          </>
        )}
      </div>

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("outboxTitle")}>
        <div className="flex flex-wrap items-end justify-between gap-3">
          <h3 className="text-sm font-bold text-erp-navy">{t("outboxTitle")}</h3>
          <Select
            id="outbox-status-filter"
            label={t("statusFilter")}
            value={status}
            placeholder={t("allStatuses")}
            options={OUTBOX_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
            onChange={(event) => {
              setStatus(event.target.value);
              setPage(1);
            }}
          />
        </div>
        {outbox.isLoading ? (
          <MonoSpinner size="sm" />
        ) : items.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("outboxEmpty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("kind")}</th>
                  <th className="py-2 pr-2">{t("document")}</th>
                  <th className="py-2 pr-2 text-right">{t("amount")}</th>
                  <th className="py-2 pr-2">{t("status")}</th>
                  <th className="py-2 pr-2 text-right">{t("attempts")}</th>
                  <th className="py-2 pr-2">{t("lastError")}</th>
                  <th className="py-2 pr-2">{t("externalRef")}</th>
                  <th className="py-2 text-right">{t("actions")}</th>
                </tr>
              </thead>
              <tbody>
                {items.map((message) => (
                  <tr key={message.id} className="border-b border-erp-border/60 align-top">
                    <td className="py-2 pr-2">{isOutboxKind(message.kind) ? t(`kinds.${message.kind}`) : "-"}</td>
                    <td className="py-2 pr-2 font-mono">{message.resourceNumber}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatCurrency(message.amount, "THB", locale)}</td>
                    <td className="py-2 pr-2">
                      <StatusBadge
                        label={isOutboxStatus(message.status) ? t(`statuses.${message.status}`) : "-"}
                        variant={outboxStatusVariant(message.status)}
                      />
                      {message.status === "failed" && message.nextAttemptAtUtc && (
                        <div className="text-erp-text-muted">
                          {t("nextAttempt", { date: formatDateTime(message.nextAttemptAtUtc, locale) })}
                        </div>
                      )}
                    </td>
                    <td className="py-2 pr-2 text-right font-mono">{message.attempts ?? 0}</td>
                    <td className="py-2 pr-2 font-mono">{message.lastError ?? "-"}</td>
                    <td className="py-2 pr-2 font-mono">{message.externalRef ?? "-"}</td>
                    <td className="py-2 text-right">
                      {canRun && (message.status === "failed" || message.status === "dead") && (
                        <Button
                          type="button"
                          size="sm"
                          variant="outline"
                          className="min-h-11"
                          disabled={busy}
                          onClick={() => void requeue(message.id ?? "")}
                        >
                          {t("requeue")}
                        </Button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        {totalPages > 1 && (
          <div className="flex items-center justify-end gap-3 text-xs">
            <Button
              type="button"
              size="sm"
              variant="outline"
              className="min-h-11"
              disabled={page <= 1}
              onClick={() => setPage((current) => current - 1)}
            >
              {t("previous")}
            </Button>
            <span className="font-mono">
              {page} / {totalPages}
            </span>
            <Button
              type="button"
              size="sm"
              variant="outline"
              className="min-h-11"
              disabled={page >= totalPages}
              onClick={() => setPage((current) => current + 1)}
            >
              {t("next")}
            </Button>
          </div>
        )}
      </div>
    </section>
  );
}
