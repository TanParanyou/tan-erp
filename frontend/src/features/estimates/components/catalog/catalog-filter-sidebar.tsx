"use client";

import React, { useState, useMemo, useRef, useEffect } from "react";
import { useTranslations, useLocale } from "next-intl";
import { Checkbox } from "@/components/ui/Checkbox";
import { Collapsible } from "@/components/ui/Collapsible";
import { IconSearch, IconClose } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import type { ItemCategoryType, CostNatureType } from "../../constants/estimate-catalog-items";
import type {
  BrandOption,
  SupplierOption,
  SubCategoryOption,
  CategoryOption,
} from "../../hooks/useCatalogFilter";

const COST_TYPES: readonly CostNatureType[] = [
  "material",
  "labor",
  "subcontract",
  "equipment",
];

const CATEGORIES: readonly { id: ItemCategoryType | "all"; labelKey: string }[] = [
  { id: "all", labelKey: "all" },
  { id: "wood", labelKey: "wood" },
  { id: "fitting", labelKey: "fitting" },
  { id: "surface", labelKey: "surface" },
  { id: "tile", labelKey: "tile" },
  { id: "stone", labelKey: "stone" },
  { id: "steel", labelKey: "steel" },
  { id: "electric", labelKey: "electric" },
  { id: "labor_service", labelKey: "labor_service" },
];

export interface CatalogFilterSidebarProps {
  selectedTypes: string[];
  toggleType: (type: string) => void;
  selectedCategory: string;
  setSelectedCategory: (cat: string) => void;
  selectedSubCategory: string;
  setSelectedSubCategory: (subCat: string) => void;
  selectedThicknesses: string[];
  toggleThickness: (thick: string) => void;
  selectedBrands: string[];
  toggleBrand: (brandId: string) => void;
  selectedSupplier: string;
  setSelectedSupplier: (sup: string) => void;
  selectedAttributes: Record<string, string>;
  setAttributeValue: (key: string, value: string) => void;
  availableCategories?: CategoryOption[];
  availableBrands: BrandOption[];
  availableSuppliers: SupplierOption[];
  availableSubCategories: SubCategoryOption[];
  availableThicknesses: string[];
  availableAttributes: Record<string, string[]>;
  onResetFilters: () => void;
  className?: string;
}

export function CatalogFilterSidebar({
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
  availableCategories,
  availableBrands,
  availableSuppliers,
  availableSubCategories,
  availableThicknesses,
  availableAttributes,
  onResetFilters,
  className,
}: CatalogFilterSidebarProps) {
  const t = useTranslations("estimates");
  const locale = useLocale() as "th" | "en";

  // Category search state
  const [showCategorySearch, setShowCategorySearch] = useState(false);
  const [categorySearch, setCategorySearch] = useState("");
  const categorySearchInputRef = useRef<HTMLInputElement>(null);

  // Brand search state
  const [showBrandSearch, setShowBrandSearch] = useState(false);
  const [brandSearch, setBrandSearch] = useState("");
  const brandSearchInputRef = useRef<HTMLInputElement>(null);

  // SubCategory autocomplete state
  const [subCategorySearch, setSubCategorySearch] = useState("");
  const [isSubCategoryOpen, setIsSubCategoryOpen] = useState(false);
  const subCategoryRef = useRef<HTMLDivElement>(null);

  // Collapsible section state (default: all expanded)
  const [collapsedSections, setCollapsedSections] = useState<Record<string, boolean>>({});

  const toggleSection = (sectionKey: string) => {
    setCollapsedSections((prev) => ({
      ...prev,
      [sectionKey]: !prev[sectionKey],
    }));
  };

  // Supplier autocomplete state
  const [supplierSearch, setSupplierSearch] = useState("");
  const [isSupplierOpen, setIsSupplierOpen] = useState(false);
  const supplierRef = useRef<HTMLDivElement>(null);

  // Focus input when search is toggled open
  useEffect(() => {
    if (showCategorySearch) {
      categorySearchInputRef.current?.focus();
    }
  }, [showCategorySearch]);

  useEffect(() => {
    if (showBrandSearch) {
      brandSearchInputRef.current?.focus();
    }
  }, [showBrandSearch]);

  // Close dropdowns on outside click
  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (subCategoryRef.current && !subCategoryRef.current.contains(e.target as Node)) {
        setIsSubCategoryOpen(false);
      }
      if (supplierRef.current && !supplierRef.current.contains(e.target as Node)) {
        setIsSupplierOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Get category display name (prioritizes dynamic availableCategories with localized name {th, en})
  const getCategoryLabel = (catId: string, labelKey?: string) => {
    if (catId === "all") return t("catalogFacets.categories.all");
    const dynamicCat = availableCategories?.find((c) => c.id === catId);
    if (dynamicCat) return dynamicCat.name[locale] || dynamicCat.name.th;
    return labelKey ? t(`catalogFacets.categories.${labelKey}`) : catId;
  };

  // Filtered categories
  const filteredCategories = useMemo(() => {
    // If availableCategories exists from API/catalog, merge them with the "all" option
    const baseCategories = availableCategories && availableCategories.length > 0
      ? [
          { id: "all", labelKey: "all", name: { th: t("catalogFacets.categories.all"), en: t("catalogFacets.categories.all") } },
          ...availableCategories.map((c) => ({
            id: c.id,
            labelKey: c.id,
            name: c.name,
          })),
        ]
      : CATEGORIES.map((c) => ({
          ...c,
          name: {
            th: t(`catalogFacets.categories.${c.labelKey}`),
            en: t(`catalogFacets.categories.${c.labelKey}`),
          },
        }));

    if (!categorySearch.trim()) return baseCategories;
    const q = categorySearch.trim().toLowerCase();
    return baseCategories.filter((cat) => {
      if (cat.id === "all") return true;
      const thMatch = cat.name.th.toLowerCase().includes(q);
      const enMatch = cat.name.en.toLowerCase().includes(q);
      const idMatch = cat.id.toLowerCase().includes(q);
      return thMatch || enMatch || idMatch;
    });
  }, [availableCategories, categorySearch, t]);

  // Filtered brands
  const filteredBrands = useMemo(() => {
    if (!brandSearch.trim()) return availableBrands;
    const q = brandSearch.trim().toLowerCase();
    return availableBrands.filter(
      (br) =>
        br.name.th.toLowerCase().includes(q) ||
        br.name.en.toLowerCase().includes(q) ||
        br.id.toLowerCase().includes(q)
    );
  }, [availableBrands, brandSearch]);

  // Filtered subcategories for autocomplete
  const filteredSubCategories = useMemo(() => {
    if (!subCategorySearch.trim()) return availableSubCategories;
    const q = subCategorySearch.trim().toLowerCase();
    return availableSubCategories.filter(
      (sub) =>
        sub.name.th.toLowerCase().includes(q) ||
        sub.name.en.toLowerCase().includes(q) ||
        sub.id.toLowerCase().includes(q)
    );
  }, [availableSubCategories, subCategorySearch]);

  // Filtered suppliers for autocomplete
  const filteredSuppliers = useMemo(() => {
    if (!supplierSearch.trim()) return availableSuppliers;
    const q = supplierSearch.trim().toLowerCase();
    return availableSuppliers.filter(
      (sup) =>
        sup.name.th.toLowerCase().includes(q) ||
        sup.name.en.toLowerCase().includes(q) ||
        sup.code.toLowerCase().includes(q) ||
        sup.id.toLowerCase().includes(q)
    );
  }, [availableSuppliers, supplierSearch]);

  // Active subcategory display name
  const activeSubCategoryItem = availableSubCategories.find((s) => s.id === selectedSubCategory);
  const activeSubCategoryName = activeSubCategoryItem
    ? activeSubCategoryItem.name[locale] || activeSubCategoryItem.name.th
    : "";

  // Active supplier display name
  const activeSupplierItem = availableSuppliers.find((s) => s.id === selectedSupplier);
  const activeSupplierName = activeSupplierItem
    ? activeSupplierItem.name[locale] || activeSupplierItem.name.th
    : "";

  const attributeKeys = Object.keys(availableAttributes);

  // Active filter counts per section
  const activeCostTypeCount = selectedTypes.length < COST_TYPES.length ? selectedTypes.length : 0;
  const activeCategoryCount = (selectedCategory !== "all" ? 1 : 0) + (selectedSubCategory !== "all" ? 1 : 0);
  const activeAttributeCount = Object.values(selectedAttributes).filter((v) => v && v !== "all").length;
  const activeBrandCount = selectedBrands.length;
  const activeSupplierCount = selectedSupplier !== "all" ? 1 : 0;

  const handleReset = () => {
    setShowCategorySearch(false);
    setCategorySearch("");
    setShowBrandSearch(false);
    setBrandSearch("");
    setSubCategorySearch("");
    setSupplierSearch("");
    setIsSubCategoryOpen(false);
    setIsSupplierOpen(false);
    onResetFilters();
  };

  return (
    <aside
      className={cn(
        "w-full md:w-64 bg-erp-surface-subtle/50 border border-erp-border p-3 space-y-4 overflow-y-auto shrink-0 text-xs flex flex-col max-h-full",
        className
      )}
    >
      {/* Header with Clear button */}
      <div className="flex items-center justify-between pb-1.5 border-b border-erp-border shrink-0">
        <span className="font-bold text-erp-navy uppercase text-[11px] tracking-wider">
          {t("catalogFacets.selectedFilters")}
        </span>
        <button
          type="button"
          onClick={handleReset}
          className="text-[11px] text-erp-navy hover:underline font-semibold"
        >
          {t("catalogFacets.clearFilters")}
        </button>
      </div>

      {/* Facet 1: Cost Type */}
      <Collapsible
        title={
          <div className="flex items-center gap-1.5">
            <span className="font-bold text-erp-text-main text-[11px] group-hover:text-erp-navy transition-colors">
              {t("catalogFacets.costType")}
            </span>
            {activeCostTypeCount > 0 && (
              <span className="px-1.5 py-0.2 bg-erp-navy text-white text-[10px] font-mono leading-none">
                {activeCostTypeCount}
              </span>
            )}
          </div>
        }
        isOpen={!collapsedSections.costType}
        onToggle={() => toggleSection("costType")}
        className="space-y-1"
      >
        {COST_TYPES.map((ct) => (
          <Checkbox
            key={ct}
            id={`filter-cost-type-${ct}`}
            checked={selectedTypes.includes(ct)}
            onChange={() => toggleType(ct)}
            label={<span className="text-xs text-erp-text-secondary">{t(`costTypes.${ct}`)}</span>}
            wrapperClassName="mb-1"
            className="w-3.5 h-3.5"
          />
        ))}
      </Collapsible>

      {/* Facet 2: Category with Search Toggle & Sub-Category Autocomplete */}
      <Collapsible
        title={
          <div className="flex items-center gap-1.5">
            <span className="font-bold text-erp-text-main text-[11px] group-hover:text-erp-navy transition-colors">
              {t("catalogFacets.category")}
            </span>
            {activeCategoryCount > 0 && (
              <span className="px-1.5 py-0.2 bg-erp-navy text-white text-[10px] font-mono leading-none">
                {activeCategoryCount}
              </span>
            )}
          </div>
        }
        isOpen={!collapsedSections.category}
        onToggle={() => toggleSection("category")}
        borderedTop
        actions={
          <button
            type="button"
            onClick={() => {
              if (collapsedSections.category) {
                setCollapsedSections((prev) => ({ ...prev, category: false }));
              }
              if (showCategorySearch) {
                setCategorySearch("");
              }
              setShowCategorySearch(!showCategorySearch);
            }}
            title={t("catalogFacets.searchCategoryPlaceholder")}
            className={`p-0.5 rounded-none border transition-colors cursor-pointer ${
              showCategorySearch
                ? "bg-erp-navy text-white border-erp-navy"
                : "text-erp-text-muted hover:text-erp-navy border-transparent hover:border-erp-border"
            }`}
          >
            <IconSearch size={13} />
          </button>
        }
      >
        {/* Category Search Input (Shown when Search icon is clicked) */}
        {showCategorySearch && (
          <div className="relative mb-1">
            <input
              ref={categorySearchInputRef}
              type="text"
              placeholder={t("catalogFacets.searchCategoryPlaceholder")}
              value={categorySearch}
              onChange={(e) => setCategorySearch(e.target.value)}
              className="w-full h-6 pl-2 pr-6 text-[11px] border border-erp-border bg-erp-surface text-erp-text-main placeholder:text-erp-text-muted focus-visible:outline-1 focus-visible:outline-erp-navy rounded-none"
            />
            {categorySearch && (
              <button
                type="button"
                onClick={() => setCategorySearch("")}
                className="absolute right-1 top-1 text-erp-text-muted hover:text-erp-danger"
              >
                <IconClose size={12} />
              </button>
            )}
          </div>
        )}

        {/* Category Radio List */}
        <div className="space-y-0.5">
          {filteredCategories.map((cat) => (
            <label
              key={cat.id}
              className="flex items-center gap-2 cursor-pointer select-none text-erp-text-secondary hover:text-erp-text-main py-0.5"
            >
              <input
                type="radio"
                name="modalCategory"
                checked={selectedCategory === cat.id}
                onChange={() => setSelectedCategory(cat.id)}
                className="w-3.5 h-3.5 text-erp-navy border-erp-border cursor-pointer shrink-0"
              />
              <span className="truncate text-xs">
                {getCategoryLabel(cat.id, cat.labelKey)}
              </span>
            </label>
          ))}
          {filteredCategories.length === 0 && (
            <p className="text-[11px] text-erp-text-muted italic py-1 text-center">
              -
            </p>
          )}
        </div>

        {/* Sub-Category Autocomplete (Input Search with Suggestions) */}
        {availableSubCategories.length > 0 && (
          <div className="pt-2 border-t border-erp-border/40 relative" ref={subCategoryRef}>
            <div className="flex items-center justify-between mb-1">
              <span className="text-[11px] text-erp-text-muted font-semibold">
                {t("catalogFacets.subCategory")}:
              </span>
              {selectedSubCategory !== "all" && (
                <button
                  type="button"
                  onClick={() => {
                    setSelectedSubCategory("all");
                    setSubCategorySearch("");
                  }}
                  className="text-[10px] text-erp-danger hover:underline"
                >
                  {t("catalogFacets.clearFilters")}
                </button>
              )}
            </div>

            {/* SubCategory Autocomplete Input */}
            <div className="relative">
              <input
                type="text"
                value={
                  isSubCategoryOpen
                    ? subCategorySearch
                    : activeSubCategoryName || (selectedSubCategory === "all" ? "" : selectedSubCategory)
                }
                placeholder={t("catalogFacets.searchSubCategoryPlaceholder")}
                onFocus={() => {
                  setSubCategorySearch("");
                  setIsSubCategoryOpen(true);
                }}
                onChange={(e) => {
                  setSubCategorySearch(e.target.value);
                  setIsSubCategoryOpen(true);
                }}
                className="w-full h-7 pl-2 pr-6 text-xs border border-erp-border bg-erp-surface text-erp-text-main placeholder:text-erp-text-muted rounded-none focus-visible:outline-1 focus-visible:outline-erp-navy"
              />
              {selectedSubCategory !== "all" && !isSubCategoryOpen ? (
                <button
                  type="button"
                  onClick={() => {
                    setSelectedSubCategory("all");
                    setSubCategorySearch("");
                  }}
                  className="absolute right-1.5 top-1.5 text-erp-text-muted hover:text-erp-danger"
                >
                  <IconClose size={13} />
                </button>
              ) : (
                <div className="absolute right-1.5 top-1.5 text-erp-text-muted pointer-events-none">
                  <IconSearch size={13} />
                </div>
              )}
            </div>

            {/* Autocomplete Suggestions Dropdown */}
            {isSubCategoryOpen && (
              <div className="absolute z-20 left-0 right-0 top-full mt-0.5 max-h-44 overflow-y-auto bg-erp-surface border border-erp-border shadow-md">
                <button
                  type="button"
                  onClick={() => {
                    setSelectedSubCategory("all");
                    setSubCategorySearch("");
                    setIsSubCategoryOpen(false);
                  }}
                  className={`w-full text-left px-2.5 py-1.5 text-xs hover:bg-erp-surface-subtle transition-colors flex items-center justify-between ${
                    selectedSubCategory === "all" ? "bg-erp-navy/10 text-erp-navy font-bold" : "text-erp-text-main"
                  }`}
                >
                  <span>{t("catalogFacets.allSubCategories")}</span>
                  {selectedSubCategory === "all" && <span>✓</span>}
                </button>

                {filteredSubCategories.map((sub) => {
                  const isSelected = selectedSubCategory === sub.id;
                  const name = sub.name[locale] || sub.name.th;
                  return (
                    <button
                      key={sub.id}
                      type="button"
                      onClick={() => {
                        setSelectedSubCategory(sub.id);
                        setSubCategorySearch("");
                        setIsSubCategoryOpen(false);
                      }}
                      className={`w-full text-left px-2.5 py-1.5 text-xs hover:bg-erp-surface-subtle transition-colors flex items-center justify-between border-t border-erp-border/40 ${
                        isSelected ? "bg-erp-navy/10 text-erp-navy font-bold" : "text-erp-text-main"
                      }`}
                    >
                      <span className="truncate">{name}</span>
                      {isSelected && <span className="ml-1">✓</span>}
                    </button>
                  );
                })}

                {filteredSubCategories.length === 0 && (
                  <div className="px-2.5 py-2 text-xs text-erp-text-muted italic text-center">
                    {t("noMatchingItems")}
                  </div>
                )}
              </div>
            )}
          </div>
        )}
      </Collapsible>

      {/* Facet 3: Dynamic Attributes (ขนาด, สี, ผิว, เกรด, มาตรฐาน ฯลฯ) */}
      {attributeKeys.length > 0 && (
        <Collapsible
          title={
            <div className="flex items-center gap-1.5">
              <span className="font-bold text-erp-text-main text-[11px] group-hover:text-erp-navy transition-colors">
                {t("catalogFacets.attributesTitle")}
              </span>
              {activeAttributeCount > 0 && (
                <span className="px-1.5 py-0.2 bg-erp-navy text-white text-[10px] font-mono leading-none">
                  {activeAttributeCount}
                </span>
              )}
            </div>
          }
          isOpen={!collapsedSections.attributes}
          onToggle={() => toggleSection("attributes")}
          borderedTop
          className="space-y-2"
        >
          <div className="space-y-2 pt-0.5">
            {attributeKeys.map((attrKey) => {
              const values = availableAttributes[attrKey] || [];
              const currentValue = selectedAttributes[attrKey] || "all";
              const attrLabel =
                attrKey === "size"
                  ? t("catalogFacets.attrSize")
                  : attrKey === "color"
                  ? t("catalogFacets.attrColor")
                  : attrKey === "finish"
                  ? t("catalogFacets.attrFinish")
                  : attrKey === "grade"
                  ? t("catalogFacets.attrGrade")
                  : attrKey === "standard"
                  ? t("catalogFacets.attrStandard")
                  : attrKey === "thickness"
                  ? t("catalogFacets.attrThickness")
                  : attrKey;

              return (
                <div key={attrKey} className="space-y-1">
                  <label className="text-[11px] text-erp-text-secondary font-medium flex items-center justify-between">
                    <span>{attrLabel}</span>
                    {currentValue !== "all" && (
                      <span className="text-[10px] text-erp-navy font-mono">
                        {currentValue}
                      </span>
                    )}
                  </label>
                  <select
                    value={currentValue}
                    onChange={(e) => setAttributeValue(attrKey, e.target.value)}
                    className="w-full h-7 px-2 text-xs border border-erp-border bg-erp-surface text-erp-text-main rounded-none focus-visible:outline-1 focus-visible:outline-erp-navy"
                  >
                    <option value="all">
                      {t("catalogFacets.allAttributes", { attribute: attrLabel })}
                    </option>
                    {values.map((val) => (
                      <option key={val} value={val}>
                        {val}
                      </option>
                    ))}
                  </select>
                </div>
              );
            })}
          </div>
        </Collapsible>
      )}

      {/* Facet 4: Brand with Search Icon Toggle */}
      {availableBrands.length > 0 && (
        <Collapsible
          title={
            <div className="flex items-center gap-1.5">
              <span className="font-bold text-erp-text-main text-[11px] group-hover:text-erp-navy transition-colors">
                {t("catalogFacets.brand")}
              </span>
              {activeBrandCount > 0 && (
                <span className="px-1.5 py-0.2 bg-erp-navy text-white text-[10px] font-mono leading-none">
                  {activeBrandCount}
                </span>
              )}
            </div>
          }
          isOpen={!collapsedSections.brand}
          onToggle={() => toggleSection("brand")}
          borderedTop
          actions={
            <div className="flex items-center gap-1.5">
              <span className="text-[10px] text-erp-text-muted font-mono">
                ({selectedBrands.length}/{availableBrands.length})
              </span>
              <button
                type="button"
                onClick={() => {
                  if (collapsedSections.brand) {
                    setCollapsedSections((prev) => ({ ...prev, brand: false }));
                  }
                  if (showBrandSearch) {
                    setBrandSearch("");
                  }
                  setShowBrandSearch(!showBrandSearch);
                }}
                title={t("catalogFacets.searchBrandPlaceholder")}
                className={`p-0.5 rounded-none border transition-colors cursor-pointer ${
                  showBrandSearch
                    ? "bg-erp-navy text-white border-erp-navy"
                    : "text-erp-text-muted hover:text-erp-navy border-transparent hover:border-erp-border"
                }`}
              >
                <IconSearch size={13} />
              </button>
            </div>
          }
        >
          {/* Brand Search Input (Shown when search icon is clicked) */}
          {showBrandSearch && (
            <div className="relative mb-1">
              <input
                ref={brandSearchInputRef}
                type="text"
                placeholder={t("catalogFacets.searchBrandPlaceholder")}
                value={brandSearch}
                onChange={(e) => setBrandSearch(e.target.value)}
                className="w-full h-6 pl-2 pr-6 text-[11px] border border-erp-border bg-erp-surface text-erp-text-main placeholder:text-erp-text-muted focus-visible:outline-1 focus-visible:outline-erp-navy rounded-none"
              />
              {brandSearch && (
                <button
                  type="button"
                  onClick={() => setBrandSearch("")}
                  className="absolute right-1 top-1 text-erp-text-muted hover:text-erp-danger"
                >
                  <IconClose size={12} />
                </button>
              )}
            </div>
          )}

          <div className="space-y-1">
            {filteredBrands.map((br) => {
              const displayName = br.name[locale] || br.name.th;
              return (
                <Checkbox
                  key={br.id}
                  id={`filter-brand-${br.id}`}
                  checked={selectedBrands.includes(br.id)}
                  onChange={() => toggleBrand(br.id)}
                  label={<span className="text-xs text-erp-text-secondary truncate block">{displayName}</span>}
                  wrapperClassName="mb-0.5"
                  className="w-3.5 h-3.5"
                />
              );
            })}
            {filteredBrands.length === 0 && (
              <p className="text-[11px] text-erp-text-muted italic py-1 text-center">
                -
              </p>
            )}
          </div>
        </Collapsible>
      )}

      {/* Facet 5: Supplier / Vendor with Autocomplete Input Search */}
      {availableSuppliers.length > 0 && (
        <div className="relative" ref={supplierRef}>
          <Collapsible
            title={
              <div className="flex items-center gap-1.5">
                <span className="font-bold text-erp-text-main text-[11px] group-hover:text-erp-navy transition-colors">
                  {t("catalogFacets.supplier")}
                </span>
                {activeSupplierCount > 0 && (
                  <span className="px-1.5 py-0.2 bg-erp-navy text-white text-[10px] font-mono leading-none">
                    {activeSupplierCount}
                  </span>
                )}
              </div>
            }
            isOpen={!collapsedSections.supplier}
            onToggle={() => toggleSection("supplier")}
            borderedTop
            actions={
              selectedSupplier !== "all" ? (
                <button
                  type="button"
                  onClick={() => {
                    setSelectedSupplier("all");
                    setSupplierSearch("");
                  }}
                  className="text-[10px] text-erp-danger hover:underline cursor-pointer"
                >
                  {t("catalogFacets.clearFilters")}
                </button>
              ) : undefined
            }
          >
            {/* Supplier Autocomplete Input */}
            <div className="relative">
              <input
                type="text"
                value={
                  isSupplierOpen
                    ? supplierSearch
                    : activeSupplierName || (selectedSupplier === "all" ? "" : selectedSupplier)
                }
                placeholder={t("catalogFacets.searchSupplierPlaceholder")}
                onFocus={() => {
                  setSupplierSearch("");
                  setIsSupplierOpen(true);
                }}
                onChange={(e) => {
                  setSupplierSearch(e.target.value);
                  setIsSupplierOpen(true);
                }}
                className="w-full h-7 pl-2 pr-6 text-xs border border-erp-border bg-erp-surface text-erp-text-main placeholder:text-erp-text-muted rounded-none focus-visible:outline-1 focus-visible:outline-erp-navy truncate"
              />
              {selectedSupplier !== "all" && !isSupplierOpen ? (
                <button
                  type="button"
                  onClick={() => {
                    setSelectedSupplier("all");
                    setSupplierSearch("");
                  }}
                  className="absolute right-1.5 top-1.5 text-erp-text-muted hover:text-erp-danger"
                >
                  <IconClose size={13} />
                </button>
              ) : (
                <div className="absolute right-1.5 top-1.5 text-erp-text-muted pointer-events-none">
                  <IconSearch size={13} />
                </div>
              )}
            </div>

            {/* Autocomplete Suggestions Dropdown */}
            {isSupplierOpen && (
              <div className="absolute z-20 left-0 right-0 top-full mt-0.5 max-h-48 overflow-y-auto bg-erp-surface border border-erp-border shadow-md">
                <button
                  type="button"
                  onClick={() => {
                    setSelectedSupplier("all");
                    setSupplierSearch("");
                    setIsSupplierOpen(false);
                  }}
                  className={`w-full text-left px-2.5 py-1.5 text-xs hover:bg-erp-surface-subtle transition-colors flex items-center justify-between ${
                    selectedSupplier === "all" ? "bg-erp-navy/10 text-erp-navy font-bold" : "text-erp-text-main"
                  }`}
                >
                  <span>{t("catalogFacets.allSuppliers")}</span>
                  {selectedSupplier === "all" && <span>✓</span>}
                </button>

                {filteredSuppliers.map((sup) => {
                  const isSelected = selectedSupplier === sup.id;
                  const name = sup.name[locale] || sup.name.th;
                  return (
                    <button
                      key={sup.id}
                      type="button"
                      onClick={() => {
                        setSelectedSupplier(sup.id);
                        setSupplierSearch("");
                        setIsSupplierOpen(false);
                      }}
                      className={`w-full text-left px-2.5 py-1.5 text-xs hover:bg-erp-surface-subtle transition-colors flex items-center justify-between border-t border-erp-border/40 ${
                        isSelected ? "bg-erp-navy/10 text-erp-navy font-bold" : "text-erp-text-main"
                      }`}
                    >
                      <div className="truncate">
                        <div className="truncate">{name}</div>
                        <div className="text-[10px] text-erp-text-muted font-mono">{sup.code}</div>
                      </div>
                      {isSelected && <span className="ml-1">✓</span>}
                    </button>
                  );
                })}

                {filteredSuppliers.length === 0 && (
                  <div className="px-2.5 py-2 text-xs text-erp-text-muted italic text-center">
                    {t("noMatchingItems")}
                  </div>
                )}
              </div>
            )}
          </Collapsible>
        </div>
      )}
    </aside>
  );
}
