import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityEditor } from "./opportunity-editor";
import { apiClient } from "@/lib/api/api-client";
import { ToastProvider } from "@/hooks/useToast";
import { ToastContainer } from "@/components/ui/Toast";

const mockPush = vi.fn();

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mockPush }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn(async () => "test-token"),
}));

import type {
  CurrentUserResponse,
  CustomerListResponse,
  SiteListResponse,
  OpportunityResponse,
} from "@/lib/api/api-client";
import type { MembershipDto } from "@/lib/permissions/can";

const mockMembershipWithBranch = {
  id: "membership-1",
  organization: { id: "org-1", name: "Org 1" },
  branch: { id: "branch-1", name: "สาขาใหญ่ (กรุงเทพ)" },
  permissions: [{ key: "opportunities.create", scope: "organization" }],
};

const mockMembershipWithoutBranch = {
  id: "membership-no-branch",
  organization: { id: "org-1", name: "Org 1" },
  branch: undefined,
  permissions: [{ key: "opportunities.create", scope: "organization" }],
};

const currentUser = {
  user: {
    id: "10000000-0000-0000-0000-000000000010",
    displayName: "คุณเจ้าของโอกาส",
    email: "owner@example.test",
  },
  memberships: [mockMembershipWithBranch],
} satisfies CurrentUserResponse;

let currentMembership: MembershipDto | null = mockMembershipWithBranch;

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    currentUser,
    selectedMembership: currentMembership,
  }),
}));

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      createOpportunity: vi.fn(),
      listCustomers: vi.fn(),
      listCustomerSites: vi.fn(),
    },
  };
});

const mockedCreateOpportunity = vi.mocked(apiClient.createOpportunity);
const mockedListCustomers = vi.mocked(apiClient.listCustomers);
const mockedListCustomerSites = vi.mocked(apiClient.listCustomerSites);

const activeCustomerA = {
  id: "10000000-0000-0000-0000-000000000001",
  code: "CUS-0001",
  displayNameTh: "บริษัท ลูกค้าเอ จำกัด",
  status: "active",
};

const activeCustomerB = {
  id: "10000000-0000-0000-0000-000000000002",
  code: "CUS-0002",
  displayNameTh: "บริษัท ลูกค้าบี จำกัด",
  status: "active",
};

const customerASite1 = {
  id: "20000000-0000-0000-0000-000000000001",
  customerId: activeCustomerA.id,
  label: "สำนักงานใหญ่ ลูกค้าเอ",
  status: "active",
};

function renderEditor(client: QueryClient): void {
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ToastProvider>
          <OpportunityEditor />
          <ToastContainer />
        </ToastProvider>
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("OpportunityEditor", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    currentMembership = mockMembershipWithBranch;

    mockedListCustomers.mockResolvedValue({
      items: [activeCustomerA, activeCustomerB],
      nextCursor: null,
    } satisfies CustomerListResponse);

    mockedListCustomerSites.mockResolvedValue({
      items: [customerASite1],
    } satisfies SiteListResponse);

    let count = 0;
    vi.spyOn(crypto, "randomUUID").mockImplementation(() => {
      count += 1;
      return `opp-key-${count}` as `${string}-${string}-${string}-${string}-${string}`;
    });
  });

  it("renders branch requirement block when selected membership has no active branch", () => {
    currentMembership = mockMembershipWithoutBranch;
    renderEditor(client);

    expect(screen.getByText("ไม่สามารถสร้างโอกาสทางการขายได้")).toBeDefined();
    expect(screen.getByText(/คุณต้องมีสมาชิกภาพที่สังกัดสาขาที่ใช้งานได้/)).toBeDefined();
  });

  it("displays read-only branch and owner context derived from selected membership and current user", () => {
    renderEditor(client);

    expect(screen.getByText("สาขาที่ดูแล:")).toBeInTheDocument();
    expect(screen.getByText("สาขาใหญ่ (กรุงเทพ)")).toBeInTheDocument();
    expect(screen.getByText("ผู้รับผิดชอบ:")).toBeInTheDocument();
    expect(screen.getByText("คุณเจ้าของโอกาส")).toBeInTheDocument();
  });

  it("submits valid opportunity and navigates to detail on success", async () => {
    mockedCreateOpportunity.mockResolvedValueOnce({
      id: "30000000-0000-0000-0000-000000000001",
      code: "OPP-0001",
      customerId: activeCustomerA.id,
      title: "โครงการปรับปรุงอาคารสำนักงาน",
      stage: "draft",
      branchId: "branch-1",
      ownerUserId: "user-1",
      workTypes: ["built-in"],
      createdAtUtc: "2026-09-08T00:00:00Z",
    } satisfies OpportunityResponse);

    renderEditor(client);

    // Wait for customer options to load
    await waitFor(() => {
      expect(screen.getByText(/บริษัท ลูกค้าเอ จำกัด/)).toBeDefined();
    });

    // Select customer
    fireEvent.change(screen.getByLabelText(/ลูกค้า/), {
      target: { value: activeCustomerA.id },
    });

    // Enter title
    fireEvent.change(screen.getByLabelText(/ชื่อโอกาสทางการขาย/), {
      target: { value: "โครงการปรับปรุงอาคารสำนักงาน" },
    });

    // Select work type checkbox
    const builtInCheckbox = screen.getByLabelText(/งานบิวท์อิน/);
    fireEvent.click(builtInCheckbox);

    // Submit
    const submitBtn = screen.getByRole("button", { name: "บันทึกโอกาสทางการขาย" });
    fireEvent.click(submitBtn);

    await waitFor(() => {
      expect(mockedCreateOpportunity).toHaveBeenCalledTimes(1);
    });

    expect(mockedCreateOpportunity).toHaveBeenCalledWith(
      expect.objectContaining({
        customerId: activeCustomerA.id,
        title: "โครงการปรับปรุงอาคารสำนักงาน",
        workTypes: ["built-in"],
      }),
      expect.objectContaining({
        token: "test-token",
        membershipId: "membership-1",
        idempotencyKey: "opp-key-1",
      })
    );

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith(
        expect.stringContaining("/opportunities/30000000-0000-0000-0000-000000000001")
      );
    });
  });

  it("clears primarySiteId synchronously when customer selection changes", async () => {
    renderEditor(client);

    await waitFor(() => {
      expect(screen.getByText(/บริษัท ลูกค้าเอ จำกัด/)).toBeDefined();
    });

    // Select customer A
    const customerSelect = screen.getByLabelText(/ลูกค้า/);
    fireEvent.change(customerSelect, {
      target: { value: activeCustomerA.id },
    });

    // Wait for Site to load
    await waitFor(() => {
      expect(mockedListCustomerSites).toHaveBeenCalledWith(activeCustomerA.id, expect.anything());
    });

    // Select site
    const siteSelect = screen.getByLabelText(/สถานที่ตั้งหลัก/);
    fireEvent.change(siteSelect, {
      target: { value: customerASite1.id },
    });
    expect((siteSelect as HTMLSelectElement).value).toBe(customerASite1.id);

    // Change customer to B
    fireEvent.change(customerSelect, {
      target: { value: activeCustomerB.id },
    });

    // primarySiteId must be cleared immediately
    expect((siteSelect as HTMLSelectElement).value).toBe("");
  });
});
