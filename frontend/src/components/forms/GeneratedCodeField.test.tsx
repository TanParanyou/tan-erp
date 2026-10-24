import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { GeneratedCodeField } from "./GeneratedCodeField";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const translations: Record<string, string> = {
      codeEntryMode: "Code Entry Mode",
      codeModeGenerated: "Auto Generated",
      codeModeManual: "Manual Entry",
      codeGeneratedAtSave: "Code will be generated upon save",
      codePlaceholder: "e.g. ITM-001",
    };
    return translations[key] ?? key;
  },
}));

describe("GeneratedCodeField", () => {
  it("renders with generated mode active and displays note instead of text input", () => {
    const onChange = vi.fn();
    const onModeChange = vi.fn();

    render(
      <GeneratedCodeField
        id="item-code"
        label="Item Code"
        value=""
        mode="generated"
        maxLength={32}
        onChange={onChange}
        onModeChange={onModeChange}
      />
    );

    const generatedRadio = screen.getByRole("radio", { name: "Auto Generated" });
    const manualRadio = screen.getByRole("radio", { name: "Manual Entry" });

    expect(generatedRadio).toBeChecked();
    expect(manualRadio).not.toBeChecked();
    expect(
      screen.getByText("Code will be generated upon save")
    ).toBeInTheDocument();
    expect(screen.queryByRole("textbox")).not.toBeInTheDocument();
  });

  it("renders with manual mode active and shows text input for code", () => {
    const onChange = vi.fn();
    const onModeChange = vi.fn();

    render(
      <GeneratedCodeField
        id="item-code"
        label="Item Code"
        value="CUSTOM-001"
        mode="manual"
        maxLength={32}
        onChange={onChange}
        onModeChange={onModeChange}
      />
    );

    const manualRadio = screen.getByRole("radio", { name: "Manual Entry" });
    expect(manualRadio).toBeChecked();

    const input = screen.getByRole("textbox", { name: /Item Code/ });
    expect(input).toBeInTheDocument();
    expect(input).toHaveValue("CUSTOM-001");

    fireEvent.change(input, { target: { value: "NEW-CODE" } });
    expect(onChange).toHaveBeenCalledWith("NEW-CODE");
  });

  it("handles mode switching via radio button click", () => {
    const onChange = vi.fn();
    const onModeChange = vi.fn();

    render(
      <GeneratedCodeField
        id="item-code"
        label="Item Code"
        value=""
        mode="generated"
        maxLength={32}
        onChange={onChange}
        onModeChange={onModeChange}
      />
    );

    const manualRadio = screen.getByRole("radio", { name: "Manual Entry" });
    fireEvent.click(manualRadio);

    expect(onModeChange).toHaveBeenCalledWith("manual");
  });

  it("disables both radios and input when disabled prop is true", () => {
    const onChange = vi.fn();
    const onModeChange = vi.fn();

    render(
      <GeneratedCodeField
        id="item-code"
        label="Item Code"
        value="CODE-123"
        mode="manual"
        maxLength={32}
        disabled
        onChange={onChange}
        onModeChange={onModeChange}
      />
    );

    expect(screen.getByRole("radio", { name: "Auto Generated" })).toBeDisabled();
    expect(screen.getByRole("radio", { name: "Manual Entry" })).toBeDisabled();
    expect(screen.getByRole("textbox", { name: /Item Code/ })).toBeDisabled();
  });

  it("displays error message on manual input when error is provided", () => {
    render(
      <GeneratedCodeField
        id="item-code"
        label="Item Code"
        value=""
        mode="manual"
        maxLength={32}
        error="Code already exists"
        onChange={vi.fn()}
        onModeChange={vi.fn()}
      />
    );

    expect(screen.getByRole("alert")).toHaveTextContent("Code already exists");
  });
});

