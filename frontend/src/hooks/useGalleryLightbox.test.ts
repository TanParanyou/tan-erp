import { describe, it, expect } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useGalleryLightbox } from "./useGalleryLightbox";

describe("useGalleryLightbox hook", () => {
  const mockItems = [
    { id: "1", title: "Image 1" },
    { id: "2", title: "Image 2" },
    { id: "3", title: "Image 3" },
  ];

  it("initializes with default closed state", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    expect(result.current.isOpen).toBe(false);
    expect(result.current.currentIndex).toBe(0);
    expect(result.current.currentItem).toEqual(mockItems[0]);
  });

  it("opens at requested index", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    act(() => {
      result.current.openAt(2);
    });

    expect(result.current.isOpen).toBe(true);
    expect(result.current.currentIndex).toBe(2);
    expect(result.current.currentItem).toEqual(mockItems[2]);
  });

  it("clamps requested index within bounds", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    act(() => {
      result.current.openAt(999);
    });

    expect(result.current.currentIndex).toBe(2);

    act(() => {
      result.current.openAt(-5);
    });

    expect(result.current.currentIndex).toBe(0);
  });

  it("navigates next with carousel wrap-around", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    act(() => {
      result.current.openAt(1);
    });

    act(() => {
      result.current.next();
    });
    expect(result.current.currentIndex).toBe(2);

    // Wrap around to 0
    act(() => {
      result.current.next();
    });
    expect(result.current.currentIndex).toBe(0);
  });

  it("navigates prev with carousel wrap-around", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    act(() => {
      result.current.openAt(0);
    });

    // Wrap around to 2
    act(() => {
      result.current.prev();
    });
    expect(result.current.currentIndex).toBe(2);

    act(() => {
      result.current.prev();
    });
    expect(result.current.currentIndex).toBe(1);
  });

  it("jumps directly to specific index with goTo", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    act(() => {
      result.current.goTo(1);
    });
    expect(result.current.currentIndex).toBe(1);
    expect(result.current.currentItem).toEqual(mockItems[1]);
  });

  it("closes cleanly", () => {
    const { result } = renderHook(() => useGalleryLightbox(mockItems));

    act(() => {
      result.current.openAt(1);
    });
    expect(result.current.isOpen).toBe(true);

    act(() => {
      result.current.close();
    });
    expect(result.current.isOpen).toBe(false);
  });

  it("handles empty array safely", () => {
    const { result } = renderHook(() => useGalleryLightbox<string>([]));

    expect(result.current.isOpen).toBe(false);
    expect(result.current.currentIndex).toBe(0);
    expect(result.current.currentItem).toBeNull();

    act(() => {
      result.current.openAt(0);
    });
    expect(result.current.isOpen).toBe(true);
    expect(result.current.currentItem).toBeNull();

    act(() => {
      result.current.next();
      result.current.prev();
    });
    expect(result.current.currentIndex).toBe(0);
  });
});
