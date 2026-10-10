import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { ApiError } from "@/lib/api/api-error";
import type { AttachmentLinkResponse } from "@/lib/api/api-client";
import { AttachmentList } from "./AttachmentList";

const mocks = vi.hoisted(() => ({
  uploadFiles: vi.fn(),
  attach: vi.fn(),
  unlink: vi.fn(),
  links: { data: undefined as { items: AttachmentLinkResponse[] } | undefined, isLoading: false, isError: false },
  toast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("@/hooks/useDeferredFileUpload", () => ({
  useDeferredFileUpload: () => ({ uploadFiles: mocks.uploadFiles, isUploading: false }),
}));
vi.mock("@/hooks/useAttachments", () => ({
  useAttachmentLinks: () => mocks.links,
  useAttachmentMutations: () => ({
    attach: { mutateAsync: mocks.attach, isPending: false },
    unlink: { mutateAsync: mocks.unlink, isPending: false },
  }),
}));
vi.mock("@/lib/api/use-api-request-context", () => ({
  useApiRequestContext: () => ({
    membershipId: "m-1",
    locale: "th",
    buildOptions: async () => ({ token: "tok", membershipId: "m-1", locale: "th" }),
  }),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: mocks.toast }) }));
vi.mock("@/components/common/AuthenticatedFileImage", () => ({
  AuthenticatedFileImage: ({ fileId, alt }: { fileId: string; alt: string }) => <img src={`blob:${fileId}`} alt={alt} />,
}));

const t = thMessages.attachments;

function link(id: string, filename: string): AttachmentLinkResponse {
  return {
    id,
    ownerType: "installation-job",
    ownerId: "job-1",
    fileId: `file-${id}`,
    purpose: "evidence",
    filename,
    mediaType: "image/jpeg",
    fileSizeBytes: 1024,
    servingUrl: `/api/v1/files/file-${id}/content`,
    createdBy: { id: "u-1", displayName: "ช่างหนึ่ง" },
    createdAtUtc: "2026-10-05T03:00:00Z",
  };
}

function renderList(props: Partial<React.ComponentProps<typeof AttachmentList>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <AttachmentList ownerType="installation-job" ownerId="job-1" canManage purposes={["evidence", "defect"]} {...props} />
    </NextIntlClientProvider>
  );
}

function pick(files: File[]) {
  fireEvent.change(screen.getByLabelText(t.selectFiles), { target: { files } });
}

const jpeg = (name: string) => new File(["x"], name, { type: "image/jpeg" });

describe("AttachmentList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.links.data = { items: [] };
    mocks.links.isLoading = false;
    mocks.links.isError = false;
    mocks.uploadFiles.mockResolvedValue({ uploadIntentId: null, files: [{ fileId: "file-new", filename: "a.jpg", mediaType: "image/jpeg", fileSizeBytes: 1, servingUrl: "" }] });
    mocks.attach.mockResolvedValue({ items: [] });
    mocks.unlink.mockResolvedValue(undefined);
  });

  it("lists attachments with filename, purpose label and author", () => {
    mocks.links.data = { items: [link("l-1", "ห้องนอน.jpg")] };
    renderList();

    expect(screen.getByText("ห้องนอน.jpg")).toBeInTheDocument();
    expect(within(screen.getByRole("list")).getByText(t.purposes.evidence)).toBeInTheDocument();
    expect(screen.getByText(t.createdBy.replace("{name}", "ช่างหนึ่ง"))).toBeInTheDocument();
  });

  it("shows the empty state and hides every write control without manage permission", () => {
    renderList({ canManage: false });

    expect(screen.getByText(t.empty)).toBeInTheDocument();
    expect(screen.queryByLabelText(t.selectFiles)).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: t.save })).not.toBeInTheDocument();
  });

  it("does not upload when files are only selected", () => {
    renderList();
    pick([jpeg("a.jpg")]);

    expect(screen.getByText("a.jpg")).toBeInTheDocument();
    expect(mocks.uploadFiles).not.toHaveBeenCalled();
    expect(mocks.attach).not.toHaveBeenCalled();
  });

  it("uploads on submit and then attaches the uploaded file ids with the chosen purpose", async () => {
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));

    await waitFor(() => expect(mocks.attach).toHaveBeenCalledTimes(1));
    expect(mocks.uploadFiles).toHaveBeenCalledTimes(1);
    expect(mocks.uploadFiles.mock.calls[0][0]).toHaveLength(1);
    expect(mocks.uploadFiles.mock.invocationCallOrder[0]).toBeLessThan(mocks.attach.mock.invocationCallOrder[0]);
    const call = mocks.attach.mock.calls[0][0] as { payload: { purpose: string; fileIds: string[] }; idempotencyKey: string };
    expect(call.payload).toEqual({ purpose: "evidence", fileIds: ["file-new"] });
    expect(call.idempotencyKey).not.toBe("");
    expect(mocks.toast.success).toHaveBeenCalledWith(t.saved);
  });

  it("keeps the uploaded file for a retry when attaching fails, without uploading it again", async () => {
    mocks.attach.mockRejectedValueOnce(new ApiError({ status: 409, code: "ATTACHMENT_DUPLICATE", message: "dup" }));
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));

    expect(await screen.findByText(t.errors.ATTACHMENT_DUPLICATE)).toBeInTheDocument();
    expect(mocks.uploadFiles).toHaveBeenCalledTimes(1);

    fireEvent.click(screen.getByRole("button", { name: t.save }));
    await waitFor(() => expect(mocks.attach).toHaveBeenCalledTimes(2));
    expect(mocks.uploadFiles).toHaveBeenCalledTimes(1);
  });

  it("reuses the same idempotency key when the user retries after a failed attach, then rotates it after success", async () => {
    mocks.attach.mockRejectedValueOnce(new ApiError({ status: 500, code: "INTERNAL_ERROR", message: "boom" }));
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));
    await screen.findByText(t.errors.failed);

    fireEvent.click(screen.getByRole("button", { name: t.save }));
    await waitFor(() => expect(mocks.attach).toHaveBeenCalledTimes(2));
    const keys = mocks.attach.mock.calls.map((call) => (call[0] as { idempotencyKey: string }).idempotencyKey);
    expect(keys[0]).toBe(keys[1]);

    await waitFor(() => expect(mocks.toast.success).toHaveBeenCalledWith(t.saved));
    pick([jpeg("b.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));
    await waitFor(() => expect(mocks.attach).toHaveBeenCalledTimes(3));
    const third = (mocks.attach.mock.calls[2][0] as { idempotencyKey: string }).idempotencyKey;
    expect(third).not.toBe(keys[0]);
  });

  it("shows an unknown failure with the generic message and keeps the pending files when the upload itself fails", async () => {
    mocks.uploadFiles.mockRejectedValueOnce(new Error("network"));
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));

    expect(await screen.findByText(t.errors.failed)).toBeInTheDocument();
    expect(mocks.attach).not.toHaveBeenCalled();
    expect(screen.getByText("a.jpg")).toBeInTheDocument();
  });

  it("rejects files that are not accepted images before anything is sent", () => {
    renderList();
    pick([new File(["x"], "notes.pdf", { type: "application/pdf" })]);

    expect(screen.getByText(t.fileRejected.replace("{name}", "notes.pdf"))).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: t.save })).not.toBeInTheDocument();
  });

  it("asks for confirmation before unlinking and only unlinks after confirming", async () => {
    mocks.links.data = { items: [link("l-1", "ห้องนอน.jpg")] };
    renderList();

    fireEvent.click(screen.getByRole("button", { name: t.unlink.action }));
    expect(screen.getByText(t.unlink.message)).toBeInTheDocument();
    expect(mocks.unlink).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.delete }));
    await waitFor(() => expect(mocks.unlink).toHaveBeenCalledWith("l-1"));
    expect(mocks.toast.success).toHaveBeenCalledWith(t.unlinked);
  });
});
