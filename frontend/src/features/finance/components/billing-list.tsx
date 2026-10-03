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
import type { BillingListItemResponse } from "@/lib/api/api-client";
import { useBillingList } from "../api/finance-queries";
import { BILLING_STATUSES, billingStatusVariant, isBillingKind, isBillingStatus } from "../finance-status";

interface Filters extends ListFilterRecord {
  status?: string;
}

export function BillingList() {
  const t = useTranslations("finance.billings");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BILLINGS_MANAGE);
  const state = useListState<Filters>({
    schema: { single: ["status"], defaultSort: "issuedAt", defaultOrder: "desc", allowedSorts: ["issuedAt"] },
    debounceMs: 300,
  });
  const result = useBillingList({
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
    return status && isBillingStatus(status) ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }] : [];
  }, [state.params.filters.status, t]);

  const columns: Column<BillingListItemResponse>[] = [
    { id: "number", header: t("number"), className: "min-w-32 font-mono", cell: (_v, row) => <Link href={`/${locale}/finance/billings/${row.id}`} className="font-mono font-semibold text-erp-navy underline">{row.number ?? "-"}</Link> },
    { id: "project", header: t("project"), cell: (_v, row) => row.projectCode ?? "-" },
    { id: "kind", header: t("kind"), cell: (_v, row) => (isBillingKind(row.kind) ? t(`kinds.${row.kind}`) : "-") },
    { id: "status", header: t("status"), cell: (_v, row) => <StatusBadge label={isBillingStatus(row.status) ? t(`statuses.${row.status}`) : "-"} variant={billingStatusVariant(row.status)} /> },
    { id: "amount", header: t("amount"), className: "text-right font-mono", cell: (_v, row) => formatCurrency(row.amount, "THB", locale) },
    { id: "paid", header: t("paid"), className: "text-right font-mono", cell: (_v, row) => formatCurrency(row.paidAmount, "THB", locale) },
    { id: "due", header: t("dueDate"), cell: (_v, row) => (row.dueDate ? formatDate(row.dueDate, locale) : "-") },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canManage ? <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/finance/billings/create`}>{t("create")}</Button> : undefined}
      />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="billing-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          <ListFilterSelect id="billing-status-filter" label={t("status")} value={state.params.filters.status ?? ""} onChange={(value) => state.actions.setFilter("status", value || undefined)} options={BILLING_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))} />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<BillingListItemResponse>
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
