"use client";

import React, { useState, useEffect } from "react";
import { ResponsiveContainer, PieChart, Pie, Cell, Tooltip } from "recharts";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";

export interface DistributionItem {
  name: string;
  value: number;
  color?: string;
}

export interface DonutChartProps {
  data: DistributionItem[];
  title?: string;
  height?: number;
  isLoading?: boolean;
  className?: string;
}

const DEFAULT_NAVY_PALETTE = ["#0B3056", "#255FA3", "#3D7BC0", "#659BD0", "#98BCDF"];

export function DonutChart({
  data,
  title,
  height = 240,
  isLoading = false,
  className,
}: DonutChartProps) {
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  if (!isMounted || isLoading) {
    return (
      <div
        style={{ height }}
        className={cn(
          "flex items-center justify-center border border-erp-border bg-erp-surface rounded-none",
          className
        )}
      >
        <MonoSpinner />
      </div>
    );
  }

  return (
    <div
      className={cn(
        "border border-erp-border bg-erp-surface p-4 rounded-none shadow-2xs",
        className
      )}
    >
      {title && (
        <div className="mb-3 text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          {title}
        </div>
      )}
      <div style={{ width: "100%", height }}>
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={data}
              cx="50%"
              cy="50%"
              innerRadius={50}
              outerRadius={80}
              paddingAngle={2}
              dataKey="value"
            >
              {data.map((entry, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={entry.color || DEFAULT_NAVY_PALETTE[index % DEFAULT_NAVY_PALETTE.length]}
                />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                backgroundColor: "#FFFFFF",
                borderColor: "#0B3056",
                borderRadius: "0px",
                fontSize: "12px",
              }}
            />
          </PieChart>
        </ResponsiveContainer>
      </div>
      <div className="mt-2 flex flex-wrap justify-center gap-3 text-xs text-erp-text-muted">
        {data.map((item, idx) => (
          <div key={item.name} className="flex items-center gap-1.5">
            <div
              className="h-2.5 w-2.5 rounded-none"
              style={{
                backgroundColor:
                  item.color || DEFAULT_NAVY_PALETTE[idx % DEFAULT_NAVY_PALETTE.length],
              }}
            />
            <span>{item.name}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

DonutChart.displayName = "DonutChart";
