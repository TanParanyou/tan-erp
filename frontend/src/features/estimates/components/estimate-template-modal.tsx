"use client";

import React, { useState, useMemo } from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Checkbox } from "@/components/ui/Checkbox";
import { Button } from "@/components/ui/Button";
import {
  ESTIMATE_TEMPLATES,
  type EstimateTemplateItem,
} from "../constants/estimate-templates";
import { calculateWorkItemSummary } from "../utils/estimate-calculations";
import {
  formatCurrencyAmount,
  formatFinancialNumber,
} from "../utils/estimate-formatters";

export interface EstimateTemplateModalProps {
  isOpen: boolean;
  onClose: () => void;
  onInsertTemplates: (selectedTemplates: EstimateTemplateItem[]) => void;
  sectionTitle?: string;
}

export function EstimateTemplateModal({
  isOpen,
  onClose,
  onInsertTemplates,
  sectionTitle,
}: EstimateTemplateModalProps) {
  const t = useTranslations("estimates");
  const tc = useTranslations("common");
  const [selectedIds, setSelectedIds] = useState<string[]>([]);

  // Reset selection when modal opens
  React.useEffect(() => {
    if (isOpen) {
      setSelectedIds([]);
    }
  }, [isOpen]);

  const toggleSelect = (id: string) => {
    setSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((item) => item !== id) : [...prev, id]
    );
  };

  const isAllSelected =
    selectedIds.length === ESTIMATE_TEMPLATES.length &&
    ESTIMATE_TEMPLATES.length > 0;

  const handleToggleAll = () => {
    if (isAllSelected) {
      setSelectedIds([]);
    } else {
      setSelectedIds(ESTIMATE_TEMPLATES.map((t) => t.id));
    }
  };

  const handleConfirm = () => {
    const selected = ESTIMATE_TEMPLATES.filter((tpl) =>
      selectedIds.includes(tpl.id)
    );
    if (selected.length > 0) {
      onInsertTemplates(selected);
      onClose();
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      size="xl"
      title={t("quickTemplates")}
      description={
        sectionTitle
          ? `${t("quickTemplatesDesc")} (${sectionTitle})`
          : t("quickTemplatesDesc")
      }
      footer={
        <div className="flex items-center justify-between w-full">
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleToggleAll}
              className="rounded-none text-xs"
            >
              {isAllSelected ? t("deselectAll") : t("selectAll")}
            </Button>
            <span className="text-xs text-erp-text-muted">
              {t("matchingCount", { count: selectedIds.length })}
            </span>
          </div>
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={onClose}
              className="rounded-none"
            >
              {tc("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              size="sm"
              disabled={selectedIds.length === 0}
              onClick={handleConfirm}
              className="rounded-none"
            >
              {t("insertTemplate")} ({selectedIds.length})
            </Button>
          </div>
        </div>
      }
    >
      <div className="space-y-3 max-h-[60vh] overflow-y-auto pr-1">
        {ESTIMATE_TEMPLATES.map((tpl) => {
          const isSelected = selectedIds.includes(tpl.id);
          const itemSummary = calculateWorkItemSummary({
            code: tpl.itemCode,
            descriptionTh: tpl.itemDescTh,
            descriptionEn: tpl.itemDescEn,
            quantity: tpl.quantity,
            unitCode: tpl.unitCode,
            sellingRuleType: tpl.sellingRule,
            sellingRuleValue: tpl.sellingRuleValue,
            sortOrder: 1,
            costComponents: tpl.costComponents.map((c, idx) => ({
              type: c.type,
              description: c.description,
              quantity: 1,
              unitCode: "lot",
              unitCost: c.unitCost,
              currency: "THB",
              sortOrder: idx + 1,
            })),
          });

          return (
            <div
              key={tpl.id}
              onClick={() => toggleSelect(tpl.id)}
              className={`p-3.5 border transition-colors cursor-pointer select-none ${
                isSelected
                  ? "border-erp-primary bg-erp-primary/5 shadow-sm"
                  : "border-border hover:border-erp-border-strong bg-erp-bg-surface"
              }`}
            >
              <div className="flex items-start justify-between gap-3">
                <div className="flex items-start gap-3">
                  <div className="pt-0.5" onClick={(e) => e.stopPropagation()}>
                    <Checkbox
                      checked={isSelected}
                      onChange={() => toggleSelect(tpl.id)}
                      aria-label={tpl.itemDescTh}
                    />
                  </div>
                  <div>
                    <div className="flex items-center gap-2 flex-wrap">
                      <span className="text-xs font-mono font-bold px-1.5 py-0.5 bg-erp-bg-neutral text-erp-text-secondary border border-border">
                        {tpl.itemCode}
                      </span>
                      <h4 className="text-sm font-semibold text-erp-text-primary">
                        {tpl.itemDescTh}
                      </h4>
                    </div>
                    <p className="text-xs text-erp-text-muted mt-0.5 font-mono">
                      {tpl.itemDescEn}
                    </p>
                  </div>
                </div>

                <div className="text-right shrink-0">
                  <div className="text-xs font-semibold text-erp-primary">
                    {formatCurrencyAmount(itemSummary.totalSellingPrice, "THB")}
                  </div>
                  <div className="text-[11px] text-erp-text-muted">
                    {t("totalCost")}: {formatCurrencyAmount(itemSummary.totalCost, "THB")}
                  </div>
                </div>
              </div>

              {/* Sub-components preview */}
              <div className="mt-2.5 pt-2 border-t border-border/60">
                <div className="text-[11px] font-medium text-erp-text-secondary mb-1">
                  {t("costComponents")} ({tpl.costComponents.length} {t("itemsCount", { count: tpl.costComponents.length })}):
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-1 text-[11px]">
                  {tpl.costComponents.map((c, idx) => (
                    <div
                      key={idx}
                      className="flex items-center justify-between text-erp-text-muted bg-erp-bg-canvas/50 px-2 py-0.5 border border-border/40 font-mono"
                    >
                      <span className="truncate pr-2">{c.description}</span>
                      <span className="font-semibold text-erp-text-secondary shrink-0">
                        {formatCurrencyAmount(c.unitCost, "THB")}
                      </span>
                    </div>
                  ))}
                </div>
                <div className="mt-2 flex items-center justify-between text-[11px] text-erp-text-muted">
                  <span>
                    {t("sellingRule")}:{" "}
                    <span className="font-medium text-erp-text-secondary">
                      {t(`sellingRuleTypes.${tpl.sellingRule}`)} {tpl.sellingRuleValue}%
                    </span>
                  </span>
                  <span>
                    {t("quantity")}: {tpl.quantity} {t(`units.${tpl.unitCode}`)}
                  </span>
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </Modal>
  );
}
