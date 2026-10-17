"use client";

import React, { useMemo } from "react";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { ListDateRangeFilter } from "@/components/ui/ListDateRangeFilter";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { LIST_PAGE_SIZES, useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import type { AuditEventResponse } from "@/lib/api/api-client";
import { formatDateTime } from "@/lib/formatters/formatters";
import { useAuditEvents } from "../api/audit-queries";

interface AuditFilters extends ListFilterRecord {
  from?: string;
  to?: string;
}

const toListPageSize = (value: number): ListPageSize | undefined => LIST_PAGE_SIZES.find((size) => size === value);

export function AuditEventList() {
  const t = useTranslations("auditLog");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const listState = useListState<AuditFilters>({ schema: { single: ["from", "to"] }, debounceMs: 350 });
  const { page, limit, filters } = listState.params;
  const { data, isLoading, isError, refetch } = useAuditEvents({
    // Date-only picker values become whole UTC days.
    from: filters.from ? `${filters.from}T00:00:00Z` : undefined,
    to: filters.to ? `${filters.to}T23:59:59Z` : undefined,
    page,
    limit,
  });

  const rows = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 1;
  const isZero = !isLoading && !isError && totalItems === 0 && !filters.from && !filters.to;

  const columns = useMemo<Column<AuditEventResponse>[]>(
    () => [
      { id: "occurredAt", header: t("list.table.occurredAt"), cell: (_v, row) => formatDateTime(row.occurredAtUtc, locale) },
      { id: "action", header: t("list.table.action"), cell: (_v, row) => row.action ?? "-" },
      { id: "resourceType", header: t("list.table.resourceType"), cell: (_v, row) => row.resourceType ?? "-" },
      { id: "resourceId", header: t("list.table.resourceId"), cell: (_v, row) => row.resourceId || "-" },
      { id: "actor", header: t("list.table.actor"), cell: (_v, row) => row.actor?.name ?? "-" },
      { id: "reason", header: t("list.table.reason"), cell: (_v, row) => row.reason ?? "-" },
    ],
    [locale, t]
  );

  return (
    <div className="space-y-6">
      <PageHeader title={t("list.title")} subtitle={t("list.subtitle")} />
      <ListToolbar>
        <div className="flex flex-wrap items-end gap-3">
          <ListDateRangeFilter
            startDate={filters.from ?? null}
            endDate={filters.to ?? null}
            onChange={(range) => {
              listState.actions.setFilter("from", range.startDate || undefined);
              listState.actions.setFilter("to", range.endDate || undefined);
            }}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("list.emptyTitle")} description={t("list.emptyDescription")} />
      ) : (
        <DataTable<AuditEventResponse>
          columns={columns}
          data={rows}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
          pagination={{ page, limit, totalPages, totalItems }}
          onPageChange={(next) => listState.actions.setPage(next)}
          onLimitChange={(next) => {
            const pageSize = toListPageSize(next);
            if (pageSize) listState.actions.setLimit(pageSize);
          }}
        />
      )}
    </div>
  );
}
