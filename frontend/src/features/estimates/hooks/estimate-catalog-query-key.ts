import type { FetchCatalogQuery } from "../api/estimate-catalog-client";

export function estimateCatalogQueryKey(
  membershipId: string | undefined,
  locale: "th" | "en",
  query: FetchCatalogQuery,
) {
  return [
    "business",
    membershipId,
    locale,
    "estimates",
    "catalog",
    query.branchId,
    query.search,
    query.itemType,
    query.categoryId,
    query.brandId,
    query.attributeKey,
    query.attributeValue,
    query.hasCost,
    query.cursor,
    query.pageSize,
  ] as const;
}

export function isEstimateCatalogQueryKey(queryKey: readonly unknown[]): boolean {
  return queryKey[0] === "business"
    && queryKey[3] === "estimates"
    && queryKey[4] === "catalog";
}
