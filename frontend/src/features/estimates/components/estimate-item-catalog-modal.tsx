"use client";

import React, { useState, useEffect, useMemo, useCallback } from "react";
import { useTranslations, useLocale } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Checkbox } from "@/components/ui/Checkbox";
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from "@/components/ui/Table";
import { Avatar } from "@/components/ui/Avatar";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { SegmentedControl } from "@/components/ui/SegmentedControl";
import { cn } from "@/lib/utils/cn";
import { useIsMobile } from "@/hooks/useMediaQuery";
import {
  IconSearch,
  IconFilter,
  IconClose,
  IconChevronLeft,
  IconChevronRight,
  IconRefresh,
  IconMaximize,
  IconMinimize,
  IconChevronUp,
  IconChevronDown,
} from "@/components/common/Icons";
import {
  useEstimateCatalog,
} from "../hooks/use-estimate-catalog";
import { usePrivateItemImage } from "../hooks/use-private-item-image";
import {
  getLocalizedText,
  type CatalogItemModel,
} from "../api/estimate-catalog-client";
import { formatFinancialNumber } from "../utils/estimate-formatters";
import { getAttributeKeyLabel } from "@/lib/utils/item-attributes";

export interface EstimateItemCatalogModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectItems: (items: CatalogItemModel[]) => void;
  currency: string;
  branchId?: string;
  singleSelect?: boolean;
}

export type CatalogScope = "all" | "cost";

interface PrivateThumbnailProps {
  fileId?: string;
  altText: string;
}

function PrivateThumbnail({ fileId, altText }: PrivateThumbnailProps) {
  const { imageUrl, isLoading, isError } = usePrivateItemImage({
    fileId,
    enabled: Boolean(fileId),
  });

  return (
    <div
      className="w-10 h-10 mx-auto bg-erp-surface border border-erp-border flex items-center justify-center overflow-hidden shrink-0"
      title={altText}
    >
      {isLoading ? (
        <div className="w-full h-full bg-erp-surface-subtle animate-pulse" />
      ) : imageUrl && !isError ? (
        <img
          src={imageUrl}
          alt={altText}
          className="w-full h-full object-cover"
        />
      ) : (
        <svg
          width="18"
          height="18"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="1.5"
          strokeLinecap="square"
          aria-hidden="true"
          className="text-erp-text-muted/60"
        >
          <rect x="3" y="3" width="18" height="18" rx="0" ry="0" />
          <circle cx="8.5" cy="8.5" r="1.5" />
          <polyline points="21 15 16 10 5 21" />
        </svg>
      )}
    </div>
  );
}

export function EstimateItemCatalogModal({
  isOpen,
  onClose,
  onSelectItems,
  currency,
  branchId,
  singleSelect = false,
}: EstimateItemCatalogModalProps) {
  const t = useTranslations("estimates");
  const tc = useTranslations("common");
  const ti = useTranslations("itemMaster");
  const locale = useLocale();
  const isMobile = useIsMobile();

  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [scope, setScope] = useState<CatalogScope>("cost");
  const [selectedType, setSelectedType] = useState<string | undefined>(undefined);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | undefined>(undefined);
  const [selectedBrandId, setSelectedBrandId] = useState<string | undefined>(undefined);
  const [selectedAttributeKey, setSelectedAttributeKey] = useState<string | undefined>(undefined);
  const [selectedAttributeValue, setSelectedAttributeValue] = useState<string | undefined>(undefined);

  // Pagination state: cursor stack for backwards navigation
  const [cursor, setCursor] = useState<string | undefined>(undefined);
  const [cursorHistory, setCursorHistory] = useState<(string | undefined)[]>([undefined]);

  // Selected items map preserved across cursor-based pages
  const [selectedItemsMap, setSelectedItemsMap] = useState<Map<string, CatalogItemModel>>(
    new Map()
  );

  const [isFullscreen, setIsFullscreen] = useState(false);
  const [isTrayOpen, setIsTrayOpen] = useState(false);
  const [showMobileFilters, setShowMobileFilters] = useState(false);

  // Debounce search input
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search.trim());
      setCursor(undefined);
      setCursorHistory([undefined]);
    }, 300);
    return () => clearTimeout(handler);
  }, [search]);

  // Reset pagination when any filter changes
  const handleTypeChange = (type: string | undefined) => {
    setSelectedType((prev) => (prev === type ? undefined : type));
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  const handleCategoryChange = (catId: string | undefined) => {
    setSelectedCategoryId((prev) => (prev === catId ? undefined : catId));
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  const handleBrandChange = (bId: string | undefined) => {
    setSelectedBrandId((prev) => (prev === bId ? undefined : bId));
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  const handleAttributeKeyChange = (key: string) => {
    setSelectedAttributeKey(key || undefined);
    setSelectedAttributeValue(undefined);
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  const handleAttributeValueChange = (value: string) => {
    setSelectedAttributeValue(value || undefined);
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  const handleScopeChange = (newScope: CatalogScope) => {
    setScope(newScope);
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  // Reset selection on open
  useEffect(() => {
    if (isOpen) {
      setSelectedItemsMap(new Map());
      setSearch("");
      setDebouncedSearch("");
      setSelectedType(undefined);
      setSelectedCategoryId(undefined);
      setSelectedBrandId(undefined);
      setSelectedAttributeKey(undefined);
      setSelectedAttributeValue(undefined);
      setCursor(undefined);
      setCursorHistory([undefined]);
      setIsTrayOpen(false);
    }
  }, [isOpen]);

  const handleClearSelection = useCallback(() => {
    setSelectedItemsMap(new Map());
  }, []);

  const handleRemoveSelectedItem = useCallback((itemId: string) => {
    setSelectedItemsMap((prev) => {
      const next = new Map(prev);
      next.delete(itemId);
      return next;
    });
  }, []);

  const { data, isLoading: isQueryLoading, isPlaceholderData, isError, refetch } = useEstimateCatalog({
    branchId: branchId || "",
    search: debouncedSearch || undefined,
    itemType: selectedType,
    categoryId: selectedCategoryId,
    brandId: selectedBrandId,
    attributeKey: selectedAttributeKey,
    attributeValue: selectedAttributeValue,
    hasCost: scope === "cost" ? true : undefined,
    cursor,
    pageSize: 20,
    enabled: Boolean(isOpen && branchId),
  });

  const isLoading = isQueryLoading || Boolean(isPlaceholderData);
  const items = data?.items ?? [];
  const facets = data?.facets;
  const pageInfo = data?.pageInfo;
  const itemTypeLabels: Record<string, string> = {
    material: ti("itemTypeMaterial"),
    labor: ti("itemTypeLabor"),
    service: ti("itemTypeService"),
    product: ti("itemTypeProduct"),
    subcontract: ti("itemTypeSubcontract"),
    other: ti("itemTypeOther"),
  };
  const attributeFacets = facets?.attributes ?? [];
  const attributeKeys = Array.from(
    new Set(attributeFacets.map((facet) => facet.key ?? "").filter(Boolean))
  );
  const attributeValues = attributeFacets.filter(
    (facet) => facet.key === selectedAttributeKey
  );

  const getAttributeLabel = (key: string) => {
    const labels: Record<string, string> = {
      fixture: t("catalogFacets.attributeKeys.fixture"),
      item_type: t("catalogFacets.attributeKeys.itemType"),
      variant: t("catalogFacets.attributeKeys.variant"),
      material: t("catalogFacets.attributeKeys.material"),
      thickness_mm: t("catalogFacets.attributeKeys.thickness"),
      width_mm: t("catalogFacets.attributeKeys.width"),
      length_mm: t("catalogFacets.attributeKeys.length"),
      finish: t("catalogFacets.attributeKeys.finish"),
      surface: t("catalogFacets.attributeKeys.surface"),
      color: t("catalogFacets.attributeKeys.color"),
      grade: t("catalogFacets.attributeKeys.grade"),
    };

    return labels[key] ?? getAttributeKeyLabel(key, locale === "en" ? "en" : "th");
  };

  const isAllCurrentPageSelected = useMemo(() => {
    if (items.length === 0) return false;
    return items.every((item) => selectedItemsMap.has(item.id));
  }, [items, selectedItemsMap]);

  const handleToggleSelectAll = useCallback(() => {
    if (singleSelect) return;
    setSelectedItemsMap((prev) => {
      const next = new Map(prev);
      if (isAllCurrentPageSelected) {
        items.forEach((item) => next.delete(item.id));
      } else {
        items.forEach((item) => next.set(item.id, item));
      }
      return next;
    });
  }, [items, isAllCurrentPageSelected, singleSelect]);

  const handleToggleItem = useCallback((item: CatalogItemModel) => {
    setSelectedItemsMap((prev) => {
      if (singleSelect) return prev.has(item.id) ? new Map() : new Map([[item.id, item]]);
      const next = new Map(prev);
      if (next.has(item.id)) {
        next.delete(item.id);
      } else {
        next.set(item.id, item);
      }
      return next;
    });
  }, [singleSelect]);

  const handleNextPage = () => {
    if (pageInfo?.nextCursor) {
      setCursor(pageInfo.nextCursor);
      setCursorHistory((prev) => [...prev, pageInfo.nextCursor!]);
    }
  };

  const handlePrevPage = () => {
    if (cursorHistory.length > 1) {
      const newHistory = [...cursorHistory];
      newHistory.pop(); // Remove current
      const prevCursor = newHistory[newHistory.length - 1];
      setCursorHistory(newHistory);
      setCursor(prevCursor);
    }
  };

  const handleResetFilters = () => {
    setSearch("");
    setDebouncedSearch("");
    setSelectedType(undefined);
    setSelectedCategoryId(undefined);
    setSelectedBrandId(undefined);
    setSelectedAttributeKey(undefined);
    setSelectedAttributeValue(undefined);
    setScope("cost");
    setCursor(undefined);
    setCursorHistory([undefined]);
  };

  const handleConfirmInsert = () => {
    const selectedList = Array.from(selectedItemsMap.values());
    if (selectedList.length > 0) {
      onSelectItems(selectedList);
      onClose();
    }
  };

  const activeFiltersCount =
    (selectedType ? 1 : 0) +
    (selectedCategoryId ? 1 : 0) +
    (selectedBrandId ? 1 : 0) +
    (selectedAttributeKey ? 1 : 0) +
    (search ? 1 : 0);

  const currentPageNumber = cursorHistory.length;

  const attributeFilters = attributeKeys.length > 0 && (
    <div className="shrink-0 border-b border-erp-border bg-erp-surface p-2 space-y-1.5">
      <div className="flex items-center justify-between">
        <span className="text-xs font-semibold text-erp-navy">
          {t("catalogFacets.attributesTitle")}
        </span>
        {selectedAttributeKey && (
          <button
            type="button"
            onClick={() => handleAttributeKeyChange("")}
            className="text-[11px] text-erp-text-muted hover:text-rose-600 underline"
          >
            {t("catalogFacets.allAttributeNames")}
          </button>
        )}
      </div>
      <div
        role="group"
        aria-label={t("catalogFacets.attributeName")}
        className="flex items-center gap-1 overflow-x-auto pb-1"
      >
        <Button
          type="button"
          size="sm"
          variant="ghost"
          aria-pressed={!selectedAttributeKey}
          onClick={() => handleAttributeKeyChange("")}
          className={cn(
            "h-8 px-2.5 text-xs font-normal border shrink-0",
            !selectedAttributeKey
              ? "border-erp-navy bg-erp-navy text-white font-semibold"
              : "border-erp-border bg-erp-surface hover:bg-erp-surface-subtle"
          )}
        >
          {t("catalogFacets.allAttributeNames")}
        </Button>
        {attributeKeys.map((key) => (
          <Button
            key={key}
            type="button"
            size="sm"
            variant="ghost"
            aria-pressed={selectedAttributeKey === key}
            onClick={() => handleAttributeKeyChange(key)}
            className={cn(
              "h-8 px-2.5 text-xs font-normal whitespace-nowrap border shrink-0",
              selectedAttributeKey === key
                ? "border-erp-navy bg-erp-navy text-white font-semibold"
                : "border-erp-border bg-erp-surface hover:bg-erp-surface-subtle"
            )}
          >
            {getAttributeLabel(key)}
          </Button>
        ))}
      </div>
      {selectedAttributeKey && (
        <fieldset className="border-t border-erp-border-subtle pt-1.5 space-y-1">
          <legend className="text-[11px] text-erp-text-muted">
            {getAttributeLabel(selectedAttributeKey)}
          </legend>
          <div className="flex flex-wrap gap-1 max-h-24 overflow-y-auto">
            <label className="relative cursor-pointer">
              <input
                type="radio"
                name="catalog-attribute-value"
                value=""
                checked={!selectedAttributeValue}
                onChange={() => handleAttributeValueChange("")}
                className="peer sr-only"
              />
              <span className="flex h-8 items-center justify-center border border-erp-border bg-erp-surface px-2.5 text-center text-xs text-erp-text-main peer-checked:border-erp-navy peer-checked:bg-erp-navy-light peer-checked:text-erp-navy peer-checked:font-semibold peer-focus-visible:outline-none peer-focus-visible:ring-2 peer-focus-visible:ring-erp-navy">
                {t("catalogFacets.allAttributeValues")}
              </span>
            </label>
            {attributeValues.map((facet) => {
              const value = facet.value ?? "";
              if (!value) return null;
              return (
                <label key={`${facet.key}:${value}`} className="relative cursor-pointer">
                  <input
                    type="radio"
                    name="catalog-attribute-value"
                    value={value}
                    checked={selectedAttributeValue === value}
                    onChange={() => handleAttributeValueChange(value)}
                    className="peer sr-only"
                  />
                  <span className="flex h-8 items-center justify-between gap-1 border border-erp-border bg-erp-surface px-2.5 text-xs text-erp-text-main peer-checked:border-erp-navy peer-checked:bg-erp-navy-light peer-checked:text-erp-navy peer-checked:font-semibold peer-focus-visible:outline-none peer-focus-visible:ring-2 peer-focus-visible:ring-erp-navy">
                    <span className="truncate" title={value}>{value}</span>
                    <span className="shrink-0 font-mono tabular-nums text-[10px] opacity-70">
                      ({facet.count ?? 0})
                    </span>
                  </span>
                </label>
              );
            })}
          </div>
        </fieldset>
      )}
    </div>
  );

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      size="full"
      title={t("catalogModalTitle")}
      description={t("catalogModalDesc")}
      className={cn(
        "transition-all duration-150",
        isFullscreen
          ? "!w-screen !h-screen !max-w-none !max-h-none !m-0 !inset-0 fixed"
          : "h-full sm:h-[92vh] sm:max-w-6xl w-full"
      )}
      headerActions={
        <button
          type="button"
          title={isFullscreen ? t("catalogFacets.exitFullscreen") : t("catalogFacets.fullscreen")}
          aria-label={isFullscreen ? t("catalogFacets.exitFullscreen") : t("catalogFacets.fullscreen")}
          onClick={() => setIsFullscreen((prev) => !prev)}
          className="shrink-0 rounded-none p-1.5 text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main focus-visible:outline-2 focus-visible:outline-erp-navy transition-colors"
        >
          {isFullscreen ? <IconMinimize size={18} /> : <IconMaximize size={18} />}
        </button>
      }
      contentClassName="p-3 sm:p-4 overflow-hidden flex flex-col flex-1 h-full min-h-0"
      footer={
        isMobile && showMobileFilters ? (
          <Button
            type="button"
            size="md"
            variant="primary"
            onClick={() => setShowMobileFilters(false)}
            className="w-full text-xs"
          >
            {t("catalogFacets.showResults")}
          </Button>
        ) : (
          <div className="flex flex-col sm:flex-row items-stretch sm:items-center justify-between w-full gap-2 sm:gap-3">
            <div className="flex items-center justify-between sm:justify-start gap-2 flex-wrap">
              {!singleSelect && (
                <Button
                  type="button"
                  variant="outline"
                  size="md"
                  onClick={handleToggleSelectAll}
                  disabled={items.length === 0 || isLoading}
                  className="text-xs"
                >
                  {isAllCurrentPageSelected ? t("deselectAll") : t("selectAll")}
                </Button>
              )}

              {!singleSelect && (
                <Button
                  type="button"
                  variant="ghost"
                  size="md"
                  onClick={() => setIsTrayOpen((prev) => !prev)}
                  disabled={selectedItemsMap.size === 0}
                  className={cn(
                    "text-xs gap-1.5 border border-erp-border",
                    isTrayOpen && "bg-erp-navy-light text-erp-navy border-erp-navy font-semibold"
                  )}
                >
                  <span>{t("catalogFacets.viewSelectedTray")}</span>
                  <span className="font-mono text-[10px] bg-erp-navy text-white px-1.5 py-0.2">
                    {selectedItemsMap.size}
                  </span>
                  {isTrayOpen ? <IconChevronDown size={14} /> : <IconChevronUp size={14} />}
                </Button>
              )}

              {selectedItemsMap.size > 0 && !singleSelect && (
                <button
                  type="button"
                  onClick={handleClearSelection}
                  className="text-[11px] text-erp-text-muted hover:text-rose-600 transition-colors px-1"
                >
                  {t("catalogFacets.clearSelection")}
                </button>
              )}
            </div>

            <div className="flex items-center justify-end gap-2">
              <Button
                type="button"
                variant="outline"
                size="md"
                onClick={onClose}
                className="text-xs flex-1 sm:flex-initial"
              >
                {tc("actions.cancel")}
              </Button>
              <Button
                type="button"
                variant="primary"
                size="md"
                disabled={selectedItemsMap.size === 0}
                onClick={handleConfirmInsert}
                className="text-xs flex-1 sm:flex-initial"
              >
                {t("insertSelectedCount", { count: selectedItemsMap.size })}
              </Button>
            </div>
          </div>
        )
      }
    >
      <div className="flex flex-col space-y-2.5 sm:space-y-3 flex-1 overflow-hidden h-full min-h-0">
        {/* Top Controls: Search, Filter Toggle, Scope */}
        <div className="flex flex-wrap items-center justify-between gap-2 pb-2.5 border-b border-erp-border shrink-0">
          <div className="flex-1 min-w-[200px]">
            <Input
              placeholder={t("searchPlaceholder")}
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              leftIcon={<IconSearch size={14} />}
              wrapperClassName="mb-0"
              className="text-xs !h-11 !min-h-[44px] py-1.5"
            />
          </div>

          <div className="flex items-center gap-2">
            <Button
              type="button"
              size="md"
              variant="outline"
              onClick={() => setShowMobileFilters((prev) => !prev)}
              className="md:hidden gap-1.5 text-xs"
              aria-expanded={showMobileFilters}
              aria-label={
                showMobileFilters
                  ? t("catalogFacets.hideFilters")
                  : t("catalogFacets.showFilters")
              }
            >
              {showMobileFilters ? <IconClose size={13} /> : <IconFilter size={13} />}
              <span>
                {showMobileFilters
                  ? t("catalogFacets.hideFilters")
                  : t("catalogFacets.showFilters")}
              </span>
              {activeFiltersCount > 0 && (
                <span className="px-1.5 py-0.5 bg-erp-navy text-white text-[10px] font-mono leading-none">
                  {activeFiltersCount}
                </span>
              )}
            </Button>

            <SegmentedControl<CatalogScope>
              value={scope}
              onChange={handleScopeChange}
              size="md"
              options={[
                { value: "all", label: t("catalogFacets.allScopes") },
                { value: "cost", label: t("catalogFacets.costScope") },
              ]}
              className="text-xs"
            />
          </div>
        </div>

        {/* Branch Guard */}
        {!branchId && (
          <div className="p-4 bg-amber-50 border border-amber-200 text-amber-800 text-xs">
            {t("noBranchSelected")}
          </div>
        )}

        {/* Content Area: Sidebar + Table */}
        {branchId && (
          <div className="flex flex-col md:flex-row gap-2.5 sm:gap-3 flex-1 overflow-hidden min-h-0">
            {/* Left Filter Sidebar */}
            <aside
              className={cn("md:w-64 md:flex md:flex-none flex-col border border-erp-border bg-erp-surface text-xs overflow-y-auto min-h-0", showMobileFilters ? "flex flex-1" : "hidden")}
            >
              <div className="sticky top-0 z-10 px-3 border-b border-erp-border bg-erp-surface flex min-h-11 items-center justify-between font-bold text-erp-navy">
                <span>{t("catalogFacets.filtersTitle")}</span>
                {activeFiltersCount > 0 && (
                  <Button type="button" size="sm" variant="ghost" onClick={handleResetFilters} className="px-2 text-[11px] font-normal text-erp-text-muted hover:text-rose-600">
                    {t("catalogFacets.clearFilters")}
                  </Button>
                )}
              </div>

              {/* Types Filter */}
              <div className="p-2.5 border-b border-erp-border-subtle">
                <div className="grid grid-cols-2 gap-1.5" role="group" aria-label={t("catalogFacets.costType")}>
                  {(facets?.itemTypes ?? []).map((facet) => {
                    const value = facet.value;
                    if (!value) return null;
                    const isTypeSelected = selectedType === value;
                    return (
                      <Button
                        key={value}
                        type="button"
                        size="sm"
                        variant={isTypeSelected ? "primary" : "ghost"}
                        aria-pressed={isTypeSelected}
                        onClick={() => handleTypeChange(value)}
                        className={cn(
                          "justify-between px-2 text-xs font-normal border border-erp-border h-9",
                          isTypeSelected
                            ? "bg-erp-navy text-white font-semibold border-erp-navy"
                            : "bg-erp-surface hover:bg-erp-surface-subtle text-erp-text-main"
                        )}
                      >
                        <span className="truncate">{itemTypeLabels[value] ?? value}</span>
                        <span
                          className={cn(
                            "font-mono tabular-nums text-[10px] ml-1 shrink-0 px-1",
                            isTypeSelected
                              ? "bg-white/20 text-white"
                              : "text-erp-text-muted"
                          )}
                        >
                          {facet.count ?? 0}
                        </span>
                      </Button>
                    );
                  })}
                </div>
              </div>

              {/* Category Filter */}
              <div className="p-2.5 border-b border-erp-border/60">
                <div className="font-semibold text-erp-text-secondary mb-1.5">
                  {t("catalogFacets.category")}
                </div>
                <div className="space-y-0.5 max-h-44 overflow-y-auto">
                  {(facets?.categories ?? []).map((cat) => {
                    const catId = cat.id || "";
                    if (!catId) return null;
                    const isChecked = selectedCategoryId === catId;
                    const catName = getLocalizedText(cat.name, locale);
                    return (
                      <label
                        key={catId}
                        className={cn(
                          "flex min-h-11 items-center justify-between gap-1 px-1.5 border border-transparent hover:bg-erp-surface-subtle cursor-pointer select-none transition-colors",
                          isChecked && "bg-erp-navy-light/60 border-erp-navy/30 text-erp-navy font-semibold"
                        )}
                      >
                        <div className="flex items-center gap-2 truncate">
                          <Avatar initial={catName} fileId={cat.imageFileId} alt={catName} size="sm" />
                          <Checkbox
                            checked={isChecked}
                            onChange={() => handleCategoryChange(catId)}
                            wrapperClassName="mb-0"
                            className="w-3.5 h-3.5"
                          />
                          <span className="truncate" title={catName}>
                            {catName}
                          </span>
                        </div>
                        <span className="text-[10px] text-erp-text-muted font-mono shrink-0 ml-1">
                          ({cat.count ?? 0})
                        </span>
                      </label>
                    );
                  })}
                </div>
              </div>

              {/* Brand Filter */}
              <div className="p-2.5">
                <div className="font-semibold text-erp-text-secondary mb-1.5">
                  {t("catalogFacets.brand")}
                </div>
                <div className="space-y-0.5 max-h-44 overflow-y-auto">
                  {(facets?.brands ?? []).map((brand) => {
                    const brandId = brand.id || "";
                    if (!brandId) return null;
                    const isChecked = selectedBrandId === brandId;
                    const brandName = getLocalizedText(brand.name, locale);
                    return (
                      <label
                        key={brandId}
                        className={cn(
                          "flex min-h-11 items-center justify-between gap-1 px-1.5 border border-transparent hover:bg-erp-surface-subtle cursor-pointer select-none transition-colors",
                          isChecked && "bg-erp-navy-light/60 border-erp-navy/30 text-erp-navy font-semibold"
                        )}
                      >
                        <div className="flex items-center gap-2 truncate">
                          <Avatar initial={brandName} fileId={brand.imageFileId} alt={brandName} size="sm" />
                          <Checkbox
                            checked={isChecked}
                            onChange={() => handleBrandChange(brandId)}
                            wrapperClassName="mb-0"
                            className="w-3.5 h-3.5"
                          />
                          <span className="truncate" title={brandName}>
                            {brandName}
                          </span>
                        </div>
                        <span className="text-[10px] text-erp-text-muted font-mono shrink-0 ml-1">
                          ({brand.count ?? 0})
                        </span>
                      </label>
                    );
                  })}
                </div>
              </div>
              {isMobile && attributeFilters}
            </aside>

            {/* Right Table */}
            <div className={cn("flex-1 flex-col overflow-hidden border border-erp-border bg-erp-surface min-h-0 min-w-0", isMobile && showMobileFilters ? "hidden" : "flex")}>
              {!isMobile && attributeFilters}
              <div className="flex-1 overflow-y-auto overflow-x-hidden min-h-0">
                {isLoading ? (
                  <div className="py-20 text-center text-xs text-erp-text-muted flex flex-col items-center justify-center gap-2">
                    <MonoSpinner size="sm" />
                    <span>{t("loadingCatalog")}</span>
                  </div>
                ) : isError ? (
                  <div className="py-20 text-center text-xs text-rose-600 flex flex-col items-center justify-center gap-3">
                    <span>{t("errorLoadingCatalog")}</span>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => void refetch()}
                      className="!rounded-none text-xs"
                    >
                      <IconRefresh size={13} className="mr-1.5" />
                      {t("retry")}
                    </Button>
                  </div>
                ) : items.length === 0 ? (
                  <div className="py-20 text-center text-xs text-erp-text-muted">
                    {t("noMatchingItems")}
                  </div>
                ) : (
                  <Table wrapperClassName="w-full border-none" className="text-xs table-fixed">
                    <TableHeader className="sticky top-0 bg-erp-surface-subtle text-erp-text-secondary border-b border-erp-border font-bold text-[11px] z-10">
                      <TableRow>
                        <TableHead scope="col" className="p-2 w-10 text-center border-r border-erp-border/60">
                          {!singleSelect ? (
                            <Checkbox
                              checked={isAllCurrentPageSelected}
                              onChange={handleToggleSelectAll}
                              aria-label={
                                isAllCurrentPageSelected
                                  ? t("catalogFacets.deselectAll")
                                  : t("catalogFacets.selectAll")
                              }
                              wrapperClassName="mb-0"
                              className="w-3.5 h-3.5"
                            />
                          ) : (
                            <span className="sr-only">{tc("actions.select")}</span>
                          )}
                        </TableHead>
                        <TableHead scope="col" className="p-2 w-14 text-center border-r border-erp-border/60">
                          <span className="sr-only">{t("catalogFacets.thumbnail")}</span>
                        </TableHead>
                        <TableHead scope="col" className="p-2 border-r border-erp-border/60">
                          {t("itemCodeAndDesc")}
                        </TableHead>
                        <TableHead scope="col" className="p-2 w-48 border-r border-erp-border/60 hidden sm:table-cell">
                          {t("catalogFacets.brand")} / {t("catalogFacets.category")}
                        </TableHead>
                        <TableHead scope="col" className="p-2 w-36 text-right">
                          {t("unitCost")} ({currency})
                        </TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody className="divide-y divide-erp-border/60">
                      {items.map((item) => {
                        const isSelected = selectedItemsMap.has(item.id);
                        const itemName = getLocalizedText(item.name, locale);
                        const itemAlt = getLocalizedText(
                          item.primaryImage?.altText,
                          locale,
                          itemName
                        );
                        const brandName = getLocalizedText(item.brand?.name, locale);
                        const categoryName = getLocalizedText(item.category.name, locale);

                        return (
                          <TableRow
                            key={item.id}
                            onClick={() => handleToggleItem(item)}
                            className={cn(
                              "cursor-pointer transition-colors",
                              isSelected
                                ? "bg-erp-navy/5 text-erp-navy font-medium border-l-4 border-l-erp-navy"
                                : "hover:bg-erp-surface-subtle/50"
                            )}
                          >
                            <TableCell
                              className="p-2 w-10 text-center border-r border-erp-border/60 align-top"
                              onClick={(e) => e.stopPropagation()}
                            >
                              <div className="flex items-center justify-center pt-1">
                                <Checkbox
                                  checked={isSelected}
                                  onChange={() => handleToggleItem(item)}
                                  aria-label={itemName}
                                  wrapperClassName="mb-0"
                                  className="w-3.5 h-3.5"
                                />
                              </div>
                            </TableCell>
                            <TableCell
                              className="p-1.5 w-14 text-center border-r border-erp-border/60 align-top"
                              onClick={(e) => e.stopPropagation()}
                            >
                              <PrivateThumbnail
                                fileId={item.primaryImage?.fileId}
                                altText={itemAlt}
                              />
                            </TableCell>
                            <TableCell className="p-2.5 border-r border-erp-border/60 align-top">
                              <div className="flex items-center gap-1.5 flex-wrap mb-0.5">
                                <span className="font-mono font-bold text-erp-navy text-xs">
                                  {item.code}
                                </span>
                                <span className="px-1.5 py-0.2 text-[9px] border border-erp-border uppercase font-mono text-erp-text-secondary bg-erp-surface-subtle">
                                  {item.itemType}
                                </span>
                              </div>
                              <div className="font-semibold text-erp-text-main text-xs leading-snug">
                                {itemName}
                              </div>
                              {item.description && (
                                <div className="text-[11px] text-erp-text-muted line-clamp-1 mt-0.5">
                                  {getLocalizedText(item.description, locale, "")}
                                </div>
                              )}
                              {Object.keys(item.attributes ?? {}).length > 0 && (
                                <div className="text-[10px] text-erp-text-muted mt-1.5 flex flex-wrap gap-1">
                                  {Object.entries(item.attributes ?? {}).map(([key, value]) => (
                                    <span
                                      key={key}
                                      className="inline-flex items-center gap-1 border border-erp-border bg-erp-surface-subtle px-1.5 py-0.5"
                                    >
                                      <span className="font-medium text-erp-text-secondary">{getAttributeLabel(key)}:</span>
                                      <span className="font-mono text-erp-text-main font-semibold">{value}</span>
                                    </span>
                                  ))}
                                </div>
                              )}
                            </TableCell>
                            <TableCell className="p-2.5 w-48 border-r border-erp-border/60 align-top hidden sm:table-cell">
                              <div className="font-semibold text-erp-text-main text-xs truncate">
                                {brandName || "-"}
                              </div>
                              <div className="text-[11px] text-erp-text-muted truncate mt-0.5">
                                {categoryName || "-"}
                              </div>
                            </TableCell>
                            <TableCell className="p-2.5 w-36 text-right align-top">
                              {item.resolvedCost ? (
                                <>
                                  <div className="font-mono font-bold text-erp-navy text-sm">
                                    {formatFinancialNumber(item.resolvedCost.amount)}
                                  </div>
                                  <div className="text-[10px] font-mono text-erp-text-muted mt-0.5">
                                    / {item.resolvedCost.unitCode}
                                  </div>
                                </>
                              ) : (
                                <span className="text-erp-text-muted font-mono">-</span>
                              )}
                            </TableCell>
                          </TableRow>
                        );
                      })}
                    </TableBody>
                  </Table>
                )}
              </div>

              {/* Pagination Toolbar */}
              {items.length > 0 && (
                <div className="flex items-center justify-between px-3 py-2 border-t border-erp-border bg-erp-surface text-xs select-none">
                  <div className="text-erp-text-muted font-mono text-[11px]">
                    {t("matchingCount", { count: items.length })}
                  </div>

                  <div className="flex items-center gap-2">
                    <span className="text-[11px] font-mono text-erp-text-secondary px-1">
                      {t("catalogFacets.pageOf", {
                        current: currentPageNumber,
                        total: pageInfo?.hasNextPage ? `${currentPageNumber}+` : currentPageNumber,
                      })}
                    </span>

                    <button
                      type="button"
                      onClick={handlePrevPage}
                      disabled={cursorHistory.length <= 1}
                      className="p-1 border border-erp-border text-erp-text-main hover:bg-erp-surface-subtle disabled:opacity-40 disabled:cursor-not-allowed rounded-none transition-colors"
                      title={t("catalogFacets.previousPage")}
                      aria-label={t("catalogFacets.previousPage")}
                    >
                      <IconChevronLeft size={14} />
                    </button>

                    <button
                      type="button"
                      onClick={handleNextPage}
                      disabled={!pageInfo?.hasNextPage}
                      className="p-1 border border-erp-border text-erp-text-main hover:bg-erp-surface-subtle disabled:opacity-40 disabled:cursor-not-allowed rounded-none transition-colors"
                      title={t("catalogFacets.nextPage")}
                      aria-label={t("catalogFacets.nextPage")}
                    >
                      <IconChevronRight size={14} />
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
        )}

        {/* Selected Items Quick-Review Tray */}
        {isTrayOpen && selectedItemsMap.size > 0 && !singleSelect && (
          <div className="shrink-0 border-t-2 border-erp-navy bg-erp-surface">
            <div className="flex items-center justify-between px-3 py-1.5 bg-erp-surface-subtle border-b border-erp-border text-xs">
              <div className="flex items-center gap-2">
                <span className="font-bold text-erp-navy">
                  {t("catalogFacets.selectedItemsTitle")}
                </span>
                <span className="font-mono text-[10px] bg-erp-navy text-white px-1.5 py-0.2">
                  {t("catalogFacets.selectedItemsCount", { count: selectedItemsMap.size })}
                </span>
              </div>
              <button
                type="button"
                onClick={() => setIsTrayOpen(false)}
                className="text-[11px] text-erp-text-muted hover:text-erp-navy flex items-center gap-1 font-medium"
              >
                <span>{t("catalogFacets.hideSelectedTray")}</span>
                <IconChevronDown size={12} />
              </button>
            </div>
            <div className="max-h-36 overflow-y-auto p-2 space-y-1 divide-y divide-erp-border/60 text-xs">
              {Array.from(selectedItemsMap.values()).map((selectedItem) => {
                const name = getLocalizedText(selectedItem.name, locale);
                return (
                  <div
                    key={selectedItem.id}
                    className="flex items-center justify-between pt-1.5 pb-1 gap-2"
                  >
                    <div className="flex items-center gap-2 truncate min-w-0">
                      <span className="font-mono font-bold text-erp-navy text-xs shrink-0">
                        {selectedItem.code}
                      </span>
                      <span className="truncate text-erp-text-main text-xs" title={name}>
                        {name}
                      </span>
                    </div>
                    <div className="flex items-center gap-3 shrink-0">
                      {selectedItem.resolvedCost && (
                        <span className="font-mono text-[11px] font-bold text-erp-navy">
                          {formatFinancialNumber(selectedItem.resolvedCost.amount)} {currency} / {selectedItem.resolvedCost.unitCode}
                        </span>
                      )}
                      <button
                        type="button"
                        onClick={() => handleRemoveSelectedItem(selectedItem.id)}
                        className="text-[11px] text-rose-600 hover:bg-rose-50 border border-rose-200 px-1.5 py-0.5 font-bold transition-colors"
                        title={t("catalogFacets.removeItem")}
                      >
                        {t("catalogFacets.removeItem")} ✕
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </Modal>
  );
}
