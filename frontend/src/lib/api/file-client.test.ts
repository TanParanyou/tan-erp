import { afterEach, describe, expect, it, vi } from "vitest";
import { FileClient } from "./file-client";

describe("FileClient", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("loads protected file content with bearer token and membership", async () => {
    const expectedBlob = new Blob(["image"], { type: "image/webp" });
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(expectedBlob, {
        status: 200,
        headers: { "Content-Type": "image/webp" },
      })
    );
    vi.stubGlobal("fetch", fetchMock);

    const client = new FileClient("https://api.example.test");
    const result = await client.getFileBlob("file-123", {
      token: "token-123",
      membershipId: "membership-123",
      locale: "th",
    });

    expect(result.type).toBe("image/webp");
    expect(fetchMock).toHaveBeenCalledWith(
      "https://api.example.test/api/v1/files/file-123/content",
      expect.objectContaining({
        headers: expect.objectContaining({
          Authorization: "Bearer token-123",
          "X-Membership-Id": "membership-123",
        }),
      })
    );
  });
});
