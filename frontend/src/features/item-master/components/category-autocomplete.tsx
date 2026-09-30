"use client";

import { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Avatar } from "@/components/ui/Avatar";
import type { ItemCategoryResponse } from "@/lib/api/api-client";
import { useItemMasterLookups } from "@/features/item-master/api/item-master-queries";
import { EntityAutocomplete } from "@/components/forms/EntityAutocomplete";

export interface CategoryAutocompleteProps {
  value?: string | null;
  onChange: (categoryId: string) => void;
  selectedOption?: ItemCategoryResponse | null;
  onSelectedOptionChange?: (category: ItemCategoryResponse | null) => void;
  itemType?: string;
  label?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
}

export function CategoryAutocomplete({
  value,
  onChange,
  selectedOption,
  onSelectedOptionChange,
  itemType,
  label,
  placeholder,
  error,
  required,
  disabled,
  className,
}: CategoryAutocompleteProps) {
  const locale = useLocale();
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const lookups = useItemMasterLookups();
  const [searchQuery, setSearchQuery] = useState("");
  const options = useMemo(() => {
    const query = searchQuery.trim().toLocaleLowerCase(locale);
    return (lookups.data?.categories ?? []).filter((category) => {
      if (category.status !== "active" || !category.id) return false;
      if (itemType && !category.allowedItemTypes?.includes(itemType)) return false;
      const name = locale === "en" ? category.name?.english ?? "" : category.name?.thai ?? "";
      return !query || `${category.code ?? ""} ${name}`.toLocaleLowerCase(locale).includes(query);
    });
  }, [itemType, locale, lookups.data?.categories, searchQuery]);
  const resolvedLabel = label ?? t("category");

  return (
    <EntityAutocomplete<ItemCategoryResponse>
      value={value}
      onChange={onChange}
      selectedItem={selectedOption}
      onSelectedItemChange={onSelectedOptionChange}
      label={resolvedLabel}
      placeholder={placeholder ?? t("categorySearchPlaceholder")}
      error={error}
      required={required}
      disabled={disabled || lookups.isError}
      className={className}
      items={options}
      isLoading={lookups.isLoading}
      emptyText={t("categoryNoResults")}
      loadingText={t("taxonomyLoading")}
      onSearchChange={setSearchQuery}
      getItemKey={(category) => category.id ?? ""}
      renderSelectedCard={(onClear, category) => (
        <div className="flex min-h-[44px] items-center gap-3 border border-erp-navy/40 bg-erp-surface px-3 py-2">
          <Avatar initial={(locale === "en" ? category?.name?.english : category?.name?.thai) ?? undefined} fileId={category?.imageFileId} alt={resolvedLabel} size="sm" />
          <span className="min-w-0 flex-1 truncate text-sm text-erp-text-main">
            {category ? `${category.code ?? "-"} · ${locale === "en" ? category.name?.english ?? "-" : category.name?.thai ?? "-"}` : "-"}
          </span>
          {!disabled && <button type="button" className="min-h-[36px] px-2 text-xs text-erp-text-muted hover:text-erp-navy" onClick={onClear}>{common("actions.clear")}</button>}
        </div>
      )}
      renderListItem={(category) => (
        <div className="flex min-w-0 items-center gap-3">
          <Avatar initial={(locale === "en" ? category.name?.english : category.name?.thai) ?? undefined} fileId={category.imageFileId} alt={resolvedLabel} size="sm" />
          <span className="min-w-0 truncate text-sm">{category.code ?? "-"} · {locale === "en" ? category.name?.english ?? "-" : category.name?.thai ?? "-"}</span>
        </div>
      )}
    />
  );
}
