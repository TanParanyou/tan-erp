import { describe, it, expect, vi } from "vitest";
import {
  validateDeferredFile,
  createDeferredFile,
  revokeDeferredFile,
  uploadDeferredFiles,
} from "./deferred-upload";

describe("deferred-upload utility", () => {
  it("validates file size correctly", () => {
    const smallFile = new File(["hello world"], "test.txt", { type: "text/plain" });
    expect(validateDeferredFile(smallFile, { maxSizeBytes: 1024 }).isValid).toBe(true);

    const largeFile = new File([new ArrayBuffer(2048)], "large.bin", { type: "application/octet-stream" });
    const result = validateDeferredFile(largeFile, { maxSizeBytes: 1024 });
    expect(result.isValid).toBe(false);
    expect(result.error).toContain("ขนาดไฟล์เกินกำหนด");
  });

  it("validates allowed MIME types", () => {
    const pngFile = new File(["png"], "image.png", { type: "image/png" });
    expect(validateDeferredFile(pngFile, { allowedTypes: ["image/*"] }).isValid).toBe(true);
    expect(validateDeferredFile(pngFile, { allowedTypes: ["application/pdf"] }).isValid).toBe(false);
  });

  it("creates and revokes deferred file with preview URL", () => {
    // Mock URL methods
    globalThis.URL.createObjectURL = vi.fn().mockReturnValue("blob:mock-url");
    globalThis.URL.revokeObjectURL = vi.fn();

    const file = new File(["test"], "sample.jpg", { type: "image/jpeg" });
    const deferred = createDeferredFile(file, "profile_pic");

    expect(deferred.previewUrl).toBe("blob:mock-url");
    expect(deferred.fieldName).toBe("profile_pic");

    revokeDeferredFile(deferred);
    expect(globalThis.URL.revokeObjectURL).toHaveBeenCalledWith("blob:mock-url");
  });

  it("uploads deferred files and maps results by fieldName", async () => {
    globalThis.URL.createObjectURL = vi.fn().mockReturnValue("blob:mock-url");
    globalThis.URL.revokeObjectURL = vi.fn();

    const file1 = new File(["1"], "doc1.pdf", { type: "application/pdf" });
    const file2 = new File(["2"], "doc2.pdf", { type: "application/pdf" });

    const deferredList = [
      createDeferredFile(file1, "attachment_a"),
      createDeferredFile(file2, "attachment_b"),
    ];

    const mockUploader = vi.fn().mockImplementation(async (file: File) => {
      return `https://storage.example.test/${file.name}`;
    });

    const result = await uploadDeferredFiles(deferredList, mockUploader);

    expect(mockUploader).toHaveBeenCalledTimes(2);
    expect(result.attachment_a).toBe("https://storage.example.test/doc1.pdf");
    expect(result.attachment_b).toBe("https://storage.example.test/doc2.pdf");
  });
});
