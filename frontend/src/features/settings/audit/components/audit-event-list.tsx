"use client";

import React, { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { ListDateRangeFilter } from "@/components/ui/ListDateRangeFilter";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { Input } from "@/components/ui/Input";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { LIST_PAGE_SIZES, useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import type { AuditEventResponse } from "@/lib/api/api-client";
import { formatDateTime } from "@/lib/formatters/formatters";
import { useAuditEvents } from "../api/audit-queries";
import { AuditInspectorDrawer, resolveActionBadgeVariant } from "./audit-inspector-drawer";

interface AuditFilters extends ListFilterRecord {
  from?: string;
  to?: string;
  action?: string;
  resourceType?: string;
  resourceId?: string;
}

const KNOWN_RESOURCE_TYPES = [
  "Customer",
  "CustomerAddress",
  "CustomerContact",
  "Site",
  "Opportunity",
  "SiteSurvey",
  "SiteSurveyRevision",
  "Estimate",
  "Quotation",
  "BillingDocument",
  "BillingAdjustmentNote",
  "Payment",
  "Item",
  "item",
  "item-brand",
  "item-category",
  "CostSource",
  "cost_record",
  "AttachmentOwner",
  "User",
  "Role",
  "RoleAssignmentRequest",
  "Membership",
  "ApprovalLimit",
  "Branch",
  "DocumentSequence",
  "DocumentTemplate",
] as const;

const toListPageSize = (value: number): ListPageSize | undefined => LIST_PAGE_SIZES.find((size) => size === value);

export function AuditEventList() {
  const t = useTranslations("auditLog");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const [selectedEvent, setSelectedEvent] = useState<AuditEventResponse | null>(null);
  const [isInspectorOpen, setIsInspectorOpen] = useState(false);

  const listState = useListState<AuditFilters>({
    schema: { single: ["from", "to", "action", "resourceType", "resourceId"] },
    debounceMs: 350,
  });
  const { page, limit, filters, search } = listState.params;

  const { data, isLoading, isError, refetch } = useAuditEvents({
    from: filters.from ? `${filters.from}T00:00:00Z` : undefined,
    to: filters.to ? `${filters.to}T23:59:59Z` : undefined,
    action: filters.action || undefined,
    resourceType: filters.resourceType || undefined,
    resourceId: (filters.resourceId || search) ? (filters.resourceId || search).trim() : undefined,
    page,
    limit,
  });

  const rows = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 1;
  const isZero =
    !isLoading &&
    !isError &&
    totalItems === 0 &&
    !filters.from &&
    !filters.to &&
    !filters.action &&
    !filters.resourceType &&
    !filters.resourceId &&
    !search;

  const resourceTypeOptions = useMemo(() => {
    return KNOWN_RESOURCE_TYPES.map((res) => ({ value: res, label: res }));
  }, []);

  const activeChips = useMemo<ActiveFilterChipItem[]>(() => {
    const chips: ActiveFilterChipItem[] = [];
    if (search) {
      chips.push({
        key: "search",
        value: search,
        label: `${t("list.searchLabel")}: ${search}`,
      });
    }
    if (filters.action) {
      chips.push({
        key: "action",
        value: filters.action,
        label: `${t("list.actionFilter")}: ${filters.action}`,
      });
    }
    if (filters.resourceType) {
      chips.push({
        key: "resourceType",
        value: filters.resourceType,
        label: `${t("list.resourceTypeFilter")}: ${filters.resourceType}`,
      });
    }
    if (filters.resourceId) {
      chips.push({
        key: "resourceId",
        value: filters.resourceId,
        label: `${t("list.resourceIdFilter")}: ${filters.resourceId}`,
      });
    }
    if (filters.from || filters.to) {
      chips.push({
        key: "dateRange",
        value: "range",
        label: `${filters.from ?? "..."} - ${filters.to ?? "..."}`,
      });
    }
    return chips;
  }, [filters, search, t]);

  const handleOpenInspector = (event: AuditEventResponse) => {
    setSelectedEvent(event);
    setIsInspectorOpen(true);
  };

  const handleCloseInspector = () => {
    setIsInspectorOpen(false);
  };

  const handleRemoveChip = (key: string) => {
    if (key === "search") {
      listState.actions.setSearch("", true);
    } else if (key === "dateRange") {
      listState.actions.setFilters({ from: undefined, to: undefined });
    } else {
      listState.actions.setFilter(key as keyof AuditFilters, undefined);
    }
  };

  const handleClearAll = () => {
    listState.actions.clearFilters();
  };

  const columns = useMemo<Column<AuditEventResponse>[]>(
    () => [
      {
        id: "occurredAt",
        header: t("list.table.occurredAt"),
        className: "whitespace-nowrap font-mono text-xs",
        cell: (_v, row) => (
          <span className="font-mono text-xs text-erp-text-main font-medium">
            {formatDateTime(row.occurredAtUtc, locale)}
          </span>
        ),
      },
      {
        id: "action",
        header: t("list.table.action"),
        className: "whitespace-nowrap",
        cell: (_v, row) =>
          row.action ? (
            <StatusBadge label={row.action} variant={resolveActionBadgeVariant(row.action)} />
          ) : (
            "-"
          ),
      },
      {
        id: "resourceType",
        header: t("list.table.resourceType"),
        className: "whitespace-nowrap",
        cell: (_v, row) =>
          row.resourceType ? (
            <span className="font-mono text-xs font-semibold text-erp-navy bg-erp-surface-muted px-2 py-0.5 border border-erp-border">
              {row.resourceType}
            </span>
          ) : (
            "-"
          ),
      },
      {
        id: "resourceId",
        header: t("list.table.resourceId"),
        className: "font-mono text-xs",
        cell: (_v, row) =>
          row.resourceId ? (
            <span className="font-mono text-xs text-erp-text-main font-medium">
              {row.resourceId}
            </span>
          ) : (
            "-"
          ),
      },
      {
        id: "actor",
        header: t("list.table.actor"),
        className: "whitespace-nowrap font-medium",
        cell: (_v, row) => row.actor?.name ?? "-",
      },
      {
        id: "reason",
        header: t("list.table.reason"),
        cell: (_v, row) => (
          <span className="text-xs text-erp-text-muted line-clamp-1 max-w-xs" title={row.reason ?? ""}>
            {row.reason ?? "-"}
          </span>
        ),
      },
      {
        id: "actions",
        header: <span className="sr-only">{t("list.table.actions")}</span>,
        className: "w-24 text-right",
        cell: (_v, row) => (
          <Button
            variant="ghost"
            size="sm"
            onClick={() => handleOpenInspector(row)}
            aria-label={`${t("list.inspect")} ${row.resourceId || row.id || ""}`}
            className="text-xs text-erp-navy font-semibold hover:bg-erp-surface-muted"
          >
            {t("list.inspect")}
          </Button>
        ),
      },
    ],
    [locale, t]
  );

  return (
    <div className="space-y-6">
      <PageHeader
        title={t("list.title")}
        subtitle={t("list.subtitle")}
      />

      {/* 2-Tier Architectural Filter Toolbar Standards (design.md 4.5 & AGENTS.md) */}
      <ListToolbar
        activeFilters={
          activeChips.length > 0 ? (
            <ActiveFilterChips
              filters={activeChips}
              onRemove={handleRemoveChip}
              onClear={handleClearAll}
            />
          ) : undefined
        }
      >
        <div className="flex flex-col gap-3 w-full">
          {/* Tier 1: Search Input */}
          <div className="flex flex-col sm:flex-row items-stretch sm:items-end justify-between gap-3 w-full">
            <ListSearchInput
              id="audit-search-input"
              label={t("list.searchLabel")}
              value={listState.draftSearch}
              isDebouncing={listState.isDebouncing}
              placeholder={t("list.searchPlaceholder")}
              onChange={(val) => listState.actions.setSearch(val)}
              onClear={() => listState.actions.setSearch("", true)}
              onSubmit={(val) => listState.actions.setSearch(val, true)}
              widthClassName="w-full sm:max-w-md lg:max-w-lg"
            />
          </div>

          {/* Tier 2: Refined 4-column Grid with thin separator */}
          <div className="border-t border-erp-border/60 pt-3 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 items-end w-full">
            <ListFilterSelect
              id="audit-filter-resource-type"
              label={t("list.resourceTypeFilter")}
              value={filters.resourceType ?? ""}
              options={resourceTypeOptions}
              allOptionLabel={t("list.allResourceTypes")}
              onChange={(val) => listState.actions.setFilter("resourceType", val || undefined)}
              widthClassName="w-full"
            />

            <Input
              id="audit-filter-action"
              label={t("list.actionFilter")}
              value={filters.action ?? ""}
              placeholder={t("list.actionPlaceholder")}
              onChange={(e) => listState.actions.setFilter("action", e.target.value.trim() || undefined)}
              wrapperClassName="!mb-0 w-full"
              labelClassName="text-sm font-semibold text-erp-navy min-h-[20px] flex items-center"
              className="h-11 min-h-[44px] text-xs sm:text-sm"
            />

            <div className="sm:col-span-2">
              <ListDateRangeFilter
                label={t("list.dateRangeLabel")}
                startDate={filters.from ?? null}
                endDate={filters.to ?? null}
                onChange={(range) => {
                  listState.actions.setFilters({
                    from: range.startDate || undefined,
                    to: range.endDate || undefined,
                  });
                }}
              />
            </div>
          </div>
        </div>
      </ListToolbar>

      {isZero ? (
        <EmptyState icon="empty" title={t("list.emptyTitle")} description={t("list.emptyDescription")} />
      ) : (
        <DataTable<AuditEventResponse>
          columns={columns}
          data={rows}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
          pagination={{ page, limit, totalPages, totalItems }}
          onPageChange={(next) => listState.actions.setPage(next)}
          onLimitChange={(next) => {
            const pageSize = toListPageSize(next);
            if (pageSize) listState.actions.setLimit(pageSize);
          }}
        />
      )}

      {/* Forensic Inspection Drawer */}
      <AuditInspectorDrawer
        event={selectedEvent}
        isOpen={isInspectorOpen}
        onClose={handleCloseInspector}
      />
    </div>
  );
}
