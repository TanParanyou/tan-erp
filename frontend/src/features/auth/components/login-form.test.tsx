import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { LoginForm } from "./login-form";

vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  signInWithEmail: vi.fn(),
}));

import { signInWithEmail } from "@/lib/auth/auth-session";

describe("LoginForm Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders email and password inputs with visible labels and 44px submit button", () => {
    render(<LoginForm locale="th" />);

    const emailLabel = screen.getByText("อีเมล");
    const emailInput = screen.getByLabelText("อีเมล");
    expect(emailLabel).toBeDefined();
    expect(emailInput).toBeDefined();

    const passwordLabel = screen.getByText("รหัสผ่าน");
    const passwordInput = screen.getByLabelText("รหัสผ่าน");
    expect(passwordLabel).toBeDefined();
    expect(passwordInput).toBeDefined();

    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });
    expect(submitButton).toBeDefined();
    expect(submitButton.style.height).toBe("44px");
  });

  it("shows validation error and sets aria-invalid and aria-describedby when submitted empty", async () => {
    render(<LoginForm locale="th" />);

    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });
    fireEvent.click(submitButton);

    await waitFor(() => {
      const emailInput = screen.getByLabelText("อีเมล");
      const passwordInput = screen.getByLabelText("รหัสผ่าน");

      expect(emailInput.getAttribute("aria-invalid")).toBe("true");
      expect(emailInput.getAttribute("aria-describedby")).toBe("email-error");
      expect(screen.getByText("กรุณากรอกอีเมล")).toBeDefined();

      expect(passwordInput.getAttribute("aria-invalid")).toBe("true");
      expect(passwordInput.getAttribute("aria-describedby")).toBe("password-error");
      expect(screen.getByText("กรุณากรอกรหัสผ่าน")).toBeDefined();
    });

    expect(signInWithEmail).not.toHaveBeenCalled();
  });

  it("shows error for invalid email format", async () => {
    render(<LoginForm locale="en" />);

    const emailInput = screen.getByLabelText("Email");
    const passwordInput = screen.getByLabelText("Password");
    const submitButton = screen.getByRole("button", { name: "Sign In" });

    fireEvent.change(emailInput, { target: { value: "invalid-email" } });
    fireEvent.change(passwordInput, { target: { value: "password123" } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText("Invalid email format")).toBeDefined();
    });

    expect(signInWithEmail).not.toHaveBeenCalled();
  });

  it("displays authentication error inside aria-live polite region on login failure", async () => {
    vi.mocked(signInWithEmail).mockRejectedValueOnce({
      code: "auth/invalid-credential",
      message: "Invalid credentials",
    });

    render(<LoginForm locale="th" />);

    const emailInput = screen.getByLabelText("อีเมล");
    const passwordInput = screen.getByLabelText("รหัสผ่าน");
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "test@example.test" } });
    fireEvent.change(passwordInput, { target: { value: "wrong-pass" } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      const alert = screen.getByRole("alert");
      expect(alert.getAttribute("aria-live")).toBe("polite");
      expect(alert.textContent).toContain("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
    });
  });

  it("sets aria-busy and disables button while submitting, preventing duplicate submit", async () => {
    let resolveLogin: () => void;
    const loginPromise = new Promise<never>((resolve) => {
      resolveLogin = () => resolve({} as never);
    });
    vi.mocked(signInWithEmail).mockReturnValueOnce(loginPromise);

    render(<LoginForm locale="th" />);

    const emailInput = screen.getByLabelText("อีเมล");
    const passwordInput = screen.getByLabelText("รหัสผ่าน");
    const submitButton = screen.getByRole("button", { name: "เข้าสู่ระบบ" });

    fireEvent.change(emailInput, { target: { value: "test@example.test" } });
    fireEvent.change(passwordInput, { target: { value: "correct-pass" } });
    fireEvent.click(submitButton);

    const form = submitButton.closest("form");
    expect(form?.getAttribute("aria-busy")).toBe("true");
    expect(submitButton.hasAttribute("disabled")).toBe(true);

    // Try second submit while busy
    fireEvent.click(submitButton);
    expect(signInWithEmail).toHaveBeenCalledTimes(1);

    // Finish submission
    resolveLogin!();
    await waitFor(() => {
      expect(form?.getAttribute("aria-busy")).toBe("false");
    });
  });
});
