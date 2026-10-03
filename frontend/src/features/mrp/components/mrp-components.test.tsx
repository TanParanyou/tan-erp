import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { MrpRunDetail } from "./mrp-run-detail";
import * as mrpQueries from "../api/mrp-queries";
import * as procurementQueries from "@/features/procurement/api/procurement-queries";
import * as inventoryQueries from "@/features/inventory/api/inventory-queries";
import { ApiError } from "@/lib/api/api-error";
import type { MrpRunResponse } from "@/lib/api/api-client";
import { MRP_ACTIONS, MRP_REASON_SOURCES, MRP_RECOMMENDATION_STATUSES, mrpErrorCode, mrpStatusVariant } from "../mrp-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/production/mrp",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const m = thMessages.mrp.runs;
const ALL = ["mrp.read", "mrp.run", "mrp.approve", "mrp.convert"];

function run(statuses: Array<{ action: string; status: string }>): MrpRunResponse {
  return {
    id: "run-1",
    number: "MRP-2026-0001",
    asOfDate: "2026-10-04",
    purchaseLeadTimeDays: 7,
    productionLeadTimeDays: 3,
    inputHash: "abcdef0123456789abcdef",
    snapshot: { demandCount: 1, supplyCount: 0, bomCount: 2, stockItemCount: 1 },
    createdBy: { id: "u-1", displayName: "ผู้วางแผน" },
    recommendations: statuses.map((s, i) => ({
      id: `rec-${i}`,
      lineNo: i + 1,
      item: { id: `i-${i}`, code: `ITEM-${i}`, nameTh: "สินค้า", unitCode: "แผ่น" },
      action: s.action,
      status: s.status,
      quantity: 3,
      needBy: "2026-10-20",
      orderBy: "2026-10-10",
      level: 0,
      grossRequirement: 8,
      stockUsed: 5,
      scheduledReceiptsUsed: 0,
      reasons: [{ sourceType: "manual", sourceRef: "SO-1", quantity: 8, needBy: "2026-10-20" }],
      rowVersion: `00000000-0000-0000-0000-00000000000${i}`,
    })),
  } as MrpRunResponse;
}

const decide = vi.fn();
const convert = vi.fn();

function renderRun(data: MrpRunResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(mrpQueries, "useMrpRun").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof mrpQueries.useMrpRun>);
  vi.spyOn(mrpQueries, "useMrpMutations").mockReturnValue({
    decide: { mutateAsync: decide, isPending: false },
    convert: { mutateAsync: convert, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof mrpQueries.useMrpMutations>);
  vi.spyOn(procurementQueries, "useSupplierList").mockReturnValue({ data: { items: [{ id: "s-1", code: "SUP-1", nameTh: "ผู้ขาย" }] } } as unknown as ReturnType<typeof procurementQueries.useSupplierList>);
  vi.spyOn(inventoryQueries, "useWarehouseList").mockReturnValue({ data: { items: [{ id: "w-1", code: "WH-1", name: "คลัง" }] } } as unknown as ReturnType<typeof inventoryQueries.useWarehouseList>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <MrpRunDetail runId="run-1" />
    </NextIntlClientProvider>
  );
}

describe("MRP status helpers and translations", () => {
  it("map recommendation statuses to badge variants", () => {
    expect(mrpStatusVariant("converted")).toBe("success");
    expect(mrpStatusVariant("proposed")).toBe("warning");
    expect(mrpStatusVariant("rejected")).toBe("danger");
  });

  it("recognise only known error codes", () => {
    expect(mrpErrorCode("MRP_SELF_APPROVAL")).toBe("MRP_SELF_APPROVAL");
    expect(mrpErrorCode("NOPE")).toBeNull();
  });

  it("have Thai and English text for every action, status, source and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const action of MRP_ACTIONS) expect(messages.mrp.runs.actionLabels[action]).toBeTruthy();
      for (const status of MRP_RECOMMENDATION_STATUSES) expect(messages.mrp.runs.statuses[status]).toBeTruthy();
      for (const source of MRP_REASON_SOURCES) expect(messages.mrp.runs.sources[source]).toBeTruthy();
      for (const code of ["MRP_SELF_APPROVAL", "MRP_NOT_CONVERTIBLE", "MRP_CONVERT_INPUT_REQUIRED", "MRP_DEMAND_INVALID"] as const) {
        expect(messages.mrp.errors[code]).toBeTruthy();
      }
      expect(messages.shell.mrpRuns).toBeTruthy();
    }
  });
});

describe("MrpRunDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("lets an approver approve a proposed buy or make, but never a shortage", () => {
    renderRun(run([{ action: "buy", status: "proposed" }, { action: "shortage", status: "proposed" }]));
    expect(screen.getAllByRole("button", { name: m.approve })).toHaveLength(1);
    expect(screen.getAllByRole("button", { name: m.reject })).toHaveLength(2);
  });

  it("explains each recommendation from its netting and source", () => {
    renderRun(run([{ action: "buy", status: "proposed" }]));
    expect(screen.getByText(m.netting.replace("{gross}", "8").replace("{stock}", "5").replace("{receipts}", "0"))).toBeDefined();
    expect(screen.getByText("SO-1")).toBeDefined();
  });

  it("sends the row version when approving", async () => {
    decide.mockResolvedValue(run([]));
    renderRun(run([{ action: "make", status: "proposed" }]));
    fireEvent.click(screen.getByRole("button", { name: m.approve }));
    await waitFor(() => expect(decide).toHaveBeenCalledWith({ runId: "run-1", recommendationId: "rec-0", rowVersion: "00000000-0000-0000-0000-000000000000", decision: "approve" }));
  });

  it("shows the translated error for a self-approval attempt", async () => {
    decide.mockRejectedValue(new ApiError({ status: 403, code: "MRP_SELF_APPROVAL", message: "raw" }));
    renderRun(run([{ action: "buy", status: "proposed" }]));
    fireEvent.click(screen.getByRole("button", { name: m.approve }));
    expect(await screen.findByText(thMessages.mrp.errors.MRP_SELF_APPROVAL)).toBeDefined();
  });

  it("requires a supplier and price before converting a buy recommendation", async () => {
    renderRun(run([{ action: "buy", status: "approved" }]));
    fireEvent.click(screen.getByRole("button", { name: m.convert }));
    fireEvent.click(await screen.findByRole("button", { name: m.confirmConvert }));
    expect(await screen.findByText(m.convertBuyRequired)).toBeDefined();
    expect(convert).not.toHaveBeenCalled();
  });

  it("hides decisions and conversion from users without those permissions", () => {
    renderRun(run([{ action: "buy", status: "proposed" }, { action: "make", status: "approved" }]), ["mrp.read"]);
    expect(screen.queryByRole("button", { name: m.approve })).toBeNull();
    expect(screen.queryByRole("button", { name: m.convert })).toBeNull();
  });
});
