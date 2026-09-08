import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider, createTranslator } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";

let testLocale: "th" | "en" = "th";

vi.mock("next-intl", async (importOriginal) => {
  const actual = await importOriginal<typeof import("next-intl")>();
  return {
    ...actual,
    useLocale: () => testLocale,
    useTranslations: (namespace?: string) => {
      const messages = testLocale === "th" ? thMessages : enMessages;
      return createTranslator({
        locale: testLocale,
        messages,
        namespace: namespace as never,
      });
    },
  };
});
import { CustomerList } from "./customer-list";
import { CustomerDetail } from "./customer-detail";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn() }),
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

let listState: { isError: boolean; error: Error | null } = { isError: false, error: null };
let detailState: { isError: boolean; error: Error | null } = { isError: false, error: null };

vi.mock("../api/customer-queries", () => ({
  useCustomerList: () => ({
    data: listState.isError
      ? undefined
      : { pages: [{ items: [], nextCursor: null }], pageParams: [undefined] },
    isLoading: false,
    isError: listState.isError,
    error: listState.error,
    refetch: vi.fn(),
    fetchNextPage: vi.fn(),
    hasNextPage: false,
    isFetchingNextPage: false,
  }),
  useCustomerDetail: () => ({
    data: undefined,
    isLoading: false,
    isError: detailState.isError,
    error: detailState.error,
    refetch: vi.fn(),
  }),
}));

function renderList(locale: "th" | "en"): void {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale={locale} messages={locale === "th" ? thMessages : enMessages}>
        <CustomerList />
      </NextIntlClientProvider>
    </QueryClientProvider>,
  );
}

function renderDetail(locale: "th" | "en"): void {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale={locale} messages={locale === "th" ? thMessages : enMessages}>
        <CustomerDetail customerId="customer-1" />
      </NextIntlClientProvider>
    </QueryClientProvider>,
  );
}

describe("customer localization", () => {
  beforeEach(() => {
    testLocale = "th";
    listState = { isError: false, error: null };
    detailState = { isError: false, error: null };
  });

  it("translates list auth error in th and en", () => {
    listState = { isError: true, error: new Error("No authentication token available") };
    renderList("th");
    expect(screen.getByText("กรุณาเข้าสู่ระบบก่อนใช้งาน")).toBeDefined();
    expect(screen.queryByText("No authentication token available")).toBeNull();
    expect(screen.queryByText("Failed to load customers")).toBeNull();
  });

  it("translates list auth error in en", () => {
    testLocale = "en";
    listState = { isError: true, error: new Error("No authentication token available") };
    renderList("en");
    expect(screen.getByText("Please sign in to continue")).toBeDefined();
  });

  it("translates list membership error in th and en", () => {
    listState = { isError: true, error: new Error("No active membership selected") };
    renderList("th");
    expect(screen.getByText("กรุณาเลือกสมาชิกภาพที่ใช้งานอยู่")).toBeDefined();
  });

  it("translates detail load error in th and en", () => {
    detailState = { isError: true, error: new Error("No customer ID provided") };
    renderDetail("th");
    expect(screen.getByText("ไม่สามารถโหลดรายละเอียดลูกค้าได้")).toBeDefined();
    expect(screen.queryByText("Customer not found")).toBeNull();
  });
});
