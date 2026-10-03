import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type QuotationHistoryResponse, type QuotationLifecycleAction } from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

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

export function quotationHistoryKey(membershipId: string | null | undefined, locale: UiLocale, estimateId: string | null | undefined) {
  return ["business", membershipId, locale, "estimates", "quotation-history", estimateId] as const;
}

export function useQuotationHistory(estimateId: string, enabled = true): UseQueryResult<QuotationHistoryResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: quotationHistoryKey(membershipId, locale, estimateId),
    enabled: Boolean(membershipId && estimateId) && enabled,
    queryFn: async ({ signal }) => apiClient.getQuotationHistory(estimateId, await options(undefined, signal)),
  });
}

/** Void and amend. Both change which document is live, so the history and the printed document are refreshed together. */
export function useQuotationLifecycleMutation(estimateId: string) {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (input: { quotationId: string; rowVersion: string; action: QuotationLifecycleAction; reason: string; idempotencyKey: string }) =>
      apiClient.quotationLifecycle(input.quotationId, input.action, input.rowVersion, input.reason, await options(input.idempotencyKey)),
    onSuccess: async (history) => {
      queryClient.setQueryData(quotationHistoryKey(membershipId, locale, estimateId), history);
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "estimates"] });
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "opportunities"] });
    },
  });
}
