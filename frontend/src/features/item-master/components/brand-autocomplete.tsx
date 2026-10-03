"use client";

import { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Avatar } from "@/components/ui/Avatar";
import type { ItemBrandResponse } from "@/lib/api/api-client";
import { useItemMasterLookups } from "@/features/item-master/api/item-master-queries";
import { EntityAutocomplete } from "@/components/forms/EntityAutocomplete";

export interface BrandAutocompleteProps {
  value?: string | null;
  onChange: (brandId: string) => void;
  selectedOption?: ItemBrandResponse | null;
  onSelectedOptionChange?: (brand: ItemBrandResponse | null) => void;
  label?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
}

export function BrandAutocomplete({
  value,
  onChange,
  selectedOption,
  onSelectedOptionChange,
  label,
  placeholder,
  error,
  required,
  disabled,
  className,
}: BrandAutocompleteProps) {
  const locale = useLocale();
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const lookups = useItemMasterLookups();
  const [searchQuery, setSearchQuery] = useState("");
  const options = useMemo(() => {
    const query = searchQuery.trim().toLocaleLowerCase(locale);
    return (lookups.data?.brands ?? []).filter((brand) => {
      if (brand.status !== "active" || !brand.id) return false;
      const name = locale === "en" ? brand.name?.english ?? "" : brand.name?.thai ?? "";
      return !query || `${brand.code ?? ""} ${name}`.toLocaleLowerCase(locale).includes(query);
    });
  }, [locale, lookups.data?.brands, searchQuery]);
  const resolvedLabel = label ?? t("brand");

  return (
    <EntityAutocomplete<ItemBrandResponse>
      value={value}
      onChange={onChange}
      selectedItem={selectedOption}
      onSelectedItemChange={onSelectedOptionChange}
      label={resolvedLabel}
      placeholder={placeholder ?? t("brandSearchPlaceholder")}
      error={error}
      required={required}
      disabled={disabled || lookups.isError}
      className={className}
      items={options}
      isLoading={lookups.isLoading}
      emptyText={t("brandNoResults")}
      loadingText={t("taxonomyLoading")}
      onSearchChange={setSearchQuery}
      getItemKey={(brand) => brand.id ?? ""}
      renderSelectedCard={(onClear, brand) => (
        <div className="flex min-h-[44px] items-center gap-3 border border-erp-navy/40 bg-erp-surface px-3 py-2">
          <Avatar initial={(locale === "en" ? brand?.name?.english : brand?.name?.thai) ?? undefined} fileId={brand?.imageFileId} alt={resolvedLabel} size="sm" />
          <span className="min-w-0 flex-1 truncate text-sm text-erp-text-main">
            {brand ? `${brand.code ?? "-"} · ${locale === "en" ? brand.name?.english ?? "-" : brand.name?.thai ?? "-"}` : "-"}
          </span>
          {!disabled && <button type="button" className="min-h-[36px] px-2 text-xs text-erp-text-muted hover:text-erp-navy" onClick={onClear}>{common("actions.clear")}</button>}
        </div>
      )}
      renderListItem={(brand) => (
        <div className="flex min-w-0 items-center gap-3">
          <Avatar initial={(locale === "en" ? brand.name?.english : brand.name?.thai) ?? undefined} fileId={brand.imageFileId} alt={resolvedLabel} size="sm" />
          <span className="min-w-0 truncate text-sm">{brand.code ?? "-"} · {locale === "en" ? brand.name?.english ?? "-" : brand.name?.thai ?? "-"}</span>
        </div>
      )}
    />
  );
}
