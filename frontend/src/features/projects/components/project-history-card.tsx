"use client";

import { useLocale, useTranslations } from "next-intl";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { ProjectControlResponse } from "@/lib/api/api-client";
import { isProjectStatus } from "../project-status";

interface ProjectHistoryCardProps {
  history: NonNullable<ProjectControlResponse["history"]>;
}

export function ProjectHistoryCard({ history }: ProjectHistoryCardProps) {
  const t = useTranslations("projects");
  const locale = useLocale();
  const statusLabel = (value: string | null | undefined): string => (isProjectStatus(value) ? t(`statuses.${value}`) : "-");

  return (
    <div className="erp-card p-5 space-y-3" role="region" aria-label={t("control.historyTitle")}>
      <h3 className="text-sm font-bold text-erp-navy">{t("control.historyTitle")}</h3>
      {history.length === 0 ? (
        <p className="text-xs text-erp-text-muted">{t("control.historyEmpty")}</p>
      ) : (
        <ol className="space-y-2">
          {history.map((entry) => (
            <li key={entry.id} className="border-l-4 border-erp-navy bg-erp-surface-subtle px-3 py-2 text-xs">
              <div className="font-semibold text-erp-text-main">
                {t("control.historyEntry", { from: statusLabel(entry.fromStatus), to: statusLabel(entry.toStatus) })}
              </div>
              <div className="text-erp-text-muted">
                {entry.actor?.displayName ?? "-"} • {entry.occurredAtUtc ? formatDateTime(entry.occurredAtUtc, locale) : "-"}
              </div>
              {entry.reason && <div className="mt-1 text-erp-text-body">{entry.reason}</div>}
            </li>
          ))}
        </ol>
      )}
    </div>
  );
}
