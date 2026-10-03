import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { SidebarNav } from "./SidebarNav";
import type { CurrentUserResponse } from "@/lib/api/api-client";

vi.mock("next/navigation", () => ({
  usePathname: () => "/th",
}));

vi.mock("next/link", () => ({
  default: ({
    children,
    href,
    onClick,
    ...props
  }: React.AnchorHTMLAttributes<HTMLAnchorElement> & { href: string }) => (
    <a
      href={href}
      onClick={(e) => {
        e.preventDefault();
        onClick?.(e);
      }}
      {...props}
    >
      {children}
    </a>
  ),
}));

const mockUser: CurrentUserResponse = {
  user: {
    id: "00000000-0000-0000-0000-000000000001",
    displayName: "สมชาย รักสงบ",
    email: "somchai@example.test",
  },
  memberships: [
    {
      id: "10000000-0000-0000-0000-000000000001",
      organization: {
        id: "20000000-0000-0000-0000-000000000001",
        name: "TEST ERP",
      },
      branch: {
        id: "30000000-0000-0000-0000-000000000001",
        name: "สาขาทดสอบ",
      },
      permissions: [
        { key: "customers.read", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
        { key: "opportunities.read", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
        { key: "items.read", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
        { key: "cost-sources.read", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
        { key: "cost-records.read", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
        { key: "document-sequences.read", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
      ],
    },
  ],
};

describe("SidebarNav Component", () => {
  it("renders menu items, search input, and expand/collapse all button in expanded mode", () => {
    render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);

    expect(screen.getByPlaceholderText("ค้นหาเมนู...")).toBeDefined();
    expect(screen.getByRole("button", { name: "ยุบทั้งหมด" })).toBeDefined();
    expect(screen.getByRole("link", { name: "หน้าหลัก" })).toBeDefined();
    expect(screen.getByText("ลูกค้าและโอกาสขาย")).toBeDefined();
    expect(screen.getByText("สินค้า")).toBeDefined();
    expect(screen.getByRole("link", { name: "ข้อมูลลูกค้า" })).toBeDefined();
    expect(screen.getByRole("link", { name: "ข้อมูลหลักสินค้า" })).toBeDefined();
  });

  it("filters menu items in real time when typing in search input", () => {
    render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);

    const searchInput = screen.getByPlaceholderText("ค้นหาเมนู...");
    fireEvent.change(searchInput, { target: { value: "ลูกค้า" } });

    // Should display items matching "ลูกค้า"
    expect(screen.getByRole("link", { name: "ข้อมูลลูกค้า" })).toBeDefined();
    // Items not matching should be filtered out
    expect(screen.queryByRole("link", { name: "ข้อมูลหลักสินค้า" })).toBeNull();
    expect(screen.queryByRole("link", { name: "รูปแบบเลขที่เอกสาร" })).toBeNull();

    // Clear search button should appear and reset when clicked
    const clearButton = screen.getByRole("button", { name: "ล้างการค้นหา" });
    fireEvent.click(clearButton);

    expect(screen.getByRole("link", { name: "ข้อมูลหลักสินค้า" })).toBeDefined();
  });

  it("shows empty state message when search query does not match any menu", () => {
    render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);

    const searchInput = screen.getByPlaceholderText("ค้นหาเมนู...");
    fireEvent.change(searchInput, { target: { value: "คำค้นหาที่ไม่มีในระบบ" } });

    expect(screen.getByText("ไม่พบเมนูที่ค้นหา")).toBeDefined();
  });

  it("toggles all subgroups open and closed with the toggle all button", () => {
    render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);

    const toggleAllBtn = screen.getByRole("button", { name: "ยุบทั้งหมด" });
    fireEvent.click(toggleAllBtn);

    // After collapsing all, subgroup links should be hidden
    expect(screen.queryByRole("link", { name: "ข้อมูลลูกค้า" })).toBeNull();
    expect(screen.queryByRole("link", { name: "ข้อมูลหลักสินค้า" })).toBeNull();
    expect(screen.getByRole("button", { name: "ขยายทั้งหมด" })).toBeDefined();

    // Click again to expand all
    fireEvent.click(screen.getByRole("button", { name: "ขยายทั้งหมด" }));
    expect(screen.getByRole("link", { name: "ข้อมูลลูกค้า" })).toBeDefined();
    expect(screen.getByRole("link", { name: "ข้อมูลหลักสินค้า" })).toBeDefined();
  });

  it("toggles individual subgroup when clicking accordion header", () => {
    render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);

    const crmHeader = screen.getByRole("button", { name: "ลูกค้าและโอกาสขาย" });
    expect(screen.getByRole("link", { name: "ข้อมูลลูกค้า" })).toBeDefined();

    // Click to collapse CRM
    fireEvent.click(crmHeader);
    expect(screen.queryByRole("link", { name: "ข้อมูลลูกค้า" })).toBeNull();

    // Click to expand CRM again
    fireEvent.click(crmHeader);
    expect(screen.getByRole("link", { name: "ข้อมูลลูกค้า" })).toBeDefined();
  });

  it("hides search and toggle-all controls in collapsed mode", () => {
    render(<SidebarNav currentUser={mockUser} isCollapsed={true} />);

    expect(screen.queryByPlaceholderText("ค้นหาเมนู...")).toBeNull();
    expect(screen.queryByRole("button", { name: "ยุบทั้งหมด" })).toBeNull();
    expect(screen.queryByRole("button", { name: "ขยายทั้งหมด" })).toBeNull();
    // Nav links should still exist for icons
    expect(screen.getByRole("link", { name: "หน้าหลัก" })).toBeDefined();
    expect(screen.getByRole("link", { name: "ข้อมูลลูกค้า" })).toBeDefined();
  });

  it("invokes onLinkClick callback when a menu link is clicked", () => {
    const handleLinkClick = vi.fn();
    render(<SidebarNav currentUser={mockUser} isCollapsed={false} onLinkClick={handleLinkClick} />);

    const homeLink = screen.getByRole("link", { name: "หน้าหลัก" });
    fireEvent.click(homeLink);

    expect(handleLinkClick).toHaveBeenCalledTimes(1);
  });

  it("shows estimate review only to memberships with estimate approval permission", () => {
    const { rerender } = render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);
    expect(screen.queryByRole("link", { name: "ตรวจสอบการประเมินราคา" })).toBeNull();

    const authorizedUser: CurrentUserResponse = {
      ...mockUser,
      memberships: mockUser.memberships?.map((membership, index) => index === 0 ? {
        ...membership,
        permissions: [...(membership.permissions ?? []), { key: "estimates.approve", scope: "organization" }],
      } : membership),
    };
    rerender(<SidebarNav currentUser={authorizedUser} isCollapsed={false} />);
    expect(screen.getByRole("link", { name: "ตรวจสอบการประเมินราคา" })).toBeDefined();
  });
});
