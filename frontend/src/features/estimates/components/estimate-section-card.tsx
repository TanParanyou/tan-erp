"use client";

import React, { useEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { useFormContext, useFieldArray, Controller, useWatch } from "react-hook-form";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
  TableCaption,
} from "@/components/ui/Table";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { IconTrash, IconPlus, IconChevronDown } from "@/components/common/Icons";
import { useConfirm } from "@/hooks/useConfirm";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { calculateSectionSummary, calculateWorkItemSummary } from "../utils/estimate-calculations";
import { formatFinancialNumber, formatPercentRate } from "../utils/estimate-formatters";
import { EstimateWorkItemCard } from "./estimate-work-item-card";
import { EstimateTemplateModal } from "./estimate-template-modal";
import type { EstimateTemplateItem } from "../constants/estimate-templates";
import { useEstimateWorkspace, scrollEstimateTarget } from "./estimate-workspace-context";
import { workspaceWorkMatches } from "../utils/estimate-workspace-filter";

interface EstimateSectionCardProps {
  sectionIndex: number;
  currency: string;
  isInitiallyExpanded?: boolean;
  branchId?: string;
  onRemoveSection: (sectionIndex: number) => void;
}

export function EstimateSectionCard({ sectionIndex, currency, isInitiallyExpanded = true, branchId, onRemoveSection }: EstimateSectionCardProps) {
  const t = useTranslations("estimates");
  const { confirm, ConfirmDialog } = useConfirm();
  const workspace = useEstimateWorkspace();
  const [isExpanded, setIsExpanded] = useState(isInitiallyExpanded);
  const [isTemplateModalOpen, setIsTemplateModalOpen] = useState(false);
  const { control, getValues, setValue, formState: { errors } } = useFormContext<EstimateWorkspaceFormData>();
  const currentSection = useWatch({ control, name: `sections.${sectionIndex}` });
  const sectionSummary = calculateSectionSummary(currentSection ?? {});
  const { fields, append, remove, move } = useFieldArray({ control, name: `sections.${sectionIndex}.workItems` });
  const focusHandled = useRef<string | undefined>(undefined);
  const focusTargetId = workspace?.focusTargetId;

  useEffect(() => {
    if (!focusTargetId) { focusHandled.current = undefined; return; }
    if (focusHandled.current === focusTargetId || !workspace) return;
    const work = currentSection?.workItems.find((item) => item.id === focusTargetId || item.costComponents.some((cost) => cost.id === focusTargetId));
    if (currentSection?.id !== focusTargetId && !work) return;
    focusHandled.current = focusTargetId;
    setIsExpanded(true);
    if (work?.uiKey) workspace.selectWork(work.uiKey);
    requestAnimationFrame(() => {
      const target = document.getElementById(`estimate-target-${focusTargetId}`);
      if (target) scrollEstimateTarget(target);
      target?.focus({ preventScroll: true });
    });
  }, [focusTargetId, currentSection, workspace]);

  useEffect(() => {
    if (errors.sections?.[sectionIndex]) setIsExpanded(true);
  }, [errors.sections, sectionIndex]);

  function handleAddWorkItem() {
    const next = fields.length + 1;
    const uiKey = crypto.randomUUID();
    append({
      uiKey, code: `ITM-${currentSection.code}-${next}`, descriptionTh: "", descriptionEn: "", quantity: 1,
      itemId: null, overrideReasonCode: "", overrideReason: "",
      unitCode: "lot", sellingRuleType: "margin", sellingRuleValue: 25, sortOrder: next,
      costComponents: [{ type: "material", description: t("defaultMaterial"), quantity: 1, unitCode: "lot", unitCost: 0, currency, sortOrder: 1, isProvisional: true }],
    });
    setIsExpanded(true); workspace?.selectWork(uiKey);
  }
  function handleInsertTemplates(templates: EstimateTemplateItem[]) {
    const additions = templates.map((template, offset) => ({
      uiKey: crypto.randomUUID(), code: `${template.itemCode}-${currentSection.code}-${fields.length + offset + 1}`,
      descriptionTh: template.itemDescTh, descriptionEn: template.itemDescEn, quantity: template.quantity,
      unitCode: template.unitCode, sellingRuleType: template.sellingRule, sellingRuleValue: template.sellingRuleValue,
      sortOrder: fields.length + offset + 1,
      costComponents: template.costComponents.map((cost, c) => ({ type: cost.type, description: cost.description, quantity: 1, unitCode: "lot", unitCost: cost.unitCost, currency, sortOrder: c + 1, isProvisional: true })),
    }));
    append(additions); setIsExpanded(true);
    if (additions[0]) workspace?.selectWork(additions[0].uiKey);
  }
  function handleDuplicateItem(index: number) {
    const source = getValues(`sections.${sectionIndex}.workItems.${index}`);
    const uiKey = crypto.randomUUID();
    append({ ...source, id: null, uiKey, code: `ITM-${currentSection.code}-${fields.length + 1}`,
      descriptionTh: t("workspace.copyDescription", { description: source.descriptionTh }),
      descriptionEn: source.descriptionEn ? t("workspace.copyDescriptionEn", { description: source.descriptionEn }) : "",
      sortOrder: fields.length + 1, costComponents: source.costComponents.map((cost, c) => ({ ...cost, id: null, sortOrder: c + 1 })),
    });
    setIsExpanded(true); workspace?.selectWork(uiKey);
  }
  function handleRemoveItem(index: number) {
    const work = getValues(`sections.${sectionIndex}.workItems.${index}`);
    if (workspace && workspace.selectedKey === work.uiKey) {
      const next = currentSection.workItems[index + 1] ?? currentSection.workItems[index - 1];
      if (next?.uiKey) workspace.selectWork(next.uiKey);
    }
    remove(index);
  }
  async function handleRequestRemoveSection() {
    if (await confirm({ title: t("confirmDeleteSection"), message: currentSection.nameTh, confirmText: t("removeSection"), variant: "danger" })) onRemoveSection(sectionIndex);
  }
  if (!currentSection) return null;
  const visible = currentSection.workItems.map((work, index) => ({ work, index })).filter(({ work }) => workspaceWorkMatches(currentSection, work, workspace?.searchQuery ?? "", workspace?.filterMode ?? "all"));
  const query = workspace?.searchQuery.trim().toLowerCase() ?? "";
  const sectionMatches = [currentSection.code, currentSection.nameTh, currentSection.nameEn].some((value) => value.toLowerCase().includes(query));
  const sectionVisible = visible.length > 0 || ((workspace?.filterMode ?? "all") === "all" && sectionMatches);
  const cards = fields.map((field, index) => (
    <div key={field.id} className={workspace && workspace.selectedKey !== field.uiKey ? "hidden" : ""}>
      <EstimateWorkItemCard sectionIndex={sectionIndex} itemIndex={index} currency={currency} branchId={branchId}
        isFirst={index === 0} isLast={index === fields.length - 1} onDuplicateItem={handleDuplicateItem} onRemoveItem={handleRemoveItem}
        onMoveUp={(i) => move(i, i - 1)} onMoveDown={(i) => move(i, i + 1)} />
    </div>
  ));

  return <>
    <section id={currentSection.id ? `estimate-target-${currentSection.id}` : undefined} tabIndex={-1} aria-label={currentSection.code}
      className={cn("border border-erp-border bg-erp-surface focus-visible:outline-2 focus-visible:outline-erp-navy", !sectionVisible && "hidden")}>
      <div className="flex flex-col gap-2 border-b border-erp-border bg-erp-surface-subtle p-2">
        <div className="flex min-w-0 flex-wrap items-start gap-2 sm:flex-nowrap">
          <Button variant="ghost" size="icon" onClick={() => setIsExpanded((value) => !value)} aria-expanded={isExpanded} aria-label={isExpanded ? t("collapseAll") : t("expandAll")} icon={<IconChevronDown size={18} className={isExpanded ? "" : "-rotate-90"} />} />
          <div className="w-24 shrink-0"><Controller control={control} name={`sections.${sectionIndex}.code`} render={({ field, fieldState }) => <Input {...field} id={`estimate-section-code-${sectionIndex}`} aria-label={t("sectionCode")} placeholder={t("sectionCode")} error={fieldState.error ? t("workspace.codeRequired") : undefined} wrapperClassName="mb-0" className="font-mono" />} /></div>
          <div className="w-full min-w-0 sm:w-auto sm:flex-1 [&>.erp-form-group]:mb-0"><MultiLangInput id={`section-name-${sectionIndex}`} label={t("workspace.sectionName")} value={{ th: currentSection.nameTh, en: currentSection.nameEn }} onChange={(value) => { setValue(`sections.${sectionIndex}.nameTh`, value.th ?? "", { shouldDirty: true }); setValue(`sections.${sectionIndex}.nameEn`, value.en ?? "", { shouldDirty: true }); }} /></div>
        </div>
        <div className="flex min-w-0 flex-wrap items-center gap-2">
          <span className="mr-auto break-words text-xs font-bold font-mono text-erp-navy" title={t("sectionSubtotal")}>{formatFinancialNumber(sectionSummary.totalSellingPrice)} {currency} <span className="text-xs">({formatPercentRate(sectionSummary.marginRate, 1)})</span></span>
          <Button size="sm" variant="outline" className="px-3 text-xs sm:text-sm" onClick={() => setIsTemplateModalOpen(true)} title={t("quickTemplatesDesc")}>{t("insertTemplate")}</Button>
          <Button size="sm" variant="outline" className="px-3 text-xs sm:text-sm" onClick={handleAddWorkItem} icon={<IconPlus size={16} />}>{t("addWorkItem")}</Button>
          <Button variant="ghost" size="icon" onClick={handleRequestRemoveSection} aria-label={t("removeSection")} icon={<IconTrash size={16} />} />
        </div>
      </div>
      <div className={isExpanded ? "overflow-auto" : "hidden"}>
        {fields.length === 0 ? (
          <p className="p-5 text-center text-sm text-erp-text-muted">{t("addWorkItem")}</p>
        ) : (
          <Table wrapperClassName="min-w-[620px]" className="text-xs">
            <TableCaption className="sr-only">
              {t("workspace.boq")}: {currentSection.nameTh}
            </TableCaption>
            <TableHeader className="bg-erp-surface-subtle text-xs text-erp-text-muted">
              <TableRow>
                <TableHead className="px-2 py-2 text-left">{t("itemCodeAndDesc")}</TableHead>
                <TableHead className="px-2 py-2 text-right">{t("quantity")}</TableHead>
                <TableHead className="px-2 py-2 text-right">{t("totalCost")}</TableHead>
                <TableHead className="px-2 py-2 text-right">{t("totalSellingPrice")}</TableHead>
                <TableHead className="px-2 py-2 text-right">{t("grossProfit")}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {visible.map(({ work, index }) => {
                const summary = calculateWorkItemSummary(work);
                const selected = workspace?.selectedKey === work.uiKey;
                return (
                  <TableRow
                    key={work.uiKey}
                    aria-selected={selected}
                    className={cn("border-t border-erp-border", selected && "bg-erp-surface-subtle")}
                  >
                    <TableCell
                      className={cn(
                        "sticky left-0 px-2 py-1 border-l-4",
                        selected ? "border-l-erp-navy bg-erp-surface-subtle" : "border-l-transparent bg-erp-surface"
                      )}
                    >
                      <Button
                        variant="ghost"
                        className="w-full justify-start px-2 text-xs text-left whitespace-normal"
                        onClick={() => {
                          if (work.uiKey) workspace?.selectWork(work.uiKey);
                        }}
                        aria-label={t("workspace.selectWorkCode", { code: work.code })}
                      >
                        <span className="block">
                          <span className="block text-xs font-mono text-erp-text-muted">{work.code}</span>
                          <span className="block font-bold">{work.descriptionTh || "-"}</span>
                        </span>
                      </Button>
                    </TableCell>
                    <TableCell className="px-2 py-2 text-right whitespace-nowrap font-mono">
                      {formatFinancialNumber(work.quantity)} <span className="font-sans text-xs">{work.unitCode}</span>
                    </TableCell>
                    <TableCell className="px-2 py-2 text-right font-mono">{formatFinancialNumber(summary.totalCost)}</TableCell>
                    <TableCell className="px-2 py-2 text-right font-mono font-bold text-erp-navy">
                      {formatFinancialNumber(summary.totalSellingPrice)}
                    </TableCell>
                    <TableCell className="px-2 py-2 text-right font-mono">
                      <span className={summary.marginRate < 30 ? "text-erp-danger" : "text-erp-success"}>
                        {formatPercentRate(summary.marginRate, 1)}
                      </span>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        )}
      </div>
      {!workspace && cards}
    </section>
    {workspace?.inspector && createPortal(cards, workspace.inspector)}
    <EstimateTemplateModal isOpen={isTemplateModalOpen} onClose={() => setIsTemplateModalOpen(false)} onInsertTemplates={handleInsertTemplates} sectionTitle={currentSection.nameTh} />
    <ConfirmDialog />
  </>;
}
