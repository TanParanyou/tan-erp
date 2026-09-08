import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { CustomerDetail } from "./customer-detail";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn() }),
}));

const organizationDraftCustomer = {
  id: "customer-1",
  code: "CUS-0001",
  customerType: "organization",
  displayNameTh: "บริษัท ตัวอย่าง จำกัด",
  displayNameEn: "Example Co.",
  preferredLocale: "th",
  status: "draft",
  primaryContact: {
    name: "คุณตัวอย่าง",
    roleTitle: null,
    phone: "081-***-5678",
    email: null,
    preferredChannel: "phone",
    isMasked: true,
  },
  duplicateCandidates: null,
  rowVersion: "019a3cf8-96f0-7c9f-b207-93aa818f4a20",
  createdAtUtc: "2026-09-07T12:00:00Z",
};

vi.mock("../api/customer-queries", () => ({
  useCustomerDetail: () => ({
    data: organizationDraftCustomer,
    isLoading: false,
    isError: false,
    error: null,
    refetch: vi.fn(),
  }),
}));

describe("CustomerDetail canonical labels", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  });

  it("renders organization draft with Thai labels", () => {
    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <CustomerDetail customerId="customer-1" />
        </NextIntlClientProvider>
      </QueryClientProvider>,
    );

    expect(screen.getByText("นิติบุคคล")).toBeDefined();
    expect(screen.getByText("ฉบับร่าง")).toBeDefined();
  });
});
