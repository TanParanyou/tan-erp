import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SurveyRevisionActions } from "./survey-revision-actions";
import * as surveyQueries from "../api/survey-queries";
import type { SiteSurveyResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));

const OPPORTUNITY_ID = "30000000-0000-0000-0000-000000000001";
const SURVEY_ID = "40000000-0000-0000-0000-000000000001";
const READY_ID = "50000000-0000-0000-0000-000000000001";
const DRAFT_ID = "50000000-0000-0000-0000-000000000002";

function buildSurvey(current: { id: string; status: string; revisionNumber: number }, latestReadyId: string | null): SiteSurveyResponse {
  return {
    id: SURVEY_ID,
    surveyNumber: "SRV-202610-ABC123",
    opportunityId: OPPORTUNITY_ID,
    currentRevision: {
      id: current.id,
      siteSurveyId: SURVEY_ID,
      revisionNumber: current.revisionNumber,
      status: current.status,
      readiness: current.status === "ready" ? "ready" : "incomplete",
      rowVersion: "00000000-0000-0000-0000-0000000000a1",
      createdAtUtc: "2026-10-01T04:00:00Z",
    },
    latestReadyRevision: latestReadyId ? { id: latestReadyId, revisionNumber: 1, snapshotHash: "v2:abc" } : null,
    rowVersion: "00000000-0000-0000-0000-000000000001",
    createdAtUtc: "2026-10-01T04:00:00Z",
  } as unknown as SiteSurveyResponse;
}

function renderActions(survey: SiteSurveyResponse, canClone = true, canVoid = true) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
        <SurveyRevisionActions survey={survey} opportunityId={OPPORTUNITY_ID} canClone={canClone} canVoid={canVoid} />
    </NextIntlClientProvider>
  );
}

describe("SurveyRevisionActions", () => {
  const cloneMutateAsync = vi.fn();
  const voidMutateAsync = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.spyOn(surveyQueries, "useCloneSurveyRevision").mockReturnValue({
      mutateAsync: cloneMutateAsync,
      isPending: false,
    } as unknown as ReturnType<typeof surveyQueries.useCloneSurveyRevision>);
    vi.spyOn(surveyQueries, "useVoidSurveyRevision").mockReturnValue({
      mutateAsync: voidMutateAsync,
      isPending: false,
    } as unknown as ReturnType<typeof surveyQueries.useVoidSurveyRevision>);
  });

  it("shows nothing without permissions", () => {
    const { container } = renderActions(buildSurvey({ id: READY_ID, status: "ready", revisionNumber: 1 }, READY_ID), false, false);
    expect(container.textContent).toBe("");
  });

  it("hides clone while a draft revision exists but still allows void", () => {
    renderActions(buildSurvey({ id: DRAFT_ID, status: "draft", revisionNumber: 2 }, READY_ID));
    expect(screen.queryByRole("button", { name: "สร้างฉบับแก้ไข" })).toBeNull();
    expect(screen.getByRole("button", { name: "ยกเลิกฉบับสำรวจ" })).toBeDefined();
  });

  it("requires a reason, then clones from the latest ready revision", async () => {
    cloneMutateAsync.mockResolvedValue({});
    renderActions(buildSurvey({ id: READY_ID, status: "ready", revisionNumber: 1 }, READY_ID));

    fireEvent.click(screen.getByRole("button", { name: "สร้างฉบับแก้ไข" }));
    const dialog = await screen.findByRole("dialog");
    const confirm = Array.from(dialog.querySelectorAll("button")).find((b) => b.textContent === "สร้างฉบับแก้ไข");
    expect(confirm).toBeDefined();

    fireEvent.click(confirm as HTMLButtonElement);
    expect(cloneMutateAsync).not.toHaveBeenCalled();
    expect(await screen.findByText("กรุณาระบุเหตุผล")).toBeDefined();

    fireEvent.change(screen.getByLabelText(/เหตุผลที่สร้างฉบับแก้ไข/), { target: { value: "ลูกค้าแก้แบบ" } });
    fireEvent.click(confirm as HTMLButtonElement);

    await waitFor(() => expect(cloneMutateAsync).toHaveBeenCalledTimes(1));
    expect(cloneMutateAsync.mock.calls[0][0].payload).toEqual({ sourceRevisionId: READY_ID, reason: "ลูกค้าแก้แบบ" });
    await waitFor(() => expect(toastMocks.success).toHaveBeenCalledWith(thMessages.surveys.cloneSuccess));
  });

  it("voids the current revision with its row version and reason", async () => {
    voidMutateAsync.mockResolvedValue({});
    renderActions(buildSurvey({ id: READY_ID, status: "ready", revisionNumber: 1 }, READY_ID));

    fireEvent.click(screen.getByRole("button", { name: "ยกเลิกฉบับสำรวจ" }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(screen.getByLabelText(/เหตุผลที่ยกเลิก/), { target: { value: "วัดผิดห้อง" } });
    const confirm = Array.from(dialog.querySelectorAll("button")).find((b) => b.textContent === "ยกเลิกฉบับสำรวจ");
    fireEvent.click(confirm as HTMLButtonElement);

    await waitFor(() => expect(voidMutateAsync).toHaveBeenCalledTimes(1));
    expect(voidMutateAsync.mock.calls[0][0].payload).toEqual({
      expectedRevisionVersion: "00000000-0000-0000-0000-0000000000a1",
      reason: "วัดผิดห้อง",
    });
  });

  it("offers clone from a voided revision when no ready revision remains", () => {
    renderActions(buildSurvey({ id: READY_ID, status: "void", revisionNumber: 1 }, null));
    expect(screen.getByRole("button", { name: "สร้างฉบับแก้ไข" })).toBeDefined();
    expect(screen.queryByRole("button", { name: "ยกเลิกฉบับสำรวจ" })).toBeNull();
  });

  it("maps SURVEY_DRAFT_EXISTS to a specific message", async () => {
    cloneMutateAsync.mockRejectedValue(new ApiError({ status: 409, code: "SURVEY_DRAFT_EXISTS", message: "conflict" }));
    renderActions(buildSurvey({ id: READY_ID, status: "ready", revisionNumber: 1 }, READY_ID));

    fireEvent.click(screen.getByRole("button", { name: "สร้างฉบับแก้ไข" }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(screen.getByLabelText(/เหตุผลที่สร้างฉบับแก้ไข/), { target: { value: "แก้ไข" } });
    fireEvent.click(Array.from(dialog.querySelectorAll("button")).find((b) => b.textContent === "สร้างฉบับแก้ไข") as HTMLButtonElement);

    await waitFor(() => expect(toastMocks.error).toHaveBeenCalledWith(thMessages.surveys.revisionDraftExists));
  });
});
