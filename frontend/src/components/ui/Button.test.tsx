import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Button } from "./Button";

describe("Button component", () => {
  it("renders children and applies variant classes", () => {
    render(<Button variant="danger">ลบข้อมูล</Button>);
    const button = screen.getByRole("button", { name: "ลบข้อมูล" });
    expect(button).toBeDefined();
    expect(button.className).toContain("erp-btn-danger");
  });

  it("handles click events when enabled", () => {
    const handleClick = vi.fn();
    render(<Button onClick={handleClick}>คลิก</Button>);
    fireEvent.click(screen.getByRole("button", { name: "คลิก" }));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it("disables click when disabled or loading", () => {
    const handleClick = vi.fn();
    const { rerender } = render(
      <Button disabled onClick={handleClick}>
        คลิก
      </Button>
    );
    const button = screen.getByRole("button", { name: "คลิก" });
    expect(button.hasAttribute("disabled")).toBe(true);
    fireEvent.click(button);
    expect(handleClick).not.toHaveBeenCalled();

    rerender(
      <Button isLoading onClick={handleClick}>
        คลิก
      </Button>
    );
    expect(button.hasAttribute("disabled")).toBe(true);
    expect(button.getAttribute("aria-busy")).toBe("true");
  });

  it("applies default size md and supports sm, lg, icon sizes", () => {
    const { rerender } = render(<Button>บันทึก</Button>);
    const button = screen.getByRole("button", { name: "บันทึก" });
    expect(button.className).toContain("erp-btn-md");

    rerender(<Button size="sm">บันทึก</Button>);
    expect(button.className).toContain("erp-btn-sm");

    rerender(<Button size="lg">บันทึก</Button>);
    expect(button.className).toContain("erp-btn-lg");

    rerender(<Button size="icon" aria-label="ค้นหา" />);
    const iconBtn = screen.getByRole("button", { name: "ค้นหา" });
    expect(iconBtn.className).toContain("erp-btn-icon");
  });
});
