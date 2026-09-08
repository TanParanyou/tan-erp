import { describe, it, expect } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useConfirm } from "./useConfirm";

describe("useConfirm hook", () => {
  it("opens confirmation dialog and resolves response", async () => {
    const { result } = renderHook(() => useConfirm());

    let promise: Promise<boolean> | null = null;
    act(() => {
      promise = result.current.confirm({
        title: "Delete Item",
        message: "Are you sure?",
      });
    });

    expect(promise).toBeInstanceOf(Promise);
    expect(result.current.ConfirmDialog).toBeDefined();
  });
});
