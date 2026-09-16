import { useQuery, useMutation, useQueryClient, type UseQueryResult, type UseMutationResult } from "@tanstack/react-query";
import {
  apiClient,
  type SiteSurveyResponse,
  type SiteSurveyRevisionResponse,
  type CreateSiteSurveyRequest,
  type UpdateSurveyDraftRequest,
  type MarkSurveyReadyRequest,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError, ApiError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { opportunityDetailQueryKey } from "@/features/opportunities/api/opportunity-queries";

export function opportunitySurveyQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  opportunityId: string | null | undefined
): readonly ["business", string | null | undefined, "th" | "en", "surveys", "opportunity", string | null | undefined] {
  return ["business", membershipId, locale, "surveys", "opportunity", opportunityId] as const;
}

export function useOpportunitySurvey(
  opportunityId: string | null | undefined
): UseQueryResult<SiteSurveyResponse | null, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: opportunitySurveyQueryKey(membershipId, normalizedLocale, opportunityId),
    queryFn: async ({ signal }) => {
      if (!opportunityId) return null;
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      try {
        const res = await apiClient.getOpportunitySurvey(opportunityId, {
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

export function useCreateSiteSurvey(
  opportunityId: string
): UseMutationResult<
  SiteSurveyResponse,
  Error,
  {
    payload: CreateSiteSurveyRequest;
    idempotencyKey: string;
  }
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.createSiteSurvey(opportunityId, payload, {
        token,
        membershipId,
        idempotencyKey,
        locale: normalizedLocale,
      });
    },
    onSuccess: (survey) => {
      // Invalidate both the opportunity detail (to reflect 'surveying' stage) and the opportunity survey query
      queryClient.invalidateQueries({
        queryKey: opportunityDetailQueryKey(membershipId, normalizedLocale, opportunityId),
      });
      queryClient.setQueryData(
        opportunitySurveyQueryKey(membershipId, normalizedLocale, opportunityId),
        survey
      );
    },
  });
}

export function useUpdateSurveyDraft(
  opportunityId: string,
  surveyId: string,
  revisionId: string
): UseMutationResult<
  SiteSurveyRevisionResponse,
  Error,
  {
    payload: UpdateSurveyDraftRequest;
    ifMatch: string;
  }
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ payload, ifMatch }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.updateSurveyDraft(opportunityId, surveyId, revisionId, payload, {
        token,
        membershipId,
        ifMatch,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: opportunitySurveyQueryKey(membershipId, normalizedLocale, opportunityId),
      });
    },
  });
}

export function useMarkSurveyReady(
  opportunityId: string,
  surveyId: string,
  revisionId: string
): UseMutationResult<
  SiteSurveyRevisionResponse,
  Error,
  {
    payload: MarkSurveyReadyRequest;
    idempotencyKey: string;
  }
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.markSurveyReady(opportunityId, surveyId, revisionId, payload, {
        token,
        membershipId,
        idempotencyKey,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: opportunityDetailQueryKey(membershipId, normalizedLocale, opportunityId),
      });
      queryClient.invalidateQueries({
        queryKey: opportunitySurveyQueryKey(membershipId, normalizedLocale, opportunityId),
      });
    },
  });
}

