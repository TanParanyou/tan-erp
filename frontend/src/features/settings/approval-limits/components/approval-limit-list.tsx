"use client";

import React, { useMemo } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { IconPlus } from "@/components/common/Icons";
import { LIST_PAGE_SIZES, useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import type { ApprovalLimitResponse } from "@/lib/api/api-client";
import { formatCurrency } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { APPROVAL_DOCUMENT_TYPES, isApprovalDocumentType } from "../approval-limit-document-types";
import { useApprovalLimits } from "../api/approval-limit-queries";

interface LimitFilters extends ListFilterRecord {
  status?: string;
  documentType?: string;
}

const STATUS_VALUES = ["active", "inactive"] as const;

const toListPageSize = (value: number): ListPageSize | undefined => LIST_PAGE_SIZES.find((size) => size === value);

export function ApprovalLimitList() {
  const t = useTranslations("approvalLimitAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.APPROVAL_LIMITS_MANAGE);

  const listState = useListState<LimitFilters>({ schema: { single: ["status", "documentType"] }, debounceMs: 350 });
  const { page, limit, filters } = listState.params;
  const { data, isLoading, isError, refetch } = useApprovalLimits({
    status: filters.status || undefined,
    documentType: filters.documentType || undefined,
    page,
    limit,
  });

  const rows = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 1;
  const isZero = !isLoading && !isError && totalItems === 0 && !filters.status && !filters.documentType;
  const goCreate = () => router.push(`/${locale}/settings/approval-limits/create`);

  const columns = useMemo<Column<ApprovalLimitResponse>[]>(
    () => [
      {
        id: "role",
        header: t("list.table.role"),
        className: "min-w-[220px]",
        cell: (_v, row) => (
          <TableEntityCell
            title={row.role?.name ?? "-"}
            href={row.id ? `/${locale}/settings/approval-limits/${row.id}` : undefined}
          />
        ),
      },
      {
        id: "documentType",
        header: t("list.table.documentType"),
        cell: (_v, row) => (row.documentType && isApprovalDocumentType(row.documentType) ? t(`documentType.${row.documentType}`) : "-"),
      },
      {
        id: "branch",
        header: t("list.table.branch"),
        cell: (_v, row) => (row.branch ? (row.branch.name ?? "-") : t("list.organizationWide")),
      },
      {
        id: "limit",
        header: t("list.table.limit"),
        cell: (_v, row) =>
          row.isUnlimited ? t("list.unlimited") : row.currency ? formatCurrency(row.maxAmount, row.currency, locale) : "-",
      },
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
              {t("list.createLimit")}
            </Button>
          ) : undefined
        }
      />
      <Alert variant="warning">{t("list.failClosedNotice")}</Alert>
      <ListToolbar>
        <div className="flex flex-wrap items-end gap-3">
          <ListFilterSelect
            id="approval-limit-document-type"
            label={t("list.documentTypeFilter")}
            value={filters.documentType ?? ""}
            onChange={(value) => listState.actions.setFilter("documentType", value || undefined)}
            options={APPROVAL_DOCUMENT_TYPES.map((value) => ({ value, label: t(`documentType.${value}`) }))}
          />
          <ListFilterSelect
            id="approval-limit-status"
            label={t("list.statusFilter")}
            value={filters.status ?? ""}
            onChange={(value) => listState.actions.setFilter("status", value || undefined)}
            options={STATUS_VALUES.map((value) => ({ value, label: t(`list.status.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState
          icon="empty"
          title={t("list.emptyTitle")}
          description={t("list.emptyDescription")}
          actionLabel={canManage ? t("list.createLimit") : undefined}
          onAction={canManage ? goCreate : undefined}
        />
      ) : (
        <DataTable<ApprovalLimitResponse>
          columns={columns}
          data={rows}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
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
