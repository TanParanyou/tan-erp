import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { MrpRunDetail } from "./mrp-run-detail";
import { MrpSettingsDrawer } from "./mrp-settings-drawer";
import { MrpRunEditor } from "./mrp-run-editor";
import * as mrpQueries from "../api/mrp-queries";
import * as procurementQueries from "@/features/procurement/api/procurement-queries";
import * as inventoryQueries from "@/features/inventory/api/inventory-queries";
import * as itemMasterQueries from "@/features/item-master/api/item-master-queries";
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

function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
      mutations: { retry: false },
    },
  });
}

function run(statuses: Array<{ action: string; status: string }>, reasons?: Array<{ sourceType: string; sourceRef: string; quantity: number; needBy: string }>): MrpRunResponse {
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
      reasons: reasons ?? [{ sourceType: "manual", sourceRef: "SO-1", quantity: 8, needBy: "2026-10-20" }],
      rowVersion: `00000000-0000-0000-0000-00000000000${i}`,
    })),
  } as MrpRunResponse;
}

const decide = vi.fn();
const convert = vi.fn();

function renderWithClient(ui: React.ReactElement, granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        {ui}
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

function renderRun(data: MrpRunResponse, granted: string[] = ALL) {
  vi.spyOn(mrpQueries, "useMrpRun").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof mrpQueries.useMrpRun>);
  vi.spyOn(mrpQueries, "useMrpMutations").mockReturnValue({
    decide: { mutateAsync: decide, isPending: false },
    convert: { mutateAsync: convert, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof mrpQueries.useMrpMutations>);
  vi.spyOn(procurementQueries, "useSupplierList").mockReturnValue({ data: { items: [{ id: "s-1", code: "SUP-1", nameTh: "ผู้ขาย" }] } } as unknown as ReturnType<typeof procurementQueries.useSupplierList>);
  vi.spyOn(inventoryQueries, "useWarehouseList").mockReturnValue({ data: { items: [{ id: "w-1", code: "WH-1", name: "คลัง" }] } } as unknown as ReturnType<typeof inventoryQueries.useWarehouseList>);

  return renderWithClient(<MrpRunDetail runId="run-1" />, granted);
}

describe("MRP status helpers and translations", () => {
  it("map recommendation statuses to badge variants", () => {
    expect(mrpStatusVariant("converted")).toBe("success");
    expect(mrpStatusVariant("proposed")).toBe("warning");
    expect(mrpStatusVariant("rejected")).toBe("danger");
  });

  it("recognise known G-14 error codes and safety_stock source", () => {
    expect(mrpErrorCode("MRP_SELF_APPROVAL")).toBe("MRP_SELF_APPROVAL");
    expect(mrpErrorCode("MRP_PLANNING_PARAMETER_INVALID")).toBe("MRP_PLANNING_PARAMETER_INVALID");
    expect(mrpErrorCode("MRP_HOLIDAY_ALREADY_EXISTS")).toBe("MRP_HOLIDAY_ALREADY_EXISTS");
    expect(mrpErrorCode("MRP_HOLIDAY_INVALID")).toBe("MRP_HOLIDAY_INVALID");
    expect(mrpErrorCode("MRP_UNIT_CONVERSION_NOT_FOUND")).toBe("MRP_UNIT_CONVERSION_NOT_FOUND");
    expect(mrpErrorCode("NOPE")).toBeNull();
    expect(MRP_REASON_SOURCES).toContain("safety_stock");
  });

  it("have Thai and English text for every action, status, source, G-14 error codes and settings", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const action of MRP_ACTIONS) expect(messages.mrp.runs.actionLabels[action]).toBeTruthy();
      for (const status of MRP_RECOMMENDATION_STATUSES) expect(messages.mrp.runs.statuses[status]).toBeTruthy();
      for (const source of MRP_REASON_SOURCES) expect(messages.mrp.runs.sources[source]).toBeTruthy();
      for (const code of [
        "MRP_SELF_APPROVAL",
        "MRP_NOT_CONVERTIBLE",
        "MRP_CONVERT_INPUT_REQUIRED",
        "MRP_DEMAND_INVALID",
        "MRP_PLANNING_PARAMETER_INVALID",
        "MRP_HOLIDAY_ALREADY_EXISTS",
        "MRP_HOLIDAY_INVALID",
        "MRP_UNIT_CONVERSION_NOT_FOUND",
      ] as const) {
        expect(messages.mrp.errors[code]).toBeTruthy();
      }
      expect(messages.mrp.settings.title).toBeTruthy();
      expect(messages.mrp.settings.tabs.parameters).toBeTruthy();
      expect(messages.mrp.settings.tabs.holidays).toBeTruthy();
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

  it("explains each recommendation from its netting and source including safety_stock", () => {
    renderRun(
      run(
        [{ action: "buy", status: "proposed" }],
        [
          { sourceType: "manual", sourceRef: "SO-1", quantity: 5, needBy: "2026-10-20" },
          { sourceType: "safety_stock", sourceRef: "SS", quantity: 3, needBy: "2026-10-20" },
        ]
      )
    );
    expect(screen.getByText(m.netting.replace("{gross}", "8").replace("{stock}", "5").replace("{receipts}", "0"))).toBeDefined();
    expect(screen.getByText("SO-1")).toBeDefined();
    expect(screen.getByText(thMessages.mrp.runs.sources.safety_stock, { exact: false })).toBeDefined();
    expect(screen.getByText("SS")).toBeDefined();
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

describe("MrpSettingsDrawer", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.spyOn(mrpQueries, "useMrpPlanningParameters").mockReturnValue({
      data: {
        items: [
          {
            id: "param-1",
            item: { id: "item-1", code: "ITEM-A", nameTh: "เหล็กแผ่น" },
            warehouse: { id: "wh-1", code: "WH-MAIN", name: "คลังหลัก" },
            safetyStock: 10,
            minOrderQuantity: 50,
            lotMultiple: 25,
            leadTimeDays: 5,
          },
        ],
        pagination: { page: 1, pageSize: 50, totalCount: 1, totalPages: 1 },
      },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof mrpQueries.useMrpPlanningParameters>);

    vi.spyOn(mrpQueries, "useMrpCalendarHolidays").mockReturnValue({
      data: {
        items: [
          {
            id: "hol-1",
            holidayDate: "2026-12-31",
            name: "วันสิ้นปี",
            createdAtUtc: "2026-10-01T00:00:00Z",
          },
        ],
      },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof mrpQueries.useMrpCalendarHolidays>);

    vi.spyOn(inventoryQueries, "useWarehouseList").mockReturnValue({
      data: { items: [{ id: "wh-1", code: "WH-MAIN", name: "คลังหลัก" }] },
    } as unknown as ReturnType<typeof inventoryQueries.useWarehouseList>);
  });

  it("renders parameters table and switches to holidays tab", async () => {
    const onClose = vi.fn();
    renderWithClient(<MrpSettingsDrawer isOpen={true} onClose={onClose} />);

    // Shows drawer title and parameters
    expect(screen.getByText(thMessages.mrp.settings.title)).toBeDefined();
    expect(screen.getByText("ITEM-A")).toBeDefined();
    expect(screen.getByText("เหล็กแผ่น")).toBeDefined();

    // Click tab holidays
    const holidaysTab = screen.getByRole("button", { name: thMessages.mrp.settings.tabs.holidays });
    fireEvent.click(holidaysTab);

    // Shows holiday form & items
    expect(await screen.findByText(thMessages.mrp.settings.addHolidayTitle)).toBeDefined();
    expect(screen.getByText("วันสิ้นปี")).toBeDefined();
  });
});

describe("MrpRunEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.spyOn(itemMasterQueries, "useItemMasterLookups").mockReturnValue({
      data: {
        categories: [],
        brands: [],
        units: [
          { id: "u-box", code: "BOX", name: { thai: "กล่อง", english: "Box" }, status: "active" },
          { id: "u-pcs", code: "PCS", name: { thai: "ชิ้น", english: "Piece" }, status: "active" },
        ],
        taxCategories: [],
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof itemMasterQueries.useItemMasterLookups>);

    vi.spyOn(mrpQueries, "useMrpMutations").mockReturnValue({
      create: { mutateAsync: vi.fn(), isPending: false },
      decide: { mutateAsync: vi.fn(), isPending: false },
      convert: { mutateAsync: vi.fn(), isPending: false },
    } as unknown as ReturnType<typeof mrpQueries.useMrpMutations>);

    vi.spyOn(mrpQueries, "useMrpPlanningParameters").mockReturnValue({
      data: { items: [], pagination: { page: 1, pageSize: 50, totalCount: 0, totalPages: 0 } },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof mrpQueries.useMrpPlanningParameters>);

    vi.spyOn(mrpQueries, "useMrpCalendarHolidays").mockReturnValue({
      data: { items: [] },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof mrpQueries.useMrpCalendarHolidays>);

    vi.spyOn(inventoryQueries, "useWarehouseList").mockReturnValue({
      data: { items: [] },
    } as unknown as ReturnType<typeof inventoryQueries.useWarehouseList>);
  });

  it("renders settings button and opens settings drawer", () => {
    renderWithClient(<MrpRunEditor />);
    const settingsBtn = screen.getByRole("button", { name: thMessages.mrp.runs.settingsButton });
    expect(settingsBtn).toBeDefined();

    fireEvent.click(settingsBtn);
    expect(screen.getByText(thMessages.mrp.settings.title)).toBeDefined();
  });
});
