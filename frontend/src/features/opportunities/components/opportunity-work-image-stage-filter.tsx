"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import { resolveOpportunityStageLabel } from "../opportunity-labels";
import { IconFilter } from "@/components/common/Icons";

export interface OpportunityWorkImageStageFilterProps {
  selectedStage: string | null;
  onSelectStage: (stage: string | null) => void;
  totalCount: number;
  stageCounts: Record<string, number>;
  className?: string;
}

const CANONICAL_STAGES = [
  "draft",
  "qualified",
  "surveying",
  "estimating",
  "proposed",
] as const;

/**
 * Compact, sharp Stage Filter component for Opportunity Work Images:
 * - Always visible as requested
 * - Ultra-compact segmented button bar (Atelier Architectural Navy Sharp, 0px radius)
 * - Clear counts and active visual indicators
 * - Accessible keyboard and click navigation
 */
export function OpportunityWorkImageStageFilter({
  selectedStage,
  onSelectStage,
  totalCount,
  stageCounts,
  className = "",
}: OpportunityWorkImageStageFilterProps) {
  const t = useTranslations("opportunities");

  return (
    <div
      className={cn(
        "flex items-center gap-1.5 overflow-x-auto pb-1 text-xs select-none",
        className
      )}
      role="toolbar"
      aria-label={t("filterByStage")}
    >
      <div className="flex items-center gap-1 text-erp-text-muted mr-1 shrink-0 font-mono text-[11px] uppercase">
        <IconFilter size={13} className="text-erp-text-muted" />
        <span>{t("filterByStage")}:</span>
      </div>

      <div className="inline-flex border border-erp-border bg-erp-surface p-0.5 rounded-none shrink-0">
        {/* All Stages Button */}
        <button
          type="button"
          onClick={() => onSelectStage(null)}
          className={cn(
            "px-2.5 py-1 text-xs font-mono tracking-wider transition-all rounded-none whitespace-nowrap cursor-pointer",
            selectedStage === null
              ? "bg-erp-navy text-white font-semibold shadow-sm"
              : "text-erp-text-muted hover:text-erp-text-main hover:bg-erp-surface-muted"
          )}
          aria-pressed={selectedStage === null}
        >
          {t("allStages")} ({totalCount})
        </button>

        {/* Canonical Stage Buttons */}
        {CANONICAL_STAGES.map((stage) => {
          const count = stageCounts[stage] ?? 0;
          const isSelected = selectedStage === stage;

          return (
            <button
              key={stage}
              type="button"
              onClick={() => onSelectStage(stage)}
              className={cn(
                "px-2.5 py-1 text-xs font-mono tracking-wider transition-all rounded-none whitespace-nowrap cursor-pointer border-l border-erp-border/60",
                isSelected
                  ? "bg-erp-navy text-white font-semibold shadow-sm"
                  : count > 0
                  ? "text-erp-text-main hover:bg-erp-surface-muted"
                  : "text-erp-text-muted/60 hover:text-erp-text-muted hover:bg-erp-surface-muted"
              )}
              aria-pressed={isSelected}
            >
              {resolveOpportunityStageLabel(stage, t)} ({count})
            </button>
          );
        })}
      </div>
    </div>
  );
}

export default OpportunityWorkImageStageFilter;
