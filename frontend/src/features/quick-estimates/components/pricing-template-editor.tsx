"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { PricingTemplateRequest, PricingTemplateResponse } from "@/lib/api/api-client";
import { usePricingTemplateMutations } from "../api/quick-estimate-queries";
import { MEASUREMENT_RULES, TAX_DISPLAYS, WORK_TYPES, quickEstimateErrorCode } from "../quick-estimate-status";

interface OptionRow {
  code: string;
  name: string;
  factor: string;
  riskModifier: string;
}

interface AddOnRow {
  code: string;
  name: string;
  amount: string;
  perLine: boolean;
}

interface FormState {
  code: string;
  workType: string;
  name: string;
  measurementRule: string;
  unitCode: string;
  referenceRate: string;
  minimumCharge: string;
  baseRangeRate: string;
  maxRangeRate: string;
  roundingStep: string;
  validityDays: string;
  taxRate: string;
  taxDisplay: string;
  directShareLimit: string;
  effectiveFrom: string;
  effectiveTo: string;
  lowConfidenceModifier: string;
  mediumConfidenceModifier: string;
  customMaterialModifier: string;
  grades: OptionRow[];
  complexities: OptionRow[];
  addOns: AddOnRow[];
  assumptions: string;
  exclusions: string;
}

// The business supplies every rate and factor: the empty form carries no invented prices.
const EMPTY: FormState = {
  code: "", workType: "built-in", name: "", measurementRule: "area", unitCode: "",
  referenceRate: "", minimumCharge: "", baseRangeRate: "", maxRangeRate: "", roundingStep: "", validityDays: "",
  taxRate: "", taxDisplay: "exclusive", directShareLimit: "", effectiveFrom: "", effectiveTo: "",
  lowConfidenceModifier: "", mediumConfidenceModifier: "", customMaterialModifier: "",
  grades: [], complexities: [], addOns: [], assumptions: "", exclusions: "",
};

function fromTemplate(template: PricingTemplateResponse): FormState {
  const num = (value: number | undefined) => (value === undefined ? "" : String(value));
  return {
    code: template.code ?? "", workType: template.workType ?? "built-in", name: template.name ?? "",
    measurementRule: template.measurementRule ?? "area", unitCode: template.unitCode ?? "",
    referenceRate: num(template.referenceRate), minimumCharge: num(template.minimumCharge),
    baseRangeRate: num(template.baseRangeRate), maxRangeRate: num(template.maxRangeRate),
    roundingStep: num(template.roundingStep), validityDays: num(template.validityDays),
    taxRate: num(template.taxRate), taxDisplay: template.taxDisplay ?? "exclusive",
    directShareLimit: num(template.directShareLimit),
    effectiveFrom: template.effectiveFrom ?? "", effectiveTo: template.effectiveTo ?? "",
    lowConfidenceModifier: num(template.lowConfidenceModifier), mediumConfidenceModifier: num(template.mediumConfidenceModifier),
    customMaterialModifier: num(template.customMaterialModifier),
    grades: (template.grades ?? []).map((g) => ({ code: g.code ?? "", name: g.name ?? "", factor: num(g.factor), riskModifier: "" })),
    complexities: (template.complexities ?? []).map((c) => ({ code: c.code ?? "", name: c.name ?? "", factor: num(c.factor), riskModifier: num(c.riskModifier) })),
    addOns: (template.addOns ?? []).map((a) => ({ code: a.code ?? "", name: a.name ?? "", amount: num(a.amount), perLine: a.perLine ?? false })),
    assumptions: (template.assumptions ?? []).join("\n"),
    exclusions: (template.exclusions ?? []).join("\n"),
  };
}

function lines(value: string): string[] {
  return value.split("\n").map((line) => line.trim()).filter((line) => line.length > 0);
}

function toRequest(form: FormState): PricingTemplateRequest {
  const n = (value: string) => Number(value);
  return {
    code: form.code.trim(), workType: form.workType, name: form.name.trim(), measurementRule: form.measurementRule, unitCode: form.unitCode.trim(),
    referenceRate: n(form.referenceRate), minimumCharge: n(form.minimumCharge), baseRangeRate: n(form.baseRangeRate), maxRangeRate: n(form.maxRangeRate),
    roundingStep: n(form.roundingStep), validityDays: n(form.validityDays), taxRate: n(form.taxRate), taxDisplay: form.taxDisplay,
    directShareLimit: n(form.directShareLimit), effectiveFrom: form.effectiveFrom || null, effectiveTo: form.effectiveTo || null,
    grades: form.grades.map((g) => ({ code: g.code.trim(), name: g.name.trim(), factor: n(g.factor) })),
    complexities: form.complexities.map((c) => ({ code: c.code.trim(), name: c.name.trim(), factor: n(c.factor), riskModifier: n(c.riskModifier || "0") })),
    addOns: form.addOns.map((a) => ({ code: a.code.trim(), name: a.name.trim(), amount: n(a.amount), perLine: a.perLine })),
    lowConfidenceModifier: n(form.lowConfidenceModifier), mediumConfidenceModifier: n(form.mediumConfidenceModifier), customMaterialModifier: n(form.customMaterialModifier),
    assumptions: lines(form.assumptions), exclusions: lines(form.exclusions),
  };
}

const REQUIRED_NUMBERS = ["referenceRate", "minimumCharge", "baseRangeRate", "maxRangeRate", "roundingStep", "validityDays", "taxRate", "directShareLimit", "lowConfidenceModifier", "mediumConfidenceModifier", "customMaterialModifier"] as const;

function isComplete(form: FormState): boolean {
  if (!form.code.trim() || !form.name.trim() || !form.unitCode.trim() || form.grades.length === 0) return false;
  if (REQUIRED_NUMBERS.some((key) => form[key].trim() === "" || Number.isNaN(Number(form[key])))) return false;
  if (form.grades.some((g) => !g.code.trim() || !g.name.trim() || !(Number(g.factor) > 0))) return false;
  if (form.complexities.some((c) => !c.code.trim() || !c.name.trim() || !(Number(c.factor) > 0))) return false;
  return !form.addOns.some((a) => !a.code.trim() || !a.name.trim() || a.amount.trim() === "" || Number.isNaN(Number(a.amount)));
}

interface PricingTemplateEditorProps {
  template?: PricingTemplateResponse;
  onDone?: () => void;
}

export function PricingTemplateEditor({ template, onDone }: PricingTemplateEditorProps) {
  const t = useTranslations("quickEstimates.templates");
  const tErrors = useTranslations("quickEstimates.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = usePricingTemplateMutations();
  const [form, setForm] = useState<FormState>(() => (template ? fromTemplate(template) : EMPTY));
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second template.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending || mutations.update.isPending;

  function set<K extends keyof FormState>(key: K, value: FormState[K]): void {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function patchRow<K extends "grades" | "complexities">(key: K, index: number, patch: Partial<OptionRow>): void {
    setForm((current) => ({ ...current, [key]: current[key].map((row, i) => (i === index ? { ...row, ...patch } : row)) }));
  }

  function patchAddOn(index: number, patch: Partial<AddOnRow>): void {
    setForm((current) => ({ ...current, addOns: current.addOns.map((row, i) => (i === index ? { ...row, ...patch } : row)) }));
  }

  async function save(): Promise<void> {
    setError(null);
    if (!isComplete(form)) {
      setError(t("fieldsInvalid"));
      return;
    }

    try {
      const payload = toRequest(form);
      if (template) {
        await mutations.update.mutateAsync({ id: template.id ?? "", rowVersion: template.rowVersion ?? "", payload });
        toast.success(t("saved"));
        onDone?.();
      } else {
        const saved = await mutations.create.mutateAsync({ payload, idempotencyKey: createKeyRef.current });
        toast.success(t("created"));
        router.push(`/${locale}/pricing-templates/${saved.id}`);
      }
    } catch (err: unknown) {
      const code = err instanceof ApiError ? quickEstimateErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  const numberField = (key: (typeof REQUIRED_NUMBERS)[number], step = "0.01") => (
    <Input type="number" step={step} label={t(`fields.${key}`)} value={form[key]} disabled={isBusy} placeholder={t(`fields.${key}Placeholder`)} onChange={(event) => set(key, event.target.value)} />
  );

  return (
    <section className="space-y-5">
      {!template && (
        <PageHeader
          title={t("createTitle")}
          subtitle={t("editorSubtitle")}
          breadcrumbs={[{ label: t("title"), href: `/${locale}/pricing-templates` }, { label: t("createTitle") }]}
        />
      )}
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <div className="erp-card space-y-4 p-5">
        <h3 className="text-sm font-bold text-erp-navy">{t("sectionBasics")}</h3>
        <div className="grid grid-cols-1 gap-3 md:grid-cols-3">
          <Input label={t("code")} required value={form.code} maxLength={40} disabled={isBusy || Boolean(template)} placeholder={t("codePlaceholder")} onChange={(event) => set("code", event.target.value)} />
          <Input label={t("name")} required value={form.name} maxLength={200} disabled={isBusy} placeholder={t("namePlaceholder")} onChange={(event) => set("name", event.target.value)} />
          <Select label={t("workType")} value={form.workType} disabled={isBusy} options={WORK_TYPES.map((value) => ({ value, label: t(`workTypes.${value}`) }))} onChange={(event) => set("workType", event.target.value)} />
          <Select label={t("measurementRule")} value={form.measurementRule} disabled={isBusy} options={MEASUREMENT_RULES.map((value) => ({ value, label: t(`measurementRules.${value}`) }))} onChange={(event) => set("measurementRule", event.target.value)} />
          <Input label={t("unitCode")} required value={form.unitCode} maxLength={20} disabled={isBusy} placeholder={t("unitCodePlaceholder")} onChange={(event) => set("unitCode", event.target.value)} />
          <Select label={t("taxDisplay")} value={form.taxDisplay} disabled={isBusy} options={TAX_DISPLAYS.map((value) => ({ value, label: t(`taxDisplays.${value}`) }))} onChange={(event) => set("taxDisplay", event.target.value)} />
          <Input type="date" label={t("effectiveFrom")} value={form.effectiveFrom} disabled={isBusy} onChange={(event) => set("effectiveFrom", event.target.value)} />
          <Input type="date" label={t("effectiveTo")} value={form.effectiveTo} disabled={isBusy} onChange={(event) => set("effectiveTo", event.target.value)} />
        </div>
      </div>

      <div className="erp-card space-y-4 p-5">
        <h3 className="text-sm font-bold text-erp-navy">{t("sectionPricing")}</h3>
        <p className="text-xs text-erp-text-muted">{t("pricingNote")}</p>
        <div className="grid grid-cols-1 gap-3 md:grid-cols-4">
          {numberField("referenceRate")}
          {numberField("minimumCharge")}
          {numberField("roundingStep")}
          {numberField("validityDays", "1")}
          {numberField("taxRate", "0.0001")}
          {numberField("directShareLimit")}
          {numberField("baseRangeRate", "0.0001")}
          {numberField("maxRangeRate", "0.0001")}
          {numberField("lowConfidenceModifier", "0.0001")}
          {numberField("mediumConfidenceModifier", "0.0001")}
          {numberField("customMaterialModifier", "0.0001")}
        </div>
      </div>

      <div className="erp-card space-y-3 p-5">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-bold text-erp-navy">{t("grades")}</h3>
          <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => set("grades", [...form.grades, { code: "", name: "", factor: "", riskModifier: "" }])}>{t("addRow")}</Button>
        </div>
        {form.grades.map((row, index) => (
          <div key={index} className="grid grid-cols-1 items-end gap-3 md:grid-cols-4">
            <Input label={t("rowCode")} value={row.code} disabled={isBusy} placeholder={t("rowCodePlaceholder")} onChange={(event) => patchRow("grades", index, { code: event.target.value })} />
            <Input label={t("rowName")} value={row.name} disabled={isBusy} placeholder={t("rowNamePlaceholder")} onChange={(event) => patchRow("grades", index, { name: event.target.value })} />
            <Input type="number" step="0.0001" label={t("factor")} value={row.factor} disabled={isBusy} placeholder={t("factorPlaceholder")} onChange={(event) => patchRow("grades", index, { factor: event.target.value })} />
            <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => set("grades", form.grades.filter((_row, i) => i !== index))}>{t("removeRow")}</Button>
          </div>
        ))}
      </div>

      <div className="erp-card space-y-3 p-5">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-bold text-erp-navy">{t("complexities")}</h3>
          <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => set("complexities", [...form.complexities, { code: "", name: "", factor: "", riskModifier: "" }])}>{t("addRow")}</Button>
        </div>
        {form.complexities.map((row, index) => (
          <div key={index} className="grid grid-cols-1 items-end gap-3 md:grid-cols-5">
            <Input label={t("rowCode")} value={row.code} disabled={isBusy} placeholder={t("rowCodePlaceholder")} onChange={(event) => patchRow("complexities", index, { code: event.target.value })} />
            <Input label={t("rowName")} value={row.name} disabled={isBusy} placeholder={t("rowNamePlaceholder")} onChange={(event) => patchRow("complexities", index, { name: event.target.value })} />
            <Input type="number" step="0.0001" label={t("factor")} value={row.factor} disabled={isBusy} placeholder={t("factorPlaceholder")} onChange={(event) => patchRow("complexities", index, { factor: event.target.value })} />
            <Input type="number" step="0.0001" label={t("riskModifier")} value={row.riskModifier} disabled={isBusy} placeholder={t("riskModifierPlaceholder")} onChange={(event) => patchRow("complexities", index, { riskModifier: event.target.value })} />
            <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => set("complexities", form.complexities.filter((_row, i) => i !== index))}>{t("removeRow")}</Button>
          </div>
        ))}
      </div>

      <div className="erp-card space-y-3 p-5">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-bold text-erp-navy">{t("addOns")}</h3>
          <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => set("addOns", [...form.addOns, { code: "", name: "", amount: "", perLine: false }])}>{t("addRow")}</Button>
        </div>
        {form.addOns.map((row, index) => (
          <div key={index} className="grid grid-cols-1 items-end gap-3 md:grid-cols-5">
            <Input label={t("rowCode")} value={row.code} disabled={isBusy} placeholder={t("rowCodePlaceholder")} onChange={(event) => patchAddOn(index, { code: event.target.value })} />
            <Input label={t("rowName")} value={row.name} disabled={isBusy} placeholder={t("rowNamePlaceholder")} onChange={(event) => patchAddOn(index, { name: event.target.value })} />
            <Input type="number" step="0.01" label={t("amount")} value={row.amount} disabled={isBusy} placeholder={t("amountPlaceholder")} onChange={(event) => patchAddOn(index, { amount: event.target.value })} />
            <div className="flex min-h-11 items-center">
              <Checkbox checked={row.perLine} disabled={isBusy} onCheckedChange={(checked) => patchAddOn(index, { perLine: checked === true })} label={t("perLine")} />
            </div>
            <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => set("addOns", form.addOns.filter((_row, i) => i !== index))}>{t("removeRow")}</Button>
          </div>
        ))}
      </div>

      <div className="erp-card grid grid-cols-1 gap-3 p-5 md:grid-cols-2">
        <Textarea label={t("assumptions")} helperText={t("onePerLine")} value={form.assumptions} disabled={isBusy} placeholder={t("assumptionsPlaceholder")} onChange={(event) => set("assumptions", event.target.value)} />
        <Textarea label={t("exclusions")} helperText={t("onePerLine")} value={form.exclusions} disabled={isBusy} placeholder={t("exclusionsPlaceholder")} onChange={(event) => set("exclusions", event.target.value)} />
      </div>

      <div className="flex gap-3">
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => (onDone ? onDone() : router.push(`/${locale}/pricing-templates`))}>{tCommon("actions.cancel")}</Button>
      </div>
    </section>
  );
}
