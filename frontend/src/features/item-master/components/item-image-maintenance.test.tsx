import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import type { ItemResponse } from "@/lib/api/api-client";
import { ItemImageMaintenance } from "./item-image-maintenance";
import { useItemImageMutations, useItemImages } from "@/features/item-master/api/item-master-queries";

const fileMocks = vi.hoisted(() => ({ createSession: vi.fn(), completeSession: vi.fn() }));
vi.mock("@/features/item-master/api/item-master-queries", () => ({ useItemImageMutations: vi.fn(), useItemImages: vi.fn() }));
vi.mock("@/lib/membership/selected-membership-context", () => ({ useSelectedMembership: () => ({ selectedMembership: { id: "membership-1", permissions: ["items.manage-images"] } }) }));
vi.mock("@/lib/permissions/can", () => ({ can: () => true }));
vi.mock("@/lib/auth/auth-session", () => ({ getAuthToken: vi.fn().mockResolvedValue("token") }));
vi.mock("@/lib/api/file-client", () => ({ fileClient: fileMocks }));

function renderMaintenance() {
  const item = { id: "item-1", name: { thai: "สินค้า", english: "Product" } } as unknown as ItemResponse;
  return render(<NextIntlClientProvider locale="th" messages={messages}><ItemImageMaintenance itemId="item-1" item={item} /></NextIntlClientProvider>);
}

describe("ItemImageMaintenance", () => {
  const attach = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useItemImages).mockReturnValue({ data: [], isLoading: false, isError: false } as unknown as ReturnType<typeof useItemImages>);
    attach.mockResolvedValue({ id: "image-1" });
    fileMocks.createSession.mockResolvedValue({ sessionId: "session-1", slots: [{ slotId: "slot-1" }] });
    fileMocks.completeSession.mockResolvedValue({ files: [{ fileId: "verified-file-1" }] });
    vi.mocked(useItemImageMutations).mockReturnValue({
      attach: { mutateAsync: attach, isPending: false, isError: false },
      setPrimary: { mutateAsync: vi.fn(), isPending: false },
      detach: { mutateAsync: vi.fn(), isPending: false },
    } as unknown as ReturnType<typeof useItemImageMutations>);
  });

  it("defers upload until submit, then attaches only the verified file", async () => {
    renderMaintenance();
    const image = new File(["image bytes"], "product.png", { type: "image/png" });
    const files = { 0: image, length: 1, item: (index: number) => index === 0 ? image : null };
    fireEvent.change(screen.getByLabelText("ไฟล์รูปภาพ"), { target: { files } });

    expect(fileMocks.createSession).not.toHaveBeenCalled();
    expect(fileMocks.completeSession).not.toHaveBeenCalled();
    expect(attach).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: "อัปโหลดและบันทึกรูปภาพ" }));
    await waitFor(() => expect(attach).toHaveBeenCalledOnce());

    expect(fileMocks.createSession).toHaveBeenCalledWith(expect.objectContaining({
      parentType: "item",
      parentId: "item-1",
      files: [expect.objectContaining({ filename: "product.png", mediaType: "image/png" })],
    }), expect.objectContaining({ membershipId: "membership-1" }));
    expect(fileMocks.createSession.mock.invocationCallOrder[0]).toBeLessThan(fileMocks.completeSession.mock.invocationCallOrder[0] ?? Number.MAX_SAFE_INTEGER);
    expect(fileMocks.completeSession.mock.invocationCallOrder[0]).toBeLessThan(attach.mock.invocationCallOrder[0] ?? Number.MAX_SAFE_INTEGER);
    expect(attach).toHaveBeenCalledWith(expect.objectContaining({ fileId: "verified-file-1", role: "gallery", isPrimary: true }));
  });
});
