import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SurveyList } from "./survey-list";
import * as oppQueries from "@/features/opportunities/api/opportunity-queries";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  usePathname: () => "/th/surveys",
  useSearchParams: () => new URLSearchParams(),
}));

describe("SurveyList Component", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    vi.clearAllMocks();
  });

  const sampleSurveyOpportunity = {
    id: "opp-srv-001",
    code: "OPP-2026-0088",
    customerId: "cust-001",
    primarySiteId: null,
    branchId: "branch-1",
    ownerUserId: "user-1",
    title: "งานตกแต่งเพนต์เฮาส์ สุขุมวิท",
    scopeSummary: "บิวต์อินห้องนอนใหญ่และห้องนั่งเล่น",
    workTypes: ["built-in", "interior"],
    expectedBudget: 500000,
    currencyCode: "THB",
    targetDecisionDate: "2026-11-15",
    nextActionAtUtc: "2026-10-25T09:00:00Z",
    nextActionNote: "นัดวัดพื้นที่หน้างานจริง",
    stage: "surveying",
    rowVersion: "version-1",
    createdAtUtc: "2026-10-01T08:00:00Z",
    customer: {
      id: "cust-001",
      displayNameTh: "คุณวิภาวรรณ สุขุมวิท",
      displayNameEn: "Wipawan Sukhumvit",
    },
    owner: {
      id: "user-1",
      displayName: "สมศักดิ์ ช่างสำรวจ",
      email: "somsak@example.test",
    },
  };

  it("renders header with title, subtitle, and new survey record button", () => {
    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: {
        pages: [{ items: [sampleSurveyOpportunity], totalCount: 1 }],
      },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <SurveyList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("งานสำรวจหน้างาน (Field Surveys)")).toBeInTheDocument();
    expect(screen.getByText("รายการโอกาสทางการขายและงานที่ต้องสำรวจพื้นที่ เก็บขนาดสเปกจริงหน้างาน")).toBeInTheDocument();
    
    // Header record new survey action button
    const recordButtons = screen.getAllByRole("link", { name: "บันทึกสำรวจหน้างาน" });
    expect(recordButtons.length).toBeGreaterThanOrEqual(1);
    expect(recordButtons[0].getAttribute("href")).toBe("/th/surveys/field");
  });

  it("renders table columns with project, customer, surveyor, and appointment date", () => {
    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: {
        pages: [{ items: [sampleSurveyOpportunity], totalCount: 1 }],
      },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <SurveyList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    // Project & Code
    expect(screen.getByText("งานตกแต่งเพนต์เฮาส์ สุขุมวิท")).toBeInTheDocument();
    expect(screen.getByText("OPP-2026-0088")).toBeInTheDocument();

    // Customer
    expect(screen.getByText("คุณวิภาวรรณ สุขุมวิท")).toBeInTheDocument();

    // Surveyor Owner
    expect(screen.getByText("สมศักดิ์ ช่างสำรวจ")).toBeInTheDocument();

    // Appointment note
    expect(screen.getByText("นัดวัดพื้นที่หน้างานจริง")).toBeInTheDocument();

    // Stage Badge
    const stageBadges = screen.getAllByText("กำลังสำรวจหน้างาน (Surveying)");
    expect(stageBadges.length).toBeGreaterThanOrEqual(1);

    // Action links
    const surveyActionLink = screen.getByRole("link", { name: "สำรวจหน้างาน" });
    expect(surveyActionLink).toBeInTheDocument();
    expect(surveyActionLink.getAttribute("href")).toBe("/th/surveys/field?opportunityId=opp-srv-001");
  });

  it("renders empty state when no survey opportunities exist", () => {
    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: {
        pages: [{ items: [], totalCount: 0 }],
      },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <SurveyList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("ไม่พบงานสำรวจหน้างาน")).toBeInTheDocument();
    expect(screen.getByText("ยังไม่มีโอกาสทางการขายที่อยู่ในขั้นตอนสำรวจ หรือไม่พบข้อมูลตามตัวกรอง")).toBeInTheDocument();
  });
});
