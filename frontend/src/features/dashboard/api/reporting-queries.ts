import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type DashboardSummaryResponse,
  type OrganizationBranchResponse,
  type PipelineReportResponse,
  type ProjectBudgetReportResponse,
  type QuotationReportResponse,
  type ReportingArAgingResponse,
  type ServiceSlaReportResponse,
  type StockValueReportResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function reportingKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "reporting"] as const;
}

export function useReportingRequestContext() {
  const locale: UiLocale = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (idempotencyKey?: string, signal?: AbortSignal) => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, idempotencyKey, signal };
  };
  return { locale, membershipId, selectedMembership, options };
}

export function useDashboardSummaryQuery(
  query: { branchId?: string; asOfDate?: string } = {},
  enabled = true
): UseQueryResult<DashboardSummaryResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "dashboard-summary", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getDashboardSummary(query, await options(undefined, signal)),
  });
}

export function usePipelineReportQuery(
  query: { branchId?: string } = {},
  enabled = true
): UseQueryResult<PipelineReportResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "pipeline", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getPipelineReport(query, await options(undefined, signal)),
  });
}

export function useQuotationReportQuery(
  query: { branchId?: string } = {},
  enabled = true
): UseQueryResult<QuotationReportResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "quotations", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getQuotationReport(query, await options(undefined, signal)),
  });
}

export function useProjectBudgetReportQuery(
  query: { branchId?: string } = {},
  enabled = true
): UseQueryResult<ProjectBudgetReportResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "projects-budget", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getProjectBudgetReport(query, await options(undefined, signal)),
  });
}

export function useStockValueReportQuery(
  query: { branchId?: string } = {},
  enabled = true
): UseQueryResult<StockValueReportResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "stock-value", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getStockValueReport(query, await options(undefined, signal)),
  });
}

export function useReportingArAgingQuery(
  query: { branchId?: string; asOfDate?: string } = {},
  enabled = true
): UseQueryResult<ReportingArAgingResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "ar-aging", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getReportingArAging(query, await options(undefined, signal)),
  });
}

export function useServiceSlaReportQuery(
  query: { branchId?: string } = {},
  enabled = true
): UseQueryResult<ServiceSlaReportResponse, Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "service-sla", query],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.getServiceSlaReport(query, await options(undefined, signal)),
  });
}

export function useReportingBranchesQuery(
  enabled = true
): UseQueryResult<OrganizationBranchResponse[], Error> {
  const { locale, membershipId, options } = useReportingRequestContext();
  return useQuery({
    queryKey: [...reportingKey(membershipId, locale), "branches"],
    enabled: Boolean(membershipId && enabled),
    queryFn: async ({ signal }) => apiClient.listOrganizationBranches(await options(undefined, signal)),
  });
}
