import React from "react";
import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import { DuplicateConfirmationModal } from "./duplicate-confirmation-modal";

describe("DuplicateConfirmationModal", () => {
  it("locks dismissal and candidate actions while creating a new customer", () => {
    const onClose = vi.fn();
    const onConfirm = vi.fn();
    const onViewCandidate = vi.fn();
    const onSelectExisting = vi.fn();

    render(
      <NextIntlClientProvider locale="th" messages={messages}>
        <DuplicateConfirmationModal
          isOpen
          onClose={onClose}
          onConfirm={onConfirm}
          onViewCandidate={onViewCandidate}
          onSelectExisting={onSelectExisting}
          candidates={[{
            id: "candidate-1",
            code: "CUS-001",
            displayNameTh: "ลูกค้าตัวอย่าง",
            maskedPhone: null,
            maskedEmail: null,
          }]}
          isLoading
        />
      </NextIntlClientProvider>,
    );

    expect(screen.getByRole("button", { name: "Close modal" })).toBeDisabled();
    expect(screen.getByRole("button", { name: messages.customers.confirmCreateNewCustomer })).toBeDisabled();
    expect(screen.getByRole("button", { name: messages.customers.viewInDrawer })).toBeDisabled();
    expect(screen.getByRole("button", { name: messages.customers.useExistingCustomer })).toBeDisabled();

    fireEvent.keyDown(document, { key: "Escape" });
    const overlay = document.querySelector(".erp-modal-overlay");
    if (!overlay) throw new Error("Modal overlay not found");
    fireEvent.click(overlay);

    expect(onClose).not.toHaveBeenCalled();
    expect(onConfirm).not.toHaveBeenCalled();
    expect(onViewCandidate).not.toHaveBeenCalled();
    expect(onSelectExisting).not.toHaveBeenCalled();
  });
});
