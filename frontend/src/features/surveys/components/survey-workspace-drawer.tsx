"use client";

import React, { useState, useEffect } from "react";
import { useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { IconSave, IconCheck, IconClose } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import {
  type SiteSurveyResponse,
  type SiteSurveyAreaResponse,
  type SiteSurveyMeasurementResponse,
  type UpdateSurveyDraftRequest,
  type UpdateSurveyAreaRequest,
  type UpdateSurveyMeasurementRequest,
} from "@/lib/api/api-client";
import { useUpdateSurveyDraft, useMarkSurveyReady } from "@/features/surveys/api/survey-queries";
import {
  surveyWorkspaceSchema,
  validateSurveyReadiness,
  type SurveyWorkspaceFormData,
} from "../schemas/survey-workspace-schema";
import { getDefaultVisitDateTime } from "../hooks/use-measurement-metadata";
import { SurveyScopeSection } from "./survey-scope-section";
import { SurveyAreasSection } from "./survey-areas-section";
import { SurveyNotesSection } from "./survey-notes-section";

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

  // Confirmation modals state
  const [isConfirmMarkReadyOpen, setIsConfirmMarkReadyOpen] = useState(false);
  const [isConfirmDiscardOpen, setIsConfirmDiscardOpen] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);

  const updateDraftMutation = useUpdateSurveyDraft(opportunityId, survey.id ?? "", revisionId);
  const markReadyMutation = useMarkSurveyReady(opportunityId, survey.id ?? "", revisionId);

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
    },
  });

  const { reset, getValues, formState: { isDirty } } = methods;

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
    const result = validateSurveyReadiness(formData);

    if (!result.isValid && result.errorKey) {
      const translatedMsg = result.errorParams
        ? t(result.errorKey as Parameters<typeof t>[0], result.errorParams)
        : t(result.errorKey as Parameters<typeof t>[0]);
      setValidationError(translatedMsg);
      return;
    }

    setValidationError(null);
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

  return (
    <FormProvider {...methods}>
      <Drawer
        isOpen={isOpen}
        onClose={handleAttemptClose}
        title={`${t("workspaceTitle")} — ${survey.surveyNumber}`}
        description={t("workspaceDesc")}
        size="xl"
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
        <div className="space-y-6 pb-6">
          {/* Status Header Block */}
          <div className="flex flex-wrap items-center justify-between gap-3 p-3 sm:p-4 border border-erp-border bg-erp-surface-subtle">
            <div>
              <div className="text-xs font-semibold text-erp-text-muted uppercase tracking-wider">
                {t("surveyNumberLabel")}
              </div>
              <div className="text-base sm:text-lg font-mono font-bold text-erp-navy">
                {survey.surveyNumber}
              </div>
            </div>
            <div className="flex items-center gap-2 sm:gap-3">
              <span className="text-xs sm:text-sm font-medium text-erp-text-body">
                {t("revisionBadge", { number: currentRevision?.revisionNumber ?? 1 })}
              </span>
              <StatusBadge
                variant={isReady ? "success" : "neutral"}
                label={isReady ? t("revisionStatuses.ready") : t("revisionStatuses.draft")}
              />
            </div>
          </div>

          {/* Validation Error Alert */}
          {validationError && (
            <Alert variant="danger" onClose={() => setValidationError(null)}>
              {validationError}
            </Alert>
          )}

          {/* 1. Scope and Actual Visit Section */}
          <SurveyScopeSection isReady={isReady} />

          {/* 2. Areas and Measurements Section */}
          <SurveyAreasSection isReady={isReady} />

          {/* 3. Notes, Constraints, and Missing Details Section */}
          <SurveyNotesSection isReady={isReady} />
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
