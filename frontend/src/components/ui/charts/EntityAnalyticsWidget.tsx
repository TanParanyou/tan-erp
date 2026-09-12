"use client";

import React, { useState } from "react";
import { StatCard } from "./StatCard";
import { TrendLineChart } from "./TrendLineChart";
import { DonutChart } from "./DonutChart";
import { useAnalyticsData, type AnalyticsTimeRange } from "@/hooks/useAnalyticsData";
import { cn } from "@/lib/utils/cn";

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
          title="ต้นทุนภายใน (BOQ Cost)"
          value={overview?.boqCost ? `฿${overview.boqCost.toLocaleString()}` : "฿0"}
          isLoading={isLoading}
        />
        <StatCard
          title="กำไรขั้นต้น (Gross Profit)"
          value={overview?.grossProfit ? `฿${overview.grossProfit.toLocaleString()}` : "฿0"}
          isLoading={isLoading}
        />
        <StatCard
          title="ยอดเสนอราคารวม (Quotation)"
          value={overview?.quotationTotal ? `฿${overview.quotationTotal.toLocaleString()}` : "฿0"}
          isLoading={isLoading}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 pt-2">
        <TrendLineChart
          title="แนวโน้มรายรับ / ยอดขาย (Trends)"
          data={chartData}
          isLoading={isLoading}
        />
        <DonutChart
          title="สัดส่วนทรัพยากร / ค่าใช้จ่าย (Distribution)"
          data={topResources}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
}

EntityAnalyticsWidget.displayName = "EntityAnalyticsWidget";
