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
import { formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { ServiceRequestResponse, ServiceRequestStep } from "@/lib/api/api-client";
import { useServiceRequest, useServiceRequestMutations } from "../api/service-queries";
import { isServicePriority, isServiceRequestStatus, serviceErrorCode, serviceRequestStatusVariant } from "../service-status";

interface ServiceRequestDetailProps {
  requestId: string;
}

type Dialog = "schedule" | "resolve" | "reopen";

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
    setDialogError(null);
  }

  async function confirm(): Promise<void> {
    if (!dialog) return;
    if ((dialog === "schedule" && !date) || (dialog !== "schedule" && !text.trim())) {
      setDialogError(t(dialog === "schedule" ? "dateRequired" : "textRequired"));
      return;
    }

    const step: ServiceRequestStep = dialog === "schedule" ? { kind: "schedule", date } : dialog === "resolve" ? { kind: "resolve", note: text.trim() } : { kind: "reopen", reason: text.trim() };
    if (await run(step, t("saved"))) setDialog(null);
  }

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
        {!request.inWarranty && <p className="text-xs text-erp-text-muted">{t("outOfWarrantyNote")}</p>}

        {canManage && (
          <div className="flex flex-wrap gap-2">
            {(status === "open" || status === "scheduled") && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("schedule")}>{t("schedule")}</Button>}
            {(status === "open" || status === "scheduled") && <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "start" }, t("saved"))}>{t("start")}</Button>}
            {status === "in_progress" && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => open("resolve")}>{t("resolve")}</Button>}
            {status === "resolved" && <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "close" }, t("saved"))}>{t("close")}</Button>}
            {(status === "resolved" || status === "closed") && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("reopen")}>{t("reopen")}</Button>}
          </div>
        )}
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
            <Input type="date" label={t("visitDate")} required value={date} disabled={busy} onChange={(event) => setDate(event.target.value)} />
          ) : (
            <Input label={t(dialog === "reopen" ? "reopenReason" : "resolutionNote")} required value={text} maxLength={500} disabled={busy} onChange={(event) => setText(event.target.value)} />
          )}
        </div>
      </Modal>
    </section>
  );
}
