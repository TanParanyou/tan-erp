"use client";

import { useMemo } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { WorkOrderListItemResponse } from "@/lib/api/api-client";
import { useWorkOrderList } from "../api/production-queries";
import { isWorkOrderStatus, WORK_ORDER_STATUSES, workOrderStatusVariant } from "../production-status";

interface OrderFilters extends ListFilterRecord {
  status?: string;
}

export function WorkOrderList() {
  const t = useTranslations("production.workOrders");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.WORK_ORDERS_MANAGE);
  const state = useListState<OrderFilters>({
    schema: { single: ["status"], defaultSort: "createdAt", defaultOrder: "desc", allowedSorts: ["createdAt"] },
    debounceMs: 300,
  });
  const result = useWorkOrderList({
    search: state.params.search || undefined,
    status: state.params.filters.status || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search && !state.params.filters.status;

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = state.params.filters.status;
    return status && isWorkOrderStatus(status) ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }] : [];
  }, [state.params.filters.status, t]);

  const columns: Column<WorkOrderListItemResponse>[] = [
    {
      id: "number",
      header: t("number"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => <Link href={`/${locale}/production/work-orders/${row.id}`} className="font-mono font-semibold text-erp-navy underline">{row.number ?? "-"}</Link>,
    },
    { id: "item", header: t("item"), className: "min-w-48", cell: (_v, row) => `${row.itemCode ?? "-"} · ${row.itemNameTh ?? "-"}` },
    { id: "project", header: t("project"), cell: (_v, row) => row.projectCode ?? "-" },
    { id: "status", header: t("status"), cell: (_v, row) => <StatusBadge label={isWorkOrderStatus(row.status) ? t(`statuses.${row.status}`) : "-"} variant={workOrderStatusVariant(row.status)} /> },
    { id: "progress", header: t("progress"), className: "text-right font-mono", cell: (_v, row) => `${formatNumber(row.completedQuantity)} / ${formatNumber(row.plannedQuantity)}` },
    { id: "created", header: t("createdAt"), cell: (_v, row) => (row.createdAtUtc ? formatDateTime(row.createdAtUtc, locale) : "-") },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canManage ? <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/production/work-orders/create`}>{t("create")}</Button> : undefined}
      />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="work-order-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          <ListFilterSelect
            id="work-order-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={WORK_ORDER_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<WorkOrderListItemResponse>
          columns={columns}
          data={rows}
          isLoading={result.isLoading}
          isError={result.isError}
          error={result.error}
          onRetry={() => { void result.refetch(); }}
          emptyTitle={t("noResults")}
          emptyDescription={t("search")}
          pagination={{ page: state.params.page, limit: state.params.limit, totalPages: pagination?.totalPages ?? 0, total: totalCount }}
          onPageChange={state.actions.setPage}
          onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
        />
      )}
    </section>
  );
}
