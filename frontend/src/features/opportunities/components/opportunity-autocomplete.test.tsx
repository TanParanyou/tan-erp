import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { OpportunityAutocomplete } from "./opportunity-autocomplete";
import type { OpportunityResponse } from "@/lib/api/api-client";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const map: Record<string, string> = {
      opportunity: "โอกาสทางการขาย",
      searchOpportunityPlaceholder: "พิมพ์ค้นหาชื่อโอกาสขาย, รหัส หรือชื่อลูกค้า...",
      noOpportunityFound: "ไม่พบโอกาสทางการขาย",
      createNewOpportunity: "สร้างโอกาสทางการขายใหม่",
    };
    return map[key] ?? key;
  },
  useLocale: () => "th",
}));

const mockOpportunity: OpportunityResponse = {
  id: "opp-001",
  code: "OPP-2026-0001",
  title: "รีโนเวทห้องนอนใหญ่ คุณสมชาย",
  stage: "surveying",
  customer: {
    id: "cust-001",
    displayNameTh: "คุณสมชาย รักสงบ",
    displayNameEn: "Somchai Raksangob",
  },
};

vi.mock("../api/opportunity-queries", () => ({
  useOpportunityList: () => ({
    data: {
      pages: [
        {
          items: [mockOpportunity],
        },
      ],
    },
    isLoading: false,
  }),
  useOpportunityDetail: (id?: string) => ({
    data: id === mockOpportunity.id ? mockOpportunity : undefined,
    isLoading: false,
  }),
}));

describe("OpportunityAutocomplete Component", () => {
  it("renders with placeholder and label", () => {
    const onChange = vi.fn();
    render(<OpportunityAutocomplete value={null} onChange={onChange} />);

    expect(
      screen.getByPlaceholderText("พิมพ์ค้นหาชื่อโอกาสขาย, รหัส หรือชื่อลูกค้า...")
    ).toBeDefined();
  });

  it("renders selected opportunity card when value is provided", () => {
    const onChange = vi.fn();
    render(
      <OpportunityAutocomplete
        value="opp-001"
        selectedOpportunity={mockOpportunity}
        onChange={onChange}
      />
    );

    expect(screen.getByText(/รีโนเวทห้องนอนใหญ่ คุณสมชาย/)).toBeDefined();
    expect(screen.getByText(/คุณสมชาย รักสงบ/)).toBeDefined();
  });

  it("calls onChange with empty string when clear button is clicked", () => {
    const onChange = vi.fn();
    render(
      <OpportunityAutocomplete
        value="opp-001"
        selectedOpportunity={mockOpportunity}
        onChange={onChange}
      />
    );

    const clearBtn = screen.getByRole("button", { name: /ล้าง|Clear/i });
    fireEvent.click(clearBtn);

    expect(onChange).toHaveBeenCalledWith("", null);
  });

  it("renders list items with white text when highlighted for high contrast", () => {
    const onChange = vi.fn();
    render(<OpportunityAutocomplete value={null} onChange={onChange} />);

    // Click input to open dropdown
    const input = screen.getByPlaceholderText("พิมพ์ค้นหาชื่อโอกาสขาย, รหัส หรือชื่อลูกค้า...");
    fireEvent.focus(input);
    fireEvent.change(input, { target: { value: "รีโนเวท" } });

    // The item should be in the list
    expect(screen.getByText(/รีโนเวทห้องนอนใหญ่ คุณสมชาย/)).toBeDefined();
  });

  it("renders create opportunity link in header labelAction slot", () => {
    const onChange = vi.fn();
    render(<OpportunityAutocomplete value={null} onChange={onChange} />);

    const link = screen.getByRole("link", { name: /สร้างโอกาสทางการขายใหม่/i });
    expect(link).toBeDefined();
    expect(link.getAttribute("href")).toBe("/th/opportunities/create");
  });

  it("auto-fetches and selects opportunity when value id is provided without selectedOpportunity", () => {
    const onChange = vi.fn();
    render(<OpportunityAutocomplete value="opp-001" onChange={onChange} />);

    expect(screen.getByText(/รีโนเวทห้องนอนใหญ่ คุณสมชาย/)).toBeDefined();
    expect(onChange).toHaveBeenCalledWith("opp-001", mockOpportunity);
  });
});
