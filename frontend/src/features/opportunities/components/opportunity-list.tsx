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
import { Avatar } from "@/components/ui/Avatar";
import {
  IconPlus,
  IconBriefcase,
  IconEye,
  IconDownload,
} from "@/components/common/Icons";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { getOpportunityStageLabelKey, CANONICAL_OPPORTUNITY_STAGES } from "../opportunity-labels";
import { getCustomerDisplayNames } from "@/features/customers/customer-labels";
import { useListState, type ListFilterRecord } from "@/hooks/useListState";
import { useDataExport } from "@/hooks/useDataExport";
import type { ExportColumn } from "@/lib/export/export-types";
import { ExportDropdown } from "@/components/ui/ExportDropdown";
import { apiClient, type OpportunityResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { CustomerAutocomplete } from "@/components/forms/CustomerAutocomplete";
import { UserAutocomplete } from "@/components/forms/UserAutocomplete";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";

interface OpportunityFilters extends ListFilterRecord {
  stage?: string;
  customerId?: string;
  ownerId?: string;
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
      single: ["stage", "customerId", "ownerId"],
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
      case "surveying":
        return t("stageSurveying");
      case "estimating":
        return t("stageEstimating");
      case "proposed":
        return t("stageProposed");
      case "won":
        return t("stageWon");
      case "lost":
        return t("stageLost");
      case "cancelled":
        return t("stageCancelled");
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
      case "surveying":
      case "estimating":
      case "proposed":
        return "warning";
      case "won":
        return "success";
      case "lost":
      case "cancelled":
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
    customerId: listState.params.filters.customerId || undefined,
    ownerId: listState.params.filters.ownerId || undefined,
    stage: listState.params.filters.stage || undefined,
    sortBy: listState.params.sort || undefined,
    sortOrder: listState.params.order || undefined,
  });

  const allItems = useMemo(
    () => data?.pages.flatMap((page) => page.items ?? []) ?? [],
    [data]
  );

  const exportColumns = useMemo<ExportColumn<OpportunityResponse>[]>(
    () => [
      { header: t("code"), accessor: (o) => o.code ?? o.id },
      { header: t("titleField"), accessor: (o) => o.title || "" },
      { header: t("customer"), accessor: (o) => o.customer?.displayNameTh || o.customer?.displayNameEn || o.customer?.code || "" },
      { header: t("owner"), accessor: (o) => o.owner?.displayName || "" },
      { header: t("stage"), accessor: (o) => resolveStageLabel(o.stage) },
      { header: t("expectedBudget"), accessor: (o) => o.expectedBudget ?? "" },
      { header: t("currencyCode"), accessor: (o) => o.currencyCode ?? "THB" },
      { header: t("targetDecisionDate"), accessor: (o) => o.targetDecisionDate ?? "" },
      { header: t("scopeSummary"), accessor: (o) => o.scopeSummary ?? "" },
    ],
    [t]
  );

  const { exportAll, isExporting } = useDataExport<OpportunityResponse>({
    filename: "opportunities",
    columns: exportColumns,
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
          customerId: listState.params.filters.customerId || undefined,
          ownerId: listState.params.filters.ownerId || undefined,
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
        id: "customer",
        header: t("customer"),
        cell: (_value, opp) => {
          if (!opp.customer) return "-";
          const { primary } = getCustomerDisplayNames(
            opp.customer.displayNameTh,
            opp.customer.displayNameEn,
            locale as "th" | "en",
          );
          return (
            <span className="text-xs text-erp-text font-medium">
              {primary}
            </span>
          );
        },
      },
      {
        id: "owner",
        header: t("owner"),
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

  const [selectedCustomerLabel, setSelectedCustomerLabel] = React.useState<string | null>(null);
  const [selectedOwnerLabel, setSelectedOwnerLabel] = React.useState<string | null>(null);

  const activeChips = useMemo<ActiveFilterChipItem[]>(() => {
    const chips: ActiveFilterChipItem[] = [];
    if (listState.params.filters.stage) {
      chips.push({
        key: "stage",
        value: listState.params.filters.stage,
        label: `${t("stage")}: ${resolveStageLabel(listState.params.filters.stage)}`,
      });
    }
    if (listState.params.filters.customerId) {
      let custName: string | null = selectedCustomerLabel;
      if (!custName) {
        const match = allItems.find((o) => o.customer?.id === listState.params.filters.customerId);
        if (match?.customer) {
          custName = (locale === "en" && match.customer.displayNameEn
            ? match.customer.displayNameEn
            : match.customer.displayNameTh) ?? null;
        }
      }
      chips.push({
        key: "customerId",
        value: listState.params.filters.customerId,
        label: `${t("customer")}: ${custName || listState.params.filters.customerId}`,
      });
    }
    if (listState.params.filters.ownerId) {
      let ownerName: string | null = selectedOwnerLabel;
      if (!ownerName) {
        const match = allItems.find((o) => o.owner?.id === listState.params.filters.ownerId);
        if (match?.owner?.displayName) {
          ownerName = match.owner.displayName;
        }
      }
      chips.push({
        key: "ownerId",
        value: listState.params.filters.ownerId,
        label: `${t("owner")}: ${ownerName || listState.params.filters.ownerId}`,
      });
    }
    return chips;
  }, [listState.params.filters, allItems, selectedCustomerLabel, selectedOwnerLabel, locale, t]);

  const isZeroOpportunities =
    !isLoading &&
    !isError &&
    allItems.length === 0 &&
    !listState.params.search &&
    !listState.params.filters.stage &&
    !listState.params.filters.customerId &&
    !listState.params.filters.ownerId;

  return (
    <div className="flex flex-col gap-5">
      {/* 2-Tier Architectural Page Header */}
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        actions={
          canCreate ? (
            <Button
              href={`/${locale}/opportunities/create`}
              variant="primary"
              size="md"
              icon={<IconPlus size={16} strokeWidth={2} />}
            >
              {t("createOpportunity")}
            </Button>
          ) : undefined
        }
      />

      {/* List Toolbar with Search, Customer, Owner, and Stage Filters */}
      <ListToolbar
        activeFilters={
          activeChips.length > 0 ? (
            <ActiveFilterChips
              filters={activeChips}
              onRemove={(key) => {
                if (key === "customerId") setSelectedCustomerLabel(null);
                if (key === "ownerId") setSelectedOwnerLabel(null);
                listState.actions.setFilter(key as keyof OpportunityFilters, undefined);
              }}
              onClear={() => {
                setSelectedCustomerLabel(null);
                setSelectedOwnerLabel(null);
                listState.actions.clearFilters();
              }}
            />
          ) : undefined
        }
      >
        <ListSearchInput
          id="opportunity-search-input"
          value={listState.draftSearch}
          onChange={(val) => listState.actions.setSearch(val)}
          onClear={() => listState.actions.setSearch("", true)}
          onSubmit={(val) => listState.actions.setSearch(val, true)}
          placeholder={t("searchPlaceholder")}
          isDebouncing={listState.isDebouncing}
          label={tCommon("actions.search")}
          widthClassName="w-full md:w-64 shrink-0"
        />

        <div className="flex flex-wrap items-end gap-3 flex-1 min-w-0">
          <ListFilterSelect
            id="filter-stage-select"
            label={t("stage")}
            value={listState.params.filters.stage || ""}
            onChange={(val) => listState.actions.setFilter("stage", val || undefined)}
            options={CANONICAL_OPPORTUNITY_STAGES.map((s) => ({
              value: s,
              label: resolveStageLabel(s),
            }))}
            widthClassName="w-full sm:w-40 shrink-0"
          />

          <CustomerAutocomplete
            variant="filter"
            value={listState.params.filters.customerId || ""}
            onChange={(val) => {
              listState.actions.setFilter("customerId", val || undefined);
              if (!val) setSelectedCustomerLabel(null);
            }}
            onSelectedCustomerChange={(c) => {
              if (c) {
                const name = (locale === "en" && c.displayNameEn ? c.displayNameEn : (c.displayNameTh || c.code || c.id)) ?? null;
                setSelectedCustomerLabel(name);
              } else {
                setSelectedCustomerLabel(null);
              }
            }}
            label={t("customer")}
            placeholder={t("customerFilterPlaceholder")}
            className="w-full sm:w-56 shrink-0"
          />

          <UserAutocomplete
            variant="filter"
            value={listState.params.filters.ownerId || ""}
            onChange={(val) => {
              listState.actions.setFilter("ownerId", val || undefined);
              if (!val) setSelectedOwnerLabel(null);
            }}
            onSelectedUserChange={(u) => {
              if (u) {
                setSelectedOwnerLabel(u.displayName || u.email || u.id || null);
              } else {
                setSelectedOwnerLabel(null);
              }
            }}
            label={t("owner")}
            placeholder={t("ownerFilterPlaceholder")}
            className="w-full sm:w-48 shrink-0"
          />
        </div>

        {/* Export Dropdown Button (CSV & Excel, Aligned to bottom edge of inputs) */}
        <div className="flex items-end self-end">
          <ExportDropdown
            onExport={(format) => exportAll(format)}
            isLoading={isExporting}
            disabled={allItems.length === 0}
            label={t("export")}
            variant="outline"
            size="sm"
          />
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
                size="md"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                className="min-w-[160px] text-xs font-semibold"
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
