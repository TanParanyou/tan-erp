"use client";

import React, { useState, useMemo, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerList } from "../api/customer-queries";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { BulkActionToolbar, BulkActionButton } from "@/components/ui/BulkActionToolbar";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { CustomerQuickViewDrawer } from "./customer-quick-view-drawer";
import {
  IconSearch,
  IconPlus,
  IconAlertCircle,
  IconEye,
  IconEdit,
  IconDownload,
  IconClose,
} from "@/components/common/Icons";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { getCustomerStatusLabelKey, getCustomerTypeLabelKey } from "../customer-labels";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useRowSelection } from "@/hooks/useRowSelection";
import { useCsvExport } from "@/hooks/useCsvExport";
import type { CsvColumn } from "@/lib/export/export-csv";
import { apiClient, type CustomerListItemResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";

interface CustomerFilters extends ListFilterRecord {
  status?: string;
  type?: string;
}

export function CustomerList() {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();

  // Full URL-synced List State Hook
  const listState = useListState<CustomerFilters>({
    schema: {
      defaultSort: "code",
      defaultOrder: "desc",
      single: ["status", "type"],
      allowedSorts: ["code", "displayNameTh", "customerType", "status"],
    },
    debounceMs: 350,
  });

  // Quick View Drawer State
  const [quickViewCustomerId, setQuickViewCustomerId] = useState<string | null>(null);

  // Row Selection Hook
  const {
    selectedIds,
    selectedCount,
    toggleSelection,
    selectAll,
    clearSelection,
  } = useRowSelection<string | number>();

  const canCreate =
    can(selectedMembership, "customers.create") &&
    can(selectedMembership, "customer-contacts.manage");

  const resolveCustomerTypeLabel = (value: string | null | undefined): string => {
    const key = getCustomerTypeLabelKey(value);
    return key ? t(key) : "-";
  };

  const resolveCustomerStatusLabel = (value: string | null | undefined): string => {
    const key = getCustomerStatusLabelKey(value);
    return key ? tCommon(`status.${key}`) : "-";
  };

  const resolveListErrorMessage = (error: Error | null): string => {
    if (error?.message === "No authentication token available") {
      return t("errors.authenticationRequired");
    }
    if (error?.message === "No active membership selected") {
      return t("errors.membershipRequired");
    }
    return t("errors.loadList");
  };

  const {
    data,
    isLoading,
    isError,
    error,
    refetch,
  } = useCustomerList({
    search: listState.params.search || undefined,
    status: listState.params.filters.status || undefined,
    customerType: listState.params.filters.type || undefined,
    sortBy: listState.params.sort || undefined,
    sortOrder: listState.params.order || undefined,
    page: listState.params.page,
    limit: listState.params.limit,
  });

  // Direct server-side data & pagination values
  const customers = data?.items ?? [];
  const totalItems = data?.totalCount ?? 0;
  const totalPages = data?.totalPages ?? (Math.ceil(totalItems / listState.params.limit) || 1);

  // Active Filter Chips
  const activeChips = useMemo<ActiveFilterChipItem[]>(() => {
    const chips: ActiveFilterChipItem[] = [];
    if (listState.params.filters.status) {
      chips.push({
        key: "status",
        value: listState.params.filters.status,
        label: `${t("statusFilterLabel")}: ${resolveCustomerStatusLabel(listState.params.filters.status)}`,
      });
    }
    if (listState.params.filters.type) {
      chips.push({
        key: "type",
        value: listState.params.filters.type,
        label: `${t("typeFilterLabel")}: ${resolveCustomerTypeLabel(listState.params.filters.type)}`,
      });
    }
    return chips;
  }, [listState.params.filters.status, listState.params.filters.type]);

  const handleRemoveChip = (key: string, value: string) => {
    listState.actions.removeFilterValue(key as "status" | "type", value);
  };

  const handleClearAllFilters = () => {
    listState.actions.clearFilters();
  };

  // Reusable CSV Export Hook
  const csvColumns = useMemo<CsvColumn<CustomerListItemResponse>[]>(
    () => [
      { header: t("code"), accessor: (c) => c.code ?? "" },
      {
        header: tCommon("fields.name"),
        accessor: (c) => (locale === "en" ? c.displayNameEn || c.displayNameTh : c.displayNameTh || c.displayNameEn) ?? "",
      },
      { header: t("customerType"), accessor: (c) => resolveCustomerTypeLabel(c.customerType) },
      { header: t("primaryContact"), accessor: (c) => c.primaryContact?.name ?? "" },
      { header: t("phone"), accessor: (c) => c.primaryContact?.phone ?? "" },
      { header: t("email"), accessor: (c) => c.primaryContact?.email ?? "" },
      { header: t("status"), accessor: (c) => resolveCustomerStatusLabel(c.status) },
    ],
    [locale, t, tCommon]
  );

  const { exportAll, exportSelected, isExporting } = useCsvExport<CustomerListItemResponse>({
    filename: "customers",
    columns: csvColumns,
    data: customers,
    selectedIds,
    getId: (c) => c.id,
    fetchAll: async () => {
      const token = await getAuthToken();
      if (!token || !selectedMembership?.id) return customers;
      const res = await apiClient.listCustomers(
        {
          token,
          membershipId: selectedMembership.id,
          locale: locale === "en" ? "en" : "th",
        },
        {
          search: listState.params.search || undefined,
          status: listState.params.filters.status || undefined,
          customerType: listState.params.filters.type || undefined,
          sortBy: listState.params.sort || undefined,
          sortOrder: listState.params.order || undefined,
          page: 1,
          limit: 1000,
        }
      );
      return res.items ?? [];
    },
  });

  // Table Columns Definition with Sticky Right Action Column
  const columns = useMemo<Column<CustomerListItemResponse>[]>(
    () => [
      {
        id: "avatar",
        header: "",
        className: "w-[52px]",
        cell: (_value: unknown, customer: CustomerListItemResponse) => {
          const isOrg = customer.customerType === "organization";
          return (
            <Avatar
              initial={customer.displayNameTh || customer.displayNameEn || undefined}
              variant={isOrg ? "navy" : "muted"}
              size="md"
              title={resolveCustomerTypeLabel(customer.customerType)}
            />
          );
        },
      },
      {
        id: "customerDetails",
        header: `${tCommon("fields.name")} / ${t("code")}`,
        className: "min-w-[240px]",
        sortable: true,
        accessorKey: "displayNameTh",
        cell: (_value: unknown, customer: CustomerListItemResponse) => {
          const customerName =
            locale === "en" && customer.displayNameEn
              ? customer.displayNameEn
              : customer.displayNameTh || customer.displayNameEn || "-";
          const secondaryName =
            customer.displayNameEn && customer.displayNameTh
              ? locale === "en"
                ? customer.displayNameTh
                : customer.displayNameEn
              : null;

          return (
            <TableEntityCell
              title={customerName}
              code={customer.code}
              subtitle={secondaryName}
              href={`/${locale}/customers/${customer.id}`}
            />
          );
        },
      },
      {
        id: "customerType",
        header: t("customerType"),
        className: "min-w-[130px]",
        sortable: true,
        accessorKey: "customerType",
        cell: (_value: unknown, customer: CustomerListItemResponse) => (
          <Badge variant="neutral" size="sm">
            {resolveCustomerTypeLabel(customer.customerType)}
          </Badge>
        ),
      },
      {
        id: "primaryContact",
        header: t("primaryContact"),
        className: "min-w-[220px]",
        cell: (_value: unknown, customer: CustomerListItemResponse) => {
          const contact = customer.primaryContact;
          if (!contact?.name && !contact?.phone && !contact?.email) {
            return <span className="text-erp-text-muted">-</span>;
          }
          return (
            <div className="flex flex-col gap-0.5">
              <span className="font-semibold text-erp-text-main text-xs">{contact.name || "-"}</span>
              <span className="font-mono text-[11px] text-erp-text-muted">
                {[contact.phone, contact.email].filter(Boolean).join(" • ") || "-"}
              </span>
            </div>
          );
        },
      },
      {
        id: "status",
        header: t("status"),
        className: "min-w-[120px]",
        sortable: true,
        accessorKey: "status",
        cell: (_value: unknown, customer: CustomerListItemResponse) => {
          const statusKey = getCustomerStatusLabelKey(customer.status);
          const label = statusKey ? tCommon(`status.${statusKey}`) : "-";
          return (
            <StatusBadge
              label={label}
              variant={customer.status === "active" ? "success" : "warning"}
            />
          );
        },
      },
      {
        id: "actions",
        header: tCommon("fields.actions"),
        className: "w-[100px]",
        sticky: "right",
        isAction: true,
        cell: (_value: unknown, customer: CustomerListItemResponse) => (
          <TableActionGroup>
            <TableAction
              icon={<IconEye size={15} />}
              label={t("quickView")}
              onClick={() => setQuickViewCustomerId(customer.id ?? null)}
            />
            <TableAction
              icon={<IconEdit size={15} />}
              label={tCommon("actions.edit")}
              href={`/${locale}/customers/${customer.id}`}
            />
          </TableActionGroup>
        ),
      },
    ],
    [locale, t, tCommon]
  );

  const isZeroCustomers =
    !isLoading &&
    !isError &&
    totalItems === 0 &&
    !listState.params.search &&
    !listState.params.filters.status &&
    !listState.params.filters.type;

  return (
    <div className="flex flex-col gap-5">
      {/* 2-Tier Architectural Page Header */}
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        actions={
          canCreate ? (
            <Button
              href={`/${locale}/customers/create`}
              variant="primary"
              size="md"
              icon={<IconPlus size={16} />}
              className="min-h-[40px] font-semibold"
            >
              {t("createCustomer")}
            </Button>
          ) : undefined
        }
      />

      {/* Advanced List Toolbar */}
      <ListToolbar
        activeFilters={
          <ActiveFilterChips
            filters={activeChips}
            onRemove={handleRemoveChip}
            onClear={handleClearAllFilters}
          />
        }
      >
        {/* Search & Filters Group */}
        <div className="flex flex-wrap items-end gap-3">
          {/* Search Input with Reusable ListSearchInput */}
          <ListSearchInput
            id="customer-search-input"
            label={t("searchLabel")}
            value={listState.draftSearch}
            isDebouncing={listState.isDebouncing}
            placeholder={t("searchPlaceholder")}
            onChange={(val) => listState.actions.setSearch(val)}
            onClear={() => listState.actions.setSearch("", true)}
            onSubmit={(val) => listState.actions.setSearch(val, true)}
            widthClassName="w-full sm:w-72"
          />

          {/* Status Filter */}
          <ListFilterSelect
            id="filter-status-select"
            label={t("statusFilterLabel")}
            value={listState.params.filters.status || ""}
            onChange={(val) => listState.actions.setFilter("status", val || undefined)}
            options={[
              { value: "active", label: tCommon("status.active") },
              { value: "draft", label: tCommon("status.draft") },
            ]}
            widthClassName="w-full sm:w-40"
          />

          {/* Customer Type Filter */}
          <ListFilterSelect
            id="filter-type-select"
            label={t("typeFilterLabel")}
            value={listState.params.filters.type || ""}
            onChange={(val) => listState.actions.setFilter("type", val || undefined)}
            options={[
              { value: "organization", label: t("organization") },
              { value: "person", label: t("person") },
            ]}
            widthClassName="w-full sm:w-44"
          />
        </div>

        {/* Export CSV Button (Aligned to bottom edge of inputs) */}
        <div className="flex items-end self-end">
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={exportAll}
            isLoading={isExporting}
            disabled={totalItems === 0}
            icon={<IconDownload size={15} />}
            className="h-10 text-xs font-medium min-h-[40px]"
          >
            {t("exportCsv")}
          </Button>
        </div>
      </ListToolbar>

      {/* Main Content Area: Table-Preserved Architecture */}
      {isZeroCustomers ? (
        <EmptyState
          icon="empty"
          title={t("emptyTitle")}
          description={t("emptyDetail")}
          actionLabel={canCreate ? t("createCustomer") : undefined}
          onAction={canCreate ? () => router.push(`/${locale}/customers/create`) : undefined}
        />
      ) : (
        <div className="flex flex-col gap-4">
          {/* Dense Architectural DataTable with Lock-Scroll Action Column & Full Pagination Footer */}
          <DataTable<CustomerListItemResponse>
            columns={columns}
            data={customers}
            isLoading={isLoading}
            isError={isError}
            error={isError ? resolveListErrorMessage(error) : null}
            onRetry={() => refetch()}
            emptyTitle={tCommon("table.noData")}
            emptyDescription={t("searchPlaceholder")}
            selectable={true}
            selectedIds={selectedIds}
            onSelect={(id) => toggleSelection(id)}
            onSelectAll={(ids) => selectAll(ids)}
            sorting={{
              key: listState.params.sort || null,
              order: listState.params.order,
            }}
            onSort={(key) => listState.actions.setSort(key)}
            pagination={{
              page: listState.params.page,
              limit: listState.params.limit,
              totalPages,
              totalItems,
            }}
            onPageChange={(p) => listState.actions.setPage(p)}
            onLimitChange={(lim) => listState.actions.setLimit(lim as ListPageSize)}
            stickyActionColumn={true}
            className="shadow-sm rounded-none border border-erp-border bg-erp-surface"
          />

          {/* Floating Bulk Action Toolbar */}
          <BulkActionToolbar selectedCount={selectedCount} onClear={clearSelection}>
            <BulkActionButton
              icon={<IconDownload size={15} />}
              label={t("exportSelected", { count: selectedCount })}
              onClick={exportSelected}
              showLabel={true}
              variant="default"
            />
          </BulkActionToolbar>
        </div>
      )}

      {/* Quick View Drawer */}
      <CustomerQuickViewDrawer
        customerId={quickViewCustomerId}
        isOpen={Boolean(quickViewCustomerId)}
        onClose={() => setQuickViewCustomerId(null)}
      />
    </div>
  );
}

