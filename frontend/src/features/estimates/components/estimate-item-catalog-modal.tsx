"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { SegmentedControl } from "@/components/ui/SegmentedControl";
import { IconSearch, IconFilter, IconClose } from "@/components/common/Icons";
import type { CatalogItem } from "../constants/estimate-catalog-items";
import { useCatalogFilter, type CatalogScope } from "../hooks/useCatalogFilter";
import { CatalogFilterSidebar } from "./catalog/catalog-filter-sidebar";
import { CatalogItemsTable } from "./catalog/catalog-items-table";
import { CatalogItemDetailDrawer } from "@/components/catalog/CatalogItemDetailDrawer";

export interface EstimateItemCatalogModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectItems: (items: CatalogItem[]) => void;
  currency: string;
  branchId?: string;
}

const SCOPES: readonly { id: CatalogScope; labelKey: string }[] = [
  { id: "all", labelKey: "allScopes" },
  { id: "cost", labelKey: "costScope" },
  { id: "sell", labelKey: "sellScope" },
];

export function EstimateItemCatalogModal({
  isOpen,
  onClose,
  onSelectItems,
  currency,
  branchId,
}: EstimateItemCatalogModalProps) {
  const t = useTranslations("estimates");
  const tc = useTranslations("common");

  const {
    search,
    setSearch,
    scope,
    setScope,
    selectedTypes,
    toggleType,
    selectedCategory,
    setSelectedCategory,
    selectedSubCategory,
    setSelectedSubCategory,
    selectedThicknesses,
    toggleThickness,
    selectedBrands,
    toggleBrand,
    selectedSupplier,
    setSelectedSupplier,
    selectedAttributes,
    setAttributeValue,
    selectedItemIds,
    toggleSelectItem,
    handleResetFilters,
    removeFilter,
    filteredItems,
    isAllVisibleSelected,
    handleToggleSelectAll,
    handleConfirmInsert,
    availableCategories,
    availableBrands,
    availableSuppliers,
    availableSubCategories,
    availableThicknesses,
    availableAttributes,
    isCatalogLoading,
  } = useCatalogFilter({
    branchId,
    isOpen,
    onSelectItems,
    onClose,
  });

  const [detailItem, setDetailItem] = React.useState<CatalogItem | null>(null);
  const [showMobileFilters, setShowMobileFilters] = React.useState(false);

  // Active filter count for mobile filter badge
  const totalActiveFilterCount =
    (selectedTypes.length < 4 ? selectedTypes.length : 0) +
    (selectedCategory !== "all" ? 1 : 0) +
    (selectedSubCategory !== "all" ? 1 : 0) +
    selectedThicknesses.length +
    selectedBrands.length +
    (selectedSupplier !== "all" ? 1 : 0) +
    Object.values(selectedAttributes).filter((v) => v && v !== "all").length;

  return (
    <>
      <Modal
        isOpen={isOpen}
        onClose={onClose}
        size="full"
        title={t("catalogModalTitle")}
        description={t("catalogModalDesc")}
        className="h-full sm:h-[92vh] sm:max-w-6xl w-full"
        contentClassName="p-3 sm:p-4 overflow-hidden flex flex-col flex-1 h-full min-h-0"
        footer={
          <div className="flex flex-col sm:flex-row items-stretch sm:items-center justify-between w-full gap-2 sm:gap-3">
            <div className="flex items-center justify-between sm:justify-start gap-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleToggleSelectAll}
                disabled={filteredItems.length === 0}
                className="!rounded-none text-xs h-8"
              >
                {isAllVisibleSelected ? t("deselectAll") : t("selectAll")}
              </Button>
              <span className="text-xs text-erp-text-muted font-mono">
                {t("matchingCount", { count: filteredItems.length })}
              </span>
            </div>

            <div className="flex items-center justify-end gap-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={onClose}
                className="!rounded-none text-xs h-8 flex-1 sm:flex-initial"
              >
                {tc("actions.cancel")}
              </Button>
              <Button
                type="button"
                variant="primary"
                size="sm"
                disabled={selectedItemIds.length === 0}
                onClick={handleConfirmInsert}
                className="!rounded-none text-xs h-8 bg-erp-navy hover:bg-erp-navy-hover text-white flex-1 sm:flex-initial"
              >
                {t("insertSelectedCount", { count: selectedItemIds.length })}
              </Button>
            </div>
          </div>
        }
      >
        <div className="flex flex-col space-y-2.5 sm:space-y-3 flex-1 overflow-hidden h-full min-h-0">
          {/* 1. Top Search Bar, Mobile Filter Toggle & Scope Toggle */}
          <div className="flex flex-wrap items-center justify-between gap-2 pb-2.5 border-b border-erp-border shrink-0">
            {/* Reusable Input with IconSearch */}
            <div className="flex-1 min-w-[200px]">
              <Input
                placeholder={t("searchPlaceholder")}
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                leftIcon={<IconSearch size={14} />}
                wrapperClassName="mb-0"
                className="text-xs !h-9 !min-h-[36px] py-1.5"
              />
            </div>

            <div className="flex items-center gap-2">
              {/* Mobile Filter Toggle Button */}
              <button
                type="button"
                onClick={() => setShowMobileFilters((prev) => !prev)}
                className="md:hidden flex items-center gap-1.5 px-2.5 h-9 min-h-[36px] text-xs border border-erp-border bg-erp-surface hover:bg-erp-surface-subtle font-medium text-erp-text-main rounded-none"
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
                {totalActiveFilterCount > 0 && (
                  <span className="px-1.5 py-0.2 bg-erp-navy text-white text-[10px] font-mono leading-none">
                    {totalActiveFilterCount}
                  </span>
                )}
              </button>

              {/* Scope Toggle (All / Cost / Sellable) */}
              <SegmentedControl
                value={scope}
                onChange={setScope}
                size="sm"
                options={SCOPES.map((sc) => ({
                  value: sc.id,
                  label: t(`catalogFacets.${sc.labelKey}`),
                }))}
                className="h-9 min-h-[36px] text-xs"
              />
            </div>
          </div>

          {/* 2. Main Content Split: Left Sidebar Filters + Right Table */}
          <div className="flex flex-col md:flex-row gap-2.5 sm:gap-3 flex-1 overflow-hidden min-h-0">
            {/* Left Facets Sidebar (Always visible on md+, conditionally shown on mobile) */}
            <CatalogFilterSidebar
              selectedTypes={selectedTypes}
              toggleType={toggleType}
              selectedCategory={selectedCategory}
              setSelectedCategory={setSelectedCategory}
              selectedSubCategory={selectedSubCategory}
              setSelectedSubCategory={setSelectedSubCategory}
              selectedThicknesses={selectedThicknesses}
              toggleThickness={toggleThickness}
              selectedBrands={selectedBrands}
              toggleBrand={toggleBrand}
              selectedSupplier={selectedSupplier}
              setSelectedSupplier={setSelectedSupplier}
              selectedAttributes={selectedAttributes}
              setAttributeValue={setAttributeValue}
              availableCategories={availableCategories}
              availableBrands={availableBrands}
              availableSuppliers={availableSuppliers}
              availableSubCategories={availableSubCategories}
              availableThicknesses={availableThicknesses}
              availableAttributes={availableAttributes}
              onResetFilters={handleResetFilters}
              className={`md:flex ${
                showMobileFilters
                  ? "flex max-h-56 sm:max-h-72 border-b md:border-b-0"
                  : "hidden"
              }`}
            />

            {/* Right Results Table */}
            <CatalogItemsTable
              items={filteredItems}
              selectedItemIds={selectedItemIds}
              toggleSelectItem={toggleSelectItem}
              currency={currency}
              search={search}
              selectedCategory={selectedCategory}
              selectedSubCategory={selectedSubCategory}
              selectedThicknesses={selectedThicknesses}
              selectedBrands={selectedBrands}
              selectedSupplier={selectedSupplier}
              selectedAttributes={selectedAttributes}
              availableCategories={availableCategories}
              availableBrands={availableBrands}
              availableSuppliers={availableSuppliers}
              availableSubCategories={availableSubCategories}
              onOpenDetail={(item) => setDetailItem(item)}
              onRemoveFilter={removeFilter}
              onClearFilters={handleResetFilters}
              onToggleSelectAll={handleToggleSelectAll}
              isAllSelected={isAllVisibleSelected}
              isCatalogLoading={isCatalogLoading}
            />
          </div>
        </div>
      </Modal>

      {/* Item Detail Drawer */}
      <CatalogItemDetailDrawer
        item={detailItem}
        isOpen={Boolean(detailItem)}
        onClose={() => setDetailItem(null)}
        currency={currency}
      />
    </>
  );
}
