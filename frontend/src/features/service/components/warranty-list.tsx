"use client";

import { useMemo } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { EmptyState } from "@/components/ui/EmptyState";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatDate } from "@/lib/formatters/formatters";
import type { WarrantyResponse } from "@/lib/api/api-client";
import { useWarrantyList } from "../api/service-queries";

interface Filters extends ListFilterRecord {
  state?: string;
}

const STATES = ["active", "expired"] as const;

function isState(value: string | undefined): value is (typeof STATES)[number] {
  return STATES.some((state) => state === value);
}

export function WarrantyList() {
  const t = useTranslations("service.warranties");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const state = useListState<Filters>({
    schema: { single: ["state"], defaultSort: "endDate", defaultOrder: "desc", allowedSorts: ["endDate"] },
    debounceMs: 300,
  });
  const result = useWarrantyList({
    search: state.params.search || undefined,
    state: state.params.filters.state || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search && !state.params.filters.state;

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const current = state.params.filters.state;
    return isState(current) ? [{ key: "state", value: current, label: `${t("state")}: ${t(`states.${current}`)}` }] : [];
  }, [state.params.filters.state, t]);

  const columns: Column<WarrantyResponse>[] = [
    { id: "number", header: t("number"), className: "min-w-32 font-mono", cell: (_v, row) => <span className="font-mono font-semibold">{row.number ?? "-"}</span> },
    { id: "project", header: t("project"), className: "min-w-48", cell: (_v, row) => row.project?.id ? <Link href={`/${locale}/projects/${row.project.id}`} className="text-erp-navy underline">{row.project.code} · {row.project.name}</Link> : "-" },
    { id: "installation", header: t("installation"), cell: (_v, row) => <Link href={`/${locale}/service/installations/${row.installationJobId}`} className="font-mono text-erp-navy underline">{row.installationNumber ?? "-"}</Link> },
    { id: "term", header: t("term"), cell: (_v, row) => `${row.startDate ? formatDate(row.startDate, locale) : "-"} – ${row.endDate ? formatDate(row.endDate, locale) : "-"} (${row.months ?? 0})` },
    { id: "state", header: t("state"), cell: (_v, row) => <StatusBadge label={t(row.isActive ? "states.active" : "states.expired")} variant={row.isActive ? "success" : "neutral"} /> },
    { id: "requests", header: t("requests"), className: "text-right font-mono", cell: (_v, row) => String(row.serviceRequestCount ?? 0) },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader title={t("title")} subtitle={t("subtitle")} breadcrumbs={[{ label: t("title") }]} />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("state", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="warranty-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          <ListFilterSelect id="warranty-state-filter" label={t("state")} value={state.params.filters.state ?? ""} onChange={(value) => state.actions.setFilter("state", value || undefined)} options={STATES.map((value) => ({ value, label: t(`states.${value}`) }))} />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<WarrantyResponse>
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
