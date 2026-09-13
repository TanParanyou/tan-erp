import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Drawer } from "./Drawer";

describe("Drawer component", () => {
  it("renders when isOpen is true", () => {
    render(
      <Drawer isOpen={true} onClose={vi.fn()} title="Drawer Title" description="Drawer Desc">
        <p>Drawer Content</p>
      </Drawer>
    );

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("Drawer Title")).toBeInTheDocument();
    expect(screen.getByText("Drawer Content")).toBeInTheDocument();
  });

  it("handles close button click", () => {
    const onClose = vi.fn();
    render(
      <Drawer isOpen={true} onClose={onClose} title="Drawer Title">
        <p>Drawer Content</p>
      </Drawer>
    );

    const closeBtn = screen.getByLabelText("Close drawer");
    fireEvent.click(closeBtn);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("applies noPadding layout correctly without px-5 py-4", () => {
    render(
      <Drawer isOpen={true} onClose={vi.fn()} noPadding={true} contentClassName="custom-content">
        <p>Full Bleed Content</p>
      </Drawer>
    );

    const contentElement = screen.getByText("Full Bleed Content").parentElement;
    expect(contentElement).toHaveClass("custom-content");
    expect(contentElement).toHaveClass("overflow-hidden");
    expect(contentElement).not.toHaveClass("px-5");
    expect(contentElement).not.toHaveClass("py-4");
  });

  it("hides header when showHeader is false", () => {
    render(
      <Drawer isOpen={true} onClose={vi.fn()} title="Hidden Title" showHeader={false}>
        <p>No Header Content</p>
      </Drawer>
    );

    expect(screen.queryByText("Hidden Title")).not.toBeInTheDocument();
    expect(screen.queryByLabelText("Close drawer")).not.toBeInTheDocument();
    expect(screen.getByText("No Header Content")).toBeInTheDocument();
  });
});
