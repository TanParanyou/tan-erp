import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ItemImportModal } from "./item-import-modal";
import * as itemQueries from "../api/item-master-queries";
import { ApiError } from "@/lib/api/api-error";
import { ITEM_IMPORT_COLUMNS, ITEM_IMPORT_ERROR_CODES, buildItemImportTemplate } from "../item-import-constants";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));

const previewMutateAsync = vi.fn();
const commitMutateAsync = vi.fn();

function mockHook() {
  vi.spyOn(itemQueries, "useItemImport").mockReturnValue({
    preview: { mutateAsync: previewMutateAsync, isPending: false },
    commit: { mutateAsync: commitMutateAsync, isPending: false },
  } as unknown as ReturnType<typeof itemQueries.useItemImport>);
}

function renderModal(onClose = vi.fn()) {
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ItemImportModal isOpen onClose={onClose} />
    </NextIntlClientProvider>
  );
  return onClose;
}

function pickFile(content: string) {
  const file = new File([content], "items.csv", { type: "text/csv" });
  Object.defineProperty(file, "text", { value: async () => content });
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

  it("rejects files over 1 MB before sending anything", async () => {
    renderModal();
    const big = new File(["x"], "big.csv", { type: "text/csv" });
    Object.defineProperty(big, "size", { value: 1_000_001 });

    fireEvent.change(screen.getByTestId("item-import-file"), { target: { files: [big] } });

    expect(await screen.findByText(t.fileTooLarge)).toBeDefined();
    expect(previewMutateAsync).not.toHaveBeenCalled();
  });
});

describe("item import constants and translations", () => {
  it("template header lists every column in order", () => {
    expect(buildItemImportTemplate()).toBe(`${ITEM_IMPORT_COLUMNS.join(",")}\n`);
    expect(ITEM_IMPORT_COLUMNS).toHaveLength(15);
  });

  it("has Thai and English text for every column and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const column of [...ITEM_IMPORT_COLUMNS, "capabilities"]) {
        expect(messages.itemMaster.import.fields[column as keyof typeof messages.itemMaster.import.fields]).toBeTruthy();
      }
      for (const code of ITEM_IMPORT_ERROR_CODES) {
        expect(messages.itemMaster.import.errors[code]).toBeTruthy();
      }
    }
  });
});
