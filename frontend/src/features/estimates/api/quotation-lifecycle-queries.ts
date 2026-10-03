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

export function acceptanceLinksKey(membershipId: string | null | undefined, locale: UiLocale, quotationId: string | null | undefined) {
  return ["business", membershipId, locale, "estimates", "acceptance-links", quotationId] as const;
}

export function useAcceptanceLinks(quotationId: string | undefined, enabled = true) {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: acceptanceLinksKey(membershipId, locale, quotationId),
    enabled: Boolean(membershipId && quotationId) && enabled,
    queryFn: async ({ signal }) => apiClient.listAcceptanceLinks(quotationId ?? "", await options(undefined, signal)),
  });
}

/** Creating returns the token once; revoking refreshes the list. Quotation history is refreshed because acceptance changes its status. */
export function useAcceptanceLinkMutations(quotationId: string) {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: acceptanceLinksKey(membershipId, locale, quotationId) });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "estimates"] });
  };
  const create = useMutation({
    mutationFn: async (input: { lifetimeDays: number; signerHint: string | null }) =>
      apiClient.createAcceptanceLink(quotationId, { lifetimeDays: input.lifetimeDays, signerHint: input.signerHint }, await options()),
    onSuccess: refresh,
  });
  const revoke = useMutation({
    mutationFn: async (input: { linkId: string }) => apiClient.revokeAcceptanceLink(input.linkId, await options()),
    onSuccess: refresh,
  });
  return { create, revoke };
}
