import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { PageBreadcrumbs } from "./PageBreadcrumbs";

describe("PageBreadcrumbs component", () => {
  it("renders home and nested breadcrumb links", () => {
    render(
      <PageBreadcrumbs
        items={[
          { label: "Customers", href: "/customers" },
          { label: "Customer Details", active: true },
        ]}
      />
    );

    expect(screen.getByText("หน้าหลัก")).toBeInTheDocument();
    expect(screen.getByText("Customers")).toBeInTheDocument();
    expect(screen.getByText("Customer Details")).toBeInTheDocument();
  });
});
