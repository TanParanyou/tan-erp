import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityWorkImageStageFilter } from "./opportunity-work-image-stage-filter";

function renderFilter(props = {}) {
  const defaultProps = {
    selectedStage: null,
    onSelectStage: vi.fn(),
    totalCount: 5,
    stageCounts: {
      draft: 2,
      surveying: 3,
    },
    ...props,
  };

  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <OpportunityWorkImageStageFilter {...defaultProps} />
    </NextIntlClientProvider>
  );
}

describe("OpportunityWorkImageStageFilter component", () => {
  it("renders all stages and count badges correctly", () => {
    renderFilter();

    expect(screen.getByRole("button", { name: /ทุกขั้นตอน \(5\)/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /ฉบับร่าง.*\(2\)/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /สำรวจหน้างาน.*\(3\)/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /ผ่านเกณฑ์.*\(0\)/i })).toBeInTheDocument();
  });

  it("handles clicking on a stage button to filter", () => {
    const onSelectStage = vi.fn();
    renderFilter({ onSelectStage });

    const draftBtn = screen.getByRole("button", { name: /ฉบับร่าง.*\(2\)/i });
    fireEvent.click(draftBtn);

    expect(onSelectStage).toHaveBeenCalledWith("draft");
  });

  it("handles clicking on all stages button to clear filter", () => {
    const onSelectStage = vi.fn();
    renderFilter({ selectedStage: "draft", onSelectStage });

    const allBtn = screen.getByRole("button", { name: /ทุกขั้นตอน \(5\)/i });
    fireEvent.click(allBtn);

    expect(onSelectStage).toHaveBeenCalledWith(null);
  });
});
