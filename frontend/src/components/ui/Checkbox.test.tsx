import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Checkbox } from "./Checkbox";

describe("Checkbox component", () => {
  it("renders with label and description", () => {
    render(
      <Checkbox
        label="Accept terms"
        description="Must be agreed to proceed"
      />
    );
    expect(screen.getByLabelText(/Accept terms/)).toBeInTheDocument();
    expect(screen.getByText("Must be agreed to proceed")).toBeInTheDocument();
  });

  it("handles checked changes", () => {
    const onChange = vi.fn();
    render(<Checkbox label="Subscribe" onChange={onChange} />);

    const checkbox = screen.getByLabelText("Subscribe");
    fireEvent.click(checkbox);
    expect(onChange).toHaveBeenCalled();
  });

  it("renders error state", () => {
    render(<Checkbox label="Accept terms" error="You must accept" />);
    expect(screen.getByRole("alert")).toHaveTextContent("You must accept");
  });
});
