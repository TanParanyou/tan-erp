"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, type EstimateCatalogResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";

export interface UseEstimateCatalogParams {
  branchId: string;
  search?: string;
  itemType?: string;
  categoryId?: string;
  brandId?: string;
  hasCost?: boolean;
  cursor?: string;
  pageSize?: number;
  enabled?: boolean;
}

export function useEstimateCatalog({
  branchId,
  search,
  itemType,
  categoryId,
  brandId,
  hasCost,
  cursor,
  pageSize = 50,
  enabled = true,
}: UseEstimateCatalogParams) {
  const membershipContext = useOptionalSelectedMembership();
  const membershipId = membershipContext?.selectedMembership?.id;
  const locale = useSafeLocale();

  return useQuery<EstimateCatalogResponse>({
    queryKey: [
      "estimates",
      "catalog",
      membershipId,
      branchId,
      search,
      itemType,
      categoryId,
      brandId,
      hasCost,
      cursor,
      pageSize,
    ],
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token || !membershipId) {
        throw new Error("AUTHENTICATION_REQUIRED");
      }

      return apiClient.getEstimateCatalog(
        {
          branchId,
          search: search?.trim() || undefined,
          itemType: itemType || undefined,
          categoryId: categoryId || undefined,
          brandId: brandId || undefined,
          hasCost,
          cursor: cursor || undefined,
          pageSize,
        },
        {
          token,
          membershipId,
          locale: locale === "en" ? "en" : "th",
          signal,
        }
      );
    },
    enabled: Boolean(enabled && branchId && membershipId),
    staleTime: 60 * 1000,
  });
}
