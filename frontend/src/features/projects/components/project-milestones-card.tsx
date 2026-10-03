"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { formatDate, formatDateTime } from "@/lib/formatters/formatters";
import type { ProjectControlResponse, ProjectMilestoneResponse } from "@/lib/api/api-client";
import type { useProjectControlMutations } from "../api/project-queries";

interface ProjectMilestonesCardProps {
  control: ProjectControlResponse;
  canEdit: boolean;
  mutations: ReturnType<typeof useProjectControlMutations>;
  describeError: (error: unknown) => string;
}

const EDITABLE_STATUSES = ["planned", "active", "on_hold"];

export function ProjectMilestonesCard({ control, canEdit, mutations, describeError }: ProjectMilestonesCardProps) {
  const t = useTranslations("projects.control");
  const locale = useLocale();
  const { toast } = useToast();
  const [name, setName] = useState("");
  const [plannedDate, setPlannedDate] = useState("");
  const [weight, setWeight] = useState("1");
  const [error, setError] = useState<string | null>(null);

  const status = control.status ?? "";
  const editable = canEdit && EDITABLE_STATUSES.includes(status);
  const canComplete = canEdit && status === "active";
  const progress = control.progress;
  const milestones: ProjectMilestoneResponse[] = control.milestones ?? [];
  const busy = mutations.addMilestone.isPending || mutations.completeMilestone.isPending || mutations.deleteMilestone.isPending;

  async function run(action: () => Promise<unknown>, successKey?: "milestoneAdded" | "milestoneCompleted" | "milestoneRemoved"): Promise<boolean> {
    setError(null);
    try {
      await action();
      if (successKey) toast.success(t(successKey));
      return true;
    } catch (err: unknown) {
      setError(describeError(err));
      return false;
    }
  }

  async function add(): Promise<void> {
    const parsedWeight = Number(weight);
    if (!name.trim() || !Number.isInteger(parsedWeight) || parsedWeight < 1 || parsedWeight > 1000) {
      setError(t("milestoneInvalid"));
      return;
    }

    const ok = await run(
      () => mutations.addMilestone.mutateAsync({ name: name.trim(), plannedDate: plannedDate || null, weight: parsedWeight }),
      "milestoneAdded"
    );
    if (ok) {
      setName("");
      setPlannedDate("");
      setWeight("1");
    }
  }

  return (
    <div className="erp-card p-5 space-y-4" role="region" aria-label={t("milestonesTitle")}>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("milestonesTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{t("milestonesHint")}</p>
        </div>
        <div className="text-right">
          <div className="font-mono text-lg font-bold text-erp-navy">{(progress?.percent ?? 0).toFixed(2)}%</div>
          <div className="text-xs text-erp-text-muted">{t("progressSummary", { done: progress?.completedMilestones ?? 0, total: progress?.totalMilestones ?? 0 })}</div>
        </div>
      </div>

      <div className="h-2 w-full bg-erp-surface-muted" role="progressbar" aria-valuemin={0} aria-valuemax={100} aria-valuenow={progress?.percent ?? 0} aria-label={t("progressLabel")}>
        <div className="h-2 bg-erp-navy" style={{ width: `${Math.min(100, Math.max(0, progress?.percent ?? 0))}%` }} />
      </div>

      <ul className="space-y-2">
        {milestones.map((milestone) => (
          <li key={milestone.id} className="flex flex-wrap items-center justify-between gap-3 border border-erp-border bg-erp-surface-subtle p-3">
            <div className="min-w-0">
              <div className="text-sm font-medium text-erp-text-main">{milestone.name}</div>
              <div className="text-xs text-erp-text-muted">
                {t("milestoneMeta", { weight: milestone.weight ?? 0, date: milestone.plannedDate ? formatDate(milestone.plannedDate, locale) : "-" })}
                {milestone.completedAtUtc && ` • ${t("completedAt", { date: formatDateTime(milestone.completedAtUtc, locale) })}`}
              </div>
            </div>
            <div className="flex items-center gap-2">
              <StatusBadge label={milestone.completedAtUtc ? t("milestoneDone") : t("milestoneOpen")} variant={milestone.completedAtUtc ? "success" : "neutral"} />
              {canComplete && !milestone.completedAtUtc && (
                <Button type="button" variant="primary" size="sm" className="min-h-11" disabled={busy} onClick={() => void run(() => mutations.completeMilestone.mutateAsync({ milestoneId: milestone.id ?? "", expectedVersion: milestone.rowVersion ?? "" }), "milestoneCompleted")}>
                  {t("completeMilestone")}
                </Button>
              )}
              {editable && !milestone.completedAtUtc && (
                <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={busy} onClick={() => void run(() => mutations.deleteMilestone.mutateAsync({ milestoneId: milestone.id ?? "", expectedVersion: milestone.rowVersion ?? "" }), "milestoneRemoved")}>
                  {t("removeMilestone")}
                </Button>
              )}
            </div>
          </li>
        ))}
        {milestones.length === 0 && <li className="text-xs text-erp-text-muted">{t("milestonesEmpty")}</li>}
      </ul>

      {editable && (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-[1fr_160px_100px_auto] sm:items-end">
          <Input label={t("milestoneName")} value={name} maxLength={200} disabled={busy} onChange={(e) => setName(e.target.value)} />
          <Input type="date" label={t("milestoneDate")} value={plannedDate} disabled={busy} onChange={(e) => setPlannedDate(e.target.value)} />
          <Input type="number" label={t("milestoneWeight")} min={1} max={1000} value={weight} disabled={busy} onChange={(e) => setWeight(e.target.value)} />
          <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.addMilestone.isPending} disabled={busy} onClick={() => void add()}>
            {t("addMilestone")}
          </Button>
        </div>
      )}
      {error && <p className="text-xs text-erp-danger" role="alert">{error}</p>}
    </div>
  );
}
