import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type ListNotificationsParams,
  type MarkAllReadResponse,
  type NotificationListResponse,
  type NotificationResponse,
  type UnreadCountResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

/** The bell's unread count refreshes this often while the tab is visible; nothing is fetched in a hidden tab. */
export const NOTIFICATION_POLL_INTERVAL_MS = 30_000;

export function notificationsKey(membershipId: string | undefined, locale: ApiLocale) {
  return ["business", membershipId, locale, "notifications"] as const;
}

export function notificationListKey(membershipId: string | undefined, locale: ApiLocale, params: ListNotificationsParams) {
  return [...notificationsKey(membershipId, locale), "list", params.unreadOnly, params.page, params.pageSize] as const;
}

export function notificationUnreadKey(membershipId: string | undefined, locale: ApiLocale) {
  return [...notificationsKey(membershipId, locale), "unread-count"] as const;
}

export function useUnreadNotificationCount(): UseQueryResult<UnreadCountResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: notificationUnreadKey(membershipId, locale),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.getUnreadNotificationCount(await buildOptions({ signal })),
    refetchInterval: NOTIFICATION_POLL_INTERVAL_MS,
    refetchIntervalInBackground: false,
  });
}

/** The list is fetched on demand (dropdown open / full page); it is refreshed by invalidation, not by its own timer. */
export function useNotificationList(params: ListNotificationsParams, enabled = true): UseQueryResult<NotificationListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: notificationListKey(membershipId, locale, params),
    enabled: enabled && Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listNotifications(await buildOptions({ signal }), params),
  });
}

export function useMarkNotificationRead(): UseMutationResult<NotificationResponse, Error, string> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  return useMutation<NotificationResponse, Error, string>({
    mutationFn: async (notificationId) => apiClient.markNotificationRead(notificationId, await buildOptions()),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: notificationsKey(membershipId, locale) });
    },
  });
}

export function useMarkAllNotificationsRead(): UseMutationResult<MarkAllReadResponse, Error, void> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  return useMutation<MarkAllReadResponse, Error, void>({
    mutationFn: async () => apiClient.markAllNotificationsRead(await buildOptions()),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: notificationsKey(membershipId, locale) });
    },
  });
}
