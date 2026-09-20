import { useQuery, useMutation, useQueryClient, type UseQueryResult, type UseMutationResult } from "@tanstack/react-query";
import {
  apiClient,
  type EstimateDetailResponse,
  type EstimateRevisionResponse,
  type CreateEstimateDraftRequest,
  type UpdateEstimateDraftRequest,
  type CalculateEstimateRequest,
  type IssueQuotationRequest,
  type QuotationResponse,
  type AcceptQuotationRequest,
  type AcceptQuotationResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError, ApiError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { opportunityDetailQueryKey } from "@/features/opportunities/api/opportunity-queries";

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
    onSuccess: () => {
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
    onSuccess: () => {
      void queryClient.invalidateQueries({
        queryKey: opportunityEstimateQueryKey(membershipId, normalizedLocale, opportunityId),
      });
    },
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
    },
  });
}
