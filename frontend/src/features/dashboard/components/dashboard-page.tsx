"use client";

import React, { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { StatusBadge, type BadgeVariant } from "@/components/ui/StatusBadge";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { PageHeader } from "@/components/layout/PageHeader";
import {
  IconBox3D,
  IconBriefcase,
  IconDownload,
  IconFileText,
  IconGrid,
  IconRefresh,
  IconSearch,
} from "@/components/common/Icons";
import { useCsvExport } from "@/hooks/useCsvExport";
import type { ExportColumn } from "@/lib/export/export-types";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type {
  ArAgingReportRowResponse,
  PipelineStageMetricsResponse,
  ProjectBudgetReportRowResponse,
  QuotationReportRowResponse,
  ServiceSlaReportRowResponse,
  StockValueReportRowResponse,
} from "@/lib/api/api-client";
import {
  useDashboardSummaryQuery,
  usePipelineReportQuery,
  useProjectBudgetReportQuery,
  useQuotationReportQuery,
  useReportingArAgingQuery,
  useReportingBranchesQuery,
  useServiceSlaReportQuery,
  useStockValueReportQuery,
} from "../api/reporting-queries";

export type ReportTabKey =
  | "pipeline"
  | "quotations"
  | "projectsBudget"
  | "stockValue"
  | "arAging"
  | "serviceSla";

export function DashboardPage() {
  const t = useTranslations("reporting");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  // Branch isolation
  const isBranchScoped = Boolean(selectedMembership?.branch);
  const userBranchName = selectedMembership?.branch?.name ?? "-";
  const [selectedBranchId, setSelectedBranchId] = useState<string>("");

  const effectiveBranchId = isBranchScoped ? undefined : selectedBranchId || undefined;

  // As of date (defaults to today's date)
  const todayStr = useMemo(() => new Date().toISOString().split("T")[0], []);
  const [asOfDate, setAsOfDate] = useState<string>(todayStr);

  // Search in table
  const [tableSearch, setTableSearch] = useState<string>("");

  // Permissions per section
  const canPipeline = can(selectedMembership, PERMISSIONS.OPPORTUNITIES_READ);
  const canQuotations = can(selectedMembership, PERMISSIONS.QUOTATIONS_READ);
  const canProjects = can(selectedMembership, PERMISSIONS.PROJECTS_READ);
  const canStock = can(selectedMembership, PERMISSIONS.INVENTORY_READ);
  const canArAging = can(selectedMembership, PERMISSIONS.BILLINGS_READ);
  const canServiceSla = can(selectedMembership, PERMISSIONS.SERVICE_REQUESTS_READ);

  const canAny =
    canPipeline || canQuotations || canProjects || canStock || canArAging || canServiceSla;

  // Permitted tabs
  const permittedTabs = useMemo<ReportTabKey[]>(() => {
    const list: ReportTabKey[] = [];
    if (canPipeline) list.push("pipeline");
    if (canQuotations) list.push("quotations");
    if (canProjects) list.push("projectsBudget");
    if (canStock) list.push("stockValue");
    if (canArAging) list.push("arAging");
    if (canServiceSla) list.push("serviceSla");
    return list;
  }, [canPipeline, canQuotations, canProjects, canStock, canArAging, canServiceSla]);

  const [activeTab, setActiveTab] = useState<ReportTabKey>(() => {
    return permittedTabs[0] ?? "pipeline";
  });

  // Queries
  const branchesQuery = useReportingBranchesQuery(!isBranchScoped);
  const dashboardQuery = useDashboardSummaryQuery(
    { branchId: effectiveBranchId, asOfDate: asOfDate || undefined },
    canAny
  );

  const pipelineQuery = usePipelineReportQuery(
    { branchId: effectiveBranchId },
    canPipeline && activeTab === "pipeline"
  );
  const quotationQuery = useQuotationReportQuery(
    { branchId: effectiveBranchId },
    canQuotations && activeTab === "quotations"
  );
  const projectBudgetQuery = useProjectBudgetReportQuery(
    { branchId: effectiveBranchId },
    canProjects && activeTab === "projectsBudget"
  );
  const stockValueQuery = useStockValueReportQuery(
    { branchId: effectiveBranchId },
    canStock && activeTab === "stockValue"
  );
  const arAgingQuery = useReportingArAgingQuery(
    { branchId: effectiveBranchId, asOfDate: asOfDate || undefined },
    canArAging && activeTab === "arAging"
  );
  const serviceSlaQuery = useServiceSlaReportQuery(
    { branchId: effectiveBranchId },
    canServiceSla && activeTab === "serviceSla"
  );

  const isFetching =
    dashboardQuery.isFetching ||
    pipelineQuery.isFetching ||
    quotationQuery.isFetching ||
    projectBudgetQuery.isFetching ||
    stockValueQuery.isFetching ||
    arAgingQuery.isFetching ||
    serviceSlaQuery.isFetching;

  function refetchAll() {
    void dashboardQuery.refetch();
    if (activeTab === "pipeline") void pipelineQuery.refetch();
    if (activeTab === "quotations") void quotationQuery.refetch();
    if (activeTab === "projectsBudget") void projectBudgetQuery.refetch();
    if (activeTab === "stockValue") void stockValueQuery.refetch();
    if (activeTab === "arAging") void arAgingQuery.refetch();
    if (activeTab === "serviceSla") void serviceSlaQuery.refetch();
  }

  // Branch options
  const branchOptions = useMemo(() => {
    const list = (branchesQuery.data ?? []).map((b) => ({
      value: b.id ?? "",
      label: `${b.name ?? "-"} (${b.code ?? "-"})`,
    }));
    return [{ value: "", label: t("filters.allBranches") }, ...list];
  }, [branchesQuery.data, t]);

  // Summaries from dashboard
  const dashData = dashboardQuery.data;
  const pSummary = dashData?.pipeline?.summary;
  const qSummary = dashData?.quotations?.summary;
  const bSummary = dashData?.projectsBudget?.summary;
  const sSummary = dashData?.stockValue?.summary;
  const aSummary = dashData?.arAging?.summary;
  const sSlaSummary = dashData?.serviceSla?.summary;

  const overdueOver30Total =
    (aSummary?.bucket31To60 ?? 0) + (aSummary?.bucket61To90 ?? 0) + (aSummary?.bucketOver90 ?? 0);

  // If user has no permissions at all
  if (!canAny) {
    return (
      <section className="space-y-6">
        <PageHeader title={t("title")} subtitle={t("subtitle")} breadcrumbs={[{ label: t("title") }]} />
        <div className="erp-card p-12 text-center space-y-3">
          <p className="text-base font-semibold text-erp-navy">{t("noPermittedReports")}</p>
          <p className="text-sm text-erp-text-muted">{t("subtitle")}</p>
        </div>
      </section>
    );
  }

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={
          <div className="flex flex-wrap items-center gap-3">
            <Button
              type="button"
              variant="outline"
              size="md"
              className="h-11 min-h-[44px]"
              icon={<IconRefresh size={18} className={isFetching ? "animate-spin" : ""} />}
              disabled={isFetching}
              onClick={refetchAll}
            >
              {t("refresh")}
            </Button>
          </div>
        }
      />

      {/* Filter toolbar */}
      <div className="erp-card p-4 flex flex-wrap items-end justify-between gap-4 border border-erp-border bg-erp-surface-subtle">
        <div className="flex flex-wrap items-end gap-3">
          {isBranchScoped ? (
            <div className="flex flex-col">
              <span className="text-xs text-erp-text-muted font-medium min-h-[20px] mb-1.5 flex items-center">
                {t("filters.branchFilterLabel")}
              </span>
              <div className="h-11 min-h-[44px] flex items-center">
                <StatusBadge
                  label={userBranchName}
                  variant="neutral"
                />
              </div>
            </div>
          ) : (
            <div className="w-64">
              <Select
                id="reporting-filter-branch"
                label={t("filters.branchFilterLabel")}
                placeholder={t("filters.allBranches")}
                value={selectedBranchId}
                options={branchOptions}
                wrapperClassName="!mb-0"
                className="h-11 min-h-[44px]"
                onChange={(e) => setSelectedBranchId(e.target.value)}
              />
            </div>
          )}

          <div className="w-48">
            <Input
              id="reporting-filter-as-of-date"
              type="date"
              label={t("filters.asOfDateLabel")}
              placeholder={t("filters.asOfDatePlaceholder")}
              value={asOfDate}
              wrapperClassName="!mb-0"
              className="h-11 min-h-[44px]"
              onChange={(e) => setAsOfDate(e.target.value)}
            />
          </div>
        </div>

        {dashData?.generatedAtUtc && (
          <div className="text-xs text-erp-text-muted">
            {t("generatedAt", { date: formatDate(dashData.generatedAtUtc, locale) })}
          </div>
        )}
      </div>

      {/* 6 Executive KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
        {/* Card 1: Pipeline */}
        {canPipeline && (
          <button
            type="button"
            onClick={() => setActiveTab("pipeline")}
            className={`erp-card p-5 text-left transition-all cursor-pointer border ${
              activeTab === "pipeline"
                ? "border-erp-navy ring-2 ring-erp-navy/20 bg-erp-surface-subtle"
                : "border-erp-border hover:border-erp-navy/50"
            }`}
          >
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs font-semibold text-erp-navy flex items-center gap-1.5">
                <IconBriefcase size={16} />
                {t("kpi.pipelineTitle")}
              </span>
              <StatusBadge
                label={`${(pSummary?.winRatePercent ?? 0).toFixed(1)}%`}
                variant="success"
              />
            </div>
            <div className="mt-3">
              <div className="text-2xl font-bold font-mono text-erp-navy">
                {formatCurrency(pSummary?.totalExpectedBudget ?? 0, "THB", locale)}
              </div>
              <div className="text-xs text-erp-text-muted mt-1 flex flex-wrap gap-x-3 gap-y-0.5">
                <span>
                  {t("kpi.openPipeline")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {formatCurrency(pSummary?.openExpectedBudget ?? 0, "THB", locale)}
                  </strong>
                </span>
                <span>
                  {t("stages.won")}:{" "}
                  <strong className="font-mono text-erp-text-main">{pSummary?.wonCount ?? 0}</strong>
                </span>
              </div>
            </div>
          </button>
        )}

        {/* Card 2: Quotations */}
        {canQuotations && (
          <button
            type="button"
            onClick={() => setActiveTab("quotations")}
            className={`erp-card p-5 text-left transition-all cursor-pointer border ${
              activeTab === "quotations"
                ? "border-erp-navy ring-2 ring-erp-navy/20 bg-erp-surface-subtle"
                : "border-erp-border hover:border-erp-navy/50"
            }`}
          >
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs font-semibold text-erp-navy flex items-center gap-1.5">
                <IconFileText size={16} />
                {t("kpi.quotationsTitle")}
              </span>
              <StatusBadge
                label={`${(qSummary?.acceptanceRatePercent ?? 0).toFixed(1)}%`}
                variant="info"
              />
            </div>
            <div className="mt-3">
              <div className="text-2xl font-bold font-mono text-erp-navy">
                {formatCurrency(qSummary?.acceptedAmount ?? 0, "THB", locale)}
              </div>
              <div className="text-xs text-erp-text-muted mt-1 flex flex-wrap gap-x-3 gap-y-0.5">
                <span>
                  {t("quotationStatuses.issued")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {formatCurrency(qSummary?.issuedAmount ?? 0, "THB", locale)}
                  </strong>
                </span>
                <span>
                  {t("quotationStatuses.accepted")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {qSummary?.acceptedCount ?? 0}
                  </strong>
                </span>
              </div>
            </div>
          </button>
        )}

        {/* Card 3: Project Budget */}
        {canProjects && (
          <button
            type="button"
            onClick={() => setActiveTab("projectsBudget")}
            className={`erp-card p-5 text-left transition-all cursor-pointer border ${
              activeTab === "projectsBudget"
                ? "border-erp-navy ring-2 ring-erp-navy/20 bg-erp-surface-subtle"
                : "border-erp-border hover:border-erp-navy/50"
            }`}
          >
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs font-semibold text-erp-navy flex items-center gap-1.5">
                <IconGrid size={16} />
                {t("kpi.projectsBudgetTitle")}
              </span>
              {(bSummary?.overBudgetCount ?? 0) > 0 ? (
                <StatusBadge
                  label={`${bSummary?.overBudgetCount ?? 0} ${t("budgetStatus.overBudget")}`}
                  variant="danger"
                />
              ) : (
                <StatusBadge label={t("budgetStatus.withinBudget")} variant="success" />
              )}
            </div>
            <div className="mt-3">
              <div className="text-2xl font-bold font-mono text-erp-navy">
                {formatCurrency(bSummary?.totalCurrentBudget ?? 0, "THB", locale)}
              </div>
              <div className="text-xs text-erp-text-muted mt-1 flex flex-wrap gap-x-3 gap-y-0.5">
                <span>
                  {t("columns.actualCost")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {formatCurrency(bSummary?.totalActualAmount ?? 0, "THB", locale)}
                  </strong>
                </span>
                <span>
                  {t("columns.variance")}:{" "}
                  <strong
                    className={`font-mono ${
                      (bSummary?.totalVarianceAmount ?? 0) < 0
                        ? "text-erp-danger"
                        : "text-erp-success"
                    }`}
                  >
                    {formatCurrency(bSummary?.totalVarianceAmount ?? 0, "THB", locale)}
                  </strong>
                </span>
              </div>
            </div>
          </button>
        )}

        {/* Card 4: Stock Value */}
        {canStock && (
          <button
            type="button"
            onClick={() => setActiveTab("stockValue")}
            className={`erp-card p-5 text-left transition-all cursor-pointer border ${
              activeTab === "stockValue"
                ? "border-erp-navy ring-2 ring-erp-navy/20 bg-erp-surface-subtle"
                : "border-erp-border hover:border-erp-navy/50"
            }`}
          >
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs font-semibold text-erp-navy flex items-center gap-1.5">
                <IconBox3D size={16} />
                {t("kpi.stockValueTitle")}
              </span>
              <StatusBadge
                label={`${sSummary?.warehouseCount ?? 0} ${t("columns.warehouseName")}`}
                variant="neutral"
              />
            </div>
            <div className="mt-3">
              <div className="text-2xl font-bold font-mono text-erp-navy">
                {formatCurrency(sSummary?.totalValue ?? 0, "THB", locale)}
              </div>
              <div className="text-xs text-erp-text-muted mt-1 flex flex-wrap gap-x-3 gap-y-0.5">
                <span>
                  SKUs:{" "}
                  <strong className="font-mono text-erp-text-main">{sSummary?.skuCount ?? 0}</strong>
                </span>
                <span>
                  {t("columns.onHandQty")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {(sSummary?.totalOnHand ?? 0).toLocaleString()}
                  </strong>
                </span>
              </div>
            </div>
          </button>
        )}

        {/* Card 5: AR Aging */}
        {canArAging && (
          <button
            type="button"
            onClick={() => setActiveTab("arAging")}
            className={`erp-card p-5 text-left transition-all cursor-pointer border ${
              activeTab === "arAging"
                ? "border-erp-navy ring-2 ring-erp-navy/20 bg-erp-surface-subtle"
                : "border-erp-border hover:border-erp-navy/50"
            }`}
          >
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs font-semibold text-erp-navy flex items-center gap-1.5">
                <IconFileText size={16} />
                {t("kpi.arAgingTitle")}
              </span>
              {overdueOver30Total > 0 ? (
                <StatusBadge label={t("kpi.overdueOver30")} variant="danger" />
              ) : (
                <StatusBadge label={t("agingBuckets.current")} variant="success" />
              )}
            </div>
            <div className="mt-3">
              <div className="text-2xl font-bold font-mono text-erp-navy">
                {formatCurrency(aSummary?.totalOutstanding ?? 0, "THB", locale)}
              </div>
              <div className="text-xs text-erp-text-muted mt-1 flex flex-wrap gap-x-3 gap-y-0.5">
                <span>
                  &gt; 30d:{" "}
                  <strong className="font-mono text-erp-danger">
                    {formatCurrency(overdueOver30Total, "THB", locale)}
                  </strong>
                </span>
                <span>
                  {t("columns.billingNumber")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {aSummary?.invoiceCount ?? 0}
                  </strong>
                </span>
              </div>
            </div>
          </button>
        )}

        {/* Card 6: Service SLA */}
        {canServiceSla && (
          <button
            type="button"
            onClick={() => setActiveTab("serviceSla")}
            className={`erp-card p-5 text-left transition-all cursor-pointer border ${
              activeTab === "serviceSla"
                ? "border-erp-navy ring-2 ring-erp-navy/20 bg-erp-surface-subtle"
                : "border-erp-border hover:border-erp-navy/50"
            }`}
          >
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs font-semibold text-erp-navy flex items-center gap-1.5">
                <IconGrid size={16} />
                {t("kpi.serviceSlaTitle")}
              </span>
              {(sSlaSummary?.slaBreachedCount ?? 0) > 0 ? (
                <StatusBadge
                  label={`${sSlaSummary?.slaBreachedCount ?? 0} ${t("slaStatuses.breached")}`}
                  variant="danger"
                />
              ) : (
                <StatusBadge label={t("slaStatuses.on_track")} variant="success" />
              )}
            </div>
            <div className="mt-3">
              <div className="text-2xl font-bold font-mono text-erp-navy">
                {(sSlaSummary?.openBacklogCount ?? 0).toLocaleString()}
              </div>
              <div className="text-xs text-erp-text-muted mt-1 flex flex-wrap gap-x-3 gap-y-0.5">
                <span>
                  {t("kpi.waitingCustomer")}:{" "}
                  <strong className="font-mono text-erp-text-main">
                    {sSlaSummary?.waitingCustomerCount ?? 0}
                  </strong>
                </span>
                <span>
                  {t("slaStatuses.warning")}:{" "}
                  <strong className="font-mono text-erp-warning">
                    {sSlaSummary?.slaWarningCount ?? 0}
                  </strong>
                </span>
              </div>
            </div>
          </button>
        )}
      </div>

      {/* Tabs bar */}
      <div className="border-b border-erp-border flex flex-wrap gap-2">
        {permittedTabs.map((tab) => (
          <button
            key={tab}
            type="button"
            onClick={() => {
              setActiveTab(tab);
              setTableSearch("");
            }}
            className={`px-4 py-2.5 text-xs font-semibold border-b-2 transition-colors ${
              activeTab === tab
                ? "border-erp-navy text-erp-navy bg-erp-surface-subtle"
                : "border-transparent text-erp-text-muted hover:text-erp-navy"
            }`}
          >
            {t(`tabs.${tab}`)}
          </button>
        ))}
      </div>

      {/* Detailed Report Content */}
      <div className="space-y-4">
        {activeTab === "pipeline" && canPipeline && (
          <PipelineReportView
            data={pipelineQuery.data}
            isLoading={pipelineQuery.isLoading}
            searchQuery={tableSearch}
            onSearchChange={setTableSearch}
          />
        )}
        {activeTab === "quotations" && canQuotations && (
          <QuotationReportView
            data={quotationQuery.data}
            isLoading={quotationQuery.isLoading}
            searchQuery={tableSearch}
            onSearchChange={setTableSearch}
          />
        )}
        {activeTab === "projectsBudget" && canProjects && (
          <ProjectBudgetReportView
            data={projectBudgetQuery.data}
            isLoading={projectBudgetQuery.isLoading}
            searchQuery={tableSearch}
            onSearchChange={setTableSearch}
          />
        )}
        {activeTab === "stockValue" && canStock && (
          <StockValueReportView
            data={stockValueQuery.data}
            isLoading={stockValueQuery.isLoading}
            searchQuery={tableSearch}
            onSearchChange={setTableSearch}
          />
        )}
        {activeTab === "arAging" && canArAging && (
          <ArAgingReportView
            data={arAgingQuery.data}
            isLoading={arAgingQuery.isLoading}
            searchQuery={tableSearch}
            onSearchChange={setTableSearch}
          />
        )}
        {activeTab === "serviceSla" && canServiceSla && (
          <ServiceSlaReportView
            data={serviceSlaQuery.data}
            isLoading={serviceSlaQuery.isLoading}
            searchQuery={tableSearch}
            onSearchChange={setTableSearch}
          />
        )}
      </div>
    </section>
  );
}

// -------------------------------------------------------------
// View 1: Pipeline Report
// -------------------------------------------------------------
function PipelineReportView({
  data,
  isLoading,
  searchQuery,
  onSearchChange,
}: {
  data: { stages?: PipelineStageMetricsResponse[] | null } | undefined;
  isLoading: boolean;
  searchQuery: string;
  onSearchChange: (val: string) => void;
}) {
  const t = useTranslations("reporting");
  const locale = useLocale();

  const rows = useMemo(() => {
    const list = data?.stages ?? [];
    if (!searchQuery.trim()) return list;
    const q = searchQuery.toLowerCase().trim();
    return list.filter((r) => {
      const stageName = r.stage ? t(`stages.${r.stage}`) : "";
      return stageName.toLowerCase().includes(q) || (r.stage ?? "").toLowerCase().includes(q);
    });
  }, [data?.stages, searchQuery, t]);

  const csvColumns = useMemo<ExportColumn<PipelineStageMetricsResponse>[]>(
    () => [
      {
        header: t("columns.stage"),
        accessor: (r) => (r.stage ? t(`stages.${r.stage}`) : "-"),
      },
      {
        header: t("columns.opportunityNumber"),
        accessor: (r) => r.opportunityCount ?? 0,
      },
      {
        header: t("columns.estimatedValue"),
        accessor: (r) => r.expectedBudgetTotal ?? 0,
      },
    ],
    [t]
  );

  const csv = useCsvExport<PipelineStageMetricsResponse>({
    columns: csvColumns,
    filename: "pipeline-report",
    data: rows,
  });

  if (isLoading) {
    return (
      <div className="py-12 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  return (
    <div className="erp-card p-5 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="w-72">
          <Input
            id="pipeline-search"
            placeholder={t("filters.searchPlaceholder")}
            value={searchQuery}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            leftIcon={<IconSearch size={16} />}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        <Button
          type="button"
          variant="outline"
          size="md"
          className="h-11 min-h-[44px]"
          icon={<IconDownload size={16} />}
          disabled={csv.isExporting || rows.length === 0}
          onClick={() => void csv.exportAll()}
        >
          {csv.isExporting ? t("exportingCsv") : t("exportCsv")}
        </Button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs">
          <thead>
            <tr className="border-b border-erp-border">
              <th className="py-2.5 pr-3">{t("columns.stage")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.opportunityNumber")}</th>
              <th className="py-2.5 text-right">{t("columns.estimatedValue")}</th>
            </tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr>
                <td colSpan={3} className="py-8 text-center text-erp-text-muted">
                  {t("emptyRows")}
                </td>
              </tr>
            ) : (
              rows.map((row, idx) => (
                <tr key={`${row.stage}-${idx}`} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2.5 pr-3 font-medium">
                    <StatusBadge
                      label={row.stage ? t(`stages.${row.stage}`) : "-"}
                      variant={getStageVariant(row.stage)}
                    />
                  </td>
                  <td className="py-2.5 pr-3 text-right font-mono font-semibold">
                    {(row.opportunityCount ?? 0).toLocaleString()}
                  </td>
                  <td className="py-2.5 text-right font-mono font-bold text-erp-navy">
                    {formatCurrency(row.expectedBudgetTotal ?? 0, "THB", locale)}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// -------------------------------------------------------------
// View 2: Quotation Report
// -------------------------------------------------------------
function QuotationReportView({
  data,
  isLoading,
  searchQuery,
  onSearchChange,
}: {
  data: { rows?: QuotationReportRowResponse[] | null } | undefined;
  isLoading: boolean;
  searchQuery: string;
  onSearchChange: (val: string) => void;
}) {
  const t = useTranslations("reporting");
  const locale = useLocale();

  const rows = useMemo(() => {
    const list = data?.rows ?? [];
    if (!searchQuery.trim()) return list;
    const q = searchQuery.toLowerCase().trim();
    return list.filter(
      (r) =>
        (r.number ?? "").toLowerCase().includes(q) ||
        (r.customerName ?? "").toLowerCase().includes(q) ||
        (r.customerCode ?? "").toLowerCase().includes(q) ||
        (r.branchCode ?? "").toLowerCase().includes(q)
    );
  }, [data?.rows, searchQuery]);

  const csvColumns = useMemo<ExportColumn<QuotationReportRowResponse>[]>(
    () => [
      {
        header: t("columns.quotationNumber"),
        accessor: (r) => r.number ?? "-",
      },
      {
        header: t("columns.customer"),
        accessor: (r) => `${r.customerCode ?? "-"} ${r.customerName ?? "-"}`,
      },
      {
        header: t("columns.branch"),
        accessor: (r) => r.branchCode ?? "-",
      },
      {
        header: t("columns.status"),
        accessor: (r) => (r.status ? t(`quotationStatuses.${r.status}`) : "-"),
      },
      {
        header: t("columns.totalAmount"),
        accessor: (r) => r.totalAmount ?? 0,
      },
      {
        header: t("columns.issuedAt"),
        accessor: (r) => (r.issuedAtUtc ? formatDate(r.issuedAtUtc, locale) : "-"),
      },
    ],
    [t, locale]
  );

  const csv = useCsvExport<QuotationReportRowResponse>({
    columns: csvColumns,
    filename: "quotation-report",
    data: rows,
  });

  if (isLoading) {
    return (
      <div className="py-12 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  return (
    <div className="erp-card p-5 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="w-72">
          <Input
            id="quotation-search"
            placeholder={t("filters.searchPlaceholder")}
            value={searchQuery}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            leftIcon={<IconSearch size={16} />}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        <Button
          type="button"
          variant="outline"
          size="md"
          className="h-11 min-h-[44px]"
          icon={<IconDownload size={16} />}
          disabled={csv.isExporting || rows.length === 0}
          onClick={() => void csv.exportAll()}
        >
          {csv.isExporting ? t("exportingCsv") : t("exportCsv")}
        </Button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs">
          <thead>
            <tr className="border-b border-erp-border">
              <th className="py-2.5 pr-3">{t("columns.quotationNumber")}</th>
              <th className="py-2.5 pr-3">{t("columns.customer")}</th>
              <th className="py-2.5 pr-3">{t("columns.branch")}</th>
              <th className="py-2.5 pr-3">{t("columns.status")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.totalAmount")}</th>
              <th className="py-2.5 text-right">{t("columns.issuedAt")}</th>
            </tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr>
                <td colSpan={6} className="py-8 text-center text-erp-text-muted">
                  {t("emptyRows")}
                </td>
              </tr>
            ) : (
              rows.map((row) => (
                <tr key={row.quotationId ?? row.number} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2.5 pr-3 font-mono font-semibold text-erp-navy">
                    {row.number ?? "-"}
                  </td>
                  <td className="py-2.5 pr-3">
                    <span className="font-mono text-[11px] text-erp-text-muted">{row.customerCode}</span>{" "}
                    <span className="font-medium">{row.customerName ?? "-"}</span>
                  </td>
                  <td className="py-2.5 pr-3 font-mono">{row.branchCode ?? "-"}</td>
                  <td className="py-2.5 pr-3">
                    <StatusBadge
                      label={row.status ? t(`quotationStatuses.${row.status}`) : "-"}
                      variant={getQuotationStatusVariant(row.status)}
                    />
                  </td>
                  <td className="py-2.5 pr-3 text-right font-mono font-bold text-erp-navy">
                    {formatCurrency(row.totalAmount ?? 0, "THB", locale)}
                  </td>
                  <td className="py-2.5 text-right font-mono text-erp-text-muted">
                    {row.issuedAtUtc ? formatDate(row.issuedAtUtc, locale) : "-"}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// -------------------------------------------------------------
// View 3: Project Budget Report
// -------------------------------------------------------------
function ProjectBudgetReportView({
  data,
  isLoading,
  searchQuery,
  onSearchChange,
}: {
  data: { rows?: ProjectBudgetReportRowResponse[] | null } | undefined;
  isLoading: boolean;
  searchQuery: string;
  onSearchChange: (val: string) => void;
}) {
  const t = useTranslations("reporting");
  const locale = useLocale();

  const rows = useMemo(() => {
    const list = data?.rows ?? [];
    if (!searchQuery.trim()) return list;
    const q = searchQuery.toLowerCase().trim();
    return list.filter(
      (r) =>
        (r.projectCode ?? "").toLowerCase().includes(q) ||
        (r.projectName ?? "").toLowerCase().includes(q) ||
        (r.branchCode ?? "").toLowerCase().includes(q)
    );
  }, [data?.rows, searchQuery]);

  const csvColumns = useMemo<ExportColumn<ProjectBudgetReportRowResponse>[]>(
    () => [
      {
        header: t("columns.projectCode"),
        accessor: (r) => r.projectCode ?? "-",
      },
      {
        header: t("columns.projectName"),
        accessor: (r) => r.projectName ?? "-",
      },
      {
        header: t("columns.branch"),
        accessor: (r) => r.branchCode ?? "-",
      },
      {
        header: t("columns.currentBudget"),
        accessor: (r) => r.currentBudget ?? 0,
      },
      {
        header: t("columns.actualCost"),
        accessor: (r) => r.actualAmount ?? 0,
      },
      {
        header: t("columns.variance"),
        accessor: (r) => r.varianceAmount ?? 0,
      },
      {
        header: t("columns.budgetStatus"),
        accessor: (r) =>
          r.isOverBudget ? t("budgetStatus.overBudget") : t("budgetStatus.withinBudget"),
      },
    ],
    [t]
  );

  const csv = useCsvExport<ProjectBudgetReportRowResponse>({
    columns: csvColumns,
    filename: "project-budget-report",
    data: rows,
  });

  if (isLoading) {
    return (
      <div className="py-12 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  return (
    <div className="erp-card p-5 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="w-72">
          <Input
            id="budget-search"
            placeholder={t("filters.searchPlaceholder")}
            value={searchQuery}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            leftIcon={<IconSearch size={16} />}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        <Button
          type="button"
          variant="outline"
          size="md"
          className="h-11 min-h-[44px]"
          icon={<IconDownload size={16} />}
          disabled={csv.isExporting || rows.length === 0}
          onClick={() => void csv.exportAll()}
        >
          {csv.isExporting ? t("exportingCsv") : t("exportCsv")}
        </Button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs">
          <thead>
            <tr className="border-b border-erp-border">
              <th className="py-2.5 pr-3">{t("columns.projectCode")}</th>
              <th className="py-2.5 pr-3">{t("columns.projectName")}</th>
              <th className="py-2.5 pr-3">{t("columns.branch")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.currentBudget")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.actualCost")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.variance")}</th>
              <th className="py-2.5 text-center">{t("columns.budgetStatus")}</th>
            </tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr>
                <td colSpan={7} className="py-8 text-center text-erp-text-muted">
                  {t("emptyRows")}
                </td>
              </tr>
            ) : (
              rows.map((row) => (
                <tr key={row.projectId ?? row.projectCode} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2.5 pr-3 font-mono font-semibold text-erp-navy">
                    {row.projectCode ?? "-"}
                  </td>
                  <td className="py-2.5 pr-3 font-medium">{row.projectName ?? "-"}</td>
                  <td className="py-2.5 pr-3 font-mono">{row.branchCode ?? "-"}</td>
                  <td className="py-2.5 pr-3 text-right font-mono">
                    {formatCurrency(row.currentBudget ?? 0, "THB", locale)}
                  </td>
                  <td className="py-2.5 pr-3 text-right font-mono">
                    {formatCurrency(row.actualAmount ?? 0, "THB", locale)}
                  </td>
                  <td
                    className={`py-2.5 pr-3 text-right font-mono font-semibold ${
                      (row.varianceAmount ?? 0) < 0 ? "text-erp-danger" : "text-erp-success"
                    }`}
                  >
                    {formatCurrency(row.varianceAmount ?? 0, "THB", locale)}
                  </td>
                  <td className="py-2.5 text-center">
                    <StatusBadge
                      label={
                        row.isOverBudget
                          ? t("budgetStatus.overBudget")
                          : t("budgetStatus.withinBudget")
                      }
                      variant={row.isOverBudget ? "danger" : "success"}
                    />
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// -------------------------------------------------------------
// View 4: Stock Value Report
// -------------------------------------------------------------
function StockValueReportView({
  data,
  isLoading,
  searchQuery,
  onSearchChange,
}: {
  data: { rows?: StockValueReportRowResponse[] | null } | undefined;
  isLoading: boolean;
  searchQuery: string;
  onSearchChange: (val: string) => void;
}) {
  const t = useTranslations("reporting");
  const locale = useLocale();

  const rows = useMemo(() => {
    const list = data?.rows ?? [];
    if (!searchQuery.trim()) return list;
    const q = searchQuery.toLowerCase().trim();
    return list.filter(
      (r) =>
        (r.warehouseCode ?? "").toLowerCase().includes(q) ||
        (r.warehouseName ?? "").toLowerCase().includes(q) ||
        (r.itemCode ?? "").toLowerCase().includes(q) ||
        (r.itemNameTh ?? "").toLowerCase().includes(q) ||
        (r.itemNameEn ?? "").toLowerCase().includes(q)
    );
  }, [data?.rows, searchQuery]);

  const csvColumns = useMemo<ExportColumn<StockValueReportRowResponse>[]>(
    () => [
      {
        header: t("columns.warehouseName"),
        accessor: (r) => `${r.warehouseCode ?? "-"} ${r.warehouseName ?? "-"}`,
      },
      {
        header: t("columns.itemCode"),
        accessor: (r) => r.itemCode ?? "-",
      },
      {
        header: t("columns.itemName"),
        accessor: (r) =>
          locale === "en" && r.itemNameEn ? r.itemNameEn : r.itemNameTh ?? "-",
      },
      {
        header: t("columns.branch"),
        accessor: (r) => r.branchCode ?? "-",
      },
      {
        header: t("columns.onHandQty"),
        accessor: (r) => r.onHand ?? 0,
      },
      {
        header: t("columns.totalValue"),
        accessor: (r) => r.totalValue ?? 0,
      },
    ],
    [t, locale]
  );

  const csv = useCsvExport<StockValueReportRowResponse>({
    columns: csvColumns,
    filename: "stock-value-report",
    data: rows,
  });

  if (isLoading) {
    return (
      <div className="py-12 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  return (
    <div className="erp-card p-5 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="w-72">
          <Input
            id="stock-search"
            placeholder={t("filters.searchPlaceholder")}
            value={searchQuery}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            leftIcon={<IconSearch size={16} />}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        <Button
          type="button"
          variant="outline"
          size="md"
          className="h-11 min-h-[44px]"
          icon={<IconDownload size={16} />}
          disabled={csv.isExporting || rows.length === 0}
          onClick={() => void csv.exportAll()}
        >
          {csv.isExporting ? t("exportingCsv") : t("exportCsv")}
        </Button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs">
          <thead>
            <tr className="border-b border-erp-border">
              <th className="py-2.5 pr-3">{t("columns.warehouseName")}</th>
              <th className="py-2.5 pr-3">{t("columns.itemCode")}</th>
              <th className="py-2.5 pr-3">{t("columns.itemName")}</th>
              <th className="py-2.5 pr-3">{t("columns.branch")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.onHandQty")}</th>
              <th className="py-2.5 text-right">{t("columns.totalValue")}</th>
            </tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr>
                <td colSpan={6} className="py-8 text-center text-erp-text-muted">
                  {t("emptyRows")}
                </td>
              </tr>
            ) : (
              rows.map((row, idx) => (
                <tr key={`${row.warehouseId}-${row.itemId}-${idx}`} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2.5 pr-3 font-medium">
                    <span className="font-mono text-erp-navy font-semibold">{row.warehouseCode}</span>{" "}
                    {row.warehouseName}
                  </td>
                  <td className="py-2.5 pr-3 font-mono font-semibold">{row.itemCode ?? "-"}</td>
                  <td className="py-2.5 pr-3">
                    {locale === "en" && row.itemNameEn ? row.itemNameEn : row.itemNameTh ?? "-"}
                  </td>
                  <td className="py-2.5 pr-3 font-mono">{row.branchCode ?? "-"}</td>
                  <td className="py-2.5 pr-3 text-right font-mono">
                    {(row.onHand ?? 0).toLocaleString()}
                  </td>
                  <td className="py-2.5 text-right font-mono font-bold text-erp-navy">
                    {formatCurrency(row.totalValue ?? 0, "THB", locale)}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// -------------------------------------------------------------
// View 5: AR Aging Report
// -------------------------------------------------------------
function ArAgingReportView({
  data,
  isLoading,
  searchQuery,
  onSearchChange,
}: {
  data: { rows?: ArAgingReportRowResponse[] | null } | undefined;
  isLoading: boolean;
  searchQuery: string;
  onSearchChange: (val: string) => void;
}) {
  const t = useTranslations("reporting");
  const locale = useLocale();

  const rows = useMemo(() => {
    const list = data?.rows ?? [];
    if (!searchQuery.trim()) return list;
    const q = searchQuery.toLowerCase().trim();
    return list.filter(
      (r) =>
        (r.number ?? "").toLowerCase().includes(q) ||
        (r.customerName ?? "").toLowerCase().includes(q) ||
        (r.customerCode ?? "").toLowerCase().includes(q) ||
        (r.projectCode ?? "").toLowerCase().includes(q)
    );
  }, [data?.rows, searchQuery]);

  const csvColumns = useMemo<ExportColumn<ArAgingReportRowResponse>[]>(
    () => [
      {
        header: t("columns.billingNumber"),
        accessor: (r) => r.number ?? "-",
      },
      {
        header: t("columns.customer"),
        accessor: (r) => `${r.customerCode ?? "-"} ${r.customerName ?? "-"}`,
      },
      {
        header: t("columns.branch"),
        accessor: (r) => r.branchCode ?? "-",
      },
      {
        header: t("columns.dueDate"),
        accessor: (r) => (r.dueDate ? formatDate(r.dueDate, locale) : "-"),
      },
      {
        header: t("columns.overdueDays"),
        accessor: (r) => r.daysOverdue ?? 0,
      },
      {
        header: t("columns.agingBucket"),
        accessor: (r) => (r.agingBucket ? t(`agingBuckets.${r.agingBucket}`) : "-"),
      },
      {
        header: t("columns.remainingAmount"),
        accessor: (r) => r.outstandingAmount ?? 0,
      },
    ],
    [t, locale]
  );

  const csv = useCsvExport<ArAgingReportRowResponse>({
    columns: csvColumns,
    filename: "ar-aging-report",
    data: rows,
  });

  if (isLoading) {
    return (
      <div className="py-12 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  return (
    <div className="erp-card p-5 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="w-72">
          <Input
            id="ar-search"
            placeholder={t("filters.searchPlaceholder")}
            value={searchQuery}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            leftIcon={<IconSearch size={16} />}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        <Button
          type="button"
          variant="outline"
          size="md"
          className="h-11 min-h-[44px]"
          icon={<IconDownload size={16} />}
          disabled={csv.isExporting || rows.length === 0}
          onClick={() => void csv.exportAll()}
        >
          {csv.isExporting ? t("exportingCsv") : t("exportCsv")}
        </Button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs">
          <thead>
            <tr className="border-b border-erp-border">
              <th className="py-2.5 pr-3">{t("columns.billingNumber")}</th>
              <th className="py-2.5 pr-3">{t("columns.customer")}</th>
              <th className="py-2.5 pr-3 font-mono">{t("columns.branch")}</th>
              <th className="py-2.5 pr-3">{t("columns.dueDate")}</th>
              <th className="py-2.5 pr-3 text-right">{t("columns.overdueDays")}</th>
              <th className="py-2.5 pr-3">{t("columns.agingBucket")}</th>
              <th className="py-2.5 text-right">{t("columns.remainingAmount")}</th>
            </tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr>
                <td colSpan={7} className="py-8 text-center text-erp-text-muted">
                  {t("emptyRows")}
                </td>
              </tr>
            ) : (
              rows.map((row) => (
                <tr key={row.billingDocumentId ?? row.number} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2.5 pr-3 font-mono font-semibold text-erp-navy">
                    {row.number ?? "-"}
                  </td>
                  <td className="py-2.5 pr-3">
                    <span className="font-mono text-[11px] text-erp-text-muted">{row.customerCode}</span>{" "}
                    <span className="font-medium">{row.customerName ?? "-"}</span>
                  </td>
                  <td className="py-2.5 pr-3 font-mono">{row.branchCode ?? "-"}</td>
                  <td className="py-2.5 pr-3 font-mono">
                    {row.dueDate ? formatDate(row.dueDate, locale) : "-"}
                  </td>
                  <td className="py-2.5 pr-3 text-right font-mono">
                    {row.daysOverdue !== undefined ? (
                      <span className={row.daysOverdue > 30 ? "text-erp-danger font-bold" : ""}>
                        {row.daysOverdue}
                      </span>
                    ) : (
                      "-"
                    )}
                  </td>
                  <td className="py-2.5 pr-3">
                    <StatusBadge
                      label={row.agingBucket ? t(`agingBuckets.${row.agingBucket}`) : "-"}
                      variant={getArAgingVariant(row.agingBucket)}
                    />
                  </td>
                  <td className="py-2.5 text-right font-mono font-bold text-erp-navy">
                    {formatCurrency(row.outstandingAmount ?? 0, "THB", locale)}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// -------------------------------------------------------------
// View 6: Service SLA Report
// -------------------------------------------------------------
function ServiceSlaReportView({
  data,
  isLoading,
  searchQuery,
  onSearchChange,
}: {
  data: { rows?: ServiceSlaReportRowResponse[] | null } | undefined;
  isLoading: boolean;
  searchQuery: string;
  onSearchChange: (val: string) => void;
}) {
  const t = useTranslations("reporting");
  const locale = useLocale();

  const rows = useMemo(() => {
    const list = data?.rows ?? [];
    if (!searchQuery.trim()) return list;
    const q = searchQuery.toLowerCase().trim();
    return list.filter(
      (r) =>
        (r.number ?? "").toLowerCase().includes(q) ||
        (r.title ?? "").toLowerCase().includes(q) ||
        (r.projectCode ?? "").toLowerCase().includes(q) ||
        (r.branchCode ?? "").toLowerCase().includes(q)
    );
  }, [data?.rows, searchQuery]);

  const csvColumns = useMemo<ExportColumn<ServiceSlaReportRowResponse>[]>(
    () => [
      {
        header: t("columns.serviceRequestNumber"),
        accessor: (r) => r.number ?? "-",
      },
      {
        header: t("columns.title"),
        accessor: (r) => r.title ?? "-",
      },
      {
        header: t("columns.project"),
        accessor: (r) => r.projectCode ?? "-",
      },
      {
        header: t("columns.priority"),
        accessor: (r) => (r.priority ? t(`servicePriorities.${r.priority}`) : "-"),
      },
      {
        header: t("columns.warranty"),
        accessor: (r) =>
          t(r.inWarranty ? "warranty.inWarranty" : "warranty.outOfWarranty"),
      },
      {
        header: t("columns.slaStatus"),
        accessor: (r) => (r.slaStatus ? t(`slaStatuses.${r.slaStatus}`) : "-"),
      },
      {
        header: t("columns.elapsedHours"),
        accessor: (r) => (r.elapsedHours !== undefined ? r.elapsedHours.toFixed(1) : "-"),
      },
    ],
    [t]
  );

  const csv = useCsvExport<ServiceSlaReportRowResponse>({
    columns: csvColumns,
    filename: "service-sla-report",
    data: rows,
  });

  if (isLoading) {
    return (
      <div className="py-12 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  return (
    <div className="erp-card p-5 space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="w-72">
          <Input
            id="sla-search"
            placeholder={t("filters.searchPlaceholder")}
            value={searchQuery}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            leftIcon={<IconSearch size={16} />}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        <Button
          type="button"
          variant="outline"
          size="md"
          className="h-11 min-h-[44px]"
          icon={<IconDownload size={16} />}
          disabled={csv.isExporting || rows.length === 0}
          onClick={() => void csv.exportAll()}
        >
          {csv.isExporting ? t("exportingCsv") : t("exportCsv")}
        </Button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs">
          <thead>
            <tr className="border-b border-erp-border">
              <th className="py-2.5 pr-3">{t("columns.serviceRequestNumber")}</th>
              <th className="py-2.5 pr-3">{t("columns.title")}</th>
              <th className="py-2.5 pr-3">{t("columns.project")}</th>
              <th className="py-2.5 pr-3">{t("columns.priority")}</th>
              <th className="py-2.5 pr-3">{t("columns.warranty")}</th>
              <th className="py-2.5 pr-3 text-center">{t("columns.slaStatus")}</th>
              <th className="py-2.5 text-right">{t("columns.elapsedHours")}</th>
            </tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr>
                <td colSpan={7} className="py-8 text-center text-erp-text-muted">
                  {t("emptyRows")}
                </td>
              </tr>
            ) : (
              rows.map((row) => (
                <tr key={row.serviceRequestId ?? row.number} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2.5 pr-3 font-mono font-semibold text-erp-navy">
                    {row.number ?? "-"}
                  </td>
                  <td className="py-2.5 pr-3 font-medium">{row.title ?? "-"}</td>
                  <td className="py-2.5 pr-3 font-mono">{row.projectCode ?? "-"}</td>
                  <td className="py-2.5 pr-3">
                    <StatusBadge
                      label={row.priority ? t(`servicePriorities.${row.priority}`) : "-"}
                      variant={getPriorityVariant(row.priority)}
                    />
                  </td>
                  <td className="py-2.5 pr-3">
                    <StatusBadge
                      label={t(row.inWarranty ? "warranty.inWarranty" : "warranty.outOfWarranty")}
                      variant={row.inWarranty ? "success" : "neutral"}
                    />
                  </td>
                  <td className="py-2.5 pr-3 text-center">
                    <StatusBadge
                      label={row.slaStatus ? t(`slaStatuses.${row.slaStatus}`) : "-"}
                      variant={getSlaVariant(row.slaStatus)}
                    />
                  </td>
                  <td className="py-2.5 text-right font-mono">
                    {row.elapsedHours !== undefined ? (
                      <span className="font-semibold">
                        {t("slaElapsedOfTarget", {
                          elapsed: row.elapsedHours.toFixed(1),
                          target: row.resolveTargetHours ?? "-",
                        })}
                      </span>
                    ) : (
                      "-"
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// -------------------------------------------------------------
// Helper Status Badge Variant Mappings
// -------------------------------------------------------------
function getStageVariant(stage: string | null | undefined): BadgeVariant {
  switch (stage) {
    case "won":
      return "success";
    case "lost":
      return "danger";
    case "proposed":
      return "warning";
    case "qualified":
    case "surveying":
    case "estimating":
      return "info";
    default:
      return "neutral";
  }
}

function getQuotationStatusVariant(status: string | null | undefined): BadgeVariant {
  switch (status) {
    case "accepted":
      return "success";
    case "rejected":
      return "danger";
    case "issued":
      return "info";
    default:
      return "neutral";
  }
}

function getArAgingVariant(bucket: string | null | undefined): BadgeVariant {
  switch (bucket) {
    case "current":
      return "success";
    case "1_30":
      return "info";
    case "31_60":
      return "warning";
    case "61_90":
    case "over_90":
      return "danger";
    default:
      return "neutral";
  }
}

function getPriorityVariant(priority: string | null | undefined): BadgeVariant {
  switch (priority) {
    case "urgent":
      return "danger";
    case "high":
      return "warning";
    case "normal":
      return "info";
    default:
      return "neutral";
  }
}

function getSlaVariant(slaStatus: string | null | undefined): BadgeVariant {
  switch (slaStatus) {
    case "on_track":
    case "met":
      return "success";
    case "warning":
    case "paused":
      return "warning";
    case "breached":
      return "danger";
    default:
      return "neutral";
  }
}
