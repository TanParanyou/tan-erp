import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { MeasurementAmountInput } from "./MeasurementAmountInput";

describe("MeasurementAmountInput", () => {
  it("renders with label, required asterisk, and initial value/unit", () => {
    const onChange = vi.fn();
    const onUnitChange = vi.fn();

    render(
      <MeasurementAmountInput
        label="ระยะและหน่วยวัด"
        required
        value={2.4}
        unitCode="m"
        onChange={onChange}
        onUnitChange={onUnitChange}
      />
    );

    expect(screen.getByText("ระยะและหน่วยวัด")).toBeInTheDocument();
    expect(screen.getByText("*")).toBeInTheDocument();

    const amountInput = screen.getByRole("spinbutton");
    expect(amountInput).toHaveValue(2.4);

    const unitSelect = screen.getByRole("combobox");
    expect(unitSelect).toHaveValue("m");

    // Live mm conversion should be displayed
    expect(screen.getByText(/=\s*2,400\s*mm/)).toBeInTheDocument();
  });

  it("calls onChange when typing valid numbers and when clearing input", () => {
    const onChange = vi.fn();
    const onUnitChange = vi.fn();

    const { rerender } = render(
      <MeasurementAmountInput
        value=""
        unitCode="m"
        onChange={onChange}
        onUnitChange={onUnitChange}
      />
    );

    const amountInput = screen.getByRole("spinbutton");
    fireEvent.change(amountInput, { target: { value: "3.5" } });
    expect(onChange).toHaveBeenCalledWith(3.5);

    rerender(
      <MeasurementAmountInput
        value={3.5}
        unitCode="m"
        onChange={onChange}
        onUnitChange={onUnitChange}
      />
    );

    fireEvent.change(amountInput, { target: { value: "" } });
    expect(onChange).toHaveBeenCalledWith(undefined);
  });

  it("calls onUnitChange when selecting a different unit", () => {
    const onChange = vi.fn();
    const onUnitChange = vi.fn();

    render(
      <MeasurementAmountInput
        value={150}
        unitCode="cm"
        onChange={onChange}
        onUnitChange={onUnitChange}
      />
    );

    const unitSelect = screen.getByRole("combobox");
    fireEvent.change(unitSelect, { target: { value: "m" } });
    expect(onUnitChange).toHaveBeenCalledWith("m");
  });

  it("does not show live mm conversion when unit is already mm", () => {
    render(
      <MeasurementAmountInput
        value={1500}
        unitCode="mm"
        onChange={vi.fn()}
        onUnitChange={vi.fn()}
      />
    );

    expect(screen.queryByText(/=\s*\d+.*mm/)).not.toBeInTheDocument();
  });

  it("does not show live mm conversion when showMmConversion is false", () => {
    render(
      <MeasurementAmountInput
        value={2.5}
        unitCode="m"
        showMmConversion={false}
        onChange={vi.fn()}
        onUnitChange={vi.fn()}
      />
    );

    expect(screen.queryByText(/=\s*\d+.*mm/)).not.toBeInTheDocument();
  });

  it("displays error message and applies error styling", () => {
    render(
      <MeasurementAmountInput
        value={-5}
        unitCode="m"
        error="ความยาวต้องมากกว่า 0"
        onChange={vi.fn()}
        onUnitChange={vi.fn()}
      />
    );

    expect(screen.getByRole("alert")).toHaveTextContent("ความยาวต้องมากกว่า 0");
    const amountInput = screen.getByRole("spinbutton");
    expect(amountInput).toHaveClass("erp-input-error");
  });

  it("prevents typing negative sign or exponent characters", () => {
    render(
      <MeasurementAmountInput
        value=""
        unitCode="m"
        onChange={vi.fn()}
        onUnitChange={vi.fn()}
      />
    );

    const amountInput = screen.getByRole("spinbutton");
    const minusEvent = fireEvent.keyDown(amountInput, { key: "-" });
    expect(minusEvent).toBe(false);

    const expEvent = fireEvent.keyDown(amountInput, { key: "e" });
    expect(expEvent).toBe(false);
  });

  it("disables both input and select when disabled prop is true", () => {
    render(
      <MeasurementAmountInput
        value={2}
        unitCode="m"
        disabled={true}
        onChange={vi.fn()}
        onUnitChange={vi.fn()}
      />
    );

    const amountInput = screen.getByRole("spinbutton");
    expect(amountInput).toBeDisabled();

    const unitSelect = screen.getByRole("combobox");
    expect(unitSelect).toBeDisabled();
  });
});
