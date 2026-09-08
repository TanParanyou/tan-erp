import { describe, it, expect, vi } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useDebounce, useDebouncedCallback } from "./useDebounce";

describe("useDebounce", () => {
  it("returns debounced value after delay", () => {
    vi.useFakeTimers();
    const { result, rerender } = renderHook(({ val }) => useDebounce(val, 300), {
      initialProps: { val: "initial" },
    });

    expect(result.current).toBe("initial");

    rerender({ val: "changed" });
    expect(result.current).toBe("initial");

    act(() => {
      vi.advanceTimersByTime(300);
    });

    expect(result.current).toBe("changed");
    vi.useRealTimers();
  });

  it("debounces callback execution", () => {
    vi.useFakeTimers();
    const cb = vi.fn();
    const { result } = renderHook(() => useDebouncedCallback(cb, 200));

    act(() => {
      result.current("arg1");
      result.current("arg2");
    });

    expect(cb).not.toHaveBeenCalled();

    act(() => {
      vi.advanceTimersByTime(200);
    });

    expect(cb).toHaveBeenCalledTimes(1);
    expect(cb).toHaveBeenCalledWith("arg2");
    vi.useRealTimers();
  });
});
