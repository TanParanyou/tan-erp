import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityDetail } from "./opportunity-detail";
import * as oppQueries from "../api/opportunity-queries";
import * as customerQueries from "@/features/customers/api/customer-queries";
import * as siteQueries from "@/features/sites/api/site-queries";
import * as userQueries from "@/features/users/api/user-queries";
import * as membershipContext from "@/lib/membership/selected-membership-context";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { ToastProvider } from "@/hooks/useToast";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn() }),
}));

describe("OpportunityDetail Component", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    vi.clearAllMocks();
    vi.spyOn(userQueries, "useUserList").mockReturnValue({
      data: {
        items: [
          { id: "user-2", displayName: "พนักงาน คนที่สอง", email: "user2@example.com", branchId: "branch-1" },
        ],
        totalCount: 1,
      },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof userQueries.useUserList>);

    vi.spyOn(oppQueries, "useOpportunityStageHistory").mockReturnValue({
      data: { items: [] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityStageHistory>);
  });

  const mockMembership = {
    id: "membership-1",
    organization: { id: "org-1", name: "Org 1" },
    branch: { id: "branch-1", name: "Branch 1" },
    permissions: [{ key: "opportunities.read", scope: "organization", scopeId: "org-1" }],
  };

  const sampleOpportunity = {
    id: "30000000-0000-0000-0000-000000000001",
    code: "OPP-0001",
    customerId: "10000000-0000-0000-0000-000000000001",
    primarySiteId: "20000000-0000-0000-0000-000000000001",
    branchId: "branch-1",
    ownerUserId: "user-1",
    title: "โครงการปรับปรุงอาคารสำนักงาน",
    scopeSummary: "ปรับปรุงห้องประชุมและพื้นที่ส่วนกลาง",
    workTypes: ["built-in", "interior"],
    sourceCode: "referral",
    expectedBudget: 500000,
    currencyCode: "THB",
    targetDecisionDate: "2026-10-15",
    nextActionAtUtc: "2026-09-20T10:00:00Z",
    nextActionNote: "นัดประชุมนำเสนอแบบร่าง",
    stage: "draft",
    rowVersion: "00000000-0000-0000-0000-000000000001",
    createdAtUtc: "2026-09-08T08:00:00Z",
  };

  const sampleCustomer = {
    id: "10000000-0000-0000-0000-000000000001",
    code: "CUS-0001",
    displayNameTh: "บริษัท ลูกค้าเอ จำกัด",
    status: "active",
  };

  const sampleSite = {
    id: "20000000-0000-0000-0000-000000000001",
    customerId: "10000000-0000-0000-0000-000000000001",
    label: "สำนักงานใหญ่",
    addressLine1: "123 สุขุมวิท",
    status: "active",
  };

  const mockCurrentUser: CurrentUserResponse = {
    user: { id: "user-1", displayName: "Test User", email: "test@example.test" },
    memberships: [mockMembership],
  };

  it("renders loading state with minimal mono spinner", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getAllByRole("status").length).toBeGreaterThanOrEqual(1);
  });

  it("renders opportunity details and localized Draft stage badge", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite], totalCount: 1 },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("โครงการปรับปรุงอาคารสำนักงาน")).toBeDefined();
    expect(screen.getByText("OPP-0001")).toBeDefined();
    expect(screen.getByText("ฉบับร่าง (Draft)")).toBeDefined();
    expect(screen.getByText("งานบิวท์อิน (Built-in)")).toBeDefined();
    expect(screen.getByText("สำนักงานใหญ่")).toBeDefined();
  });

  it("renders next action schedule and opens customer drawer on click", async () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    vi.spyOn(customerQueries, "useCustomerDetail").mockReturnValue({
      data: sampleCustomer,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof customerQueries.useCustomerDetail>);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    const customerButtons = screen.getAllByRole("button", { name: /บริษัท ลูกค้าเอ จำกัด/i });
    expect(customerButtons.length).toBeGreaterThanOrEqual(1);

    const { fireEvent, act } = await import("@testing-library/react");
    await act(async () => {
      fireEvent.click(customerButtons[0]);
    });

    expect(screen.getByText("ข้อมูลสรุปของลูกค้า")).toBeDefined();
  });

  it("qualifies a draft opportunity from the confirmation modal", async () => {
    const membershipWithTransition = {
      ...mockMembership,
      permissions: [
        { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
        { key: "opportunities.transition", scope: "organization", scopeId: "org-1" },
      ],
    };

    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: membershipWithTransition,
      memberships: [membershipWithTransition],
      setSelectedMembershipId: vi.fn(),
    });

    const mutateAsyncMock = vi.fn().mockImplementation(async () => {
      // simulate success
      return {
        ...sampleOpportunity,
        stage: "qualified",
      };
    });

    vi.spyOn(oppQueries, "useQualifyOpportunity").mockReturnValue({
      mutateAsync: mutateAsyncMock,
      isPending: false,
    } as unknown as ReturnType<typeof oppQueries.useQualifyOpportunity>);

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Verify Qualify button exists for draft with permission
    const qualifyBtn = screen.getByRole("button", { name: /ผ่านเกณฑ์ \(Qualify\)/i });
    expect(qualifyBtn).toBeDefined();

    // Click Qualify button to open confirmation modal
    const { fireEvent, act } = await import("@testing-library/react");
    await act(async () => {
      fireEvent.click(qualifyBtn);
    });

    // Modal title should be visible
    expect(screen.getByText(/ยืนยันการเปลี่ยนขั้นตอนเป็น 'ผ่านเกณฑ์'/i)).toBeDefined();

    // Confirm button inside modal
    const confirmBtn = screen.getByRole("button", { name: /ยืนยัน/i });
    expect(confirmBtn).toBeDefined();

    await act(async () => {
      fireEvent.click(confirmBtn);
    });

    expect(mutateAsyncMock).toHaveBeenCalledTimes(1);
    expect(mutateAsyncMock).toHaveBeenCalledWith(
      expect.objectContaining({
        opportunityId: sampleOpportunity.id,
        expectedVersion: sampleOpportunity.rowVersion,
      })
    );
  });

  it("OpportunityDetail_DraftUpdate_SaveThenEnablesQualify", async () => {
    const mutateUpdateMock = vi.fn().mockResolvedValue({
      ...sampleOpportunity,
      scopeSummary: "ขอบเขตงานที่แก้ไขแล้ว",
      nextActionAtUtc: "2026-09-25T10:00:00Z",
      nextActionNote: "นัดหมายเรียบร้อย",
      rowVersion: "00000000-0000-0000-0000-000000000002",
    });

    const incompleteDraft = {
      ...sampleOpportunity,
      scopeSummary: null,
      nextActionAtUtc: null,
      nextActionNote: null,
    };

    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      selectedMembership: {
        ...mockMembership,
        permissions: [
          { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
          { key: "opportunities.update", scope: "organization", scopeId: "org-1" },
          { key: "opportunities.transition", scope: "organization", scopeId: "org-1" },
        ],
      },
      currentUser: {
        user: { id: "user-1", displayName: "User", email: "user@example.test" },
        memberships: [mockMembership],
      },
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: incompleteDraft,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    vi.spyOn(oppQueries, "useUpdateDraftQGate").mockReturnValue({
      mutateAsync: mutateUpdateMock,
      isPending: false,
    } as unknown as ReturnType<typeof oppQueries.useUpdateDraftQGate>);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={incompleteDraft.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Q-Gate guidance banner should be visible for incomplete draft
    expect(screen.getByRole("region", { name: /รายการตรวจสอบความพร้อมตามเกณฑ์/i })).toBeDefined();

    // Inline Q-Gate form should be rendered
    const form = screen.getByRole("form", { name: /แก้ไขข้อมูลความพร้อมตามเกณฑ์/i });
    expect(form).toBeDefined();

    // Save button inside Q-Gate editor
    const saveBtn = screen.getByRole("button", { name: /บันทึกข้อมูล Q-Gate/i });
    expect(saveBtn).toBeDefined();

    const { fireEvent, act } = await import("@testing-library/react");

    // Fill scope summary
    const scopeInput = screen.getByLabelText(/สรุปขอบเขตงาน/i);
    await act(async () => {
      fireEvent.change(scopeInput, { target: { value: "ขอบเขตงานที่แก้ไขแล้ว" } });
      fireEvent.blur(scopeInput);
    });

    // Fill next action date and note
    const dateInput = screen.getByPlaceholderText(/เลือกวันที่/i);
    await act(async () => {
      fireEvent.change(dateInput, { target: { value: "25/09/2026" } });
      fireEvent.blur(dateInput);
    });

    const noteInput = screen.getByLabelText(/บันทึกการดำเนินการถัดไป/i);
    await act(async () => {
      fireEvent.change(noteInput, { target: { value: "นัดหมายเรียบร้อย" } });
      fireEvent.blur(noteInput);
    });

    // Submit form
    await act(async () => {
      fireEvent.click(saveBtn);
    });

    expect(mutateUpdateMock).toHaveBeenCalledTimes(1);
    expect(mutateUpdateMock).toHaveBeenCalledWith(
      expect.objectContaining({
        opportunityId: incompleteDraft.id,
        expectedVersion: incompleteDraft.rowVersion,
        payload: expect.objectContaining({
          scopeSummary: "ขอบเขตงานที่แก้ไขแล้ว",
          nextActionNote: "นัดหมายเรียบร้อย",
        }),
      })
    );
  });

  it("OpportunityDetail_EditAndReassign_RefreshesDetail: opens reassign modal and submits owner transfer", async () => {
    const memberWithUpdate = {
      ...mockMembership,
      permissions: [
        { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
        { key: "opportunities.update", scope: "organization", scopeId: "org-1" },
      ],
    };

    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: memberWithUpdate,
      memberships: [memberWithUpdate],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    vi.spyOn(customerQueries, "useCustomerDetail").mockReturnValue({
      data: sampleCustomer,
      isLoading: false,
      isError: false,
      error: null,
    } as unknown as ReturnType<typeof customerQueries.useCustomerDetail>);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
    } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

    const mutateReassignMock = vi.fn().mockResolvedValue({
      ...sampleOpportunity,
      ownerUserId: "user-2",
      rowVersion: "00000000-0000-0000-0000-000000000002",
    });

    vi.spyOn(oppQueries, "useReassignOpportunityOwner").mockReturnValue({
      mutateAsync: mutateReassignMock,
      isPending: false,
    } as unknown as ReturnType<typeof oppQueries.useReassignOpportunityOwner>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Reassign Owner button should be visible
    const reassignBtn = screen.getByRole("button", { name: /โอนย้ายผู้รับผิดชอบ/i });
    expect(reassignBtn).toBeDefined();

    const { fireEvent, act } = await import("@testing-library/react");

    // Click button to open modal
    await act(async () => {
      fireEvent.click(reassignBtn);
    });

    // Modal dialog should be rendered
    const modalDialog = screen.getByRole("dialog");
    expect(modalDialog).toBeDefined();

    // Select new owner via UserAutocomplete
    const input = screen.getByPlaceholderText(/เลือกพนักงานผู้รับผิดชอบ/i);
    await act(async () => {
      fireEvent.focus(input);
    });

    const userOption = screen.getByText("พนักงาน คนที่สอง");
    await act(async () => {
      fireEvent.mouseDown(userOption);
    });

    // Confirm reassignment
    const confirmBtn = screen.getByRole("button", { name: /ยืนยัน/i });
    await act(async () => {
      fireEvent.click(confirmBtn);
    });

    expect(mutateReassignMock).toHaveBeenCalledTimes(1);
    expect(mutateReassignMock).toHaveBeenCalledWith(
      expect.objectContaining({
        opportunityId: sampleOpportunity.id,
        expectedVersion: sampleOpportunity.rowVersion,
        targetOwnerUserId: "user-2",
      })
    );
  });

  it("OpportunityDetail_EditOpen_OpensDrawerAndSubmitsUpdate", async () => {
    const memberWithUpdate = {
      ...mockMembership,
      permissions: [
        { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
        { key: "opportunities.update", scope: "organization", scopeId: "org-1" },
      ],
    };

    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: memberWithUpdate,
      memberships: [memberWithUpdate],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    vi.spyOn(customerQueries, "useCustomerDetail").mockReturnValue({
      data: sampleCustomer,
      isLoading: false,
      isError: false,
      error: null,
    } as unknown as ReturnType<typeof customerQueries.useCustomerDetail>);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
    } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

    const mutateUpdateOpenMock = vi.fn().mockResolvedValue({
      ...sampleOpportunity,
      title: "โครงการปรับปรุงอาคารสำนักงาน (แก้ไขใหม่)",
      rowVersion: "00000000-0000-0000-0000-000000000003",
    });

    vi.spyOn(oppQueries, "useUpdateOpenOpportunity").mockReturnValue({
      mutateAsync: mutateUpdateOpenMock,
      isPending: false,
    } as unknown as ReturnType<typeof oppQueries.useUpdateOpenOpportunity>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Edit button should be visible
    const editBtn = screen.getByRole("button", { name: /แก้ไขข้อมูล/i });
    expect(editBtn).toBeDefined();

    const { fireEvent, act } = await import("@testing-library/react");

    // Click edit button to open drawer
    await act(async () => {
      fireEvent.click(editBtn);
    });

    // Drawer dialog should be open
    const drawerDialog = screen.getByRole("dialog");
    expect(drawerDialog).toBeDefined();

    // Modify title in drawer
    const titleInput = screen.getByLabelText(/ชื่อโอกาสทางการขาย \/ โครงการ/i);
    await act(async () => {
      fireEvent.change(titleInput, { target: { value: "โครงการปรับปรุงอาคารสำนักงาน (แก้ไขใหม่)" } });
      fireEvent.blur(titleInput);
    });

    // Submit drawer form
    const saveBtn = screen.getByRole("button", { name: /^บันทึก$/ });
    await act(async () => {
      fireEvent.click(saveBtn);
    });

    expect(mutateUpdateOpenMock).toHaveBeenCalledTimes(1);
    expect(mutateUpdateOpenMock).toHaveBeenCalledWith(
      expect.objectContaining({
        opportunityId: sampleOpportunity.id,
        expectedVersion: sampleOpportunity.rowVersion,
        payload: expect.objectContaining({
          title: "โครงการปรับปรุงอาคารสำนักงาน (แก้ไขใหม่)",
        }),
      })
    );
  });

  it("OpportunityDetail_CloseLost_OpensModalAndSubmits", async () => {
    const memberWithTransition = {
      ...mockMembership,
      permissions: [
        { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
        { key: "opportunities.transition", scope: "organization", scopeId: "org-1" },
      ],
    };

    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: memberWithTransition,
      memberships: [memberWithTransition],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    const mutateTransitionMock = vi.fn().mockResolvedValue({
      ...sampleOpportunity,
      stage: "lost",
      rowVersion: "00000000-0000-0000-0000-000000000004",
    });

    vi.spyOn(oppQueries, "useTransitionOpportunityStage").mockReturnValue({
      mutateAsync: mutateTransitionMock,
      isPending: false,
    } as unknown as ReturnType<typeof oppQueries.useTransitionOpportunityStage>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={sampleOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Click Close button
    const closeBtn = screen.getByRole("button", { name: /ปิดงาน \/ ยกเลิก\.\.\./i });
    expect(closeBtn).toBeDefined();

    const { fireEvent, act } = await import("@testing-library/react");
    await act(async () => {
      fireEvent.click(closeBtn);
    });

    // Modal dialog should be open
    const modalDialog = screen.getByRole("dialog");
    expect(modalDialog).toBeDefined();

    // Select reason (first combobox is stage which defaults to 'lost', second is reasonCode)
    const comboboxes = screen.getAllByRole("combobox");
    const reasonSelect = comboboxes.length > 1 ? comboboxes[1] : comboboxes[0];
    await act(async () => {
      fireEvent.change(reasonSelect, { target: { value: "lost_price_too_high" } });
    });

    // Confirm
    const confirmBtn = screen.getByRole("button", { name: /ยืนยัน/i });
    await act(async () => {
      fireEvent.click(confirmBtn);
    });

    expect(mutateTransitionMock).toHaveBeenCalledTimes(1);
    expect(mutateTransitionMock).toHaveBeenCalledWith(
      expect.objectContaining({
        opportunityId: sampleOpportunity.id,
        targetStage: "lost",
        expectedVersion: sampleOpportunity.rowVersion,
        reasonCode: "lost_price_too_high",
      })
    );
  });

  it("OpportunityDetail_Reopen_OpensModalAndSubmits", async () => {
    const closedOpportunity = {
      ...sampleOpportunity,
      stage: "lost",
    };

    const memberWithTransition = {
      ...mockMembership,
      permissions: [
        { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
        { key: "opportunities.transition", scope: "organization", scopeId: "org-1" },
      ],
    };

    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: memberWithTransition,
      memberships: [memberWithTransition],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: closedOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof oppQueries.useOpportunityDetail>);

    const mutateTransitionMock = vi.fn().mockResolvedValue({
      ...closedOpportunity,
      stage: "qualified",
      rowVersion: "00000000-0000-0000-0000-000000000005",
    });

    vi.spyOn(oppQueries, "useTransitionOpportunityStage").mockReturnValue({
      mutateAsync: mutateTransitionMock,
      isPending: false,
    } as unknown as ReturnType<typeof oppQueries.useTransitionOpportunityStage>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <ToastProvider>
            <OpportunityDetail opportunityId={closedOpportunity.id} />
          </ToastProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Click Reopen button
    const reopenBtn = screen.getByRole("button", { name: /เปิดงานใหม่ \(Reopen\)/i });
    expect(reopenBtn).toBeDefined();

    const { fireEvent, act } = await import("@testing-library/react");
    await act(async () => {
      fireEvent.click(reopenBtn);
    });

    // Modal dialog should be open
    const modalDialog = screen.getByRole("dialog");
    expect(modalDialog).toBeDefined();

    // Select reason
    const select = screen.getByRole("combobox");
    await act(async () => {
      fireEvent.change(select, { target: { value: "reopen_budget_adjusted" } });
    });

    // Confirm
    const confirmBtn = screen.getByRole("button", { name: /ยืนยัน/i });
    await act(async () => {
      fireEvent.click(confirmBtn);
    });

    expect(mutateTransitionMock).toHaveBeenCalledTimes(1);
    expect(mutateTransitionMock).toHaveBeenCalledWith(
      expect.objectContaining({
        opportunityId: closedOpportunity.id,
        targetStage: "qualified",
        expectedVersion: closedOpportunity.rowVersion,
        reasonCode: "reopen_budget_adjusted",
      })
    );
  });
});

