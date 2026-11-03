"use client";

import { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useTranslations, useLocale } from "next-intl";
import { useQueryClient } from "@tanstack/react-query";
import { useCustomerList, customerQueryRootKey } from "../api/customer-queries";
import { getCustomerQueryErrorMessage } from "../customer-errors";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { BulkActionToolbar } from "@/components/ui/BulkActionToolbar";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { Modal } from "@/components/ui/Modal";
import { Input } from "@/components/ui/Input";
import { CustomerQuickViewDrawer } from "./customer-quick-view-drawer";
import {
  IconPlus,
  IconEye,
  IconEdit,
  IconLock,
} from "@/components/common/Icons";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { getCustomerDisplayNames, getCustomerStatusLabelKey, getCustomerTypeLabelKey } from "../customer-labels";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useRowSelection } from "@/hooks/useRowSelection";
import { useDataExport } from "@/hooks/useDataExport";
import type { ExportColumn } from "@/lib/export/export-types";
import { ExportDropdown } from "@/components/ui/ExportDropdown";
import { apiClient, type CustomerListItemResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useToast } from "@/hooks/useToast";
import { getApiErrorMessage } from "@/lib/api/api-error";

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

  const { toast } = useToast();
  const queryClient = useQueryClient();

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

  // Deactivate Customer Modal State
  const [deactivatingCustomer, setDeactivatingCustomer] = useState<CustomerListItemResponse | null>(null);
  const [deactivateReason, setDeactivateReason] = useState("");
  const [isDeactivating, setIsDeactivating] = useState(false);

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

  const canDeactivate = can(selectedMembership, "customers.deactivate");

  const handleDeactivate = async () => {
    if (!deactivatingCustomer?.id || !deactivateReason.trim()) return;
    const token = await getAuthToken();
    const membershipId = selectedMembership?.id;
    if (!token || !membershipId) {
      toast.error(!token ? t("errors.authenticationRequired") : t("errors.membershipRequired"));
      return;
    }

    setIsDeactivating(true);
    try {
      // Fetch fresh customer details to get the exact rowVersion for If-Match
      const customerDetail = await apiClient.getCustomer(deactivatingCustomer.id, {
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
      });

      if (!customerDetail.rowVersion) {
        throw new Error(t("errors.saveUnexpected"));
      }

      await apiClient.deactivateCustomer(
        deactivatingCustomer.id,
        { reason: deactivateReason.trim() },
        {
          token,
          membershipId,
          ifMatch: customerDetail.rowVersion,
          idempotencyKey: crypto.randomUUID(),
          locale: locale === "en" ? "en" : "th",
        }
      );

      setDeactivatingCustomer(null);
      setDeactivateReason("");
      toast.success(t("deactivateSuccess"));
      await queryClient.invalidateQueries({
        queryKey: customerQueryRootKey(membershipId, locale === "en" ? "en" : "th"),
      });
      await refetch();
    } catch (err: unknown) {
      toast.error(getApiErrorMessage(err, t("errors.saveUnexpected")));
    } finally {
      setIsDeactivating(false);
    }
  };

  const resolveCustomerTypeLabel = (value: string | null | undefined): string => {
    const key = getCustomerTypeLabelKey(value);
    return key ? t(key) : "-";
  };

  const resolveCustomerStatusLabel = (value: string | null | undefined): string => {
    const key = getCustomerStatusLabelKey(value);
    return key ? tCommon(`status.${key}`) : "-";
  };

  const resolveListErrorMessage = (error: Error | null): string => {
    return getCustomerQueryErrorMessage(error, {
      authenticationRequired: t("errors.authenticationRequired"),
      membershipRequired: t("errors.membershipRequired"),
      fallback: t("errors.loadList"),
    });
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
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? (Math.ceil(totalItems / listState.params.limit) || 1);

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

  // Reusable Data Export Hook (CSV & Excel)
  const exportColumns = useMemo<ExportColumn<CustomerListItemResponse>[]>(
    () => [
      { header: t("code"), accessor: (c) => c.code ?? "" },
      {
        header: tCommon("fields.name"),
        accessor: (c) => getCustomerDisplayNames(c.displayNameTh, c.displayNameEn, locale === "en" ? "en" : "th", "").primary,
      },
      { header: t("customerType"), accessor: (c) => resolveCustomerTypeLabel(c.customerType) },
      { header: t("primaryContact"), accessor: (c) => c.primaryContact?.name ?? "" },
      { header: t("phone"), accessor: (c) => c.primaryContact?.phone ?? "" },
      { header: t("email"), accessor: (c) => c.primaryContact?.email ?? "" },
      { header: t("status"), accessor: (c) => resolveCustomerStatusLabel(c.status) },
    ],
    [locale, t, tCommon]
  );

  const { exportAll, exportSelected, isExporting } = useDataExport<CustomerListItemResponse>({
    filename: "customers",
    columns: exportColumns,
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
          const { primary } = getCustomerDisplayNames(
            customer.displayNameTh,
            customer.displayNameEn,
            locale === "en" ? "en" : "th",
          );
          return (
            <Avatar
              initial={primary !== "-" ? primary : undefined}
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
          const { primary: customerName, secondary: secondaryName } = getCustomerDisplayNames(
            customer.displayNameTh,
            customer.displayNameEn,
            locale === "en" ? "en" : "th",
          );

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
        className: "w-[120px]",
        sticky: "right",
        isAction: true,
        cell: (_value: unknown, customer: CustomerListItemResponse) => {
          const isCustomerActive = customer.status === "active";
          return (
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
              {canDeactivate && isCustomerActive && (
                <TableAction
                  icon={<IconLock size={15} />}
                  label={t("deactivateAction")}
                  variant="danger"
                  onClick={() => {
                    setDeactivatingCustomer(customer);
                    setDeactivateReason("");
                  }}
                />
              )}
            </TableActionGroup>
          );
        },
      },
    ],
    [canDeactivate, locale, t, tCommon]
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
              className="font-semibold"
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

        {/* Export Dropdown Button (CSV & Excel, Aligned to bottom edge of inputs) */}
        <div className="flex items-end self-end">
          <ExportDropdown
            onExport={(format) => exportAll(format)}
            isLoading={isExporting}
            disabled={totalItems === 0}
            label={t("export")}
            variant="outline"
            size="sm"
          />
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
            <ExportDropdown
              onExport={(format) => exportSelected(format)}
              isLoading={isExporting}
              label={t("exportSelected", { count: selectedCount })}
              variant="bulk"
              direction="up"
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

      {/* Deactivate Customer Modal */}
      <Modal
        isOpen={Boolean(deactivatingCustomer)}
        onClose={() => {
          if (!isDeactivating) {
            setDeactivatingCustomer(null);
            setDeactivateReason("");
          }
        }}
        title={t("deactivateModalTitle")}
        description={t("deactivateModalDesc")}
        closeDisabled={isDeactivating}
      >
        <div className="flex flex-col gap-5 p-5">
          <Input
            label={t("deactivateReason")}
            required
            maxLength={500}
            placeholder={t("deactivateReasonPlaceholder")}
            value={deactivateReason}
            onChange={(event) => setDeactivateReason(event.target.value)}
          />
          <div className="flex justify-end gap-2">
            <Button
              variant="outline"
              disabled={isDeactivating}
              onClick={() => {
                setDeactivatingCustomer(null);
                setDeactivateReason("");
              }}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              variant="danger"
              isLoading={isDeactivating}
              disabled={isDeactivating || !deactivateReason.trim()}
              onClick={() => void handleDeactivate()}
            >
              {tCommon("actions.confirm")}
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
