"use client";

import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type AttachItemImageRequest, type CategoryAttributeTemplateResponse, type CostRecordResponse, type CostReviewQueueParams, type CostReviewQueueResponse, type CostSourceRequest, type CostSourceResponse, type CreateCostRecordRequest, type CreateItemBarcodeRequest, type CreateItemBrandRequest, type CreateItemCategoryRequest, type CreateItemRequest, type CreateItemUnitConversionRequest, type CreateItemTaxCategoryRequest, type CreateUnitOfMeasureRequest, type ItemBarcodeResponse, type ItemBrandResponse, type ItemCategoryResponse, type ItemImageDetailResponse, type ItemResponse, type ItemTaxCategoryResponse, type ItemUnitConversionResponse, type ListItemsParams, type OrganizationBranchResponse, type PagedItemsResponse, type RequestOptions, type SetCategoryAttributeTemplatesRequest, type UnitConversionResponse, type UnitOfMeasureResponse, type UpdateCostRecordRequest, type UpdateCostSourceRequest, type UpdateItemBrandRequest, type UpdateItemCategoryRequest, type UpdateItemRequest, type UpdateItemTaxCategoryRequest, type UpdateUnitOfMeasureRequest } from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

const itemMasterKey = (membershipId: string | undefined, locale: "th" | "en") => ["business", membershipId, locale, "item-master"] as const;
export const itemListQueryKey = (membershipId: string | undefined, locale: "th" | "en", params: ListItemsParams) => [...itemMasterKey(membershipId, locale), "items", params] as const;
export const itemDetailQueryKey = (membershipId: string | undefined, locale: "th" | "en", id: string | undefined) => [...itemMasterKey(membershipId, locale), "item", id] as const;
export const categoryAttributeTemplateQueryKey = (membershipId: string | undefined, locale: "th" | "en", categoryId: string | undefined) => [...itemMasterKey(membershipId, locale), "category-attribute-template", categoryId] as const;
export const costSourcesQueryKey = (membershipId: string | undefined, locale: "th" | "en") => [...itemMasterKey(membershipId, locale), "cost-sources"] as const;
export const costReviewQueueQueryKey = (membershipId: string | undefined, locale: "th" | "en", params: CostReviewQueueParams) => [...itemMasterKey(membershipId, locale), "cost-review-queue", params] as const;
export const itemCostsQueryKey = (membershipId: string | undefined, locale: "th" | "en", itemId: string | undefined) => [...itemMasterKey(membershipId, locale), "item-costs", itemId] as const;
export const itemBarcodesQueryKey = (membershipId: string | undefined, locale: "th" | "en", itemId: string | undefined) => [...itemMasterKey(membershipId, locale), "item-barcodes", itemId] as const;
export const itemUnitConversionsQueryKey = (membershipId: string | undefined, locale: "th" | "en", itemId: string | undefined) => [...itemMasterKey(membershipId, locale), "item-unit-conversions", itemId] as const;
export const sharedUnitConversionsQueryKey = (membershipId: string | undefined, locale: "th" | "en") => [...itemMasterKey(membershipId, locale), "shared-unit-conversions"] as const;
export const branchesQueryKey = (membershipId: string | undefined, locale: "th" | "en") => ["business", membershipId, locale, "organization-branches"] as const;
export const itemImagesQueryKey = (membershipId: string | undefined, locale: "th" | "en", itemId: string | undefined) => [...itemMasterKey(membershipId, locale), "item-images", itemId] as const;

export function useOrganizationBranches(): UseQueryResult<OrganizationBranchResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: branchesQueryKey(membershipId, locale),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.listOrganizationBranches({ token, membershipId, locale, signal });
    },
  });
}

export function useItemImages(itemId: string | undefined): UseQueryResult<ItemImageDetailResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: itemImagesQueryKey(membershipId, locale, itemId),
    enabled: Boolean(membershipId && itemId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId || !itemId) throw new MembershipRequiredError();
      return apiClient.listItemImages(itemId, { token, membershipId, locale, signal });
    },
  });
}

export function useItemImageMutations(itemId: string) {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale };
  };
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: itemImagesQueryKey(membershipId, locale, itemId) });
    await queryClient.invalidateQueries({ queryKey: itemDetailQueryKey(membershipId, locale, itemId) });
  };
  const attach = useMutation({ mutationFn: async (payload: AttachItemImageRequest) => apiClient.attachItemImage(itemId, payload, await options()), onSuccess: refresh });
  const setPrimary = useMutation({ mutationFn: async (imageId: string) => apiClient.setPrimaryItemImage(itemId, imageId, await options()), onSuccess: refresh });
  const detach = useMutation({ mutationFn: async (imageId: string) => apiClient.detachItemImage(itemId, imageId, await options()), onSuccess: refresh });
  return { attach, setPrimary, detach };
}

export function useItemBarcodes(itemId: string | undefined): UseQueryResult<ItemBarcodeResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: itemBarcodesQueryKey(membershipId, locale, itemId),
    enabled: Boolean(membershipId && itemId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId || !itemId) throw new MembershipRequiredError();
      return apiClient.listItemBarcodes(itemId, { token, membershipId, locale, signal });
    },
  });
}

export function useItemUnitConversions(itemId: string | undefined): UseQueryResult<ItemUnitConversionResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: itemUnitConversionsQueryKey(membershipId, locale, itemId),
    enabled: Boolean(membershipId && itemId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId || !itemId) throw new MembershipRequiredError();
      return apiClient.listItemUnitConversions(itemId, { token, membershipId, locale, signal });
    },
  });
}

export function useItemUnitConversionMutations(itemId: string) {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const create = useMutation({
    mutationFn: async (payload: CreateItemUnitConversionRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.createItemUnitConversion(itemId, payload, { token, membershipId, locale, idempotencyKey: crypto.randomUUID() });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: itemUnitConversionsQueryKey(membershipId, locale, itemId) });
      await queryClient.invalidateQueries({ queryKey: itemBarcodesQueryKey(membershipId, locale, itemId) });
    },
  });
  return { create };
}

export function useSharedUnitConversions(): UseQueryResult<UnitConversionResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: sharedUnitConversionsQueryKey(membershipId, locale),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.listUnitConversions({ token, membershipId, locale, signal });
    },
  });
}

export function useSharedUnitConversionMutations() {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const create = useMutation({
    mutationFn: async (payload: CreateItemUnitConversionRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.createUnitConversion(payload, { token, membershipId, locale, idempotencyKey: crypto.randomUUID() });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: sharedUnitConversionsQueryKey(membershipId, locale) });
      await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "item-unit-conversions"] });
      await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "item-barcodes"] });
    },
  });
  return { create };
}

export function useItemBarcodeMutations(itemId: string) {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (ifMatch?: string, idempotencyKey?: string): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, ifMatch, idempotencyKey };
  };
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: itemBarcodesQueryKey(membershipId, locale, itemId) });
    await queryClient.invalidateQueries({ queryKey: itemDetailQueryKey(membershipId, locale, itemId) });
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "items"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "estimates", "catalog"] });
  };
  const create = useMutation({
    mutationFn: async (payload: CreateItemBarcodeRequest) => apiClient.createItemBarcode(itemId, payload, await options(undefined, crypto.randomUUID())),
    onSuccess: refresh,
  });
  const setPrimary = useMutation({
    mutationFn: async (input: { barcodeId: string; rowVersion: string }) => apiClient.setItemBarcodePrimary(itemId, input.barcodeId, input.rowVersion, await options(input.rowVersion)),
    onSuccess: refresh,
  });
  const deactivate = useMutation({
    mutationFn: async (input: { barcodeId: string; rowVersion: string }) => apiClient.deactivateItemBarcode(itemId, input.barcodeId, input.rowVersion, await options(input.rowVersion)),
    onSuccess: refresh,
  });
  return { create, setPrimary, deactivate };
}

export function useItemAliasMutations(itemId: string) {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (rowVersion?: string): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, ifMatch: rowVersion };
  };
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: itemDetailQueryKey(membershipId, locale, itemId) });
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "items"] });
  };
  const add = useMutation({ mutationFn: async (input: { alias: { thai: string; english?: string | null }; rowVersion?: string }) => apiClient.addItemAlias(itemId, { alias: input.alias }, await options(input.rowVersion)), onSuccess: refresh });
  const remove = useMutation({ mutationFn: async (input: { aliasId: string; rowVersion?: string }) => apiClient.removeItemAlias(itemId, input.aliasId, await options(input.rowVersion)), onSuccess: refresh });
  return { add, remove };
}

export function useItemList(params: ListItemsParams): UseQueryResult<PagedItemsResponse, Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: itemListQueryKey(membershipId, locale, params),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.listItems({ token, membershipId, locale, signal }, params);
    },
  });
}

export function useItemDetail(id: string | undefined): UseQueryResult<ItemResponse, Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: itemDetailQueryKey(membershipId, locale, id),
    enabled: Boolean(membershipId && id && id !== "create"),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId || !id) throw new MembershipRequiredError();
      return apiClient.getItem(id, { token, membershipId, locale, signal });
    },
  });
}

export function useCostSources(): UseQueryResult<CostSourceResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: costSourcesQueryKey(membershipId, locale),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.listCostSources({ token, membershipId, locale, signal });
    },
  });
}

export function useItemMasterLookups(): UseQueryResult<{ categories: ItemCategoryResponse[]; brands: ItemBrandResponse[]; units: UnitOfMeasureResponse[]; taxCategories: ItemTaxCategoryResponse[] }, Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: [...itemMasterKey(membershipId, locale), "lookups"],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      const options = { token, membershipId, locale, signal };
      const [categories, brands, units, taxCategories] = await Promise.all([
        apiClient.listItemCategories(options),
        apiClient.listItemBrands(options),
        apiClient.listUnitsOfMeasure(options),
        apiClient.listItemTaxCategories(options),
      ]);
      return { categories, brands, units, taxCategories };
    },
  });
}

export function useItemMasterReferenceData(): UseQueryResult<{ categories: ItemCategoryResponse[]; brands: ItemBrandResponse[]; units: UnitOfMeasureResponse[]; taxCategories: ItemTaxCategoryResponse[] }, Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: [...itemMasterKey(membershipId, locale), "reference-data"],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      const options = { token, membershipId, locale, signal };
      const [categories, brands, units, taxCategories] = await Promise.all([
        apiClient.listItemCategories(options), apiClient.listItemBrands(options), apiClient.listUnitsOfMeasure(options),
        apiClient.listItemTaxCategories(options),
      ]);
      return { categories, brands, units, taxCategories };
    },
  });
}

export function useItemMasterReferenceMutations() {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (rowVersion?: string, idempotencyKey?: string): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, ifMatch: rowVersion, idempotencyKey };
  };
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "reference-data"] });
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "lookups"] });
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "items"] });
  };
  const createCategory = useMutation({ mutationFn: async (payload: CreateItemCategoryRequest) => apiClient.createItemCategory(payload, await options(undefined, crypto.randomUUID())), onSuccess: refresh });
  const updateCategory = useMutation({ mutationFn: async ({ id, rowVersion, payload }: { id: string; rowVersion: string; payload: UpdateItemCategoryRequest }) => apiClient.updateItemCategory(id, payload, await options(rowVersion)), onSuccess: refresh });
  const createBrand = useMutation({ mutationFn: async (payload: CreateItemBrandRequest) => apiClient.createItemBrand(payload, await options(undefined, crypto.randomUUID())), onSuccess: refresh });
  const updateBrand = useMutation({ mutationFn: async ({ id, rowVersion, payload }: { id: string; rowVersion: string; payload: UpdateItemBrandRequest }) => apiClient.updateItemBrand(id, payload, await options(rowVersion)), onSuccess: refresh });
  const createTaxCategory = useMutation({ mutationFn: async (payload: CreateItemTaxCategoryRequest) => apiClient.createItemTaxCategory(payload, await options(undefined, crypto.randomUUID())), onSuccess: refresh });
  const updateTaxCategory = useMutation({ mutationFn: async ({ id, rowVersion, payload }: { id: string; rowVersion: string; payload: UpdateItemTaxCategoryRequest }) => apiClient.updateItemTaxCategory(id, payload, await options(rowVersion)), onSuccess: refresh });
  const createUnit = useMutation({ mutationFn: async (payload: CreateUnitOfMeasureRequest) => apiClient.createUnitOfMeasure(payload, await options(undefined, crypto.randomUUID())), onSuccess: refresh });
  const updateUnit = useMutation({ mutationFn: async ({ id, rowVersion, payload }: { id: string; rowVersion: string; payload: UpdateUnitOfMeasureRequest }) => apiClient.updateUnitOfMeasure(id, payload, await options(rowVersion)), onSuccess: refresh });
  return { createCategory, updateCategory, createBrand, updateBrand, createTaxCategory, updateTaxCategory, createUnit, updateUnit };
}

export function useCategoryAttributeTemplate(categoryId: string | undefined): UseQueryResult<CategoryAttributeTemplateResponse, Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: categoryAttributeTemplateQueryKey(membershipId, locale, categoryId),
    enabled: Boolean(membershipId && categoryId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId || !categoryId) throw new MembershipRequiredError();
      return apiClient.getCategoryAttributeTemplates(categoryId, { token, membershipId, locale, signal });
    },
  });
}

export function useCategoryAttributeTemplateMutations(categoryId: string | undefined) {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale };
  };
  const setTemplates = useMutation({
    mutationFn: async (payload: SetCategoryAttributeTemplatesRequest) => {
      if (!categoryId) throw new Error("CategoryId is required");
      return apiClient.setCategoryAttributeTemplates(categoryId, payload, await options());
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: categoryAttributeTemplateQueryKey(membershipId, locale, categoryId),
      });
    },
  });
  return { setTemplates };
}

export function useCostReviewQueue(params: CostReviewQueueParams): UseQueryResult<CostReviewQueueResponse, Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: costReviewQueueQueryKey(membershipId, locale, params),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.listCostReviewQueue(params, { token, membershipId, locale, signal });
    },
  });
}

export function useItemCosts(itemId: string | undefined): UseQueryResult<CostRecordResponse[], Error> {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  return useQuery({
    queryKey: itemCostsQueryKey(membershipId, locale, itemId),
    enabled: Boolean(membershipId && itemId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId || !itemId) throw new MembershipRequiredError();
      return apiClient.listItemCosts(itemId, { token, membershipId, locale, signal });
    },
  });
}

export function useItemCostMutations(itemId: string) {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (ifMatch?: string, idempotencyKey?: string): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, ifMatch, idempotencyKey };
  };
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: itemCostsQueryKey(membershipId, locale, itemId) });
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "cost-review-queue"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "estimates", "catalog"] });
  };
  const create = useMutation({ mutationFn: async ({ payload, idempotencyKey }: { payload: CreateCostRecordRequest; idempotencyKey: string }) => apiClient.createCostRecord(itemId, payload, await options(undefined, idempotencyKey)), onSuccess: refresh });
  const update = useMutation({ mutationFn: async ({ costId, rowVersion, payload }: { costId: string; rowVersion: string; payload: UpdateCostRecordRequest }) => apiClient.updateCostRecord(itemId, costId, payload, await options(rowVersion)), onSuccess: refresh });
  const submit = useMutation({ mutationFn: async ({ costId, rowVersion }: { costId: string; rowVersion: string }) => apiClient.submitCostRecord(itemId, costId, rowVersion, await options(rowVersion)), onSuccess: refresh });
  return { create, update, submit };
}

export function useCostReviewMutations() {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const requestOptions = async (): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale };
  };
  const invalidateQueue = async () => queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "cost-review-queue"] });
  const approve = useMutation({
    mutationFn: async (input: { itemId: string; costId: string; rowVersion: string }) => apiClient.approveCostRecord(input.itemId, input.costId, input.rowVersion, await requestOptions()),
    onSuccess: invalidateQueue,
  });
  const returnForChanges = useMutation({
    mutationFn: async (input: { itemId: string; costId: string; rowVersion: string; reason: string }) => apiClient.returnCostRecord(input.itemId, input.costId, input.rowVersion, input.reason, await requestOptions()),
    onSuccess: invalidateQueue,
  });
  const publish = useMutation({
    mutationFn: async (input: { itemId: string; costId: string; rowVersion: string; idempotencyKey: string }) => apiClient.publishCostRecord(input.itemId, input.costId, input.rowVersion, input.idempotencyKey, await requestOptions()),
    onSuccess: invalidateQueue,
  });
  return { approve, returnForChanges, publish };
}

export interface ItemMasterMutations {
  createItem: UseMutationResult<ItemResponse, Error, CreateItemRequest>;
  attachItemImage: UseMutationResult<ItemImageDetailResponse, Error, { id: string; payload: AttachItemImageRequest }>;
  updateItem: UseMutationResult<ItemResponse, Error, { id: string; rowVersion: string; payload: UpdateItemRequest }>;
  setBranchAvailability: UseMutationResult<ItemResponse, Error, { id: string; rowVersion: string; mode: string; branchIds: string[] }>;
  activateItem: UseMutationResult<ItemResponse, Error, { id: string; rowVersion: string }>;
  deactivateItem: UseMutationResult<ItemResponse, Error, { id: string; rowVersion: string; reasonCode: string; reason: string }>;
  createCostSource: UseMutationResult<CostSourceResponse, Error, { payload: CostSourceRequest; idempotencyKey: string }>;
  updateCostSource: UseMutationResult<CostSourceResponse, Error, { id: string; rowVersion: string; payload: UpdateCostSourceRequest }>;
  deactivateCostSource: UseMutationResult<CostSourceResponse, Error, { id: string; rowVersion: string }>;
}

export function useItemMasterMutations(): ItemMasterMutations {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (ifMatch?: string, idempotencyKey?: string): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, ifMatch, idempotencyKey };
  };
  const refreshItems = async () => {
    await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "items"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "estimates", "catalog"] });
  };
  const createItem = useMutation({ mutationFn: async (payload: CreateItemRequest) => apiClient.createItem(payload, await options(undefined, crypto.randomUUID())) , onSuccess: refreshItems });
  const updateItem = useMutation({
    mutationFn: async ({ id, rowVersion, payload }: { id: string; rowVersion: string; payload: UpdateItemRequest }) => apiClient.updateItem(id, payload, await options(rowVersion)),
    onSuccess: async (_result, input) => {
      await refreshItems();
      await queryClient.invalidateQueries({ queryKey: itemDetailQueryKey(membershipId, locale, input.id) });
    },
  });
  const setBranchAvailability = useMutation({
    mutationFn: async ({ id, rowVersion, mode, branchIds }: { id: string; rowVersion: string; mode: string; branchIds: string[] }) => apiClient.setItemBranchAvailability(id, { mode, branchIds }, await options(rowVersion)),
    onSuccess: async (_result, input) => {
      await refreshItems();
      await queryClient.invalidateQueries({ queryKey: itemDetailQueryKey(membershipId, locale, input.id) });
    },
  });
  const refreshItem = async (id: string) => {
    await refreshItems();
    await queryClient.invalidateQueries({ queryKey: itemDetailQueryKey(membershipId, locale, id) });
  };
  const attachItemImage = useMutation({
    mutationFn: async ({ id, payload }: { id: string; payload: AttachItemImageRequest }) => apiClient.attachItemImage(id, payload, await options()),
    onSuccess: async (_result, input) => {
      await refreshItem(input.id);
      await queryClient.invalidateQueries({ queryKey: itemImagesQueryKey(membershipId, locale, input.id) });
    },
  });
  const activateItem = useMutation({ mutationFn: async ({ id, rowVersion }: { id: string; rowVersion: string }) => apiClient.activateItem(id, await options(rowVersion)), onSuccess: async (_result, input) => refreshItem(input.id) });
  const deactivateItem = useMutation({ mutationFn: async ({ id, rowVersion, reasonCode, reason }: { id: string; rowVersion: string; reasonCode: string; reason: string }) => apiClient.deactivateItem(id, { reasonCode, reason }, await options(rowVersion)), onSuccess: async (_result, input) => refreshItem(input.id) });
  const createCostSource = useMutation({ mutationFn: async ({ payload, idempotencyKey }: { payload: CostSourceRequest; idempotencyKey: string }) => apiClient.createCostSource(payload, await options(undefined, idempotencyKey)), onSuccess: () => queryClient.invalidateQueries({ queryKey: costSourcesQueryKey(membershipId, locale) }) });
  const updateCostSource = useMutation({ mutationFn: async ({ id, rowVersion, payload }: { id: string; rowVersion: string; payload: UpdateCostSourceRequest }) => apiClient.updateCostSource(id, payload, await options(rowVersion)), onSuccess: () => queryClient.invalidateQueries({ queryKey: costSourcesQueryKey(membershipId, locale) }) });
  const deactivateCostSource = useMutation({ mutationFn: async ({ id, rowVersion }: { id: string; rowVersion: string }) => apiClient.deactivateCostSource(id, await options(rowVersion)), onSuccess: () => queryClient.invalidateQueries({ queryKey: costSourcesQueryKey(membershipId, locale) }) });
  return { createItem, attachItemImage, updateItem, setBranchAvailability, activateItem, deactivateItem, createCostSource, updateCostSource, deactivateCostSource };
}

export function useItemImport() {
  const queryClient = useQueryClient();
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (idempotencyKey?: string): Promise<RequestOptions> => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, idempotencyKey };
  };
  const preview = useMutation({
    mutationFn: async (content: string) => apiClient.previewItemImport({ content }, await options()),
  });
  const commit = useMutation({
    mutationFn: async ({ content, expectedContentSha256, idempotencyKey }: { content: string; expectedContentSha256: string; idempotencyKey: string }) =>
      apiClient.commitItemImport({ content, expectedContentSha256 }, await options(idempotencyKey)),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: [...itemMasterKey(membershipId, locale), "items"] });
    },
  });
  return { preview, commit };
}

