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
        <div className="flex min-h-[44px] items-center gap-3 border border-erp-navy/40 bg-erp-surface px-3 py-2">
          <span className="min-w-0 flex-1 truncate text-sm text-erp-text-main">{item ? `${item.code ?? "-"} · ${nameOf(item)}` : "-"}</span>
          {!disabled && <button type="button" className="min-h-[36px] px-2 text-xs text-erp-text-muted hover:text-erp-navy" onClick={onClear}>{common("actions.clear")}</button>}
        </div>
      )}
      renderListItem={(item) => (
        <span className="min-w-0 truncate text-sm">{item.code ?? "-"} · {nameOf(item)}{item.baseUnit?.code ? ` (${item.baseUnit.code})` : ""}</span>
      )}
    />
  );
}
