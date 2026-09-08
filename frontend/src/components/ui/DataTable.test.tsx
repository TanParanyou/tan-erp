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

    const codeHeader = screen.getByText("Code");
    fireEvent.click(codeHeader);
    expect(onSort).toHaveBeenCalledWith("code");
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
});
