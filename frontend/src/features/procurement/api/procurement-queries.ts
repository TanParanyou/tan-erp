import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type ConvertPurchaseRequestsRequest,
  type GoodsReceiptRequest,
  type ListPurchaseOrdersParams,
  type ListPurchaseRequestsParams,
  type ListSupplierInvoicesParams,
  type ListSuppliersParams,
  type PurchaseOrderAction,
  type PurchaseOrderListResponse,
  type PurchaseOrderRequest,
  type PurchaseOrderResponse,
  type PurchaseRequestAction,
  type PurchaseRequestListResponse,
  type PurchaseRequestRequest,
  type PurchaseRequestResponse,
  type SupplierInvoiceAction,
  type SupplierInvoiceListResponse,
  type SupplierInvoiceRequest,
  type SupplierInvoiceResponse,
  type SupplierListResponse,
  type SupplierRequest,
  type SupplierResponse,
  type SupplierReturnRequest,
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
  const returnGoods = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: SupplierReturnRequest; idempotencyKey?: string }) =>
      apiClient.postSupplierReturn(input.id, input.rowVersion, input.payload, await options(input.idempotencyKey)),
    onSuccess: async (order) => {
      await store(order);
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "inventory"] });
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "projects"] });
    },
  });
  return { create, update, act, receive, returnGoods };
}

export function usePurchaseRequestList(params: ListPurchaseRequestsParams): UseQueryResult<PurchaseRequestListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "purchase-requests", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listPurchaseRequests(await options(undefined, signal), params),
  });
}

export function usePurchaseRequest(id: string | undefined): UseQueryResult<PurchaseRequestResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "purchase-requests", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getPurchaseRequest(id ?? "", await options(undefined, signal)),
  });
}

export function usePurchaseRequestMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (request: PurchaseRequestResponse) => {
    queryClient.setQueryData([...procurementKey(membershipId, locale), "purchase-requests", "detail", request.id], request);
    await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "purchase-requests", "list"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: PurchaseRequestRequest; idempotencyKey?: string }) =>
      apiClient.createPurchaseRequest(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const update = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: PurchaseRequestRequest }) =>
      apiClient.updatePurchaseRequest(input.id, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const act = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; action: PurchaseRequestAction; note: string | null }) =>
      apiClient.purchaseRequestAction(input.id, input.action, input.rowVersion, input.note, await options()),
    onSuccess: store,
  });
  const convert = useMutation({
    mutationFn: async (input: { payload: ConvertPurchaseRequestsRequest; idempotencyKey?: string }) =>
      apiClient.convertPurchaseRequests(input.payload, await options(input.idempotencyKey)),
    onSuccess: async (order) => {
      queryClient.setQueryData([...procurementKey(membershipId, locale), "purchase-orders", "detail", order.id], order);
      await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "purchase-orders"] });
      await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "purchase-requests"] });
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "projects"] });
    },
  });
  return { create, update, act, convert };
}

export function useSupplierInvoiceList(params: ListSupplierInvoicesParams): UseQueryResult<SupplierInvoiceListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "supplier-invoices", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listSupplierInvoices(await options(undefined, signal), params),
  });
}

export function useSupplierInvoice(id: string | undefined): UseQueryResult<SupplierInvoiceResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...procurementKey(membershipId, locale), "supplier-invoices", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getSupplierInvoice(id ?? "", await options(undefined, signal)),
  });
}

export function useSupplierInvoiceMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (invoice: SupplierInvoiceResponse) => {
    queryClient.setQueryData([...procurementKey(membershipId, locale), "supplier-invoices", "detail", invoice.id], invoice);
    await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "supplier-invoices", "list"] });
    await queryClient.invalidateQueries({ queryKey: [...procurementKey(membershipId, locale), "purchase-orders"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "projects"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "finance"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: SupplierInvoiceRequest; idempotencyKey?: string }) =>
      apiClient.createSupplierInvoice(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const update = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: SupplierInvoiceRequest }) =>
      apiClient.updateSupplierInvoice(input.id, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const act = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; action: SupplierInvoiceAction; note: string | null }) =>
      apiClient.supplierInvoiceAction(input.id, input.action, input.rowVersion, input.note, await options()),
    onSuccess: store,
  });
  return { create, update, act };
}
