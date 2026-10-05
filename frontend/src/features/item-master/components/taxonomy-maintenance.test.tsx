import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { act, fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import { TaxonomyMaintenance } from "./taxonomy-maintenance";
import {
  useItemMasterReferenceData,
  useItemMasterReferenceMutations,
  useSharedUnitConversionMutations,
  useSharedUnitConversions,
  useCategoryAttributeTemplate,
  useCategoryAttributeTemplateMutations,
} from "@/features/item-master/api/item-master-queries";
import { uploadVerifiedItemImage } from "@/features/item-master/api/upload-item-image";

vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useItemMasterReferenceData: vi.fn(),
  useItemMasterReferenceMutations: vi.fn(),
  useSharedUnitConversionMutations: vi.fn(),
  useSharedUnitConversions: vi.fn(),
  useCategoryAttributeTemplate: vi.fn(),
  useCategoryAttributeTemplateMutations: vi.fn(),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/item-master/reference-data",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({ useSelectedMembership: () => ({ selectedMembership: { id: "membership-1", permissions: ["items.manage-taxonomy"] } }) }));
vi.mock("@/lib/permissions/can", () => ({ can: () => true }));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: vi.fn(), error: vi.fn() } }) }));
vi.mock("@/hooks/useAuthenticatedFileUrl", () => ({ useAuthenticatedFileUrl: () => ({ objectUrl: null, isLoading: false, isError: false }) }));
vi.mock("@/lib/auth/auth-session", () => ({ getAuthToken: vi.fn().mockResolvedValue("test-token") }));
vi.mock("@/features/item-master/api/upload-item-image", () => ({ uploadVerifiedItemImage: vi.fn().mockResolvedValue("verified-category-image") }));
vi.mock("@/components/forms/ImageUpload", () => ({
  ImageUpload: ({ label, onChange }: { label?: string; onChange: (file: File | null) => void }) => <label>{label}<input type="file" aria-label={label} onChange={(event) => onChange(event.target.files?.[0] ?? null)} /></label>,
}));
vi.mock("@/components/ui/Avatar", () => ({
  Avatar: ({ initial, fileId }: { initial?: string; fileId?: string | null }) => <span data-testid="taxonomy-avatar" data-file-id={fileId ?? "none"}>{initial}</span>,
}));

function renderPage() {
  return render(<NextIntlClientProvider locale="th" messages={messages}><TaxonomyMaintenance /></NextIntlClientProvider>);
}

describe("TaxonomyMaintenance", () => {
  const createCategory = vi.fn();
  const updateCategory = vi.fn();
  const createBrand = vi.fn();
  const updateBrand = vi.fn();
  const createUnit = vi.fn();
  const updateUnit = vi.fn();
  const createTaxCategory = vi.fn();
  const updateTaxCategory = vi.fn();
  const createConversion = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useItemMasterReferenceData).mockReturnValue({
      data: {
        categories: [
          { id: "parent-1", code: "ROOT", name: { thai: "วัสดุ", english: "Materials" }, status: "active", rowVersion: "parent-version" },
          { id: "category-1", code: "MAT", name: { thai: "ชิ้นส่วน", english: "Components" }, description: { thai: "คำอธิบาย", english: "Description" }, parentCategoryId: "parent-1", allowedItemTypes: ["product"], sortOrder: 4, status: "active", imageFileId: "category-image-1", rowVersion: "version-1" },
        ],
        brands: [],
        units: [
          { id: "unit-1", code: "M3", name: { thai: "ลูกบาศก์เมตร", english: "Cubic meter" }, symbol: "m³", dimension: "volume", decimalScale: 3, roundingMode: "half_up", status: "active", rowVersion: "unit-version-1" },
          { id: "unit-2", code: "PCS", name: { thai: "ชิ้น", english: "Piece" }, symbol: "ชิ้น", dimension: "count", decimalScale: 0, roundingMode: "half_up", status: "active", rowVersion: "unit-version-2" },
        ],
        taxCategories: [],
      }, isLoading: false, isError: false, refetch: vi.fn(),
    } as unknown as ReturnType<typeof useItemMasterReferenceData>);
    vi.mocked(useItemMasterReferenceMutations).mockReturnValue({
      createCategory: { mutateAsync: createCategory, isPending: false, isError: false },
      updateCategory: { mutateAsync: updateCategory, isPending: false, isError: false },
      createBrand: { mutateAsync: createBrand, isPending: false, isError: false },
      updateBrand: { mutateAsync: updateBrand, isPending: false, isError: false },
      createUnit: { mutateAsync: createUnit, isPending: false, isError: false },
      updateUnit: { mutateAsync: updateUnit, isPending: false, isError: false },
      createTaxCategory: { mutateAsync: createTaxCategory, isPending: false, isError: false },
      updateTaxCategory: { mutateAsync: updateTaxCategory, isPending: false, isError: false },
    } as unknown as ReturnType<typeof useItemMasterReferenceMutations>);
    vi.mocked(useSharedUnitConversions).mockReturnValue({ data: [], isLoading: false, isError: false } as unknown as ReturnType<typeof useSharedUnitConversions>);
    vi.mocked(useSharedUnitConversionMutations).mockReturnValue({ create: { mutateAsync: createConversion, isPending: false, isError: false } } as unknown as ReturnType<typeof useSharedUnitConversionMutations>);
    vi.mocked(useCategoryAttributeTemplate).mockReturnValue({
      data: { categoryId: "category-1", templates: [] },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useCategoryAttributeTemplate>);
    vi.mocked(useCategoryAttributeTemplateMutations).mockReturnValue({
      setTemplates: { mutateAsync: vi.fn(), isPending: false, isError: false },
    } as unknown as ReturnType<typeof useCategoryAttributeTemplateMutations>);
  });

  it("shows the localized reference list and its empty state after switching sections", () => {
    renderPage();
    expect(screen.getByRole("row", { name: /วัสดุ/ })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "รหัสหมวดหมู่" })).toBeInTheDocument();
    expect(screen.getByText("ชื่อหมวดหมู่")).toBeInTheDocument();
    const categoryRow = screen.getByRole("row", { name: /MAT.*ชิ้นส่วน/ });
    expect(within(categoryRow).getByTestId("taxonomy-avatar")).toHaveAttribute("data-file-id", "category-image-1");
    expect(screen.getByRole("button", { name: "ส่งออกข้อมูลอ้างอิง" })).toBeEnabled();
    fireEvent.click(screen.getByRole("tab", { name: "แบรนด์สินค้า" }));
    expect(screen.getByText("ยังไม่มีข้อมูลอ้างอิง")).toBeInTheDocument();
  });

  it("keeps invalid forms open and does not issue a create request", async () => {
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดหมู่" }));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));
    expect((await screen.findAllByText("กรุณากรอกข้อมูลที่จำเป็น")).length).toBeGreaterThan(0);
    expect(within(screen.getByRole("dialog")).getByRole("group", { name: /ชนิดรายการที่อนุญาต/ })).toBeInTheDocument();
    expect(screen.getByText("ต้องกรอกชื่อภาษาไทย ส่วนชื่อภาษาอังกฤษไม่บังคับ")).toBeInTheDocument();
    expect(createCategory).not.toHaveBeenCalled();
    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });

  it("locks drawer actions and multilingual controls while a category save is pending", async () => {
    createCategory.mockImplementationOnce(() => new Promise<void>(() => undefined));
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดหมู่" }));
    const dialog = within(screen.getByRole("dialog"));
    fireEvent.click(dialog.getByRole("radio", { name: "กรอกรหัสเอง" }));
    fireEvent.change(dialog.getAllByRole("textbox")[0]!, { target: { value: "MAT-NEW" } });
    fireEvent.change(dialog.getByPlaceholderText("ชื่อหมวดหมู่ (TH)"), { target: { value: "วัสดุใหม่" } });
    const saveButton = dialog.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" });
    act(() => {
      saveButton.click();
      saveButton.click();
    });

    await waitFor(() => expect(createCategory).toHaveBeenCalledOnce());
    expect(dialog.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" })).toBeDisabled();
    expect(dialog.getByRole("button", { name: "ยกเลิก" })).toBeDisabled();
    expect(dialog.getByRole("button", { name: "ปิด" })).toBeDisabled();
    expect(dialog.getAllByRole("button", { name: "EN" }).every((button) => (button as HTMLButtonElement).disabled)).toBe(true);
    expect(dialog.getAllByRole("button", { name: "Copy TH to EN" }).every((button) => (button as HTMLButtonElement).disabled)).toBe(true);
  });

  it("creates a tax category from the reference data screen", async () => {
    createTaxCategory.mockResolvedValue({ id: "tax-category-1", code: "VAT7" });
    renderPage();
    fireEvent.click(screen.getByRole("tab", { name: "หมวดภาษี" }));
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดภาษี" }));
    const dialog = within(screen.getByRole("dialog"));
    fireEvent.click(dialog.getByRole("radio", { name: "กรอกรหัสเอง" }));
    fireEvent.change(dialog.getByRole("textbox", { name: /รหัสหมวดภาษี/ }), { target: { value: "VAT7" } });
    fireEvent.change(dialog.getByPlaceholderText("ชื่อหมวดภาษี (TH)"), { target: { value: "ภาษีมูลค่าเพิ่ม 7%" } });
    fireEvent.click(dialog.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));

    await waitFor(() => expect(createTaxCategory).toHaveBeenCalledWith(expect.objectContaining({ code: "VAT7", name: { thai: "ภาษีมูลค่าเพิ่ม 7%", english: null } })));
  });

  it("uses GEN by default and submits null so the server issues the code on save", async () => {
    createCategory.mockResolvedValue({ id: "category-generated", code: "CAT-00001" });
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดหมู่" }));
    const dialog = within(screen.getByRole("dialog"));
    expect(dialog.getByRole("radio", { name: "สร้างรหัสอัตโนมัติ" })).toBeChecked();
    fireEvent.change(dialog.getByPlaceholderText("ชื่อหมวดหมู่ (TH)"), { target: { value: "หมวดใหม่" } });
    fireEvent.click(dialog.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));

    await waitFor(() => expect(createCategory).toHaveBeenCalledWith(expect.objectContaining({ code: null, name: { thai: "หมวดใหม่", english: null } })));
  });

  it("defers category image upload until save and attaches the verified file", async () => {
    createCategory.mockResolvedValue({ id: "category-image-1", code: "CAT-00001", rowVersion: "row-version-1" });
    updateCategory.mockResolvedValue({ id: "category-image-1", code: "CAT-00001", rowVersion: "row-version-2" });
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดหมู่" }));
    const dialog = within(screen.getByRole("dialog"));
    fireEvent.change(dialog.getByPlaceholderText("ชื่อหมวดหมู่ (TH)"), { target: { value: "หมวดพร้อมภาพ" } });
    fireEvent.change(dialog.getByLabelText("ไฟล์รูปภาพ"), { target: { files: [new File(["image"], "category.jpg", { type: "image/jpeg" })] } });

    expect(uploadVerifiedItemImage).not.toHaveBeenCalled();
    fireEvent.click(dialog.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));

    await waitFor(() => expect(uploadVerifiedItemImage).toHaveBeenCalledWith(expect.objectContaining({
      itemId: "category-image-1",
      parentType: "item-category",
      membershipId: "membership-1",
    })));
    await waitFor(() => expect(updateCategory).toHaveBeenCalledWith(expect.objectContaining({
      id: "category-image-1",
      rowVersion: "row-version-1",
      payload: expect.objectContaining({ imageFileId: "verified-category-image" }),
    })));
  });

  it("marks brand and tax-category required fields while leaving descriptions optional", async () => {
    renderPage();
    fireEvent.click(screen.getByRole("tab", { name: "แบรนด์สินค้า" }));
    fireEvent.click(screen.getByRole("button", { name: "สร้างแบรนด์" }));
    let dialog = within(screen.getByRole("dialog"));
    expect(dialog.getByRole("radio", { name: "สร้างรหัสอัตโนมัติ" })).toBeChecked();
    fireEvent.click(dialog.getByRole("radio", { name: "กรอกรหัสเอง" }));
    expect(dialog.getByLabelText(/รหัสแบรนด์/)).toBeRequired();
    expect(dialog.getByLabelText(/ชื่อแบรนด์/)).toBeRequired();
    expect(dialog.getByLabelText(/ลำดับการแสดง/)).toBeRequired();
    expect(dialog.getByLabelText(/รายละเอียด/)).not.toBeRequired();
    fireEvent.click(dialog.getByRole("button", { name: "ยกเลิก" }));
    fireEvent.click(await screen.findByRole("button", { name: "ละทิ้งข้อมูล" }));

    fireEvent.click(screen.getByRole("tab", { name: "หมวดภาษี" }));
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดภาษี" }));
    dialog = within(screen.getByRole("dialog"));
    fireEvent.click(dialog.getByRole("radio", { name: "กรอกรหัสเอง" }));
    expect(dialog.getByLabelText(/รหัสหมวดภาษี/)).toBeRequired();
    expect(dialog.getByLabelText(/ชื่อหมวดภาษี/)).toBeRequired();
    expect(dialog.getByLabelText(/ลำดับการแสดง/)).toBeRequired();
  });

  it("requires a unit symbol before creating a unit", async () => {
    renderPage();
    fireEvent.click(screen.getByRole("tab", { name: "หน่วยนับ" }));
    fireEvent.click(screen.getByRole("button", { name: "สร้างหน่วย" }));
    const dialog = within(screen.getByRole("dialog"));
    fireEvent.click(dialog.getByRole("radio", { name: "กรอกรหัสเอง" }));
    expect(dialog.getByLabelText(/รหัสหน่วย/)).toBeRequired();
    expect(dialog.getByLabelText(/ชื่อหน่วย/)).toBeRequired();
    expect(dialog.getByLabelText(/สัญลักษณ์หน่วย/)).toBeRequired();
    expect(dialog.getByRole("combobox", { name: /มิติหน่วย/ })).toBeRequired();
    expect(dialog.getByLabelText(/จำนวนตำแหน่งทศนิยม/)).toBeRequired();
    expect(dialog.getByRole("combobox", { name: /วิธีปัดเศษ/ })).toHaveValue("half_up");
    expect(dialog.getByRole("option", { name: "ใกล้สุด; กรณีกึ่งกลางปัดออกจากศูนย์" })).toBeInTheDocument();
    expect(dialog.getByRole("option", { name: "ใกล้สุด; กรณีกึ่งกลางปัดไปเลขคู่" })).toBeInTheDocument();
    fireEvent.change(dialog.getByRole("textbox", { name: /รหัสหน่วย/ }), { target: { value: "BOX" } });
    fireEvent.change(dialog.getByPlaceholderText("ชื่อหน่วย (TH)"), { target: { value: "กล่อง" } });
    fireEvent.click(dialog.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));

    expect(await dialog.findByText("กรุณากรอกข้อมูลที่จำเป็น")).toBeInTheDocument();
    expect(createUnit).not.toHaveBeenCalled();
  });

  it("validates shared unit conversions and submits valid values", async () => {
    renderPage();
    fireEvent.click(screen.getByRole("tab", { name: "หน่วยนับ" }));
    const fromUnit = screen.getByRole("combobox", { name: /หน่วยต้นทาง/ });
    const toUnit = screen.getByRole("combobox", { name: /หน่วยปลายทาง/ });
    const factor = screen.getByLabelText(/จำนวนหน่วยฐานต่อหนึ่งหน่วย/);
    const effectiveTo = screen.getByLabelText(/สิ้นสุดวันที่มีผล/);
    const reason = screen.getByLabelText(/เหตุผล/);
    const submitButton = screen.getByRole("button", { name: "เพิ่มอัตราแปลง" });

    fireEvent.change(fromUnit, { target: { value: "unit-1" } });
    fireEvent.change(toUnit, { target: { value: "unit-1" } });
    fireEvent.change(factor, { target: { value: "1.1234567" } });
    fireEvent.change(effectiveTo, { target: { value: "2020-01-01" } });
    fireEvent.change(reason, { target: { value: "ทดสอบ" } });
    fireEvent.click(submitButton);

    expect(await screen.findByText("หน่วยต้นทางและปลายทางต้องต่างกัน")).toBeInTheDocument();
    expect(screen.getByText("กรอกตัวคูณที่มากกว่า 0 และมีทศนิยมไม่เกิน 6 ตำแหน่ง")).toBeInTheDocument();
    expect(screen.getByText("วันสิ้นสุดต้องไม่อยู่ก่อนวันเริ่มต้น")).toBeInTheDocument();
    expect(createConversion).not.toHaveBeenCalled();

    fireEvent.change(factor, { target: { value: "0" } });
    fireEvent.change(effectiveTo, { target: { value: "" } });
    fireEvent.click(submitButton);
    expect(await screen.findByText("กรอกตัวคูณที่มากกว่า 0 และมีทศนิยมไม่เกิน 6 ตำแหน่ง")).toBeInTheDocument();
    expect(createConversion).not.toHaveBeenCalled();

    fireEvent.change(toUnit, { target: { value: "unit-2" } });
    fireEvent.change(factor, { target: { value: "12" } });
    fireEvent.change(effectiveTo, { target: { value: "" } });
    fireEvent.click(submitButton);

    await waitFor(() => expect(createConversion).toHaveBeenCalledWith(expect.objectContaining({
      fromUnitId: "unit-1",
      toUnitId: "unit-2",
      factor: "12",
      reason: "ทดสอบ",
    })));
  });

  it("offers documented dimensions instead of requiring users to type them", () => {
    renderPage();
    fireEvent.click(screen.getByRole("tab", { name: "หน่วยนับ" }));
    fireEvent.click(screen.getByRole("button", { name: "สร้างหน่วย" }));
    expect(screen.getByRole("combobox", { name: /มิติหน่วย/ })).toBeInTheDocument();
    expect(screen.getByRole("option", { name: "ความยาว" })).toBeInTheDocument();
    expect(screen.getByRole("option", { name: "ปริมาตร" })).toBeInTheDocument();
    expect(screen.getByText("กำหนดการปัดปริมาณให้ตรงกับจำนวนตำแหน่งทศนิยมของหน่วย")).toBeInTheDocument();
  });

  it("preserves unit precision and rounding policy when editing other fields", async () => {
    renderPage();
    fireEvent.click(screen.getByRole("tab", { name: "หน่วยนับ" }));
    const unitRow = screen.getByText("M3").closest("tr");
    if (!unitRow) throw new Error("Unit row not found");
    fireEvent.click(within(unitRow).getByRole("button", { name: "แก้ไข" }));
    expect(screen.getByDisplayValue("3")).toBeInTheDocument();
    expect(screen.getByRole("combobox", { name: /วิธีปัดเศษ/ })).toHaveValue("half_up");
    fireEvent.change(screen.getByRole("combobox", { name: /วิธีปัดเศษ/ }), { target: { value: "half_even" } });
    fireEvent.change(screen.getByPlaceholderText("ชื่อหน่วย (TH)"), { target: { value: "ลูกบาศก์เมตรปรับปรุง" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));

    await waitFor(() => expect(updateUnit).toHaveBeenCalledOnce());
    expect(updateUnit).toHaveBeenCalledWith(expect.objectContaining({
      id: "unit-1",
      rowVersion: "unit-version-1",
      payload: expect.objectContaining({ decimalScale: 3, roundingMode: "half_even", dimension: "volume", symbol: "m³" }),
    }));
  });

  it("preserves Category parent, allowed Item Type, description and sort order when editing", async () => {
    renderPage();
    const categoryRow = screen.getByRole("row", { name: /MAT.*ชิ้นส่วน/ });
    fireEvent.click(within(categoryRow).getByRole("button", { name: "แก้ไข" }));
    expect(screen.getByRole("checkbox", { name: "สินค้า" })).toBeChecked();
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลอ้างอิง" }));

    await waitFor(() => expect(updateCategory).toHaveBeenCalledOnce());
    expect(updateCategory).toHaveBeenCalledWith(expect.objectContaining({
      id: "category-1",
      rowVersion: "version-1",
      payload: expect.objectContaining({
        parentCategoryId: "parent-1",
        allowedItemTypes: ["product"],
        description: { thai: "คำอธิบาย", english: "Description" },
        sortOrder: 4,
      }),
    }));
  });
  it("keeps draft values when cancelling the discard confirmation", async () => {
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: "สร้างหมวดหมู่" }));
    const drawer = within(screen.getByRole("dialog"));
    fireEvent.change(drawer.getByPlaceholderText("ชื่อหมวดหมู่ (TH)"), { target: { value: "งานไม้" } });
    fireEvent.click(drawer.getByRole("button", { name: "ปิด" }));
    expect(screen.getAllByRole("dialog")).toHaveLength(2);
    const confirmation = within(screen.getAllByRole("dialog")[1]);
    fireEvent.click(confirmation.getByRole("button", { name: "ยกเลิก" }));
    expect(screen.getAllByRole("dialog")).toHaveLength(1);
    expect(drawer.getByPlaceholderText("ชื่อหมวดหมู่ (TH)")).toHaveValue("งานไม้");
    fireEvent.keyDown(document, { key: "Escape" });
    fireEvent.click(await screen.findByRole("button", { name: "ละทิ้งข้อมูล" }));
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(createCategory).not.toHaveBeenCalled();
  });

  it("opens Category Specification Templates drawer and saves configured templates", async () => {
    const setTemplates = vi.fn().mockResolvedValue({
      categoryId: "category-1",
      templates: [],
    });
    vi.mocked(useCategoryAttributeTemplateMutations).mockReturnValue({
      setTemplates: { mutateAsync: setTemplates, isPending: false, isError: false },
    } as unknown as ReturnType<typeof useCategoryAttributeTemplateMutations>);

    renderPage();
    const categoryRow = screen.getByRole("row", { name: /MAT.*ชิ้นส่วน/ });
    const templateBtn = within(categoryRow).getByRole("button", { name: "จัดการเทมเพลตสเปก" });
    fireEvent.click(templateBtn);

    // Verify Drawer is open
    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText(/เทมเพลตสเปก \(MAT\)/)).toBeInTheDocument();

    // Click Add Specification Template
    const addBtns = screen.getAllByRole("button", { name: /\+ เพิ่มสเปกในเทมเพลต/ });
    fireEvent.click(addBtns[0]);

    // Fill in Key and Name TH
    fireEvent.change(screen.getByPlaceholderText("e.g. thickness_mm"), {
      target: { value: "core_material" },
    });
    fireEvent.change(screen.getByPlaceholderText("เช่น ความหนา"), {
      target: { value: "วัสดุแกน" },
    });

    // Click Save
    const saveBtn = screen.getByRole("button", { name: "บันทึกเทมเพลตสเปก" });
    fireEvent.click(saveBtn);

    await waitFor(() => expect(setTemplates).toHaveBeenCalledOnce());
    expect(setTemplates).toHaveBeenCalledWith({
      templates: [
        expect.objectContaining({
          key: "core_material",
          name: expect.objectContaining({
            thai: "วัสดุแกน",
          }),
          dataType: "text",
        }),
      ],
    });
  });
});
