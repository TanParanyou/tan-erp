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

  it("renders back button with backHref and backLabel", () => {
    render(
      <PageHeader
        title="Create Customer"
        backHref="/customers"
        backLabel="Back to Customers"
      />
    );

    const backLink = screen.getByRole("link", { name: "Back to Customers" });
    expect(backLink).toBeInTheDocument();
    expect(backLink).toHaveAttribute("href", "/customers");
  });

  it("renders back button alongside breadcrumbs when both are provided", () => {
    render(
      <PageHeader
        title="Create Customer"
        breadcrumbs={[{ label: "Customers", href: "/customers" }, { label: "Create" }]}
        backHref="/customers"
        backLabel="Back to Customers"
      />
    );

    const backLinks = screen.getAllByRole("link", { name: "Back to Customers" });
    expect(backLinks.length).toBeGreaterThanOrEqual(1);
    expect(backLinks[0]).toHaveAttribute("href", "/customers");
  });

  it("renders button and triggers onBack callback when clicked", () => {
    let clicked = false;
    render(
      <PageHeader
        title="Create Customer"
        onBack={() => {
          clicked = true;
        }}
        backLabel="Back to Customers"
      />
    );

    const backButton = screen.getByRole("button", { name: "Back to Customers" });
    expect(backButton).toBeInTheDocument();
    backButton.click();
    expect(clicked).toBe(true);
  });

  it("renders onBack button alongside breadcrumbs when both are provided", () => {
    let clicked = false;
    render(
      <PageHeader
        title="Create Customer"
        breadcrumbs={[{ label: "Customers", href: "/customers" }, { label: "Create" }]}
        onBack={() => {
          clicked = true;
        }}
        backLabel="Back to Customers"
      />
    );

    const backButton = screen.getByRole("button", { name: "Back to Customers" });
    expect(backButton).toBeInTheDocument();
    backButton.click();
    expect(clicked).toBe(true);
  });
});


