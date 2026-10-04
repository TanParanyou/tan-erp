"use client";

import { useMemo, useRef, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useToast } from "@/hooks/useToast";
import { formatCurrency } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { QuickEstimateListItemResponse } from "@/lib/api/api-client";
import { useQuickEstimateList, useQuickEstimateMutations } from "../api/quick-estimate-queries";
import { QUICK_ESTIMATE_STATUSES, isQuickEstimateStatus, isShareDecision, quickEstimateStatusVariant, shareDecisionVariant } from "../quick-estimate-status";


interface Filters extends ListFilterRecord {
  status?: string;
}

export function QuickEstimateList() {
  const t = useTranslations("quickEstimates.estimates");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.QUICK_ESTIMATES_CREATE);
  const mutations = useQuickEstimateMutations();
  const [createError, setCreateError] = useState(false);
  // One key per click session so a retried create replays instead of opening a second draft.
  const createKeyRef = useRef(crypto.randomUUID());
  const state = useListState<Filters>({
    schema: { single: ["status"], defaultSort: "updatedAt", defaultOrder: "desc", allowedSorts: ["updatedAt"] },
    debounceMs: 300,
  });
  const result = useQuickEstimateList({
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
    return status && isQuickEstimateStatus(status) ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }] : [];
  }, [state.params.filters.status, t]);

  async function create(): Promise<void> {
    setCreateError(false);
    try {
      const created = await mutations.create.mutateAsync(createKeyRef.current);
      createKeyRef.current = crypto.randomUUID();
      toast.success(t("created"));
      router.push(`/${locale}/quick-estimates/${created.id}`);
    } catch {
      setCreateError(true);
    }
  }

  const columns: Column<QuickEstimateListItemResponse>[] = [
    { id: "number", header: t("number"), className: "min-w-32 font-mono", cell: (_v, row) => <Link href={`/${locale}/quick-estimates/${row.id}`} className="font-mono font-semibold text-erp-navy underline">{row.number ?? "-"}</Link> },
    { id: "room", header: t("roomOrArea"), cell: (_v, row) => row.roomOrArea || "-" },
    { id: "template", header: t("template"), className: "font-mono", cell: (_v, row) => row.templateCode ?? "-" },
    { id: "range", header: t("range"), className: "text-right font-mono", cell: (_v, row) => (row.displayedLower == null || row.displayedUpper == null ? "-" : `${formatCurrency(row.displayedLower, "THB", locale)} – ${formatCurrency(row.displayedUpper, "THB", locale)}`) },
    { id: "share", header: t("shareDecision"), cell: (_v, row) => (isShareDecision(row.shareDecision) ? <StatusBadge label={t(`shareDecisions.${row.shareDecision}`)} variant={shareDecisionVariant(row.shareDecision)} /> : "-") },
    { id: "status", header: t("status"), cell: (_v, row) => <StatusBadge label={isQuickEstimateStatus(row.status) ? t(`statuses.${row.status}`) : "-"} variant={quickEstimateStatusVariant(row.status)} /> },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canCreate ? <Button size="md" icon={<IconPlus size={16} />} isLoading={mutations.create.isPending} disabled={mutations.create.isPending} onClick={() => void create()}>{t("create")}</Button> : undefined}
      />
      {createError && <Alert variant="danger" onClose={() => setCreateError(false)}>{t("createFailed")}</Alert>}
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="quick-estimate-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          <ListFilterSelect id="quick-estimate-status-filter" label={t("status")} value={state.params.filters.status ?? ""} onChange={(value) => state.actions.setFilter("status", value || undefined)} options={QUICK_ESTIMATE_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))} />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<QuickEstimateListItemResponse>
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
