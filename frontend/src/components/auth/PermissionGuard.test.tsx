"use client";

import React from "react";
import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { PermissionGuard } from "./PermissionGuard";
import { SelectedMembershipProvider } from "@/lib/membership/selected-membership-context";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { CurrentUserResponse } from "@/lib/api/api-client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const mockUserWithReadAccess: CurrentUserResponse = {
  user: { id: "user-1", displayName: "Test User", email: "test@example.test" },
  memberships: [
    {
      id: "membership-1",
      organization: { id: "org-1", name: "Org 1" },
      permissions: [
        {
          key: PERMISSIONS.CUSTOMERS_READ,
          scope: "organization",
          scopeId: "org-1",
        },
      ],
    },
  ],
} as unknown as CurrentUserResponse;

const mockUserWithoutAccess: CurrentUserResponse = {
  user: { id: "user-2", displayName: "No Access User", email: "noaccess@example.test" },
  memberships: [
    {
      id: "membership-2",
      organization: { id: "org-2", name: "Org 2" },
      permissions: [],
    },
  ],
} as unknown as CurrentUserResponse;

function renderWithContext(ui: React.ReactNode, currentUser: CurrentUserResponse) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={queryClient}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <SelectedMembershipProvider currentUser={currentUser}>
          {ui}
        </SelectedMembershipProvider>
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("PermissionGuard", () => {
  it("renders children when user has required permission", () => {
    renderWithContext(
      <PermissionGuard permission={PERMISSIONS.CUSTOMERS_READ}>
        <div data-testid="protected-content">Secret Content</div>
      </PermissionGuard>,
      mockUserWithReadAccess
    );

    expect(screen.getByTestId("protected-content")).toBeDefined();
    expect(screen.getByText("Secret Content")).toBeDefined();
  });

  it("renders default access denied card when user lacks permission", () => {
    renderWithContext(
      <PermissionGuard permission={PERMISSIONS.CUSTOMERS_READ}>
        <div data-testid="protected-content">Secret Content</div>
      </PermissionGuard>,
      mockUserWithoutAccess
    );

    expect(screen.queryByTestId("protected-content")).toBeNull();
    expect(screen.getByRole("alert")).toBeDefined();
    expect(screen.getByText("ไม่มีสิทธิ์เข้าถึง")).toBeDefined();
  });

  it("renders custom fallback when provided", () => {
    renderWithContext(
      <PermissionGuard
        permission={PERMISSIONS.CUSTOMERS_READ}
        fallback={<div data-testid="custom-fallback">Custom Denied Message</div>}
      >
        <div data-testid="protected-content">Secret Content</div>
      </PermissionGuard>,
      mockUserWithoutAccess
    );

    expect(screen.queryByTestId("protected-content")).toBeNull();
    expect(screen.getByTestId("custom-fallback")).toBeDefined();
    expect(screen.getByText("Custom Denied Message")).toBeDefined();
  });

  it("handles array of permissions with requireAll=true", () => {
    renderWithContext(
      <PermissionGuard
        permission={[PERMISSIONS.CUSTOMERS_READ, PERMISSIONS.CUSTOMERS_CREATE]}
        requireAll={true}
      >
        <div data-testid="protected-content">Secret Content</div>
      </PermissionGuard>,
      mockUserWithReadAccess
    );

    // Only has CUSTOMERS_READ, lacks CUSTOMERS_CREATE -> access denied
    expect(screen.queryByTestId("protected-content")).toBeNull();
    expect(screen.getByRole("alert")).toBeDefined();
  });

  it("handles array of permissions with requireAll=false", () => {
    renderWithContext(
      <PermissionGuard
        permission={[PERMISSIONS.CUSTOMERS_READ, PERMISSIONS.CUSTOMERS_CREATE]}
        requireAll={false}
      >
        <div data-testid="protected-content">Secret Content</div>
      </PermissionGuard>,
      mockUserWithReadAccess
    );

    // Has CUSTOMERS_READ, so requireAll=false allows access
    expect(screen.getByTestId("protected-content")).toBeDefined();
  });
});
