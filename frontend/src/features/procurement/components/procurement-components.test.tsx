import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { PurchaseOrderDetail } from "./purchase-order-detail";
import { SupplierList } from "./supplier-list";
import { PurchaseRequestList } from "./purchase-request-list";
import { SupplierInvoiceDetail } from "./supplier-invoice-detail";
import * as procurementQueries from "../api/procurement-queries";
import { ApiError } from "@/lib/api/api-error";
import type {
  PurchaseOrderResponse,
  PurchaseRequestListItemResponse,
  SupplierInvoiceResponse,
  SupplierResponse,
} from "@/lib/api/api-client";
import {
  PURCHASE_ORDER_STATUSES,
  PURCHASE_REQUEST_STATUSES,
  SUPPLIER_INVOICE_STATUSES,
  SUPPLIER_INVOICE_MATCH_STATUSES,
  procurementErrorCode,
  purchaseOrderStatusVariant,
  purchaseRequestStatusVariant,
  supplierInvoiceStatusVariant,
  supplierInvoiceMatchStatusVariant,
} from "../procurement-status";

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
const reqT = thMessages.procurement.requests;
const invT = thMessages.procurement.invoices;
const ALL = [
  "purchase-orders.create",
  "purchase-orders.approve",
  "goods-receipts.create",
  "suppliers.manage",
  "suppliers.read",
  "purchase-orders.read",
];

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
      { id: "l-1", lineNo: 1, itemId: "i-1", itemCode: "PLY", itemNameTh: "ไม้อัด", unitId: "u", unitCode: "แผ่น", quantity: 10, unitPrice: 1250, lineTotal: 12500, receivedQuantity: 6, remainingQuantity: 4, invoicedQuantity: 0, uninvoicedReceivedQuantity: 6 },
      { id: "l-2", lineNo: 2, itemId: "i-2", itemCode: "LAM", itemNameTh: "ลามิเนต", unitId: "u", unitCode: "แผ่น", quantity: 4, unitPrice: 625, lineTotal: 2500, receivedQuantity: 0, remainingQuantity: 4, invoicedQuantity: 0, uninvoicedReceivedQuantity: 0 },
    ],
    receipts: [],
    returns: [],
    invoices: [],
    sourcePurchaseRequests: [],
    ...overrides,
  } as PurchaseOrderResponse;
}

const act = vi.fn();
const receive = vi.fn();
const returnGoods = vi.fn();

function mockOrder(data: PurchaseOrderResponse) {
  vi.spyOn(procurementQueries, "usePurchaseOrder").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof procurementQueries.usePurchaseOrder>);
  vi.spyOn(procurementQueries, "usePurchaseOrderMutations").mockReturnValue({
    act: { mutateAsync: act, isPending: false },
    receive: { mutateAsync: receive, isPending: false },
    returnGoods: { mutateAsync: returnGoods, isPending: false },
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

describe("procurement status helpers and translations", () => {
  it("map purchase order statuses to badge variants", () => {
    expect(purchaseOrderStatusVariant("approved")).toBe("success");
    expect(purchaseOrderStatusVariant("partially_received")).toBe("info");
    expect(purchaseOrderStatusVariant("submitted")).toBe("warning");
    expect(purchaseOrderStatusVariant("cancelled")).toBe("danger");
    expect(purchaseOrderStatusVariant("draft")).toBe("neutral");
  });

  it("map purchase request and supplier invoice statuses to badge variants", () => {
    expect(purchaseRequestStatusVariant("approved")).toBe("success");
    expect(purchaseRequestStatusVariant("converted")).toBe("info");
    expect(purchaseRequestStatusVariant("submitted")).toBe("warning");
    expect(purchaseRequestStatusVariant("rejected")).toBe("danger");
    expect(purchaseRequestStatusVariant("draft")).toBe("neutral");

    expect(supplierInvoiceStatusVariant("matched")).toBe("success");
    expect(supplierInvoiceStatusVariant("approved")).toBe("success");
    expect(supplierInvoiceStatusVariant("on_hold")).toBe("warning");
    expect(supplierInvoiceStatusVariant("rejected")).toBe("danger");
    expect(supplierInvoiceStatusVariant("voided")).toBe("danger");
    expect(supplierInvoiceStatusVariant("draft")).toBe("neutral");

    expect(supplierInvoiceMatchStatusVariant("matched")).toBe("success");
    expect(supplierInvoiceMatchStatusVariant("price_variance")).toBe("warning");
    expect(supplierInvoiceMatchStatusVariant("pending")).toBe("neutral");
  });

  it("recognise only known backend error codes", () => {
    expect(procurementErrorCode("PURCHASE_ORDER_OVER_BUDGET")).toBe("PURCHASE_ORDER_OVER_BUDGET");
    expect(procurementErrorCode("PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE")).toBe("PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE");
    expect(procurementErrorCode("SUPPLIER_INVOICE_OVER_RECEIVED")).toBe("SUPPLIER_INVOICE_OVER_RECEIVED");
    expect(procurementErrorCode("SUPPLIER_RETURN_OVER_RETURNED")).toBe("SUPPLIER_RETURN_OVER_RETURNED");
    expect(procurementErrorCode("SOMETHING_ELSE")).toBeNull();
  });

  it("have Thai and English text for every status and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of PURCHASE_ORDER_STATUSES) expect(messages.procurement.orders.statuses[status]).toBeTruthy();
      for (const status of PURCHASE_REQUEST_STATUSES) expect(messages.procurement.requests.statuses[status]).toBeTruthy();
      for (const status of SUPPLIER_INVOICE_STATUSES) expect(messages.procurement.invoices.statuses[status]).toBeTruthy();
      for (const status of SUPPLIER_INVOICE_MATCH_STATUSES) expect(messages.procurement.invoices.matchStatuses[status]).toBeTruthy();

      for (const code of [
        "SUPPLIER_CODE_CONFLICT",
        "PURCHASE_ORDER_OVER_BUDGET",
        "PURCHASE_ORDER_SELF_APPROVAL",
        "GOODS_RECEIPT_OVER_RECEIVED",
        "PURCHASE_ORDER_HAS_RECEIPTS",
        "PURCHASE_REQUEST_VERSION_CONFLICT",
        "PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE",
        "SUPPLIER_RETURN_INVALID",
        "SUPPLIER_RETURN_OVER_RETURNED",
        "SUPPLIER_INVOICE_DUPLICATE_NUMBER",
        "SUPPLIER_INVOICE_OVER_RECEIVED",
      ] as const) {
        expect(messages.procurement.errors[code]).toBeTruthy();
      }
      expect(messages.shell.purchaseOrders).toBeTruthy();
      expect(messages.shell.purchaseRequests).toBeTruthy();
      expect(messages.shell.supplierInvoices).toBeTruthy();
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

  it("renders returns and posts a supplier return", async () => {
    returnGoods.mockResolvedValue(order());
    const orderWithReturn = order({
      receipts: [
        {
          id: "gr-1",
          number: "GR-0001",
          receivedAtUtc: "2026-10-29T10:00:00Z",
          totalQuantity: 6,
          lineCount: 1,
          lines: [
            {
              id: "grl-1",
              purchaseOrderLineId: "l-1",
              itemCode: "PLY",
              itemNameTh: "ไม้อัด",
              unitCode: "แผ่น",
              quantity: 6,
              unitPrice: 1250,
              returnedQuantity: 0,
              remainingReturnableQuantity: 6,
            },
          ],
        },
      ],
      returns: [
        {
          id: "srn-1",
          number: "SRN-2026-0001",
          goodsReceiptId: "gr-1",
          goodsReceiptNumber: "GR-0001",
          returnedAtUtc: "2026-10-29T11:00:00Z",
          reason: "สินค้าชำรุด",
          totalReturnAmount: 2500,
          returnedBy: { id: "u-1", displayName: "ผู้ตรวจสอบ" },
          lines: [],
        },
      ],
    });

    renderDetail(orderWithReturn);

    // Shows existing return
    expect(screen.getByText("SRN-2026-0001")).toBeDefined();
    expect(screen.getByText(new RegExp("สินค้าชำรุด"))).toBeDefined();

    // Click record return
    const recordReturnBtn = screen.getByRole("button", { name: o.recordReturn });
    fireEvent.click(recordReturnBtn);

    const dialog = await screen.findByRole("dialog");
    const reasonInput = within(dialog).getByLabelText(new RegExp(o.returnReason));
    fireEvent.change(reasonInput, { target: { value: "แผ่นไม้หัก" } });

    const returnQtyInput = within(dialog).getByLabelText(o.returnQuantityFor.replace("{item}", "PLY"));
    fireEvent.change(returnQtyInput, { target: { value: "2" } });

    const submitBtn = within(dialog).getByRole("button", { name: thMessages.common.actions.save });
    fireEvent.click(submitBtn);

    await waitFor(() => expect(returnGoods).toHaveBeenCalledTimes(1));
    const call = returnGoods.mock.calls[0][0];
    expect(call.payload).toMatchObject({
      goodsReceiptId: "gr-1",
      reason: "แผ่นไม้หัก",
      lines: [{ goodsReceiptLineId: "grl-1", quantity: 2 }],
    });
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

describe("PurchaseRequestList & Conversion", () => {
  const convertMutate = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    permissions.granted = ALL;
    vi.spyOn(procurementQueries, "usePurchaseRequestList").mockReturnValue({
      data: {
        items: [
          {
            id: "pr-1",
            number: "PR-2026-0001",
            status: "approved",
            totalEstimatedAmount: 12000,
            projectCode: "PRJ-01",
            createdAtUtc: "2026-10-29T10:00:00Z",
          },
          {
            id: "pr-2",
            number: "PR-2026-0002",
            status: "draft",
            totalEstimatedAmount: 5000,
            projectCode: "PRJ-01",
            createdAtUtc: "2026-10-29T11:00:00Z",
          },
        ] as PurchaseRequestListItemResponse[],
        pagination: { page: 1, pageSize: 25, totalCount: 2, totalPages: 1 },
      },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof procurementQueries.usePurchaseRequestList>);

    vi.spyOn(procurementQueries, "usePurchaseRequestMutations").mockReturnValue({
      create: { mutateAsync: vi.fn(), isPending: false },
      update: { mutateAsync: vi.fn(), isPending: false },
      act: { mutateAsync: vi.fn(), isPending: false },
      convert: { mutateAsync: convertMutate, isPending: false },
    } as unknown as ReturnType<typeof procurementQueries.usePurchaseRequestMutations>);

    vi.spyOn(procurementQueries, "useSupplierList").mockReturnValue({
      data: {
        items: [
          { id: "s-1", code: "SUP-001", nameTh: "ซัพพลายเออร์ เอ" },
        ] as SupplierResponse[],
        pagination: { page: 1, pageSize: 100, totalCount: 1, totalPages: 1 },
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof procurementQueries.useSupplierList>);
  });

  it("allows selecting approved PR and converting to PO", async () => {
    convertMutate.mockResolvedValue({ id: "po-new-1", number: "PO-2026-9999" });

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <PurchaseRequestList />
      </NextIntlClientProvider>
    );

    // Only approved row has a checkbox (plus header checkbox)
    const checkboxes = screen.getAllByRole("checkbox");
    expect(checkboxes.length).toBe(2);

    // Select row checkbox
    fireEvent.click(checkboxes[1]);

    // Convert button appears
    const convertBtn = screen.getByRole("button", { name: new RegExp(reqT.convertTitle.replace(/[()]/g, "\\$&")) });
    expect(convertBtn).toBeDefined();

    fireEvent.click(convertBtn);

    // Modal opens
    const dialog = await screen.findByRole("dialog");
    const supplierSelect = within(dialog).getByLabelText(new RegExp(reqT.supplier));
    fireEvent.change(supplierSelect, { target: { value: "s-1" } });

    const submitBtn = within(dialog).getByRole("button", { name: reqT.convertSubmit });
    fireEvent.click(submitBtn);

    await waitFor(() => expect(convertMutate).toHaveBeenCalledTimes(1));
    expect(convertMutate.mock.calls[0][0].payload).toMatchObject({
      purchaseRequestIds: ["pr-1"],
      supplierId: "s-1",
    });
  });
});

describe("SupplierInvoiceDetail & 3-Way Match", () => {
  const invoiceAct = vi.fn();

  const mockInvoiceData: SupplierInvoiceResponse = {
    id: "sinv-1",
    number: "SINV-2026-0001",
    supplierInvoiceNumber: "INV-SUP-999",
    status: "on_hold",
    matchStatus: "price_variance",
    subtotalAmount: 13500,
    taxAmount: 945,
    totalAmount: 14445,
    totalPoAmount: 12500,
    totalVarianceAmount: 1000,
    rowVersion: "inv-v1",
    purchaseOrder: { id: "po-1", number: "PO-2026-0001" },
    supplier: { id: "s-1", code: "SUP-001", nameTh: "บริษัท ไม้ดี" },
    lines: [
      {
        id: "sil-1",
        lineNo: 1,
        purchaseOrderLineId: "l-1",
        itemCode: "PLY",
        itemNameTh: "ไม้อัด",
        quantity: 10,
        poUnitPrice: 1250,
        unitPrice: 1350,
        lineTotal: 13500,
        priceVarianceAmount: 1000,
        priceVariancePercent: 8.0,
        exceedsTolerance: true,
      },
    ],
  } as SupplierInvoiceResponse;

  beforeEach(() => {
    vi.clearAllMocks();
    permissions.granted = ALL;
    vi.spyOn(procurementQueries, "useSupplierInvoice").mockReturnValue({
      data: mockInvoiceData,
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof procurementQueries.useSupplierInvoice>);

    vi.spyOn(procurementQueries, "useSupplierInvoiceMutations").mockReturnValue({
      create: { mutateAsync: vi.fn(), isPending: false },
      update: { mutateAsync: vi.fn(), isPending: false },
      act: { mutateAsync: invoiceAct, isPending: false },
    } as unknown as ReturnType<typeof procurementQueries.useSupplierInvoiceMutations>);
  });

  it("shows on_hold warning banner and allows approver to release hold", async () => {
    invoiceAct.mockResolvedValue({ ...mockInvoiceData, status: "approved" });

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <SupplierInvoiceDetail supplierInvoiceId="sinv-1" />
      </NextIntlClientProvider>
    );

    // Warning banner for on_hold
    expect(screen.getByText(invT.onHoldBannerTitle)).toBeDefined();
    expect(screen.getByText(invT.exceedsTolerance)).toBeDefined();

    // Approver clicks approve to release hold
    const approveBtn = screen.getByRole("button", { name: invT.approve });
    fireEvent.click(approveBtn);

    await waitFor(() =>
      expect(invoiceAct).toHaveBeenCalledWith({
        id: "sinv-1",
        rowVersion: "inv-v1",
        action: "approve",
        note: null,
      })
    );
  });

  it("allows creator to void an approved or matched invoice with a reason", async () => {
    vi.spyOn(procurementQueries, "useSupplierInvoice").mockReturnValue({
      data: { ...mockInvoiceData, status: "approved" },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof procurementQueries.useSupplierInvoice>);

    invoiceAct.mockResolvedValue({ ...mockInvoiceData, status: "voided" });

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <SupplierInvoiceDetail supplierInvoiceId="sinv-1" />
      </NextIntlClientProvider>
    );

    const voidBtn = screen.getByRole("button", { name: invT.voidAction });
    fireEvent.click(voidBtn);

    const dialog = await screen.findByRole("dialog");
    const reasonInput = within(dialog).getByLabelText(new RegExp(invT.reason));
    fireEvent.change(reasonInput, { target: { value: "เอกสารออกผิดพลาด" } });

    const confirmBtn = within(dialog).getByRole("button", { name: invT.confirm });
    fireEvent.click(confirmBtn);

    await waitFor(() =>
      expect(invoiceAct).toHaveBeenCalledWith({
        id: "sinv-1",
        rowVersion: "inv-v1",
        action: "void",
        note: "เอกสารออกผิดพลาด",
      })
    );
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

  it("validates phone, email, and taxId format when provided", async () => {
    create.mockResolvedValue(supplier);
    renderList();

    fireEvent.click(screen.getAllByRole("button", { name: s.create })[0]);
    const dialog = await screen.findByRole("dialog");
    const nameInput = within(dialog).getByLabelText(new RegExp(s.nameTh.replace(/[()]/g, "\\$&")));
    const saveBtn = within(dialog).getByRole("button", { name: thMessages.common.actions.save });

    fireEvent.change(nameInput, { target: { value: "บริษัท ทดสอบ" } });

    // Invalid Tax ID (not 13 digits)
    const taxInput = within(dialog).getByLabelText(new RegExp(s.taxId.replace(/[()]/g, "\\$&")));
    fireEvent.change(taxInput, { target: { value: "12345" } });
    fireEvent.click(saveBtn);
    expect(await within(dialog).findByText(s.invalidTaxId)).toBeDefined();
    expect(create).not.toHaveBeenCalled();

    // Valid Tax ID, invalid Phone
    fireEvent.change(taxInput, { target: { value: "0105550000000" } });
    const phoneInput = within(dialog).getByPlaceholderText(s.phonePlaceholder);
    fireEvent.change(phoneInput, { target: { value: "12345" } });
    fireEvent.click(saveBtn);
    expect(await within(dialog).findByText(s.invalidPhone)).toBeDefined();
    expect(create).not.toHaveBeenCalled();

    // Valid Phone, invalid Email
    fireEvent.change(phoneInput, { target: { value: "081-234-5678" } });
    const emailInput = within(dialog).getByLabelText(new RegExp(s.email.replace(/[()]/g, "\\$&")));
    fireEvent.change(emailInput, { target: { value: "not-an-email" } });
    fireEvent.click(saveBtn);
    expect(await within(dialog).findByText(s.invalidEmail)).toBeDefined();
    expect(create).not.toHaveBeenCalled();

    // Valid Email -> succeeds
    fireEvent.change(emailInput, { target: { value: "test@example.com" } });
    fireEvent.click(saveBtn);
    await waitFor(() => expect(create).toHaveBeenCalledTimes(1));
    expect(create.mock.calls[0][0].payload).toMatchObject({
      nameTh: "บริษัท ทดสอบ",
      taxId: "0105550000000",
      phone: "081-234-5678",
      email: "test@example.com",
    });
  });

  it("hides management actions without the manage permission", () => {
    permissions.granted = ["suppliers.read"];
    renderList();

    expect(screen.queryByRole("button", { name: s.create })).toBeNull();
    expect(screen.queryByRole("button", { name: s.edit })).toBeNull();
  });
});
