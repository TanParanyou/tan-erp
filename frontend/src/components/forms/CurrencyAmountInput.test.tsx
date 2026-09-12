import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { CurrencyAmountInput } from "./CurrencyAmountInput";

describe("CurrencyAmountInput", () => {
  it("renders with label and inputs correctly", () => {
    const onAmountChange = vi.fn();
    const onCurrencyChange = vi.fn();

    render(
      <CurrencyAmountInput
        label="งบประมาณที่คาดหวัง"
        required
        amountValue={50000}
        currencyValue="THB"
        onAmountChange={onAmountChange}
        onCurrencyChange={onCurrencyChange}
      />
    );

    expect(screen.getByText("งบประมาณที่คาดหวัง")).toBeInTheDocument();
    expect(screen.getByText("*")).toBeInTheDocument();

    const amountInput = screen.getByRole("spinbutton");
    expect(amountInput).toHaveValue(50000);

    const currencySelect = screen.getByRole("combobox");
    expect(currencySelect).toHaveValue("THB");
  });

  it("calls onAmountChange when typing valid numbers", () => {
    const onAmountChange = vi.fn();
    const onCurrencyChange = vi.fn();

    const { rerender } = render(
      <CurrencyAmountInput
        amountValue={undefined}
        currencyValue="THB"
        onAmountChange={onAmountChange}
        onCurrencyChange={onCurrencyChange}
      />
    );

    const amountInput = screen.getByRole("spinbutton");
    fireEvent.change(amountInput, { target: { value: "123456" } });
    expect(onAmountChange).toHaveBeenCalledWith(123456);

    rerender(
      <CurrencyAmountInput
        amountValue={123456}
        currencyValue="THB"
        onAmountChange={onAmountChange}
        onCurrencyChange={onCurrencyChange}
      />
    );

    fireEvent.change(amountInput, { target: { value: "" } });
    expect(onAmountChange).toHaveBeenCalledWith(undefined);
  });

  it("calls onCurrencyChange when selecting a different currency", () => {
    const onAmountChange = vi.fn();
    const onCurrencyChange = vi.fn();

    render(
      <CurrencyAmountInput
        amountValue={1000}
        currencyValue="THB"
        onAmountChange={onAmountChange}
        onCurrencyChange={onCurrencyChange}
      />
    );

    const currencySelect = screen.getByRole("combobox");
    fireEvent.change(currencySelect, { target: { value: "USD" } });
    expect(onCurrencyChange).toHaveBeenCalledWith("USD");
  });

  it("displays amountError and applies error styling", () => {
    render(
      <CurrencyAmountInput
        amountValue={-50}
        currencyValue="THB"
        amountError="งบประมาณไม่สามารถติดลบได้"
        onAmountChange={vi.fn()}
        onCurrencyChange={vi.fn()}
      />
    );

    expect(screen.getByRole("alert")).toHaveTextContent("งบประมาณไม่สามารถติดลบได้");
    const amountInput = screen.getByRole("spinbutton");
    expect(amountInput).toHaveClass("erp-input-error");
  });

  it("prevents typing negative sign or exponent characters", () => {
    render(
      <CurrencyAmountInput
        amountValue={undefined}
        currencyValue="THB"
        onAmountChange={vi.fn()}
        onCurrencyChange={vi.fn()}
      />
    );

    const amountInput = screen.getByRole("spinbutton");
    const minusEvent = fireEvent.keyDown(amountInput, { key: "-" });
    expect(minusEvent).toBe(false);

    const expEvent = fireEvent.keyDown(amountInput, { key: "e" });
    expect(expEvent).toBe(false);
  });
});
