import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { NotificationResponse } from "@/lib/api/api-client";
import { NotificationListPage } from "./notification-list-page";

const state = vi.hoisted(() => ({
  search: "",
  items: [] as NotificationResponse[],
  totalCount: 0,
  isError: false,
  listArgs: [] as unknown[],
  push: vi.fn(),
  replace: vi.fn(),
  markRead: vi.fn(),
  markAll: vi.fn(),
  markAllPending: false,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: state.push, replace: state.replace }),
  usePathname: () => "/th/notifications",
  useSearchParams: () => new URLSearchParams(state.search),
}));
vi.mock("@/hooks/useNotifications", () => ({
  useNotificationList: (params: unknown) => {
    state.listArgs.push(params);
    return {
      data: { items: state.items, pagination: { page: 1, pageSize: 10, totalCount: state.totalCount, totalPages: 1 } },
      isLoading: false,
      isError: state.isError,
      error: null,
      refetch: vi.fn(),
    };
  },
  useMarkNotificationRead: () => ({ mutate: state.markRead, isPending: false }),
  useMarkAllNotificationsRead: () => ({ mutate: state.markAll, isPending: state.markAllPending }),
}));

const row = (over: Partial<NotificationResponse>): NotificationResponse => ({
  id: "n-1",
  type: "estimate.approval-requested",
  payload: { documentNumber: "EST-1", actorDisplayName: "สมชาย" },
  deepLink: "/estimates/e-1",
  createdAtUtc: "2026-10-08T00:00:00Z",
  readAtUtc: null,
  ...over,
});

const renderPage = () =>
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <NotificationListPage />
    </NextIntlClientProvider>,
  );

describe("NotificationListPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    state.search = "";
    state.items = [row({}), row({ id: "n-2", type: "customer.created", payload: {}, deepLink: null, readAtUtc: "2026-10-08T01:00:00Z" })];
    state.totalCount = 2;
    state.isError = false;
    state.listArgs = [];
    state.markAllPending = false;
  });

  it("lists registered and unknown types without throwing, with read status text", () => {
    renderPage();
    expect(screen.getByText("สมชาย ส่งใบประมาณราคา EST-1 เพื่อรออนุมัติ")).toBeDefined();
    expect(screen.getByText(thMessages.notifications.unknownType)).toBeDefined();
    expect(screen.getAllByText(thMessages.notifications.list.statusUnread).length).toBeGreaterThanOrEqual(1);
  });

  it("queries all rows by default and unread only when the filter is set in the URL, never above 50 per page", () => {
    renderPage();
    expect(state.listArgs.at(-1)).toEqual({ unreadOnly: false, page: 1, pageSize: 10 });

    state.search = "status=unread&limit=100";
    state.listArgs = [];
    renderPage();
    expect(state.listArgs.at(-1)).toEqual({ unreadOnly: true, page: 1, pageSize: 50 });
  });

  it("marks a single row read from its action, only for unread rows", () => {
    renderPage();
    const markButtons = screen.getAllByRole("button", { name: thMessages.notifications.list.markRead });
    expect(markButtons).toHaveLength(1);
    fireEvent.click(markButtons[0]);
    expect(state.markRead).toHaveBeenCalledWith("n-1");
  });

  it("links a row to its locale-prefixed document, and offers no link when the reader lost access", () => {
    renderPage();
    const links = screen.getAllByRole("link", { name: thMessages.notifications.list.open });
    expect(links).toHaveLength(1);
    expect(links[0].getAttribute("href")).toBe("/th/estimates/e-1");
  });

  it("marks all read without a confirmation modal, and locks the button while the request runs", () => {
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: thMessages.notifications.list.markAllRead }));
    expect(state.markAll).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("dialog")).toBeNull();
  });

  it("disables mark-all while pending or when nothing is unread", () => {
    state.markAllPending = true;
    const first = renderPage();
    expect((screen.getByRole("button", { name: thMessages.notifications.list.markAllRead }) as HTMLButtonElement).disabled).toBe(true);
    first.unmount();

    state.markAllPending = false;
    state.items = [row({ readAtUtc: "2026-10-08T01:00:00Z" })];
    renderPage();
    expect((screen.getByRole("button", { name: thMessages.notifications.list.markAllRead }) as HTMLButtonElement).disabled).toBe(true);
  });

  it("shows the table error state when loading fails", () => {
    state.isError = true;
    renderPage();
    expect(screen.getByText(thMessages.notifications.loadFailed)).toBeDefined();
  });

  it("shows the empty state when there are no rows", () => {
    state.items = [];
    state.totalCount = 0;
    renderPage();
    expect(within(document.body).getByText(thMessages.notifications.list.empty)).toBeDefined();
  });
});
