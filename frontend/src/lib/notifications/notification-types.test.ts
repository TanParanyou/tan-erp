import { describe, expect, it } from "vitest";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import {
  NOTIFICATION_TYPES,
  isNotificationType,
  notificationMessageKey,
  notificationMessageValues,
} from "./notification-types";

describe("notification types", () => {
  it("mirrors the backend NotificationTypes whitelist", () => {
    expect(Object.keys(NOTIFICATION_TYPES).sort()).toEqual([
      "change-order.approval-requested",
      "cost-record.approval-requested",
      "estimate.approval-requested",
      "membership.expiring",
      "mrp-run.approval-requested",
      "project.budget-overrun",
      "purchase-order.approval-requested",
      "role-assignment.approval-requested",
    ]);
  });

  it("narrows only registered types, exactly", () => {
    expect(isNotificationType("estimate.approval-requested")).toBe(true);
    expect(isNotificationType("Estimate.Approval-Requested")).toBe(false);
    expect(isNotificationType("customer.created")).toBe(false);
    expect(notificationMessageKey("customer.created")).toBeNull();
    expect(notificationMessageKey("mrp-run.approval-requested")).toBe("mrpRunApprovalRequested");
  });

  it("passes only the declared fields to the message and shows a dash for a missing one", () => {
    expect(
      notificationMessageValues("estimate.approval-requested", {
        documentNumber: "EST-1",
        actorDisplayName: "สมชาย",
        unexpected: "x",
      }),
    ).toEqual({ documentNumber: "EST-1", actorDisplayName: "สมชาย" });
    expect(notificationMessageValues("estimate.approval-requested", { documentNumber: "EST-1" })).toEqual({
      documentNumber: "EST-1",
      actorDisplayName: "-",
    });
  });

  it.each([
    ["th", thMessages],
    ["en", enMessages],
  ])("has a %s message for every type, using every declared field", (_locale, messages) => {
    for (const descriptor of Object.values(NOTIFICATION_TYPES)) {
      const template: string = messages.notifications.types[descriptor.messageKey];
      expect(template).toBeTruthy();
      for (const field of descriptor.fields) expect(template).toContain(`{${field}}`);
    }
  });

  it("has the same notifications keys in th and en", () => {
    const flat = (value: unknown, prefix = ""): string[] =>
      typeof value === "object" && value !== null
        ? Object.entries(value).flatMap(([key, child]) => flat(child, `${prefix}${key}.`))
        : [prefix];
    expect(flat(enMessages.notifications).sort()).toEqual(flat(thMessages.notifications).sort());
  });

  it("declares exactly the payload fields the membership expiring message uses", () => {
    expect(NOTIFICATION_TYPES["membership.expiring"].fields).toEqual(["subjectDisplayName", "daysRemaining"]);
    expect(
      notificationMessageValues("membership.expiring", {
        subjectDisplayName: "สมหญิง",
        daysRemaining: "7",
        resourceId: "ignored",
        parentId: "ignored",
      }),
    ).toEqual({ subjectDisplayName: "สมหญิง", daysRemaining: "7" });
  });
});
