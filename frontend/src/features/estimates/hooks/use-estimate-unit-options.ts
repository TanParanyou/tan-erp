"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, type EstimateUnitOptionsResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";

export interface UseEstimateUnitOptionsParams {
  branchId?: string;
  itemId: string;
}

export function useEstimateUnitOptions({ branchId, itemId }: UseEstimateUnitOptionsParams) {
  const membershipId = useOptionalSelectedMembership()?.selectedMembership?.id;
  const locale = useSafeLocale();

  return useQuery<EstimateUnitOptionsResponse>({
    queryKey: ["business", membershipId, locale, "estimates", "catalog", "unit-options", branchId, itemId],
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token || !membershipId || !branchId) {
        throw new Error("AUTHENTICATION_REQUIRED");
      }
      return apiClient.getEstimateUnitOptions(itemId, branchId, {
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
        signal,
      });
    },
    enabled: Boolean(membershipId && branchId),
    staleTime: 60 * 1000,
  });
}
