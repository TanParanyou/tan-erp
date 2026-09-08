import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useClipboard } from "./useClipboard";

describe("useClipboard", () => {
  beforeEach(() => {
    Object.assign(navigator, {
      clipboard: {
        writeText: vi.fn().mockResolvedValue(undefined),
      },
    });
  });

  it("copies text to clipboard and resets after timeout", async () => {
    vi.useFakeTimers();
    const { result } = renderHook(() => useClipboard({ timeout: 1000 }));

    expect(result.current.copied).toBe(false);

    let success = false;
    await act(async () => {
      success = await result.current.copy("test string");
    });

    expect(success).toBe(true);
    expect(result.current.copied).toBe(true);
    expect(navigator.clipboard.writeText).toHaveBeenCalledWith("test string");

    act(() => {
      vi.advanceTimersByTime(1000);
    });

    expect(result.current.copied).toBe(false);
    vi.useRealTimers();
  });
});
