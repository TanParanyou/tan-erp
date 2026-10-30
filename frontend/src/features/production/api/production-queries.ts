import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type BomDraftRequest,
  type BomListResponse,
  type BomRequest,
  type BomResponse,
  type BomRevisionAction,
  type CreateWorkCenterRequest,
  type GetWorkCenterCapacityParams,
  type ListBomsParams,
  type ListWorkCentersParams,
  type ListWorkOrdersParams,
  type UpdateWorkCenterRequest,
  type WorkCenterCapacityViewResponse,
  type WorkCenterListResponse,
  type WorkCenterResponse,
  type WorkOrderCompleteOperationRequest,
  type WorkOrderListResponse,
  type WorkOrderMaterialsRequest,
  type WorkOrderRequest,
  type WorkOrderResponse,
  type WorkOrderStockOperation,
  type WorkOrderSubcontractOperationRequest,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function productionKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "production"] as const;
}

function useRequestContext() {
  const locale: UiLocale = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (idempotencyKey?: string, signal?: AbortSignal) => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, idempotencyKey, signal };
  };
  return { locale, membershipId, options };
}

export function useWorkCenterList(params: ListWorkCentersParams, enabled = true): UseQueryResult<WorkCenterListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "work-centers", "list", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listWorkCenters(await options(undefined, signal), params),
  });
}

export function useWorkCenter(id: string | undefined): UseQueryResult<WorkCenterResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "work-centers", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getWorkCenter(id ?? "", await options(undefined, signal)),
  });
}

export function useWorkCenterCapacity(params: GetWorkCenterCapacityParams, enabled = true): UseQueryResult<WorkCenterCapacityViewResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "work-centers", "capacity", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getWorkCenterCapacity(await options(undefined, signal), params),
  });
}

export function useWorkCenterMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: [...productionKey(membershipId, locale), "work-centers"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: CreateWorkCenterRequest; idempotencyKey: string }) =>
      apiClient.createWorkCenter(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const update = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: UpdateWorkCenterRequest }) =>
      apiClient.updateWorkCenter(input.id, input.rowVersion, input.payload, await options()),
    onSuccess: refresh,
  });
  const setActive = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; active: boolean }) =>
      apiClient.setWorkCenterActive(input.id, input.rowVersion, input.active, await options()),
    onSuccess: refresh,
  });
  return { create, update, setActive };
}

export function useBomList(params: ListBomsParams): UseQueryResult<BomListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "boms", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listBoms(await options(undefined, signal), params),
  });
}

export function useBom(id: string | undefined): UseQueryResult<BomResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "boms", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getBom(id ?? "", await options(undefined, signal)),
  });
}

/** BOM writes. Each call returns the fresh BOM, which is cached so the page shows the new revision versions immediately. */
export function useBomMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (bom: BomResponse) => {
    queryClient.setQueryData([...productionKey(membershipId, locale), "boms", "detail", bom.id], bom);
    await queryClient.invalidateQueries({ queryKey: [...productionKey(membershipId, locale), "boms", "list"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: BomRequest; idempotencyKey: string }) =>
      apiClient.createBom(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const createRevision = useMutation({
    mutationFn: async (input: { id: string; payload: BomDraftRequest }) =>
      apiClient.createBomRevision(input.id, input.payload, await options()),
    onSuccess: store,
  });
  const updateDraft = useMutation({
    mutationFn: async (input: { id: string; revisionId: string; rowVersion: string; payload: BomDraftRequest }) =>
      apiClient.updateBomDraft(input.id, input.revisionId, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const act = useMutation({
    mutationFn: async (input: { id: string; revisionId: string; rowVersion: string; action: BomRevisionAction }) =>
      apiClient.bomRevisionAction(input.id, input.revisionId, input.action, input.rowVersion, await options()),
    onSuccess: store,
  });
  return { create, createRevision, updateDraft, act };
}

export function useWorkOrderList(params: ListWorkOrdersParams, enabled = true): UseQueryResult<WorkOrderListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "work-orders", "list", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listWorkOrders(await options(undefined, signal), params),
  });
}

export function useWorkOrder(id: string | undefined): UseQueryResult<WorkOrderResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...productionKey(membershipId, locale), "work-orders", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getWorkOrder(id ?? "", await options(undefined, signal)),
  });
}

/** Work order writes. Stock-affecting calls also refresh stock balances and movements. */
export function useWorkOrderMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (order: WorkOrderResponse) => {
    queryClient.setQueryData([...productionKey(membershipId, locale), "work-orders", "detail", order.id], order);
    await queryClient.invalidateQueries({ queryKey: [...productionKey(membershipId, locale), "work-orders", "list"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "inventory"] });
    await queryClient.invalidateQueries({ queryKey: [...productionKey(membershipId, locale), "work-centers", "capacity"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: WorkOrderRequest; idempotencyKey: string }) =>
      apiClient.createWorkOrder(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const release = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string }) => apiClient.releaseWorkOrder(input.id, input.rowVersion, await options()),
    onSuccess: store,
  });
  const cancel = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; reason: string }) =>
      apiClient.cancelWorkOrder(input.id, input.rowVersion, input.reason, await options()),
    onSuccess: store,
  });
  const materials = useMutation({
    mutationFn: async (input: { id: string; operation: WorkOrderStockOperation; payload: WorkOrderMaterialsRequest; idempotencyKey: string }) =>
      apiClient.workOrderMaterials(input.id, input.operation, input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const complete = useMutation({
    mutationFn: async (input: { id: string; quantity: number; idempotencyKey: string }) =>
      apiClient.completeWorkOrder(input.id, input.quantity, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const completeOperation = useMutation({
    mutationFn: async (input: { id: string; operationId: string; payload: WorkOrderCompleteOperationRequest; idempotencyKey: string }) =>
      apiClient.completeWorkOrderOperation(input.id, input.operationId, input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const subcontractOperation = useMutation({
    mutationFn: async (input: { id: string; operationId: string; payload: WorkOrderSubcontractOperationRequest; idempotencyKey: string }) =>
      apiClient.subcontractWorkOrderOperation(input.id, input.operationId, input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  return { create, release, cancel, materials, complete, completeOperation, subcontractOperation };
}
