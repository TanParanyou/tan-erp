"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatCurrency, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type {
  StockBalanceResponse,
  StockLocationBalanceResponse,
  StockSerialResponse,
} from "@/lib/api/api-client";
import {
  useStockBalances,
  useStockLocationBalances,
  useStockSerials,
  useWarehouseList,
} from "../api/inventory-queries";
import { StockDocumentModal, type StockDocumentKind } from "./stock-document-modal";
import { StockReserveModal } from "./stock-reserve-modal";
import { InventoryPeriodCloseModal } from "./inventory-period-close-modal";

interface BalanceFilters extends ListFilterRecord {
  warehouseId?: string;
  inStock?: string;
}

type BalanceView = "warehouse" | "location" | "serials";

const WAREHOUSE_PAGE_LIMIT = 100;

export function StockBalanceList() {
  const t = useTranslations("inventory.stock");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canIssue = can(selectedMembership, PERMISSIONS.INVENTORY_ISSUE);
  const canReceive = can(selectedMembership, PERMISSIONS.INVENTORY_RECEIVE);
  const canTransfer = can(selectedMembership, PERMISSIONS.INVENTORY_TRANSFER);
  const canAdjust = can(selectedMembership, PERMISSIONS.INVENTORY_ADJUST);
  const canReserve = can(selectedMembership, PERMISSIONS.INVENTORY_RESERVE);
  const canReadWarehouses = can(selectedMembership, PERMISSIONS.WAREHOUSES_READ);
  const canManageWarehouses = can(selectedMembership, PERMISSIONS.WAREHOUSES_MANAGE);

  const [view, setView] = useState<BalanceView>("warehouse");
  const [documentKind, setDocumentKind] = useState<StockDocumentKind | null>(null);
  const [reserveFor, setReserveFor] = useState<StockBalanceResponse | null>(null);
  const [showPeriodClose, setShowPeriodClose] = useState(false);

  const state = useListState<BalanceFilters>({
    schema: {
      single: ["warehouseId", "inStock"],
      defaultSort: "code",
      defaultOrder: "asc",
      allowedSorts: ["code"],
    },
    debounceMs: 300,
  });

  const warehouses = useWarehouseList(
    { page: 1, pageSize: WAREHOUSE_PAGE_LIMIT },
    canReadWarehouses
  );

  const warehouseBalances = useStockBalances({
    warehouseId: state.params.filters.warehouseId || undefined,
    search: state.params.search || undefined,
    inStockOnly: state.params.filters.inStock === "true",
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const locationBalances = useStockLocationBalances(
    {
      warehouseId: state.params.filters.warehouseId || undefined,
      itemId: undefined,
      inStockOnly: state.params.filters.inStock === "true",
      page: state.params.page,
      pageSize: state.params.limit,
    },
    view === "location"
  );

  const serialBalances = useStockSerials(
    {
      warehouseId: state.params.filters.warehouseId || undefined,
      search: state.params.search || undefined,
      page: state.params.page,
      pageSize: state.params.limit,
    },
    view === "serials"
  );

  function handleViewChange(nextView: BalanceView) {
    setView(nextView);
    state.actions.setPage(1);
  }

  const warehouseColumns: Column<StockBalanceResponse>[] = [
    {
      id: "warehouse",
      header: t("warehouse"),
      cell: (_v, row) => `${row.warehouse?.code ?? "-"} · ${row.warehouse?.name ?? "-"}`,
      className: "min-w-40",
    },
    {
      id: "item",
      header: t("item"),
      cell: (_v, row) => `${row.item?.code ?? "-"} · ${row.item?.nameTh ?? "-"}`,
      className: "min-w-52",
    },
    {
      id: "unit",
      header: t("unit"),
      cell: (_v, row) => row.item?.unitCode ?? "-",
    },
    {
      id: "onHand",
      header: t("onHand"),
      className: "text-right font-mono",
      cell: (_v, row) => formatNumber(row.onHand),
    },
    {
      id: "reserved",
      header: t("reserved"),
      className: "text-right font-mono",
      cell: (_v, row) => formatNumber(row.reserved),
    },
    {
      id: "available",
      header: t("available"),
      className: "text-right font-mono font-semibold",
      cell: (_v, row) => formatNumber(row.available),
    },
    {
      id: "averageCost",
      header: t("averageCost"),
      className: "text-right font-mono",
      cell: (_v, row) => formatCurrency(row.averageCost, "THB", locale),
    },
    {
      id: "totalValue",
      header: t("totalValue"),
      className: "text-right font-mono",
      cell: (_v, row) => formatCurrency(row.totalValue, "THB", locale),
    },
    {
      id: "actions",
      header: t("actions"),
      isAction: true,
      cell: (_v, row) =>
        canReserve && (row.available ?? 0) > 0 ? (
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="min-h-11"
            onClick={() => setReserveFor(row)}
          >
            {t("reserve")}
          </Button>
        ) : null,
    },
  ];

  const locationColumns: Column<StockLocationBalanceResponse>[] = [
    {
      id: "warehouse",
      header: t("warehouse"),
      cell: (_v, row) => `${row.warehouse?.code ?? "-"} · ${row.warehouse?.name ?? "-"}`,
      className: "min-w-36",
    },
    {
      id: "location",
      header: t("location"),
      className: "min-w-36 font-mono",
      cell: (_v, row) => (row.location?.code ? `${row.location.code} · ${row.location.name ?? ""}` : "-"),
    },
    {
      id: "item",
      header: t("item"),
      cell: (_v, row) => `${row.item?.code ?? "-"} · ${row.item?.nameTh ?? "-"}`,
      className: "min-w-48",
    },
    {
      id: "lotNumber",
      header: t("lotNumber"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => row.lotNumber ?? "-",
    },
    {
      id: "onHand",
      header: t("onHand"),
      className: "text-right font-mono font-semibold min-w-28",
      cell: (_v, row) => formatNumber(row.onHand),
    },
    {
      id: "updatedAt",
      header: t("updatedAt"),
      className: "min-w-36 text-xs",
      cell: (_v, row) => (row.updatedAtUtc ? formatDateTime(row.updatedAtUtc, locale) : "-"),
    },
  ];

  const serialColumns: Column<StockSerialResponse>[] = [
    {
      id: "serialNumber",
      header: t("serialNumber"),
      className: "font-mono font-semibold min-w-36",
      cell: (_v, row) => row.serialNumber ?? "-",
    },
    {
      id: "item",
      header: t("item"),
      cell: (_v, row) => `${row.item?.code ?? "-"} · ${row.item?.nameTh ?? "-"}`,
      className: "min-w-48",
    },
    {
      id: "warehouse",
      header: t("warehouse"),
      cell: (_v, row) => `${row.warehouse?.code ?? "-"}`,
      className: "min-w-28",
    },
    {
      id: "location",
      header: t("location"),
      className: "min-w-28 font-mono",
      cell: (_v, row) => row.location?.code ?? "-",
    },
    {
      id: "lotNumber",
      header: t("lotNumber"),
      className: "min-w-28 font-mono",
      cell: (_v, row) => row.lotNumber ?? "-",
    },
    {
      id: "status",
      header: t("status"),
      className: "min-w-28",
      cell: (_v, row) => (
        <StatusBadge
          label={
            row.status === "in_stock"
              ? t("serialStatuses.in_stock")
              : row.status === "issued"
              ? t("serialStatuses.issued")
              : row.status === "returned_out"
              ? t("serialStatuses.returned_out")
              : row.status ?? "-"
          }
          variant={
            row.status === "in_stock"
              ? "success"
              : row.status === "issued"
              ? "warning"
              : "neutral"
          }
        />
      ),
    },
    {
      id: "updatedAt",
      header: t("updatedAt"),
      className: "min-w-36 text-xs",
      cell: (_v, row) => (row.updatedAtUtc ? formatDateTime(row.updatedAtUtc, locale) : "-"),
    },
  ];

  const currentResult =
    view === "warehouse"
      ? warehouseBalances
      : view === "location"
      ? locationBalances
      : serialBalances;

  const currentPagination = currentResult.data?.pagination;
  const currentTotal = currentPagination?.totalCount ?? 0;

  return (
    <section className="space-y-5" aria-busy={currentResult.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={(
          <div className="flex flex-wrap gap-2">
            {canReceive && (
              <Button variant="outline" onClick={() => setDocumentKind("return")}>
                {t("return")}
              </Button>
            )}
            {canIssue && (
              <Button variant="outline" onClick={() => setDocumentKind("issue")}>
                {t("issue")}
              </Button>
            )}
            {canTransfer && (
              <Button variant="outline" onClick={() => setDocumentKind("transfer")}>
                {t("transfer")}
              </Button>
            )}
            {canAdjust && (
              <Button variant="outline" onClick={() => setDocumentKind("adjustment")}>
                {t("adjust")}
              </Button>
            )}
            {canManageWarehouses && (
              <Button variant="outline" onClick={() => setShowPeriodClose(true)}>
                {t("periodClose")}
              </Button>
            )}
          </div>
        )}
      />

      <div className="flex border-b border-erp-border gap-2">
        <button
          type="button"
          onClick={() => handleViewChange("warehouse")}
          className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
            view === "warehouse"
              ? "border-erp-navy text-erp-navy font-semibold"
              : "border-transparent text-erp-text-muted hover:text-erp-text-main"
          }`}
        >
          {t("views.warehouseBalances")}
        </button>
        <button
          type="button"
          onClick={() => handleViewChange("location")}
          className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
            view === "location"
              ? "border-erp-navy text-erp-navy font-semibold"
              : "border-transparent text-erp-text-muted hover:text-erp-text-main"
          }`}
        >
          {t("views.locationBalances")}
        </button>
        <button
          type="button"
          onClick={() => handleViewChange("serials")}
          className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
            view === "serials"
              ? "border-erp-navy text-erp-navy font-semibold"
              : "border-transparent text-erp-text-muted hover:text-erp-text-main"
          }`}
        >
          {t("views.serials")}
        </button>
      </div>

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
              options={(warehouses.data?.items ?? []).map((w) => ({
                value: w.id ?? "",
                label: `${w.code ?? "-"} · ${w.name ?? "-"}`,
              }))}
            />
          )}
          {view !== "serials" && (
            <ListFilterSelect
              id="stock-instock-filter"
              label={t("stockFilter")}
              value={state.params.filters.inStock ?? ""}
              onChange={(value) => state.actions.setFilter("inStock", value || undefined)}
              options={[{ value: "true", label: t("inStockOnly") }]}
            />
          )}
        </div>
      </ListToolbar>

      {view === "warehouse" && (
        <>
          <p className="text-xs text-erp-text-muted">
            {t("totalValueLine", {
              amount: formatCurrency(warehouseBalances.data?.totalValue ?? 0, "THB", locale),
            })}
          </p>
          <DataTable<StockBalanceResponse>
            columns={warehouseColumns}
            data={warehouseBalances.data?.items ?? []}
            isLoading={warehouseBalances.isLoading}
            isError={warehouseBalances.isError}
            error={warehouseBalances.error}
            onRetry={() => {
              void warehouseBalances.refetch();
            }}
            emptyTitle={t("noResults")}
            emptyDescription={t("search")}
            pagination={{
              page: state.params.page,
              limit: state.params.limit,
              totalPages: currentPagination?.totalPages ?? 0,
              total: currentTotal,
            }}
            onPageChange={state.actions.setPage}
            onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
          />
        </>
      )}

      {view === "location" && (
        <DataTable<StockLocationBalanceResponse>
          columns={locationColumns}
          data={locationBalances.data?.items ?? []}
          isLoading={locationBalances.isLoading}
          isError={locationBalances.isError}
          error={locationBalances.error}
          onRetry={() => {
            void locationBalances.refetch();
          }}
          emptyTitle={t("noResults")}
          emptyDescription={t("search")}
          pagination={{
            page: state.params.page,
            limit: state.params.limit,
            totalPages: currentPagination?.totalPages ?? 0,
            total: currentTotal,
          }}
          onPageChange={state.actions.setPage}
          onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
        />
      )}

      {view === "serials" && (
        <DataTable<StockSerialResponse>
          columns={serialColumns}
          data={serialBalances.data?.items ?? []}
          isLoading={serialBalances.isLoading}
          isError={serialBalances.isError}
          error={serialBalances.error}
          onRetry={() => {
            void serialBalances.refetch();
          }}
          emptyTitle={t("noResults")}
          emptyDescription={t("search")}
          pagination={{
            page: state.params.page,
            limit: state.params.limit,
            totalPages: currentPagination?.totalPages ?? 0,
            total: currentTotal,
          }}
          onPageChange={state.actions.setPage}
          onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
        />
      )}

      <StockDocumentModal
        kind={documentKind}
        onClose={() => setDocumentKind(null)}
        defaultWarehouseId={state.params.filters.warehouseId || undefined}
      />
      <StockReserveModal balance={reserveFor} onClose={() => setReserveFor(null)} />
      <InventoryPeriodCloseModal
        isOpen={showPeriodClose}
        onClose={() => setShowPeriodClose(false)}
      />
    </section>
  );
}
