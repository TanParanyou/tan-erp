"use client";

import React, { useEffect, useState, useMemo, useRef } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useForm, FormProvider, useFieldArray, Controller, useWatch, type FieldErrors } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations, useLocale } from "next-intl";
import { cn } from "@/lib/utils/cn";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Alert } from "@/components/ui/Alert";
import { FormTabs } from "@/components/forms/FormTabs";
import { IconSave, IconCheck, IconPlus } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { useConfirm } from "@/hooks/useConfirm";
import { useKeyboardShortcut } from "@/hooks/useKeyboardShortcut";
import type { EstimateDetailResponse, EstimateRevisionResponse, CalculateEstimateRequest } from "@/lib/api/api-client";
import { useUpdateEstimateDraft, useCalculateEstimate } from "../api/estimate-queries";
import { ApiError } from "@/lib/api/api-error";
import { estimateWorkspaceSchema, type EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { workspaceWorkMatches } from "../utils/estimate-workspace-filter";
import { calculateLiveWorkspaceHud } from "../utils/estimate-calculations";
import { isEstimateCalculationSnapshot, exportEstimateSnapshotCsv } from "../utils/estimate-export";
import { toWorkspaceForm, toDraftRequest, catalogCostComponents } from "../utils/estimate-workspace-mapper";
import { formatFinancialNumber } from "../utils/estimate-formatters";
import { EstimateWorkspaceHud } from "./estimate-workspace-hud";
import { EstimateSectionCard } from "./estimate-section-card";
import { EstimateItemCatalogModal } from "./estimate-item-catalog-modal";
import { EstimateWorkspaceContext, scrollEstimateTarget, type WorkspaceTarget, type EstimateWorkspaceContextValue } from "./estimate-workspace-context";
import type { CatalogItemModel } from "../api/estimate-catalog-client";
import { isEstimateCatalogQueryKey } from "../hooks/estimate-catalog-query-key";

interface EstimateWorkspaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  opportunityId?: string;
  estimate: EstimateDetailResponse;
  focusTargetId?: string;
}

export function EstimateWorkspaceDrawer(props: EstimateWorkspaceDrawerProps) {
  return <EstimateWorkspaceSession key={props.estimate.currentRevision?.id ?? props.estimate.id} {...props} />;
}

function EstimateWorkspaceSession({ isOpen, onClose, opportunityId, estimate, focusTargetId }: EstimateWorkspaceDrawerProps) {
  const t = useTranslations("estimates");
  const tc = useTranslations("common");
  const locale = useLocale();
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const { confirm, ConfirmDialog } = useConfirm();
  const [searchQuery, setSearchQuery] = useState("");
  const [filterMode, setFilterMode] = useState<"all" | "low_margin">("all");
  const serverRevision = estimate.currentRevision;
  const [activeRowVersion, setActiveRowVersion] = useState(estimate.currentRevision?.rowVersion);
  const revisionId = estimate.currentRevision?.id ?? "";
  const currency = serverRevision?.currency ?? "THB";
  const [initialValues] = useState(() => toWorkspaceForm(estimate.currentRevision));
  const methods = useForm<EstimateWorkspaceFormData>({
    resolver: zodResolver(estimateWorkspaceSchema), defaultValues: initialValues, shouldUnregister: false,
  });
  const { control, handleSubmit, reset, getValues, setValue, formState: { isDirty, dirtyFields } } = methods;
  const { fields, append, remove } = useFieldArray({ control, name: "sections" });
  const watchedSections = useWatch({ control, name: "sections" });
  const watchedDiscountType = useWatch({ control, name: "discountType" });
  const watchedDiscountValue = useWatch({ control, name: "discountValue" });
  const liveHudMetrics = calculateLiveWorkspaceHud(watchedSections, watchedDiscountValue, watchedDiscountType);
  const [selectedKey, setSelectedKey] = useState<string | null>(initialValues.sections[0]?.workItems[0]?.uiKey ?? null);
  const [inspector, setInspector] = useState<HTMLDivElement | null>(null);
  const [mobileView, setMobileView] = useState<"boq" | "detail">("boq");
  const [catalogTarget, setCatalogTarget] = useState<WorkspaceTarget | null>(null);
  const [errorTarget, setErrorTarget] = useState<EstimateWorkspaceContextValue["errorTarget"]>(null);
  const [showDiscounts, setShowDiscounts] = useState(false);
  const [notice, setNotice] = useState<"invalid" | "calculationFailed" | null>(null);
  const [operation, setOperation] = useState<"save" | "calculate" | null>(null);
  const busy = useRef(false);
  const wasOpen = useRef(false);

  // Query refreshes must not silently rebase an open editing session onto a newer version.
  useEffect(() => {
    if (isOpen && !wasOpen.current) {
      const values = toWorkspaceForm(estimate.currentRevision);
      reset(values);
      setActiveRowVersion(estimate.currentRevision?.rowVersion);
      setSelectedKey(values.sections[0]?.workItems[0]?.uiKey ?? null);
      setNotice(null);
      setCatalogTarget(null);
      setErrorTarget(null);
    }
    wasOpen.current = isOpen;
  }, [isOpen, estimate.currentRevision, reset]);

  const updateDraftMutation = useUpdateEstimateDraft(opportunityId ?? "", estimate.id ?? "", revisionId);
  const calculateMutation = useCalculateEstimate(opportunityId ?? "", estimate.id ?? "", revisionId);
  const isPending = operation !== null || updateDraftMutation.isPending || calculateMutation.isPending;
  const allWorks = watchedSections.flatMap((section) => section.workItems);
  const effectiveSelection = allWorks.some((work) => work.uiKey === selectedKey) ? selectedKey : allWorks[0]?.uiKey ?? null;
  const selectedWork = allWorks.find((work) => work.uiKey === effectiveSelection);

  function selectView(view: "boq" | "detail") {
    setMobileView(view);
    if (window.innerWidth < 992) requestAnimationFrame(() => {
      const target = document.getElementById(`tabpanel-${view}`);
      if (target) scrollEstimateTarget(target);
    });
  }
  function selectWork(key: string) { setSelectedKey(key); selectView("detail"); }

  function showErrors(errors: FieldErrors<EstimateWorkspaceFormData>) {
    setNotice("invalid");
    if (errors.discountType || errors.discountValue || errors.discountReasonCode) setShowDiscounts(true);
    setSearchQuery("");
    setFilterMode("all");
    for (let s = 0; s < watchedSections.length; s++) {
      const sectionErrors = errors.sections?.[s];
      if (!sectionErrors) continue;
      for (let w = 0; w < watchedSections[s].workItems.length; w++) {
        const workErrors = sectionErrors.workItems?.[w];
        const key = watchedSections[s].workItems[w].uiKey;
        if (!workErrors || !key) continue;
        const tab = workErrors.costComponents ? "cost" : workErrors.sellingRuleType || workErrors.sellingRuleValue || workErrors.sellingRuleReasonCode ? "pricing" : "info";
        selectWork(key);
        setErrorTarget((previous) => ({ key, tab, attempt: (previous?.attempt ?? 0) + 1 }));
        return;
      }
      setMobileView("boq");
      requestAnimationFrame(() => document.getElementById(`estimate-section-code-${s}`)?.focus());
      return;
    }
  }

  function reportError(error: unknown, fallback: "saveDraftFailed" | "calculateFailed") {
    if (!(error instanceof ApiError)) { toast.error(tc("alerts.errorTitle")); return; }
    switch (error.code) {
      case "ESTIMATE_VERSION_CONFLICT": toast.error(t("versionConflict")); break;
      case "ESTIMATE_INVALID_STATE": toast.error(t("invalidState")); break;
      case "ESTIMATE_UNIT_INVALID": toast.error(t("unitInvalid")); break;
      case "ITEM_COST_VERSION_CONFLICT":
      case "ITEM_COST_STALE":
      case "ITEM_COST_AMBIGUOUS":
      case "ITEM_COST_NOT_FOUND": {
        void queryClient.invalidateQueries({ predicate: ({ queryKey }) => isEstimateCatalogQueryKey(queryKey) });
        const message = { ITEM_COST_VERSION_CONFLICT: "costVersionConflict", ITEM_COST_STALE: "costStale", ITEM_COST_AMBIGUOUS: "costAmbiguous", ITEM_COST_NOT_FOUND: "costNotFound" } as const;
        toast.error(t(message[error.code])); break;
      }
      default: toast.error(t(fallback));
    }
  }

  function applyRevision(revision: EstimateRevisionResponse, captured: EstimateWorkspaceFormData, preserveDiscount: boolean) {
    const values = toWorkspaceForm(revision, captured);
    setActiveRowVersion(revision.rowVersion);
    reset(values);
    if (preserveDiscount) {
      setValue("discountType", captured.discountType, { shouldDirty: true });
      setValue("discountValue", captured.discountValue, { shouldDirty: true });
      setValue("discountReasonCode", captured.discountReasonCode, { shouldDirty: true });
    }
  }

  async function persistDraft(values: EstimateWorkspaceFormData, version: string) {
    const result = await updateDraftMutation.mutateAsync({ payload: toDraftRequest(values, version), ifMatch: `"${version}"` });
    applyRevision(result, values, true);
    return result;
  }

  const handleSaveDraft = handleSubmit(async (values) => {
    if (busy.current || !activeRowVersion) return;
    busy.current = true; setOperation("save"); setNotice(null);
    try { await persistDraft(values, activeRowVersion); toast.success(t("saveDraftSuccess")); }
    catch (error: unknown) { reportError(error, "saveDraftFailed"); }
    finally { busy.current = false; setOperation(null); }
  }, showErrors);

  const handleRecalculate = handleSubmit(async (values) => {
    if (busy.current || !activeRowVersion) return;
    if (!values.discountType || (values.discountType === "percent" && values.discountValue > 100)) {
      setShowDiscounts(true); methods.setError("discountType", { type: "validate", message: "discountInvalid" }); setNotice("invalid"); return;
    }
    if (values.discountValue > 0 && !values.discountReasonCode.trim()) {
      setShowDiscounts(true);
      methods.setError("discountReasonCode", { type: "validate", message: "discountReasonRequired" });
      methods.setFocus("discountReasonCode"); setNotice("invalid"); return;
    }
    busy.current = true; setOperation("calculate"); setNotice(null);
    let phase: "save" | "calculate" = "save";
    try {
      let version = activeRowVersion;
      if (dirtyFields.sections) {
        const saved = await persistDraft(values, version);
        if (!saved.rowVersion) throw new Error("Missing revision version in save response");
        version = saved.rowVersion;
      }
      phase = "calculate";
      const payload: CalculateEstimateRequest = {
        expectedRevisionVersion: version, discountType: values.discountType,
        discountValue: values.discountValue / (values.discountType === "percent" ? 100 : 1),
        discountReasonCode: values.discountValue > 0 ? values.discountReasonCode.trim() : undefined,
      };
      const calculated = await calculateMutation.mutateAsync(payload);
      // Use the rebased form, which now includes IDs assigned by the successful draft save.
      applyRevision(calculated, getValues(), false);
      toast.success(t("calculateSuccess"));
    } catch (error: unknown) {
      if (phase === "calculate") setNotice("calculationFailed");
      reportError(error, phase === "save" ? "saveDraftFailed" : "calculateFailed");
    } finally { busy.current = false; setOperation(null); }
  }, showErrors);

  useKeyboardShortcut({ key: "s", ctrlOrMeta: true, enabled: isOpen && !isPending, preventDefault: true }, () => { void handleSaveDraft(); });

  const parsedSnapshot = useMemo(() => {
    if (!serverRevision?.calculationSnapshotJson) return null;
    try { const raw: unknown = JSON.parse(serverRevision.calculationSnapshotJson); return isEstimateCalculationSnapshot(raw) ? raw : null; }
    catch { return null; }
  }, [serverRevision?.calculationSnapshotJson]);
  const [isExporting, setIsExporting] = useState(false);
  function handleExportSnapshot() {
    if (!parsedSnapshot) return;
    setIsExporting(true);
    try { exportEstimateSnapshotCsv(`BOQ_${estimate.number ?? "Estimate"}_R${serverRevision?.revisionNo ?? 1}_snapshot`, parsedSnapshot); }
    finally { setIsExporting(false); }
  }

  async function handleAttemptClose() {
    if (busy.current) return;
    if (!isDirty || await confirm({ title: t("discardChangesTitle"), message: t("discardChangesMessage"), confirmText: tc("actions.discard"), cancelText: tc("actions.cancel"), variant: "warning" })) onClose();
  }
  function handleAddSection() {
    const next = fields.length + 1;
    append({ uiKey: crypto.randomUUID(), code: `SEC-${next}`, nameTh: t("defaultSectionNameTh", { number: next }), nameEn: t("defaultSectionNameEn", { number: next }), sortOrder: next, workItems: [] });
    setSearchQuery(""); setFilterMode("all"); setMobileView("boq");
  }
  function handleInsertFromCatalog(items: CatalogItemModel[]) {
    if (!catalogTarget) return;
    const sections = getValues("sections");
    const s = sections.findIndex((section) => section.uiKey === catalogTarget.sectionKey);
    const w = sections[s]?.workItems.findIndex((work) => work.uiKey === catalogTarget.workKey) ?? -1;
    if (s < 0 || w < 0) return;
    if (catalogTarget.mode === "workItem") {
      const item = items[0];
      if (!item) return;
      const workPath = `sections.${s}.workItems.${w}` as const;
      setValue(`${workPath}.itemId`, item.id, { shouldDirty: true, shouldValidate: true });
      setValue(`${workPath}.item`, { id: item.id, code: item.code, nameTh: item.name.thai, nameEn: item.name.english ?? null }, { shouldDirty: true });
      setValue(`${workPath}.code`, item.code, { shouldDirty: true, shouldValidate: true });
      setValue(`${workPath}.descriptionTh`, item.name.thai, { shouldDirty: true, shouldValidate: true });
      setValue(`${workPath}.descriptionEn`, item.name.english ?? "", { shouldDirty: true, shouldValidate: true });
      setValue(`${workPath}.unitCode`, item.baseUnit.code, { shouldDirty: true, shouldValidate: true });
      setValue(`${workPath}.overrideReasonCode`, "", { shouldDirty: true });
      setValue(`${workPath}.overrideReason`, "", { shouldDirty: true });
      return;
    }
    const path = `sections.${s}.workItems.${w}.costComponents` as const;
    const costs = getValues(path);
    setValue(path, [...costs, ...catalogCostComponents(items, locale, costs.length)], { shouldDirty: true, shouldValidate: true });
  }
  const selectedSection = watchedSections.find((section) => section.workItems.some((work) => work.uiKey === effectiveSelection));
  const hasMatches = watchedSections.some((section) => section.workItems.some((work) => workspaceWorkMatches(section, work, searchQuery, filterMode)) || (filterMode === "all" && [section.code, section.nameTh, section.nameEn].some((value) => value.toLowerCase().includes(searchQuery.trim().toLowerCase()))));
  const snapshotOutdated = isDirty || serverRevision?.calculationOutdated === true;

  return (
    <Drawer isOpen={isOpen} onClose={handleAttemptClose} title={t("workspaceTitle")} size="full" closeOnOverlayClick={false} overlayClassName="[&>.erp-drawer-window>div:first-child]:py-2 [&>.erp-drawer-window>div:first-child]:px-4" noPadding>
      <FormProvider {...methods}>
        <EstimateWorkspaceContext.Provider value={{ selectedKey: effectiveSelection, selectWork, inspector, searchQuery, filterMode, openCatalog: setCatalogTarget, catalogTarget, focusTargetId, errorTarget }}>
          <fieldset disabled={isPending || catalogTarget !== null} className="flex h-full min-h-0 min-w-0 flex-col border-0 p-0 m-0 bg-erp-canvas min-[992px]:[--erp-min-touch:36px] [&_.erp-input]:text-xs [&_.erp-select]:text-xs [&_.erp-label]:text-xs [&_.erp-form-group]:gap-1">
              <div className="shrink-0 min-[992px]:hidden"><FormTabs activeTab={mobileView} onChange={selectView} ariaLabel={t("workspace.navigation")} tabs={[{ id: "boq", label: t("workspace.boq") }, { id: "detail", label: t("workspace.detail") }]} /></div>
            <div data-estimate-scroll className="flex min-h-0 flex-1 flex-col overflow-y-auto min-[992px]:overflow-hidden">
            <div className="shrink-0"><EstimateWorkspaceHud estimateNumber={estimate.number ?? ""} revisionNo={serverRevision?.revisionNo ?? 1} status={serverRevision?.status ?? "draft"} currency={currency} hudMetrics={liveHudMetrics} /></div>

              <div className="shrink-0 flex flex-wrap items-center justify-between gap-2 border-b border-erp-border bg-erp-surface px-3 py-2 sm:px-4">
                <div className="flex min-w-0 flex-1 flex-wrap items-center gap-2">
                  <Input aria-label={t("searchPlaceholder")} placeholder={t("searchPlaceholder")} value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} wrapperClassName="mb-0 min-w-40 flex-1 max-w-sm" />
                  <Button size="sm" variant={filterMode === "all" ? "primary" : "outline"} aria-pressed={filterMode === "all"} onClick={() => setFilterMode("all")}>{t("filterAll")}</Button>
                  <Button size="sm" variant={filterMode === "low_margin" ? "danger" : "outline"} aria-pressed={filterMode === "low_margin"} onClick={() => setFilterMode("low_margin")}>{t("filterWarning")}</Button>
                </div>
                <div className="flex flex-wrap items-center gap-2">
                  <Button size="sm" variant="outline" onClick={handleExportSnapshot} disabled={isExporting || !parsedSnapshot} isLoading={isExporting} title={!parsedSnapshot ? t("exportSnapshotRequired") : t("workspace.snapshotExport")}>{t("exportCsv")}</Button>
                  <Button size="sm" variant="outline" icon={<IconPlus size={14} />} onClick={handleAddSection}>{t("addSection")}</Button>
                  <Button size="sm" onClick={() => { if (selectedSection?.uiKey && effectiveSelection) setCatalogTarget({ sectionKey: selectedSection.uiKey, workKey: effectiveSelection }); }} disabled={!selectedWork} title={selectedWork?.code ?? t("workspace.selectWork")}>{t("workspace.openCatalog")}</Button>
                </div>
              </div>
              {notice && <Alert variant="warning" className="shrink-0">{t(notice === "invalid" ? "workspace.validationFailed" : "workspace.calculationFailed")}</Alert>}

              <div className="grid min-h-[320px] shrink-0 min-[992px]:min-h-0 min-[992px]:flex-1 min-[992px]:shrink min-[992px]:grid-cols-[minmax(0,1fr)_clamp(360px,34vw,460px)]">
                <div data-estimate-scroll role="tabpanel" id="tabpanel-boq" aria-labelledby="tab-boq" className={cn("min-h-0 min-w-0 overflow-auto p-2 sm:p-3", mobileView === "detail" && "hidden min-[992px]:block")}>
                  <h2 className="mb-2 text-sm font-bold text-erp-navy">{t("workspace.boq")}</h2>
                  {fields.length === 0 && <div className="border border-dashed border-erp-border bg-erp-surface p-6 text-center"><p className="mb-4 text-sm text-erp-text-muted">{t("noSectionsYet")}</p><Button onClick={handleAddSection} icon={<IconPlus size={16} />}>{t("addSection")}</Button></div>}
                  {fields.length > 0 && !hasMatches && <p className="p-6 text-center text-sm text-erp-text-muted">{t("noMatchingItems")}</p>}
                  <div className="space-y-3">{fields.map((field, s) => <EstimateSectionCard key={field.id} sectionIndex={s} currency={currency} branchId={estimate.branchId} onRemoveSection={remove} />)}</div>
                </div>
                <div data-estimate-scroll role="tabpanel" id="tabpanel-detail" aria-labelledby="tab-detail" className={cn("min-h-0 min-w-0 overflow-auto border-l border-erp-border bg-erp-surface", mobileView === "boq" && "hidden min-[992px]:block")}>
                  <div className="border-b border-erp-border px-3 py-2"><h2 className="text-sm font-bold text-erp-navy">{t("workspace.detail")}</h2><p className="mt-1 text-xs text-erp-text-muted">{selectedWork ? t("workspace.catalogDestination", { code: selectedWork.code }) : t("workspace.selectWork")}</p></div>
                  {!selectedWork && <p className="p-6 text-sm text-erp-text-muted">{t("workspace.selectWork")}</p>}
                  <div ref={setInspector} />
                </div>
              </div>
              <div className="flex shrink-0 flex-wrap gap-x-5 gap-y-1 border-t border-erp-border bg-erp-surface-subtle px-4 py-2 text-xs sm:px-6" role="status">
                <span className="font-semibold text-erp-navy">{t("workspace.officialSnapshot")}</span>
                {parsedSnapshot ? <><span>{t("netBeforeTax")}: {formatFinancialNumber(parsedSnapshot.netBeforeTax)} {currency}</span><span>{t("taxAmount")}: {formatFinancialNumber(parsedSnapshot.taxAmount)} {currency}</span><span className="font-bold">{t("grandTotal")}: {formatFinancialNumber(parsedSnapshot.grandTotal)} {currency}</span></> : <span>{t("exportSnapshotRequired")}</span>}
                {snapshotOutdated && <span className="text-erp-warning-text">{t("workspace.snapshotOutdated")}</span>}
              </div>
            </div>
          {/* Sticky form actions remain accessible while the workspace content scrolls */}
          <div className="shrink-0 bg-erp-surface border-t border-erp-border px-3 sm:px-4 py-2 flex flex-wrap items-center justify-between gap-3 shadow-lg">
            {/* Discount Setting */}
            <Button variant="outline" className="min-[992px]:hidden" aria-expanded={showDiscounts} onClick={() => setShowDiscounts((value) => !value)}>{t("discountType")}</Button>
            <div className={cn("min-w-0 w-full flex-wrap items-end gap-2.5 min-[992px]:w-auto", showDiscounts ? "flex" : "hidden min-[992px]:flex")}>
              <Controller
                control={control}
                name="discountType"
                render={({ field, fieldState }) => (
                  <Select
                    {...field}
                    id="estimate-discount-type"
                    label={t("discountType")}
                    error={fieldState.error ? t("workspace.discountInvalid") : undefined}
                    value={field.value}
                    onChange={(event) => {
                      field.onChange(event.currentTarget.value);
                      if (event.currentTarget.value === "none") {
                        methods.setValue("discountValue", 0, { shouldDirty: true });
                        methods.setValue("discountReasonCode", "", { shouldDirty: true });
                      } else if (event.currentTarget.value !== field.value) {
                        methods.setValue("discountValue", 0, { shouldDirty: true });
                        methods.setValue("discountReasonCode", "", { shouldDirty: true });
                      }
                    }}
                    options={[
                      { value: "none", label: t("discountTypes.none") },
                      { value: "percent", label: t("discountTypes.percent") },
                      { value: "fixed-amount", label: t("discountTypes.fixedAmount") },
                    ]}
                    placeholder={t("discountTypePlaceholder")}
                    wrapperClassName="mb-0 w-full sm:w-40"
                    className="text-xs"
                  />
                )}
              />
              <Controller
                control={control}
                name="discountValue"
                render={({ field, fieldState }) => (
                  <Input
                    {...field}
                    id="estimate-discount-value"
                    label={t("discountValue")}
                    placeholder={t("workspace.discountValuePlaceholder")}
                    error={fieldState.error ? t("workspace.discountInvalid") : undefined}
                    type="number"
                    step={watchedDiscountType === "percent" ? "0.01" : "100"}
                    min="0"
                    max={watchedDiscountType === "percent" ? "100" : undefined}
                    value={field.value ?? 0}
                    onChange={(event) => field.onChange(Number(event.target.value))}
                    wrapperClassName="mb-0 w-32"
                    className="text-xs text-right font-mono"
                    disabled={watchedDiscountType === "none" || watchedDiscountType === ""}
                  />
                )}
              />
              <span className="pb-2 text-xs font-mono text-erp-text-muted">
                {watchedDiscountType === "percent" ? t("discountRateUnit") : currency}
              </span>
              {watchedDiscountValue > 0 && (
                <Controller
                  control={control}
                  name="discountReasonCode"
                  render={({ field, fieldState }) => (
                    <Input
                      {...field}
                      id="estimate-discount-reason"
                      label={t("discountReasonCode")}
                      placeholder={t("discountReasonCodePlaceholder")}
                      error={fieldState.error ? t("discountReasonRequired") : undefined}
                      required
                      maxLength={64}
                      value={field.value}
                      onChange={field.onChange}
                      wrapperClassName="mb-0 w-full sm:w-48"
                      className="text-xs"
                    />
                  )}
                />
              )}
            </div>

            <span role="status" className="text-xs text-erp-text-muted">{isDirty ? t("workspace.unsaved") : t("workspace.saved")}</span>
            {/* Action Buttons with Keyboard Shortcut Hint */}
            <div className="grid w-full grid-cols-2 gap-2 sm:flex sm:w-auto sm:flex-wrap sm:items-center sm:gap-2.5">
              <Button
                type="button"
                variant="outline"
                size="md"
                onClick={() => void handleSaveDraft()}
                disabled={isPending}
                isLoading={operation === "save"}
                className="whitespace-normal text-xs sm:text-sm"

                title={t("saveDraftShortcut")}
              >
                <IconSave size={14} className="mr-1.5" />
                <span className="mr-1.5">{t("saveDraftAction")}</span>
                <span className="hidden sm:inline text-[11px] font-mono px-1 py-0.2 bg-erp-bg-neutral text-erp-text-muted border border-border">
                  ⌘S
                </span>
              </Button>
              <Button
                type="button"
                variant="primary"
                size="md"
                onClick={() => void handleRecalculate()}
                disabled={isPending}
                isLoading={operation === "calculate"}
                className="whitespace-normal text-xs sm:text-sm"

              >
                <IconCheck size={14} className="mr-1.5" />
                {t("workspace.saveAndCalculate")}
              </Button>
            </div>
          </div>
          </fieldset>
          <EstimateItemCatalogModal isOpen={catalogTarget !== null} onClose={() => setCatalogTarget(null)} onSelectItems={handleInsertFromCatalog} currency={currency} branchId={estimate.branchId} singleSelect={catalogTarget?.mode === "workItem"} />
        </EstimateWorkspaceContext.Provider>
      </FormProvider>
      <ConfirmDialog />
    </Drawer>
  );
}
