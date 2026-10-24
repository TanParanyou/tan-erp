import React, { useState } from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { AvatarUpload } from "./avatar-upload";

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

    fireEvent.click(screen.getByRole("button", { name: "ลบ" }));
    expect(screen.getByRole("button", { name: "อัปโหลดรูปโปรไฟล์" })).toBeInTheDocument();
  });
});
