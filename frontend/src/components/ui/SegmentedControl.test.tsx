import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { SegmentedControl } from "./SegmentedControl";

describe("SegmentedControl", () => {
  const options = [
    { value: "all", label: "ทั้งหมด" },
    { value: "cost", label: "ต้นทุน" },
    { value: "sell", label: "ราคาขาย" },
  ] as const;

  it("renders all options with proper labels", () => {
    render(
      <SegmentedControl
        value="all"
        onChange={vi.fn()}
        options={options}
      />
    );

    expect(screen.getByRole("button", { name: "ทั้งหมด" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "ต้นทุน" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "ราคาขาย" })).toBeInTheDocument();
  });

  it("calls onChange with correct value when an option is clicked", () => {
    const handleChange = vi.fn();
    render(
      <SegmentedControl
        value="all"
        onChange={handleChange}
        options={options}
      />
    );

    fireEvent.click(screen.getByRole("button", { name: "ต้นทุน" }));
    expect(handleChange).toHaveBeenCalledWith("cost");
  });

  it("applies active styles to the selected option", () => {
    render(
      <SegmentedControl
        value="cost"
        onChange={vi.fn()}
        options={options}
      />
    );

    const costButton = screen.getByRole("button", { name: "ต้นทุน" });
    const allButton = screen.getByRole("button", { name: "ทั้งหมด" });

    expect(costButton.className).toContain("bg-erp-navy text-white");
    expect(allButton.className).not.toContain("bg-erp-navy text-white");
  });

  it("disables buttons when disabled prop is true", () => {
    render(
      <SegmentedControl
        value="all"
        onChange={vi.fn()}
        options={options}
        disabled
      />
    );

    const buttons = screen.getAllByRole("button");
    buttons.forEach((btn) => {
      expect(btn).toBeDisabled();
    });
  });
});
