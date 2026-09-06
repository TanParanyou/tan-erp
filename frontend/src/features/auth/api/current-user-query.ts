import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type CurrentUserResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import type { SupportedLocale } from "@/lib/i18n/locales";

export function currentUserQueryKey(firebaseUid: string | null | undefined, locale: SupportedLocale = "th") {
  return ["current-user", firebaseUid, locale] as const;
}

export function useCurrentUser(
  firebaseUid: string | null | undefined,
  locale?: SupportedLocale
): UseQueryResult<CurrentUserResponse, Error> {
  const routeLocale = useSafeLocale();
  const effectiveLocale = locale || routeLocale;

  return useQuery<CurrentUserResponse, Error>({
    queryKey: currentUserQueryKey(firebaseUid, effectiveLocale),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) {
        throw new Error("No authentication token available");
      }
      return apiClient.getCurrentUser(token, effectiveLocale, signal);
    },
    enabled: Boolean(firebaseUid),
    retry: (failureCount, error) => {
      // Do not retry 401/403
      const status = error && typeof error === "object" && "status" in error ? (error as { status?: unknown }).status : undefined;
      if (status === 401 || status === 403) {
        return false;
      }
      return failureCount < 2;
    },
  });
}
