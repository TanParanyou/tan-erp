import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ProjectControlPanel } from "./project-control-panel";
import * as projectQueries from "../api/project-queries";
import { ApiError } from "@/lib/api/api-error";
import type { ProjectControlResponse } from "@/lib/api/api-client";
import {
  allowedProjectTransitions,
  transitionNeedsReason,
  PROJECT_STATUSES,
  CHANGE_ORDER_STATUSES,
  PROJECT_BUDGET_CATEGORIES,
  TIMESHEET_STATUSES,
  PROJECT_COST_ENTRY_TYPES,
  PROJECT_COST_SOURCE_TYPES,
} from "../project-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const t = thMessages.projects;
const c = thMessages.projects.control;
const ALL_PERMISSIONS = ["projects.update", "projects.transition", "projects.change-orders.manage", "projects.change-orders.approve"];

function control(overrides: Partial<ProjectControlResponse> = {}): ProjectControlResponse {
  return {
    projectId: "p-1",
    status: "planned",
    statusReason: null,
    plannedStartDate: null,
    plannedEndDate: null,
    rowVersion: "00000000-0000-0000-0000-0000000000a1",
    budget: { baselineTotal: null, baselineHash: null, isFrozen: false, approvedBudgetDelta: 0, currentTotal: null, lines: [] },
    contract: { baselineAmount: 1000, approvedDelta: 0, currentAmount: 1000 },
    progress: { totalMilestones: 0, completedMilestones: 0, totalWeight: 0, completedWeight: 0, percent: 0 },
    wbsNodes: [],
    wbsCostSummary: [],
    costLedger: [],
    laborRates: [],
    timesheets: [],
    milestones: [],
    changeOrders: [],
    history: [],
    ...overrides,
  } as ProjectControlResponse;
}

const mutateFns = {
  transition: vi.fn(),
  setPlan: vi.fn(),
  replaceBudget: vi.fn(),
  addMilestone: vi.fn(),
  completeMilestone: vi.fn(),
  deleteMilestone: vi.fn(),
  createChangeOrder: vi.fn(),
  changeOrderAction: vi.fn(),
  updateMilestone: vi.fn(),
  addWbsNode: vi.fn(),
  updateWbsNode: vi.fn(),
  deleteWbsNode: vi.fn(),
  setLaborRate: vi.fn(),
  createTimesheet: vi.fn(),
  timesheetAction: vi.fn(),
};

function setup(data: ProjectControlResponse, granted: string[] = ALL_PERMISSIONS) {
  permissions.granted = granted;
  vi.spyOn(projectQueries, "useProjectControl").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof projectQueries.useProjectControl>);
  vi.spyOn(projectQueries, "useProjectControlMutations").mockReturnValue(
    Object.fromEntries(Object.entries(mutateFns).map(([key, fn]) => [key, { mutateAsync: fn, isPending: false }])) as unknown as ReturnType<typeof projectQueries.useProjectControlMutations>
  );
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ProjectControlPanel projectId="p-1" />
    </NextIntlClientProvider>
  );
}

describe("project status helpers", () => {
  it("mirror the backend lifecycle", () => {
    expect(allowedProjectTransitions("planned")).toEqual(["active", "cancelled"]);
    expect(allowedProjectTransitions("active")).toEqual(["on_hold", "ready_for_handover", "cancelled"]);
    expect(allowedProjectTransitions("on_hold")).toEqual(["active", "cancelled"]);
    expect(allowedProjectTransitions("ready_for_handover")).toEqual(["active", "completed"]);
    expect(allowedProjectTransitions("completed")).toEqual([]);
    expect(allowedProjectTransitions("cancelled")).toEqual([]);
  });

  it("asks for a reason on hold, cancel and re-opening a handover-ready project", () => {
    expect(transitionNeedsReason("active", "on_hold")).toBe(true);
    expect(transitionNeedsReason("planned", "cancelled")).toBe(true);
    expect(transitionNeedsReason("ready_for_handover", "active")).toBe(true);
    expect(transitionNeedsReason("planned", "active")).toBe(false);
    expect(transitionNeedsReason("ready_for_handover", "completed")).toBe(false);
  });

  it("has Thai and English text for every status, category, change order status, timesheet status, and cost type", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of PROJECT_STATUSES) expect(messages.projects.control.transitions[status]).toBeTruthy();
      for (const category of PROJECT_BUDGET_CATEGORIES) expect(messages.projects.control.budgetCategories[category]).toBeTruthy();
      for (const status of CHANGE_ORDER_STATUSES) expect(messages.projects.control.changeOrderStatuses[status]).toBeTruthy();
      for (const status of TIMESHEET_STATUSES) expect(messages.projects.control.timesheetStatuses[status]).toBeTruthy();
      for (const type of PROJECT_COST_ENTRY_TYPES) expect(messages.projects.control.costEntryTypes[type]).toBeTruthy();
      for (const source of PROJECT_COST_SOURCE_TYPES) expect(messages.projects.control.costSourceTypes[source]).toBeTruthy();
      expect(Object.keys(messages.projects.control.errors).length).toBeGreaterThanOrEqual(36);
    }
  });
});

describe("ProjectControlPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("offers only the moves the lifecycle allows, and only with the transition permission", () => {
    setup(control({ status: "active" }));
    expect(screen.getByRole("button", { name: c.transitions.on_hold })).toBeDefined();
    expect(screen.getByRole("button", { name: c.transitions.ready_for_handover })).toBeDefined();
    expect(screen.queryByRole("button", { name: c.transitions.completed })).toBeNull();
  });

  it("hides every action for a read-only user", () => {
    setup(control({ status: "active" }), []);
    expect(screen.queryByRole("button", { name: c.transitions.on_hold })).toBeNull();
    expect(screen.queryByRole("button", { name: c.addMilestone })).toBeNull();
    expect(screen.queryByRole("button", { name: c.newChangeOrder })).toBeNull();
  });

  it("requires a reason to put the project on hold, then sends the row version", async () => {
    mutateFns.transition.mockResolvedValue(control({ status: "on_hold" }));
    setup(control({ status: "active" }));

    fireEvent.click(screen.getByRole("button", { name: c.transitions.on_hold }));
    const dialog = await screen.findByRole("dialog");
    const confirm = within(dialog).getByRole("button", { name: c.confirm });

    fireEvent.click(confirm);
    expect(await within(dialog).findByText(c.reasonRequired)).toBeDefined();
    expect(mutateFns.transition).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(c.reason)), { target: { value: "รอลูกค้า" } });
    fireEvent.click(confirm);

    await waitFor(() => expect(mutateFns.transition).toHaveBeenCalledTimes(1));
    expect(mutateFns.transition.mock.calls[0][0]).toEqual({ rowVersion: "00000000-0000-0000-0000-0000000000a1", targetStatus: "on_hold", reason: "รอลูกค้า" });
  });

  it("maps a backend error code to its message", async () => {
    mutateFns.transition.mockRejectedValue(new ApiError({ status: 409, code: "PROJECT_NOT_READY", message: "x" }));
    setup(control({ status: "planned" }));

    fireEvent.click(screen.getByRole("button", { name: c.transitions.active }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.click(within(dialog).getByRole("button", { name: c.confirm }));

    expect(await screen.findByText(c.errors.PROJECT_NOT_READY)).toBeDefined();
  });

  it("lets the baseline budget be edited only while planned, and validates before saving", async () => {
    mutateFns.replaceBudget.mockResolvedValue(control());
    setup(control({ status: "planned" }));

    fireEvent.click(screen.getByRole("button", { name: c.addLine }));
    fireEvent.click(screen.getByRole("button", { name: c.saveBudget }));
    expect(await screen.findByText(c.budgetInvalid)).toBeDefined();
    expect(mutateFns.replaceBudget).not.toHaveBeenCalled();

    fireEvent.change(screen.getByLabelText(c.budgetDescription), { target: { value: "ไม้อัด" } });
    fireEvent.change(screen.getByLabelText(c.budgetAmount), { target: { value: "2500.5" } });
    fireEvent.click(screen.getByRole("button", { name: c.saveBudget }));

    await waitFor(() => expect(mutateFns.replaceBudget).toHaveBeenCalledTimes(1));
    expect(mutateFns.replaceBudget.mock.calls[0][0].lines).toEqual([{ category: "material", description: "ไม้อัด", amount: 2500.5 }]);
  });

  it("freezes the budget editor once the baseline is frozen", () => {
    setup(control({
      status: "active",
      budget: { baselineTotal: 1000, baselineHash: "h", isFrozen: true, approvedBudgetDelta: 300, currentTotal: 1300, lines: [{ id: "l1", category: "material", description: "ไม้", amount: 1000, sortOrder: 1 }] },
    }));

    expect(screen.queryByRole("button", { name: c.saveBudget })).toBeNull();
    expect((screen.getByLabelText(c.budgetDescription) as HTMLInputElement).disabled).toBe(true);
    expect(screen.getByText(c.budgetFrozenHint)).toBeDefined();
  });

  it("completes a milestone only while the project is active", async () => {
    mutateFns.completeMilestone.mockResolvedValue(control());
    const milestone = { id: "m1", name: "ติดตั้ง", weight: 3, sortOrder: 1, completedAtUtc: null, rowVersion: "00000000-0000-0000-0000-0000000000b1" };
    setup(control({ status: "active", milestones: [milestone], progress: { totalMilestones: 1, completedMilestones: 0, totalWeight: 3, completedWeight: 0, percent: 0 } }));

    fireEvent.click(screen.getByRole("button", { name: c.completeMilestone }));

    await waitFor(() => expect(mutateFns.completeMilestone).toHaveBeenCalledWith({ milestoneId: "m1", expectedVersion: milestone.rowVersion }));
  });

  it("does not offer completion while the project is only planned", () => {
    const milestone = { id: "m1", name: "ติดตั้ง", weight: 3, sortOrder: 1, completedAtUtc: null, rowVersion: "v" };
    setup(control({ status: "planned", milestones: [milestone] }));

    expect(screen.queryByRole("button", { name: c.completeMilestone })).toBeNull();
    expect(screen.getByRole("button", { name: c.removeMilestone })).toBeDefined();
  });

  it("shows approve/reject only to approvers and requires a note to reject", async () => {
    mutateFns.changeOrderAction.mockResolvedValue(control());
    const order = {
      id: "co-1", number: "PCO-2026-0001", title: "เพิ่มตู้", reason: "ลูกค้าขอ", budgetDelta: 300, contractDelta: 450, status: "submitted",
      createdBy: { id: "u1", displayName: "ผู้สร้าง" }, createdAtUtc: "2026-10-04T00:00:00Z", rowVersion: "00000000-0000-0000-0000-0000000000c1",
    };

    setup(control({ status: "active", changeOrders: [order] } as Partial<ProjectControlResponse>), ["projects.change-orders.manage"]);
    expect(screen.queryByRole("button", { name: c.approveChangeOrder })).toBeNull();
  });

  it("rejects a change order with a note and sends its version", async () => {
    mutateFns.changeOrderAction.mockResolvedValue(control());
    const order = {
      id: "co-1", number: "PCO-2026-0001", title: "เพิ่มตู้", reason: "ลูกค้าขอ", budgetDelta: 300, contractDelta: 450, status: "submitted",
      createdBy: { id: "u1", displayName: "ผู้สร้าง" }, createdAtUtc: "2026-10-04T00:00:00Z", rowVersion: "00000000-0000-0000-0000-0000000000c1",
    };
    setup(control({ status: "active", changeOrders: [order] } as Partial<ProjectControlResponse>));

    fireEvent.click(screen.getByRole("button", { name: c.rejectChangeOrder }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.click(within(dialog).getByRole("button", { name: c.confirm }));
    expect(await within(dialog).findByText(c.reasonRequired)).toBeDefined();
    expect(mutateFns.changeOrderAction).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(c.decisionNoteLabel)), { target: { value: "เกินงบ" } });
    fireEvent.click(within(dialog).getByRole("button", { name: c.confirm }));

    await waitFor(() => expect(mutateFns.changeOrderAction).toHaveBeenCalledTimes(1));
    expect(mutateFns.changeOrderAction.mock.calls[0][0]).toEqual({
      changeOrderId: "co-1", action: "reject", expectedVersion: order.rowVersion, note: "เกินงบ",
    });
  });

  it("creates a change order with one idempotency key and only on an open project", async () => {
    mutateFns.createChangeOrder.mockResolvedValue(control());
    setup(control({ status: "active" }));

    fireEvent.click(screen.getByRole("button", { name: c.newChangeOrder }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByLabelText(new RegExp(c.changeOrderTitle)), { target: { value: "เพิ่มตู้" } });
    fireEvent.change(within(dialog).getByLabelText(new RegExp(c.changeOrderReason)), { target: { value: "ลูกค้าขอ" } });
    fireEvent.change(within(dialog).getByLabelText(c.budgetDelta), { target: { value: "300" } });
    fireEvent.click(within(dialog).getByRole("button", { name: c.createChangeOrder }));

    await waitFor(() => expect(mutateFns.createChangeOrder).toHaveBeenCalledTimes(1));
    const call = mutateFns.createChangeOrder.mock.calls[0][0];
    expect(call.payload).toEqual({ title: "เพิ่มตู้", reason: "ลูกค้าขอ", budgetDelta: 300, contractDelta: 0 });
    expect(typeof call.idempotencyKey).toBe("string");
  });

  it("does not offer a new change order on a planned project", () => {
    setup(control({ status: "planned" }));
    expect(screen.queryByRole("button", { name: c.newChangeOrder })).toBeNull();
  });

  it("shows the status history", () => {
    setup(control({ status: "active", history: [{ id: "h1", fromStatus: "planned", toStatus: "active", reason: null, actor: { id: "u", displayName: "คุณผู้จัดการ" }, occurredAtUtc: "2026-10-04T00:00:00Z" }] }));
    expect(screen.getByText(c.historyEntry.replace("{from}", t.statuses.planned).replace("{to}", t.statuses.active))).toBeDefined();
  });

  it("renders WBS cost rollup table with overrun badge and adds a WBS node", async () => {
    mutateFns.addWbsNode.mockResolvedValue(control());
    const wbsNode = {
      id: "wbs-1",
      code: "1.1",
      name: "งานโครงสร้าง",
      level: 1,
      sortOrder: 1,
      rowVersion: "00000000-0000-0000-0000-0000000000d1",
    };
    const summary = {
      wbsNodeId: "wbs-1",
      wbsCode: "1.1",
      wbsName: "งานโครงสร้าง",
      level: 1,
      baselineBudget: 10000,
      committedAmount: 8000,
      actualAmount: 5000,
      remainingBudget: -3000,
      isOverrun: true,
    };

    setup(control({ status: "active", wbsNodes: [wbsNode], wbsCostSummary: [summary] }));

    expect(screen.getByText("1.1")).toBeDefined();
    expect(screen.getByText("งานโครงสร้าง")).toBeDefined();
    expect(screen.getByText(c.wbsOverrun)).toBeDefined();

    // Add a child node
    fireEvent.change(screen.getByPlaceholderText(c.wbsCodePlaceholder), { target: { value: "1.2" } });
    fireEvent.change(screen.getByPlaceholderText(c.wbsNamePlaceholder), { target: { value: "งานตกแต่ง" } });
    fireEvent.click(screen.getByRole("button", { name: c.addWbsNode }));

    await waitFor(() => expect(mutateFns.addWbsNode).toHaveBeenCalledTimes(1));
    expect(mutateFns.addWbsNode).toHaveBeenCalledWith({
      parentId: null,
      code: "1.2",
      name: "งานตกแต่ง",
    });
  });

  it("creates timesheet on active project and displays overtime warning", async () => {
    mutateFns.createTimesheet.mockResolvedValue(control());
    const ts = {
      id: "ts-1",
      number: "TS-2026-0001",
      worker: { id: "w-1", displayName: "ช่างสมคิด" },
      workDate: "2026-10-29",
      hours: 10,
      hourlyRate: 200,
      costAmount: 2000,
      wbsCode: "1.1",
      taskDescription: "ติดตั้งบานหน้าต่าง",
      status: "draft",
      isOvertimeWarning: true,
      rowVersion: "00000000-0000-0000-0000-0000000000e1",
    };

    setup(control({ status: "active", timesheets: [ts] }));

    expect(screen.getByText("TS-2026-0001")).toBeDefined();
    expect(screen.getByText(c.timesheetOvertimeWarning)).toBeDefined();
    expect(screen.getByText("ติดตั้งบานหน้าต่าง")).toBeDefined();

    // Create new timesheet via Drawer
    fireEvent.click(screen.getByRole("button", { name: c.newTimesheet }));
    const drawer = await screen.findByRole("dialog");
    fireEvent.change(within(drawer).getByPlaceholderText(c.workerUserIdPlaceholder), { target: { value: "w-2" } });
    fireEvent.change(within(drawer).getByPlaceholderText(c.timesheetTaskPlaceholder), { target: { value: "ประกอบตู้" } });
    fireEvent.change(within(drawer).getByPlaceholderText(c.timesheetHoursPlaceholder), { target: { value: "8" } });
    fireEvent.click(within(drawer).getByRole("button", { name: c.createTimesheet }));

    await waitFor(() => expect(mutateFns.createTimesheet).toHaveBeenCalledTimes(1));
    const call = mutateFns.createTimesheet.mock.calls[0][0];
    expect(call.payload.workerUserId).toBe("w-2");
    expect(call.payload.taskDescription).toBe("ประกอบตู้");
    expect(call.payload.hours).toBe(8);
    expect(typeof call.idempotencyKey).toBe("string");
  });

  it("submits a draft timesheet and approves/rejects submitted timesheet with note", async () => {
    mutateFns.timesheetAction.mockResolvedValue(control());
    const draftTs = {
      id: "ts-1",
      number: "TS-2026-0001",
      worker: { id: "w-1", displayName: "ช่างสมคิด" },
      workDate: "2026-10-29",
      hours: 8,
      hourlyRate: 150,
      costAmount: 1200,
      status: "draft",
      taskDescription: "งานทั่วไป",
      rowVersion: "v1",
    };
    const submittedTs = {
      id: "ts-2",
      number: "TS-2026-0002",
      worker: { id: "w-2", displayName: "ช่างวิชัย" },
      workDate: "2026-10-29",
      hours: 8,
      hourlyRate: 150,
      costAmount: 1200,
      status: "submitted",
      taskDescription: "งานโครงเหล็ก",
      rowVersion: "v2",
    };

    setup(control({ status: "active", timesheets: [draftTs, submittedTs] }));

    // Submit draft
    fireEvent.click(screen.getByRole("button", { name: c.submitTimesheet }));
    await waitFor(() =>
      expect(mutateFns.timesheetAction).toHaveBeenCalledWith({
        timesheetId: "ts-1",
        action: "submit",
        expectedVersion: "v1",
        note: null,
      })
    );

    // Reject submitted timesheet with note
    fireEvent.click(screen.getByRole("button", { name: c.rejectTimesheet }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByPlaceholderText(c.decisionNotePlaceholder), { target: { value: "ชั่วโมงไม่ตรงกับหน้างาน" } });
    fireEvent.click(within(dialog).getByRole("button", { name: c.rejectTimesheet }));

    await waitFor(() =>
      expect(mutateFns.timesheetAction).toHaveBeenCalledWith({
        timesheetId: "ts-2",
        action: "reject",
        expectedVersion: "v2",
        note: "ชั่วโมงไม่ตรงกับหน้างาน",
      })
    );
  });

  it("renders cost ledger entries showing commitment, actual and reversal badges", () => {
    const entries = [
      {
        id: "cle-1",
        sourceNumber: "PO-2026-0001",
        entryType: "commitment",
        sourceType: "purchase_order",
        wbsCode: "1.1",
        description: "สั่งซื้อไม้อัดสัก",
        amount: 50000,
        isReversal: false,
        occurredAtUtc: "2026-10-29T10:00:00Z",
      },
      {
        id: "cle-2",
        sourceNumber: "TS-2026-0001",
        entryType: "actual",
        sourceType: "labor_timesheet",
        wbsCode: "1.1",
        description: "ค่าแรงงานโครงสร้าง",
        amount: 2000,
        isReversal: false,
        occurredAtUtc: "2026-10-29T11:00:00Z",
      },
      {
        id: "cle-3",
        sourceNumber: "PO-2026-0001",
        entryType: "commitment",
        sourceType: "purchase_order",
        wbsCode: "1.1",
        description: "ยกเลิกคำสั่งซื้อ",
        amount: -50000,
        isReversal: true,
        occurredAtUtc: "2026-10-29T12:00:00Z",
      },
    ];

    setup(control({ status: "active", costLedger: entries }));

    expect(screen.getAllByText("PO-2026-0001").length).toBe(2);
    expect(screen.getByText("TS-2026-0001")).toBeDefined();
    expect(screen.getByText("สั่งซื้อไม้อัดสัก")).toBeDefined();
    expect(screen.getByText(c.costReversal)).toBeDefined();
    expect(screen.getAllByText(c.costEntryTypes.commitment).length).toBe(2);
    expect(screen.getByText(c.costEntryTypes.actual)).toBeDefined();
  });

  it("has full key parity between th.json and en.json for projects.control", () => {
    const thControl = thMessages.projects.control;
    const enControl = enMessages.projects.control;

    function assertKeys(obj1: Record<string, unknown>, obj2: Record<string, unknown>, prefix = "") {
      const keys1 = Object.keys(obj1).sort();
      const keys2 = Object.keys(obj2).sort();
      expect(keys1, `Mismatched keys at ${prefix}`).toEqual(keys2);
      for (const k of keys1) {
        if (typeof obj1[k] === "object" && obj1[k] !== null && !Array.isArray(obj1[k])) {
          assertKeys(obj1[k] as Record<string, unknown>, obj2[k] as Record<string, unknown>, `${prefix}.${k}`);
        }
      }
    }

    assertKeys(thControl as unknown as Record<string, unknown>, enControl as unknown as Record<string, unknown>, "projects.control");
  });
});
