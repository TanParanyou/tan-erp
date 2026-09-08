import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { BulkActionToolbar, BulkActionButton } from "./BulkActionToolbar";

describe("BulkActionToolbar component", () => {
  it("renders when selectedCount > 0 and handles clear", () => {
    const onClear = vi.fn();
    const onDelete = vi.fn();

    render(
      <BulkActionToolbar selectedCount={3} onClear={onClear}>
        <BulkActionButton
          icon={<span>🗑</span>}
          label="Delete All"
          onClick={onDelete}
        />
      </BulkActionToolbar>
    );

    expect(screen.getByText("3")).toBeInTheDocument();
    expect(screen.getByRole("region")).toBeInTheDocument();

    const deleteBtn = screen.getByLabelText("Delete All");
    fireEvent.click(deleteBtn);
    expect(onDelete).toHaveBeenCalledTimes(1);

    const clearBtn = screen.getByLabelText("ล้างค่าที่เลือก");
    fireEvent.click(clearBtn);
    expect(onClear).toHaveBeenCalledTimes(1);
  });

  it("returns null when selectedCount is 0", () => {
    const { container } = render(
      <BulkActionToolbar selectedCount={0} onClear={vi.fn()}>
        <div />
      </BulkActionToolbar>
    );
    expect(container).toBeEmptyDOMElement();
  });
});
