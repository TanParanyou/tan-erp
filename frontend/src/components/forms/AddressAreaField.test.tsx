import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { AddressAreaField } from "./AddressAreaField";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
}));

vi.mock("./AddressAutocomplete", () => ({
  AddressAutocomplete: ({
    id,
    label,
    hint,
    disabled,
    onSelect,
  }: {
    id?: string;
    label?: string;
    hint?: string;
    disabled?: boolean;
    onSelect?: (val: unknown) => void;
  }) => (
    <div data-testid="mock-address-autocomplete">
      <label htmlFor={id}>{label}</label>
      <input id={id} disabled={disabled} placeholder={hint} />
      <button
        type="button"
        data-testid="mock-select-btn"
        onClick={() =>
          onSelect?.({
            subdistrict: "คลองตันเหนือ",
            district: "วัฒนา",
            province: "กรุงเทพมหานคร",
            postalCode: "10110",
            countryCode: "TH",
          })
        }
      >
        Select
      </button>
    </div>
  ),
}));

describe("AddressAreaField", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders autocomplete when no address is selected", () => {
    render(
      <AddressAreaField
        label="พื้นที่ / รหัสไปรษณีย์"
        hint="ค้นหาตำบล อำเภอ หรือรหัสไปรษณีย์"
        onSelect={vi.fn()}
        onClear={vi.fn()}
      />
    );

    expect(screen.getByTestId("mock-address-autocomplete")).toBeDefined();
    expect(screen.getByText("พื้นที่ / รหัสไปรษณีย์")).toBeDefined();
  });

  it("displays error message when provided and no address is selected", () => {
    render(
      <AddressAreaField
        label="พื้นที่"
        error="กรุณาระบุที่อยู่ให้ครบถ้วน"
        onSelect={vi.fn()}
        onClear={vi.fn()}
      />
    );

    expect(screen.getByRole("alert").textContent).toBe("กรุณาระบุที่อยู่ให้ครบถ้วน");
  });

  it("renders summary card when address is fully selected", () => {
    render(
      <AddressAreaField
        value={{
          subdistrict: "คลองตันเหนือ",
          district: "วัฒนา",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
          countryCode: "TH",
        }}
        onSelect={vi.fn()}
        onClear={vi.fn()}
      />
    );

    expect(screen.queryByTestId("mock-address-autocomplete")).toBeNull();
    expect(screen.getByText("คลองตันเหนือ » วัฒนา » กรุงเทพมหานคร")).toBeDefined();
    expect(screen.getByText("10110 • TH")).toBeDefined();
    expect(screen.getByRole("button", { name: "changeArea" })).toBeDefined();
  });

  it("calls onClear when change button is clicked", () => {
    const onClearMock = vi.fn();

    render(
      <AddressAreaField
        value={{
          subdistrict: "คลองตันเหนือ",
          district: "วัฒนา",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
        }}
        onSelect={vi.fn()}
        onClear={onClearMock}
      />
    );

    const changeBtn = screen.getByRole("button", { name: "changeArea" });
    fireEvent.click(changeBtn);

    expect(onClearMock).toHaveBeenCalledTimes(1);
  });

  it("disables change button when disabled prop is true", () => {
    render(
      <AddressAreaField
        disabled
        value={{
          subdistrict: "คลองตันเหนือ",
          district: "วัฒนา",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
        }}
        onSelect={vi.fn()}
        onClear={vi.fn()}
      />
    );

    const changeBtn = screen.getByRole("button", { name: "changeArea" });
    expect(changeBtn.hasAttribute("disabled")).toBe(true);
  });
});
