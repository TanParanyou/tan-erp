import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import {
  SurveyStylePreferenceSelector,
  type SurveyDesignStyleOption,
  type SurveyMoodToneOption,
} from "./survey-style-preference-selector";

const mockStyles: SurveyDesignStyleOption[] = [
  {
    code: "modern_luxury",
    label: "Modern Luxury",
    description: "โมเดิร์น ลักชูรี่ หรูหรา",
    imageFileId: "img-style-1",
  },
  {
    code: "japandi",
    label: "Japandi",
    description: "มูจิ อบอุ่น เรียบง่าย",
  },
];

const mockTones: SurveyMoodToneOption[] = [
  {
    code: "warm_earthy",
    label: "Warm & Earthy",
    description: "โทนอุ่น น้ำตาล ครีม",
    colorPalette: ["#F5EBE0", "#D6CCC2"],
  },
];

function renderWithProviders(ui: React.ReactElement) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  return render(
    <QueryClientProvider client={queryClient}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        {ui}
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("SurveyStylePreferenceSelector", () => {
  it("renders style options and triggers change when clicked", () => {
    const onStyleChange = vi.fn();
    const onToneChange = vi.fn();

    renderWithProviders(
      <SurveyStylePreferenceSelector
        designStyles={mockStyles}
        moodTones={mockTones}
        selectedDesignStyle=""
        selectedMoodTone=""
        onDesignStyleChange={onStyleChange}
        onMoodToneChange={onToneChange}
      />
    );

    expect(screen.getByText("Modern Luxury")).toBeInTheDocument();
    expect(screen.getByText("Japandi")).toBeInTheDocument();
    expect(screen.getByText("Warm & Earthy")).toBeInTheDocument();

    const modernBtn = screen.getByRole("button", { name: /Modern Luxury/i });
    fireEvent.click(modernBtn);
    expect(onStyleChange).toHaveBeenCalledWith("modern_luxury");

    const toneBtn = screen.getByRole("button", { name: /Warm & Earthy/i });
    fireEvent.click(toneBtn);
    expect(onToneChange).toHaveBeenCalledWith("warm_earthy");
  });

  it("triggers toggle off when clicking selected style", () => {
    const onStyleChange = vi.fn();

    renderWithProviders(
      <SurveyStylePreferenceSelector
        designStyles={mockStyles}
        moodTones={mockTones}
        selectedDesignStyle="modern_luxury"
        selectedMoodTone=""
        onDesignStyleChange={onStyleChange}
        onMoodToneChange={vi.fn()}
      />
    );

    const modernBtn = screen.getByRole("button", { name: /Modern Luxury/i });
    fireEvent.click(modernBtn);
    expect(onStyleChange).toHaveBeenCalledWith("");
  });
});
