import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor, act } from "@testing-library/react";
import { AccessGate } from "./access-gate";
import { ApiError } from "@/lib/api/api-error";
import type { User } from "firebase/auth";
import type { CurrentUserResponse } from "@/lib/api/api-client";

const mockPush = vi.fn();
vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}));

let mockAuthStateCallback: ((user: User | null) => void) | null = null;
vi.mock("@/lib/auth/auth-session", () => ({
  subscribeToAuthChanges: vi.fn((cb) => {
    mockAuthStateCallback = cb;
    return vi.fn();
  }),
  signOutSession: vi.fn().mockResolvedValue(undefined),
}));

let mockQueryResult: {
  data: CurrentUserResponse | undefined;
  isLoading: boolean;
  error: unknown;
  refetch: ReturnType<typeof vi.fn>;
} = {
  data: undefined,
  isLoading: false,
  error: null,
  refetch: vi.fn(),
};

vi.mock("@/features/auth/api/current-user-query", () => ({
  useCurrentUser: () => mockQueryResult,
}));

import { signOutSession } from "@/lib/auth/auth-session";

describe("AccessGate Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockAuthStateCallback = null;
    mockQueryResult = {
      data: undefined,
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    };
  });

  it("1. renders loading status with aria-busy while Firebase session is resolving", () => {
    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    const status = screen.getByRole("status");
    expect(status.getAttribute("aria-busy")).toBe("true");
    expect(status.textContent).toContain("กำลังตรวจสอบตัวตน...");
  });

  it("2. redirects to /th/login when no Firebase session exists", async () => {
    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    // Simulate Firebase reporting no user
    act(() => {
      mockAuthStateCallback!(null);
    });

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith("/th/login");
    });
  });

  it("3. renders profile loading status with accessible text while Current User query is loading", () => {
    mockQueryResult = {
      data: undefined,
      isLoading: true,
      error: null,
      refetch: vi.fn(),
    };

    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    // Provide user so session loading finishes
    act(() => {
      mockAuthStateCallback!({ uid: "uid-1" } as User);
    });

    const status = screen.getByRole("status");
    expect(status.getAttribute("aria-busy")).toBe("true");
    expect(screen.getByText("กำลังโหลดข้อมูลผู้ใช้และสิทธิ์...")).toBeDefined();
  });

  it("4. handles 401 AUTHENTICATION_INVALID by signing out and redirecting to login", async () => {
    mockQueryResult = {
      data: undefined,
      isLoading: false,
      error: new ApiError({
        status: 401,
        code: "AUTHENTICATION_INVALID",
        message: "Session expired",
      }),
      refetch: vi.fn(),
    };

    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    act(() => {
      mockAuthStateCallback!({ uid: "uid-1" } as User);
    });

    await waitFor(() => {
      expect(signOutSession).toHaveBeenCalled();
      expect(mockPush).toHaveBeenCalledWith("/th/login");
    });
  });

  it("5. renders no-membership recovery state for 403 ACTIVE_MEMBERSHIP_REQUIRED", () => {
    mockQueryResult = {
      data: undefined,
      isLoading: false,
      error: new ApiError({
        status: 403,
        code: "ACTIVE_MEMBERSHIP_REQUIRED",
        message: "No active membership found",
      }),
      refetch: vi.fn(),
    };

    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    act(() => {
      mockAuthStateCallback!({ uid: "uid-1" } as User);
    });

    const alert = screen.getByRole("alert");
    expect(alert).toBeDefined();
    expect(screen.getByText("ไม่พบสมาชิกภาพที่ใช้งานได้")).toBeDefined();
    expect(screen.getByRole("button", { name: "ออกจากระบบ" })).toBeDefined();
  });

  it("6. renders disabled-user recovery state for 403 USER_ACCESS_DISABLED", () => {
    mockQueryResult = {
      data: undefined,
      isLoading: false,
      error: new ApiError({
        status: 403,
        code: "USER_ACCESS_DISABLED",
        message: "User disabled",
      }),
      refetch: vi.fn(),
    };

    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    act(() => {
      mockAuthStateCallback!({ uid: "uid-1" } as User);
    });

    const alert = screen.getByRole("alert");
    expect(alert).toBeDefined();
    expect(screen.getByText("บัญชีผู้ใช้ถูกระงับ")).toBeDefined();
  });

  it("7. renders retry button and traceId on network / 500 error", () => {
    const mockRefetch = vi.fn();
    mockQueryResult = {
      data: undefined,
      isLoading: false,
      error: new ApiError({
        status: 500,
        code: "INTERNAL_SERVER_ERROR",
        message: "Internal error occurred",
        traceId: "test-trace-12345",
      }),
      refetch: mockRefetch,
    };

    render(
      <AccessGate>
        {() => <div>Protected Content</div>}
      </AccessGate>
    );

    act(() => {
      mockAuthStateCallback!({ uid: "uid-1" } as User);
    });

    const alert = screen.getByRole("alert");
    expect(alert).toBeDefined();
    expect(screen.getByText("เกิดข้อผิดพลาดในการเชื่อมต่อ")).toBeDefined();
    expect(screen.getByText(/test-trace-12345/)).toBeDefined();

    const retryButton = screen.getByRole("button", { name: "ลองใหม่อีกครั้ง" });
    fireEvent.click(retryButton);
    expect(mockRefetch).toHaveBeenCalledTimes(1);
  });

  it("8. renders children with Current User when authenticated and active (200 OK)", () => {
    const mockData: CurrentUserResponse = {
      user: { id: "u-1", displayName: "Somsak", email: "somsak@example.test" },
      memberships: [
        {
          id: "m-1",
          organization: { id: "o-1", name: "TEST_ONLY Project ERP" },
          branch: { id: "b-1", name: "สาขาทดสอบ" },
          permissions: [{ key: "organizations.read", scope: "Organization" }],
        },
      ],
    };

    mockQueryResult = {
      data: mockData,
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    };

    render(
      <AccessGate>
        {(currentUser) => (
          <div data-testid="protected-content">
            Welcome {currentUser.user?.displayName}
          </div>
        )}
      </AccessGate>
    );

    act(() => {
      mockAuthStateCallback!({ uid: "uid-1" } as User);
    });

    expect(screen.getByTestId("protected-content").textContent).toContain("Welcome Somsak");
  });
});
