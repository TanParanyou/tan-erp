import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { FormActionBar } from "./FormActionBar";

vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

describe("FormActionBar component", () => {
  it("renders save button and triggers onSave", () => {
    const onSave = vi.fn();
    render(<FormActionBar onSave={onSave} saveButtonType="button" saveText="Save Client" />);

    const saveBtn = screen.getByRole("button", { name: "Save Client" });
    fireEvent.click(saveBtn);
    expect(onSave).toHaveBeenCalledTimes(1);
  });

  it("shows unsaved changes alert when isDirty is true", () => {
    render(<FormActionBar isDirty={true} unsavedText="Changes not saved yet" />);
    expect(screen.getByText("Changes not saved yet")).toBeInTheDocument();
  });
});
