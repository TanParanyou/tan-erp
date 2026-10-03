import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messagesTh from "@/messages/th.json";
import type { ItemResponse, PagedItemsResponse } from "@/lib/api/api-client";
import { apiClient } from "@/lib/api/api-client";
import * as csvExport from "@/lib/export/export-csv";
import { useItemList } from "@/features/item-master/api/item-master-queries";
import { ItemMasterList } from "./item-master-list";

const state = vi.hoisted(() => ({
  permissions: ["items.read", "items.create"],
  router: { push: vi.fn(), replace: vi.fn() },
  searchParams: new URLSearchParams(),
}));

vi.mock("next/navigation", () => ({
  usePathname: () => "/th/item-master",
  useRouter: () => state.router,
  useSearchParams: () => state.searchParams,
}));
vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useItemList: vi.fn(),
  useItemImport: () => ({
    preview: { mutateAsync: vi.fn(), isPending: false },
    commit: { mutateAsync: vi.fn(), isPending: false },
  }),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1", permissions: state.permissions } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => state.permissions.includes(permission),
}));
vi.mock("@/lib/auth/auth-session", () => ({ getAuthToken: vi.fn().mockResolvedValue("test-token") }));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: vi.fn(), error: vi.fn() } }) }));
vi.mock("@/components/common/AuthenticatedFileImage", () => ({
  AuthenticatedFileImage: ({ fileId, alt }: { fileId: string; alt?: string }) => <img data-file-id={fileId} alt={alt} />,
}));

const item: ItemResponse = {
  id: "item-1",
  code: "SKU-001",
  itemType: "product",
  status: "active",
  name: { thai: "โต๊ะ", english: "Table" },
  category: { id: "category-1", code: "FURN", name: { thai: "เฟอร์นิเจอร์", english: "Furniture" } },
  brand: { id: "brand-1", code: "TAN", name: { thai: "แทน", english: "Tan" } },
  baseUnit: { id: "unit-1", code: "EA" },
  primaryImage: { id: "image-1", fileId: "file-1", role: "product", isPrimary: true, displayOrder: 0, altText: { thai: "โต๊ะไม้", english: "Wooden table" } },
};

function renderList() {
  return render(
    <NextIntlClientProvider locale="th" messages={messagesTh}>
      <ItemMasterList />
    </NextIntlClientProvider>,
  );
}

function mockItems(items: ItemResponse[], totalCount = items.length) {
  vi.mocked(useItemList).mockReturnValue({
    data: { items, totalCount, pageNumber: 1, pageSize: 10 } satisfies PagedItemsResponse,
    isLoading: false,
    isError: false,
    error: null,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof useItemList>);
}

describe("ItemMasterList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    state.permissions = ["items.read", "items.create"];
    state.searchParams = new URLSearchParams();
    mockItems([]);
  });

  it("shows the first-item empty state and routes its action to item creation", () => {
    renderList();

    expect(screen.getByText("เพิ่มสินค้าแรกเพื่อเริ่มใช้ในข้อมูลอ้างอิงและประมาณราคา")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสินค้า" }));

    expect(state.router.push).toHaveBeenCalledWith("/th/item-master/create");
  });

  it("offers CSV import only to users who can create items and opens the import dialog", () => {
    mockItems([item]);
    renderList();

    fireEvent.click(screen.getByRole("button", { name: messagesTh.itemMaster.import.action }));
    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });

  it("hides CSV import without the create permission", () => {
    state.permissions = ["items.read"];
    mockItems([item]);
    renderList();

    expect(screen.queryByRole("button", { name: messagesTh.itemMaster.import.action })).toBeNull();
  });

  it("uses shared table actions, filters, row selection and current-page selected export", () => {
    mockItems([item]);
    renderList();

    expect(screen.getByRole("link", { name: "แก้ไข" })).toHaveAttribute("href", "/th/item-master/item-1");
    expect(screen.getByLabelText("ประเภทสินค้า")).toBeInTheDocument();
    expect(screen.getByLabelText("สถานะ")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("checkbox", { name: "เลือกรายการแถวที่ 1" }));
    expect(screen.getByRole("button", { name: "ส่งออกที่เลือก (1)" })).toBeInTheDocument();
  });

  it("shows the primary product image in the item list", () => {
    mockItems([item]);
    renderList();

    expect(screen.getByRole("img", { name: "โต๊ะไม้" })).toHaveAttribute("data-file-id", "file-1");
  });

  it("exports every filtered page through the existing API and CSV utility", async () => {
    const secondItem: ItemResponse = { ...item, id: "item-2", code: "SKU-002" };
    mockItems([item], 2);
    const listItems = vi.spyOn(apiClient, "listItems")
      .mockResolvedValueOnce({ items: [item], totalCount: 2, pageNumber: 1, pageSize: 100 })
      .mockResolvedValueOnce({ items: [secondItem], totalCount: 2, pageNumber: 2, pageSize: 100 });
    const exportSpy = vi.spyOn(csvExport, "exportToCsv").mockImplementation(() => {});
    renderList();

    fireEvent.click(screen.getByRole("button", { name: "ส่งออก CSV" }));

    await waitFor(() => expect(exportSpy).toHaveBeenCalledWith(expect.objectContaining({ data: [item, secondItem] })));
    expect(listItems).toHaveBeenCalledTimes(2);
    expect(listItems).toHaveBeenCalledWith(
      expect.objectContaining({ membershipId: "membership-1", locale: "th" }),
      expect.objectContaining({ pageNumber: 1, pageSize: 100, sortBy: "code", sortOrder: "asc" }),
    );
    expect(listItems).toHaveBeenLastCalledWith(
      expect.objectContaining({ membershipId: "membership-1", locale: "th" }),
      expect.objectContaining({ pageNumber: 2, pageSize: 100, sortBy: "code", sortOrder: "asc" }),
    );
    listItems.mockRestore();
    exportSpy.mockRestore();
  });
});
