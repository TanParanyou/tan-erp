import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { DateTimePicker } from "./DateTimePicker";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";

function renderWithLocale(ui: React.ReactElement) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      {ui}
    </NextIntlClientProvider>
  );
}

describe("DateTimePicker", () => {
  it("renders date and time inputs with label and values", () => {
    renderWithLocale(<DateTimePicker label="วัน-เวลาดำเนินการ" value="2026-09-15T10:30" />);

    expect(screen.getByText(/วัน-เวลาดำเนินการ/)).toBeInTheDocument();
    const dateInput = screen.getByDisplayValue("15/09/2026") as HTMLInputElement;
    const timeInput = screen.getByDisplayValue("10:30") as HTMLInputElement;

    expect(dateInput).toBeInTheDocument();
    expect(timeInput).toBeInTheDocument();
  });

  it("calls onChange with combined string when time changes", () => {
    const handleChange = vi.fn();
    renderWithLocale(<DateTimePicker label="วัน-เวลา" value="2026-09-15T10:30" onChange={handleChange} />);

    const timeInput = screen.getByDisplayValue("10:30");
    fireEvent.change(timeInput, { target: { value: "14:00" } });

    expect(handleChange).toHaveBeenCalledWith("2026-09-15T14:00");
  });

  it("clears value when close button is clicked", () => {
    const handleChange = vi.fn();
    renderWithLocale(<DateTimePicker label="วัน-เวลา" value="2026-09-15T10:30" onChange={handleChange} />);

    const closeButtons = screen.getAllByRole("button", { name: "Close" });
    expect(closeButtons.length).toBeGreaterThan(0);
    fireEvent.click(closeButtons[0]);

    expect(handleChange).toHaveBeenCalledWith("");
  });
});
