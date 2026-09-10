import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { SelectWithOther } from "./SelectWithOther";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
}));

describe("SelectWithOther component", () => {
  const options = [
    { value: "walk_in", label: "Walk In" },
    { value: "other", label: "Other" },
  ];

  it("does not render other input when value is not other", () => {
    render(
      <SelectWithOther
        selectProps={{
          id: "test-select",
          label: "Source",
          options,
          value: "walk_in",
          onChange: vi.fn(),
        }}
        otherProps={{
          id: "test-other",
          label: "Other note",
          value: "",
          onChange: vi.fn(),
        }}
      />
    );

    expect(screen.getByLabelText("Source")).toBeInTheDocument();
    expect(screen.queryByLabelText(/Other note/i)).not.toBeInTheDocument();
  });

  it("renders other input when trigger value is selected", () => {
    render(
      <SelectWithOther
        selectProps={{
          id: "test-select",
          label: "Source",
          options,
          value: "other",
          onChange: vi.fn(),
        }}
        otherProps={{
          id: "test-other",
          label: "Other note",
          value: "Exhibition",
          onChange: vi.fn(),
        }}
      />
    );

    expect(screen.getByLabelText("Source")).toBeInTheDocument();
    expect(screen.getByLabelText(/Other note/i)).toBeInTheDocument();
  });
});
