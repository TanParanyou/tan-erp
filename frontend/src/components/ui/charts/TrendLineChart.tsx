"use client";

import React, { useState, useEffect } from "react";
import {
  ResponsiveContainer,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
} from "recharts";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";

export interface TrendDataPoint {
  date: string;
  value: number;
  label?: string;
}

export interface TrendLineChartProps {
  data: TrendDataPoint[];
  title?: string;
  dataKey?: string;
  color?: string;
  height?: number;
  isLoading?: boolean;
  className?: string;
}

export function TrendLineChart({
  data,
  title,
  dataKey = "value",
  color = "#0B3056",
  height = 240,
  isLoading = false,
  className,
}: TrendLineChartProps) {
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
        <div className="mb-4 text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          {title}
        </div>
      )}
      <div style={{ width: "100%", height }}>
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={data} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
            <defs>
              <linearGradient id="trendGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={color} stopOpacity={0.25} />
                <stop offset="95%" stopColor={color} stopOpacity={0.0} />
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" stroke="#D1DEEC" opacity={0.6} />
            <XAxis
              dataKey="date"
              stroke="#536B88"
              fontSize={11}
              tickLine={false}
              axisLine={{ stroke: "#D1DEEC" }}
            />
            <YAxis
              stroke="#536B88"
              fontSize={11}
              tickLine={false}
              axisLine={{ stroke: "#D1DEEC" }}
            />
            <Tooltip
              contentStyle={{
                backgroundColor: "#FFFFFF",
                borderColor: "#0B3056",
                borderRadius: "0px",
                fontSize: "12px",
              }}
            />
            <Area
              type="monotone"
              dataKey={dataKey}
              stroke={color}
              strokeWidth={2}
              fillOpacity={1}
              fill="url(#trendGradient)"
            />
          </AreaChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}

TrendLineChart.displayName = "TrendLineChart";
