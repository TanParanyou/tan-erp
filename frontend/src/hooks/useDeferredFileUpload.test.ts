import { describe, expect, it, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useDeferredFileUpload } from "./useDeferredFileUpload";
import { fileClient } from "@/lib/api/file-client";
import { ApiError } from "@/lib/api/api-error";

vi.mock("@/lib/api/file-client", () => ({
  fileClient: {
    createSession: vi.fn(),
    completeSession: vi.fn(),
  },
}));

const mockedCreateSession = vi.mocked(fileClient.createSession);
const mockedCompleteSession = vi.mocked(fileClient.completeSession);

describe("useDeferredFileUpload hook", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    let count = 0;
    vi.spyOn(crypto, "randomUUID").mockImplementation(() => {
      count += 1;
      return `mock-uuid-${count}` as `${string}-${string}-${string}-${string}-${string}`;
    });
  });

  it("handles single file upload for new parent with creationIntentId and exact slotId", async () => {
    mockedCreateSession.mockResolvedValueOnce({
      sessionId: "sess-1",
      expiresAtUtc: "2099-01-01T00:00:00Z",
      slots: [{ slotId: "slot-abc", filename: "avatar.webp", mediaType: "image/webp", fileSizeBytes: 1024 }],
    });

    mockedCompleteSession.mockResolvedValueOnce({
      sessionId: "sess-1",
      files: [{ fileId: "file-xyz", filename: "avatar.webp", mediaType: "image/webp", fileSizeBytes: 1024, servingUrl: "http://example.com/avatar.webp" }],
    });

    const { result } = renderHook(() =>
      useDeferredFileUpload({ parentType: "customer" })
    );

    const dummyFile = new File(["bytes"], "avatar.webp", { type: "image/webp" });

    let uploadRes: { uploadIntentId: string | null; fileId?: string } | undefined;
    await act(async () => {
      uploadRes = await result.current.uploadSingleFile(dummyFile, {
        token: "auth-token",
        membershipId: "mem-1",
        locale: "th",
      });
    });

    expect(mockedCreateSession).toHaveBeenCalledTimes(1);
    const sessionReq = mockedCreateSession.mock.calls[0][0];
    expect(sessionReq.parentType).toBe("customer");
    expect(sessionReq.parentId).toBeNull();
    expect(sessionReq.creationIntentId).toBe("mock-uuid-1");
    expect(sessionReq.files).toEqual([
      {
        filename: "avatar.webp",
        mediaType: "image/webp",
        fileSizeBytes: dummyFile.size,
      },
    ]);

    expect(mockedCompleteSession).toHaveBeenCalledTimes(1);
    const completeCall = mockedCompleteSession.mock.calls[0];
    expect(completeCall[0]).toBe("sess-1");
    expect(completeCall[1]).toEqual([{ slotId: "slot-abc", file: dummyFile }]);

    expect(uploadRes).toEqual({
      uploadIntentId: "mock-uuid-1",
      fileId: "file-xyz",
    });
    expect(result.current.isUploading).toBe(false);
    expect(result.current.uploadError).toBeNull();
  });

  it("handles multiple files upload with captions for an existing parent without creationIntentId", async () => {
    mockedCreateSession.mockResolvedValueOnce({
      sessionId: "sess-site-1",
      expiresAtUtc: "2099-01-01T00:00:00Z",
      slots: [
        { slotId: "slot-1", filename: "front.webp", mediaType: "image/webp", fileSizeBytes: 2000 },
        { slotId: "slot-2", filename: "back.webp", mediaType: "image/webp", fileSizeBytes: 3000 },
      ],
    });

    mockedCompleteSession.mockResolvedValueOnce({
      sessionId: "sess-site-1",
      files: [
        { fileId: "file-1", filename: "front.webp", mediaType: "image/webp", fileSizeBytes: 2000, servingUrl: "http://example.com/front.webp" },
        { fileId: "file-2", filename: "back.webp", mediaType: "image/webp", fileSizeBytes: 3000, servingUrl: "http://example.com/back.webp" },
      ],
    });

    const { result } = renderHook(() =>
      useDeferredFileUpload({ parentType: "site", parentId: "existing-site-id" })
    );

    const file1 = new File(["front"], "front.webp", { type: "image/webp" });
    const file2 = new File(["back"], "back.webp", { type: "image/webp" });

    let uploadRes: Awaited<ReturnType<typeof result.current.uploadFiles>> | undefined;
    await act(async () => {
      uploadRes = await result.current.uploadFiles(
        [
          { file: file1, caption: "ด้านหน้าอาคาร" },
          { file: file2, caption: "ด้านหลังอาคาร" },
        ],
        { token: "auth-token", membershipId: "mem-1" }
      );
    });

    expect(mockedCreateSession).toHaveBeenCalledTimes(1);
    const sessionReq = mockedCreateSession.mock.calls[0][0];
    expect(sessionReq.parentType).toBe("site");
    expect(sessionReq.parentId).toBe("existing-site-id");
    expect(sessionReq.creationIntentId).toBeNull();

    expect(mockedCompleteSession).toHaveBeenCalledTimes(1);
    const completeCall = mockedCompleteSession.mock.calls[0];
    expect(completeCall[0]).toBe("sess-site-1");
    expect(completeCall[1]).toEqual([
      { slotId: "slot-1", file: file1 },
      { slotId: "slot-2", file: file2 },
    ]);

    expect(uploadRes?.uploadIntentId).toBeNull();
    expect(uploadRes?.files).toHaveLength(2);
    expect(uploadRes?.files[0]).toEqual({
      fileId: "file-1",
      filename: "front.webp",
      mediaType: "image/webp",
      fileSizeBytes: 2000,
      servingUrl: "http://example.com/front.webp",
      caption: "ด้านหน้าอาคาร",
    });
    expect(uploadRes?.files[1].caption).toBe("ด้านหลังอาคาร");
    expect(uploadRes?.primaryFileId).toBe("file-1");
  });

  it("returns empty result without calling API when file list is empty", async () => {
    const { result } = renderHook(() =>
      useDeferredFileUpload({ parentType: "customer" })
    );

    let uploadRes: Awaited<ReturnType<typeof result.current.uploadFiles>> | undefined;
    await act(async () => {
      uploadRes = await result.current.uploadFiles([], { token: "token" });
    });

    expect(mockedCreateSession).not.toHaveBeenCalled();
    expect(mockedCompleteSession).not.toHaveBeenCalled();
    expect(uploadRes).toEqual({
      uploadIntentId: null,
      files: [],
    });
  });

  it("rotates intent ID on resetIntent()", async () => {
    mockedCreateSession.mockResolvedValue({
      sessionId: "sess-x",
      expiresAtUtc: "2099-01-01T00:00:00Z",
      slots: [{ slotId: "slot-x", filename: "f.webp", mediaType: "image/webp", fileSizeBytes: 100 }],
    });

    mockedCompleteSession.mockResolvedValue({
      sessionId: "sess-x",
      files: [{ fileId: "file-x", filename: "f.webp", mediaType: "image/webp", fileSizeBytes: 100, servingUrl: "http://example.com/f.webp" }],
    });

    const { result } = renderHook(() =>
      useDeferredFileUpload({ parentType: "customer" })
    );

    const f1 = new File(["1"], "f.webp", { type: "image/webp" });
    await act(async () => {
      await result.current.uploadSingleFile(f1, { token: "token" });
    });

    expect(mockedCreateSession.mock.calls[0][0].creationIntentId).toBe("mock-uuid-1");

    // Reset intent
    act(() => {
      result.current.resetIntent();
    });

    // Next upload gets a new intent ID
    await act(async () => {
      await result.current.uploadSingleFile(f1, { token: "token" });
    });

    expect(mockedCreateSession.mock.calls[1][0].creationIntentId).toBe("mock-uuid-2");
  });

  it("handles errors cleanly and records uploadError state", async () => {
    mockedCreateSession.mockRejectedValueOnce(
      new ApiError({ status: 400, code: "REQUEST_VALIDATION_FAILED", message: "Invalid payload" })
    );

    const { result } = renderHook(() =>
      useDeferredFileUpload({ parentType: "customer" })
    );

    const dummyFile = new File(["x"], "avatar.webp", { type: "image/webp" });

    let thrownError: unknown = null;
    await act(async () => {
      try {
        await result.current.uploadSingleFile(dummyFile, { token: "token" });
      } catch (err) {
        thrownError = err;
      }
    });

    expect(thrownError).toBeInstanceOf(ApiError);
    expect(result.current.isUploading).toBe(false);
    expect(result.current.uploadError).toBe("Invalid payload");
  });
});
