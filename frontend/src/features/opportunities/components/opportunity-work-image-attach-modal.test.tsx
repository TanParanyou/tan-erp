import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityWorkImageAttachModal } from "./opportunity-work-image-attach-modal";

vi.mock("../api/opportunity-queries", () => ({
  useAttachWorkImages: vi.fn(() => ({
    mutateAsync: vi.fn(),
    isPending: false,
  })),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn().mockResolvedValue("test-token"),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "mem-123" },
  }),
}));

vi.mock("@/lib/i18n/i18n-context", () => ({
  useSafeLocale: () => "th",
}));

describe("OpportunityWorkImageAttachModal component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  function renderModal(props = {}) {
    const queryClient = new QueryClient({
      defaultOptions: { queries: { retry: false } },
    });

    const defaultProps = {
      isOpen: true,
      onClose: vi.fn(),
      opportunityId: "opp-123",
      expectedVersion: "ver-1",
      ...props,
    };

    return render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <QueryClientProvider client={queryClient}>
          <OpportunityWorkImageAttachModal {...defaultProps} />
        </QueryClientProvider>
      </NextIntlClientProvider>
    );
  }

  it("renders attach modal when isOpen is true with camera button", () => {
    renderModal();

    expect(
      screen.getByText("แนบภาพถ่ายหน้างาน (Attach Work Images)")
    ).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /ถ่ายภาพด้วยกล้อง/i })
    ).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /เลือกไฟล์ภาพจากเครื่อง/i })
    ).toBeInTheDocument();
  });

  it("calls onClose when cancel button is clicked", () => {
    const onClose = vi.fn();
    renderModal({ onClose });

    const cancelBtn = screen.getByRole("button", { name: "ยกเลิก" });
    fireEvent.click(cancelBtn);

    expect(onClose).toHaveBeenCalled();
  });

  it("has submit button disabled when no images are selected", () => {
    renderModal();

    const submitBtn = screen.getByRole("button", { name: /แนบภาพถ่ายหน้างาน \(0\)/i });
    expect(submitBtn).toBeDisabled();
  });
});
