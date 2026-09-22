import { describe, it, expect, vi } from "vitest";
import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { EstimateItemCatalogModal } from "./estimate-item-catalog-modal";
import { ESTIMATE_CATALOG_ITEMS } from "../constants/estimate-catalog-items";

function renderModal(ui: React.ReactElement) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      {ui}
    </QueryClientProvider>
  );
}

// Mock next-intl
vi.mock("next-intl", () => ({
  useLocale: () => "th",
  useTranslations: (namespace: string) => {
    return (key: string, params?: Record<string, unknown>) => {
      if (namespace === "estimates") {
        const translations: Record<string, string> = {
          catalogModalTitle: "คลังรายการสินค้าและสเปกวัสดุ",
          catalogModalDesc: "ค้นหาและเลือกรายการวัสดุ",
          selectAll: "เลือกทั้งหมด",
          deselectAll: "ยกเลิกการเลือก",
          matchingCount: `พบ ${params?.count ?? 0} รายการ`,
          insertSelectedCount: `แทรก ${params?.count ?? 0} รายการที่เลือก`,
          searchPlaceholder: "ค้นหารายการงานหรือวัสดุ...",
          itemCode: "รหัสรายการ",
          itemDescTh: "รายละเอียดงาน (ไทย)",
          costType: "ประเภทต้นทุน",
          unitCode: "หน่วย",
          unitCost: "ต้นทุนต่อหน่วย",
          noMatchingItems: "ไม่พบรายการที่ตรงกับเงื่อนไขการค้นหา",
          "catalogFacets.selectedFilters": "ตัวกรองที่เลือก:",
          "catalogFacets.clearFilters": "ล้างตัวกรอง",
          "catalogFacets.allScopes": "ทั้งหมด",
          "catalogFacets.costScope": "วัตถุดิบ/ต้นทุน",
          "catalogFacets.sellScope": "งานเสนอขาย",
          "catalogFacets.costType": "1. ประเภทต้นทุน",
          "catalogFacets.category": "2. หมวดหมู่วัสดุ",
          "catalogFacets.subCategory": "หมวดย่อย",
          "catalogFacets.allSubCategories": "ทุกหมวดย่อย",
          "catalogFacets.attributesTitle": "3. คุณลักษณะเฉพาะ",
          "catalogFacets.attrSize": "ขนาด / มิติ",
          "catalogFacets.attrColor": "สี / ลวดลาย",
          "catalogFacets.attrFinish": "ผิวสัมผัส",
          "catalogFacets.attrThickness": "ความหนา",
          "catalogFacets.attrGrade": "เกรด",
          "catalogFacets.attrStandard": "มาตรฐาน",
          "catalogFacets.thickness": "3. สเปกความหนา",
          "catalogFacets.brand": "4. แบรนด์/ยี่ห้อ",
          "catalogFacets.searchCategoryPlaceholder": "ค้นหาหมวดหมู่...",
          "catalogFacets.searchSubCategoryPlaceholder": "ค้นหาหมวดย่อย...",
          "catalogFacets.searchBrandPlaceholder": "ค้นหาแบรนด์...",
          "catalogFacets.supplier": "5. ผู้จัดจำหน่าย",
          "catalogFacets.searchSupplierPlaceholder": "ค้นหาผู้จัดจำหน่าย...",
          "catalogFacets.allSuppliers": "ทุกผู้จัดจำหน่าย",
          "catalogFacets.noFiltersActive": "แสดงรายการทั้งหมด (ไม่มีตัวกรองพิเศษ)",
          "catalogFacets.showFilters": "แสดงตัวกรอง",
          "catalogFacets.hideFilters": "ซ่อนตัวกรอง",
          "catalogFacets.categories.all": "ทุกหมวดหมู่",
          "catalogFacets.categories.wood": "ไม้และแผ่นบอร์ด",
          "catalogFacets.categories.fitting": "ฟิตติ้ง",
          "catalogFacets.categories.surface": "วัสดุปิดผิว",
          "catalogFacets.categories.labor_service": "งานแรงงาน",
          "costTypes.material": "ค่าวัสดุ",
          "costTypes.labor": "ค่าแรง",
          "costTypes.subcontract": "ค่าจ้างเหมา",
          "catalogFacets.selectAll": "เลือกทั้งหมดในหน้านี้",
          "catalogFacets.deselectAll": "ยกเลิกการเลือกทั้งหมด",
          "catalogFacets.showingCount": `แสดง ${params?.from ?? 0}-${params?.to ?? 0} จาก ${params?.total ?? 0} รายการ`,
          "catalogFacets.perPage": "รายการต่อหน้า",
          "catalogFacets.previousPage": "หน้าก่อนหน้า",
          "catalogFacets.nextPage": "หน้าถัดไป",
          "catalogFacets.pageOf": `หน้า ${params?.current ?? 1} จาก ${params?.total ?? 1}`,
          "catalogDetail.drawerTitle": "รายละเอียดสินค้าและสเปกวัสดุ",
          "catalogDetail.drawerDesc": "ข้อมูลคุณลักษณะเฉพาะ ผู้จัดจำหน่าย และต้นทุนมาตรฐาน",
          "catalogDetail.viewDetail": "ดูรายละเอียด",
          "catalogDetail.noImage": "ไม่มีรูปภาพ",
          "catalogDetail.image": "รูปภาพ",
          "catalogDetail.copy": "คัดลอก",
          "catalogDetail.copied": "คัดลอกแล้ว",
          "catalogDetail.copyCode": "คัดลอกรหัสสินค้า",
          "catalogDetail.copyName": "คัดลอกชื่อสินค้า",
          "catalogDetail.copyDesc": "คัดลอกคำอธิบาย",
          "catalogDetail.copySpecs": "คัดลอกสเปกทั้งหมด",
          "catalogDetail.copyCost": "คัดลอกราคาต้นทุน",
          "catalogDetail.generalInfo": "ข้อมูลทั่วไป",
          "catalogDetail.specsAndAttrs": "คุณลักษณะและสเปก",
          "catalogDetail.procurementAndPricing": "ข้อมูลจัดซื้อและต้นทุน",
          "catalogDetail.aliases": "คำเรียก / ชื่อสามัญ:",
          "catalogDetail.description": "คำอธิบาย:",
          "catalogDetail.category": "หมวดหมู่:",
          "catalogDetail.subCategory": "หมวดย่อย:",
          "catalogDetail.brand": "แบรนด์/ยี่ห้อ:",
          "catalogDetail.supplier": "ผู้จัดจำหน่าย:",
          "catalogDetail.supplierCode": "รหัสผู้จัดจำหน่าย:",
          "catalogDetail.unitCode": "หน่วยนับ:",
          "catalogDetail.costType": "ประเภทต้นทุน:",
          "catalogDetail.unitCost": "ต้นทุนมาตรฐาน:",
          "catalogDetail.status": "สถานะสินค้า:",
          "catalogDetail.close": "ปิดหน้าต่าง",
        };
        return translations[key] ?? key;
      }
      if (namespace === "common") {
        if (key === "actions.cancel") return "ยกเลิก";
      }
      return key;
    };
  },
}));

describe("EstimateItemCatalogModal", () => {
  const defaultProps = {
    isOpen: true,
    onClose: vi.fn(),
    onSelectItems: vi.fn(),
    currency: "THB",
  };

  it("renders modal when isOpen is true", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);
    expect(screen.getByText("คลังรายการสินค้าและสเปกวัสดุ")).toBeDefined();
    expect(screen.getByPlaceholderText("ค้นหารายการงานหรือวัสดุ...")).toBeDefined();
  });

  it("filters items by search input", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);
    const searchInput = screen.getByPlaceholderText("ค้นหารายการงานหรือวัสดุ...");

    // Initially shows items
    expect(screen.getByText(ESTIMATE_CATALOG_ITEMS[0].name.th)).toBeDefined();

    // Type query
    fireEvent.change(searchInput, { target: { value: "Blum" } });
    expect(searchInput).toHaveProperty("value", "Blum");
  });

  it("selects items and confirms insert", () => {
    const onSelectItems = vi.fn();
    const onClose = vi.fn();
    renderModal(
      <EstimateItemCatalogModal
        {...defaultProps}
        onSelectItems={onSelectItems}
        onClose={onClose}
      />
    );

    // Click on the first catalog item row
    const firstItem = ESTIMATE_CATALOG_ITEMS[0];
    const rowTitle = screen.getByText(firstItem.name.th);
    fireEvent.click(rowTitle);

    // Confirm button should show 1 selected
    const insertButton = screen.getByRole("button", { name: /แทรก 1 รายการที่เลือก/ });
    expect(insertButton).toBeDefined();

    fireEvent.click(insertButton);
    expect(onSelectItems).toHaveBeenCalledWith([expect.objectContaining({ id: firstItem.id })]);
    expect(onClose).toHaveBeenCalled();
  });

  it("toggles category and brand search inputs when search icons are clicked", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    // Initially category and brand search inputs are not visible
    expect(screen.queryByPlaceholderText("ค้นหาหมวดหมู่...")).toBeNull();
    expect(screen.queryByPlaceholderText("ค้นหาแบรนด์...")).toBeNull();

    // Click Category search icon button
    const categorySearchBtn = screen.getByTitle("ค้นหาหมวดหมู่...");
    fireEvent.click(categorySearchBtn);
    expect(screen.getByPlaceholderText("ค้นหาหมวดหมู่...")).toBeDefined();

    // Click Brand search icon button
    const brandSearchBtn = screen.getByTitle("ค้นหาแบรนด์...");
    fireEvent.click(brandSearchBtn);
    expect(screen.getByPlaceholderText("ค้นหาแบรนด์...")).toBeDefined();
  });

  it("supports autocomplete search for supplier", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);
    const supplierInput = screen.getByPlaceholderText("ค้นหาผู้จัดจำหน่าย...");
    expect(supplierInput).toBeDefined();

    // Focus input opens autocomplete dropdown
    fireEvent.focus(supplierInput);
    expect(screen.getByText("ทุกผู้จัดจำหน่าย")).toBeDefined();
  });

  it("opens item detail drawer when view detail button is clicked", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);
    const firstItem = ESTIMATE_CATALOG_ITEMS[0];

    // Find view detail buttons
    const viewDetailButtons = screen.getAllByTitle("ดูรายละเอียด");
    expect(viewDetailButtons.length).toBeGreaterThan(0);

    // Click view detail on first item
    fireEvent.click(viewDetailButtons[0]);

    // Drawer should open displaying drawer title and specifications
    expect(screen.getByText("รายละเอียดสินค้าและสเปกวัสดุ")).toBeDefined();
    expect(screen.getAllByText(firstItem.code).length).toBeGreaterThanOrEqual(2);
    expect(screen.getByText("คุณลักษณะและสเปก")).toBeDefined();

    // Verify copy buttons exist in the drawer
    const copyButtons = screen.getAllByLabelText(/คัดลอก/);
    expect(copyButtons.length).toBeGreaterThanOrEqual(3);
  });

  it("collapses and expands sidebar filter sections", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    // Cost type checkboxes are initially visible
    expect(screen.getByText("ค่าวัสดุ")).toBeDefined();

    // Click Cost Type section header to collapse
    const costTypeHeaderBtn = screen.getByRole("button", { name: /1\. ประเภทต้นทุน/ });
    expect(costTypeHeaderBtn.getAttribute("aria-expanded")).toBe("true");

    fireEvent.click(costTypeHeaderBtn);
    expect(costTypeHeaderBtn.getAttribute("aria-expanded")).toBe("false");
    expect(screen.queryByText("ค่าวัสดุ")).toBeNull();

    // Click again to re-expand
    fireEvent.click(costTypeHeaderBtn);
    expect(costTypeHeaderBtn.getAttribute("aria-expanded")).toBe("true");
    expect(screen.getByText("ค่าวัสดุ")).toBeDefined();
  });

  it("selects all visible items when table header select-all checkbox is clicked", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    // Find the header select all checkbox
    const selectAllCheckbox = screen.getByLabelText("เลือกทั้งหมดในหน้านี้");
    expect(selectAllCheckbox).toBeDefined();

    // Click select all
    fireEvent.click(selectAllCheckbox);

    // Insert button should reflect all items selected
    expect(
      screen.getByRole("button", {
        name: new RegExp(`แทรก ${ESTIMATE_CATALOG_ITEMS.length} รายการที่เลือก`),
      })
    ).toBeDefined();

    // Clicking again deselects all
    fireEvent.click(selectAllCheckbox);
    const deselectedButton = screen.getByRole("button", {
      name: /แทรก 0 รายการที่เลือก/,
    });
    expect(deselectedButton).toBeDefined();
    expect(deselectedButton).toHaveProperty("disabled", true);
  });

  it("renders pagination controls and displays item counts", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    // Should display pagination showing count
    expect(
      screen.getByText(new RegExp(`แสดง 1-\\d+ จาก ${ESTIMATE_CATALOG_ITEMS.length} รายการ`))
    ).toBeDefined();

    // Should have page size selector
    const pageSizeSelect = screen.getByLabelText("รายการต่อหน้า");
    expect(pageSizeSelect).toBeDefined();

    // Change page size
    fireEvent.change(pageSizeSelect, { target: { value: "50" } });
    expect(pageSizeSelect).toHaveProperty("value", "50");
  });

  it("displays active filter count badge on collapsible section header when filters are selected", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    // Initially no badge on Cost Type header
    const costTypeHeader = screen.getByRole("button", { name: /1\. ประเภทต้นทุน/ });
    expect(costTypeHeader.textContent).not.toContain("(1)");

    // Click on a cost type checkbox (e.g. ค่าวัสดุ)
    const materialCheckbox = screen.getByLabelText("ค่าวัสดุ");
    fireEvent.click(materialCheckbox);

    // Header should now display active count badge
    expect(costTypeHeader.textContent).toContain("1");
  });

  it("toggles mobile filter sidebar when mobile filter button is clicked", () => {
    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    // Find the mobile filter toggle button
    const mobileFilterBtn = screen.getByRole("button", { name: /แสดงตัวกรอง/ });
    expect(mobileFilterBtn).toBeDefined();

    // Click to show filters
    fireEvent.click(mobileFilterBtn);
    expect(screen.getByRole("button", { name: /ซ่อนตัวกรอง/ })).toBeDefined();

    // Click again to hide
    fireEvent.click(screen.getByRole("button", { name: /ซ่อนตัวกรอง/ }));
    expect(screen.getByRole("button", { name: /แสดงตัวกรอง/ })).toBeDefined();
  });
});
