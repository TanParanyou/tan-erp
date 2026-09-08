import { describe, it, expect, vi } from "vitest";
import { renderHook, act } from "@testing-library/react";
import React from "react";
import { ToastProvider, useToast } from "./useToast";

describe("useToast hook", () => {
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <ToastProvider>{children}</ToastProvider>
  );

  it("throws error if used outside ToastProvider", () => {
    expect(() => renderHook(() => useToast())).toThrow(
      "useToast must be used within ToastProvider"
    );
  });

  it("adds and removes toasts with auto-duration", () => {
    vi.useFakeTimers();
    const { result } = renderHook(() => useToast(), { wrapper });

    act(() => {
      result.current.toast.success("Saved successfully");
    });

    expect(result.current.toasts.length).toBe(1);
    expect(result.current.toasts[0].message).toBe("Saved successfully");
    expect(result.current.toasts[0].type).toBe("success");

    act(() => {
      vi.advanceTimersByTime(4000);
    });

    expect(result.current.toasts.length).toBe(0);
    vi.useRealTimers();
  });
});
