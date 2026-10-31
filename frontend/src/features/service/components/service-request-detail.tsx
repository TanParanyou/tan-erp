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
import { AttachmentList } from "@/components/forms/AttachmentList";
import { SignatureCapturePanel } from "@/components/forms/SignatureCapturePanel";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { ServiceRequestResponse, ServiceRequestStep } from "@/lib/api/api-client";
import { useServiceRequest, useServiceRequestMutations } from "../api/service-queries";
import {
  isServicePriority,
  isServiceRequestStatus,
  serviceErrorCode,
  serviceRequestStatusVariant,
  slaStatusVariant,
} from "../service-status";

interface ServiceRequestDetailProps {
  requestId: string;
}

type Dialog = "schedule" | "resolve" | "reopen" | "wait-customer" | "charge";

export function ServiceRequestDetail({ requestId }: ServiceRequestDetailProps) {
  const query = useServiceRequest(requestId);
  const t = useTranslations("service.requests");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;
  return <DetailView key={query.data.rowVersion} request={query.data} />;
}

function DetailView({ request }: { request: ServiceRequestResponse }) {
  const t = useTranslations("service.requests");
  const tErrors = useTranslations("service.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.SERVICE_REQUESTS_MANAGE);
  const mutations = useServiceRequestMutations();

  const status = request.status ?? "";
  const busy = mutations.step.isPending;
  const [message, setMessage] = useState<string | null>(null);
  const [dialog, setDialog] = useState<Dialog | null>(null);
  const [text, setText] = useState("");
  const [date, setDate] = useState("");
  const [chargeAmount, setChargeAmount] = useState("");
  const [dialogError, setDialogError] = useState<string | null>(null);

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? serviceErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function run(step: ServiceRequestStep, success: string): Promise<boolean> {
    setMessage(null);
    try {
      await mutations.step.mutateAsync({ id: request.id ?? "", rowVersion: request.rowVersion ?? "", step });
      toast.success(success);
      return true;
    } catch (err: unknown) {
      setMessage(describe(err));
      return false;
    }
  }

  function open(next: Dialog): void {
    setDialog(next);
    setText("");
    setDate("");
    setChargeAmount("");
    setDialogError(null);
  }

  async function confirm(): Promise<void> {
    if (!dialog) return;

    if (dialog === "schedule") {
      if (!date) {
        setDialogError(t("dateRequired"));
        return;
      }
      if (await run({ kind: "schedule", date }, t("saved"))) setDialog(null);
      return;
    }

    if (dialog === "charge") {
      const amountVal = Number(chargeAmount);
      if (!(amountVal > 0) || !text.trim()) {
        setDialogError(t("textRequired"));
        return;
      }
      if (await run({ kind: "charge", amount: amountVal, note: text.trim() }, t("saved"))) setDialog(null);
      return;
    }

    if (!text.trim()) {
      setDialogError(t("textRequired"));
      return;
    }

    let step: ServiceRequestStep;
    if (dialog === "wait-customer") {
      step = { kind: "wait-customer", reason: text.trim() };
    } else if (dialog === "resolve") {
      step = { kind: "resolve", note: text.trim() };
    } else {
      step = { kind: "reopen", reason: text.trim() };
    }

    if (await run(step, t("saved"))) setDialog(null);
  }

  const attachmentsEditable = canManage && ["open", "scheduled", "in_progress", "waiting_customer", "resolved"].includes(status);
  const signatureEditable = canManage && status === "resolved";

  return (
    <section className="space-y-5">
      <PageHeader
        title={request.number ?? "-"}
        subtitle={request.title ?? "-"}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/service/requests` }, { label: request.number ?? "-" }]}
        actions={<StatusBadge label={isServiceRequestStatus(status) ? t(`statuses.${status}`) : "-"} variant={serviceRequestStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card space-y-4 p-5">
        <dl className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2 lg:grid-cols-3">
          <div><dt className="text-xs text-erp-text-muted">{t("project")}</dt><dd className="font-medium">{request.project?.id ? <Link href={`/${locale}/projects/${request.project.id}`} className="font-mono text-erp-navy underline">{request.project.code}</Link> : "-"} · {request.project?.name ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("priority")}</dt><dd className="font-medium">{isServicePriority(request.priority) ? t(`priorities.${request.priority}`) : "-"}</dd></div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("warranty")}</dt>
            <dd className="font-medium">
              <StatusBadge label={t(request.inWarranty ? "inWarranty" : "outOfWarranty")} variant={request.inWarranty ? "success" : "neutral"} />
              {request.warranty?.number && <span className="ml-2 font-mono text-xs">{request.warranty.number}</span>}
            </dd>
          </div>
          <div><dt className="text-xs text-erp-text-muted">{t("scheduledDate")}</dt><dd className="font-medium">{request.scheduledDate ? formatDate(request.scheduledDate, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{request.createdBy?.displayName ?? "-"}</dd></div>
        </dl>
        <p className="text-sm text-erp-text-body">{request.description}</p>
        {request.resolutionNote && <p className="text-xs text-erp-text-muted">{t("resolutionLine", { note: request.resolutionNote })}</p>}

        {/* SLA Information Card */}
        {request.sla && (
          <div className="erp-card p-4 space-y-2 border-l-4 border-erp-navy bg-erp-surface-subtle" role="region" aria-label={t("slaTitle")}>
            <div className="flex flex-wrap items-center justify-between gap-2">
              <span className="font-semibold text-xs text-erp-navy">{t("slaTitle")}</span>
              <StatusBadge
                label={request.sla.status ? t(`slaStatuses.${request.sla.status}`) : "-"}
                variant={slaStatusVariant(request.sla.status)}
              />
            </div>
            <div className="grid grid-cols-2 gap-3 text-xs sm:grid-cols-4">
              <div>
                <span className="text-erp-text-muted">{t("slaElapsed")}</span>
                <div className="font-mono font-semibold">{t("slaHoursValue", { hours: request.sla.elapsedHours !== undefined ? request.sla.elapsedHours.toFixed(1) : "0" })}</div>
              </div>
              <div>
                <span className="text-erp-text-muted">{t("slaResolve")}</span>
                <div className="font-mono font-semibold">{t("slaHoursValue", { hours: String(request.sla.resolveHours ?? "-") })}</div>
              </div>
              <div>
                <span className="text-erp-text-muted">{t("slaPaused")}</span>
                <div className="font-mono font-semibold">{request.sla.pausedSeconds ? t("slaMinutesValue", { minutes: String(Math.round(request.sla.pausedSeconds / 60)) }) : "-"}</div>
              </div>
              <div>
                <span className="text-erp-text-muted">{t("scheduledDate")}</span>
                <div className="font-mono font-semibold">{request.scheduledDate ? formatDate(request.scheduledDate, locale) : "-"}</div>
              </div>
            </div>
          </div>
        )}

        {/* Out of warranty charge information */}
        {!request.inWarranty && (
          <div className="space-y-2">
            {request.billingDocumentId ? (
              <div className="erp-card p-4 space-y-1 border-l-4 border-erp-navy bg-erp-surface-subtle">
                <span className="font-semibold text-xs text-erp-navy">{t("chargesTitle")}</span>
                <div className="text-xs flex flex-wrap items-center gap-3">
                  <div>
                    <span className="text-erp-text-muted">{t("serviceChargeAmount")}: </span>
                    <span className="font-mono font-bold text-erp-navy">
                      {formatCurrency(request.serviceChargeAmount ?? 0, "THB", locale)}
                    </span>
                  </div>
                  {request.billingDocumentNumber && (
                    <div>
                      <span className="text-erp-text-muted">{t("billingDocument")}: </span>
                      <Link
                        href={`/${locale}/finance/billings/${request.billingDocumentId}`}
                        className="font-mono text-erp-navy underline font-medium"
                      >
                        {request.billingDocumentNumber}
                      </Link>
                    </div>
                  )}
                </div>
                {request.serviceChargeNote && (
                  <p className="text-xs text-erp-text-muted mt-1">{request.serviceChargeNote}</p>
                )}
              </div>
            ) : (
              <p className="text-xs text-erp-text-muted">{t("outOfWarrantyNote")}</p>
            )}
          </div>
        )}

        {canManage && (
          <div className="flex flex-wrap gap-2 pt-2">
            {(status === "open" || status === "scheduled") && (
              <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("schedule")}>
                {t("schedule")}
              </Button>
            )}
            {(status === "open" || status === "scheduled") && (
              <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "start" }, t("saved"))}>
                {t("start")}
              </Button>
            )}
            {(status === "open" || status === "scheduled" || status === "in_progress") && (
              <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("wait-customer")}>
                {t("waitCustomer")}
              </Button>
            )}
            {status === "waiting_customer" && (
              <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "resume" }, t("saved"))}>
                {t("resume")}
              </Button>
            )}
            {!request.inWarranty && !request.billingDocumentId && (
              <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("charge")}>
                {t("charge")}
              </Button>
            )}
            {status === "in_progress" && (
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => open("resolve")}>
                {t("resolve")}
              </Button>
            )}
            {status === "resolved" && (
              <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "close" }, t("saved"))}>
                {t("close")}
              </Button>
            )}
            {(status === "resolved" || status === "closed") && (
              <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("reopen")}>
                {t("reopen")}
              </Button>
            )}
          </div>
        )}
      </div>

      {/* Attachments Section */}
      <div className="erp-card space-y-3 p-5">
        <AttachmentList
          ownerType="service-request"
          ownerId={request.id ?? ""}
          canManage={attachmentsEditable}
          purposes={["evidence", "defect", "general"]}
        />
      </div>

      {/* Signature Handover Section */}
      <div className="erp-card space-y-3 p-5">
        <SignatureCapturePanel
          ownerType="service-request"
          ownerId={request.id ?? ""}
          purpose="handover"
          canCapture={signatureEditable}
        />
      </div>

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("historyTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("historyTitle")}</h3>
        <ul className="space-y-2">
          {(request.events ?? []).map((event, index) => (
            <li key={`${event.occurredAtUtc}-${index}`} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
              <div className="font-semibold">{isServiceRequestStatus(event.toStatus) ? t(`statuses.${event.toStatus}`) : "-"}</div>
              <div className="text-erp-text-muted">{event.actor?.displayName ?? "-"} · {event.occurredAtUtc ? formatDateTime(event.occurredAtUtc, locale) : "-"}</div>
              {event.note && <div className="mt-1 text-erp-text-body">{event.note}</div>}
            </li>
          ))}
        </ul>
      </div>

      <Modal
        isOpen={dialog !== null}
        onClose={() => { if (!busy) setDialog(null); }}
        title={dialog ? t(`dialogs.${dialog}.title`) : undefined}
        description={dialog ? t(`dialogs.${dialog}.description`) : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setDialog(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirm()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <div className="space-y-4">
          {dialogError && <Alert variant="danger" onClose={() => setDialogError(null)}>{dialogError}</Alert>}
          {dialog === "schedule" ? (
            <Input id="service-request-schedule-date" type="date" label={t("visitDate")} placeholder={t("datePlaceholder")} required value={date} disabled={busy} className="h-11 min-h-[44px]" onChange={(event) => setDate(event.target.value)} />
          ) : dialog === "charge" ? (
            <div className="space-y-3">
              <Input
                id="service-request-charge-amount"
                type="number"
                min={0}
                step="0.01"
                label={t("chargeAmount")}
                placeholder={t("chargeAmountPlaceholder")}
                required
                value={chargeAmount}
                disabled={busy}
                className="h-11 min-h-[44px]"
                onChange={(event) => setChargeAmount(event.target.value)}
              />
              <Input
                id="service-request-charge-note"
                label={t("chargeNote")}
                placeholder={t("chargeNotePlaceholder")}
                required
                value={text}
                maxLength={500}
                disabled={busy}
                className="h-11 min-h-[44px]"
                onChange={(event) => setText(event.target.value)}
              />
            </div>
          ) : (
            <Input
              id="service-request-dialog-text"
              label={t(dialog === "reopen" ? "reopenReason" : dialog === "wait-customer" ? "waitReason" : "resolutionNote")}
              required
              value={text}
              maxLength={500}
              disabled={busy}
              className="h-11 min-h-[44px]"
              placeholder={t(dialog === "reopen" ? "reopenReasonPlaceholder" : dialog === "wait-customer" ? "waitReasonPlaceholder" : "resolutionNotePlaceholder")}
              onChange={(event) => setText(event.target.value)}
            />
          )}
        </div>
      </Modal>
    </section>
  );
}
