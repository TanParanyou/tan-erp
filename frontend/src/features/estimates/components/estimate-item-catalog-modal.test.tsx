import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import { EstimateItemCatalogModal } from "./estimate-item-catalog-modal";
import { useEstimateCatalog } from "../hooks/use-estimate-catalog";
import type { CatalogItemModel, CatalogModel } from "../api/estimate-catalog-client";
import thMessages from "@/messages/th.json";

vi.mock("../hooks/use-estimate-catalog", () => ({
  useEstimateCatalog: vi.fn(),
}));

vi.mock("../hooks/use-private-item-image", () => ({
  usePrivateItemImage: vi.fn().mockReturnValue({
    imageUrl: null,
    isLoading: false,
    isError: false,
  }),
}));

const mockItems: CatalogItemModel[] = [
  {
    id: "item-1",
    code: "WD-001",
    name: {
      thai: "ไม้อัดสัก 4 มม.",
      english: "Teak Plywood 4mm",
    },
    description: {
      thai: "เกรด A สำหรับงานตกแต่งภายใน",
      english: "Grade A for interior design",
    },
    itemType: "material",
    category: {
      id: "cat-1",
      code: "CAT-WD",
      name: { thai: "งานไม้", english: "Woodwork" },
      parentCategoryId: null,
    },
    brand: {
      id: "brand-1",
      code: "BR-VAN",
      name: { thai: "วนชัย", english: "Vanachai" },
    },
    baseUnit: {
      id: "u-sheet",
      code: "SHEET",
      name: { thai: "แผ่น", english: "Sheet" },
      symbol: "แผ่น",
    },
    resolvedCost: {
      costRecordId: "cost-1",
      version: 1,
      amount: 450,
      currency: "THB",
      unitCode: "SHEET",
      scope: "branch",
      effectiveFromUtc: "2026-01-01T00:00:00Z",
      policyVersion: "1.0",
    },
  },
  {
    id: "item-2",
    code: "LB-001",
    name: {
      thai: "ค่าแรงติดตั้งโครงไม้",
      english: "Carpentry Framework Labor",
    },
    itemType: "labor",
    category: {
      id: "cat-2",
      code: "CAT-LB",
      name: { thai: "งานแรงงาน", english: "Labor" },
      parentCategoryId: null,
    },
    baseUnit: {
      id: "u-m",
      code: "M",
      name: { thai: "เมตร", english: "Meter" },
      symbol: "ม.",
    },
  },
];

const mockCatalogData: CatalogModel = {
  items: mockItems,
  facets: {
    itemTypes: [
      { value: "material", count: 1 },
      { value: "labor", count: 1 },
    ],
    categories: [
      {
        id: "cat-1",
        name: { thai: "งานไม้", english: "Woodwork" },
        count: 1,
      },
      {
        id: "cat-2",
        name: { thai: "งานแรงงาน", english: "Labor" },
        count: 1,
      },
    ],
    brands: [
      {
        id: "brand-1",
        name: { thai: "วนชัย", english: "Vanachai" },
        count: 1,
      },
    ],
  },
  pageInfo: {
    nextCursor: "cursor-page-2",
    hasNextPage: true,
  },
};

function renderModal(ui: React.ReactElement) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      {ui}
    </NextIntlClientProvider>
  );
}

describe("EstimateItemCatalogModal (Server-State Driven)", () => {
  const defaultProps = {
    isOpen: true,
    onClose: vi.fn(),
    onSelectItems: vi.fn(),
    currency: "THB",
    branchId: "branch-uuid-1",
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders branch guard warning when branchId is missing", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} branchId={undefined} />);

    expect(screen.getByText("กรุณาระบุสาขาก่อนค้นหาคลังวัสดุ")).toBeInTheDocument();
  });

  it("renders loading spinner while catalog query is pending", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    expect(screen.getByText("กำลังโหลดรายการจากคลังวัสดุ...")).toBeInTheDocument();
  });

  it("renders error state and handles retry button click", () => {
    const mockRefetch = vi.fn();
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    expect(screen.getByText("เกิดข้อผิดพลาดในการโหลดรายการคลังวัสดุ")).toBeInTheDocument();
    const retryBtn = screen.getByRole("button", { name: /ลองใหม่/i });
    fireEvent.click(retryBtn);
    expect(mockRefetch).toHaveBeenCalledTimes(1);
  });

  it("renders empty state when no matching items returned", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: {
        items: [],
        facets: { itemTypes: [], categories: [], brands: [] },
        pageInfo: { nextCursor: null, hasNextPage: false },
      },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    expect(screen.getByText("ไม่พบรายการที่ตรงกับเงื่อนไขการค้นหา")).toBeInTheDocument();
  });

  it("renders items with localized names, code, type badge, and resolved cost", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: mockCatalogData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    expect(screen.getByText("WD-001")).toBeInTheDocument();
    expect(screen.getByText("ไม้อัดสัก 4 มม.")).toBeInTheDocument();
    expect(screen.getAllByText("วนชัย").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByText("450.00")).toBeInTheDocument();
    expect(screen.getByText("/ SHEET")).toBeInTheDocument();

    expect(screen.getByText("LB-001")).toBeInTheDocument();
    expect(screen.getByText("ค่าแรงติดตั้งโครงไม้")).toBeInTheDocument();
  });

  it("keeps confirm button disabled when no items are selected, enables on selection", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: mockCatalogData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    const confirmBtn = screen.getByRole("button", { name: /แทรก 0 รายการที่เลือก/i });
    expect(confirmBtn).toBeDisabled();

    // Toggle first item
    const firstItemRow = screen.getByText("WD-001").closest("tr");
    expect(firstItemRow).not.toBeNull();
    fireEvent.click(firstItemRow!);

    expect(screen.getByRole("button", { name: /แทรก 1 รายการที่เลือก/i })).toBeEnabled();
  });

  it("supports toggle select all on current page", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: mockCatalogData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    const selectAllBtn = screen.getByRole("button", { name: "เลือกทั้งหมด" });
    fireEvent.click(selectAllBtn);

    expect(screen.getByRole("button", { name: /แทรก 2 รายการที่เลือก/i })).toBeEnabled();

    // Deselect all
    const deselectBtn = screen.getByRole("button", { name: "ยกเลิกการเลือก" });
    fireEvent.click(deselectBtn);

    expect(screen.getByRole("button", { name: /แทรก 0 รายการที่เลือก/i })).toBeDisabled();
  });

  it("confirms selection and passes selected CatalogItemModel[] to onSelectItems", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: mockCatalogData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    const firstItemRow = screen.getByText("WD-001").closest("tr");
    fireEvent.click(firstItemRow!);

    const confirmBtn = screen.getByRole("button", { name: /แทรก 1 รายการที่เลือก/i });
    fireEvent.click(confirmBtn);

    expect(defaultProps.onSelectItems).toHaveBeenCalledWith([mockItems[0]]);
    expect(defaultProps.onClose).toHaveBeenCalled();
  });

  it("triggers search input change and calls useEstimateCatalog with debounced search", async () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: mockCatalogData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    const searchInput = screen.getByPlaceholderText("ค้นหารายการงานหรือวัสดุ...");
    fireEvent.change(searchInput, { target: { value: "ไม้อัด" } });

    await waitFor(
      () => {
        expect(useEstimateCatalog).toHaveBeenCalledWith(
          expect.objectContaining({
            search: "ไม้อัด",
          })
        );
      },
      { timeout: 1000 }
    );
  });

  it("handles cursor-based next and previous page navigation", () => {
    vi.mocked(useEstimateCatalog).mockReturnValue({
      data: mockCatalogData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useEstimateCatalog>);

    renderModal(<EstimateItemCatalogModal {...defaultProps} />);

    const nextBtn = screen.getByRole("button", { name: "หน้าถัดไป" });
    expect(nextBtn).toBeEnabled();

    const prevBtn = screen.getByRole("button", { name: "หน้าก่อนหน้า" });
    expect(prevBtn).toBeDisabled();

    // Click next page
    fireEvent.click(nextBtn);

    expect(useEstimateCatalog).toHaveBeenCalledWith(
      expect.objectContaining({
        cursor: "cursor-page-2",
      })
    );
  });
});
