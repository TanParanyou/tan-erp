"use client";

import React from "react";
import {
  IconChevronLeft,
  IconChevronRight,
  IconChevronsLeft,
  IconChevronsRight,
  IconArrowUp,
  IconArrowDown,
  IconArrowUpDown,
} from "@/components/common/Icons";
import { Checkbox } from "./Checkbox";
import { MonoSpinner } from "./MonoSpinner";
import { cn } from "@/lib/utils/cn";
import type { SortState } from "@/hooks/useDataTable";
import { useTranslations } from "next-intl";

export interface Column<T> {
  id?: string;
  header: React.ReactNode;
  accessorKey?: keyof T;
  cell?: (value: unknown, row: T) => React.ReactNode;
  className?: string;
  sortable?: boolean;
  sticky?: "left" | "right" | boolean;
  isAction?: boolean;
}

export interface DataTablePagination {
  page: number;
  limit: number;
  totalPages: number;
  total?: number;
  totalItems?: number;
}

export interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  pagination?: DataTablePagination;
  sorting?: SortState;
  isLoading?: boolean;
  onPageChange?: (page: number) => void;
  onLimitChange?: (limit: number) => void;
  onSort?: (key: string) => void;
  className?: string;
  hidePagination?: boolean;
  stickyActionColumn?: boolean;

  // Row selection
  selectable?: boolean;
  selectedIds?: Set<string | number>;
  onSelect?: (id: string | number) => void;
  onSelectAll?: (ids: (string | number)[]) => void;
}

export function DataTable<T>({
  columns,
  data,
  pagination,
  sorting,
  isLoading = false,
  onPageChange,
  onLimitChange,
  onSort,
  className,
  hidePagination = false,
  stickyActionColumn = true,
  selectable = false,
  selectedIds = new Set(),
  onSelect,
  onSelectAll,
}: DataTableProps<T>) {
  const t = useTranslations("common.table");
  const tStates = useTranslations("common.states");
  const safeData = data || [];
  const page = pagination?.page || 1;
  const totalPages = pagination?.totalPages ?? 0;
  const limit = pagination?.limit || 25;
  const totalItems =
    pagination?.total ?? pagination?.totalItems ?? safeData.length;
  const sortingState = sorting || { key: null, order: "asc" as const };

  const getPageNumbers = () => {
    const pages: (number | string)[] = [];
    const maxVisible = 5;

    if (totalPages <= maxVisible) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else if (page <= 3) {
      for (let i = 1; i <= 3; i++) pages.push(i);
      pages.push("...");
      pages.push(totalPages);
    } else if (page >= totalPages - 2) {
      pages.push(1);
      pages.push("...");
      for (let i = totalPages - 2; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      pages.push("...");
      pages.push(page);
      pages.push("...");
      pages.push(totalPages);
    }
    return pages;
  };

  const isRowSelected = (row: T): boolean => {
    const id = (row as Record<string, unknown>).id as string | number | undefined;
    return id !== undefined && selectedIds.has(id);
  };

  const isAllSelected =
    safeData.length > 0 &&
    safeData.every((row) => {
      const id = (row as Record<string, unknown>).id as string | number | undefined;
      return id !== undefined && selectedIds.has(id);
    });

  const isSomeSelected =
    safeData.length > 0 &&
    safeData.some((row) => {
      const id = (row as Record<string, unknown>).id as string | number | undefined;
      return id !== undefined && selectedIds.has(id);
    }) &&
    !isAllSelected;

  const handleSelectAll = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!onSelectAll) return;
    if (e.target.checked) {
      const ids = safeData
        .map((row) => (row as Record<string, unknown>).id as string | number | undefined)
        .filter((id): id is string | number => id !== undefined);
      onSelectAll(ids);
    } else {
      onSelectAll([]);
    }
  };

  const getColumnStickyPosition = (
    col: Column<T>,
    colIdx: number,
    totalCols: number
  ): "left" | "right" | null => {
    if (col.sticky === "left") return "left";
    if (col.sticky === "right" || col.sticky === true) return "right";
    if (col.sticky === false) return null;

    if (col.isAction) return "right";
    const colId = (col.id || (col.accessorKey as string) || "").toString().toLowerCase();
    if (colId === "actions" || colId === "action") return "right";

    if (stickyActionColumn && colIdx === totalCols - 1 && !col.accessorKey && Boolean(col.cell)) {
      return "right";
    }

    if (typeof col.header === "string") {
      const headerText = col.header.toLowerCase().trim();
      if (
        headerText === "จัดการ" ||
        headerText.includes("จัดการ") ||
        headerText === "actions" ||
        headerText === "action"
      ) {
        return "right";
      }
    }

    return null;
  };

  return (
    <div className={cn("w-full space-y-3", className)}>
      <div className="erp-table-wrapper rounded-none border border-erp-border bg-erp-surface">
        <table className="erp-table">
          <thead>
            <tr>
              {selectable && (
                <th className="erp-th w-[48px] px-3 py-2.5">
                  <Checkbox
                    checked={isAllSelected}
                    aria-label={t("selectAll")}
                    ref={(input: HTMLInputElement | null) => {
                      if (input) input.indeterminate = isSomeSelected;
                    }}
                    onChange={handleSelectAll}
                  />
                </th>
              )}
              {columns.map((col, idx) => {
                const isSortable = Boolean(col.sortable && col.accessorKey && onSort);
                const isSorted = sortingState.key === col.accessorKey;
                const isAsc = isSorted && sortingState.order === "asc";
                const isDesc = isSorted && sortingState.order === "desc";
                const stickyPos = getColumnStickyPosition(col, idx, columns.length);
                const isStickyRight = stickyPos === "right";
                const isStickyLeft = stickyPos === "left";

                const sortTooltip = isSortable
                  ? isSorted
                    ? isAsc
                      ? t("sortAscending")
                      : t("sortDescending")
                    : t("sortNone")
                  : undefined;

                return (
                  <th
                    key={idx}
                    role={isSortable ? "button" : undefined}
                    tabIndex={isSortable ? 0 : undefined}
                    aria-sort={
                      isSorted
                        ? isAsc
                          ? "ascending"
                          : "descending"
                        : isSortable
                        ? "none"
                        : undefined
                    }
                    title={sortTooltip}
                    className={cn(
                      "erp-th transition-colors duration-150 relative",
                      isSortable &&
                        "erp-th-sortable group/th select-none cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-erp-navy",
                      isSorted && "bg-[#EAF0F6] text-erp-navy border-b-2 border-b-erp-navy font-bold",
                      isStickyRight &&
                        "sticky right-0 z-20 shadow-[-2px_0_4px_rgba(0,0,0,0.08)] bg-erp-surface-subtle border-l border-erp-border",
                      isStickyLeft &&
                        "sticky left-0 z-20 shadow-[2px_0_4px_rgba(0,0,0,0.08)] bg-erp-surface-subtle border-r border-erp-border",
                      col.className
                    )}
                    onClick={() =>
                      isSortable && onSort?.(col.accessorKey as string)
                    }
                    onKeyDown={(e) => {
                      if (isSortable && (e.key === "Enter" || e.key === " ")) {
                        e.preventDefault();
                        onSort?.(col.accessorKey as string);
                      }
                    }}
                  >
                    <div className="flex items-center justify-between gap-2">
                      <span
                        className={cn(
                          "truncate",
                          isSorted ? "text-erp-navy font-bold" : "text-erp-text-main"
                        )}
                      >
                        {col.header}
                      </span>
                      {isSortable && (
                        <span
                          aria-hidden="true"
                          className={cn(
                            "w-5 h-5 inline-flex items-center justify-center shrink-0 rounded-none border transition-all duration-150",
                            isSorted
                              ? "bg-erp-navy text-white border-erp-navy shadow-xs"
                              : "border-transparent text-erp-text-muted/60 group-hover/th:border-erp-border group-hover/th:bg-white group-hover/th:text-erp-navy opacity-0 group-hover/th:opacity-100"
                          )}
                        >
                          {isAsc ? (
                            <IconArrowUp size={12} strokeWidth={2.5} />
                          ) : isDesc ? (
                            <IconArrowDown size={12} strokeWidth={2.5} />
                          ) : (
                            <IconArrowUpDown size={12} strokeWidth={2} />
                          )}
                        </span>
                      )}
                    </div>
                  </th>
                );
              })}
            </tr>
          </thead>

          <tbody>
            {isLoading ? (
              <tr>
                <td
                  colSpan={columns.length + (selectable ? 1 : 0)}
                  className="h-44 text-center"
                >
                  <MonoSpinner size="md" label={tStates("loading")} />
                </td>
              </tr>
            ) : safeData.length > 0 ? (
              safeData.map((row, rowIdx) => {
                const rowObj = row as Record<string, unknown>;
                const rowId = (rowObj.id as string | number | undefined) ?? rowIdx;
                const isSelected = isRowSelected(row);

                return (
                  <tr
                    key={rowId}
                    className={cn(
                      "erp-tr transition-colors",
                      isSelected && "erp-tr-selected"
                    )}
                  >
                    {selectable && (
                      <td className="erp-td w-[48px] px-3 py-2.5">
                        <Checkbox
                          checked={isSelected}
                          aria-label={`${t("selectRow")} ${rowIdx + 1}`}
                          onChange={() => {
                            const id = rowObj.id as string | number | undefined;
                            if (id !== undefined) onSelect?.(id);
                          }}
                        />
                      </td>
                    )}
                    {columns.map((col, colIdx) => {
                      const stickyPos = getColumnStickyPosition(col, colIdx, columns.length);
                      const isStickyRight = stickyPos === "right";
                      const isStickyLeft = stickyPos === "left";

                      return (
                        <td
                          key={colIdx}
                          className={cn(
                            "erp-td whitespace-nowrap",
                            isStickyRight &&
                              cn(
                                "sticky right-0 z-10 border-l border-erp-border shadow-[-2px_0_4px_rgba(0,0,0,0.05)]",
                                isSelected ? "bg-erp-navy-light" : "bg-erp-surface"
                              ),
                            isStickyLeft &&
                              cn(
                                "sticky left-0 z-10 border-r border-erp-border shadow-[2px_0_4px_rgba(0,0,0,0.05)]",
                                isSelected ? "bg-erp-navy-light" : "bg-erp-surface"
                              ),
                            col.className
                          )}
                        >
                          {col.cell
                            ? col.cell(
                                col.accessorKey
                                  ? rowObj[col.accessorKey as string]
                                  : undefined,
                                row
                              )
                            : col.accessorKey
                            ? String(rowObj[col.accessorKey as string] ?? "")
                            : null}
                        </td>
                      );
                    })}
                  </tr>
                );
              })
            ) : (
              <tr>
                <td
                  colSpan={columns.length + (selectable ? 1 : 0)}
                  className="h-32 text-center text-erp-text-muted"
                >
                  {t("emptyTable")}
                </td>
              </tr>
            )}
          </tbody>
        </table>

        {/* Pagination */}
        {!hidePagination && (
          <div className="flex flex-col sm:flex-row items-center justify-between border-t border-erp-border bg-erp-surface-muted px-4 py-2.5 gap-3">
            <div className="flex items-center gap-3 text-xs text-erp-text-muted">
              {totalItems > 0 ? (
                <span>
                  {t("showing")}{" "}
                  <span className="font-semibold text-erp-text-main">
                    {(page - 1) * limit + 1}
                  </span>{" "}
                  {t("to")}{" "}
                  <span className="font-semibold text-erp-text-main">
                    {Math.min(page * limit, totalItems)}
                  </span>{" "}
                  {t("of")}{" "}
                  <span className="font-semibold text-erp-text-main">
                    {totalItems}
                  </span>{" "}
                  {t("entries")}
                </span>
              ) : (
                <span>{t("noEntries")}</span>
              )}

              {onLimitChange && (
                <div className="flex items-center gap-1.5 ml-2">
                  <select
                    value={limit}
                    aria-label={t("itemsPerPage")}
                    onChange={(e) => onLimitChange(Number(e.target.value))}
                    className="h-7 px-2 border border-erp-border bg-erp-surface text-xs text-erp-text-main rounded-none"
                  >
                    {[10, 25, 50, 100].map((size) => (
                      <option key={size} value={size}>
                        {size} / {t("page")}
                      </option>
                    ))}
                  </select>
                </div>
              )}
            </div>

            <div className="flex items-center gap-1">
              <button
                type="button"
                onClick={() => onPageChange?.(1)}
                disabled={page === 1 || totalPages === 0 || isLoading}
                aria-label={t("firstPage")}
                className="p-1 rounded-none border border-erp-border bg-erp-surface text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main disabled:opacity-40 disabled:cursor-not-allowed"
              >
                <IconChevronsLeft size={16} />
              </button>
              <button
                type="button"
                onClick={() => onPageChange?.(page - 1)}
                disabled={page === 1 || totalPages === 0 || isLoading}
                aria-label={t("previous")}
                className="p-1 rounded-none border border-erp-border bg-erp-surface text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main disabled:opacity-40 disabled:cursor-not-allowed"
              >
                <IconChevronLeft size={16} />
              </button>

              {getPageNumbers().map((p, i) =>
                p === "..." ? (
                  <span key={i} className="px-1 text-xs text-erp-text-muted">
                    ...
                  </span>
                ) : (
                  <button
                    key={i}
                    type="button"
                    onClick={() => onPageChange?.(p as number)}
                    className={cn(
                      "h-7 w-7 rounded-none text-xs font-mono font-medium transition-colors",
                      page === p
                        ? "bg-erp-navy text-white font-bold"
                        : "border border-erp-border bg-erp-surface text-erp-text-main hover:bg-erp-surface-muted"
                    )}
                  >
                    {p}
                  </button>
                )
              )}

              <button
                type="button"
                onClick={() => onPageChange?.(page + 1)}
                disabled={page >= totalPages || totalPages === 0 || isLoading}
                aria-label={t("next")}
                className="p-1 rounded-none border border-erp-border bg-erp-surface text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main disabled:opacity-40 disabled:cursor-not-allowed"
              >
                <IconChevronRight size={16} />
              </button>
              <button
                type="button"
                onClick={() => onPageChange?.(totalPages)}
                disabled={page >= totalPages || totalPages === 0 || isLoading}
                aria-label={t("lastPage")}
                className="p-1 rounded-none border border-erp-border bg-erp-surface text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main disabled:opacity-40 disabled:cursor-not-allowed"
              >
                <IconChevronsRight size={16} />
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default DataTable;
