import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { NotificationResponse } from "@/lib/api/api-client";
import { NotificationBell } from "./NotificationBell";

const state = vi.hoisted(() => ({
  membership: { id: "m-1" } as { id: string } | null,
  unread: 2,
  items: [] as NotificationResponse[],
  push: vi.fn(),
  markRead: vi.fn(),
  markAll: vi.fn(),
  listCalls: [] as boolean[],
}));

vi.mock("next/navigation", () => ({ useRouter: () => ({ push: state.push }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: state.membership }),
}));
vi.mock("@/hooks/useNotifications", () => ({
  useUnreadNotificationCount: () => ({ data: { unreadCount: state.unread } }),
  useNotificationList: (_params: unknown, enabled: boolean) => {
    state.listCalls.push(enabled);
    return { data: enabled ? { items: state.items } : undefined, isLoading: false, isError: false };
  },
  useMarkNotificationRead: () => ({ mutateAsync: state.markRead }),
  useMarkAllNotificationsRead: () => ({ mutate: state.markAll }),
}));

const notification = (over: Partial<NotificationResponse>): NotificationResponse => ({
  id: "n-1",
  type: "purchase-order.approval-requested",
  payload: { documentNumber: "PO-1", actorDisplayName: "สมชาย" },
  deepLink: "/procurement/purchase-orders/po-1",
  createdAtUtc: "2026-10-08T00:00:00Z",
  readAtUtc: null,
  ...over,
});

const renderBell = () =>
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <NotificationBell />
    </NextIntlClientProvider>,
  );
const open = () => fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.title }));

describe("NotificationBell", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    state.membership = { id: "m-1" };
    state.unread = 2;
    state.items = [notification({})];
    state.listCalls = [];
    state.markRead.mockResolvedValue(undefined);
  });

  it("renders nothing without a selected membership", () => {
    state.membership = null;
    const { container } = renderBell();
    expect(container.firstChild).toBeNull();
  });

  it("does not fetch the list until the dropdown is opened", () => {
    renderBell();
    expect(state.listCalls.every((enabled) => enabled === false)).toBe(true);
    open();
    expect(state.listCalls.at(-1)).toBe(true);
  });

  it("marks an unread item read and then navigates to its locale-prefixed deep link", async () => {
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: /PO-1/ }));

    await waitFor(() => expect(state.push).toHaveBeenCalledWith("/th/procurement/purchase-orders/po-1"));
    expect(state.markRead).toHaveBeenCalledWith("n-1");
    expect(state.markRead.mock.invocationCallOrder[0]).toBeLessThan(state.push.mock.invocationCallOrder[0]);
  });

  it("does not mark an already-read item again, and does not navigate when the link is null", async () => {
    state.items = [notification({ readAtUtc: "2026-10-08T01:00:00Z", deepLink: null })];
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: /PO-1/ }));

    // The dropdown closes on item click; wait for that so the (synchronous) click handler has settled.
    await waitFor(() => expect(screen.queryByRole("button", { name: /PO-1/ })).toBeNull());
    expect(state.markRead).not.toHaveBeenCalled();
    expect(state.push).not.toHaveBeenCalled();
  });

  it("still navigates when marking read fails (the item just stays unread)", async () => {
    state.markRead.mockRejectedValue(new Error("network"));
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: /PO-1/ }));
    await waitFor(() => expect(state.push).toHaveBeenCalledTimes(1));
  });

  it("renders a neutral line for an unknown type instead of throwing", () => {
    state.items = [notification({ type: "customer.created", payload: {}, deepLink: null })];
    renderBell();
    open();
    expect(screen.getByText(thMessages.notifications.unknownType)).toBeDefined();
  });

  it("marks everything read and opens the full page from the footer", () => {
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.markAllRead }));
    expect(state.markAll).toHaveBeenCalledTimes(1);
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.viewAll }));
    expect(state.push).toHaveBeenCalledWith("/th/notifications");
  });
});
