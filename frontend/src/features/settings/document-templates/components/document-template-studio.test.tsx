import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { A4DocumentPreview } from "./a4-document-preview";
import { DocumentTemplateStudio } from "./document-template-studio";
import type { DocumentTemplateLayoutConfig, DocumentTemplateItem } from "../types";

const mockLayout: DocumentTemplateLayoutConfig = {
  header: {
    showLogo: true,
    logoPosition: "left",
    logoHeight: "48px",
    companyNameTh: "บริษัท แทน คอนสตรัคชั่น แอนด์ ดีไซน์ จำกัด",
    companyNameEn: "Tan Construction & Design Co., Ltd.",
    taxId: "0105560000000",
    address: "123 สุขุมวิท กรุงเทพฯ",
    phone: "02-123-4567",
    email: "info@tan-erp.com",
    documentTitle: "ใบเสนอราคา",
  },
  columns: {
    showSku: true,
    showSpecs: true,
    showThumbnail: true,
    showLineDiscount: true,
  },
  summary: {
    showVat: true,
    showWithholdingTax: true,
    showBahtText: true,
  },
  terms: {
    validityPeriodText: "30 วัน",
    commercialTermsText: "เครดิต 30 วัน",
    showBankDetails: true,
    bankName: "กสิกรไทย",
    accountNo: "123-4-56789-0",
    accountName: "บจก. แทน",
    footerNote: "หมายเหตุทดสอบ",
  },
  signatures: {
    signatureCount: 3,
    box1Label: "ผู้สั่งซื้อ / ลูกค้า",
    box2Label: "พนักงานขาย / ผู้เสนอราคา",
    box3Label: "ผู้อนุมัติ / ผู้มีอำนาจลงนาม",
  },
};

const mockTemplates: DocumentTemplateItem[] = [
  {
    id: "tmpl-1",
    organizationId: "org-1",
    branchId: null,
    documentType: "quotation",
    name: "แบบฟอร์มมาตรฐาน",
    isDefault: true,
    layoutConfigJson: JSON.stringify(mockLayout),
    logoFileId: null,
    isActive: true,
    createdAtUtc: "2026-10-24T00:00:00Z",
    updatedAtUtc: "2026-10-24T00:00:00Z",
    rowVersion: "rev-1",
  },
];

vi.mock("next-intl", () => ({
  useTranslations: () => {
    const t = (key: string, values?: Record<string, string | number>) => {
      if (values?.name) return `${key} (${values.name})`;
      return key;
    };
    return t;
  },
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "mem-1" },
  }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn().mockResolvedValue("test-token"),
}));

vi.mock("../api/document-template-queries", () => ({
  useDocumentTemplates: () => ({
    data: mockTemplates,
    isLoading: false,
    refetch: vi.fn(),
  }),
  useCreateDocumentTemplate: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
  useUpdateDocumentTemplate: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
  useDeleteDocumentTemplate: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

describe("A4DocumentPreview", () => {
  it("renders header, company details, line items, summary and 3 signature boxes", () => {
    render(<A4DocumentPreview layout={mockLayout} />);

    expect(screen.getByText("บริษัท แทน คอนสตรัคชั่น แอนด์ ดีไซน์ จำกัด")).toBeInTheDocument();
    expect(screen.getByText("Tan Construction & Design Co., Ltd.")).toBeInTheDocument();
    expect(screen.getByText("ใบเสนอราคา")).toBeInTheDocument();
    expect(screen.getByText("รหัสสินค้า")).toBeInTheDocument();
    expect(screen.getByText("WIN-ALU-001")).toBeInTheDocument();
    expect(screen.getByText("ผู้สั่งซื้อ / ลูกค้า")).toBeInTheDocument();
    expect(screen.getByText("พนักงานขาย / ผู้เสนอราคา")).toBeInTheDocument();
    expect(screen.getByText("ผู้อนุมัติ / ผู้มีอำนาจลงนาม")).toBeInTheDocument();
  });

  it("renders 2 signature boxes when signatureCount is 2", () => {
    const layout2Boxes: DocumentTemplateLayoutConfig = {
      ...mockLayout,
      signatures: {
        signatureCount: 2,
        box1Label: "ลูกค้า",
        box2Label: "ผู้ขาย",
      },
    };

    render(<A4DocumentPreview layout={layout2Boxes} />);

    expect(screen.getByText("ลูกค้า")).toBeInTheDocument();
    expect(screen.getByText("ผู้ขาย")).toBeInTheDocument();
    expect(screen.queryByText("ผู้อนุมัติ / ผู้มีอำนาจลงนาม")).not.toBeInTheDocument();
  });
});

describe("DocumentTemplateStudio", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders studio toolbar, action buttons, tabs and live preview", () => {
    render(<DocumentTemplateStudio />);

    expect(screen.getByText("title")).toBeInTheDocument();
    expect(screen.getByText("actions.save")).toBeInTheDocument();
    expect(screen.getByText("actions.reset")).toBeInTheDocument();
    expect(screen.getByText("tabs.branding")).toBeInTheDocument();
    expect(screen.getByText("tabs.columns")).toBeInTheDocument();
    expect(screen.getByText("tabs.terms")).toBeInTheDocument();
    expect(screen.getByText("tabs.signatures")).toBeInTheDocument();
    expect(screen.getByText("previewTitle")).toBeInTheDocument();
  });
});
