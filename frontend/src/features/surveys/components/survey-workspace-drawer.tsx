"use client";

import React, { useState, useEffect } from "react";
import { useForm, FormProvider, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Tooltip } from "@/components/ui/Tooltip";
import { FormTabs, type FormTabItem } from "@/components/forms/FormTabs";
import {
  IconSave,
  IconCheck,
  IconClose,
  IconMaximize,
  IconMinimize,
} from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { cn } from "@/lib/utils/cn";
import {
  type SiteSurveyResponse,
  type SiteSurveyAreaResponse,
  type SiteSurveyMeasurementResponse,
  type UpdateSurveyDraftRequest,
  type UpdateSurveyAreaRequest,
  type UpdateSurveyMeasurementRequest,
  type SiteSurveyChecklistResultResponse,
  type SiteSurveyEvidenceResponse,
} from "@/lib/api/api-client";
import { useUpdateSurveyDraft, useMarkSurveyReady, useSurveyTemplateVersions } from "@/features/surveys/api/survey-queries";
import {
  surveyWorkspaceSchema,
  validateSurveyReadiness,
  type SurveyWorkspaceFormData,
} from "../schemas/survey-workspace-schema";
import { getDefaultVisitDateTime } from "../hooks/use-measurement-metadata";
import { SurveyScopeSection } from "./survey-scope-section";
import { SurveyAreasSection } from "./survey-areas-section";
import { SurveyNotesSection } from "./survey-notes-section";
import { SurveyChecklistSection } from "./survey-checklist-section";
import { SurveyEvidenceSection } from "./survey-evidence-section";

interface SurveyWorkspaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  opportunityId: string;
  survey: SiteSurveyResponse;
  currentOpportunityVersion: string;
}

export function SurveyWorkspaceDrawer({
  isOpen,
  onClose,
  opportunityId,
  survey,
  currentOpportunityVersion,
}: SurveyWorkspaceDrawerProps) {
  const t = useTranslations("surveys");
  const tc = useTranslations("common");
  const { toast } = useToast();

  const currentRevision = survey.currentRevision;
  const revisionId = currentRevision?.id ?? "";
  const isReady = currentRevision?.status === "ready";

  // Fullscreen state (Side Drawer vs Fullscreen Studio)
  const [isFullScreen, setIsFullScreen] = useState(false);

  // Tabs navigation state
  type SurveyWorkspaceTab = "areas" | "scope" | "checklist" | "notes";
  const [activeTab, setActiveTab] = useState<SurveyWorkspaceTab>("areas");
  const [tabErrorMap, setTabErrorMap] = useState<Record<SurveyWorkspaceTab, boolean>>({
    areas: false,
    scope: false,
    checklist: false,
    notes: false,
  });

  // Confirmation modals state
  const [isConfirmMarkReadyOpen, setIsConfirmMarkReadyOpen] = useState(false);
  const [isConfirmDiscardOpen, setIsConfirmDiscardOpen] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);

  const updateDraftMutation = useUpdateSurveyDraft(opportunityId, survey.id ?? "", revisionId);
  const markReadyMutation = useMarkSurveyReady(opportunityId, survey.id ?? "", revisionId);

  // The revision's template decides which checklist items and how much evidence it needs.
  const templatesQuery = useSurveyTemplateVersions();
  const template = templatesQuery.data?.items?.find((item) => item.code === currentRevision?.surveyTemplateVersion) ?? null;
  const requiredChecklistItems = template?.requiredChecklistItems ?? [];
  const minimumEvidenceCount = template?.minimumEvidenceCount ?? 0;
  const showChecklist = requiredChecklistItems.length > 0;
  const showEvidence = minimumEvidenceCount > 0 || (currentRevision?.evidence?.length ?? 0) > 0;

  // Initialize React Hook Form
  const methods = useForm<SurveyWorkspaceFormData>({
    resolver: zodResolver(surveyWorkspaceSchema),
    defaultValues: {
      visitedAt: getDefaultVisitDateTime(),
      scopeSummary: "",
      assumptions: [],
      constraints: [],
      missingDetails: [],
      areas: [],
      checklist: [],
      evidence: [],
    },
  });

  const { reset, getValues, formState: { isDirty } } = methods;

  // Watch fields for reactive HUD metrics and tab badges
  const watchedAreas = useWatch({ control: methods.control, name: "areas" }) || [];
  const watchedEvidence = useWatch({ control: methods.control, name: "evidence" }) || [];
  const watchedChecklist = useWatch({ control: methods.control, name: "checklist" }) || [];
  const watchedAssumptions = useWatch({ control: methods.control, name: "assumptions" }) || [];
  const watchedConstraints = useWatch({ control: methods.control, name: "constraints" }) || [];
  const watchedMissing = useWatch({ control: methods.control, name: "missingDetails" }) || [];

  const totalMeasurementPoints = watchedAreas.reduce(
    (sum, a) => sum + (a?.measurements?.length || 0),
    0
  );
  const checklistPassedCount = watchedChecklist.filter((c) => c?.result === "pass").length;
  const totalNotesCount =
    (watchedAssumptions?.length || 0) +
    (watchedConstraints?.length || 0) +
    (watchedMissing?.length || 0);

  // Populate data when revision changes
  useEffect(() => {
    if (currentRevision) {
      let initialVisitedAt = getDefaultVisitDateTime();
      if (currentRevision.visitedAtUtc) {
        try {
          const d = new Date(currentRevision.visitedAtUtc);
          initialVisitedAt = d.toISOString().slice(0, 16);
        } catch {
          initialVisitedAt = getDefaultVisitDateTime();
        }
      }

      const initialAreas = (currentRevision.areas && currentRevision.areas.length > 0)
        ? currentRevision.areas.map((a: SiteSurveyAreaResponse, aIdx: number) => ({
            id: a.id,
            code: a.code ?? `AREA-${String(aIdx + 1).padStart(2, "0")}`,
            name: a.name ?? "",
            description: a.description ?? "",
            sortOrder: a.sortOrder ?? aIdx + 1,
            measurements: (a.measurements ?? []).map(
              (m: SiteSurveyMeasurementResponse, mIdx: number) => ({
                id: m.id,
                measurementType: m.measurementType ?? "width",
                value: m.value ?? 0,
                unitCode: m.unitCode ?? "m",
                captureMethod: m.captureMethod ?? "measured",
                notes: m.notes ?? "",
                sortOrder: m.sortOrder ?? mIdx + 1,
              })
            ),
          }))
        : [];

      reset({
        visitedAt: initialVisitedAt,
        scopeSummary: currentRevision.scopeSummary ?? "",
        assumptions: currentRevision.assumptions ? [...currentRevision.assumptions] : [],
        constraints: currentRevision.constraints ? [...currentRevision.constraints] : [],
        missingDetails: currentRevision.missingDetails ? [...currentRevision.missingDetails] : [],
        areas: initialAreas,
        checklist: (currentRevision.checklist ?? []).map((c: SiteSurveyChecklistResultResponse) => ({
          itemCode: c.itemCode ?? "",
          result: (c.result as SurveyWorkspaceFormData["checklist"][number]["result"]) ?? "pass",
          note: c.note ?? null,
        })),
        evidence: (currentRevision.evidence ?? []).map((e: SiteSurveyEvidenceResponse, idx: number) => ({
          fileId: e.fileId ?? "",
          kind: (e.kind as SurveyWorkspaceFormData["evidence"][number]["kind"]) ?? "site_photo",
          caption: e.caption ?? null,
          sortOrder: e.sortOrder ?? idx + 1,
        })),
      });
    }
  }, [currentRevision, reset]);

  // Build payload for API
  const buildUpdatePayload = (data: SurveyWorkspaceFormData): UpdateSurveyDraftRequest => {
    const areaRequests: UpdateSurveyAreaRequest[] = data.areas.map((a, aIdx) => {
      const measurementRequests: UpdateSurveyMeasurementRequest[] = a.measurements.map(
        (m, mIdx) => ({
          id: m.id || null,
          measurementType: m.measurementType,
          value: Number(m.value),
          unitCode: m.unitCode,
          captureMethod: m.captureMethod,
          notes: m.notes || null,
          sortOrder: m.sortOrder ?? mIdx + 1,
        })
      );

      return {
        id: a.id || null,
        code: a.code,
        name: a.name,
        description: a.description || null,
        sortOrder: a.sortOrder ?? aIdx + 1,
        measurements: measurementRequests,
      };
    });

    return {
      expectedRevisionVersion: currentRevision?.rowVersion ?? "",
      visitedAtUtc: data.visitedAt ? new Date(data.visitedAt).toISOString() : null,
      scopeSummary: data.scopeSummary.trim() || null,
      assumptions: data.assumptions,
      constraints: data.constraints,
      missingDetails: data.missingDetails,
      areas: areaRequests,
      // Only send what the template uses; omitted lists leave the stored values untouched.
      checklist: showChecklist
        ? data.checklist.map((c) => ({ itemCode: c.itemCode, result: c.result, note: c.note?.trim() || null }))
        : undefined,
      evidence: showEvidence
        ? data.evidence.map((e, idx) => ({
            fileId: e.fileId,
            kind: e.kind,
            caption: e.caption?.trim() || null,
            sortOrder: e.sortOrder ?? idx + 1,
          }))
        : undefined,
    };
  };

  // Safe Close Handler with Unsaved Changes Guard
  const handleAttemptClose = () => {
    if (isDirty && !isReady) {
      setIsConfirmDiscardOpen(true);
    } else {
      onClose();
    }
  };

  const handleConfirmDiscard = () => {
    setIsConfirmDiscardOpen(false);
    onClose();
  };

  // Save Draft Handler
  const handleSaveDraft = async () => {
    if (!currentRevision?.rowVersion) return;
    setValidationError(null);

    try {
      const formData = getValues();
      const payload = buildUpdatePayload(formData);
      await updateDraftMutation.mutateAsync({
        payload,
        ifMatch: `"${currentRevision.rowVersion}"`,
      });
      toast.success(t("saveDraftSuccess"));
      reset(formData); // Clear dirty state
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : tc("alerts.errorTitle");
      toast.error(errorMsg);
    }
  };

  // Validate & Open Confirmation for Mark Ready
  const handleOpenConfirmMarkReady = () => {
    const formData = getValues();
    const result = validateSurveyReadiness(
      formData,
      template ? { requiredChecklistItems, minimumEvidenceCount } : null
    );

    if (!result.isValid && result.errorKey) {
      const translatedMsg = result.errorParams
        ? t(result.errorKey as Parameters<typeof t>[0], result.errorParams)
        : t(result.errorKey as Parameters<typeof t>[0]);
      setValidationError(translatedMsg);

      // Determine failing tab to auto-switch and display red dot indicator
      let failingTab: SurveyWorkspaceTab = "areas";
      if (
        result.errorKey.startsWith("readinessErrors.missingVisit") ||
        result.errorKey.startsWith("readinessErrors.missingScope")
      ) {
        failingTab = "scope";
      } else if (
        result.errorKey.startsWith("readinessErrors.missingChecklistItem") ||
        result.errorKey.startsWith("readinessErrors.checklistNoteRequired") ||
        result.errorKey.startsWith("readinessErrors.missingEvidence")
      ) {
        failingTab = "checklist";
      } else if (
        result.errorKey.startsWith("readinessErrors.missingArea") ||
        result.errorKey.startsWith("readinessErrors.missingAreaName") ||
        result.errorKey.startsWith("readinessErrors.missingMeasurement")
      ) {
        failingTab = "areas";
      }

      const nextTabErrorMap: Record<SurveyWorkspaceTab, boolean> = {
        areas: false,
        scope: false,
        checklist: false,
        notes: false,
      };
      nextTabErrorMap[failingTab] = true;
      setTabErrorMap(nextTabErrorMap);
      setActiveTab(failingTab);
      return;
    }

    setValidationError(null);
    setTabErrorMap({ areas: false, scope: false, checklist: false, notes: false });
    setIsConfirmMarkReadyOpen(true);
  };

  // Confirm Mark Ready Handler
  const handleConfirmMarkReady = async () => {
    if (!currentRevision?.rowVersion) return;

    try {
      const formData = getValues();
      const draftPayload = buildUpdatePayload(formData);
      const updatedRev = await updateDraftMutation.mutateAsync({
        payload: draftPayload,
        ifMatch: `"${currentRevision.rowVersion}"`,
      });

      const markReadyVersion = updatedRev?.rowVersion ?? currentRevision.rowVersion;
      await markReadyMutation.mutateAsync({
        payload: {
          expectedRevisionVersion: markReadyVersion,
          expectedOpportunityVersion: currentOpportunityVersion,
        },
        idempotencyKey: `mark-ready-${revisionId}-${Date.now()}`,
      });

      toast.success(t("markReadySuccess"));
      setIsConfirmMarkReadyOpen(false);
      onClose();
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : tc("alerts.errorTitle");
      toast.error(errorMsg);
    }
  };

  const isSaving = updateDraftMutation.isPending || markReadyMutation.isPending;

  const tabs: FormTabItem<SurveyWorkspaceTab>[] = [
    {
      id: "areas",
      label: t("workspaceTabs.areas"),
      count: watchedAreas.length,
      hasError: tabErrorMap.areas,
    },
    {
      id: "scope",
      label: t("workspaceTabs.scope"),
      hasError: tabErrorMap.scope,
    },
    ...(showChecklist || showEvidence
      ? [
          {
            id: "checklist" as SurveyWorkspaceTab,
            label: t("workspaceTabs.checklist"),
            count: watchedEvidence.length,
            hasError: tabErrorMap.checklist,
          },
        ]
      : []),
    {
      id: "notes",
      label: t("workspaceTabs.notes"),
      count: totalNotesCount,
      hasError: tabErrorMap.notes,
    },
  ];

  return (
    <FormProvider {...methods}>
      <Drawer
        isOpen={isOpen}
        onClose={handleAttemptClose}
        title={`${t("workspaceTitle")} — ${survey.surveyNumber}`}
        description={t("workspaceDesc")}
        size={isFullScreen ? "full" : "xl"}
        headerActions={
          <Tooltip
            content={isFullScreen ? t("collapseFullscreen") : t("expandFullscreen")}
            position="bottom"
          >
            <button
              type="button"
              onClick={() => setIsFullScreen((prev) => !prev)}
              className="flex min-h-11 min-w-11 items-center justify-center text-erp-text-muted hover:text-erp-text-main hover:bg-erp-surface-muted transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy cursor-pointer"
              title={isFullScreen ? t("collapseFullscreen") : t("expandFullscreen")}
              aria-label={isFullScreen ? t("collapseFullscreen") : t("expandFullscreen")}
            >
              {isFullScreen ? (
                <IconMinimize size={20} strokeWidth={2} />
              ) : (
                <IconMaximize size={20} strokeWidth={2} />
              )}
            </button>
          </Tooltip>
        }
        footer={
          <div className="flex items-center justify-between gap-2 sm:gap-3 w-full">
            <Button
              variant="outline"
              onClick={handleAttemptClose}
              disabled={isSaving}
              className="min-h-[44px] min-w-[44px] px-3 sm:px-4 flex items-center justify-center gap-2"
              title={tc("actions.cancel")}
              aria-label={tc("actions.cancel")}
            >
              <IconClose size={18} strokeWidth={2} />
              <span className="hidden sm:inline">{tc("actions.cancel")}</span>
            </Button>

            {!isReady && (
              <div className="flex items-center gap-2 sm:gap-3">
                <Button
                  variant="secondary"
                  onClick={handleSaveDraft}
                  disabled={isSaving}
                  className="min-h-[44px] min-w-[44px] px-3 sm:px-4 flex items-center justify-center gap-2"
                  title={t("saveDraftAction")}
                  aria-label={t("saveDraftAction")}
                >
                  <IconSave size={18} strokeWidth={2} />
                  <span className="hidden sm:inline">{t("saveDraftAction")}</span>
                </Button>
                <Button
                  variant="primary"
                  onClick={handleOpenConfirmMarkReady}
                  disabled={isSaving}
                  className="min-h-[44px] min-w-[44px] px-3 sm:px-4 flex items-center justify-center gap-2 font-semibold"
                  title={t("markReadyAction")}
                  aria-label={t("markReadyAction")}
                >
                  <IconCheck size={18} strokeWidth={2} />
                  <span className="hidden sm:inline">{t("markReadyAction")}</span>
                </Button>
              </div>
            )}
          </div>
        }
      >
        <div className="space-y-5 pb-6">
          {/* Architectural Survey HUD Ribbon */}
          <div className="flex flex-wrap items-center justify-between gap-3 p-3 sm:p-4 border border-erp-border bg-erp-surface-subtle">
            <div className="flex items-center gap-3">
              <div>
                <div className="text-[11px] font-bold text-erp-text-muted uppercase tracking-wider">
                  {t("surveyNumberLabel")}
                </div>
                <div className="text-base sm:text-lg font-mono font-bold text-erp-navy">
                  {survey.surveyNumber}
                </div>
              </div>
              <div className="h-7 w-px bg-erp-border hidden sm:block" />
              <div className="flex items-center gap-2">
                <span className="text-xs sm:text-sm font-medium text-erp-text-body">
                  {t("revisionBadge", { number: currentRevision?.revisionNumber ?? 1 })}
                </span>
                <StatusBadge
                  variant={isReady ? "success" : "neutral"}
                  label={isReady ? t("revisionStatuses.ready") : t("revisionStatuses.draft")}
                />
              </div>
            </div>

            {/* Live Readiness Metric Chips */}
            <div className="flex flex-wrap items-center gap-2 text-xs">
              <div className="px-2.5 py-1 border border-erp-border bg-erp-surface flex items-center gap-1.5 shadow-xs">
                <span className="font-semibold text-erp-text-main">📐</span>
                <span className="font-bold text-erp-navy">
                  {t("hud.areasSummary", {
                    count: watchedAreas.length,
                    measurements: totalMeasurementPoints,
                  })}
                </span>
              </div>

              {showEvidence && (
                <div className="px-2.5 py-1 border border-erp-border bg-erp-surface flex items-center gap-1.5 shadow-xs">
                  <span className="font-semibold text-erp-text-main">📸</span>
                  <span
                    className={cn(
                      "font-bold",
                      watchedEvidence.length >= minimumEvidenceCount
                        ? "text-emerald-700"
                        : "text-amber-700"
                    )}
                  >
                    {minimumEvidenceCount > 0
                      ? t("hud.evidenceSummary", {
                          count: watchedEvidence.length,
                          min: minimumEvidenceCount,
                        })
                      : t("hud.evidenceSummaryNoMin", {
                          count: watchedEvidence.length,
                        })}
                  </span>
                </div>
              )}

              {showChecklist && (
                <div className="px-2.5 py-1 border border-erp-border bg-erp-surface flex items-center gap-1.5 shadow-xs">
                  <span className="font-semibold text-erp-text-main">✅</span>
                  <span
                    className={cn(
                      "font-bold",
                      checklistPassedCount === requiredChecklistItems.length
                        ? "text-emerald-700"
                        : "text-erp-navy"
                    )}
                  >
                    {t("hud.checklistSummary", {
                      passed: checklistPassedCount,
                      total: requiredChecklistItems.length,
                    })}
                  </span>
                </div>
              )}

              <div className="px-2.5 py-1 border border-erp-border bg-erp-surface flex items-center gap-1.5 shadow-xs">
                <span className="font-semibold text-erp-text-main">⚠️</span>
                <span className="font-bold text-erp-text-main">
                  {t("hud.notesSummary", { count: totalNotesCount })}
                </span>
              </div>
            </div>
          </div>

          {/* Validation Error Alert */}
          {validationError && (
            <Alert variant="danger" onClose={() => setValidationError(null)}>
              {validationError}
            </Alert>
          )}

          {/* FormTabs Navigation */}
          <FormTabs<SurveyWorkspaceTab>
            ariaLabel={t("workspaceTabs.areas")}
            activeTab={activeTab}
            onChange={(tab) => {
              setActiveTab(tab);
              setTabErrorMap((prev) => ({ ...prev, [tab]: false }));
            }}
            tabs={tabs}
          />

          {/* Tab Panels: All kept in DOM with CSS hidden to preserve input registrations */}
          <div className={cn("space-y-6", activeTab !== "areas" && "hidden")}>
            <SurveyAreasSection
              isReady={isReady}
              areaEvidenceEnabled={template?.snapshotHashVersion === "v4"}
            />
          </div>

          <div className={cn("space-y-6", activeTab !== "scope" && "hidden")}>
            <SurveyScopeSection isReady={isReady} />
          </div>

          {(showChecklist || showEvidence) && (
            <div className={cn("space-y-6", activeTab !== "checklist" && "hidden")}>
              <div
                className={cn(
                  "grid gap-6",
                  isFullScreen ? "grid-cols-1 lg:grid-cols-2" : "grid-cols-1"
                )}
              >
                {showChecklist && (
                  <div className="border border-erp-border bg-erp-surface p-4 shadow-xs">
                    <SurveyChecklistSection
                      requiredItems={requiredChecklistItems}
                      isReady={isReady}
                    />
                  </div>
                )}
                {showEvidence && (
                  <div className="border border-erp-border bg-erp-surface p-4 shadow-xs">
                    <SurveyEvidenceSection
                      opportunityId={opportunityId}
                      minimumEvidenceCount={minimumEvidenceCount}
                      isReady={isReady}
                    />
                  </div>
                )}
              </div>
            </div>
          )}

          <div className={cn("space-y-6", activeTab !== "notes" && "hidden")}>
            <SurveyNotesSection isReady={isReady} />
          </div>
        </div>
      </Drawer>

      {/* Safety Confirmation Modal for Mark Ready */}
      <ConfirmationModal
        isOpen={isConfirmMarkReadyOpen}
        onClose={() => setIsConfirmMarkReadyOpen(false)}
        onConfirm={handleConfirmMarkReady}
        title={t("markReadyConfirmTitle")}
        message={t("markReadyConfirmDesc")}
        variant="warning"
        confirmText={isSaving ? tc("states.saving") : t("markReadyAction")}
        cancelText={tc("actions.cancel")}
        isLoading={isSaving}
      />

      {/* Safety Confirmation Modal for Unsaved Changes */}
      <ConfirmationModal
        isOpen={isConfirmDiscardOpen}
        onClose={() => setIsConfirmDiscardOpen(false)}
        onConfirm={handleConfirmDiscard}
        title={t("confirmUnsavedChangesTitle")}
        message={t("confirmUnsavedChangesDesc")}
        variant="danger"
        confirmText={tc("actions.discard")}
        cancelText={tc("actions.continueEditing")}
      />
    </FormProvider>
  );
}
