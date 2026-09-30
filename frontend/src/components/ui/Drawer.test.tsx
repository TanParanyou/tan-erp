import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { Modal } from "./Modal";
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
  it("contains keyboard focus and restores it to the opener", async () => {
    const opener = document.createElement("button");
    document.body.append(opener);
    opener.focus();
    const { unmount } = render(<Drawer isOpen onClose={vi.fn()} title="Edit"><input aria-label="Name" /><button>Save</button></Drawer>);
    const close = screen.getByLabelText("Close drawer");
    await waitFor(() => expect(close).toHaveFocus());
    screen.getByRole("button", { name: "Save" }).focus();
    fireEvent.keyDown(document, { key: "Tab" });
    expect(close).toHaveFocus();
    fireEvent.keyDown(document, { key: "Tab", shiftKey: true });
    expect(screen.getByRole("button", { name: "Save" })).toHaveFocus();
    unmount();
    expect(opener).toHaveFocus();
    opener.remove();
  });

  it("locks every close route while saving", () => {
    const onClose = vi.fn();
    render(<Drawer isOpen closeDisabled onClose={onClose} title="Saving">Content</Drawer>);
    expect(screen.getByLabelText("Close drawer")).toBeDisabled();
    fireEvent.click(screen.getByLabelText("Close drawer"));
    fireEvent.click(screen.getByRole("dialog").parentElement!);
    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).not.toHaveBeenCalled();
  });

  it("lets only the top confirmation handle Escape", async () => {
    const closeDrawer = vi.fn();
    const closeModal = vi.fn();
    render(<><Drawer isOpen onClose={closeDrawer} title="Edit"><input aria-label="Name" /></Drawer><Modal isOpen onClose={closeModal} title="Discard"><button>Keep editing</button></Modal></>);
    await waitFor(() => expect(screen.getByLabelText("Close modal")).toHaveFocus());
    fireEvent.keyDown(document, { key: "Escape" });
    expect(closeModal).toHaveBeenCalledOnce();
    expect(closeDrawer).not.toHaveBeenCalled();
  });

});
