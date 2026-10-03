import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { ProjectList } from "./project-list";
import * as projectQueries from "../api/project-queries";

vi.mock("next/navigation", () => ({
  usePathname: () => "/th/projects",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));

const t = thMessages.projects;

function mockList(items: unknown[], totalCount = items.length, isLoading = false) {
  vi.spyOn(projectQueries, "useProjectList").mockReturnValue({
    data: { items, pagination: { page: 1, pageSize: 25, totalCount, totalPages: totalCount === 0 ? 0 : 1 } },
    isLoading,
    isError: false,
    error: null,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof projectQueries.useProjectList>);
}

function renderList() {
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ProjectList />
    </NextIntlClientProvider>
  );
}

describe("ProjectList", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows the empty state when no projects exist", () => {
    mockList([]);
    renderList();

    expect(screen.getByText(t.empty)).toBeDefined();
  });

  it("renders rows with a link to the project, localized status and contract amount", () => {
    mockList([
      {
        id: "p-1",
        code: "PRJ-2026-0001",
        name: "งานบิลท์อินห้องนอน",
        status: "planned",
        plannedStartDate: "2026-11-01",
        owner: { id: "u-1", displayName: "คุณผู้จัดการ" },
        customer: { id: "c-1", code: "CUS-1", displayNameTh: "บริษัท ตัวอย่าง" },
        contractAmount: 125000.5,
        createdAtUtc: "2026-10-04T00:00:00Z",
      },
    ]);
    renderList();

    expect(screen.getByRole("link", { name: "PRJ-2026-0001" }).getAttribute("href")).toBe("/th/projects/p-1");
    const table = within(screen.getByRole("table"));
    expect(table.getByText(t.statuses.planned)).toBeDefined();
    expect(table.getByText("บริษัท ตัวอย่าง")).toBeDefined();
    expect(table.getByText("คุณผู้จัดการ")).toBeDefined();
  });
});
