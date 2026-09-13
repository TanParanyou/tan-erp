"use client";

import React from "react";
import { useFormContext, Controller } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { IconTrash, IconCopy } from "@/components/common/Icons";
import { useConfirm } from "@/hooks/useConfirm";
import { useEstimateOptions } from "../options/estimate-options";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { calculateWorkItemSummary } from "../utils/estimate-calculations";
import {
  formatFinancialNumber,
  formatSignedFinancialAmount,
  formatPercentRate,
} from "../utils/estimate-formatters";
import { EstimateCostComponentTable } from "./estimate-cost-component-table";

interface EstimateWorkItemCardProps {
  sectionIndex: number;
  itemIndex: number;
  currency: string;
  isFirst?: boolean;
  isLast?: boolean;
  onDuplicateItem: (itemIndex: number) => void;
  onRemoveItem: (itemIndex: number) => void;
  onMoveUp?: (itemIndex: number) => void;
  onMoveDown?: (itemIndex: number) => void;
}

export function EstimateWorkItemCard({
  sectionIndex,
  itemIndex,
  currency,
  isFirst = false,
  isLast = false,
  onDuplicateItem,
  onRemoveItem,
  onMoveUp,
  onMoveDown,
}: EstimateWorkItemCardProps) {
  const t = useTranslations("estimates");
  const { options } = useEstimateOptions();
  const { confirm, ConfirmDialog } = useConfirm();
  const { control, watch, setValue } = useFormContext<EstimateWorkspaceFormData>();

  const currentItem = watch(`sections.${sectionIndex}.workItems.${itemIndex}`);
  const itemCalc = calculateWorkItemSummary(currentItem || {});

  const handleRequestRemove = async () => {
    const itemDesc = currentItem?.descriptionTh || currentItem?.code;
    const ok = await confirm({
      title: t("confirmDeleteWorkItem"),
      message: itemDesc ? `"${itemDesc}"` : t("confirmDeleteWorkItem"),
      confirmText: t("removeWorkItem"),
      variant: "danger",
    });

    if (ok) {
      onRemoveItem(itemIndex);
    }
  };

  return (
    <div className="border border-erp-border bg-erp-surface shadow-sm">
      {/* 1. Work Item Header Bar: Identity & Actions in Perfect Alignment */}
      <div className="bg-erp-surface-subtle/80 px-4 py-2 border-b border-erp-border flex flex-wrap items-center justify-between gap-2">
        <div className="flex items-center gap-2">
          <span className="text-xs font-mono font-bold px-2 py-0.5 bg-erp-navy text-white">
            #{itemIndex + 1}
          </span>
          <span className="text-xs font-mono font-bold text-erp-navy">
            {currentItem?.code || `ITEM-${itemIndex + 1}`}
          </span>
          {currentItem?.descriptionTh && (
            <span className="text-xs text-erp-text-muted hidden md:inline truncate max-w-[280px]">
              — {currentItem.descriptionTh}
            </span>
          )}
        </div>

        {/* Action Controls: 32x32px Fixed Dimension Buttons */}
        <div className="flex items-center gap-1">
          {onMoveUp && (
            <button
              type="button"
              disabled={isFirst}
              onClick={() => onMoveUp(itemIndex)}
              className="w-8 h-8 text-erp-text-muted hover:text-erp-navy hover:bg-erp-surface disabled:opacity-25 disabled:cursor-not-allowed transition-colors flex items-center justify-center border border-erp-border"
              title={t("moveUp")}
              aria-label={t("moveUp")}
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
                <path d="M18 15l-6-6-6 6" />
              </svg>
            </button>
          )}
          {onMoveDown && (
            <button
              type="button"
              disabled={isLast}
              onClick={() => onMoveDown(itemIndex)}
              className="w-8 h-8 text-erp-text-muted hover:text-erp-navy hover:bg-erp-surface disabled:opacity-25 disabled:cursor-not-allowed transition-colors flex items-center justify-center border border-erp-border"
              title={t("moveDown")}
              aria-label={t("moveDown")}
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
                <path d="M6 9l6 6 6-6" />
              </svg>
            </button>
          )}

          <button
            type="button"
            onClick={() => onDuplicateItem(itemIndex)}
            className="w-8 h-8 text-erp-text-muted hover:text-erp-navy hover:bg-erp-surface transition-colors flex items-center justify-center border border-erp-border"
            title={t("duplicateWorkItem")}
            aria-label={t("duplicateWorkItem")}
          >
            <IconCopy size={15} />
          </button>

          <button
            type="button"
            onClick={handleRequestRemove}
            className="w-8 h-8 text-erp-text-muted hover:text-rose-600 hover:bg-rose-50 transition-colors flex items-center justify-center border border-erp-border"
            title={t("removeWorkItem")}
            aria-label={t("removeWorkItem")}
          >
            <IconTrash size={15} />
          </button>
        </div>
      </div>

      {/* 2. Form Fields Grid: Balanced 12-Column Grid */}
      <div className="p-4 space-y-3.5 bg-erp-surface">
        {/* Row 1: Code & Description TH/EN */}
        <div className="grid grid-cols-12 gap-3">
          <div className="col-span-12 sm:col-span-3">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.workItems.${itemIndex}.code`}
              render={({ field }) => (
                <Input
                  label={t("itemCode")}
                  value={field.value ?? ""}
                  onChange={field.onChange}
                  wrapperClassName="mb-0"
                  className="font-mono text-xs h-9 min-h-[36px]"
                />
              )}
            />
          </div>
          <div className="col-span-12 sm:col-span-9">
            <MultiLangInput
              label={t("itemDesc")}
              value={{
                th: currentItem?.descriptionTh ?? "",
                en: currentItem?.descriptionEn ?? "",
              }}
              onChange={(val) => {
                setValue(`sections.${sectionIndex}.workItems.${itemIndex}.descriptionTh`, val.th ?? "", {
                  shouldDirty: true,
                  shouldValidate: true,
                });
                setValue(`sections.${sectionIndex}.workItems.${itemIndex}.descriptionEn`, val.en ?? "", {
                  shouldDirty: true,
                  shouldValidate: true,
                });
              }}
              placeholder={{
                th: t("itemDescPlaceholder"),
                en: "e.g. Sliding Wardrobe H2400",
              }}
            />
          </div>
        </div>

        {/* Row 2: Quantity, Unit, Selling Rule, Rule Value */}
        <div className="grid grid-cols-12 gap-3">
          <div className="col-span-6 sm:col-span-3">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.workItems.${itemIndex}.quantity`}
              render={({ field }) => (
                <Input
                  label={t("quantity")}
                  type="number"
                  step="any"
                  min="0.001"
                  placeholder={t("quantityPlaceholder")}
                  value={field.value ?? ""}
                  onChange={(e) => field.onChange(Number(e.target.value))}
                  wrapperClassName="mb-0"
                  className="text-right font-mono text-xs h-9 min-h-[36px]"
                />
              )}
            />
          </div>

          <div className="col-span-6 sm:col-span-3">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.workItems.${itemIndex}.unitCode`}
              render={({ field }) => (
                <Input
                  label={t("unitCode")}
                  list={`units-list-${sectionIndex}-${itemIndex}`}
                  value={field.value ?? "lot"}
                  onChange={field.onChange}
                  wrapperClassName="mb-0"
                  className="font-mono text-xs h-9 min-h-[36px]"
                />
              )}
            />
            <datalist id={`units-list-${sectionIndex}-${itemIndex}`}>
              {options.units.map((u) => (
                <option key={u.value} value={u.value}>
                  {u.label}
                </option>
              ))}
            </datalist>
          </div>

          <div className="col-span-6 sm:col-span-3">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.workItems.${itemIndex}.sellingRuleType`}
              render={({ field }) => (
                <Select
                  label={t("sellingRule")}
                  options={options.sellingRuleTypes}
                  value={field.value ?? "margin"}
                  onChange={field.onChange}
                  wrapperClassName="mb-0"
                  className="text-xs h-9 min-h-[36px]"
                />
              )}
            />
          </div>

          <div className="col-span-6 sm:col-span-3">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.workItems.${itemIndex}.sellingRuleValue`}
              render={({ field }) => (
                <Input
                  label={t("sellingRuleValue")}
                  type="number"
                  step="any"
                  value={field.value ?? ""}
                  onChange={(e) => field.onChange(Number(e.target.value))}
                  wrapperClassName="mb-0"
                  className="text-right font-mono text-xs h-9 min-h-[36px]"
                />
              )}
            />
          </div>
        </div>

        {/* 3. Cost Components Breakdown Table */}
        <EstimateCostComponentTable
          sectionIndex={sectionIndex}
          itemIndex={itemIndex}
          currency={currency}
        />
      </div>

      {/* 4. Work Item Calculated Footer */}
      <div className="border-t border-erp-border flex flex-wrap justify-between items-center gap-3 text-xs font-mono bg-erp-surface-subtle/50 px-4 py-2.5">
        <div className="flex items-center gap-4 flex-wrap">
          <div>
            <span className="text-erp-text-muted font-sans">{t("totalCost")}: </span>
            <span className="font-bold text-erp-text-main">
              {formatFinancialNumber(itemCalc.totalCost)} {currency}
            </span>
          </div>
          <div>
            <span className="text-erp-text-muted font-sans">{t("unitSellingPrice")}: </span>
            <span className="font-bold text-erp-navy">
              {formatFinancialNumber(itemCalc.unitSellingPrice)} {currency}
            </span>
          </div>
        </div>

        <div className="flex items-center gap-4 flex-wrap">
          <div>
            <span className="text-erp-text-muted font-sans">{t("totalSellingPrice")}: </span>
            <span className="font-bold text-erp-navy text-sm">
              {formatFinancialNumber(itemCalc.totalSellingPrice)} {currency}
            </span>
          </div>
          <div className="flex items-center gap-1.5">
            <span className="text-erp-text-muted font-sans">{t("grossProfit")}: </span>
            <span
              className={`font-bold ${
                itemCalc.grossProfit >= 0 ? "text-emerald-700" : "text-rose-600"
              }`}
            >
              {formatSignedFinancialAmount(itemCalc.grossProfit)} {currency}
            </span>
            <span
              className={`text-[11px] px-1.5 py-0.5 border font-mono ${
                itemCalc.marginRate >= 30
                  ? "bg-emerald-50 text-emerald-700 border-emerald-300"
                  : "bg-rose-50 text-rose-700 border-rose-300"
              }`}
            >
              {formatPercentRate(itemCalc.marginRate, 1)}
            </span>
          </div>
        </div>
      </div>

      <ConfirmDialog />
    </div>
  );
}
