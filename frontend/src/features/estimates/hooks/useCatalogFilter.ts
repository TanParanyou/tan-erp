import { useState, useMemo, useEffect, useCallback } from "react";
import {
  ESTIMATE_CATALOG_ITEMS,
  filterCatalogItems,
  type CatalogItem,
  type CostNatureType,
  type LocalizedString,
} from "../constants/estimate-catalog-items";

export type CatalogScope = "all" | "cost" | "sell";

export interface BrandOption {
  id: string;
  name: LocalizedString;
}

export interface CategoryOption {
  id: string;
  name: LocalizedString;
}

export interface SupplierOption {
  id: string;
  code: string;
  name: LocalizedString;
}

export interface SubCategoryOption {
  id: string;
  categoryId: string;
  name: LocalizedString;
}

export interface UseCatalogFilterProps {
  isOpen: boolean;
  onSelectItems: (items: CatalogItem[]) => void;
  onClose: () => void;
}

export function useCatalogFilter({
  isOpen,
  onSelectItems,
  onClose,
}: UseCatalogFilterProps) {
  const [search, setSearch] = useState("");
  const [scope, setScope] = useState<CatalogScope>("cost");
  const [selectedTypes, setSelectedTypes] = useState<string[]>([
    "material",
    "labor",
    "subcontract",
    "equipment",
  ]);
  const [selectedCategory, setSelectedCategory] = useState<string>("all");
  const [selectedSubCategory, setSelectedSubCategory] = useState<string>("all");
  const [selectedThicknesses, setSelectedThicknesses] = useState<string[]>([]);
  const [selectedBrands, setSelectedBrands] = useState<string[]>([]);
  const [selectedSupplier, setSelectedSupplier] = useState<string>("all");
  const [selectedAttributes, setSelectedAttributes] = useState<Record<string, string>>({});
  const [selectedItemIds, setSelectedItemIds] = useState<string[]>([]);

  // Reset selection when modal opens
  useEffect(() => {
    if (isOpen) {
      setSelectedItemIds([]);
    }
  }, [isOpen]);

  // When category changes, reset sub-category and attributes
  const handleCategoryChange = useCallback((cat: string) => {
    setSelectedCategory(cat);
    setSelectedSubCategory("all");
    setSelectedAttributes({});
    setSelectedThicknesses([]);
  }, []);

  // Extract unique available categories from catalog
  const availableCategories = useMemo<CategoryOption[]>(() => {
    const catMap = new Map<string, CategoryOption>();
    for (const item of ESTIMATE_CATALOG_ITEMS) {
      if (item.category.id && !catMap.has(item.category.id)) {
        catMap.set(item.category.id, {
          id: item.category.id,
          name: item.category.name,
        });
      }
    }
    return Array.from(catMap.values());
  }, []);

  // Extract unique available brands from catalog
  const availableBrands = useMemo<BrandOption[]>(() => {
    const brandMap = new Map<string, BrandOption>();
    for (const item of ESTIMATE_CATALOG_ITEMS) {
      if (item.brand.id && !brandMap.has(item.brand.id)) {
        brandMap.set(item.brand.id, {
          id: item.brand.id,
          name: item.brand.name,
        });
      }
    }
    return Array.from(brandMap.values());
  }, []);

  // Extract unique available suppliers from catalog
  const availableSuppliers = useMemo<SupplierOption[]>(() => {
    const supMap = new Map<string, SupplierOption>();
    for (const item of ESTIMATE_CATALOG_ITEMS) {
      if (item.supplier && !supMap.has(item.supplier.id)) {
        supMap.set(item.supplier.id, {
          id: item.supplier.id,
          code: item.supplier.code,
          name: item.supplier.name,
        });
      }
    }
    return Array.from(supMap.values());
  }, []);

  // Extract available sub-categories for current category
  const availableSubCategories = useMemo<SubCategoryOption[]>(() => {
    const subMap = new Map<string, SubCategoryOption>();
    for (const item of ESTIMATE_CATALOG_ITEMS) {
      if (selectedCategory !== "all" && item.category.id !== selectedCategory) {
        continue;
      }
      if (item.subCategory && !subMap.has(item.subCategory.id)) {
        subMap.set(item.subCategory.id, {
          id: item.subCategory.id,
          categoryId: item.category.id,
          name: item.subCategory.name,
        });
      }
    }
    return Array.from(subMap.values());
  }, [selectedCategory]);

  // Extract unique available thicknesses
  const availableThicknesses = useMemo(() => {
    const thickSet = new Set<string>();
    for (const item of ESTIMATE_CATALOG_ITEMS) {
      if (selectedCategory !== "all" && item.category.id !== selectedCategory) {
        continue;
      }
      if (item.specs?.thickness) {
        thickSet.add(item.specs.thickness);
      }
    }
    return Array.from(thickSet).sort((a, b) => {
      const numA = parseFloat(a) || 0;
      const numB = parseFloat(b) || 0;
      return numA - numB;
    });
  }, [selectedCategory]);

  // Extract available dynamic attributes for selected category
  const availableAttributes = useMemo(() => {
    const attrMap: Record<string, Set<string>> = {};
    for (const item of ESTIMATE_CATALOG_ITEMS) {
      if (selectedCategory !== "all" && item.category.id !== selectedCategory) {
        continue;
      }
      if (item.attributes) {
        for (const [k, v] of Object.entries(item.attributes)) {
          if (!attrMap[k]) attrMap[k] = new Set<string>();
          attrMap[k].add(v);
        }
      }
    }

    const result: Record<string, string[]> = {};
    for (const [k, set] of Object.entries(attrMap)) {
      result[k] = Array.from(set).sort();
    }
    return result;
  }, [selectedCategory]);

  // Toggle Type filter
  const toggleType = useCallback((type: CostNatureType | string) => {
    setSelectedTypes((prev) =>
      prev.includes(type) ? prev.filter((t) => t !== type) : [...prev, type]
    );
  }, []);

  // Toggle Thickness filter
  const toggleThickness = useCallback((thick: string) => {
    setSelectedThicknesses((prev) =>
      prev.includes(thick) ? prev.filter((t) => t !== thick) : [...prev, thick]
    );
  }, []);

  // Toggle Brand filter
  const toggleBrand = useCallback((brandId: string) => {
    setSelectedBrands((prev) =>
      prev.includes(brandId) ? prev.filter((b) => b !== brandId) : [...prev, brandId]
    );
  }, []);

  // Update Dynamic Attribute value
  const setAttributeValue = useCallback((key: string, value: string) => {
    setSelectedAttributes((prev) => {
      if (!value || value === "all") {
        const next = { ...prev };
        delete next[key];
        return next;
      }
      return { ...prev, [key]: value };
    });
  }, []);

  // Toggle Item selection
  const toggleSelectItem = useCallback((id: string) => {
    setSelectedItemIds((prev) =>
      prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id]
    );
  }, []);

  // Reset all filters
  const handleResetFilters = useCallback(() => {
    setSearch("");
    setScope("all");
    setSelectedTypes(["material", "labor", "subcontract", "equipment"]);
    setSelectedCategory("all");
    setSelectedSubCategory("all");
    setSelectedThicknesses([]);
    setSelectedBrands([]);
    setSelectedSupplier("all");
    setSelectedAttributes({});
  }, []);

  // Remove a single active filter
  const removeFilter = useCallback((type: string, val?: string) => {
    if (type === "search") setSearch("");
    else if (type === "category") handleCategoryChange("all");
    else if (type === "subCategory") setSelectedSubCategory("all");
    else if (type === "supplier") setSelectedSupplier("all");
    else if (type === "thickness" && val) {
      setSelectedThicknesses((prev) => prev.filter((t) => t !== val));
    } else if (type === "brand" && val) {
      setSelectedBrands((prev) => prev.filter((b) => b !== val));
    } else if (type.startsWith("attr_") && val) {
      const attrKey = type.replace("attr_", "");
      setAttributeValue(attrKey, "all");
    }
  }, [handleCategoryChange, setAttributeValue]);

  // Filter items based on active criteria
  const filteredItems = useMemo(() => {
    return filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      search,
      scope,
      itemTypes: selectedTypes,
      category: selectedCategory,
      subCategory: selectedSubCategory,
      thicknesses: selectedThicknesses,
      brands: selectedBrands,
      suppliers: selectedSupplier !== "all" ? [selectedSupplier] : [],
      attributes: selectedAttributes,
    });
  }, [
    search,
    scope,
    selectedTypes,
    selectedCategory,
    selectedSubCategory,
    selectedThicknesses,
    selectedBrands,
    selectedSupplier,
    selectedAttributes,
  ]);

  // Handle select all visible
  const isAllVisibleSelected =
    filteredItems.length > 0 &&
    filteredItems.every((item) => selectedItemIds.includes(item.id));

  const handleToggleSelectAll = useCallback(() => {
    if (isAllVisibleSelected) {
      const visibleIds = new Set(filteredItems.map((i) => i.id));
      setSelectedItemIds((prev) => prev.filter((id) => !visibleIds.has(id)));
    } else {
      const newIds = new Set([...selectedItemIds, ...filteredItems.map((i) => i.id)]);
      setSelectedItemIds(Array.from(newIds));
    }
  }, [isAllVisibleSelected, filteredItems, selectedItemIds]);

  // Confirm insert
  const handleConfirmInsert = useCallback(() => {
    const selected = ESTIMATE_CATALOG_ITEMS.filter((item) =>
      selectedItemIds.includes(item.id)
    );
    if (selected.length > 0) {
      onSelectItems(selected);
      onClose();
    }
  }, [selectedItemIds, onSelectItems, onClose]);

  return {
    search,
    setSearch,
    scope,
    setScope,
    selectedTypes,
    toggleType,
    selectedCategory,
    setSelectedCategory: handleCategoryChange,
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
  };
}
