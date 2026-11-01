import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type CalculationPolicyListResponse,
  type CalculationPolicyVersionResponse,
  type CreateCalculationPolicyRequest,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

function useRequestContext() {
  const locale: "th" | "en" = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (signal?: AbortSignal) => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, signal };
  };
  return { locale, membershipId, options };
}

export function useCalculationPolicies(): UseQueryResult<CalculationPolicyListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: ["business", membershipId, locale, "calculation-policies", "list"],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listCalculationPolicies(await options(signal)),
  });
}

export function useCalculationPolicyMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "calculation-policies"] });
  };
  const create = useMutation({
    mutationFn: async (payload: CreateCalculationPolicyRequest): Promise<CalculationPolicyVersionResponse> =>
      apiClient.createCalculationPolicy(payload, await options()),
    onSuccess: refresh,
  });
  const publish = useMutation({
    mutationFn: async (id: string): Promise<CalculationPolicyVersionResponse> =>
      apiClient.publishCalculationPolicy(id, await options()),
    onSuccess: refresh,
  });
  return { create, publish };
}
