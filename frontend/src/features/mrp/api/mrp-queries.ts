import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type ListMrpRunsParams,
  type MrpConvertRequest,
  type MrpRecommendationDecision,
  type MrpRunListResponse,
  type MrpRunRequest,
  type MrpRunResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function mrpKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "mrp"] as const;
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

export function useMrpRunList(params: ListMrpRunsParams): UseQueryResult<MrpRunListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...mrpKey(membershipId, locale), "runs", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listMrpRuns(await options(undefined, signal), params),
  });
}

export function useMrpRun(id: string | undefined): UseQueryResult<MrpRunResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...mrpKey(membershipId, locale), "runs", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getMrpRun(id ?? "", await options(undefined, signal)),
  });
}

/** Planning run writes. Each call returns the fresh run, which is cached so the page shows the new recommendation versions immediately. */
export function useMrpMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (run: MrpRunResponse) => {
    queryClient.setQueryData([...mrpKey(membershipId, locale), "runs", "detail", run.id], run);
    await queryClient.invalidateQueries({ queryKey: [...mrpKey(membershipId, locale), "runs", "list"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: MrpRunRequest; idempotencyKey: string }) =>
      apiClient.createMrpRun(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const decide = useMutation({
    mutationFn: async (input: { runId: string; recommendationId: string; rowVersion: string; decision: MrpRecommendationDecision }) =>
      apiClient.decideMrpRecommendation(input.runId, input.recommendationId, input.decision, input.rowVersion, await options()),
    onSuccess: store,
  });
  const convert = useMutation({
    mutationFn: async (input: { runId: string; recommendationId: string; rowVersion: string; payload: MrpConvertRequest }) =>
      apiClient.convertMrpRecommendation(input.runId, input.recommendationId, input.rowVersion, input.payload, await options()),
    onSuccess: async (run) => {
      await store(run);
      // Converting creates a draft Purchase Order or Work Order.
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "procurement"] });
      await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "production"] });
    },
  });
  return { create, decide, convert };
}
