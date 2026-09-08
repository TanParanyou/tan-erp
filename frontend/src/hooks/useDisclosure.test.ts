import { describe, it, expect, vi } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useDisclosure, createDisclosureState } from "./useDisclosure";

describe("useDisclosure", () => {
  it("initializes with default values", () => {
    const { result } = renderHook(() => useDisclosure());
    expect(result.current.isOpen).toBe(false);
    expect(result.current.data).toBeNull();
  });

  it("opens with optional payload", () => {
    const onOpen = vi.fn();
    const { result } = renderHook(() => useDisclosure<string>({ onOpen }));

    act(() => {
      result.current.open("item-123");
    });

    expect(result.current.isOpen).toBe(true);
    expect(result.current.data).toBe("item-123");
    expect(onOpen).toHaveBeenCalledWith("item-123");
  });

  it("closes and resets data", () => {
    const onClose = vi.fn();
    const { result } = renderHook(() =>
      useDisclosure<number>({ defaultIsOpen: true, defaultData: 42, onClose })
    );

    act(() => {
      result.current.close();
    });

    expect(result.current.isOpen).toBe(false);
    expect(result.current.data).toBeNull();
    expect(onClose).toHaveBeenCalled();
  });

  it("toggles state", () => {
    const { result } = renderHook(() => useDisclosure());

    act(() => {
      result.current.toggle();
    });
    expect(result.current.isOpen).toBe(true);

    act(() => {
      result.current.toggle();
    });
    expect(result.current.isOpen).toBe(false);
  });

  it("works with createDisclosureState for pure logic", () => {
    const state = createDisclosureState<string>();
    expect(state.isOpen).toBe(false);

    state.open("test");
    expect(state.isOpen).toBe(true);
    expect(state.data).toBe("test");

    state.close();
    expect(state.isOpen).toBe(false);
    expect(state.data).toBeNull();
  });
});
