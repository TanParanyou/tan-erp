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
import { estimateCatalogQueryKey } from "./estimate-catalog-query-key";

export interface UseEstimateCatalogParams {
  branchId: string;
  search?: string;
  itemType?: string;
  categoryId?: string;
  brandId?: string;
  attributeKey?: string;
  attributeValue?: string;
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
  attributeKey,
  attributeValue,
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
    attributeKey: attributeKey || undefined,
    attributeValue: attributeValue || undefined,
    hasCost,
    cursor: cursor || undefined,
    pageSize,
  };

  return useQuery<CatalogModel>({
    queryKey: estimateCatalogQueryKey(membershipId, locale, query),
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
    placeholderData: (previousData, previousQuery) => {
      const previousKey = previousQuery?.queryKey;
      if (!previousKey) return undefined;
      return previousKey[1] === membershipId
        && previousKey[2] === locale
        && previousKey[5] === branchId
        && previousKey[12] === hasCost
        ? previousData
        : undefined;
    },
    staleTime: 60 * 1000,
  });
}
