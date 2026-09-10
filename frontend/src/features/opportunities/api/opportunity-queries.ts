import { useQuery, useInfiniteQuery, type UseQueryResult, type UseInfiniteQueryResult } from "@tanstack/react-query";
import { apiClient, type OpportunityListResponse, type OpportunityResponse, type ListOpportunitiesParams } from "@/lib/api/api-client";
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
