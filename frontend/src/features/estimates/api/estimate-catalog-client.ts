import { apiClient } from "@/lib/api/api-client";
import type { components } from "@/generated/api/tan-erp.v1";

export class EstimateCatalogContractError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "EstimateCatalogContractError";
  }
}

export type LocalizedTextResponse = components["schemas"]["LocalizedTextResponse"];
export type EstimateCatalogItemResponse = components["schemas"]["EstimateCatalogItemResponse"];
export type CatalogResolvedCostResponse = components["schemas"]["CatalogResolvedCostResponse"];
export type CatalogPrimaryImageResponse = components["schemas"]["CatalogPrimaryImageResponse"];
export type CatalogFacetsResponse = components["schemas"]["CatalogFacetsResponse"];
export type CatalogPageInfoResponse = components["schemas"]["CatalogPageInfoResponse"];
export type EstimateCatalogResponse = components["schemas"]["EstimateCatalogResponse"];

export interface CatalogItemModel {
  id: string;
  code: string;
  name: LocalizedTextResponse;
  description?: LocalizedTextResponse;
  itemType: string;
  category: {
    id: string;
    code: string;
    name: LocalizedTextResponse;
    parentCategoryId?: string | null;
  };
  brand?: {
    id: string;
    code: string;
    name: LocalizedTextResponse;
  };
  baseUnit: {
    id: string;
    code: string;
    name: LocalizedTextResponse;
    symbol: string;
  };
  primaryImage?: {
    fileId: string;
    altText?: LocalizedTextResponse;
  };
  resolvedCost?: {
    costRecordId: string;
    version: number;
    amount: number;
    currency: string;
    unitCode: string;
    scope: string;
    effectiveFromUtc: string;
    policyVersion?: string | null;
  };
}

export interface CatalogModel {
  items: CatalogItemModel[];
  facets: CatalogFacetsResponse;
  pageInfo: {
    nextCursor?: string | null;
    hasNextPage: boolean;
  };
}

export function validateAndMapCatalogItem(raw: unknown): CatalogItemModel {
  if (!raw || typeof raw !== "object") {
    throw new EstimateCatalogContractError("Catalog item must be a non-null object.");
  }

  const item = raw as Partial<EstimateCatalogItemResponse>;

  if (!item.id || typeof item.id !== "string") {
    throw new EstimateCatalogContractError("Item 'id' is required and must be a string.");
  }

  if (!item.code || typeof item.code !== "string") {
    throw new EstimateCatalogContractError(`Item '${item.id}' missing required 'code'.`);
  }

  if (!item.name || typeof item.name !== "object" || !item.name.thai) {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required Thai name.`);
  }

  if (!item.itemType || typeof item.itemType !== "string") {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required 'itemType'.`);
  }

  if (!item.category || typeof item.category !== "object" || !item.category.id) {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required 'category'.`);
  }

  if (!item.baseUnit || typeof item.baseUnit !== "object" || !item.baseUnit.id) {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required 'baseUnit'.`);
  }

  let resolvedCost: CatalogItemModel["resolvedCost"];
  if (item.resolvedCost) {
    const cost = item.resolvedCost;
    if (!cost.costRecordId || typeof cost.amount !== "number" || !cost.currency || !cost.unitCode) {
      throw new EstimateCatalogContractError(`Item '${item.code}' has invalid resolvedCost contract.`);
    }
    resolvedCost = {
      costRecordId: cost.costRecordId,
      version: cost.version ?? 1,
      amount: cost.amount,
      currency: cost.currency,
      unitCode: cost.unitCode,
      scope: cost.scope ?? "organization",
      effectiveFromUtc: cost.effectiveFromUtc ?? new Date().toISOString(),
      policyVersion: cost.policyVersion,
    };
  }

  let primaryImage: CatalogItemModel["primaryImage"];
  if (item.primaryImage && item.primaryImage.fileId) {
    primaryImage = {
      fileId: item.primaryImage.fileId,
      altText: item.primaryImage.altText,
    };
  }

  return {
    id: item.id,
    code: item.code,
    name: item.name,
    description: item.description,
    itemType: item.itemType,
    category: {
      id: item.category.id,
      code: item.category.code ?? "",
      name: item.category.name ?? { thai: "-" },
      parentCategoryId: item.category.parentCategoryId,
    },
    brand: item.brand?.id
      ? {
          id: item.brand.id,
          code: item.brand.code ?? "",
          name: item.brand.name ?? { thai: "-" },
        }
      : undefined,
    baseUnit: {
      id: item.baseUnit.id,
      code: item.baseUnit.code ?? "",
      name: item.baseUnit.name ?? { thai: "-" },
      symbol: item.baseUnit.symbol ?? "",
    },
    primaryImage,
    resolvedCost,
  };
}

export function validateAndMapCatalogResponse(response: unknown): CatalogModel {
  if (!response || typeof response !== "object") {
    throw new EstimateCatalogContractError("Catalog response must be a non-null object.");
  }

  const resp = response as Partial<EstimateCatalogResponse>;
  const rawItems = resp.items ?? [];
  const items = rawItems.map(validateAndMapCatalogItem);

  const facets: CatalogFacetsResponse = resp.facets ?? {
    itemTypes: [],
    categories: [],
    brands: [],
  };

  const pageInfo = {
    nextCursor: resp.pageInfo?.nextCursor ?? null,
    hasNextPage: Boolean(resp.pageInfo?.hasNextPage),
  };

  return { items, facets, pageInfo };
}

export function getLocalizedText(
  text?: LocalizedTextResponse | null,
  locale?: string,
  emptyFallback = "-"
): string {
  if (!text) return emptyFallback;
  if (locale === "en" && text.english && text.english.trim().length > 0) {
    return text.english;
  }
  if (text.thai && text.thai.trim().length > 0) {
    return text.thai;
  }
  if (text.english && text.english.trim().length > 0) {
    return text.english;
  }
  return emptyFallback;
}

export interface FetchCatalogQuery {
  branchId: string;
  search?: string;
  itemType?: string;
  categoryId?: string;
  brandId?: string;
  hasCost?: boolean;
  cursor?: string;
  pageSize?: number;
}

export interface FetchCatalogOptions {
  token: string;
  membershipId: string;
  locale?: "en" | "th";
  signal?: AbortSignal;
}

export async function fetchEstimateCatalog(
  query: FetchCatalogQuery,
  options: FetchCatalogOptions
): Promise<CatalogModel> {
  const rawResponse = await apiClient.getEstimateCatalog(query, options);
  return validateAndMapCatalogResponse(rawResponse);
}
