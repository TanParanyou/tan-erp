import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useCamera } from "./useCamera";

describe("useCamera Hook", () => {
  let mockTracks: { stop: ReturnType<typeof vi.fn> }[];
  let mockStream: MediaStream;

  beforeEach(() => {
    mockTracks = [{ stop: vi.fn() }];
    mockStream = {
      getTracks: vi.fn(() => mockTracks),
    } as unknown as MediaStream;

    Object.defineProperty(global.navigator, "mediaDevices", {
      writable: true,
      value: {
        getUserMedia: vi.fn().mockResolvedValue(mockStream),
        enumerateDevices: vi.fn().mockResolvedValue([
          { kind: "videoinput", deviceId: "cam1", label: "Front Camera" },
          { kind: "videoinput", deviceId: "cam2", label: "Back Camera" },
        ]),
      },
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("initializes with expected default states", () => {
    const { result } = renderHook(() => useCamera());

    expect(result.current.isActive).toBe(false);
    expect(result.current.isLoading).toBe(false);
    expect(result.current.error).toBeNull();
    expect(result.current.stream).toBeNull();
    expect(result.current.facingMode).toBe("environment");
    expect(result.current.isSupported).toBe(true);
  });

  it("starts camera and sets active state and stream", async () => {
    const { result } = renderHook(() => useCamera());

    let success = false;
    await act(async () => {
      success = await result.current.startCamera();
    });

    expect(success).toBe(true);
    expect(result.current.isActive).toBe(true);
    expect(result.current.isLoading).toBe(false);
    expect(result.current.stream).toBe(mockStream);
    expect(result.current.devices.length).toBe(2);
    expect(result.current.hasMultipleCameras).toBe(true);
  });

  it("stops camera and terminates all tracks", async () => {
    const { result } = renderHook(() => useCamera());

    await act(async () => {
      await result.current.startCamera();
    });

    expect(result.current.isActive).toBe(true);

    act(() => {
      result.current.stopCamera();
    });

    expect(result.current.isActive).toBe(false);
    expect(result.current.stream).toBeNull();
    expect(mockTracks[0].stop).toHaveBeenCalled();
  });

  it("handles Permission Denied error", async () => {
    const permissionError = new DOMException("Permission denied", "NotAllowedError");
    vi.mocked(navigator.mediaDevices.getUserMedia).mockRejectedValueOnce(permissionError);

    const onError = vi.fn();
    const { result } = renderHook(() => useCamera({ onError }));

    let success = true;
    await act(async () => {
      success = await result.current.startCamera();
    });

    expect(success).toBe(false);
    expect(result.current.isActive).toBe(false);
    expect(result.current.error?.code).toBe("PERMISSION_DENIED");
    expect(onError).toHaveBeenCalledWith(expect.objectContaining({ code: "PERMISSION_DENIED" }));
  });

  it("handles Device Not Found error", async () => {
    const notFoundError = new DOMException("No camera", "NotFoundError");
    vi.mocked(navigator.mediaDevices.getUserMedia).mockRejectedValueOnce(notFoundError);

    const { result } = renderHook(() => useCamera());

    await act(async () => {
      await result.current.startCamera();
    });

    expect(result.current.error?.code).toBe("NOT_FOUND");
  });

  it("switches camera between front and back correctly", async () => {
    const { result } = renderHook(() => useCamera({ initialFacingMode: "environment" }));

    await act(async () => {
      await result.current.startCamera();
    });

    expect(result.current.facingMode).toBe("environment");

    // Switches from environment (Back) to user (Front)
    await act(async () => {
      await result.current.switchCamera();
    });

    expect(result.current.facingMode).toBe("user");
    expect(result.current.selectedDeviceId).toBe("cam1");

    // Switches back from user (Front) to environment (Back)
    await act(async () => {
      await result.current.switchCamera();
    });

    expect(result.current.facingMode).toBe("environment");
    expect(result.current.selectedDeviceId).toBe("cam2");
  });

  it("captures photo returns File when video element has valid dimensions", async () => {
    const { result } = renderHook(() => useCamera());

    const mockVideo = document.createElement("video");
    Object.defineProperty(mockVideo, "videoWidth", { value: 1280 });
    Object.defineProperty(mockVideo, "videoHeight", { value: 720 });

    // Mock HTMLCanvasElement toBlob
    vi.spyOn(HTMLCanvasElement.prototype, "getContext").mockReturnValue({
      drawImage: vi.fn(),
    } as unknown as CanvasRenderingContext2D);

    vi.spyOn(HTMLCanvasElement.prototype, "toBlob").mockImplementation((callback) => {
      const mockBlob = new Blob(["fake-image-content"], { type: "image/webp" });
      callback(mockBlob);
    });

    let photo: unknown;
    await act(async () => {
      photo = await result.current.capturePhoto({
        videoElement: mockVideo,
        fileNamePrefix: "test_photo",
        format: "image/webp",
      });
    });

    expect(photo).toBeInstanceOf(File);
    if (photo instanceof File) {
      expect(photo.type).toBe("image/webp");
      expect(photo.name).toMatch(/^test_photo_\d{8}_\d{6}\.webp$/);
    }
  });

  it("returns null if video element has 0 dimensions", async () => {
    const { result } = renderHook(() => useCamera());

    const mockVideo = document.createElement("video");
    Object.defineProperty(mockVideo, "videoWidth", { value: 0 });
    Object.defineProperty(mockVideo, "videoHeight", { value: 0 });

    let photo: unknown;
    await act(async () => {
      photo = await result.current.capturePhoto({ videoElement: mockVideo });
    });

    expect(photo).toBeNull();
  });
});
