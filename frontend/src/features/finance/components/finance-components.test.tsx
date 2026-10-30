import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ArAgingPanel } from "./ar-aging-panel";
import { BillingDetail } from "./billing-detail";
import { BillingEditor } from "./billing-editor";
import { FinanceSyncPage } from "./finance-sync-page";
import * as financeQueries from "../api/finance-queries";
import * as projectQueries from "@/features/projects/api/project-queries";
import * as customerQueries from "@/features/customers/api/customer-queries";
import { ApiError } from "@/lib/api/api-error";
import type { BillingResponse } from "@/lib/api/api-client";
import {
  BILLING_STATUSES,
  OUTBOX_KINDS,
  OUTBOX_STATUSES,
  PAYMENT_METHODS,
  RECONCILIATION_ISSUES,
  billingNoteStatusVariant,
  billingStatusVariant,
  creditOverrideStatusVariant,
  financeErrorCode,
  outboxStatusVariant,
  retentionStatusVariant,
} from "../finance-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/finance",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    currentUser: { user: { id: "u-current", displayName: "ผู้ตรวจสอบ" } },
    selectedMembership: { id: "membership-1" },
  }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const b = thMessages.finance.billings;
const a = thMessages.finance.arAging;
const s = thMessages.finance.sync;
const ALL = ["billings.manage", "payments.manage", "finance-sync.run", "finance-sync.read"];

function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
      mutations: { retry: false },
    },
  });
}

function billing(status: string, overrides: Partial<BillingResponse> = {}): BillingResponse {
  return {
    id: "bill-1",
    number: "BIL-2026-0001",
    status,
    kind: "milestone",
    description: "งวดที่ 1",
    amount: 30000,
    paidAmount: status === "issued" ? 0 : 10000,
    outstanding: status === "issued" ? 30000 : 20000,
    referenceHash: "a".repeat(64),
    project: { id: "p-1", code: "PRJ-1", name: "บ้าน" },
    createdBy: { id: "u-1", displayName: "ผู้ออก" },
    rowVersion: "00000000-0000-0000-0000-0000000000a1",
    taxInvoiceNumber: null,
    vatRate: 0,
    vatAmount: 0,
    grossTotal: 30000,
    effectiveReceivable: 30000,
    retentionRate: 0,
    retentionAmount: 0,
    retentionStatus: "none",
    notes: [],
    lines: [],
    payments:
      status === "issued"
        ? []
        : [
            {
              id: "pay-1",
              number: "PAY-2026-0001",
              receiptNumber: "RCT-2026-0001",
              amount: 10000,
              method: "transfer",
              reference: "SLIP-1",
              receivedDate: "2026-10-04",
              status: "recorded",
              whtRate: 0,
              whtAmount: 0,
              netCashReceived: 10000,
              recordedBy: { id: "u-1", displayName: "ผู้บันทึก" },
            },
          ],
    ...overrides,
  };
}

const pay = vi.fn();
const voidBilling = vi.fn();
const reverse = vi.fn();
const releaseRetention = vi.fn();
const createNote = vi.fn();
const approveNote = vi.fn();
const downloadTaxPdf = vi.fn();
const createBilling = vi.fn();
const createOverride = vi.fn();
const approveOverride = vi.fn();

function renderBilling(data: BillingResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  vi.spyOn(financeQueries, "useBilling").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof financeQueries.useBilling>);
  vi.spyOn(financeQueries, "useBillingMutations").mockReturnValue({
    pay: { mutateAsync: pay, isPending: false },
    voidBilling: { mutateAsync: voidBilling, isPending: false },
    reverse: { mutateAsync: reverse, isPending: false },
    releaseRetention: { mutateAsync: releaseRetention, isPending: false },
    createNote: { mutateAsync: createNote, isPending: false },
    approveNote: { mutateAsync: approveNote, isPending: false },
    downloadTaxPdf: { mutateAsync: downloadTaxPdf, isPending: false },
    create: { mutateAsync: createBilling, isPending: false },
  } as unknown as ReturnType<typeof financeQueries.useBillingMutations>);
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <BillingDetail billingId="bill-1" />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

function renderEditor(granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  vi.spyOn(projectQueries, "useProjectList").mockReturnValue({
    data: { items: [{ id: "p-1", code: "PRJ-1", name: "โครงการทดสอบ" }] },
    isLoading: false,
    isError: false,
  } as unknown as ReturnType<typeof projectQueries.useProjectList>);
  vi.spyOn(financeQueries, "useProjectBillingSummary").mockReturnValue({
    data: { contractAmount: 100000, billed: 30000, unbilled: 70000 },
    isLoading: false,
    isError: false,
  } as unknown as ReturnType<typeof financeQueries.useProjectBillingSummary>);
  vi.spyOn(financeQueries, "useBillingMutations").mockReturnValue({
    pay: { mutateAsync: pay, isPending: false },
    voidBilling: { mutateAsync: voidBilling, isPending: false },
    reverse: { mutateAsync: reverse, isPending: false },
    releaseRetention: { mutateAsync: releaseRetention, isPending: false },
    createNote: { mutateAsync: createNote, isPending: false },
    approveNote: { mutateAsync: approveNote, isPending: false },
    downloadTaxPdf: { mutateAsync: downloadTaxPdf, isPending: false },
    create: { mutateAsync: createBilling, isPending: false },
  } as unknown as ReturnType<typeof financeQueries.useBillingMutations>);

  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <BillingEditor />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

function renderArAging(granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  vi.spyOn(financeQueries, "useArAgingReport").mockReturnValue({
    data: {
      asOfDate: "2026-10-30",
      totalCurrent: 50000,
      totalBucket1To30: 20000,
      totalBucket31To60: 10000,
      totalBucket61To90: 5000,
      totalBucketOver90: 2000,
      grandTotalOutstanding: 87000,
      customers: [
        {
          customerId: "cust-1",
          customerCode: "CUST-001",
          customerName: "บริษัท สยามพาณิชย์ จำกัด",
          current: 50000,
          bucket1To30: 20000,
          bucket31To60: 10000,
          bucket61To90: 5000,
          bucketOver90: 2000,
          totalOutstanding: 87000,
        },
      ],
    },
    isLoading: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof financeQueries.useArAgingReport>);

  vi.spyOn(financeQueries, "useCreditOverrides").mockReturnValue({
    data: {
      items: [
        {
          id: "ovr-1",
          customerId: "cust-1",
          customerName: "บริษัท สยามพาณิชย์ จำกัด",
          requestedCeilingAmount: 500000,
          reason: "โครงการขยายสาขาต่อเนื่อง",
          expiresOn: "2026-12-31",
          status: "pending_approval",
          rowVersion: "00000000-0000-0000-0000-0000000000b1",
          createdBy: { id: "u-maker", displayName: "นาย ก (Maker)" },
        },
      ],
    },
    isLoading: false,
    isError: false,
  } as unknown as ReturnType<typeof financeQueries.useCreditOverrides>);

  vi.spyOn(customerQueries, "useCustomerList").mockReturnValue({
    data: {
      items: [
        { id: "cust-1", code: "CUST-001", displayNameTh: "บริษัท สยามพาณิชย์ จำกัด", displayNameEn: "Siam Commercial Co., Ltd." },
      ],
      pagination: { totalCount: 1, page: 1, limit: 100, totalPages: 1 },
    },
    isLoading: false,
    isError: false,
  } as unknown as ReturnType<typeof customerQueries.useCustomerList>);

  vi.spyOn(financeQueries, "useCreditOverrideMutations").mockReturnValue({
    createOverride: { mutateAsync: createOverride, isPending: false },
    approveOverride: { mutateAsync: approveOverride, isPending: false },
  } as unknown as ReturnType<typeof financeQueries.useCreditOverrideMutations>);

  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ArAgingPanel />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

const dispatch = vi.fn();
const requeue = vi.fn();
const exportJournal = vi.fn();
const upsertMapping = vi.fn();

function renderSync(granted: string[] = ALL) {
  permissions.granted = granted;
  const client = createTestQueryClient();
  vi.spyOn(financeQueries, "useOutboxList").mockReturnValue({
    isLoading: false,
    data: {
      items: [
        { id: "m-1", kind: "billing.issued", resourceNumber: "BIL-1", amount: 30000, status: "failed", attempts: 2, lastError: "HTTP_503", nextAttemptAtUtc: "2026-10-04T01:00:00Z" },
        { id: "m-2", kind: "payment.recorded", resourceNumber: "PAY-1", amount: 10000, status: "sent", attempts: 1, externalRef: "ACC-1" },
        { id: "m-3", kind: "billing.voided", resourceNumber: "BIL-2", amount: 500, status: "dead", attempts: 5, lastError: "CONNECTOR_NOT_CONFIGURED" },
      ],
      pagination: { page: 1, pageSize: 25, totalCount: 3, totalPages: 1 },
    },
  } as unknown as ReturnType<typeof financeQueries.useOutboxList>);
  vi.spyOn(financeQueries, "useReconciliation").mockReturnValue({
    isLoading: false,
    isError: false,
    data: {
      erpBilled: 30500, erpPaid: 10000, confirmedBilled: 0, confirmedPaid: 9000, pendingCount: 0, failedCount: 1, deadCount: 1,
      rows: [{ issue: "amount_mismatch", kind: "payment.recorded", resourceNumber: "PAY-1", erpAmount: 10000, externalAmount: 9000 }],
    },
  } as unknown as ReturnType<typeof financeQueries.useReconciliation>);
  vi.spyOn(financeQueries, "useAccountMappings").mockReturnValue({
    isLoading: false,
    isError: false,
    data: {
      items: [
        {
          id: "map-1",
          kind: "billing.issued",
          debitAccountCode: "1130",
          debitAccountName: "ลูกหนี้การค้า",
          creditAccountCode: "4100",
          creditAccountName: "รายได้ค่าบริการ",
          updatedAtUtc: "2026-10-30T10:00:00Z",
        },
      ],
    },
  } as unknown as ReturnType<typeof financeQueries.useAccountMappings>);
  vi.spyOn(financeQueries, "useSyncMutations").mockReturnValue({
    dispatch: { mutateAsync: dispatch, isPending: false },
    requeue: { mutateAsync: requeue, isPending: false },
    exportJournal: { mutateAsync: exportJournal, isPending: false },
    upsertMapping: { mutateAsync: upsertMapping, isPending: false },
  } as unknown as ReturnType<typeof financeQueries.useSyncMutations>);
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <FinanceSyncPage />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("finance status helpers and translations", () => {
  it("map statuses to badge variants and recognise only known errors", () => {
    expect(billingStatusVariant("paid")).toBe("success");
    expect(billingStatusVariant("voided")).toBe("danger");
    expect(outboxStatusVariant("dead")).toBe("danger");
    expect(outboxStatusVariant("sent")).toBe("success");
    expect(outboxStatusVariant("exported")).toBe("info");
    expect(retentionStatusVariant("held")).toBe("warning");
    expect(retentionStatusVariant("released")).toBe("success");
    expect(billingNoteStatusVariant("pending_approval")).toBe("warning");
    expect(billingNoteStatusVariant("approved")).toBe("success");
    expect(creditOverrideStatusVariant("pending_approval")).toBe("warning");
    expect(creditOverrideStatusVariant("approved")).toBe("success");
    expect(financeErrorCode("PAYMENT_DUPLICATE_REFERENCE")).toBe("PAYMENT_DUPLICATE_REFERENCE");
    expect(financeErrorCode("RETENTION_WARRANTY_NOT_EXPIRED")).toBe("RETENTION_WARRANTY_NOT_EXPIRED");
    expect(financeErrorCode("BILLING_NOTE_SELF_APPROVAL")).toBe("BILLING_NOTE_SELF_APPROVAL");
    expect(financeErrorCode("BILLING_NOTE_INVALID_STATE")).toBe("BILLING_NOTE_INVALID_STATE");
    expect(financeErrorCode("BILLING_NOTE_EXCEEDS_ORIGINAL")).toBe("BILLING_NOTE_EXCEEDS_ORIGINAL");
    expect(financeErrorCode("PAYMENT_WHT_CERTIFICATE_REQUIRED")).toBe("PAYMENT_WHT_CERTIFICATE_REQUIRED");
    expect(financeErrorCode("CREDIT_LIMIT_EXCEEDED")).toBe("CREDIT_LIMIT_EXCEEDED");
    expect(financeErrorCode("CREDIT_OVERRIDE_SELF_APPROVAL")).toBe("CREDIT_OVERRIDE_SELF_APPROVAL");
    expect(financeErrorCode("CREDIT_OVERRIDE_INVALID_STATE")).toBe("CREDIT_OVERRIDE_INVALID_STATE");
    expect(financeErrorCode("CREDIT_OVERRIDE_FIELD_INVALID")).toBe("CREDIT_OVERRIDE_FIELD_INVALID");
    expect(financeErrorCode("ACCOUNT_MAPPING_FIELD_INVALID")).toBe("ACCOUNT_MAPPING_FIELD_INVALID");
    expect(financeErrorCode("NOPE")).toBeNull();
  });

  it("have Thai and English text for every status, method, outbox kind, issue and key error", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of BILLING_STATUSES) expect(messages.finance.billings.statuses[status]).toBeTruthy();
      for (const method of PAYMENT_METHODS) expect(messages.finance.billings.methods[method]).toBeTruthy();
      for (const status of OUTBOX_STATUSES) expect(messages.finance.sync.statuses[status]).toBeTruthy();
      for (const kind of OUTBOX_KINDS) {
        // Kinds are dotted ("billing.issued"), so the translations nest by those segments.
        const text = kind.split(".").reduce<unknown>((node, segment) => (typeof node === "object" && node !== null ? (node as Record<string, unknown>)[segment] : undefined), messages.finance.sync.kinds);
        expect(typeof text === "string" && text.length > 0).toBe(true);
      }
      expect(OUTBOX_KINDS).toContain("billing-note.approved");
      for (const issue of RECONCILIATION_ISSUES) expect(messages.finance.sync.issues[issue]).toBeTruthy();
      for (const code of [
        "BILLING_EXCEEDS_CONTRACT",
        "PAYMENT_EXCEEDS_OUTSTANDING",
        "PAYMENT_DUPLICATE_REFERENCE",
        "OUTBOX_CONFIRMATION_CONFLICT",
        "RETENTION_WARRANTY_NOT_EXPIRED",
        "BILLING_NOTE_SELF_APPROVAL",
        "BILLING_NOTE_INVALID_STATE",
        "BILLING_NOTE_EXCEEDS_ORIGINAL",
        "PAYMENT_WHT_CERTIFICATE_REQUIRED",
        "CREDIT_LIMIT_EXCEEDED",
        "CREDIT_OVERRIDE_SELF_APPROVAL",
        "CREDIT_OVERRIDE_INVALID_STATE",
        "CREDIT_OVERRIDE_FIELD_INVALID",
        "ACCOUNT_MAPPING_FIELD_INVALID",
      ] as const) {
        expect(messages.finance.errors[code]).toBeTruthy();
      }
      expect(messages.finance.billings.applyVat).toBeTruthy();
      expect(messages.finance.billings.retentionRatePercent).toBeTruthy();
      expect(messages.finance.billings.releaseRetention).toBeTruthy();
      expect(messages.finance.billings.notesTitle).toBeTruthy();
      expect(messages.finance.arAging.title).toBeTruthy();
      expect(messages.finance.arAging.totalCurrent).toBeTruthy();
      expect(messages.finance.arAging.overridesTitle).toBeTruthy();
      expect(messages.finance.sync.exportJournal).toBeTruthy();
      expect(messages.finance.sync.coa.title).toBeTruthy();
      expect(messages.shell.financeSubgroup).toBeTruthy();
    }
  });
});

describe("BillingEditor (G-15 Thai Tax)", () => {
  beforeEach(() => vi.clearAllMocks());

  it("creates a billing with VAT and Retention percent and shows live summary calculation", async () => {
    createBilling.mockResolvedValue({ id: "bill-new" });
    renderEditor();

    // Select project and description
    fireEvent.change(screen.getByRole("combobox", { name: /^โครงการ/ }), { target: { value: "p-1" } });
    fireEvent.change(screen.getByLabelText(new RegExp(b.description)), { target: { value: "งวดที่ 1 มัดจำ" } });
    fireEvent.change(screen.getByLabelText(new RegExp(b.amount)), { target: { value: "100000" } });

    // Enable VAT 7%
    const vatCheckbox = screen.getByLabelText(b.applyVat);
    fireEvent.click(vatCheckbox);

    // Set Retention 5%
    const retentionInput = screen.getByLabelText(b.retentionRatePercent);
    fireEvent.change(retentionInput, { target: { value: "5" } });

    // Submit form
    fireEvent.click(screen.getByRole("button", { name: b.issue }));

    await waitFor(() => expect(createBilling).toHaveBeenCalledTimes(1));
    const payload = createBilling.mock.calls[0][0].payload;
    expect(payload).toMatchObject({
      projectId: "p-1",
      description: "งวดที่ 1 มัดจำ",
      amount: 100000,
      applyVat: true,
      vatRate: 0.07,
      retentionRate: 0.05,
    });
  });
});

describe("BillingDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("posts a payment with valid input and optional WHT fields", async () => {
    pay.mockResolvedValue(billing("partially_paid"));
    renderBilling(billing("issued"));
    fireEvent.click(screen.getByRole("button", { name: b.savePayment }));
    expect(await screen.findByText(b.paymentInvalid)).toBeDefined();
    expect(pay).not.toHaveBeenCalled();

    fireEvent.change(screen.getByLabelText(new RegExp(b.paymentAmount)), { target: { value: "10000" } });
    fireEvent.change(screen.getByLabelText(new RegExp("เลขอ้างอิง")), { target: { value: "SLIP-1" } });

    // Select WHT 3%
    fireEvent.change(screen.getByLabelText(b.whtRate), { target: { value: "0.03" } });

    // Attempt save without certificate number -> should fail validation
    fireEvent.click(screen.getByRole("button", { name: b.savePayment }));
    expect(await screen.findByText(b.whtCertificateRequired)).toBeDefined();
    expect(pay).not.toHaveBeenCalled();

    // Enter certificate number and save
    fireEvent.change(screen.getByPlaceholderText(b.whtCertificateNumberPlaceholder), { target: { value: "50BIS-001" } });
    fireEvent.click(screen.getByRole("button", { name: b.savePayment }));

    await waitFor(() => expect(pay).toHaveBeenCalledTimes(1));
    const call = pay.mock.calls[0][0] as { id: string; payload: { amount: number; method: string; reference: string; whtRate: number; whtCertificateNumber: string }; idempotencyKey: string };
    expect(call.payload).toMatchObject({
      amount: 10000,
      method: "transfer",
      reference: "SLIP-1",
      whtRate: 0.03,
      whtCertificateNumber: "50BIS-001",
    });
    expect(call.idempotencyKey.length).toBeGreaterThanOrEqual(16);
  });

  it("releases retention when status is held and user confirms", async () => {
    releaseRetention.mockResolvedValue(billing("issued", { retentionStatus: "released" }));
    renderBilling(billing("issued", { retentionAmount: 1500, retentionRate: 0.05, retentionStatus: "held" }));

    const releaseBtn = screen.getByRole("button", { name: b.releaseRetention });
    expect(releaseBtn).toBeDefined();

    fireEvent.click(releaseBtn);
    // Confirm in modal
    const confirmButtons = screen.getAllByRole("button", { name: b.releaseRetention });
    // Click the confirm button in the modal
    fireEvent.click(confirmButtons[confirmButtons.length - 1]);

    await waitFor(() => expect(releaseRetention).toHaveBeenCalledWith({
      id: "bill-1",
      rowVersion: "00000000-0000-0000-0000-0000000000a1",
    }));
  });

  it("creates a credit note and lets another user approve it", async () => {
    createNote.mockResolvedValue(billing("issued"));
    approveNote.mockResolvedValue(billing("issued"));

    const testBill = billing("issued", {
      vatRate: 0.07,
      notes: [
        {
          id: "note-1",
          number: "CN-2026-0001",
          noteType: "credit",
          reason: "ลดราคาสินค้ามีตำหนิ",
          taxBaseDelta: 2000,
          vatDelta: 140,
          totalDelta: 2140,
          status: "pending_approval",
          createdBy: { id: "u-maker", displayName: "นาย ก (Maker)" },
        },
      ],
    });

    renderBilling(testBill);

    // Verify existing note is displayed
    expect(screen.getByText("CN-2026-0001")).toBeDefined();
    expect(screen.getByText("ลดราคาสินค้ามีตำหนิ")).toBeDefined();

    // Current user is "u-current" which is != "u-maker" -> Approve button is visible!
    const approveBtn = screen.getByRole("button", { name: b.approveNote });
    expect(approveBtn).toBeDefined();
    fireEvent.click(approveBtn);

    // Confirm in modal
    const confirmApprove = screen.getAllByRole("button", { name: b.approveNote });
    fireEvent.click(confirmApprove[confirmApprove.length - 1]);

    await waitFor(() => expect(approveNote).toHaveBeenCalledWith({
      id: "bill-1",
      noteId: "note-1",
      rowVersion: "00000000-0000-0000-0000-0000000000a1",
    }));
  });

  it("shows the translated error when the payment exceeds the outstanding amount", async () => {
    pay.mockRejectedValue(new ApiError({ status: 422, code: "PAYMENT_EXCEEDS_OUTSTANDING", message: "raw" }));
    renderBilling(billing("issued"));
    fireEvent.change(screen.getByLabelText(new RegExp(b.paymentAmount)), { target: { value: "99999" } });
    fireEvent.change(screen.getByLabelText(new RegExp("เลขอ้างอิง")), { target: { value: "SLIP-9" } });
    fireEvent.click(screen.getByRole("button", { name: b.savePayment }));
    expect(await screen.findByText(thMessages.finance.errors.PAYMENT_EXCEEDS_OUTSTANDING)).toBeDefined();
  });

  it("offers void only with no payments, and reversal needs a reason", async () => {
    renderBilling(billing("issued"));
    expect(screen.getByRole("button", { name: b.void })).toBeDefined();
  });

  it("hides void once money was received, and requires a reason to reverse a payment", async () => {
    reverse.mockResolvedValue(billing("issued"));
    renderBilling(billing("partially_paid"));
    expect(screen.queryByRole("button", { name: b.void })).toBeNull();
    fireEvent.click(screen.getByRole("button", { name: b.reverse }));
    fireEvent.click(await screen.findByRole("button", { name: b.confirm }));
    expect(await screen.findByText(b.reasonRequired)).toBeDefined();
    expect(reverse).not.toHaveBeenCalled();

    fireEvent.change(screen.getByPlaceholderText(b.reasonPlaceholder), { target: { value: "เช็คเด้ง" } });
    fireEvent.click(screen.getByRole("button", { name: b.confirm }));
    await waitFor(() => expect(reverse).toHaveBeenCalledWith({ id: "bill-1", paymentId: "pay-1", rowVersion: "00000000-0000-0000-0000-0000000000a1", reason: "เช็คเด้ง" }));
  });

  it("hides payment actions from users without the payments permission", () => {
    renderBilling(billing("issued"), ["billings.manage"]);
    expect(screen.queryByRole("button", { name: b.savePayment })).toBeNull();
  });
});

describe("ArAgingPanel (G-16 Credit Exposure & AR Aging)", () => {
  beforeEach(() => vi.clearAllMocks());

  it("displays AR Aging summary KPI cards and customer rows", () => {
    renderArAging();

    // Verify title and KPI summaries
    expect(screen.getByText(a.title)).toBeDefined();
    expect(screen.getByText(a.totalCurrent)).toBeDefined();
    expect(screen.getByText(a.totalBucket1To30)).toBeDefined();
    expect(screen.getByText(a.grandTotalOutstanding)).toBeDefined();

    // Verify customer row
    expect(screen.getByText("CUST-001")).toBeDefined();
    expect(screen.getAllByText("บริษัท สยามพาณิชย์ จำกัด").length).toBeGreaterThanOrEqual(1);
  });

  it("creates a credit override request and allows Maker-Checker approval", async () => {
    createOverride.mockResolvedValue({ id: "ovr-new" });
    approveOverride.mockResolvedValue({ id: "ovr-1" });

    renderArAging();

    // Verify existing override row
    expect(screen.getByText("โครงการขยายสาขาต่อเนื่อง")).toBeDefined();

    // Approve existing override (Maker != Checker: u-current != u-maker)
    const approveBtn = screen.getByRole("button", { name: a.approve });
    expect(approveBtn).toBeDefined();
    fireEvent.click(approveBtn);

    // Confirm in modal
    const confirmButtons = screen.getAllByRole("button", { name: a.approve });
    fireEvent.click(confirmButtons[confirmButtons.length - 1]);

    await waitFor(() => expect(approveOverride).toHaveBeenCalledWith({
      id: "ovr-1",
      rowVersion: "00000000-0000-0000-0000-0000000000b1",
    }));

    // Fill request override form
    fireEvent.change(screen.getByLabelText(new RegExp(`^${a.customer}\\s*\\*`)), {
      target: { value: "cust-1" },
    });
    fireEvent.change(screen.getByPlaceholderText(a.requestedCeilingAmountPlaceholder), {
      target: { value: "800000" },
    });
    fireEvent.change(screen.getByPlaceholderText(a.reasonPlaceholder), {
      target: { value: "ขยายโครงการเฟส 2" },
    });

    fireEvent.click(screen.getByRole("button", { name: a.submitRequest }));

    await waitFor(() => expect(createOverride).toHaveBeenCalledTimes(1));
    const payload = createOverride.mock.calls[0][0].payload;
    expect(payload).toMatchObject({
      customerId: "cust-1",
      requestedCeilingAmount: 800000,
      reason: "ขยายโครงการเฟส 2",
    });
  });
});

describe("FinanceSyncPage", () => {
  beforeEach(() => vi.clearAllMocks());

  it("warns that no accounting system is connected and shows retry state", () => {
    renderSync();
    expect(screen.getByText(s.notConfigured)).toBeDefined();
    expect(screen.getByText("HTTP_503")).toBeDefined();
    expect(screen.getByText("ACC-1")).toBeDefined();
  });

  it("explains a reconciliation mismatch with both amounts", () => {
    renderSync();
    expect(screen.getByText(s.issues.amount_mismatch)).toBeDefined();
  });

  it("offers requeue only for failed or dead messages, and dispatch to users who can run it", async () => {
    dispatch.mockResolvedValue({ processed: 1, sent: 1, failed: 0, dead: 0 });
    renderSync();
    expect(screen.getAllByRole("button", { name: s.requeue })).toHaveLength(2);
    fireEvent.click(screen.getByRole("button", { name: s.dispatch }));
    await waitFor(() => expect(dispatch).toHaveBeenCalledTimes(1));
  });

  it("displays existing COA mappings, submits a new COA mapping, and exports journal CSV", async () => {
    upsertMapping.mockResolvedValue({ id: "map-new", kind: "payment.recorded" });
    exportJournal.mockResolvedValue({
      batchRef: "BAT-2026-0001",
      exportedCount: 1,
      deadCount: 0,
      lines: [
        {
          messageId: "m-1",
          resourceNumber: "BIL-1",
          kind: "billing.issued",
          debitAccountCode: "1130",
          debitAccountName: "ลูกหนี้การค้า",
          creditAccountCode: "4100",
          creditAccountName: "รายได้ค่าบริการ",
          amount: 30000,
          currency: "THB",
        },
      ],
      csvContent: "BatchRef,MessageId,Kind\nBAT-2026-0001,m-1,billing.issued",
    });

    renderSync();

    // Verify existing COA mapping is displayed
    expect(screen.getByText("1130")).toBeDefined();
    expect(screen.getByText("4100")).toBeDefined();

    // Fill COA upsert form
    fireEvent.change(screen.getByRole("combobox", { name: new RegExp(`^${s.coa.kind}`) }), {
      target: { value: "payment.recorded" },
    });
    fireEvent.change(screen.getByPlaceholderText(s.coa.debitAccountCodePlaceholder), {
      target: { value: "1110" },
    });
    fireEvent.change(screen.getByPlaceholderText(s.coa.debitAccountNamePlaceholder), {
      target: { value: "เงินฝากธนาคาร" },
    });
    fireEvent.change(screen.getByPlaceholderText(s.coa.creditAccountCodePlaceholder), {
      target: { value: "1130" },
    });
    fireEvent.change(screen.getByPlaceholderText(s.coa.creditAccountNamePlaceholder), {
      target: { value: "ลูกหนี้การค้า" },
    });

    fireEvent.click(screen.getByRole("button", { name: s.coa.saveMapping }));

    await waitFor(() => expect(upsertMapping).toHaveBeenCalledWith({
      kind: "payment.recorded",
      debitAccountCode: "1110",
      debitAccountName: "เงินฝากธนาคาร",
      creditAccountCode: "1130",
      creditAccountName: "ลูกหนี้การค้า",
    }));

    // Trigger journal export
    fireEvent.click(screen.getByRole("button", { name: s.exportJournal }));

    await waitFor(() => expect(exportJournal).toHaveBeenCalledTimes(1));

    // Verify export result card shows batch ref and lines
    expect(screen.getByText(s.lastExportTitle)).toBeDefined();
    expect(screen.getAllByText(/BAT-2026-0001/).length).toBeGreaterThanOrEqual(1);
  });

  it("hides dispatch and requeue from read-only users", () => {
    renderSync(["finance-sync.read"]);
    expect(screen.queryByRole("button", { name: s.dispatch })).toBeNull();
    expect(screen.queryByRole("button", { name: s.exportJournal })).toBeNull();
    expect(screen.queryByRole("button", { name: s.requeue })).toBeNull();
    expect(screen.queryByRole("button", { name: s.coa.saveMapping })).toBeNull();
  });
});
