"use client";

import React, { useState } from "react";
import { StatCard } from "./StatCard";
import { TrendLineChart } from "./TrendLineChart";
import { DonutChart } from "./DonutChart";
import { useAnalyticsData, type AnalyticsTimeRange } from "@/hooks/useAnalyticsData";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface EntityAnalyticsWidgetProps {
  resourceType?: string;
  resourceId?: string;
  title?: string;
  className?: string;
}

export function EntityAnalyticsWidget({
  resourceType,
  resourceId,
  title = "Operational Financial HUD",
  className,
}: EntityAnalyticsWidgetProps) {
  const t = useTranslations("common.analyticsWidget");
  const [timeRange, setTimeRange] = useState<AnalyticsTimeRange>("30d");
  const { overview, trends, topResources, isLoading } = useAnalyticsData({
    timeRange,
    resourceType,
    resourceId,
  });

  const chartData = trends.map((t) => ({
    date: t.date,
    value: Number(t.revenue || t.views || 0),
  }));

  return (
    <div className={cn("space-y-4 border border-erp-border bg-erp-surface p-5 rounded-none", className)}>
      <div className="flex flex-wrap items-center justify-between gap-3 border-b border-erp-border pb-3">
        <span className="text-sm font-bold tracking-tight text-erp-navy uppercase">
          {title}
        </span>
        <div className="flex border border-erp-border bg-erp-surface-subtle p-0.5 rounded-none">
          {(['7d', '30d', '90d'] as AnalyticsTimeRange[]).map((r) => (
            <button
              key={r}
              type="button"
              onClick={() => setTimeRange(r)}
              className={cn(
                "px-2.5 py-1 text-xs font-medium transition-colors rounded-none",
                timeRange === r
                  ? "bg-erp-navy text-white font-bold"
                  : "text-erp-text-muted hover:text-erp-text-main"
              )}
            >
              {r.toUpperCase()}
            </button>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
        <StatCard
          title={t("boqCost")}
          value={overview?.boqCost ? `฿${overview.boqCost.toLocaleString()}` : "฿0"}
          isLoading={isLoading}
        />
        <StatCard
          title={t("grossProfit")}
          value={overview?.grossProfit ? `฿${overview.grossProfit.toLocaleString()}` : "฿0"}
          isLoading={isLoading}
        />
        <StatCard
          title={t("quotationTotal")}
          value={overview?.quotationTotal ? `฿${overview.quotationTotal.toLocaleString()}` : "฿0"}
          isLoading={isLoading}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 pt-2">
        <TrendLineChart
          title={t("trendsTitle")}
          data={chartData}
          isLoading={isLoading}
        />
        <DonutChart
          title={t("distributionTitle")}
          data={topResources}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
}

EntityAnalyticsWidget.displayName = "EntityAnalyticsWidget";
