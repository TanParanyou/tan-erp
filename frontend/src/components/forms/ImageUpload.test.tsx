import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import { ImageUpload } from "./ImageUpload";

vi.mock("@/lib/media/image-optimization", () => ({
  optimizeImageToWebP: vi.fn(async (file: File) => ({
    file,
    isOptimized: true,
  })),
}));

describe("ImageUpload component", () => {
  beforeEach(() => {
    global.URL.createObjectURL = vi.fn(() => "blob:http://localhost/upload-test");
    global.URL.revokeObjectURL = vi.fn();
  });

  it("renders upload button and triggers file input", () => {
    render(<ImageUpload label="Profile Photo" onChange={vi.fn()} />);

    expect(screen.getByText("Profile Photo")).toBeInTheDocument();
    expect(screen.getByRole("button")).toBeInTheDocument();
  });

  it("handles valid image file selection in deferred flow", async () => {
    const onChange = vi.fn();
    render(<ImageUpload label="Profile Photo" onChange={onChange} />);

    const file = new File(["dummy content"], "logo.png", { type: "image/png" });
    const input = document.querySelector('input[type="file"]') as HTMLInputElement;

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    expect(onChange).toHaveBeenCalledWith(expect.any(File));
    expect(screen.getByAltText("Preview")).toBeInTheDocument();
  });

  it("revokes object URL on remove and calls onChange with null", async () => {
    const onChange = vi.fn();
    render(<ImageUpload label="Profile Photo" onChange={onChange} />);

    const file = new File(["dummy content"], "logo.png", { type: "image/png" });
    const input = document.querySelector('input[type="file"]') as HTMLInputElement;

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    const removeBtn = screen.getByLabelText("ลบ");
    await act(async () => {
      fireEvent.click(removeBtn);
    });

    expect(onChange).toHaveBeenCalledWith(null);
    expect(global.URL.revokeObjectURL).toHaveBeenCalledWith("blob:http://localhost/upload-test");
  });

  it("revokes object URL on component unmount to prevent memory leak", async () => {
    const { unmount } = render(<ImageUpload label="Profile Photo" onChange={vi.fn()} />);

    const file = new File(["dummy content"], "logo.png", { type: "image/png" });
    const input = document.querySelector('input[type="file"]') as HTMLInputElement;

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    unmount();
    expect(global.URL.revokeObjectURL).toHaveBeenCalledWith("blob:http://localhost/upload-test");
  });
});
