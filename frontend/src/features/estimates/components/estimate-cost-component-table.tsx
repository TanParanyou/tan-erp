"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray, Controller, useWatch } from "react-hook-form";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { IconTrash, IconPlus } from "@/components/common/Icons";
import { useConfirm } from "@/hooks/useConfirm";
import { useEstimateOptions } from "../options/estimate-options";
import { QUICK_COST_PRESETS, type QuickCostPreset } from "../constants/estimate-templates";
import {
  type CatalogItemModel,
} from "../api/estimate-catalog-client";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { calculateCostComponentSubtotal } from "../utils/estimate-calculations";
import { formatFinancialNumber } from "../utils/estimate-formatters";
import { Button } from "@/components/ui/Button";
import { useEstimateWorkspace } from "./estimate-workspace-context";
import { catalogCostComponents } from "../utils/estimate-workspace-mapper";
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
  const { control, getValues, formState: { errors } } = useFormContext<EstimateWorkspaceFormData>();
  const workspace = useEstimateWorkspace();
  const [isCatalogOpen, setIsCatalogOpen] = useState(false);

  const fieldArrayName = `sections.${sectionIndex}.workItems.${itemIndex}.costComponents` as const;

  const { fields, append, remove } = useFieldArray({
    control,
    name: fieldArrayName,
  });

  const costComponents = useWatch({ control, name: fieldArrayName }) ?? [];

  const handleAddCost = () => {
    append({
      type: "material",
      description: "",
      quantity: 1,
      unitCode: "lot",
      unitCost: 0,
      currency,
      sortOrder: fields.length + 1,
      isProvisional: true,
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
      isProvisional: true,
    });
  };

  const handleInsertFromCatalog = (selectedItems: CatalogItemModel[]) => {
    append(catalogCostComponents(selectedItems, locale, fields.length));
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
    <div className="bg-erp-surface">
      {/* 1. Header & Add Actions */}
      <div className="flex flex-wrap justify-between items-center mb-2.5 gap-2">
        <span className="text-xs font-bold text-erp-navy flex items-center gap-1.5">
          {t("costComponents")} ({fields.length})
        </span>

        <div className="flex items-center gap-2">
          {/* Browse Catalog Button */}
          <Button size="sm" variant="outline"
            type="button"
            onClick={() => {
              if (workspace) {
                const section = getValues(`sections.${sectionIndex}`);
                const work = section.workItems[itemIndex];
                if (section.uiKey && work.uiKey) workspace.openCatalog({ sectionKey: section.uiKey, workKey: work.uiKey });
              } else setIsCatalogOpen(true);
            }}
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
          </Button>

          {/* Add Blank Cost Button */}
          <Button size="sm" variant="outline"
            type="button"
            onClick={handleAddCost}
          >
            <IconPlus size={13} />
            {t("addComponent")}
          </Button>
        </div>
      </div>

      {/* 2. Quick Cost Chips */}
      <div className="flex flex-wrap items-center gap-1.5 mb-3 pb-2.5 border-b border-border/60">
        <span className="text-xs font-semibold text-erp-text-muted mr-1">
          {t("quickAddCostTitle")}
        </span>
        {QUICK_COST_PRESETS.map((preset) => (
          <Button size="sm" variant="outline"
            key={preset.id}
            type="button"
            onClick={() => handleQuickAddCost(preset)}
            title={`${preset.defaultDesc} (@ ${preset.defaultCost} ${currency})`}
          >
            <span className="text-erp-text-muted hover:text-white">+</span>
            <span>{t(preset.labelKey)}</span>
          </Button>
        ))}
      </div>

      {/* 3. Cost Components Table */}
      {fields.length === 0 ? (
        <div className="text-center py-5 text-xs text-erp-text-muted border border-dashed border-erp-border bg-erp-surface-subtle/50">
          {t("addComponent")}
        </div>
      ) : (
        <div className="border border-erp-border">
          {/* Component forms */}
          <div className="divide-y divide-erp-border/60">
            {fields.map((field, cIdx) => {
              const currentItem = costComponents[cIdx];
              if (!currentItem) return null;
              const subtotal = calculateCostComponentSubtotal(
                currentItem.quantity,
                currentItem.unitCost
              );

              return (
                <div
                  key={field.id}
                  id={currentItem.id ? `estimate-target-${currentItem.id}` : undefined}
                  tabIndex={-1}
                  className="grid grid-cols-2 gap-2 items-start p-2 text-xs focus-visible:outline-2 focus-visible:outline-erp-navy"
                >
                  {/* Cost Type Select */}
                  <div className="col-span-2">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.type`}
                      render={({ field: selectField }) => (
                        <Select
                          id={`cost-type-${sectionIndex}-${itemIndex}-${cIdx}`}
                          label={t("costType")}
                          options={options.costTypes}
                          value={selectField.value}
                          onChange={selectField.onChange}
                          wrapperClassName="mb-0"
                        />
                      )}
                    />
                  </div>

                  {/* Description Input */}
                  <div className="col-span-2">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.description`}
                      render={({ field: inputField }) => (
                        <Input
                          label={t("costDesc")}
                          id={`cost-component-desc-${sectionIndex}-${itemIndex}-${cIdx}`}
                          value={inputField.value ?? ""}
                          placeholder={t("costDescPlaceholder")}
                          onChange={inputField.onChange}
                          wrapperClassName="mb-0"
                        />
                      )}
                    />
                  </div>

                  {/* Quantity Input */}
                  <div className="col-span-1">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.quantity`}
                      render={({ field: qtyField }) => (
                        <Input
                          type="number"
                          step="any"
                          min="0"
                          id={`cost-quantity-${sectionIndex}-${itemIndex}-${cIdx}`}
                          label={t("quantity")}
                          error={errors.sections?.[sectionIndex]?.workItems?.[itemIndex]?.costComponents?.[cIdx]?.quantity ? t("workspace.costInvalid") : undefined}
                          value={qtyField.value ?? ""}
                          onChange={(e) => qtyField.onChange(Number(e.target.value))}
                          wrapperClassName="mb-0"
                          className="text-right font-mono"
                        />
                      )}
                    />
                  </div>

                  {/* Unit Cost Input */}
                  <div className="col-span-1">
                    <Controller
                      control={control}
                      name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.unitCost`}
                      render={({ field: costField }) => (
                        <Input
                          id={`cost-component-unit-cost-${sectionIndex}-${itemIndex}-${cIdx}`}
                          type="number"
                          step="any"
                          min="0"
                          label={t("unitCost")}
                          error={errors.sections?.[sectionIndex]?.workItems?.[itemIndex]?.costComponents?.[cIdx]?.unitCost ? t("workspace.costInvalid") : undefined}
                          value={costField.value ?? ""}
                          onChange={(e) => costField.onChange(Number(e.target.value))}
                          wrapperClassName="mb-0"
                          className="text-right font-mono"
                        />
                      )}
                    />
                  </div>

                  {/* Line Subtotal */}
                  <div className="col-span-1 text-right font-mono text-xs font-bold text-erp-navy truncate">
                    {formatFinancialNumber(subtotal)}
                  </div>

                  {/* Delete Action Button */}
                  <div className="col-span-1 flex items-center justify-center">
                    <Button size="sm" variant="outline"
                      type="button"
                      onClick={() => handleRemoveCost(cIdx)}
                      className="text-erp-text-muted"
                      title={t("removeWorkItem")}
                      aria-label={t("removeWorkItem")}
                    >
                      <IconTrash size={14} />
                    </Button>
                  </div>
                  {currentItem.isProvisional !== false && (
                    <div className="col-span-2 grid grid-cols-1 gap-2 border-t border-erp-border/40 pt-2">
                      <Controller
                        control={control}
                        name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.provisionalReasonCode`}
                        render={({ field: reasonField }) => (
                          <Select
                            options={[
                              { value: "supplier-quote-pending", label: t("provisionalReasons.supplierQuotePending") },
                              { value: "market-benchmark", label: t("provisionalReasons.marketBenchmark") },
                              { value: "historical-reference", label: t("provisionalReasons.historicalReference") },
                              { value: "engineering-allowance", label: t("provisionalReasons.engineeringAllowance") },
                              { value: "other", label: t("provisionalReasons.other") },
                            ]}
                            value={reasonField.value ?? ""}
                            onChange={reasonField.onChange}
                            placeholder={t("provisionalReasonCode")}
                            required
                            wrapperClassName="mb-0"
                            aria-label={t("provisionalReasonCode")}
                          />
                        )}
                      />
                      <Controller
                        control={control}
                        name={`sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}.provisionalNote`}
                        render={({ field: noteField }) => (
                          <Input
                            value={noteField.value ?? ""}
                            placeholder={t("provisionalNote")}
                            onChange={noteField.onChange}
                            wrapperClassName="mb-0"
                            aria-label={t("provisionalNote")}
                          />
                        )}
                      />
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Multi-tier Faceted Item Catalog Modal */}
      {!workspace && <EstimateItemCatalogModal
        isOpen={isCatalogOpen}
        onClose={() => setIsCatalogOpen(false)}
        onSelectItems={handleInsertFromCatalog}
        currency={currency}
        branchId={branchId}
      />}

      <ConfirmDialog />
    </div>
  );
}
