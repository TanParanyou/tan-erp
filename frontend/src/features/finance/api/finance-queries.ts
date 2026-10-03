import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type BillingListResponse,
  type BillingRequest,
  type BillingResponse,
  type DispatchResultResponse,
  type FinanceReconciliationResponse,
  type ListBillingsParams,
  type ListOutboxParams,
  type OutboxListResponse,
  type PaymentRequest,
  type ProjectBillingSummaryResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function financeKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "finance"] as const;
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

export function useBillingList(params: ListBillingsParams): UseQueryResult<BillingListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...financeKey(membershipId, locale), "billings", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listBillings(await options(undefined, signal), params),
  });
}

export function useBilling(id: string | undefined): UseQueryResult<BillingResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...financeKey(membershipId, locale), "billings", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getBilling(id ?? "", await options(undefined, signal)),
  });
}

export function useProjectBillingSummary(projectId: string | undefined): UseQueryResult<ProjectBillingSummaryResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...financeKey(membershipId, locale), "billings", "summary", projectId],
    enabled: Boolean(membershipId && projectId),
    queryFn: async ({ signal }) => apiClient.getProjectBillingSummary(projectId ?? "", await options(undefined, signal)),
  });
}

/** Billing and payment writes. Each call returns the fresh billing, which is cached so the page shows the new row version immediately. */
export function useBillingMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (billing: BillingResponse) => {
    queryClient.setQueryData([...financeKey(membershipId, locale), "billings", "detail", billing.id], billing);
    await queryClient.invalidateQueries({ queryKey: [...financeKey(membershipId, locale), "billings", "list"] });
    await queryClient.invalidateQueries({ queryKey: [...financeKey(membershipId, locale), "billings", "summary"] });
    // Every change queues a message for accounting.
    await queryClient.invalidateQueries({ queryKey: [...financeKey(membershipId, locale), "sync"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: BillingRequest; idempotencyKey: string }) =>
      apiClient.createBilling(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const voidBilling = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; reason: string }) =>
      apiClient.voidBilling(input.id, input.rowVersion, input.reason, await options()),
    onSuccess: store,
  });
  const pay = useMutation({
    mutationFn: async (input: { id: string; payload: PaymentRequest; idempotencyKey: string }) =>
      apiClient.recordPayment(input.id, input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const reverse = useMutation({
    mutationFn: async (input: { id: string; paymentId: string; rowVersion: string; reason: string }) =>
      apiClient.reversePayment(input.id, input.paymentId, input.rowVersion, input.reason, await options()),
    onSuccess: store,
  });
  return { create, voidBilling, pay, reverse };
}

export function useOutboxList(params: ListOutboxParams): UseQueryResult<OutboxListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...financeKey(membershipId, locale), "sync", "outbox", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listAccountingOutbox(await options(undefined, signal), params),
  });
}

export function useReconciliation(): UseQueryResult<FinanceReconciliationResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...financeKey(membershipId, locale), "sync", "reconciliation"],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.getFinanceReconciliation(await options(undefined, signal)),
  });
}

export function useSyncMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: [...financeKey(membershipId, locale), "sync"] });
  };
  const dispatch = useMutation<DispatchResultResponse, Error, void>({
    mutationFn: async () => apiClient.dispatchAccountingOutbox(await options()),
    onSuccess: refresh,
  });
  const requeue = useMutation({
    mutationFn: async (input: { id: string }) => apiClient.requeueAccountingMessage(input.id, await options()),
    onSuccess: refresh,
  });
  return { dispatch, requeue };
}
