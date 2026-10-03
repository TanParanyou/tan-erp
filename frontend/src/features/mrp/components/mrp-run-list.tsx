"use client";

import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { MrpRunListItemResponse } from "@/lib/api/api-client";
import { useMrpRunList } from "../api/mrp-queries";

export function MrpRunList() {
  const t = useTranslations("mrp.runs");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canRun = can(selectedMembership, PERMISSIONS.MRP_RUN);
  const state = useListState<ListFilterRecord>({
    schema: { defaultSort: "createdAt", defaultOrder: "desc", allowedSorts: ["createdAt"] },
    debounceMs: 300,
  });
  const result = useMrpRunList({ search: state.params.search || undefined, page: state.params.page, pageSize: state.params.limit });

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search;

  const columns: Column<MrpRunListItemResponse>[] = [
    {
      id: "number",
      header: t("number"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => <Link href={`/${locale}/production/mrp/${row.id}`} className="font-mono font-semibold text-erp-navy underline">{row.number ?? "-"}</Link>,
    },
    { id: "asOf", header: t("asOfDate"), cell: (_v, row) => (row.asOfDate ? formatDate(row.asOfDate, locale) : "-") },
    { id: "count", header: t("recommendationCount"), className: "text-right font-mono", cell: (_v, row) => String(row.recommendationCount ?? 0) },
    { id: "shortage", header: t("shortageCount"), className: "text-right font-mono", cell: (_v, row) => String(row.shortageCount ?? 0) },
    { id: "open", header: t("openCount"), className: "text-right font-mono", cell: (_v, row) => String(row.openCount ?? 0) },
    { id: "created", header: t("createdAt"), cell: (_v, row) => (row.createdAtUtc ? formatDateTime(row.createdAtUtc, locale) : "-") },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canRun ? <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/production/mrp/create`}>{t("create")}</Button> : undefined}
      />
      <ListToolbar>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="mrp-run-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<MrpRunListItemResponse>
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
