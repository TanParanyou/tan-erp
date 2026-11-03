"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray, Controller, useWatch } from "react-hook-form";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Badge } from "@/components/ui/Badge";
import { Tooltip } from "@/components/ui/Tooltip";
import { IconTrash, IconPlus, IconList } from "@/components/common/Icons";
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
import { EstimateCostUnitSelect } from "./estimate-cost-unit-select";

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

  const totalOfLines = costComponents.reduce((sum, line) => sum + calculateCostComponentSubtotal(line.quantity, line.unitCost), 0);
  const openCatalog = () => {
    if (workspace) {
      const section = getValues(`sections.${sectionIndex}`);
      const work = section.workItems[itemIndex];
      if (section.uiKey && work.uiKey) workspace.openCatalog({ sectionKey: section.uiKey, workKey: work.uiKey });
    } else setIsCatalogOpen(true);
  };

  return (
    <div className="bg-erp-surface">
      {/* Summary + the two ways to add a line; the primary action is the blank line */}
      <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
        <div className="min-w-0">
          <div className="text-xs text-erp-text-muted">{t("costComponents")} ({fields.length})</div>
          <div className="font-mono text-base font-bold text-erp-navy">{formatFinancialNumber(totalOfLines)} <span className="text-xs font-normal text-erp-text-muted">{currency}</span></div>
          <div className="text-[11px] text-erp-text-muted">{t("costLinesTotal")}</div>
        </div>
        <div className="flex flex-wrap items-center gap-2">
          <Button size="sm" variant="outline" type="button" onClick={openCatalog} title={t("catalogModalDesc")} icon={<IconList size={14} />}>{t("browseCatalog")}</Button>
          <Button size="sm" variant="primary" type="button" onClick={handleAddCost} icon={<IconPlus size={14} />}>{t("addComponent")}</Button>
        </div>
      </div>

      <Select
        id={`cost-quick-add-${sectionIndex}-${itemIndex}`}
        aria-label={t("quickAddCostTitle")}
        placeholder={t("quickAddCostTitle")}
        options={QUICK_COST_PRESETS.map((preset) => ({ value: preset.id, label: `${t(preset.labelKey)} (@ ${preset.defaultCost} ${currency})` }))}
        value=""
        onChange={(event) => {
          const preset = QUICK_COST_PRESETS.find((candidate) => candidate.id === event.target.value);
          if (preset) handleQuickAddCost(preset);
        }}
        wrapperClassName="mb-3"
      />

      {fields.length === 0 ? (
        <div className="border border-dashed border-erp-border bg-erp-surface-subtle p-5 text-center text-xs text-erp-text-muted">
          {t("addComponent")}
        </div>
      ) : (
        <div className="space-y-3">
          {fields.map((field, cIdx) => {
            const currentItem = costComponents[cIdx];
            if (!currentItem) return null;
            const subtotal = calculateCostComponentSubtotal(currentItem.quantity, currentItem.unitCost);
            const base = `sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${cIdx}` as const;
            const lineErrors = errors.sections?.[sectionIndex]?.workItems?.[itemIndex]?.costComponents?.[cIdx];
            const isProvisional = currentItem.isProvisional !== false;

            return (
              <div
                key={field.id}
                id={currentItem.id ? `estimate-target-${currentItem.id}` : undefined}
                tabIndex={-1}
                className="border border-erp-border text-xs focus-visible:outline-2 focus-visible:outline-erp-navy"
              >
                <div className="flex items-center justify-between gap-2 border-b border-erp-border bg-erp-surface-subtle px-2 py-1">
                  <div className="flex min-w-0 items-center gap-2">
                    <span className="font-semibold text-erp-text-main">{t("costLineNumber", { number: cIdx + 1 })}</span>
                    <Badge variant={isProvisional ? "warning" : "success"}>{isProvisional ? t("costBadgeEstimated") : t("costBadgeConfirmed")}</Badge>
                  </div>
                  <Tooltip content={t("removeCostLine")} position="bottom">
                    <Button size="icon" variant="ghost" type="button" onClick={() => handleRemoveCost(cIdx)} aria-label={t("removeCostLine")} icon={<IconTrash size={16} />} />
                  </Tooltip>
                </div>

                <div className="grid grid-cols-2 gap-2 p-2">
                  <div className="col-span-2">
                    <Controller
                      control={control}
                      name={`${base}.description`}
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

                  <div className={currentItem.itemId ? "col-span-2" : "col-span-1"}>
                    <Controller
                      control={control}
                      name={`${base}.type`}
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
                  {!currentItem.itemId && (
                    <div className="col-span-1">
                      <Controller
                        control={control}
                        name={`${base}.unitCode`}
                        render={({ field: unitField }) => (
                          <Input
                            id={`cost-unit-code-${sectionIndex}-${itemIndex}-${cIdx}`}
                            label={t("unitCode")}
                            placeholder={t("unitCodePlaceholder")}
                            list={`cost-units-${sectionIndex}-${itemIndex}`}
                            value={unitField.value ?? ""}
                            onChange={unitField.onChange}
                            wrapperClassName="mb-0"
                          />
                        )}
                      />
                    </div>
                  )}

                  <EstimateCostUnitSelect sectionIndex={sectionIndex} itemIndex={itemIndex} costIndex={cIdx} branchId={branchId} />

                  <div className="col-span-1">
                    <Controller
                      control={control}
                      name={`${base}.quantity`}
                      render={({ field: qtyField }) => (
                        <Input
                          type="number"
                          step="any"
                          min="0"
                          id={`cost-quantity-${sectionIndex}-${itemIndex}-${cIdx}`}
                          label={t("quantity")}
                          placeholder={t("quantityPlaceholder")}
                          error={lineErrors?.quantity ? t("workspace.costInvalid") : undefined}
                          value={qtyField.value ?? ""}
                          onChange={(e) => qtyField.onChange(Number(e.target.value))}
                          wrapperClassName="mb-0"
                          className="text-right font-mono"
                        />
                      )}
                    />
                  </div>
                  <div className="col-span-1">
                    <Controller
                      control={control}
                      name={`${base}.unitCost`}
                      render={({ field: costField }) => (
                        <Input
                          id={`cost-component-unit-cost-${sectionIndex}-${itemIndex}-${cIdx}`}
                          type="number"
                          step="any"
                          min="0"
                          label={t("unitCost")}
                          placeholder={t("unitCostPlaceholder")}
                          error={lineErrors?.unitCost ? t("workspace.costInvalid") : undefined}
                          value={costField.value ?? ""}
                          onChange={(e) => costField.onChange(Number(e.target.value))}
                          wrapperClassName="mb-0"
                          className="text-right font-mono"
                        />
                      )}
                    />
                  </div>

                  <div className="col-span-2 flex flex-wrap items-baseline justify-end gap-x-2 border-t border-erp-border pt-2 font-mono">
                    <span className="text-erp-text-muted">{formatFinancialNumber(currentItem.quantity)} {currentItem.unitCode} × {formatFinancialNumber(currentItem.unitCost)} =</span>
                    <span className="text-sm font-bold text-erp-navy">{formatFinancialNumber(subtotal)} {currency}</span>
                  </div>

                  {isProvisional && (
                    <div className="col-span-2 grid grid-cols-1 gap-2 border border-erp-warning-border bg-erp-warning-bg p-2">
                      <p className="text-erp-warning-text">{t("provisionalHint")}</p>
                      <Controller
                        control={control}
                        name={`${base}.provisionalReasonCode`}
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
                        name={`${base}.provisionalNote`}
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
              </div>
            );
          })}
        </div>
      )}
      <datalist id={`cost-units-${sectionIndex}-${itemIndex}`}>{options.units.map((unit) => <option key={unit.value} value={unit.value}>{unit.label}</option>)}</datalist>

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
