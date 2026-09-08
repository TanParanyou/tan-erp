import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { PageLoading } from "./PageLoading";

describe("PageLoading component", () => {
  it("renders with fallback text or custom text", () => {
    render(<PageLoading text="Fetching records..." />);
    expect(screen.getByText("Fetching records...")).toBeInTheDocument();
    expect(screen.getByRole("status")).toBeInTheDocument();
  });
});
