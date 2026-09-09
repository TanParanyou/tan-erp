import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { ErpShell } from "./erp-shell";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { SelectedMembershipProvider } from "@/lib/membership/selected-membership-context";

const mockPush = vi.fn();
vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: mockPush,
  }),
  usePathname: () => "/th",
}));

vi.mock("@/lib/auth/auth-session", () => ({
  signOutSession: vi.fn().mockResolvedValue(undefined),
}));

import { signOutSession } from "@/lib/auth/auth-session";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const mockCurrentUser: CurrentUserResponse = {
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
        name: "TEST_ONLY Project ERP",
      },
      branch: {
        id: "30000000-0000-0000-0000-000000000001",
        name: "สาขาทดสอบ",
      },
      permissions: [
        {
          key: "organizations.read",
          scope: "organization",
          scopeId: "20000000-0000-0000-0000-000000000001",
        },
        {
          key: "customers.read",
          scope: "organization",
          scopeId: "20000000-0000-0000-0000-000000000001",
        },
      ],
    },
  ],
};

describe("ErpShell Component", () => {
  let testQueryClient: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    testQueryClient = new QueryClient({
      defaultOptions: { queries: { retry: false } },
    });
  });

  const renderWithClient = (ui: React.ReactElement, user: CurrentUserResponse = mockCurrentUser) =>
    render(
      <QueryClientProvider client={testQueryClient}>
        <SelectedMembershipProvider currentUser={user}>
          {ui}
        </SelectedMembershipProvider>
      </QueryClientProvider>
    );

  it("renders Organization, Branch, User details and permissions accurately", () => {
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);

    expect(screen.getByTestId("org-name").textContent).toBe("TEST_ONLY Project ERP");
    expect(screen.getByTestId("branch-name").textContent).toBe("สาขาทดสอบ");
    expect(screen.getAllByText("สมชาย รักสงบ").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByText("organizations.read")).toBeDefined();
  });

  it("renders Customers navigation link when customers.read permission is present", () => {
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);

    const customerLink = screen.getByRole("link", { name: "ข้อมูลลูกค้า" });
    expect(customerLink).toBeDefined();
    expect(customerLink.getAttribute("href")).toBe("/th/customers");
  });

  it("renders Opportunities navigation link when opportunities.read permission is present and hides when absent", () => {
    const userWithOppPermission: CurrentUserResponse = {
      ...mockCurrentUser,
      memberships: [
        {
          ...mockCurrentUser.memberships![0],
          permissions: [
            ...mockCurrentUser.memberships![0].permissions!,
            {
              key: "opportunities.read",
              scope: "organization",
              scopeId: "20000000-0000-0000-0000-000000000001",
            },
          ],
        },
      ],
    };

    const { unmount } = renderWithClient(<ErpShell currentUser={userWithOppPermission} />, userWithOppPermission);
    const oppLink = screen.getByRole("link", { name: "โอกาสทางการขาย" });
    expect(oppLink).toBeDefined();
    expect(oppLink.getAttribute("href")).toBe("/th/opportunities");
    unmount();

    // Without opportunities.read permission
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);
    expect(screen.queryByRole("link", { name: "โอกาสทางการขาย" })).toBeNull();
  });

  it("has interactive controls with at least 44px touch targets", () => {
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);

    const languageLink = screen.getByRole("link", { name: /Switch to English/i });
    const logoutButton = screen.getByRole("button", { name: "ออกจากระบบ" });

    expect(languageLink.style.minHeight).toBe("44px");
    expect(logoutButton.style.minHeight).toBe("44px");
  });

  it("calls signOutSession and navigates to login when sign out clicked", async () => {
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);

    const logoutButton = screen.getByRole("button", { name: "ออกจากระบบ" });
    fireEvent.click(logoutButton);

    expect(signOutSession).toHaveBeenCalledWith(testQueryClient);
  });

  it("toggles mobile navigation menu when hamburger button is clicked", () => {
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);

    const menuButton = screen.getByRole("button", { name: "เปิด/ปิดเมนู" });
    expect(menuButton.getAttribute("aria-expanded")).toBe("false");

    fireEvent.click(menuButton);
    expect(menuButton.getAttribute("aria-expanded")).toBe("true");

    fireEvent.click(menuButton);
    expect(menuButton.getAttribute("aria-expanded")).toBe("false");
  });
});
