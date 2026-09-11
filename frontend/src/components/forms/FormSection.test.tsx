import React from "react";
import { render, screen } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import { FormSection } from "./FormSection";

describe("FormSection", () => {
  it("renders children correctly", () => {
    render(
      <FormSection>
        <div>Content Inside</div>
      </FormSection>
    );

    expect(screen.getByText("Content Inside")).toBeInTheDocument();
  });

  it("renders title, description, and headerAction when provided", () => {
    render(
      <FormSection
        title="General Information"
        description="Fill out basic details"
        headerAction={<button type="button">Edit</button>}
      >
        <p>Section Body</p>
      </FormSection>
    );

    expect(screen.getByRole("heading", { level: 2, name: /general information/i })).toBeInTheDocument();
    expect(screen.getByText("Fill out basic details")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /edit/i })).toBeInTheDocument();
    expect(screen.getByText("Section Body")).toBeInTheDocument();
  });

  it("applies custom classNames", () => {
    const { container } = render(
      <FormSection
        title="Title"
        className="custom-root"
        headerClassName="custom-header"
        contentClassName="custom-content"
      >
        <div>Body</div>
      </FormSection>
    );

    expect(container.firstChild).toHaveClass("custom-root");
    expect(container.querySelector(".custom-header")).toBeInTheDocument();
    expect(container.querySelector(".custom-content")).toBeInTheDocument();
  });
});
