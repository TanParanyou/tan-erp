import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { DocumentSequenceList } from "./document-sequence-list";
import type { useDocumentSequences } from "../api/document-sequence-queries";

let mockQueryResult: ReturnType<typeof useDocumentSequences> = {
  data: [
    {
      documentType: "estimates",
      prefix: "EST",
      formatPattern: "{PREFIX}-{YYYY}{MM}-{SEQ}",
      resetPeriod: "Monthly",
      padding: 4,
      isBranchSpecific: false,
      samplePreview: "EST-202609-0001",
      rowVersion: "AAAA==",
    },
    {
      documentType: "surveys",
      prefix: "SRV",
      formatPattern: "{PREFIX}-{BRANCH}-{YYYY}{MM}-{SEQ}",
      resetPeriod: "Monthly",
      padding: 4,
      isBranchSpecific: true,
      samplePreview: "SRV-HQ-202609-0001",
      rowVersion: "BBBB==",
    },
  ],
  isLoading: false,
  isError: false,
  error: null,
  refetch: vi.fn(),
} as unknown as ReturnType<typeof useDocumentSequences>;

vi.mock("../api/document-sequence-queries", () => ({
  useDocumentSequences: () => mockQueryResult,
  useUpdateDocumentSequence: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
  usePreviewDocumentSequence: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

function renderList(client: QueryClient) {
  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <DocumentSequenceList />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("DocumentSequenceList component", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    mockQueryResult = {
      data: [
        {
          documentType: "estimates",
          prefix: "EST",
          formatPattern: "{PREFIX}-{YYYY}{MM}-{SEQ}",
          resetPeriod: "Monthly",
          padding: 4,
          isBranchSpecific: false,
          samplePreview: "EST-202609-0001",
          rowVersion: "AAAA==",
        },
      ],
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useDocumentSequences>;
  });

  it("renders list and data rows using DataTable", () => {
    renderList(client);

    expect(screen.getByText("ตั้งค่ารูปแบบเลขที่เอกสาร (Document Numbering)")).toBeInTheDocument();
    expect(screen.getByText("EST")).toBeInTheDocument();
    expect(screen.getByText("EST-202609-0001")).toBeInTheDocument();
  });

  it("renders loading spinner state when isLoading is true", () => {
    mockQueryResult = {
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useDocumentSequences>;

    renderList(client);
    expect(screen.getByText("กำลังโหลดข้อมูล...")).toBeInTheDocument();
  });

  it("renders error message and retry button when isError is true", () => {
    mockQueryResult = {
      data: undefined,
      isLoading: false,
      isError: true,
      error: new Error("Server error"),
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useDocumentSequences>;

    renderList(client);
    expect(
      screen.getByText("ไม่สามารถโหลดข้อมูลรูปแบบเลขที่เอกสารได้ กรุณาลองใหม่อีกครั้ง")
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "ลองใหม่อีกครั้ง" })).toBeInTheDocument();
  });
});
