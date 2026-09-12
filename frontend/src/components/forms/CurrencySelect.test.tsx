import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { CurrencySelect, DEFAULT_CURRENCY_OPTIONS } from "./CurrencySelect";

describe("CurrencySelect", () => {
  it("renders default currency options with THB as default", () => {
    const handleChange = vi.fn();
    render(<CurrencySelect value="THB" onChange={handleChange} label="สกุลเงิน" />);

    const select = screen.getByLabelText("สกุลเงิน") as HTMLSelectElement;
    expect(select.value).toBe("THB");
    expect(screen.getByText(/THB - บาทไทย/)).toBeInTheDocument();
    expect(screen.getByText(/USD - ดอลลาร์สหรัฐ/)).toBeInTheDocument();
  });

  it("handles value change properly", () => {
    const handleChange = vi.fn();
    render(<CurrencySelect value="THB" onChange={handleChange} label="สกุลเงิน" />);

    const select = screen.getByLabelText("สกุลเงิน");
    fireEvent.change(select, { target: { value: "USD" } });

    expect(handleChange).toHaveBeenCalledTimes(1);
  });
});
