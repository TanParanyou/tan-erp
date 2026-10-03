"use client";

import { useRef, useState } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { WorkOrderResponse, WorkOrderStockOperation } from "@/lib/api/api-client";
import { useWorkOrder, useWorkOrderMutations } from "../api/production-queries";
import { isWorkOrderStatus, isWorkOrderTransactionKind, productionErrorCode, workOrderStatusVariant } from "../production-status";

interface WorkOrderDetailProps {
  workOrderId: string;
}

const STOCK_STATUSES = ["released", "in_progress"];

export function WorkOrderDetail({ workOrderId }: WorkOrderDetailProps) {
  const query = useWorkOrder(workOrderId);
  const t = useTranslations("production.workOrders");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;

  // Keyed by row version so quantity inputs reset to the new balances after every change.
  return <DetailView key={query.data.rowVersion} order={query.data} />;
}

function DetailView({ order }: { order: WorkOrderResponse }) {
  const t = useTranslations("production.workOrders");
  const tErrors = useTranslations("production.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.WORK_ORDERS_MANAGE);
  const canOperate = can(selectedMembership, PERMISSIONS.WORK_ORDERS_OPERATE);
  const mutations = useWorkOrderMutations();

  const status = order.status ?? "";
  const materials = order.materials ?? [];
  const stockOpen = STOCK_STATUSES.includes(status);
  const [message, setMessage] = useState<string | null>(null);
  const [issueQty, setIssueQty] = useState<Record<string, string>>({});
  const [returnQty, setReturnQty] = useState<Record<string, string>>({});
  const [completeQty, setCompleteQty] = useState("");
  const [cancelOpen, setCancelOpen] = useState(false);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);
  // One key per operation session, regenerated after each success so a retry replays but a new action does not.
  const issueKeyRef = useRef(crypto.randomUUID());
  const returnKeyRef = useRef(crypto.randomUUID());
  const completeKeyRef = useRef(crypto.randomUUID());

  const busy = mutations.release.isPending || mutations.cancel.isPending || mutations.materials.isPending || mutations.complete.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? productionErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function release(): Promise<void> {
    setMessage(null);
    try {
      await mutations.release.mutateAsync({ id: order.id ?? "", rowVersion: order.rowVersion ?? "" });
      toast.success(t("actionDone.release"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function confirmCancel(): Promise<void> {
    if (!reason.trim()) {
      setReasonError(true);
      return;
    }

    setMessage(null);
    try {
      await mutations.cancel.mutateAsync({ id: order.id ?? "", rowVersion: order.rowVersion ?? "", reason: reason.trim() });
      toast.success(t("actionDone.cancel"));
      setCancelOpen(false);
    } catch (err: unknown) {
      setCancelOpen(false);
      setMessage(describe(err));
    }
  }

  async function postMaterials(operation: WorkOrderStockOperation): Promise<void> {
    setMessage(null);
    const quantities = operation === "issues" ? issueQty : returnQty;
    const lines = materials
      .map((m) => ({ itemId: m.item?.id ?? "", quantity: Number(quantities[m.item?.id ?? ""] ?? 0) }))
      .filter((l) => l.quantity > 0);
    if (lines.length === 0 || lines.some((l) => !Number.isFinite(l.quantity))) {
      setMessage(t("quantitiesInvalid"));
      return;
    }

    const keyRef = operation === "issues" ? issueKeyRef : returnKeyRef;
    try {
      await mutations.materials.mutateAsync({ id: order.id ?? "", operation, payload: { lines }, idempotencyKey: keyRef.current });
      keyRef.current = crypto.randomUUID();
      toast.success(t(operation === "issues" ? "issuedToast" : "returnedToast"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function complete(): Promise<void> {
    setMessage(null);
    const quantity = Number(completeQty);
    if (!(quantity > 0) || !Number.isFinite(quantity)) {
      setMessage(t("quantitiesInvalid"));
      return;
    }

    try {
      await mutations.complete.mutateAsync({ id: order.id ?? "", quantity, idempotencyKey: completeKeyRef.current });
      completeKeyRef.current = crypto.randomUUID();
      toast.success(t("completedDone"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  const remainingToComplete = (order.plannedQuantity ?? 0) - (order.completedQuantity ?? 0);

  return (
    <section className="space-y-5">
      <PageHeader
        title={order.number ?? "-"}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/work-orders` }, { label: order.number ?? "-" }]}
        actions={<StatusBadge label={isWorkOrderStatus(status) ? t(`statuses.${status}`) : "-"} variant={workOrderStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card p-5 space-y-4">
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 text-sm">
          <div><dt className="text-xs text-erp-text-muted">{t("item")}</dt><dd className="font-medium">{order.item?.code ?? "-"} · {order.item?.nameTh ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("bom")}</dt><dd className="font-mono font-medium">{order.bomCode ?? "-"} · R{order.bomRevisionNo ?? 0}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("warehouse")}</dt><dd className="font-medium">{order.warehouse?.code ?? "-"} · {order.warehouse?.name ?? "-"}</dd></div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("project")}</dt>
            <dd className="font-medium">{order.project?.id ? <Link href={`/${locale}/projects/${order.project.id}`} className="font-mono text-erp-navy underline">{order.project.code}</Link> : "-"}</dd>
          </div>
          <div><dt className="text-xs text-erp-text-muted">{t("progress")}</dt><dd className="font-mono font-semibold">{formatNumber(order.completedQuantity)} / {formatNumber(order.plannedQuantity)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("costAllocated")}</dt><dd className="font-mono font-semibold">{formatCurrency(order.costAllocated, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{order.createdBy?.displayName ?? "-"}</dd></div>
        </dl>
        {order.note && <p className="text-xs text-erp-text-body">{t("noteLine", { note: order.note })}</p>}
        {order.cancelReason && <p className="text-xs text-erp-text-muted">{t("cancelReason", { reason: order.cancelReason })}</p>}
        <div className="flex flex-wrap gap-2">
          {status === "draft" && canManage && (
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.release.isPending} disabled={busy} onClick={() => void release()}>{t("release")}</Button>
          )}
          {["draft", "released", "in_progress"].includes(status) && canManage && (order.completedQuantity ?? 0) === 0 && (
            <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => { setReason(""); setReasonError(false); setCancelOpen(true); }}>{t("cancel")}</Button>
          )}
        </div>
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("materialsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("materialsTitle")}</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-erp-border">
                <th className="py-2 pr-2">{t("material")}</th>
                <th className="py-2 pr-2 text-right">{t("required")}</th>
                <th className="py-2 pr-2 text-right">{t("issued")}</th>
                <th className="py-2 pr-2 text-right">{t("returned")}</th>
                <th className="py-2 pr-2 text-right">{t("remaining")}</th>
                {stockOpen && canOperate && <th className="py-2 pr-2 text-right">{t("issueNow")}</th>}
                {stockOpen && canOperate && <th className="py-2 text-right">{t("returnNow")}</th>}
              </tr>
            </thead>
            <tbody>
              {materials.map((material) => {
                const itemId = material.item?.id ?? "";
                return (
                  <tr key={material.id} className="border-b border-erp-border/60 align-middle">
                    <td className="py-2 pr-2">{material.item?.code ?? "-"} · {material.item?.nameTh ?? "-"} ({material.item?.unitCode ?? "-"})</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(material.requiredQuantity)}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(material.issuedQuantity)}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(material.returnedQuantity)}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(material.remainingQuantity)}</td>
                    {stockOpen && canOperate && (
                      <td className="py-2 pr-2 text-right">
                        <Input type="number" min={0} step="0.0001" aria-label={t("issueQuantityFor", { item: material.item?.code ?? "-" })} disabled={busy} value={issueQty[itemId] ?? ""} onChange={(event) => setIssueQty((current) => ({ ...current, [itemId]: event.target.value }))} />
                      </td>
                    )}
                    {stockOpen && canOperate && (
                      <td className="py-2 text-right">
                        <Input type="number" min={0} step="0.0001" aria-label={t("returnQuantityFor", { item: material.item?.code ?? "-" })} disabled={busy} value={returnQty[itemId] ?? ""} onChange={(event) => setReturnQty((current) => ({ ...current, [itemId]: event.target.value }))} />
                      </td>
                    )}
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
        {stockOpen && canOperate && (
          <div className="flex flex-wrap gap-3">
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.materials.isPending} disabled={busy} onClick={() => void postMaterials("issues")}>{t("postIssue")}</Button>
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => void postMaterials("returns")}>{t("postReturn")}</Button>
          </div>
        )}
      </div>

      {status === "in_progress" && canOperate && (
        <div className="erp-card p-5 space-y-3" role="region" aria-label={t("completeTitle")}>
          <h3 className="text-sm font-bold text-erp-navy">{t("completeTitle")}</h3>
          <p className="text-xs text-erp-text-muted">{t("completeHint", { remaining: formatNumber(remainingToComplete) })}</p>
          <div className="flex flex-wrap items-end gap-3">
            <Input type="number" min={0} step="0.0001" label={t("completeQuantity")} value={completeQty} disabled={busy} onChange={(event) => setCompleteQty(event.target.value)} />
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.complete.isPending} disabled={busy} onClick={() => void complete()}>{t("postCompletion")}</Button>
          </div>
        </div>
      )}

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("transactionsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("transactionsTitle")}</h3>
        {(order.transactions ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("transactionsEmpty")}</p>
        ) : (
          <ul className="space-y-2">
            {(order.transactions ?? []).map((tx) => (
              <li key={tx.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="font-semibold text-erp-text-main">
                  {isWorkOrderTransactionKind(tx.kind) ? t(`kinds.${tx.kind}`) : "-"} • <span className="font-mono">{tx.stockDocumentNumber}</span>
                </div>
                <div className="text-erp-text-muted">
                  {formatNumber(tx.quantity)} • {formatCurrency(tx.value, "THB", locale)} • {tx.actor?.displayName ?? "-"} • {tx.createdAtUtc ? formatDateTime(tx.createdAtUtc, locale) : "-"}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      <Modal
        isOpen={cancelOpen}
        onClose={() => { if (!busy) setCancelOpen(false); }}
        title={t("cancel")}
        description={t("cancelDescription")}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setCancelOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="danger" className="min-h-11" isLoading={mutations.cancel.isPending} disabled={busy} onClick={() => void confirmCancel()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <Input
          label={t("reason")}
          required
          value={reason}
          maxLength={500}
          disabled={busy}
          error={reasonError ? t("reasonRequired") : undefined}
          onChange={(event) => { setReason(event.target.value); if (event.target.value.trim()) setReasonError(false); }}
        />
      </Modal>
    </section>
  );
}
