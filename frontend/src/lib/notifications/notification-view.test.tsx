import React from "react";
import { describe, expect, it } from "vitest";
import { renderHook } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { NotificationResponse } from "@/lib/api/api-client";
import { localizedNotificationHref, useNotificationText } from "./notification-view";

const base: NotificationResponse = {
  id: "n-1",
  type: "estimate.approval-requested",
  payload: { documentNumber: "EST-1", actorDisplayName: "สมชาย", costTotal: "999" },
  deepLink: "/estimates/e-1",
  createdAtUtc: "2026-10-08T00:00:00Z",
  readAtUtc: null,
};

function textFor(notification: NotificationResponse): string {
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <NextIntlClientProvider locale="th" messages={thMessages}>{children}</NextIntlClientProvider>
  );
  return renderHook(() => useNotificationText(), { wrapper }).result.current(notification);
}

describe("notification view helpers", () => {
  it("renders the registered message with only the declared payload fields", () => {
    expect(textFor(base)).toBe("สมชาย ส่งใบประมาณราคา EST-1 เพื่อรออนุมัติ");
  });

  it("renders a neutral line for an unknown type and never throws", () => {
    expect(textFor({ ...base, type: "customer.created" })).toBe(thMessages.notifications.unknownType);
  });

  it("builds a locale-prefixed path only from a same-origin absolute path", () => {
    expect(localizedNotificationHref("th", "/estimates/e-1")).toBe("/th/estimates/e-1");
    expect(localizedNotificationHref("en", null)).toBeNull();
    expect(localizedNotificationHref("th", "//evil.test/x")).toBeNull();
    expect(localizedNotificationHref("th", "https://evil.test/x")).toBeNull();
  });
});
