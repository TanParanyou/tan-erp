import React from "react";
import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { NotificationCenter, type NotificationItem } from "./NotificationCenter";

const items: NotificationItem[] = [
  { id: "n-1", title: "ใบประมาณราคา EST-1 รออนุมัติ", timeText: "8 ต.ค. 69 10:00", isRead: false, href: "/th/estimates/e-1" },
  { id: "n-2", title: "ใบสั่งซื้อ PO-1 รออนุมัติ", timeText: "7 ต.ค. 69 09:00", isRead: true, href: null },
];

function renderCenter(props: Partial<React.ComponentProps<typeof NotificationCenter>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <NotificationCenter notifications={items} unreadCount={1} {...props} />
    </NextIntlClientProvider>,
  );
}

const bellButton = () => screen.getByRole("button", { name: thMessages.common.notificationCenter.title });

describe("NotificationCenter (controlled)", () => {
  it("shows the unread count from props, capped at 99+, and announces it politely", () => {
    renderCenter({ unreadCount: 150 });
    expect(screen.getByText("99+")).toBeDefined();
    const live = screen.getByRole("status");
    expect(live.getAttribute("aria-live")).toBe("polite");
    expect(live.textContent).toBe("150 รายการที่ยังไม่อ่าน");
  });

  it("falls back to counting unread items only when unreadCount is not provided", () => {
    renderCenter({ unreadCount: undefined });
    expect(screen.getByText("1")).toBeDefined();
  });

  it("renders no badge when nothing is unread", () => {
    renderCenter({ unreadCount: 0 });
    expect(screen.queryByText("0")).toBeNull();
  });

  it("opens, reports it, and calls onItemClick with the clicked item (parent decides what happens next)", () => {
    const onOpenChange = vi.fn();
    const onItemClick = vi.fn();
    renderCenter({ onOpenChange, onItemClick });

    fireEvent.click(bellButton());
    expect(onOpenChange).toHaveBeenLastCalledWith(true);
    expect(bellButton().getAttribute("aria-expanded")).toBe("true");

    fireEvent.click(screen.getByRole("button", { name: /EST-1/ }));
    expect(onItemClick).toHaveBeenCalledWith(items[0]);
    expect(onOpenChange).toHaveBeenLastCalledWith(false);
  });

  it("closes on Escape and returns focus to the bell button", () => {
    renderCenter();
    fireEvent.click(bellButton());
    const item = screen.getByRole("button", { name: /EST-1/ });
    item.focus();

    fireEvent.keyDown(item, { key: "Escape" });
    expect(screen.queryByRole("button", { name: /EST-1/ })).toBeNull();
    expect(document.activeElement).toBe(bellButton());
    expect(bellButton().getAttribute("aria-expanded")).toBe("false");
  });

  it("closes when clicking outside", () => {
    renderCenter();
    fireEvent.click(bellButton());
    fireEvent.mouseDown(document.body);
    expect(screen.queryByRole("button", { name: /EST-1/ })).toBeNull();
  });

  it("offers mark-all only while something is unread, and view-all", () => {
    const onMarkAllRead = vi.fn();
    const onViewAll = vi.fn();
    const { unmount } = renderCenter({ onMarkAllRead, onViewAll });
    fireEvent.click(bellButton());
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.markAllRead }));
    expect(onMarkAllRead).toHaveBeenCalledTimes(1);
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.viewAll }));
    expect(onViewAll).toHaveBeenCalledTimes(1);
    unmount();

    renderCenter({ unreadCount: 0, onMarkAllRead });
    fireEvent.click(bellButton());
    expect(screen.queryByRole("button", { name: thMessages.common.notificationCenter.markAllRead })).toBeNull();
  });

  it("shows loading, error and empty states inside the open panel", () => {
    const { unmount } = renderCenter({ isLoading: true, notifications: [] });
    fireEvent.click(bellButton());
    expect(screen.getByText(thMessages.common.notificationCenter.loading)).toBeDefined();
    unmount();

    const errored = renderCenter({ isError: true, notifications: [] });
    fireEvent.click(bellButton());
    expect(screen.getByText(thMessages.common.notificationCenter.loadFailed)).toBeDefined();
    errored.unmount();

    renderCenter({ notifications: [], unreadCount: 0 });
    fireEvent.click(bellButton());
    expect(screen.getByText(thMessages.common.notificationCenter.empty)).toBeDefined();
  });
});
