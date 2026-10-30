"use client";

import { useMemo, useRef, useState } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { BillingAdjustmentNoteResponse, BillingResponse, PaymentResponse } from "@/lib/api/api-client";
import { useBilling, useBillingMutations } from "../api/finance-queries";
import {
  PAYMENT_METHODS,
  billingNoteStatusVariant,
  billingStatusVariant,
  financeErrorCode,
  isBillingKind,
  isBillingStatus,
  isPaymentMethod,
  retentionStatusVariant,
} from "../finance-status";

interface BillingDetailProps {
  billingId: string;
}

type Dialog =
  | { kind: "void" }
  | { kind: "reverse"; payment: PaymentResponse }
  | { kind: "releaseRetention" }
  | { kind: "approveNote"; note: BillingAdjustmentNoteResponse };

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
}

function round2(val: number): number {
  return Math.round(val * 100) / 100;
}

export function BillingDetail({ billingId }: BillingDetailProps) {
  const query = useBilling(billingId);
  const t = useTranslations("finance.billings");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;

  // Keyed by row version so the payment form resets to the new outstanding amount after every change.
  return <DetailView key={query.data.rowVersion} billing={query.data} />;
}

function DetailView({ billing }: { billing: BillingResponse }) {
  const t = useTranslations("finance.billings");
  const tErrors = useTranslations("finance.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { currentUser, selectedMembership } = useSelectedMembership();
  const currentUserId = currentUser?.user?.id;
  const canManage = can(selectedMembership, PERMISSIONS.BILLINGS_MANAGE);
  const canPay = can(selectedMembership, PERMISSIONS.PAYMENTS_MANAGE);
  const mutations = useBillingMutations();

  const status = billing.status ?? "";
  const busy =
    mutations.pay.isPending ||
    mutations.voidBilling.isPending ||
    mutations.reverse.isPending ||
    mutations.releaseRetention.isPending ||
    mutations.createNote.isPending ||
    mutations.approveNote.isPending ||
    mutations.downloadTaxPdf.isPending;

  const [message, setMessage] = useState<string | null>(null);

  // Payment form state
  const [amount, setAmount] = useState("");
  const [method, setMethod] = useState("transfer");
  const [reference, setReference] = useState("");
  const [received, setReceived] = useState(todayIso);
  const [whtRate, setWhtRate] = useState("0");
  const [whtType, setWhtType] = useState("service_3");
  const [whtCertNumber, setWhtCertNumber] = useState("");

  // Credit / Debit Note form state
  const [noteType, setNoteType] = useState<"credit" | "debit">("credit");
  const [noteTaxBaseDelta, setNoteTaxBaseDelta] = useState("");
  const [noteReason, setNoteReason] = useState("");

  // Modal states
  const [dialog, setDialog] = useState<Dialog | null>(null);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);

  // Idempotency keys
  const payKeyRef = useRef(crypto.randomUUID());
  const noteKeyRef = useRef(crypto.randomUUID());

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? financeErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  // Live WHT calculation
  const paymentCalc = useMemo(() => {
    const payAmt = Number(amount) > 0 ? Number(amount) : 0;
    const rate = Number(whtRate) > 0 ? Number(whtRate) : 0;
    const whtAmt = round2(payAmt * rate);
    const netCash = round2(payAmt - whtAmt);
    return { payAmt, rate, whtAmt, netCash };
  }, [amount, whtRate]);

  // Live Note calculation
  const noteCalc = useMemo(() => {
    const base = Number(noteTaxBaseDelta) > 0 ? Number(noteTaxBaseDelta) : 0;
    const vatR = billing.vatRate ?? 0;
    const vatD = round2(base * vatR);
    const totalD = round2(base + vatD);
    return { base, vatD, totalD };
  }, [noteTaxBaseDelta, billing.vatRate]);

  async function handleDownloadTaxPdf(): Promise<void> {
    try {
      const blob = await mutations.downloadTaxPdf.mutateAsync({ id: billing.id ?? "" });
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `${billing.taxInvoiceNumber ?? billing.number ?? "tax-document"}.pdf`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
      toast.success(t("pdfDownloaded"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function pay(): Promise<void> {
    setMessage(null);
    const value = Number(amount);
    if (!(value > 0) || Math.round(value * 100) / 100 !== value || !reference.trim() || !received) {
      setMessage(t("paymentInvalid"));
      return;
    }

    const rate = Number(whtRate);
    if (rate > 0 && !whtCertNumber.trim()) {
      setMessage(t("whtCertificateRequired"));
      return;
    }

    try {
      await mutations.pay.mutateAsync({
        id: billing.id ?? "",
        payload: {
          amount: value,
          method,
          reference: reference.trim(),
          receivedDate: received,
          whtRate: rate > 0 ? rate : 0,
          whtType: rate > 0 ? (whtType || null) : null,
          whtCertificateNumber: rate > 0 ? whtCertNumber.trim() : null,
        },
        idempotencyKey: payKeyRef.current,
      });
      payKeyRef.current = crypto.randomUUID();
      toast.success(t("paymentRecorded"));
      setAmount("");
      setReference("");
      setWhtRate("0");
      setWhtCertNumber("");
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function handleCreateNote(): Promise<void> {
    setMessage(null);
    const delta = Number(noteTaxBaseDelta);
    if (!(delta > 0) || Math.round(delta * 100) / 100 !== delta || !noteReason.trim()) {
      setMessage(t("noteInvalid"));
      return;
    }

    try {
      await mutations.createNote.mutateAsync({
        id: billing.id ?? "",
        payload: {
          noteType,
          taxBaseDelta: delta,
          reason: noteReason.trim(),
        },
        idempotencyKey: noteKeyRef.current,
      });
      noteKeyRef.current = crypto.randomUUID();
      setNoteTaxBaseDelta("");
      setNoteReason("");
      toast.success(t("noteCreated"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function handleApproveNoteConfirm(note: BillingAdjustmentNoteResponse): Promise<void> {
    setMessage(null);
    try {
      await mutations.approveNote.mutateAsync({
        id: billing.id ?? "",
        noteId: note.id ?? "",
        rowVersion: billing.rowVersion ?? "",
      });
      toast.success(t("noteApproved"));
      setDialog(null);
    } catch (err: unknown) {
      setDialog(null);
      setMessage(describe(err));
    }
  }

  async function handleReleaseRetentionConfirm(): Promise<void> {
    setMessage(null);
    try {
      await mutations.releaseRetention.mutateAsync({
        id: billing.id ?? "",
        rowVersion: billing.rowVersion ?? "",
      });
      toast.success(t("retentionReleased"));
      setDialog(null);
    } catch (err: unknown) {
      setDialog(null);
      setMessage(describe(err));
    }
  }

  async function confirmVoidOrReverse(): Promise<void> {
    if (!dialog) return;
    if (dialog.kind !== "void" && dialog.kind !== "reverse") return;
    if (!reason.trim()) {
      setReasonError(true);
      return;
    }

    setMessage(null);
    try {
      if (dialog.kind === "void") {
        await mutations.voidBilling.mutateAsync({ id: billing.id ?? "", rowVersion: billing.rowVersion ?? "", reason: reason.trim() });
        toast.success(t("voided"));
      } else {
        await mutations.reverse.mutateAsync({ id: billing.id ?? "", paymentId: dialog.payment.id ?? "", rowVersion: billing.rowVersion ?? "", reason: reason.trim() });
        toast.success(t("reversed"));
      }

      setDialog(null);
    } catch (err: unknown) {
      setDialog(null);
      setMessage(describe(err));
    }
  }

  function open(next: Dialog): void {
    setDialog(next);
    setReason("");
    setReasonError(false);
  }

  const canReceive = canPay && (status === "issued" || status === "partially_paid");
  const hasRetention = (billing.retentionRate ?? 0) > 0 || (billing.retentionStatus && billing.retentionStatus !== "none");
  const canReleaseRetention = canManage && billing.retentionStatus === "held" && status !== "voided";

  return (
    <section className="space-y-5">
      <PageHeader
        title={billing.number ?? "-"}
        subtitle={billing.description ?? "-"}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/finance/billings` }, { label: billing.number ?? "-" }]}
        actions={
          <div className="flex flex-wrap items-center gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="min-h-11"
              isLoading={mutations.downloadTaxPdf.isPending}
              disabled={busy}
              onClick={() => void handleDownloadTaxPdf()}
            >
              {t("downloadTaxPdf")}
            </Button>
            <StatusBadge label={isBillingStatus(status) ? t(`statuses.${status}`) : "-"} variant={billingStatusVariant(status)} />
          </div>
        }
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      {/* Main Billing Card */}
      <div className="erp-card space-y-4 p-5">
        <dl className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2 lg:grid-cols-4">
          <div><dt className="text-xs text-erp-text-muted">{t("project")}</dt><dd className="font-medium">{billing.project?.id ? <Link href={`/${locale}/projects/${billing.project.id}`} className="font-mono text-erp-navy underline">{billing.project.code}</Link> : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("kind")}</dt><dd className="font-medium">{isBillingKind(billing.kind) ? t(`kinds.${billing.kind}`) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("dueDate")}</dt><dd className="font-medium">{billing.dueDate ? formatDate(billing.dueDate, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{billing.createdBy?.displayName ?? "-"}</dd></div>

          <div><dt className="text-xs text-erp-text-muted">{t("taxBase")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.amount, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("paid")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.paidAmount, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("outstanding")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.outstanding, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("referenceHash")}</dt><dd className="font-mono text-xs" title={billing.referenceHash ?? undefined}>{(billing.referenceHash ?? "").slice(0, 16)}</dd></div>

          {/* Thai Tax G-15 fields */}
          {billing.taxInvoiceNumber && (
            <div><dt className="text-xs text-erp-text-muted">{t("taxInvoiceNumber")}</dt><dd className="font-mono font-semibold text-erp-navy">{billing.taxInvoiceNumber}</dd></div>
          )}
          {(billing.vatRate ?? 0) > 0 && (
            <div>
              <dt className="text-xs text-erp-text-muted">{t("vatAmount")} ({(billing.vatRate! * 100).toFixed(0)}%)</dt>
              <dd className="font-mono font-semibold">{formatCurrency(billing.vatAmount, "THB", locale)}</dd>
            </div>
          )}
          {billing.grossTotal != null && (
            <div><dt className="text-xs text-erp-text-muted">{t("grossTotal")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.grossTotal, "THB", locale)}</dd></div>
          )}
          {billing.effectiveReceivable != null && (
            <div><dt className="text-xs text-erp-text-muted font-bold text-erp-navy">{t("effectiveReceivable")}</dt><dd className="font-mono font-bold text-erp-navy">{formatCurrency(billing.effectiveReceivable, "THB", locale)}</dd></div>
          )}

          {/* Retention Section */}
          {hasRetention && (
            <div className="sm:col-span-2 lg:col-span-4 border-t border-erp-border pt-3">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <div className="flex flex-wrap items-center gap-4 text-xs">
                  <div>
                    <span className="text-erp-text-muted">{t("retentionRatePercent")}: </span>
                    <span className="font-mono font-semibold">{((billing.retentionRate ?? 0) * 100).toFixed(1)}%</span>
                  </div>
                  <div>
                    <span className="text-erp-text-muted">{t("retentionAmount")}: </span>
                    <span className="font-mono font-semibold text-erp-warning">{formatCurrency(billing.retentionAmount, "THB", locale)}</span>
                  </div>
                  <div className="flex items-center gap-1.5">
                    <span className="text-erp-text-muted">{t("retentionStatus")}: </span>
                    <StatusBadge
                      label={t(`retentionStatuses.${billing.retentionStatus ?? "none"}`)}
                      variant={retentionStatusVariant(billing.retentionStatus)}
                    />
                  </div>
                </div>
                {canReleaseRetention && (
                  <Button
                    type="button"
                    size="sm"
                    variant="outline"
                    className="min-h-11"
                    disabled={busy}
                    onClick={() => open({ kind: "releaseRetention" })}
                  >
                    {t("releaseRetention")}
                  </Button>
                )}
              </div>
            </div>
          )}
        </dl>

        {billing.voidReason && <p className="text-xs text-erp-text-muted">{t("voidReasonLine", { reason: billing.voidReason })}</p>}
        <p className="text-xs text-erp-text-muted">{t("immutableNote")}</p>
        {canManage && status === "issued" && (billing.paidAmount ?? 0) === 0 && (
          <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => open({ kind: "void" })}>{t("void")}</Button>
        )}
      </div>

      {/* Record Payment Form (with WHT) */}
      {canReceive && (
        <div className="erp-card space-y-4 p-5" role="region" aria-label={t("recordPayment")}>
          <h3 className="text-sm font-bold text-erp-navy">{t("recordPayment")}</h3>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 md:grid-cols-4">
            <Input type="number" min={0} step="0.01" label={t("paymentAmount")} placeholder={t("paymentAmountPlaceholder")} value={amount} disabled={busy} onChange={(event) => setAmount(event.target.value)} />
            <Select label={t("method")} value={method} disabled={busy} options={PAYMENT_METHODS.map((value) => ({ value, label: t(`methods.${value}`) }))} onChange={(event) => setMethod(event.target.value)} />
            <Input label={t("paymentReference")} placeholder={t("paymentReferencePlaceholder")} value={reference} maxLength={100} disabled={busy} onChange={(event) => setReference(event.target.value)} />
            <Input type="date" label={t("receivedDate")} placeholder={t("datePlaceholder")} value={received} disabled={busy} onChange={(event) => setReceived(event.target.value)} />

            {/* WHT Fields */}
            <Select
              label={t("whtRate")}
              value={whtRate}
              disabled={busy}
              options={[
                { value: "0", label: t("whtRates.rate_0") },
                { value: "0.01", label: t("whtRates.rate_1") },
                { value: "0.02", label: t("whtRates.rate_2") },
                { value: "0.03", label: t("whtRates.rate_3") },
                { value: "0.05", label: t("whtRates.rate_5") },
              ]}
              onChange={(event) => setWhtRate(event.target.value)}
            />
            {Number(whtRate) > 0 && (
              <>
                <Select
                  label={t("whtType")}
                  value={whtType}
                  disabled={busy}
                  options={[
                    { value: "service_3", label: t("whtTypes.service_3") },
                    { value: "transport_1", label: t("whtTypes.transport_1") },
                    { value: "advertising_2", label: t("whtTypes.advertising_2") },
                    { value: "rent_5", label: t("whtTypes.rent_5") },
                  ]}
                  onChange={(event) => setWhtType(event.target.value)}
                />
                <Input
                  label={t("whtCertificateNumber")}
                  placeholder={t("whtCertificateNumberPlaceholder")}
                  required
                  value={whtCertNumber}
                  maxLength={100}
                  disabled={busy}
                  onChange={(event) => setWhtCertNumber(event.target.value)}
                />
              </>
            )}
          </div>

          {/* Live WHT & Net Cash Summary */}
          {paymentCalc.payAmt > 0 && Number(whtRate) > 0 && (
            <div className="border border-erp-border bg-erp-surface-subtle p-3 text-xs flex flex-wrap gap-4">
              <div>
                <span className="text-erp-text-muted">{t("paymentAmount")}: </span>
                <span className="font-mono font-semibold">{formatCurrency(paymentCalc.payAmt, "THB", locale)}</span>
              </div>
              <div>
                <span className="text-erp-text-muted">{t("whtAmount")}: </span>
                <span className="font-mono font-semibold text-erp-warning">-{formatCurrency(paymentCalc.whtAmt, "THB", locale)}</span>
              </div>
              <div>
                <span className="text-erp-text-muted font-bold text-erp-navy">{t("netCashReceived")}: </span>
                <span className="font-mono font-bold text-erp-navy">{formatCurrency(paymentCalc.netCash, "THB", locale)}</span>
              </div>
            </div>
          )}

          <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.pay.isPending} disabled={busy} onClick={() => void pay()}>{t("savePayment")}</Button>
        </div>
      )}

      {/* Credit Note / Debit Note Section */}
      <div className="erp-card space-y-4 p-5" role="region" aria-label={t("notesTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("notesTitle")}</h3>

        {/* Existing Notes Table/List */}
        {(billing.notes ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("notesEmpty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("number")}</th>
                  <th className="py-2 pr-2">{t("noteType")}</th>
                  <th className="py-2 pr-2">{t("noteReason")}</th>
                  <th className="py-2 pr-2 text-right">{t("taxBaseDelta")}</th>
                  <th className="py-2 pr-2 text-right">{t("vatDelta")}</th>
                  <th className="py-2 pr-2 text-right">{t("totalDelta")}</th>
                  <th className="py-2 pr-2">{t("noteStatus")}</th>
                  <th className="py-2 text-right">{tCommon("actions.save") ? "Actions" : ""}</th>
                </tr>
              </thead>
              <tbody>
                {(billing.notes ?? []).map((note) => {
                  const isPending = note.status === "pending_approval";
                  const isCreator = note.createdBy?.id && note.createdBy.id === currentUserId;
                  const canApproveThis = isPending && canManage && !isCreator;

                  return (
                    <tr key={note.id} className="border-b border-erp-border/60 align-middle">
                      <td className="py-2 pr-2 font-mono font-medium">{note.number ?? "-"}</td>
                      <td className="py-2 pr-2">{t(`noteTypes.${note.noteType ?? "credit"}`)}</td>
                      <td className="py-2 pr-2 max-w-xs truncate">{note.reason ?? "-"}</td>
                      <td className="py-2 pr-2 text-right font-mono">{formatCurrency(note.taxBaseDelta, "THB", locale)}</td>
                      <td className="py-2 pr-2 text-right font-mono">{formatCurrency(note.vatDelta, "THB", locale)}</td>
                      <td className="py-2 pr-2 text-right font-mono font-semibold">{formatCurrency(note.totalDelta, "THB", locale)}</td>
                      <td className="py-2 pr-2">
                        <StatusBadge
                          label={t(`noteStatuses.${note.status ?? "pending_approval"}`)}
                          variant={billingNoteStatusVariant(note.status)}
                        />
                      </td>
                      <td className="py-2 text-right">
                        {canApproveThis && (
                          <Button
                            type="button"
                            size="sm"
                            variant="primary"
                            className="min-h-11"
                            disabled={busy}
                            onClick={() => open({ kind: "approveNote", note })}
                          >
                            {t("approveNote")}
                          </Button>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}

        {/* Create Note Form */}
        {canManage && status !== "voided" && (
          <div className="border border-erp-border bg-erp-surface-subtle p-4 space-y-3">
            <h4 className="text-xs font-bold text-erp-navy uppercase tracking-wider">{t("createNote")}</h4>
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
              <Select
                label={t("noteType")}
                value={noteType}
                disabled={busy}
                options={[
                  { value: "credit", label: t("noteTypes.credit") },
                  { value: "debit", label: t("noteTypes.debit") },
                ]}
                onChange={(e) => setNoteType(e.target.value as "credit" | "debit")}
              />
              <Input
                type="number"
                min={0}
                step="0.01"
                label={t("taxBaseDelta")}
                placeholder={t("taxBaseDeltaPlaceholder")}
                required
                value={noteTaxBaseDelta}
                disabled={busy}
                onChange={(e) => setNoteTaxBaseDelta(e.target.value)}
              />
              <Input
                label={t("noteReason")}
                placeholder={t("noteReasonPlaceholder")}
                required
                maxLength={500}
                value={noteReason}
                disabled={busy}
                onChange={(e) => setNoteReason(e.target.value)}
              />
            </div>

            {/* Note live preview */}
            {noteCalc.base > 0 && (
              <div className="text-xs flex flex-wrap gap-4 text-erp-text-body pt-1">
                <div>
                  <span className="text-erp-text-muted">{t("vatDelta")}: </span>
                  <span className="font-mono font-medium">{formatCurrency(noteCalc.vatD, "THB", locale)}</span>
                </div>
                <div>
                  <span className="text-erp-text-muted font-bold text-erp-navy">{t("totalDelta")}: </span>
                  <span className="font-mono font-bold text-erp-navy">{formatCurrency(noteCalc.totalD, "THB", locale)}</span>
                </div>
              </div>
            )}

            <div className="flex justify-end pt-2">
              <Button
                type="button"
                variant="primary"
                className="min-h-11"
                isLoading={mutations.createNote.isPending}
                disabled={busy}
                onClick={() => void handleCreateNote()}
              >
                {t("createNote")}
              </Button>
            </div>
          </div>
        )}
      </div>

      {/* Payments History List */}
      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("paymentsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("paymentsTitle")}</h3>
        {(billing.payments ?? []).length === 0 ? <p className="text-xs text-erp-text-muted">{t("paymentsEmpty")}</p> : (
          <ul className="space-y-2">
            {(billing.payments ?? []).map((payment) => (
              <li key={payment.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="flex flex-wrap items-center gap-2">
                  <span className="font-mono font-semibold">{payment.number}</span>
                  {payment.receiptNumber && (
                    <span className="font-mono font-semibold text-erp-navy">({payment.receiptNumber})</span>
                  )}
                  <span className="font-mono">{formatCurrency(payment.amount, "THB", locale)}</span>
                  <span>{isPaymentMethod(payment.method) ? t(`methods.${payment.method}`) : "-"}</span>
                  <span className="font-mono text-erp-text-muted">{payment.reference}</span>
                  <StatusBadge label={t(payment.status === "reversed" ? "paymentReversed" : "paymentActive")} variant={payment.status === "reversed" ? "danger" : "success"} />
                </div>
                <div className="text-erp-text-muted mt-1">
                  {payment.receivedDate ? formatDate(payment.receivedDate, locale) : "-"} · {payment.recordedBy?.displayName ?? "-"}
                  {(payment.whtAmount ?? 0) > 0 && (
                    <span className="ml-2 font-mono text-erp-warning">
                      · WHT: {formatCurrency(payment.whtAmount, "THB", locale)} ({payment.whtCertificateNumber ?? "-"})
                      {payment.netCashReceived != null && (
                        <span className="text-erp-text-body font-semibold ml-1">
                          · Net: {formatCurrency(payment.netCashReceived, "THB", locale)}
                        </span>
                      )}
                    </span>
                  )}
                </div>
                {payment.reversalReason && <div className="text-erp-text-muted">{t("reversalReasonLine", { reason: payment.reversalReason })}</div>}
                {canPay && payment.status !== "reversed" && status !== "voided" && (
                  <Button type="button" size="sm" variant="outline" className="mt-2 min-h-11" disabled={busy} onClick={() => open({ kind: "reverse", payment })}>{t("reverse")}</Button>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>

      {/* Confirmation Modal for Void & Reverse */}
      <Modal
        isOpen={dialog !== null && (dialog.kind === "void" || dialog.kind === "reverse")}
        onClose={() => { if (!busy) setDialog(null); }}
        title={dialog?.kind === "void" ? t("voidTitle") : dialog?.kind === "reverse" ? t("reverseTitle") : undefined}
        description={dialog?.kind === "void" ? t("voidDescription") : dialog?.kind === "reverse" ? t("reverseDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setDialog(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="danger" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirmVoidOrReverse()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <Input
          label={t("reason")}
          placeholder={t("reasonPlaceholder")}
          required
          value={reason}
          maxLength={500}
          disabled={busy}
          error={reasonError ? t("reasonRequired") : undefined}
          onChange={(event) => { setReason(event.target.value); if (event.target.value.trim()) setReasonError(false); }}
        />
      </Modal>

      {/* Confirmation Modal for Release Retention */}
      <ConfirmationModal
        isOpen={dialog !== null && dialog.kind === "releaseRetention"}
        onClose={() => setDialog(null)}
        onConfirm={() => void handleReleaseRetentionConfirm()}
        title={t("releaseRetentionTitle")}
        message={t("releaseRetentionDescription")}
        confirmText={t("releaseRetention")}
        variant="info"
        isLoading={mutations.releaseRetention.isPending}
      />

      {/* Confirmation Modal for Approve Note */}
      <ConfirmationModal
        isOpen={dialog !== null && dialog.kind === "approveNote"}
        onClose={() => setDialog(null)}
        onConfirm={() => {
          if (dialog?.kind === "approveNote") {
            void handleApproveNoteConfirm(dialog.note);
          }
        }}
        title={t("approveNoteTitle")}
        message={dialog?.kind === "approveNote" ? t("approveNoteDescription", {
          number: dialog.note.number ?? "-",
          type: t(`noteTypes.${dialog.note.noteType ?? "credit"}`),
          amount: formatCurrency(dialog.note.totalDelta, "THB", locale),
        }) : ""}
        confirmText={t("approveNote")}
        variant="info"
        isLoading={mutations.approveNote.isPending}
      />
    </section>
  );
}
