"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray, Controller } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { IconTrash, IconPlus, IconChevronDown } from "@/components/common/Icons";
import { useConfirm } from "@/hooks/useConfirm";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { calculateSectionSummary } from "../utils/estimate-calculations";
import {
  formatFinancialNumber,
  formatPercentRate,
} from "../utils/estimate-formatters";
import { EstimateWorkItemCard } from "./estimate-work-item-card";
import { EstimateTemplateModal } from "./estimate-template-modal";
import type { EstimateTemplateItem } from "../constants/estimate-templates";

interface EstimateSectionCardProps {
  sectionIndex: number;
  currency: string;
  isInitiallyExpanded?: boolean;
  branchId?: string;
  onRemoveSection: (sectionIndex: number) => void;
}

export function EstimateSectionCard({
  sectionIndex,
  currency,
  isInitiallyExpanded = true,
  branchId,
  onRemoveSection,
}: EstimateSectionCardProps) {
  const t = useTranslations("estimates");
  const { confirm, ConfirmDialog } = useConfirm();
  const [isExpanded, setIsExpanded] = useState(isInitiallyExpanded);
  const [isTemplateModalOpen, setIsTemplateModalOpen] = useState(false);

  const { control, watch, getValues } = useFormContext<EstimateWorkspaceFormData>();

  const currentSection = watch(`sections.${sectionIndex}`);
  const sectionSummary = calculateSectionSummary(currentSection || {});

  const { fields, append, remove, move } = useFieldArray({
    control,
    name: `sections.${sectionIndex}.workItems`,
  });

  const handleAddWorkItem = () => {
    const nextIdx = fields.length + 1;
    const secCode = currentSection?.code || `SEC-${sectionIndex + 1}`;

    append({
      code: `ITM-${secCode}-${nextIdx}`,
      descriptionTh: "",
      descriptionEn: "",
      quantity: 1,
      unitCode: "lot",
      sellingRuleType: "margin",
      sellingRuleValue: 25,
      sortOrder: nextIdx,
      costComponents: [
        {
          type: "material",
          description: t("defaultMaterial"),
          quantity: 1,
          unitCode: "lot",
          unitCost: 0,
          currency,
          sortOrder: 1,
        },
      ],
    });

    if (!isExpanded) {
      setIsExpanded(true);
    }
  };

  const handleInsertTemplates = (templates: EstimateTemplateItem[]) => {
    templates.forEach((tpl) => {
      const nextIdx = fields.length + 1;
      const secCode = currentSection?.code || `SEC-${sectionIndex + 1}`;

      append({
        code: `${tpl.itemCode}-${secCode}-${nextIdx}`,
        descriptionTh: tpl.itemDescTh,
        descriptionEn: tpl.itemDescEn,
        quantity: tpl.quantity,
        unitCode: tpl.unitCode,
        sellingRuleType: tpl.sellingRule,
        sellingRuleValue: tpl.sellingRuleValue,
        sortOrder: nextIdx,
        costComponents: tpl.costComponents.map((c, cIdx) => ({
          type: c.type,
          description: c.description,
          quantity: 1,
          unitCode: "lot",
          unitCost: c.unitCost,
          currency,
          sortOrder: cIdx + 1,
        })),
      });
    });

    if (!isExpanded) {
      setIsExpanded(true);
    }
  };

  const handleDuplicateItem = (itemIndex: number) => {
    const sourceItem = getValues(`sections.${sectionIndex}.workItems.${itemIndex}`);
    if (!sourceItem) return;

    const nextIdx = fields.length + 1;
    const secCode = currentSection?.code || `SEC-${sectionIndex + 1}`;

    append({
      ...sourceItem,
      id: null,
      code: `ITM-${secCode}-${nextIdx}`,
      descriptionTh: `${sourceItem.descriptionTh || ""} (Copy)`,
      descriptionEn: sourceItem.descriptionEn ? `${sourceItem.descriptionEn} (Copy)` : "",
      sortOrder: nextIdx,
      costComponents: (sourceItem.costComponents || []).map((c, cIdx) => ({
        ...c,
        id: null,
        sortOrder: cIdx + 1,
      })),
    });
  };

  const handleMoveUp = (itemIndex: number) => {
    if (itemIndex > 0) {
      move(itemIndex, itemIndex - 1);
    }
  };

  const handleMoveDown = (itemIndex: number) => {
    if (itemIndex < fields.length - 1) {
      move(itemIndex, itemIndex + 1);
    }
  };

  const handleRemoveItem = (itemIndex: number) => {
    remove(itemIndex);
  };

  const handleRequestRemoveSection = async () => {
    const secName = currentSection?.nameTh || currentSection?.code;
    const ok = await confirm({
      title: t("confirmDeleteSection"),
      message: secName ? `"${secName}"` : t("confirmDeleteSection"),
      confirmText: t("removeSection"),
      variant: "danger",
    });

    if (ok) {
      onRemoveSection(sectionIndex);
    }
  };

  return (
    <div className="bg-erp-surface border border-erp-border shadow-sm">
      {/* Section Header: Uniform 36px Height (h-9) across all elements */}
      <div className="bg-erp-surface-subtle p-3 border-b border-erp-border flex flex-wrap items-center justify-between gap-3">
        {/* Left Side: Chevron, Section Code, and Section Name in horizontal alignment */}
        <div className="flex items-center gap-2 flex-1 min-w-[300px]">
          {/* Accordion Expand/Collapse Button */}
          <button
            type="button"
            onClick={() => setIsExpanded((prev) => !prev)}
            className="w-9 h-9 text-erp-text-muted hover:text-erp-navy hover:bg-erp-surface transition-colors flex items-center justify-center border border-erp-border shrink-0"
            aria-label={isExpanded ? t("collapseAll") : t("expandAll")}
          >
            <span
              className={`inline-block transition-transform duration-150 ${
                isExpanded ? "rotate-0" : "-rotate-90"
              }`}
            >
              <IconChevronDown size={18} />
            </span>
          </button>

          {/* Section Code Input (Uniform 36px height) */}
          <div className="w-28 sm:w-36 shrink-0">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.code`}
              render={({ field }) => (
                <Input
                  placeholder={t("sectionCode")}
                  value={field.value ?? ""}
                  onChange={field.onChange}
                  wrapperClassName="mb-0"
                  className="font-mono text-xs h-9 min-h-[36px] py-1"
                />
              )}
            />
          </div>

          {/* Section Name TH Input (Uniform 36px height, fills remaining space) */}
          <div className="flex-1 min-w-[160px]">
            <Controller
              control={control}
              name={`sections.${sectionIndex}.nameTh`}
              render={({ field }) => (
                <Input
                  placeholder={t("defaultSectionNameTh", { number: sectionIndex + 1 })}
                  value={field.value ?? ""}
                  onChange={field.onChange}
                  wrapperClassName="mb-0"
                  className="text-xs h-9 min-h-[36px] py-1 font-medium"
                />
              )}
            />
          </div>
        </div>

        {/* Right Side: Financial Subtotal & Action Bar (All h-9 equal height) */}
        <div className="flex flex-wrap items-center gap-2">
          {/* Section Financial Subtotal Pill */}
          <div className="h-9 flex items-center gap-2 px-3 bg-erp-surface border border-erp-border text-xs font-mono">
            <span className="text-erp-text-muted font-sans hidden lg:inline">
              {t("sectionSubtotal")}:
            </span>
            <span className="font-bold text-erp-navy">
              {formatFinancialNumber(sectionSummary.totalSellingPrice)} {currency}
            </span>
            <span
              className={`text-[11px] px-1.5 py-0.2 border ${
                sectionSummary.marginRate >= 30
                  ? "bg-emerald-50 text-emerald-700 border-emerald-300"
                  : "bg-rose-50 text-rose-700 border-rose-300"
              }`}
              title={`GP: ${formatFinancialNumber(sectionSummary.grossProfit)} ${currency}`}
            >
              {formatPercentRate(sectionSummary.marginRate, 1)}
            </span>
          </div>

          {/* Insert Template Button */}
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => setIsTemplateModalOpen(true)}
            className="!rounded-none h-9 text-xs border-erp-border hover:bg-erp-surface text-erp-text-main"
            title={t("quickTemplatesDesc")}
          >
            <svg
              className="w-3.5 h-3.5 mr-1 text-erp-navy"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <rect x="3" y="3" width="18" height="18" rx="0" />
              <path d="M3 9h18" />
              <path d="M9 21V9" />
            </svg>
            {t("insertTemplate")}
          </Button>

          {/* Add Work Item Button */}
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={handleAddWorkItem}
            className="!rounded-none h-9 text-xs border-erp-navy text-erp-navy hover:bg-erp-surface"
          >
            <IconPlus size={14} className="mr-1" />
            {t("addWorkItem")}
          </Button>

          {/* Remove Section Button */}
          <button
            type="button"
            onClick={handleRequestRemoveSection}
            className="w-9 h-9 text-erp-text-muted hover:text-rose-600 hover:bg-rose-50 transition-colors flex items-center justify-center border border-erp-border shrink-0"
            title={t("removeSection")}
            aria-label={t("removeSection")}
          >
            <IconTrash size={16} />
          </button>
        </div>
      </div>

      {/* Section Work Items Body */}
      {isExpanded && (
        <div className="p-3 sm:p-4 space-y-4 bg-erp-canvas/40">
          {fields.length === 0 ? (
            <div className="text-center py-6 text-xs text-erp-text-muted border border-dashed border-erp-border bg-erp-surface">
              {t("addWorkItem")}
            </div>
          ) : (
            fields.map((field, wIdx) => (
              <EstimateWorkItemCard
                key={field.id}
                sectionIndex={sectionIndex}
                itemIndex={wIdx}
                currency={currency}
                branchId={branchId}
                isFirst={wIdx === 0}
                isLast={wIdx === fields.length - 1}
                onDuplicateItem={handleDuplicateItem}
                onRemoveItem={handleRemoveItem}
                onMoveUp={handleMoveUp}
                onMoveDown={handleMoveDown}
              />
            ))
          )}
        </div>
      )}

      {/* Template Selection Modal */}
      <EstimateTemplateModal
        isOpen={isTemplateModalOpen}
        onClose={() => setIsTemplateModalOpen(false)}
        onInsertTemplates={handleInsertTemplates}
        sectionTitle={currentSection?.nameTh || currentSection?.code}
      />

      <ConfirmDialog />
    </div>
  );
}
