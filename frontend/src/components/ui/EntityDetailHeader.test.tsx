import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { EntityDetailHeader } from "./EntityDetailHeader";

describe("EntityDetailHeader component", () => {
  it("renders title, code, subtitle and back link", () => {
    render(
      <EntityDetailHeader
        title="Acme Corporation"
        subtitle="Global Interior Solutions"
        code="CUS-001"
        backLabel="Back to list"
        backHref="/en/customers"
      />
    );

    expect(screen.getByText("Acme Corporation")).toBeInTheDocument();
    expect(screen.getByText("Global Interior Solutions")).toBeInTheDocument();
    expect(screen.getByText("CUS-001")).toBeInTheDocument();
    expect(screen.getByText("Back to list")).toBeInTheDocument();
  });

  it("renders metrics with financial formatting", () => {
    render(
      <EntityDetailHeader
        title="Office Interior Project"
        metrics={[
          { label: "Budget", value: "฿1,500,000", isFinancial: true },
          { label: "Owner", value: "Tan Paranyou" },
        ]}
      />
    );

    expect(screen.getByText("Budget")).toBeInTheDocument();
    expect(screen.getByText("฿1,500,000")).toBeInTheDocument();
    expect(screen.getByText("Owner")).toBeInTheDocument();
    expect(screen.getByText("Tan Paranyou")).toBeInTheDocument();
  });

  it("renders actions toolbar", () => {
    render(
      <EntityDetailHeader
        title="Project X"
        actions={<button type="button">Activate</button>}
      />
    );

    expect(screen.getByRole("button", { name: "Activate" })).toBeInTheDocument();
  });

  it("renders copy button next to code and handles copying", async () => {
    const writeTextMock = vi.fn().mockResolvedValue(undefined);
    Object.assign(navigator, {
      clipboard: {
        writeText: writeTextMock,
      },
    });

    render(
      <EntityDetailHeader
        title="Project X"
        code="CUS-2026-001"
        copyCodeLabel="Copy code"
        copiedLabel="Copied"
      />
    );

    const copyBtn = screen.getByRole("button", { name: "Copy code" });
    expect(copyBtn).toBeInTheDocument();

    const { fireEvent, act } = await import("@testing-library/react");
    await act(async () => {
      fireEvent.click(copyBtn);
    });

    expect(writeTextMock).toHaveBeenCalledWith("CUS-2026-001");
  });

  it("does not render copy button when enableCopyCode is false", () => {
    render(
      <EntityDetailHeader
        title="Project X"
        code="CUS-2026-001"
        enableCopyCode={false}
        copyCodeLabel="Copy code"
      />
    );

    expect(screen.queryByRole("button", { name: "Copy code" })).not.toBeInTheDocument();
  });
});
