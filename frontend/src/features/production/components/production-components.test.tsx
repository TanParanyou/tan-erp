import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { BomDetail } from "./bom-detail";
import { WorkOrderDetail } from "./work-order-detail";
import * as productionQueries from "../api/production-queries";
import { ApiError } from "@/lib/api/api-error";
import type { BomResponse, WorkOrderResponse } from "@/lib/api/api-client";
import { BOM_REVISION_STATUSES, WORK_ORDER_STATUSES, WORK_ORDER_TRANSACTION_KINDS, productionErrorCode, workOrderStatusVariant } from "../production-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/production",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const w = thMessages.production.workOrders;
const b = thMessages.production.boms;
const ALL = ["boms.manage", "boms.approve", "work-orders.manage", "work-orders.operate"];

function bom(status: string): BomResponse {
  return {
    id: "bom-1",
    code: "BOM-00001",
    item: { id: "i-fg", code: "FG-1", nameTh: "ตู้สำเร็จรูป", unitCode: "ชิ้น" },
    revisions: [{
      id: "rev-1", revisionNo: 1, status, outputQuantity: 1, rowVersion: "00000000-0000-0000-0000-0000000000b1",
      createdBy: { id: "u-1", displayName: "ผู้จัดทำ" },
      lines: [{ id: "l-1", component: { id: "i-1", code: "PLY", nameTh: "ไม้อัด", unitCode: "แผ่น" }, quantity: 2, scrapPercent: 10, grossQuantity: 2.2 }],
    }],
  } as BomResponse;
}

function workOrder(overrides: Partial<WorkOrderResponse> = {}): WorkOrderResponse {
  return {
    id: "wo-1",
    number: "WO-2026-0001",
    status: "in_progress",
    item: { id: "i-fg", code: "FG-1", nameTh: "ตู้สำเร็จรูป", unitCode: "ชิ้น" },
    bomCode: "BOM-00001",
    bomRevisionNo: 1,
    warehouse: { id: "w-1", code: "WH-00001", name: "คลังหลัก" },
    plannedQuantity: 4,
    completedQuantity: 0,
    costAllocated: 0,
    rowVersion: "00000000-0000-0000-0000-0000000000c1",
    createdBy: { id: "u-1", displayName: "ผู้สร้าง" },
    materials: [
      { id: "m-1", item: { id: "i-1", code: "PLY", nameTh: "ไม้อัด", unitCode: "แผ่น" }, requiredQuantity: 8, issuedQuantity: 0, returnedQuantity: 0, netIssuedQuantity: 0, remainingQuantity: 8, issuedValue: 0, returnedValue: 0 },
    ],
    transactions: [],
    ...overrides,
  } as WorkOrderResponse;
}

const materials = vi.fn();
const complete = vi.fn();
const release = vi.fn();
const cancel = vi.fn();

function renderWorkOrder(data: WorkOrderResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(productionQueries, "useWorkOrder").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof productionQueries.useWorkOrder>);
  vi.spyOn(productionQueries, "useWorkOrderMutations").mockReturnValue({
    materials: { mutateAsync: materials, isPending: false },
    complete: { mutateAsync: complete, isPending: false },
    release: { mutateAsync: release, isPending: false },
    cancel: { mutateAsync: cancel, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof productionQueries.useWorkOrderMutations>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <WorkOrderDetail workOrderId="wo-1" />
    </NextIntlClientProvider>
  );
}

function renderBom(data: BomResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(productionQueries, "useBom").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof productionQueries.useBom>);
  vi.spyOn(productionQueries, "useBomMutations").mockReturnValue({
    act: { mutateAsync: vi.fn(), isPending: false },
    createRevision: { mutateAsync: vi.fn(), isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
    updateDraft: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof productionQueries.useBomMutations>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <BomDetail bomId="bom-1" />
    </NextIntlClientProvider>
  );
}

describe("production status helpers and translations", () => {
  it("map work order statuses to badge variants", () => {
    expect(workOrderStatusVariant("completed")).toBe("success");
    expect(workOrderStatusVariant("in_progress")).toBe("info");
    expect(workOrderStatusVariant("cancelled")).toBe("danger");
    expect(workOrderStatusVariant("draft")).toBe("neutral");
  });

  it("recognise only known backend error codes", () => {
    expect(productionErrorCode("BOM_CYCLE")).toBe("BOM_CYCLE");
    expect(productionErrorCode("INVENTORY_INSUFFICIENT_STOCK")).toBe("INVENTORY_INSUFFICIENT_STOCK");
    expect(productionErrorCode("SOMETHING_ELSE")).toBeNull();
  });

  it("have Thai and English text for every status, transaction kind and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of BOM_REVISION_STATUSES) expect(messages.production.boms.statuses[status]).toBeTruthy();
      for (const status of WORK_ORDER_STATUSES) expect(messages.production.workOrders.statuses[status]).toBeTruthy();
      for (const kind of WORK_ORDER_TRANSACTION_KINDS) expect(messages.production.workOrders.kinds[kind]).toBeTruthy();
      for (const code of ["BOM_CYCLE", "BOM_SELF_APPROVAL", "PRODUCTION_MATERIAL_SHORTAGE", "PRODUCTION_OVER_COMPLETED", "INVENTORY_INSUFFICIENT_STOCK"] as const) {
        expect(messages.production.errors[code]).toBeTruthy();
      }
      expect(messages.inventory.movements.kinds.return_in).toBeTruthy();
      expect(messages.shell.workOrders).toBeTruthy();
    }
  });
});

describe("BomDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("offers approve and edit on a draft, and obsolete on an approved revision", () => {
    renderBom(bom("draft"));
    expect(screen.getByRole("button", { name: b.approve })).toBeDefined();
    expect(screen.getByText(b.editDraft)).toBeDefined();
  });

  it("hides approval from users without the approve permission", () => {
    renderBom(bom("draft"), ["boms.manage"]);
    expect(screen.queryByRole("button", { name: b.approve })).toBeNull();
  });
});

describe("WorkOrderDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("posts an issue with only the entered quantities and an idempotency key", async () => {
    materials.mockResolvedValue(workOrder());
    renderWorkOrder(workOrder());
    fireEvent.change(screen.getByLabelText(w.issueQuantityFor.replace("{item}", "PLY")), { target: { value: "3" } });
    fireEvent.click(screen.getByRole("button", { name: w.postIssue }));
    await waitFor(() => expect(materials).toHaveBeenCalledTimes(1));
    const call = materials.mock.calls[0][0] as { operation: string; payload: { lines: Array<{ itemId: string; quantity: number }> }; idempotencyKey: string };
    expect(call.operation).toBe("issues");
    expect(call.payload.lines).toEqual([{ itemId: "i-1", quantity: 3 }]);
    expect(call.idempotencyKey.length).toBeGreaterThanOrEqual(16);
  });

  it("shows the translated stock error instead of the raw message", async () => {
    materials.mockRejectedValue(new ApiError({ status: 422, code: "INVENTORY_INSUFFICIENT_STOCK", message: "raw" }));
    renderWorkOrder(workOrder());
    fireEvent.change(screen.getByLabelText(w.issueQuantityFor.replace("{item}", "PLY")), { target: { value: "3" } });
    fireEvent.click(screen.getByRole("button", { name: w.postIssue }));
    expect(await screen.findByText(thMessages.production.errors.INVENTORY_INSUFFICIENT_STOCK)).toBeDefined();
  });

  it("rejects an empty issue without calling the API", async () => {
    renderWorkOrder(workOrder());
    fireEvent.click(screen.getByRole("button", { name: w.postIssue }));
    expect(await screen.findByText(w.quantitiesInvalid)).toBeDefined();
    expect(materials).not.toHaveBeenCalled();
  });

  it("shows the output form only while in progress, and release only for a draft", () => {
    renderWorkOrder(workOrder({ status: "draft" }));
    expect(screen.getByRole("button", { name: w.release })).toBeDefined();
    expect(screen.queryByText(w.completeTitle)).toBeNull();
  });

  it("hides stock actions from users without the operate permission", () => {
    renderWorkOrder(workOrder(), ["work-orders.manage"]);
    expect(screen.queryByRole("button", { name: w.postIssue })).toBeNull();
  });
});
