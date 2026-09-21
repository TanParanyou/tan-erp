import React, { createRef, useState } from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { PhoneInput } from "./PhoneInput";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
}));

describe("PhoneInput component", () => {
  it("renders with tel type, forwards ref, and defaults country to +66 (Thailand)", () => {
    const ref = createRef<HTMLInputElement>();
    render(<PhoneInput ref={ref} label="Phone" id="phone-test" />);

    const input = screen.getByRole("textbox");
    expect(input).toBeInTheDocument();
    expect(input.getAttribute("type")).toBe("tel");
    expect(ref.current).toBe(input);

    const countrySelect = screen.getByRole("combobox");
    expect(countrySelect).toBeInTheDocument();
    expect((countrySelect as HTMLSelectElement).value).toBe("+66");
  });

  it("renders error message and sets aria-invalid", () => {
    render(
      <PhoneInput
        label="Phone"
        id="phone-err-test"
        error="Invalid phone number"
      />
    );

    const input = screen.getByRole("textbox");
    expect(input.getAttribute("aria-invalid")).toBe("true");
    expect(screen.getByRole("alert")).toHaveTextContent("Invalid phone number");
  });

  it("disables both select and input when disabled prop is true", () => {
    render(<PhoneInput label="Phone" id="phone-disabled-test" disabled />);

    const input = screen.getByRole("textbox");
    const countrySelect = screen.getByRole("combobox");

    expect(input).toBeDisabled();
    expect(countrySelect).toBeDisabled();
  });

  it("handles user typing domestic Thai number", () => {
    const handleChange = vi.fn();
    render(
      <PhoneInput
        label="Phone"
        id="phone-type-test"
        onChange={handleChange}
      />
    );

    const input = screen.getByRole("textbox");
    fireEvent.change(input, { target: { value: "0812345678" } });

    expect(handleChange).toHaveBeenCalledTimes(1);
    const event = handleChange.mock.calls[0][0];
    expect(event.target.value).toBe("0812345678");
  });

  it("handles country select change and formats number with new dial code", () => {
    function ControlledWrapper() {
      const [value, setValue] = useState("0812345678");
      return (
        <PhoneInput
          label="Phone"
          id="phone-country-change"
          value={value}
          onChange={(e) => setValue(e.target.value)}
        />
      );
    }

    render(<ControlledWrapper />);

    const countrySelect = screen.getByRole("combobox") as HTMLSelectElement;
    expect(countrySelect.value).toBe("+66");

    // Change to Singapore +65
    fireEvent.change(countrySelect, { target: { value: "+65" } });

    expect(countrySelect.value).toBe("+65");
    const input = screen.getByRole("textbox") as HTMLInputElement;
    expect(input.value).toBe("812345678");
  });

  it("automatically detects country code when user pastes international number starting with +", () => {
    function ControlledWrapper() {
      const [value, setValue] = useState("");
      return (
        <PhoneInput
          label="Phone"
          id="phone-paste-test"
          value={value}
          onChange={(e) => setValue(e.target.value)}
        />
      );
    }

    render(<ControlledWrapper />);

    const input = screen.getByRole("textbox");
    const countrySelect = screen.getByRole("combobox") as HTMLSelectElement;
    expect(countrySelect.value).toBe("+66");

    // User pastes +1 US number
    fireEvent.change(input, { target: { value: "+1 202 555 0125" } });

    expect(countrySelect.value).toBe("+1");
  });

  it("sets maxLength based on selected country (10 for Thailand)", () => {
    render(<PhoneInput label="Phone" id="phone-maxlen-test" />);

    const input = screen.getByRole("textbox");
    expect(input.getAttribute("maxlength")).toBe("10");

    const countrySelect = screen.getByRole("combobox") as HTMLSelectElement;
    // Switch to Singapore (+65) which has maxInputLength = 8
    fireEvent.change(countrySelect, { target: { value: "+65" } });
    expect(input.getAttribute("maxlength")).toBe("8");
  });

  it("sanitizes non-numeric characters on typing", () => {
    const handleChange = vi.fn();
    render(<PhoneInput label="Phone" id="phone-sanitize-test" onChange={handleChange} />);

    const input = screen.getByRole("textbox");
    fireEvent.change(input, { target: { value: "081-234-abcd-5678" } });

    expect(handleChange).toHaveBeenCalledTimes(1);
    const event = handleChange.mock.calls[0][0];
    expect(event.target.value).toBe("081-234--5678");
  });
});
