import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Tabs } from "./Tabs";

describe("Tabs component", () => {
  const tabs = [
    { id: "general", label: "General Information" },
    { id: "address", label: "Address & Branch" },
  ];

  it("renders tab items and handles tab switching", () => {
    const setActiveTab = vi.fn();
    render(
      <Tabs tabs={tabs} activeTab="general" setActiveTab={setActiveTab} />
    );

    expect(screen.getByRole("tab", { name: "General Information" })).toHaveAttribute(
      "aria-selected",
      "true"
    );

    const addressTab = screen.getByRole("tab", { name: "Address & Branch" });
    expect(addressTab).toHaveAttribute("aria-selected", "false");

    fireEvent.click(addressTab);
    expect(setActiveTab).toHaveBeenCalledWith("address");
  });
});
