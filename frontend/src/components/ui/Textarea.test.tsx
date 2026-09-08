import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { Textarea } from "./Textarea";

describe("Textarea component", () => {
  it("renders with label and handles text input", () => {
    const onChange = vi.fn();
    render(<Textarea label="Notes" placeholder="Enter notes" onChange={onChange} />);

    expect(screen.getByLabelText(/Notes/)).toBeInTheDocument();
    const textarea = screen.getByPlaceholderText("Enter notes");
    fireEvent.change(textarea, { target: { value: "Custom notes" } });

    expect(onChange).toHaveBeenCalled();
  });

  it("renders error state", () => {
    render(<Textarea label="Notes" error="Notes cannot be empty" />);
    expect(screen.getByRole("alert")).toHaveTextContent("Notes cannot be empty");
  });
});
