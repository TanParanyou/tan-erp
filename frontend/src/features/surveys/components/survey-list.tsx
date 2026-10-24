"use client";

import React, { useMemo } from "react";
import { useRouter } from "next/navigation";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityList } from "@/features/opportunities/api/opportunity-queries";
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
import { Avatar } from "@/components/ui/Avatar";
import {
  IconPlus,
  IconMapPin,
  IconEye,
} from "@/components/common/Icons";
import { useListState, type ListFilterRecord } from "@/hooks/useListState";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import type { OpportunityResponse } from "@/lib/api/api-client";
import { getOpportunityStageLabelKey } from "@/features/opportunities/opportunity-labels";

interface SurveyListFilters extends ListFilterRecord {
  stage?: string;
}

export function SurveyList() {
  const router = useRouter();
  const t = useTranslations("surveyList");
  const tOpp = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const listState = useListState<SurveyListFilters>({
    schema: {
      defaultSort: "code",
      defaultOrder: "desc",
      single: ["stage"],
      allowedSorts: ["code", "title", "stage"],
    },
    debounceMs: 350,
  });

  // Default to showing "surveying" stage if no stage filter is explicitly selected
  const activeStage = listState.params.filters.stage ?? "surveying";

  const query = useOpportunityList({
    search: listState.params.search || undefined,
    stage: activeStage === "all" ? undefined : activeStage,
    limit: 20,
  });

  const allItems: OpportunityResponse[] = useMemo(
    () => query.data?.pages.flatMap((page) => page.items ?? []) ?? [],
    [query.data]
  );

  const resolveStageBadgeVariant = (stage: string | null | undefined): "neutral" | "primary" | "success" | "warning" | "danger" | "info" => {
    const key = getOpportunityStageLabelKey(stage);
    switch (key) {
      case "surveying":
        return "primary";
      case "qualified":
        return "info";
      case "estimating":
        return "warning";
      case "won":
        return "success";
      default:
        return "neutral";
    }
  };

  const resolveStageLabel = (stage: string | null | undefined): string => {
    const key = getOpportunityStageLabelKey(stage);
    switch (key) {
      case "surveying":
        return t("stageSurveying");
      case "qualified":
        return t("stageQualified");
      case "estimating":
        return t("stageEstimating");
      default:
        return stage ?? "-";
    }
  };

  const columns = useMemo<Column<OpportunityResponse>[]>(
    () => [
      {
        id: "project",
        header: t("project"),
        accessorKey: "title",
        cell: (_value, opp) => (
          <TableEntityCell
            title={opp.title ?? "-"}
            subtitle={opp.code ?? undefined}
            href={`/${locale}/opportunities/${opp.id}`}
          />
        ),
      },
      {
        id: "customer",
        header: t("customer"),
        cell: (_value, opp) => {
          if (!opp.customer) return <span className="text-erp-text-muted">-</span>;
          const name = locale === "en"
            ? opp.customer.displayNameEn || opp.customer.displayNameTh || "-"
            : opp.customer.displayNameTh || opp.customer.displayNameEn || "-";
          return (
            <div className="flex flex-col text-xs">
              <span className="font-medium text-erp-text">{name}</span>
            </div>
          );
        },
      },
      {
        id: "surveyor",
        header: t("surveyor"),
        cell: (_value, opp) => {
          if (!opp.owner) return <span className="text-erp-text-muted">-</span>;
          return (
            <div className="flex items-center gap-1.5 min-w-0">
              <Avatar
                initial={opp.owner.displayName || undefined}
                size="sm"
                variant="navy"
                className="shrink-0"
              />
              <span className="text-xs text-erp-text font-medium truncate max-w-[130px]" title={opp.owner.displayName || undefined}>
                {opp.owner.displayName}
              </span>
            </div>
          );
        },
      },
      {
        id: "appointment",
        header: t("appointment"),
        cell: (_value, opp) => (
          <div className="flex flex-col text-xs">
            {opp.nextActionAtUtc ? (
              <>
                <span className="text-erp-text font-medium">
                  {new Date(opp.nextActionAtUtc).toLocaleDateString(locale === "th" ? "th-TH" : "en-US")}
                </span>
                {opp.nextActionNote && (
                  <span className="text-[11px] text-erp-text-muted mt-0.5 truncate max-w-[150px]" title={opp.nextActionNote}>
                    {opp.nextActionNote}
                  </span>
                )}
              </>
            ) : (
              <span className="text-erp-text-muted">-</span>
            )}
          </div>
        ),
      },
      {
        id: "stage",
        header: tOpp("stage"),
        cell: (_value, opp) => (
          <Badge variant={resolveStageBadgeVariant(opp.stage)}>
            {resolveStageLabel(opp.stage)}
          </Badge>
        ),
      },
      {
        id: "actions",
        header: tCommon("actions.view"),
        align: "right",
        cell: (_value, opp) => (
          <TableActionGroup>
            <TableAction
              label={t("actionSurvey")}
              href={`/${locale}/surveys/field?opportunityId=${opp.id}`}
              icon={<IconMapPin size={15} strokeWidth={2} />}
            />
            <TableAction
              label={t("actionView")}
              href={`/${locale}/opportunities/${opp.id}`}
              icon={<IconEye size={15} strokeWidth={2} />}
            />
          </TableActionGroup>
        ),
      },
    ],
    [locale, t, tOpp, tCommon]
  );

  const activeChips = useMemo<ActiveFilterChipItem[]>(() => {
    const chips: ActiveFilterChipItem[] = [];
    if (listState.params.filters.stage && listState.params.filters.stage !== "all") {
      chips.push({
        key: "stage",
        value: listState.params.filters.stage,
        label: `${tOpp("stage")}: ${resolveStageLabel(listState.params.filters.stage)}`,
      });
    }
    return chips;
  }, [listState.params.filters.stage, tOpp]);

  const handleChipRemove = (chipKey: string) => {
    if (chipKey === "stage") {
      listState.actions.setFilter("stage", undefined);
    }
  };

  const handleClearAll = () => {
    listState.actions.clearFilters();
  };

  return (
    <div className="space-y-4">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        actions={
          <Button
            href={`/${locale}/surveys/field`}
            variant="primary"
            size="md"
            icon={<IconPlus size={16} />}
          >
            {t("recordNewSurvey")}
          </Button>
        }
      />

      <ListToolbar
        activeFilters={
          activeChips.length > 0 ? (
            <ActiveFilterChips
              filters={activeChips}
              onRemove={handleChipRemove}
              onClear={handleClearAll}
            />
          ) : undefined
        }
      >
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 w-full">
          <div className="flex-1 max-w-md">
            <ListSearchInput
              id="survey-search-input"
              value={listState.draftSearch}
              onChange={(val) => listState.actions.setSearch(val)}
              onClear={() => listState.actions.setSearch("", true)}
              onSubmit={(val) => listState.actions.setSearch(val, true)}
              placeholder={t("searchPlaceholder")}
              isDebouncing={listState.isDebouncing}
            />
          </div>
          <div className="flex items-center gap-3">
            <ListFilterSelect
              id="survey-stage-filter"
              label={tOpp("stage")}
              value={listState.params.filters.stage ?? "surveying"}
              onChange={(val) => listState.actions.setFilter("stage", val || undefined)}
              options={[
                { value: "surveying", label: t("stageSurveying") },
                { value: "qualified", label: t("stageQualified") },
                { value: "estimating", label: t("stageEstimating") },
                { value: "all", label: t("stageAll") },
              ]}
              allOptionLabel={t("stageAll")}
              widthClassName="w-full sm:w-56"
            />
          </div>
        </div>
      </ListToolbar>

      {query.isLoading ? (
        <div className="flex min-h-[300px] items-center justify-center border border-erp-border bg-white" role="status">
          <MonoSpinner size="lg" />
        </div>
      ) : allItems.length === 0 ? (
        <EmptyState
          title={t("emptyTitle")}
          description={t("emptyDetail")}
          actionLabel={t("recordNewSurvey")}
          onAction={() => router.push(`/${locale}/surveys/field`)}
        />
      ) : (
        <div className="space-y-4">
          <DataTable<OpportunityResponse>
            data={allItems}
            columns={columns}
          />

          {query.hasNextPage && (
            <div className="flex justify-center pt-2 pb-6">
              <Button
                type="button"
                variant="outline"
                size="md"
                onClick={() => query.fetchNextPage()}
                disabled={query.isFetchingNextPage}
              >
                {query.isFetchingNextPage ? tCommon("states.loading") : tCommon("actions.loadMore")}
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
