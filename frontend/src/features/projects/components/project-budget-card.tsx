"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { useToast } from "@/hooks/useToast";
import { formatCurrency } from "@/lib/formatters/formatters";
import type { ProjectBudgetLineRequest, ProjectControlResponse } from "@/lib/api/api-client";
import type { useProjectControlMutations } from "../api/project-queries";
import { isBudgetCategory, PROJECT_BUDGET_CATEGORIES, type ProjectBudgetCategoryValue } from "../project-status";

interface ProjectBudgetCardProps {
  control: ProjectControlResponse;
  canEdit: boolean;
  mutation: ReturnType<typeof useProjectControlMutations>["replaceBudget"];
  describeError: (error: unknown) => string;
}

interface DraftLine {
  key: string;
  category: ProjectBudgetCategoryValue;
  description: string;
  amount: string;
}

function toDraft(control: ProjectControlResponse): DraftLine[] {
  return (control.budget?.lines ?? []).map((line) => ({
    key: line.id ?? crypto.randomUUID(),
    category: isBudgetCategory(line.category) ? line.category : "other",
    description: line.description ?? "",
    amount: String(line.amount ?? 0),
  }));
}

export function ProjectBudgetCard({ control, canEdit, mutation, describeError }: ProjectBudgetCardProps) {
  const t = useTranslations("projects.control");
  const locale = useLocale();
  const { toast } = useToast();
  const budget = control.budget;
  const editable = canEdit && control.status === "planned" && !budget?.isFrozen;

  const [lines, setLines] = useState<DraftLine[]>(() => toDraft(control));
  const [error, setError] = useState<string | null>(null);

  const draftTotal = lines.reduce((sum, line) => sum + (Number(line.amount) || 0), 0);

  function update(key: string, patch: Partial<DraftLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  async function save(): Promise<void> {
    setError(null);
    if (lines.length === 0 || lines.some((line) => !line.description.trim() || !Number.isFinite(Number(line.amount)) || Number(line.amount) < 0)) {
      setError(t("budgetInvalid"));
      return;
    }

    const payload: ProjectBudgetLineRequest[] = lines.map((line) => ({
      category: line.category,
      description: line.description.trim(),
      amount: Number(line.amount),
    }));
    try {
      await mutation.mutateAsync({ rowVersion: control.rowVersion ?? "", lines: payload });
      toast.success(t("budgetSaved"));
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  return (
    <div className="erp-card p-5 space-y-4" role="region" aria-label={t("budgetTitle")}>
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("budgetTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{budget?.isFrozen ? t("budgetFrozenHint") : t("budgetEditableHint")}</p>
        </div>
        <dl className="grid grid-cols-3 gap-4 text-right text-xs">
          <div><dt className="text-erp-text-muted">{t("baselineBudget")}</dt><dd className="font-mono font-semibold">{budget?.baselineTotal != null ? formatCurrency(budget.baselineTotal, "THB", locale) : "-"}</dd></div>
          <div><dt className="text-erp-text-muted">{t("approvedBudgetDelta")}</dt><dd className="font-mono font-semibold">{formatCurrency(budget?.approvedBudgetDelta ?? 0, "THB", locale)}</dd></div>
          <div><dt className="text-erp-text-muted">{t("currentBudget")}</dt><dd className="font-mono font-semibold">{budget?.currentTotal != null ? formatCurrency(budget.currentTotal, "THB", locale) : "-"}</dd></div>
        </dl>
      </div>

      <div className="space-y-2">
        {lines.map((line) => (
          <div key={line.key} className="grid grid-cols-1 gap-2 sm:grid-cols-[160px_1fr_160px_auto] sm:items-end">
            <Select
              aria-label={t("budgetCategory")}
              value={line.category}
              disabled={!editable || mutation.isPending}
              options={PROJECT_BUDGET_CATEGORIES.map((value) => ({ value, label: t(`budgetCategories.${value}`) }))}
              onChange={(e) => { const next = e.target.value; if (isBudgetCategory(next)) update(line.key, { category: next }); }}
            />
            <Input aria-label={t("budgetDescription")} placeholder={t("budgetDescription")} value={line.description} maxLength={200} disabled={!editable || mutation.isPending} onChange={(e) => update(line.key, { description: e.target.value })} />
            <Input aria-label={t("budgetAmount")} placeholder={t("budgetAmount")} type="number" min={0} step="0.01" value={line.amount} disabled={!editable || mutation.isPending} onChange={(e) => update(line.key, { amount: e.target.value })} />
            {editable && (
              <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={mutation.isPending} onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}>
                {t("removeLine")}
              </Button>
            )}
          </div>
        ))}
        {lines.length === 0 && <p className="text-xs text-erp-text-muted">{t("budgetEmpty")}</p>}
      </div>

      {editable && (
        <div className="flex flex-wrap items-center justify-between gap-3">
          <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={mutation.isPending} onClick={() => setLines((current) => [...current, { key: crypto.randomUUID(), category: "material", description: "", amount: "0" }])}>
            {t("addLine")}
          </Button>
          <div className="flex items-center gap-3">
            <span className="font-mono text-xs text-erp-text-muted">{t("draftTotal", { amount: formatCurrency(draftTotal, "THB", locale) })}</span>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutation.isPending} disabled={mutation.isPending} onClick={() => void save()}>
              {t("saveBudget")}
            </Button>
          </div>
        </div>
      )}
      {error && <p className="text-xs text-erp-danger" role="alert">{error}</p>}
    </div>
  );
}
