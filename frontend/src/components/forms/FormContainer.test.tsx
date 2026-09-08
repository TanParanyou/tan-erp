import React from "react";
import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { FormContainer } from "./FormContainer";

describe("FormContainer component", () => {
  it("renders header, errorBanner, topAlert, children, and actionBar slots", () => {
    render(
      <FormContainer
        header={<header data-testid="test-header">Header Content</header>}
        errorBanner={<div data-testid="test-error">Error Alert</div>}
        topAlert={<div data-testid="test-alert">Top Alert</div>}
        actionBar={<footer data-testid="test-action-bar">Action Bar</footer>}
      >
        <div data-testid="test-children">Form Body Content</div>
      </FormContainer>
    );

    expect(screen.getByTestId("test-header")).toBeDefined();
    expect(screen.getByTestId("test-error")).toBeDefined();
    expect(screen.getByTestId("test-alert")).toBeDefined();
    expect(screen.getByTestId("test-children")).toBeDefined();
    expect(screen.getByTestId("test-action-bar")).toBeDefined();
  });

  it("applies maxWidth configuration correctly", () => {
    const { container, rerender } = render(
      <FormContainer maxWidth="lg">
        <div>Content</div>
      </FormContainer>
    );

    expect(container.querySelector(".max-w-4xl")).not.toBeNull();

    rerender(
      <FormContainer maxWidth="sm">
        <div>Content</div>
      </FormContainer>
    );
    expect(container.querySelector(".max-w-xl")).not.toBeNull();
  });

  it("renders as form when asForm is true", () => {
    const { container } = render(
      <FormContainer asForm data-testid="form-root">
        <div>Form Content</div>
      </FormContainer>
    );

    const formEl = container.querySelector("form");
    expect(formEl).not.toBeNull();
    expect(formEl?.getAttribute("data-testid")).toBe("form-root");
  });
});
