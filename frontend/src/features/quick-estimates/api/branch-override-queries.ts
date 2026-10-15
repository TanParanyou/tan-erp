import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type BranchRateOverrideListResponse, type BranchRateOverrideRequest } from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";

export function branchOverridesKey(membershipId: string | undefined, locale: string, templateId: string) {
  return ["business", membershipId, locale, "quick-estimates", "templates", "branch-overrides", templateId] as const;
}

export function useBranchOverrides(templateId: string): UseQueryResult<BranchRateOverrideListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: branchOverridesKey(membershipId, locale, templateId),
    enabled: Boolean(membershipId && templateId),
    queryFn: async ({ signal }) => apiClient.listBranchRateOverrides(templateId, await buildOptions({ signal })),
  });
}

/** Every write refreshes the template's override list. */
export function useBranchOverrideMutations(templateId: string) {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => { await queryClient.invalidateQueries({ queryKey: branchOverridesKey(membershipId, locale, templateId) }); };
  const request = useMutation({
    mutationFn: async (input: { payload: BranchRateOverrideRequest; idempotencyKey: string }) =>
      apiClient.requestBranchRateOverride(templateId, input.payload, await buildOptions({ idempotencyKey: input.idempotencyKey })),
    onSuccess: refresh,
  });
  const decide = useMutation({
    mutationFn: async (input: { overrideId: string; rowVersion: string; decision: "approved" | "returned"; note: string | null }) =>
      apiClient.decideBranchRateOverride(templateId, input.overrideId, input.rowVersion, input.decision, input.note, await buildOptions()),
    onSuccess: refresh,
  });
  const revoke = useMutation({
    mutationFn: async (input: { overrideId: string; rowVersion: string; reason: string }) =>
      apiClient.revokeBranchRateOverride(templateId, input.overrideId, input.rowVersion, input.reason, await buildOptions()),
    onSuccess: refresh,
  });
  return { request, decide, revoke };
}
