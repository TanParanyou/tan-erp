import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import type { CatalogItemModel } from "../api/estimate-catalog-client";
import { EstimateWorkspaceDrawer } from "./estimate-workspace-drawer";

const mocks = vi.hoisted(() => ({ update: vi.fn(), calculate: vi.fn(), confirm: vi.fn(), success: vi.fn(), error: vi.fn() }));
vi.mock("../api/estimate-queries", () => ({
  useUpdateEstimateDraft: () => ({ isPending: false, mutateAsync: mocks.update }),
  useCalculateEstimate: () => ({ isPending: false, mutateAsync: mocks.calculate }),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: mocks.error } }) }));
vi.mock("@/hooks/useConfirm", () => ({ useConfirm: () => ({ confirm: mocks.confirm, ConfirmDialog: () => null }) }));
vi.mock("./estimate-template-modal", () => ({ EstimateTemplateModal: () => null }));
const catalogItem: CatalogItemModel = {
  id: "catalog-item", code: "MAT-1", name: { thai: "วัสดุจากคลัง", english: "Catalog material" }, itemType: "material", costComponentType: "material",
  category: { id: "category", code: "CAT", name: { thai: "วัสดุ" } },
  baseUnit: { id: "unit", code: "sheet", name: { thai: "แผ่น" }, symbol: "sheet" },
  resolvedCost: { costRecordId: "cost-record", version: 3, amount: 150, currency: "THB", unitCode: "sheet", scope: "organization", effectiveFromUtc: "2026-09-28T00:00:00Z", costSourceId: "source", sourceReference: "supplier-quote" },
};
vi.mock("./estimate-item-catalog-modal", () => ({
  EstimateItemCatalogModal: ({ isOpen, onClose, onSelectItems }: { isOpen: boolean; onClose: () => void; onSelectItems: (items: CatalogItemModel[]) => void }) => isOpen ? (
    <div role="dialog" aria-label="catalog fixture"><button onClick={() => { onSelectItems([catalogItem]); onClose(); }}>insert catalog fixture</button><button onClick={onClose}>close catalog fixture</button></div>
  ) : null,
}));

const estimate: EstimateDetailResponse = {
  id: "estimate", branchId: "branch", number: "EST-TEST",
  currentRevision: {
    id: "revision", rowVersion: "version-1", revisionNo: 1, status: "draft", currency: "THB", discountType: "none", discountValue: 0,
    sections: [{ id: "section", code: "SEC", nameTh: "หมวดงาน", nameEn: "Section", sortOrder: 1, workItems: [
      { id: "work-a", code: "WORK-A", descriptionTh: "งานหนึ่ง", descriptionEn: "First work", quantity: 1, unitCode: "lot", sellingRuleType: "margin", sellingRuleValue: 0.4, sortOrder: 1,
        overrideReasonCode: "test-custom-work-item", overrideReason: "Test fixture custom item rationale",
        costComponents: [{ id: "cost-a", type: "material", description: "ต้นทุนหนึ่ง", quantity: 1, unitCode: "lot", unitCost: 100, currency: "THB", sortOrder: 1, isProvisional: false }] },
      { id: "work-b", code: "WORK-B", descriptionTh: "งานสอง", descriptionEn: "Second work", quantity: 1, unitCode: "lot", sellingRuleType: "margin", sellingRuleValue: 0.1, sortOrder: 2,
        overrideReasonCode: "test-custom-work-item", overrideReason: "Test fixture custom item rationale",
        costComponents: [{ id: "cost-b", type: "material", description: "ต้นทุนสอง", quantity: 1, unitCode: "lot", unitCost: 50, currency: "THB", sortOrder: 1, isProvisional: false }] },
    ] }],
  },
};
function renderWorkspace(focusTargetId?: string) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(<QueryClientProvider client={client}>
    <style>{'.hidden { display: none; } .hidden[class*="min-[992px]:block"] { display: block; }'}</style>
    <EstimateWorkspaceDrawer isOpen onClose={vi.fn()} estimate={estimate} focusTargetId={focusTargetId} />
  </QueryClientProvider>);
}
const choose = (code: string) => fireEvent.click(screen.getByRole("button", { name: `แก้ไขรายการ ${code}` }));
const tab = (name: string) => fireEvent.click(screen.getByRole("tab", { name: new RegExp(`^${name}`) }));
const save = () => fireEvent.click(screen.getByRole("button", { name: /บันทึกฉบับร่าง/ }));

describe("workspace BOQ and inspector regression", () => {
  beforeEach(() => { vi.clearAllMocks(); mocks.confirm.mockResolvedValue(true); mocks.update.mockImplementation(() => new Promise(() => {})); });

  it("retains edits through switching tabs, filtering and collapsing sections", async () => {
    renderWorkspace();
    choose("WORK-B");
    fireEvent.change(screen.getByRole("textbox", { name: "รายละเอียดงาน" }), { target: { value: "ข้อมูลที่แก้ไว้" } });
    tab("ราคาขาย");
    fireEvent.change(screen.getByRole("spinbutton", { name: "ค่ากำหนด" }), { target: { value: "15" } });
    tab("ข้อมูลรายการ");
    expect(screen.getByRole("textbox", { name: "รายละเอียดงาน" })).toHaveValue("ข้อมูลที่แก้ไว้");
    fireEvent.change(screen.getByRole("textbox", { name: /ค้นหารายการ/ }), { target: { value: "WORK-A" } });
    expect(screen.queryByRole("button", { name: "แก้ไขรายการ WORK-B" })).not.toBeInTheDocument();
    expect(screen.getByText("รายการที่กำลังแก้ไม่อยู่ในผลค้นหาหรือตัวกรองปัจจุบัน")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /ยุบ/ }));
    save();
    await waitFor(() => expect(mocks.update).toHaveBeenCalled());
    expect(mocks.update.mock.calls[0][0].payload.sections[0].workItems[1]).toMatchObject({ descriptionTh: "ข้อมูลที่แก้ไว้", sellingRuleValue: 0.15 });
  });

  it("keeps the selected item and its entered values when reordered", async () => {
    renderWorkspace(); choose("WORK-B");
    fireEvent.change(screen.getByRole("textbox", { name: "รายละเอียดงาน" }), { target: { value: "ย้ายขึ้นพร้อมข้อมูล" } });
    fireEvent.click(screen.getByRole("button", { name: /เลื่อนขึ้น/ }));
    expect(screen.getByRole("textbox", { name: "รายละเอียดงาน" })).toHaveValue("ย้ายขึ้นพร้อมข้อมูล");
    tab("ต้นทุน");
    expect(screen.getByRole("textbox", { name: "รายละเอียดต้นทุน" })).toHaveValue("ต้นทุนสอง");
    fireEvent.change(screen.getByRole("spinbutton", { name: "ต้นทุนต่อหน่วย" }), { target: { value: "75" } });
    save();
    await waitFor(() => expect(mocks.update).toHaveBeenCalled());
    expect(mocks.update.mock.calls[0][0].payload.sections[0].workItems).toMatchObject([
      { id: "work-b", descriptionTh: "ย้ายขึ้นพร้อมข้อมูล", costComponents: [{ id: "cost-b", unitCost: 75 }] },
      { id: "work-a", costComponents: [{ id: "cost-a", unitCost: 100 }] },
    ]);
  });

  it("inserts catalog references into the selected work item and retains them when duplicated", async () => {
    renderWorkspace(); choose("WORK-B");
    fireEvent.click(screen.getByRole("button", { name: /เปิดคลังรายการสินค้า/ }));
    fireEvent.click(screen.getByRole("button", { name: "insert catalog fixture" }));
    tab("ต้นทุน");
    expect(screen.getByDisplayValue("วัสดุจากคลัง")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /คัดลอกรายการ/ }));
    save();
    await waitFor(() => expect(mocks.update).toHaveBeenCalled());
    const works = mocks.update.mock.calls[0][0].payload.sections[0].workItems;
    expect(works[0].costComponents).toHaveLength(1);
    expect(works[1].costComponents[1]).toMatchObject({ itemId: "catalog-item", costRecordId: "cost-record", costRecordVersion: 3, unitCode: "sheet", unitCost: 150 });
    expect(works[2].id).toBeNull();
    expect(works[2].costComponents[1]).toMatchObject({ id: null, itemId: "catalog-item", costRecordVersion: 3 });
  });

  it("keeps quick cost presets and selects a remaining item after confirmed deletion", async () => {
    renderWorkspace(); choose("WORK-B"); tab("ต้นทุน");
    const preset = screen.getByRole("button", { name: /ไม้อัด HMR/ });
    fireEvent.click(preset);
    expect(screen.getAllByRole("spinbutton", { name: "ต้นทุนต่อหน่วย" })).toHaveLength(2);
    fireEvent.click(screen.getAllByRole("button", { name: "ลบรายการ" })[0]);
    await waitFor(() => expect(screen.getByRole("textbox", { name: "รายละเอียดงาน" })).toHaveValue("งานหนึ่ง"));
    expect(mocks.confirm).toHaveBeenCalled();
  });

  it("opens cost readiness targets even though the inspector initially shows another item", async () => {
    renderWorkspace("cost-b");
    await waitFor(() => expect(screen.getByRole("textbox", { name: "รายละเอียดต้นทุน" })).toHaveValue("ต้นทุนสอง"));
    expect(document.getElementById("estimate-target-cost-b")).toBeInTheDocument();
  });
});
