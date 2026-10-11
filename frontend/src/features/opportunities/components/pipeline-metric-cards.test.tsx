import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { PipelineMetricCards } from "./pipeline-metric-cards";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const messages: Record<string, string> = {
      totalPipelineValue: "Total Pipeline Value",
      activeDeals: "Active Deals",
      closingThisMonth: "Closing This Month",
      winRate: "Win Rate",
      wonValue: "Won Value",
      dealsUnit: "deals",
      percentUnit: "%",
    };
    return messages[key] || key;
  },
}));

describe("PipelineMetricCards", () => {
  it("renders metric cards with correct values and units", () => {
    const metrics = {
      totalPipelineValue: 1250000,
      activeDealsCount: 14,
      closingThisMonthCount: 5,
      wonValue: 450000,
      wonDealsCount: 3,
      winRatePercentage: 75,
    };

    render(<PipelineMetricCards metrics={metrics} />);

    expect(screen.getByText("Total Pipeline Value")).toBeInTheDocument();
    expect(screen.getByText("1,250,000 THB")).toBeInTheDocument();

    expect(screen.getByText("Active Deals")).toBeInTheDocument();
    expect(screen.getByText("14 deals")).toBeInTheDocument();

    expect(screen.getByText("Closing This Month")).toBeInTheDocument();
    expect(screen.getByText("5 deals")).toBeInTheDocument();

    expect(screen.getByText("Win Rate")).toBeInTheDocument();
    expect(screen.getByText("75%")).toBeInTheDocument();
    expect(screen.getByText("Won Value: 450,000 THB (3 deals)")).toBeInTheDocument();
  });

  it("handles loading state gracefully", () => {
    render(<PipelineMetricCards isLoading={true} />);

    expect(screen.getByText("Total Pipeline Value")).toBeInTheDocument();
    expect(screen.getByText("Active Deals")).toBeInTheDocument();
  });
});
