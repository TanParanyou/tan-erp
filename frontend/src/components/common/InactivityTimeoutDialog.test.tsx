import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { InactivityTimeoutDialog } from "./InactivityTimeoutDialog";

describe("InactivityTimeoutDialog component", () => {
  it("renders countdown and buttons when isOpen is true", () => {
    const onStayLoggedIn = vi.fn();
    const onLogout = vi.fn();

    render(
      <InactivityTimeoutDialog
        isOpen={true}
        secondsRemaining={45}
        onStayLoggedIn={onStayLoggedIn}
        onLogout={onLogout}
      />
    );

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText(/45/)).toBeInTheDocument();

    const stayBtn = screen.getByRole("button", { name: "ใช้งานต่อ" });
    fireEvent.click(stayBtn);
    expect(onStayLoggedIn).toHaveBeenCalledTimes(1);

    const logoutBtn = screen.getByRole("button", { name: "ออกจากระบบทันที" });
    fireEvent.click(logoutBtn);
    expect(onLogout).toHaveBeenCalledTimes(1);
  });

  it("renders null when isOpen is false", () => {
    const { container } = render(
      <InactivityTimeoutDialog
        isOpen={false}
        secondsRemaining={45}
        onStayLoggedIn={vi.fn()}
        onLogout={vi.fn()}
      />
    );
    expect(container).toBeEmptyDOMElement();
  });
});
