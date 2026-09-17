import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import { MultiImagePicker, type PendingImageItem } from "./MultiImagePicker";

vi.mock("@/lib/media/image-optimization", () => ({
  optimizeImageToWebP: vi.fn(async (file: File) => ({
    file,
    isOptimized: true,
    savedPercent: 50,
  })),
}));

// Mock CameraCaptureModal to test integration easily
vi.mock("./CameraCaptureModal", () => ({
  CameraCaptureModal: ({
    isOpen,
    onCapture,
    onTriggerNativeCamera,
  }: {
    isOpen: boolean;
    onCapture: (file: File) => void;
    onTriggerNativeCamera?: () => void;
  }) => {
    if (!isOpen) return null;
    return (
      <div data-testid="camera-capture-modal">
        <button
          onClick={() => {
            const fakeCapturedFile = new File(["photo-bytes"], "site_camera_snap.webp", {
              type: "image/webp",
            });
            onCapture(fakeCapturedFile);
          }}
        >
          Mock Capture Snapshot
        </button>
        <button onClick={() => onTriggerNativeCamera?.()}>
          Mock Native Camera Fallback
        </button>
      </div>
    );
  },
}));

describe("MultiImagePicker component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    global.URL.createObjectURL = vi.fn(() => "blob:http://localhost/test-preview");
    global.URL.revokeObjectURL = vi.fn();
  });

  it("renders browse files button and take photo button when enabled", () => {
    render(
      <MultiImagePicker
        items={[]}
        onChange={vi.fn()}
        enableCamera={true}
      />
    );

    expect(screen.getByRole("button", { name: /เลือกไฟล์ภาพจากเครื่อง|Browse files/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /ถ่ายภาพด้วยกล้อง|Take photo with camera/i })).toBeInTheDocument();
  });

  it("does not render camera button when enableCamera is false", () => {
    render(
      <MultiImagePicker
        items={[]}
        onChange={vi.fn()}
        enableCamera={false}
      />
    );

    expect(screen.getByRole("button", { name: /เลือกไฟล์ภาพจากเครื่อง|Browse files/i })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /ถ่ายภาพด้วยกล้อง|Take photo with camera/i })).not.toBeInTheDocument();
  });

  it("opens CameraCaptureModal when camera button is clicked", () => {
    render(
      <MultiImagePicker
        items={[]}
        onChange={vi.fn()}
        enableCamera={true}
      />
    );

    const cameraBtn = screen.getByRole("button", { name: /ถ่ายภาพด้วยกล้อง|Take photo with camera/i });
    fireEvent.click(cameraBtn);

    expect(screen.getByTestId("camera-capture-modal")).toBeInTheDocument();
  });

  it("receives captured photo from CameraCaptureModal and triggers optimization", async () => {
    const onChange = vi.fn();
    render(
      <MultiImagePicker
        items={[]}
        onChange={onChange}
        enableCamera={true}
      />
    );

    const cameraBtn = screen.getByRole("button", { name: /ถ่ายภาพด้วยกล้อง|Take photo with camera/i });
    fireEvent.click(cameraBtn);

    const mockCaptureBtn = screen.getByText("Mock Capture Snapshot");
    await act(async () => {
      fireEvent.click(mockCaptureBtn);
    });

    expect(onChange).toHaveBeenCalled();
  });

  it("renders pending image items with caption input and remove button", () => {
    const items: PendingImageItem[] = [
      {
        id: "img-1",
        originalFile: new File(["data"], "test.png", { type: "image/png" }),
        previewUrl: "blob:http://localhost/test-preview",
        caption: "Living room wall",
        isOptimizing: false,
        savingsSummary: "WebP (40% saved)",
      },
    ];

    const onChange = vi.fn();
    render(<MultiImagePicker items={items} onChange={onChange} />);

    expect(screen.getByDisplayValue("Living room wall")).toBeInTheDocument();
    expect(screen.getByText("WebP (40% saved)")).toBeInTheDocument();
  });
});
