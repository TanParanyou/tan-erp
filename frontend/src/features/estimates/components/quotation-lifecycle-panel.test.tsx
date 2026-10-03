import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { QuotationLifecyclePanel } from "./quotation-lifecycle-panel";
import * as lifecycleQueries from "../api/quotation-lifecycle-queries";
import { ApiError } from "@/lib/api/api-error";
import type { QuotationHistoryResponse } from "@/lib/api/api-client";
import { QUOTATION_STATUSES, quotationLifecycleErrorCode, quotationStatusVariant } from "../quotation-lifecycle-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", async (importOriginal) => {
  const original = await importOriginal<typeof import("@/lib/permissions/can")>();
  return { ...original, can: (_membership: unknown, permission: string) => permissions.granted.includes(permission) };
});

const l = thMessages.quotationLifecycle;
const mutate = vi.fn();

function history(statuses: string[]): QuotationHistoryResponse {
  return {
    estimateId: "est-1",
    items: statuses.map((status, i) => ({
      id: `q-${i}`,
      number: `QT-0${i + 1}`,
      status,
      totalAmount: 1000,
      estimateRevisionId: "rev-1",
      estimateRevisionNo: 1,
      issuedAtUtc: "2026-10-04T00:00:00Z",
      rowVersion: `00000000-0000-0000-0000-00000000000${i}`,
      supersededByNumber: status === "superseded" ? "QT-02" : null,
      supersedesNumber: i === 1 && statuses[0] === "superseded" ? "QT-01" : null,
      amendmentReason: i === 1 && statuses[0] === "superseded" ? "แก้ที่อยู่" : null,
      voidReason: status === "voided" ? "ลูกค้าถอนคำขอ" : null,
      voidedBy: status === "voided" ? { id: "u-1", displayName: "ผู้ใช้" } : null,
      voidedAtUtc: status === "voided" ? "2026-10-05T00:00:00Z" : null,
    })),
  } as QuotationHistoryResponse;
}

function renderPanel(data: QuotationHistoryResponse | undefined, granted = ["quotations.void", "quotations.amend"]) {
  permissions.granted = granted;
  vi.spyOn(lifecycleQueries, "useQuotationHistory").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof lifecycleQueries.useQuotationHistory>);
  vi.spyOn(lifecycleQueries, "useQuotationLifecycleMutation").mockReturnValue({ mutateAsync: mutate, isPending: false } as unknown as ReturnType<typeof lifecycleQueries.useQuotationLifecycleMutation>);
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <QuotationLifecyclePanel estimateId="est-1" />
    </NextIntlClientProvider>
  );
}

describe("quotation lifecycle helpers and translations", () => {
  it("map statuses to badge variants and recognise only known errors", () => {
    expect(quotationStatusVariant("accepted")).toBe("success");
    expect(quotationStatusVariant("voided")).toBe("danger");
    expect(quotationStatusVariant("superseded")).toBe("neutral");
    expect(quotationLifecycleErrorCode("QUOTATION_ACCEPTED_LOCKED")).toBe("QUOTATION_ACCEPTED_LOCKED");
    expect(quotationLifecycleErrorCode("NOPE")).toBeNull();
  });

  it("have Thai and English text for every status and error", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of QUOTATION_STATUSES) expect(messages.quotationLifecycle.statuses[status]).toBeTruthy();
      for (const code of ["QUOTATION_ACCEPTED_LOCKED", "QUOTATION_INVALID_STATE", "QUOTATION_VERSION_CONFLICT"] as const) {
        expect(messages.quotationLifecycle.errors[code]).toBeTruthy();
      }
    }
  });
});

describe("QuotationLifecyclePanel", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows amend and void only on the live issued quotation", () => {
    renderPanel(history(["superseded", "issued"]));
    expect(screen.getAllByRole("button", { name: l.amend })).toHaveLength(1);
    expect(screen.getAllByRole("button", { name: l.void })).toHaveLength(1);
    expect(screen.getByText(l.supersededBy.replace("{number}", "QT-02"))).toBeDefined();
    expect(screen.getByText(new RegExp(l.supersedes.replace("{number}", "QT-01")))).toBeDefined();
  });

  it("offers no actions on an accepted quotation or without permission", () => {
    renderPanel(history(["accepted"]));
    expect(screen.queryByRole("button", { name: l.void })).toBeNull();
  });

  it("hides the buttons from users without void/amend permissions", () => {
    renderPanel(history(["issued"]), ["quotations.read"]);
    expect(screen.queryByRole("button", { name: l.amend })).toBeNull();
    expect(screen.queryByRole("button", { name: l.void })).toBeNull();
  });

  it("requires a reason before voiding and then sends the row version and a key", async () => {
    mutate.mockResolvedValue(history(["voided"]));
    renderPanel(history(["issued"]));
    fireEvent.click(screen.getByRole("button", { name: l.void }));
    fireEvent.click(await screen.findByRole("button", { name: l.confirm }));
    expect(await screen.findByText(l.reasonRequired)).toBeDefined();
    expect(mutate).not.toHaveBeenCalled();

    fireEvent.change(screen.getByLabelText(new RegExp(l.reason)), { target: { value: "ลูกค้าถอนคำขอ" } });
    fireEvent.click(screen.getByRole("button", { name: l.confirm }));
    await waitFor(() => expect(mutate).toHaveBeenCalledTimes(1));
    const call = mutate.mock.calls[0][0] as { action: string; reason: string; rowVersion: string; idempotencyKey: string };
    expect(call).toMatchObject({ action: "void", reason: "ลูกค้าถอนคำขอ", rowVersion: "00000000-0000-0000-0000-000000000000" });
    expect(call.idempotencyKey.length).toBeGreaterThanOrEqual(16);
  });

  it("shows the translated error when the quotation was locked meanwhile", async () => {
    mutate.mockRejectedValue(new ApiError({ status: 409, code: "QUOTATION_ACCEPTED_LOCKED", message: "raw" }));
    renderPanel(history(["issued"]));
    fireEvent.click(screen.getByRole("button", { name: l.amend }));
    fireEvent.change(await screen.findByLabelText(new RegExp(l.reason)), { target: { value: "แก้ไข" } });
    fireEvent.click(screen.getByRole("button", { name: l.confirm }));
    expect(await screen.findByText(thMessages.quotationLifecycle.errors.QUOTATION_ACCEPTED_LOCKED)).toBeDefined();
  });
});
