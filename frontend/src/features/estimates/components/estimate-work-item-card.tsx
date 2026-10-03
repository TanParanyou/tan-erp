"use client";

import React, { useEffect, useState } from "react";
import { useFormContext, Controller, useWatch, type FieldErrors } from "react-hook-form";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { FormSection } from "@/components/forms/FormSection";
import { FormTabs, useFormTabErrors } from "@/components/forms/FormTabs";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { IconTrash, IconCopy } from "@/components/common/Icons";
import { useConfirm } from "@/hooks/useConfirm";
import { useEstimateOptions } from "../options/estimate-options";
import type { EstimateWorkspaceFormData, WorkItemFormData } from "../schemas/estimate-workspace-schema";
import { calculateWorkItemSummary } from "../utils/estimate-calculations";
import { formatFinancialNumber, formatSignedFinancialAmount, formatPercentRate } from "../utils/estimate-formatters";
import { EstimateCostComponentTable } from "./estimate-cost-component-table";
import { useEstimateWorkspace } from "./estimate-workspace-context";
import { workspaceWorkMatches } from "../utils/estimate-workspace-filter";

type DetailTab = "info" | "cost" | "pricing";
interface EstimateWorkItemCardProps {
  sectionIndex: number; itemIndex: number; currency: string; branchId?: string;
  isFirst?: boolean; isLast?: boolean;
  onDuplicateItem: (index: number) => void; onRemoveItem: (index: number) => void;
  onMoveUp?: (index: number) => void; onMoveDown?: (index: number) => void;
}

export function EstimateWorkItemCard({ sectionIndex, itemIndex, currency, branchId, isFirst = false, isLast = false, onDuplicateItem, onRemoveItem, onMoveUp, onMoveDown }: EstimateWorkItemCardProps) {
  const t = useTranslations("estimates");
  const { options } = useEstimateOptions();
  const { confirm, ConfirmDialog } = useConfirm();
  const workspace = useEstimateWorkspace();
  const { control, getValues, setValue, formState: { errors } } = useFormContext<EstimateWorkspaceFormData>();
  const path = `sections.${sectionIndex}.workItems.${itemIndex}` as const;
  const currentItem = useWatch({ control, name: path });
  const itemCalc = calculateWorkItemSummary(currentItem ?? {});
  const itemErrors: FieldErrors<WorkItemFormData> = errors.sections?.[sectionIndex]?.workItems?.[itemIndex] ?? {};
  const [activeTab, setActiveTab] = useState<DetailTab>("info");
  const scope = `work-${sectionIndex}-${itemIndex}`;
  const { tabErrorMap } = useFormTabErrors<DetailTab, WorkItemFormData>({
    tabFieldsMap: { info: ["code", "descriptionTh", "descriptionEn", "quantity", "unitCode", "overrideReasonCode", "overrideReason"], cost: ["costComponents"], pricing: ["sellingRuleType", "sellingRuleValue", "sellingRuleReasonCode"] },
    errors: itemErrors, setActiveTab,
  });
  const requestedError = workspace?.errorTarget;
  useEffect(() => {
    if (!requestedError || requestedError.key !== currentItem?.uiKey) return;
    setActiveTab(requestedError.tab);
    requestAnimationFrame(() => {
      const panel = document.getElementById(`tabpanel-${scope}-${requestedError.tab}`);
      panel?.querySelector<HTMLElement>("[aria-invalid='true']")?.focus();
    });
  }, [requestedError, currentItem?.uiKey, scope]);
  const focusTargetId = workspace?.focusTargetId;
  useEffect(() => {
    if (!focusTargetId) return;
    if (currentItem?.costComponents.some((cost) => cost.id === focusTargetId)) setActiveTab("cost");
    else if (currentItem?.id === focusTargetId && currentItem.sellingRuleType === "fixed_price" && !currentItem.sellingRuleReasonCode) setActiveTab("pricing");
  }, [focusTargetId, currentItem?.id, currentItem?.sellingRuleType, currentItem?.sellingRuleReasonCode, currentItem?.costComponents]);

  async function handleRequestRemove() {
    if (await confirm({ title: t("confirmDeleteWorkItem"), message: currentItem.descriptionTh, confirmText: t("removeWorkItem"), variant: "danger" })) onRemoveItem(itemIndex);
  }
  if (!currentItem) return null;
  const hiddenByFilter = workspace && !workspaceWorkMatches(getValues(`sections.${sectionIndex}`), currentItem, workspace.searchQuery, workspace.filterMode);
  const panelProps = (tab: DetailTab) => ({ role: "tabpanel", id: `tabpanel-${scope}-${tab}`, "aria-labelledby": `tab-${scope}-${tab}`, className: activeTab === tab ? "p-3" : "hidden p-3" });
  return (
    <div id={currentItem?.id ? `estimate-target-${currentItem.id}` : undefined} tabIndex={-1} className="bg-erp-surface focus-visible:outline-2 focus-visible:outline-erp-navy">
      {hiddenByFilter && <Alert variant="info">{t("workspace.selectedOutsideFilter")}</Alert>}
      <FormSection title={currentItem.descriptionTh || t("itemDesc")} description={currentItem.code} className="!border-0 !p-3 !gap-2" headerClassName="pb-2 [&_h2]:text-sm sm:flex-row sm:items-start [&>div:first-child]:flex-1 [&_h2]:whitespace-normal [&_h2]:break-words [&_h2]:normal-case" headerAction={
        <div className="flex flex-wrap gap-1">
          {onMoveUp && <Button variant="ghost" size="icon" disabled={isFirst} onClick={() => onMoveUp(itemIndex)} aria-label={t("moveUp")}><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M18 15l-6-6-6 6" /></svg></Button>}
          {onMoveDown && <Button variant="ghost" size="icon" disabled={isLast} onClick={() => onMoveDown(itemIndex)} aria-label={t("moveDown")}><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M6 9l6 6 6-6" /></svg></Button>}
          <Button variant="ghost" size="icon" onClick={() => onDuplicateItem(itemIndex)} aria-label={t("duplicateWorkItem")} icon={<IconCopy size={16} />} />
          <Button variant="ghost" size="icon" onClick={handleRequestRemove} aria-label={t("removeWorkItem")} icon={<IconTrash size={16} />} />
        </div>
      }>
        <div className="flex flex-wrap justify-between items-center gap-3"><span className="text-xs text-erp-text-muted">{t("totalCost")}</span><span className="font-mono text-base font-bold text-erp-navy">{formatFinancialNumber(itemCalc.totalCost)} {currency}</span></div>
      </FormSection>
      <FormTabs activeTab={`${scope}-${activeTab}`} onChange={(id) => { for (const tab of ["info", "cost", "pricing"] as const) if (id === `${scope}-${tab}`) setActiveTab(tab); }} ariaLabel={t("workspace.detailTabs")}
        tabs={[{ id: `${scope}-info`, label: t("workspace.infoTab"), hasError: tabErrorMap.info }, { id: `${scope}-cost`, label: t("workspace.costTab"), count: currentItem.costComponents.length, hasError: tabErrorMap.cost }, { id: `${scope}-pricing`, label: t("workspace.pricingTab"), hasError: tabErrorMap.pricing }]} />
      <div {...panelProps("info")}>
        <div className="space-y-3">
          <Controller control={control} name={`${path}.code`} render={({ field, fieldState }) => <Input {...field} id={`${scope}-code`} label={t("itemCode")} error={fieldState.error ? t("workspace.codeRequired") : undefined} wrapperClassName="mb-0" />} />
          <MultiLangInput id={`work-description-${sectionIndex}-${itemIndex}`} label={t("itemDesc")} value={{ th: currentItem.descriptionTh, en: currentItem.descriptionEn }} onChange={(value) => {
            setValue(`${path}.descriptionTh`, value.th ?? "", { shouldDirty: true, shouldValidate: true });
            setValue(`${path}.descriptionEn`, value.en ?? "", { shouldDirty: true, shouldValidate: true });
          }} placeholder={{ th: t("itemDescPlaceholder"), en: t("workspace.descriptionPlaceholderEn") }} />
          <div className="grid grid-cols-2 gap-3">
            <Controller control={control} name={`${path}.quantity`} render={({ field, fieldState }) => <Input {...field} id={`${scope}-quantity`} type="number" min="0.001" step="any" label={t("quantity")} onChange={(e) => field.onChange(Number(e.target.value))} error={fieldState.error ? t("workspace.quantityInvalid") : undefined} wrapperClassName="mb-0" />} />
            <Controller control={control} name={`${path}.unitCode`} render={({ field }) => <Input {...field} id={`${scope}-unitCode`} label={t("unitCode")} list={`units-${scope}`} wrapperClassName="mb-0" />} />
            <datalist id={`units-${scope}`}>{options.units.map((unit) => <option key={unit.value} value={unit.value}>{unit.label}</option>)}</datalist>
          </div>
          <div className="flex flex-wrap items-center justify-between gap-2 border-t border-erp-border pt-3">
            <div className="min-w-0 text-xs">
              <span className="mr-2 text-erp-text-muted">{t("itemMasterLink")}</span>
              {currentItem.item ? <span className="font-medium text-erp-navy">{currentItem.item.code} · {currentItem.item.nameTh}</span> : <span className="text-erp-text-muted">{t("customWorkItem")}</span>}
            </div>
            <div className="flex gap-2">
              {currentItem.itemId && <Button type="button" size="sm" variant="outline" onClick={() => {
                setValue(`${path}.itemId`, null, { shouldDirty: true, shouldValidate: true });
                setValue(`${path}.item`, null, { shouldDirty: true });
                setValue(`${path}.overrideReasonCode`, "", { shouldDirty: true, shouldValidate: true });
                setValue(`${path}.overrideReason`, "", { shouldDirty: true, shouldValidate: true });
              }}>{t("unlinkItemMaster")}</Button>}
              {workspace && currentItem.uiKey && <Button type="button" size="sm" variant="outline" onClick={() => {
                const sectionKey = getValues(`sections.${sectionIndex}.uiKey`);
                const workKey = currentItem.uiKey;
                if (sectionKey && workKey) workspace.openCatalog({ sectionKey, workKey, mode: "workItem" });
              }}>{currentItem.itemId ? t("changeItemMaster") : t("selectItemMaster")}</Button>}
            </div>
          </div>
          {!currentItem.itemId && <div className="grid gap-3 border-t border-erp-border pt-3 sm:grid-cols-2">
            <Controller control={control} name={`${path}.overrideReasonCode`} render={({ field, fieldState }) => <Input {...field} value={field.value ?? ""} id={`${scope}-overrideReasonCode`} label={t("customWorkItemReasonCode")} error={fieldState.error ? t("customWorkItemReasonCodeRequired") : undefined} wrapperClassName="mb-0" />} />
            <Controller control={control} name={`${path}.overrideReason`} render={({ field, fieldState }) => <Textarea {...field} value={field.value ?? ""} id={`${scope}-overrideReason`} label={t("customWorkItemReason")} error={fieldState.error ? t("customWorkItemReasonRequired") : undefined} rows={2} />} />
          </div>}
        </div>
      </div>
      <div {...panelProps("cost")}><EstimateCostComponentTable sectionIndex={sectionIndex} itemIndex={itemIndex} currency={currency} branchId={branchId} /></div>
      <div {...panelProps("pricing")}>
        <div className="space-y-3">
          <Controller control={control} name={`${path}.sellingRuleType`} render={({ field }) => <Select {...field} id={`${scope}-sellingRuleType`} label={t("sellingRule")} options={options.sellingRuleTypes} wrapperClassName="mb-0" />} />
          <Controller control={control} name={`${path}.sellingRuleValue`} render={({ field, fieldState }) => <Input {...field} id={`${scope}-sellingRuleValue`} type="number" step="any" label={t("sellingRuleValue")} onChange={(e) => field.onChange(Number(e.target.value))} error={fieldState.error ? t("workspace.priceInvalid") : undefined} wrapperClassName="mb-0" />} />
          {currentItem.sellingRuleType === "fixed_price" && <Controller control={control} name={`${path}.sellingRuleReasonCode`} render={({ field, fieldState }) => <Input {...field} value={field.value ?? ""} id={`selling-rule-reason-${sectionIndex}-${itemIndex}`} label={t("sellingRuleReasonCode")} required maxLength={64} error={fieldState.error ? t("fixedPriceReasonRequired") : undefined} wrapperClassName="mb-0" />} />}
          <dl className="grid grid-cols-2 gap-2 border-t border-erp-border pt-3 text-xs">
            <dt className="text-erp-text-muted">{t("unitSellingPrice")}</dt><dd className="text-right font-mono font-bold">{formatFinancialNumber(itemCalc.unitSellingPrice)} {currency}</dd>
            <dt className="text-erp-text-muted">{t("totalSellingPrice")}</dt><dd className="text-right font-mono font-bold text-erp-navy">{formatFinancialNumber(itemCalc.totalSellingPrice)} {currency}</dd>
            <dt className="text-erp-text-muted">{t("grossProfit")}</dt><dd className={cn("text-right font-mono", itemCalc.grossProfit < 0 ? "text-erp-danger" : "text-erp-success")}>{formatSignedFinancialAmount(itemCalc.grossProfit)} {currency} ({formatPercentRate(itemCalc.marginRate, 1)})</dd>
          </dl>
        </div>
      </div>
      <ConfirmDialog />
    </div>
  );
}
