import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityWorkImageAttachModal } from "./opportunity-work-image-attach-modal";

import { fileClient } from "@/lib/api/file-client";
import { useAttachWorkImages } from "../api/opportunity-queries";
import { ApiError } from "@/lib/api/api-error";

vi.mock("../api/opportunity-queries", () => ({
  useAttachWorkImages: vi.fn(() => ({
    mutateAsync: vi.fn(),
    isPending: false,
  })),
}));

vi.mock("@/lib/api/file-client", () => ({
  fileClient: {
    createSession: vi.fn(),
    completeSession: vi.fn(),
    getFileUrl: vi.fn(),
  },
}));

vi.mock("@/components/forms/MultiImagePicker", () => ({
  MultiImagePicker: ({ onChange }: { onChange: (items: unknown[]) => void }) => (
    <div>
      <button type="button">ถ่ายภาพด้วยกล้อง</button>
      <button type="button">เลือกไฟล์ภาพจากเครื่อง</button>
      <button
        type="button"
        data-testid="add-dummy-images"
        onClick={() =>
          onChange([
            {
              id: "img-1",
              originalFile: new File(["dummy1"], "photo1.webp", { type: "image/webp" }),
              caption: "Photo 1",
              isOptimizing: false,
            },
            {
              id: "img-2",
              originalFile: new File(["dummy2"], "photo2.webp", { type: "image/webp" }),
              caption: "Photo 2",
              isOptimizing: false,
            },
          ])
        }
      >
        Add Dummy Images
      </button>
    </div>
  ),
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

  it("reuses verified fileId on retry without re-uploading completed files", async () => {
    const mockMutateAsync = vi
      .fn()
      .mockRejectedValueOnce(new ApiError({
        status: 409,
        code: "OPPORTUNITY_VERSION_CONFLICT",
        message: "Version conflict on first attempt",
      }))
      .mockResolvedValueOnce({
        items: [],
        opportunityRowVersion: "ver-2",
      });

    vi.mocked(useAttachWorkImages).mockReturnValue({
      mutateAsync: mockMutateAsync,
      isPending: false,
    } as unknown as ReturnType<typeof useAttachWorkImages>);

    vi.mocked(fileClient.createSession).mockResolvedValue({
      sessionId: "sess-1",
      expiresAtUtc: "2026-09-22T00:00:00Z",
      slots: [
        { slotId: "slot-1", filename: "photo1.webp", mediaType: "image/webp", fileSizeBytes: 100 },
        { slotId: "slot-2", filename: "photo2.webp", mediaType: "image/webp", fileSizeBytes: 100 },
      ],
    });

    vi.mocked(fileClient.completeSession).mockResolvedValue({
      sessionId: "sess-1",
      files: [
        { fileId: "file-verified-1", filename: "photo1.webp", mediaType: "image/webp", fileSizeBytes: 100, servingUrl: "" },
        { fileId: "file-verified-2", filename: "photo2.webp", mediaType: "image/webp", fileSizeBytes: 100, servingUrl: "" },
      ],
    });

    renderModal();

    // 1. Add images
    fireEvent.click(screen.getByTestId("add-dummy-images"));

    // 2. First submit -> upload session created, but attach fails
    const submitBtn = screen.getByRole("button", { name: /แนบภาพถ่ายหน้างาน \(2\)/i });
    fireEvent.click(submitBtn);

    expect(await screen.findByText("Version conflict on first attempt")).toBeInTheDocument();

    expect(fileClient.createSession).toHaveBeenCalledTimes(1);
    expect(fileClient.completeSession).toHaveBeenCalledTimes(1);
    expect(mockMutateAsync).toHaveBeenCalledTimes(1);

    // 3. Second submit (Retry) -> does NOT call createSession or completeSession again!
    fireEvent.click(submitBtn);

    await vi.waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledTimes(2);
    });

    // Sessions are NOT created again on retry because fileIds are reused
    expect(fileClient.createSession).toHaveBeenCalledTimes(1);
    expect(fileClient.completeSession).toHaveBeenCalledTimes(1);

    // Both verified file IDs are attached
    expect(mockMutateAsync).toHaveBeenLastCalledWith(
      expect.objectContaining({
        images: [
          { fileId: "file-verified-1", caption: "Photo 1" },
          { fileId: "file-verified-2", caption: "Photo 2" },
        ],
      })
    );

    const firstAttachIntent = mockMutateAsync.mock.calls[0][0].idempotencyKey;
    const retriedAttachIntent = mockMutateAsync.mock.calls[1][0].idempotencyKey;
    expect(retriedAttachIntent).toBe(firstAttachIntent);
  });
});
