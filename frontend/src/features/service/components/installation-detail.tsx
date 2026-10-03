"use client";

import { useState } from "react";
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
import { formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { DefectResponse, InstallationResponse, InstallationStep } from "@/lib/api/api-client";
import { useInstallation, useInstallationMutations } from "../api/service-queries";
import {
  DEFECT_SEVERITIES,
  defectStatusVariant,
  installationStatusVariant,
  isDefectSeverity,
  isDefectStatus,
  isInstallationStatus,
  serviceErrorCode,
} from "../service-status";

interface InstallationDetailProps {
  installationId: string;
}

type Dialog =
  | { kind: "cancel" }
  | { kind: "report-defect" }
  | { kind: "handover" }
  | { kind: "resolve-defect"; defect: DefectResponse }
  | { kind: "reopen-defect"; defect: DefectResponse };

export function InstallationDetail({ installationId }: InstallationDetailProps) {
  const query = useInstallation(installationId);
  const t = useTranslations("service.installations");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;

  // Keyed by row version so form inputs reset after every change.
  return <DetailView key={query.data.rowVersion} job={query.data} />;
}

function DetailView({ job }: { job: InstallationResponse }) {
  const t = useTranslations("service.installations");
  const tErrors = useTranslations("service.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.INSTALLATIONS_MANAGE);
  const canOperate = can(selectedMembership, PERMISSIONS.INSTALLATIONS_OPERATE);
  const canHandover = can(selectedMembership, PERMISSIONS.INSTALLATIONS_HANDOVER);
  const mutations = useInstallationMutations();

  const status = job.status ?? "";
  const checklist = job.checklist ?? [];
  const defects = job.defects ?? [];
  const busy = mutations.step.isPending;
  const [message, setMessage] = useState<string | null>(null);
  const [dialog, setDialog] = useState<Dialog | null>(null);
  const [text, setText] = useState("");
  const [severity, setSeverity] = useState("minor");
  const [outcome, setOutcome] = useState("accepted");
  const [signer, setSigner] = useState("");
  const [months, setMonths] = useState("");
  const [handoverDate, setHandoverDate] = useState("");
  const [dialogError, setDialogError] = useState<string | null>(null);

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? serviceErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function run(step: InstallationStep, success?: string): Promise<boolean> {
    setMessage(null);
    try {
      await mutations.step.mutateAsync({ id: job.id ?? "", rowVersion: job.rowVersion ?? "", step });
      if (success) toast.success(success);
      return true;
    } catch (err: unknown) {
      setMessage(describe(err));
      return false;
    }
  }

  function open(next: Dialog): void {
    setDialog(next);
    setText("");
    setSeverity("minor");
    setOutcome("accepted");
    setSigner("");
    setMonths("");
    setHandoverDate("");
    setDialogError(null);
  }

  async function confirm(): Promise<void> {
    if (!dialog) return;
    setDialogError(null);
    let step: InstallationStep;
    switch (dialog.kind) {
      case "cancel":
        step = { kind: "cancel", reason: text.trim() };
        break;
      case "report-defect":
        step = { kind: "report-defect", description: text.trim(), severity };
        break;
      case "resolve-defect":
        step = { kind: "resolve-defect", defectId: dialog.defect.id ?? "", note: text.trim() };
        break;
      case "reopen-defect":
        step = { kind: "reopen-defect", defectId: dialog.defect.id ?? "", reason: text.trim() };
        break;
      case "handover": {
        const warranty = months.trim() === "" ? null : Number(months);
        if (!signer.trim() || (outcome === "accepted" && (warranty === null || !Number.isInteger(warranty) || warranty < 0 || warranty > 120)) || (outcome === "disputed" && !text.trim())) {
          setDialogError(t(outcome === "accepted" ? "handoverAcceptInvalid" : "handoverDisputeInvalid"));
          return;
        }

        step = { kind: "handover", request: { outcome, signerName: signer.trim(), note: text.trim() || null, warrantyMonths: outcome === "accepted" ? warranty : null, handoverDate: handoverDate || null } };
        break;
      }
    }

    if (dialog.kind !== "handover" && !text.trim()) {
      setDialogError(t("textRequired"));
      return;
    }

    if (await run(step, dialog.kind === "handover" ? t(outcome === "accepted" ? "handedOver" : "disputed") : t("saved"))) setDialog(null);
  }

  const dialogTitle = dialog ? t(`dialogs.${dialog.kind}.title`) : undefined;

  return (
    <section className="space-y-5">
      <PageHeader
        title={job.number ?? "-"}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/service/installations` }, { label: job.number ?? "-" }]}
        actions={<StatusBadge label={isInstallationStatus(status) ? t(`statuses.${status}`) : "-"} variant={installationStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card space-y-4 p-5">
        <dl className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2 lg:grid-cols-3">
          <div>
            <dt className="text-xs text-erp-text-muted">{t("project")}</dt>
            <dd className="font-medium">{job.project?.id ? <Link href={`/${locale}/projects/${job.project.id}`} className="font-mono text-erp-navy underline">{job.project.code}</Link> : "-"} · {job.project?.name ?? "-"}</dd>
          </div>
          <div><dt className="text-xs text-erp-text-muted">{t("schedule")}</dt><dd className="font-medium">{job.scheduledStart ? formatDate(job.scheduledStart, locale) : "-"} – {job.scheduledEnd ? formatDate(job.scheduledEnd, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("crew")}</dt><dd className="font-medium">{job.crewName ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{job.createdBy?.displayName ?? "-"}</dd></div>
          {job.handover && (
            <div className="sm:col-span-2 lg:col-span-3">
              <dt className="text-xs text-erp-text-muted">{t("handoverTitle")}</dt>
              <dd className="font-medium">
                {t("handoverSummary", { name: job.handover.signerName ?? "-", date: job.handover.date ? formatDate(job.handover.date, locale) : "-", months: job.handover.warrantyMonths ?? 0 })}
                {job.warranty?.number && <> · <span className="font-mono">{job.warranty.number}</span> ({job.warranty.startDate ? formatDate(job.warranty.startDate, locale) : "-"} – {job.warranty.endDate ? formatDate(job.warranty.endDate, locale) : "-"})</>}
              </dd>
            </div>
          )}
        </dl>
        {job.note && <p className="text-xs text-erp-text-body">{t("noteLine", { note: job.note })}</p>}
        {job.cancelReason && <p className="text-xs text-erp-text-muted">{t("cancelReason", { reason: job.cancelReason })}</p>}
        {(job.disputeCount ?? 0) > 0 && job.lastDisputeNote && <p className="text-xs text-erp-text-muted">{t("lastDispute", { count: job.disputeCount ?? 0, note: job.lastDisputeNote })}</p>}

        <div className="flex flex-wrap gap-2">
          {status === "planned" && canOperate && <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "start" }, t("started"))}>{t("start")}</Button>}
          {status === "in_progress" && canOperate && <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void run({ kind: "ready" }, t("markedReady"))}>{t("markReady")}</Button>}
          {status === "ready_for_handover" && canHandover && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => open({ kind: "handover" })}>{t("recordHandover")}</Button>}
          {["planned", "in_progress", "ready_for_handover"].includes(status) && canManage && <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => open({ kind: "cancel" })}>{t("cancel")}</Button>}
        </div>
      </div>

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("checklistTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("checklistTitle")}</h3>
        {checklist.length === 0 ? <p className="text-xs text-erp-text-muted">{t("checklistEmpty")}</p> : (
          <ul className="space-y-2">
            {checklist.map((item) => (
              <li key={item.id} className="flex flex-wrap items-center gap-3 border border-erp-border bg-erp-surface-subtle p-3 text-sm">
                <input
                  type="checkbox"
                  className="h-5 w-5"
                  aria-label={item.title ?? ""}
                  checked={Boolean(item.done)}
                  disabled={busy || status !== "in_progress" || !canOperate}
                  onChange={(event) => void run({ kind: "checklist", itemId: item.id ?? "", done: event.target.checked })}
                />
                <span className="flex-1">{item.title}</span>
                {item.required && <span className="text-xs font-semibold text-erp-navy">{t("required")}</span>}
                {item.done && item.doneAtUtc && <span className="text-xs text-erp-text-muted">{item.doneBy?.displayName ?? "-"} · {formatDateTime(item.doneAtUtc, locale)}</span>}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("defectsTitle")}>
        <div className="flex flex-wrap items-center justify-between gap-3">
          <h3 className="text-sm font-bold text-erp-navy">{t("defectsTitle")}</h3>
          {canOperate && ["in_progress", "ready_for_handover"].includes(status) && <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => open({ kind: "report-defect" })}>{t("reportDefect")}</Button>}
        </div>
        {defects.length === 0 ? <p className="text-xs text-erp-text-muted">{t("defectsEmpty")}</p> : (
          <ul className="space-y-2">
            {defects.map((defect) => (
              <li key={defect.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
                <div className="flex flex-wrap items-center gap-2">
                  <span className="font-mono font-semibold">#{defect.no}</span>
                  <StatusBadge label={isDefectStatus(defect.status) ? t(`defectStatuses.${defect.status}`) : "-"} variant={defectStatusVariant(defect.status)} />
                  <span className="font-semibold">{isDefectSeverity(defect.severity) ? t(`severities.${defect.severity}`) : "-"}</span>
                  <span>{defect.description}</span>
                </div>
                {defect.resolutionNote && <div className="mt-1 text-erp-text-muted">{t("resolutionLine", { name: defect.resolvedBy?.displayName ?? "-", note: defect.resolutionNote })}</div>}
                {defect.verifiedBy && <div className="text-erp-text-muted">{t("verifiedLine", { name: defect.verifiedBy.displayName ?? "-" })}</div>}
                {(defect.reopenCount ?? 0) > 0 && <div className="text-erp-text-muted">{t("reopenedLine", { count: defect.reopenCount ?? 0, reason: defect.lastReopenReason ?? "-" })}</div>}
                {canOperate && (
                  <div className="mt-2 flex flex-wrap gap-2">
                    {(defect.status === "open" || defect.status === "reopened") && status === "in_progress" && <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => open({ kind: "resolve-defect", defect })}>{t("resolve")}</Button>}
                    {defect.status === "resolved" && status === "in_progress" && <Button type="button" size="sm" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run({ kind: "verify-defect", defectId: defect.id ?? "" }, t("verified"))}>{t("verify")}</Button>}
                    {(defect.status === "resolved" || defect.status === "verified") && ["in_progress", "ready_for_handover"].includes(status) && <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => open({ kind: "reopen-defect", defect })}>{t("reopen")}</Button>}
                  </div>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>

      <Modal
        isOpen={dialog !== null}
        onClose={() => { if (!busy) setDialog(null); }}
        title={dialogTitle}
        description={dialog ? t(`dialogs.${dialog.kind}.description`) : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setDialog(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant={dialog?.kind === "cancel" ? "danger" : "primary"} className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirm()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <div className="space-y-4">
          {dialogError && <Alert variant="danger" onClose={() => setDialogError(null)}>{dialogError}</Alert>}
          {dialog?.kind === "handover" && (
            <>
              <Select label={t("outcome")} value={outcome} disabled={busy} options={[{ value: "accepted", label: t("outcomes.accepted") }, { value: "disputed", label: t("outcomes.disputed") }]} onChange={(event) => setOutcome(event.target.value)} />
              <Input label={t("signerName")} required value={signer} maxLength={200} disabled={busy} onChange={(event) => setSigner(event.target.value)} />
              {outcome === "accepted" && (
                <>
                  <Input type="number" min={0} max={120} step={1} label={t("warrantyMonths")} required value={months} disabled={busy} onChange={(event) => setMonths(event.target.value)} />
                  <Input type="date" label={t("handoverDate")} value={handoverDate} disabled={busy} onChange={(event) => setHandoverDate(event.target.value)} />
                </>
              )}
            </>
          )}
          {dialog?.kind === "report-defect" && (
            <Select label={t("severity")} value={severity} disabled={busy} options={DEFECT_SEVERITIES.map((value) => ({ value, label: t(`severities.${value}`) }))} onChange={(event) => setSeverity(event.target.value)} />
          )}
          <Input label={t(dialog?.kind === "handover" ? (outcome === "disputed" ? "disputeReason" : "handoverNote") : "text")} required={dialog?.kind !== "handover" || outcome === "disputed"} value={text} maxLength={500} disabled={busy} onChange={(event) => setText(event.target.value)} />
        </div>
      </Modal>
    </section>
  );
}
