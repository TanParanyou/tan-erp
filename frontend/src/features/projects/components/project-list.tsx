"use client";

import { useMemo } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { EmptyState } from "@/components/ui/EmptyState";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import type { ProjectListItemResponse } from "@/lib/api/api-client";
import { useProjectList } from "../api/project-queries";
import { isProjectStatus, PROJECT_STATUSES, projectStatusVariant } from "../project-status";

interface ProjectFilters extends ListFilterRecord {
  status?: string;
}

export function ProjectList() {
  const t = useTranslations("projects");
  const common = useTranslations("common");
  const locale = useLocale();
  const state = useListState<ProjectFilters>({
    schema: { single: ["status"], defaultSort: "createdAt", defaultOrder: "desc", allowedSorts: ["createdAt"] },
    debounceMs: 300,
  });

  const result = useProjectList({
    search: state.params.search || undefined,
    status: state.params.filters.status || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const totalPages = pagination?.totalPages ?? 0;
  const isZeroProjects = !result.isLoading && !result.isError && totalCount === 0
    && !state.params.search && !state.params.filters.status;

  const statusLabel = (status: string | null | undefined): string =>
    isProjectStatus(status) ? t(`statuses.${status}`) : "-";

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = state.params.filters.status;
    return status && isProjectStatus(status)
      ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }]
      : [];
  }, [state.params.filters.status, t]);

  const columns = useMemo<Column<ProjectListItemResponse>[]>(() => [
    {
      id: "code",
      header: t("code"),
      className: "min-w-32 font-mono",
      cell: (_value, row) => (
        <Link href={`/${locale}/projects/${row.id}`} className="font-mono font-semibold text-erp-navy underline">
          {row.code ?? "-"}
        </Link>
      ),
    },
    { id: "name", header: t("name"), accessorKey: "name", className: "min-w-48" },
    {
      id: "customer",
      header: t("customer"),
      cell: (_value, row) => (locale === "en" ? row.customer?.displayNameEn ?? row.customer?.displayNameTh : row.customer?.displayNameTh) ?? "-",
    },
    { id: "owner", header: t("owner"), cell: (_value, row) => row.owner?.displayName ?? "-" },
    {
      id: "status",
      header: t("status"),
      cell: (_value, row) => <StatusBadge label={statusLabel(row.status)} variant={projectStatusVariant(row.status)} />,
    },
    {
      id: "contractAmount",
      header: t("contractAmount"),
      className: "text-right font-mono",
      cell: (_value, row) => formatCurrency(row.contractAmount, "THB", locale),
    },
    {
      id: "plannedStartDate",
      header: t("plannedStartDate"),
      cell: (_value, row) => (row.plannedStartDate ? formatDate(row.plannedStartDate, locale) : "-"),
    },
  // statusLabel only depends on t
  // eslint-disable-next-line react-hooks/exhaustive-deps
  ], [locale, t]);

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader title={t("title")} subtitle={t("subtitle")} breadcrumbs={[{ label: t("title") }]} />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="project-search"
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
            id="project-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={PROJECT_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZeroProjects ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<ProjectListItemResponse>
          columns={columns}
          data={rows}
          isLoading={result.isLoading}
          isError={result.isError}
          error={result.error}
          onRetry={() => { void result.refetch(); }}
          emptyTitle={t("noResults")}
          emptyDescription={t("search")}
          pagination={{ page: state.params.page, limit: state.params.limit, totalPages, total: totalCount }}
          onPageChange={state.actions.setPage}
          onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
        />
      )}
    </section>
  );
}
