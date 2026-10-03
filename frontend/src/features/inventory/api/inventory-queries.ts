import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type AdjustStockRequest,
  type IssueStockRequest,
  type ListStockBalancesParams,
  type ListStockMovementsParams,
  type ListWarehousesParams,
  type ReconciliationResponse,
  type ReserveStockRequest,
  type StockBalanceListResponse,
  type StockMovementListResponse,
  type TransferStockRequest,
  type WarehouseListResponse,
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

/** Stock postings. Every successful post refreshes balances, the ledger and the reconciliation, and the purchase orders that show receipt status. */
export function useStockMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: inventoryKey(membershipId, locale) });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "procurement"] });
  };
  const receive = useMutation({
    mutationFn: async (input: { goodsReceiptId: string; warehouseId: string; idempotencyKey: string }) =>
      apiClient.receiveGoodsReceiptIntoStock(input.goodsReceiptId, input.warehouseId, await options(input.idempotencyKey)),
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
  const reserve = useMutation({
    mutationFn: async (input: { payload: ReserveStockRequest; idempotencyKey: string }) => apiClient.reserveStock(input.payload, await options(input.idempotencyKey)),
    onSuccess: refresh,
  });
  return { receive, issue, transfer, adjust, reserve };
}
