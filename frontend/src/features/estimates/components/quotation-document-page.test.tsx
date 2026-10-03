import { describe, expect, it, vi, beforeEach } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import type { QuotationDocumentResponse } from "@/lib/api/api-client";
import { QuotationDocumentPage } from "./quotation-document-page";

const mocks = vi.hoisted(() => ({
  permissions: ["quotations.issue"] as string[],
  queryState: {} as { data?: QuotationDocumentResponse; isPending: boolean; isError: boolean },
  useQuotationDocument: vi.fn(),
}));

vi.mock("../api/estimate-queries", () => ({
  useQuotationDocument: (estimateId: string, locale: "th" | "en") => {
    mocks.useQuotationDocument(estimateId, locale);
    return mocks.queryState;
  },
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-1",
      permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })),
    },
  }),
}));

vi.mock("next-intl", () => ({
  useTranslations: (namespace: string) => (key: string) => `${namespace}.${key}`,
  createTranslator: () => (key: string) => `doc.${key}`,
}));

const documentPayload: QuotationDocumentResponse = {
  number: "QT-2026-0001",
  issuedAtUtc: "2026-10-03T03:00:00Z",
  currency: "THB",
  locale: "th",
  hasIncompleteTranslations: false,
  customer: { displayName: "ลูกค้า" },
  sections: [],
  totals: { subtotal: 0, discountAmount: 0, netBeforeTax: 0, taxAmount: 0, grandTotal: 0 },
};

describe("QuotationDocumentPage", () => {
  beforeEach(() => {
    mocks.permissions = ["quotations.issue"];
    mocks.queryState = { data: documentPayload, isPending: false, isError: false };
    mocks.useQuotationDocument.mockClear();
    vi.stubGlobal("print", vi.fn());
  });

  it("blocks users without the quotation permission", () => {
    mocks.permissions = [];
    render(<QuotationDocumentPage estimateId="estimate-1" uiLocale="th" />);
    expect(screen.getByText("quotationDocument.noPermission")).toBeInTheDocument();
    expect(screen.queryByTestId("quotation-document")).not.toBeInTheDocument();
  });

  it("prints the document and switches the document language", () => {
    const printSpy = vi.spyOn(window, "print").mockImplementation(() => undefined);
    render(<QuotationDocumentPage estimateId="estimate-1" uiLocale="th" />);

    fireEvent.click(screen.getByRole("button", { name: "quotationDocument.print" }));
    expect(printSpy).toHaveBeenCalledTimes(1);

    fireEvent.click(screen.getByRole("button", { name: "quotationDocument.languageEn" }));
    expect(mocks.useQuotationDocument).toHaveBeenLastCalledWith("estimate-1", "en");
  });

  it("warns when translations are incomplete and shows load errors", () => {
    mocks.queryState = { data: { ...documentPayload, hasIncompleteTranslations: true }, isPending: false, isError: false };
    const { rerender } = render(<QuotationDocumentPage estimateId="estimate-1" uiLocale="en" />);
    expect(screen.getByText("quotationDocument.incompleteTranslations")).toBeInTheDocument();

    mocks.queryState = { data: undefined, isPending: false, isError: true };
    rerender(<QuotationDocumentPage estimateId="estimate-1" uiLocale="en" />);
    expect(screen.getByText("quotationDocument.loadFailed")).toBeInTheDocument();
  });
});
