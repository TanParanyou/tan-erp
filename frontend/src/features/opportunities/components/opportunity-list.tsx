"use client";

import React, { useMemo } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityList } from "../api/opportunity-queries";
import { isAuthenticationRequiredError, isMembershipRequiredError } from "@/lib/api/api-error";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { Button } from "@/components/ui/Button";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { Badge } from "@/components/ui/Badge";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import {
  IconPlus,
  IconBriefcase,
  IconEye,
  IconDownload,
} from "@/components/common/Icons";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { getOpportunityStageLabelKey, CANONICAL_OPPORTUNITY_STAGES } from "../opportunity-labels";
import { useListState, type ListFilterRecord } from "@/hooks/useListState";
import { useCsvExport } from "@/hooks/useCsvExport";
import type { CsvColumn } from "@/lib/export/export-csv";
import { apiClient, type OpportunityResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";

interface OpportunityFilters extends ListFilterRecord {
  stage?: string;
}

export function OpportunityList() {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();

  const listState = useListState<OpportunityFilters>({
    schema: {
      defaultSort: "code",
      defaultOrder: "desc",
      single: ["stage"],
      allowedSorts: ["code", "title", "expectedBudget", "stage"],
    },
    debounceMs: 350,
  });

  const canCreate = can(selectedMembership, "opportunities.create");

  const resolveStageLabel = (stage: string | null | undefined): string => {
    const key = getOpportunityStageLabelKey(stage);
    if (!key) return t("unknownStage");
    switch (key) {
      case "draft":
        return t("stageDraft");
      case "qualified":
        return t("stageQualified");
      case "estimation":
        return t("stageEstimation");
      case "proposal":
        return t("stageProposal");
      case "won":
        return t("stageWon");
      case "lost":
        return t("stageLost");
      default:
        return t("unknownStage");
    }
  };

  const resolveStageVariant = (stage: string | null | undefined): "neutral" | "primary" | "success" | "warning" | "danger" | "info" => {
    const key = getOpportunityStageLabelKey(stage);
    switch (key) {
      case "draft":
        return "neutral";
      case "qualified":
        return "info";
      case "estimation":
      case "proposal":
        return "warning";
      case "won":
        return "success";
      case "lost":
        return "danger";
      default:
        return "neutral";
    }
  };

  const resolveListErrorMessage = (error: Error | null): string => {
    if (isAuthenticationRequiredError(error)) {
      return tCommon("feedback.operationFailed");
    }
    if (isMembershipRequiredError(error)) {
      return t("errors.branchRequiredDetail");
    }
    return t("errors.loadList");
  };

  const {
    data,
    isLoading,
    isError,
    error,
    refetch,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useOpportunityList({
    search: listState.params.search || undefined,
    stage: listState.params.filters.stage || undefined,
  });

  const allItems = useMemo(
    () => data?.pages.flatMap((page) => page.items ?? []) ?? [],
    [data]
  );

  const csvColumns = useMemo<CsvColumn<OpportunityResponse>[]>(
    () => [
      { header: t("code"), accessor: (o) => o.code ?? o.id },
      { header: t("titleField"), accessor: (o) => o.title || "" },
      { header: t("stage"), accessor: (o) => resolveStageLabel(o.stage) },
      { header: t("expectedBudget"), accessor: (o) => o.expectedBudget ?? "" },
      { header: t("currencyCode"), accessor: (o) => o.currencyCode ?? "THB" },
      { header: t("targetDecisionDate"), accessor: (o) => o.targetDecisionDate ?? "" },
      { header: t("scopeSummary"), accessor: (o) => o.scopeSummary ?? "" },
    ],
    [t]
  );

  const { exportAll, isExporting } = useCsvExport<OpportunityResponse>({
    filename: "opportunities",
    columns: csvColumns,
    data: allItems,
    fetchAll: async () => {
      const token = await getAuthToken();
      if (!token || !selectedMembership?.id) return allItems;
      const res = await apiClient.listOpportunities(
        {
          token,
          membershipId: selectedMembership.id,
          locale: locale === "en" ? "en" : "th",
        },
        {
          search: listState.params.search || undefined,
          stage: listState.params.filters.stage || undefined,
          limit: 1000,
        }
      );
      return res.items ?? [];
    },
  });

  const columns = useMemo<Column<OpportunityResponse>[]>(
    () => [
      {
        id: "code",
        header: `${t("titleField")} / ${t("code")}`,
        sortable: true,
        accessorKey: "code",
        cell: (_value, opp) => (
          <TableEntityCell
            title={opp.title || "-"}
            code={opp.code ?? opp.id}
            subtitle={opp.scopeSummary}
            href={`/${locale}/opportunities/${opp.id}`}
          />
        ),
      },
      {
        id: "stage",
        header: t("stage"),
        sortable: true,
        accessorKey: "stage",
        cell: (_value, opp) => (
          <Badge variant={resolveStageVariant(opp.stage)}>
            {resolveStageLabel(opp.stage)}
          </Badge>
        ),
      },
      {
        id: "expectedBudget",
        header: t("expectedBudget"),
        align: "right",
        sortable: true,
        accessorKey: "expectedBudget",
        cell: (_value, opp) => (
          <span className="font-mono text-xs text-erp-text font-medium">
            {opp.expectedBudget !== null && opp.expectedBudget !== undefined
              ? `${opp.expectedBudget.toLocaleString()} ${opp.currencyCode ?? "THB"}`
              : "-"}
          </span>
        ),
      },
      {
        id: "nextActionAt",
        header: t("nextActionAt"),
        cell: (_value, opp) => (
          <div className="flex flex-col text-xs">
            {opp.nextActionAtUtc ? (
              <>
                <span className="text-erp-text">
                  {new Date(opp.nextActionAtUtc).toLocaleDateString(locale === "th" ? "th-TH" : "en-US")}
                </span>
                {opp.nextActionNote && (
                  <span className="text-[11px] text-erp-text-muted mt-0.5">{opp.nextActionNote}</span>
                )}
              </>
            ) : (
              <span className="text-erp-text-muted">-</span>
            )}
          </div>
        ),
      },
      {
        id: "actions",
        header: tCommon("actions.view"),
        align: "right",
        cell: (_value, opp) => (
          <TableActionGroup>
            <TableAction
              label={tCommon("actions.view")}
              href={`/${locale}/opportunities/${opp.id}`}
              icon={<IconEye size={15} strokeWidth={2} />}
            />
          </TableActionGroup>
        ),
      },
    ],
    [locale, t, tCommon]
  );

  const isZeroOpportunities =
    !isLoading &&
    !isError &&
    allItems.length === 0 &&
    !listState.params.search &&
    !listState.params.filters.stage;

  return (
    <div className="flex flex-col gap-5">
      {/* 2-Tier Architectural Page Header */}
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        actions={
          canCreate ? (
            <Link
              href={`/${locale}/opportunities/create`}
              className="inline-flex items-center gap-2 h-9 px-4 text-xs font-semibold text-white bg-erp-navy hover:bg-erp-navy-hover transition-colors rounded-none focus:outline-none focus:ring-2 focus:ring-erp-navy focus:ring-offset-1"
            >
              <IconPlus size={16} strokeWidth={2.5} />
              <span>{t("createOpportunity")}</span>
            </Link>
          ) : undefined
        }
      />

      {/* List Toolbar with Search and Stage Filter */}
      <ListToolbar>
        <ListSearchInput
          id="opportunity-search-input"
          value={listState.draftSearch}
          onChange={(val) => listState.actions.setSearch(val)}
          onClear={() => listState.actions.setSearch("", true)}
          onSubmit={(val) => listState.actions.setSearch(val, true)}
          placeholder={t("searchPlaceholder")}
          isDebouncing={listState.isDebouncing}
          label={tCommon("actions.search")}
          widthClassName="w-full md:w-72"
        />

        <div className="flex flex-wrap items-end gap-3 flex-1">
          <ListFilterSelect
            id="filter-stage-select"
            label={t("stage")}
            value={listState.params.filters.stage || ""}
            onChange={(val) => listState.actions.setFilter("stage", val || undefined)}
            options={CANONICAL_OPPORTUNITY_STAGES.map((s) => ({
              value: s,
              label: resolveStageLabel(s),
            }))}
            widthClassName="w-48"
          />
        </div>

        {/* Export CSV Button (Aligned to bottom edge of inputs) */}
        <div className="flex items-end self-end">
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={exportAll}
            isLoading={isExporting}
            disabled={allItems.length === 0}
            icon={<IconDownload size={15} />}
            className="h-10 text-xs font-medium min-h-[40px]"
          >
            {t("exportCsv")}
          </Button>
        </div>
      </ListToolbar>

      {/* Zero opportunities: Empty State */}
      {isZeroOpportunities ? (
        <EmptyState
          icon="empty"
          title={t("emptyTitle")}
          description={t("emptyDetail")}
          actionLabel={canCreate ? t("createOpportunity") : undefined}
          onAction={canCreate ? () => router.push(`/${locale}/opportunities/create`) : undefined}
        />
      ) : (
        <div className="space-y-4">
          <DataTable<OpportunityResponse>
            data={allItems}
            columns={columns}
            isLoading={isLoading}
            isError={isError}
            error={isError ? resolveListErrorMessage(error) : null}
            onRetry={() => refetch()}
            emptyTitle={tCommon("table.noData")}
            emptyDescription={t("searchPlaceholder")}
            sorting={{
              key: listState.params.sort || null,
              order: listState.params.order,
            }}
            onSort={(key) => listState.actions.setSort(key)}
            stickyActionColumn={true}
          />

          {/* Keyset Load More */}
          {hasNextPage && (
            <div className="flex justify-center pt-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                style={{ minHeight: "40px", minWidth: "160px" }}
                className="text-xs font-semibold"
              >
                {isFetchingNextPage ? <MonoSpinner size="sm" /> : t("loadMore")}
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
