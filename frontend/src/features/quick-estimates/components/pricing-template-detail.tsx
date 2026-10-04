"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
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
import { formatCurrency } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { PricingTemplateResponse, TemplateStepName } from "@/lib/api/api-client";
import { usePricingTemplate, usePricingTemplateMutations } from "../api/quick-estimate-queries";
import { isMeasurementRule, isTaxDisplay, isTemplateStatus, isWorkType, quickEstimateErrorCode, templateStatusVariant } from "../quick-estimate-status";
import { PricingTemplateEditor } from "./pricing-template-editor";

interface PricingTemplateDetailProps {
  templateId: string;
}

export function PricingTemplateDetail({ templateId }: PricingTemplateDetailProps) {
  const query = usePricingTemplate(templateId);
  const t = useTranslations("quickEstimates.templates");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;

  // Keyed by row version so local dialog state resets after every change.
  return <DetailView key={query.data.rowVersion} template={query.data} />;
}

function DetailView({ template }: { template: PricingTemplateResponse }) {
  const t = useTranslations("quickEstimates.templates");
  const tErrors = useTranslations("quickEstimates.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.PRICING_TEMPLATES_MANAGE);
  const canApprove = can(selectedMembership, PERMISSIONS.PRICING_TEMPLATES_APPROVE);
  const mutations = usePricingTemplateMutations();

  const status = template.status ?? "";
  const id = template.id ?? "";
  const rowVersion = template.rowVersion ?? "";
  const busy = mutations.step.isPending || mutations.decide.isPending || mutations.newVersion.isPending;
  const [editing, setEditing] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [decision, setDecision] = useState<"approved" | "returned" | null>(null);
  const [note, setNote] = useState("");

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? quickEstimateErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function step(name: TemplateStepName): Promise<void> {
    setMessage(null);
    try {
      await mutations.step.mutateAsync({ id, rowVersion, step: name });
      toast.success(t(`stepDone.${name}`));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function decide(): Promise<void> {
    if (!decision) return;
    setMessage(null);
    try {
      await mutations.decide.mutateAsync({ id, rowVersion, decision, note: note.trim() || null });
      toast.success(t(decision === "approved" ? "approvedDone" : "returnedDone"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
    setDecision(null);
  }

  async function newVersion(): Promise<void> {
    setMessage(null);
    try {
      const created = await mutations.newVersion.mutateAsync(id);
      toast.success(t("newVersionDone"));
      router.push(`/${locale}/pricing-templates/${created.id}`);
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  if (editing) return <PricingTemplateEditor template={template} onDone={() => setEditing(false)} />;

  const grades = template.grades ?? [];
  const complexities = template.complexities ?? [];
  const addOns = template.addOns ?? [];

  return (
    <section className="space-y-5">
      <PageHeader
        title={`${template.code ?? "-"} v${template.version ?? "-"}`}
        subtitle={template.name ?? "-"}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/pricing-templates` }, { label: template.code ?? "-" }]}
        actions={<StatusBadge label={isTemplateStatus(status) ? t(`statuses.${status}`) : "-"} variant={templateStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      {status === "calibration" && <Alert variant="warning">{t("calibrationNote")}</Alert>}

      <div className="erp-card space-y-3 p-5">
        <div className="flex flex-wrap gap-3">
          {canManage && status === "draft" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setEditing(true)}>{tCommon("actions.edit")}</Button>}
          {canManage && status === "draft" && <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.step.isPending} disabled={busy} onClick={() => void step("submit")}>{t("submit")}</Button>}
          {canApprove && status === "submitted" && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => { setNote(""); setDecision("approved"); }}>{t("approve")}</Button>}
          {canApprove && status === "submitted" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => { setNote(""); setDecision("returned"); }}>{t("return")}</Button>}
          {canApprove && status === "approved" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => void step("calibration")}>{t("startCalibration")}</Button>}
          {canApprove && (status === "approved" || status === "calibration") && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void step("activate")}>{t("activate")}</Button>}
          {canApprove && (status === "active" || status === "calibration") && <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => void step("disable")}>{t("disable")}</Button>}
          {canManage && (status === "active" || status === "disabled") && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => void newVersion()}>{t("newVersion")}</Button>}
        </div>
        <dl className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2 lg:grid-cols-4">
          <div><dt className="text-xs text-erp-text-muted">{t("workType")}</dt><dd className="font-medium">{isWorkType(template.workType) ? t(`workTypes.${template.workType}`) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("measurementRule")}</dt><dd className="font-medium">{isMeasurementRule(template.measurementRule) ? t(`measurementRules.${template.measurementRule}`) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("unitCode")}</dt><dd className="font-mono font-medium">{template.unitCode ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("taxDisplay")}</dt><dd className="font-medium">{isTaxDisplay(template.taxDisplay) ? t(`taxDisplays.${template.taxDisplay}`) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.referenceRate")}</dt><dd className="font-mono font-semibold">{formatCurrency(template.referenceRate, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.minimumCharge")}</dt><dd className="font-mono font-semibold">{formatCurrency(template.minimumCharge, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.directShareLimit")}</dt><dd className="font-mono font-semibold">{formatCurrency(template.directShareLimit, "THB", locale)}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.roundingStep")}</dt><dd className="font-mono font-medium">{template.roundingStep ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.baseRangeRate")}</dt><dd className="font-mono font-medium">{template.baseRangeRate ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.maxRangeRate")}</dt><dd className="font-mono font-medium">{template.maxRangeRate ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.validityDays")}</dt><dd className="font-mono font-medium">{template.validityDays ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("fields.taxRate")}</dt><dd className="font-mono font-medium">{template.taxRate ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{template.createdBy?.displayName ?? "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("decidedBy")}</dt><dd className="font-medium">{template.decidedBy?.displayName ?? "-"}</dd></div>
        </dl>
        {template.decisionNote && <p className="text-xs text-erp-text-muted">{t("decisionNoteLine", { note: template.decisionNote })}</p>}
      </div>

      <div className="erp-card space-y-2 p-5">
        <h3 className="text-sm font-bold text-erp-navy">{t("grades")}</h3>
        {grades.length === 0 ? <p className="text-xs text-erp-text-muted">-</p> : (
          <ul className="space-y-1 text-xs">{grades.map((g) => <li key={g.code} className="font-mono">{g.code} · {g.name} · ×{g.factor}</li>)}</ul>
        )}
        <h3 className="pt-2 text-sm font-bold text-erp-navy">{t("complexities")}</h3>
        {complexities.length === 0 ? <p className="text-xs text-erp-text-muted">-</p> : (
          <ul className="space-y-1 text-xs">{complexities.map((c) => <li key={c.code} className="font-mono">{c.code} · {c.name} · ×{c.factor} · {t("riskModifier")} {c.riskModifier}</li>)}</ul>
        )}
        <h3 className="pt-2 text-sm font-bold text-erp-navy">{t("addOns")}</h3>
        {addOns.length === 0 ? <p className="text-xs text-erp-text-muted">-</p> : (
          <ul className="space-y-1 text-xs">{addOns.map((a) => <li key={a.code} className="font-mono">{a.code} · {a.name} · {formatCurrency(a.amount, "THB", locale)}{a.perLine ? ` · ${t("perLine")}` : ""}</li>)}</ul>
        )}
      </div>

      <Modal
        isOpen={decision !== null}
        onClose={() => { if (!busy) setDecision(null); }}
        title={decision ? t(decision === "approved" ? "approveTitle" : "returnTitle") : undefined}
        description={t("decisionDescription")}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setDecision(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.decide.isPending} disabled={busy} onClick={() => void decide()}>{tCommon("actions.confirm")}</Button>
          </div>
        )}
      >
        <Input label={t("decisionNote")} value={note} maxLength={500} disabled={busy} onChange={(event) => setNote(event.target.value)} />
      </Modal>
    </section>
  );
}
