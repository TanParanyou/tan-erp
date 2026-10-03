import { describe, expect, it } from "vitest";
import { render, screen, within } from "@testing-library/react";
import type { QuotationDocumentResponse } from "@/lib/api/api-client";
import { QuotationDocumentView } from "./quotation-document-view";

const label = (key: string) => `label.${key}`;

function buildDocument(overrides: Partial<QuotationDocumentResponse> = {}): QuotationDocumentResponse {
  return {
    number: "QT-2026-0001",
    issuedAtUtc: "2026-10-03T03:00:00Z",
    currency: "THB",
    locale: "th",
    hasIncompleteTranslations: false,
    customer: {
      customerType: "company",
      displayName: "บริษัท ตัวอย่าง",
      displayNameTh: "บริษัท ตัวอย่าง",
      displayNameEn: null,
      legalName: "บริษัท ตัวอย่าง จำกัด",
      taxIdentifier: "0105500000000",
      branchCode: "00000",
      address: {
        label: "Billing",
        addressLine1: "99 ถนนตัวอย่าง",
        subdistrict: "แขวงหนึ่ง",
        district: "เขตสอง",
        province: "กรุงเทพมหานคร",
        postalCode: "10110",
        countryCode: "TH",
      },
    },
    sections: [
      {
        code: "S1",
        name: "งานโครงสร้าง",
        nameTh: "งานโครงสร้าง",
        nameEn: null,
        subtotal: 40000,
        workItems: [
          {
            code: "W1",
            description: "ชั้นวางทีวี",
            descriptionTh: "ชั้นวางทีวี",
            descriptionEn: null,
            quantity: 2,
            unitCode: "SET",
            unitPrice: 20000,
            lineTotal: 40000,
          },
        ],
      },
    ],
    totals: {
      subtotal: 40000,
      discountType: "amount",
      discountValue: 5000,
      discountAmount: 5000,
      netBeforeTax: 35000,
      taxAmount: 2450,
      grandTotal: 37450,
    },
    ...overrides,
  };
}

describe("QuotationDocumentView", () => {
  it("renders server-provided values without recomputing totals", () => {
    render(<QuotationDocumentView document={buildDocument()} documentLocale="th" label={label} />);

    const doc = screen.getByTestId("quotation-document");
    expect(within(doc).getByText("QT-2026-0001")).toBeInTheDocument();
    expect(within(doc).getByText("ชั้นวางทีวี")).toBeInTheDocument();
    expect(within(doc).getByText("99 ถนนตัวอย่าง แขวงหนึ่ง เขตสอง กรุงเทพมหานคร 10110")).toBeInTheDocument();
    expect(within(doc).getByText("35,000.00")).toBeInTheDocument();
    expect(within(doc).getByText("2,450.00")).toBeInTheDocument();
  });

  it("shows a dash for missing values instead of guessing another field", () => {
    const doc = buildDocument({
      customer: { displayName: null, displayNameTh: "ชื่อไทย", displayNameEn: null, taxIdentifier: null },
      sections: [
        {
          code: "S1",
          name: null,
          nameTh: "ชื่อไทย",
          nameEn: null,
          subtotal: 0,
          workItems: [
            {
              code: "W1",
              description: null,
              descriptionTh: "ข้อความไทย",
              descriptionEn: null,
              quantity: 1,
              unitCode: "EA",
              unitPrice: 0,
              lineTotal: 0,
            },
          ],
        },
      ],
    });
    render(<QuotationDocumentView document={doc} documentLocale="en" label={label} />);

    expect(screen.queryByText("ข้อความไทย")).not.toBeInTheDocument();
    expect(screen.queryByText("ชื่อไทย")).not.toBeInTheDocument();
    expect(screen.getAllByText("-").length).toBeGreaterThan(0);
  });

  it("never assumes a currency when the document has none", () => {
    render(<QuotationDocumentView document={buildDocument({ currency: null })} documentLocale="th" label={label} />);

    expect(screen.queryByText(/THB/)).not.toBeInTheDocument();
    expect(screen.queryByText(/฿/)).not.toBeInTheDocument();
    expect(screen.getByText("37,450.00")).toBeInTheDocument();
  });

  it("never renders internal cost or margin labels", () => {
    render(<QuotationDocumentView document={buildDocument()} documentLocale="th" label={label} />);
    expect(screen.queryByText(/cost|margin|markup/i)).not.toBeInTheDocument();
  });
});
