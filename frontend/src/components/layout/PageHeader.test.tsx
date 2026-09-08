import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { PageHeader } from "./PageHeader";

describe("PageHeader component", () => {
  it("renders title, subtitle, breadcrumbs, and actions", () => {
    render(
      <PageHeader
        title="Customer Directory"
        subtitle="Manage client information"
        breadcrumbs={[{ label: "Customers", href: "/customers" }]}
        actions={<button>Create Customer</button>}
      />
    );

    expect(screen.getByText("Customer Directory")).toBeInTheDocument();
    expect(screen.getByText("Manage client information")).toBeInTheDocument();
    expect(screen.getByText("Customers")).toBeInTheDocument();
    expect(screen.getByText("Create Customer")).toBeInTheDocument();
  });
});
