"use client";

import { useQuery } from "@tanstack/react-query";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";
import {
  fetchEstimateCatalog,
  type CatalogModel,
  type FetchCatalogQuery,
} from "../api/estimate-catalog-client";

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

  const query: FetchCatalogQuery = {
    branchId,
    search: search?.trim() || undefined,
    itemType: itemType || undefined,
    categoryId: categoryId || undefined,
    brandId: brandId || undefined,
    hasCost,
    cursor: cursor || undefined,
    pageSize,
  };

  return useQuery<CatalogModel>({
    queryKey: [
      "estimates",
      "catalog",
      membershipId,
      branchId,
      query.search,
      query.itemType,
      query.categoryId,
      query.brandId,
      query.hasCost,
      query.cursor,
      query.pageSize,
    ],
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token || !membershipId) {
        throw new Error("AUTHENTICATION_REQUIRED");
      }

      return fetchEstimateCatalog(query, {
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
        signal,
      });
    },
    enabled: Boolean(enabled && branchId && membershipId),
    staleTime: 60 * 1000,
  });
}
