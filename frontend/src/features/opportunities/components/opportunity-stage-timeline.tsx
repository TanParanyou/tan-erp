"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { useOpportunityStageHistory } from "../api/opportunity-queries";
import { formatDateTime } from "@/lib/formatters/formatters";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { Badge } from "@/components/ui/Badge";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { getOpportunityStageLabelKey } from "../opportunity-labels";

interface OpportunityStageTimelineProps {
  opportunityId: string;
}

export function OpportunityStageTimeline({ opportunityId }: OpportunityStageTimelineProps) {
  const t = useTranslations("opportunities");
  const locale = useSafeLocale();
  const { data, isLoading, isError } = useOpportunityStageHistory(opportunityId);

  const resolveStageLabel = (stage: string | null | undefined): string => {
    if (!stage) return "-";
    const key = getOpportunityStageLabelKey(stage);
    switch (key) {
      case "draft":
        return t("stageDraft");
      case "qualified":
        return t("stageQualified");
      case "surveying":
        return t("stageSurveying");
      case "estimating":
        return t("stageEstimating");
      case "proposed":
        return t("stageProposed");
      case "won":
        return t("stageWon");
      case "lost":
        return t("stageLost");
      case "cancelled":
        return t("stageCancelled");
      default:
        return stage;
    }
  };

  const resolveStageVariant = (
    stage: string | null | undefined
  ): "neutral" | "primary" | "success" | "warning" | "danger" | "info" => {
    if (!stage) return "neutral";
    const key = getOpportunityStageLabelKey(stage);
    switch (key) {
      case "draft":
        return "neutral";
      case "qualified":
        return "info";
      case "surveying":
      case "estimating":
      case "proposed":
        return "warning";
      case "won":
        return "success";
      case "lost":
      case "cancelled":
        return "danger";
      default:
        return "neutral";
    }
  };

  const resolveReasonLabel = (reasonCode: string | null | undefined): string | null => {
    if (!reasonCode) return null;
    try {
      return t(`reasons.${reasonCode}` as Parameters<typeof t>[0]);
    } catch {
      return reasonCode;
    }
  };

  if (isLoading) {
    return (
      <div className="erp-card p-6 flex justify-center items-center">
        <MonoSpinner size="md" />
      </div>
    );
  }

  if (isError || !data) {
    return null;
  }

  const items = data.items || [];

  return (
    <div className="erp-card p-6 flex flex-col gap-4">
      <div className="flex flex-col gap-1 border-b border-erp-border pb-3">
        <h3 className="text-base font-bold text-erp-navy">
          {t("stageHistoryTitle")}
        </h3>
        <p className="text-xs text-erp-text-muted">
          {t("stageHistorySubtitle")}
        </p>
      </div>

      {items.length === 0 ? (
        <p className="text-sm text-erp-text-muted py-4 text-center">
          {t("stageHistoryEmpty")}
        </p>
      ) : (
        <div className="relative pl-6 border-l-2 border-erp-border space-y-6">
          {items.map((item) => {
            const reasonLabel = resolveReasonLabel(item.reasonCode);
            return (
              <div key={item.id} className="relative group">
                {/* Dot marker */}
                <span
                  className="absolute -left-[31px] top-1.5 w-3 h-3 bg-erp-navy border-2 border-white ring-1 ring-erp-border"
                  style={{ borderRadius: "0px" }}
                />

                <div className="flex flex-col gap-1.5 bg-erp-bg-subtle p-3 border border-erp-border">
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <div className="flex items-center gap-2">
                      <Badge variant={resolveStageVariant(item.fromStage)} size="sm">
                        {resolveStageLabel(item.fromStage)}
                      </Badge>
                      <span className="text-xs text-erp-text-muted">→</span>
                      <Badge variant={resolveStageVariant(item.toStage)} size="sm">
                        {resolveStageLabel(item.toStage)}
                      </Badge>
                    </div>
                    <time className="text-xs font-mono text-erp-text-muted">
                      {formatDateTime(item.occurredAtUtc, locale)}
                    </time>
                  </div>

                  {reasonLabel && (
                    <div className="text-xs text-erp-text font-medium mt-1">
                      <span className="text-erp-text-muted">{t("reasonLabel")}: </span>
                      {reasonLabel}
                    </div>
                  )}

                  {item.note && (
                    <div className="text-xs text-erp-text-muted italic bg-white/60 p-2 border border-erp-border/50 mt-1">
                      &ldquo;{item.note}&rdquo;
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
