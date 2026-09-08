import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import { CopyButton } from "./CopyButton";

describe("CopyButton component", () => {
  beforeEach(() => {
    Object.assign(navigator, {
      clipboard: {
        writeText: vi.fn().mockResolvedValue(undefined),
      },
    });
  });

  it("copies text and reflects copied state", async () => {
    render(<CopyButton text="DOC-001" label="Copy Code" copiedLabel="Code Copied" />);

    const button = screen.getByLabelText("Copy Code");
    await act(async () => {
      fireEvent.click(button);
    });

    expect(screen.getByText("Code Copied")).toBeInTheDocument();
  });
});
