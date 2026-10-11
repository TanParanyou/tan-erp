"use client";

import React, { useMemo } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { Button } from "@/components/ui/Button";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord } from "@/hooks/useListState";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useAdminBranches } from "../api/organization-admin-queries";

interface BranchFilters extends ListFilterRecord {
  status?: string;
}

const STATUS_VALUES = ["active", "inactive"] as const;
type BranchStatus = (typeof STATUS_VALUES)[number];
const isBranchStatus = (value: string | undefined): value is BranchStatus => STATUS_VALUES.some((s) => s === value);

export function BranchAdminList() {
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BRANCHES_MANAGE);

  const listState = useListState<BranchFilters>({
    schema: { defaultSort: "code", defaultOrder: "asc", single: ["status"], allowedSorts: ["code", "name"] },
    debounceMs: 350,
  });
  const status = isBranchStatus(listState.params.filters.status) ? listState.params.filters.status : undefined;
  const { data, isLoading, isError, refetch } = useAdminBranches(status);
  const branches = data ?? [];

  const columns = useMemo<Column<AdminBranchResponse>[]>(
    () => [
      {
        id: "name",
        header: t("branches.table.name"),
        className: "min-w-[240px]",
        cell: (_v, row) => <TableEntityCell title={row.name ?? "-"} code={row.code} href={`/${locale}/settings/branches/${row.id}`} />,
      },
      { id: "taxBranchCode", header: t("branches.table.taxBranchCode"), cell: (_v, row) => row.taxBranchCode ?? "-" },
      {
        id: "status",
        header: t("branches.table.status"),
        cell: (_v, row) => (
          <StatusBadge
            label={row.isActive ? t("branches.status.active") : t("branches.status.inactive")}
            variant={row.isActive ? "success" : "neutral"}
          />
        ),
      },
    ],
    [locale, t]
  );

  const goCreate = () => router.push(`/${locale}/settings/branches/create`);

  return (
    <div className="space-y-6">
      <PageHeader
        title={t("branches.title")}
        subtitle={t("branches.subtitle")}
        actions={
          canManage ? (
            <Button variant="primary" size="md" icon={<IconPlus size={16} />} onClick={goCreate}>
              {t("branches.createBranch")}
            </Button>
          ) : undefined
        }
      />
      <ListToolbar>
        <ListFilterSelect
          id="branch-admin-status"
          label={t("branches.statusFilter")}
          value={status ?? ""}
          onChange={(value) => listState.actions.setFilter("status", value || undefined)}
          options={STATUS_VALUES.map((value) => ({ value, label: t(`branches.statusFilterOptions.${value}`) }))}
        />
      </ListToolbar>
      {!isLoading && !isError && branches.length === 0 ? (
        <EmptyState
          icon="empty"
          title={t("branches.emptyTitle")}
          description={t("branches.emptyDescription")}
          actionLabel={canManage ? t("branches.createBranch") : undefined}
          onAction={canManage ? goCreate : undefined}
        />
      ) : (
        <DataTable<AdminBranchResponse>
          columns={columns}
          data={branches}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
        />
      )}
    </div>
  );
}
