"use client";

import React, { useMemo } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { Button } from "@/components/ui/Button";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { IconPlus } from "@/components/common/Icons";
import { LIST_PAGE_SIZES, useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import type { RoleDefinitionSummaryResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useRoleDefinitions } from "../api/role-management-queries";

interface RoleFilters extends ListFilterRecord {
  status?: string;
}

const STATUS_VALUES = ["active", "inactive"] as const;

/** Narrows a DataTable page-size value to the list state's allowed page sizes. */
const toListPageSize = (value: number): ListPageSize | undefined => LIST_PAGE_SIZES.find((size) => size === value);

export function RoleDefinitionList() {
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ROLES_MANAGE);

  const listState = useListState<RoleFilters>({
    schema: { defaultSort: "name", defaultOrder: "asc", single: ["status"], allowedSorts: ["name", "createdAt"] },
    debounceMs: 350,
  });
  const { search, page, limit, sort, order, filters } = listState.params;
  const { data, isLoading, isError, refetch } = useRoleDefinitions({
    search: search || undefined,
    status: filters.status || undefined,
    sortBy: sort,
    sortOrder: order,
    page,
    limit,
  });

  const roles = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 1;
  const isZeroRoles = !isLoading && !isError && totalItems === 0 && !search && !filters.status;
  const goCreate = () => router.push(`/${locale}/settings/roles/create`);

  const columns = useMemo<Column<RoleDefinitionSummaryResponse>[]>(
    () => [
      {
        id: "name",
        header: t("list.table.name"),
        sortable: true,
        accessorKey: "name",
        className: "min-w-[260px]",
        cell: (_v, row) => (
          <div className="flex flex-col gap-1">
            <TableEntityCell
              title={row.name ?? "-"}
              subtitle={row.description}
              href={row.id ? `/${locale}/settings/roles/${row.id}` : undefined}
            />
            <div className="flex flex-wrap gap-1">
              {row.isSystem ? <StatusBadge label={t("list.badges.system")} variant="info" /> : null}
              {row.requiresApproval ? <StatusBadge label={t("list.badges.requiresApproval")} variant="warning" /> : null}
              {row.hasPendingChange ? <StatusBadge label={t("list.badges.pendingChange")} variant="warning" /> : null}
            </div>
          </div>
        ),
      },
      { id: "members", header: t("list.table.members"), cell: (_v, row) => String(row.memberCount ?? "-") },
      { id: "permissions", header: t("list.table.permissions"), cell: (_v, row) => String(row.permissionCount ?? "-") },
      {
        id: "status",
        header: t("list.table.status"),
        cell: (_v, row) => (
          <StatusBadge
            label={row.isActive ? t("list.status.active") : t("list.status.inactive")}
            variant={row.isActive ? "success" : "neutral"}
          />
        ),
      },
    ],
    [locale, t]
  );

  return (
    <div className="space-y-6">
      <PageHeader
        title={t("list.title")}
        subtitle={t("list.subtitle")}
        actions={
          canManage ? (
            <Button variant="primary" size="md" icon={<IconPlus size={16} />} onClick={goCreate}>
              {t("list.createRole")}
            </Button>
          ) : undefined
        }
      />
      <ListToolbar>
        <div className="flex flex-wrap items-end gap-3">
          <ListSearchInput
            value={listState.draftSearch}
            isDebouncing={listState.isDebouncing}
            placeholder={t("list.searchPlaceholder")}
            onChange={(value) => listState.actions.setSearch(value)}
            onClear={() => listState.actions.setSearch("", true)}
            onSubmit={(value) => listState.actions.setSearch(value, true)}
          />
          <ListFilterSelect
            id="role-admin-status"
            label={t("list.statusFilter")}
            value={filters.status ?? ""}
            onChange={(value) => listState.actions.setFilter("status", value || undefined)}
            options={STATUS_VALUES.map((value) => ({ value, label: t(`list.status.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZeroRoles ? (
        <EmptyState
          icon="empty"
          title={t("list.emptyTitle")}
          description={t("list.emptyDescription")}
          actionLabel={canManage ? t("list.createRole") : undefined}
          onAction={canManage ? goCreate : undefined}
        />
      ) : (
        <DataTable<RoleDefinitionSummaryResponse>
          columns={columns}
          data={roles}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
          sorting={{ key: sort ?? null, order }}
          onSort={(key) => listState.actions.setSort(key)}
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
