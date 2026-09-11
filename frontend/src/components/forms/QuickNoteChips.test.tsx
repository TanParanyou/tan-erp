import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { QuickNoteChips, type QuickTemplateItem } from "./QuickNoteChips";

describe("QuickNoteChips", () => {
  const templates: QuickTemplateItem[] = [
    { id: "1", label: "Template One" },
    { id: "2", label: "Template Two", value: "Value Two" },
  ];

  it("renders templates and invokes onSelect with appropriate value", () => {
    const onSelect = vi.fn();
    render(
      <QuickNoteChips
        label="Quick options:"
        templates={templates}
        onSelect={onSelect}
      />
    );

    expect(screen.getByText("Quick options:")).toBeInTheDocument();
    expect(screen.getByText("Template One")).toBeInTheDocument();
    expect(screen.getByText("Template Two")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Template One"));
    expect(onSelect).toHaveBeenCalledWith("Template One");

    fireEvent.click(screen.getByText("Template Two"));
    expect(onSelect).toHaveBeenCalledWith("Value Two");
  });

  it("returns null when templates array is empty", () => {
    const { container } = render(
      <QuickNoteChips templates={[]} onSelect={vi.fn()} />
    );
    expect(container.firstChild).toBeNull();
  });

  it("disables buttons when disabled prop is true", () => {
    render(
      <QuickNoteChips
        templates={templates}
        onSelect={vi.fn()}
        disabled={true}
      />
    );

    expect(screen.getByRole("button", { name: /Template One/ })).toBeDisabled();
    expect(screen.getByRole("button", { name: /Template Two/ })).toBeDisabled();
  });
});
