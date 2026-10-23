import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ItemImportModal } from "./item-import-modal";
import * as itemQueries from "../api/item-master-queries";
import { ApiError } from "@/lib/api/api-error";
import {
  COST_IMPORT_COLUMNS,
  ITEM_IMPORT_COLUMNS,
  ITEM_IMPORT_ERROR_CODES,
  buildCostImportTemplate,
  buildItemImportTemplate,
} from "../item-import-constants";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));

// Mock ItemImportJobPanel to inspect its rendering without coupling to child queries
vi.mock("./item-import-job-panel", () => ({
  ItemImportJobPanel: ({ content, kind, mode }: { content: string; kind: string; mode: string }) => (
    <div data-testid="item-import-job-panel" data-kind={kind} data-mode={mode} data-content-len={content.length}>
      Mock Job Panel
    </div>
  ),
}));

const previewMutateAsync = vi.fn();
const commitMutateAsync = vi.fn();

function mockHook() {
  vi.spyOn(itemQueries, "useItemImport").mockReturnValue({
    preview: { mutateAsync: previewMutateAsync, isPending: false },
    commit: { mutateAsync: commitMutateAsync, isPending: false },
  } as unknown as ReturnType<typeof itemQueries.useItemImport>);
}

function renderModal(
  props: { canImportItems?: boolean; canImportCosts?: boolean; canUpsert?: boolean } = {},
  onClose = vi.fn()
) {
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ItemImportModal
        isOpen
        onClose={onClose}
        canImportItems={props.canImportItems ?? true}
        canImportCosts={props.canImportCosts ?? false}
        canUpsert={props.canUpsert ?? false}
      />
    </NextIntlClientProvider>
  );
  return onClose;
}

function pickFile(content: string, size?: number) {
  const file = new File([content], "items.csv", { type: "text/csv" });
  Object.defineProperty(file, "text", { value: async () => content });
  if (typeof size === "number") {
    Object.defineProperty(file, "size", { value: size });
  }
  fireEvent.change(screen.getByTestId("item-import-file"), { target: { files: [file] } });
}

const t = thMessages.itemMaster.import;

describe("ItemImportModal", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockHook();
  });

  it("keeps the import button disabled until a file is previewed", () => {
    renderModal();

    expect((screen.getByRole("button", { name: t.commitAction.replace("{count}", "0") }) as HTMLButtonElement).disabled).toBe(true);
  });

  it("shows per-row errors, invalid rows first, and blocks commit", async () => {
    previewMutateAsync.mockResolvedValue({
      contentSha256: "hash-1",
      totalRows: 2,
      validRows: 1,
      invalidRows: 1,
      rows: [
        { rowNumber: 2, code: "A-1", nameTh: "ไม้อัด", isValid: true, errors: [] },
        { rowNumber: 3, code: "A-2", nameTh: "กาว", isValid: false, errors: [{ field: "categoryCode", code: "NOT_FOUND" }] },
      ],
    });
    renderModal();

    pickFile("csv-content");

    expect(await screen.findByText(`${t.fields.categoryCode}: ${t.errors.NOT_FOUND}`)).toBeDefined();
    expect(screen.getByText(t.summary.replace("{total}", "2").replace("{valid}", "1").replace("{invalid}", "1"))).toBeDefined();
    const bodyRows = screen.getAllByRole("row").slice(1);
    expect(bodyRows[0].textContent).toContain("A-2");
    expect((screen.getByRole("button", { name: t.commitAction.replace("{count}", "1") }) as HTMLButtonElement).disabled).toBe(true);
    expect(commitMutateAsync).not.toHaveBeenCalled();
  });

  it("commits the previewed content with its hash and one idempotency key, then closes", async () => {
    previewMutateAsync.mockResolvedValue({ contentSha256: "hash-2", totalRows: 1, validRows: 1, invalidRows: 0, rows: [{ rowNumber: 2, code: null, nameTh: "ไม้", isValid: true, errors: [] }] });
    commitMutateAsync.mockResolvedValue({ batchId: "b1", createdCount: 1, contentSha256: "hash-2", replayed: false });
    const onClose = renderModal();

    pickFile("csv-content");
    const commitButton = await screen.findByRole("button", { name: t.commitAction.replace("{count}", "1") });
    await waitFor(() => expect((commitButton as HTMLButtonElement).disabled).toBe(false));
    fireEvent.click(commitButton);

    await waitFor(() => expect(commitMutateAsync).toHaveBeenCalledTimes(1));
    const call = commitMutateAsync.mock.calls[0][0];
    expect(call.content).toBe("csv-content");
    expect(call.expectedContentSha256).toBe("hash-2");
    expect(typeof call.idempotencyKey).toBe("string");
    await waitFor(() => expect(toastMocks.success).toHaveBeenCalledWith(t.success.replace("{count}", "1")));
    expect(onClose).toHaveBeenCalled();
  });

  it("maps backend error codes to specific messages", async () => {
    previewMutateAsync.mockRejectedValue(new ApiError({ status: 422, code: "ITEM_IMPORT_FILE_INVALID", message: "bad" }));
    renderModal();

    pickFile("csv-content");

    expect(await screen.findByText(t.fileInvalid)).toBeDefined();
  });

  it("rejects files over the 10 MB background-job limit before sending anything", async () => {
    renderModal();
    const big = new File(["x"], "big.csv", { type: "text/csv" });
    Object.defineProperty(big, "size", { value: 10_000_001 });

    fireEvent.change(screen.getByTestId("item-import-file"), { target: { files: [big] } });

    expect(await screen.findByText(t.fileTooLarge)).toBeDefined();
    expect(previewMutateAsync).not.toHaveBeenCalled();
  });

  describe("G-06 Phase 2: Upsert Mode, Cost Import & Background Jobs", () => {
    it("supports upsert mode with outcome counts and success notification", async () => {
      previewMutateAsync.mockResolvedValue({
        contentSha256: "hash-upsert",
        totalRows: 3,
        validRows: 3,
        invalidRows: 0,
        createCount: 1,
        updateCount: 2,
        unchangedCount: 0,
        rows: [
          { rowNumber: 2, code: "ITEM-1", nameTh: "สินค้าใหม่", isValid: true, outcome: "created", errors: [] },
          { rowNumber: 3, code: "ITEM-2", nameTh: "สินค้าอัปเดต", isValid: true, outcome: "updated", errors: [] },
          { rowNumber: 4, code: "ITEM-3", nameTh: "สินค้าอัปเดต 2", isValid: true, outcome: "updated", errors: [] },
        ],
      });
      commitMutateAsync.mockResolvedValue({
        createdCount: 1,
        updatedCount: 2,
        unchangedCount: 0,
        contentSha256: "hash-upsert",
      });

      renderModal({ canImportItems: true, canUpsert: true });

      // Change mode to upsert
      const modeSelect = screen.getByLabelText(t.modeLabel);
      fireEvent.change(modeSelect, { target: { value: "upsert" } });
      expect(screen.getByText(t.upsertHint)).toBeDefined();

      pickFile("item-csv-upsert");

      await waitFor(() => {
        expect(previewMutateAsync).toHaveBeenCalledWith({
          content: "item-csv-upsert",
          kind: "items",
          mode: "upsert",
        });
      });

      // Shows outcome summary
      expect(screen.getByText(
        t.summaryOutcome.replace("{created}", "1").replace("{updated}", "2").replace("{unchanged}", "0")
      )).toBeDefined();

      // Commit button reflects upsert mode
      const commitBtn = screen.getByRole("button", { name: t.commitActionUpsert.replace("{count}", "3") });
      fireEvent.click(commitBtn);

      await waitFor(() => {
        expect(commitMutateAsync).toHaveBeenCalledWith(expect.objectContaining({
          kind: "items",
          mode: "upsert",
        }));
      });

      await waitFor(() => {
        expect(toastMocks.success).toHaveBeenCalledWith(
          t.successUpsert.replace("{created}", "1").replace("{updated}", "2").replace("{unchanged}", "0")
        );
      });
    });

    it("supports cost import kind creating draft costs without upsert option", async () => {
      previewMutateAsync.mockResolvedValue({
        contentSha256: "hash-costs",
        totalRows: 1,
        validRows: 1,
        invalidRows: 0,
        createCount: 1,
        updateCount: 0,
        unchangedCount: 0,
        rows: [
          { rowNumber: 2, code: "ITEM-1", nameTh: "-", isValid: true, outcome: "created", errors: [] },
        ],
      });
      commitMutateAsync.mockResolvedValue({
        createdCount: 1,
        contentSha256: "hash-costs",
      });

      renderModal({ canImportItems: true, canImportCosts: true, canUpsert: true });

      // Change kind to costs
      const kindSelect = screen.getByLabelText(t.kindLabel);
      fireEvent.change(kindSelect, { target: { value: "costs" } });

      expect(screen.getByText(t.costHint)).toBeDefined();
      // In costs kind, mode selector is either hidden or fixed to create
      expect(screen.queryByLabelText(t.modeLabel)).toBeNull();

      pickFile("cost-csv-data");

      await waitFor(() => {
        expect(previewMutateAsync).toHaveBeenCalledWith({
          content: "cost-csv-data",
          kind: "costs",
          mode: "create",
        });
      });

      const commitBtn = screen.getByRole("button", { name: t.commitActionCosts.replace("{count}", "1") });
      fireEvent.click(commitBtn);

      await waitFor(() => {
        expect(commitMutateAsync).toHaveBeenCalledWith(expect.objectContaining({
          kind: "costs",
          mode: "create",
        }));
      });

      await waitFor(() => {
        expect(toastMocks.success).toHaveBeenCalledWith(t.successCosts.replace("{count}", "1"));
      });
    });

    it("delegates files exceeding immediate preview limits to ItemImportJobPanel", async () => {
      renderModal({ canImportItems: true, canUpsert: true });

      // Simulate a file with 502 lines (> 500 rows limit)
      const lines = ["header", ...Array.from({ length: 502 }, (_, i) => `row-${i}`)];
      const largeContent = lines.join("\n");

      pickFile(largeContent);

      // Should not call synchronous preview
      expect(previewMutateAsync).not.toHaveBeenCalled();

      // Job panel should be rendered with content and selected options
      const jobPanel = await screen.findByTestId("item-import-job-panel");
      expect(jobPanel).toBeDefined();
      expect(jobPanel.getAttribute("data-kind")).toBe("items");
      expect(jobPanel.getAttribute("data-mode")).toBe("create");
    });
  });
});

describe("item import constants and translations", () => {
  it("template header lists every column in order", () => {
    expect(buildItemImportTemplate()).toBe(`${ITEM_IMPORT_COLUMNS.join(",")}\n`);
    expect(ITEM_IMPORT_COLUMNS).toHaveLength(15);
  });

  it("builds cost import template with exact expected columns", () => {
    expect(buildCostImportTemplate()).toBe(`${COST_IMPORT_COLUMNS.join(",")}\n`);
    expect(COST_IMPORT_COLUMNS).toHaveLength(13);
  });

  it("has Thai and English text for every column and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const column of [...ITEM_IMPORT_COLUMNS, ...COST_IMPORT_COLUMNS, "capabilities"]) {
        expect(messages.itemMaster.import.fields[column as keyof typeof messages.itemMaster.import.fields]).toBeTruthy();
      }
      for (const code of ITEM_IMPORT_ERROR_CODES) {
        expect(messages.itemMaster.import.errors[code]).toBeTruthy();
      }
    }
  });
});
