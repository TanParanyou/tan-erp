"use client";

import { useLocale, useTranslations } from "next-intl";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { formatCurrency, formatDateTime } from "@/lib/formatters/formatters";
import type { ProjectControlResponse, ProjectCostLedgerEntryResponse } from "@/lib/api/api-client";
import { isProjectCostEntryType, isProjectCostSourceType } from "../project-status";

interface ProjectCostLedgerCardProps {
  control: ProjectControlResponse;
}

export function ProjectCostLedgerCard({ control }: ProjectCostLedgerCardProps) {
  const t = useTranslations("projects.control");
  const locale = useLocale();
  const entries: ProjectCostLedgerEntryResponse[] = control.costLedger ?? [];

  return (
    <div className="erp-card p-5 space-y-4" role="region" aria-label={t("costLedgerTitle")}>
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("costLedgerTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{t("costLedgerHint")}</p>
        </div>
      </div>

      {entries.length === 0 ? (
        <p className="text-xs text-erp-text-muted">{t("costLedgerEmpty")}</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead className="border-b border-erp-border bg-erp-surface-subtle font-semibold text-erp-navy">
              <tr>
                <th className="p-2.5">{t("costSource")}</th>
                <th className="p-2.5">{t("budgetCategory")}</th>
                <th className="p-2.5">WBS</th>
                <th className="p-2.5">{t("budgetDescription")}</th>
                <th className="p-2.5 text-right">{t("budgetAmount")}</th>
                <th className="p-2.5 text-center">{t("statusHeader")}</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-erp-border">
              {entries.map((entry) => {
                const entryTypeLabel = isProjectCostEntryType(entry.entryType)
                  ? t(`costEntryTypes.${entry.entryType}`)
                  : (entry.entryType ?? "-");
                const sourceTypeLabel = isProjectCostSourceType(entry.sourceType)
                  ? t(`costSourceTypes.${entry.sourceType}`)
                  : (entry.sourceType ?? "-");

                return (
                  <tr
                    key={entry.id ?? `${entry.sourceNumber}-${entry.occurredAtUtc}`}
                    className="hover:bg-erp-surface-subtle/50"
                  >
                    <td className="p-2.5">
                      <div className="font-mono font-bold text-erp-navy">{entry.sourceNumber ?? "-"}</div>
                      <div className="text-[11px] text-erp-text-muted">{sourceTypeLabel}</div>
                    </td>
                    <td className="p-2.5">
                      <StatusBadge
                        label={entryTypeLabel}
                        variant={entry.entryType === "actual" ? "info" : "warning"}
                      />
                    </td>
                    <td className="p-2.5 font-mono">{entry.wbsCode ?? "-"}</td>
                    <td className="p-2.5">
                      <div className="text-erp-text-main">{entry.description ?? "-"}</div>
                      <div className="text-[11px] text-erp-text-muted">
                        {entry.actor?.displayName ? `${entry.actor.displayName} • ` : ""}
                        {entry.occurredAtUtc ? formatDateTime(entry.occurredAtUtc, locale) : "-"}
                      </div>
                    </td>
                    <td className="p-2.5 text-right font-mono font-semibold">
                      {formatCurrency(entry.amount ?? 0, "THB", locale)}
                    </td>
                    <td className="p-2.5 text-center">
                      {entry.isReversal && (
                        <StatusBadge label={t("costReversal")} variant="neutral" />
                      )}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
