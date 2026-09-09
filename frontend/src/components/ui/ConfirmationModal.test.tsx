import React, { useState } from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { ConfirmationModal } from "./ConfirmationModal";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";

function renderWithIntl(ui: React.ReactNode) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      {ui}
    </NextIntlClientProvider>
  );
}

describe("ConfirmationModal component", () => {
  it("renders with message, accessible dialog attributes, and confirms action", () => {
    const onConfirm = vi.fn();
    const onClose = vi.fn();

    renderWithIntl(
      <ConfirmationModal
        isOpen={true}
        onClose={onClose}
        onConfirm={onConfirm}
        title="Delete Customer"
        message="Are you sure you want to delete this customer?"
      />
    );

    const dialog = screen.getByRole("dialog");
    expect(dialog).toBeInTheDocument();
    expect(dialog).toHaveAttribute("aria-modal", "true");
    expect(screen.getByText("Delete Customer")).toBeInTheDocument();
    expect(screen.getByText("Are you sure you want to delete this customer?")).toBeInTheDocument();

    const deleteBtn = screen.getByRole("button", { name: "ลบ" });
    fireEvent.click(deleteBtn);
    expect(onConfirm).toHaveBeenCalledTimes(1);

    const cancelBtn = screen.getByRole("button", { name: "ยกเลิก" });
    fireEvent.click(cancelBtn);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("closes on Escape key only when idle, not when loading", () => {
    const onClose = vi.fn();
    const onConfirm = vi.fn();

    const { rerender } = renderWithIntl(
      <ConfirmationModal
        isOpen={true}
        onClose={onClose}
        onConfirm={onConfirm}
        title="Confirm Title"
        message="Confirm Message"
        isLoading={false}
      />
    );

    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).toHaveBeenCalledTimes(1);

    onClose.mockClear();

    rerender(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ConfirmationModal
          isOpen={true}
          onClose={onClose}
          onConfirm={onConfirm}
          title="Confirm Title"
          message="Confirm Message"
          isLoading={true}
        />
      </NextIntlClientProvider>
    );

    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).not.toHaveBeenCalled();
  });

  it("disables both confirm and cancel buttons during loading", () => {
    const onConfirm = vi.fn();
    const onClose = vi.fn();

    renderWithIntl(
      <ConfirmationModal
        isOpen={true}
        onClose={onClose}
        onConfirm={onConfirm}
        title="Confirm Title"
        message="Confirm Message"
        isLoading={true}
      />
    );

    const buttons = screen.getAllByRole("button");
    buttons.forEach((btn) => {
      expect(btn).toBeDisabled();
    });
  });

  it("manages focus: autofocuses or puts focus in modal when opened and returns to trigger when closed", async () => {
    function ModalWithTrigger() {
      const [open, setOpen] = useState(false);
      return (
        <div>
          <button id="open-btn" onClick={() => setOpen(true)}>
            Open
          </button>
          <ConfirmationModal
            isOpen={open}
            onClose={() => setOpen(false)}
            onConfirm={() => setOpen(false)}
            title="Focus Test"
            message="Focus Test Message"
          />
        </div>
      );
    }

    renderWithIntl(<ModalWithTrigger />);

    const openBtn = screen.getByRole("button", { name: "Open" });
    openBtn.focus();
    expect(document.activeElement).toBe(openBtn);

    fireEvent.click(openBtn);

    await waitFor(() => {
      const dialog = screen.getByRole("dialog");
      expect(dialog).toBeInTheDocument();
      expect(dialog.contains(document.activeElement)).toBe(true);
    });

    const cancelBtn = screen.getByRole("button", { name: "ยกเลิก" });
    fireEvent.click(cancelBtn);

    // After modal closes, focus should return to the trigger element
    await waitFor(() => {
      expect(document.activeElement).toBe(openBtn);
    });
  });
});
