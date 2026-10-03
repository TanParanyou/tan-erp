"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import type { AcceptanceLinkResponse } from "@/lib/api/api-client";
import { useAcceptanceLinkMutations, useAcceptanceLinks, useQuotationHistory } from "../api/quotation-lifecycle-queries";
import { acceptanceLinkErrorCode, acceptanceLinkStateLabel, acceptanceLinkStateVariant } from "../quotation-lifecycle-status";

interface AcceptanceLinksPanelProps {
  estimateId: string;
}

const DEFAULT_DAYS = "7";

export function AcceptanceLinksPanel({ estimateId }: AcceptanceLinksPanelProps) {
  const history = useQuotationHistory(estimateId);
  const live = (history.data?.items ?? []).find((item) => item.status === "issued" || item.status === "accepted");
  if (!live?.id) return null;
  return <PanelBody quotationId={live.id} quotationStatus={live.status ?? ""} quotationNumber={live.number ?? "-"} />;
}

interface PanelBodyProps {
  quotationId: string;
  quotationStatus: string;
  quotationNumber: string;
}

function PanelBody({ quotationId, quotationStatus, quotationNumber }: PanelBodyProps) {
  const t = useTranslations("externalAcceptance.links");
  const tErrors = useTranslations("externalAcceptance.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canShare = can(selectedMembership, PERMISSIONS.QUOTATIONS_SHARE);
  const links = useAcceptanceLinks(quotationId);
  const mutations = useAcceptanceLinkMutations(quotationId);

  const [open, setOpen] = useState(false);
  const [days, setDays] = useState(DEFAULT_DAYS);
  const [hint, setHint] = useState("");
  const [createdUrl, setCreatedUrl] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const busy = mutations.create.isPending || mutations.revoke.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? acceptanceLinkErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function create(): Promise<void> {
    const lifetime = Number(days);
    if (!Number.isInteger(lifetime) || lifetime < 1 || lifetime > 30) {
      setMessage(tErrors("ACCEPTANCE_LINK_LIFETIME_INVALID"));
      return;
    }

    setMessage(null);
    try {
      const created = await mutations.create.mutateAsync({ lifetimeDays: lifetime, signerHint: hint.trim() || null });
      setCreatedUrl(`${window.location.origin}/${locale}${created.publicPath}`);
      setOpen(false);
      setHint("");
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function revoke(link: AcceptanceLinkResponse): Promise<void> {
    setMessage(null);
    try {
      await mutations.revoke.mutateAsync({ linkId: link.id ?? "" });
      toast.success(t("revoked"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function copy(): Promise<void> {
    if (!createdUrl) return;
    try {
      await navigator.clipboard.writeText(createdUrl);
      toast.success(t("copied"));
    } catch {
      toast.error(t("copyFailed"));
    }
  }

  return (
    <section className="erp-card space-y-3 p-5 print:hidden" aria-label={t("title")}>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 className="text-sm font-bold text-erp-navy">{t("title", { number: quotationNumber })}</h2>
        {canShare && quotationStatus === "issued" && (
          <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => { setDays(DEFAULT_DAYS); setOpen(true); }}>{t("create")}</Button>
        )}
      </div>
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      {createdUrl && (
        <Alert variant="warning" onClose={() => setCreatedUrl(null)}>
          <p className="text-xs">{t("shownOnce")}</p>
          <p className="mt-1 break-all font-mono text-xs">{createdUrl}</p>
          <Button type="button" size="sm" variant="outline" className="mt-2 min-h-11" onClick={() => void copy()}>{t("copy")}</Button>
        </Alert>
      )}
      <ul className="space-y-2">
        {(links.data?.items ?? []).map((link) => {
          const state = acceptanceLinkStateLabel(link);
          return (
            <li key={link.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
              <div className="flex flex-wrap items-center gap-2">
                <StatusBadge label={t(`states.${state}`)} variant={acceptanceLinkStateVariant(state)} />
                <span className="text-erp-text-muted">{t("expires", { date: link.expiresAtUtc ? formatDateTime(link.expiresAtUtc, locale) : "-" })}</span>
                {link.signerHint && <span>{link.signerHint}</span>}
                <span className="text-erp-text-muted">{link.createdBy?.displayName ?? "-"}</span>
              </div>
              {link.evidence && (
                <div className="mt-1 text-erp-text-body">
                  {t("evidence", { name: link.evidence.signerName ?? "-", role: link.evidence.signerRole ?? "-", date: link.evidence.acceptedAtUtc ? formatDateTime(link.evidence.acceptedAtUtc, locale) : "-" })}
                  {link.evidence.hasSignatureImage ? ` · ${t("hasSignature")}` : ""}
                </div>
              )}
              {canShare && link.status === "active" && (
                <Button type="button" size="sm" variant="outline" className="mt-2 min-h-11" disabled={busy} onClick={() => void revoke(link)}>{t("revoke")}</Button>
              )}
            </li>
          );
        })}
        {(links.data?.items ?? []).length === 0 && <li className="text-xs text-erp-text-muted">{t("empty")}</li>}
      </ul>

      <Modal
        isOpen={open}
        onClose={() => { if (!busy) setOpen(false); }}
        title={t("createTitle")}
        description={t("createDescription")}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.create.isPending} disabled={busy} onClick={() => void create()}>{t("confirmCreate")}</Button>
          </div>
        )}
      >
        <div className="space-y-4">
          <Input type="number" min={1} max={30} step={1} label={t("lifetimeDays")} value={days} disabled={busy} onChange={(event) => setDays(event.target.value)} />
          <Input label={t("signerHint")} value={hint} maxLength={200} disabled={busy} onChange={(event) => setHint(event.target.value)} />
        </div>
      </Modal>
    </section>
  );
}
