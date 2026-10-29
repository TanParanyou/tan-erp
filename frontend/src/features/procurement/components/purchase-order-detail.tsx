"use client";

import { useRef, useState } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
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
import {
  isPurchaseOrderStatus,
  isSupplierInvoiceMatchStatus,
  isSupplierInvoiceStatus,
  procurementErrorCode,
  purchaseOrderStatusVariant,
  supplierInvoiceMatchStatusVariant,
  supplierInvoiceStatusVariant,
} from "../procurement-status";

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
  const tInvoice = useTranslations("procurement.invoices");
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

  // Supplier Return modal state
  const [isReturnModalOpen, setIsReturnModalOpen] = useState(false);
  const [returnReceiptId, setReturnReceiptId] = useState(order.receipts?.[0]?.id ?? "");
  const [returnReason, setReturnReason] = useState("");
  const [returnQuantities, setReturnQuantities] = useState<Record<string, string>>({});
  const [returnMessage, setReturnMessage] = useState<string | null>(null);
  const returnKeyRef = useRef(crypto.randomUUID());

  const busy = mutations.act.isPending || mutations.receive.isPending || mutations.returnGoods.isPending;

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

  const selectedReceipt = (order.receipts ?? []).find((r) => r.id === returnReceiptId);
  const returnableLines = selectedReceipt?.lines ?? [];

  async function postReturn(): Promise<void> {
    setReturnMessage(null);
    if (!returnReason.trim()) {
      setReturnMessage(t("returnReasonRequired"));
      return;
    }

    const returnEntries = returnableLines
      .map((line) => ({ goodsReceiptLineId: line.id ?? "", quantity: Number(returnQuantities[line.id ?? ""] ?? 0) }))
      .filter((entry) => entry.quantity > 0);

    if (returnEntries.length === 0 || returnEntries.some((entry) => !Number.isFinite(entry.quantity))) {
      setReturnMessage(t("returnInvalid"));
      return;
    }

    try {
      await mutations.returnGoods.mutateAsync({
        id: order.id ?? "",
        rowVersion: order.rowVersion ?? "",
        payload: {
          goodsReceiptId: returnReceiptId,
          returnedAtUtc: null,
          reason: returnReason.trim(),
          lines: returnEntries,
        },
        idempotencyKey: returnKeyRef.current,
      });
      returnKeyRef.current = crypto.randomUUID();
      toast.success(t("returnSuccess"));
      setIsReturnModalOpen(false);
      setReturnQuantities({});
      setReturnReason("");
    } catch (err: unknown) {
      setReturnMessage(describe(err));
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

        {((order.sourcePurchaseRequests ?? []).length > 0) && (
          <div className="pt-2 border-t border-erp-border">
            <dt className="text-xs font-semibold text-erp-navy mb-1">{t("sourceRequestsTitle")}</dt>
            <dd className="flex flex-wrap gap-2 text-xs">
              {(order.sourcePurchaseRequests ?? []).map((spr) => (
                <Link
                  key={spr.id}
                  href={`/${locale}/procurement/purchase-requests/${spr.id}`}
                  className="font-mono text-erp-navy underline bg-erp-surface-subtle px-2 py-1 border border-erp-border"
                >
                  {spr.number ?? "-"}
                </Link>
              ))}
            </dd>
          </div>
        )}

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
                <th className="py-2 pr-2 text-right">{t("invoicedQuantity")}</th>
                <th className="py-2 pr-2 text-right">{t("uninvoicedReceived")}</th>
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
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.invoicedQuantity ?? 0)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.uninvoicedReceivedQuantity ?? 0)}</td>
                  {canReceive && RECEIVING_STATUSES.includes(status) && (
                    <td className="py-2 text-right">
                      <Input
                        type="number"
                        min={0}
                        max={line.remainingQuantity ?? 0}
                        step="0.0001"
                        aria-label={t("receiveQuantityFor", { item: line.itemCode ?? "-" })}
                        placeholder={t("receiveQuantityPlaceholder")}
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
            <Input label={t("receiptNote")} value={receiptNote} maxLength={500} disabled={busy} placeholder={t("receiptNotePlaceholder")} onChange={(event) => setReceiptNote(event.target.value)} />
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.receive.isPending} disabled={busy} onClick={() => void postReceipt()}>{t("postReceipt")}</Button>
          </div>
        )}
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("receiptsTitle")}>
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-bold text-erp-navy">{t("receiptsTitle")}</h3>
          {canReceive && (order.receipts ?? []).some((r) => (r.lines ?? []).some((l) => (l.remainingReturnableQuantity ?? 0) > 0)) && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="min-h-11"
              disabled={busy}
              onClick={() => {
                setReturnMessage(null);
                setReturnReason("");
                setReturnQuantities({});
                setIsReturnModalOpen(true);
              }}
            >
              {t("recordReturn")}
            </Button>
          )}
        </div>
        {(order.receipts ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("receiptsEmpty")}</p>
        ) : (
          <ul className="space-y-2">
            {(order.receipts ?? []).map((receipt) => (
              <li key={receipt.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="font-semibold text-erp-text-main"><span className="font-mono">{receipt.number}</span> • {t("receiptSummary", { lines: receipt.lineCount ?? 0, quantity: formatNumber(receipt.totalQuantity) })}</div>
                <div className="text-erp-text-muted">{receipt.receivedBy?.displayName ?? "-"} • {receipt.receivedAtUtc ? formatDateTime(receipt.receivedAtUtc, locale) : "-"}</div>
                {receipt.note && <div className="mt-1 text-erp-text-body">{receipt.note}</div>}

                {(receipt.lines ?? []).length > 0 && (
                  <div className="mt-2 pl-2 border-l-2 border-erp-border space-y-1">
                    {(receipt.lines ?? []).map((rl) => (
                      <div key={rl.id} className="text-xs text-erp-text-body flex flex-wrap gap-2 justify-between">
                        <span>{rl.itemCode} · {rl.itemNameTh}</span>
                        <span className="font-mono text-erp-text-muted">
                          {formatNumber(rl.quantity)} {rl.unitCode} (
                          {t("returned")}: {formatNumber(rl.returnedQuantity ?? 0)}, {t("returnable")}: {formatNumber(rl.remainingReturnableQuantity ?? 0)}
                          )
                        </span>
                      </div>
                    ))}
                  </div>
                )}

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

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("returnsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("returnsTitle")}</h3>
        {(order.returns ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("returnsEmpty")}</p>
        ) : (
          <ul className="space-y-2">
            {(order.returns ?? []).map((ret) => (
              <li key={ret.id} className="border-l-4 border-erp-warning bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="font-semibold text-erp-text-main">
                  <span className="font-mono">{ret.number}</span> • {t("returnSummary", { lines: (ret.lines ?? []).length, amount: formatCurrency(ret.totalReturnAmount, "THB", locale) })}
                </div>
                <div className="text-erp-text-muted">
                  {t("returnGoodsReceipt")}: <span className="font-mono font-medium">{ret.goodsReceiptNumber ?? "-"}</span> • {ret.returnedBy?.displayName ?? "-"} • {ret.returnedAtUtc ? formatDateTime(ret.returnedAtUtc, locale) : "-"}
                </div>
                {ret.reason && <div className="mt-1 text-erp-text-body">{t("returnReason")}: {ret.reason}</div>}
                <div className="mt-2 flex flex-wrap items-center gap-2">
                  {ret.stockDocumentNumber ? (
                    <span className="font-mono text-erp-success">{t("returnDoneWithStock", { number: ret.stockDocumentNumber })}</span>
                  ) : (
                    <span className="text-erp-text-muted">{t("returnNotInStock")}</span>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("invoicesTitle")}>
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-bold text-erp-navy">{t("invoicesTitle")}</h3>
          {canCreate && (
            <Button
              href={`/${locale}/procurement/supplier-invoices/create?purchaseOrderId=${order.id}`}
              variant="outline"
              size="sm"
              className="min-h-11"
            >
              {t("createInvoice")}
            </Button>
          )}
        </div>
        {(order.invoices ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("invoicesEmpty")}</p>
        ) : (
          <ul className="space-y-2">
            {(order.invoices ?? []).map((inv) => (
              <li key={inv.id} className="border-l-4 border-erp-info bg-erp-surface-subtle px-3 py-2 text-xs flex flex-wrap items-center justify-between gap-2">
                <div>
                  <Link href={`/${locale}/procurement/supplier-invoices/${inv.id}`} className="font-mono font-semibold text-erp-navy underline">
                    {inv.number ?? "-"}
                  </Link>
                  <span className="text-erp-text-muted ml-2">
                    ({inv.supplierInvoiceNumber ?? "-"})
                  </span>
                  <div className="text-erp-text-muted">
                    {t("total")}: <span className="font-mono font-medium">{formatCurrency(inv.totalAmount, "THB", locale)}</span>
                    {Boolean(inv.totalVarianceAmount) && (
                      <span className="text-erp-warning ml-2 font-mono">
                        (var: {formatCurrency(inv.totalVarianceAmount, "THB", locale)})
                      </span>
                    )}
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <StatusBadge
                    label={isSupplierInvoiceStatus(inv.status) ? tInvoice(`statuses.${inv.status}`) : "-"}
                    variant={supplierInvoiceStatusVariant(inv.status)}
                  />
                  {inv.matchStatus && isSupplierInvoiceMatchStatus(inv.matchStatus) && (
                    <StatusBadge
                      label={tInvoice(`matchStatuses.${inv.matchStatus}`)}
                      variant={supplierInvoiceMatchStatusVariant(inv.matchStatus)}
                    />
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
          placeholder={t("reasonPlaceholder")}
          error={noteError ? t("reasonRequired") : undefined}
          onChange={(event) => { setNote(event.target.value); if (event.target.value.trim()) setNoteError(false); }}
        />
      </Modal>

      <Modal
        isOpen={isReturnModalOpen}
        onClose={() => { if (!busy) setIsReturnModalOpen(false); }}
        title={t("recordReturn")}
        description={t("returnReasonPlaceholder")}
        size="md"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setIsReturnModalOpen(false)}>
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              className="min-h-11"
              isLoading={mutations.returnGoods.isPending}
              disabled={busy}
              onClick={() => void postReturn()}
            >
              {tCommon("actions.save")}
            </Button>
          </div>
        )}
      >
        <div className="space-y-4">
          {returnMessage && <Alert variant="danger">{returnMessage}</Alert>}
          <Select
            label={t("returnGoodsReceipt")}
            required
            value={returnReceiptId}
            placeholder={t("selectGoodsReceipt")}
            disabled={busy}
            options={(order.receipts ?? []).map((r) => ({
              value: r.id ?? "",
              label: `${r.number ?? "-"} (${t("receiptSummary", { lines: (r.lines ?? []).length, quantity: formatNumber(r.totalQuantity) })})`,
            }))}
            onChange={(event) => {
              setReturnReceiptId(event.target.value);
              setReturnQuantities({});
            }}
          />
          <Input
            label={t("returnReason")}
            required
            value={returnReason}
            maxLength={500}
            disabled={busy}
            placeholder={t("returnReasonPlaceholder")}
            onChange={(event) => setReturnReason(event.target.value)}
          />
          <div className="space-y-2 border-t border-erp-border pt-3">
            <h4 className="text-xs font-semibold text-erp-navy">{t("linesTitle")}</h4>
            <div className="overflow-x-auto">
              <table className="w-full text-left text-xs">
                <thead>
                  <tr className="border-b border-erp-border">
                    <th className="py-2 pr-2">{t("item")}</th>
                    <th className="py-2 pr-2 text-right">{t("received")}</th>
                    <th className="py-2 pr-2 text-right">{t("returnable")}</th>
                    <th className="py-2 text-right">{t("returnQuantity")}</th>
                  </tr>
                </thead>
                <tbody>
                  {returnableLines.map((line) => (
                    <tr key={line.id} className="border-b border-erp-border/60 align-middle">
                      <td className="py-2 pr-2">
                        {line.itemCode} · {line.itemNameTh} ({line.unitCode})
                      </td>
                      <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.quantity)}</td>
                      <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.remainingReturnableQuantity ?? 0)}</td>
                      <td className="py-2 text-right">
                        <Input
                          type="number"
                          min={0}
                          max={line.remainingReturnableQuantity ?? 0}
                          step="0.0001"
                          aria-label={t("returnQuantityFor", { item: line.itemCode ?? "-" })}
                          placeholder={t("returnQuantityPlaceholder")}
                          disabled={busy || (line.remainingReturnableQuantity ?? 0) <= 0}
                          value={returnQuantities[line.id ?? ""] ?? ""}
                          onChange={(event) =>
                            setReturnQuantities((current) => ({
                              ...current,
                              [line.id ?? ""]: event.target.value,
                            }))
                          }
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </Modal>
    </section>
  );
}
