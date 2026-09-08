import { describe, it, expect, vi } from "vitest";
import { renderHook } from "@testing-library/react";
import { useKeyboardShortcut } from "./useKeyboardShortcut";

describe("useKeyboardShortcut", () => {
  it("triggers callback when matching key is pressed", () => {
    const cb = vi.fn();
    renderHook(() =>
      useKeyboardShortcut({ key: "s", ctrlOrMeta: true }, cb)
    );

    const event = new KeyboardEvent("keydown", {
      key: "s",
      ctrlKey: true,
      cancelable: true,
    });
    window.dispatchEvent(event);

    expect(cb).toHaveBeenCalledTimes(1);
  });

  it("does not trigger when modifier key is missing", () => {
    const cb = vi.fn();
    renderHook(() =>
      useKeyboardShortcut({ key: "s", ctrlOrMeta: true }, cb)
    );

    const event = new KeyboardEvent("keydown", {
      key: "s",
      ctrlKey: false,
      metaKey: false,
    });
    window.dispatchEvent(event);

    expect(cb).not.toHaveBeenCalled();
  });
});
