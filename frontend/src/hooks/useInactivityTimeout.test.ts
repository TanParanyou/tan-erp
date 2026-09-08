import { describe, it, expect, vi } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useInactivityTimeout } from "./useInactivityTimeout";

describe("useInactivityTimeout hook", () => {
  it("warns before timing out and triggers onTimeout", async () => {
    vi.useFakeTimers();
    const onTimeout = vi.fn();

    const { result } = renderHook(() =>
      useInactivityTimeout({
        idleTimeoutMs: 10000,
        warningDurationMs: 4000,
        checkIntervalMs: 1000,
        onTimeout,
      })
    );

    expect(result.current.isWarningOpen).toBe(false);

    // Advance 7 seconds (elapsed = 7s, remaining = 3s <= warningDurationMs of 4s)
    act(() => {
      vi.advanceTimersByTime(7000);
    });

    expect(result.current.isWarningOpen).toBe(true);
    expect(result.current.secondsRemaining).toBeLessThanOrEqual(4);

    // Advance remaining time to trigger timeout
    act(() => {
      vi.advanceTimersByTime(4000);
    });

    expect(onTimeout).toHaveBeenCalledTimes(1);
    vi.useRealTimers();
  });
});
