"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import type { QuotationHistoryItemResponse, QuotationLifecycleAction } from "@/lib/api/api-client";
import { useQuotationHistory, useQuotationLifecycleMutation } from "../api/quotation-lifecycle-queries";
import { isQuotationStatus, quotationLifecycleErrorCode, quotationStatusVariant } from "../quotation-lifecycle-status";

interface QuotationLifecyclePanelProps {
  estimateId: string;
}

export function QuotationLifecyclePanel({ estimateId }: QuotationLifecyclePanelProps) {
  const t = useTranslations("quotationLifecycle");
  const tErrors = useTranslations("quotationLifecycle.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canVoid = can(selectedMembership, PERMISSIONS.QUOTATIONS_VOID);
  const canAmend = can(selectedMembership, PERMISSIONS.QUOTATIONS_AMEND);
  const history = useQuotationHistory(estimateId);
  const mutation = useQuotationLifecycleMutation(estimateId);

  const [target, setTarget] = useState<{ item: QuotationHistoryItemResponse; action: QuotationLifecycleAction } | null>(null);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  // One key per confirmation so a retried amendment replays instead of issuing a second replacement.
  const keyRef = useRef(crypto.randomUUID());

  const items = history.data?.items ?? [];
  if (history.isLoading || history.isError || items.length === 0) return null;

  function open(item: QuotationHistoryItemResponse, action: QuotationLifecycleAction): void {
    setTarget({ item, action });
    setReason("");
    setReasonError(false);
    keyRef.current = crypto.randomUUID();
  }

  async function confirm(): Promise<void> {
    if (!target) return;
    if (!reason.trim()) {
      setReasonError(true);
      return;
    }

    setMessage(null);
    try {
      await mutation.mutateAsync({
        quotationId: target.item.id ?? "",
        rowVersion: target.item.rowVersion ?? "",
        action: target.action,
        reason: reason.trim(),
        idempotencyKey: keyRef.current,
      });
      toast.success(t(target.action === "void" ? "voided" : "amended"));
      setTarget(null);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? quotationLifecycleErrorCode(err.code) : null;
      setTarget(null);
      setMessage(code ? tErrors(code) : tErrors("failed"));
    }
  }

  const busy = mutation.isPending;

  return (
    <section className="erp-card space-y-3 p-5 print:hidden" aria-label={t("title")}>
      <h2 className="text-sm font-bold text-erp-navy">{t("title")}</h2>
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      <ul className="space-y-2">
        {items.map((item) => (
          <li key={item.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
            <div className="flex flex-wrap items-center gap-2">
              <span className="font-mono font-semibold text-erp-text-main">{item.number}</span>
              <StatusBadge label={isQuotationStatus(item.status) ? t(`statuses.${item.status}`) : "-"} variant={quotationStatusVariant(item.status)} />
              <span className="font-mono">{formatCurrency(item.totalAmount, "THB", locale)}</span>
              <span className="text-erp-text-muted">R{item.estimateRevisionNo} · {item.issuedAtUtc ? formatDateTime(item.issuedAtUtc, locale) : "-"}</span>
            </div>
            {item.supersedesNumber && <div className="mt-1 text-erp-text-muted">{t("supersedes", { number: item.supersedesNumber })}{item.amendmentReason ? ` · ${item.amendmentReason}` : ""}</div>}
            {item.supersededByNumber && <div className="mt-1 text-erp-text-muted">{t("supersededBy", { number: item.supersededByNumber })}</div>}
            {item.voidReason && (
              <div className="mt-1 text-erp-text-muted">
                {t("voidedBy", { name: item.voidedBy?.displayName ?? "-", date: item.voidedAtUtc ? formatDateTime(item.voidedAtUtc, locale) : "-" })} · {item.voidReason}
              </div>
            )}
            {item.status === "issued" && (canAmend || canVoid) && (
              <div className="mt-2 flex flex-wrap gap-2">
                {canAmend && <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => open(item, "amend")}>{t("amend")}</Button>}
                {canVoid && <Button type="button" size="sm" variant="danger" className="min-h-11" disabled={busy} onClick={() => open(item, "void")}>{t("void")}</Button>}
              </div>
            )}
          </li>
        ))}
      </ul>

      <Modal
        isOpen={target !== null}
        onClose={() => { if (!busy) setTarget(null); }}
        title={target ? t(target.action === "void" ? "voidTitle" : "amendTitle", { number: target.item.number ?? "-" }) : undefined}
        description={target ? t(target.action === "void" ? "voidDescription" : "amendDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setTarget(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant={target?.action === "void" ? "danger" : "primary"} className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirm()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <Input
          label={t("reason")}
          required
          placeholder={t("reasonPlaceholder")}
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
