"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Badge } from "@/components/ui/Badge";
import { StatusBadge } from "@/components/ui/StatusBadge";
import type { WorkspaceFinancialSummary } from "../utils/estimate-calculations";
import {
  formatFinancialNumber,
  formatSignedFinancialAmount,
  formatPercentRate,
} from "../utils/estimate-formatters";

interface EstimateWorkspaceHudProps {
  estimateNumber: string;
  revisionNo: number;
  status: string;
  currency: string;
  hudMetrics: WorkspaceFinancialSummary;
}

export function EstimateWorkspaceHud({
  estimateNumber,
  revisionNo,
  status,
  currency,
  hudMetrics,
}: EstimateWorkspaceHudProps) {
  const t = useTranslations("estimates");

  return (
    <div className="bg-erp-surface border-b border-erp-border">
      {/* Tier 1: Project Identity & Title Block */}
      <div className="bg-erp-navy text-white px-4 sm:px-6 py-3 flex flex-wrap justify-between items-center gap-3">
        <div className="flex items-center gap-3">
          <span className="text-xs uppercase tracking-wider text-erp-slate-300 font-mono">
            {estimateNumber}
          </span>
          <Badge variant="neutral" className="!rounded-none bg-white/10 text-white font-mono text-xs border border-white/20">
            {t("revision", { number: revisionNo })}
          </Badge>
          <StatusBadge label={status} />
        </div>
        <div className="text-xs text-erp-slate-300 font-mono">
          {t("sectionsCount", { count: hudMetrics.totalSections })} • {hudMetrics.totalWorkItems} {t("itemsCount", { count: hudMetrics.totalWorkItems })}
        </div>
      </div>

      {/* Tier 2: Operational Financial HUD (Responsive 3-column / 2-row on mobile) */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 divide-y sm:divide-y-0 sm:divide-x divide-erp-border bg-erp-surface">
        {/* Cell 1: BOQ Cost */}
        <div className="p-3 sm:p-4">
          <span className="text-[11px] font-bold uppercase tracking-wider text-erp-text-muted block">
            {t("boqCost")}
          </span>
          <div className="flex items-baseline gap-2 mt-1">
            <span className="font-mono text-lg sm:text-xl font-bold text-erp-text-main">
              {formatFinancialNumber(hudMetrics.totalCost)}
            </span>
            <span className="text-xs font-mono text-erp-text-muted">{currency}</span>
          </div>
          <span className="text-[11px] text-erp-text-subtle block mt-0.5">
            {t("boqCostSubtext")}
          </span>
        </div>

        {/* Cell 2: Gross Profit & Health Benchmark */}
        <div className="p-3 sm:p-4">
          <div className="flex items-center justify-between gap-2">
            <span className="text-[11px] font-bold uppercase tracking-wider text-erp-text-muted block">
              {t("grossProfit")}
            </span>
            <span
              className={`text-[10px] font-mono font-bold px-1.5 py-0.5 border !rounded-none ${
                hudMetrics.isGpHealthy
                  ? "bg-emerald-50 text-emerald-700 border-emerald-300"
                  : "bg-rose-50 text-rose-700 border-rose-300"
              }`}
            >
              {hudMetrics.isGpHealthy
                ? t("gpBenchmark", { rate: formatPercentRate(hudMetrics.marginRate, 1) })
                : t("gpBelowBenchmark", { rate: formatPercentRate(hudMetrics.marginRate, 1) })}
            </span>
          </div>
          <div className="flex items-baseline gap-2 mt-1">
            <span
              className={`font-mono text-lg sm:text-xl font-bold ${
                hudMetrics.isGpHealthy ? "text-emerald-700" : "text-rose-600"
              }`}
            >
              {formatSignedFinancialAmount(hudMetrics.grossProfit)}
            </span>
            <span className="text-xs font-mono text-erp-text-muted">{currency}</span>
          </div>
          <span className="text-[11px] text-erp-text-subtle block mt-0.5 font-mono">
            {t("marginRate")}: {formatPercentRate(hudMetrics.marginRate)} • {t("markupRate")}: {formatPercentRate(hudMetrics.markupRate)}
          </span>
        </div>

        {/* Cell 3: Hero Quotation Total */}
        <div className="p-3 sm:p-4 sm:col-span-2 lg:col-span-1 bg-erp-navy/5 border-l-0 lg:border-l border-erp-navy">
          <span className="text-[11px] font-bold uppercase tracking-wider text-erp-navy block">
            {t("quotationTotal")}
          </span>
          <div className="flex items-baseline gap-2 mt-1">
            <span className="font-mono text-xl sm:text-2xl font-bold text-erp-navy">
              {formatFinancialNumber(hudMetrics.netSelling)}
            </span>
            <span className="text-xs font-mono font-bold text-erp-navy">{currency}</span>
          </div>
          <span className="text-[11px] text-erp-text-muted block mt-0.5">
            {t("quotationTotalSubtext")}
          </span>
        </div>
      </div>
    </div>
  );
}
