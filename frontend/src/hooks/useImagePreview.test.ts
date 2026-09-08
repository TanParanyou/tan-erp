import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useImagePreview } from "./useImagePreview";

describe("useImagePreview hook", () => {
  beforeEach(() => {
    global.URL.createObjectURL = vi.fn(() => "blob:http://localhost/test-uuid");
    global.URL.revokeObjectURL = vi.fn();
  });

  it("selects file, creates object URL and clears preview", () => {
    const { result } = renderHook(() => useImagePreview());

    const file = new File(["dummy content"], "photo.png", { type: "image/png" });

    act(() => {
      const ok = result.current.selectFile(file);
      expect(ok).toBe(true);
    });

    expect(result.current.selectedFile).toBe(file);
    expect(result.current.previewUrl).toBe("blob:http://localhost/test-uuid");

    act(() => {
      result.current.clearPreview();
    });

    expect(result.current.selectedFile).toBeNull();
    expect(result.current.previewUrl).toBe("");
    expect(global.URL.revokeObjectURL).toHaveBeenCalled();
  });

  it("validates file type", () => {
    const { result } = renderHook(() => useImagePreview());
    const textFile = new File(["text"], "notes.txt", { type: "text/plain" });

    act(() => {
      const ok = result.current.selectFile(textFile);
      expect(ok).toBe(false);
    });

    expect(result.current.error).toContain("Invalid file type");
  });
});
