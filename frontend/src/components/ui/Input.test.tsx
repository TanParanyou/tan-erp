import React, { createRef } from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { Input } from "./Input";

// Mock next-intl
vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
}));

describe("Input component", () => {
  it("forwards ref to underlying input element", () => {
    const ref = createRef<HTMLInputElement>();
    render(<Input ref={ref} label="Username" id="user-input" />);

    expect(ref.current).not.toBeNull();
    expect(ref.current?.tagName).toBe("INPUT");
    expect(ref.current?.id).toBe("user-input");
  });

  it("sets aria-invalid and aria-describedby properly when error is passed", () => {
    render(<Input label="Tax ID" id="tax-input" error="Tax ID is required" />);

    const input = screen.getByRole("textbox");
    expect(input.getAttribute("aria-invalid")).toBe("true");
    expect(input.getAttribute("aria-describedby")).toBe("tax-input-error");

    const errorAlert = screen.getByRole("alert");
    expect(errorAlert.textContent).toBe("Tax ID is required");
    expect(errorAlert.id).toBe("tax-input-error");
  });

  it("sets aria-describedby for helperText when no error is present", () => {
    render(<Input label="Tax ID" id="tax-input" helperText="Enter 13-digit number" />);

    const input = screen.getByRole("textbox");
    expect(input.getAttribute("aria-invalid")).toBe("false");
    expect(input.getAttribute("aria-describedby")).toBe("tax-input-helper");
  });
});
