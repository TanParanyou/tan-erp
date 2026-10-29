"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatNumber } from "@/lib/formatters/formatters";
import type {
  PurchaseOrderResponse,
  SupplierInvoiceRequest,
  SupplierInvoiceResponse,
} from "@/lib/api/api-client";
import {
  usePurchaseOrder,
  usePurchaseOrderList,
  useSupplierInvoice,
  useSupplierInvoiceMutations,
} from "../api/procurement-queries";
import { procurementErrorCode } from "../procurement-status";

interface InvoiceDraftLine {
  key: string;
  purchaseOrderLineId: string;
  itemLabel: string;
  poUnitPrice: number;
  availableQuantity: number;
  quantity: string;
  unitPrice: string;
}

interface SupplierInvoiceEditorProps {
  /** Existing draft to edit; omit to create a new invoice. */
  supplierInvoiceId?: string;
}

function fromInvoice(inv: SupplierInvoiceResponse): InvoiceDraftLine[] {
  return (inv.lines ?? []).map((line) => ({
    key: line.id ?? crypto.randomUUID(),
    purchaseOrderLineId: line.purchaseOrderLineId ?? "",
    itemLabel: `${line.itemCode ?? "-"} · ${line.itemNameTh ?? "-"} (${line.unitCode ?? "-"})`,
    poUnitPrice: line.poUnitPrice ?? 0,
    availableQuantity: line.quantity ?? 0,
    quantity: String(line.quantity ?? 1),
    unitPrice: String(line.unitPrice ?? 0),
  }));
}

export function SupplierInvoiceEditor({ supplierInvoiceId }: SupplierInvoiceEditorProps) {
  const invoiceQuery = useSupplierInvoice(supplierInvoiceId);
  const t = useTranslations("procurement.invoices");

  if (supplierInvoiceId && invoiceQuery.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }

  if (supplierInvoiceId && (invoiceQuery.isError || !invoiceQuery.data)) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  if (supplierInvoiceId && invoiceQuery.data?.status !== "draft") {
    return <Alert variant="warning">{t("onlyDraftEditable")}</Alert>;
  }

  return <EditorForm key={invoiceQuery.data?.rowVersion ?? "new"} invoice={invoiceQuery.data} />;
}

function EditorForm({ invoice }: { invoice?: SupplierInvoiceResponse }) {
  const t = useTranslations("procurement.invoices");
  const tOrders = useTranslations("procurement.orders");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const searchParams = useSearchParams();
  const { toast } = useToast();
  const mutations = useSupplierInvoiceMutations();

  const initialPoId = invoice?.purchaseOrder?.id ?? searchParams.get("purchaseOrderId") ?? "";
  const [purchaseOrderId, setPurchaseOrderId] = useState(initialPoId);

  // Load eligible POs
  const ordersQuery = usePurchaseOrderList({ page: 1, pageSize: 100 });
  const selectedPoQuery = usePurchaseOrder(purchaseOrderId || undefined);

  const [supplierInvoiceNumber, setSupplierInvoiceNumber] = useState(invoice?.supplierInvoiceNumber ?? "");
  const [invoiceDate, setInvoiceDate] = useState(invoice?.invoiceDate ?? new Date().toISOString().split("T")[0]);
  const [dueDate, setDueDate] = useState(invoice?.dueDate ?? "");
  const [taxAmount, setTaxAmount] = useState(String(invoice?.taxAmount ?? "0"));
  const [taxCustomized, setTaxCustomized] = useState(Boolean(invoice));
  const [note, setNote] = useState(invoice?.note ?? "");
  const [lines, setLines] = useState<InvoiceDraftLine[]>(invoice ? fromInvoice(invoice) : []);
  const [error, setError] = useState<string | null>(null);
  const createKeyRef = useRef(crypto.randomUUID());

  // Automatically fill lines from PO lines that have received items
  useEffect(() => {
    if (invoice || !selectedPoQuery.data) return;
    const po: PurchaseOrderResponse = selectedPoQuery.data;
    const poLines = po.lines ?? [];
    // Prefer lines that have uninvoicedReceivedQuantity > 0, or fallback to remaining lines
    const candidateLines = poLines.filter((l) => (l.uninvoicedReceivedQuantity ?? 0) > 0);
    const linesToUse = candidateLines.length > 0 ? candidateLines : poLines;

    const draftLines: InvoiceDraftLine[] = linesToUse.map((l) => ({
      key: l.id ?? crypto.randomUUID(),
      purchaseOrderLineId: l.id ?? "",
      itemLabel: `${l.itemCode ?? "-"} · ${l.itemNameTh ?? "-"} (${l.unitCode ?? "-"})`,
      poUnitPrice: l.unitPrice ?? 0,
      availableQuantity: l.uninvoicedReceivedQuantity ?? l.receivedQuantity ?? l.quantity ?? 0,
      quantity: String(l.uninvoicedReceivedQuantity ?? l.receivedQuantity ?? l.quantity ?? 1),
      unitPrice: String(l.unitPrice ?? 0),
    }));

    setLines(draftLines);
  }, [invoice, selectedPoQuery.data]);

  const subtotal = lines.reduce(
    (sum, line) => sum + (Number(line.quantity) || 0) * (Number(line.unitPrice) || 0),
    0,
  );

  // Auto calculate 7% VAT if not customized by user
  useEffect(() => {
    if (!taxCustomized && !invoice) {
      const calcVat = Math.round(subtotal * 0.07 * 100) / 100;
      setTaxAmount(String(calcVat));
    }
  }, [subtotal, taxCustomized, invoice]);

  const tax = Number(taxAmount) || 0;
  const total = subtotal + tax;

  const isBusy = mutations.create.isPending || mutations.update.isPending;

  function updateLine(key: string, patch: Partial<InvoiceDraftLine>): void {
    setLines((current) => current.map((l) => (l.key === key ? { ...l, ...patch } : l)));
  }

  async function save(): Promise<void> {
    setError(null);
    if (!purchaseOrderId) {
      setError(t("purchaseOrderRequired"));
      return;
    }
    if (!supplierInvoiceNumber.trim()) {
      setError(t("invoiceNumberRequired"));
      return;
    }
    if (!invoiceDate) {
      setError(t("invoiceDateRequired"));
      return;
    }
    if (!dueDate) {
      setError(t("dueDateRequired"));
      return;
    }

    const parsedLines = lines.map((l) => ({
      purchaseOrderLineId: l.purchaseOrderLineId,
      quantity: Number(l.quantity),
      unitPrice: Number(l.unitPrice),
    }));

    if (
      parsedLines.length === 0 ||
      parsedLines.some((l) => !(l.quantity > 0) || !(l.unitPrice >= 0) || !Number.isFinite(l.quantity) || !Number.isFinite(l.unitPrice))
    ) {
      setError(t("linesInvalid"));
      return;
    }

    const payload: SupplierInvoiceRequest = {
      purchaseOrderId,
      supplierInvoiceNumber: supplierInvoiceNumber.trim(),
      invoiceDate,
      dueDate,
      taxAmount: tax,
      note: note.trim() || null,
      lines: parsedLines,
    };

    try {
      const saved = invoice
        ? await mutations.update.mutateAsync({ id: invoice.id ?? "", rowVersion: invoice.rowVersion ?? "", payload })
        : await mutations.create.mutateAsync({ payload, idempotencyKey: createKeyRef.current });
      toast.success(invoice ? t("updated") : t("created"));
      router.push(`/${locale}/procurement/supplier-invoices/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={invoice ? t("editTitle", { number: invoice.number ?? "-" }) : t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[
          { label: t("title"), href: `/${locale}/procurement/supplier-invoices` },
          { label: invoice?.number ?? t("create") },
        ]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <div className="erp-card p-5 grid grid-cols-1 gap-4 md:grid-cols-2">
        <Select
          label={t("purchaseOrder")}
          required
          value={purchaseOrderId}
          placeholder={t("selectPurchaseOrder")}
          disabled={isBusy || Boolean(invoice)}
          options={(ordersQuery.data?.items ?? []).map((po) => ({
            value: po.id ?? "",
            label: `${po.number ?? "-"} · ${po.supplierNameTh ?? "-"} (${formatCurrency(po.totalAmount, "THB", locale)})`,
          }))}
          onChange={(event) => {
            setPurchaseOrderId(event.target.value);
            setLines([]);
          }}
        />

        <Input
          label={t("supplierInvoiceNumber")}
          required
          value={supplierInvoiceNumber}
          maxLength={100}
          disabled={isBusy}
          placeholder={t("invoiceNumberPlaceholder")}
          onChange={(event) => setSupplierInvoiceNumber(event.target.value)}
        />

        <Input
          type="date"
          label={t("invoiceDate")}
          required
          placeholder={t("datePlaceholder")}
          value={invoiceDate}
          disabled={isBusy}
          onChange={(event) => setInvoiceDate(event.target.value)}
        />

        <Input
          type="date"
          label={t("dueDate")}
          required
          placeholder={t("datePlaceholder")}
          value={dueDate}
          disabled={isBusy}
          onChange={(event) => setDueDate(event.target.value)}
        />

        <Input
          type="number"
          step="0.01"
          min={0}
          label={t("tax")}
          value={taxAmount}
          disabled={isBusy}
          placeholder={t("taxAmountPlaceholder")}
          onChange={(event) => {
            setTaxAmount(event.target.value);
            setTaxCustomized(true);
          }}
        />

        <Input
          label={t("note")}
          value={note}
          maxLength={500}
          disabled={isBusy}
          placeholder={t("notePlaceholder")}
          onChange={(event) => setNote(event.target.value)}
        />
      </div>

      <div className="erp-card p-5 space-y-4">
        <h3 className="text-sm font-bold text-erp-navy">{t("linesTitle")}</h3>
        {selectedPoQuery.isLoading && (
          <div className="py-8 flex justify-center"><MonoSpinner size="md" /></div>
        )}

        <ul className="space-y-3">
          {lines.map((line) => {
            const qty = Number(line.quantity) || 0;
            const price = Number(line.unitPrice) || 0;
            const diff = price - line.poUnitPrice;
            const varianceAmount = diff * qty;
            const variancePercent = line.poUnitPrice > 0 ? (diff / line.poUnitPrice) * 100 : 0;
            const exceedsTolerance = Math.abs(variancePercent) > 2;

            return (
              <li
                key={line.key}
                className="border border-erp-border bg-erp-surface-subtle p-3 space-y-2"
              >
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <span className="text-sm font-medium text-erp-text-main">{line.itemLabel}</span>
                  <div className="flex items-center gap-2">
                    <span className="text-xs text-erp-text-muted">
                      {t("availableUninvoiced", { quantity: formatNumber(line.availableQuantity) })}
                    </span>
                    {Math.abs(diff) > 0.001 ? (
                      <StatusBadge
                        label={exceedsTolerance ? t("exceedsTolerance") : t("withinTolerance")}
                        variant={exceedsTolerance ? "warning" : "info"}
                      />
                    ) : (
                      <StatusBadge label={t("matchStatuses.matched")} variant="success" />
                    )}
                  </div>
                </div>

                <div className="grid grid-cols-1 gap-2 sm:grid-cols-4 sm:items-end">
                  <Input
                    type="number"
                    min={0}
                    step="0.0001"
                    label={t("quantity")}
                    placeholder={t("quantityPlaceholder")}
                    value={line.quantity}
                    disabled={isBusy}
                    onChange={(event) => updateLine(line.key, { quantity: event.target.value })}
                  />
                  <div>
                    <span className="text-xs text-erp-text-muted block mb-1.5">{t("poUnitPrice")}</span>
                    <div className="h-11 min-h-[44px] flex items-center px-3 border border-erp-border bg-erp-surface-disabled font-mono text-sm">
                      {formatCurrency(line.poUnitPrice, "THB", locale)}
                    </div>
                  </div>
                  <Input
                    type="number"
                    min={0}
                    step="0.01"
                    label={t("unitPrice")}
                    placeholder={t("unitPricePlaceholder")}
                    value={line.unitPrice}
                    disabled={isBusy}
                    onChange={(event) => updateLine(line.key, { unitPrice: event.target.value })}
                  />
                  <div>
                    <span className="text-xs text-erp-text-muted block mb-1.5">{t("lineTotal")}</span>
                    <div className="h-11 min-h-[44px] flex items-center justify-between px-3 border border-erp-border bg-erp-surface font-mono text-sm">
                      <span>{formatCurrency(qty * price, "THB", locale)}</span>
                      {Math.abs(diff) > 0.001 && (
                        <span className={`text-xs ${exceedsTolerance ? "text-erp-danger font-semibold" : "text-erp-warning"}`}>
                          ({variancePercent > 0 ? "+" : ""}{variancePercent.toFixed(1)}%)
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </li>
            );
          })}
          {lines.length === 0 && !selectedPoQuery.isLoading && (
            <li className="text-xs text-erp-text-muted">{t("linesEmpty")}</li>
          )}
        </ul>
      </div>

      <div className="erp-card p-5 space-y-2">
        <div className="flex justify-between text-sm">
          <span className="text-erp-text-muted">{t("subtotal")}</span>
          <span className="font-mono font-medium">{formatCurrency(subtotal, "THB", locale)}</span>
        </div>
        <div className="flex justify-between text-sm">
          <span className="text-erp-text-muted">{t("tax")}</span>
          <span className="font-mono font-medium">{formatCurrency(tax, "THB", locale)}</span>
        </div>
        <div className="flex justify-between text-base font-bold border-t border-erp-border pt-2">
          <span className="text-erp-navy">{t("total")}</span>
          <span className="font-mono text-erp-navy">{formatCurrency(total, "THB", locale)}</span>
        </div>
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <span className="font-mono text-sm font-semibold text-erp-navy">
          {t("draftTotal", { amount: formatCurrency(total, "THB", locale) })}
        </span>
        <div className="flex gap-3">
          <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>
            {tCommon("actions.cancel")}
          </Button>
          <Button
            type="button"
            variant="primary"
            className="min-h-11"
            isLoading={isBusy}
            disabled={isBusy || lines.length === 0}
            onClick={() => void save()}
          >
            {tCommon("actions.save")}
          </Button>
        </div>
      </div>
    </section>
  );
}
