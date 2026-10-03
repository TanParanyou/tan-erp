"use client";

import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { BomListItemResponse } from "@/lib/api/api-client";
import { useBomList } from "../api/production-queries";
import { bomRevisionStatusVariant, isBomRevisionStatus } from "../production-status";

export function BomList() {
  const t = useTranslations("production.boms");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BOMS_MANAGE);
  const state = useListState<ListFilterRecord>({
    schema: { defaultSort: "code", defaultOrder: "asc", allowedSorts: ["code"] },
    debounceMs: 300,
  });
  const result = useBomList({ search: state.params.search || undefined, page: state.params.page, pageSize: state.params.limit });

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search;

  const columns: Column<BomListItemResponse>[] = [
    {
      id: "code",
      header: t("code"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => <Link href={`/${locale}/production/boms/${row.id}`} className="font-mono font-semibold text-erp-navy underline">{row.code ?? "-"}</Link>,
    },
    { id: "item", header: t("item"), className: "min-w-48", cell: (_v, row) => `${row.item?.code ?? "-"} · ${row.item?.nameTh ?? "-"}` },
    { id: "approved", header: t("approvedRevision"), className: "font-mono", cell: (_v, row) => (row.approvedRevisionNo ? `R${row.approvedRevisionNo}` : "-") },
    { id: "latest", header: t("latestRevision"), cell: (_v, row) => (
      <span className="inline-flex items-center gap-2">
        <span className="font-mono">{row.latestRevisionNo ? `R${row.latestRevisionNo}` : "-"}</span>
        <StatusBadge label={isBomRevisionStatus(row.latestStatus) ? t(`statuses.${row.latestStatus}`) : "-"} variant={bomRevisionStatusVariant(row.latestStatus)} />
      </span>
    ) },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canManage ? <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/production/boms/create`}>{t("create")}</Button> : undefined}
      />
      <ListToolbar>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="bom-search"
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
        <DataTable<BomListItemResponse>
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
