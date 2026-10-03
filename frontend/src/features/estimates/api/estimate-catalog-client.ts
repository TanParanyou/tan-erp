import { apiClient } from "@/lib/api/api-client";
import type { components, paths } from "@/generated/api/tan-erp.v1";

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
  name: LocalizedTextResponse & { thai: string };
  description?: LocalizedTextResponse;
  itemType: string;
  costComponentType: string;
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
  attributes?: Record<string, string>;
  resolvedCost?: {
    costRecordId: string;
    version: number;
    amount: number;
    currency: string;
    unitCode: string;
    scope: string;
    effectiveFromUtc: string;
    policyVersion?: string | null;
    costSourceId?: string | null;
    costSourceCode?: string | null;
    sourceReference?: string | null;
    evidenceFileId?: string | null;
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

  if (!item.costComponentType || typeof item.costComponentType !== "string") {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required 'costComponentType'.`);
  }

  if (!item.category || !item.category.id || !item.category.code || !item.category.name?.thai) {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required 'category'.`);
  }

  if (!item.baseUnit || !item.baseUnit.id || !item.baseUnit.code || !item.baseUnit.name?.thai || !item.baseUnit.symbol) {
    throw new EstimateCatalogContractError(`Item '${item.code}' missing required 'baseUnit'.`);
  }

  if (item.brand && (!item.brand.id || !item.brand.code || !item.brand.name?.thai)) {
    throw new EstimateCatalogContractError(`Item '${item.code}' has invalid 'brand'.`);
  }

  const attributes: Record<string, string> = {};
  if (item.attributes && typeof item.attributes === "object" && !Array.isArray(item.attributes)) {
    for (const [key, value] of Object.entries(item.attributes)) {
      if (typeof value === "string") attributes[key] = value;
    }
  }

  let resolvedCost: CatalogItemModel["resolvedCost"];
  if (item.resolvedCost) {
    const cost = item.resolvedCost;
    if (!cost.costRecordId || typeof cost.amount !== "number" || !Number.isFinite(cost.amount)
      || typeof cost.version !== "number" || cost.version < 1 || !cost.currency || !cost.unitCode
      || !cost.scope || !cost.effectiveFromUtc || Number.isNaN(Date.parse(cost.effectiveFromUtc))) {
      throw new EstimateCatalogContractError(`Item '${item.code}' has invalid resolvedCost contract.`);
    }
    resolvedCost = {
      costRecordId: cost.costRecordId,
      version: cost.version,
      amount: cost.amount,
      currency: cost.currency,
      unitCode: cost.unitCode,
      scope: cost.scope,
      effectiveFromUtc: cost.effectiveFromUtc,
      policyVersion: cost.policyVersion,
      costSourceId: cost.costSourceId,
      costSourceCode: cost.costSourceCode,
      sourceReference: cost.sourceReference,
      evidenceFileId: cost.evidenceFileId,
    };
  }

  let primaryImage: CatalogItemModel["primaryImage"];
  if (item.primaryImage && !item.primaryImage.fileId) {
    throw new EstimateCatalogContractError(`Item '${item.code}' has invalid 'primaryImage'.`);
  }
  if (item.primaryImage?.fileId) {
    primaryImage = {
      fileId: item.primaryImage.fileId,
      altText: item.primaryImage.altText,
    };
  }

  return {
    id: item.id,
    code: item.code,
    name: { thai: item.name.thai, english: item.name.english },
    description: item.description,
    itemType: item.itemType,
    costComponentType: item.costComponentType,
    category: {
      id: item.category.id,
      code: item.category.code,
      name: item.category.name,
      parentCategoryId: item.category.parentCategoryId,
    },
    brand: item.brand?.id
      ? {
          id: item.brand.id,
          code: item.brand.code!,
          name: item.brand.name!,
        }
      : undefined,
    baseUnit: {
      id: item.baseUnit.id,
      code: item.baseUnit.code,
      name: item.baseUnit.name,
      symbol: item.baseUnit.symbol,
    },
    attributes,
    primaryImage,
    resolvedCost,
  };
}

export function validateAndMapCatalogResponse(response: unknown): CatalogModel {
  if (!response || typeof response !== "object") {
    throw new EstimateCatalogContractError("Catalog response must be a non-null object.");
  }

  const resp = response as Partial<EstimateCatalogResponse>;
  if (!Array.isArray(resp.items) || !resp.facets || !Array.isArray(resp.facets.itemTypes)
    || !Array.isArray(resp.facets.categories) || !Array.isArray(resp.facets.brands)
    || !Array.isArray(resp.facets.attributes)
    || !resp.pageInfo || typeof resp.pageInfo.hasNextPage !== "boolean") {
    throw new EstimateCatalogContractError("Catalog response is missing required page or facet fields.");
  }

  const rawItems = resp.items;
  const items = rawItems.map(validateAndMapCatalogItem);

  const facets: CatalogFacetsResponse = resp.facets;

  const pageInfo = {
    nextCursor: resp.pageInfo?.nextCursor ?? null,
    hasNextPage: resp.pageInfo.hasNextPage,
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
  if (locale !== "en" && text.thai && text.thai.trim().length > 0) {
    return text.thai;
  }
  return emptyFallback;
}

export type FetchCatalogQuery = NonNullable<paths["/api/v1/estimate-catalog/items"]["get"]["parameters"]["query"]>;

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
