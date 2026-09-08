import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Select } from "./Select";

describe("Select component", () => {
  const options = [
    { value: "1", label: "Option 1" },
    { value: "2", label: "Option 2" },
  ];

  it("renders with label and options", () => {
    render(<Select label="Status" options={options} placeholder="Select status" />);
    expect(screen.getByLabelText(/Status/)).toBeInTheDocument();
    expect(screen.getByText("Select status")).toBeInTheDocument();
    expect(screen.getByText("Option 1")).toBeInTheDocument();
  });

  it("displays error message and helper text", () => {
    const { rerender } = render(
      <Select label="Status" options={options} helperText="Help text" />
    );
    expect(screen.getByText("Help text")).toBeInTheDocument();

    rerender(
      <Select label="Status" options={options} error="Field is required" />
    );
    expect(screen.getByRole("alert")).toHaveTextContent("Field is required");
  });

  it("calls onChange when user selects an option", () => {
    const onChange = vi.fn();
    render(<Select label="Status" options={options} onChange={onChange} />);

    fireEvent.change(screen.getByRole("combobox"), { target: { value: "2" } });
    expect(onChange).toHaveBeenCalled();
  });
});
