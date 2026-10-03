import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { PurchaseOrderDetail } from "./purchase-order-detail";
import { SupplierList } from "./supplier-list";
import * as procurementQueries from "../api/procurement-queries";
import { ApiError } from "@/lib/api/api-error";
import type { PurchaseOrderResponse, SupplierResponse } from "@/lib/api/api-client";
import { PURCHASE_ORDER_STATUSES, procurementErrorCode, purchaseOrderStatusVariant } from "../procurement-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/procurement",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const o = thMessages.procurement.orders;
const s = thMessages.procurement.suppliers;
const ALL = ["purchase-orders.create", "purchase-orders.approve", "goods-receipts.create", "suppliers.manage", "suppliers.read", "purchase-orders.read"];

function order(overrides: Partial<PurchaseOrderResponse> = {}): PurchaseOrderResponse {
  return {
    id: "po-1",
    number: "PO-2026-0001",
    status: "approved",
    totalAmount: 15000,
    rowVersion: "00000000-0000-0000-0000-0000000000a1",
    supplier: { id: "s-1", code: "SUP-00001", nameTh: "บริษัท ไม้ดี" },
    createdBy: { id: "u-1", displayName: "ผู้สร้าง" },
    lines: [
      { id: "l-1", lineNo: 1, itemId: "i-1", itemCode: "PLY", itemNameTh: "ไม้อัด", unitId: "u", unitCode: "แผ่น", quantity: 10, unitPrice: 1250, lineTotal: 12500, receivedQuantity: 6, remainingQuantity: 4 },
      { id: "l-2", lineNo: 2, itemId: "i-2", itemCode: "LAM", itemNameTh: "ลามิเนต", unitId: "u", unitCode: "แผ่น", quantity: 4, unitPrice: 625, lineTotal: 2500, receivedQuantity: 0, remainingQuantity: 4 },
    ],
    receipts: [],
    ...overrides,
  } as PurchaseOrderResponse;
}

const act = vi.fn();
const receive = vi.fn();

function mockOrder(data: PurchaseOrderResponse) {
  vi.spyOn(procurementQueries, "usePurchaseOrder").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof procurementQueries.usePurchaseOrder>);
  vi.spyOn(procurementQueries, "usePurchaseOrderMutations").mockReturnValue({
    act: { mutateAsync: act, isPending: false },
    receive: { mutateAsync: receive, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
    update: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof procurementQueries.usePurchaseOrderMutations>);
}

function renderDetail(data: PurchaseOrderResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  mockOrder(data);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <PurchaseOrderDetail purchaseOrderId="po-1" />
    </NextIntlClientProvider>
  );
}

describe("purchase order status helpers and translations", () => {
  it("map statuses to badge variants", () => {
    expect(purchaseOrderStatusVariant("approved")).toBe("success");
    expect(purchaseOrderStatusVariant("partially_received")).toBe("info");
    expect(purchaseOrderStatusVariant("submitted")).toBe("warning");
    expect(purchaseOrderStatusVariant("cancelled")).toBe("danger");
    expect(purchaseOrderStatusVariant("draft")).toBe("neutral");
  });

  it("recognise only known backend error codes", () => {
    expect(procurementErrorCode("PURCHASE_ORDER_OVER_BUDGET")).toBe("PURCHASE_ORDER_OVER_BUDGET");
    expect(procurementErrorCode("SOMETHING_ELSE")).toBeNull();
  });

  it("have Thai and English text for every status and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of PURCHASE_ORDER_STATUSES) expect(messages.procurement.orders.statuses[status]).toBeTruthy();
      for (const code of ["SUPPLIER_CODE_CONFLICT", "PURCHASE_ORDER_OVER_BUDGET", "PURCHASE_ORDER_SELF_APPROVAL", "GOODS_RECEIPT_OVER_RECEIVED", "PURCHASE_ORDER_HAS_RECEIPTS"] as const) {
        expect(messages.procurement.errors[code]).toBeTruthy();
      }
      expect(messages.shell.purchaseOrders).toBeTruthy();
      expect(messages.shell.suppliers).toBeTruthy();
    }
  });
});

describe("PurchaseOrderDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows approve/reject only for a submitted order to an approver", () => {
    renderDetail(order({ status: "submitted" }));
    expect(screen.getByRole("button", { name: o.approve })).toBeDefined();
    expect(screen.getByRole("button", { name: o.reject })).toBeDefined();
  });

  it("hides approval from a user without the approve permission", () => {
    renderDetail(order({ status: "submitted" }), ["purchase-orders.create"]);
    expect(screen.queryByRole("button", { name: o.approve })).toBeNull();
  });

  it("offers edit and submit for a draft, sending the row version", async () => {
    act.mockResolvedValue(order({ status: "submitted" }));
    renderDetail(order({ status: "draft" }));

    expect(screen.getByRole("link", { name: o.editDraft }).getAttribute("href")).toBe("/th/procurement/purchase-orders/po-1/edit");
    fireEvent.click(screen.getByRole("button", { name: o.submit }));

    await waitFor(() => expect(act).toHaveBeenCalledWith({ id: "po-1", rowVersion: "00000000-0000-0000-0000-0000000000a1", action: "submit", note: null }));
  });

  it("requires a reason to reject", async () => {
    act.mockResolvedValue(order({ status: "rejected" }));
    renderDetail(order({ status: "submitted" }));

    fireEvent.click(screen.getByRole("button", { name: o.reject }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.click(within(dialog).getByRole("button", { name: o.confirm }));
    expect(await within(dialog).findByText(o.reasonRequired)).toBeDefined();
    expect(act).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(o.reason)), { target: { value: "ราคาสูง" } });
    fireEvent.click(within(dialog).getByRole("button", { name: o.confirm }));

    await waitFor(() => expect(act).toHaveBeenCalledWith({ id: "po-1", rowVersion: "00000000-0000-0000-0000-0000000000a1", action: "reject", note: "ราคาสูง" }));
  });

  it("posts a goods receipt with only the entered lines and one idempotency key", async () => {
    receive.mockResolvedValue(order());
    renderDetail(order());

    fireEvent.change(screen.getByLabelText(o.receiveQuantityFor.replace("{item}", "PLY")), { target: { value: "3" } });
    fireEvent.click(screen.getByRole("button", { name: o.postReceipt }));

    await waitFor(() => expect(receive).toHaveBeenCalledTimes(1));
    const call = receive.mock.calls[0][0];
    expect(call.id).toBe("po-1");
    expect(call.payload.lines).toEqual([{ purchaseOrderLineId: "l-1", quantity: 3 }]);
    expect(typeof call.idempotencyKey).toBe("string");
    await waitFor(() => expect(toastMocks.success).toHaveBeenCalledWith(o.receiptPosted));
  });

  it("refuses an empty receipt and maps an over-receipt error", async () => {
    renderDetail(order());
    fireEvent.click(screen.getByRole("button", { name: o.postReceipt }));
    expect(await screen.findByText(o.receiptInvalid)).toBeDefined();
    expect(receive).not.toHaveBeenCalled();

    receive.mockRejectedValue(new ApiError({ status: 422, code: "GOODS_RECEIPT_OVER_RECEIVED", message: "x" }));
    fireEvent.change(screen.getByLabelText(o.receiveQuantityFor.replace("{item}", "LAM")), { target: { value: "9" } });
    fireEvent.click(screen.getByRole("button", { name: o.postReceipt }));
    expect(await screen.findByText(thMessages.procurement.errors.GOODS_RECEIPT_OVER_RECEIVED)).toBeDefined();
  });

  it("does not offer receiving for a draft order or without the receive permission", () => {
    renderDetail(order({ status: "draft" }));
    expect(screen.queryByRole("button", { name: o.postReceipt })).toBeNull();
  });

  it("hides receiving without the permission", () => {
    renderDetail(order(), ["purchase-orders.read"]);
    expect(screen.queryByRole("button", { name: o.postReceipt })).toBeNull();
  });
});

describe("SupplierList", () => {
  const create = vi.fn();
  const supplier: SupplierResponse = { id: "s-1", code: "SUP-00001", nameTh: "บริษัท ไม้ดี", paymentTermDays: 30, status: "active", rowVersion: "v1" } as SupplierResponse;

  beforeEach(() => {
    vi.clearAllMocks();
    permissions.granted = ALL;
    vi.spyOn(procurementQueries, "useSupplierList").mockReturnValue({
      data: { items: [supplier], pagination: { page: 1, pageSize: 25, totalCount: 1, totalPages: 1 } },
      isLoading: false, isError: false, error: null, refetch: vi.fn(),
    } as unknown as ReturnType<typeof procurementQueries.useSupplierList>);
    vi.spyOn(procurementQueries, "useSupplierMutations").mockReturnValue({
      create: { mutateAsync: create, isPending: false },
      update: { mutateAsync: vi.fn(), isPending: false },
      setActive: { mutateAsync: vi.fn(), isPending: false },
    } as unknown as ReturnType<typeof procurementQueries.useSupplierMutations>);
  });

  function renderList() {
    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <SupplierList />
      </NextIntlClientProvider>
    );
  }

  it("validates the form before creating a supplier", async () => {
    create.mockResolvedValue(supplier);
    renderList();

    fireEvent.click(screen.getAllByRole("button", { name: s.create })[0]);
    const dialog = await screen.findByRole("dialog");
    fireEvent.click(within(dialog).getByRole("button", { name: thMessages.common.actions.save }));
    expect(await within(dialog).findByText(s.invalid)).toBeDefined();
    expect(create).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByLabelText(new RegExp(s.nameTh.replace(/[()]/g, "\\$&"))), { target: { value: "บริษัท ใหม่" } });
    fireEvent.click(within(dialog).getByRole("button", { name: thMessages.common.actions.save }));

    await waitFor(() => expect(create).toHaveBeenCalledTimes(1));
    expect(create.mock.calls[0][0].payload).toMatchObject({ nameTh: "บริษัท ใหม่", paymentTermDays: 30, nameEn: null });
  });

  it("hides management actions without the manage permission", () => {
    permissions.granted = ["suppliers.read"];
    renderList();

    expect(screen.queryByRole("button", { name: s.create })).toBeNull();
    expect(screen.queryByRole("button", { name: s.edit })).toBeNull();
  });
});
