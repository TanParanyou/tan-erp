import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { ExportDropdown } from "./ExportDropdown";

// Mock next-intl useTranslations
vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const translations: Record<string, string> = {
      export: "ส่งออก",
      exportCsv: "ส่งออกเป็น CSV (.csv)",
      exportExcel: "ส่งออกเป็น Excel (.xlsx)",
    };
    return translations[key] ?? key;
  },
}));

describe("ExportDropdown component", () => {
  it("renders export button with default label and toggles menu on click", () => {
    const onExport = vi.fn();
    render(<ExportDropdown onExport={onExport} />);

    const button = screen.getByRole("button", { name: "ส่งออก" });
    expect(button).toBeInTheDocument();
    expect(screen.queryByRole("menu")).not.toBeInTheDocument();

    fireEvent.click(button);
    expect(screen.getByRole("menu")).toBeInTheDocument();
    expect(screen.getByText("ส่งออกเป็น CSV (.csv)")).toBeInTheDocument();
    expect(screen.getByText("ส่งออกเป็น Excel (.xlsx)")).toBeInTheDocument();
  });

  it("calls onExport('csv') when CSV option is clicked", () => {
    const onExport = vi.fn();
    render(<ExportDropdown onExport={onExport} />);

    fireEvent.click(screen.getByRole("button", { name: "ส่งออก" }));
    fireEvent.click(screen.getByText("ส่งออกเป็น CSV (.csv)"));

    expect(onExport).toHaveBeenCalledWith("csv");
    expect(screen.queryByRole("menu")).not.toBeInTheDocument();
  });

  it("calls onExport('xlsx') when Excel option is clicked", () => {
    const onExport = vi.fn();
    render(<ExportDropdown onExport={onExport} />);

    fireEvent.click(screen.getByRole("button", { name: "ส่งออก" }));
    fireEvent.click(screen.getByText("ส่งออกเป็น Excel (.xlsx)"));

    expect(onExport).toHaveBeenCalledWith("xlsx");
    expect(screen.queryByRole("menu")).not.toBeInTheDocument();
  });

  it("disables trigger when disabled or isLoading is true", () => {
    const onExport = vi.fn();
    const { rerender } = render(<ExportDropdown onExport={onExport} disabled={true} />);

    const button = screen.getByRole("button", { name: "ส่งออก" });
    expect(button).toBeDisabled();

    fireEvent.click(button);
    expect(screen.queryByRole("menu")).not.toBeInTheDocument();

    rerender(<ExportDropdown onExport={onExport} isLoading={true} />);
    expect(button).toBeDisabled();
  });

  it("closes dropdown on Escape key", () => {
    const onExport = vi.fn();
    render(<ExportDropdown onExport={onExport} />);

    fireEvent.click(screen.getByRole("button", { name: "ส่งออก" }));
    expect(screen.getByRole("menu")).toBeInTheDocument();

    fireEvent.keyDown(document, { key: "Escape" });
    expect(screen.queryByRole("menu")).not.toBeInTheDocument();
  });
});
