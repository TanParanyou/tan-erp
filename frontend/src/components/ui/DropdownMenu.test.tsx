import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { DropdownMenu } from "./DropdownMenu";

describe("DropdownMenu Component", () => {
  const items = [
    { key: "edit", label: "แก้ไขข้อมูล", onClick: vi.fn() },
    { key: "delete", label: "ลบ", onClick: vi.fn(), variant: "danger" as const, dividerAbove: true },
  ];

  it("renders trigger and opens menu on click", () => {
    render(<DropdownMenu triggerLabel="จัดการ" items={items} />);

    const triggerBtn = screen.getByRole("button", { name: /จัดการ/i });
    expect(triggerBtn).toBeDefined();

    // Menu should initially be closed
    expect(screen.queryByRole("menu")).toBeNull();

    // Click to open
    fireEvent.click(triggerBtn);
    expect(screen.getByRole("menu")).toBeDefined();
    expect(screen.getByText("แก้ไขข้อมูล")).toBeDefined();
    expect(screen.getByText("ลบ")).toBeDefined();
  });

  it("calls onClick and closes menu when an item is selected", () => {
    render(<DropdownMenu triggerLabel="จัดการ" items={items} />);

    fireEvent.click(screen.getByRole("button", { name: /จัดการ/i }));

    const editItem = screen.getByText("แก้ไขข้อมูล");
    fireEvent.click(editItem);

    expect(items[0].onClick).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("menu")).toBeNull();
  });

  it("closes when pressing Escape key", () => {
    render(<DropdownMenu triggerLabel="จัดการ" items={items} />);

    fireEvent.click(screen.getByRole("button", { name: /จัดการ/i }));
    expect(screen.getByRole("menu")).toBeDefined();

    fireEvent.keyDown(document, { key: "Escape" });
    expect(screen.queryByRole("menu")).toBeNull();
  });

  it("closes when clicking outside", () => {
    render(
      <div>
        <div data-testid="outside">Outside</div>
        <DropdownMenu triggerLabel="จัดการ" items={items} />
      </div>
    );

    fireEvent.click(screen.getByRole("button", { name: /จัดการ/i }));
    expect(screen.getByRole("menu")).toBeDefined();

    fireEvent.mouseDown(screen.getByTestId("outside"));
    expect(screen.queryByRole("menu")).toBeNull();
  });
});
