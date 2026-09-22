"use client";

import React, { useState, useEffect } from "react";
import { useTranslations, useLocale } from "next-intl";
import { Checkbox } from "@/components/ui/Checkbox";
import { IconEye, IconChevronLeft, IconChevronRight } from "@/components/common/Icons";
import { useAuthenticatedFileUrl } from "@/hooks/useAuthenticatedFileUrl";
import { formatFinancialNumber } from "../../utils/estimate-formatters";
import type { CatalogItem } from "../../constants/estimate-catalog-items";
import type { BrandOption, SupplierOption, SubCategoryOption, CategoryOption } from "../../hooks/useCatalogFilter";

interface CatalogItemThumbnailProps {
  primaryImageFileId?: string;
  imageUrl?: string;
  name: string;
  onClick?: () => void;
  viewDetailLabel: string;
}

function CatalogItemThumbnail({
  primaryImageFileId,
  imageUrl,
  name,
  onClick,
  viewDetailLabel,
}: CatalogItemThumbnailProps) {
  const { objectUrl, isLoading } = useAuthenticatedFileUrl(primaryImageFileId || "");
  const [hasError, setHasError] = useState(false);

  const displayUrl = objectUrl || imageUrl;

  return (
    <div
      onClick={onClick}
      className="w-9 h-9 mx-auto bg-erp-surface border border-erp-border flex items-center justify-center overflow-hidden cursor-pointer hover:border-erp-navy transition-colors group"
      title={viewDetailLabel}
    >
      {isLoading ? (
        <div className="w-full h-full bg-erp-surface-subtle animate-pulse" />
      ) : displayUrl && !hasError ? (
        <img
          src={displayUrl}
          alt={name}
          onError={() => setHasError(true)}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform"
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

export interface CatalogItemsTableProps {
  items: CatalogItem[];
  selectedItemIds: string[];
  toggleSelectItem: (id: string) => void;
  currency: string;
  search: string;
  selectedCategory: string;
  selectedSubCategory: string;
  selectedThicknesses: string[];
  selectedBrands: string[];
  selectedSupplier: string;
  selectedAttributes: Record<string, string>;
  availableCategories?: CategoryOption[];
  availableBrands: BrandOption[];
  availableSuppliers: SupplierOption[];
  availableSubCategories: SubCategoryOption[];
  onOpenDetail?: (item: CatalogItem) => void;
  onRemoveFilter: (type: string, val?: string) => void;
  onClearFilters: () => void;
  onToggleSelectAll?: () => void;
  isAllSelected?: boolean;
  isCatalogLoading?: boolean;
}

export function CatalogItemsTable({
  items,
  selectedItemIds,
  toggleSelectItem,
  currency,
  search,
  selectedCategory,
  selectedSubCategory,
  selectedThicknesses,
  selectedBrands,
  selectedSupplier,
  selectedAttributes,
  availableCategories,
  availableBrands,
  availableSuppliers,
  availableSubCategories,
  onOpenDetail,
  onRemoveFilter,
  onClearFilters,
  onToggleSelectAll,
  isAllSelected = false,
  isCatalogLoading = false,
}: CatalogItemsTableProps) {
  const t = useTranslations("estimates");
  const locale = useLocale() as "th" | "en";

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState<number>(20);

  // Reset page when items or filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [
    items.length,
    search,
    selectedCategory,
    selectedSubCategory,
    selectedThicknesses.length,
    selectedBrands.length,
    selectedSupplier,
  ]);

  const totalItems = items.length;
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));
  const validPage = Math.min(currentPage, totalPages);
  const startIndex = (validPage - 1) * pageSize;
  const endIndex = Math.min(startIndex + pageSize, totalItems);
  const paginatedItems = items.slice(startIndex, endIndex);

  // Check if all visible items on current page are selected
  const isCurrentPageAllSelected =
    paginatedItems.length > 0 &&
    paginatedItems.every((item) => selectedItemIds.includes(item.id));

  const handleToggleCurrentPageSelectAll = () => {
    if (onToggleSelectAll) {
      onToggleSelectAll();
    } else {
      paginatedItems.forEach((item) => {
        if (!selectedItemIds.includes(item.id)) {
          toggleSelectItem(item.id);
        }
      });
    }
  };

  const activeAttributeEntries = Object.entries(selectedAttributes).filter(
    ([, val]) => val && val !== "all"
  );

  const hasActiveFilters =
    Boolean(search) ||
    selectedCategory !== "all" ||
    selectedSubCategory !== "all" ||
    selectedThicknesses.length > 0 ||
    selectedBrands.length > 0 ||
    selectedSupplier !== "all" ||
    activeAttributeEntries.length > 0;

  // Lookup helper for category localized name
  const getCategoryDisplayName = (categoryId: string) => {
    const cat = availableCategories?.find((c) => c.id === categoryId);
    if (cat) return cat.name[locale] || cat.name.th;
    return t(`catalogFacets.categories.${categoryId}`);
  };

  // Lookup helper for brand localized name
  const getBrandDisplayName = (brandId: string) => {
    const br = availableBrands.find((b) => b.id === brandId);
    if (br) return br.name[locale] || br.name.th;
    return brandId;
  };

  // Lookup helper for supplier localized name
  const getSupplierDisplayName = (supplierId: string) => {
    const sup = availableSuppliers.find((s) => s.id === supplierId);
    if (sup) return sup.name[locale] || sup.name.th;
    return supplierId;
  };

  // Lookup helper for subcategory localized name
  const getSubCategoryDisplayName = (subCatId: string) => {
    const sub = availableSubCategories.find((s) => s.id === subCatId);
    if (sub) return sub.name[locale] || sub.name.th;
    return subCatId;
  };

  return (
    <div className="flex-1 flex flex-col overflow-hidden border border-erp-border bg-erp-surface min-h-0 min-w-0">
      {/* Active Tags Bar */}
      <div className="flex flex-wrap items-center gap-1.5 p-2 border-b border-erp-border bg-erp-surface-subtle/30 text-[11px] shrink-0">
        <span className="text-erp-text-muted font-medium mr-1">
          {t("catalogFacets.selectedFilters")}
        </span>

        {search && (
          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20 font-mono">
            <span>&quot;{search}&quot;</span>
            <button
              type="button"
              onClick={() => onRemoveFilter("search")}
              aria-label="Remove search filter"
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        )}

        {selectedCategory !== "all" && (
          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20">
            <span>{getCategoryDisplayName(selectedCategory)}</span>
            <button
              type="button"
              onClick={() => onRemoveFilter("category")}
              aria-label="Remove category filter"
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        )}

        {selectedSubCategory !== "all" && (
          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20">
            <span>{getSubCategoryDisplayName(selectedSubCategory)}</span>
            <button
              type="button"
              onClick={() => onRemoveFilter("subCategory")}
              aria-label="Remove subcategory filter"
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        )}

        {selectedThicknesses.map((th) => (
          <span
            key={th}
            className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20 font-mono"
          >
            <span>{th}</span>
            <button
              type="button"
              onClick={() => onRemoveFilter("thickness", th)}
              aria-label={`Remove thickness ${th}`}
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        ))}

        {selectedBrands.map((brId) => (
          <span
            key={brId}
            className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20"
          >
            <span>{getBrandDisplayName(brId)}</span>
            <button
              type="button"
              onClick={() => onRemoveFilter("brand", brId)}
              aria-label={`Remove brand ${brId}`}
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        ))}

        {selectedSupplier !== "all" && (
          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20">
            <span>{getSupplierDisplayName(selectedSupplier)}</span>
            <button
              type="button"
              onClick={() => onRemoveFilter("supplier")}
              aria-label="Remove supplier filter"
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        )}

        {activeAttributeEntries.map(([attrKey, attrVal]) => (
          <span
            key={attrKey}
            className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-erp-navy/10 text-erp-navy border border-erp-navy/20 font-mono"
          >
            <span>
              {attrKey}: {attrVal}
            </span>
            <button
              type="button"
              onClick={() => onRemoveFilter(`attr_${attrKey}`, attrVal)}
              aria-label={`Remove attribute ${attrKey}`}
              className="text-erp-text-muted hover:text-erp-danger font-bold ml-0.5"
            >
              ×
            </button>
          </span>
        ))}

        {!hasActiveFilters && (
          <span className="text-erp-text-muted">
            {t("catalogFacets.noFiltersActive")}
          </span>
        )}

        {hasActiveFilters && (
          <button
            type="button"
            onClick={onClearFilters}
            className="text-[10px] text-erp-navy hover:underline font-semibold ml-auto"
          >
            {t("catalogFacets.clearFilters")}
          </button>
        )}
      </div>

      {/* Table Area (Zero horizontal scroll with compact intelligent column grouping) */}
      <div className="flex-1 overflow-y-auto overflow-x-hidden min-h-0 overscroll-contain">
        {isCatalogLoading ? (
          <div className="py-16 text-center text-xs text-erp-text-muted flex flex-col items-center justify-center gap-2">
            <div className="w-5 h-5 border-2 border-erp-navy border-t-transparent animate-spin" />
            <span>{t("loadingCatalog")}</span>
          </div>
        ) : items.length === 0 ? (
          <div className="py-16 text-center text-xs text-erp-text-muted">
            {t("noMatchingItems")}
          </div>
        ) : (
          <table className="w-full text-left border-collapse text-xs table-fixed">
            <thead className="sticky top-0 bg-erp-surface-subtle text-erp-text-secondary border-b border-erp-border font-bold text-[11px] z-10">
              <tr>
                {/* Col 1: Select All / Deselect All Checkbox */}
                <th className="p-2 w-8 text-center">
                  <div className="flex items-center justify-center">
                    <Checkbox
                      checked={isCurrentPageAllSelected || isAllSelected}
                      onChange={handleToggleCurrentPageSelectAll}
                      aria-label={
                        isCurrentPageAllSelected || isAllSelected
                          ? t("catalogFacets.deselectAll")
                          : t("catalogFacets.selectAll")
                      }
                      title={
                        isCurrentPageAllSelected || isAllSelected
                          ? t("catalogFacets.deselectAll")
                          : t("catalogFacets.selectAll")
                      }
                      wrapperClassName="mb-0"
                      className="w-3.5 h-3.5"
                    />
                  </div>
                </th>
                {/* Col 2: Image */}
                <th className="p-2 w-11 text-center border-r border-erp-border/60">
                  <span className="sr-only">Image</span>
                </th>
                {/* Col 3: Item Code & Description */}
                <th className="p-2 border-r border-erp-border/60">
                  {t("itemCodeAndDesc")}
                </th>
                {/* Col 4: Category, Brand & Supplier */}
                <th className="p-2 w-48 border-r border-erp-border/60 hidden sm:table-cell">
                  {t("catalogFacets.brand")} / {t("catalogFacets.supplier")}
                </th>
                {/* Col 5: Unit & Cost */}
                <th className="p-2 w-28 text-right border-r border-erp-border/60">
                  {t("unitCost")} ({currency})
                </th>
                {/* Col 6: Quick Action (Detail Drawer) */}
                <th className="p-2 w-9 text-center">
                  <span className="sr-only">Actions</span>
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-erp-border/60">
              {paginatedItems.map((item) => {
                const isSelected = selectedItemIds.includes(item.id);
                const brandName = item.brand.name[locale] || item.brand.name.th;
                const supplierName = item.supplier
                  ? item.supplier.name[locale] || item.supplier.name.th
                  : "-";

                // Format attribute badges for quick scan
                const attrBadges = item.attributes
                  ? Object.entries(item.attributes).slice(0, 3)
                  : [];

                return (
                  <tr
                    key={item.id}
                    onClick={() => toggleSelectItem(item.id)}
                    className={`cursor-pointer transition-colors ${
                      isSelected
                        ? "bg-erp-navy/10 text-erp-navy font-medium"
                        : "hover:bg-erp-surface-subtle/50"
                    }`}
                  >
                    {/* Col 1: Checkbox */}
                    <td className="p-2 text-center align-top" onClick={(e) => e.stopPropagation()}>
                      <div className="flex items-center justify-center pt-0.5">
                        <Checkbox
                          checked={isSelected}
                          onChange={() => toggleSelectItem(item.id)}
                          aria-label={item.name[locale] || item.name.th}
                          wrapperClassName="mb-0"
                          className="w-3.5 h-3.5"
                        />
                      </div>
                    </td>

                    {/* Col 2: Image Thumbnail with Fallback */}
                    <td className="p-1.5 text-center border-r border-erp-border/60 align-top" onClick={(e) => e.stopPropagation()}>
                      <CatalogItemThumbnail
                        primaryImageFileId={item.primaryImageFileId}
                        imageUrl={item.imageUrl}
                        name={item.name[locale] || item.name.th}
                        onClick={() => onOpenDetail?.(item)}
                        viewDetailLabel={t("catalogDetail.viewDetail")}
                      />
                    </td>

                    {/* Col 3: Code + Name + Badges + Subcategory/Aliases */}
                    <td className="p-2 border-r border-erp-border/60 align-top">
                      <div className="flex items-center gap-1.5 flex-wrap mb-0.5">
                        <span className="font-mono font-bold text-erp-navy text-xs">
                          {item.code}
                        </span>
                        <span className="px-1 py-0.2 text-[9px] border border-erp-border uppercase font-mono text-erp-text-secondary">
                          {item.itemType}
                        </span>
                      </div>
                      <div className="font-semibold text-erp-text-main text-xs leading-tight">
                        {item.name[locale] || item.name.th}
                      </div>
                      <div className="text-[11px] font-mono text-erp-text-muted truncate">
                        {item.name[locale === "th" ? "en" : "th"]}
                      </div>

                      {/* Aliases & Attribute Badges */}
                      {(attrBadges.length > 0 || (item.aliases && item.aliases.length > 0)) && (
                        <div className="flex flex-wrap gap-1 mt-1">
                          {item.aliases && item.aliases.length > 0 && (
                            <span className="px-1 py-0.2 text-[10px] bg-erp-surface-subtle text-erp-text-muted border border-erp-border/60 italic truncate max-w-[200px]">
                              &quot;{item.aliases.map((a) => a[locale] || a.th).join(", ")}&quot;
                            </span>
                          )}
                          {attrBadges.map(([k, v]) => (
                            <span
                              key={k}
                              className="px-1 py-0.2 text-[10px] bg-erp-surface text-erp-navy border border-erp-border font-mono whitespace-nowrap"
                            >
                              {k}: {v}
                            </span>
                          ))}
                        </div>
                      )}
                    </td>

                    {/* Col 4: Brand & Supplier */}
                    <td className="p-2 border-r border-erp-border/60 align-top hidden sm:table-cell">
                      <div className="font-medium text-erp-text-main text-xs truncate">
                        {brandName}
                      </div>
                      <div className="text-[11px] text-erp-text-muted truncate mt-0.5" title={supplierName}>
                        {supplierName}
                      </div>
                    </td>

                    {/* Col 5: Unit Code & Cost */}
                    <td className="p-2 text-right border-r border-erp-border/60 align-top">
                      <div className="font-mono font-bold text-erp-navy text-xs">
                        {formatFinancialNumber(item.pricing.defaultUnitCost)}
                      </div>
                      <div className="text-[10px] font-mono text-erp-text-muted">
                        / {item.pricing.baseUnitCode}
                      </div>
                    </td>

                    {/* Col 6: View Details Button */}
                    <td className="p-1.5 text-center align-top" onClick={(e) => e.stopPropagation()}>
                      <button
                        type="button"
                        onClick={() => onOpenDetail?.(item)}
                        className="p-1 text-erp-text-muted hover:text-erp-navy hover:bg-erp-navy/10 rounded-none transition-colors cursor-pointer"
                        title={t("catalogDetail.viewDetail")}
                        aria-label={t("catalogDetail.viewDetail")}
                      >
                        <IconEye size={14} />
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>

      {/* Pagination Footer Toolbar */}
      {totalItems > 0 && (
        <div className="flex items-center justify-between px-3 py-2 border-t border-erp-border bg-erp-surface text-xs select-none">
          <div className="text-erp-text-muted font-mono text-[11px]">
            {t("catalogFacets.showingCount", {
              from: totalItems === 0 ? 0 : startIndex + 1,
              to: endIndex,
              total: totalItems,
            })}
          </div>

          <div className="flex items-center gap-3">
            {/* Page Size Selector */}
            <div className="flex items-center gap-1.5 text-erp-text-secondary text-[11px]">
              <span className="hidden sm:inline">{t("catalogFacets.perPage")}:</span>
              <select
                value={pageSize}
                onChange={(e) => {
                  setPageSize(Number(e.target.value));
                  setCurrentPage(1);
                }}
                className="bg-erp-surface border border-erp-border text-erp-text-main px-1.5 py-0.5 text-xs rounded-none font-mono focus:outline-none focus:border-erp-navy"
                aria-label={t("catalogFacets.perPage")}
              >
                <option value={20}>20</option>
                <option value={50}>50</option>
                <option value={100}>100</option>
              </select>
            </div>

            {/* Page Indicator & Prev/Next Buttons */}
            <div className="flex items-center gap-1">
              <span className="text-[11px] font-mono text-erp-text-secondary px-1">
                {t("catalogFacets.pageOf", {
                  current: validPage,
                  total: totalPages,
                })}
              </span>

              <button
                type="button"
                onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                disabled={validPage <= 1}
                className="p-1 border border-erp-border text-erp-text-main hover:bg-erp-surface-subtle disabled:opacity-40 disabled:cursor-not-allowed rounded-none transition-colors"
                title={t("catalogFacets.previousPage")}
                aria-label={t("catalogFacets.previousPage")}
              >
                <IconChevronLeft size={14} />
              </button>

              <button
                type="button"
                onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                disabled={validPage >= totalPages}
                className="p-1 border border-erp-border text-erp-text-main hover:bg-erp-surface-subtle disabled:opacity-40 disabled:cursor-not-allowed rounded-none transition-colors"
                title={t("catalogFacets.nextPage")}
                aria-label={t("catalogFacets.nextPage")}
              >
                <IconChevronRight size={14} />
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
