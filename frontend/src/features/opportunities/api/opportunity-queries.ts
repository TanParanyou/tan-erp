import { useQuery, useInfiniteQuery, useMutation, useQueryClient, type UseQueryResult, type UseInfiniteQueryResult, type UseMutationResult } from "@tanstack/react-query";
import {
  apiClient,
  type OpportunityListResponse,
  type OpportunityResponse,
  type ListOpportunitiesParams,
  type UpdateDraftQGateRequest,
  type UpdateOpenOpportunityRequest,
  type ReassignOpportunityOwnerRequest,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function opportunityListQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  params?: ListOpportunitiesParams
): readonly ["business", string | null | undefined, "th" | "en", "opportunities", "list", string | null, string | null, string | null, number] {
  return [
    "business",
    membershipId,
    locale,
    "opportunities",
    "list",
    params?.search ?? null,
    params?.customerId ?? null,
    params?.stage ?? null,
    params?.limit ?? 25,
  ] as const;
}

export function opportunityDetailQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  opportunityId: string | null | undefined
): readonly ["business", string | null | undefined, "th" | "en", "opportunities", "detail", string | null | undefined] {
  return ["business", membershipId, locale, "opportunities", "detail", opportunityId] as const;
}

export function useOpportunityList(
  params?: ListOpportunitiesParams
): UseInfiniteQueryResult<{ pages: OpportunityListResponse[]; pageParams: (string | undefined)[] }, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useInfiniteQuery({
    queryKey: opportunityListQueryKey(membershipId, normalizedLocale, params),
    initialPageParam: undefined as string | undefined,
    queryFn: async ({ pageParam, signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.listOpportunities(
        {
          token,
          membershipId,
          locale: normalizedLocale,
          signal,
        },
        {
          ...params,
          cursor: pageParam,
        }
      );
    },
    getNextPageParam: (lastPage) => lastPage.nextCursor || undefined,
    enabled: Boolean(membershipId),
  });
}

export function useOpportunityDetail(
  opportunityId: string | null | undefined
): UseQueryResult<OpportunityResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: opportunityDetailQueryKey(membershipId, normalizedLocale, opportunityId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }
      if (!opportunityId) {
        throw new Error("No opportunity ID provided");
      }

      return apiClient.getOpportunity(opportunityId, {
        token,
        membershipId,
        locale: normalizedLocale,
        signal,
      });
    },
    enabled: Boolean(membershipId && opportunityId && opportunityId !== "create" && opportunityId !== "add"),
  });
}

export interface TransitionOpportunityStageVariables {
  opportunityId: string;
  targetStage?: string;
  expectedVersion: string;
  reasonCode?: string;
  note?: string;
  idempotencyKey?: string;
}

export type QualifyOpportunityVariables = TransitionOpportunityStageVariables;

export function useTransitionOpportunityStage(): UseMutationResult<
  OpportunityResponse,
  Error,
  TransitionOpportunityStageVariables
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      opportunityId,
      targetStage = "qualified",
      expectedVersion,
      reasonCode,
      note,
      idempotencyKey,
    }: TransitionOpportunityStageVariables) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.transitionOpportunityStage(
        opportunityId,
        {
          targetStage,
          expectedVersion,
          reasonCode,
          note,
        },
        {
          token,
          membershipId,
          locale: normalizedLocale,
          idempotencyKey,
        }
      );
    },
    onSuccess: (updatedOpportunity, variables) => {
      queryClient.setQueryData(
        opportunityDetailQueryKey(membershipId, normalizedLocale, variables.opportunityId),
        updatedOpportunity
      );

      // Invalidate stage history
      void queryClient.invalidateQueries({
        queryKey: opportunityStageHistoryQueryKey(membershipId, variables.opportunityId),
      });

      void queryClient.invalidateQueries({
        predicate: (query) => {
          const key = query.queryKey;
          return (
            Array.isArray(key) &&
            key[0] === "business" &&
            key[1] === membershipId &&
            key[3] === "opportunities" &&
            key[4] === "list"
          );
        },
      });
    },
  });
}

export function useQualifyOpportunity(): UseMutationResult<
  OpportunityResponse,
  Error,
  QualifyOpportunityVariables
> {
  return useTransitionOpportunityStage();
}

export function opportunityStageHistoryQueryKey(
  membershipId: string | null | undefined,
  opportunityId: string | null | undefined
): readonly ["business", string | null | undefined, "opportunities", "stage-history", string | null | undefined] {
  return ["business", membershipId, "opportunities", "stage-history", opportunityId] as const;
}

export function useOpportunityStageHistory(
  opportunityId: string | null | undefined
): UseQueryResult<import("@/lib/api/api-client").OpportunityStageHistoryListResponse, Error> {
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: opportunityStageHistoryQueryKey(membershipId, opportunityId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }
      if (!opportunityId) {
        throw new Error("No opportunity ID provided");
      }

      return apiClient.getOpportunityStageHistory(opportunityId, {
        token,
        membershipId,
        signal,
      });
    },
    enabled: Boolean(membershipId && opportunityId && opportunityId !== "create" && opportunityId !== "add"),
  });
}

export interface UpdateDraftQGateVariables {
  opportunityId: string;
  expectedVersion: string;
  payload: UpdateDraftQGateRequest;
  idempotencyKey?: string;
}

export function useUpdateDraftQGate(): UseMutationResult<
  OpportunityResponse,
  Error,
  UpdateDraftQGateVariables
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ opportunityId, expectedVersion, payload, idempotencyKey }: UpdateDraftQGateVariables) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.updateDraftQGate(
        opportunityId,
        payload,
        {
          token,
          membershipId,
          locale: normalizedLocale,
          idempotencyKey,
          ifMatch: `"${expectedVersion}"`,
        }
      );
    },
    onSuccess: (updatedOpportunity, variables) => {
      queryClient.setQueryData(
        opportunityDetailQueryKey(membershipId, normalizedLocale, variables.opportunityId),
        updatedOpportunity
      );

      void queryClient.invalidateQueries({
        predicate: (query) => {
          const key = query.queryKey;
          return (
            Array.isArray(key) &&
            key[0] === "business" &&
            key[1] === membershipId &&
            key[3] === "opportunities" &&
            key[4] === "list"
          );
        },
      });
    },
  });
}

export interface UpdateOpenOpportunityVariables {
  opportunityId: string;
  expectedVersion: string;
  payload: UpdateOpenOpportunityRequest;
  idempotencyKey?: string;
}

export function useUpdateOpenOpportunity(): UseMutationResult<
  OpportunityResponse,
  Error,
  UpdateOpenOpportunityVariables
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      opportunityId,
      expectedVersion,
      payload,
      idempotencyKey,
    }: UpdateOpenOpportunityVariables) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.updateOpenOpportunity(
        opportunityId,
        payload,
        {
          token,
          membershipId,
          locale: normalizedLocale,
          idempotencyKey,
          ifMatch: `"${expectedVersion}"`,
        }
      );
    },
    onSuccess: (updatedOpportunity, variables) => {
      queryClient.setQueryData(
        opportunityDetailQueryKey(membershipId, normalizedLocale, variables.opportunityId),
        updatedOpportunity
      );

      void queryClient.invalidateQueries({
        predicate: (query) => {
          const key = query.queryKey;
          return (
            Array.isArray(key) &&
            key[0] === "business" &&
            key[1] === membershipId &&
            key[3] === "opportunities" &&
            key[4] === "list"
          );
        },
      });
    },
  });
}

export interface ReassignOpportunityOwnerVariables {
  opportunityId: string;
  expectedVersion: string;
  targetOwnerUserId: string;
  idempotencyKey?: string;
}

export function useReassignOpportunityOwner(): UseMutationResult<
  OpportunityResponse,
  Error,
  ReassignOpportunityOwnerVariables
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      opportunityId,
      expectedVersion,
      targetOwnerUserId,
      idempotencyKey,
    }: ReassignOpportunityOwnerVariables) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.reassignOpportunityOwner(
        opportunityId,
        {
          targetOwnerUserId,
          expectedVersion,
        },
        {
          token,
          membershipId,
          locale: normalizedLocale,
          idempotencyKey,
          ifMatch: `"${expectedVersion}"`,
        }
      );
    },
    onSuccess: (updatedOpportunity, variables) => {
      queryClient.setQueryData(
        opportunityDetailQueryKey(membershipId, normalizedLocale, variables.opportunityId),
        updatedOpportunity
      );

      void queryClient.invalidateQueries({
        predicate: (query) => {
          const key = query.queryKey;
          return (
            Array.isArray(key) &&
            key[0] === "business" &&
            key[1] === membershipId &&
            key[3] === "opportunities" &&
            key[4] === "list"
          );
        },
      });
    },
  });
}

