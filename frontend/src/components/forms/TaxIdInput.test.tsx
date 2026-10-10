import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { TaxIdInput } from "./TaxIdInput";

describe("TaxIdInput", () => {
  it("renders with placeholder and formatted default value", () => {
    render(
      <TaxIdInput
        label="Tax ID"
        id="tax-id-field"
        defaultValue="0105550000000"
        placeholder="0-0000-00000-00-0"
      />
    );

    const input = screen.getByLabelText("Tax ID") as HTMLInputElement;
    expect(input.value).toBe("0-1055-50000-00-0");
    expect(input.placeholder).toBe("0-0000-00000-00-0");
  });

  it("formats input value as user types and emits raw digits by default", () => {
    const handleValueChange = vi.fn();
    const handleChange = vi.fn();

    render(
      <TaxIdInput
        label="Tax ID"
        onValueChange={handleValueChange}
        onChange={handleChange}
      />
    );

    const input = screen.getByLabelText("Tax ID") as HTMLInputElement;
    fireEvent.change(input, { target: { value: "01055" } });

    expect(input.value).toBe("0-1055");
    expect(handleValueChange).toHaveBeenCalledWith("01055");
    expect(handleChange).toHaveBeenCalled();
  });

  it("formats full 13 digits properly", () => {
    const handleValueChange = vi.fn();

    render(
      <TaxIdInput
        label="Tax ID"
        onValueChange={handleValueChange}
      />
    );

    const input = screen.getByLabelText("Tax ID") as HTMLInputElement;
    fireEvent.change(input, { target: { value: "0105550000000" } });

    expect(input.value).toBe("0-1055-50000-00-0");
    expect(handleValueChange).toHaveBeenCalledWith("0105550000000");
  });

  it("emits formatted value when returnFormatted is true", () => {
    const handleValueChange = vi.fn();

    render(
      <TaxIdInput
        label="Tax ID"
        returnFormatted={true}
        onValueChange={handleValueChange}
      />
    );

    const input = screen.getByLabelText("Tax ID") as HTMLInputElement;
    fireEvent.change(input, { target: { value: "0105550000000" } });

    expect(handleValueChange).toHaveBeenCalledWith("0-1055-50000-00-0");
  });

  it("respects controlled value updates", () => {
    const { rerender } = render(
      <TaxIdInput label="Tax ID" value="0105550000000" />
    );

    const input = screen.getByLabelText("Tax ID") as HTMLInputElement;
    expect(input.value).toBe("0-1055-50000-00-0");

    rerender(<TaxIdInput label="Tax ID" value="1234567890123" />);
    expect(input.value).toBe("1-2345-67890-12-3");
  });
});
