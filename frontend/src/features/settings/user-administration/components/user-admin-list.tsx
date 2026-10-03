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
import { StatusBadge, type BadgeVariant } from "@/components/ui/StatusBadge";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { IconEye, IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import type { AdminUserResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useAdminUsers } from "../api/user-admin-queries";

interface UserAdminFilters extends ListFilterRecord {
  status?: string;
}

const STATUS_VARIANTS: Record<string, BadgeVariant> = {
  active: "success",
  pending: "warning",
  inactive: "neutral",
};

const STATUS_VALUES = ["pending", "active", "inactive"] as const;

export function UserAdminList() {
  const t = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();

  const canCreate =
    can(selectedMembership, PERMISSIONS.USERS_MANAGE) &&
    can(selectedMembership, PERMISSIONS.MEMBERSHIPS_MANAGE) &&
    can(selectedMembership, PERMISSIONS.ROLES_ASSIGN);

  const listState = useListState<UserAdminFilters>({
    schema: {
      defaultSort: "createdAt",
      defaultOrder: "desc",
      single: ["status"],
      allowedSorts: ["displayName", "email", "createdAt"],
    },
    debounceMs: 350,
  });
  const { search, page, limit, sort, order, filters } = listState.params;

  const { data, isLoading, isError, refetch } = useAdminUsers({
    search: search || undefined,
    status: filters.status || undefined,
    sortBy: sort,
    sortOrder: order,
    page,
    limit,
  });

  const users = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 1;
  const hasFilters = Boolean(search || filters.status);
  const isZeroUsers = !isLoading && !isError && totalItems === 0 && !hasFilters;

  const columns = useMemo<Column<AdminUserResponse>[]>(
    () => [
      {
        id: "user",
        header: t("table.user"),
        sortable: true,
        accessorKey: "displayName",
        className: "min-w-[240px]",
        cell: (_value, user) => (
          <TableEntityCell
            title={user.displayName ?? "-"}
            subtitle={user.email}
            href={`/${locale}/settings/users/${user.id}`}
          />
        ),
      },
      {
        id: "status",
        header: t("table.status"),
        className: "min-w-[120px]",
        cell: (_value, user) => (
          <StatusBadge
            label={t(`status.${user.status ?? "inactive"}`)}
            variant={STATUS_VARIANTS[user.status ?? "inactive"]}
          />
        ),
      },
      {
        id: "access",
        header: t("table.access"),
        className: "min-w-[260px]",
        cell: (_value, user) => {
          const membership = user.memberships?.[0];
          if (!membership) return <span className="text-erp-text-muted">-</span>;
          const roles = (membership.roles ?? []).map((role) => role.name).join(", ");
          const pendingCount = membership.pendingRoleRequests?.length ?? 0;
          return (
            <div className="flex flex-col text-xs">
              <span className="text-erp-text-main">{roles || "-"}</span>
              <span className="text-erp-text-muted">
                {membership.branch?.name ?? t("organizationWide")}
                {pendingCount > 0 ? ` · ${t("pendingRequestsCount", { count: pendingCount })}` : ""}
              </span>
            </div>
          );
        },
      },
      {
        id: "actions",
        header: tCommon("actions.manage"),
        isAction: true,
        className: "w-[100px] text-right",
        cell: (_value, user) => (
          <TableActionGroup>
            <TableAction
              icon={<IconEye size={15} />}
              label={tCommon("actions.view")}
              onClick={() => router.push(`/${locale}/settings/users/${user.id}`)}
            />
          </TableActionGroup>
        ),
      },
    ],
    [locale, router, t, tCommon]
  );

  return (
    <div className="space-y-6">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        actions={
          canCreate ? (
            <Button variant="primary" size="md" icon={<IconPlus size={16} />} onClick={() => router.push(`/${locale}/settings/users/create`)}>
              {t("createUser")}
            </Button>
          ) : undefined
        }
      />

      <ListToolbar>
        <div className="flex flex-wrap items-end gap-3">
          <ListSearchInput
            value={listState.draftSearch}
            isDebouncing={listState.isDebouncing}
            placeholder={t("searchPlaceholder")}
            onChange={(value) => listState.actions.setSearch(value)}
            onClear={() => listState.actions.setSearch("", true)}
            onSubmit={(value) => listState.actions.setSearch(value, true)}
          />
          <ListFilterSelect
            id="user-admin-status"
            label={t("table.status")}
            value={filters.status ?? ""}
            onChange={(value) => listState.actions.setFilter("status", value || undefined)}
            options={STATUS_VALUES.map((value) => ({ value, label: t(`status.${value}`) }))}
          />
        </div>
      </ListToolbar>

      {isZeroUsers ? (
        <EmptyState
          icon="empty"
          title={t("emptyTitle")}
          description={t("emptyDescription")}
          actionLabel={canCreate ? t("createUser") : undefined}
          onAction={canCreate ? () => router.push(`/${locale}/settings/users/create`) : undefined}
        />
      ) : (
        <DataTable<AdminUserResponse>
          columns={columns}
          data={users}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
          emptyDescription={t("searchPlaceholder")}
          sorting={{ key: sort ?? null, order }}
          onSort={(key) => listState.actions.setSort(key)}
          pagination={{ page, limit, totalPages, totalItems }}
          onPageChange={(next) => listState.actions.setPage(next)}
          onLimitChange={(next) => listState.actions.setLimit(next as ListPageSize)}
          stickyActionColumn={true}
        />
      )}
    </div>
  );
}
