import { useQuery, useMutation, useQueryClient, type UseQueryResult, type UseMutationResult } from "@tanstack/react-query";
import {
  apiClient,
  type SiteSurveyResponse,
  type SiteSurveyRevisionResponse,
  type CreateSiteSurveyRequest,
  type UpdateSurveyDraftRequest,
  type MarkSurveyReadyRequest,
  type CloneSurveyRevisionRequest,
  type VoidSurveyRevisionRequest,
  type SurveyTemplateVersionListResponse,
  type SurveyTemplateVersionResponse,
  type PublishSurveyTemplateVersionRequest,
  type SurveyReferenceDataResponse,
  type SurveyDesignStyleResponse,
  type CreateSurveyDesignStyleRequest,
  type UpdateSurveyDesignStyleRequest,
  type SurveyMoodToneResponse,
  type CreateSurveyMoodToneRequest,
  type UpdateSurveyMoodToneRequest,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError, ApiError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership, useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";
import { opportunityDetailQueryKey } from "@/features/opportunities/api/opportunity-queries";

export function opportunitySurveyQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  opportunityId: string | null | undefined
): readonly ["business", string | null | undefined, "th" | "en", "surveys", "opportunity", string | null | undefined] {
  return ["business", membershipId, locale, "surveys", "opportunity", opportunityId] as const;
}

export function surveyTemplateVersionsQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en"
): readonly ["business", string | null | undefined, "th" | "en", "surveys", "template-versions"] {
  return ["business", membershipId, locale, "surveys", "template-versions"] as const;
}

export function surveyReferenceDataQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en"
): readonly ["business", string | null | undefined, "th" | "en", "surveys", "reference-data"] {
  return ["business", membershipId, locale, "surveys", "reference-data"] as const;
}

export function useSurveyReferenceData(): UseQueryResult<SurveyReferenceDataResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const membershipContext = useOptionalSelectedMembership();
  const membershipId = membershipContext?.selectedMembership?.id;

  return useQuery({
    queryKey: surveyReferenceDataQueryKey(membershipId, normalizedLocale),
    queryFn: async () => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.getSurveyReferenceData({ token, membershipId, locale: normalizedLocale });
    },
    enabled: Boolean(membershipId),
    staleTime: 60 * 60 * 1000,
  });
}

export function useSurveyTemplateVersions(): UseQueryResult<SurveyTemplateVersionListResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: surveyTemplateVersionsQueryKey(membershipId, normalizedLocale),
    queryFn: async () => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.listSurveyTemplateVersions({ token, membershipId, locale: normalizedLocale });
    },
    enabled: Boolean(membershipId),
    // System-owned versions are immutable, so they never need to refetch during a session.
    staleTime: Infinity,
  });
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

export function useCloneSurveyRevision(
  opportunityId: string,
  surveyId: string
): UseMutationResult<
  SiteSurveyRevisionResponse,
  Error,
  {
    payload: CloneSurveyRevisionRequest;
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

      return apiClient.cloneSurveyRevision(opportunityId, surveyId, payload, {
        token,
        membershipId,
        idempotencyKey,
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

export function useVoidSurveyRevision(
  opportunityId: string,
  surveyId: string,
  revisionId: string
): UseMutationResult<
  SiteSurveyRevisionResponse,
  Error,
  {
    payload: VoidSurveyRevisionRequest;
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

      return apiClient.voidSurveyRevision(opportunityId, surveyId, revisionId, payload, {
        token,
        membershipId,
        idempotencyKey,
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

export function surveyDesignStylesQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  onlyActive?: boolean
): readonly ["business", string | null | undefined, "th" | "en", "surveys", "design-styles", boolean | undefined] {
  return ["business", membershipId, locale, "surveys", "design-styles", onlyActive] as const;
}

export function surveyMoodTonesQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  onlyActive?: boolean
): readonly ["business", string | null | undefined, "th" | "en", "surveys", "mood-tones", boolean | undefined] {
  return ["business", membershipId, locale, "surveys", "mood-tones", onlyActive] as const;
}

export function useSurveyDesignStyles(
  onlyActive?: boolean
): UseQueryResult<SurveyDesignStyleResponse[], Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const membershipContext = useOptionalSelectedMembership();
  const membershipId = membershipContext?.selectedMembership?.id;

  return useQuery({
    queryKey: surveyDesignStylesQueryKey(membershipId, normalizedLocale, onlyActive),
    queryFn: async () => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.listSurveyDesignStyles(
        { token, membershipId, locale: normalizedLocale },
        onlyActive
      );
    },
    enabled: Boolean(membershipId),
  });
}

export function useCreateSurveyDesignStyle(): UseMutationResult<
  SurveyDesignStyleResponse,
  Error,
  CreateSurveyDesignStyleRequest
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload: CreateSurveyDesignStyleRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.createSurveyDesignStyle(payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "surveys", "design-styles"],
      });
      queryClient.invalidateQueries({
        queryKey: surveyReferenceDataQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

export function useUpdateSurveyDesignStyle(): UseMutationResult<
  SurveyDesignStyleResponse,
  Error,
  { id: string; payload: UpdateSurveyDesignStyleRequest }
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, payload }: { id: string; payload: UpdateSurveyDesignStyleRequest }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.updateSurveyDesignStyle(id, payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "surveys", "design-styles"],
      });
      queryClient.invalidateQueries({
        queryKey: surveyReferenceDataQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

export function useSurveyMoodTones(
  onlyActive?: boolean
): UseQueryResult<SurveyMoodToneResponse[], Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const membershipContext = useOptionalSelectedMembership();
  const membershipId = membershipContext?.selectedMembership?.id;

  return useQuery({
    queryKey: surveyMoodTonesQueryKey(membershipId, normalizedLocale, onlyActive),
    queryFn: async () => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.listSurveyMoodTones(
        { token, membershipId, locale: normalizedLocale },
        onlyActive
      );
    },
    enabled: Boolean(membershipId),
  });
}

export function useCreateSurveyMoodTone(): UseMutationResult<
  SurveyMoodToneResponse,
  Error,
  CreateSurveyMoodToneRequest
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload: CreateSurveyMoodToneRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.createSurveyMoodTone(payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "surveys", "mood-tones"],
      });
      queryClient.invalidateQueries({
        queryKey: surveyReferenceDataQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

export function useUpdateSurveyMoodTone(): UseMutationResult<
  SurveyMoodToneResponse,
  Error,
  { id: string; payload: UpdateSurveyMoodToneRequest }
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, payload }: { id: string; payload: UpdateSurveyMoodToneRequest }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.updateSurveyMoodTone(id, payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "surveys", "mood-tones"],
      });
      queryClient.invalidateQueries({
        queryKey: surveyReferenceDataQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

export function usePublishSurveyTemplateVersion(): UseMutationResult<
  SurveyTemplateVersionResponse,
  Error,
  PublishSurveyTemplateVersionRequest
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload: PublishSurveyTemplateVersionRequest) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.publishSurveyTemplateVersion(payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: surveyTemplateVersionsQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

export function useRetireSurveyTemplateVersion(): UseMutationResult<
  SurveyTemplateVersionResponse,
  Error,
  string
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (code: string) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.retireSurveyTemplateVersion(code, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: surveyTemplateVersionsQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

