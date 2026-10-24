"use client";

import React, { useState, useEffect } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import type { OpportunityResponse } from "@/lib/api/api-client";
import { useOpportunityList, useOpportunityDetail } from "../api/opportunity-queries";
import { EntityAutocomplete } from "@/components/forms/EntityAutocomplete";
import { IconPlus } from "@/components/common/Icons";

export interface OpportunityAutocompleteProps {
  value?: string | null;
  selectedOpportunity?: OpportunityResponse | null;
  onChange: (opportunityId: string, opportunity: OpportunityResponse | null) => void;
  label?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
  showCreateAction?: boolean;
  createActionHref?: string;
}

const MAX_OPPORTUNITIES = 20;

export function OpportunityAutocomplete({
  value,
  selectedOpportunity,
  onChange,
  label,
  placeholder,
  error,
  required,
  disabled = false,
  className,
  showCreateAction = true,
  createActionHref,
}: OpportunityAutocompleteProps) {
  const locale = useLocale();
  const t = useTranslations("fieldSurvey");
  const common = useTranslations("common");

  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<OpportunityResponse | null>(
    selectedOpportunity ?? null
  );

  const query = useOpportunityList({
    search: search.trim() || undefined,
    limit: MAX_OPPORTUNITIES,
  });

  const opportunities: OpportunityResponse[] =
    query.data?.pages.flatMap((page) => page.items ?? []) ?? [];

  // Auto-fetch detail if value is passed from URL/parent but selected isn't resolved yet
  const detailQuery = useOpportunityDetail(!selected && value ? value : undefined);

  useEffect(() => {
    if (detailQuery.data && !selected) {
      setSelected(detailQuery.data);
      onChange(value ?? "", detailQuery.data);
    }
  }, [detailQuery.data, selected, value, onChange]);

  const getCustomerName = (opp: OpportunityResponse | null | undefined): string => {
    if (!opp?.customer) return "-";
    return locale === "en"
      ? opp.customer.displayNameEn || opp.customer.displayNameTh || "-"
      : opp.customer.displayNameTh || opp.customer.displayNameEn || "-";
  };

  const handleSelect = (id: string) => {
    const item = opportunities.find((o) => o.id === id) ?? selected;
    setSelected(item);
    onChange(id, item);
  };

  const handleClear = () => {
    setSelected(null);
    onChange("", null);
  };

  const createAction = showCreateAction ? (
    <Link
      href={createActionHref ?? `/${locale}/opportunities/create`}
      target="_blank"
      className="inline-flex items-center gap-1 text-[11px] font-semibold text-erp-navy hover:underline"
    >
      <IconPlus size={12} />
      <span>{t("createNewOpportunity")}</span>
    </Link>
  ) : null;

  return (
    <div className={className}>
      <EntityAutocomplete<OpportunityResponse>
        value={value}
        onChange={handleSelect}
        selectedItem={selected ?? selectedOpportunity ?? null}
        onSelectedItemChange={setSelected}
        label={label ?? t("opportunity")}
        labelAction={createAction}
        placeholder={placeholder ?? t("searchOpportunityPlaceholder")}
        error={error}
        required={required}
        disabled={disabled}
        className="!mb-0"
        items={opportunities}
        isLoading={query.isLoading}
        emptyText={t("noOpportunityFound")}
        loadingText={common("states.loading")}
        onSearchChange={setSearch}
        getItemKey={(item) => item.id ?? ""}
        renderSelectedCard={() => (
          <div className="flex h-11 min-h-[44px] items-center justify-between border border-erp-navy/40 bg-erp-surface px-3 py-2 text-xs">
            <div className="min-w-0 flex-1 truncate pr-2">
              <span className="font-semibold text-erp-navy">
                {selected?.code ? `${selected.code} · ` : ""}
                {selected?.title}
              </span>
              <span className="ml-1 text-erp-text-muted">
                ({getCustomerName(selected)})
              </span>
            </div>
            {!disabled && (
              <button
                type="button"
                onClick={handleClear}
                className="inline-flex min-h-[36px] min-w-[36px] items-center justify-center text-xs font-medium text-erp-text-muted hover:text-erp-navy active:text-erp-navy-active"
              >
                {common("actions.clear")}
              </button>
            )}
          </div>
        )}
        renderListItem={(item, isHighlighted) => (
          <div className="flex flex-col py-1 text-xs">
            <div className="flex items-center justify-between gap-2">
              <span
                className={`truncate font-semibold ${
                  isHighlighted ? "text-white" : "text-erp-navy"
                }`}
              >
                {item.code ? `${item.code} · ` : ""}
                {item.title}
              </span>
              {item.stage && (
                <span
                  className={`shrink-0 border px-1.5 py-0.5 text-[10px] font-semibold uppercase tracking-wider ${
                    isHighlighted
                      ? "border-white/40 bg-white/20 text-white"
                      : "border-erp-border bg-erp-surface-subtle text-erp-text-main"
                  }`}
                >
                  {item.stage}
                </span>
              )}
            </div>
            <span
              className={`truncate ${
                isHighlighted ? "text-white/80" : "text-erp-text-muted"
              }`}
            >
              {getCustomerName(item)}
            </span>
          </div>
        )}
      />
    </div>
  );
}
