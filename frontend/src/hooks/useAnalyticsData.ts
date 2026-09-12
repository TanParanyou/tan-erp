"use client";

import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";

export type AnalyticsTimeRange = "7d" | "30d" | "90d" | "custom";

export interface UseAnalyticsOptions {
  timeRange?: AnalyticsTimeRange;
  customFrom?: string;
  customTo?: string;
  resourceType?: string;
  resourceId?: string;
  topLimit?: number;
  enabled?: boolean;
}

export interface AnalyticsOverview {
  totalViews?: number;
  uniqueVisitors?: number;
  bounceRate?: number;
  avgDuration?: number;
  boqCost?: number;
  grossProfit?: number;
  quotationTotal?: number;
}

export interface AnalyticsTrendItem {
  date: string;
  views?: number;
  visitors?: number;
  cost?: number;
  revenue?: number;
  [key: string]: unknown;
}

export interface AnalyticsDistributionItem {
  name: string;
  value: number;
  color?: string;
}

export interface AnalyticsData {
  stats: AnalyticsOverview;
  trends: AnalyticsTrendItem[];
  distribution: AnalyticsDistributionItem[];
}

export function useAnalyticsData(options: UseAnalyticsOptions = {}) {
  const {
    timeRange = "30d",
    customFrom,
    customTo,
    resourceType,
    resourceId,
    topLimit = 5,
    enabled = true,
  } = options;

  const dateParams = useMemo(() => {
    if (timeRange === "custom") {
      return {
        from: customFrom || new Date().toISOString().split("T")[0],
        to: customTo || new Date().toISOString().split("T")[0],
      };
    }

    const to = new Date().toISOString().split("T")[0];
    const d = new Date();
    if (timeRange === "7d") d.setDate(d.getDate() - 6);
    else if (timeRange === "30d") d.setDate(d.getDate() - 29);
    else if (timeRange === "90d") d.setDate(d.getDate() - 89);
    const from = d.toISOString().split("T")[0];

    return { from, to };
  }, [timeRange, customFrom, customTo]);

  const overviewQuery = useQuery({
    queryKey: ["analytics", "overview", dateParams],
    queryFn: async (): Promise<AnalyticsOverview> => {
      return {
        totalViews: 0,
        uniqueVisitors: 0,
        boqCost: 0,
        grossProfit: 0,
        quotationTotal: 0,
      };
    },
    enabled,
    staleTime: 60 * 1000,
  });

  const trendsQuery = useQuery({
    queryKey: ["analytics", "trends", resourceType, dateParams],
    queryFn: async (): Promise<AnalyticsTrendItem[]> => {
      return [];
    },
    enabled,
    staleTime: 60 * 1000,
  });

  const topResourcesQuery = useQuery({
    queryKey: ["analytics", "top-resources", resourceType, topLimit, dateParams],
    queryFn: async (): Promise<AnalyticsDistributionItem[]> => {
      return [];
    },
    enabled,
    staleTime: 60 * 1000,
  });

  const resourceStatsQuery = useQuery({
    queryKey: ["analytics", "resource", resourceType, resourceId, dateParams],
    queryFn: async (): Promise<AnalyticsOverview> => {
      return {};
    },
    enabled: enabled && Boolean(resourceType && resourceId),
    staleTime: 60 * 1000,
  });

  const isLoading =
    overviewQuery.isLoading ||
    trendsQuery.isLoading ||
    topResourcesQuery.isLoading ||
    (Boolean(resourceType && resourceId) && resourceStatsQuery.isLoading);

  const isFetching =
    overviewQuery.isFetching ||
    trendsQuery.isFetching ||
    topResourcesQuery.isFetching ||
    (Boolean(resourceType && resourceId) && resourceStatsQuery.isFetching);

  return {
    dateParams,
    overview: overviewQuery.data,
    trends: trendsQuery.data || [],
    topResources: topResourcesQuery.data || [],
    resourceStats: resourceStatsQuery.data,
    isLoading,
    isFetching,
    refetch: () => {
      overviewQuery.refetch();
      trendsQuery.refetch();
      topResourcesQuery.refetch();
      if (resourceType && resourceId) {
        resourceStatsQuery.refetch();
      }
    },
  };
}
