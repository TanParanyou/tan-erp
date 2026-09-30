"use client";

import { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { Modal } from "@/components/ui/Modal";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { PageHeader } from "@/components/layout/PageHeader";
import { useListState } from "@/hooks/useListState";
import { useEstimateReviewQueue } from "@/features/estimates/api/estimate-queries";
import type { EstimateReviewQueueParams, EstimateReviewQueueResponse } from "@/lib/api/api-client";

type ReviewRow = NonNullable<EstimateReviewQueueResponse["items"]>[number];

export function EstimateReviewQueue() {
  const t = useTranslations("estimates");
  const common = useTranslations("common");
  const locale = useLocale();
  const state = useListState({ schema: { defaultOrder: "desc" }, debounceMs: 300 });
  const params = useMemo<EstimateReviewQueueParams>(() => ({
    search: state.params.search || undefined,
    pageNumber: state.params.page,
    pageSize: state.params.limit,
  }), [state.params.limit, state.params.page, state.params.search]);
  const result = useEstimateReviewQueue(params);
  const [selectedRow, setSelectedRow] = useState<ReviewRow | null>(null);
  const rows = result.data?.items ?? [];
  const columns = useMemo<Column<ReviewRow>[]>(() => [
    { id: "estimate", header: t("estimateNumber"), cell: (_value, row) => <span className="font-mono">{row.estimateNumber ?? "-"}</span> },
    { id: "opportunity", header: t("reviewOpportunity"), cell: (_value, row) => row.opportunity?.title ?? "-", className: "min-w-48" },
    { id: "customer", header: t("reviewCustomer"), cell: (_value, row) => locale === "en" ? row.customer?.displayNameEn ?? row.customer?.displayNameTh ?? "-" : row.customer?.displayNameTh ?? "-" },
    { id: "branch", header: t("reviewBranch"), cell: (_value, row) => row.branch?.name ?? "-" },
    { id: "revision", header: t("reviewRevision"), cell: (_value, row) => t("revision", { number: row.revisionNo ?? 0 }) },
    { id: "grandTotal", header: t("grandTotal"), cell: (_value, row) => row.currency && row.grandTotal !== undefined ? new Intl.NumberFormat(locale, { style: "currency", currency: row.currency }).format(row.grandTotal) : "-", className: "text-right tabular-nums" },
    { id: "margin", header: t("reviewMargin"), cell: (_value, row) => row.marginRate !== undefined ? new Intl.NumberFormat(locale, { style: "percent", maximumFractionDigits: 2 }).format(row.marginRate) : "-", className: "text-right tabular-nums" },
    { id: "readiness", header: t("reviewReadiness"), cell: (_value, row) => <span className="capitalize">{row.readiness ?? "-"}{(row.provisionalCostCount ?? 0) > 0 ? ` · ${t("provisionalCostCount", { count: row.provisionalCostCount ?? 0 })}` : ""}</span> },
    { id: "requestedAt", header: t("reviewRequestedAt"), cell: (_value, row) => row.requestedAtUtc ? new Intl.DateTimeFormat(locale, { dateStyle: "medium", timeStyle: "short" }).format(new Date(row.requestedAtUtc)) : "-" },
    { id: "actions", header: common("actions.actions"), isAction: true, sticky: "right", cell: (_value, row) => <Button type="button" size="sm" variant="outline" className="min-h-[44px]" onClick={() => setSelectedRow(row)}>{t("reviewDetails")}</Button> },
  ], [common, locale, t]);

  return (
    <section className="space-y-5">
      <PageHeader title={t("reviewQueueTitle")} breadcrumbs={[{ label: t("reviewQueueTitle") }]} />
      <ListToolbar>
        <Input label={common("actions.search")} value={state.draftSearch} onChange={(event) => state.actions.setSearch(event.target.value)} onKeyDown={(event) => { if (event.key === "Enter") state.actions.setSearch(state.draftSearch, true); }} placeholder={t("reviewSearchPlaceholder")} />
      </ListToolbar>
      <DataTable<ReviewRow>
        columns={columns}
        data={rows}
        isLoading={result.isLoading}
        isError={result.isError}
        error={result.error}
        onRetry={() => { void result.refetch(); }}
        emptyTitle={t("reviewQueueEmpty")}
        pagination={{ page: state.params.page, limit: state.params.limit, totalPages: Math.ceil((result.data?.totalCount ?? 0) / state.params.limit), total: result.data?.totalCount ?? 0 }}
        onPageChange={state.actions.setPage}
        onLimitChange={(limit) => { if ([10, 25, 50, 100].includes(limit)) state.actions.setLimit(limit as 10 | 25 | 50 | 100); }}
      />
      <Modal isOpen={Boolean(selectedRow)} onClose={() => setSelectedRow(null)} title={t("reviewDetails")} size="2xl">
        {selectedRow && <div className="space-y-5">
          <div className="grid gap-4 sm:grid-cols-2">
            <Detail label={t("estimateNumber")} value={selectedRow.estimateNumber} />
            <Detail label={t("revision", { number: selectedRow.revisionNo ?? 0 })} value={selectedRow.revisionStatus} />
            <Detail label={t("reviewRequestedBy")} value={selectedRow.requestedBy?.displayName} />
            <Detail label={t("reviewSubmissionNote")} value={selectedRow.submissionNote} />
            <Detail label={t("grandTotal")} value={selectedRow.currency ? new Intl.NumberFormat(locale, { style: "currency", currency: selectedRow.currency }).format(selectedRow.grandTotal ?? 0) : "-"} />
            <Detail label={t("reviewMargin")} value={selectedRow.marginRate !== undefined ? new Intl.NumberFormat(locale, { style: "percent", maximumFractionDigits: 2 }).format(selectedRow.marginRate) : "-"} />
            <Detail label={t("calculationVersion")} value={String(selectedRow.calculationVersion ?? "-")} />
            <Detail label={t("reviewRoutePolicy")} value={`${selectedRow.frozenRoute?.policyCode ?? "-"} v${selectedRow.frozenRoute?.policyVersion ?? "-"}`} />
            <Detail label={t("reviewScope")} value={`${selectedRow.frozenRoute?.scopeType ?? "-"} · ${selectedRow.frozenRoute?.scopeId ?? "-"}`} />
            <Detail label={t("reviewAssignedTo")} value={selectedRow.frozenRoute?.reviewer?.displayName} />
            <Detail label={t("reviewRouteHash")} value={selectedRow.frozenRoute?.routeHash} mono />
            <Detail label={t("reviewApprovalTriggers")} value={selectedRow.frozenRoute?.triggers?.length
              ? selectedRow.frozenRoute.triggers.map((trigger) => `${trigger.code}${trigger.actualValue !== undefined && trigger.actualValue !== null ? ` (${formatTriggerValue(trigger.actualValue, trigger.unit, locale)}; ${t("reviewTriggerThreshold")} ${trigger.thresholdValue !== undefined && trigger.thresholdValue !== null ? formatTriggerValue(trigger.thresholdValue, trigger.unit, locale) : "-"})` : ""}`).join(" · ")
              : t("reviewNoApprovalTriggers")} />
            <Detail label={t("reviewInputHash")} value={selectedRow.calculationInputHash} mono />
            <Detail label={t("reviewSnapshotHash")} value={selectedRow.calculationSnapshotHash} mono />
          </div>
          <div className="space-y-2">
            <h3 className="font-semibold">{t("reviewExceptions")}</h3>
            {(selectedRow.readinessReasons ?? []).length === 0 ? <p className="text-sm text-muted-foreground">{t("reviewNoExceptions")}</p> : <ul className="list-disc space-y-1 pl-5 text-sm">{selectedRow.readinessReasons?.map((reason, index) => <li key={`${reason.code}-${reason.targetId ?? index}`}>{reason.code} · {reason.targetType} · {reason.targetField}</li>)}</ul>}
            {(selectedRow.provisionalCostCount ?? 0) > 0 && <p className="text-sm text-warning">{t("provisionalCostCount", { count: selectedRow.provisionalCostCount ?? 0 })}</p>}
          </div>
          <div className="space-y-2">
            <h3 className="font-semibold">{t("reviewPriceOverrides")}</h3>
            {(selectedRow.priceOverrides ?? []).length === 0
              ? <p className="text-sm text-muted-foreground">{t("reviewNoPriceOverrides")}</p>
              : <ul className="divide-y divide-border border-y border-border">{selectedRow.priceOverrides?.map((priceOverride, index) => <li className="space-y-1 py-3 text-sm" key={`${priceOverride.workItemCode}-${index}`}>
                <div className="flex flex-wrap items-baseline justify-between gap-2"><strong>{priceOverride.workItemCode} · {priceOverride.workItemDescription}</strong><span className="font-mono tabular-nums">{selectedRow.currency ? new Intl.NumberFormat(locale, { style: "currency", currency: selectedRow.currency }).format(priceOverride.fixedPriceUnitAmount ?? 0) : "-"}</span></div>
                <div className="text-warning">{t("sellingRuleReasonCode")}: {priceOverride.reasonCode ?? "-"}</div>
              </li>)}</ul>}
          </div>
          <div className="space-y-2">
            <h3 className="font-semibold">{t("reviewCostEvidence")}</h3>
            {(selectedRow.costEvidence ?? []).length === 0 ? <p className="text-sm text-muted-foreground">{t("reviewNoCostEvidence")}</p> : <ul className="divide-y divide-border border-y border-border">{selectedRow.costEvidence?.map((evidence, index) => <li className="space-y-1 py-3 text-sm" key={`${evidence.workItemCode}-${evidence.description}-${index}`}>
              <div className="flex flex-wrap items-baseline justify-between gap-2"><strong>{evidence.workItemCode} · {evidence.workItemDescription}</strong><span className="font-mono tabular-nums">{evidence.currency ? new Intl.NumberFormat(locale, { style: "currency", currency: evidence.currency }).format(evidence.totalCost ?? 0) : "-"}</span></div>
              <div className="text-muted-foreground">{evidence.type} · {evidence.description} · {evidence.quantity} {evidence.unitCode} × {evidence.unitCost}</div>
              <div className="break-words text-xs text-muted-foreground">{evidence.itemCode ? `${evidence.itemCode} · ` : ""}{evidence.costSourceCode ?? "-"}{evidence.sourceReference ? ` · ${evidence.sourceReference}` : ""}{evidence.costRecordVersion ? ` · v${evidence.costRecordVersion}` : ""}{evidence.evidenceFileId ? ` · ${t("reviewEvidenceFile")}: ${evidence.evidenceFileId}` : ""}{evidence.costRecordReason ? ` · ${evidence.costRecordReason}` : ""}</div>
              {evidence.isProvisional && <div className="text-xs text-warning">{t("reviewProvisionalCost")}: {evidence.provisionalReasonCode ?? "-"}{evidence.provisionalNote ? ` · ${evidence.provisionalNote}` : ""}</div>}
            </li>)}</ul>}
          </div>
          <div className="space-y-2">
            <h3 className="font-semibold">{t("reviewRevisionDiff")}</h3>
            {selectedRow.revisionDiff ? <div className="grid gap-3 border-y border-border py-3 sm:grid-cols-2 lg:grid-cols-3">
              <Detail label={t("reviewPreviousTotal")} value={selectedRow.currency ? new Intl.NumberFormat(locale, { style: "currency", currency: selectedRow.currency }).format(selectedRow.revisionDiff.previousGrandTotal ?? 0) : "-"} />
              <Detail label={t("reviewTotalDelta")} value={selectedRow.currency ? new Intl.NumberFormat(locale, { style: "currency", currency: selectedRow.currency, signDisplay: "always" }).format(selectedRow.revisionDiff.grandTotalDelta ?? 0) : "-"} />
              <Detail label={t("reviewAddedWorkItems")} value={String(selectedRow.revisionDiff.addedWorkItems ?? 0)} />
              <Detail label={t("reviewChangedWorkItems")} value={String(selectedRow.revisionDiff.changedWorkItems ?? 0)} />
              <Detail label={t("reviewRemovedWorkItems")} value={String(selectedRow.revisionDiff.removedWorkItems ?? 0)} />
            </div> : <p className="text-sm text-muted-foreground">{t("reviewNoPreviousRevision")}</p>}
          </div>
          <div className="flex flex-wrap justify-end gap-2 border-t border-border pt-4">
            <Button type="button" variant="outline" onClick={() => setSelectedRow(null)}>{common("actions.close")}</Button>
            {selectedRow.opportunityId && <Button href={`/${locale}/opportunities/${selectedRow.opportunityId}`} className="min-h-[44px]">{t("reviewOpenEstimate")}</Button>}
          </div>
        </div>}
      </Modal>
    </section>
  );
}

function Detail({ label, value, mono = false }: { label: string; value: string | null | undefined; mono?: boolean }) {
  return <div className="min-w-0"><div className="text-xs text-muted-foreground">{label}</div><div className={mono ? "break-all font-mono text-xs" : "break-words text-sm"}>{value || "-"}</div></div>;
}

function formatTriggerValue(value: number, unit: string | null | undefined, locale: string) {
  if (unit === "rate") return new Intl.NumberFormat(locale, { style: "percent", maximumFractionDigits: 2 }).format(value);
  if (unit === "THB") return new Intl.NumberFormat(locale, { style: "currency", currency: "THB" }).format(value);
  return `${new Intl.NumberFormat(locale).format(value)}${unit ? ` ${unit}` : ""}`;
}
