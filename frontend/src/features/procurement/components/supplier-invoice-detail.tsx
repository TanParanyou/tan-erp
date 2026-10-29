"use client";

import { useState } from "react";
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
import { formatCurrency, formatDate, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { SupplierInvoiceAction, SupplierInvoiceResponse } from "@/lib/api/api-client";
import { useSupplierInvoice, useSupplierInvoiceMutations } from "../api/procurement-queries";
import {
  isSupplierInvoiceMatchStatus,
  isSupplierInvoiceStatus,
  procurementErrorCode,
  supplierInvoiceMatchStatusVariant,
  supplierInvoiceStatusVariant,
} from "../procurement-status";

interface SupplierInvoiceDetailProps {
  supplierInvoiceId: string;
}

type NoteAction = Extract<SupplierInvoiceAction, "reject" | "cancel" | "void">;

export function SupplierInvoiceDetail({ supplierInvoiceId }: SupplierInvoiceDetailProps) {
  const query = useSupplierInvoice(supplierInvoiceId);
  const t = useTranslations("procurement.invoices");

  if (query.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }
  if (query.isError || !query.data) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  return <DetailView key={query.data.rowVersion} invoice={query.data} />;
}

function DetailView({ invoice }: { invoice: SupplierInvoiceResponse }) {
  const t = useTranslations("procurement.invoices");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_CREATE);
  const canApprove = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_APPROVE);
  const mutations = useSupplierInvoiceMutations();

  const status = invoice.status ?? "";
  const lines = invoice.lines ?? [];
  const [noteAction, setNoteAction] = useState<NoteAction | null>(null);
  const [note, setNote] = useState("");
  const [noteError, setNoteError] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const busy = mutations.act.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function run(action: SupplierInvoiceAction, actionNote: string | null): Promise<boolean> {
    setMessage(null);
    try {
      await mutations.act.mutateAsync({
        id: invoice.id ?? "",
        rowVersion: invoice.rowVersion ?? "",
        action,
        note: actionNote,
      });
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

  const supplierName = invoice.supplier
    ? `${invoice.supplier.code ?? "-"} · ${locale === "en" ? invoice.supplier.nameEn ?? invoice.supplier.nameTh : invoice.supplier.nameTh}`
    : "-";

  return (
    <section className="space-y-5">
      <PageHeader
        title={`${invoice.number ?? "-"}`}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[
          { label: t("title"), href: `/${locale}/procurement/supplier-invoices` },
          { label: invoice.number ?? "-" },
        ]}
        actions={
          <div className="flex items-center gap-2">
            <StatusBadge
              label={isSupplierInvoiceStatus(status) ? t(`statuses.${status}`) : "-"}
              variant={supplierInvoiceStatusVariant(status)}
            />
            {invoice.matchStatus && isSupplierInvoiceMatchStatus(invoice.matchStatus) && (
              <StatusBadge
                label={t(`matchStatuses.${invoice.matchStatus}`)}
                variant={supplierInvoiceMatchStatusVariant(invoice.matchStatus)}
              />
            )}
          </div>
        }
      />

      {status === "on_hold" && (
        <Alert variant="warning" title={t("onHoldBannerTitle")}>
          {t("onHoldBannerDetail")}
        </Alert>
      )}

      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card p-5 space-y-4">
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 text-sm">
          <div>
            <dt className="text-xs text-erp-text-muted">{t("supplierInvoiceNumber")}</dt>
            <dd className="font-medium font-mono">{invoice.supplierInvoiceNumber ?? "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("purchaseOrder")}</dt>
            <dd className="font-medium">
              {invoice.purchaseOrder?.id ? (
                <Link
                  href={`/${locale}/procurement/purchase-orders/${invoice.purchaseOrder.id}`}
                  className="font-mono text-erp-navy underline"
                >
                  {invoice.purchaseOrder.number ?? "-"}
                </Link>
              ) : (
                "-"
              )}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("supplier")}</dt>
            <dd className="font-medium">{supplierName}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("project")}</dt>
            <dd className="font-medium">
              {invoice.project?.id ? (
                <Link href={`/${locale}/projects/${invoice.project.id}`} className="font-mono text-erp-navy underline">
                  {invoice.project.code}
                </Link>
              ) : (
                "-"
              )}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("invoiceDate")}</dt>
            <dd className="font-medium">{invoice.invoiceDate ? formatDate(invoice.invoiceDate, locale) : "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("dueDate")}</dt>
            <dd className="font-medium">{invoice.dueDate ? formatDate(invoice.dueDate, locale) : "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("subtotal")}</dt>
            <dd className="font-mono font-medium">{formatCurrency(invoice.subtotalAmount, "THB", locale)}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("tax")}</dt>
            <dd className="font-mono font-medium">{formatCurrency(invoice.taxAmount, "THB", locale)}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("total")}</dt>
            <dd className="font-mono font-semibold text-erp-navy">{formatCurrency(invoice.totalAmount, "THB", locale)}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("poTotal")}</dt>
            <dd className="font-mono font-medium">{formatCurrency(invoice.totalPoAmount, "THB", locale)}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("variance")}</dt>
            <dd className={`font-mono font-semibold ${Boolean(invoice.totalVarianceAmount) ? "text-erp-warning" : ""}`}>
              {formatCurrency(invoice.totalVarianceAmount, "THB", locale)}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt>
            <dd className="font-medium">{invoice.createdBy?.displayName ?? "-"}</dd>
          </div>
          {invoice.decidedBy && (
            <div>
              <dt className="text-xs text-erp-text-muted">{t("decidedBy")}</dt>
              <dd className="font-medium">{invoice.decidedBy.displayName ?? "-"}</dd>
            </div>
          )}
          {invoice.voidedBy && (
            <div>
              <dt className="text-xs text-erp-text-muted">{t("voidedBy")}</dt>
              <dd className="font-medium">{invoice.voidedBy.displayName ?? "-"}</dd>
            </div>
          )}
        </dl>

        {invoice.note && <p className="text-xs text-erp-text-body">{t("noteLine", { note: invoice.note })}</p>}
        {invoice.decisionNote && <p className="text-xs text-erp-text-muted">{t("decisionNote", { note: invoice.decisionNote })}</p>}
        {invoice.cancelReason && <p className="text-xs text-erp-text-muted">{t("cancelReason", { reason: invoice.cancelReason })}</p>}
        {invoice.voidReason && <p className="text-xs text-erp-danger font-medium">{t("voidReason", { reason: invoice.voidReason })}</p>}

        <div className="flex flex-wrap gap-2">
          {status === "draft" && canCreate && (
            <>
              <Button href={`/${locale}/procurement/supplier-invoices/${invoice.id}/edit`} variant="outline" className="min-h-11">
                {t("editDraft")}
              </Button>
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run("submit", null)}>
                {t("submit")}
              </Button>
              <Button
                type="button"
                variant="outline"
                className="min-h-11"
                disabled={busy}
                onClick={() => {
                  setNote("");
                  setNoteError(false);
                  setNoteAction("cancel");
                }}
              >
                {t("cancel")}
              </Button>
            </>
          )}

          {status === "on_hold" && canApprove && (
            <>
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run("approve", null)}>
                {t("approve")}
              </Button>
              <Button
                type="button"
                variant="danger"
                className="min-h-11"
                disabled={busy}
                onClick={() => {
                  setNote("");
                  setNoteError(false);
                  setNoteAction("reject");
                }}
              >
                {t("reject")}
              </Button>
            </>
          )}

          {["matched", "approved"].includes(status) && canCreate && (
            <Button
              type="button"
              variant="danger"
              className="min-h-11"
              disabled={busy}
              onClick={() => {
                setNote("");
                setNoteError(false);
                setNoteAction("void");
              }}
            >
              {t("voidAction")}
            </Button>
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
                <th className="py-2 pr-2 text-right">{t("poUnitPrice")}</th>
                <th className="py-2 pr-2 text-right">{t("unitPrice")}</th>
                <th className="py-2 pr-2 text-right">{t("lineTotal")}</th>
                <th className="py-2 pr-2 text-right">{t("priceVarianceAmount")}</th>
                <th className="py-2 pr-2 text-right">{t("priceVariancePercent")}</th>
                <th className="py-2 text-center">{t("status")}</th>
              </tr>
            </thead>
            <tbody>
              {lines.map((line) => (
                <tr key={line.id} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2 pr-2 font-mono">{line.lineNo}</td>
                  <td className="py-2 pr-2">
                    {line.itemCode} · {line.itemNameTh} ({line.unitCode})
                  </td>
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.quantity)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatCurrency(line.poUnitPrice, "THB", locale)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatCurrency(line.unitPrice, "THB", locale)}</td>
                  <td className="py-2 pr-2 text-right font-mono">{formatCurrency(line.lineTotal, "THB", locale)}</td>
                  <td className={`py-2 pr-2 text-right font-mono ${Boolean(line.priceVarianceAmount) ? "text-erp-warning" : ""}`}>
                    {formatCurrency(line.priceVarianceAmount, "THB", locale)}
                  </td>
                  <td className={`py-2 pr-2 text-right font-mono ${Boolean(line.priceVariancePercent) ? "text-erp-warning" : ""}`}>
                    {line.priceVariancePercent ? `${line.priceVariancePercent > 0 ? "+" : ""}${line.priceVariancePercent.toFixed(1)}%` : "0.0%"}
                  </td>
                  <td className="py-2 text-center">
                    {line.exceedsTolerance ? (
                      <StatusBadge label={t("exceedsTolerance")} variant="warning" />
                    ) : (
                      <StatusBadge label={t("withinTolerance")} variant="success" />
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal
        isOpen={noteAction !== null}
        onClose={() => { if (!busy) setNoteAction(null); }}
        title={noteAction ? t(noteAction === "reject" ? "reject" : noteAction === "cancel" ? "cancel" : "voidAction") : undefined}
        description={
          noteAction
            ? t(
                noteAction === "reject"
                  ? "rejectDescription"
                  : noteAction === "cancel"
                  ? "cancelDescription"
                  : "voidDescription",
              )
            : undefined
        }
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setNoteAction(null)}>
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="danger"
              className="min-h-11"
              isLoading={mutations.act.isPending}
              disabled={busy}
              onClick={() => void confirmNoteAction()}
            >
              {t("confirm")}
            </Button>
          </div>
        }
      >
        <Input
          label={t("reason")}
          required
          value={note}
          maxLength={500}
          disabled={busy}
          placeholder={t("reasonPlaceholder")}
          error={noteError ? t("reasonRequired") : undefined}
          onChange={(event) => {
            setNote(event.target.value);
            if (event.target.value.trim()) setNoteError(false);
          }}
        />
      </Modal>
    </section>
  );
}
