import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { ConfirmationModal } from "./ConfirmationModal";

describe("ConfirmationModal component", () => {
  it("renders with message and confirms action", () => {
    const onConfirm = vi.fn();
    const onClose = vi.fn();

    render(
      <ConfirmationModal
        isOpen={true}
        onClose={onClose}
        onConfirm={onConfirm}
        title="Delete Customer"
        message="Are you sure you want to delete this customer?"
      />
    );

    expect(screen.getByText("Delete Customer")).toBeInTheDocument();
    expect(screen.getByText("Are you sure you want to delete this customer?")).toBeInTheDocument();

    const deleteBtn = screen.getByRole("button", { name: "ลบ" });
    fireEvent.click(deleteBtn);
    expect(onConfirm).toHaveBeenCalledTimes(1);

    const cancelBtn = screen.getByRole("button", { name: "ยกเลิก" });
    fireEvent.click(cancelBtn);
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});
