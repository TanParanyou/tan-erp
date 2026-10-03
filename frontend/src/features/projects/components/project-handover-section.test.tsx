import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ProjectHandoverSection } from "./project-handover-section";
import * as projectQueries from "../api/project-queries";
import { ApiError } from "@/lib/api/api-error";
import { PROJECT_STATUSES } from "../project-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("@/components/forms/UserAutocomplete", () => ({
  UserAutocomplete: ({ onChange, label }: { onChange: (id: string) => void; label?: string }) => (
    <button type="button" onClick={() => onChange("user-owner-1")}>{label}</button>
  ),
}));

const t = thMessages.projects;
const mutateAsync = vi.fn();

interface HandoverSourceFixture {
  quotationId: string;
  quotationNumber: string;
  quotationStatus: string;
  quotationRowVersion: string;
  contractAmount: number;
  opportunityStage: string;
  existingProjectId: string | null;
  existingProjectCode: string | null;
}

const source: HandoverSourceFixture = {
  quotationId: "q-1",
  quotationNumber: "QT-2026-0001",
  quotationStatus: "accepted",
  quotationRowVersion: "00000000-0000-0000-0000-0000000000a1",
  contractAmount: 125000.5,
  opportunityStage: "won",
  existingProjectId: null,
  existingProjectCode: null,
};

function mockSource(data: HandoverSourceFixture | null, isLoading = false) {
  vi.spyOn(projectQueries, "useProjectHandoverSource").mockReturnValue({
    data,
    isLoading,
  } as unknown as ReturnType<typeof projectQueries.useProjectHandoverSource>);
}

function renderSection(overrides: Partial<React.ComponentProps<typeof ProjectHandoverSection>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ProjectHandoverSection
        opportunityId="opp-1"
        opportunityName="งานบิลท์อิน"
        branchId="branch-1"
        isWon
        canRead
        canCreate
        {...overrides}
      />
    </NextIntlClientProvider>
  );
}

describe("ProjectHandoverSection", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.spyOn(projectQueries, "useCreateProjectFromHandover").mockReturnValue({
      mutateAsync,
      isPending: false,
    } as unknown as ReturnType<typeof projectQueries.useCreateProjectFromHandover>);
    mockSource(source);
  });

  it("renders nothing unless the opportunity is won and the user can read projects", () => {
    const { container, rerender } = renderSection({ isWon: false });
    expect(container.textContent).toBe("");

    rerender(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ProjectHandoverSection opportunityId="opp-1" opportunityName="x" isWon canRead={false} canCreate />
      </NextIntlClientProvider>
    );
    expect(container.textContent).toBe("");
  });

  it("links to the existing project instead of offering a second handover", () => {
    mockSource({ ...source, existingProjectId: "p-1", existingProjectCode: "PRJ-2026-0001" });
    renderSection();

    expect(screen.getByRole("link", { name: t.openProject.replace("{code}", "PRJ-2026-0001") }).getAttribute("href")).toBe("/th/projects/p-1");
    expect(screen.queryByRole("button", { name: t.handoverAction })).toBeNull();
  });

  it("explains the missing permission instead of showing the action", () => {
    renderSection({ canCreate: false });

    expect(screen.getByText(t.handoverPermissionRequired)).toBeDefined();
    expect(screen.queryByRole("button", { name: t.handoverAction })).toBeNull();
  });

  it("requires an owner, then submits the quotation version with one idempotency key", async () => {
    mutateAsync.mockResolvedValue({ code: "PRJ-2026-0001" });
    renderSection();

    fireEvent.click(screen.getByRole("button", { name: t.handoverAction }));
    const dialog = await screen.findByRole("dialog");
    const confirm = Array.from(dialog.querySelectorAll("button")).find((b) => b.textContent === t.handoverConfirm) as HTMLButtonElement;

    fireEvent.click(confirm);
    expect(await screen.findByText(t.ownerRequired)).toBeDefined();
    expect(mutateAsync).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: t.owner }));
    fireEvent.click(confirm);

    await waitFor(() => expect(mutateAsync).toHaveBeenCalledTimes(1));
    const call = mutateAsync.mock.calls[0][0];
    expect(call.payload).toEqual({
      quotationId: "q-1",
      expectedQuotationVersion: source.quotationRowVersion,
      ownerUserId: "user-owner-1",
      plannedStartDate: null,
      name: null,
    });
    expect(typeof call.idempotencyKey).toBe("string");
    await waitFor(() => expect(toastMocks.success).toHaveBeenCalledWith(t.handoverSuccess.replace("{code}", "PRJ-2026-0001")));
  });

  it("maps backend error codes to specific messages", async () => {
    mutateAsync.mockRejectedValue(new ApiError({ status: 409, code: "PROJECT_ALREADY_EXISTS", message: "dup" }));
    renderSection();

    fireEvent.click(screen.getByRole("button", { name: t.handoverAction }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.click(screen.getByRole("button", { name: t.owner }));
    fireEvent.click(Array.from(dialog.querySelectorAll("button")).find((b) => b.textContent === t.handoverConfirm) as HTMLButtonElement);

    expect(await screen.findByText(t.errors.alreadyExists)).toBeDefined();
  });
});

describe("project translations", () => {
  it("have Thai and English text for every status and error", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of PROJECT_STATUSES) {
        expect(messages.projects.statuses[status]).toBeTruthy();
      }
      for (const key of ["alreadyExists", "notAllowed", "versionConflict", "failed"] as const) {
        expect(messages.projects.errors[key]).toBeTruthy();
      }
      expect(messages.shell.projects).toBeTruthy();
      expect(messages.documentNumbering.types.projects).toBeTruthy();
    }
  });
});
