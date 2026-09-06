import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type CurrentUserResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import type { SupportedLocale } from "@/lib/i18n/locales";

export function currentUserQueryKey(firebaseUid: string | null | undefined, locale: SupportedLocale = "th") {
  return ["current-user", firebaseUid, locale] as const;
}

export function useCurrentUser(
  firebaseUid: string | null | undefined,
  locale: SupportedLocale = "th"
): UseQueryResult<CurrentUserResponse, Error> {
  return useQuery({
    queryKey: currentUserQueryKey(firebaseUid, locale),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new Error("No authentication token available");
      }
      return apiClient.getCurrentUser(token, locale, signal);
    },
    enabled: Boolean(firebaseUid),
    retry: (failureCount, error: any) => {
      // Do not retry 401/403
      if (error?.status === 401 || error?.status === 403) {
        return false;
      }
      return failureCount < 2;
    },
  });
}
