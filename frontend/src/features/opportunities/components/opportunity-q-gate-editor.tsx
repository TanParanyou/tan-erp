"use client";

import React, { useState, useMemo } from "react";
import { useTranslations } from "next-intl";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "@/components/ui/Button";
import { Textarea } from "@/components/ui/Textarea";
import { Checkbox } from "@/components/ui/Checkbox";
import { DateTimePicker } from "@/components/ui/DateTimePicker";
import { Input } from "@/components/ui/Input";
import { Alert } from "@/components/ui/Alert";
import { QuickNoteChips, type QuickTemplateItem } from "@/components/forms/QuickNoteChips";
import {
  useUpdateDraftQGate,
  type UpdateDraftQGateVariables,
} from "../api/opportunity-queries";
import type { OpportunityResponse } from "@/lib/api/api-client";
import {
  CANONICAL_WORK_TYPES,
  resolveWorkTypeLabel,
} from "../opportunity-labels";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";

interface OpportunityQGateEditorProps {
  opportunity: OpportunityResponse;
  onSuccess?: () => void;
}

const createQGateSchema = (tValidation: (key: "required" | "invalidFormat" | "actionDateNotePairRequired") => string) =>
  z
    .object({
      scopeSummary: z.string().trim().max(2000, tValidation("invalidFormat")).optional().or(z.literal("")),
      workTypes: z
        .array(z.enum(CANONICAL_WORK_TYPES))
        .min(1, tValidation("required")),
      nextActionAtUtc: z.string().trim().optional().or(z.literal("")),
      nextActionNote: z.string().trim().max(500, tValidation("invalidFormat")).optional().or(z.literal("")),
    })
    .refine(
      (data) => {
        const hasDate = Boolean(data.nextActionAtUtc && data.nextActionAtUtc.trim() !== "");
        const hasNote = Boolean(data.nextActionNote && data.nextActionNote.trim() !== "");
        return (hasDate && hasNote) || (!hasDate && !hasNote);
      },
      {
        message: tValidation("actionDateNotePairRequired"),
        path: ["nextActionNote"],
      }
    );

type QGateFormValues = z.infer<ReturnType<typeof createQGateSchema>>;

export function OpportunityQGateEditor({
  opportunity,
  onSuccess,
}: OpportunityQGateEditorProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const tValidation = useTranslations("common.validation");
  const { toast } = useToast();
  const updateMutation = useUpdateDraftQGate();

  const [submitError, setSubmitError] = useState<string | null>(null);

  const schema = useMemo(
    () =>
      createQGateSchema((key) =>
        tValidation(
          key as "required" | "invalidFormat" | "actionDateNotePairRequired"
        )
      ),
    [tValidation]
  );

  const defaultWorkTypes = useMemo(() => {
    if (!opportunity.workTypes || opportunity.workTypes.length === 0) return [];
    return opportunity.workTypes.filter((wt) =>
      CANONICAL_WORK_TYPES.includes(wt as (typeof CANONICAL_WORK_TYPES)[number])
    ) as (typeof CANONICAL_WORK_TYPES)[number][];
  }, [opportunity.workTypes]);

  const {
    control,
    handleSubmit,
    setValue,
    watch,
    formState: { isSubmitting, errors },
  } = useForm<QGateFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      scopeSummary: opportunity.scopeSummary || "",
      workTypes: defaultWorkTypes,
      nextActionAtUtc: opportunity.nextActionAtUtc || "",
      nextActionNote: opportunity.nextActionNote || "",
    },
    mode: "onBlur",
  });

  const quickActionTemplates: QuickTemplateItem[] = useMemo(
    () => [
      { id: "followup", label: t("quickNoteFollowUp"), text: t("quickNoteFollowUp") },
      { id: "survey", label: t("quickNoteSurvey"), text: t("quickNoteSurvey") },
      { id: "quotation", label: t("quickNoteQuotation"), text: t("quickNoteQuotation") },
      { id: "board", label: t("quickNoteBoardMeeting"), text: t("quickNoteBoardMeeting") },
      { id: "budget", label: t("quickNoteBudgetWait"), text: t("quickNoteBudgetWait") },
    ],
    [t]
  );

  const handleApplyQuickNote = (text: string): void => {
    const currentNote = watch("nextActionNote") || "";
    if (!currentNote.trim()) {
      setValue("nextActionNote", text, { shouldValidate: true, shouldDirty: true });
    } else {
      setValue("nextActionNote", `${currentNote} - ${text}`, {
        shouldValidate: true,
        shouldDirty: true,
      });
    }
  };

  const onSubmit = async (values: QGateFormValues) => {
    if (!opportunity.id || !opportunity.rowVersion) {
      return;
    }
    setSubmitError(null);
    try {
      const vars: UpdateDraftQGateVariables = {
        opportunityId: opportunity.id,
        expectedVersion: opportunity.rowVersion,
        payload: {
          scopeSummary: values.scopeSummary ? values.scopeSummary.trim() : null,
          workTypes: values.workTypes,
          nextActionAtUtc: values.nextActionAtUtc && values.nextActionAtUtc.trim() !== ""
            ? new Date(values.nextActionAtUtc).toISOString()
            : null,
          nextActionNote: values.nextActionNote ? values.nextActionNote.trim() : null,
        },
        idempotencyKey: crypto.randomUUID(),
      };

      await updateMutation.mutateAsync(vars);
      toast.success(t("qGateSavedSuccess"));
      if (onSuccess) onSuccess();
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.status === 409 && err.code === "OPPORTUNITY_VERSION_CONFLICT") {
          const msg = t("errors.qualifyConflict");
          setSubmitError(msg);
          toast.error(msg);
          return;
        }
      }
      const msg = err instanceof Error ? err.message : t("errors.saveUnexpected");
      setSubmitError(msg);
      toast.error(msg);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="erp-card p-6 border-erp-border bg-erp-surface flex flex-col gap-6 shadow-sm"
      aria-label={t("editQGateTitle")}
    >
      <div className="border-b border-erp-border-subtle pb-3">
        <h3 className="text-base font-bold text-erp-navy">{t("editQGateTitle")}</h3>
        <p className="text-xs text-erp-text-muted mt-1">{t("editQGateSubtitle")}</p>
      </div>

      {submitError && (
        <Alert variant="danger" title={tCommon("feedback.operationFailed")}>
          {submitError}
        </Alert>
      )}

      {/* 1. Scope Summary */}
      <div>
        <label htmlFor="qgate-scope-summary" className="block text-xs font-bold text-erp-text-main mb-1">
          {t("scopeSummary")} <span className="text-erp-danger">*</span>
        </label>
        <Controller
          name="scopeSummary"
          control={control}
          render={({ field }) => (
            <Textarea
              id="qgate-scope-summary"
              rows={3}
              placeholder={t("scopeSummaryPlaceholder")}
              error={errors.scopeSummary?.message}
              value={field.value ?? ""}
              onChange={field.onChange}
              onBlur={field.onBlur}
              disabled={isSubmitting || updateMutation.isPending}
            />
          )}
        />
      </div>

      {/* 2. Work Types */}
      <div>
        <label className="block text-xs font-bold text-erp-text-main mb-1.5">
          {t("workTypes")} <span className="text-erp-danger">*</span>
        </label>
        <Controller
          name="workTypes"
          control={control}
          render={({ field }) => {
            const currentSelected = field.value || [];
            return (
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
                {CANONICAL_WORK_TYPES.map((wt) => {
                  const isChecked = currentSelected.includes(wt);
                  return (
                    <label
                      key={wt}
                      className={`flex items-center gap-2 p-2 border text-xs cursor-pointer select-none transition-colors ${
                        isChecked
                          ? "border-erp-navy bg-erp-navy/5 font-semibold text-erp-navy"
                          : "border-erp-border bg-erp-surface text-erp-text-main hover:bg-erp-bg-subtle"
                      }`}
                    >
                      <Checkbox
                        checked={isChecked}
                        disabled={isSubmitting || updateMutation.isPending}
                        onChange={(e) => {
                          if (e.target.checked) {
                            field.onChange([...currentSelected, wt]);
                          } else {
                            field.onChange(currentSelected.filter((item) => item !== wt));
                          }
                        }}
                      />
                      <span>{resolveWorkTypeLabel(wt, t)}</span>
                    </label>
                  );
                })}
              </div>
            );
          }}
        />
        {errors.workTypes && (
          <p className="text-xs text-erp-danger mt-1">{errors.workTypes.message}</p>
        )}
      </div>

      {/* 3. Next Action Follow-up */}
      <div className="border-t border-erp-border-subtle pt-4 flex flex-col gap-3">
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 items-start">
          <div>
            <label htmlFor="qgate-next-action-date" className="block text-xs font-bold text-erp-text-main mb-1">
              {t("nextActionAt")} <span className="text-erp-danger">*</span>
            </label>
            <Controller
              name="nextActionAtUtc"
              control={control}
              render={({ field }) => (
                <DateTimePicker
                  id="qgate-next-action-date"
                  value={field.value ?? ""}
                  onChange={field.onChange}
                  error={errors.nextActionAtUtc?.message}
                  disabled={isSubmitting || updateMutation.isPending}
                />
              )}
            />
          </div>

          <div>
            <label htmlFor="qgate-next-action-note" className="block text-xs font-bold text-erp-text-main mb-1">
              {t("nextActionNote")} <span className="text-erp-danger">*</span>
            </label>
            <Controller
              name="nextActionNote"
              control={control}
              render={({ field }) => (
                <Input
                  id="qgate-next-action-note"
                  placeholder={t("nextActionNotePlaceholder")}
                  error={errors.nextActionNote?.message}
                  value={field.value ?? ""}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  disabled={isSubmitting || updateMutation.isPending}
                />
              )}
            />
          </div>
        </div>

        <div>
          <span className="text-[11px] text-erp-text-muted font-semibold block mb-1">
            {t("quickNotesLabel")}
          </span>
          <QuickNoteChips
            templates={quickActionTemplates}
            onSelect={handleApplyQuickNote}
          />
        </div>
      </div>

      <div className="flex justify-end pt-2">
        <Button
          type="submit"
          variant="primary"
          size="md"
          isLoading={isSubmitting || updateMutation.isPending}
          disabled={isSubmitting || updateMutation.isPending}
        >
          {t("saveQGateAction")}
        </Button>
      </div>
    </form>
  );
}
