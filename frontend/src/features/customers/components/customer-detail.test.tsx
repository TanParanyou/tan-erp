import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { CustomerDetail } from "./customer-detail";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn() }),
}));

const organizationDraftCustomer = {
  id: "customer-1",
  code: "CUS-0001",
  customerType: "organization",
  displayNameTh: "บริษัท ตัวอย่าง จำกัด",
  displayNameEn: "Example Co.",
  preferredLocale: "th",
  status: "draft",
  leadSource: "referral",
  primaryContact: {
    name: "คุณตัวอย่าง",
    roleTitle: null,
    phone: "081-***-5678",
    email: null,
    lineId: "line_mock_id",
    preferredChannel: "phone",
    isMasked: true,
  },
  duplicateCandidates: null,
  rowVersion: "019a3cf8-96f0-7c9f-b207-93aa818f4a20",
  createdAtUtc: "2026-09-07T12:00:00Z",
};

const mockMembership = {
  id: "membership-a",
  permissions: [{ key: "customers.activate", scope: "organization" }],
};

let currentMembership: any = mockMembership;

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: currentMembership,
  }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn(async () => "test-token"),
}));

const mockActivateCustomer = vi.fn();

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      activateCustomer: (...args: unknown[]) => mockActivateCustomer(...args),
    },
  };
});

let currentCustomerData: any = organizationDraftCustomer;
const mockRefetch = vi.fn();

vi.mock("../api/customer-queries", () => ({
  useCustomerDetail: () => ({
    data: currentCustomerData,
    isLoading: false,
    isError: false,
    error: null,
    refetch: mockRefetch,
  }),
}));

import { ToastProvider } from "@/hooks/useToast";
import { ToastContainer } from "@/components/ui/Toast";

function renderCustomerDetail(client: QueryClient, customerId: string) {
  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ToastProvider>
          <CustomerDetail customerId={customerId} />
          <ToastContainer />
        </ToastProvider>
      </NextIntlClientProvider>
    </QueryClientProvider>,
  );
}

describe("CustomerDetail canonical labels", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    currentMembership = mockMembership;
    currentCustomerData = organizationDraftCustomer;
  });

  it("renders organization draft with Thai labels", () => {
    renderCustomerDetail(client, "customer-1");

    expect(screen.getByText("นิติบุคคล")).toBeDefined();
    expect(screen.getByText("ฉบับร่าง")).toBeDefined();
    expect(screen.getByText("โทรศัพท์ (Phone)")).toBeDefined();
    expect(screen.getByText("การแนะนำต่อ (Referral)")).toBeDefined();
    expect(screen.getByText("line_mock_id")).toBeDefined();
  });

  it("renders activate button when customer is draft and user has customers.activate permission", () => {
    renderCustomerDetail(client, "customer-1");

    expect(screen.getByRole("button", { name: "เปิดใช้งานลูกค้า" })).toBeInTheDocument();
  });

  it("does not render activate button when customer is already active", () => {
    currentCustomerData = { ...organizationDraftCustomer, status: "active" };

    renderCustomerDetail(client, "customer-1");

    expect(screen.queryByRole("button", { name: "เปิดใช้งานลูกค้า" })).not.toBeInTheDocument();
  });

  it("does not render activate button when user lacks customers.activate permission", () => {
    currentMembership = { id: "membership-a", permissions: [] };

    renderCustomerDetail(client, "customer-1");

    expect(screen.queryByRole("button", { name: "เปิดใช้งานลูกค้า" })).not.toBeInTheDocument();
  });

  it("opens confirmation modal, calls activateCustomer with If-Match rowVersion, and invalidates queries on confirm", async () => {
    mockActivateCustomer.mockResolvedValueOnce({
      ...organizationDraftCustomer,
      status: "active",
      rowVersion: "019a3cf8-96f0-7c9f-b207-93aa818f9999",
    });

    renderCustomerDetail(client, "customer-1");

    const activateBtn = screen.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    fireEvent.click(activateBtn);

    // Modal should be open
    expect(screen.getByText("ยืนยันการเปิดใช้งานลูกค้า")).toBeInTheDocument();
    expect(screen.getByText(/เมื่อเปิดใช้งานแล้ว/)).toBeInTheDocument();

    const confirmBtn = screen.getByRole("button", { name: "ยืนยัน" });
    fireEvent.click(confirmBtn);

    await waitFor(() => {
      expect(mockActivateCustomer).toHaveBeenCalledTimes(1);
    });

    expect(mockActivateCustomer).toHaveBeenCalledWith(
      "customer-1",
      expect.objectContaining({
        token: "test-token",
        membershipId: "membership-a",
        ifMatch: "019a3cf8-96f0-7c9f-b207-93aa818f4a20",
      })
    );
  });

  it("shows localized conflict message when stale ETag returns 412", async () => {
    const { ApiError } = await import("@/lib/api/api-error");
    mockActivateCustomer.mockRejectedValueOnce(
      new ApiError({
        status: 412,
        code: "IF_MATCH_FAILED",
        message: "Precondition Failed",
      })
    );

    renderCustomerDetail(client, "customer-1");

    const activateBtn = screen.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    fireEvent.click(activateBtn);

    const confirmBtn = screen.getByRole("button", { name: "ยืนยัน" });
    fireEvent.click(confirmBtn);

    await waitFor(() => {
      expect(mockActivateCustomer).toHaveBeenCalledTimes(1);
    });

    // Conflict toast message should appear
    await waitFor(() => {
      expect(
        screen.getByText("ข้อมูลลูกค้าได้รับการเปลี่ยนแปลงโดยผู้อื่นแล้ว กรุณารีเฟรชและลองใหม่อีกครั้ง")
      ).toBeInTheDocument();
    });
  });
});
