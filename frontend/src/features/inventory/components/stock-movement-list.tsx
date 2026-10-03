"use client";

import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { EmptyState } from "@/components/ui/EmptyState";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatCurrency, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import type { StockMovementResponse } from "@/lib/api/api-client";
import { useStockMovements, useStockReconciliation, useWarehouseList } from "../api/inventory-queries";
import { isMovementKind, MOVEMENT_KINDS } from "../inventory-status";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";

interface MovementFilters extends ListFilterRecord {
  warehouseId?: string;
  kind?: string;
}

const WAREHOUSE_PAGE_LIMIT = 100;

export function StockMovementList() {
  const t = useTranslations("inventory.movements");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canReadWarehouses = can(selectedMembership, PERMISSIONS.WAREHOUSES_READ);
  const state = useListState<MovementFilters>({
    schema: { single: ["warehouseId", "kind"], defaultSort: "postedAt", defaultOrder: "desc", allowedSorts: ["postedAt"] },
    debounceMs: 300,
  });
  const warehouses = useWarehouseList({ page: 1, pageSize: WAREHOUSE_PAGE_LIMIT }, canReadWarehouses);
  const reconciliation = useStockReconciliation();
  const result = useStockMovements({
    warehouseId: state.params.filters.warehouseId || undefined,
    kind: state.params.filters.kind || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.filters.warehouseId && !state.params.filters.kind;
  const inconsistent = reconciliation.data?.inconsistentCount ?? 0;

  const columns: Column<StockMovementResponse>[] = [
    { id: "postedAt", header: t("postedAt"), cell: (_v, row) => (row.postedAtUtc ? formatDateTime(row.postedAtUtc, locale) : "-"), className: "min-w-40" },
    { id: "document", header: t("document"), className: "font-mono", cell: (_v, row) => row.documentNumber ?? "-" },
    { id: "kind", header: t("kind"), cell: (_v, row) => <StatusBadge label={isMovementKind(row.kind) ? t(`kinds.${row.kind}`) : "-"} variant={(row.quantityDelta ?? 0) >= 0 ? "success" : "warning"} /> },
    { id: "warehouse", header: t("warehouse"), cell: (_v, row) => `${row.warehouse?.code ?? "-"}` },
    { id: "item", header: t("item"), cell: (_v, row) => `${row.item?.code ?? "-"} · ${row.item?.nameTh ?? "-"}`, className: "min-w-48" },
    { id: "quantity", header: t("quantity"), className: "text-right font-mono", cell: (_v, row) => formatNumber(row.quantityDelta) },
    { id: "unitCost", header: t("unitCost"), className: "text-right font-mono", cell: (_v, row) => formatCurrency(row.unitCost, "THB", locale) },
    { id: "value", header: t("value"), className: "text-right font-mono", cell: (_v, row) => formatCurrency(row.valueDelta, "THB", locale) },
    { id: "onHandAfter", header: t("onHandAfter"), className: "text-right font-mono", cell: (_v, row) => formatNumber(row.onHandAfter) },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={reconciliation.data ? (
          <StatusBadge
            label={inconsistent === 0 ? t("reconciled", { count: reconciliation.data.rowCount ?? 0 }) : t("mismatch", { count: inconsistent })}
            variant={inconsistent === 0 ? "success" : "danger"}
          />
        ) : undefined}
      />
      <ListToolbar>
        <div className="flex w-full flex-wrap items-end gap-3">
          {canReadWarehouses && (
            <ListFilterSelect
              id="movement-warehouse-filter"
              label={t("warehouse")}
              value={state.params.filters.warehouseId ?? ""}
              onChange={(value) => state.actions.setFilter("warehouseId", value || undefined)}
              options={(warehouses.data?.items ?? []).map((w) => ({ value: w.id ?? "", label: `${w.code ?? "-"} · ${w.name ?? "-"}` }))}
            />
          )}
          <ListFilterSelect
            id="movement-kind-filter"
            label={t("kind")}
            value={state.params.filters.kind ?? ""}
            onChange={(value) => state.actions.setFilter("kind", value || undefined)}
            options={MOVEMENT_KINDS.map((value) => ({ value, label: t(`kinds.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<StockMovementResponse>
          columns={columns}
          data={rows}
          isLoading={result.isLoading}
          isError={result.isError}
          error={result.error}
          onRetry={() => { void result.refetch(); }}
          emptyTitle={t("noResults")}
          emptyDescription={t("emptyDetail")}
          pagination={{ page: state.params.page, limit: state.params.limit, totalPages: pagination?.totalPages ?? 0, total: totalCount }}
          onPageChange={state.actions.setPage}
          onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
        />
      )}
    </section>
  );
}
