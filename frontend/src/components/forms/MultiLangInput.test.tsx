import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { MultiLangInput } from "./MultiLangInput";

describe("MultiLangInput component", () => {
  it("renders with label and switches between TH and EN tabs", () => {
    const onChange = vi.fn();
    render(
      <MultiLangInput
        label="Product Name"
        value={{ th: "สินค้า ก", en: "Product A" }}
        onChange={onChange}
      />
    );

    expect(screen.getByLabelText(/Product Name/)).toBeInTheDocument();
    expect(screen.getByDisplayValue("สินค้า ก")).toBeInTheDocument();

    const enTab = screen.getByRole("button", { name: "EN" });
    fireEvent.click(enTab);

    expect(screen.getByDisplayValue("Product A")).toBeInTheDocument();
  });

  it("handles copy from TH to EN", () => {
    const onChange = vi.fn();
    render(
      <MultiLangInput
        label="Product Name"
        value={{ th: "สินค้า ก", en: "" }}
        onChange={onChange}
      />
    );

    const copyBtn = screen.getByLabelText("Copy TH to EN");
    fireEvent.click(copyBtn);

    expect(onChange).toHaveBeenCalledWith({
      th: "สินค้า ก",
      en: "สินค้า ก",
    });
  });
});
