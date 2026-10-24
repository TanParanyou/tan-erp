import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SurveyMasterDataManager } from "./survey-master-data-manager";
import type { SurveyDesignStyleResponse, SurveyMoodToneResponse } from "@/lib/api/api-client";

const mockStyles: SurveyDesignStyleResponse[] = [
  {
    id: "style-1",
    code: "modern",
    name: { thai: "โมเดิร์น", english: "Modern" },
    description: { thai: "เรียบหรู ทันสมัย", english: "Sleek and contemporary" },
    sortOrder: 1,
    isActive: true,
    createdAtUtc: "2026-10-24T00:00:00Z",
  },
  {
    id: "style-2",
    code: "minimal",
    name: { thai: "มินิมอล", english: "Minimal" },
    description: { thai: "เรียบง่าย โปร่งสบาย", english: "Simple and airy" },
    sortOrder: 2,
    isActive: false,
    createdAtUtc: "2026-10-24T00:00:00Z",
  },
];

const mockTones: SurveyMoodToneResponse[] = [
  {
    id: "tone-1",
    code: "warm-cozy",
    name: { thai: "อบอุ่น ผ่อนคลาย", english: "Warm Cozy" },
    description: { thai: "โทนสีอบอุ่น นุ่มนวล", english: "Warm and cozy tones" },
    colorPalette: ["#F5E6D3", "#D4A373", "#A98467"],
    sortOrder: 1,
    isActive: true,
    createdAtUtc: "2026-10-24T00:00:00Z",
  },
];

const mockCreateStyle = vi.fn().mockResolvedValue({});
const mockUpdateStyle = vi.fn().mockResolvedValue({});
const mockCreateTone = vi.fn().mockResolvedValue({});
const mockUpdateTone = vi.fn().mockResolvedValue({});
const mockRefetchStyles = vi.fn();
const mockRefetchTones = vi.fn();

vi.mock("../../api/survey-queries", () => ({
  useSurveyDesignStyles: () => ({
    data: mockStyles,
    isLoading: false,
    isError: false,
    refetch: mockRefetchStyles,
  }),
  useSurveyMoodTones: () => ({
    data: mockTones,
    isLoading: false,
    isError: false,
    refetch: mockRefetchTones,
  }),
  useCreateSurveyDesignStyle: () => ({
    mutateAsync: mockCreateStyle,
    isPending: false,
  }),
  useUpdateSurveyDesignStyle: () => ({
    mutateAsync: mockUpdateStyle,
    isPending: false,
  }),
  useCreateSurveyMoodTone: () => ({
    mutateAsync: mockCreateTone,
    isPending: false,
  }),
  useUpdateSurveyMoodTone: () => ({
    mutateAsync: mockUpdateTone,
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
    defaultOptions: {
      queries: { retry: false },
    },
  });

  return render(
    <QueryClientProvider client={queryClient}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <SurveyMasterDataManager />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("SurveyMasterDataManager", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders page header and design styles by default", () => {
    renderComponent();

    expect(screen.getByText("ตั้งค่าข้อมูลสำรวจและดีไซน์ (Survey & Design Settings)")).toBeInTheDocument();
    expect(screen.getByText("modern")).toBeInTheDocument();
    expect(screen.getByText("โมเดิร์น")).toBeInTheDocument();
    expect(screen.getByText("Modern")).toBeInTheDocument();
    expect(screen.getByText("minimal")).toBeInTheDocument();
  });

  it("switches to mood tones tab and displays color swatches", () => {
    renderComponent();

    const moodTab = screen.getByRole("button", { name: /มู้ดและโทนสี/i });
    fireEvent.click(moodTab);

    expect(screen.getByText("warm-cozy")).toBeInTheDocument();
    expect(screen.getByText("อบอุ่น ผ่อนคลาย")).toBeInTheDocument();
    expect(screen.getByText("Warm Cozy")).toBeInTheDocument();

    const swatches = document.querySelectorAll("[title='#F5E6D3']");
    expect(swatches.length).toBeGreaterThan(0);
  });

  it("opens create drawer when clicking add button", () => {
    renderComponent();

    const addBtn = screen.getByRole("button", { name: /เพิ่มสไตล์การออกแบบ/i });
    fireEvent.click(addBtn);

    expect(screen.getByPlaceholderText(/เช่น modern, japandi/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /บันทึกข้อมูล/i })).toBeInTheDocument();
  });

  it("opens edit drawer with prefilled data when edit button is clicked", () => {
    renderComponent();

    const editButtons = screen.getAllByRole("button", { name: /แก้ไข/i });
    fireEvent.click(editButtons[0]);

    const codeInput = screen.getByDisplayValue("modern");
    expect(codeInput).toBeDisabled(); // Code is disabled in edit mode
    expect(screen.getByDisplayValue("โมเดิร์น")).toBeInTheDocument();
  });

  it("submits create design style successfully", async () => {
    renderComponent();

    fireEvent.click(screen.getByRole("button", { name: /เพิ่มสไตล์การออกแบบ/i }));

    const codeInput = screen.getByPlaceholderText(/เช่น modern, japandi/i);
    fireEvent.change(codeInput, { target: { value: "industrial" } });

    const nameInputs = screen.getAllByRole("textbox");
    // Find the Thai name input
    const thaiNameInput = nameInputs.find(
      (input) => (input as HTMLInputElement).placeholder?.includes("ชื่อสไตล์การออกแบบ") || (input as HTMLInputElement).value === ""
    );
    if (thaiNameInput) {
      fireEvent.change(thaiNameInput, { target: { value: "อินดัสเทรียล" } });
    }

    const saveButton = screen.getByRole("button", { name: /บันทึกข้อมูล/i });
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(mockCreateStyle).toHaveBeenCalledWith(
        expect.objectContaining({
          code: "industrial",
          name: expect.objectContaining({
            thai: "อินดัสเทรียล",
          }),
        })
      );
      expect(mockToastSuccess).toHaveBeenCalled();
    });
  });
});
