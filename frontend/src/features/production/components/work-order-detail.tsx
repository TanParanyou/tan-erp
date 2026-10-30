"use client";

import { useRef, useState } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Badge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { usePurchaseOrderList } from "@/features/procurement/api/procurement-queries";
import type {
  WorkOrderOperationResponse,
  WorkOrderResponse,
  WorkOrderStockOperation,
} from "@/lib/api/api-client";
import { useWorkOrder, useWorkOrderMutations } from "../api/production-queries";
import {
  isOperationStatus,
  isWorkOrderStatus,
  isWorkOrderTransactionKind,
  productionErrorCode,
  workOrderStatusVariant,
} from "../production-status";

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
  const tBom = useTranslations("production.boms");
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
  const operations = order.operations ?? [];
  const laborEntries = order.laborEntries ?? [];
  const stockOpen = STOCK_STATUSES.includes(status);

  const [message, setMessage] = useState<string | null>(null);
  const [issueQty, setIssueQty] = useState<Record<string, string>>({});
  const [returnQty, setReturnQty] = useState<Record<string, string>>({});
  const [completeQty, setCompleteQty] = useState("");
  const [cancelOpen, setCancelOpen] = useState(false);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);

  // Complete Operation Modal state
  const [completeOpOpen, setCompleteOpOpen] = useState(false);
  const [selectedOp, setSelectedOp] = useState<WorkOrderOperationResponse | null>(null);
  const [actualHours, setActualHours] = useState("");
  const [opNote, setOpNote] = useState("");
  const [opError, setOpError] = useState<string | null>(null);

  // Link Subcontract PO Modal state
  const [subcontractOpen, setSubcontractOpen] = useState(false);
  const [subcontractPoId, setSubcontractPoId] = useState("");
  const [subcontractCost, setSubcontractCost] = useState("");
  const [subcontractNote, setSubcontractNote] = useState("");
  const [subcontractError, setSubcontractError] = useState<string | null>(null);

  const purchaseOrdersQuery = usePurchaseOrderList(
    { status: "approved", page: 1, pageSize: 100 },
  );

  // Idempotency keys
  const issueKeyRef = useRef(crypto.randomUUID());
  const returnKeyRef = useRef(crypto.randomUUID());
  const completeKeyRef = useRef(crypto.randomUUID());
  const completeOpKeyRef = useRef(crypto.randomUUID());
  const subcontractKeyRef = useRef(crypto.randomUUID());

  const busy =
    mutations.release.isPending ||
    mutations.cancel.isPending ||
    mutations.materials.isPending ||
    mutations.complete.isPending ||
    mutations.completeOperation.isPending ||
    mutations.subcontractOperation.isPending;

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

  function openCompleteOperation(op: WorkOrderOperationResponse): void {
    setSelectedOp(op);
    const planned = (op.plannedSetupHours ?? 0) + (op.plannedRunHours ?? 0);
    setActualHours(op.actualHours != null ? String(op.actualHours) : (planned > 0 ? String(planned) : ""));
    setOpNote(op.note ?? "");
    setOpError(null);
    setCompleteOpOpen(true);
  }

  function openSubcontractOperation(op: WorkOrderOperationResponse): void {
    setSelectedOp(op);
    setSubcontractPoId(op.subcontractPurchaseOrder?.id ?? "");
    setSubcontractCost(op.subcontractCost ? String(op.subcontractCost) : "");
    setSubcontractNote(op.note ?? "");
    setSubcontractError(null);
    setSubcontractOpen(true);
  }

  async function submitCompleteOperation(): Promise<void> {
    if (!selectedOp?.id) return;
    const hours = Number(actualHours);
    if (!(hours > 0) || !Number.isFinite(hours)) {
      setOpError(t("actualHoursRequired"));
      return;
    }

    setOpError(null);
    try {
      await mutations.completeOperation.mutateAsync({
        id: order.id ?? "",
        operationId: selectedOp.id,
        payload: {
          actualHours: hours,
          note: opNote.trim() || null,
        },
        idempotencyKey: completeOpKeyRef.current,
      });
      completeOpKeyRef.current = crypto.randomUUID();
      toast.success(t("operationCompletedDone"));
      setCompleteOpOpen(false);
      setSelectedOp(null);
    } catch (err: unknown) {
      setOpError(describe(err));
    }
  }

  async function submitSubcontractOperation(): Promise<void> {
    if (!selectedOp?.id) return;
    if (!subcontractPoId) {
      setSubcontractError(t("purchaseOrderRequired"));
      return;
    }
    const cost = Number(subcontractCost);
    if (!(cost > 0) || !Number.isFinite(cost)) {
      setSubcontractError(t("costAmountRequired"));
      return;
    }

    setSubcontractError(null);
    try {
      await mutations.subcontractOperation.mutateAsync({
        id: order.id ?? "",
        operationId: selectedOp.id,
        payload: {
          purchaseOrderId: subcontractPoId,
          costAmount: cost,
          note: subcontractNote.trim() || null,
        },
        idempotencyKey: subcontractKeyRef.current,
      });
      subcontractKeyRef.current = crypto.randomUUID();
      toast.success(t("operationSubcontractedDone"));
      setSubcontractOpen(false);
      setSelectedOp(null);
    } catch (err: unknown) {
      setSubcontractError(describe(err));
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
          <div>
            <dt className="text-xs text-erp-text-muted">{t("plannedDates")}</dt>
            <dd className="font-medium">
              {order.plannedStartDate || order.plannedEndDate
                ? `${order.plannedStartDate ?? "-"} ~ ${order.plannedEndDate ?? "-"}`
                : "-"}
            </dd>
          </div>
          <div><dt className="text-xs text-erp-text-muted">{t("netMaterialValue")}</dt><dd className="font-mono font-semibold">{formatCurrency(order.netMaterialValue, "THB", locale)}</dd></div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("laborCost")}</dt>
            <dd className="font-mono font-semibold">
              {formatCurrency(order.laborCost, "THB", locale)}
              {order.laborHours ? <span className="text-xs font-normal text-erp-text-muted ml-1">({formatNumber(order.laborHours)} {t("hoursAbbr")})</span> : null}
            </dd>
          </div>
          <div><dt className="text-xs text-erp-text-muted">{t("subcontractCost")}</dt><dd className="font-mono font-semibold">{formatCurrency(order.subcontractCost, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("totalManufacturingCost")}</dt><dd className="font-mono font-bold text-erp-navy">{formatCurrency(order.totalManufacturingCost, "THB", locale)}</dd></div>
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
                        <Input type="number" min={0} step="0.0001" aria-label={t("issueQuantityFor", { item: material.item?.code ?? "-" })} placeholder={t("issueQuantityPlaceholder")} disabled={busy} value={issueQty[itemId] ?? ""} onChange={(event) => setIssueQty((current) => ({ ...current, [itemId]: event.target.value }))} />
                      </td>
                    )}
                    {stockOpen && canOperate && (
                      <td className="py-2 text-right">
                        <Input type="number" min={0} step="0.0001" aria-label={t("returnQuantityFor", { item: material.item?.code ?? "-" })} placeholder={t("returnQuantityPlaceholder")} disabled={busy} value={returnQty[itemId] ?? ""} onChange={(event) => setReturnQty((current) => ({ ...current, [itemId]: event.target.value }))} />
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

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("operationsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("operationsTitle")}</h3>
        {operations.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("operationsEmpty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2 w-16">{tBom("routing.sequence")}</th>
                  <th className="py-2 pr-2 w-24">{tBom("routing.operationCode")}</th>
                  <th className="py-2 pr-2">{tBom("routing.operationName")}</th>
                  <th className="py-2 pr-2">{tBom("routing.workCenter")}</th>
                  <th className="py-2 pr-2 text-right">{t("plannedHours")}</th>
                  <th className="py-2 pr-2 text-right">{t("actualHours")}</th>
                  <th className="py-2 pr-2">{t("subcontractPoAndCost")}</th>
                  <th className="py-2 pr-2">{t("status")}</th>
                  {canOperate && ["released", "in_progress"].includes(status) && <th className="py-2 text-right">{t("actions")}</th>}
                </tr>
              </thead>
              <tbody>
                {operations.map((op) => {
                  const plannedHours = (op.plannedSetupHours ?? 0) + (op.plannedRunHours ?? 0);
                  const isCompleted = op.status === "completed";
                  return (
                    <tr key={op.id ?? op.sequence} className="border-b border-erp-border/60 align-middle">
                      <td className="py-2 pr-2 font-mono font-medium">{op.sequence}</td>
                      <td className="py-2 pr-2 font-mono">{op.operationCode ?? "-"}</td>
                      <td className="py-2 pr-2 font-medium">{op.operationName ?? "-"}</td>
                      <td className="py-2 pr-2">
                        {op.isSubcontracted ? (
                          <Badge variant="warning">{tBom("routing.subcontractBadge")}</Badge>
                        ) : (
                          <span>{op.workCenter?.code ? `${op.workCenter.code} · ${op.workCenter.name ?? ""}` : "-"}</span>
                        )}
                      </td>
                      <td className="py-2 pr-2 text-right font-mono">{formatNumber(plannedHours)}</td>
                      <td className="py-2 pr-2 text-right font-mono">{op.actualHours != null ? formatNumber(op.actualHours) : "-"}</td>
                      <td className="py-2 pr-2">
                        {op.isSubcontracted ? (
                          <div>
                            {op.subcontractPurchaseOrder?.code ? (
                              <span className="font-mono font-medium">{op.subcontractPurchaseOrder.code}</span>
                            ) : (
                              <span className="text-erp-text-muted">-</span>
                            )}
                            {op.subcontractCost != null && op.subcontractCost > 0 ? (
                              <div className="font-mono text-xs">{formatCurrency(op.subcontractCost, "THB", locale)}</div>
                            ) : null}
                          </div>
                        ) : (
                          <span className="text-erp-text-muted">-</span>
                        )}
                      </td>
                      <td className="py-2 pr-2">
                        <StatusBadge
                          label={isOperationStatus(op.status) ? t(`operationStatuses.${op.status}`) : "-"}
                          variant={op.status === "completed" ? "success" : "neutral"}
                        />
                      </td>
                      {canOperate && ["released", "in_progress"].includes(status) && (
                        <td className="py-2 text-right">
                          {!isCompleted && !op.isSubcontracted && (
                            <Button
                              type="button"
                              variant="outline"
                              size="sm"
                              className="min-h-11"
                              disabled={busy}
                              onClick={() => openCompleteOperation(op)}
                            >
                              {t("completeOperation")}
                            </Button>
                          )}
                          {!isCompleted && op.isSubcontracted && (
                            <Button
                              type="button"
                              variant="outline"
                              size="sm"
                              className="min-h-11"
                              disabled={busy}
                              onClick={() => openSubcontractOperation(op)}
                            >
                              {t("linkSubcontractPo")}
                            </Button>
                          )}
                        </td>
                      )}
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("laborEntriesTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("laborEntriesTitle")}</h3>
        {laborEntries.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("laborEntriesEmpty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("laborNumber")}</th>
                  <th className="py-2 pr-2">{t("worker")}</th>
                  <th className="py-2 pr-2">{t("workDate")}</th>
                  <th className="py-2 pr-2 text-right">{t("hours")}</th>
                  <th className="py-2 pr-2 text-right">{t("hourlyRate")}</th>
                  <th className="py-2 pr-2 text-right">{t("costAmountHeader")}</th>
                  <th className="py-2 pr-2">{t("taskDescription")}</th>
                  <th className="py-2">{t("decidedAt")}</th>
                </tr>
              </thead>
              <tbody>
                {laborEntries.map((entry) => (
                  <tr key={entry.id ?? entry.number} className="border-b border-erp-border/60 align-middle">
                    <td className="py-2 pr-2 font-mono font-medium">{entry.number ?? "-"}</td>
                    <td className="py-2 pr-2">{entry.worker?.displayName ?? "-"}</td>
                    <td className="py-2 pr-2 font-mono">{entry.workDate ?? "-"}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(entry.hours)}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatCurrency(entry.hourlyRate, "THB", locale)}</td>
                    <td className="py-2 pr-2 text-right font-mono font-semibold">{formatCurrency(entry.costAmount, "THB", locale)}</td>
                    <td className="py-2 pr-2 text-erp-text-muted">{entry.taskDescription ?? "-"}</td>
                    <td className="py-2 text-erp-text-muted font-mono">{entry.decidedAtUtc ? formatDateTime(entry.decidedAtUtc, locale) : "-"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {status === "in_progress" && canOperate && (
        <div className="erp-card p-5 space-y-3" role="region" aria-label={t("completeTitle")}>
          <h3 className="text-sm font-bold text-erp-navy">{t("completeTitle")}</h3>
          <p className="text-xs text-erp-text-muted">{t("completeHint", { remaining: formatNumber(remainingToComplete) })}</p>
          <div className="flex flex-wrap items-end gap-3">
            <Input type="number" min={0} step="0.0001" label={t("completeQuantity")} value={completeQty} disabled={busy} placeholder={t("completeQuantityPlaceholder")} onChange={(event) => setCompleteQty(event.target.value)} />
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

      {/* Cancel Order Confirmation Modal */}
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
          placeholder={t("reasonPlaceholder")}
          error={reasonError ? t("reasonRequired") : undefined}
          onChange={(event) => { setReason(event.target.value); if (event.target.value.trim()) setReasonError(false); }}
        />
      </Modal>

      {/* Complete Operation Modal */}
      <Modal
        isOpen={completeOpOpen}
        onClose={() => { if (!busy) setCompleteOpOpen(false); }}
        title={t("completeOperationTitle")}
        description={t("completeOperationDescription")}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setCompleteOpOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.completeOperation.isPending} disabled={busy} onClick={() => void submitCompleteOperation()}>{tCommon("actions.save")}</Button>
          </div>
        )}
      >
        <div className="space-y-3">
          {opError && <Alert variant="danger" onClose={() => setOpError(null)}>{opError}</Alert>}
          <div className="text-xs text-erp-text-muted">
            {selectedOp?.sequence} · {selectedOp?.operationCode} · {selectedOp?.operationName}
          </div>
          <Input
            type="number"
            min={0.01}
            step="0.01"
            label={t("actualHours")}
            required
            value={actualHours}
            disabled={busy}
            placeholder={t("actualHoursPlaceholder")}
            onChange={(e) => setActualHours(e.target.value)}
          />
          <Input
            label={t("note")}
            value={opNote}
            maxLength={500}
            disabled={busy}
            placeholder={t("operationNotePlaceholder")}
            onChange={(e) => setOpNote(e.target.value)}
          />
        </div>
      </Modal>

      {/* Link Subcontract PO Modal */}
      <Modal
        isOpen={subcontractOpen}
        onClose={() => { if (!busy) setSubcontractOpen(false); }}
        title={t("linkSubcontractPoTitle")}
        description={t("linkSubcontractPoDescription")}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setSubcontractOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.subcontractOperation.isPending} disabled={busy} onClick={() => void submitSubcontractOperation()}>{tCommon("actions.save")}</Button>
          </div>
        )}
      >
        <div className="space-y-3">
          {subcontractError && <Alert variant="danger" onClose={() => setSubcontractError(null)}>{subcontractError}</Alert>}
          <div className="text-xs text-erp-text-muted">
            {selectedOp?.sequence} · {selectedOp?.operationCode} · {selectedOp?.operationName}
          </div>
          <Select
            label={t("purchaseOrder")}
            required
            value={subcontractPoId}
            placeholder={t("selectPurchaseOrder")}
            disabled={busy}
            options={(purchaseOrdersQuery.data?.items ?? []).map((po) => ({
              value: po.id ?? "",
              label: `${po.number ?? "-"} · ${po.supplierNameTh ?? po.supplierCode ?? "-"} (${formatCurrency(po.totalAmount, "THB", locale)})`,
            }))}
            onChange={(e) => {
              setSubcontractPoId(e.target.value);
              setSubcontractError(null);
            }}
          />
          <Input
            type="number"
            min={0.01}
            step="0.01"
            label={t("costAmount")}
            required
            value={subcontractCost}
            disabled={busy}
            placeholder={t("costAmountPlaceholder")}
            onChange={(e) => {
              setSubcontractCost(e.target.value);
              setSubcontractError(null);
            }}
          />
          <Input
            label={t("note")}
            value={subcontractNote}
            maxLength={500}
            disabled={busy}
            placeholder={t("subcontractNotePlaceholder")}
            onChange={(e) => setSubcontractNote(e.target.value)}
          />
        </div>
      </Modal>
    </section>
  );
}
