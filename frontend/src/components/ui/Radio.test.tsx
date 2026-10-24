import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Radio, RadioGroup } from "./Radio";

describe("Radio component", () => {
  it("renders with label and description", () => {
    render(
      <Radio
        name="test-radio"
        value="option1"
        label="Option 1"
        description="First available option"
      />
    );

    expect(screen.getByLabelText(/Option 1/)).toBeInTheDocument();
    expect(screen.getByText("First available option")).toBeInTheDocument();
  });

  it("handles standalone radio click", () => {
    const onChange = vi.fn();
    render(
      <Radio
        name="test-radio"
        value="option1"
        label="Option 1"
        onChange={onChange}
      />
    );

    const radio = screen.getByLabelText("Option 1");
    fireEvent.click(radio);
    expect(onChange).toHaveBeenCalled();
  });

  it("renders error state", () => {
    render(
      <Radio
        name="test-radio"
        value="option1"
        label="Option 1"
        error="Selection is required"
      />
    );

    expect(screen.getByRole("alert")).toHaveTextContent("Selection is required");
  });

  it("disables interaction when disabled prop is true", () => {
    const onChange = vi.fn();
    render(
      <Radio
        name="test-radio"
        value="option1"
        label="Disabled Option"
        disabled
        onChange={onChange}
      />
    );

    const radio = screen.getByLabelText("Disabled Option");
    expect(radio).toBeDisabled();
    fireEvent.click(radio);
    expect(onChange).not.toHaveBeenCalled();
  });
});

describe("RadioGroup component", () => {
  it("renders with options array and handles value changes", () => {
    const onChange = vi.fn();
    render(
      <RadioGroup
        name="mode"
        value="generated"
        onChange={onChange}
        options={[
          { value: "generated", label: "Auto Generated" },
          { value: "manual", label: "Manual Entry" },
        ]}
      />
    );

    const autoRadio = screen.getByRole("radio", { name: "Auto Generated" });
    const manualRadio = screen.getByRole("radio", { name: "Manual Entry" });

    expect(autoRadio).toBeChecked();
    expect(manualRadio).not.toBeChecked();

    fireEvent.click(manualRadio);
    expect(onChange).toHaveBeenCalledWith("manual");
  });

  it("renders with Radio children using context", () => {
    const onChange = vi.fn();
    render(
      <RadioGroup
        name="plan"
        value="pro"
        onChange={onChange}
        orientation="horizontal"
      >
        <Radio value="basic" label="Basic Plan" />
        <Radio value="pro" label="Pro Plan" />
      </RadioGroup>
    );

    const basicRadio = screen.getByRole("radio", { name: "Basic Plan" });
    const proRadio = screen.getByRole("radio", { name: "Pro Plan" });

    expect(proRadio).toBeChecked();
    expect(basicRadio).not.toBeChecked();

    fireEvent.click(basicRadio);
    expect(onChange).toHaveBeenCalledWith("basic");
  });

  it("renders group label and error message", () => {
    render(
      <RadioGroup
        name="status"
        label="Choose status"
        error="Status is mandatory"
        options={[
          { value: "active", label: "Active" },
          { value: "inactive", label: "Inactive" },
        ]}
      />
    );

    expect(screen.getByText("Choose status")).toBeInTheDocument();
    expect(screen.getByRole("alert")).toHaveTextContent("Status is mandatory");
  });

  it("propagates disabled prop to all radio options", () => {
    render(
      <RadioGroup
        name="delivery"
        disabled
        options={[
          { value: "standard", label: "Standard" },
          { value: "express", label: "Express" },
        ]}
      />
    );

    expect(screen.getByRole("radio", { name: "Standard" })).toBeDisabled();
    expect(screen.getByRole("radio", { name: "Express" })).toBeDisabled();
  });
});

