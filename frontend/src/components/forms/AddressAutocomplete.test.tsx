import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { AddressAutocomplete } from "./AddressAutocomplete";
import { apiClient } from "@/lib/api/api-client";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn().mockResolvedValue("mock-token"),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "mock-membership-id" },
  }),
}));

describe("AddressAutocomplete", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders input with combobox role and placeholder", () => {
    render(
      <AddressAutocomplete
        label="Address Search"
        placeholder="Type to search"
        onSelect={vi.fn()}
      />
    );

    expect(screen.getByLabelText("Address Search")).toBeDefined();
    const input = screen.getByRole("combobox");
    expect(input.getAttribute("placeholder")).toBe("Type to search");
    expect(input.getAttribute("aria-expanded")).toBe("false");
  });

  it("searches and calls onSelect when an item is selected", async () => {
    const onSelectMock = vi.fn();
    const searchMock = vi.spyOn(apiClient, "searchAddresses").mockResolvedValue({
      items: [
        {
          subdistrictCode: "103901",
          subdistrict: "คลองตันเหนือ",
          district: "วัฒนา",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
          countryCode: "TH",
          latitude: 13.7386,
          longitude: 100.5847,
          displayText: "คลองตันเหนือ » วัฒนา » กรุงเทพมหานคร 10110",
        },
      ],
    });

    render(<AddressAutocomplete onSelect={onSelectMock} />);

    const input = screen.getByRole("combobox");
    fireEvent.change(input, { target: { value: "10110" } });

    await waitFor(() => {
      expect(searchMock).toHaveBeenCalledWith(
        "10110",
        expect.objectContaining({ token: "mock-token" }),
        20
      );
    });

    const option = await screen.findByRole("option");
    expect(option.textContent).toContain("คลองตันเหนือ");

    fireEvent.click(option);

    expect(onSelectMock).toHaveBeenCalledWith({
      subdistrict: "คลองตันเหนือ",
      district: "วัฒนา",
      province: "กรุงเทพมหานคร",
      postalCode: "10110",
      countryCode: "TH",
      latitude: 13.7386,
      longitude: 100.5847,
    });
  });

  it("navigates options via keyboard and selects on Enter", async () => {
    const onSelectMock = vi.fn();
    vi.spyOn(apiClient, "searchAddresses").mockResolvedValue({
      items: [
        {
          subdistrictCode: "103901",
          subdistrict: "คลองตันเหนือ",
          district: "วัฒนา",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
          countryCode: "TH",
          latitude: null,
          longitude: null,
          displayText: "คลองตันเหนือ » วัฒนา » กรุงเทพมหานคร 10110",
        },
      ],
    });

    render(<AddressAutocomplete onSelect={onSelectMock} />);

    const input = screen.getByRole("combobox");
    fireEvent.change(input, { target: { value: "คลองตัน" } });

    await screen.findByRole("option");

    // Press ArrowDown to highlight first item
    fireEvent.keyDown(input, { key: "ArrowDown" });
    // Press Enter to select
    fireEvent.keyDown(input, { key: "Enter" });

    expect(onSelectMock).toHaveBeenCalledWith(
      expect.objectContaining({
        subdistrict: "คลองตันเหนือ",
        district: "วัฒนา",
      })
    );
  });
});
