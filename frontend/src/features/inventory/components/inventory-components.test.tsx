import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { StockDocumentModal } from "./stock-document-modal";
import { StockReserveModal } from "./stock-reserve-modal";
import { PutIntoStockButton } from "./put-into-stock-button";
import { StockBalanceList } from "./stock-balance-list";
import * as inventoryQueries from "../api/inventory-queries";
import * as projectQueries from "@/features/projects/api/project-queries";
import { ApiError } from "@/lib/api/api-error";
import type { StockBalanceResponse } from "@/lib/api/api-client";
import { inventoryErrorCode, isMovementKind, MOVEMENT_KINDS } from "../inventory-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/inventory/stock",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));
vi.mock("@/features/item-master/components/item-autocomplete", () => ({
  ItemAutocomplete: ({ onChange }: { onChange: (id: string, item: { code: string; name: { thai: string }; baseUnit: { code: string } }) => void }) => (
    <button type="button" onClick={() => onChange("item-1", { code: "PLY", name: { thai: "ไม้อัด" }, baseUnit: { code: "แผ่น" } })}>pick-item</button>
  ),
}));

const d = thMessages.inventory.documents;
const r = thMessages.inventory.reserve;
const rc = thMessages.inventory.receive;
const st = thMessages.inventory.stock;

const warehouses = { items: [
  { id: "w-1", code: "WH-00001", name: "คลังหลัก", status: "active", rowVersion: "v" },
  { id: "w-2", code: "WH-00002", name: "คลังสอง", status: "active", rowVersion: "v" },
], pagination: { page: 1, pageSize: 100, totalCount: 2, totalPages: 1 } };

const issue = vi.fn();
const transfer = vi.fn();
const adjust = vi.fn();
const reserve = vi.fn();
const receive = vi.fn();

function mockHooks() {
  vi.spyOn(inventoryQueries, "useWarehouseList").mockReturnValue({ data: warehouses, isLoading: false, isError: false } as unknown as ReturnType<typeof inventoryQueries.useWarehouseList>);
  vi.spyOn(inventoryQueries, "useStockMutations").mockReturnValue({
    issue: { mutateAsync: issue, isPending: false },
    transfer: { mutateAsync: transfer, isPending: false },
    adjust: { mutateAsync: adjust, isPending: false },
    reserve: { mutateAsync: reserve, isPending: false },
    receive: { mutateAsync: receive, isPending: false },
  } as unknown as ReturnType<typeof inventoryQueries.useStockMutations>);
  vi.spyOn(projectQueries, "useProjectList").mockReturnValue({
    data: { items: [{ id: "p-1", code: "PRJ-1", name: "งานตู้" }], pagination: { page: 1, pageSize: 100, totalCount: 1, totalPages: 1 } },
  } as unknown as ReturnType<typeof projectQueries.useProjectList>);
}

function wrap(node: React.ReactNode) {
  return render(<NextIntlClientProvider locale="th" messages={thMessages}>{node}</NextIntlClientProvider>);
}

describe("inventory status helpers and translations", () => {
  it("recognise movement kinds and backend error codes", () => {
    expect(isMovementKind("transfer_in")).toBe(true);
    expect(isMovementKind("bogus")).toBe(false);
    expect(inventoryErrorCode("INVENTORY_INSUFFICIENT_STOCK")).toBe("INVENTORY_INSUFFICIENT_STOCK");
    expect(inventoryErrorCode("OTHER")).toBeNull();
  });

  it("have Thai and English text for every movement kind and error", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const kind of MOVEMENT_KINDS) expect(messages.inventory.movements.kinds[kind]).toBeTruthy();
      expect(Object.keys(messages.inventory.errors).length).toBeGreaterThanOrEqual(24);
      expect(messages.shell.stockBalances).toBeTruthy();
      expect(messages.shell.warehouses).toBeTruthy();
      expect(messages.procurement.orders.putIntoStockDone).toBeTruthy();
    }
  });
});

describe("StockDocumentModal", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    permissions.granted = ["projects.read"];
    mockHooks();
  });

  it("validates, then issues with the project, reason and one idempotency key", async () => {
    issue.mockResolvedValue({});
    const onClose = vi.fn();
    wrap(<StockDocumentModal kind="issue" onClose={onClose} />);
    const dialog = await screen.findByRole("dialog");

    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));
    expect(await within(dialog).findByText(d.warehouseRequired)).toBeDefined();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(d.warehouse)), { target: { value: "w-1" } });
    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));
    expect(await within(dialog).findByText(d.linesInvalid)).toBeDefined();
    expect(issue).not.toHaveBeenCalled();

    fireEvent.click(within(dialog).getByRole("button", { name: "pick-item" }));
    fireEvent.change(within(dialog).getByLabelText(d.quantity), { target: { value: "3" } });
    fireEvent.change(within(dialog).getByLabelText(new RegExp(d.project.slice(0, 8))), { target: { value: "p-1" } });
    fireEvent.change(within(dialog).getByLabelText(d.reason), { target: { value: "เบิกใช้งาน" } });
    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));

    await waitFor(() => expect(issue).toHaveBeenCalledTimes(1));
    const call = issue.mock.calls[0][0];
    expect(call.payload).toEqual({ warehouseId: "w-1", projectId: "p-1", reason: "เบิกใช้งาน", lines: [{ itemId: "item-1", quantity: 3 }] });
    expect(typeof call.idempotencyKey).toBe("string");
    await waitFor(() => expect(onClose).toHaveBeenCalled());
    expect(toastMocks.success).toHaveBeenCalledWith(d.posted.issue);
  });

  it("refuses a transfer between the same warehouse before calling the API", async () => {
    wrap(<StockDocumentModal kind="transfer" onClose={vi.fn()} />);
    const dialog = await screen.findByRole("dialog");

    fireEvent.change(within(dialog).getByLabelText(new RegExp(d.fromWarehouse)), { target: { value: "w-1" } });
    fireEvent.change(within(dialog).getByLabelText(new RegExp(d.toWarehouse)), { target: { value: "w-1" } });
    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));

    expect(await within(dialog).findByText(thMessages.inventory.errors.INVENTORY_TRANSFER_SAME_WAREHOUSE)).toBeDefined();
    expect(transfer).not.toHaveBeenCalled();
  });

  it("requires a reason for an adjustment, allows a counted quantity of 0 and sends the optional cost", async () => {
    adjust.mockResolvedValue({});
    wrap(<StockDocumentModal kind="adjustment" onClose={vi.fn()} defaultWarehouseId="w-1" />);
    const dialog = await screen.findByRole("dialog");

    fireEvent.click(within(dialog).getByRole("button", { name: "pick-item" }));
    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));
    expect(await within(dialog).findByText(d.reasonRequired)).toBeDefined();
    expect(adjust).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(d.reason)), { target: { value: "ของหาย" } });
    fireEvent.change(within(dialog).getByLabelText(d.unitCost), { target: { value: "150" } });
    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));

    await waitFor(() => expect(adjust).toHaveBeenCalledTimes(1));
    expect(adjust.mock.calls[0][0].payload).toEqual({ warehouseId: "w-1", reason: "ของหาย", lines: [{ itemId: "item-1", countedQuantity: 0, unitCost: 150 }] });
  });

  it("maps an insufficient-stock error to its message and keeps the dialog open", async () => {
    issue.mockRejectedValue(new ApiError({ status: 422, code: "INVENTORY_INSUFFICIENT_STOCK", message: "x" }));
    const onClose = vi.fn();
    wrap(<StockDocumentModal kind="issue" onClose={onClose} defaultWarehouseId="w-1" />);
    const dialog = await screen.findByRole("dialog");

    fireEvent.click(within(dialog).getByRole("button", { name: "pick-item" }));
    fireEvent.click(within(dialog).getByRole("button", { name: d.post }));

    expect(await within(dialog).findByText(thMessages.inventory.errors.INVENTORY_INSUFFICIENT_STOCK)).toBeDefined();
    expect(onClose).not.toHaveBeenCalled();
  });
});

describe("StockReserveModal and PutIntoStockButton", () => {
  const balance = { warehouse: { id: "w-1", code: "WH-1", name: "คลัง" }, item: { id: "item-1", code: "PLY", nameTh: "ไม้อัด", unitCode: "แผ่น" }, available: 7 } as StockBalanceResponse;

  beforeEach(() => {
    vi.clearAllMocks();
    permissions.granted = [];
    mockHooks();
  });

  it("reserves the chosen quantity for the chosen project", async () => {
    reserve.mockResolvedValue({});
    wrap(<StockReserveModal balance={balance} onClose={vi.fn()} />);
    const dialog = await screen.findByRole("dialog");

    fireEvent.click(within(dialog).getByRole("button", { name: r.confirm }));
    expect(await within(dialog).findByText(r.invalid)).toBeDefined();
    expect(reserve).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(r.project)), { target: { value: "p-1" } });
    fireEvent.change(within(dialog).getByLabelText(new RegExp(r.quantity)), { target: { value: "4" } });
    fireEvent.click(within(dialog).getByRole("button", { name: r.confirm }));

    await waitFor(() => expect(reserve).toHaveBeenCalledTimes(1));
    expect(reserve.mock.calls[0][0].payload).toEqual({ warehouseId: "w-1", itemId: "item-1", projectId: "p-1", quantity: 4, note: null });
  });

  it("puts a goods receipt into the chosen warehouse and reports a duplicate posting", async () => {
    receive.mockRejectedValue(new ApiError({ status: 409, code: "INVENTORY_RECEIPT_ALREADY_POSTED", message: "x" }));
    wrap(<PutIntoStockButton goodsReceiptId="gr-1" />);

    fireEvent.click(screen.getByRole("button", { name: rc.action }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.click(within(dialog).getByRole("button", { name: rc.confirm }));
    expect(await within(dialog).findByText(rc.warehouseRequired)).toBeDefined();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(rc.warehouse)), { target: { value: "w-2" } });
    fireEvent.click(within(dialog).getByRole("button", { name: rc.confirm }));

    await waitFor(() => expect(receive).toHaveBeenCalledTimes(1));
    expect(receive.mock.calls[0][0]).toMatchObject({ goodsReceiptId: "gr-1", warehouseId: "w-2" });
    expect(await within(dialog).findByText(thMessages.inventory.errors.INVENTORY_RECEIPT_ALREADY_POSTED)).toBeDefined();
  });
});

describe("StockBalanceList", () => {
  const balance = {
    id: "b-1",
    warehouse: { id: "w-1", code: "WH-1", name: "คลังหลัก" },
    item: { id: "item-1", code: "PLY", nameTh: "ไม้อัด", unitCode: "แผ่น" },
    onHand: 10, reserved: 4, available: 6, averageCost: 1250, totalValue: 12500,
  } as StockBalanceResponse;

  beforeEach(() => {
    vi.clearAllMocks();
    mockHooks();
    vi.spyOn(inventoryQueries, "useStockBalances").mockReturnValue({
      data: { items: [balance], pagination: { page: 1, pageSize: 25, totalCount: 1, totalPages: 1 }, totalValue: 12500 },
      isLoading: false, isError: false, error: null, refetch: vi.fn(),
    } as unknown as ReturnType<typeof inventoryQueries.useStockBalances>);
  });

  it("shows only the actions the user may perform, and reserve only with available stock", () => {
    permissions.granted = ["inventory.read", "inventory.issue", "inventory.reserve"];
    wrap(<StockBalanceList />);

    expect(screen.getByRole("button", { name: st.issue })).toBeDefined();
    expect(screen.queryByRole("button", { name: st.transfer })).toBeNull();
    expect(screen.queryByRole("button", { name: st.adjust })).toBeNull();
    expect(screen.getByRole("button", { name: st.reserve })).toBeDefined();
    const table = within(screen.getByRole("table"));
    expect(table.getByText("PLY · ไม้อัด")).toBeDefined();
  });

  it("hides every action for a read-only user", () => {
    permissions.granted = ["inventory.read"];
    wrap(<StockBalanceList />);

    expect(screen.queryByRole("button", { name: st.issue })).toBeNull();
    expect(screen.queryByRole("button", { name: st.reserve })).toBeNull();
  });
});
