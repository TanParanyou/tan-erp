import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { DataTable, type Column } from "./DataTable";

describe("DataTable component", () => {
  interface Item {
    id: number;
    code: string;
    name: string;
  }

  const columns: Column<Item>[] = [
    { header: "Code", accessorKey: "code", sortable: true },
    { header: "Name", accessorKey: "name" },
  ];

  const data: Item[] = [
    { id: 1, code: "C001", name: "Alpha" },
    { id: 2, code: "C002", name: "Beta" },
  ];

  it("renders table headers and rows", () => {
    render(<DataTable columns={columns} data={data} />);

    expect(screen.getByText("Code")).toBeInTheDocument();
    expect(screen.getByText("Name")).toBeInTheDocument();
    expect(screen.getByText("C001")).toBeInTheDocument();
    expect(screen.getByText("Beta")).toBeInTheDocument();
  });

  it("handles sorting trigger on click", () => {
    const onSort = vi.fn();
    render(<DataTable columns={columns} data={data} onSort={onSort} />);

    const codeHeader = screen.getByRole("button", { name: /code/i });
    expect(codeHeader).toHaveAttribute("aria-sort", "none");
    fireEvent.click(codeHeader);
    expect(onSort).toHaveBeenCalledWith("code");
  });

  it("handles sorting trigger via keyboard Enter and Space", () => {
    const onSort = vi.fn();
    render(<DataTable columns={columns} data={data} onSort={onSort} />);

    const codeHeader = screen.getByRole("button", { name: /code/i });
    fireEvent.keyDown(codeHeader, { key: "Enter" });
    expect(onSort).toHaveBeenCalledWith("code");

    fireEvent.keyDown(codeHeader, { key: " " });
    expect(onSort).toHaveBeenCalledTimes(2);
  });

  it("reflects active sort state with aria-sort and custom styling", () => {
    const { rerender } = render(
      <DataTable
        columns={columns}
        data={data}
        onSort={vi.fn()}
        sorting={{ key: "code", order: "asc" }}
      />
    );

    const codeHeaderAsc = screen.getByRole("button", { name: /code/i });
    expect(codeHeaderAsc).toHaveAttribute("aria-sort", "ascending");

    rerender(
      <DataTable
        columns={columns}
        data={data}
        onSort={vi.fn()}
        sorting={{ key: "code", order: "desc" }}
      />
    );

    const codeHeaderDesc = screen.getByRole("button", { name: /code/i });
    expect(codeHeaderDesc).toHaveAttribute("aria-sort", "descending");
  });

  it("handles selectable row clicks", () => {
    const onSelect = vi.fn();
    const selectedIds = new Set<string | number>([1]);

    render(
      <DataTable
        columns={columns}
        data={data}
        selectable
        selectedIds={selectedIds}
        onSelect={onSelect}
      />
    );

    const checkboxes = screen.getAllByRole("checkbox");
    // [0] is select all, [1] is row 1 (checked), [2] is row 2
    expect(checkboxes[1]).toBeChecked();
    fireEvent.click(checkboxes[2]);
    expect(onSelect).toHaveBeenCalledWith(2);
  });

  it("renders error state within table frame and triggers onRetry", () => {
    const onRetry = vi.fn();
    render(
      <DataTable
        columns={columns}
        data={[]}
        isError={true}
        error="Network timeout occurred"
        onRetry={onRetry}
      />
    );

    // Headers are still preserved
    expect(screen.getByText("Code")).toBeInTheDocument();
    expect(screen.getByText("Name")).toBeInTheDocument();

    // Error message and retry button are shown
    expect(screen.getByText("Network timeout occurred")).toBeInTheDocument();
    const retryBtn = screen.getByRole("button", { name: /ลองใหม่อีกครั้ง|retry/i });
    expect(retryBtn).toBeInTheDocument();
    fireEvent.click(retryBtn);
    expect(onRetry).toHaveBeenCalledTimes(1);
  });

  it("renders custom emptyTitle and emptyDescription within table frame", () => {
    render(
      <DataTable
        columns={columns}
        data={[]}
        emptyTitle="No records found for filter"
        emptyDescription="Try broadening your search criteria"
      />
    );

    // Headers are still preserved
    expect(screen.getByText("Code")).toBeInTheDocument();
    expect(screen.getByText("Name")).toBeInTheDocument();

    expect(screen.getByText("No records found for filter")).toBeInTheDocument();
    expect(screen.getByText("Try broadening your search criteria")).toBeInTheDocument();
  });
});
