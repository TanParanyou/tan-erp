import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import { CameraCaptureModal } from "./CameraCaptureModal";
import type { UseCameraReturn } from "@/hooks/useCamera";

const mockStartCamera = vi.fn().mockResolvedValue(true);
const mockStopCamera = vi.fn();
const mockSwitchCamera = vi.fn().mockResolvedValue(undefined);
const mockSelectDevice = vi.fn().mockResolvedValue(undefined);
const mockCapturePhoto = vi.fn();

let mockUseCameraState: Partial<UseCameraReturn> = {};

vi.mock("@/hooks/useCamera", () => ({
  useCamera: () => ({
    videoRef: { current: null },
    stream: null,
    isActive: true,
    isLoading: false,
    error: null,
    devices: [],
    selectedDeviceId: null,
    facingMode: "environment",
    hasMultipleCameras: false,
    isSupported: true,
    startCamera: mockStartCamera,
    stopCamera: mockStopCamera,
    switchCamera: mockSwitchCamera,
    selectDevice: mockSelectDevice,
    capturePhoto: mockCapturePhoto,
    ...mockUseCameraState,
  }),
}));

describe("CameraCaptureModal component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseCameraState = {
      isActive: true,
      isLoading: false,
      error: null,
      devices: [],
    };
    global.URL.createObjectURL = vi.fn(() => "blob:http://localhost/captured-photo");
    global.URL.revokeObjectURL = vi.fn();
  });

  it("renders when isOpen is true and starts camera", () => {
    render(
      <CameraCaptureModal
        isOpen={true}
        onClose={vi.fn()}
        onCapture={vi.fn()}
      />
    );

    expect(screen.getByRole("button", { name: /ถ่ายภาพ|Take Photo/i })).toBeInTheDocument();
    expect(mockStartCamera).toHaveBeenCalled();
  });

  it("handles shutter click and displays snapshot review mode", async () => {
    const fakeFile = new File(["test-image"], "site_photo_test.webp", { type: "image/webp" });
    mockCapturePhoto.mockResolvedValueOnce(fakeFile);

    render(
      <CameraCaptureModal
        isOpen={true}
        onClose={vi.fn()}
        onCapture={vi.fn()}
      />
    );

    const shutterBtn = screen.getByRole("button", { name: /ถ่ายภาพ|Take Photo/i });
    await act(async () => {
      fireEvent.click(shutterBtn);
    });

    expect(mockCapturePhoto).toHaveBeenCalled();
    // After capturing, review buttons should appear
    expect(screen.getByRole("button", { name: /ถ่ายใหม่|Retake/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /ใช้ภาพนี้|Use Photo/i })).toBeInTheDocument();
  });

  it("calls onCapture with captured file when confirming snapshot", async () => {
    const onCapture = vi.fn();
    const fakeFile = new File(["test-image"], "site_photo_test.webp", { type: "image/webp" });
    mockCapturePhoto.mockResolvedValueOnce(fakeFile);

    render(
      <CameraCaptureModal
        isOpen={true}
        onClose={vi.fn()}
        onCapture={onCapture}
      />
    );

    const shutterBtn = screen.getByRole("button", { name: /ถ่ายภาพ|Take Photo/i });
    await act(async () => {
      fireEvent.click(shutterBtn);
    });

    const confirmBtn = screen.getByRole("button", { name: /ใช้ภาพนี้|Use Photo/i });
    await act(async () => {
      fireEvent.click(confirmBtn);
    });

    expect(onCapture).toHaveBeenCalledWith(fakeFile);
  });

  it("resumes camera and clears preview when Retake is clicked", async () => {
    const fakeFile = new File(["test-image"], "site_photo_test.webp", { type: "image/webp" });
    mockCapturePhoto.mockResolvedValueOnce(fakeFile);

    render(
      <CameraCaptureModal
        isOpen={true}
        onClose={vi.fn()}
        onCapture={vi.fn()}
      />
    );

    const shutterBtn = screen.getByRole("button", { name: /ถ่ายภาพ|Take Photo/i });
    await act(async () => {
      fireEvent.click(shutterBtn);
    });

    const retakeBtn = screen.getByRole("button", { name: /ถ่ายใหม่|Retake/i });
    await act(async () => {
      fireEvent.click(retakeBtn);
    });

    expect(mockStartCamera).toHaveBeenCalled();
    expect(global.URL.revokeObjectURL).toHaveBeenCalledWith("blob:http://localhost/captured-photo");
    expect(screen.getByRole("button", { name: /ถ่ายภาพ|Take Photo/i })).toBeInTheDocument();
  });

  it("shows error state and triggers native camera fallback when provided", () => {
    mockUseCameraState = {
      isActive: false,
      error: { code: "PERMISSION_DENIED" },
    };

    const onTriggerNativeCamera = vi.fn();

    render(
      <CameraCaptureModal
        isOpen={true}
        onClose={vi.fn()}
        onCapture={vi.fn()}
        onTriggerNativeCamera={onTriggerNativeCamera}
      />
    );

    const fallbackBtn = screen.getByRole("button", { name: /ถ่ายผ่านกล้องของอุปกรณ์|Use Device Camera/i });
    expect(fallbackBtn).toBeInTheDocument();

    fireEvent.click(fallbackBtn);
    expect(onTriggerNativeCamera).toHaveBeenCalled();
  });
});
