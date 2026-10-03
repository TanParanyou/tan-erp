"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useOutboxList, useReconciliation, useSyncMutations } from "../api/finance-queries";
import { OUTBOX_STATUSES, financeErrorCode, isOutboxKind, isOutboxStatus, isReconciliationIssue, outboxStatusVariant } from "../finance-status";

const PAGE_SIZE = 25;

export function FinanceSyncPage() {
  const t = useTranslations("finance.sync");
  const tErrors = useTranslations("finance.errors");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canRun = can(selectedMembership, PERMISSIONS.FINANCE_SYNC_RUN);
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const outbox = useOutboxList({ status: status || undefined, page, pageSize: PAGE_SIZE });
  const reconciliation = useReconciliation();
  const mutations = useSyncMutations();
  const [message, setMessage] = useState<string | null>(null);
  const busy = mutations.dispatch.isPending || mutations.requeue.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? financeErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function dispatch(): Promise<void> {
    setMessage(null);
    try {
      const result = await mutations.dispatch.mutateAsync();
      toast.success(t("dispatched", { processed: result.processed ?? 0, sent: result.sent ?? 0, failed: result.failed ?? 0, dead: result.dead ?? 0 }));
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

  const recon = reconciliation.data;
  const items = outbox.data?.items ?? [];
  const totalPages = outbox.data?.pagination?.totalPages ?? 0;

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canRun ? <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.dispatch.isPending} disabled={busy} onClick={() => void dispatch()}>{t("dispatch")}</Button> : undefined}
      />
      <Alert variant="warning">{t("notConfigured")}</Alert>
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("reconciliationTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("reconciliationTitle")}</h3>
        {reconciliation.isLoading ? <MonoSpinner size="sm" /> : reconciliation.isError || !recon ? <p className="text-xs text-erp-text-muted">{t("loadError")}</p> : (
          <>
            <dl className="grid grid-cols-2 gap-3 text-xs md:grid-cols-4">
              <div><dt className="text-erp-text-muted">{t("erpBilled")}</dt><dd className="font-mono font-semibold">{formatCurrency(recon.erpBilled, "THB", locale)}</dd></div>
              <div><dt className="text-erp-text-muted">{t("confirmedBilled")}</dt><dd className="font-mono font-semibold">{formatCurrency(recon.confirmedBilled, "THB", locale)}</dd></div>
              <div><dt className="text-erp-text-muted">{t("erpPaid")}</dt><dd className="font-mono font-semibold">{formatCurrency(recon.erpPaid, "THB", locale)}</dd></div>
              <div><dt className="text-erp-text-muted">{t("confirmedPaid")}</dt><dd className="font-mono font-semibold">{formatCurrency(recon.confirmedPaid, "THB", locale)}</dd></div>
              <div><dt className="text-erp-text-muted">{t("statuses.pending")}</dt><dd className="font-mono font-semibold">{recon.pendingCount ?? 0}</dd></div>
              <div><dt className="text-erp-text-muted">{t("statuses.failed")}</dt><dd className="font-mono font-semibold">{recon.failedCount ?? 0}</dd></div>
              <div><dt className="text-erp-text-muted">{t("statuses.dead")}</dt><dd className="font-mono font-semibold">{recon.deadCount ?? 0}</dd></div>
            </dl>
            {(recon.rows ?? []).length === 0 ? <p className="text-xs text-erp-success">{t("reconciled")}</p> : (
              <ul className="space-y-1 text-xs">
                {(recon.rows ?? []).map((row, index) => (
                  <li key={`${row.messageId ?? row.resourceNumber}-${index}`} className="border-l-4 border-erp-danger bg-erp-surface-subtle px-3 py-1">
                    <span className="font-semibold">{isReconciliationIssue(row.issue) ? t(`issues.${row.issue}`) : "-"}</span>
                    {" · "}{isOutboxKind(row.kind) ? t(`kinds.${row.kind}`) : "-"} <span className="font-mono">{row.resourceNumber}</span>
                    {" · "}{formatCurrency(row.erpAmount, "THB", locale)}
                    {row.externalAmount !== null && row.externalAmount !== undefined && <> → {formatCurrency(row.externalAmount, "THB", locale)}</>}
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
          <Select label={t("statusFilter")} value={status} placeholder={t("allStatuses")} options={OUTBOX_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))} onChange={(event) => { setStatus(event.target.value); setPage(1); }} />
        </div>
        {outbox.isLoading ? <MonoSpinner size="sm" /> : items.length === 0 ? <p className="text-xs text-erp-text-muted">{t("outboxEmpty")}</p> : (
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
                      <StatusBadge label={isOutboxStatus(message.status) ? t(`statuses.${message.status}`) : "-"} variant={outboxStatusVariant(message.status)} />
                      {message.status === "failed" && message.nextAttemptAtUtc && <div className="text-erp-text-muted">{t("nextAttempt", { date: formatDateTime(message.nextAttemptAtUtc, locale) })}</div>}
                    </td>
                    <td className="py-2 pr-2 text-right font-mono">{message.attempts ?? 0}</td>
                    <td className="py-2 pr-2 font-mono">{message.lastError ?? "-"}</td>
                    <td className="py-2 pr-2 font-mono">{message.externalRef ?? "-"}</td>
                    <td className="py-2 text-right">
                      {canRun && (message.status === "failed" || message.status === "dead") && (
                        <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => void requeue(message.id ?? "")}>{t("requeue")}</Button>
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
            <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={page <= 1} onClick={() => setPage((current) => current - 1)}>{t("previous")}</Button>
            <span className="font-mono">{page} / {totalPages}</span>
            <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={page >= totalPages} onClick={() => setPage((current) => current + 1)}>{t("next")}</Button>
          </div>
        )}
      </div>
    </section>
  );
}
