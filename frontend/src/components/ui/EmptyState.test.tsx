import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { EmptyState } from "./EmptyState";

describe("EmptyState component", () => {
  it("renders title, description and actions", () => {
    const onAction = vi.fn();
    render(
      <EmptyState
        title="No customers found"
        description="Try clearing filters"
        actionLabel="Create Customer"
        onAction={onAction}
      />
    );

    expect(screen.getByText("No customers found")).toBeInTheDocument();
    expect(screen.getByText("Try clearing filters")).toBeInTheDocument();

    const actionButton = screen.getByRole("button", { name: "Create Customer" });
    fireEvent.click(actionButton);
    expect(onAction).toHaveBeenCalledTimes(1);
  });
});
