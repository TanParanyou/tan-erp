"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import type { ItemResponse } from "@/lib/api/api-client";
import { useItemList } from "@/features/item-master/api/item-master-queries";
import { EntityAutocomplete } from "@/components/forms/EntityAutocomplete";

export interface ItemAutocompleteProps {
  value?: string | null;
  onChange: (itemId: string, item: ItemResponse | null) => void;
  label?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  /** Only items whose capabilities allow purchasing. */
  purchasableOnly?: boolean;
  /** Only items that can be produced. */
  producibleOnly?: boolean;
  /** Only items that are tracked in stock. */
  stockableOnly?: boolean;
  className?: string;
}

const MAX_RESULTS = 20;

/** Searches active catalog items on the server (no client-side filtering of an unbounded list). */
export function ItemAutocomplete({
  value,
  onChange,
  label,
  placeholder,
  error,
  required,
  disabled,
  purchasableOnly = false,
  producibleOnly = false,
  stockableOnly = false,
  className,
}: ItemAutocompleteProps) {
  const locale = useLocale();
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<ItemResponse | null>(null);
  const query = useItemList({ search: search.trim() || undefined, status: "active", sortBy: "code", sortOrder: "asc", pageNumber: 1, pageSize: MAX_RESULTS });
  const items = (query.data?.items ?? []).filter((item) => item.id && (!purchasableOnly || item.capabilities?.canPurchase) && (!producibleOnly || item.capabilities?.canProduce) && (!stockableOnly || item.capabilities?.canStock));
  const nameOf = (item: ItemResponse | null | undefined): string => (locale === "en" ? item?.name?.english : item?.name?.thai) ?? "-";

  return (
    <EntityAutocomplete<ItemResponse>
      value={value}
      onChange={(id) => onChange(id, items.find((item) => item.id === id) ?? selected)}
      selectedItem={selected}
      onSelectedItemChange={setSelected}
      label={label ?? t("itemAutocompleteLabel")}
      placeholder={placeholder ?? t("itemAutocompletePlaceholder")}
      error={error}
      required={required}
      disabled={disabled}
      className={className}
      items={items}
      isLoading={query.isLoading}
      emptyText={t("noResults")}
      loadingText={t("loading")}
      onSearchChange={setSearch}
      getItemKey={(item) => item.id ?? ""}
      renderSelectedCard={(onClear, item) => (
        <div className="flex min-h-[44px] items-center justify-between gap-3 border border-erp-navy/40 bg-erp-surface px-3 py-1.5">
          <div className="flex min-w-0 flex-1 flex-col">
            <span className="truncate text-xs font-semibold text-erp-navy">
              {item ? nameOf(item) : "-"}
            </span>
            <span className="truncate text-[11px] text-erp-text-muted">
              {item?.code ?? "-"}
              {item?.baseUnit?.code ? ` · ${item.baseUnit.code}` : ""}
            </span>
          </div>
          {!disabled && (
            <button
              type="button"
              className="inline-flex min-h-[36px] min-w-[36px] items-center justify-center text-xs font-medium text-erp-text-muted hover:text-erp-navy"
              onClick={onClear}
            >
              {common("actions.clear")}
            </button>
          )}
        </div>
      )}
      renderListItem={(item, isHighlighted) => (
        <div className="flex min-w-0 flex-1 flex-col py-0.5 text-left">
          <span
            className={`truncate text-xs font-semibold ${
              isHighlighted ? "text-white" : "text-erp-navy"
            }`}
          >
            {nameOf(item)}
          </span>
          <span
            className={`truncate text-[11px] ${
              isHighlighted ? "text-white/80" : "text-erp-text-muted"
            }`}
          >
            {item.code ?? "-"}
            {item.baseUnit?.code ? ` · ${item.baseUnit.code}` : ""}
          </span>
        </div>
      )}
    />
  );
}
