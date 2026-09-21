"use client";

import React, { useEffect, useState, useMemo } from "react";
import { useForm, FormProvider, useFieldArray, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { IconSave, IconCheck, IconPlus } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { useConfirm } from "@/hooks/useConfirm";
import { useKeyboardShortcut } from "@/hooks/useKeyboardShortcut";
import type {
  EstimateDetailResponse,
  UpdateEstimateDraftRequest,
  CalculateEstimateRequest,
} from "@/lib/api/api-client";
import {
  useUpdateEstimateDraft,
  useCalculateEstimate,
} from "@/features/estimates/api/estimate-queries";
import { ApiError } from "@/lib/api/api-error";
import {
  estimateWorkspaceSchema,
  type EstimateWorkspaceFormData,
} from "../schemas/estimate-workspace-schema";
import {
  calculateLiveWorkspaceHud,
  calculateWorkItemSummary,
} from "../utils/estimate-calculations";
import {
  isEstimateCalculationSnapshot,
  exportEstimateSnapshotCsv,
  type EstimateCalculationSnapshot,
} from "../utils/estimate-export";
import { EstimateWorkspaceHud } from "./estimate-workspace-hud";
import { EstimateSectionCard } from "./estimate-section-card";

interface EstimateWorkspaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  opportunityId?: string;
  estimate: EstimateDetailResponse;
}

export function EstimateWorkspaceDrawer({
  isOpen,
  onClose,
  opportunityId,
  estimate,
}: EstimateWorkspaceDrawerProps) {
  const t = useTranslations("estimates");
  const tc = useTranslations("common");
  const { toast } = useToast();
  const { confirm, ConfirmDialog } = useConfirm();

  const [searchQuery, setSearchQuery] = useState("");
  const [filterMode, setFilterMode] = useState<"all" | "low_margin">("all");

  const currentRevision = estimate.currentRevision;
  const revisionId = currentRevision?.id ?? "";
  const currency = currentRevision?.currency ?? "THB";

  const [activeRowVersion, setActiveRowVersion] = useState<string | null>(null);

  // Keep activeRowVersion in sync when currentRevision changes from query updates
  useEffect(() => {
    if (currentRevision?.rowVersion) {
      setActiveRowVersion(currentRevision.rowVersion);
    }
  }, [currentRevision?.rowVersion]);

  const effectiveRowVersion = activeRowVersion || currentRevision?.rowVersion;

  const methods = useForm<EstimateWorkspaceFormData>({
    resolver: zodResolver(estimateWorkspaceSchema),
    defaultValues: {
      discountAmount: 0,
      sections: [],
    },
  });

  const {
    control,
    handleSubmit,
    reset,
    watch,
    getValues,
    formState: { isDirty },
  } = methods;

  const { fields, append, remove } = useFieldArray({
    control,
    name: "sections",
  });

  // Populate form from currentRevision
  useEffect(() => {
    if (currentRevision && !isDirty) {
      const initialSections = (currentRevision.sections || []).map((s, sIdx) => ({
        id: s.id,
        code: s.code || `SEC-${sIdx + 1}`,
        nameTh: s.nameTh || "",
        nameEn: s.nameEn || "",
        sortOrder: s.sortOrder || sIdx + 1,
        workItems: (s.workItems || []).map((w, wIdx) => ({
          id: w.id,
          code: w.code || `ITM-${s.code || sIdx + 1}-${wIdx + 1}`,
          descriptionTh: w.descriptionTh || "",
          descriptionEn: w.descriptionEn || "",
          quantity: w.quantity || 1,
          unitCode: w.unitCode || "lot",
          sellingRuleType: w.sellingRuleType || "margin",
          sellingRuleValue: w.sellingRuleValue ?? 20,
          sortOrder: w.sortOrder || wIdx + 1,
          costComponents: (w.costComponents || []).map((c, cIdx) => ({
            id: c.id,
            type: c.type || "material",
            description: c.description || "",
            quantity: c.quantity || 1,
            unitCode: c.unitCode || "lot",
            unitCost: c.unitCost || 0,
            currency: c.currency || currency,
            sortOrder: c.sortOrder || cIdx + 1,
          })),
        })),
      }));

      reset({
        discountAmount: currentRevision.discountAmount || 0,
        sections: initialSections,
      });
    }
  }, [currentRevision, reset, currency, isDirty]);

  const updateDraftMutation = useUpdateEstimateDraft(opportunityId ?? "", estimate.id ?? "", revisionId);
  const calculateMutation = useCalculateEstimate(opportunityId ?? "", estimate.id ?? "", revisionId);
  const isPending = updateDraftMutation.isPending || calculateMutation.isPending;

  // Live real-time calculations for HUD
  const watchedSections = watch("sections");
  const watchedDiscount = watch("discountAmount");
  const liveHudMetrics = calculateLiveWorkspaceHud(watchedSections, watchedDiscount);

  // Keyboard shortcut: Cmd+S / Ctrl+S to save draft
  useKeyboardShortcut(
    {
      key: "s",
      ctrlOrMeta: true,
      enabled: isOpen && !isPending,
      preventDefault: true,
    },
    () => {
      handleSaveDraft();
    }
  );

  // Verified Server Calculation Snapshot for CSV Export
  const parsedSnapshot = useMemo<EstimateCalculationSnapshot | null>(() => {
    if (!currentRevision?.calculationSnapshotJson) {
      return null;
    }
    try {
      const raw: unknown = JSON.parse(currentRevision.calculationSnapshotJson);
      return isEstimateCalculationSnapshot(raw) ? raw : null;
    } catch {
      return null;
    }
  }, [currentRevision?.calculationSnapshotJson]);

  const [isExporting, setIsExporting] = useState(false);

  const handleExportSnapshot = React.useCallback(() => {
    if (!parsedSnapshot) return;
    setIsExporting(true);
    try {
      const filename = `BOQ_${estimate.number || "Estimate"}_R${currentRevision?.revisionNo ?? 1}_snapshot`;
      exportEstimateSnapshotCsv(filename, parsedSnapshot);
    } finally {
      setIsExporting(false);
    }
  }, [estimate.number, currentRevision?.revisionNo, parsedSnapshot]);

  // Filtered Section Indices for Live Search & Low Margin Filter
  const visibleSectionIndices = useMemo(() => {
    const query = searchQuery.trim().toLowerCase();
    if (!query && filterMode === "all") {
      return fields.map((_, idx) => idx);
    }

    return fields
      .map((_, idx) => idx)
      .filter((sIdx) => {
        const sec = watchedSections?.[sIdx];
        if (!sec) return false;

        const matchSec =
          sec.code?.toLowerCase().includes(query) ||
          sec.nameTh?.toLowerCase().includes(query) ||
          sec.nameEn?.toLowerCase().includes(query);

        const matchingItems = (sec.workItems || []).filter((w) => {
          if (!w) return false;
          const summary = calculateWorkItemSummary(w);

          if (filterMode === "low_margin" && summary.marginRate >= 30) {
            return false;
          }

          if (!query) return true;

          const matchItem =
            w.code?.toLowerCase().includes(query) ||
            w.descriptionTh?.toLowerCase().includes(query) ||
            w.descriptionEn?.toLowerCase().includes(query);

          const matchCost = (w.costComponents || []).some(
            (c) => c?.description?.toLowerCase().includes(query)
          );

          return matchItem || matchCost;
        });

        if (filterMode === "low_margin") {
          return matchingItems.length > 0;
        }

        return matchSec || matchingItems.length > 0;
      });
  }, [fields, watchedSections, searchQuery, filterMode]);

  // Safe Close Handler (Unsaved Changes Guard)
  const handleAttemptClose = async () => {
    if (isDirty) {
      const ok = await confirm({
        title: t("discardChangesTitle"),
        message: t("discardChangesMessage"),
        confirmText: tc("actions.discard"),
        cancelText: tc("actions.cancel"),
        variant: "warning",
      });
      if (ok) {
        onClose();
      }
    } else {
      onClose();
    }
  };

  const handleAddSection = () => {
    const nextIdx = fields.length + 1;
    append({
      code: `SEC-${nextIdx}`,
      nameTh: t("defaultSectionNameTh", { number: nextIdx }),
      nameEn: t("defaultSectionNameEn", { number: nextIdx }),
      sortOrder: nextIdx,
      workItems: [],
    });
  };

  const handleRemoveSection = (sectionIndex: number) => {
    remove(sectionIndex);
  };

  const buildUpdatePayload = (data: EstimateWorkspaceFormData, targetVersion?: string): UpdateEstimateDraftRequest => {
    return {
      expectedRevisionVersion: targetVersion || effectiveRowVersion || "",
      sections: (data.sections || []).map((s, sIdx) => ({
        id: s.id || null,
        code: s.code,
        nameTh: s.nameTh,
        nameEn: s.nameEn,
        sortOrder: sIdx + 1,
        workItems: (s.workItems || []).map((w, wIdx) => ({
          id: w.id || null,
          code: w.code,
          descriptionTh: w.descriptionTh,
          descriptionEn: w.descriptionEn,
          quantity: Number(w.quantity) || 1,
          unitCode: w.unitCode,
          sellingRuleType: w.sellingRuleType,
          sellingRuleValue: Number(w.sellingRuleValue) || 0,
          sortOrder: wIdx + 1,
          costComponents: (w.costComponents || []).map((c, cIdx) => ({
            id: c.id || null,
            type: c.type,
            description: c.description,
            quantity: Number(c.quantity) || 1,
            unitCode: c.unitCode,
            unitCost: Number(c.unitCost) || 0,
            currency: c.currency || currency,
            sortOrder: cIdx + 1,
          })),
        })),
      })),
    };
  };

  const handleSaveDraft = async () => {
    if (!effectiveRowVersion) return;
    try {
      const formData = getValues();
      const payload = buildUpdatePayload(formData, effectiveRowVersion);
      const updatedRev = await updateDraftMutation.mutateAsync({
        payload,
        ifMatch: `"${effectiveRowVersion}"`,
      });
      if (updatedRev?.rowVersion) {
        setActiveRowVersion(updatedRev.rowVersion);
      }
      toast.success(t("saveDraftSuccess"));

      const updatedSections = (updatedRev.sections || []).map((s, sIdx) => ({
        id: s.id,
        code: s.code || `SEC-${sIdx + 1}`,
        nameTh: s.nameTh || "",
        nameEn: s.nameEn || "",
        sortOrder: s.sortOrder || sIdx + 1,
        workItems: (s.workItems || []).map((w, wIdx) => ({
          id: w.id,
          code: w.code || `ITM-${s.code || sIdx + 1}-${wIdx + 1}`,
          descriptionTh: w.descriptionTh || "",
          descriptionEn: w.descriptionEn || "",
          quantity: w.quantity || 1,
          unitCode: w.unitCode || "lot",
          sellingRuleType: w.sellingRuleType || "margin",
          sellingRuleValue: w.sellingRuleValue ?? 20,
          sortOrder: w.sortOrder || wIdx + 1,
          costComponents: (w.costComponents || []).map((c, cIdx) => ({
            id: c.id,
            type: c.type || "material",
            description: c.description || "",
            quantity: c.quantity || 1,
            unitCode: c.unitCode || "lot",
            unitCost: c.unitCost || 0,
            currency: c.currency || currency,
            sortOrder: c.sortOrder || cIdx + 1,
          })),
        })),
      }));

      reset({
        discountAmount: Number(formData.discountAmount) || 0,
        sections: updatedSections,
      });
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.code === "ESTIMATE_VERSION_CONFLICT") {
          toast.error(t("versionConflict"));
        } else if (err.code === "ESTIMATE_INVALID_STATE") {
          toast.error(t("invalidState"));
        } else {
          toast.error(t("saveDraftFailed"));
        }
      } else {
        toast.error(tc("alerts.errorTitle"));
      }
    }
  };

  const handleRecalculate = async () => {
    if (!effectiveRowVersion) return;
    try {
      const formData = getValues();
      const calcPayload: CalculateEstimateRequest = {
        expectedRevisionVersion: effectiveRowVersion,
        discountAmount: Number(formData.discountAmount) || 0,
      };
      const calculatedRev = await calculateMutation.mutateAsync(calcPayload);
      if (calculatedRev?.rowVersion) {
        setActiveRowVersion(calculatedRev.rowVersion);
      }

      toast.success(t("calculateSuccess"));
      reset(formData);
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.code === "ESTIMATE_VERSION_CONFLICT") {
          toast.error(t("versionConflict"));
        } else if (err.code === "ESTIMATE_INVALID_STATE") {
          toast.error(t("invalidState"));
        } else {
          toast.error(t("calculateFailed"));
        }
      } else {
        toast.error(tc("alerts.errorTitle"));
      }
    }
  };

  return (
    <Drawer
      isOpen={isOpen}
      onClose={handleAttemptClose}
      title={t("workspaceTitle")}
      size="full"
      closeOnOverlayClick={false}
      noPadding
    >
      <FormProvider {...methods}>
        <div className="flex flex-col h-full min-h-0 bg-erp-canvas">
          {/* 2-Tier Architectural Title & Financial HUD */}
          <div className="shrink-0">
            <EstimateWorkspaceHud
              estimateNumber={estimate.number || ""}
              revisionNo={currentRevision?.revisionNo ?? 1}
              status={currentRevision?.status ?? "draft"}
              currency={currency}
              hudMetrics={liveHudMetrics}
            />
          </div>

          {/* Main Scrollable Content */}
          <div className="flex-1 overflow-y-auto min-h-0 p-4 sm:p-6 space-y-5">
            {/* Toolbar: Uniform 36px Height across all interactive controls */}
            <div className="flex flex-wrap justify-between items-center bg-erp-surface p-3 sm:p-4 border border-erp-border shadow-sm gap-3">
              <div className="flex items-center gap-2.5 flex-1 min-w-[280px]">
                {/* Live Search Input (Height: 36px / h-9) */}
                <div className="relative w-56 sm:w-72">
                  <Input
                    placeholder={t("searchPlaceholder")}
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    wrapperClassName="mb-0"
                    className="text-xs h-9 min-h-[36px] pl-8 py-1"
                  />
                  <span className="absolute left-2.5 top-1/2 -translate-y-1/2 text-erp-text-muted pointer-events-none">
                    <svg
                      className="w-3.5 h-3.5"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="2"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    >
                      <circle cx="11" cy="11" r="8" />
                      <line x1="21" y1="21" x2="16.65" y2="16.65" />
                    </svg>
                  </span>
                </div>

                {/* Filter Mode Toggle (Height: 36px / h-9) */}
                <div className="inline-flex h-9 border border-erp-border bg-erp-bg-neutral text-xs overflow-hidden">
                  <button
                    type="button"
                    onClick={() => setFilterMode("all")}
                    className={`px-3 h-full font-medium transition-colors ${
                      filterMode === "all"
                        ? "bg-erp-navy text-white"
                        : "text-erp-text-secondary hover:text-erp-text-primary"
                    }`}
                  >
                    {t("filterAll")}
                  </button>
                  <button
                    type="button"
                    onClick={() => setFilterMode("low_margin")}
                    className={`px-3 h-full font-medium transition-colors ${
                      filterMode === "low_margin"
                        ? "bg-rose-600 text-white"
                        : "text-rose-700 hover:bg-rose-50"
                    }`}
                  >
                    {t("filterWarning")}
                  </button>
                </div>
              </div>

              {/* Action Buttons: Export CSV & Add Section (Both h-9) */}
              <div className="flex items-center gap-2">
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleExportSnapshot}
                  disabled={isExporting || !parsedSnapshot}
                  isLoading={isExporting}
                  title={!parsedSnapshot ? t("exportSnapshotRequired") : undefined}
                  className="!rounded-none h-9 text-xs border-erp-border text-erp-text-secondary hover:bg-erp-surface-subtle disabled:opacity-50"
                >
                  <svg
                    className="w-3.5 h-3.5 mr-1.5 text-erp-navy"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  >
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                    <polyline points="7 10 12 15 17 10" />
                    <line x1="12" y1="15" x2="12" y2="3" />
                  </svg>
                  {isExporting ? t("exporting") : t("exportCsv")}
                </Button>

                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleAddSection}
                  className="!rounded-none h-9 text-xs border-erp-navy text-erp-navy hover:bg-erp-surface-subtle"
                >
                  <IconPlus size={14} className="mr-1.5" />
                  {t("addSection")}
                </Button>
              </div>
            </div>

            {/* Sections List */}
            {fields.length === 0 ? (
              <div className="bg-erp-surface border border-dashed border-erp-border p-12 text-center text-erp-text-muted">
                <p className="mb-4">{t("noSectionsYet")}</p>
                <Button
                  type="button"
                  variant="primary"
                  size="sm"
                  onClick={handleAddSection}
                  className="!rounded-none h-9 bg-erp-navy hover:bg-erp-navy-hover text-white"
                >
                  <IconPlus size={14} className="mr-1.5" />
                  {t("addSection")}
                </Button>
              </div>
            ) : visibleSectionIndices.length === 0 ? (
              <div className="bg-erp-surface border border-dashed border-erp-border p-12 text-center text-erp-text-muted">
                <p>{t("noMatchingItems")}</p>
              </div>
            ) : (
              <div className="space-y-5">
                {visibleSectionIndices.map((sIdx) => (
                  <EstimateSectionCard
                    key={fields[sIdx]?.id || sIdx}
                    sectionIndex={sIdx}
                    currency={currency}
                    onRemoveSection={handleRemoveSection}
                  />
                ))}
              </div>
            )}
          </div>

          {/* Sticky Drawer Footer Action Bar: Perfectly Aligned 36px Controls */}
          <div className="shrink-0 bg-erp-surface border-t border-erp-border px-4 sm:px-6 py-3 flex flex-wrap items-center justify-between gap-4 shadow-lg">
            {/* Discount Setting */}
            <div className="flex items-center gap-2.5">
              <span className="text-xs font-bold text-erp-text-main">
                {t("discount")}:
              </span>
              <Controller
                control={control}
                name="discountAmount"
                render={({ field }) => (
                  <Input
                    id="estimate-discount-input"
                    type="number"
                    step="100"
                    min="0"
                    value={field.value ?? 0}
                    onChange={(e) => field.onChange(Number(e.target.value))}
                    wrapperClassName="mb-0"
                    className="w-28 sm:w-32 text-xs text-right font-mono h-9 min-h-[36px] py-1"
                  />
                )}
              />
              <span className="text-xs font-mono text-erp-text-muted">{currency}</span>
            </div>

            {/* Action Buttons with Keyboard Shortcut Hint */}
            <div className="flex items-center gap-2.5">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleSaveDraft}
                disabled={isPending}
                isLoading={updateDraftMutation.isPending}
                className="!rounded-none h-9 text-xs border-erp-border text-erp-text-main hover:bg-erp-surface-subtle"
                title={t("saveDraftShortcut")}
              >
                <IconSave size={14} className="mr-1.5" />
                <span className="mr-1.5">{t("saveDraftAction")}</span>
                <span className="text-[10px] font-mono px-1 py-0.2 bg-erp-bg-neutral text-erp-text-muted border border-border">
                  ⌘S
                </span>
              </Button>
              <Button
                type="button"
                variant="primary"
                size="sm"
                onClick={handleRecalculate}
                disabled={isPending}
                isLoading={calculateMutation.isPending}
                className="!rounded-none h-9 text-xs bg-erp-navy hover:bg-erp-navy-hover text-white"
              >
                <IconCheck size={14} className="mr-1.5" />
                {t("calculateAction")}
              </Button>
            </div>
          </div>
        </div>
      </FormProvider>

      <ConfirmDialog />
    </Drawer>
  );
}
