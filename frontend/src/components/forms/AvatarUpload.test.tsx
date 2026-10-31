import React, { useState } from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { AvatarUpload } from "./AvatarUpload";

function AvatarHarness({ existingUrl }: { existingUrl?: string | null }) {
  const [file, setFile] = useState<File | null>(null);
  return (
    <AvatarUpload
      file={file}
      onFileChange={setFile}
      existingUrl={existingUrl}
      displayName="สมชาย ใจดี"
    />
  );
}

describe("AvatarUpload", () => {
  it("renders upload button and default placeholder when no avatar exists", () => {
    render(<AvatarHarness />);

    expect(screen.getByRole("button", { name: "อัปโหลดรูปโปรไฟล์" })).toBeInTheDocument();
    expect(screen.getByText("รองรับไฟล์ PNG, JPG หรือ WebP ขนาดไม่เกิน 5MB")).toBeInTheDocument();
  });

  it("allows selecting a file and then removing it", () => {
    render(<AvatarHarness />);

    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    expect(fileInput).toBeInTheDocument();

    const testFile = new File(["dummy content"], "avatar.png", { type: "image/png" });
    fireEvent.change(fileInput, { target: { files: [testFile] } });

    expect(screen.getByRole("button", { name: "ลบ" })).toBeInTheDocument();
    expect(screen.getByText("avatar.png")).toBeInTheDocument();
    expect(screen.getByText("พร้อมบันทึก")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "ลบ" }));
    expect(screen.getByRole("button", { name: "อัปโหลดรูปโปรไฟล์" })).toBeInTheDocument();
  });

  it("displays validation error when file exceeds 5MB limit", () => {
    render(<AvatarHarness />);

    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    const largeFile = new File([new ArrayBuffer(6 * 1024 * 1024)], "large.png", { type: "image/png" });

    fireEvent.change(fileInput, { target: { files: [largeFile] } });

    expect(screen.getByRole("alert")).toBeInTheDocument();
    expect(screen.getByText("ขนาดไฟล์เกิน 5MB กรุณาเลือกไฟล์ใหม่")).toBeInTheDocument();
  });

  it("supports drag and drop to select a file", () => {
    render(<AvatarHarness />);

    const avatarBox = screen.getByRole("button", { name: "อัปโหลดรูปโปรไฟล์" });
    const droppedFile = new File(["image-bytes"], "dropped.webp", { type: "image/webp" });

    fireEvent.dragOver(avatarBox);
    expect(screen.getByText("วางรูปภาพที่นี่เพื่ออัปโหลด")).toBeInTheDocument();

    fireEvent.drop(avatarBox, {
      dataTransfer: {
        files: [droppedFile],
      },
    });

    expect(screen.getByText("dropped.webp")).toBeInTheDocument();
  });

  it("triggers file picker when clicking avatar box directly", () => {
    render(<AvatarHarness />);

    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    const clickSpy = vi.spyOn(fileInput, "click");

    const avatarBox = screen.getByRole("button", { name: "อัปโหลดรูปโปรไฟล์" });
    fireEvent.click(avatarBox);

    expect(clickSpy).toHaveBeenCalledTimes(1);
  });

  it("opens GalleryLightboxModal when clicking preview on existing avatar", () => {
    render(<AvatarHarness existingUrl="https://example.com/avatar.jpg" />);

    const previewBtn = screen.getByRole("button", { name: "ดูตัวอย่างรูปภาพ" });
    fireEvent.click(previewBtn);

    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });
});
