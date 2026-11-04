import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import type { ItemResponse } from "@/lib/api/api-client";
import { ItemBarcodeMaintenance } from "./item-barcode-maintenance";
import { useItemBarcodeMutations, useItemBarcodes, useItemMasterLookups, useItemUnitConversionMutations, useItemUnitConversions, useSharedUnitConversions } from "@/features/item-master/api/item-master-queries";

vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useItemBarcodeMutations: vi.fn(),
  useItemBarcodes: vi.fn(),
  useItemMasterLookups: vi.fn(),
  useItemUnitConversionMutations: vi.fn(),
  useItemUnitConversions: vi.fn(),
  useSharedUnitConversions: vi.fn(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({ useSelectedMembership: () => ({ selectedMembership: { permissions: ["items.manage-barcodes"] } }) }));
vi.mock("@/lib/permissions/can", () => ({ can: () => true }));

function renderEditor() {
  const item = {
    id: "item-1",
    status: "draft",
    baseUnit: { id: "unit-each", code: "EA", name: { thai: "ชิ้น", english: "Each" } },
  } as unknown as ItemResponse;
  return render(<NextIntlClientProvider locale="th" messages={messages}><ItemBarcodeMaintenance itemId="item-1" item={item} /></NextIntlClientProvider>);
}

describe("ItemBarcodeMaintenance", () => {
  const createBarcode = vi.fn();
  const createConversion = vi.fn();
  const updateConversion = vi.fn();
  const deactivateConversion = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useItemBarcodes).mockReturnValue({ data: [], isLoading: false, isError: false } as unknown as ReturnType<typeof useItemBarcodes>);
    vi.mocked(useItemMasterLookups).mockReturnValue({ data: { categories: [], brands: [], units: [
      { id: "unit-each", code: "EA", status: "active", name: { thai: "ชิ้น", english: "Each" } },
      { id: "unit-box", code: "BOX", status: "active", name: { thai: "กล่อง", english: "Box" } },
    ] }, isLoading: false, isError: false } as unknown as ReturnType<typeof useItemMasterLookups>);
    vi.mocked(useItemUnitConversions).mockReturnValue({ data: [], isLoading: false, isError: false } as unknown as ReturnType<typeof useItemUnitConversions>);
    vi.mocked(useSharedUnitConversions).mockReturnValue({ data: [], isLoading: false, isError: false } as unknown as ReturnType<typeof useSharedUnitConversions>);
    createBarcode.mockResolvedValue({ id: "barcode-1" });
    createConversion.mockResolvedValue({ id: "conversion-1" });
    updateConversion.mockResolvedValue({ id: "conversion-1" });
    deactivateConversion.mockResolvedValue({ id: "conversion-1" });
    vi.mocked(useItemBarcodeMutations).mockReturnValue({
      create: { mutateAsync: createBarcode, isPending: false, isError: false },
      setPrimary: { mutate: vi.fn(), isPending: false, isError: false },
      deactivate: { mutateAsync: vi.fn(), isPending: false, isError: false },
    } as unknown as ReturnType<typeof useItemBarcodeMutations>);
    vi.mocked(useItemUnitConversionMutations).mockReturnValue({
      create: { mutateAsync: createConversion, isPending: false, isError: false },
      update: { mutateAsync: updateConversion, isPending: false, isError: false },
      deactivate: { mutateAsync: deactivateConversion, isPending: false, isError: false },
    } as unknown as ReturnType<typeof useItemUnitConversionMutations>);
  });

  it("creates a versioned packaging conversion targeting the item base unit", async () => {
    renderEditor();
    fireEvent.change(screen.getByLabelText("หน่วยต้นทาง"), { target: { value: "unit-box" } });
    fireEvent.change(screen.getByLabelText(/จำนวนหน่วยฐานต่อหนึ่งหน่วย/), { target: { value: "12" } });
    fireEvent.change(screen.getByLabelText(/เหตุผล/), { target: { value: "บรรจุ 12 ชิ้น" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มอัตราแปลง" }));

    await waitFor(() => expect(createConversion).toHaveBeenCalledOnce());
    expect(createConversion.mock.calls[0]?.[0]).toMatchObject({
      fromUnitId: "unit-box",
      toUnitId: "unit-each",
      factor: "12",
      reason: "บรรจุ 12 ชิ้น",
    });
    expect(createBarcode).not.toHaveBeenCalled();
  });
  it("requires a barcode value before submitting a GTIN", () => {
    renderEditor();
    fireEvent.click(screen.getByRole("button", { name: "เพิ่ม Barcode" }));
    expect(createBarcode).not.toHaveBeenCalled();
  });

  it("allows multiple alternate internal codes for the same item", async () => {
    renderEditor();
    fireEvent.change(screen.getByLabelText("ชนิดรหัส"), { target: { value: "internal" } });
    fireEvent.change(screen.getByLabelText(/ค่า Barcode \/ GTIN \/ รหัสภายใน/), { target: { value: "ALT-SKU-01" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่ม Barcode" }));
    await waitFor(() => expect(createBarcode).toHaveBeenCalledTimes(1));

    fireEvent.change(screen.getByLabelText(/ค่า Barcode \/ GTIN \/ รหัสภายใน/), { target: { value: "ALT-SKU-02" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่ม Barcode" }));
    await waitFor(() => expect(createBarcode).toHaveBeenCalledTimes(2));
    expect(createBarcode.mock.calls.map(([payload]) => payload.value)).toEqual(["ALT-SKU-01", "ALT-SKU-02"]);
    expect(createBarcode.mock.calls.every(([payload]) => payload.identifierType === "internal")).toBe(true);
  });

  it("opens edit modal and updates an active packaging conversion", async () => {
    vi.mocked(useItemUnitConversions).mockReturnValue({
      data: [
        {
          id: "conv-1",
          itemId: "item-1",
          fromUnitId: "unit-box",
          fromUnitCode: "BOX",
          toUnitId: "unit-each",
          toUnitCode: "EA",
          factor: "12",
          effectiveFrom: "2026-01-01",
          reason: "กล่อง 12 ชิ้น",
          status: "active",
          rowVersion: "version-1",
        },
      ],
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof useItemUnitConversions>);

    renderEditor();
    expect(screen.getByText("BOX → EA")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "แก้ไข" }));

    expect(screen.getByRole("heading", { name: "แก้ไขอัตราแปลงหน่วย" })).toBeInTheDocument();
    const factorInput = screen.getByDisplayValue("12");
    fireEvent.change(factorInput, { target: { value: "24" } });
    const reasonInput = screen.getByDisplayValue("กล่อง 12 ชิ้น");
    fireEvent.change(reasonInput, { target: { value: "ปรับเป็น 24 ชิ้น" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึก" }));

    await waitFor(() => expect(updateConversion).toHaveBeenCalledOnce());
    expect(updateConversion).toHaveBeenCalledWith({
      id: "conv-1",
      payload: {
        factor: "24",
        effectiveFrom: "2026-01-01",
        effectiveTo: null,
        reason: "ปรับเป็น 24 ชิ้น",
      },
      rowVersion: "version-1",
    });
  });

  it("opens deactivate modal and deactivates a packaging conversion with reason", async () => {
    vi.mocked(useItemUnitConversions).mockReturnValue({
      data: [
        {
          id: "conv-1",
          itemId: "item-1",
          fromUnitId: "unit-box",
          fromUnitCode: "BOX",
          toUnitId: "unit-each",
          toUnitCode: "EA",
          factor: "12",
          effectiveFrom: "2026-01-01",
          reason: "กล่อง 12 ชิ้น",
          status: "active",
          rowVersion: "version-1",
        },
      ],
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof useItemUnitConversions>);

    renderEditor();
    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งาน" }));

    expect(screen.getByRole("heading", { name: "ปิดใช้งานอัตราแปลงหน่วย" })).toBeInTheDocument();
    const modal = screen.getByRole("dialog");
    const reasonInput = within(modal).getByPlaceholderText(/ระบุเหตุผลในการปรับปรุงต้นทุน/);
    fireEvent.change(reasonInput, { target: { value: "ยกเลิกกล่องรุ่นนี้" } });
    fireEvent.click(within(modal).getByRole("button", { name: "ยืนยันปิดใช้งาน" }));

    await waitFor(() => expect(deactivateConversion).toHaveBeenCalledOnce());
    expect(deactivateConversion).toHaveBeenCalledWith({
      id: "conv-1",
      payload: {
        reason: "ยกเลิกกล่องรุ่นนี้",
      },
      rowVersion: "version-1",
    });
  });
});
