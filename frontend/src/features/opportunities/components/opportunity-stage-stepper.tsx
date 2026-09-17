"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { IconCheckCircle } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface OpportunityStageStepperProps {
  currentStage: string | null | undefined;
  className?: string;
}

interface StepItem {
  key: string;
  labelKey: string;
  stepNumber: number;
}

const PIPELINE_STEPS: StepItem[] = [
  { key: "draft", labelKey: "stageDraft", stepNumber: 1 },
  { key: "qualified", labelKey: "stageQualified", stepNumber: 2 },
  { key: "surveying", labelKey: "stageSurveying", stepNumber: 3 },
  { key: "estimating", labelKey: "stageEstimating", stepNumber: 4 },
  { key: "proposed", labelKey: "stageProposed", stepNumber: 5 },
  { key: "won", labelKey: "stageWon", stepNumber: 6 },
];

export function OpportunityStageStepper({ currentStage, className }: OpportunityStageStepperProps) {
  const t = useTranslations("opportunities");

  const stage = currentStage || "draft";
  const isLost = stage === "lost";
  const isCancelled = stage === "cancelled";
  const isClosed = isLost || isCancelled;

  // Determine active step index
  let activeIndex = PIPELINE_STEPS.findIndex((s) => s.key === stage);
  if (isClosed) {
    // If closed lost/cancelled, mark as stopped
    activeIndex = -1;
  } else if (activeIndex === -1) {
    activeIndex = 0;
  }

  const currentStepItem = PIPELINE_STEPS[activeIndex >= 0 ? activeIndex : 0];
  const currentStepLabel = t(currentStepItem.labelKey);

  return (
    <div
      role="region"
      aria-label={t("stageStepperTitle")}
      className={cn(
        "w-full bg-erp-surface border border-erp-border p-3 sm:p-4 shadow-xs select-none",
        className
      )}
    >
      {/* Mobile Compact Progress Strip (shown on mobile, hidden on sm+) */}
      <div className="flex flex-col gap-2 sm:hidden">
        <div className="flex items-center justify-between text-xs font-semibold">
          <span className="text-erp-navy">
            {isClosed ? (
              <span className="text-erp-danger font-bold">
                {isLost ? t("stageLost") : t("stageCancelled")}
              </span>
            ) : (
              t("currentStageStep", {
                current: activeIndex + 1,
                total: PIPELINE_STEPS.length,
                label: currentStepLabel,
              })
            )}
          </span>
          <span className="font-mono text-[11px] text-erp-text-muted">
            {isClosed ? t("stageTerminated") : `${Math.round(((activeIndex + 1) / PIPELINE_STEPS.length) * 100)}%`}
          </span>
        </div>
        {/* Progress Bar with 0px sharp corners */}
        <div className="w-full h-1.5 bg-erp-surface-subtle border border-erp-border flex">
          {PIPELINE_STEPS.map((step, idx) => {
            const isPassed = !isClosed && idx < activeIndex;
            const isCurrent = !isClosed && idx === activeIndex;
            return (
              <div
                key={step.key}
                className={cn(
                  "flex-1 h-full border-r border-erp-border last:border-r-0 transition-colors",
                  isPassed && "bg-erp-navy",
                  isCurrent && "bg-erp-navy",
                  !isPassed && !isCurrent && "bg-transparent",
                  isClosed && "bg-erp-border-subtle"
                )}
              />
            );
          })}
        </div>
      </div>

      {/* Desktop & Tablet Architectural Chevron / Step Chain (hidden on mobile, shown on sm+) */}
      <div className="hidden sm:grid sm:grid-cols-6 gap-2">
        {PIPELINE_STEPS.map((step, idx) => {
          const isPassed = !isClosed && idx < activeIndex;
          const isCurrent = !isClosed && idx === activeIndex;
          const isUpcoming = !isClosed && idx > activeIndex;

          return (
            <div
              key={step.key}
              className={cn(
                "relative flex items-center gap-2 p-2.5 border text-xs transition-all",
                isCurrent &&
                  "bg-erp-navy text-white border-erp-navy font-bold shadow-xs",
                isPassed &&
                  "bg-erp-surface-subtle text-erp-navy border-erp-border font-semibold",
                isUpcoming &&
                  "bg-erp-surface text-erp-text-muted border-erp-border/60 opacity-80",
                isClosed &&
                  "bg-erp-surface text-erp-text-muted border-erp-border/40 opacity-50"
              )}
            >
              {/* Step indicator */}
              <div className="shrink-0 flex items-center justify-center w-5 h-5 font-mono text-[10px] font-bold">
                {isPassed ? (
                  <IconCheckCircle size={16} className="text-erp-navy" />
                ) : (
                  <span
                    className={cn(
                      "w-4 h-4 flex items-center justify-center border text-[10px]",
                      isCurrent
                        ? "border-white bg-white text-erp-navy"
                        : "border-erp-border text-erp-text-muted bg-erp-surface"
                    )}
                  >
                    {step.stepNumber}
                  </span>
                )}
              </div>

              {/* Step label */}
              <span className="truncate leading-tight">{t(step.labelKey)}</span>
            </div>
          );
        })}
      </div>
    </div>
  );
}
