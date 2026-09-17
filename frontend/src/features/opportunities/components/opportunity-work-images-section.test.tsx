import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityWorkImagesSection } from "./opportunity-work-images-section";
import * as queries from "../api/opportunity-queries";
import * as toastHook from "@/hooks/useToast";
import type { OpportunityWorkImageResponse } from "@/lib/api/api-client";

// Mock queries
vi.mock("../api/opportunity-queries", () => ({
  useOpportunityWorkImages: vi.fn(),
  useDetachWorkImage: vi.fn(),
  useAttachWorkImages: vi.fn(() => ({
    mutateAsync: vi.fn(),
    isPending: false,
  })),
}));

// Mock toast
const mockToast = {
  toast: {
    success: vi.fn(),
    error: vi.fn(),
    info: vi.fn(),
    warning: vi.fn(),
  },
};
vi.mock("@/hooks/useToast", () => ({
  useToast: () => mockToast,
  ToastProvider: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

// Mock fileClient
vi.mock("@/lib/api/file-client", () => ({
  fileClient: {
    getFileUrl: (id: string) => `https://files.test/${id}`,
  },
}));

function renderComponent(props = {}) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });

  const defaultProps = {
    opportunityId: "opp-123",
    currentStage: "surveying",
    rowVersion: "rev-1",
    canManage: true,
    ...props,
  };

  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <QueryClientProvider client={queryClient}>
        <OpportunityWorkImagesSection {...defaultProps} />
      </QueryClientProvider>
    </NextIntlClientProvider>
  );
}

describe("OpportunityWorkImagesSection Component", () => {
  const mockMutateAsync = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(queries.useDetachWorkImage).mockReturnValue({
      mutateAsync: mockMutateAsync,
      isPending: false,
    } as unknown as ReturnType<typeof queries.useDetachWorkImage>);
  });

  it("renders loading spinner while fetching images", () => {
    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    renderComponent();
    expect(screen.getByText("กำลังโหลดภาพถ่ายหน้างาน...")).toBeInTheDocument();
  });

  it("renders empty state when there are no images", () => {
    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: { items: [], nextCursor: null },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    renderComponent();
    expect(screen.getByText("ยังไม่มีภาพถ่ายหน้างานแนบในโอกาสทางการขายนี้")).toBeInTheDocument();
    expect(
      screen.getByText("แนบภาพถ่ายสภาพพื้นที่จริง ตำแหน่งหน้างาน หรือแบบร่าง เพื่อใช้อ้างอิงการสำรวจและประมาณราคา")
    ).toBeInTheDocument();
  });

  it("renders image gallery with localized stage badge, caption, and uploader", () => {
    const mockImages: OpportunityWorkImageResponse[] = [
      {
        id: "img-1",
        fileId: "file-1",
        stageAtAttach: "draft",
        caption: "ภาพถ่ายร่างห้องครัว",
        displayOrder: 1,
        createdAtUtc: "2026-09-17T08:00:00Z",
        createdBy: { id: "user-1", displayName: "นาย ก" },
      },
      {
        id: "img-2",
        fileId: "file-2",
        stageAtAttach: "surveying",
        caption: "ภาพถ่ายวัดพื้นที่หน้างาน",
        displayOrder: 2,
        createdAtUtc: "2026-09-17T09:00:00Z",
        createdBy: { id: "user-2", displayName: "นาย ข" },
      },
    ];

    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: { items: mockImages, nextCursor: null },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    renderComponent();

    // Verify badges and captions
    expect(screen.getByText("ภาพถ่ายร่างห้องครัว")).toBeInTheDocument();
    expect(screen.getByText("ภาพถ่ายวัดพื้นที่หน้างาน")).toBeInTheDocument();
    expect(screen.getByText("นาย ก")).toBeInTheDocument();
    expect(screen.getByText("นาย ข")).toBeInTheDocument();

    // Stage badges should be translated (not raw English)
    expect(screen.getAllByText(/ฉบับร่าง/)[0]).toBeInTheDocument();
    expect(screen.getAllByText(/สำรวจหน้างาน/)[0]).toBeInTheDocument();
  });

  it("filters images by clicking stage tabs", () => {
    const mockImages: OpportunityWorkImageResponse[] = [
      {
        id: "img-1",
        fileId: "file-1",
        stageAtAttach: "draft",
        caption: "ภาพฉบับร่าง",
        displayOrder: 1,
        createdAtUtc: "2026-09-17T08:00:00Z",
        createdBy: { id: "user-1", displayName: "นาย ก" },
      },
      {
        id: "img-2",
        fileId: "file-2",
        stageAtAttach: "surveying",
        caption: "ภาพสำรวจ",
        displayOrder: 2,
        createdAtUtc: "2026-09-17T09:00:00Z",
        createdBy: { id: "user-2", displayName: "นาย ข" },
      },
    ];

    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: { items: mockImages, nextCursor: null },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    renderComponent();

    expect(screen.getByText("ภาพฉบับร่าง")).toBeInTheDocument();
    expect(screen.getByText("ภาพสำรวจ")).toBeInTheDocument();

    const draftBtn = screen.getByRole("button", { name: /ฉบับร่าง.*\(1\)/i });
    fireEvent.click(draftBtn);

    // Only draft image should be visible
    expect(screen.getByText("ภาพฉบับร่าง")).toBeInTheDocument();
    expect(screen.queryByText("ภาพสำรวจ")).not.toBeInTheDocument();

    // Clear filter back to all stages
    const allBtn = screen.getByRole("button", { name: /ทุกขั้นตอน \(2\)/i });
    fireEvent.click(allBtn);

    expect(screen.getByText("ภาพฉบับร่าง")).toBeInTheDocument();
    expect(screen.getByText("ภาพสำรวจ")).toBeInTheDocument();
  });

  it("opens GalleryLightboxModal when clicking on an image card", () => {
    const mockImages: OpportunityWorkImageResponse[] = [
      {
        id: "img-1",
        fileId: "file-1",
        stageAtAttach: "surveying",
        caption: "ภาพสำรวจหลัก",
        displayOrder: 1,
        createdAtUtc: "2026-09-17T08:00:00Z",
        createdBy: { id: "user-1", displayName: "นาย ก" },
      },
    ];

    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: { items: mockImages, nextCursor: null },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    renderComponent();

    // Click thumbnail container
    const cardThumbnail = screen.getByRole("button", { name: "ภาพสำรวจหลัก" });
    fireEvent.click(cardThumbnail);

    // Lightbox modal dialog should be open
    const dialog = screen.getByRole("dialog");
    expect(dialog).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /ปิดหน้าต่างรูปภาพ/i })).toBeInTheDocument();
  });

  it("opens ConfirmationModal and performs detach flow with toast notification", async () => {
    const mockImages: OpportunityWorkImageResponse[] = [
      {
        id: "img-to-delete",
        fileId: "file-1",
        stageAtAttach: "surveying",
        caption: "ภาพถ่ายที่จะลบ",
        displayOrder: 1,
        createdAtUtc: "2026-09-17T08:00:00Z",
        createdBy: { id: "user-1", displayName: "นาย ก" },
      },
    ];

    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: { items: mockImages, nextCursor: null },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    renderComponent();

    // Click delete button on card
    const deleteBtn = screen.getByRole("button", { name: /ลบ ภาพถ่ายที่จะลบ/i });
    fireEvent.click(deleteBtn);

    // ConfirmationModal should appear
    expect(screen.getByText("ยืนยันการถอดภาพถ่าย")).toBeInTheDocument();
    expect(
      screen.getByText("คุณแน่ใจหรือไม่ว่าต้องการถอดภาพถ่ายนี้ออกจากโอกาสทางการขาย?")
    ).toBeInTheDocument();

    // Click confirm delete in modal
    const confirmDeleteBtn = screen.getAllByRole("button", { name: /^ลบ$/i })[0];
    fireEvent.click(confirmDeleteBtn);

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledWith(
        expect.objectContaining({
          opportunityId: "opp-123",
          imageId: "img-to-delete",
          expectedVersion: "rev-1",
        })
      );
      expect(mockToast.toast.success).toHaveBeenCalledWith("ถอดภาพถ่ายเรียบร้อยแล้ว");
    });
  });

  it("hides attach and delete buttons when canManage is false or opportunity is closed", () => {
    const mockImages: OpportunityWorkImageResponse[] = [
      {
        id: "img-1",
        fileId: "file-1",
        stageAtAttach: "won",
        caption: "ภาพงานจบ",
        displayOrder: 1,
        createdAtUtc: "2026-09-17T08:00:00Z",
        createdBy: { id: "user-1", displayName: "นาย ก" },
      },
    ];

    vi.mocked(queries.useOpportunityWorkImages).mockReturnValue({
      data: { items: mockImages, nextCursor: null },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof queries.useOpportunityWorkImages>);

    // Render with canManage = false
    const { rerender } = renderComponent({ canManage: false, currentStage: "surveying" });

    expect(screen.queryByRole("button", { name: /แนบภาพถ่ายหน้างาน/i })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /ลบ/i })).not.toBeInTheDocument();

    // Rerender with currentStage = 'won' (closed)
    rerender(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <QueryClientProvider client={new QueryClient()}>
          <OpportunityWorkImagesSection
            opportunityId="opp-123"
            currentStage="won"
            rowVersion="rev-1"
            canManage={true}
          />
        </QueryClientProvider>
      </NextIntlClientProvider>
    );

    expect(screen.queryByRole("button", { name: /แนบภาพถ่ายหน้างาน/i })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /ลบ/i })).not.toBeInTheDocument();
  });
});
