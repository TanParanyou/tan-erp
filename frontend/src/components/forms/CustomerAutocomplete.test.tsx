import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { CustomerAutocomplete } from "./CustomerAutocomplete";
import { apiClient } from "@/lib/api/api-client";

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      listCustomers: vi.fn(),
    },
  };
});

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn(async () => "test-token"),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    currentUser: null,
    selectedMembership: { id: "membership-1", branch: { id: "branch-1" } },
  }),
}));

const mockedListCustomers = vi.mocked(apiClient.listCustomers);

const mockCustomers = [
  {
    id: "cus-1",
    code: "CUS-001",
    displayNameTh: "บริษัท ทดสอบ 1 จำกัด",
    status: "active",
  },
  {
    id: "cus-2",
    code: "CUS-002",
    displayNameTh: "บริษัท ทดสอบ 2 จำกัด",
    status: "active",
  },
];

function renderWithProviders(ui: React.ReactElement, client: QueryClient) {
  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        {ui}
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("CustomerAutocomplete", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    mockedListCustomers.mockResolvedValue({
      items: mockCustomers,
      pagination: {
        page: 1,
        pageSize: 25,
        totalCount: mockCustomers.length,
        totalPages: 1,
        nextCursor: null,
      },
    });
  });

  it("renders search input initially and opens suggestions on focus", async () => {
    const handleChange = vi.fn();
    renderWithProviders(<CustomerAutocomplete value="" onChange={handleChange} />, client);

    const input = screen.getByPlaceholderText(/พิมพ์เพื่อค้นหาชื่อหรือรหัสลูกค้า/);
    expect(input).toBeInTheDocument();

    fireEvent.focus(input);

    await waitFor(() => {
      expect(screen.getByText("บริษัท ทดสอบ 1 จำกัด")).toBeInTheDocument();
      expect(screen.getByText("บริษัท ทดสอบ 2 จำกัด")).toBeInTheDocument();
    });
  });

  it("selects customer and renders selected summary card", async () => {
    const handleChange = vi.fn();
    const { rerender } = renderWithProviders(
      <CustomerAutocomplete value="" onChange={handleChange} />,
      client
    );

    const input = screen.getByPlaceholderText(/พิมพ์เพื่อค้นหาชื่อหรือรหัสลูกค้า/);
    fireEvent.focus(input);

    await waitFor(() => {
      expect(screen.getByText("บริษัท ทดสอบ 1 จำกัด")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByText("บริษัท ทดสอบ 1 จำกัด"));
    expect(handleChange).toHaveBeenCalledWith("cus-1");

    // Re-render with selected value
    rerender(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <CustomerAutocomplete value="cus-1" onChange={handleChange} />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("บริษัท ทดสอบ 1 จำกัด")).toBeInTheDocument();
    expect(screen.getByText("[CUS-001]")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "เปลี่ยนลูกค้า" })).toBeInTheDocument();
  });

  it("clears selection when Change Customer button is clicked", () => {
    const handleChange = vi.fn();
    renderWithProviders(
      <CustomerAutocomplete value="cus-1" onChange={handleChange} />,
      client
    );

    const changeBtn = screen.getByRole("button", { name: "เปลี่ยนลูกค้า" });
    fireEvent.click(changeBtn);

    expect(handleChange).toHaveBeenCalledWith("");
  });
});
