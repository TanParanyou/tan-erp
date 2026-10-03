import { useQuery, useMutation, useQueryClient, type UseQueryResult, type UseMutationResult } from "@tanstack/react-query";
import {
  apiClient,
  type EstimateDetailResponse,
  type EstimateRevisionResponse,
  type CreateEstimateDraftRequest,
  type UpdateEstimateDraftRequest,
  type CalculateEstimateRequest,
  type SubmitEstimateRequest,
  type ReviewEstimateRequest,
  type CreateEstimateRevisionRequest,
  type CancelEstimateRequest,
  type IssueQuotationRequest,
  type QuotationResponse,
  type AcceptQuotationRequest,
  type AcceptQuotationResponse,
  type QuotationDocumentResponse,
  type EstimateReviewQueueParams,
  type EstimateReviewQueueResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError, ApiError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import {
  opportunityDetailQueryKey,
  opportunityStageHistoryQueryKey,
} from "@/features/opportunities/api/opportunity-queries";

export function opportunityEstimateQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  opportunityId: string | null | undefined
): readonly ["business", string | null | undefined, "th" | "en", "estimates", "opportunity", string | null | undefined] {
  return ["business", membershipId, locale, "estimates", "opportunity", opportunityId] as const;
}

export function estimateDetailQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  estimateId: string | null | undefined
): readonly ["business", string | null | undefined, "th" | "en", "estimates", "detail", string | null | undefined] {
  return ["business", membershipId, locale, "estimates", "detail", estimateId] as const;
}

export function quotationDocumentQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  estimateId: string | null | undefined,
  documentLocale: "th" | "en"
): readonly ["business", string | null | undefined, "th" | "en", "estimates", "quotation-document", string | null | undefined, "th" | "en"] {
  return ["business", membershipId, locale, "estimates", "quotation-document", estimateId, documentLocale] as const;
}

export const estimateReviewQueueQueryKey = (
  membershipId: string | null | undefined,
  locale: "th" | "en",
  params: EstimateReviewQueueParams
) => ["business", membershipId, locale, "estimates", "review-queue", params] as const;

export function useEstimateReviewQueue(params: EstimateReviewQueueParams): UseQueryResult<EstimateReviewQueueResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: estimateReviewQueueQueryKey(membershipId, normalizedLocale, params),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.listEstimateReviewQueue(params, { token, membershipId, locale: normalizedLocale, signal });
    },
    staleTime: 30 * 1000,
  });
}

export function useOpportunityEstimate(
  opportunityId: string | null | undefined
): UseQueryResult<EstimateDetailResponse | null, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
    queryFn: async ({ signal }) => {
      if (!opportunityId) return null;
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      try {
        const res = await apiClient.getOpportunityEstimate(opportunityId, {
          token,
          membershipId,
          locale: normalizedLocale,
          signal,
        });
        return res ?? null;
      } catch (err: unknown) {
        if (err instanceof ApiError && (err.status === 404 || err.status === 204)) {
          return null;
        }
        if (err && typeof err === "object" && "status" in err && ((err as { status: number }).status === 404 || (err as { status: number }).status === 204)) {
          return null;
        }
        throw err;
      }
    },
    enabled: Boolean(membershipId && opportunityId),
    staleTime: 60 * 1000,
  });
}

export function useCreateEstimate(
  opportunityId: string
): UseMutationResult<EstimateDetailResponse, Error, CreateEstimateDraftRequest> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async (payload: CreateEstimateDraftRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.createEstimate(payload, {
        token,
        membershipId,
        idempotencyKey: crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: (createdEstimate) => {
      queryClient.setQueryData(
        opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
        createdEstimate
      );
      void queryClient.invalidateQueries({
        queryKey: opportunityDetailQueryKey(membershipId, normalizedLocale, opportunityId),
      });
    },
  });
}

/** Publish mutation responses to both existing estimate views before background refetches. */
function updateRevisionCache(
  queryClient: ReturnType<typeof useQueryClient>,
  membershipId: string | null | undefined,
  locale: "th" | "en",
  opportunityId: string,
  estimateId: string,
  revisionId: string,
  revision: EstimateRevisionResponse,
) {
  const apply = (current: EstimateDetailResponse | null | undefined) =>
    current?.id === estimateId && current.currentRevision?.id === revisionId
      ? { ...current, currentRevision: revision }
      : current;
  queryClient.setQueryData<EstimateDetailResponse | null>(opportunityEstimateQueryKey(membershipId, locale, opportunityId), apply);
  queryClient.setQueryData<EstimateDetailResponse | null>(estimateDetailQueryKey(membershipId, locale, estimateId), apply);
}

export function useUpdateEstimateDraft(
  opportunityId: string,
  estimateId: string,
  revisionId: string
): UseMutationResult<EstimateRevisionResponse, Error, { payload: UpdateEstimateDraftRequest; ifMatch: string }> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, ifMatch }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.updateEstimateDraft(estimateId, revisionId, payload, {
        token,
        membershipId,
        ifMatch,
        locale: normalizedLocale,
      });
    },
    onSuccess: (revision) => {
      updateRevisionCache(queryClient, membershipId, normalizedLocale, opportunityId, estimateId, revisionId, revision);
      void queryClient.invalidateQueries({ queryKey: estimateDetailQueryKey(membershipId, normalizedLocale, estimateId) });
      void queryClient.invalidateQueries({
        queryKey: opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
      });
    },
  });
}

export function useCalculateEstimate(
  opportunityId: string,
  estimateId: string,
  revisionId: string
): UseMutationResult<EstimateRevisionResponse, Error, CalculateEstimateRequest> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async (payload: CalculateEstimateRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.calculateEstimate(estimateId, revisionId, payload, {
        token,
        membershipId,
        idempotencyKey: crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: (revision) => {
      updateRevisionCache(queryClient, membershipId, normalizedLocale, opportunityId, estimateId, revisionId, revision);
      void queryClient.invalidateQueries({ queryKey: estimateDetailQueryKey(membershipId, normalizedLocale, estimateId) });
      void queryClient.invalidateQueries({
        queryKey: opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
      });
    },
  });
}

type ConditionalEstimateMutation<TRequest> = {
  payload: TRequest;
  ifMatch: string;
  idempotencyKey?: string;
};

function invalidateEstimateAndOpportunity(
  queryClient: ReturnType<typeof useQueryClient>,
  membershipId: string | null | undefined,
  locale: "th" | "en",
  opportunityId: string
): void {
  void queryClient.invalidateQueries({
    queryKey: opportunityEstimateQueryKey(membershipId, locale, opportunityId),
  });
  void queryClient.invalidateQueries({
    queryKey: opportunityDetailQueryKey(membershipId, locale, opportunityId),
  });
  void queryClient.invalidateQueries({
    queryKey: opportunityStageHistoryQueryKey(membershipId, opportunityId),
  });
}

export function useSubmitEstimate(
  opportunityId: string,
  estimateId: string
): UseMutationResult<EstimateDetailResponse, Error, ConditionalEstimateMutation<SubmitEstimateRequest>> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, ifMatch, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.submitEstimate(estimateId, payload, {
        token,
        membershipId,
        ifMatch,
        idempotencyKey: idempotencyKey ?? crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: () => invalidateEstimateAndOpportunity(queryClient, membershipId, normalizedLocale, opportunityId),
  });
}

export function useReviewEstimate(
  opportunityId: string,
  estimateId: string
): UseMutationResult<EstimateDetailResponse, Error, ConditionalEstimateMutation<ReviewEstimateRequest>> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, ifMatch, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.reviewEstimate(estimateId, payload, {
        token,
        membershipId,
        ifMatch,
        idempotencyKey: idempotencyKey ?? crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: () => invalidateEstimateAndOpportunity(queryClient, membershipId, normalizedLocale, opportunityId),
  });
}

export function useCreateEstimateRevision(
  opportunityId: string,
  estimateId: string
): UseMutationResult<EstimateDetailResponse, Error, ConditionalEstimateMutation<CreateEstimateRevisionRequest>> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, ifMatch, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.createEstimateRevision(estimateId, payload, {
        token,
        membershipId,
        ifMatch,
        idempotencyKey: idempotencyKey ?? crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: () => invalidateEstimateAndOpportunity(queryClient, membershipId, normalizedLocale, opportunityId),
  });
}

export function useCancelEstimate(
  opportunityId: string,
  estimateId: string
): UseMutationResult<EstimateDetailResponse, Error, ConditionalEstimateMutation<CancelEstimateRequest>> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, ifMatch, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.cancelEstimate(estimateId, payload, {
        token,
        membershipId,
        ifMatch,
        idempotencyKey: idempotencyKey ?? crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: () => invalidateEstimateAndOpportunity(queryClient, membershipId, normalizedLocale, opportunityId),
  });
}

export function useIssueQuotation(
  opportunityId: string,
  estimateId: string
): UseMutationResult<QuotationResponse, Error, { payload: IssueQuotationRequest; idempotencyKey?: string }> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.issueQuotation(estimateId, payload, {
        token,
        membershipId,
        idempotencyKey: idempotencyKey ?? crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({
        queryKey: opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
      });
      void queryClient.invalidateQueries({
        queryKey: opportunityDetailQueryKey(membershipId, normalizedLocale, opportunityId),
      });
      void queryClient.invalidateQueries({
        queryKey: opportunityStageHistoryQueryKey(membershipId, opportunityId),
      });
    },
  });
}

export function useAcceptQuotation(
  opportunityId: string,
  estimateId: string
): UseMutationResult<AcceptQuotationResponse, Error, { payload: AcceptQuotationRequest; idempotencyKey?: string }> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.acceptQuotation(estimateId, payload, {
        token,
        membershipId,
        idempotencyKey: idempotencyKey ?? crypto.randomUUID(),
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({
        queryKey: opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
      });
      void queryClient.invalidateQueries({
        queryKey: opportunityDetailQueryKey(membershipId, normalizedLocale, opportunityId),
      });
      void queryClient.invalidateQueries({
        queryKey: opportunityStageHistoryQueryKey(membershipId, opportunityId),
      });
    },
  });
}

export function useQuotationDocument(
  estimateId: string,
  documentLocale: "th" | "en"
): UseQueryResult<QuotationDocumentResponse, Error> {
  const uiLocale = useSafeLocale();
  const normalizedUiLocale = uiLocale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: quotationDocumentQueryKey(membershipId, normalizedUiLocale, estimateId, documentLocale),
    enabled: Boolean(membershipId && estimateId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return apiClient.getQuotationDocument(estimateId, documentLocale, {
        token,
        membershipId,
        locale: documentLocale,
        signal,
      });
    },
  });
}
