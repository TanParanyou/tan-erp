import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import { CostRecordMaintenance } from "./cost-record-maintenance";
import { useCostSources, useItemCostMutations, useItemCosts, useItemMasterLookups } from "@/features/item-master/api/item-master-queries";

const fileMocks = vi.hoisted(() => ({ createSession: vi.fn(), completeSession: vi.fn() }));
vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useCostSources: vi.fn(), useItemCostMutations: vi.fn(), useItemCosts: vi.fn(), useItemMasterLookups: vi.fn(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({ useSelectedMembership: () => ({ selectedMembership: { id: "membership-1", permissions: ["cost-records.create", "cost-records.submit"] } }) }));
vi.mock("@/lib/permissions/can", () => ({ can: () => true }));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: vi.fn(), error: vi.fn() } }) }));
vi.mock("@/lib/auth/auth-session", () => ({ getAuthToken: vi.fn().mockResolvedValue("token") }));
vi.mock("@/lib/api/file-client", () => ({ fileClient: fileMocks }));

function renderEditor() {
  return render(<NextIntlClientProvider locale="th" messages={messages}><CostRecordMaintenance itemId="item-1" /></NextIntlClientProvider>);
}

describe("CostRecordMaintenance", () => {
  const create = vi.fn();
  const update = vi.fn();
  const submit = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useItemCosts).mockReturnValue({ data: [], isLoading: false, isError: false, refetch: vi.fn() } as unknown as ReturnType<typeof useItemCosts>);
    vi.mocked(useItemMasterLookups).mockReturnValue({ data: { categories: [], units: [{ id: "11111111-1111-4111-8111-111111111111", code: "EA", name: { thai: "ชิ้น", english: "Each" }, status: "active" }] }, isLoading: false, isError: false } as unknown as ReturnType<typeof useItemMasterLookups>);
    vi.mocked(useCostSources).mockReturnValue({ data: [{ id: "22222222-2222-4222-8222-222222222222", code: "MANUAL", name: { thai: "ต้นทุนภายใน", english: "Internal" }, sourceType: "manual", isActive: true }], isLoading: false, isError: false } as unknown as ReturnType<typeof useCostSources>);
    create.mockResolvedValue({ id: "cost-1", rowVersion: "row-1" });
    update.mockResolvedValue({ id: "cost-1", rowVersion: "row-2" });
    fileMocks.createSession.mockResolvedValue({ sessionId: "session-1", slots: [{ slotId: "slot-1" }] });
    fileMocks.completeSession.mockResolvedValue({ files: [{ fileId: "file-1" }] });
    vi.mocked(useItemCostMutations).mockReturnValue({ create: { mutateAsync: create, isPending: false }, update: { mutateAsync: update, isPending: false }, submit: { mutateAsync: submit, isPending: false } } as unknown as ReturnType<typeof useItemCostMutations>);
  });

  it("creates a draft using a safe four-decimal amount and required provenance", async () => {
    renderEditor();
    fireEvent.click(screen.getByRole("button", { name: "สร้างฉบับร่างต้นทุน" }));
    fireEvent.change(screen.getByLabelText(/แหล่งที่มาต้นทุน/), { target: { value: "22222222-2222-4222-8222-222222222222" } });
    fireEvent.change(screen.getByLabelText(/หน่วย/), { target: { value: "11111111-1111-4111-8111-111111111111" } });
    fireEvent.change(screen.getByLabelText(/ราคา/), { target: { value: "1234.1250" } });
    fireEvent.change(screen.getByLabelText(/สกุลเงิน/), { target: { value: "THB" } });
    fireEvent.change(screen.getByLabelText(/เริ่มมีผล/), { target: { value: "2026-09-25T09:00" } });
    fireEvent.change(screen.getByLabelText(/เลขอ้างอิง/), { target: { value: "PO-123" } });
    fireEvent.change(screen.getByLabelText(/เหตุผล/), { target: { value: "ใบเสนอราคาผู้ขาย" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกร่างต้นทุน" }));

    await waitFor(() => expect(create).toHaveBeenCalledOnce());
    expect(create.mock.calls[0]?.[0].payload).toMatchObject({
      amount: 1234.125,
      costSourceId: "22222222-2222-4222-8222-222222222222",
      unitId: "11111111-1111-4111-8111-111111111111",
      sourceReference: "PO-123",
      reason: "ใบเสนอราคาผู้ขาย",
    });
    expect(create.mock.calls[0]?.[0].idempotencyKey).toEqual(expect.any(String));
  });

  it("uploads evidence only after the draft exists, then binds the verified file", async () => {
    renderEditor();
    fireEvent.click(screen.getByRole("button", { name: "สร้างฉบับร่างต้นทุน" }));
    fireEvent.change(screen.getByLabelText(/แหล่งที่มาต้นทุน/), { target: { value: "22222222-2222-4222-8222-222222222222" } });
    fireEvent.change(screen.getByLabelText(/หน่วย/), { target: { value: "11111111-1111-4111-8111-111111111111" } });
    fireEvent.change(screen.getByLabelText(/ราคา/), { target: { value: "25.5000" } });
    fireEvent.change(screen.getByLabelText(/สกุลเงิน/), { target: { value: "THB" } });
    fireEvent.change(screen.getByLabelText(/เริ่มมีผล/), { target: { value: "2026-09-25T09:00" } });
    fireEvent.change(screen.getByLabelText(/เลขอ้างอิง/), { target: { value: "QUOTE-1" } });
    fireEvent.change(screen.getByLabelText(/เหตุผล/), { target: { value: "Quoted cost" } });
    const evidence = new File(["proof"], "quote.pdf", { type: "application/pdf" });
    const files = { 0: evidence, length: 1, item: (index: number) => index === 0 ? evidence : null };
    fireEvent.change(screen.getByLabelText("หลักฐาน"), { target: { files } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกร่างต้นทุน" }));

    await waitFor(() => expect(update).toHaveBeenCalledOnce());
    expect(create.mock.invocationCallOrder[0]).toBeLessThan(fileMocks.createSession.mock.invocationCallOrder[0] ?? Number.MAX_SAFE_INTEGER);
    expect(fileMocks.createSession.mock.invocationCallOrder[0]).toBeLessThan(fileMocks.completeSession.mock.invocationCallOrder[0] ?? Number.MAX_SAFE_INTEGER);
    expect(fileMocks.completeSession.mock.invocationCallOrder[0]).toBeLessThan(update.mock.invocationCallOrder[0] ?? Number.MAX_SAFE_INTEGER);
    expect(update.mock.calls[0]?.[0].payload.evidenceFileId).toBe("file-1");
  });
});
