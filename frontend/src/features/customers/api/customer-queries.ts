import { useQuery, useInfiniteQuery, type UseQueryResult, type UseInfiniteQueryResult } from "@tanstack/react-query";
import { apiClient, type CustomerListResponse, type CustomerResponse, type ListCustomersParams } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function customerListQueryKey(membershipId: string | null | undefined, params?: ListCustomersParams) {
  return ["customers", "list", membershipId, params?.search, params?.status] as const;
}

export function customerDetailQueryKey(membershipId: string | null | undefined, customerId: string | null | undefined) {
  return ["customers", "detail", membershipId, customerId] as const;
}

export function useCustomerList(
  params?: ListCustomersParams
): UseInfiniteQueryResult<{ pages: CustomerListResponse[]; pageParams: (string | undefined)[] }, Error> {
  const locale = useSafeLocale();
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useInfiniteQuery({
    queryKey: customerListQueryKey(membershipId, params),
    initialPageParam: undefined as string | undefined,
    queryFn: async ({ pageParam, signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new Error("No authentication token available");
      }
      if (!membershipId) {
        throw new Error("No active membership selected");
      }

      return apiClient.listCustomers(
        {
          token,
          membershipId,
          locale: locale === "en" ? "en" : "th",
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

export function useCustomerDetail(
  customerId: string | null | undefined
): UseQueryResult<CustomerResponse, Error> {
  const locale = useSafeLocale();
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: customerDetailQueryKey(membershipId, customerId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new Error("No authentication token available");
      }
      if (!membershipId) {
        throw new Error("No active membership selected");
      }
      if (!customerId) {
        throw new Error("No customer ID provided");
      }

      return apiClient.getCustomer(customerId, {
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
        signal,
      });
    },
    enabled: Boolean(membershipId && customerId && customerId !== "create" && customerId !== "add"),
  });
}
