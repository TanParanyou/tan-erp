import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { DetailNavigation } from "./DetailNavigation";

describe("DetailNavigation component", () => {
  it("renders breadcrumbs, back button, and actions", () => {
    render(
      <DetailNavigation
        breadcrumbs={[{ label: "List", href: "/list" }]}
        backHref="/list"
        backLabel="Back to Customer List"
        actions={<button>Edit</button>}
      />
    );

    expect(screen.getByText("Back to Customer List")).toBeInTheDocument();
    expect(screen.getByText("Edit")).toBeInTheDocument();
  });
});
