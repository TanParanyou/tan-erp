import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ApiError } from "@/lib/api/api-error";
import type { QuotationHistoryItemResponse } from "@/lib/api/api-client";
import { QuotationDocumentActions } from "./quotation-document-actions";
import * as lifecycleQueries from "../api/quotation-lifecycle-queries";

const d = thMessages.quotationLifecycle.document;
const download = vi.fn();
const withDocument = { document: { sha256: "a".repeat(64), byteCount: 100, rendererVersion: "v1", renderedAtUtc: "2026-10-12T00:00:00Z" } };

function renderActions(overrides: Partial<QuotationHistoryItemResponse>) {
  vi.spyOn(lifecycleQueries, "useQuotationDocumentDownload").mockReturnValue({ mutateAsync: download, isPending: false } as unknown as ReturnType<typeof lifecycleQueries.useQuotationDocumentDownload>);
  const item = { id: "q-1", number: "QT-2026-0001", status: "issued", ...overrides } as QuotationHistoryItemResponse;
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <QuotationDocumentActions item={item} />
    </NextIntlClientProvider>
  );
}

describe("QuotationDocumentActions", () => {
  beforeEach(() => vi.clearAllMocks());

  it("has Thai and English text for the document keys and the new error", () => {
    for (const messages of [thMessages, enMessages]) {
      expect(messages.quotationLifecycle.document.download).toBeTruthy();
      expect(messages.quotationLifecycle.document.hash).toBeTruthy();
      expect(messages.quotationLifecycle.errors.QUOTATION_DOCUMENT_NOT_AVAILABLE).toBeTruthy();
    }
  });

  it("downloads the stored PDF under the quotation number and shows the start of its hash", async () => {
    download.mockResolvedValue(undefined);
    renderActions(withDocument);
    expect(screen.getByText(d.hash.replace("{hash}", "a".repeat(16)))).toBeDefined();
    fireEvent.click(screen.getByRole("button", { name: d.download }));
    await waitFor(() => expect(download).toHaveBeenCalledWith({ quotationId: "q-1", fileName: "QT-2026-0001.pdf" }));
  });

  it("renders nothing for a quotation issued before PDFs were stored, and shows a translated error on failure", async () => {
    expect(renderActions({}).container.firstChild).toBeNull();
    download.mockRejectedValue(new ApiError({ status: 404, code: "QUOTATION_DOCUMENT_NOT_AVAILABLE", message: "raw" }));
    renderActions(withDocument);
    fireEvent.click(screen.getByRole("button", { name: d.download }));
    expect(await screen.findByText(thMessages.quotationLifecycle.errors.QUOTATION_DOCUMENT_NOT_AVAILABLE)).toBeDefined();
  });
});
