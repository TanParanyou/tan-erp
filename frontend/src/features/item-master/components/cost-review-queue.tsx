"use client";

import { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { Modal } from "@/components/ui/Modal";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useCostReviewQueue } from "@/features/item-master/api/item-master-queries";
import { useCostReviewMutations } from "@/features/item-master/api/item-master-queries";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { useToast } from "@/hooks/useToast";
import type { CostReviewQueueParams, CostReviewQueueResponse } from "@/lib/api/api-client";
import type { components } from "@/generated/api/tan-erp.v1";

type ReviewRow = NonNullable<CostReviewQueueResponse["items"]>[number];
type ReviewFilters = ListFilterRecord & { status?: string };

function localizedName(name: components["schemas"]["LocalizedTextResponse"] | undefined, locale: string): string {
  return locale === "en" ? name?.english ?? "-" : name?.thai ?? "-";
}

function amountText(amount: number | undefined, currency: string | null | undefined, locale: string): string {
  if (amount === undefined || !currency) return "-";
  return new Intl.NumberFormat(locale === "en" ? "en" : "th", { style: "currency", currency }).format(amount);
}

export function CostReviewQueue() {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership, currentUser } = useSelectedMembership();
  const { toast } = useToast();
  const state = useListState<ReviewFilters>({ schema: { single: ["status"], defaultOrder: "desc" }, debounceMs: 300 });
  const statusValue = state.params.filters.status;
  const status = typeof statusValue === "string" && (statusValue === "submitted" || statusValue === "approved") ? statusValue : undefined;
  const params = useMemo<CostReviewQueueParams>(() => ({
    search: state.params.search || undefined,
    status,
    pageNumber: state.params.page,
    pageSize: state.params.limit,
  }), [state.params.limit, state.params.page, state.params.search, status]);
  const result = useCostReviewQueue(params);
  const mutations = useCostReviewMutations();
  const [returningRow, setReturningRow] = useState<ReviewRow | null>(null);
  const [publishingRow, setPublishingRow] = useState<ReviewRow | null>(null);
  const [returnReason, setReturnReason] = useState("");
  const isApprover = can(selectedMembership, "cost-records.approve");
  const isPublisher = can(selectedMembership, "cost-records.publish");
  const rows = result.data?.items ?? [];
  const totalCount = result.data?.totalCount ?? 0;
  const columns = useMemo<Column<ReviewRow>[]>(() => [
    { id: "item", header: t("name"), cell: (_value, row) => <span><span className="font-mono text-xs">{row.item?.code ?? "-"}</span><br />{localizedName(row.item?.name, locale)}</span>, className: "min-w-52" },
    { id: "source", header: t("costSources"), cell: (_value, row) => row.costSource?.code ?? "-" },
    { id: "reference", header: t("sourceReference"), accessorKey: "sourceReference" },
    { id: "amount", header: t("amount"), cell: (_value, row) => amountText(row.amount, row.currency, locale), className: "text-right tabular-nums" },
    { id: "unit", header: t("unit"), cell: (_value, row) => row.unit?.code ?? "-" },
    { id: "period", header: t("effectiveFrom"), cell: (_value, row) => row.effectiveFromUtc ? new Intl.DateTimeFormat(locale, { dateStyle: "medium", timeZone: "UTC" }).format(new Date(row.effectiveFromUtc)) : "-" },
    { id: "reason", header: t("reason"), accessorKey: "reason", className: "min-w-48" },
    { id: "evidence", header: t("evidence"), cell: (_value, row) => row.evidenceFileId ? t("evidenceAttached") : "-" },
    { id: "maker", header: t("maker"), cell: (_value, row) => row.maker?.displayName ?? "-" },
    { id: "lastEditor", header: t("lastEditor"), cell: (_value, row) => row.lastEditor?.displayName ?? "-" },
    { id: "status", header: t("status"), cell: (_value, row) => row.status === "submitted" ? t("statusSubmitted") : row.status === "approved" ? t("statusApproved") : "-" },
    { id: "actions", header: t("rowActions"), isAction: true, sticky: "right", cell: (_value, row) => (
      <div className="flex justify-end gap-2">
        {row.status === "submitted" && isApprover && <Button type="button" size="sm" className="min-h-[44px]" disabled={mutations.approve.isPending || !row.itemId || !row.id || !row.rowVersion || row.maker?.id === currentUser.user?.id || row.lastEditor?.id === currentUser.user?.id} title={row.maker?.id === currentUser.user?.id || row.lastEditor?.id === currentUser.user?.id ? t("selfApprovalBlocked") : undefined} onClick={() => { if (!row.itemId || !row.id || !row.rowVersion) return; void mutations.approve.mutateAsync({ itemId: row.itemId, costId: row.id, rowVersion: row.rowVersion }).then(() => toast.success(t("costApproved"))).catch(() => toast.error(t("versionConflict"))); }}>{t("approve")}</Button>}
        {row.status === "submitted" && isApprover && <Button type="button" size="sm" variant="outline" className="min-h-[44px]" disabled={mutations.returnForChanges.isPending || !row.itemId || !row.id || !row.rowVersion} onClick={() => { if (!row.itemId || !row.id || !row.rowVersion) return; setReturningRow(row); setReturnReason(""); }}>{t("returnForChanges")}</Button>}
        {row.status === "approved" && isPublisher && <Button type="button" size="sm" className="min-h-[44px]" disabled={mutations.publish.isPending || !row.itemId || !row.id || !row.rowVersion} onClick={() => { if (row.itemId && row.id && row.rowVersion) setPublishingRow(row); }}>{t("publish")}</Button>}
      </div>
    ) },
  ], [currentUser.user?.id, isApprover, isPublisher, locale, mutations.approve.isPending, mutations.publish.isPending, mutations.returnForChanges.isPending, t, toast]);

  return (
    <section className="space-y-5">
      <PageHeader title={t("reviewQueue")} backHref={`/${locale}/item-master`} breadcrumbs={[{ label: t("title"), href: `/${locale}/item-master` }, { label: t("reviewQueue") }]} />
      <ListToolbar>
        <div className="grid w-full grid-cols-1 gap-3 sm:grid-cols-2">
          <Input label={common("actions.search")} value={state.draftSearch} onChange={(event) => state.actions.setSearch(event.target.value)} onKeyDown={(event) => { if (event.key === "Enter") state.actions.setSearch(state.draftSearch, true); }} placeholder={t("search")} />
          <div className="erp-form-group"><label className="erp-label" htmlFor="cost-review-status">{t("reviewStatus")}</label><select id="cost-review-status" className="erp-input" value={status ?? ""} onChange={(event) => state.actions.setFilter("status", event.target.value || undefined)}><option value="">{common("filters.all")}</option><option value="submitted">{t("statusSubmitted")}</option><option value="approved">{t("statusApproved")}</option></select></div>
        </div>
      </ListToolbar>
      <DataTable<ReviewRow>
        columns={columns}
        data={rows}
        isLoading={result.isLoading}
        isError={result.isError}
        error={result.error}
        onRetry={() => { void result.refetch(); }}
        emptyTitle={t("noReviewRows")}
        pagination={{ page: state.params.page, limit: state.params.limit, totalPages: Math.ceil(totalCount / state.params.limit), total: totalCount }}
        onPageChange={state.actions.setPage}
        onLimitChange={(limit) => { if ([10, 25, 50, 100].includes(limit)) state.actions.setLimit(limit as ListPageSize); }}
      />
      <Modal isOpen={Boolean(returningRow)} onClose={() => { if (!mutations.returnForChanges.isPending) setReturningRow(null); }} closeDisabled={mutations.returnForChanges.isPending} title={t("returnForChanges")}>
        <form className="space-y-4" onSubmit={(event) => { event.preventDefault(); if (!returningRow?.itemId || !returningRow.id || !returningRow.rowVersion || !returnReason.trim()) return; void mutations.returnForChanges.mutateAsync({ itemId: returningRow.itemId, costId: returningRow.id, rowVersion: returningRow.rowVersion, reason: returnReason.trim() }).then(() => { setReturningRow(null); toast.success(t("costReturned")); }).catch(() => toast.error(t("versionConflict"))); }}>
          <Input label={t("reason")} required value={returnReason} onChange={(event) => setReturnReason(event.target.value)} maxLength={500} disabled={mutations.returnForChanges.isPending} />
          <div className="flex justify-end gap-2"><Button type="button" variant="outline" disabled={mutations.returnForChanges.isPending} onClick={() => setReturningRow(null)}>{common("actions.cancel")}</Button><Button type="submit" variant="danger" isLoading={mutations.returnForChanges.isPending} disabled={mutations.returnForChanges.isPending || !returnReason.trim()}>{t("returnForChanges")}</Button></div>
        </form>
      </Modal>
      <ConfirmationModal
        isOpen={Boolean(publishingRow)}
        onClose={() => { if (!mutations.publish.isPending) setPublishingRow(null); }}
        onConfirm={async () => {
          if (!publishingRow?.itemId || !publishingRow.id || !publishingRow.rowVersion) return;
          try {
            await mutations.publish.mutateAsync({ itemId: publishingRow.itemId, costId: publishingRow.id, rowVersion: publishingRow.rowVersion, idempotencyKey: crypto.randomUUID() });
            setPublishingRow(null);
            toast.success(t("costPublished"));
          } catch {
            toast.error(t("versionConflict"));
          }
        }}
        title={t("publish")}
        message={t("publishConfirm", { code: publishingRow?.item?.code ?? "-" })}
        confirmText={t("publish")}
        cancelText={common("actions.cancel")}
        variant="info"
        isLoading={mutations.publish.isPending}
      />
    </section>
  );
}
