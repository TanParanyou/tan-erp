import React from "react";
import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { NextIntlClientProvider } from "next-intl";
import { ToastProvider } from "@/hooks/useToast";
import thMessages from "@/messages/th.json";
import { useCurrentLocation } from "./useCurrentLocation";

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <NextIntlClientProvider locale="th" messages={thMessages}>
    <ToastProvider>{children}</ToastProvider>
  </NextIntlClientProvider>
);

describe("useCurrentLocation", () => {
  const originalGeolocation = navigator.geolocation;

  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    Object.defineProperty(navigator, "geolocation", {
      value: originalGeolocation,
      configurable: true,
      writable: true,
    });
  });

  it("handles geolocation not supported", async () => {
    Object.defineProperty(navigator, "geolocation", {
      value: undefined,
      configurable: true,
      writable: true,
    });

    const onError = vi.fn();
    const { result } = renderHook(() => useCurrentLocation({ onError }), { wrapper });

    let coords: unknown;
    await act(async () => {
      coords = await result.current.getCurrentLocation();
    });

    expect(coords).toBeNull();
    expect(onError).toHaveBeenCalled();
  });

  it("fetches coordinates successfully", async () => {
    const mockGetCurrentPosition = vi.fn((success) => {
      success({
        coords: {
          latitude: 13.7563309,
          longitude: 100.5017651,
        },
      });
    });

    Object.defineProperty(navigator, "geolocation", {
      value: {
        getCurrentPosition: mockGetCurrentPosition,
      },
      configurable: true,
      writable: true,
    });

    const onSuccess = vi.fn();
    const { result } = renderHook(() => useCurrentLocation({ onSuccess }), { wrapper });

    let coords: unknown;
    await act(async () => {
      coords = await result.current.getCurrentLocation();
    });

    expect(coords).toEqual({
      latitude: 13.7563309,
      longitude: 100.5017651,
    });
    expect(onSuccess).toHaveBeenCalledWith({
      latitude: 13.7563309,
      longitude: 100.5017651,
    });
  });

  it("handles geolocation error", async () => {
    const mockGetCurrentPosition = vi.fn((_success, error) => {
      error({
        code: 1, // PERMISSION_DENIED
        PERMISSION_DENIED: 1,
        POSITION_UNAVAILABLE: 2,
        TIMEOUT: 3,
        message: "User denied Geolocation",
      });
    });

    Object.defineProperty(navigator, "geolocation", {
      value: {
        getCurrentPosition: mockGetCurrentPosition,
      },
      configurable: true,
      writable: true,
    });

    const onError = vi.fn();
    const { result } = renderHook(() => useCurrentLocation({ onError }), { wrapper });

    let coords: unknown;
    await act(async () => {
      coords = await result.current.getCurrentLocation();
    });

    expect(coords).toBeNull();
    expect(onError).toHaveBeenCalled();
  });
});
