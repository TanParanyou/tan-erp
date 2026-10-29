import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SurveyTemplateManager } from "./survey-template-manager";
import type { SurveyTemplateVersionResponse } from "@/lib/api/api-client";

const mockTemplates: SurveyTemplateVersionResponse[] = [
  {
    code: "ST-2026-V1",
    requiredChecklistItems: ["ตรวจโครงสร้างเสา", "วัดความสูงเพดาน"],
    minimumEvidenceCount: 1,
    minimumAreaEvidenceCount: 1,
    snapshotHashVersion: "v2",
    effectiveFromUtc: "2026-10-01T00:00:00Z",
    effectiveToUtc: "2026-10-15T00:00:00Z",
    isCurrent: false,
  },
  {
    code: "ST-2026-V4",
    requiredChecklistItems: ["ตรวจความปลอดภัย", "ตรวจสอบแนวท่อ", "วัดระยะทางเดิน"],
    minimumEvidenceCount: 2,
    minimumAreaEvidenceCount: 1,
    snapshotHashVersion: "v4",
    effectiveFromUtc: "2026-10-15T00:00:00Z",
    effectiveToUtc: null,
    isCurrent: true,
  },
];

const mockPublish = vi.fn().mockResolvedValue({});
const mockRetire = vi.fn().mockResolvedValue({});
const mockRefetch = vi.fn();

vi.mock("../../api/survey-queries", () => ({
  useSurveyTemplateVersions: () => ({
    data: { items: mockTemplates },
    isLoading: false,
    isError: false,
    refetch: mockRefetch,
  }),
  usePublishSurveyTemplateVersion: () => ({
    mutateAsync: mockPublish,
    isPending: false,
  }),
  useRetireSurveyTemplateVersion: () => ({
    mutateAsync: mockRetire,
    isPending: false,
  }),
}));

const mockToastSuccess = vi.fn();
const mockToastError = vi.fn();

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({
    toast: {
      success: mockToastSuccess,
      error: mockToastError,
    },
  }),
}));

function renderComponent() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });

  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <QueryClientProvider client={queryClient}>
        <SurveyTemplateManager />
      </QueryClientProvider>
    </NextIntlClientProvider>
  );
}

describe("SurveyTemplateManager Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders template table with records correctly", () => {
    renderComponent();

    expect(screen.getByText("ST-2026-V1")).toBeInTheDocument();
    expect(screen.getByText("ST-2026-V4")).toBeInTheDocument();
    expect(screen.getByText("รุ่นปัจจุบัน")).toBeInTheDocument();
    expect(screen.getByText("v4")).toBeInTheDocument();
    expect(screen.getByText("v2")).toBeInTheDocument();
  });

  it("opens publish drawer when clicking publish new button", () => {
    renderComponent();

    const publishBtn = screen.getByRole("button", { name: /เผยแพร่แม่แบบใหม่/i });
    fireEvent.click(publishBtn);

    expect(screen.getByText("เผยแพร่แม่แบบการสำรวจใหม่")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("เช่น ST-2026-V3")).toBeInTheDocument();
  });

  it("opens retire modal and confirms retirement for active template", async () => {
    renderComponent();

    const retireBtn = screen.getByRole("button", { name: "ยกเลิกใช้งาน" });
    fireEvent.click(retireBtn);

    expect(screen.getByText("ยืนยันการยกเลิกแม่แบบการสำรวจ")).toBeInTheDocument();
    expect(
      screen.getByText("คุณต้องการสิ้นสุดระยะเวลาการใช้งานของแม่แบบ ST-2026-V4 หรือไม่?")
    ).toBeInTheDocument();

    const confirmBtn = screen.getByRole("button", { name: "ยืนยันยกเลิกแม่แบบ" });
    fireEvent.click(confirmBtn);

    await waitFor(() => {
      expect(mockRetire).toHaveBeenCalledWith("ST-2026-V4");
      expect(mockToastSuccess).toHaveBeenCalledWith("ยกเลิกแม่แบบการสำรวจสำเร็จแล้ว");
    });
  });
});
