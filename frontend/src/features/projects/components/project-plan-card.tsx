"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { useToast } from "@/hooks/useToast";
import { formatDate } from "@/lib/formatters/formatters";
import type { ProjectControlResponse } from "@/lib/api/api-client";
import type { useProjectControlMutations } from "../api/project-queries";

interface ProjectPlanCardProps {
  control: ProjectControlResponse;
  canEdit: boolean;
  mutation: ReturnType<typeof useProjectControlMutations>["setPlan"];
  describeError: (error: unknown) => string;
}

const EDITABLE_STATUSES = ["planned", "active", "on_hold"];

export function ProjectPlanCard({ control, canEdit, mutation, describeError }: ProjectPlanCardProps) {
  const t = useTranslations("projects.control");
  const locale = useLocale();
  const { toast } = useToast();
  const [start, setStart] = useState(control.plannedStartDate ?? "");
  const [end, setEnd] = useState(control.plannedEndDate ?? "");
  const [error, setError] = useState<string | null>(null);

  const editable = canEdit && EDITABLE_STATUSES.includes(control.status ?? "");
  const isDirty = start !== (control.plannedStartDate ?? "") || end !== (control.plannedEndDate ?? "");

  async function save(): Promise<void> {
    setError(null);
    try {
      await mutation.mutateAsync({
        rowVersion: control.rowVersion ?? "",
        payload: { plannedStartDate: start || null, plannedEndDate: end || null },
      });
      toast.success(t("planSaved"));
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  return (
    <div className="erp-card p-5 space-y-3" role="region" aria-label={t("planTitle")}>
      <h3 className="text-sm font-bold text-erp-navy">{t("planTitle")}</h3>
      {editable ? (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-3 sm:items-end">
          <Input type="date" label={t("plannedStart")} value={start} disabled={mutation.isPending} onChange={(e) => setStart(e.target.value)} />
          <Input type="date" label={t("plannedEnd")} value={end} disabled={mutation.isPending} onChange={(e) => setEnd(e.target.value)} />
          <Button type="button" variant="primary" className="min-h-11" isLoading={mutation.isPending} disabled={!isDirty || mutation.isPending} onClick={() => void save()}>
            {t("savePlan")}
          </Button>
        </div>
      ) : (
        <dl className="grid grid-cols-2 gap-3 text-sm">
          <div><dt className="text-xs text-erp-text-muted">{t("plannedStart")}</dt><dd className="font-medium">{control.plannedStartDate ? formatDate(control.plannedStartDate, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("plannedEnd")}</dt><dd className="font-medium">{control.plannedEndDate ? formatDate(control.plannedEndDate, locale) : "-"}</dd></div>
        </dl>
      )}
      {error && <p className="text-xs text-erp-danger" role="alert">{error}</p>}
    </div>
  );
}
