import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { act, renderHook, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { apiClient } from "@/lib/api/api-client";
import {
  NOTIFICATION_POLL_INTERVAL_MS,
  notificationUnreadKey,
  notificationsKey,
  useMarkAllNotificationsRead,
  useMarkNotificationRead,
  useNotificationList,
  useUnreadNotificationCount,
} from "./useNotifications";

const context = vi.hoisted(() => ({ membershipId: "m-1" as string | undefined }));

vi.mock("@/lib/api/use-api-request-context", () => ({
  useApiRequestContext: () => ({
    membershipId: context.membershipId,
    locale: "th",
    buildOptions: async () => ({ token: "tok", membershipId: context.membershipId ?? "", locale: "th" }),
  }),
}));
vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const original = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...original,
    apiClient: {
      listNotifications: vi.fn(),
      getUnreadNotificationCount: vi.fn(),
      markNotificationRead: vi.fn(),
      markAllNotificationsRead: vi.fn(),
    },
  };
});

function setup() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
  return { queryClient, wrapper };
}

describe("useNotifications", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    context.membershipId = "m-1";
  });

  it("namespaces keys by membership and locale", () => {
    expect(notificationsKey("m-1", "th")).not.toEqual(notificationsKey("m-2", "th"));
    expect(notificationsKey("m-1", "th")).not.toEqual(notificationsKey("m-1", "en"));
    expect(notificationUnreadKey("m-1", "th").slice(0, 4)).toEqual(notificationsKey("m-1", "th"));
  });

  it("polls the unread count only while the tab is visible", async () => {
    vi.mocked(apiClient.getUnreadNotificationCount).mockResolvedValue({ unreadCount: 3 });
    const { queryClient, wrapper } = setup();
    const { result } = renderHook(() => useUnreadNotificationCount(), { wrapper });

    await waitFor(() => expect(result.current.data?.unreadCount).toBe(3));
    const options = queryClient.getQueryCache().find({ queryKey: notificationUnreadKey("m-1", "th") })?.observers[0]?.options;
    expect(options?.refetchInterval).toBe(NOTIFICATION_POLL_INTERVAL_MS);
    expect(options?.refetchIntervalInBackground).toBe(false);
  });

  it("does not call the API without a selected membership", async () => {
    context.membershipId = undefined;
    const { wrapper } = setup();
    renderHook(() => useUnreadNotificationCount(), { wrapper });
    renderHook(() => useNotificationList({ unreadOnly: false, page: 1, pageSize: 20 }), { wrapper });

    await act(async () => {});
    expect(apiClient.getUnreadNotificationCount).not.toHaveBeenCalled();
    expect(apiClient.listNotifications).not.toHaveBeenCalled();
  });

  it("refreshes the list and the count after mark-read and read-all", async () => {
    vi.mocked(apiClient.markNotificationRead).mockResolvedValue({
      id: "n-1",
      type: "estimate.approval-requested",
      payload: {},
      deepLink: null,
      createdAtUtc: "2026-10-08T00:00:00Z",
      readAtUtc: "2026-10-08T00:01:00Z",
    });
    vi.mocked(apiClient.markAllNotificationsRead).mockResolvedValue({ updatedCount: 2 });
    const { queryClient, wrapper } = setup();
    const invalidate = vi.spyOn(queryClient, "invalidateQueries");

    const markRead = renderHook(() => useMarkNotificationRead(), { wrapper });
    await act(async () => {
      await markRead.result.current.mutateAsync("n-1");
    });
    expect(apiClient.markNotificationRead).toHaveBeenCalledWith("n-1", expect.objectContaining({ membershipId: "m-1" }));
    expect(invalidate).toHaveBeenCalledWith({ queryKey: notificationsKey("m-1", "th") });

    invalidate.mockClear();
    const markAll = renderHook(() => useMarkAllNotificationsRead(), { wrapper });
    await act(async () => {
      await markAll.result.current.mutateAsync();
    });
    expect(invalidate).toHaveBeenCalledWith({ queryKey: notificationsKey("m-1", "th") });
  });
});
