"use client";

import React, { useState, useEffect } from "react";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Modal } from "@/components/ui/Modal";
import { StatusBadge } from "@/components/ui/StatusBadge";
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

interface SurveyWorkspaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  opportunityId: string;
  survey: SiteSurveyResponse;
  currentOpportunityVersion: string;
}

interface EditableMeasurement {
  id?: string | null;
  measurementType: string;
  value: number;
  unitCode: string;
  captureMethod: string;
  notes?: string | null;
  sortOrder: number;
}

interface EditableArea {
  id?: string | null;
  code: string;
  name: string;
  description?: string | null;
  sortOrder: number;
  measurements: EditableMeasurement[];
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

  // Form states
  const [visitedAt, setVisitedAt] = useState<string>("");
  const [scopeSummary, setScopeSummary] = useState<string>("");
  const [assumptions, setAssumptions] = useState<string[]>([]);
  const [newAssumption, setNewAssumption] = useState<string>("");
  const [constraints, setConstraints] = useState<string[]>([]);
  const [newConstraint, setNewConstraint] = useState<string>("");
  const [missingDetails, setMissingDetails] = useState<string[]>([]);
  const [newMissingDetail, setNewMissingDetail] = useState<string>("");
  const [areas, setAreas] = useState<EditableArea[]>([]);

  // Confirmation modal for Mark Ready
  const [isConfirmMarkReadyOpen, setIsConfirmMarkReadyOpen] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);

  const updateDraftMutation = useUpdateSurveyDraft(opportunityId, survey.id ?? "", revisionId);
  const markReadyMutation = useMarkSurveyReady(opportunityId, survey.id ?? "", revisionId);

  // Initialize draft data
  useEffect(() => {
    if (currentRevision) {
      if (currentRevision.visitedAtUtc) {
        // Format ISO date to yyyy-MM-ddTHH:mm for datetime-local
        try {
          const d = new Date(currentRevision.visitedAtUtc);
          setVisitedAt(d.toISOString().slice(0, 16));
        } catch {
          setVisitedAt("");
        }
      } else {
        setVisitedAt("");
      }

      setScopeSummary(currentRevision.scopeSummary ?? "");
      setAssumptions(currentRevision.assumptions ? [...currentRevision.assumptions] : []);
      setConstraints(currentRevision.constraints ? [...currentRevision.constraints] : []);
      setMissingDetails(currentRevision.missingDetails ? [...currentRevision.missingDetails] : []);

      if (currentRevision.areas && currentRevision.areas.length > 0) {
        setAreas(
          currentRevision.areas.map((a: SiteSurveyAreaResponse, aIdx: number) => ({
            id: a.id,
            code: a.code ?? `AREA-${String(aIdx + 1).padStart(2, "0")}`,
            name: a.name ?? "",
            description: a.description ?? "",
            sortOrder: a.sortOrder ?? aIdx + 1,
            measurements: (a.measurements ?? []).map((m: SiteSurveyMeasurementResponse, mIdx: number) => ({
              id: m.id,
              measurementType: m.measurementType ?? "width",
              value: m.value ?? 0,
              unitCode: m.unitCode ?? "m",
              captureMethod: m.captureMethod ?? "measured",
              notes: m.notes ?? "",
              sortOrder: m.sortOrder ?? mIdx + 1,
            })),
          }))
        );
      } else {
        setAreas([]);
      }
    }
  }, [currentRevision]);

  // Area handlers
  const handleAddArea = () => {
    const nextIdx = areas.length + 1;
    setAreas((prev) => [
      ...prev,
      {
        code: `AREA-${String(nextIdx).padStart(2, "0")}`,
        name: "",
        description: "",
        sortOrder: nextIdx,
        measurements: [
          {
            measurementType: "width",
            value: 1,
            unitCode: "m",
            captureMethod: "measured",
            notes: "",
            sortOrder: 1,
          },
        ],
      },
    ]);
  };

  const handleRemoveArea = (areaIdx: number) => {
    setAreas((prev) => prev.filter((_, idx) => idx !== areaIdx));
  };

  const handleAreaChange = (areaIdx: number, field: keyof EditableArea, value: unknown) => {
    setAreas((prev) =>
      prev.map((a, idx) => (idx === areaIdx ? { ...a, [field]: value } : a))
    );
  };

  // Measurement handlers
  const handleAddMeasurement = (areaIdx: number) => {
    setAreas((prev) =>
      prev.map((a, idx) => {
        if (idx !== areaIdx) return a;
        const nextMIdx = a.measurements.length + 1;
        return {
          ...a,
          measurements: [
            ...a.measurements,
            {
              measurementType: "length",
              value: 1,
              unitCode: "m",
              captureMethod: "measured",
              notes: "",
              sortOrder: nextMIdx,
            },
          ],
        };
      })
    );
  };

  const handleRemoveMeasurement = (areaIdx: number, mIdx: number) => {
    setAreas((prev) =>
      prev.map((a, idx) => {
        if (idx !== areaIdx) return a;
        return {
          ...a,
          measurements: a.measurements.filter((_, midx) => midx !== mIdx),
        };
      })
    );
  };

  const handleMeasurementChange = (
    areaIdx: number,
    mIdx: number,
    field: keyof EditableMeasurement,
    value: unknown
  ) => {
    setAreas((prev) =>
      prev.map((a, idx) => {
        if (idx !== areaIdx) return a;
        return {
          ...a,
          measurements: a.measurements.map((m, midx) =>
            midx === mIdx ? { ...m, [field]: value } : m
          ),
        };
      })
    );
  };

  // Build payload
  const buildUpdatePayload = (): UpdateSurveyDraftRequest => {
    const areaRequests: UpdateSurveyAreaRequest[] = areas.map((a, aIdx) => {
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
      visitedAtUtc: visitedAt ? new Date(visitedAt).toISOString() : null,
      scopeSummary: scopeSummary.trim() || null,
      assumptions,
      constraints,
      missingDetails,
      areas: areaRequests,
    };
  };

  const handleSaveDraft = async () => {
    if (!currentRevision?.rowVersion) return;
    setValidationError(null);

    try {
      const payload = buildUpdatePayload();
      await updateDraftMutation.mutateAsync({
        payload,
        ifMatch: `"${currentRevision.rowVersion}"`,
      });
      toast.success(t("saveDraftSuccess"));
    } catch (err: unknown) {
      const errorMsg =
        err instanceof Error ? err.message : tc("alerts.errorTitle");
      toast.error(errorMsg);
    }
  };

  const handleValidateForReady = (): boolean => {
    if (!visitedAt) {
      setValidationError(t("readinessErrors.missingVisit"));
      return false;
    }
    if (!scopeSummary.trim()) {
      setValidationError(t("readinessErrors.missingScope"));
      return false;
    }
    if (areas.length === 0) {
      setValidationError(t("readinessErrors.missingArea"));
      return false;
    }
    for (const area of areas) {
      if (!area.name.trim()) {
        setValidationError(`Area code ${area.code} missing name`);
        return false;
      }
      if (area.measurements.length === 0) {
        setValidationError(t("readinessErrors.missingMeasurement"));
        return false;
      }
      for (const m of area.measurements) {
        if (!m.value || m.value <= 0) {
          setValidationError(t("readinessErrors.missingMeasurement"));
          return false;
        }
      }
    }
    setValidationError(null);
    return true;
  };

  const handleOpenConfirmMarkReady = () => {
    if (handleValidateForReady()) {
      setIsConfirmMarkReadyOpen(true);
    }
  };

  const handleConfirmMarkReady = async () => {
    if (!currentRevision?.rowVersion) return;

    try {
      // First save draft with latest changes
      const draftPayload = buildUpdatePayload();
      const updatedRev = await updateDraftMutation.mutateAsync({
        payload: draftPayload,
        ifMatch: `"${currentRevision.rowVersion}"`,
      });

      // Now call mark ready with updated version and current opp version
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
      const errorMsg =
        err instanceof Error ? err.message : tc("alerts.errorTitle");
      toast.error(errorMsg);
    }
  };

  const isSaving = updateDraftMutation.isPending || markReadyMutation.isPending;

  return (
    <>
      <Drawer
        isOpen={isOpen}
        onClose={onClose}
        title={`${t("workspaceTitle")} — ${survey.surveyNumber}`}
        description={t("workspaceDesc")}
        size="xl"
      >
        <div className="space-y-6 pb-20">
          {/* Status Header */}
          <div className="flex flex-wrap items-center justify-between gap-4 p-4 border border-[#0B3056]/20 bg-slate-50 dark:bg-slate-900/50">
            <div>
              <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider">
                {t("surveyNumberLabel")}
              </div>
              <div className="text-lg font-mono font-bold text-[#0B3056] dark:text-sky-400">
                {survey.surveyNumber}
              </div>
            </div>
            <div className="flex items-center gap-3">
              <span className="text-sm font-medium">
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
            <div className="p-3 bg-red-50 dark:bg-red-950/40 border border-red-300 dark:border-red-800 text-red-700 dark:text-red-300 text-sm">
              {validationError}
            </div>
          )}

          {/* Visit Date and Scope Summary */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label htmlFor="visited-at-input" className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1">
                {t("visitedAtLabel")}
              </label>
              <Input
                id="visited-at-input"
                type="datetime-local"
                value={visitedAt}
                onChange={(e) => setVisitedAt(e.target.value)}
                disabled={isReady}
              />
            </div>
            <div className="md:col-span-2">
              <label htmlFor="scope-summary-input" className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1">
                {t("scopeSummaryLabel")} <span className="text-red-500">*</span>
              </label>
              <Textarea
                id="scope-summary-input"
                rows={3}
                placeholder={t("scopeSummaryPlaceholder")}
                value={scopeSummary}
                onChange={(e) => setScopeSummary(e.target.value)}
                disabled={isReady}
              />
            </div>
          </div>

          {/* Areas & Measurements Section */}
          <div className="border-t border-slate-200 dark:border-slate-800 pt-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-base font-bold text-[#0B3056] dark:text-sky-400">
                {t("areasTitle")}
              </h3>
              {!isReady && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleAddArea}
                  type="button"
                >
                  {t("addAreaAction")}
                </Button>
              )}
            </div>

            {areas.length === 0 ? (
              <div className="p-6 text-center border-2 border-dashed border-slate-200 dark:border-slate-800 text-slate-500 text-sm">
                {t("readinessErrors.missingArea")}
              </div>
            ) : (
              <div className="space-y-6">
                {areas.map((area, aIdx) => (
                  <div
                    key={area.id ?? aIdx}
                    className="p-4 border border-[#0B3056]/20 bg-white dark:bg-slate-900"
                  >
                    {/* Area Header */}
                    <div className="flex flex-wrap items-center justify-between gap-3 mb-3 pb-3 border-b border-slate-100 dark:border-slate-800">
                      <div className="flex items-center gap-3 flex-1">
                        <Input
                          className="w-28 font-mono font-bold"
                          value={area.code}
                          placeholder={t("areaCodeLabel")}
                          onChange={(e) =>
                            handleAreaChange(aIdx, "code", e.target.value)
                          }
                          disabled={isReady}
                        />
                        <Input
                          className="flex-1 font-semibold"
                          value={area.name}
                          placeholder={t("areaNameLabel")}
                          onChange={(e) =>
                            handleAreaChange(aIdx, "name", e.target.value)
                          }
                          disabled={isReady}
                        />
                      </div>
                      {!isReady && (
                        <Button
                          variant="danger"
                          size="sm"
                          onClick={() => handleRemoveArea(aIdx)}
                          type="button"
                        >
                          {t("removeAreaAction")}
                        </Button>
                      )}
                    </div>

                    {/* Measurements Table */}
                    <div className="space-y-2">
                      <div className="text-xs font-bold text-slate-500 uppercase tracking-wider">
                        {t("measurementsTitle")}
                      </div>
                      <div className="overflow-x-auto">
                        <table className="w-full text-left text-sm border-collapse">
                          <thead>
                            <tr className="bg-slate-50 dark:bg-slate-800/60 text-slate-600 dark:text-slate-300 text-xs">
                              <th className="p-2 border border-slate-200 dark:border-slate-800 w-36">
                                {t("measurementTypeLabel")}
                              </th>
                              <th className="p-2 border border-slate-200 dark:border-slate-800 w-28">
                                {t("measurementValueLabel")}
                              </th>
                              <th className="p-2 border border-slate-200 dark:border-slate-800 w-24">
                                {t("measurementUnitLabel")}
                              </th>
                              <th className="p-2 border border-slate-200 dark:border-slate-800">
                                {t("measurementNotesLabel")}
                              </th>
                              {!isReady && (
                                <th className="p-2 border border-slate-200 dark:border-slate-800 w-16 text-center">
                                  {tc("actions.delete")}
                                </th>
                              )}
                            </tr>
                          </thead>
                          <tbody>
                            {area.measurements.map((m, mIdx) => (
                              <tr key={m.id ?? mIdx}>
                                <td className="p-1 border border-slate-200 dark:border-slate-800">
                                  <select
                                    className="erp-select text-xs py-1 px-2 w-full"
                                    value={m.measurementType}
                                    onChange={(e) =>
                                      handleMeasurementChange(
                                        aIdx,
                                        mIdx,
                                        "measurementType",
                                        e.target.value
                                      )
                                    }
                                    disabled={isReady}
                                  >
                                    <option value="width">{t("types.width")}</option>
                                    <option value="height">{t("types.height")}</option>
                                    <option value="depth">{t("types.depth")}</option>
                                    <option value="length">{t("types.length")}</option>
                                    <option value="area">{t("types.area")}</option>
                                    <option value="opening">{t("types.opening")}</option>
                                    <option value="count">{t("types.count")}</option>
                                    <option value="custom">{t("types.custom")}</option>
                                  </select>
                                </td>
                                <td className="p-1 border border-slate-200 dark:border-slate-800">
                                  <Input
                                    type="number"
                                    step="0.01"
                                    min="0.0001"
                                    value={m.value}
                                    placeholder={t("measurementValueLabel")}
                                    onChange={(e) =>
                                      handleMeasurementChange(
                                        aIdx,
                                        mIdx,
                                        "value",
                                        parseFloat(e.target.value) || 0
                                      )
                                    }
                                    disabled={isReady}
                                  />
                                </td>
                                <td className="p-1 border border-slate-200 dark:border-slate-800">
                                  <select
                                    className="erp-select text-xs py-1 px-2 w-full"
                                    value={m.unitCode}
                                    onChange={(e) =>
                                      handleMeasurementChange(
                                        aIdx,
                                        mIdx,
                                        "unitCode",
                                        e.target.value
                                      )
                                    }
                                    disabled={isReady}
                                  >
                                    <option value="m">m</option>
                                    <option value="cm">cm</option>
                                    <option value="mm">mm</option>
                                    <option value="sqm">sqm</option>
                                    <option value="unit">unit</option>
                                  </select>
                                </td>
                                <td className="p-1 border border-slate-200 dark:border-slate-800">
                                  <Input
                                    value={m.notes ?? ""}
                                    placeholder={t("measurementNotesLabel")}
                                    onChange={(e) =>
                                      handleMeasurementChange(
                                        aIdx,
                                        mIdx,
                                        "notes",
                                        e.target.value
                                      )
                                    }
                                    disabled={isReady}
                                  />
                                </td>
                                {!isReady && (
                                  <td className="p-1 border border-slate-200 dark:border-slate-800 text-center">
                                    <button
                                      type="button"
                                      className="text-red-500 hover:text-red-700 font-bold px-2 py-1"
                                      onClick={() =>
                                        handleRemoveMeasurement(aIdx, mIdx)
                                      }
                                      title={t("removeMeasurementAction")}
                                    >
                                      ✕
                                    </button>
                                  </td>
                                )}
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                      {!isReady && (
                        <div className="pt-2">
                          <Button
                            variant="secondary"
                            size="sm"
                            type="button"
                            onClick={() => handleAddMeasurement(aIdx)}
                          >
                            {t("addMeasurementAction")}
                          </Button>
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Assumptions, Constraints, Missing Details */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 border-t border-slate-200 dark:border-slate-800 pt-6">
            {/* Assumptions */}
            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300">
                {t("assumptionsTitle")}
              </label>
              <div className="space-y-1">
                {assumptions.map((item, idx) => (
                  <div
                    key={idx}
                    className="flex items-center justify-between text-xs p-2 bg-slate-100 dark:bg-slate-800"
                  >
                    <span>{item}</span>
                    {!isReady && (
                      <button
                        type="button"
                        onClick={() =>
                          setAssumptions((prev) =>
                            prev.filter((_, i) => i !== idx)
                          )
                        }
                        className="text-red-500 font-bold ml-2"
                      >
                        ✕
                      </button>
                    )}
                  </div>
                ))}
                {!isReady && (
                  <div className="flex gap-2 pt-1">
                    <Input
                      placeholder={t("addNotePlaceholder")}
                      value={newAssumption}
                      onChange={(e) => setNewAssumption(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter" && newAssumption.trim()) {
                          e.preventDefault();
                          setAssumptions((prev) => [
                            ...prev,
                            newAssumption.trim(),
                          ]);
                          setNewAssumption("");
                        }
                      }}
                    />
                  </div>
                )}
              </div>
            </div>

            {/* Constraints */}
            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300">
                {t("constraintsTitle")}
              </label>
              <div className="space-y-1">
                {constraints.map((item, idx) => (
                  <div
                    key={idx}
                    className="flex items-center justify-between text-xs p-2 bg-slate-100 dark:bg-slate-800"
                  >
                    <span>{item}</span>
                    {!isReady && (
                      <button
                        type="button"
                        onClick={() =>
                          setConstraints((prev) =>
                            prev.filter((_, i) => i !== idx)
                          )
                        }
                        className="text-red-500 font-bold ml-2"
                      >
                        ✕
                      </button>
                    )}
                  </div>
                ))}
                {!isReady && (
                  <div className="flex gap-2 pt-1">
                    <Input
                      placeholder={t("addNotePlaceholder")}
                      value={newConstraint}
                      onChange={(e) => setNewConstraint(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter" && newConstraint.trim()) {
                          e.preventDefault();
                          setConstraints((prev) => [
                            ...prev,
                            newConstraint.trim(),
                          ]);
                          setNewConstraint("");
                        }
                      }}
                    />
                  </div>
                )}
              </div>
            </div>

            {/* Missing Details */}
            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300">
                {t("missingDetailsTitle")}
              </label>
              <div className="space-y-1">
                {missingDetails.map((item, idx) => (
                  <div
                    key={idx}
                    className="flex items-center justify-between text-xs p-2 bg-slate-100 dark:bg-slate-800"
                  >
                    <span>{item}</span>
                    {!isReady && (
                      <button
                        type="button"
                        onClick={() =>
                          setMissingDetails((prev) =>
                            prev.filter((_, i) => i !== idx)
                          )
                        }
                        className="text-red-500 font-bold ml-2"
                      >
                        ✕
                      </button>
                    )}
                  </div>
                ))}
                {!isReady && (
                  <div className="flex gap-2 pt-1">
                    <Input
                      placeholder={t("addNotePlaceholder")}
                      value={newMissingDetail}
                      onChange={(e) => setNewMissingDetail(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter" && newMissingDetail.trim()) {
                          e.preventDefault();
                          setMissingDetails((prev) => [
                            ...prev,
                            newMissingDetail.trim(),
                          ]);
                          setNewMissingDetail("");
                        }
                      }}
                    />
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Footer Actions */}
        <div className="fixed bottom-0 left-0 right-0 p-4 bg-white dark:bg-slate-900 border-t border-[#0B3056]/20 flex items-center justify-end gap-3 z-10">
          <Button variant="outline" onClick={onClose} disabled={isSaving}>
            {tc("actions.cancel")}
          </Button>

          {!isReady && (
            <>
              <Button
                variant="secondary"
                onClick={handleSaveDraft}
                disabled={isSaving}
              >
                {t("saveDraftAction")}
              </Button>
              <Button
                variant="primary"
                onClick={handleOpenConfirmMarkReady}
                disabled={isSaving}
              >
                {t("markReadyAction")}
              </Button>
            </>
          )}
        </div>
      </Drawer>

      {/* Safety Confirmation Modal for Mark Ready */}
      <Modal
        isOpen={isConfirmMarkReadyOpen}
        onClose={() => setIsConfirmMarkReadyOpen(false)}
        title={t("markReadyConfirmTitle")}
        description={t("markReadyConfirmDesc")}
      >
        <div className="p-4 bg-amber-50 dark:bg-amber-950/40 border border-amber-300 dark:border-amber-800 text-amber-800 dark:text-amber-200 text-sm mb-6">
          {t("markReadyConfirmDesc")}
        </div>
        <div className="flex items-center justify-end gap-3">
          <Button
            variant="outline"
            onClick={() => setIsConfirmMarkReadyOpen(false)}
            disabled={isSaving}
          >
            {tc("actions.cancel")}
          </Button>
          <Button
            variant="primary"
            onClick={handleConfirmMarkReady}
            disabled={isSaving}
          >
            {isSaving ? tc("states.saving") : t("markReadyAction")}
          </Button>
        </div>
      </Modal>
    </>
  );
}
