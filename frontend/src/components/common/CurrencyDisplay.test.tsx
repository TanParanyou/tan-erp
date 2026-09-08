import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { CurrencyDisplay } from "./CurrencyDisplay";

describe("CurrencyDisplay component", () => {
  it("renders formatted currency in mono font", () => {
    render(<CurrencyDisplay amount={1500} currency="THB" locale="th" />);
    const el = screen.getByText(/1,500\.00/);
    expect(el).toBeInTheDocument();
    expect(el).toHaveClass("font-mono");
  });
});
