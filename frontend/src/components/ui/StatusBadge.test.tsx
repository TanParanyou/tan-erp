import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { StatusBadge } from "./StatusBadge";

describe("StatusBadge component", () => {
  it("renders with given label and automatic variant mapping", () => {
    render(<StatusBadge label="Active" />);
    const badge = screen.getByText("Active");
    expect(badge).toBeInTheDocument();
    expect(badge).toHaveClass("erp-badge-success");
  });

  it("renders with custom variant override", () => {
    render(<StatusBadge label="Custom" variant="danger" />);
    const badge = screen.getByText("Custom");
    expect(badge).toHaveClass("erp-badge-danger");
  });

  it("renders with icon", () => {
    render(
      <StatusBadge
        label="Success"
        icon={<span data-testid="status-icon">✓</span>}
      />
    );
    expect(screen.getByTestId("status-icon")).toBeInTheDocument();
  });
});
