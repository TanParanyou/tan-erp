import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { useState } from "react";
import { Modal } from "./Modal";

describe("Modal component", () => {
  it("renders when isOpen is true", () => {
    render(
      <Modal isOpen={true} onClose={vi.fn()} title="Modal Title" description="Modal Desc">
        <p>Modal Content</p>
      </Modal>
    );

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("Modal Title")).toBeInTheDocument();
    expect(screen.getByText("Modal Desc")).toBeInTheDocument();
    expect(screen.getByText("Modal Content")).toBeInTheDocument();
  });

  it("does not render when isOpen is false", () => {
    render(
      <Modal isOpen={false} onClose={vi.fn()} title="Modal Title">
        <p>Modal Content</p>
      </Modal>
    );

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
  });

  it("calls onClose on close button click", () => {
    const onClose = vi.fn();
    render(
      <Modal isOpen={true} onClose={onClose} title="Modal Title">
        <p>Modal Content</p>
      </Modal>
    );

    const closeBtn = screen.getByLabelText("Close modal");
    fireEvent.click(closeBtn);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("keeps focus in the active input when an inline onClose callback changes", async () => {
    function ControlledModal() {
      const [value, setValue] = useState("");
      return (
        <Modal isOpen onClose={() => undefined} title="Reference">
          <input aria-label="First input" />
          <input aria-label="Second input" value={value} onChange={(event) => setValue(event.target.value)} />
        </Modal>
      );
    }

    render(<ControlledModal />);
    const secondInput = screen.getByLabelText("Second input");
    await waitFor(() => expect(screen.getByLabelText("Close modal")).toHaveFocus());

    secondInput.focus();
    expect(secondInput).toHaveFocus();
    fireEvent.change(secondInput, { target: { value: "a" } });
    await new Promise((resolve) => setTimeout(resolve, 10));

    expect(secondInput).toHaveFocus();
  });
});
