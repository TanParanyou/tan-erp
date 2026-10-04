import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type EffectiveTemplateResponse,
  type ListPricingTemplatesParams,
  type ListQuickEstimatesParams,
  type PricingTemplateListResponse,
  type PricingTemplateRequest,
  type PricingTemplateResponse,
  type QuickEstimateDraftRequest,
  type QuickEstimateListResponse,
  type QuickEstimateResponse,
  type TemplateStepName,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function quickEstimateKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "quick-estimates"] as const;
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

export function usePricingTemplateList(params: ListPricingTemplatesParams): UseQueryResult<PricingTemplateListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...quickEstimateKey(membershipId, locale), "templates", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listPricingTemplates(await options(undefined, signal), params),
  });
}

export function usePricingTemplate(id: string | undefined): UseQueryResult<PricingTemplateResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...quickEstimateKey(membershipId, locale), "templates", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getPricingTemplate(id ?? "", await options(undefined, signal)),
  });
}

export function useEffectiveTemplates(): UseQueryResult<EffectiveTemplateResponse[], Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...quickEstimateKey(membershipId, locale), "templates", "effective"],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listEffectivePricingTemplates(await options(undefined, signal)),
  });
}

export function useQuickEstimateList(params: ListQuickEstimatesParams): UseQueryResult<QuickEstimateListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...quickEstimateKey(membershipId, locale), "estimates", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listQuickEstimates(await options(undefined, signal), params),
  });
}

export function useQuickEstimate(id: string | undefined): UseQueryResult<QuickEstimateResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...quickEstimateKey(membershipId, locale), "estimates", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getQuickEstimate(id ?? "", await options(undefined, signal)),
  });
}

/** Template writes. Each call returns the fresh template, which is cached so the page shows the new row version immediately. */
export function usePricingTemplateMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (template: PricingTemplateResponse) => {
    queryClient.setQueryData([...quickEstimateKey(membershipId, locale), "templates", "detail", template.id], template);
    await queryClient.invalidateQueries({ queryKey: [...quickEstimateKey(membershipId, locale), "templates", "list"] });
    await queryClient.invalidateQueries({ queryKey: [...quickEstimateKey(membershipId, locale), "templates", "effective"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: PricingTemplateRequest; idempotencyKey: string }) =>
      apiClient.createPricingTemplate(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const update = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: PricingTemplateRequest }) =>
      apiClient.updatePricingTemplate(input.id, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const step = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; step: TemplateStepName }) =>
      apiClient.pricingTemplateStep(input.id, input.rowVersion, input.step, await options()),
    onSuccess: store,
  });
  const decide = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; decision: "approved" | "returned"; note: string | null }) =>
      apiClient.decidePricingTemplate(input.id, input.rowVersion, input.decision, input.note, await options()),
    onSuccess: store,
  });
  const newVersion = useMutation({
    mutationFn: async (id: string) => apiClient.newPricingTemplateVersion(id, await options()),
    onSuccess: store,
  });
  return { create, update, step, decide, newVersion };
}

/** Quick estimate writes. Each call returns the fresh estimate, which is cached so the page shows the new row version immediately. */
export function useQuickEstimateMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (estimate: QuickEstimateResponse) => {
    queryClient.setQueryData([...quickEstimateKey(membershipId, locale), "estimates", "detail", estimate.id], estimate);
    await queryClient.invalidateQueries({ queryKey: [...quickEstimateKey(membershipId, locale), "estimates", "list"] });
  };
  const create = useMutation({
    mutationFn: async (idempotencyKey: string) => apiClient.createQuickEstimate(await options(idempotencyKey)),
    onSuccess: store,
  });
  const saveDraft = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; payload: QuickEstimateDraftRequest }) =>
      apiClient.patchQuickEstimateDraft(input.id, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const calculate = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string }) => apiClient.calculateQuickEstimate(input.id, input.rowVersion, await options()),
    onSuccess: store,
  });
  const submitReview = useMutation({
    mutationFn: async (input: { id: string; sourceVersion: number; idempotencyKey: string }) =>
      apiClient.submitQuickEstimateReview(input.id, input.sourceVersion, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const decideReview = useMutation({
    mutationFn: async (input: { id: string; sourceVersion: number; decision: "approved" | "returned"; reasonCode: string; idempotencyKey: string }) =>
      apiClient.decideQuickEstimateReview(input.id, input.sourceVersion, input.decision, input.reasonCode, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const share = useMutation({
    mutationFn: async (input: { id: string; sourceVersion: number; idempotencyKey: string }) =>
      apiClient.shareQuickEstimate(input.id, input.sourceVersion, locale, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const convert = useMutation({
    mutationFn: async (input: { id: string; sourceVersion: number; siteSurveyRevisionId: string; idempotencyKey: string }) =>
      apiClient.convertQuickEstimate(input.id, input.sourceVersion, input.siteSurveyRevisionId, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  return { create, saveDraft, calculate, submitReview, decideReview, share, convert };
}
