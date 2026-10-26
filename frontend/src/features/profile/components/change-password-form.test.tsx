import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { ChangePasswordForm } from "./change-password-form";

vi.mock("@/lib/auth/auth-session", () => ({
  changeOwnPassword: vi.fn(),
}));

const mockToast = {
  success: vi.fn(),
  error: vi.fn(),
};

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({
    toast: mockToast,
  }),
}));

import { changeOwnPassword } from "@/lib/auth/auth-session";

describe("ChangePasswordForm", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders current password, new password, and confirm password fields with submit button", () => {
    render(<ChangePasswordForm />);

    expect(screen.getByLabelText(/รหัสผ่านปัจจุบัน/)).toBeDefined();
    expect(screen.getByLabelText(/^รหัสผ่านใหม่/)).toBeDefined();
    expect(screen.getByLabelText(/ยืนยันรหัสผ่านใหม่/)).toBeDefined();
    expect(screen.getByRole("button", { name: "เปลี่ยนรหัสผ่าน" })).toBeDefined();
  });

  it("shows validation error when submitted empty", async () => {
    render(<ChangePasswordForm />);

    fireEvent.click(screen.getByRole("button", { name: "เปลี่ยนรหัสผ่าน" }));

    await waitFor(() => {
      expect(screen.getByText("กรุณากรอกรหัสผ่านปัจจุบัน")).toBeDefined();
    });

    expect(changeOwnPassword).not.toHaveBeenCalled();
  });

  it("shows validation error when new password does not match confirmation", async () => {
    render(<ChangePasswordForm />);

    fireEvent.change(screen.getByLabelText(/รหัสผ่านปัจจุบัน/), { target: { value: "oldPassword123" } });
    fireEvent.change(screen.getByLabelText(/^รหัสผ่านใหม่/), { target: { value: "newPassword123" } });
    fireEvent.change(screen.getByLabelText(/ยืนยันรหัสผ่านใหม่/), { target: { value: "mismatchPassword" } });

    fireEvent.click(screen.getByRole("button", { name: "เปลี่ยนรหัสผ่าน" }));

    await waitFor(() => {
      expect(screen.getByText("รหัสผ่านยืนยันไม่ตรงกัน")).toBeDefined();
    });

    expect(changeOwnPassword).not.toHaveBeenCalled();
  });

  it("submits change password and displays success toast when valid", async () => {
    vi.mocked(changeOwnPassword).mockResolvedValueOnce();

    render(<ChangePasswordForm />);

    fireEvent.change(screen.getByLabelText(/รหัสผ่านปัจจุบัน/), { target: { value: "oldPassword123" } });
    fireEvent.change(screen.getByLabelText(/^รหัสผ่านใหม่/), { target: { value: "newPassword123" } });
    fireEvent.change(screen.getByLabelText(/ยืนยันรหัสผ่านใหม่/), { target: { value: "newPassword123" } });

    fireEvent.click(screen.getByRole("button", { name: "เปลี่ยนรหัสผ่าน" }));

    await waitFor(() => {
      expect(changeOwnPassword).toHaveBeenCalledWith("oldPassword123", "newPassword123");
      expect(mockToast.success).toHaveBeenCalledWith("เปลี่ยนรหัสผ่านเรียบร้อยแล้ว");
    });
  });

  it("shows error alert on auth failure (wrong password)", async () => {
    vi.mocked(changeOwnPassword).mockRejectedValueOnce({ code: "auth/wrong-password" });

    render(<ChangePasswordForm />);

    fireEvent.change(screen.getByLabelText(/รหัสผ่านปัจจุบัน/), { target: { value: "wrongOldPassword" } });
    fireEvent.change(screen.getByLabelText(/^รหัสผ่านใหม่/), { target: { value: "newPassword123" } });
    fireEvent.change(screen.getByLabelText(/ยืนยันรหัสผ่านใหม่/), { target: { value: "newPassword123" } });

    fireEvent.click(screen.getByRole("button", { name: "เปลี่ยนรหัสผ่าน" }));

    await waitFor(() => {
      expect(screen.getByRole("alert")).toBeDefined();
      expect(screen.getByText("รหัสผ่านปัจจุบันไม่ถูกต้อง")).toBeDefined();
    });
  });
});
