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
import type { EffectiveTemplateResponse, QuickEstimateDraftRequest, QuickEstimateResponse } from "@/lib/api/api-client";
import { useEffectiveTemplates, useQuickEstimate, useQuickEstimateMutations } from "../api/quick-estimate-queries";
import { CONFIDENCES, isQuickEstimateStatus, isShareDecision, isShareReason, quickEstimateErrorCode, quickEstimateStatusVariant, shareDecisionVariant } from "../quick-estimate-status";

interface MeasurementRow {
  lineId: string;
  workSubtype: string;
  widthM: string;
  heightM: string;
  depthM: string;
  quantity: string;
}

interface QuickEstimateWorkspaceProps {
  estimateId: string;
}

const EDITABLE_STATUSES = ["draft", "calculated", "returned"];

export function QuickEstimateWorkspace({ estimateId }: QuickEstimateWorkspaceProps) {
  const query = useQuickEstimate(estimateId);
  const templates = useEffectiveTemplates();
  const t = useTranslations("quickEstimates.estimates");

  if (query.isLoading || templates.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;

  // Keyed by row version so the form resets to the server state after every change.
  return <WorkspaceView key={query.data.rowVersion} estimate={query.data} templates={templates.data ?? []} />;
}

function numberOrNull(value: string): number | null {
  return value.trim() === "" ? null : Number(value);
}

function WorkspaceView({ estimate, templates }: { estimate: QuickEstimateResponse; templates: EffectiveTemplateResponse[] }) {
  const t = useTranslations("quickEstimates.estimates");
  const tErrors = useTranslations("quickEstimates.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canUpdate = can(selectedMembership, PERMISSIONS.QUICK_ESTIMATES_UPDATE);
  const canReview = can(selectedMembership, PERMISSIONS.QUICK_ESTIMATES_REVIEW);
  const canShare = can(selectedMembership, PERMISSIONS.QUICK_ESTIMATES_SHARE);
  const canConvert = can(selectedMembership, PERMISSIONS.QUICK_ESTIMATES_CONVERT);
  const mutations = useQuickEstimateMutations();

  const id = estimate.id ?? "";
  const rowVersion = estimate.rowVersion ?? "";
  const status = estimate.status ?? "";
  const calc = estimate.currentCalculation;
  const editable = canUpdate && EDITABLE_STATUSES.includes(status);

  const [templateId, setTemplateId] = useState(estimate.template?.id ?? "");
  const [propertyType, setPropertyType] = useState(estimate.propertyType ?? "");
  const [roomOrArea, setRoomOrArea] = useState(estimate.roomOrArea ?? "");
  const [gradeCode, setGradeCode] = useState(estimate.gradeCode ?? "");
  const [complexityCodes, setComplexityCodes] = useState<string[]>(estimate.complexityCodes ?? []);
  const [addOnCodes, setAddOnCodes] = useState<string[]>(estimate.addOnCodes ?? []);
  const [confidence, setConfidence] = useState(estimate.measurementConfidence ?? "high");
  const [customMaterial, setCustomMaterial] = useState(estimate.customMaterial ?? false);
  const [rows, setRows] = useState<MeasurementRow[]>(() =>
    (estimate.measurements ?? []).map((m) => ({
      lineId: m.lineId ?? crypto.randomUUID(), workSubtype: m.workSubtype ?? "",
      widthM: m.widthM == null ? "" : String(m.widthM), heightM: m.heightM == null ? "" : String(m.heightM),
      depthM: m.depthM == null ? "" : String(m.depthM), quantity: String(m.quantity ?? 1),
    })),
  );
  const [message, setMessage] = useState<string | null>(null);
  const [dialog, setDialog] = useState<"approved" | "returned" | "convert" | null>(null);
  const [reasonCode, setReasonCode] = useState("");
  const [surveyRevisionId, setSurveyRevisionId] = useState("");
  const [dialogError, setDialogError] = useState(false);
  // Keys are regenerated after each success so a retry replays but a new action does not.
  const actionKeyRef = useRef(crypto.randomUUID());

  const template = templates.find((item) => item.id === templateId);
  const busy = mutations.saveDraft.isPending || mutations.calculate.isPending || mutations.submitReview.isPending || mutations.decideReview.isPending || mutations.share.isPending || mutations.convert.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? quickEstimateErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function toggle(list: string[], code: string, setter: (next: string[]) => void): void {
    setter(list.includes(code) ? list.filter((item) => item !== code) : [...list, code]);
  }

  function patchRow(index: number, patch: Partial<MeasurementRow>): void {
    setRows((current) => current.map((row, i) => (i === index ? { ...row, ...patch } : row)));
  }

  function draftPayload(): QuickEstimateDraftRequest {
    return {
      templateId: templateId || null, propertyType: propertyType.trim(), roomOrArea: roomOrArea.trim(), gradeCode: gradeCode || null,
      complexityCodes, addOnCodes, measurementConfidence: confidence, customMaterial,
      measurements: rows.map((row) => ({
        lineId: row.lineId, workSubtype: row.workSubtype.trim() || null,
        widthM: numberOrNull(row.widthM), heightM: numberOrNull(row.heightM), depthM: numberOrNull(row.depthM), quantity: Number(row.quantity || "0"),
      })),
    };
  }

  async function run(action: () => Promise<unknown>, success: string): Promise<boolean> {
    setMessage(null);
    try {
      await action();
      actionKeyRef.current = crypto.randomUUID();
      toast.success(success);
      return true;
    } catch (err: unknown) {
      setMessage(describe(err));
      return false;
    }
  }

  async function saveAndCalculate(): Promise<void> {
    setMessage(null);
    try {
      const saved = await mutations.saveDraft.mutateAsync({ id, rowVersion, payload: draftPayload() });
      await mutations.calculate.mutateAsync({ id, rowVersion: saved.rowVersion ?? "" });
      toast.success(t("calculated"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function confirmDialog(): Promise<void> {
    if (!dialog) return;
    const version = estimate.currentCalculationVersion ?? 0;
    if (dialog === "convert") {
      if (!surveyRevisionId.trim()) { setDialogError(true); return; }
      const done = await run(() => mutations.convert.mutateAsync({ id, sourceVersion: version, siteSurveyRevisionId: surveyRevisionId.trim(), idempotencyKey: actionKeyRef.current }), t("convertedDone"));
      if (done) setDialog(null);
      return;
    }

    if (!reasonCode.trim()) { setDialogError(true); return; }
    const done = await run(() => mutations.decideReview.mutateAsync({ id, sourceVersion: version, decision: dialog, reasonCode: reasonCode.trim(), idempotencyKey: actionKeyRef.current }), t(dialog === "approved" ? "approvedDone" : "returnedDone"));
    if (done) setDialog(null);
  }

  function open(next: "approved" | "returned" | "convert"): void {
    setDialog(next);
    setReasonCode("");
    setSurveyRevisionId("");
    setDialogError(false);
  }

  const version = estimate.currentCalculationVersion ?? 0;
  const shareable = calc?.shareDecision === "shareable" || status === "approved";

  return (
    <section className="space-y-5">
      <PageHeader
        title={estimate.number ?? "-"}
        subtitle={estimate.roomOrArea || t("noRoom")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/quick-estimates` }, { label: estimate.number ?? "-" }]}
        actions={<StatusBadge label={isQuickEstimateStatus(status) ? t(`statuses.${status}`) : "-"} variant={quickEstimateStatusVariant(status)} />}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      <Alert variant="info">{t("indicativeNote")}</Alert>

      <div className="erp-card space-y-4 p-5">
        <h3 className="text-sm font-bold text-erp-navy">{t("sectionInput")}</h3>
        <div className="grid grid-cols-1 gap-3 md:grid-cols-3">
          <Select label={t("template")} value={templateId} disabled={!editable || busy} options={[{ value: "", label: t("selectTemplate") }, ...templates.map((item) => ({ value: item.id ?? "", label: `${item.code ?? ""} v${item.version ?? ""} · ${item.name ?? ""}` }))]} onChange={(event) => { setTemplateId(event.target.value); setGradeCode(""); setComplexityCodes([]); setAddOnCodes([]); }} />
          <Input label={t("propertyType")} value={propertyType} maxLength={100} disabled={!editable || busy} onChange={(event) => setPropertyType(event.target.value)} />
          <Input label={t("roomOrArea")} value={roomOrArea} maxLength={200} disabled={!editable || busy} onChange={(event) => setRoomOrArea(event.target.value)} />
          <Select label={t("grade")} value={gradeCode} disabled={!editable || busy || !template} options={[{ value: "", label: t("selectGrade") }, ...(template?.grades ?? []).map((grade) => ({ value: grade.code ?? "", label: grade.name ?? "" }))]} onChange={(event) => setGradeCode(event.target.value)} />
          <Select label={t("confidence")} value={confidence} disabled={!editable || busy} options={CONFIDENCES.map((value) => ({ value, label: t(`confidences.${value}`) }))} onChange={(event) => setConfidence(event.target.value)} />
          <label className="flex min-h-11 items-center gap-2 self-end text-sm"><input type="checkbox" checked={customMaterial} disabled={!editable || busy} onChange={(event) => setCustomMaterial(event.target.checked)} />{t("customMaterial")}</label>
        </div>
        {template && (template.complexities ?? []).length > 0 && (
          <fieldset className="space-y-1"><legend className="text-xs font-semibold text-erp-text-muted">{t("complexities")}</legend>
            <div className="flex flex-wrap gap-4">{(template.complexities ?? []).map((item) => (
              <label key={item.code} className="flex min-h-11 items-center gap-2 text-sm"><input type="checkbox" checked={complexityCodes.includes(item.code ?? "")} disabled={!editable || busy} onChange={() => toggle(complexityCodes, item.code ?? "", setComplexityCodes)} />{item.name}</label>
            ))}</div>
          </fieldset>
        )}
        {template && (template.addOns ?? []).length > 0 && (
          <fieldset className="space-y-1"><legend className="text-xs font-semibold text-erp-text-muted">{t("addOns")}</legend>
            <div className="flex flex-wrap gap-4">{(template.addOns ?? []).map((item) => (
              <label key={item.code} className="flex min-h-11 items-center gap-2 text-sm"><input type="checkbox" checked={addOnCodes.includes(item.code ?? "")} disabled={!editable || busy} onChange={() => toggle(addOnCodes, item.code ?? "", setAddOnCodes)} />{item.name}{item.perLine ? ` (${t("perLine")})` : ""}</label>
            ))}</div>
          </fieldset>
        )}

        <div className="flex items-center justify-between pt-2">
          <h3 className="text-sm font-bold text-erp-navy">{t("measurements")}</h3>
          {editable && <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => setRows([...rows, { lineId: crypto.randomUUID(), workSubtype: "", widthM: "", heightM: "", depthM: "", quantity: "1" }])}>{t("addMeasurement")}</Button>}
        </div>
        {rows.map((row, index) => (
          <div key={row.lineId} className="grid grid-cols-2 items-end gap-3 md:grid-cols-6">
            <Input label={t("workSubtype")} value={row.workSubtype} disabled={!editable || busy} onChange={(event) => patchRow(index, { workSubtype: event.target.value })} />
            <Input type="number" step="0.01" label={t("widthM")} value={row.widthM} disabled={!editable || busy} onChange={(event) => patchRow(index, { widthM: event.target.value })} />
            <Input type="number" step="0.01" label={t("heightM")} value={row.heightM} disabled={!editable || busy} onChange={(event) => patchRow(index, { heightM: event.target.value })} />
            <Input type="number" step="0.01" label={t("depthM")} value={row.depthM} disabled={!editable || busy} onChange={(event) => patchRow(index, { depthM: event.target.value })} />
            <Input type="number" step="1" label={t("quantity")} value={row.quantity} disabled={!editable || busy} onChange={(event) => patchRow(index, { quantity: event.target.value })} />
            {editable && <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => setRows(rows.filter((_row, i) => i !== index))}>{t("removeRow")}</Button>}
          </div>
        ))}
        {editable && (
          <div className="flex gap-3">
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.saveDraft.isPending || mutations.calculate.isPending} disabled={busy} onClick={() => void saveAndCalculate()}>{t("saveAndCalculate")}</Button>
          </div>
        )}
      </div>

      {calc && (
        <div className="erp-card space-y-3 p-5" role="region" aria-label={t("resultTitle")}>
          <h3 className="text-sm font-bold text-erp-navy">{t("resultTitle")} · {t("version", { version: calc.version ?? 0 })}</h3>
          <p className="font-mono text-xl font-bold text-erp-navy">{formatCurrency(calc.displayedLower, "THB", locale)} – {formatCurrency(calc.displayedUpper, "THB", locale)}</p>
          <dl className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2 lg:grid-cols-4">
            <div><dt className="text-xs text-erp-text-muted">{t("netAmount")}</dt><dd className="font-mono font-semibold">{formatCurrency(calc.netAmount, "THB", locale)}</dd></div>
            <div><dt className="text-xs text-erp-text-muted">{t("taxAmount")}</dt><dd className="font-mono font-semibold">{formatCurrency(calc.taxAmount, "THB", locale)}</dd></div>
            <div><dt className="text-xs text-erp-text-muted">{t("validUntil")}</dt><dd className="font-medium">{calc.validUntil ? formatDate(calc.validUntil, locale) : "-"}</dd></div>
            <div><dt className="text-xs text-erp-text-muted">{t("shareDecision")}</dt><dd>{isShareDecision(calc.shareDecision) ? <StatusBadge label={t(`shareDecisions.${calc.shareDecision}`)} variant={shareDecisionVariant(calc.shareDecision)} /> : "-"}</dd></div>
          </dl>
          {(calc.reasonCodes ?? []).length > 0 && (
            <ul className="space-y-1 text-xs text-erp-text-muted">{(calc.reasonCodes ?? []).map((reason) => <li key={reason}>{isShareReason(reason) ? t(`shareReasons.${reason}`) : reason}</li>)}</ul>
          )}
          <ul className="space-y-1 text-xs">{(calc.lines ?? []).map((line) => (
            <li key={line.lineId} className="font-mono">{line.workSubtype || "-"} · {t("billableQuantity")} {line.billableQuantity} · {formatCurrency(line.adjustedAmount, "THB", locale)}</li>
          ))}</ul>
          <p className="font-mono text-xs text-erp-text-muted" title={calc.inputHash ?? undefined}>{t("inputHash")}: {(calc.inputHash ?? "").slice(0, 16)}</p>
        </div>
      )}

      <div className="erp-card space-y-3 p-5" role="region" aria-label={t("workflowTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("workflowTitle")}</h3>
        <div className="flex flex-wrap gap-3">
          {canUpdate && status === "calculated" && <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.submitReview.isPending} disabled={busy} onClick={() => void run(() => mutations.submitReview.mutateAsync({ id, sourceVersion: version, idempotencyKey: actionKeyRef.current }), t("submittedDone"))}>{t("submitReview")}</Button>}
          {canReview && status === "pending_review" && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => open("approved")}>{t("approve")}</Button>}
          {canReview && status === "pending_review" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("returned")}>{t("return")}</Button>}
          {canShare && shareable && status !== "draft" && status !== "converted" && <Button type="button" variant="outline" className="min-h-11" isLoading={mutations.share.isPending} disabled={busy} onClick={() => void run(() => mutations.share.mutateAsync({ id, sourceVersion: version, idempotencyKey: actionKeyRef.current }), t("sharedDone"))}>{t("share")}</Button>}
          {canConvert && (status === "calculated" || status === "approved") && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("convert")}>{t("convert")}</Button>}
        </div>
        {(estimate.reviews ?? []).map((review) => (
          <p key={review.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">{t("reviewLine", { version: review.sourceVersion ?? 0, status: review.status ?? "-", by: review.decidedBy?.displayName ?? review.requestedBy?.displayName ?? "-" })}{review.reasonCode ? ` · ${review.reasonCode}` : ""}</p>
        ))}
        {(estimate.shares ?? []).map((share) => (
          <p key={share.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">{t("shareLine", { version: share.sourceVersion ?? 0, by: share.createdBy?.displayName ?? "-" })}</p>
        ))}
        {(estimate.conversions ?? []).map((conversion) => (
          <p key={conversion.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
            {t("conversionLine", { version: conversion.sourceVersion ?? 0 })}{" "}
            <Link href={`/${locale}/estimates/${conversion.officialEstimateId}`} className="font-mono text-erp-navy underline">{t("openEstimate")}</Link>
          </p>
        ))}
      </div>

      <Modal
        isOpen={dialog !== null}
        onClose={() => { if (!busy) setDialog(null); }}
        title={dialog ? t(dialog === "convert" ? "convertTitle" : dialog === "approved" ? "approveTitle" : "returnTitle") : undefined}
        description={dialog ? t(dialog === "convert" ? "convertDescription" : "decisionDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setDialog(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirmDialog()}>{tCommon("actions.confirm")}</Button>
          </div>
        )}
      >
        {dialog === "convert" ? (
          <Input label={t("surveyRevisionId")} required value={surveyRevisionId} disabled={busy} error={dialogError ? t("surveyRevisionRequired") : undefined} onChange={(event) => { setSurveyRevisionId(event.target.value); setDialogError(false); }} />
        ) : (
          <Input label={t("reasonCode")} required value={reasonCode} maxLength={64} disabled={busy} error={dialogError ? t("reasonCodeRequired") : undefined} onChange={(event) => { setReasonCode(event.target.value); setDialogError(false); }} />
        )}
      </Modal>
    </section>
  );
}

