import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { LoginForm } from "./login-form";
import { ApiError } from "@/lib/api/api-error";

const mockPush = vi.fn();
vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  signInWithEmailFlow: vi.fn(),
  signInWithGoogleFlow: vi.fn(),
  verifyTwoFactorLoginFlow: vi.fn(),
}));

import {
  signInWithEmailFlow,
  verifyTwoFactorLoginFlow,
} from "@/lib/auth/auth-session";

describe("LoginForm Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders email and password inputs with visible labels and 44px submit button", () => {
    render(<LoginForm />);

    const emailLabel = screen.getByText("อีเมล");
    const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
    expect(emailLabel).toBeDefined();
    expect(emailInput).toBeDefined();

    const passwordLabel = screen.getByText("รหัสผ่าน");
    const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });
    expect(passwordLabel).toBeDefined();
    expect(passwordInput).toBeDefined();

    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });
    expect(submitButton).toBeDefined();
    expect(submitButton.style.height).toBe("44px");

    // Also check Google sign in button
    expect(screen.getByRole("button", { name: "เข้าสู่ระบบด้วย Google" })).toBeDefined();
  });

  it("shows validation error and sets aria-invalid and aria-describedby when submitted empty", async () => {
    render(<LoginForm />);

    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });
    fireEvent.click(submitButton);

    await waitFor(() => {
      const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
      const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });

      expect(emailInput.getAttribute("aria-invalid")).toBe("true");
      expect(emailInput.getAttribute("aria-describedby")).toBe("email-error");
      expect(screen.getByText("กรุณากรอกอีเมล")).toBeDefined();

      expect(passwordInput.getAttribute("aria-invalid")).toBe("true");
      expect(passwordInput.getAttribute("aria-describedby")).toBe("password-error");
      expect(screen.getByText("กรุณากรอกรหัสผ่าน")).toBeDefined();
    });

    expect(signInWithEmailFlow).not.toHaveBeenCalled();
  });

  it("shows error for invalid email format", async () => {
    render(<LoginForm />);

    const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
    const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "invalid-email" } });
    fireEvent.change(passwordInput, { target: { value: "password123" } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText("รูปแบบอีเมลไม่ถูกต้อง")).toBeDefined();
    });

    expect(signInWithEmailFlow).not.toHaveBeenCalled();
  });

  it("displays authentication error on invalid credentials failure", async () => {
    vi.mocked(signInWithEmailFlow).mockRejectedValueOnce(
      new ApiError({
        status: 401,
        code: "INVALID_CREDENTIALS",
        message: "Invalid credentials",
      })
    );

    render(<LoginForm />);

    const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
    const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "test@example.test" } });
    fireEvent.change(passwordInput, { target: { value: "wrong-pass" } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      const alert = screen.getByRole("alert");
      expect(alert.textContent).toContain("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
    });
  });

  it("sets aria-busy and disables button while submitting, preventing duplicate submit", async () => {
    let resolveLogin: () => void;
    const loginPromise = new Promise<{ status: "authenticated"; user: { uid: string; id: string; email: string; displayName: string }; accessToken: string }>((resolve) => {
      resolveLogin = () =>
        resolve({
          status: "authenticated",
          user: { uid: "u1", id: "u1", email: "test@example.test", displayName: "Test User" },
          accessToken: "token123",
        });
    });
    vi.mocked(signInWithEmailFlow).mockReturnValueOnce(loginPromise);

    render(<LoginForm />);

    const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
    const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "test@example.test" } });
    fireEvent.change(passwordInput, { target: { value: "correct-pass" } });
    fireEvent.click(submitButton);

    const form = submitButton.closest("form");
    expect(form?.getAttribute("aria-busy")).toBe("true");
    expect(submitButton.hasAttribute("disabled")).toBe(true);

    // Try second submit while busy
    fireEvent.click(submitButton);
    expect(signInWithEmailFlow).toHaveBeenCalledTimes(1);

    // Finish submission
    resolveLogin!();
    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith("/th");
    });
  });

  it("transitions to 2FA challenge step and verifies OTP code", async () => {
    vi.mocked(signInWithEmailFlow).mockResolvedValueOnce({
      status: "requires_two_factor",
      challengeToken: "chal_123",
    });

    render(<LoginForm />);

    const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
    const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "test@example.test" } });
    fireEvent.change(passwordInput, { target: { value: "correct-pass" } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText("การยืนยันตัวตนสองขั้นตอน")).toBeDefined();
    });

    // Toggle to backup code input
    const toggleBackupBtn = screen.getByRole("button", { name: "ใช้รหัสสำรองฉุกเฉิน (Emergency Backup Code)" });
    fireEvent.click(toggleBackupBtn);

    const backupCodeInput = screen.getByLabelText(/รหัสสำรองฉุกเฉิน/);
    expect(backupCodeInput).toBeDefined();

    vi.mocked(verifyTwoFactorLoginFlow).mockResolvedValueOnce({
      status: "authenticated",
      user: { uid: "u1", id: "u1", email: "test@example.test", displayName: "Test User" },
      accessToken: "token123",
    });

    fireEvent.change(backupCodeInput, { target: { value: "ABCD-EFGH" } });
    const verifyButton = screen.getByRole("button", { name: "ยืนยันรหัส" });
    fireEvent.click(verifyButton);

    await waitFor(() => {
      expect(verifyTwoFactorLoginFlow).toHaveBeenCalledWith("chal_123", "ABCD-EFGH");
      expect(mockPush).toHaveBeenCalledWith("/th");
    });
  });

  it("handles mandatory 2FA setup and backup codes confirmation step", async () => {
    vi.mocked(signInWithEmailFlow).mockResolvedValueOnce({
      status: "requires_two_factor_setup",
      challengeToken: "chal_setup_456",
      totpSetup: {
        secretKey: "SECRETKEY1234",
        otpAuthUri: "otpauth://totp/tan-erp:test?secret=SECRETKEY1234",
      },
    });

    render(<LoginForm />);

    const emailInput = screen.getByLabelText(/อีเมล/, { selector: "input" });
    const passwordInput = screen.getByLabelText(/รหัสผ่าน/, { selector: "input" });
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "test@example.test" } });
    fireEvent.change(passwordInput, { target: { value: "correct-pass" } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText("ผู้ดูแลระบบกำหนดให้ต้องเปิดใช้ 2FA")).toBeDefined();
      expect(screen.getByText("SECRETKEY1234")).toBeDefined();
    });

    // Verify OTP and transition to backup codes step
    vi.mocked(verifyTwoFactorLoginFlow).mockResolvedValueOnce({
      status: "authenticated",
      user: { uid: "u1", id: "u1", email: "test@example.test", displayName: "Test User" },
      accessToken: "token123",
      backupCodes: ["1111-2222", "3333-4444", "5555-6666"],
    });

    // Fill OTP
    const otpInputs = screen.getAllByRole("textbox");
    fireEvent.paste(otpInputs[0], {
      clipboardData: { getData: () => "123456" },
    });

    const activateBtn = screen.getByRole("button", { name: "ยืนยันและเปิดใช้งาน 2FA" });
    fireEvent.click(activateBtn);

    await waitFor(() => {
      expect(screen.getByText("รหัสสำรองฉุกเฉิน (Single-Use Backup Codes)")).toBeDefined();
      expect(screen.getByText("1111-2222")).toBeDefined();
      expect(screen.getByText("3333-4444")).toBeDefined();
    });

    const continueBtn = screen.getByRole("button", { name: "ฉันบันทึกรหัสสำรองแล้ว เข้าสู่ระบบ" });
    fireEvent.click(continueBtn);

    expect(mockPush).toHaveBeenCalledWith("/th");
  });

  it("shows warning when Google button is clicked without client ID configured", () => {
    delete process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;

    render(<LoginForm />);

    const googleBtn = screen.getByRole("button", { name: "เข้าสู่ระบบด้วย Google" });
    fireEvent.click(googleBtn);

    expect(screen.getByText("ระบบยังไม่ได้กำหนดค่า Google Client ID ในสภาพแวดล้อมนี้")).toBeDefined();
  });

  it("ensures password reveal button is accessible via keyboard and has min 44x44px target", () => {
    render(<LoginForm />);

    const reveal = screen.getByRole("button", { name: "แสดงรหัสผ่าน" });
    expect(reveal.tabIndex).toBe(0);
    expect(reveal.style.minWidth).toBe("44px");
    expect(reveal.style.minHeight).toBe("44px");
  });
});
