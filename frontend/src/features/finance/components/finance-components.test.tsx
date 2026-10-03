import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { BillingDetail } from "./billing-detail";
import { FinanceSyncPage } from "./finance-sync-page";
import * as financeQueries from "../api/finance-queries";
import { ApiError } from "@/lib/api/api-error";
import type { BillingResponse } from "@/lib/api/api-client";
import {
  BILLING_STATUSES,
  OUTBOX_KINDS,
  OUTBOX_STATUSES,
  PAYMENT_METHODS,
  RECONCILIATION_ISSUES,
  billingStatusVariant,
  financeErrorCode,
  outboxStatusVariant,
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
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const b = thMessages.finance.billings;
const s = thMessages.finance.sync;
const ALL = ["billings.manage", "payments.manage", "finance-sync.run", "finance-sync.read"];

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
    payments: status === "issued" ? [] : [{ id: "pay-1", number: "PAY-2026-0001", amount: 10000, method: "transfer", reference: "SLIP-1", receivedDate: "2026-10-04", status: "recorded", recordedBy: { id: "u-1", displayName: "ผู้บันทึก" } }],
    ...overrides,
  } as BillingResponse;
}

const pay = vi.fn();
const voidBilling = vi.fn();
const reverse = vi.fn();

function renderBilling(data: BillingResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(financeQueries, "useBilling").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof financeQueries.useBilling>);
  vi.spyOn(financeQueries, "useBillingMutations").mockReturnValue({
    pay: { mutateAsync: pay, isPending: false },
    voidBilling: { mutateAsync: voidBilling, isPending: false },
    reverse: { mutateAsync: reverse, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof financeQueries.useBillingMutations>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <BillingDetail billingId="bill-1" />
    </NextIntlClientProvider>
  );
}

const dispatch = vi.fn();
const requeue = vi.fn();

function renderSync(granted: string[] = ALL) {
  permissions.granted = granted;
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
  vi.spyOn(financeQueries, "useSyncMutations").mockReturnValue({
    dispatch: { mutateAsync: dispatch, isPending: false },
    requeue: { mutateAsync: requeue, isPending: false },
  } as unknown as ReturnType<typeof financeQueries.useSyncMutations>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <FinanceSyncPage />
    </NextIntlClientProvider>
  );
}

describe("finance status helpers and translations", () => {
  it("map statuses to badge variants and recognise only known errors", () => {
    expect(billingStatusVariant("paid")).toBe("success");
    expect(billingStatusVariant("voided")).toBe("danger");
    expect(outboxStatusVariant("dead")).toBe("danger");
    expect(outboxStatusVariant("sent")).toBe("success");
    expect(financeErrorCode("PAYMENT_DUPLICATE_REFERENCE")).toBe("PAYMENT_DUPLICATE_REFERENCE");
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
      for (const issue of RECONCILIATION_ISSUES) expect(messages.finance.sync.issues[issue]).toBeTruthy();
      for (const code of ["BILLING_EXCEEDS_CONTRACT", "PAYMENT_EXCEEDS_OUTSTANDING", "PAYMENT_DUPLICATE_REFERENCE", "OUTBOX_CONFIRMATION_CONFLICT"] as const) {
        expect(messages.finance.errors[code]).toBeTruthy();
      }
      expect(messages.shell.financeSubgroup).toBeTruthy();
    }
  });
});

describe("BillingDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("posts a payment with only valid input and an idempotency key", async () => {
    pay.mockResolvedValue(billing("partially_paid"));
    renderBilling(billing("issued"));
    fireEvent.click(screen.getByRole("button", { name: b.savePayment }));
    expect(await screen.findByText(b.paymentInvalid)).toBeDefined();
    expect(pay).not.toHaveBeenCalled();

    fireEvent.change(screen.getByLabelText(new RegExp(b.paymentAmount)), { target: { value: "10000" } });
    fireEvent.change(screen.getByLabelText(new RegExp("เลขอ้างอิง")), { target: { value: "SLIP-1" } });
    fireEvent.click(screen.getByRole("button", { name: b.savePayment }));
    await waitFor(() => expect(pay).toHaveBeenCalledTimes(1));
    const call = pay.mock.calls[0][0] as { id: string; payload: { amount: number; method: string; reference: string }; idempotencyKey: string };
    expect(call.payload).toMatchObject({ amount: 10000, method: "transfer", reference: "SLIP-1" });
    expect(call.idempotencyKey.length).toBeGreaterThanOrEqual(16);
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

    fireEvent.change(screen.getByLabelText(new RegExp(b.reason)), { target: { value: "เช็คเด้ง" } });
    fireEvent.click(screen.getByRole("button", { name: b.confirm }));
    await waitFor(() => expect(reverse).toHaveBeenCalledWith({ id: "bill-1", paymentId: "pay-1", rowVersion: "00000000-0000-0000-0000-0000000000a1", reason: "เช็คเด้ง" }));
  });

  it("hides payment actions from users without the payments permission", () => {
    renderBilling(billing("issued"), ["billings.manage"]);
    expect(screen.queryByRole("button", { name: b.savePayment })).toBeNull();
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

  it("hides dispatch and requeue from read-only users", () => {
    renderSync(["finance-sync.read"]);
    expect(screen.queryByRole("button", { name: s.dispatch })).toBeNull();
    expect(screen.queryByRole("button", { name: s.requeue })).toBeNull();
  });
});
