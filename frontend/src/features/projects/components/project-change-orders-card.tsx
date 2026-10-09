"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Drawer } from "@/components/ui/Drawer";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { formatCurrency, formatDateTime } from "@/lib/formatters/formatters";
import type { ProjectChangeOrderAction, ProjectChangeOrderResponse, ProjectControlResponse } from "@/lib/api/api-client";
import type { useProjectControlMutations } from "../api/project-queries";
import { changeOrderStatusVariant, isChangeOrderStatus } from "../project-status";

interface ProjectChangeOrdersCardProps {
  control: ProjectControlResponse;
  canManage: boolean;
  canApprove: boolean;
  mutations: ReturnType<typeof useProjectControlMutations>;
  describeError: (error: unknown) => string;
}

interface PendingDecision {
  order: ProjectChangeOrderResponse;
  action: Extract<ProjectChangeOrderAction, "approve" | "reject">;
}

const OPEN_STATUSES = ["active", "on_hold"];

export function ProjectChangeOrdersCard({ control, canManage, canApprove, mutations, describeError }: ProjectChangeOrdersCardProps) {
  const t = useTranslations("projects.control");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const orders: ProjectChangeOrderResponse[] = control.changeOrders ?? [];
  const canCreate = canManage && OPEN_STATUSES.includes(control.status ?? "");

  const [isCreating, setIsCreating] = useState(false);
  const [title, setTitle] = useState("");
  const [reason, setReason] = useState("");
  const [budgetDelta, setBudgetDelta] = useState("0");
  const [contractDelta, setContractDelta] = useState("0");
  const [pending, setPending] = useState<PendingDecision | null>(null);
  const [note, setNote] = useState("");
  const [error, setError] = useState<string | null>(null);
  // One key per create dialog session so a retried submit replays instead of creating a duplicate.
  const createKeyRef = useRef(crypto.randomUUID());

  const busy = mutations.createChangeOrder.isPending || mutations.changeOrderAction.isPending;

  function openCreate(): void {
    createKeyRef.current = crypto.randomUUID();
    setTitle("");
    setReason("");
    setBudgetDelta("0");
    setContractDelta("0");
    setError(null);
    setIsCreating(true);
  }

  async function create(): Promise<void> {
    const budget = Number(budgetDelta);
    const contract = Number(contractDelta);
    if (!title.trim() || !reason.trim() || !Number.isFinite(budget) || !Number.isFinite(contract)) {
      setError(t("changeOrderInvalid"));
      return;
    }

    try {
      await mutations.createChangeOrder.mutateAsync({
        payload: { title: title.trim(), reason: reason.trim(), budgetDelta: budget, contractDelta: contract },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("changeOrderCreated"));
      setIsCreating(false);
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  async function act(order: ProjectChangeOrderResponse, action: ProjectChangeOrderAction, actionNote: string | null): Promise<boolean> {
    setError(null);
    try {
      await mutations.changeOrderAction.mutateAsync({
        changeOrderId: order.id ?? "",
        action,
        expectedVersion: order.rowVersion ?? "",
        note: actionNote,
      });
      toast.success(t(`changeOrderActionDone.${action}`));
      return true;
    } catch (err: unknown) {
      setError(describeError(err));
      return false;
    }
  }

  async function confirmDecision(): Promise<void> {
    if (!pending) return;
    if (pending.action === "reject" && !note.trim()) {
      setError(t("reasonRequired"));
      return;
    }

    if (await act(pending.order, pending.action, note.trim() || null)) {
      setPending(null);
    }
  }

  return (
    <div className="erp-card p-5 space-y-4" role="region" aria-label={t("changeOrdersTitle")}>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("changeOrdersTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{t("changeOrdersHint")}</p>
        </div>
        <div className="text-right text-xs">
          <div className="text-erp-text-muted">{t("currentContract")}</div>
          <div className="font-mono text-sm font-bold text-erp-navy">{formatCurrency(control.contract?.currentAmount ?? 0, "THB", locale)}</div>
        </div>
      </div>

      <ul className="space-y-2">
        {orders.map((order) => (
          <li key={order.id} className="space-y-2 border border-erp-border bg-erp-surface-subtle p-3">
            <div className="flex flex-wrap items-center justify-between gap-2">
              <div className="min-w-0">
                <div className="text-sm font-medium text-erp-text-main"><span className="font-mono">{order.number}</span> — {order.title}</div>
                <div className="text-xs text-erp-text-muted">
                  {t("changeOrderMeta", { creator: order.createdBy?.displayName ?? "-", date: order.createdAtUtc ? formatDateTime(order.createdAtUtc, locale) : "-" })}
                </div>
              </div>
              <StatusBadge label={isChangeOrderStatus(order.status) ? t(`changeOrderStatuses.${order.status}`) : "-"} variant={changeOrderStatusVariant(order.status)} />
            </div>
            <p className="text-xs text-erp-text-body">{order.reason}</p>
            <div className="flex flex-wrap items-center justify-between gap-2">
              <div className="font-mono text-xs text-erp-text-muted">
                {t("deltaSummary", { budget: formatCurrency(order.budgetDelta ?? 0, "THB", locale), contract: formatCurrency(order.contractDelta ?? 0, "THB", locale) })}
              </div>
              <div className="flex flex-wrap gap-2">
                {canManage && order.status === "draft" && (
                  <Button type="button" variant="primary" size="sm" className="min-h-11" disabled={busy} onClick={() => void act(order, "submit", null)}>{t("submitChangeOrder")}</Button>
                )}
                {canManage && (order.status === "draft" || order.status === "submitted") && (
                  <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={busy} onClick={() => void act(order, "cancel", null)}>{t("cancelChangeOrder")}</Button>
                )}
                {canApprove && order.status === "submitted" && (
                  <>
                    <Button type="button" variant="primary" size="sm" className="min-h-11" disabled={busy} onClick={() => { setNote(""); setError(null); setPending({ order, action: "approve" }); }}>{t("approveChangeOrder")}</Button>
                    <Button type="button" variant="danger" size="sm" className="min-h-11" disabled={busy} onClick={() => { setNote(""); setError(null); setPending({ order, action: "reject" }); }}>{t("rejectChangeOrder")}</Button>
                  </>
                )}
              </div>
            </div>
            {order.decisionNote && <p className="text-xs text-erp-text-muted">{t("decisionNote", { note: order.decisionNote })}</p>}
          </li>
        ))}
        {orders.length === 0 && <li className="text-xs text-erp-text-muted">{t("changeOrdersEmpty")}</li>}
      </ul>

      {canCreate && (
        <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={openCreate}>{t("newChangeOrder")}</Button>
      )}
      {error && !isCreating && !pending && <p className="text-xs text-erp-danger" role="alert">{error}</p>}

      <Drawer
        isOpen={isCreating}
        onClose={() => { if (!busy) setIsCreating(false); }}
        title={t("newChangeOrder")}
        description={t("changeOrderCreateDesc")}
        size="md"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        footer={(
          <div className="flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3">
            <Button
              type="button"
              variant="outline"
              className="w-full sm:w-auto min-h-11"
              disabled={busy}
              onClick={() => setIsCreating(false)}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              className="w-full sm:w-auto min-h-11"
              isLoading={mutations.createChangeOrder.isPending}
              disabled={busy}
              onClick={() => void create()}
            >
              {t("createChangeOrder")}
            </Button>
          </div>
        )}
      >
        <div className="space-y-3 py-2">
          {error && <p className="text-xs text-erp-danger" role="alert">{error}</p>}
          <Input label={t("changeOrderTitle")} required value={title} maxLength={200} disabled={busy} placeholder={t("titlePlaceholder")} onChange={(e) => setTitle(e.target.value)} />
          <Input label={t("changeOrderReason")} required value={reason} maxLength={1000} disabled={busy} placeholder={t("reasonPlaceholder")} onChange={(e) => setReason(e.target.value)} />
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <Input type="number" step="0.01" label={t("budgetDelta")} value={budgetDelta} disabled={busy} placeholder={t("budgetDeltaPlaceholder")} onChange={(e) => setBudgetDelta(e.target.value)} />
            <Input type="number" step="0.01" label={t("contractDelta")} value={contractDelta} disabled={busy} placeholder={t("contractDeltaPlaceholder")} onChange={(e) => setContractDelta(e.target.value)} />
          </div>
        </div>
      </Drawer>

      <Modal
        isOpen={pending !== null}
        onClose={() => { if (!busy) setPending(null); }}
        title={pending ? t(pending.action === "approve" ? "approveChangeOrder" : "rejectChangeOrder") : undefined}
        description={pending ? t(pending.action === "approve" ? "approveDescription" : "rejectDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setPending(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant={pending?.action === "reject" ? "danger" : "primary"} className="min-h-11" isLoading={mutations.changeOrderAction.isPending} disabled={busy} onClick={() => void confirmDecision()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <div className="space-y-3">
          {error && <p className="text-xs text-erp-danger" role="alert">{error}</p>}
          <Input label={t("decisionNoteLabel")} required={pending?.action === "reject"} value={note} maxLength={500} disabled={busy} placeholder={t("notePlaceholder")} onChange={(e) => setNote(e.target.value)} />
        </div>
      </Modal>
    </div>
  );
}
