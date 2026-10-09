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
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { BillingResponse, PaymentResponse } from "@/lib/api/api-client";
import { useBilling, useBillingMutations } from "../api/finance-queries";
import { PAYMENT_METHODS, billingStatusVariant, financeErrorCode, isBillingKind, isBillingStatus, isPaymentMethod } from "../finance-status";

interface BillingDetailProps {
  billingId: string;
}

type Dialog = { kind: "void" } | { kind: "reverse"; payment: PaymentResponse };

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
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
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BILLINGS_MANAGE);
  const canPay = can(selectedMembership, PERMISSIONS.PAYMENTS_MANAGE);
  const mutations = useBillingMutations();

  const status = billing.status ?? "";
  const busy = mutations.pay.isPending || mutations.voidBilling.isPending || mutations.reverse.isPending;
  const [message, setMessage] = useState<string | null>(null);
  const [amount, setAmount] = useState("");
  const [method, setMethod] = useState("transfer");
  const [reference, setReference] = useState("");
  const [received, setReceived] = useState(todayIso);
  const [dialog, setDialog] = useState<Dialog | null>(null);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);
  // One key per payment form session, regenerated after each success so a retry replays but a new payment does not.
  const payKeyRef = useRef(crypto.randomUUID());

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? financeErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function pay(): Promise<void> {
    setMessage(null);
    const value = Number(amount);
    if (!(value > 0) || Math.round(value * 100) / 100 !== value || !reference.trim() || !received) {
      setMessage(t("paymentInvalid"));
      return;
    }

    try {
      await mutations.pay.mutateAsync({ id: billing.id ?? "", payload: { amount: value, method, reference: reference.trim(), receivedDate: received }, idempotencyKey: payKeyRef.current });
      payKeyRef.current = crypto.randomUUID();
      toast.success(t("paymentRecorded"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function confirm(): Promise<void> {
    if (!dialog) return;
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

  return (
    <section className="space-y-5">
      <PageHeader
        title={billing.number ?? "-"}
        subtitle={billing.description ?? "-"}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/finance/billings` }, { label: billing.number ?? "-" }]}
        actions={<StatusBadge label={isBillingStatus(status) ? t(`statuses.${status}`) : "-"} variant={billingStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card space-y-4 p-5">
        <dl className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2 lg:grid-cols-4">
          <div><dt className="text-xs text-erp-text-muted">{t("project")}</dt><dd className="font-medium">{billing.project?.id ? <Link href={`/${locale}/projects/${billing.project.id}`} className="font-mono text-erp-navy underline">{billing.project.code}</Link> : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("kind")}</dt><dd className="font-medium">{isBillingKind(billing.kind) ? t(`kinds.${billing.kind}`) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("dueDate")}</dt><dd className="font-medium">{billing.dueDate ? formatDate(billing.dueDate, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{billing.createdBy?.displayName ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("amount")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.amount, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("paid")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.paidAmount, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("outstanding")}</dt><dd className="font-mono font-semibold">{formatCurrency(billing.outstanding, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("referenceHash")}</dt><dd className="font-mono text-xs" title={billing.referenceHash ?? undefined}>{(billing.referenceHash ?? "").slice(0, 16)}</dd></div>
        </dl>
        {billing.voidReason && <p className="text-xs text-erp-text-muted">{t("voidReasonLine", { reason: billing.voidReason })}</p>}
        <p className="text-xs text-erp-text-muted">{t("immutableNote")}</p>
        {canManage && (status === "issued") && (billing.paidAmount ?? 0) === 0 && (
          <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => open({ kind: "void" })}>{t("void")}</Button>
        )}
      </div>

      {canReceive && (
        <div className="erp-card space-y-3 p-5" role="region" aria-label={t("recordPayment")}>
          <h3 className="text-sm font-bold text-erp-navy">{t("recordPayment")}</h3>
          <div className="grid grid-cols-1 gap-3 md:grid-cols-4">
            <Input type="number" min={0} step="0.01" label={t("paymentAmount")} placeholder={t("paymentAmountPlaceholder")} value={amount} disabled={busy} onChange={(event) => setAmount(event.target.value)} />
            <Select label={t("method")} value={method} disabled={busy} options={PAYMENT_METHODS.map((value) => ({ value, label: t(`methods.${value}`) }))} onChange={(event) => setMethod(event.target.value)} />
            <Input label={t("paymentReference")} placeholder={t("paymentReferencePlaceholder")} value={reference} maxLength={100} disabled={busy} onChange={(event) => setReference(event.target.value)} />
            <Input type="date" label={t("receivedDate")} value={received} disabled={busy} onChange={(event) => setReceived(event.target.value)} />
          </div>
          <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.pay.isPending} disabled={busy} onClick={() => void pay()}>{t("savePayment")}</Button>
        </div>
      )}

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("paymentsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("paymentsTitle")}</h3>
        {(billing.payments ?? []).length === 0 ? <p className="text-xs text-erp-text-muted">{t("paymentsEmpty")}</p> : (
          <ul className="space-y-2">
            {(billing.payments ?? []).map((payment) => (
              <li key={payment.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="flex flex-wrap items-center gap-2">
                  <span className="font-mono font-semibold">{payment.number}</span>
                  <span className="font-mono">{formatCurrency(payment.amount, "THB", locale)}</span>
                  <span>{isPaymentMethod(payment.method) ? t(`methods.${payment.method}`) : "-"}</span>
                  <span className="font-mono text-erp-text-muted">{payment.reference}</span>
                  <StatusBadge label={t(payment.status === "reversed" ? "paymentReversed" : "paymentActive")} variant={payment.status === "reversed" ? "danger" : "success"} />
                </div>
                <div className="text-erp-text-muted">{payment.receivedDate ? formatDate(payment.receivedDate, locale) : "-"} · {payment.recordedBy?.displayName ?? "-"}</div>
                {payment.reversalReason && <div className="text-erp-text-muted">{t("reversalReasonLine", { reason: payment.reversalReason })}</div>}
                {canPay && payment.status !== "reversed" && status !== "voided" && (
                  <Button type="button" size="sm" variant="outline" className="mt-2 min-h-11" disabled={busy} onClick={() => open({ kind: "reverse", payment })}>{t("reverse")}</Button>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>

      <Modal
        isOpen={dialog !== null}
        onClose={() => { if (!busy) setDialog(null); }}
        title={dialog ? t(dialog.kind === "void" ? "voidTitle" : "reverseTitle") : undefined}
        description={dialog ? t(dialog.kind === "void" ? "voidDescription" : "reverseDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setDialog(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="danger" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirm()}>{t("confirm")}</Button>
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
    </section>
  );
}
