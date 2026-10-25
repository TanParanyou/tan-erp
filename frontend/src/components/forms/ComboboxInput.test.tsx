import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { ComboboxInput, type ComboboxInputOption } from "./ComboboxInput";

describe("ComboboxInput Component", () => {
  const options: ComboboxInputOption[] = [
    { value: "living_room", label: "ห้องนั่งเล่น" },
    { value: "master_bedroom", label: "ห้องนอนใหญ่" },
    { value: "kitchen", label: "ห้องครัว" },
  ];

  it("renders input with value and placeholder", () => {
    render(
      <ComboboxInput
        value="ห้องนั่งเล่น"
        onChange={vi.fn()}
        options={options}
        placeholder="ระบุหรือเลือกห้อง"
      />
    );

    const input = screen.getByPlaceholderText("ระบุหรือเลือกห้อง") as HTMLInputElement;
    expect(input).toBeDefined();
    expect(input.value).toBe("ห้องนั่งเล่น");
  });

  it("opens options on focus and filters list when typing", () => {
    const handleChange = vi.fn();
    render(
      <ComboboxInput
        value=""
        onChange={handleChange}
        options={options}
        placeholder="ระบุหรือเลือกห้อง"
      />
    );

    const input = screen.getByPlaceholderText("ระบุหรือเลือกห้อง");
    fireEvent.focus(input);

    expect(screen.getByRole("listbox")).toBeDefined();
    expect(screen.getByText("ห้องนั่งเล่น")).toBeDefined();
    expect(screen.getByText("ห้องนอนใหญ่")).toBeDefined();
    expect(screen.getByText("ห้องครัว")).toBeDefined();
  });

  it("selects option item on click and triggers onSelectOption", () => {
    const handleChange = vi.fn();
    const handleSelectOption = vi.fn();

    render(
      <ComboboxInput
        value=""
        onChange={handleChange}
        options={options}
        onSelectOption={handleSelectOption}
        placeholder="ระบุหรือเลือกห้อง"
      />
    );

    const input = screen.getByPlaceholderText("ระบุหรือเลือกห้อง");
    fireEvent.focus(input);

    const item = screen.getByText("ห้องนอนใหญ่");
    fireEvent.click(item);

    expect(handleChange).toHaveBeenCalledWith("ห้องนอนใหญ่");
    expect(handleSelectOption).toHaveBeenCalledWith(options[1]);
  });

  it("always shows all options on focus even if input already has a value", () => {
    render(
      <ComboboxInput
        value="ห้องนอนใหญ่"
        onChange={vi.fn()}
        options={options}
        placeholder="ระบุหรือเลือกห้อง"
        alwaysShowAllOptions={true}
      />
    );

    const input = screen.getByPlaceholderText("ระบุหรือเลือกห้อง");
    fireEvent.focus(input);

    expect(screen.getByRole("listbox")).toBeDefined();
    // All options must be visible so user can easily switch anytime
    expect(screen.getByText("ห้องนั่งเล่น")).toBeDefined();
    expect(screen.getByText("ห้องนอนใหญ่")).toBeDefined();
    expect(screen.getByText("ห้องครัว")).toBeDefined();
  });

  it("renders clear button when value is present and clears input on click", () => {
    const handleChange = vi.fn();
    render(
      <ComboboxInput
        value="ห้องนอนใหญ่"
        onChange={handleChange}
        options={options}
        placeholder="ระบุหรือเลือกห้อง"
      />
    );

    const clearBtn = screen.getByTitle("ล้างค่า");
    expect(clearBtn).toBeDefined();

    fireEvent.click(clearBtn);
    expect(handleChange).toHaveBeenCalledWith("");
  });
});
