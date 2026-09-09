import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { CustomerList } from "./customer-list";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  usePathname: () => "/th/customers",
  useSearchParams: () => new URLSearchParams(),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-a",
      organization: { id: "org-a", name: "Org A" },
      permissions: [{ key: "customers.create", scope: "organization", scopeId: "org-a" }],
    },
  }),
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
};

const personActiveCustomer = {
  id: "customer-2",
  code: "CUS-0002",
  customerType: "person",
  displayNameTh: "นาย ตัวอย่าง",
  displayNameEn: null,
  preferredLocale: "th",
  status: "active",
  primaryContact: {
    name: "นาย ตัวอย่าง",
    roleTitle: null,
    phone: null,
    email: "exa***@example.test",
    preferredChannel: "email",
    isMasked: true,
  },
};

vi.mock("../api/customer-queries", () => ({
  useCustomerList: () => ({
    data: {
      items: [organizationDraftCustomer, personActiveCustomer],
      totalCount: 2,
      page: 1,
      pageSize: 25,
      totalPages: 1,
      nextCursor: null,
    },
    isLoading: false,
    isError: false,
    error: null,
    refetch: vi.fn(),
  }),
}));

describe("CustomerList canonical labels", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  });

  it("renders organization draft with Thai labels", () => {
    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <CustomerList />
        </NextIntlClientProvider>
      </QueryClientProvider>,
    );

    expect(screen.getAllByText("นิติบุคคล").length).toBeGreaterThan(0);
    expect(screen.getAllByText("ฉบับร่าง").length).toBeGreaterThan(0);
  });

  it("renders person active distinctly from organization draft", () => {
    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <CustomerList />
        </NextIntlClientProvider>
      </QueryClientProvider>,
    );

    expect(screen.getAllByText("บุคคลธรรมดา").length).toBeGreaterThan(0);
    expect(screen.getAllByText("ใช้งานอยู่").length).toBeGreaterThan(0);
  });
});
