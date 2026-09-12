import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type UserListResponse,
  type ListUsersParams,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function userListQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  params?: ListUsersParams
) {
  return [
    "business",
    membershipId,
    locale,
    "users",
    "list",
    params?.branchId ?? null,
    params?.search ?? null,
    params?.limit ?? 25,
  ] as const;
}

export function useUserList(
  params?: ListUsersParams,
  options?: { enabled?: boolean }
): UseQueryResult<UserListResponse, Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: userListQueryKey(membershipId, normalizedLocale, params),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.listUsers(
        { token, membershipId, locale: normalizedLocale, signal },
        params
      );
    },
    enabled: Boolean(membershipId) && (options?.enabled ?? true),
  });
}
