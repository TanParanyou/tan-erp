import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type SiteListResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function customerSiteListQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  customerId: string | null | undefined
): readonly ["business", string | null | undefined, "th" | "en", "sites", "list", string | null | undefined] {
  return ["business", membershipId, locale, "sites", "list", customerId] as const;
}

export function useCustomerSiteList(
  customerId: string | null | undefined
): UseQueryResult<SiteListResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: customerSiteListQueryKey(membershipId, normalizedLocale, customerId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new Error("No authentication token available");
      }
      if (!membershipId) {
        throw new Error("No active membership selected");
      }
      if (!customerId) {
        throw new Error("No customer ID provided");
      }

      return apiClient.listCustomerSites(customerId, {
        token,
        membershipId,
        locale: normalizedLocale,
        signal,
      });
    },
    enabled: Boolean(membershipId && customerId && customerId !== "create" && customerId !== "add"),
  });
}
