import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type GoodsReceiptRequest,
  type ListPurchaseOrdersParams,
  type ListSuppliersParams,
  type PurchaseOrderAction,
  type PurchaseOrderListResponse,
  type PurchaseOrderRequest,
  type PurchaseOrderResponse,
  type SupplierListResponse,
  type SupplierRequest,
  type SupplierResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function procurementKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "procurement"] as const;
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

export function useSupplierList(params: ListSuppliersParams, enabled = true): UseQueryResult<SupplierListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "suppliers", "list", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listSuppliers(await options(undefined, signal), params),
  });
}

export function useSupplierMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "suppliers"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: SupplierRequest; idempotencyKey: string }) =>
      apiClient.createSupplier(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const update = useMutation({
    mutationFn: async (input: { supplier: SupplierResponse; payload: SupplierRequest }) =>
      apiClient.updateSupplier(input.supplier.id ?? "", input.supplier.rowVersion ?? "", input.payload, await options()),
    onSuccess: refresh,
  });
  const setActive = useMutation({
    mutationFn: async (input: { supplier: SupplierResponse; active: boolean }) =>
      apiClient.setSupplierActive(input.supplier.id ?? "", input.supplier.rowVersion ?? "", input.active, await options()),
    onSuccess: refresh,
  });
  return { create, update, setActive };
}

export function usePurchaseOrderList(params: ListPurchaseOrdersParams): UseQueryResult<PurchaseOrderListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "purchase-orders", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listPurchaseOrders(await options(undefined, signal), params),
  });
}

export function usePurchaseOrder(id: string | undefined): UseQueryResult<PurchaseOrderResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "purchase-orders", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getPurchaseOrder(id ?? "", await options(undefined, signal)),
  });
}

/** Purchase order writes. Each call returns the fresh order, which is cached so the page shows the new row version immediately. */
export function usePurchaseOrderMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (order: PurchaseOrderResponse) => {
    queryClient.setQueryData([...procurementKey(membershipId, locale), "purchase-orders", "detail", order.id], order);
    await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "purchase-orders", "list"] });
    // Approved orders commit project budget
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "projects"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: PurchaseOrderRequest; idempotencyKey: string }) =>
      apiClient.createPurchaseOrder(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const update = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: PurchaseOrderRequest }) =>
      apiClient.updatePurchaseOrder(input.id, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const act = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; action: PurchaseOrderAction; note: string | null }) =>
      apiClient.purchaseOrderAction(input.id, input.action, input.rowVersion, input.note, await options()),
    onSuccess: store,
  });
  const receive = useMutation({
    mutationFn: async (input: { id: string; payload: GoodsReceiptRequest; idempotencyKey: string }) =>
      apiClient.postGoodsReceipt(input.id, input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  return { create, update, act, receive };
}
