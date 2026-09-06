"use client";

import React from "react";
import { Checkbox } from "@/components/ui/Checkbox";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { EmptyState } from "@/components/ui/EmptyState";
import {
  IconChevronLeft,
  IconChevronRight,
  IconChevronsLeft,
  IconChevronsRight,
  IconArrowUpDown,
  IconArrowUp,
  IconArrowDown,
} from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import { useTranslations, useLocale } from "next-intl";
import type { SortState } from "@/hooks/useDataTableState";

export interface ColumnDef<T> {
  id?: string;
  header: React.ReactNode;
  accessorKey?: keyof T;
  cell?: (value: unknown, row: T) => React.ReactNode;
  sortable?: boolean;
  sortKey?: string;
  width?: string | number;
  align?: "left" | "center" | "right";
  className?: string;
}

export interface DataTablePagination {
  page: number;
  limit: number;
  totalPages: number;
  totalItems?: number;
}

export interface DataTableProps<T> {
  columns: ColumnDef<T>[];
  data: T[];
  pagination?: DataTablePagination;
  sorting?: SortState;
  isLoading?: boolean;

  // Actions & Listeners
  onPageChange?: (page: number) => void;
  onLimitChange?: (limit: number) => void;
  onSort?: (key: string) => void;

  // Selection
  selectable?: boolean;
  selectedIds?: Set<string | number>;
  onSelect?: (id: string | number) => void;
  onSelectAll?: (ids: (string | number)[]) => void;
  getRowId?: (row: T, index: number) => string | number;

  // Presentation
  emptyText?: string;
  emptyTitle?: string;
  emptyDescription?: string;
  emptyAction?: {
    label: string;
    onClick?: () => void;
    href?: string;
  };
  actions?: React.ReactNode;
  onPageSizeChange?: (size: number) => void;
  className?: string;
  hidePagination?: boolean;
}

export function DataTable<T>({
  columns,
  data,
  pagination,
  sorting,
  isLoading = false,
  onPageChange,
  onLimitChange,
  onPageSizeChange,
  onSort,
  selectable = false,
  selectedIds = new Set(),
  onSelect,
  onSelectAll,
  getRowId = (row, index) => ((row as Record<string, unknown>).id as string | number) || index,
  emptyText,
  emptyTitle,
  emptyDescription,
  emptyAction,
  actions,
  className,
  hidePagination = false,
}: DataTableProps<T>) {
  const t = useTranslations("common.table");
  const tStates = useTranslations("common.states");
  const tFeedback = useTranslations("common.feedback");
  const locale = useLocale();
  const safeData = data || [];
  const page = pagination?.page || 1;
  const totalPages = pagination?.totalPages ?? Math.ceil(safeData.length / (pagination?.limit || 25));
  const limit = pagination?.limit || 25;
  const totalItems = pagination?.totalItems ?? safeData.length;
  const resolvedEmptyTitle = emptyTitle || emptyText || tFeedback("noData");

  const handleLimitChange = (newLimit: number) => {
    if (onPageSizeChange) {
      onPageSizeChange(newLimit);
    } else if (onLimitChange) {
      onLimitChange(newLimit);
    }
  };

  const currentIds = safeData.map((row, idx) => getRowId(row, idx));
  const isAllSelected = currentIds.length > 0 && currentIds.every((id) => selectedIds.has(id));
  const isIndeterminate = currentIds.some((id) => selectedIds.has(id)) && !isAllSelected;

  const handleHeaderCheckboxToggle = () => {
    if (onSelectAll) {
      onSelectAll(currentIds);
    }
  };

  const renderSortIndicator = (col: ColumnDef<T>) => {
    if (!col.sortable) return null;
    const key = col.sortKey || (col.accessorKey as string);
    if (!key) return null;

    const isCurrent = sorting?.key === key;
    if (!isCurrent) {
      return (
        <span style={{ color: "var(--erp-text-muted)", marginLeft: "0.25rem", verticalAlign: "middle" }}>
          <IconArrowUpDown size={14} />
        </span>
      );
    }

    return (
      <span style={{ color: "var(--erp-navy)", marginLeft: "0.25rem", verticalAlign: "middle" }}>
        {sorting?.order === "asc" ? <IconArrowUp size={14} /> : <IconArrowDown size={14} />}
      </span>
    );
  };

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

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "0.75rem", width: "100%" }}>
      {actions && (
        <div style={{ display: "flex", alignItems: "center", justifyContent: "flex-end", gap: "0.5rem" }}>
          {actions}
        </div>
      )}
      <div className={cn("erp-table-wrapper", className)}>
        {isLoading && (
          <div
            style={{
              position: "absolute",
              inset: 0,
              backgroundColor: "rgba(255, 255, 255, 0.7)",
              backdropFilter: "blur(1px)",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              zIndex: 10,
            }}
          >
            <MonoSpinner size="md" label={tStates("loading")} />
          </div>
        )}

        <table className="erp-table">
          <thead>
            <tr>
              {selectable && (
                <th
                  className="erp-th"
                  style={{ width: "48px", textAlign: "center", paddingLeft: "1rem", paddingRight: "0.5rem" }}
                >
                  <Checkbox
                    checked={isAllSelected}
                    indeterminate={isIndeterminate}
                    onChange={handleHeaderCheckboxToggle}
                    aria-label={t("selectAll")}
                  />
                </th>
              )}

              {columns.map((col, idx) => {
                const sortKey = col.sortKey || (col.accessorKey as string);
                const isSortable = !!col.sortable && !!sortKey;

                return (
                  <th
                    key={col.id || (col.accessorKey as string) || idx}
                    className={cn("erp-th", isSortable && "erp-th-sortable", col.className)}
                    style={{
                      width: col.width,
                      textAlign: col.align || "left",
                    }}
                    onClick={isSortable && onSort ? () => onSort(sortKey) : undefined}
                  >
                    <div
                      style={{
                        display: "inline-flex",
                        alignItems: "center",
                        justifyContent: col.align === "right" ? "flex-end" : col.align === "center" ? "center" : "flex-start",
                        width: "100%",
                      }}
                    >
                      <span>{col.header}</span>
                      {renderSortIndicator(col)}
                    </div>
                  </th>
                );
              })}
            </tr>
          </thead>

          <tbody>
            {safeData.length === 0 && !isLoading ? (
              <tr>
                <td colSpan={columns.length + (selectable ? 1 : 0)} style={{ padding: 0 }}>
                  <EmptyState
                    title={resolvedEmptyTitle}
                    description={emptyDescription}
                    action={emptyAction}
                  />
                </td>
              </tr>
            ) : (
              safeData.map((row, rowIdx) => {
                const rowId = getRowId(row, rowIdx);
                const isSelected = selectedIds.has(rowId);

                return (
                  <tr
                    key={String(rowId)}
                    className={cn("erp-tr", isSelected && "erp-tr-selected")}
                  >
                    {selectable && (
                      <td
                        className="erp-td"
                        style={{ width: "48px", textAlign: "center", paddingLeft: "1rem", paddingRight: "0.5rem" }}
                      >
                        <Checkbox
                          checked={isSelected}
                          onChange={onSelect ? () => onSelect(rowId) : undefined}
                          aria-label={`${t("selectRow")} ${rowIdx + 1}`}
                        />
                      </td>
                    )}

                    {columns.map((col, colIdx) => {
                      const value = col.accessorKey ? row[col.accessorKey] : undefined;
                      const content = col.cell ? col.cell(value, row) : (value as React.ReactNode);

                      return (
                        <td
                          key={col.id || (col.accessorKey as string) || colIdx}
                          className={cn("erp-td", col.className)}
                          style={{
                            textAlign: col.align || "left",
                          }}
                        >
                          {content ?? "-"}
                        </td>
                      );
                    })}
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination Bar */}
      {!hidePagination && totalPages > 0 && (
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            flexWrap: "wrap",
            gap: "1rem",
            padding: "0.5rem 0",
            fontSize: "0.8125rem",
            color: "var(--erp-text-muted)",
          }}
        >
          {/* Left: Total Items and Page Size Selector */}
          <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
            <span>
              {t("total")} <strong>{totalItems.toLocaleString(locale === "th" ? "th-TH" : "en-US")}</strong> {t("items")}
            </span>

            {(onLimitChange || onPageSizeChange) && (
              <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
                <span>{t("showing")}</span>
                <select
                  value={limit}
                  onChange={(e) => handleLimitChange(Number(e.target.value))}
                  className="erp-select"
                  style={{
                    minHeight: "32px",
                    height: "32px",
                    padding: "0 1.75rem 0 0.5rem",
                    width: "auto",
                    fontSize: "0.8125rem",
                  }}
                  aria-label={t("rowsPerPage")}
                >
                  <option value={10}>10</option>
                  <option value={25}>25</option>
                  <option value={50}>50</option>
                  <option value={100}>100</option>
                </select>
                <span>{t("itemsPerPage")}</span>
              </div>
            )}
          </div>

          {/* Right: Pagination Page Buttons */}
          {onPageChange && (
            <div style={{ display: "flex", alignItems: "center", gap: "0.25rem" }}>
              <button
                type="button"
                className="erp-btn erp-btn-outline erp-btn-sm"
                onClick={() => onPageChange(1)}
                disabled={page <= 1}
                aria-label={t("firstPage")}
                style={{ padding: "0.25rem 0.5rem", minHeight: "32px" }}
              >
                <IconChevronsLeft size={16} />
              </button>

              <button
                type="button"
                className="erp-btn erp-btn-outline erp-btn-sm"
                onClick={() => onPageChange(page - 1)}
                disabled={page <= 1}
                aria-label={t("previous")}
                style={{ padding: "0.25rem 0.5rem", minHeight: "32px" }}
              >
                <IconChevronLeft size={16} />
              </button>

              {getPageNumbers().map((p, idx) => {
                if (p === "...") {
                  return (
                    <span
                      key={`dots-${idx}`}
                      style={{ padding: "0 0.5rem", color: "var(--erp-text-muted)" }}
                    >
                      ...
                    </span>
                  );
                }

                const pageNum = Number(p);
                const isActive = pageNum === page;

                return (
                  <button
                    key={pageNum}
                    type="button"
                    className={cn(
                      "erp-btn erp-btn-sm",
                      isActive ? "erp-btn-primary" : "erp-btn-outline"
                    )}
                    onClick={() => onPageChange(pageNum)}
                    style={{
                      minWidth: "32px",
                      minHeight: "32px",
                      padding: "0.25rem 0.5rem",
                    }}
                  >
                    {pageNum}
                  </button>
                );
              })}

              <button
                type="button"
                className="erp-btn erp-btn-outline erp-btn-sm"
                onClick={() => onPageChange(page + 1)}
                disabled={page >= totalPages}
                aria-label={t("next")}
                style={{ padding: "0.25rem 0.5rem", minHeight: "32px" }}
              >
                <IconChevronRight size={16} />
              </button>

              <button
                type="button"
                className="erp-btn erp-btn-outline erp-btn-sm"
                onClick={() => onPageChange(totalPages)}
                disabled={page >= totalPages}
                aria-label={t("lastPage")}
                style={{ padding: "0.25rem 0.5rem", minHeight: "32px" }}
              >
                <IconChevronsRight size={16} />
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
