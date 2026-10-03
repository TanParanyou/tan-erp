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
import { formatCurrency, formatDate, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { PurchaseOrderAction, PurchaseOrderResponse } from "@/lib/api/api-client";
import { PutIntoStockButton } from "@/features/inventory/components/put-into-stock-button";
import { usePurchaseOrder, usePurchaseOrderMutations } from "../api/procurement-queries";
import { isPurchaseOrderStatus, procurementErrorCode, purchaseOrderStatusVariant } from "../procurement-status";

interface PurchaseOrderDetailProps {
  purchaseOrderId: string;
}

type NoteAction = Extract<PurchaseOrderAction, "reject" | "cancel">;

const RECEIVING_STATUSES = ["approved", "partially_received"];

export function PurchaseOrderDetail({ purchaseOrderId }: PurchaseOrderDetailProps) {
  const query = usePurchaseOrder(purchaseOrderId);
  const t = useTranslations("procurement.orders");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;

  // Keyed by row version so receipt inputs reset to the new remaining quantities after every change.
  return <DetailView key={query.data.rowVersion} order={query.data} />;
}

function DetailView({ order }: { order: PurchaseOrderResponse }) {
  const t = useTranslations("procurement.orders");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_CREATE);
  const canApprove = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_APPROVE);
  const canReceive = can(selectedMembership, PERMISSIONS.GOODS_RECEIPTS_CREATE);
  const canPutIntoStock = can(selectedMembership, PERMISSIONS.INVENTORY_RECEIVE);
  const mutations = usePurchaseOrderMutations();

  const status = order.status ?? "";
  const lines = order.lines ?? [];
  const [noteAction, setNoteAction] = useState<NoteAction | null>(null);
  const [note, setNote] = useState("");
  const [noteError, setNoteError] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [quantities, setQuantities] = useState<Record<string, string>>({});
  const [receiptNote, setReceiptNote] = useState("");
  // One key per receipt form session, regenerated after a successful post.
  const receiptKeyRef = useRef(crypto.randomUUID());

  const busy = mutations.act.isPending || mutations.receive.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function run(action: PurchaseOrderAction, actionNote: string | null): Promise<boolean> {
    setMessage(null);
    try {
      await mutations.act.mutateAsync({ id: order.id ?? "", rowVersion: order.rowVersion ?? "", action, note: actionNote });
      toast.success(t(`actionDone.${action}`));
      return true;
    } catch (err: unknown) {
      setMessage(describe(err));
      return false;
    }
  }

  async function confirmNoteAction(): Promise<void> {
    if (!noteAction) return;
    if (!note.trim()) {
      setNoteError(true);
      return;
    }

    if (await run(noteAction, note.trim())) setNoteAction(null);
  }

  async function postReceipt(): Promise<void> {
    setMessage(null);
    const entries = lines
      .map((line) => ({ purchaseOrderLineId: line.id ?? "", quantity: Number(quantities[line.id ?? ""] ?? 0) }))
      .filter((entry) => entry.quantity > 0);
    if (entries.length === 0 || entries.some((entry) => !Number.isFinite(entry.quantity))) {
      setMessage(t("receiptInvalid"));
      return;
    }

    try {
      await mutations.receive.mutateAsync({
        id: order.id ?? "",
        payload: { receivedAtUtc: null, note: receiptNote.trim() || null, lines: entries },
        idempotencyKey: receiptKeyRef.current,
      });
      receiptKeyRef.current = crypto.randomUUID();
      toast.success(t("receiptPosted"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  const supplierName = order.supplier ? `${order.supplier.code ?? "-"} · ${locale === "en" ? order.supplier.nameEn ?? order.supplier.nameTh : order.supplier.nameTh}` : "-";

  return (
    <section className="space-y-5">
      <PageHeader
        title={`${order.number ?? "-"}`}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/procurement/purchase-orders` }, { label: order.number ?? "-" }]}
        actions={<StatusBadge label={isPurchaseOrderStatus(status) ? t(`statuses.${status}`) : "-"} variant={purchaseOrderStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card p-5 space-y-4">
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 text-sm">
          <div><dt className="text-xs text-erp-text-muted">{t("supplier")}</dt><dd className="font-medium">{supplierName}</dd></div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("project")}</dt>
            <dd className="font-medium">
              {order.project?.id ? <Link href={`/${locale}/projects/${order.project.id}`} className="font-mono text-erp-navy underline">{order.project.code}</Link> : "-"}
            </dd>
          </div>
          <div><dt className="text-xs text-erp-text-muted">{t("expectedDelivery")}</dt><dd className="font-medium">{order.expectedDeliveryDate ? formatDate(order.expectedDeliveryDate, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{order.createdBy?.displayName ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("decidedBy")}</dt><dd className="font-medium">{order.decidedBy?.displayName ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("total")}</dt><dd className="font-mono font-semibold">{formatCurrency(order.totalAmount, "THB", locale)}</dd></div>
        </dl>
        {order.note && <p className="text-xs text-erp-text-body">{t("noteLine", { note: order.note })}</p>}
        {order.decisionNote && <p className="text-xs text-erp-text-muted">{t("decisionNote", { note: order.decisionNote })}</p>}
        {order.cancelReason && <p className="text-xs text-erp-text-muted">{t("cancelReason", { reason: order.cancelReason })}</p>}

        <div className="flex flex-wrap gap-2">
          {status === "draft" && canCreate && (
            <>
              <Button href={`/${locale}/procurement/purchase-orders/${order.id}/edit`} variant="outline" className="min-h-11">{t("editDraft")}</Button>
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run("submit", null)}>{t("submit")}</Button>
            </>
          )}
          {status === "submitted" && canApprove && (
            <>
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run("approve", null)}>{t("approve")}</Button>
              <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => { setNote(""); setNoteError(false); setNoteAction("reject"); }}>{t("reject")}</Button>
            </>
          )}
          {["draft", "submitted", "approved"].includes(status) && canCreate && (
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => { setNote(""); setNoteError(false); setNoteAction("cancel"); }}>{t("cancel")}</Button>
          )}
        </div>
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("linesTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("linesTitle")}</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-erp-border">
                <th className="py-2 pr-2">{t("lineNo")}</th>
                <th className="py-2 pr-2">{t("item")}</th>
                <th className="py-2 pr-2 text-right">{t("quantity")}</th>
                <th className="py-2 pr-2 text-right">{t("unitPrice")}</th>
                <th className="py-2 pr-2 text-right">{t("lineTotal")}</th>
                <th className="py-2 pr-2 text-right">{t("received")}</th>
                <th className="py-2 pr-2 text-right">{t("remaining")}</th>
                {canReceive && RECEIVING_STATUSES.includes(status) && <th className="py-2 text-right">{t("receiveNow")}</th>}
              </tr>
            </thead>
            <tbody>
              {lines.map((line) => (
                <tr key={line.id} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2 pr-2 font-mono">{line.lineNo}</td>
                  <td className="py-2 pr-2">{line.itemCode} · {line.itemNameTh} ({line.unitCode})</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.quantity)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatCurrency(line.unitPrice, "THB", locale)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatCurrency(line.lineTotal, "THB", locale)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.receivedQuantity)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.remainingQuantity)}</td>
                  {canReceive && RECEIVING_STATUSES.includes(status) && (
                    <td className="py-2 text-right">
                      <Input
                        type="number"
                        min={0}
                        max={line.remainingQuantity ?? 0}
                        step="0.0001"
                        aria-label={t("receiveQuantityFor", { item: line.itemCode ?? "-" })}
                        disabled={busy || (line.remainingQuantity ?? 0) <= 0}
                        value={quantities[line.id ?? ""] ?? ""}
                        onChange={(event) => setQuantities((current) => ({ ...current, [line.id ?? ""]: event.target.value }))}
                      />
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {canReceive && RECEIVING_STATUSES.includes(status) && (
          <div className="flex flex-wrap items-end gap-3">
            <Input label={t("receiptNote")} value={receiptNote} maxLength={500} disabled={busy} onChange={(event) => setReceiptNote(event.target.value)} />
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.receive.isPending} disabled={busy} onClick={() => void postReceipt()}>{t("postReceipt")}</Button>
          </div>
        )}
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("receiptsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("receiptsTitle")}</h3>
        {(order.receipts ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("receiptsEmpty")}</p>
        ) : (
          <ul className="space-y-2">
            {(order.receipts ?? []).map((receipt) => (
              <li key={receipt.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="font-semibold text-erp-text-main"><span className="font-mono">{receipt.number}</span> • {t("receiptSummary", { lines: receipt.lineCount ?? 0, quantity: formatNumber(receipt.totalQuantity) })}</div>
                <div className="text-erp-text-muted">{receipt.receivedBy?.displayName ?? "-"} • {receipt.receivedAtUtc ? formatDateTime(receipt.receivedAtUtc, locale) : "-"}</div>
                {receipt.note && <div className="mt-1 text-erp-text-body">{receipt.note}</div>}
                <div className="mt-2 flex flex-wrap items-center gap-2">
                  {receipt.stockDocumentNumber ? (
                    <span className="font-mono text-erp-success">{t("putIntoStockDone", { number: receipt.stockDocumentNumber })}</span>
                  ) : canPutIntoStock ? (
                    <PutIntoStockButton goodsReceiptId={receipt.id ?? ""} />
                  ) : (
                    <span className="text-erp-text-muted">{t("notInStock")}</span>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      <Modal
        isOpen={noteAction !== null}
        onClose={() => { if (!busy) setNoteAction(null); }}
        title={noteAction ? t(noteAction === "reject" ? "reject" : "cancel") : undefined}
        description={noteAction ? t(noteAction === "reject" ? "rejectDescription" : "cancelDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setNoteAction(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="danger" className="min-h-11" isLoading={mutations.act.isPending} disabled={busy} onClick={() => void confirmNoteAction()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <Input
          label={t("reason")}
          required
          value={note}
          maxLength={500}
          disabled={busy}
          error={noteError ? t("reasonRequired") : undefined}
          onChange={(event) => { setNote(event.target.value); if (event.target.value.trim()) setNoteError(false); }}
        />
      </Modal>
    </section>
  );
}
