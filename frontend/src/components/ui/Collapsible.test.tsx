import { describe, it, expect, vi } from "vitest";
import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { Collapsible } from "./Collapsible";

describe("Collapsible Component", () => {
  it("renders title and children when open by default", () => {
    render(
      <Collapsible title="Test Section">
        <div>Content Inside</div>
      </Collapsible>
    );

    expect(screen.getByText("Test Section")).toBeDefined();
    expect(screen.getByText("Content Inside")).toBeDefined();
    const button = screen.getByRole("button", { name: /Test Section/i });
    expect(button.getAttribute("aria-expanded")).toBe("true");
  });

  it("toggles content visibility when clicking header in uncontrolled mode", () => {
    render(
      <Collapsible title="Test Section">
        <div>Content Inside</div>
      </Collapsible>
    );

    const button = screen.getByRole("button", { name: /Test Section/i });

    // Collapse
    fireEvent.click(button);
    expect(button.getAttribute("aria-expanded")).toBe("false");
    expect(screen.queryByText("Content Inside")).toBeNull();

    // Expand
    fireEvent.click(button);
    expect(button.getAttribute("aria-expanded")).toBe("true");
    expect(screen.getByText("Content Inside")).toBeDefined();
  });

  it("supports defaultOpen=false", () => {
    render(
      <Collapsible title="Test Section" defaultOpen={false}>
        <div>Content Inside</div>
      </Collapsible>
    );

    expect(screen.queryByText("Content Inside")).toBeNull();
  });

  it("supports controlled mode via isOpen and onToggle", () => {
    const handleToggle = vi.fn();
    const { rerender } = render(
      <Collapsible title="Controlled Section" isOpen={false} onToggle={handleToggle}>
        <div>Content Inside</div>
      </Collapsible>
    );

    expect(screen.queryByText("Content Inside")).toBeNull();

    const button = screen.getByRole("button", { name: /Controlled Section/i });
    fireEvent.click(button);
    expect(handleToggle).toHaveBeenCalledWith(true);

    // Parent updates prop
    rerender(
      <Collapsible title="Controlled Section" isOpen={true} onToggle={handleToggle}>
        <div>Content Inside</div>
      </Collapsible>
    );
    expect(screen.getByText("Content Inside")).toBeDefined();
  });

  it("renders actions without triggering accordion toggle", () => {
    const actionClick = vi.fn();
    render(
      <Collapsible
        title="With Action"
        actions={
          <button type="button" onClick={actionClick}>
            Action Btn
          </button>
        }
      >
        <div>Content Inside</div>
      </Collapsible>
    );

    const actionBtn = screen.getByRole("button", { name: "Action Btn" });
    fireEvent.click(actionBtn);
    expect(actionClick).toHaveBeenCalledTimes(1);

    // Content should remain open
    expect(screen.getByText("Content Inside")).toBeDefined();
  });
});
