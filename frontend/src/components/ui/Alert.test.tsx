import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { Alert } from "./Alert";

describe("Alert", () => {
  it("renders with default danger variant and message", () => {
    render(<Alert>Operation failed</Alert>);

    const alert = screen.getByRole("alert");
    expect(alert).toBeInTheDocument();
    expect(alert).toHaveTextContent("Operation failed");
    expect(alert).toHaveClass("border-erp-danger-border");
  });

  it("renders title when provided", () => {
    render(<Alert title="Error occurred">Please try again</Alert>);

    expect(screen.getByText("Error occurred")).toBeInTheDocument();
    expect(screen.getByText("Please try again")).toBeInTheDocument();
  });

  it("handles dismiss callback when onClose is provided", () => {
    const handleClose = vi.fn();
    render(<Alert onClose={handleClose}>Dismissable message</Alert>);

    const dismissBtn = screen.getByRole("button", { name: /dismiss alert/i });
    expect(dismissBtn).toBeInTheDocument();
    fireEvent.click(dismissBtn);
    expect(handleClose).toHaveBeenCalledTimes(1);
  });

  it("renders different variants correctly", () => {
    const { rerender } = render(<Alert variant="warning">Warning alert</Alert>);
    expect(screen.getByRole("alert")).toHaveClass("border-erp-warning-border");

    rerender(<Alert variant="success">Success alert</Alert>);
    expect(screen.getByRole("alert")).toHaveClass("border-erp-success-border");

    rerender(<Alert variant="info">Info alert</Alert>);
    expect(screen.getByRole("alert")).toHaveClass("border-erp-border");
  });
});
