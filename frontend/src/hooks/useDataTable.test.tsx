import { describe, it, expect } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import React from "react";
import { useDataTable } from "./useDataTable";

describe("useDataTable", () => {
  const queryClient = new QueryClient();
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );

  const sampleData = [
    { id: 1, name: "Alpha", role: "Manager" },
    { id: 2, name: "Beta", role: "Developer" },
    { id: 3, name: "Gamma", role: "Designer" },
  ];

  it("handles client-side sorting and pagination", () => {
    const { result } = renderHook(
      () => useDataTable({ data: sampleData, initialLimit: 2 }),
      { wrapper }
    );

    expect(result.current.data.length).toBe(2);
    expect(result.current.pagination.totalPages).toBe(2);

    act(() => {
      result.current.onSort("name");
    });
    expect(result.current.sort.key).toBe("name");
    expect(result.current.sort.order).toBe("asc");

    act(() => {
      result.current.onPageChange(2);
    });
    expect(result.current.pagination.page).toBe(2);
  });

  it("filters data on search", () => {
    const { result } = renderHook(
      () => useDataTable({ data: sampleData }),
      { wrapper }
    );

    act(() => {
      result.current.onSearch("Alpha");
    });
    expect(result.current.data.length).toBe(1);
    expect(result.current.data[0].name).toBe("Alpha");
  });
});
