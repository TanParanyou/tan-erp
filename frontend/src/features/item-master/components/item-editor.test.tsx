import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messagesTh from "@/messages/th.json";
import type { ItemResponse } from "@/lib/api/api-client";
import { ItemEditor } from "./item-editor";
import { useCategoryAttributeTemplate, useItemDetail, useItemMasterLookups, useItemMasterMutations, useOrganizationBranches } from "@/features/item-master/api/item-master-queries";

const state = vi.hoisted(() => ({
  permissions: ["items.create", "items.update"],
  router: { push: vi.fn(), replace: vi.fn() },
  itemQuery: { data: undefined as ItemResponse | undefined, isLoading: false, isError: false, error: new Error("load failed"), refetch: vi.fn() },
  categoryTemplateQuery: { data: undefined, isLoading: false, isError: false },
  mutations: {
    createItem: { mutateAsync: vi.fn(), isPending: false, isError: false },
    attachItemImage: { mutateAsync: vi.fn(), isPending: false, isError: false },
    updateItem: { mutateAsync: vi.fn(), isPending: false, isError: false },
    activateItem: { mutateAsync: vi.fn(), isPending: false },
    deactivateItem: { mutateAsync: vi.fn(), isPending: false },
    setBranchAvailability: { isPending: false, isError: false },
  },
}));

vi.mock("next/navigation", () => ({ useRouter: () => state.router }));
vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useItemDetail: vi.fn(), useItemMasterLookups: vi.fn(), useItemMasterMutations: vi.fn(), useOrganizationBranches: vi.fn(),
  useCategoryAttributeTemplate: vi.fn(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({ useSelectedMembership: () => ({ selectedMembership: { id: "membership-1", permissions: state.permissions } }) }));
vi.mock("@/lib/permissions/can", () => ({ can: (_membership: unknown, permission: string) => state.permissions.includes(permission) }));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: vi.fn(), error: vi.fn() } }) }));
vi.mock("@/lib/auth/auth-session", () => ({ getAuthToken: vi.fn().mockResolvedValue("token") }));
vi.mock("@/features/item-master/api/upload-item-image", () => ({ uploadVerifiedItemImage: vi.fn().mockResolvedValue("verified-file-1") }));
vi.mock("@/components/forms/ImageUpload", () => ({ ImageUpload: ({ label, onChange, disabled }: { label: string; onChange: (file: File | null) => void; disabled?: boolean }) => <label>{label}<input type="file" aria-label={label} disabled={disabled} onChange={(event) => onChange(event.target.files?.[0] ?? null)} /></label> }));
vi.mock("@/features/item-master/components/item-barcode-maintenance", () => ({ ItemBarcodeMaintenance: () => null }));
vi.mock("@/features/item-master/components/item-alias-maintenance", () => ({ ItemAliasMaintenance: () => null }));
vi.mock("@/features/item-master/components/item-image-maintenance", () => ({ ItemImageMaintenance: () => null }));
vi.mock("@/features/item-master/components/cost-record-maintenance", () => ({ CostRecordMaintenance: () => null }));

const categoryId = "11111111-1111-4111-8111-111111111111";
const unitId = "22222222-2222-4222-8222-222222222222";

function renderEditor(id = "create") {
  const messages = messagesTh;
  return render(<NextIntlClientProvider locale="th" messages={messages}><ItemEditor id={id} /></NextIntlClientProvider>);
}

describe("ItemEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    state.permissions = ["items.create", "items.update"];
    state.itemQuery = { data: undefined, isLoading: false, isError: false, error: new Error("load failed"), refetch: vi.fn() };
    Object.assign(state.mutations.createItem, { mutateAsync: vi.fn().mockResolvedValue({ id: "new-item" }), isPending: false, isError: false });
    Object.assign(state.mutations.attachItemImage, { mutateAsync: vi.fn().mockResolvedValue({ id: "image-1" }), isPending: false, isError: false });
    Object.assign(state.mutations.updateItem, { mutateAsync: vi.fn().mockResolvedValue({ id: "item-1" }), isPending: false, isError: false });
    vi.mocked(useItemDetail).mockImplementation(() => state.itemQuery as unknown as ReturnType<typeof useItemDetail>);
    vi.mocked(useItemMasterLookups).mockReturnValue({
      data: {
        categories: [
          { id: categoryId, code: "MAT", name: { thai: "วัสดุ", english: "Material" }, status: "active", allowedItemTypes: ["material"] },
          { id: "product-category", code: "FG", name: { thai: "สินค้าสำเร็จรูป", english: "Finished goods" }, status: "active", allowedItemTypes: ["product"] },
        ],
        brands: [],
        units: [{ id: unitId, code: "EA", name: { thai: "ชิ้น", english: "Each" }, status: "active" }],
      }, isLoading: false, isError: false,
    } as unknown as ReturnType<typeof useItemMasterLookups>);
    vi.mocked(useOrganizationBranches).mockReturnValue({ data: [], isLoading: false, isError: false } as unknown as ReturnType<typeof useOrganizationBranches>);
    vi.mocked(useItemMasterMutations).mockReturnValue(state.mutations as unknown as ReturnType<typeof useItemMasterMutations>);
    vi.mocked(useCategoryAttributeTemplate).mockReturnValue(state.categoryTemplateQuery as unknown as ReturnType<typeof useCategoryAttributeTemplate>);
  });

  it("round-trips existing capabilities and tax category while updating name with the current ETag", async () => {
    state.itemQuery.data = {
      id: "item-1", code: "MAT-1", rowVersion: "etag-v4", itemType: "material", status: "draft",
      category: { id: categoryId }, baseUnit: { id: unitId }, name: { thai: "ไม้", english: "Wood" },
      capabilities: { canSell: true, canCost: true, canPurchase: false, canStock: true, canProduce: false },
      taxCategoryCode: "MATERIAL", availabilityMode: "all_branches", branchAvailabilities: [], attributes: { thickness: "18mm" },
    } as unknown as ItemResponse;
    renderEditor("item-1");
    await screen.findByDisplayValue("ไม้");
    fireEvent.change(screen.getByPlaceholderText("ชื่อสินค้า (TH)"), { target: { value: "ไม้อัด" } });
    fireEvent.click(screen.getByRole("tab", { name: "การใช้งานและคุณสมบัติ" }));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกสินค้า" }));

    await waitFor(() => expect(state.mutations.updateItem.mutateAsync).toHaveBeenCalledOnce());
    expect(state.mutations.updateItem.mutateAsync).toHaveBeenCalledWith(expect.objectContaining({ id: "item-1", rowVersion: "etag-v4" }));
    expect(state.mutations.updateItem.mutateAsync.mock.calls[0]?.[0].payload).toMatchObject({
      capabilities: { canSell: true, canCost: true, canPurchase: false, canStock: true, canProduce: false },
      taxCategoryCode: "MATERIAL", attributes: { thickness: "18mm" },
    });
  });

  it("marks and opens the first tab with validation errors", async () => {
    renderEditor();
    fireEvent.click(screen.getByRole("button", { name: "บันทึกสินค้า" }));
    const tab = screen.getByRole("tab", { name: /ข้อมูลสินค้า/ });
    await waitFor(() => expect(tab).toHaveAttribute("aria-selected", "true"));
    expect(tab.querySelector('[aria-label="has error"]')).not.toBeNull();
  });

  it("blocks save when the membership lacks create permission", () => {
    state.permissions = ["items.read"];
    renderEditor();
    expect(screen.queryByRole("button", { name: "บันทึกสินค้า" })).toBeNull();
  });

  it("defers image upload until submit and retries attachment without creating a duplicate item", async () => {
    const { uploadVerifiedItemImage } = await import("@/features/item-master/api/upload-item-image");
    state.permissions = ["items.create", "items.manage-images"];
    vi.mocked(state.mutations.attachItemImage.mutateAsync).mockRejectedValueOnce(new Error("attach failed"));
    renderEditor();
    fireEvent.change(screen.getAllByRole("textbox")[0] as HTMLInputElement, { target: { value: "MAT-IMAGE" } });
    fireEvent.focus(screen.getByRole("combobox", { name: /หมวดหมู่/ }));
    fireEvent.click(await screen.findByText("MAT · วัสดุ"));
    fireEvent.change(screen.getByLabelText(/หน่วย/), { target: { value: unitId } });
    fireEvent.change(screen.getByPlaceholderText("ชื่อสินค้า (TH)"), { target: { value: "สินค้าทดสอบ" } });
    fireEvent.click(screen.getByRole("tab", { name: "รายละเอียดเสริม" }));
    const image = new File(["image"], "item.png", { type: "image/png" });
    fireEvent.change(screen.getByLabelText("ไฟล์รูปภาพ"), { target: { files: [image] } });
    fireEvent.change(document.querySelector("input[id^=\"input-ข้อความอธิบายรูปภาพ\"]") as HTMLInputElement, { target: { value: "ภาพสินค้า" } });

    expect(uploadVerifiedItemImage).not.toHaveBeenCalled();
    fireEvent.click(screen.getByRole("button", { name: "บันทึกสินค้า" }));
    await screen.findByRole("button", { name: "ลองบันทึกรูปภาพอีกครั้ง" });
    expect(state.mutations.createItem.mutateAsync).toHaveBeenCalledOnce();
    expect(uploadVerifiedItemImage).toHaveBeenCalledOnce();

    fireEvent.click(screen.getByRole("button", { name: "ลองบันทึกรูปภาพอีกครั้ง" }));
    await waitFor(() => expect(state.mutations.attachItemImage.mutateAsync).toHaveBeenCalledTimes(2));
    expect(state.mutations.createItem.mutateAsync).toHaveBeenCalledOnce();
    expect(uploadVerifiedItemImage).toHaveBeenCalledOnce();
    expect(state.router.replace).toHaveBeenCalledWith("/th/item-master/new-item");
  });

  it("filters category autocomplete choices to the selected Item Type", async () => {
    renderEditor();
    const categoryInput = screen.getByRole("combobox", { name: /หมวดหมู่/ });
    fireEvent.focus(categoryInput);
    expect(await screen.findByText("MAT · วัสดุ")).toBeInTheDocument();
    expect(screen.queryByText("FG · สินค้าสำเร็จรูป")).toBeNull();
    fireEvent.change(screen.getByLabelText(/ประเภทสินค้า/), { target: { value: "product" } });
    fireEvent.focus(categoryInput);
    expect(await screen.findByText("FG · สินค้าสำเร็จรูป")).toBeInTheDocument();
    expect(screen.queryByText("MAT · วัสดุ")).toBeNull();
  });

  it("keeps reference creation on its dedicated maintenance screen", () => {
    state.permissions = ["items.create", "items.update", "items.manage-taxonomy"];
    renderEditor();
    expect(screen.queryByRole("button", { name: "สร้างหมวดหมู่" })).toBeNull();
    expect(screen.queryByRole("button", { name: "สร้างแบรนด์" })).toBeNull();
    expect(screen.queryByRole("button", { name: "สร้างหน่วย" })).toBeNull();
  });

  it("shows minimal loading and retryable detail errors", () => {
    state.itemQuery.isLoading = true;
    const loading = renderEditor("item-1");
    expect(screen.getByText("กำลังโหลดข้อมูลสินค้า")).toBeInTheDocument();
    loading.unmount();
    state.itemQuery.isLoading = false;
    state.itemQuery.isError = true;
    renderEditor("item-1");
    expect(screen.getByRole("alert")).toHaveTextContent("load failed");
    fireEvent.click(screen.getByRole("button", { name: "ลองใหม่อีกครั้ง" }));
    expect(state.itemQuery.refetch).toHaveBeenCalledOnce();
  });

  it("keeps submitted values after a rejected update and prevents a second submit while pending", async () => {
    let complete: (() => void) | undefined;
    state.itemQuery.data = {
      id: "item-1", code: "MAT-1", rowVersion: "etag-v4", itemType: "material", status: "draft",
      category: { id: categoryId }, baseUnit: { id: unitId }, name: { thai: "ไม้" }, capabilities: { canSell: false, canCost: true, canPurchase: true, canStock: false, canProduce: false },
      availabilityMode: "all_branches", branchAvailabilities: [],
    } as unknown as ItemResponse;
    state.mutations.updateItem.mutateAsync.mockImplementationOnce(() => new Promise<void>((resolve) => { complete = resolve; }));
    renderEditor("item-1");
    await screen.findByDisplayValue("ไม้");
    fireEvent.change(screen.getByPlaceholderText("ชื่อสินค้า (TH)"), { target: { value: "ไม้อัด" } });
    const save = screen.getByRole("button", { name: "บันทึกสินค้า" });
    fireEvent.click(save);
    await waitFor(() => expect(state.mutations.updateItem.mutateAsync).toHaveBeenCalledOnce());
    fireEvent.click(save);
    expect(state.mutations.updateItem.mutateAsync).toHaveBeenCalledOnce();
    complete?.();
    await waitFor(() => expect(screen.getByPlaceholderText("ชื่อสินค้า (TH)")).toHaveValue("ไม้อัด"));
  });

  it("adds and updates specifications via quick presets and custom input", async () => {
    state.itemQuery.data = {
      id: "item-1", code: "MAT-1", rowVersion: "etag-v4", itemType: "material", status: "draft",
      category: { id: categoryId }, baseUnit: { id: unitId }, name: { thai: "ไม้อัด" },
      capabilities: { canSell: true, canCost: true, canPurchase: false, canStock: true, canProduce: false },
      availabilityMode: "all_branches", branchAvailabilities: [],
    } as unknown as ItemResponse;
    renderEditor("item-1");
    await screen.findByDisplayValue("ไม้อัด");
    fireEvent.click(screen.getByRole("tab", { name: "การใช้งานและคุณสมบัติ" }));

    // Click quick preset +สี (color)
    const colorPreset = screen.getByRole("button", { name: /\+สี/ });
    fireEvent.click(colorPreset);

    // Find select input for color and choose "white_matte"
    const colorSelects = screen.getAllByRole("combobox");
    const lastSelect = colorSelects[colorSelects.length - 1] as HTMLSelectElement;
    fireEvent.change(lastSelect, { target: { value: "white_matte" } });

    fireEvent.click(screen.getByRole("button", { name: "บันทึกสินค้า" }));

    await waitFor(() => expect(state.mutations.updateItem.mutateAsync).toHaveBeenCalledOnce());
    expect(state.mutations.updateItem.mutateAsync.mock.calls[0]?.[0].payload).toMatchObject({
      attributes: { color: "white_matte" },
    });
  });
});
