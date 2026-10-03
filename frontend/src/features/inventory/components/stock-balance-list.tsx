"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatCurrency, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { StockBalanceResponse } from "@/lib/api/api-client";
import { useStockBalances, useWarehouseList } from "../api/inventory-queries";
import { StockDocumentModal, type StockDocumentKind } from "./stock-document-modal";
import { StockReserveModal } from "./stock-reserve-modal";

interface BalanceFilters extends ListFilterRecord {
  warehouseId?: string;
  inStock?: string;
}

const WAREHOUSE_PAGE_LIMIT = 100;

export function StockBalanceList() {
  const t = useTranslations("inventory.stock");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canIssue = can(selectedMembership, PERMISSIONS.INVENTORY_ISSUE);
  const canTransfer = can(selectedMembership, PERMISSIONS.INVENTORY_TRANSFER);
  const canAdjust = can(selectedMembership, PERMISSIONS.INVENTORY_ADJUST);
  const canReserve = can(selectedMembership, PERMISSIONS.INVENTORY_RESERVE);
  const canReadWarehouses = can(selectedMembership, PERMISSIONS.WAREHOUSES_READ);

  const state = useListState<BalanceFilters>({
    schema: { single: ["warehouseId", "inStock"], defaultSort: "code", defaultOrder: "asc", allowedSorts: ["code"] },
    debounceMs: 300,
  });
  const warehouses = useWarehouseList({ page: 1, pageSize: WAREHOUSE_PAGE_LIMIT }, canReadWarehouses);
  const result = useStockBalances({
    warehouseId: state.params.filters.warehouseId || undefined,
    search: state.params.search || undefined,
    inStockOnly: state.params.filters.inStock === "true",
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const [documentKind, setDocumentKind] = useState<StockDocumentKind | null>(null);
  const [reserveFor, setReserveFor] = useState<StockBalanceResponse | null>(null);

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search && !state.params.filters.warehouseId && !state.params.filters.inStock;

  const columns: Column<StockBalanceResponse>[] = [
    { id: "warehouse", header: t("warehouse"), cell: (_v, row) => `${row.warehouse?.code ?? "-"} · ${row.warehouse?.name ?? "-"}`, className: "min-w-40" },
    { id: "item", header: t("item"), cell: (_v, row) => `${row.item?.code ?? "-"} · ${row.item?.nameTh ?? "-"}`, className: "min-w-52" },
    { id: "unit", header: t("unit"), cell: (_v, row) => row.item?.unitCode ?? "-" },
    { id: "onHand", header: t("onHand"), className: "text-right font-mono", cell: (_v, row) => formatNumber(row.onHand) },
    { id: "reserved", header: t("reserved"), className: "text-right font-mono", cell: (_v, row) => formatNumber(row.reserved) },
    { id: "available", header: t("available"), className: "text-right font-mono font-semibold", cell: (_v, row) => formatNumber(row.available) },
    { id: "averageCost", header: t("averageCost"), className: "text-right font-mono", cell: (_v, row) => formatCurrency(row.averageCost, "THB", locale) },
    { id: "totalValue", header: t("totalValue"), className: "text-right font-mono", cell: (_v, row) => formatCurrency(row.totalValue, "THB", locale) },
    {
      id: "actions",
      header: t("actions"),
      isAction: true,
      cell: (_v, row) => canReserve && (row.available ?? 0) > 0 ? (
        <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={() => setReserveFor(row)}>{t("reserve")}</Button>
      ) : null,
    },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={(
          <div className="flex flex-wrap gap-2">
            {canIssue && <Button variant="outline" onClick={() => setDocumentKind("issue")}>{t("issue")}</Button>}
            {canTransfer && <Button variant="outline" onClick={() => setDocumentKind("transfer")}>{t("transfer")}</Button>}
            {canAdjust && <Button variant="outline" onClick={() => setDocumentKind("adjustment")}>{t("adjust")}</Button>}
          </div>
        )}
      />
      <ListToolbar>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="stock-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          {canReadWarehouses && (
            <ListFilterSelect
              id="stock-warehouse-filter"
              label={t("warehouse")}
              value={state.params.filters.warehouseId ?? ""}
              onChange={(value) => state.actions.setFilter("warehouseId", value || undefined)}
              options={(warehouses.data?.items ?? []).map((w) => ({ value: w.id ?? "", label: `${w.code ?? "-"} · ${w.name ?? "-"}` }))}
            />
          )}
          <ListFilterSelect
            id="stock-instock-filter"
            label={t("stockFilter")}
            value={state.params.filters.inStock ?? ""}
            onChange={(value) => state.actions.setFilter("inStock", value || undefined)}
            options={[{ value: "true", label: t("inStockOnly") }]}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <>
          <p className="text-xs text-erp-text-muted">{t("totalValueLine", { amount: formatCurrency(result.data?.totalValue ?? 0, "THB", locale) })}</p>
          <DataTable<StockBalanceResponse>
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
        </>
      )}

      <StockDocumentModal kind={documentKind} onClose={() => setDocumentKind(null)} defaultWarehouseId={state.params.filters.warehouseId || undefined} />
      <StockReserveModal balance={reserveFor} onClose={() => setReserveFor(null)} />
    </section>
  );
}
