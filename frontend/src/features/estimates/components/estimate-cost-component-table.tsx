"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray, Controller } from "react-hook-form";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { IconTrash, IconPlus } from "@/components/common/Icons";
import { useConfirm } from "@/hooks/useConfirm";
import { useEstimateOptions } from "../options/estimate-options";
import { QUICK_COST_PRESETS, type QuickCostPreset } from "../constants/estimate-templates";
import {
  getLocalizedText,
  type CatalogItemModel,
} from "../api/estimate-catalog-client";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { calculateCostComponentSubtotal } from "../utils/estimate-calculations";
import { formatFinancialNumber } from "../utils/estimate-formatters";
import { EstimateItemCatalogModal } from "./estimate-item-catalog-modal";

interface EstimateCostComponentTableProps {
  sectionIndex: number;
  itemIndex: number;
  currency: string;
  branchId?: string;
}

export function EstimateCostComponentTable({
  sectionIndex,
  itemIndex,
  currency,
  branchId,
}: EstimateCostComponentTableProps) {
  const t = useTranslations("estimates");
  const locale = useLocale();
  const { options } = useEstimateOptions();
  const { confirm, ConfirmDialog } = useConfirm();
  const { control, watch } = useFormContext<EstimateWorkspaceFormData>();
  const [isCatalogOpen, setIsCatalogOpen] = useState(false);

  const fieldArrayName = `sections.${sectionIndex}.workItems.${itemIndex}.costComponents` as const;

  const { fields, append, remove } = useFieldArray({
    control,
    name: fieldArrayName,
  });

  const costComponents = watch(fieldArrayName) || [];

  const handleAddCost = () => {
    append({
      type: "material",
      description: "",
      quantity: 1,
      unitCode: "lot",
      unitCost: 0,
      currency,
      sortOrder: fields.length + 1,
    });
  };

  const handleQuickAddCost = (preset: QuickCostPreset) => {
    append({
      type: preset.costType,
      description: preset.defaultDesc,
      quantity: 1,
      unitCode: "lot",
      unitCost: preset.defaultCost,
      currency,
      sortOrder: fields.length + 1,
    });
  };

  const handleInsertFromCatalog = (selectedItems: CatalogItemModel[]) => {
    selectedItems.forEach((item, selectedIndex) => {
      const rawType = (item.itemType || "").toLowerCase();
      const componentType: "material" | "labor" | "subcontract" | "equipment" =
        rawType === "labor" || rawType === "subcontract" || rawType === "equipment"
          ? rawType
          : "material";

      append({
        type: componentType,
        description: getLocalizedText(item.name, locale),
        quantity: 1,
        unitCode: item.resolvedCost?.unitCode || item.baseUnit.code || "lot",
        unitCost: item.resolvedCost?.amount ?? 0,
        currency: item.resolvedCost?.currency || currency,
        sortOrder: fields.length + selectedIndex + 1,
        itemId: item.id,
        costRecordId: item.resolvedCost?.costRecordId || null,
        costRecordVersion: item.resolvedCost?.version ?? null,
      });
    });
  };

  const handleRemoveCost = async (index: number) => {
    const comp = costComponents[index];
    const hasValue =
      (comp?.description && comp.description.trim().length > 0) ||
      (comp?.unitCost && comp.unitCost > 0);

    if (hasValue) {
      const ok = await confirm({
        title: t("confirmDeleteCostComponent"),
        message: comp.description ? `"${comp.description}"` : t("confirmDeleteCostComponent"),
        confirmText: t("removeWorkItem"),
        variant: "danger",
      });
      if (ok) {
        remove(index);
      }
    } else {
      remove(index);
    }
  };

  return (
    <div className="bg-erp-surface border border-erp-border p-3.5 mt-2">
      {/* 1. Header & Add Actions */}
      <div className="flex flex-wrap justify-between items-center mb-2.5 gap-2">
        <span className="text-xs font-bold text-erp-navy flex items-center gap-1.5">
          {t("costComponents")} ({fields.length})
        </span>

        <div className="flex items-center gap-2">
          {/* Browse Catalog Button */}
          <button
            type="button"
            onClick={() => setIsCatalogOpen(true)}
            className="inline-flex items-center gap-1 text-xs font-semibold text-erp-navy hover:text-white hover:bg-erp-navy py-1 px-2.5 border border-erp-navy transition-colors bg-white"
            title={t("catalogModalDesc")}
          >
            <svg
              className="w-3.5 h-3.5"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3" />
            </svg>
            {t("browseCatalog")}
          </button>

          {/* Add Blank Cost Button */}
          <button
            type="button"
            onClick={handleAddCost}
            className="inline-flex items-center gap-1 text-xs font-semibold text-erp-text-main hover:text-erp-navy hover:bg-erp-surface-subtle py-1 px-2.5 border border-erp-border transition-colors bg-white"
          >
            <IconPlus size={13} />
            {t("addComponent")}
          </button>
        </div>
      </div>

      {/* 2. Quick Cost Chips */}
      <div className="flex flex-wrap items-center gap-1.5 mb-3 pb-2.5 border-b border-border/60">
        <span className="text-[11px] font-semibold text-erp-text-muted mr-1">
          {t("quickAddCostTitle")}
        </span>
        {QUICK_COST_PRESETS.map((preset) => (
          <button
            key={preset.id}
            type="button"
            onClick={() => handleQuickAddCost(preset)}
            className="inline-flex items-center gap-1 px-2 py-0.5 text-[11px] rounded-none border border-border bg-erp-bg-neutral hover:bg-erp-navy hover:text-white hover:border-erp-navy transition-colors text-erp-text-secondary font-mono"
            title={`${preset.defaultDesc} (@ ${preset.defaultCost} ${currency})`}
          >
            <span className="text-erp-text-muted hover:text-white">+</span>
            <span>{t(preset.labelKey)}</span>
          </button>
        ))}
      </div>

      {/* 3. Cost Components Table */}
      {fields.length === 0 ? (
        <div className="text-center py-5 text-xs text-erp-text-muted border border-dashed border-erp-border bg-erp-surface-subtle/50">
          {t("addComponent")}
        </div>
      ) : (
        <div className="border border-erp-border">
          {/* Table Head (Visible on sm screens and up) */}
          <div className="hidden sm:grid grid-cols-12 gap-2 px-3 py-1.5 bg-erp-surface-subtle text-[11px] font-bold text-erp-text-secondary border-b border-erp-border items-center">
            <div className="col-span-3">{t("costType")}</div>
            <div className="col-span-4">{t("costDesc")}</div>
            <div className="col-span-1 text-right">{t("quantity")}</div>
            <div className="col-span-2 text-right">{t("unitCost")}</div>
            <div className="col-span-1 text-right">{t("totalCost")}</div>
            <div className="col-span-1 text-center"></div>
          </div>

          {/* Table Rows */}
          <div className="divide-y divide-erp-border/60">
            {fields.map((field, cIdx) => {
              const currentItem = costComponents[cIdx] || field;
              const subtotal = calculateCostComponentSubtotal(
                currentItem.quantity,
                currentItem.unitCost
              );

              return (
                <div
                  key={field.id}
                  className="grid grid-cols-12 gap-2 items-center px-3 py-1.5 text-xs hover:bg-erp-surface-subtle/40 transition-colors"
                >
                  {/* Cost Type Select */}
                  <div className="col-span-12 sm:col-span-3">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.type`}
                      render={({ field: selectField }) => (
                        <Select
                          options={options.costTypes}
                          value={selectField.value}
                          onChange={selectField.onChange}
                          wrapperClassName="mb-0"
                          className="h-8 min-h-[32px] py-0.5 text-xs"
                        />
                      )}
                    />
                  </div>

                  {/* Description Input */}
                  <div className="col-span-12 sm:col-span-4">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.description`}
                      render={({ field: inputField }) => (
                        <Input
                          id={`cost-component-desc-${sectionIndex}-${itemIndex}-${cIdx}`}
                          value={inputField.value ?? ""}
                          placeholder={t("costDescPlaceholder")}
                          onChange={inputField.onChange}
                          wrapperClassName="mb-0"
                          className="h-8 min-h-[32px] py-0.5 text-xs"
                        />
                      )}
                    />
                  </div>

                  {/* Quantity Input */}
                  <div className="col-span-4 sm:col-span-1">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.quantity`}
                      render={({ field: qtyField }) => (
                        <Input
                          type="number"
                          step="any"
                          min="0"
                          placeholder="1"
                          value={qtyField.value ?? ""}
                          onChange={(e) => qtyField.onChange(Number(e.target.value))}
                          wrapperClassName="mb-0"
                          className="h-8 min-h-[32px] py-0.5 text-right font-mono text-xs"
                        />
                      )}
                    />
                  </div>

                  {/* Unit Cost Input */}
                  <div className="col-span-4 sm:col-span-2">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.unitCost`}
                      render={({ field: costField }) => (
                        <Input
                          id={`cost-component-unit-cost-${sectionIndex}-${itemIndex}-${cIdx}`}
                          type="number"
                          step="any"
                          min="0"
                          placeholder="0.00"
                          value={costField.value ?? ""}
                          onChange={(e) => costField.onChange(Number(e.target.value))}
                          wrapperClassName="mb-0"
                          className="h-8 min-h-[32px] py-0.5 text-right font-mono text-xs"
                        />
                      )}
                    />
                  </div>

                  {/* Line Subtotal */}
                  <div className="col-span-3 sm:col-span-1 text-right font-mono text-xs font-bold text-erp-navy truncate">
                    {formatFinancialNumber(subtotal)}
                  </div>

                  {/* Delete Action Button */}
                  <div className="col-span-1 sm:col-span-1 flex items-center justify-center">
                    <button
                      type="button"
                      onClick={() => handleRemoveCost(cIdx)}
                      className="w-7 h-7 text-erp-text-muted hover:text-rose-600 hover:bg-rose-50 transition-colors flex items-center justify-center"
                      title={t("removeWorkItem")}
                      aria-label={t("removeWorkItem")}
                    >
                      <IconTrash size={14} />
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Multi-tier Faceted Item Catalog Modal */}
      <EstimateItemCatalogModal
        isOpen={isCatalogOpen}
        onClose={() => setIsCatalogOpen(false)}
        onSelectItems={handleInsertFromCatalog}
        currency={currency}
        branchId={branchId}
      />

      <ConfirmDialog />
    </div>
  );
}
