"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { StatCard } from "@/components/ui/charts/StatCard";
import { type OpportunityMetricsResponse } from "@/lib/api/api-client";
import {
  IconFileText,
  IconBriefcase,
  IconCalendar,
  IconCheckCircle,
} from "@/components/common/Icons";

export interface PipelineMetricCardsProps {
  metrics?: OpportunityMetricsResponse | null;
  isLoading?: boolean;
}

export function PipelineMetricCards({
  metrics,
  isLoading = false,
}: PipelineMetricCardsProps) {
  const t = useTranslations("opportunities.metrics");

  const formattedPipelineValue = metrics?.totalPipelineValue !== undefined
    ? `${metrics.totalPipelineValue.toLocaleString()} THB`
    : "-";

  const activeDealsValue = metrics?.activeDealsCount !== undefined
    ? `${metrics.activeDealsCount.toLocaleString()} ${t("dealsUnit")}`
    : "-";

  const closingThisMonthValue = metrics?.closingThisMonthCount !== undefined
    ? `${metrics.closingThisMonthCount.toLocaleString()} ${t("dealsUnit")}`
    : "-";

  const winRateValue = metrics?.winRatePercentage !== undefined
    ? `${metrics.winRatePercentage}%`
    : "-";

  const wonSubtitle = metrics?.wonValue !== undefined
    ? `${t("wonValue")}: ${metrics.wonValue.toLocaleString()} THB (${metrics.wonDealsCount ?? 0} ${t("dealsUnit")})`
    : undefined;

  return (
    <div
      data-testid="pipeline-metric-cards"
      className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 w-full"
    >
      <StatCard
        title={t("totalPipelineValue")}
        value={formattedPipelineValue}
        isLoading={isLoading}
        icon={<IconFileText size={18} />}
      />

      <StatCard
        title={t("activeDeals")}
        value={activeDealsValue}
        isLoading={isLoading}
        icon={<IconBriefcase size={18} />}
      />

      <StatCard
        title={t("closingThisMonth")}
        value={closingThisMonthValue}
        isLoading={isLoading}
        icon={<IconCalendar size={18} />}
      />

      <StatCard
        title={t("winRate")}
        value={winRateValue}
        subtitle={wonSubtitle}
        isLoading={isLoading}
        icon={<IconCheckCircle size={18} />}
      />
    </div>
  );
}
