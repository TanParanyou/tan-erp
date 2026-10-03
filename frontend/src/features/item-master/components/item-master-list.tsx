"use client";

import { useCallback, useEffect, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { EmptyState } from "@/components/ui/EmptyState";
import { Button } from "@/components/ui/Button";
import { BulkActionToolbar, BulkActionButton } from "@/components/ui/BulkActionToolbar";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconDownload, IconEdit, IconPlus } from "@/components/common/Icons";
import { Avatar } from "@/components/ui/Avatar";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useRowSelection } from "@/hooks/useRowSelection";
import { useCsvExport } from "@/hooks/useCsvExport";
import type { CsvColumn } from "@/lib/export/export-csv";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useItemList } from "@/features/item-master/api/item-master-queries";
import type { ItemResponse, ListItemsParams } from "@/lib/api/api-client";
import { apiClient } from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useToast } from "@/hooks/useToast";
import { itemFormSchema } from "@/features/item-master/schemas/item-form-schema";

interface ItemFilters extends ListFilterRecord {
  status?: string;
  itemType?: string;
}

function localizedName(name: ItemResponse["name"] | null, locale: string): string {
  if (locale === "en") return name?.english ?? "-";
  return name?.thai ?? "-";
}

export function ItemMasterList() {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const state = useListState<ItemFilters>({
    schema: { single: ["status", "itemType"], defaultSort: "code", defaultOrder: "asc", allowedSorts: ["code", "itemType", "status"] },
    debounceMs: 300,
  });
  const rowSelection = useRowSelection<string>();
  const query = useMemo<ListItemsParams>(() => ({
    search: state.params.search || undefined,
    itemType: state.params.filters.itemType || undefined,
    status: state.params.filters.status || undefined,
    sortBy: state.params.sort,
    sortOrder: state.params.order,
    pageNumber: state.params.page,
    pageSize: state.params.limit,
  }), [state.params.filters.itemType, state.params.filters.status, state.params.limit, state.params.order, state.params.page, state.params.search, state.params.sort]);
  const result = useItemList(query);
  const writable = can(selectedMembership, PERMISSIONS.ITEMS_CREATE);
  const canReadTaxonomy = can(selectedMembership, "items.read");
  const canReadCostSources = can(selectedMembership, "cost-sources.read");
  const rows = result.data?.items ?? [];
  const totalCount = result.data?.totalCount ?? 0;
  const totalPages = Math.ceil(totalCount / (result.data?.pageSize ?? state.params.limit));
  const isZeroItems = !result.isLoading && !result.isError && totalCount === 0
    && !state.params.search && !state.params.filters.status && !state.params.filters.itemType;

  useEffect(() => {
    rowSelection.clearSelection();
  }, [rowSelection.clearSelection, selectedMembership?.id, state.params.filters.itemType, state.params.filters.status, state.params.order, state.params.page, state.params.search, state.params.sort]);

  const itemTypeLabel = useCallback((itemType: string | null | undefined): string => {
    switch (itemType) {
      case "material": return t("itemTypeMaterial");
      case "labor": return t("itemTypeLabor");
      case "service": return t("itemTypeService");
      case "product": return t("itemTypeProduct");
      case "subcontract": return t("itemTypeSubcontract");
      case "other": return t("itemTypeOther");
      default: return "-";
    }
  }, [t]);

  const statusLabel = useCallback((status: string | null | undefined): string => {
    switch (status) {
      case "active": return t("statusActive");
      case "draft": return t("statusDraft");
      case "inactive": return t("statusInactive");
      default: return "-";
    }
  }, [t]);

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const filters: ActiveFilterChipItem[] = [];
    const itemType = state.params.filters.itemType;
    const status = state.params.filters.status;
    if (itemType) filters.push({ key: "itemType", value: itemType, label: `${t("type")}: ${itemTypeLabel(itemType)}` });
    if (status) filters.push({ key: "status", value: status, label: `${t("status")}: ${statusLabel(status)}` });
    return filters;
  }, [itemTypeLabel, state.params.filters.itemType, state.params.filters.status, statusLabel, t]);

  const removeFilter = (key: string) => {
    if (key === "itemType") state.actions.setFilter("itemType", undefined);
    if (key === "status") state.actions.setFilter("status", undefined);
  };

  const csvColumns = useMemo<CsvColumn<ItemResponse>[]>(() => [
    { header: t("code"), accessor: (item) => item.code ?? "" },
    { header: t("name"), accessor: (item) => localizedName(item.name, locale) },
    { header: t("type"), accessor: (item) => itemTypeLabel(item.itemType) },
    { header: t("category"), accessor: (item) => localizedName(item.category?.name ?? null, locale) },
    { header: t("brand"), accessor: (item) => localizedName(item.brand?.name ?? null, locale) },
    { header: t("unit"), accessor: (item) => item.baseUnit?.code ?? "" },
    { header: t("status"), accessor: (item) => statusLabel(item.status) },
  ], [itemTypeLabel, locale, statusLabel, t]);

  const fetchAllItems = useCallback(async () => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    const membershipId = selectedMembership?.id;
    if (!membershipId) throw new MembershipRequiredError();

    const params: ListItemsParams = {
      search: state.params.search || undefined,
      itemType: state.params.filters.itemType || undefined,
      status: state.params.filters.status || undefined,
      sortBy: state.params.sort,
      sortOrder: state.params.order,
      pageNumber: 1,
      pageSize: 100,
    };
    const items: ItemResponse[] = [];
    let pageNumber = 1;
    let totalCount: number | undefined;
    do {
      const response = await apiClient.listItems({
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
      }, { ...params, pageNumber });
      if (!response.items || response.totalCount === undefined) throw new Error("Item export response is incomplete");
      items.push(...response.items);
      totalCount = response.totalCount;
      pageNumber += 1;
      if (response.items.length === 0 && items.length < totalCount) throw new Error("Item export response ended before all rows were loaded");
    } while (items.length < totalCount);
    return items;
  }, [locale, selectedMembership?.id, state.params.filters.itemType, state.params.filters.status, state.params.order, state.params.search, state.params.sort]);

  const handleExportAll = async () => {
    try {
      await exportAll();
    } catch {
      toast.error(t("exportFailed"));
    }
  };

  const { exportAll, exportSelected, isExporting } = useCsvExport<ItemResponse>({
    filename: "items",
    columns: csvColumns,
    data: rows,
    selectedIds: rowSelection.selectedIds,
    getId: (item) => item.id,
    fetchAll: fetchAllItems,
  });

  const columns = useMemo<Column<ItemResponse>[]>(() => [
    {
      id: "image",
      header: t("image"),
      cell: (_value, row) => {
        const name = localizedName(row.name, locale);
        const altText = locale === "en" ? row.primaryImage?.altText?.english : row.primaryImage?.altText?.thai;
        return <Avatar initial={name} fileId={row.primaryImage?.fileId} alt={altText ?? name} size="md" />;
      },
      className: "w-16",
    },
    { id: "code", header: t("code"), accessorKey: "code", sortable: true, className: "min-w-36 font-mono" },
    { id: "name", header: t("name"), cell: (_value, row) => localizedName(row.name, locale), className: "min-w-48" },
    { id: "itemType", header: t("type"), accessorKey: "itemType", sortable: true, cell: (_value, row) => itemTypeLabel(row.itemType), className: "min-w-36" },
    { id: "category", header: t("category"), cell: (_value, row) => row.category?.name ? localizedName(row.category.name, locale) : "-" },
    { id: "unit", header: t("unit"), cell: (_value, row) => row.baseUnit?.code ?? "-" },
    { id: "status", header: t("status"), accessorKey: "status", sortable: true, cell: (_value, row) => statusLabel(row.status) },
    {
      id: "actions",
      header: t("rowActions"),
      isAction: true,
      sticky: "right",
      cell: (_value, row) => (
        <TableActionGroup>
          <TableAction
            icon={<IconEdit size={15} />}
            label={t("edit")}
            href={`/${locale}/item-master/${row.id}`}
          />
        </TableActionGroup>
      ),
    },
  ], [itemTypeLabel, locale, statusLabel, t]);

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={<div className="flex flex-wrap gap-2">
          {canReadTaxonomy && <Button variant="outline" href={`/${locale}/item-master/reference-data`}>{t("referenceData")}</Button>}
          {canReadCostSources && <Button variant="outline" href={`/${locale}/item-master/cost-sources`}>{t("costSources")}</Button>}
          <Button variant="outline" href={`/${locale}/item-master/cost-reviews`}>{t("reviewQueue")}</Button>
          {writable && <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/item-master/create`}>{t("create")}</Button>}
        </div>}
      />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={removeFilter} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="item-master-search"
            label={common("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          <ListFilterSelect
            id="item-master-type-filter"
            label={t("type")}
            value={state.params.filters.itemType ?? ""}
            onChange={(value) => state.actions.setFilter("itemType", value || undefined)}
            options={itemFormSchema.shape.itemType.options.map((value) => ({ value, label: itemTypeLabel(value) }))}
          />
          <ListFilterSelect
            id="item-master-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={["active", "draft", "inactive"].map((value) => ({ value, label: statusLabel(value) }))}
          />
          <Button type="button" variant="outline" size="sm" icon={<IconDownload size={15} />} isLoading={isExporting} disabled={totalCount === 0} onClick={() => { void handleExportAll(); }}>
            {t("exportCsv")}
          </Button>
        </div>
      </ListToolbar>
      {isZeroItems ? (
        <EmptyState
          icon="empty"
          title={t("empty")}
          description={t("emptyDetail")}
          actionLabel={writable ? t("create") : undefined}
          onAction={writable ? () => router.push(`/${locale}/item-master/create`) : undefined}
        />
      ) : (
        <div className="flex flex-col gap-4">
          <DataTable<ItemResponse>
            columns={columns}
            data={rows}
            isLoading={result.isLoading}
            isError={result.isError}
            error={result.error}
            onRetry={() => { void result.refetch(); }}
            emptyTitle={t("noResults")}
            emptyDescription={t("search")}
            selectable
            selectedIds={rowSelection.selectedIds}
            onSelect={(id) => rowSelection.toggleSelection(String(id))}
            onSelectAll={(ids) => rowSelection.selectAll(ids.map(String))}
            pagination={{ page: state.params.page, limit: state.params.limit, totalPages, total: totalCount }}
            sorting={{ key: state.params.sort ?? null, order: state.params.order }}
            onSort={state.actions.setSort}
            onPageChange={state.actions.setPage}
            onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
            stickyActionColumn
          />
          <BulkActionToolbar selectedCount={rowSelection.selectedCount} onClear={rowSelection.clearSelection}>
            <BulkActionButton
              icon={<IconDownload size={15} />}
              label={t("exportSelected", { count: rowSelection.selectedCount })}
              onClick={exportSelected}
              showLabel
            />
          </BulkActionToolbar>
        </div>
      )}
    </section>
  );
}
