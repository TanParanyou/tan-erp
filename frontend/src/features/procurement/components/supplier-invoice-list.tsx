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
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { SupplierInvoiceListItemResponse } from "@/lib/api/api-client";
import { useSupplierInvoiceList } from "../api/procurement-queries";
import {
  isSupplierInvoiceMatchStatus,
  isSupplierInvoiceStatus,
  SUPPLIER_INVOICE_STATUSES,
  supplierInvoiceMatchStatusVariant,
  supplierInvoiceStatusVariant,
} from "../procurement-status";

interface InvoiceFilters extends ListFilterRecord {
  status?: string;
}

export function SupplierInvoiceList() {
  const t = useTranslations("procurement.invoices");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_CREATE);

  const state = useListState<InvoiceFilters>({
    schema: { single: ["status"], defaultSort: "createdAt", defaultOrder: "desc", allowedSorts: ["createdAt"] },
    debounceMs: 300,
  });

  const result = useSupplierInvoiceList({
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
    return status && isSupplierInvoiceStatus(status)
      ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }]
      : [];
  }, [state.params.filters.status, t]);

  const columns: Column<SupplierInvoiceListItemResponse>[] = [
    {
      id: "number",
      header: t("number"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => (
        <Link href={`/${locale}/procurement/supplier-invoices/${row.id}`} className="font-mono font-semibold text-erp-navy underline">
          {row.number ?? "-"}
        </Link>
      ),
    },
    {
      id: "supplierInvoiceNumber",
      header: t("supplierInvoiceNumber"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => row.supplierInvoiceNumber ?? "-",
    },
    {
      id: "purchaseOrder",
      header: t("purchaseOrder"),
      className: "min-w-32 font-mono",
      cell: (_v, row) =>
        row.purchaseOrderId && row.purchaseOrderNumber ? (
          <Link href={`/${locale}/procurement/purchase-orders/${row.purchaseOrderId}`} className="font-mono text-erp-navy underline">
            {row.purchaseOrderNumber}
          </Link>
        ) : (
          "-"
        ),
    },
    {
      id: "supplier",
      header: t("supplier"),
      className: "min-w-44",
      cell: (_v, row) => `${row.supplierCode ?? "-"} · ${row.supplierNameTh ?? "-"}`,
    },
    {
      id: "status",
      header: t("status"),
      cell: (_v, row) => (
        <StatusBadge
          label={isSupplierInvoiceStatus(row.status) ? t(`statuses.${row.status}`) : "-"}
          variant={supplierInvoiceStatusVariant(row.status)}
        />
      ),
    },
    {
      id: "matchStatus",
      header: t("matchStatus"),
      cell: (_v, row) =>
        row.matchStatus && isSupplierInvoiceMatchStatus(row.matchStatus) ? (
          <StatusBadge
            label={t(`matchStatuses.${row.matchStatus}`)}
            variant={supplierInvoiceMatchStatusVariant(row.matchStatus)}
          />
        ) : (
          "-"
        ),
    },
    {
      id: "total",
      header: t("total"),
      className: "text-right font-mono",
      cell: (_v, row) => formatCurrency(row.totalAmount, "THB", locale),
    },
    {
      id: "variance",
      header: t("variance"),
      className: "text-right font-mono",
      cell: (_v, row) =>
        Boolean(row.totalVarianceAmount) ? (
          <span className="text-erp-warning font-mono font-medium">
            {formatCurrency(row.totalVarianceAmount, "THB", locale)}
          </span>
        ) : (
          <span className="text-erp-text-muted">-</span>
        ),
    },
    {
      id: "invoiceDate",
      header: t("invoiceDate"),
      cell: (_v, row) => (row.invoiceDate ? formatDate(row.invoiceDate, locale) : "-"),
    },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={
          canCreate ? (
            <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/procurement/supplier-invoices/create`}>
              {t("create")}
            </Button>
          ) : undefined
        }
      />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="supplier-invoice-search"
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
            id="supplier-invoice-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={SUPPLIER_INVOICE_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<SupplierInvoiceListItemResponse>
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
