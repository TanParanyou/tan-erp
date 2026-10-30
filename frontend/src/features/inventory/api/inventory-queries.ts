import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type AdjustStockRequest,
  type CloseInventoryPeriodRequest,
  type CreateWarehouseLocationRequest,
  type InventoryPeriodCloseListResponse,
  type InventoryPeriodCloseResponse,
  type IssueStockRequest,
  type ListStockBalancesParams,
  type ListStockLocationBalancesParams,
  type ListStockMovementsParams,
  type ListStockSerialsParams,
  type ListWarehousesParams,
  type ReceiveGoodsReceiptLineRequest,
  type ReconciliationResponse,
  type ReserveStockRequest,
  type ReturnStockRequest,
  type StockBalanceListResponse,
  type StockLocationBalanceListResponse,
  type StockMovementListResponse,
  type StockSerialListResponse,
  type TransferStockRequest,
  type UpdateWarehouseLocationRequest,
  type WarehouseListResponse,
  type WarehouseLocationListResponse,
  type WarehouseLocationResponse,
  type WarehouseRequest,
  type WarehouseResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function inventoryKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "inventory"] as const;
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

export function useWarehouseList(params: ListWarehousesParams, enabled = true): UseQueryResult<WarehouseListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "warehouses", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listWarehouses(await options(undefined, signal), params),
  });
}

export function useWarehouseLocations(warehouseId: string | undefined, enabled = true): UseQueryResult<WarehouseLocationListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "warehouses", warehouseId, "locations"],
    enabled: Boolean(membershipId) && Boolean(warehouseId) && enabled,
    queryFn: async ({ signal }) => apiClient.listWarehouseLocations(warehouseId!, await options(undefined, signal)),
  });
}

export function useWarehouseLocationMutations(warehouseId: string | undefined) {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({
      queryKey: [...inventoryKey(membershipId, locale), "warehouses", warehouseId, "locations"],
    });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: CreateWarehouseLocationRequest; idempotencyKey?: string }) =>
      apiClient.createWarehouseLocation(warehouseId!, input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const update = useMutation({
    mutationFn: async (input: { location: WarehouseLocationResponse; payload: UpdateWarehouseLocationRequest }) =>
      apiClient.updateWarehouseLocation(
        warehouseId!,
        input.location.id ?? "",
        input.location.rowVersion ?? "",
        input.payload,
        await options()
      ),
    onSuccess: refresh,
  });
  return { create, update };
}

export function useWarehouseMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: [...inventoryKey(membershipId, locale), "warehouses"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: WarehouseRequest; idempotencyKey: string }) => apiClient.createWarehouse(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const update = useMutation({
    mutationFn: async (input: { warehouse: WarehouseResponse; payload: WarehouseRequest }) =>
      apiClient.updateWarehouse(input.warehouse.id ?? "", input.warehouse.rowVersion ?? "", input.payload, await options()),
    onSuccess: refresh,
  });
  const setActive = useMutation({
    mutationFn: async (input: { warehouse: WarehouseResponse; active: boolean }) =>
      apiClient.setWarehouseActive(input.warehouse.id ?? "", input.warehouse.rowVersion ?? "", input.active, await options()),
    onSuccess: refresh,
  });
  return { create, update, setActive };
}

export function useStockBalances(params: ListStockBalancesParams): UseQueryResult<StockBalanceListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "balances", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listStockBalances(await options(undefined, signal), params),
  });
}

export function useStockLocationBalances(params: ListStockLocationBalancesParams, enabled = true): UseQueryResult<StockLocationBalanceListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "balances", "locations", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listStockLocationBalances(await options(undefined, signal), params),
  });
}

export function useStockSerials(params: ListStockSerialsParams, enabled = true): UseQueryResult<StockSerialListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "serials", params],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listStockSerials(await options(undefined, signal), params),
  });
}

export function useStockMovements(params: ListStockMovementsParams): UseQueryResult<StockMovementListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "movements", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listStockMovements(await options(undefined, signal), params),
  });
}

export function useStockReconciliation(): UseQueryResult<ReconciliationResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "reconciliation"],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.getStockReconciliation(await options(undefined, signal)),
  });
}

export function useInventoryPeriodCloses(enabled = true): UseQueryResult<InventoryPeriodCloseListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "period-closes"],
    enabled: Boolean(membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listInventoryPeriodCloses(await options(undefined, signal)),
  });
}

export function useInventoryPeriodClose(id: string | undefined, enabled = true): UseQueryResult<InventoryPeriodCloseResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...inventoryKey(membershipId, locale), "period-closes", id],
    enabled: Boolean(membershipId) && Boolean(id) && enabled,
    queryFn: async ({ signal }) => apiClient.getInventoryPeriodClose(id!, await options(undefined, signal)),
  });
}

export function useCloseInventoryPeriod() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (input: { payload: CloseInventoryPeriodRequest; idempotencyKey?: string }) =>
      apiClient.closeInventoryPeriod(input.payload, await options(input.idempotencyKey)),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: inventoryKey(membershipId, locale) });
    },
  });
}

/** Stock postings. Every successful post refreshes balances, the ledger and the reconciliation, and the purchase orders that show receipt status. */
export function useStockMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: inventoryKey(membershipId, locale) });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "procurement"] });
  };
  const receive = useMutation({
    mutationFn: async (input: {
      goodsReceiptId: string;
      warehouseId: string;
      idempotencyKey: string;
      locationId?: string | null;
      lines?: ReceiveGoodsReceiptLineRequest[];
    }) =>
      apiClient.receiveGoodsReceiptIntoStock(
        input.goodsReceiptId,
        input.warehouseId,
        await options(input.idempotencyKey),
        { locationId: input.locationId, lines: input.lines }
      ),
    onSuccess: refresh,
  });
  const issue = useMutation({
    mutationFn: async (input: { payload: IssueStockRequest; idempotencyKey: string }) => apiClient.issueStock(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const transfer = useMutation({
    mutationFn: async (input: { payload: TransferStockRequest; idempotencyKey: string }) => apiClient.transferStock(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const adjust = useMutation({
    mutationFn: async (input: { payload: AdjustStockRequest; idempotencyKey: string }) => apiClient.adjustStock(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  const returnStock = useMutation({
    mutationFn: async (input: { payload: ReturnStockRequest; idempotencyKey: string }) =>
      apiClient.returnStock(input.payload, await options(input.idempotencyKey)),
    onSuccess: async () => {
      await refresh();
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "projects"] });
    },
  });
  const reserve = useMutation({
    mutationFn: async (input: { payload: ReserveStockRequest; idempotencyKey: string }) => apiClient.reserveStock(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  return { receive, issue, transfer, adjust, returnStock, reserve };
}
