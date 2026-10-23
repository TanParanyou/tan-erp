import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { ItemImportJobPanel } from "./item-import-job-panel";
import * as itemQueries from "../api/item-master-queries";
import type { ItemImportJobResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";

const t = thMessages.itemMaster.import;
const startMutateAsync = vi.fn();
let mockStatusData: Partial<ItemImportJobResponse> | undefined;
let mockStatusIsError = false;

function mockQueries() {
  vi.spyOn(itemQueries, "useStartItemImportJob").mockReturnValue({
    mutateAsync: startMutateAsync,
    isPending: false,
  } as unknown as ReturnType<typeof itemQueries.useStartItemImportJob>);

  vi.spyOn(itemQueries, "useItemImportJobStatus").mockImplementation((_jobId) => {
    return {
      data: mockStatusData as ItemImportJobResponse | undefined,
      isLoading: false,
      isError: mockStatusIsError,
    } as unknown as ReturnType<typeof itemQueries.useItemImportJobStatus>;
  });
}

function renderPanel(props: { kind?: "items" | "costs"; mode?: "create" | "upsert" } = {}) {
  const describeField = (field: string | null | undefined, code: string | null | undefined) => `${field}:${code}`;
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ItemImportJobPanel
        content={"line1\nline2\nline3"}
        kind={props.kind ?? "items"}
        mode={props.mode ?? "create"}
        describeField={describeField}
      />
    </NextIntlClientProvider>
  );
}

function makeJob(overrides: Partial<ItemImportJobResponse> = {}): ItemImportJobResponse {
  return {
    id: "job-123",
    status: "running",
    totalRows: 1000,
    processedRows: 400,
    totalBatches: 10,
    completedBatches: 4,
    rejectedBatches: 0,
    createdCount: 400,
    updatedCount: 0,
    unchangedCount: 0,
    errorRowCount: 0,
    errorsTruncated: false,
    errors: [],
    failureCode: null,
    ...overrides,
  };
}

describe("ItemImportJobPanel (G-06 Background Job Progress)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockStatusData = undefined;
    mockStatusIsError = false;
    mockQueries();
  });

  it("renders start button and initiates background job with idempotency key", async () => {
    startMutateAsync.mockResolvedValue({ id: "job-created-999" });

    renderPanel({ kind: "items", mode: "create" });

    const startBtn = screen.getByRole("button", { name: t.job.start });
    fireEvent.click(startBtn);

    await waitFor(() => {
      expect(startMutateAsync).toHaveBeenCalledWith({
        content: "line1\nline2\nline3",
        kind: "items",
        mode: "create",
        idempotencyKey: expect.any(String),
      });
    });
  });

  it("shows start failure alert when mutation rejects with ApiError", async () => {
    startMutateAsync.mockRejectedValue(new ApiError({ status: 500, code: "ITEM_IMPORT_JOB_FAILED", message: "Failed" }));

    renderPanel();
    fireEvent.click(screen.getByRole("button", { name: t.job.start }));

    expect(await screen.findByText(t.job.startFailed)).toBeDefined();
  });

  it("renders progress and metrics while job is running", async () => {
    startMutateAsync.mockResolvedValue({ id: "job-123" });
    mockStatusData = makeJob({
      status: "running",
      totalRows: 1000,
      processedRows: 600,
      totalBatches: 10,
      completedBatches: 6,
      rejectedBatches: 0,
      createdCount: 500,
      updatedCount: 100,
      unchangedCount: 0,
    });

    renderPanel({ kind: "items", mode: "upsert" });
    fireEvent.click(screen.getByRole("button", { name: t.job.start }));

    expect(await screen.findByText(t.job.status.running)).toBeDefined();
    expect(screen.getByText(t.job.progress.replace("{processed}", "600").replace("{total}", "1000"))).toBeDefined();
    expect(screen.getByText(
      t.job.batches.replace("{done}", "6").replace("{total}", "10").replace("{rejected}", "0")
    )).toBeDefined();
    expect(screen.getByText(
      t.summaryOutcome.replace("{created}", "500").replace("{updated}", "100").replace("{unchanged}", "0")
    )).toBeDefined();
    expect(screen.getByText(t.job.closeHint)).toBeDefined();
  });

  it("renders completed_with_errors state with error table and rejected notice", async () => {
    startMutateAsync.mockResolvedValue({ id: "job-123" });
    mockStatusData = makeJob({
      status: "completed_with_errors",
      totalRows: 500,
      processedRows: 500,
      totalBatches: 5,
      completedBatches: 4,
      rejectedBatches: 1,
      errorRowCount: 1,
      errors: [
        {
          rowNumber: 15,
          code: "ITEM-ERR",
          errors: [{ field: "brandCode", code: "NOT_FOUND" }],
        },
      ],
    });

    renderPanel();
    fireEvent.click(screen.getByRole("button", { name: t.job.start }));

    expect(await screen.findByText(t.job.status.completed_with_errors)).toBeDefined();
    expect(screen.getByText(t.job.rejectedNotice.replace("{batches}", "1").replace("{rows}", "1"))).toBeDefined();
    expect(screen.getByText("ITEM-ERR")).toBeDefined();
    expect(screen.getByText("brandCode:NOT_FOUND")).toBeDefined();
  });

  it("renders failed state with translated failure code", async () => {
    startMutateAsync.mockResolvedValue({ id: "job-123" });
    mockStatusData = makeJob({
      status: "failed",
      failureCode: "ITEM_IMPORT_BATCH_CONFLICT",
    });

    renderPanel();
    fireEvent.click(screen.getByRole("button", { name: t.job.start }));

    expect(await screen.findByText(t.job.status.failed)).toBeDefined();
    expect(screen.getByText(t.job.failureCodes.ITEM_IMPORT_BATCH_CONFLICT)).toBeDefined();
  });

  it("shows loadFailed warning if fetching job status encounters an error", async () => {
    startMutateAsync.mockResolvedValue({ id: "job-123" });
    mockStatusIsError = true;

    renderPanel();
    fireEvent.click(screen.getByRole("button", { name: t.job.start }));

    expect(await screen.findByText(t.job.loadFailed)).toBeDefined();
  });
});
