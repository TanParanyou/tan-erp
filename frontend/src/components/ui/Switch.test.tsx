import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Switch } from "./Switch";

describe("Switch component", () => {
  it("renders with label and description", () => {
    render(<Switch label="Enable Notifications" description="Receive alert emails" />);
    expect(screen.getByLabelText(/Enable Notifications/)).toBeInTheDocument();
    expect(screen.getByText("Receive alert emails")).toBeInTheDocument();
  });

  it("handles toggling", () => {
    const onChange = vi.fn();
    render(<Switch label="Dark mode" onChange={onChange} />);

    const toggle = screen.getByLabelText("Dark mode");
    fireEvent.click(toggle);
    expect(onChange).toHaveBeenCalled();
  });

  it("renders error message", () => {
    render(<Switch label="Agree" error="Required option" />);
    expect(screen.getByRole("alert")).toHaveTextContent("Required option");
  });
});
