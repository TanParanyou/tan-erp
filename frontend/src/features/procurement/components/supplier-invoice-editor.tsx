"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { IconTrash } from "@/components/common/Icons";
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
  itemCode: string;
  itemName: string;
  unitCode: string;
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
    itemCode: line.itemCode ?? "-",
    itemName: line.itemNameTh ?? "-",
    unitCode: line.unitCode ?? "-",
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
      itemCode: l.itemCode ?? "-",
      itemName: l.itemNameTh ?? "-",
      unitCode: l.unitCode ?? "-",
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
  const isEdit = Boolean(invoice);

  const isDirty = useMemo(() => {
    if (!invoice) {
      return Boolean(purchaseOrderId || supplierInvoiceNumber || note || lines.length > 0);
    }
    const origPoId = invoice.purchaseOrder?.id ?? "";
    const origNumber = invoice.supplierInvoiceNumber ?? "";
    const origInvoiceDate = invoice.invoiceDate ?? "";
    const origDueDate = invoice.dueDate ?? "";
    const origNote = invoice.note ?? "";
    const origTax = String(invoice.taxAmount ?? "0");
    const origLines = fromInvoice(invoice);

    if (
      purchaseOrderId !== origPoId ||
      supplierInvoiceNumber !== origNumber ||
      invoiceDate !== origInvoiceDate ||
      dueDate !== origDueDate ||
      taxAmount !== origTax ||
      note !== origNote
    ) {
      return true;
    }
    if (lines.length !== origLines.length) return true;
    return lines.some((line, index) => {
      const orig = origLines[index];
      return (
        line.purchaseOrderLineId !== orig.purchaseOrderLineId ||
        line.quantity !== orig.quantity ||
        line.unitPrice !== orig.unitPrice
      );
    });
  }, [invoice, purchaseOrderId, supplierInvoiceNumber, invoiceDate, dueDate, taxAmount, note, lines]);

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
    <section className="space-y-6 pb-20">
      <PageHeader
        title={invoice ? t("editTitle", { number: invoice.number ?? "-" }) : t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[
          { label: t("title"), href: `/${locale}/procurement/supplier-invoices` },
          { label: invoice?.number ?? t("create") },
        ]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <FormSection title={t("title")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
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
      </FormSection>

      <FormSection
        title={t("linesTitle")}
        headerAction={
          <div className="flex items-center gap-3 font-mono text-xs">
            <span className="text-erp-text-muted">{t("subtotal")}: {formatCurrency(subtotal, "THB", locale)}</span>
            <span className="text-erp-text-muted">+ {t("tax")}: {formatCurrency(tax, "THB", locale)}</span>
            <span className="font-bold text-erp-navy text-sm">= {formatCurrency(total, "THB", locale)}</span>
          </div>
        }
      >
        {selectedPoQuery.isLoading && (
          <div className="py-8 flex justify-center"><MonoSpinner size="md" /></div>
        )}

        <div className="overflow-x-auto border border-erp-border bg-erp-surface">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-erp-border bg-erp-surface-subtle text-erp-navy font-bold">
                <th className="py-2.5 px-3 w-12 text-center">#</th>
                <th className="py-2.5 px-3 min-w-[200px]">{t("linesTitle")}</th>
                <th className="py-2.5 px-3 w-32">{t("quantity")}</th>
                <th className="py-2.5 px-3 w-36">{t("poUnitPrice")}</th>
                <th className="py-2.5 px-3 w-36">{t("unitPrice")}</th>
                <th className="py-2.5 px-3 w-36 text-right">{t("lineTotal")}</th>
                <th className="py-2.5 px-3 w-36 text-center">{t("matchStatus")}</th>
                <th className="py-2.5 px-3 w-14 text-center">
                  <span className="sr-only">{tCommon("actions.delete")}</span>
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-erp-border/60">
              {lines.map((line, index) => {
                const qty = Number(line.quantity) || 0;
                const price = Number(line.unitPrice) || 0;
                const diff = price - line.poUnitPrice;
                const variancePercent = line.poUnitPrice > 0 ? (diff / line.poUnitPrice) * 100 : 0;
                const exceedsTolerance = Math.abs(variancePercent) > 2;

                return (
                  <tr key={line.key} className="align-middle hover:bg-erp-surface-subtle/50 transition-colors">
                    <td className="py-2 px-3 text-center font-mono text-erp-text-muted">
                      {index + 1}
                    </td>
                    <td className="py-2 px-3">
                      <div className="text-sm font-medium text-erp-text-main">
                        <span className="font-mono text-erp-navy font-semibold">{line.itemCode}</span> · {line.itemName}
                      </div>
                      <div className="text-xs text-erp-text-muted">
                        {line.unitCode} · {t("availableUninvoiced", { quantity: formatNumber(line.availableQuantity) })}
                      </div>
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        type="number"
                        min={0}
                        step="0.0001"
                        aria-label={t("quantity")}
                        placeholder={t("quantityPlaceholder")}
                        value={line.quantity}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px] font-mono text-right"
                        onChange={(event) => updateLine(line.key, { quantity: event.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      <div className="h-9 min-h-[36px] flex items-center justify-end px-3 border border-erp-border bg-erp-surface-disabled font-mono text-xs text-erp-text-muted">
                        {formatCurrency(line.poUnitPrice, "THB", locale)}
                      </div>
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        type="number"
                        min={0}
                        step="0.01"
                        aria-label={t("unitPrice")}
                        placeholder={t("unitPricePlaceholder")}
                        value={line.unitPrice}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px] font-mono text-right"
                        onChange={(event) => updateLine(line.key, { unitPrice: event.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3 text-right">
                      <div className="font-mono font-semibold text-erp-navy text-sm">
                        {formatCurrency(qty * price, "THB", locale)}
                      </div>
                      {Math.abs(diff) > 0.001 && (
                        <div className={`text-[11px] font-mono ${exceedsTolerance ? "text-erp-danger font-semibold" : "text-erp-warning"}`}>
                          ({variancePercent > 0 ? "+" : ""}{variancePercent.toFixed(1)}%)
                        </div>
                      )}
                    </td>
                    <td className="py-2 px-3 text-center">
                      {Math.abs(diff) > 0.001 ? (
                        <StatusBadge
                          label={exceedsTolerance ? t("exceedsTolerance") : t("withinTolerance")}
                          variant={exceedsTolerance ? "warning" : "info"}
                        />
                      ) : (
                        <StatusBadge label={t("matchStatuses.matched")} variant="success" />
                      )}
                    </td>
                    <td className="py-2 px-3 text-center">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="h-9 w-9 text-erp-danger hover:text-erp-danger hover:bg-erp-danger/10"
                        disabled={isBusy}
                        title={tCommon("actions.delete")}
                        aria-label={tCommon("actions.delete")}
                        icon={<IconTrash size={16} />}
                        onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}
                      />
                    </td>
                  </tr>
                );
              })}
              {lines.length === 0 && !selectedPoQuery.isLoading && (
                <tr>
                  <td colSpan={8} className="py-8 text-center text-sm text-erp-text-muted">
                    {t("linesEmpty")}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </FormSection>

      <FormActionBar
        isEditMode={isEdit}
        isDirty={isDirty}
        isLoading={isBusy}
        isSaveDisabled={lines.length === 0}
        sticky
        saveButtonType="button"
        onSave={() => void save()}
        onCancel={() => router.back()}
        statusContent={
          <span className="font-mono text-sm font-semibold text-erp-navy">
            {t("draftTotal", { amount: formatCurrency(total, "THB", locale) })}
          </span>
        }
      />
    </section>
  );
}
