import { describe, it, expect } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useRowSelection } from "./useRowSelection";

describe("useRowSelection", () => {
  it("manages selected IDs", () => {
    const { result } = renderHook(() => useRowSelection<number>([1]));

    expect(result.current.isSelected(1)).toBe(true);
    expect(result.current.selectedCount).toBe(1);

    act(() => {
      result.current.toggleSelection(2);
    });
    expect(result.current.isSelected(2)).toBe(true);
    expect(result.current.selectedCount).toBe(2);

    act(() => {
      result.current.toggleSelection(1);
    });
    expect(result.current.isSelected(1)).toBe(false);

    act(() => {
      result.current.clearSelection();
    });
    expect(result.current.selectedCount).toBe(0);
  });
});
