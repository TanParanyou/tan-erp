import { useQuery, useInfiniteQuery, type UseQueryResult, type UseInfiniteQueryResult } from "@tanstack/react-query";
import { apiClient, type CustomerListResponse, type CustomerResponse, type DuplicateCustomerResponse, type ListCustomersParams } from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function customerListQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  params?: ListCustomersParams,
): readonly [
  "business",
  string | null | undefined,
  "th" | "en",
  "customers",
  "list",
  string | null,
  string | null,
  string | null,
  string | null,
  string | null,
  number,
  number,
] {
  return [
    "business",
    membershipId,
    locale,
    "customers",
    "list",
    params?.search ?? null,
    params?.status ?? null,
    params?.customerType ?? null,
    params?.sortBy ?? null,
    params?.sortOrder ?? null,
    params?.page ?? 1,
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
): UseQueryResult<CustomerListResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: customerListQueryKey(membershipId, normalizedLocale, params),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.listCustomers(
        {
          token,
          membershipId,
          locale: normalizedLocale,
          signal,
        },
        params
      );
    },
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
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
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

export interface CheckDuplicatesParams {
  name?: string | null;
  phone?: string | null;
  email?: string | null;
}

export function customerDuplicateCheckQueryKey(
  membershipId: string | null | undefined,
  params: CheckDuplicatesParams,
): readonly [
  "business",
  string | null | undefined,
  "customers",
  "check-duplicates",
  string | null,
  string | null,
  string | null,
] {
  return [
    "business",
    membershipId,
    "customers",
    "check-duplicates",
    params.name?.trim() || null,
    params.phone?.trim() || null,
    params.email?.trim() || null,
  ] as const;
}

export function useCustomerDuplicateCheck(
  params: CheckDuplicatesParams,
  enabled = true,
): UseQueryResult<DuplicateCustomerResponse[], Error> {
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";

  const trimmedName = params.name?.trim() || "";
  const trimmedPhone = params.phone?.trim() || "";
  const trimmedEmail = params.email?.trim() || "";
  const hasInput = trimmedName.length >= 2 || trimmedPhone.length >= 3 || trimmedEmail.length >= 3;

  return useQuery({
    queryKey: customerDuplicateCheckQueryKey(membershipId, params),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new AuthenticationRequiredError();
      }
      if (!membershipId) {
        throw new MembershipRequiredError();
      }

      return apiClient.checkCustomerDuplicates(
        {
          name: trimmedName || null,
          phone: trimmedPhone || null,
          email: trimmedEmail || null,
        },
        {
          token,
          membershipId,
          locale: normalizedLocale,
          signal,
        },
      );
    },
    enabled: Boolean(enabled && membershipId && hasInput),
    staleTime: 10_000,
  });
}

