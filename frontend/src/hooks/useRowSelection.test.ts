import { describe, it, expect } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useRowSelection } from "./useRowSelection";

describe("useRowSelection", () => {
  it("initializes empty and toggles item selection", () => {
    const { result } = renderHook(() => useRowSelection<string>());

    expect(result.current.selectedCount).toBe(0);
    expect(result.current.isSelected("row-1")).toBe(false);

    act(() => {
      result.current.toggle("row-1");
    });
    expect(result.current.selectedCount).toBe(1);
    expect(result.current.isSelected("row-1")).toBe(true);

    act(() => {
      result.current.toggle("row-1");
    });
    expect(result.current.selectedCount).toBe(0);
  });

  it("handles selectAll and clearSelection correctly", () => {
    const { result } = renderHook(() => useRowSelection<string>());
    const allIds = ["a", "b", "c"];

    act(() => {
      result.current.selectAll(allIds);
    });
    expect(result.current.selectedCount).toBe(3);
    expect(result.current.isAllSelected(allIds)).toBe(true);

    act(() => {
      result.current.clearSelection();
    });
    expect(result.current.selectedCount).toBe(0);
    expect(result.current.isAllSelected(allIds)).toBe(false);
  });
});
