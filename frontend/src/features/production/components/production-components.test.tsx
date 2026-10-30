import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { BomDetail } from "./bom-detail";
import { BomEditor } from "./bom-editor";
import { WorkOrderDetail } from "./work-order-detail";
import { WorkCenterDrawer } from "./work-center-drawer";
import * as productionQueries from "../api/production-queries";
import * as procurementQueries from "@/features/procurement/api/procurement-queries";
import { ApiError } from "@/lib/api/api-error";
import type {
  BomResponse,
  WorkCenterCapacityViewResponse,
  WorkCenterListResponse,
  WorkCenterResponse,
  WorkOrderResponse,
} from "@/lib/api/api-client";
import {
  BOM_REVISION_STATUSES,
  WORK_ORDER_STATUSES,
  WORK_ORDER_TRANSACTION_KINDS,
  WORK_CENTER_STATUSES,
  OPERATION_STATUSES,
  productionErrorCode,
  workOrderStatusVariant,
} from "../production-status";

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
const wc = thMessages.production.workCenters;
const ALL = ["boms.read", "boms.manage", "boms.approve", "work-orders.read", "work-orders.manage", "work-orders.operate", "purchase-orders.read"];

function createTestQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
    },
  });
}

function bom(status: string): BomResponse {
  return {
    id: "bom-1",
    code: "BOM-00001",
    item: { id: "i-fg", code: "FG-1", nameTh: "ตู้สำเร็จรูป", unitCode: "ชิ้น" },
    revisions: [{
      id: "rev-1", revisionNo: 1, status, outputQuantity: 1, rowVersion: "00000000-0000-0000-0000-0000000000b1",
      createdBy: { id: "u-1", displayName: "ผู้จัดทำ" },
      lines: [{ id: "l-1", component: { id: "i-1", code: "PLY", nameTh: "ไม้อัด", unitCode: "แผ่น" }, quantity: 2, scrapPercent: 10, grossQuantity: 2.2 }],
      routingSteps: [
        {
          id: "rs-1",
          sequence: 10,
          operationCode: "OP-010",
          operationName: "ตัดไม้",
          workCenter: { id: "wc-1", code: "WC-CUT", name: "แผนกตัด" },
          setupHours: 0.5,
          runHours: 1.5,
          isSubcontracted: false,
          stdSubcontractCost: 0,
          note: "ตัดตามแบบ",
        },
        {
          id: "rs-2",
          sequence: 20,
          operationCode: "OP-020",
          operationName: "ชุบสีภายนอก",
          workCenter: null,
          setupHours: 0,
          runHours: 0,
          isSubcontracted: true,
          stdSubcontractCost: 450,
          note: "ส่งชุบโรงงานภายนอก",
        },
      ],
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
    netMaterialValue: 2400,
    laborHours: 5,
    laborCost: 1500,
    subcontractCost: 800,
    totalManufacturingCost: 4700,
    plannedStartDate: "2026-11-01",
    plannedEndDate: "2026-11-05",
    rowVersion: "00000000-0000-0000-0000-0000000000c1",
    createdBy: { id: "u-1", displayName: "ผู้สร้าง" },
    materials: [
      { id: "m-1", item: { id: "i-1", code: "PLY", nameTh: "ไม้อัด", unitCode: "แผ่น" }, requiredQuantity: 8, issuedQuantity: 0, returnedQuantity: 0, netIssuedQuantity: 0, remainingQuantity: 8, issuedValue: 0, returnedValue: 0 },
    ],
    operations: [
      {
        id: "op-1",
        sequence: 10,
        operationCode: "OP-010",
        operationName: "ตัดไม้",
        workCenter: { id: "wc-1", code: "WC-CUT", name: "แผนกตัด" },
        plannedSetupHours: 0.5,
        plannedRunHours: 1.5,
        actualHours: null,
        isSubcontracted: false,
        subcontractPurchaseOrder: null,
        subcontractCost: 0,
        status: "pending",
        note: null,
        completedAtUtc: null,
      },
      {
        id: "op-2",
        sequence: 20,
        operationCode: "OP-020",
        operationName: "ชุบสีภายนอก",
        workCenter: null,
        plannedSetupHours: 0,
        plannedRunHours: 0,
        actualHours: null,
        isSubcontracted: true,
        subcontractPurchaseOrder: null,
        subcontractCost: 0,
        status: "pending",
        note: null,
        completedAtUtc: null,
      },
    ],
    laborEntries: [
      {
        id: "le-1",
        number: "TS-2026-0001",
        worker: { id: "w-1", displayName: "สมชาย ช่างไม้" },
        workDate: "2026-11-02",
        hours: 5,
        hourlyRate: 300,
        costAmount: 1500,
        taskDescription: "ตัดไม้และไสผิวงาน",
        decidedAtUtc: "2026-11-02T17:00:00Z",
      },
    ],
    transactions: [],
    ...overrides,
  } as WorkOrderResponse;
}

const materials = vi.fn();
const complete = vi.fn();
const release = vi.fn();
const cancel = vi.fn();
const completeOperation = vi.fn();
const subcontractOperation = vi.fn();

function renderWorkOrder(data: WorkOrderResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  vi.spyOn(productionQueries, "useWorkOrder").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof productionQueries.useWorkOrder>);
  vi.spyOn(productionQueries, "useWorkOrderMutations").mockReturnValue({
    materials: { mutateAsync: materials, isPending: false },
    complete: { mutateAsync: complete, isPending: false },
    release: { mutateAsync: release, isPending: false },
    cancel: { mutateAsync: cancel, isPending: false },
    completeOperation: { mutateAsync: completeOperation, isPending: false },
    subcontractOperation: { mutateAsync: subcontractOperation, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof productionQueries.useWorkOrderMutations>);
  vi.spyOn(procurementQueries, "usePurchaseOrderList").mockReturnValue({
    data: {
      items: [
        { id: "po-1", number: "PO-2026-0001", status: "approved", totalAmount: 5000, supplierCode: "SUP-01", supplierNameTh: "บริษัท ชุบสี จำกัด" },
      ],
      pagination: { page: 1, pageSize: 100, totalCount: 1, totalPages: 1 },
    },
    isLoading: false,
    isError: false,
  } as unknown as ReturnType<typeof procurementQueries.usePurchaseOrderList>);

  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <WorkOrderDetail workOrderId="wo-1" />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

function renderBom(data: BomResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  vi.spyOn(productionQueries, "useBom").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof productionQueries.useBom>);
  vi.spyOn(productionQueries, "useBomMutations").mockReturnValue({
    act: { mutateAsync: vi.fn(), isPending: false },
    createRevision: { mutateAsync: vi.fn(), isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
    updateDraft: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof productionQueries.useBomMutations>);
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <BomDetail bomId="bom-1" />
      </NextIntlClientProvider>
    </QueryClientProvider>
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
    expect(productionErrorCode("PRODUCTION_WORK_CENTER_CODE_CONFLICT")).toBe("PRODUCTION_WORK_CENTER_CODE_CONFLICT");
    expect(productionErrorCode("PRODUCTION_OPERATION_NOT_SUBCONTRACTED")).toBe("PRODUCTION_OPERATION_NOT_SUBCONTRACTED");
    expect(productionErrorCode("SOMETHING_ELSE")).toBeNull();
  });

  it("have Thai and English text for every status, transaction kind and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of BOM_REVISION_STATUSES) expect(messages.production.boms.statuses[status]).toBeTruthy();
      for (const status of WORK_ORDER_STATUSES) expect(messages.production.workOrders.statuses[status]).toBeTruthy();
      for (const kind of WORK_ORDER_TRANSACTION_KINDS) expect(messages.production.workOrders.kinds[kind]).toBeTruthy();
      for (const status of WORK_CENTER_STATUSES) {
        expect(status === "active" ? messages.production.workCenters.statusActive : messages.production.workCenters.statusInactive).toBeTruthy();
      }
      for (const status of OPERATION_STATUSES) {
        expect(messages.production.workOrders.operationStatuses[status]).toBeTruthy();
      }
      for (const code of [
        "BOM_CYCLE",
        "BOM_SELF_APPROVAL",
        "PRODUCTION_MATERIAL_SHORTAGE",
        "PRODUCTION_OVER_COMPLETED",
        "INVENTORY_INSUFFICIENT_STOCK",
        "PRODUCTION_WORK_CENTER_CODE_CONFLICT",
        "PRODUCTION_WORK_CENTER_VERSION_CONFLICT",
        "PRODUCTION_WORK_CENTER_IN_USE",
        "PRODUCTION_WORK_CENTER_INACTIVE",
        "PRODUCTION_WORK_CENTER_INVALID",
        "BOM_ROUTING_INVALID",
        "PRODUCTION_OPERATION_NOT_SUBCONTRACTED",
        "PRODUCTION_OPERATION_ALREADY_COMPLETED",
        "PRODUCTION_OPERATION_INVALID",
        "PRODUCTION_SUBCONTRACT_PO_NOT_APPROVED",
        "PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO",
        "PRODUCTION_HAS_ACTUAL_COSTS",
      ] as const) {
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

  it("displays routing steps when present in revision", () => {
    renderBom(bom("approved"));
    expect(screen.getByText("ตัดไม้")).toBeDefined();
    expect(screen.getByText("ชุบสีภายนอก")).toBeDefined();
    expect(screen.getByText(b.routing.subcontractBadge)).toBeDefined();
  });
});

describe("BomEditor routing steps", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders routing steps section and allows adding an operation", async () => {
    const client = createTestQueryClient();
    vi.spyOn(productionQueries, "useWorkCenterList").mockReturnValue({
      data: {
        items: [{ id: "wc-1", code: "WC-01", name: "สายตัด 1", status: "active", dailyCapacityHours: 8, hourlyRate: 250 }],
        pagination: { page: 1, pageSize: 100, totalCount: 1, totalPages: 1 },
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof productionQueries.useWorkCenterList>);
    vi.spyOn(productionQueries, "useBom").mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof productionQueries.useBom>);
    vi.spyOn(productionQueries, "useBomMutations").mockReturnValue({
      create: { mutateAsync: vi.fn(), isPending: false },
      updateDraft: { mutateAsync: vi.fn(), isPending: false },
    } as unknown as ReturnType<typeof productionQueries.useBomMutations>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <BomEditor />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText(b.routing.title)).toBeDefined();
    const addStepBtn = screen.getByRole("button", { name: b.routing.addStep });
    fireEvent.click(addStepBtn);

    expect(screen.getByPlaceholderText(b.routing.operationNamePlaceholder)).toBeDefined();
  });
});

describe("WorkCenterDrawer", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders work centers list and switches to capacity tab", () => {
    const client = createTestQueryClient();
    const mockCenters: WorkCenterResponse[] = [
      { id: "wc-1", code: "WC-01", name: "สายตัด", dailyCapacityHours: 8, hourlyRate: 300, status: "active", rowVersion: "v1" },
    ];
    const mockCapacity: WorkCenterCapacityViewResponse = {
      fromDate: "2026-11-01",
      toDate: "2026-11-14",
      workCenters: [
        {
          workCenterId: "wc-1",
          code: "WC-01",
          name: "สายตัด",
          dailyCapacityHours: 8,
          totalCapacityHours: 80,
          totalPlannedHours: 40,
          utilizationPercent: 50,
          isOverloaded: false,
          days: [
            { date: "2026-11-01", capacityHours: 8, plannedHours: 4, utilizationPercent: 50, isOverloaded: false },
          ],
        },
      ],
    };

    vi.spyOn(productionQueries, "useWorkCenterList").mockReturnValue({
      data: { items: mockCenters, pagination: { page: 1, pageSize: 100, totalCount: 1, totalPages: 1 } },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof productionQueries.useWorkCenterList>);

    vi.spyOn(productionQueries, "useWorkCenterCapacity").mockReturnValue({
      data: mockCapacity,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof productionQueries.useWorkCenterCapacity>);

    vi.spyOn(productionQueries, "useWorkCenterMutations").mockReturnValue({
      create: { mutateAsync: vi.fn(), isPending: false },
      update: { mutateAsync: vi.fn(), isPending: false },
      setActive: { mutateAsync: vi.fn(), isPending: false },
    } as unknown as ReturnType<typeof productionQueries.useWorkCenterMutations>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <WorkCenterDrawer isOpen={true} onClose={vi.fn()} canManage={true} />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText(wc.title)).toBeDefined();
    expect(screen.getByText("สายตัด")).toBeDefined();

    // Switch to Capacity tab
    const capacityTab = screen.getByRole("button", { name: wc.tabs.capacity });
    fireEvent.click(capacityTab);
    expect(screen.getByText(wc.normalCapacity)).toBeDefined();
  });
});

describe("WorkOrderDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("displays cost breakdown and schedule dates", () => {
    renderWorkOrder(workOrder());
    expect(screen.getByText(w.netMaterialValue)).toBeDefined();
    expect(screen.getByText(w.laborCost)).toBeDefined();
    expect(screen.getByText(w.subcontractCost)).toBeDefined();
    expect(screen.getByText(w.totalManufacturingCost)).toBeDefined();
    expect(screen.getByText("2026-11-01 ~ 2026-11-05")).toBeDefined();
  });

  it("displays routing operations and completes an internal operation", async () => {
    completeOperation.mockResolvedValue(workOrder());
    renderWorkOrder(workOrder());

    expect(screen.getByText(w.operationsTitle)).toBeDefined();
    expect(screen.getByText("ตัดไม้")).toBeDefined();

    const completeBtn = screen.getByRole("button", { name: w.completeOperation });
    fireEvent.click(completeBtn);

    // Modal opens
    expect(screen.getByText(w.completeOperationTitle)).toBeDefined();
    const actualHoursInput = screen.getByPlaceholderText(w.actualHoursPlaceholder);
    fireEvent.change(actualHoursInput, { target: { value: "2.5" } });

    // Click save in modal
    const saveBtns = screen.getAllByRole("button", { name: thMessages.common.actions.save });
    fireEvent.click(saveBtns[saveBtns.length - 1]);

    await waitFor(() => expect(completeOperation).toHaveBeenCalledTimes(1));
    const call = completeOperation.mock.calls[0][0] as {
      id: string;
      operationId: string;
      payload: { actualHours: number };
      idempotencyKey: string;
    };
    expect(call.operationId).toBe("op-1");
    expect(call.payload.actualHours).toBe(2.5);
  });

  it("links a subcontract purchase order to a subcontracted operation", async () => {
    subcontractOperation.mockResolvedValue(workOrder());
    renderWorkOrder(workOrder());

    const linkPoBtn = screen.getByRole("button", { name: w.linkSubcontractPo });
    fireEvent.click(linkPoBtn);

    // Modal opens
    expect(screen.getByText(w.linkSubcontractPoTitle)).toBeDefined();
    const costInput = screen.getByPlaceholderText(w.costAmountPlaceholder);
    fireEvent.change(costInput, { target: { value: "850" } });

    const select = screen.getByLabelText(new RegExp(`^${w.purchaseOrder.replace("(", "\\(").replace(")", "\\)")}`));
    fireEvent.change(select, { target: { value: "po-1" } });

    const saveBtns = screen.getAllByRole("button", { name: thMessages.common.actions.save });
    fireEvent.click(saveBtns[saveBtns.length - 1]);

    await waitFor(() => expect(subcontractOperation).toHaveBeenCalledTimes(1));
    const call = subcontractOperation.mock.calls[0][0] as {
      id: string;
      operationId: string;
      payload: { purchaseOrderId: string; costAmount: number };
      idempotencyKey: string;
    };
    expect(call.operationId).toBe("op-2");
    expect(call.payload.purchaseOrderId).toBe("po-1");
    expect(call.payload.costAmount).toBe(850);
  });

  it("displays approved labor timesheets (G-20) table", () => {
    renderWorkOrder(workOrder());
    expect(screen.getByText(w.laborEntriesTitle)).toBeDefined();
    expect(screen.getByText("TS-2026-0001")).toBeDefined();
    expect(screen.getByText("สมชาย ช่างไม้")).toBeDefined();
    expect(screen.getByText("ตัดไม้และไสผิวงาน")).toBeDefined();
  });

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
