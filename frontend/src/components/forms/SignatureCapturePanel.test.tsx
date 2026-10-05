import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { ApiError } from "@/lib/api/api-error";
import type { SignatureCaptureResponse } from "@/lib/api/api-client";
import { SignatureCapturePanel } from "./SignatureCapturePanel";

const PNG_DATA_URL = "data:image/png;base64,iVBORw0KGgo=";

const mocks = vi.hoisted(() => ({
  uploadSingleFile: vi.fn(),
  capture: vi.fn(),
  signatures: { data: undefined as { items: SignatureCaptureResponse[] } | undefined, isLoading: false, isError: false },
  toast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("@/hooks/useDeferredFileUpload", () => ({
  useDeferredFileUpload: () => ({ uploadSingleFile: mocks.uploadSingleFile, isUploading: false }),
}));
vi.mock("@/hooks/useAttachments", () => ({
  useSignatureCaptures: () => mocks.signatures,
  useCaptureSignature: () => ({ mutateAsync: mocks.capture, isPending: false }),
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
// jsdom has no 2D canvas, so the pad is replaced by a stub that reports a PNG data URL.
vi.mock("./SignaturePad", () => ({
  SignaturePad: ({ onChange }: { onChange: (value: string | null) => void }) => (
    <div>
      <button type="button" onClick={() => onChange(PNG_DATA_URL)}>draw</button>
      <button type="button" onClick={() => onChange(null)}>wipe</button>
    </div>
  ),
}));

const t = thMessages.attachments.signature;
const consent = t.consentVersions["handover-2026-10-v1"];

function renderPanel(props: Partial<React.ComponentProps<typeof SignatureCapturePanel>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <SignatureCapturePanel ownerType="installation-job" ownerId="job-1" purpose="handover" canCapture {...props} />
    </NextIntlClientProvider>
  );
}

function fillValid() {
  fireEvent.change(screen.getByLabelText(new RegExp(t.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
  fireEvent.change(screen.getByLabelText(new RegExp(t.signerRole)), { target: { value: "เจ้าของบ้าน" } });
  fireEvent.click(screen.getByRole("checkbox", { name: new RegExp(t.consentLabel) }));
  fireEvent.click(screen.getByRole("button", { name: "draw" }));
}

describe("SignatureCapturePanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.signatures.data = { items: [] };
    mocks.signatures.isLoading = false;
    mocks.signatures.isError = false;
    mocks.uploadSingleFile.mockResolvedValue({ uploadIntentId: null, fileId: "file-sig" });
    mocks.capture.mockResolvedValue({});
  });

  it("lists captured signatures with signer, hash and image", () => {
    mocks.signatures.data = {
      items: [{
        id: "s-1", ownerType: "installation-job", ownerId: "job-1", purpose: "handover", signerName: "คุณสมชาย ใจดี", signerRole: "เจ้าของบ้าน",
        signedAtUtc: "2026-10-05T03:00:00Z", imageFileId: "file-sig", servingUrl: "/x", consentTextVersion: "handover-2026-10-v1",
        contentHash: "a".repeat(64), capturedBy: { id: "u-1", displayName: "ผู้บันทึก" },
      }],
    };
    renderPanel({ canCapture: false });

    expect(screen.getByText(/คุณสมชาย ใจดี/)).toBeInTheDocument();
    expect(screen.getByText(t.hashLabel.replace("{hash}", "a".repeat(12)))).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: t.capture })).not.toBeInTheDocument();
  });

  it("shows the empty state when there is no signature", () => {
    renderPanel({ canCapture: false });
    expect(screen.getByText(t.empty)).toBeInTheDocument();
  });

  it("keeps the save button disabled until name, consent and a signature are all present", () => {
    renderPanel();
    const save = screen.getByRole("button", { name: t.capture });
    expect(save).toBeDisabled();

    fireEvent.change(screen.getByLabelText(new RegExp(t.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
    expect(save).toBeDisabled();
    fireEvent.click(screen.getByRole("checkbox", { name: new RegExp(t.consentLabel) }));
    expect(save).toBeDisabled();
    fireEvent.click(screen.getByRole("button", { name: "draw" }));
    expect(save).toBeEnabled();
    fireEvent.click(screen.getByRole("button", { name: "wipe" }));
    expect(save).toBeDisabled();
  });

  it("shows the consent wording of the current version", () => {
    renderPanel();
    expect(screen.getByText(consent)).toBeInTheDocument();
  });

  it("uploads the PNG on submit, then captures with the consent version and the uploaded file id", async () => {
    renderPanel();
    expect(mocks.uploadSingleFile).not.toHaveBeenCalled();
    fillValid();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));

    await waitFor(() => expect(mocks.capture).toHaveBeenCalledTimes(1));
    const uploaded = mocks.uploadSingleFile.mock.calls[0][0] as File;
    expect(uploaded.type).toBe("image/png");
    expect(mocks.uploadSingleFile.mock.invocationCallOrder[0]).toBeLessThan(mocks.capture.mock.invocationCallOrder[0]);
    const call = mocks.capture.mock.calls[0][0] as { payload: Record<string, unknown>; idempotencyKey: string };
    expect(call.payload).toEqual({
      purpose: "handover",
      signerName: "คุณสมชาย ใจดี",
      signerRole: "เจ้าของบ้าน",
      imageFileId: "file-sig",
      consentAccepted: true,
      consentTextVersion: "handover-2026-10-v1",
    });
    expect(mocks.toast.success).toHaveBeenCalledWith(t.saved);
  });

  it("does not capture when the upload fails", async () => {
    mocks.uploadSingleFile.mockRejectedValueOnce(new Error("network"));
    renderPanel();
    fillValid();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));

    expect(await screen.findByText(thMessages.attachments.errors.failed)).toBeInTheDocument();
    expect(mocks.capture).not.toHaveBeenCalled();
  });

  it("reuses the uploaded image when the capture fails and the same signature is submitted again", async () => {
    mocks.capture.mockRejectedValueOnce(new ApiError({ status: 409, code: "ATTACHMENT_OWNER_LOCKED", message: "locked" }));
    renderPanel();
    fillValid();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));

    expect(await screen.findByText(thMessages.attachments.errors.ATTACHMENT_OWNER_LOCKED)).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));
    await waitFor(() => expect(mocks.capture).toHaveBeenCalledTimes(2));
    expect(mocks.uploadSingleFile).toHaveBeenCalledTimes(1);
  });
});
