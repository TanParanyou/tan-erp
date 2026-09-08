import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import React from "react";
import { ToastContainer } from "./Toast";
import { ToastProvider, useToast } from "@/hooks/useToast";

function ToastTrigger() {
  const { toast } = useToast();
  return (
    <button onClick={() => toast.success("Success notification", "Item updated")}>
      Show Toast
    </button>
  );
}

describe("ToastContainer component", () => {
  it("renders toast when triggered and dismisses on click", () => {
    render(
      <ToastProvider>
        <ToastTrigger />
        <ToastContainer />
      </ToastProvider>
    );

    const trigger = screen.getByText("Show Toast");
    fireEvent.click(trigger);

    expect(screen.getByText("Success notification")).toBeInTheDocument();
    expect(screen.getByText("Item updated")).toBeInTheDocument();

    const dismissBtn = screen.getByLabelText("Dismiss notification");
    fireEvent.click(dismissBtn);

    expect(screen.queryByText("Success notification")).not.toBeInTheDocument();
  });
});
