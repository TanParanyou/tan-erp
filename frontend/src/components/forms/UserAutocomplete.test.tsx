import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { UserAutocomplete } from "./UserAutocomplete";
import { apiClient } from "@/lib/api/api-client";

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      listUsers: vi.fn(),
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

const mockedListUsers = vi.mocked(apiClient.listUsers);

const mockUsers = [
  {
    id: "user-1",
    displayName: "สมชาย ใจดี",
    email: "somchai@example.com",
    branchId: "branch-1",
  },
  {
    id: "user-2",
    displayName: "สมศักดิ์ รักชาติ",
    email: "somsak@example.com",
    branchId: "branch-1",
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

describe("UserAutocomplete", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    mockedListUsers.mockResolvedValue({
      items: mockUsers,
      totalCount: 2,
    });
  });

  it("renders search input initially and opens suggestions on focus", async () => {
    const handleChange = vi.fn();
    renderWithProviders(<UserAutocomplete value="" onChange={handleChange} branchId="branch-1" />, client);

    const input = screen.getByPlaceholderText(/พิมพ์ชื่อหรืออีเมลเพื่อค้นหาพนักงาน/);
    expect(input).toBeInTheDocument();

    fireEvent.focus(input);

    await waitFor(() => {
      expect(screen.getByText("สมชาย ใจดี")).toBeInTheDocument();
      expect(screen.getByText("สมศักดิ์ รักชาติ")).toBeInTheDocument();
    });
  });

  it("selects user and calls onChange with user id", async () => {
    const handleChange = vi.fn();
    renderWithProviders(
      <UserAutocomplete value="" onChange={handleChange} branchId="branch-1" />,
      client
    );

    const input = screen.getByPlaceholderText(/พิมพ์ชื่อหรืออีเมลเพื่อค้นหาพนักงาน/);
    fireEvent.focus(input);

    await waitFor(() => {
      expect(screen.getByText("สมชาย ใจดี")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByText("สมชาย ใจดี"));

    expect(handleChange).toHaveBeenCalledWith("user-1");
  });

  it("renders selected user summary card when value is provided", async () => {
    const handleChange = vi.fn();
    renderWithProviders(
      <UserAutocomplete value="user-1" onChange={handleChange} branchId="branch-1" />,
      client
    );

    await waitFor(() => {
      expect(screen.getByText("สมชาย ใจดี")).toBeInTheDocument();
      expect(screen.getByText("somchai@example.com")).toBeInTheDocument();
    });

    expect(screen.getByRole("button", { name: "เปลี่ยนพนักงาน" })).toBeInTheDocument();
  });

  it("clears selection when change user button is clicked", async () => {
    const handleChange = vi.fn();
    renderWithProviders(
      <UserAutocomplete value="user-1" onChange={handleChange} branchId="branch-1" />,
      client
    );

    await waitFor(() => {
      expect(screen.getByText("เปลี่ยนพนักงาน")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByText("เปลี่ยนพนักงาน"));

    expect(handleChange).toHaveBeenCalledWith("");
  });
});
