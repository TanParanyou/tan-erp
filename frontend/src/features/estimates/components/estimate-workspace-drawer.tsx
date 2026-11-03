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
import { Tooltip } from "@/components/ui/Tooltip";
import { IconSave, IconCheck, IconPlus, IconClose, IconList, IconAlertTriangle, IconDownload, IconFileText, IconRotateCcw, IconRotateCw, IconClock } from "@/components/common/Icons";
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
import { EstimateHistoryModal } from "./estimate-history-modal";
import { EstimateWorkspaceContext, type WorkspaceDetailTab, type WorkspaceTarget, type EstimateWorkspaceContextValue } from "./estimate-workspace-context";
import type { CatalogItemModel } from "../api/estimate-catalog-client";
import { isEstimateCatalogQueryKey } from "../hooks/estimate-catalog-query-key";
import { useFormHistory } from "../hooks/use-form-history";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { clearWorkspaceDraft, draftStorageKey, loadWorkspaceDraft, saveWorkspaceDraft } from "../utils/estimate-workspace-draft-storage";

interface EstimateWorkspaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  opportunityId?: string;
  estimate: EstimateDetailResponse;
  focusTargetId?: string;
  readOnly?: boolean;
}

export function EstimateWorkspaceDrawer(props: EstimateWorkspaceDrawerProps) {
  return <EstimateWorkspaceSession key={props.estimate.currentRevision?.id ?? props.estimate.id} {...props} />;
}

function EstimateWorkspaceSession({ isOpen, onClose, opportunityId, estimate, focusTargetId, readOnly = false }: EstimateWorkspaceDrawerProps) {
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
  const lastReturn = serverRevision?.status === "returned" ? serverRevision.lastReturn : undefined;
  const [initialValues] = useState(() => toWorkspaceForm(estimate.currentRevision));
  const methods = useForm<EstimateWorkspaceFormData>({
    resolver: zodResolver(estimateWorkspaceSchema), defaultValues: initialValues, shouldUnregister: false,
  });
  const { control, handleSubmit, reset, getValues, setValue, formState: { isDirty, dirtyFields } } = methods;
  const { fields, append, remove, move, replace } = useFieldArray({ control, name: "sections" });
  const history = useFormHistory(methods);
  const { selectedMembership } = useSelectedMembership();
  const draftKey = draftStorageKey(selectedMembership?.id ?? "", estimate.id ?? "", revisionId);
  const [pendingDraft, setPendingDraft] = useState<ReturnType<typeof loadWorkspaceDraft>>(null);
  const pendingDraftRef = useRef(false);
  const baseVersionRef = useRef(estimate.currentRevision?.rowVersion);
  const watchedSections = useWatch({ control, name: "sections" });
  const watchedDiscountType = useWatch({ control, name: "discountType" });
  const watchedDiscountValue = useWatch({ control, name: "discountValue" });
  const liveHudMetrics = calculateLiveWorkspaceHud(watchedSections, watchedDiscountValue, watchedDiscountType);
  const [selectedKey, setSelectedKey] = useState<string | null>(initialValues.sections[0]?.workItems[0]?.uiKey ?? null);
  const [inspector, setInspector] = useState<HTMLDivElement | null>(null);
  const [detailOpen, setDetailOpen] = useState(false);
  const [detailTab, setDetailTab] = useState<WorkspaceDetailTab>("info");
  const [catalogTarget, setCatalogTarget] = useState<WorkspaceTarget | null>(null);
  const [errorTarget, setErrorTarget] = useState<EstimateWorkspaceContextValue["errorTarget"]>(null);
  const [justCalculated, setJustCalculated] = useState(false);
  const [conflict, setConflict] = useState(false);
  const [historyOpen, setHistoryOpen] = useState(false);
  const latestEstimate = useRef(estimate);
  const [notice, setNotice] = useState<"invalid" | "calculationFailed" | null>(null);
  const [operation, setOperation] = useState<"save" | "calculate" | null>(null);
  const busy = useRef(false);
  const wasOpen = useRef(false);
  function trackVersion(version: string | undefined) {
    baseVersionRef.current = version;
    setActiveRowVersion(version);
  }

  // Query refreshes must not silently rebase an open editing session onto a newer version.
  useEffect(() => {
    if (isOpen && !wasOpen.current) {
      const values = toWorkspaceForm(estimate.currentRevision);
      reset(values);
      history.rebase();
      trackVersion(estimate.currentRevision?.rowVersion);
      const draft = readOnly ? null : loadWorkspaceDraft(draftKey, estimate.currentRevision?.rowVersion);
      pendingDraftRef.current = draft !== null;
      setPendingDraft(draft);
      setSelectedKey(values.sections[0]?.workItems[0]?.uiKey ?? null);
      setNotice(null);
      setDetailOpen(false);
      setCatalogTarget(null);
      setErrorTarget(null);
    }
    wasOpen.current = isOpen;
  }, [isOpen, estimate.currentRevision, reset, history, draftKey, readOnly]);

  useEffect(() => { latestEstimate.current = estimate; });

  // Keep unsaved edits in this tab's session so a crash or expired session does not lose them.
  useEffect(() => {
    if (!isOpen || readOnly) return;
    let timer: ReturnType<typeof setTimeout> | undefined;
    const subscription = methods.watch(() => {
      clearTimeout(timer);
      timer = setTimeout(() => {
        if (pendingDraftRef.current) return;
        const version = baseVersionRef.current;
        if (methods.formState.isDirty && version) saveWorkspaceDraft(draftKey, version, methods.getValues());
        else clearWorkspaceDraft(draftKey);
      }, 800);
    });
    return () => { subscription.unsubscribe(); clearTimeout(timer); };
  }, [isOpen, methods, draftKey, readOnly]);

  const updateDraftMutation = useUpdateEstimateDraft(opportunityId ?? "", estimate.id ?? "", revisionId);
  const calculateMutation = useCalculateEstimate(opportunityId ?? "", estimate.id ?? "", revisionId);
  const isPending = operation !== null || updateDraftMutation.isPending || calculateMutation.isPending;
  const allWorks = watchedSections.flatMap((section) => section.workItems);
  const effectiveSelection = allWorks.some((work) => work.uiKey === selectedKey) ? selectedKey : allWorks[0]?.uiKey ?? null;
  const selectedWork = allWorks.find((work) => work.uiKey === effectiveSelection);

  // Below 992px the inspector is a bottom sheet, so choosing a work item opens it.
  function selectWork(key: string) {
    setSelectedKey(key);
    if (window.innerWidth < 992) setDetailOpen(true);
  }

  function moveWorkItem(workKey: string, targetSectionKey: string) {
    const sections = getValues("sections");
    const from = sections.findIndex((section) => section.workItems.some((work) => work.uiKey === workKey));
    const to = sections.findIndex((section) => section.uiKey === targetSectionKey);
    const work = sections[from]?.workItems.find((candidate) => candidate.uiKey === workKey);
    if (from < 0 || to < 0 || from === to || !work) return;
    replace(sections.map((section, index) => {
      if (index === from) return { ...section, workItems: section.workItems.filter((candidate) => candidate.uiKey !== workKey).map((candidate, order) => ({ ...candidate, sortOrder: order + 1 })) };
      if (index === to) return { ...section, workItems: [...section.workItems, { ...work, sortOrder: section.workItems.length + 1 }] };
      return section;
    }));
  }

  function showErrors(errors: FieldErrors<EstimateWorkspaceFormData>) {
    setNotice("invalid");
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
      setDetailOpen(false);
      requestAnimationFrame(() => document.getElementById(`estimate-section-code-${s}`)?.focus());
      return;
    }
  }

  function reportError(error: unknown, fallback: "saveDraftFailed" | "calculateFailed") {
    if (!(error instanceof ApiError)) { toast.error(tc("alerts.errorTitle")); return; }
    switch (error.code) {
      case "ESTIMATE_VERSION_CONFLICT": setConflict(true); toast.error(t("versionConflict")); break;
      case "ESTIMATE_INVALID_STATE": toast.error(t("invalidState")); break;
      case "ESTIMATE_UNIT_INVALID": toast.error(t("unitInvalid")); break;
      case "ESTIMATE_UNIT_CONVERSION_NOT_FOUND": toast.error(t("unitConversionNotFound")); break;
      case "ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH": toast.error(t("unitConversionDimensionMismatch")); break;
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
    trackVersion(revision.rowVersion);
    reset(values);
    clearWorkspaceDraft(draftKey);
    if (preserveDiscount) {
      setValue("discountType", captured.discountType, { shouldDirty: true });
      setValue("discountValue", captured.discountValue, { shouldDirty: true });
      setValue("discountReasonCode", captured.discountReasonCode, { shouldDirty: true });
    }
    history.rebase();
  }

  async function handleReloadLatest() {
    if (!await confirm({ title: t("reloadLatestConfirmTitle"), message: t("reloadLatestConfirmMessage"), confirmText: t("reloadLatest"), cancelText: tc("actions.cancel"), variant: "warning" })) return;
    await queryClient.invalidateQueries({ predicate: ({ queryKey }) => queryKey.includes("estimates") });
    // Let the refreshed props commit so the form is rebuilt from the newest server revision.
    await new Promise((resolve) => setTimeout(resolve, 0));
    const latest = latestEstimate.current.currentRevision;
    const values = toWorkspaceForm(latest);
    trackVersion(latest?.rowVersion);
    reset(values);
    history.rebase();
    clearWorkspaceDraft(draftKey);
    setSelectedKey(values.sections[0]?.workItems[0]?.uiKey ?? null);
    setConflict(false);
    setNotice(null);
    setJustCalculated(false);
  }

  function restoreDraft() {
    if (!pendingDraft) return;
    reset(pendingDraft.values, { keepDefaultValues: true });
    history.rebase();
    setSelectedKey(pendingDraft.values.sections[0]?.workItems[0]?.uiKey ?? null);
    pendingDraftRef.current = false;
    setPendingDraft(null);
  }
  function discardDraft() {
    clearWorkspaceDraft(draftKey);
    pendingDraftRef.current = false;
    setPendingDraft(null);
  }

  async function persistDraft(values: EstimateWorkspaceFormData, version: string) {
    const result = await updateDraftMutation.mutateAsync({ payload: toDraftRequest(values, version), ifMatch: `"${version}"` });
    applyRevision(result, values, true);
    return result;
  }

  const handleSaveDraft = handleSubmit(async (values) => {
    if (busy.current || !activeRowVersion) return;
    busy.current = true; setOperation("save"); setNotice(null); setJustCalculated(false);
    try { await persistDraft(values, activeRowVersion); toast.success(t("saveDraftSuccess")); }
    catch (error: unknown) { reportError(error, "saveDraftFailed"); }
    finally { busy.current = false; setOperation(null); }
  }, showErrors);

  const handleRecalculate = handleSubmit(async (values) => {
    if (busy.current || !activeRowVersion) return;
    if (!values.discountType || (values.discountType === "percent" && values.discountValue > 100)) {
      methods.setError("discountType", { type: "validate", message: "discountInvalid" }); setNotice("invalid"); return;
    }
    if (values.discountValue > 0 && !values.discountReasonCode.trim()) {
      methods.setError("discountReasonCode", { type: "validate", message: "discountReasonRequired" });
      methods.setFocus("discountReasonCode"); setNotice("invalid"); return;
    }
    busy.current = true; setOperation("calculate"); setNotice(null); setJustCalculated(false);
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
      setJustCalculated(true);
      toast.success(t("calculateSuccess"));
    } catch (error: unknown) {
      if (phase === "calculate") setNotice("calculationFailed");
      reportError(error, phase === "save" ? "saveDraftFailed" : "calculateFailed");
    } finally { busy.current = false; setOperation(null); }
  }, showErrors);

  useKeyboardShortcut({ key: "s", ctrlOrMeta: true, enabled: isOpen && !isPending && !readOnly, preventDefault: true }, () => { void handleSaveDraft(); });
  // Inside a text field Ctrl+Z keeps its native per-field undo; the form-wide history applies elsewhere.
  const outsideEditable = (e: KeyboardEvent) => !(e.target instanceof Element && e.target.closest("input, textarea, select, [contenteditable='true']"));
  useKeyboardShortcut({ key: "z", ctrlOrMeta: true, enabled: isOpen && !isPending && !readOnly, preventDefault: false }, (e) => { if (e.shiftKey || !outsideEditable(e)) return; e.preventDefault(); history.undo(); });
  useKeyboardShortcut({ key: "z", ctrlOrMeta: true, shift: true, enabled: isOpen && !isPending && !readOnly, preventDefault: false }, (e) => { if (!outsideEditable(e)) return; e.preventDefault(); history.redo(); });

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
    if (!isDirty || await confirm({ title: t("discardChangesTitle"), message: t("discardChangesMessage"), confirmText: tc("actions.discard"), cancelText: tc("actions.cancel"), variant: "warning" })) { clearWorkspaceDraft(draftKey); onClose(); }
  }
  function handleAddSection() {
    const next = fields.length + 1;
    append({ uiKey: crypto.randomUUID(), code: `SEC-${next}`, nameTh: t("defaultSectionNameTh", { number: next }), nameEn: t("defaultSectionNameEn", { number: next }), sortOrder: next, workItems: [] });
    setSearchQuery(""); setFilterMode("all"); setDetailOpen(false);
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
  const discountFields = (
        <div inert={readOnly} className="flex min-w-0 flex-wrap items-end gap-2.5">
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
                wrapperClassName="mb-0 w-36"
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
                  wrapperClassName="mb-0 w-full sm:w-44"
                  className="text-xs"
                />
              )}
            />
          )}
        </div>
  );


  return (
    <Drawer isOpen={isOpen} onClose={handleAttemptClose} title={t("workspaceTitle")} size="full" closeOnOverlayClick={false} showHeader={false} noPadding>
      <FormProvider {...methods}>
        <EstimateWorkspaceContext.Provider value={{ selectedKey: effectiveSelection, selectWork, inspector, searchQuery, filterMode, openCatalog: setCatalogTarget, catalogTarget, focusTargetId, errorTarget, detailTab, setDetailTab, moveWorkItem, readOnly }}>
          <fieldset disabled={isPending || catalogTarget !== null} className="flex h-full min-h-0 min-w-0 flex-row border-0 p-0 m-0 bg-erp-canvas min-[992px]:[--erp-min-touch:36px] [&_.erp-input]:text-xs [&_.erp-select]:text-xs [&_.erp-label]:text-xs [&_.erp-form-group]:gap-1">
            <div className="flex min-h-0 min-w-0 flex-1 flex-col">
              <div className="flex shrink-0 items-center justify-between gap-3 border-b border-erp-border bg-erp-surface py-1 pl-4 pr-1">
                <h2 className="min-w-0 truncate text-base font-semibold text-erp-text-main">{t("workspaceTitle")}</h2>
                <Button size="icon" variant="ghost" onClick={handleAttemptClose} aria-label={tc("actions.close")} title={tc("actions.close")} icon={<IconClose size={20} />} />
              </div>
              <div className="relative flex min-h-0 flex-1 flex-col">
                <div data-estimate-scroll className="flex min-h-0 flex-1 flex-col overflow-y-auto min-[992px]:overflow-hidden">
                  {conflict && (
                    <Alert variant="danger" className="shrink-0">
                      <p className="font-semibold">{t("versionConflictTitle")}</p>
                      <p>{t("versionConflictBody")}</p>
                      <Button size="sm" variant="primary" className="mt-2" onClick={() => void handleReloadLatest()}>{t("reloadLatest")}</Button>
                    </Alert>
                  )}
                  {pendingDraft && (
                    <Alert variant="info" className="shrink-0">
                      <p>{t("workspace.draftFound", { time: new Intl.DateTimeFormat(locale, { dateStyle: "medium", timeStyle: "short" }).format(new Date(pendingDraft.savedAtUtc)) })}</p>
                      <div className="mt-2 flex gap-2">
                        <Button size="sm" variant="primary" onClick={restoreDraft}>{t("workspace.draftRestore")}</Button>
                        <Button size="sm" variant="outline" onClick={discardDraft}>{t("workspace.draftDiscard")}</Button>
                      </div>
                    </Alert>
                  )}
                  {readOnly && <Alert variant="info" className="shrink-0">{t("workspace.readOnlyNotice")}</Alert>}
                  {lastReturn && (
                    <Alert variant="warning" className="shrink-0">
                      <p className="font-semibold">{t("workspace.returnedBy", { name: lastReturn.returnedBy.displayName, date: new Intl.DateTimeFormat(locale, { dateStyle: "medium" }).format(new Date(lastReturn.returnedAtUtc)) })}</p>
                      <p>{lastReturn.reasonCode}: {lastReturn.note}</p>
                    </Alert>
                  )}
                  <div className="shrink-0"><EstimateWorkspaceHud estimateNumber={estimate.number ?? ""} revisionNo={serverRevision?.revisionNo ?? 1} status={serverRevision?.status ?? "draft"} currency={currency} hudMetrics={liveHudMetrics} discountSlot={discountFields} /></div>

                  <div className="shrink-0 flex flex-wrap items-center justify-between gap-2 border-b border-erp-border bg-erp-surface px-3 py-2 sm:px-4">
                    <div className="flex min-w-0 flex-1 items-center gap-2">
                      <Input aria-label={t("searchPlaceholder")} placeholder={t("searchPlaceholder")} value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} wrapperClassName="mb-0 min-w-40 flex-1 max-w-sm" />
                      <Tooltip content={t("filterWarning")} position="bottom">
                        <Button size="icon" variant={filterMode === "low_margin" ? "primary" : "outline"} aria-pressed={filterMode === "low_margin"} aria-label={t("filterWarning")} onClick={() => setFilterMode((mode) => (mode === "all" ? "low_margin" : "all"))} icon={<IconAlertTriangle size={16} />} />
                      </Tooltip>
                    </div>
                    <div className="flex items-center gap-2">
                      <Tooltip content={parsedSnapshot ? t("workspace.snapshotExport") : t("exportSnapshotRequired")} position="bottom">
                        <Button size="icon" variant="outline" onClick={handleExportSnapshot} disabled={isExporting || !parsedSnapshot} isLoading={isExporting} aria-label={t("exportCsv")} icon={<IconDownload size={16} />} />
                      </Tooltip>
                      <Tooltip content={t("viewHistory")} position="bottom">
                        <Button size="icon" variant="outline" onClick={() => setHistoryOpen(true)} aria-label={t("viewHistory")} icon={<IconClock size={16} />} />
                      </Tooltip>
                      {!readOnly && <>
                      <Tooltip content={t("workspace.undo")} position="bottom">
                        <Button size="icon" variant="outline" onClick={history.undo} disabled={!history.canUndo} aria-label={t("workspace.undo")} icon={<IconRotateCcw size={16} />} />
                      </Tooltip>
                      <Tooltip content={t("workspace.redo")} position="bottom">
                        <Button size="icon" variant="outline" onClick={history.redo} disabled={!history.canRedo} aria-label={t("workspace.redo")} icon={<IconRotateCw size={16} />} />
                      </Tooltip>
                      <Tooltip content={t("addSection")} position="bottom">
                        <Button size="icon" variant="outline" onClick={handleAddSection} aria-label={t("addSection")} icon={<IconPlus size={16} />} />
                      </Tooltip>
                      <Tooltip content={selectedWork ? `${t("workspace.openCatalog")} · ${selectedWork.code}` : t("workspace.selectWork")} position="bottom">
                        <Button size="icon" variant="primary" onClick={() => { if (selectedSection?.uiKey && effectiveSelection) setCatalogTarget({ sectionKey: selectedSection.uiKey, workKey: effectiveSelection }); }} disabled={!selectedWork} aria-label={t("workspace.openCatalog")} icon={<IconList size={16} />} />
                      </Tooltip>
                      </>}
                    </div>
                  </div>
                  {notice && <Alert variant="warning" className="shrink-0">{t(notice === "invalid" ? "workspace.validationFailed" : "workspace.calculationFailed")}</Alert>}

                  <div data-estimate-scroll data-boq-scope="" className="min-h-[320px] min-w-0 shrink-0 overflow-auto p-2 sm:p-3 min-[992px]:min-h-0 min-[992px]:flex-1 min-[992px]:shrink">
                    <h2 className="mb-2 text-sm font-bold text-erp-navy">{t("workspace.boq")}</h2>
                    {fields.length === 0 && <div className="border border-dashed border-erp-border bg-erp-surface p-6 text-center"><p className="mb-4 text-sm text-erp-text-muted">{t("noSectionsYet")}</p>{!readOnly && <Button onClick={handleAddSection} icon={<IconPlus size={16} />}>{t("addSection")}</Button>}</div>}
                    {fields.length > 0 && !hasMatches && <p className="p-6 text-center text-sm text-erp-text-muted">{t("noMatchingItems")}</p>}
                    <div className="space-y-3">{fields.map((field, s) => <EstimateSectionCard key={field.id} sectionIndex={s} currency={currency} branchId={estimate.branchId} onRemoveSection={remove} onMoveSection={(from, delta) => move(from, from + delta)} isFirstSection={s === 0} isLastSection={s === fields.length - 1} />)}</div>
                  </div>

                  <div className="flex shrink-0 flex-wrap items-center gap-x-5 gap-y-1 border-t border-erp-border bg-erp-surface-subtle px-4 py-2 text-xs sm:px-6" role="status">
                    <span className="font-semibold text-erp-navy">{t("workspace.officialSnapshot")}</span>
                    {parsedSnapshot ? <><span>{t("netBeforeTax")}: {formatFinancialNumber(parsedSnapshot.netBeforeTax)} {currency}</span><span>{t("taxAmount")}: {formatFinancialNumber(parsedSnapshot.taxAmount)} {currency}</span><span className="font-bold">{t("grandTotal")}: {formatFinancialNumber(parsedSnapshot.grandTotal)} {currency}</span></> : <span>{t("workspace.snapshotNotCalculated")}</span>}
                    {snapshotOutdated && <span className="font-semibold text-erp-warning-text">{t("workspace.snapshotOutdated")}</span>}
                  </div>
                </div>
                {/* Phone shortcut to the inspector sheet; sits above the sticky action bar */}
                <Button size="icon" variant="primary" onClick={() => setDetailOpen(true)} aria-label={t("workspace.detail")} aria-expanded={detailOpen} icon={<IconFileText size={22} />} className={cn("absolute bottom-3 right-3 z-10 h-14 w-14 shadow-lg min-[992px]:hidden", detailOpen && "hidden")} />
              </div>

              {/* Action bar stays pinned below the scrolling workspace */}
              {readOnly ? (
                <div className="shrink-0 flex items-center justify-end border-t border-erp-border bg-erp-surface px-3 py-2 sm:px-4">
                  <Button type="button" variant="outline" size="md" onClick={onClose}>{tc("actions.close")}</Button>
                </div>
              ) : (
              <div className="shrink-0 bg-erp-surface border-t border-erp-border px-3 sm:px-4 py-2 flex flex-wrap items-center justify-between gap-3 shadow-lg">
                <div className="flex min-w-0 flex-wrap items-center gap-2">
                  <span role="status" className="text-xs text-erp-text-muted">{isDirty ? t("workspace.unsaved") : t("workspace.saved")}</span>
                  {justCalculated && !isDirty && (
                    <>
                      <span className="text-xs font-semibold text-erp-success">{t("workspace.calculatedReady")}</span>
                      <Button size="sm" variant="outline" onClick={onClose}>{t("workspace.closeToSubmit")}</Button>
                    </>
                  )}
                </div>
                <div className="flex w-full items-center gap-2 sm:w-auto sm:gap-2.5">
                  <Tooltip content={t("saveDraftShortcut")} position="top">
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      onClick={() => void handleSaveDraft()}
                      disabled={isPending}
                      isLoading={operation === "save"}
                      aria-label={t("saveDraftAction")}
                      icon={<IconSave size={18} />}
                    />
                  </Tooltip>
                  <Button
                    type="button"
                    variant="primary"
                    size="md"
                    onClick={() => void handleRecalculate()}
                    disabled={isPending}
                    isLoading={operation === "calculate"}
                    className="flex-1 whitespace-normal text-xs sm:flex-none sm:text-sm"
                  >
                    <IconCheck size={14} className="mr-1.5" />
                    {t("workspace.saveAndCalculate")}
                  </Button>
                </div>
              </div>
              )}
            </div>

            {detailOpen && <div className="fixed inset-0 z-20 bg-black/40 min-[992px]:hidden" aria-hidden="true" onClick={() => setDetailOpen(false)} />}
            {/* Inspector: full-height column on desktop, bottom sheet on phones */}
            <aside aria-label={t("workspace.detail")} className={cn(
              "fixed inset-x-0 bottom-0 top-[12vh] z-30 flex min-h-0 flex-col border-t-2 border-erp-navy bg-erp-surface shadow-2xl transition-transform motion-reduce:transition-none",
              "min-[992px]:static min-[992px]:inset-auto min-[992px]:z-auto min-[992px]:w-[clamp(360px,34vw,460px)] min-[992px]:shrink-0 min-[992px]:translate-y-0 min-[992px]:border-l min-[992px]:border-t-0 min-[992px]:shadow-none min-[992px]:visible",
              detailOpen ? "translate-y-0 visible" : "translate-y-full invisible"
            )}>
              <div className="flex min-h-12 shrink-0 items-center gap-2 border-b border-erp-border bg-erp-surface-subtle py-1 pl-3 pr-1">
                <div className="min-w-0 flex-1">
                  <h2 className="truncate text-sm font-bold text-erp-navy">{t("workspace.detail")}{selectedWork && <span className="ml-2 font-mono text-xs font-normal text-erp-text-muted">{selectedWork.code}</span>}</h2>
                </div>
                <Button size="icon" variant="ghost" className="min-[992px]:hidden" onClick={() => setDetailOpen(false)} aria-label={tc("actions.close")} title={tc("actions.close")} icon={<IconClose size={18} />} />
              </div>
              {!selectedWork && <p className="p-6 text-sm text-erp-text-muted">{t("workspace.selectWork")}</p>}
              <div ref={setInspector} data-estimate-scroll className="min-h-0 flex-1 overflow-auto" />
            </aside>
          </fieldset>
          <EstimateHistoryModal isOpen={historyOpen} onClose={() => setHistoryOpen(false)} estimateId={estimate.id ?? ""} />
          <EstimateItemCatalogModal isOpen={catalogTarget !== null} onClose={() => setCatalogTarget(null)} onSelectItems={handleInsertFromCatalog} currency={currency} branchId={estimate.branchId} singleSelect={catalogTarget?.mode === "workItem"} />
        </EstimateWorkspaceContext.Provider>
      </FormProvider>
      <ConfirmDialog />
    </Drawer>
  );
}
