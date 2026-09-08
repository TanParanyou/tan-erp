import { useQuery, useInfiniteQuery, type UseQueryResult, type UseInfiniteQueryResult } from "@tanstack/react-query";
import { apiClient, type CustomerListResponse, type CustomerResponse, type ListCustomersParams } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function customerListQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  params?: ListCustomersParams,
): readonly ["business", string | null | undefined, "th" | "en", "customers", "list", string | null, string | null, number] {
  return [
    "business",
    membershipId,
    locale,
    "customers",
    "list",
    params?.search ?? null,
    params?.status ?? null,
    params?.limit ?? 25,
  ] as const;
}

export function customerDetailQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  customerId: string | null | undefined,
): readonly ["business", string | null | undefined, "th" | "en", "customers", "detail", string | null | undefined] {
  return ["business", membershipId, locale, "customers", "detail", customerId] as const;
}

export function useCustomerList(
  params?: ListCustomersParams
): UseInfiniteQueryResult<{ pages: CustomerListResponse[]; pageParams: (string | undefined)[] }, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useInfiniteQuery({
    queryKey: customerListQueryKey(membershipId, normalizedLocale, params),
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

export function useCustomerDetail(
  customerId: string | null | undefined
): UseQueryResult<CustomerResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: customerDetailQueryKey(membershipId, normalizedLocale, customerId),
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
        locale: normalizedLocale,
        signal,
      });
    },
    enabled: Boolean(membershipId && customerId && customerId !== "create" && customerId !== "add"),
  });
}
