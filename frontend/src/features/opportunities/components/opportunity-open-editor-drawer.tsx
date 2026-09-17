"use client";

import React, { useState, useMemo, useCallback } from "react";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import {
  updateOpenOpportunityFormSchema,
  CANONICAL_WORK_TYPES,
  type UpdateOpenOpportunityFormValues,
} from "../schemas/opportunity-form-schema";
import {
  getOpportunityLeadSourceOptions,
  resolveWorkTypeLabel,
} from "../opportunity-labels";
import {
  useUpdateOpenOpportunity,
} from "../api/opportunity-queries";
import { useCustomerSiteList } from "@/features/sites/api/site-queries";
import type { OpportunityResponse } from "@/lib/api/api-client";
import { Drawer } from "@/components/ui/Drawer";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { Checkbox } from "@/components/ui/Checkbox";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { DatePicker } from "@/components/ui/DatePicker";
import { DateTimePicker } from "@/components/ui/DateTimePicker";
import { CurrencyAmountInput } from "@/components/forms/CurrencyAmountInput";
import { SelectWithOther } from "@/components/forms/SelectWithOther";
import { QuickNoteChips, type QuickTemplateItem } from "@/components/forms/QuickNoteChips";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { IconAlertCircle } from "@/components/common/Icons";

interface OpportunityOpenEditorDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  opportunity: OpportunityResponse;
  onSuccess?: () => void;
}

export function OpportunityOpenEditorDrawer({
  isOpen,
  onClose,
  opportunity,
  onSuccess,
}: OpportunityOpenEditorDrawerProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const tValidation = useTranslations("common.validation");
  const { toast } = useToast();
  const updateMutation = useUpdateOpenOpportunity();

  const [submitError, setSubmitError] = useState<string | null>(null);

  // Load customer sites
  const { data: siteData } = useCustomerSiteList(opportunity.customer?.id);
  const siteList = siteData?.items ?? [];

  // Parse initial sourceCode
  const initialSource = opportunity.sourceCode || "";
  const initialIsOther = initialSource.startsWith("other:") || initialSource === "other";
  const [selectedSourceOption, setSelectedSourceOption] = useState<string>(
    initialIsOther ? "other" : initialSource
  );
  const [otherSourceDetail, setOtherSourceDetail] = useState<string>(
    initialSource.startsWith("other:") ? initialSource.replace("other:", "") : ""
  );

  const schema = useMemo(
    () =>
      updateOpenOpportunityFormSchema((key) =>
        tValidation(
          key as "required" | "invalidFormat" | "invalidNumber" | "positiveNumber" | "actionDateNotePairRequired"
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
  } = useForm<UpdateOpenOpportunityFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      primarySiteId: opportunity.primarySite?.id || "",
      title: opportunity.title || "",
      scopeSummary: opportunity.scopeSummary || "",
      workTypes: defaultWorkTypes,
      sourceCode: opportunity.sourceCode || "",
      expectedBudget: opportunity.expectedBudget ?? undefined,
      currencyCode: opportunity.currencyCode || "THB",
      targetDecisionDate: opportunity.targetDecisionDate || "",
      nextActionAtUtc: opportunity.nextActionAtUtc || "",
      nextActionNote: opportunity.nextActionNote || "",
    },
    mode: "onBlur",
  });

  const leadSourceOptions = useMemo(() => getOpportunityLeadSourceOptions(t), [t]);

  const handleSourceSelect = useCallback(
    (val: string) => {
      setSelectedSourceOption(val);
      if (val === "other") {
        setValue("sourceCode", otherSourceDetail ? `other:${otherSourceDetail}` : "other", {
          shouldValidate: true,
          shouldDirty: true,
        });
      } else {
        setValue("sourceCode", val, { shouldValidate: true, shouldDirty: true });
      }
    },
    [otherSourceDetail, setValue]
  );

  const handleOtherSourceChange = useCallback(
    (text: string) => {
      setOtherSourceDetail(text);
      setValue("sourceCode", text ? `other:${text}` : "other", {
        shouldValidate: true,
        shouldDirty: true,
      });
    },
    [setValue]
  );

  const quickActionTemplates = useMemo<QuickTemplateItem[]>(
    () => [
      { id: "followUp", label: t("quickNoteFollowUp") },
      { id: "survey", label: t("quickNoteSurvey") },
      { id: "quotation", label: t("quickNoteQuotation") },
      { id: "boardMeeting", label: t("quickNoteBoardMeeting") },
      { id: "budgetWait", label: t("quickNoteBudgetWait") },
    ],
    [t]
  );

  const handleApplyQuickNote = useCallback(
    (value: string) => {
      setValue("nextActionNote", value, {
        shouldValidate: true,
        shouldDirty: true,
      });
    },
    [setValue]
  );

  const onSubmit = async (values: UpdateOpenOpportunityFormValues) => {
    if (!opportunity.id || !opportunity.rowVersion) return;
    setSubmitError(null);

    try {
      await updateMutation.mutateAsync({
        opportunityId: opportunity.id,
        expectedVersion: opportunity.rowVersion,
        idempotencyKey: crypto.randomUUID(),
        payload: {
          title: values.title.trim(),
          primarySiteId: values.primarySiteId ? values.primarySiteId.trim() : null,
          scopeSummary: values.scopeSummary?.trim() || null,
          workTypes: values.workTypes,
          sourceCode: values.sourceCode?.trim() || null,
          expectedBudget: values.expectedBudget ?? null,
          currencyCode: values.expectedBudget ? values.currencyCode || "THB" : null,
          targetDecisionDate: values.targetDecisionDate || null,
          nextActionAtUtc:
            values.nextActionAtUtc && values.nextActionAtUtc.trim() !== ""
              ? new Date(values.nextActionAtUtc).toISOString()
              : null,
          nextActionNote: values.nextActionNote?.trim() || null,
        },
      });

      toast.success(t("editOpportunitySuccess"));
      onSuccess?.();
      onClose();
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.status === 409 && err.code === "OPPORTUNITY_VERSION_CONFLICT") {
          setSubmitError(t("errors.qualifyConflict"));
          toast.error(t("errors.qualifyConflict"));
          return;
        }
      }
      const msg = err instanceof Error ? err.message : t("errors.saveUnexpected");
      setSubmitError(msg);
      toast.error(msg);
    }
  };

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={t("editOpportunityTitle")}
      description={opportunity.code ?? undefined}
      size="xl"
      footer={
        <div className="flex justify-end gap-3 w-full">
          <Button
            type="button"
            variant="outline"
            size="md"
            onClick={onClose}
            disabled={isSubmitting || updateMutation.isPending}
          >
            {tCommon("actions.cancel")}
          </Button>
          <Button
            type="submit"
            form="opportunity-open-edit-form"
            variant="primary"
            size="md"
            isLoading={isSubmitting || updateMutation.isPending}
            disabled={isSubmitting || updateMutation.isPending}
          >
            {tCommon("actions.save")}
          </Button>
        </div>
      }
    >
      {submitError && (
        <Alert variant="danger" title={tCommon("feedback.operationFailed")} className="mb-4">
          <div className="flex items-center gap-2">
            <IconAlertCircle size={16} />
            <span>{submitError}</span>
          </div>
        </Alert>
      )}

      <form
        id="opportunity-open-edit-form"
        onSubmit={handleSubmit(onSubmit)}
        className="flex flex-col gap-6"
      >
        {/* Section 1: Project Title & Site */}
        <FormSection title={t("sectionProjectAndScope")}>
          <div className="flex flex-col gap-4">
            <Controller
              name="title"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <Input
                  label={t("titleField")}
                  {...field}
                  placeholder={t("titlePlaceholder")}
                  error={error?.message}
                  required
                />
              )}
            />

            <Controller
              name="primarySiteId"
              control={control}
              render={({ field }) => (
                <Select
                  label={t("primarySite")}
                  {...field}
                  options={[
                    { value: "", label: t("primarySitePlaceholder") },
                    ...siteList.map((site) => ({
                      value: site.id || "",
                      label: `${site.label || site.addressLine1} (${site.district || "-"}, ${site.province || "-"})`,
                    })),
                  ]}
                />
              )}
            />

            <Controller
              name="workTypes"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-semibold text-erp-text">
                    {t("workTypes")} <span className="text-erp-danger">*</span>
                  </label>
                  <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
                    {CANONICAL_WORK_TYPES.map((wt) => {
                      const isChecked = field.value?.includes(wt);
                      return (
                        <Checkbox
                          key={wt}
                          label={resolveWorkTypeLabel(wt, t)}
                          checked={isChecked}
                          onChange={(e) => {
                            const current = field.value || [];
                            const next = e.target.checked
                              ? [...current, wt]
                              : current.filter((item) => item !== wt);
                            field.onChange(next);
                          }}
                        />
                      );
                    })}
                  </div>
                  {error && <span className="text-xs text-erp-danger">{error.message}</span>}
                </div>
              )}
            />

            <Controller
              name="scopeSummary"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <Textarea
                  label={t("scopeSummary")}
                  {...field}
                  placeholder={t("scopeSummaryPlaceholder")}
                  error={error?.message}
                  rows={3}
                />
              )}
            />

            <Controller
              name="sourceCode"
              control={control}
              render={({ fieldState: { error } }) => (
                <SelectWithOther
                  selectProps={{
                    label: t("sourceCode"),
                    value: selectedSourceOption,
                    placeholder: t("sourceSelectPlaceholder"),
                    options: leadSourceOptions,
                    error: error?.message,
                    onChange: (e) => handleSourceSelect(e.target.value),
                  }}
                  otherProps={{
                    placeholder: t("sourceOtherDetailPlaceholder"),
                    value: otherSourceDetail,
                    onChange: (e) => handleOtherSourceChange(e.target.value),
                  }}
                  triggerValue="other"
                />
              )}
            />
          </div>
        </FormSection>

        {/* Section 2: Budget & Timeline */}
        <FormSection title={t("sectionCommercialAndTimeline")}>
          <div className="flex flex-col gap-4">
            <Controller
              name="expectedBudget"
              control={control}
              render={({ field: budgetField, fieldState: { error: budgetError } }) => (
                <Controller
                  name="currencyCode"
                  control={control}
                  render={({ field: currencyField, fieldState: { error: currencyError } }) => (
                    <CurrencyAmountInput
                      label={t("expectedBudget")}
                      amountValue={budgetField.value}
                      onAmountChange={(val) => budgetField.onChange(val)}
                      currencyValue={currencyField.value || "THB"}
                      onCurrencyChange={(curr) => currencyField.onChange(curr)}
                      amountError={budgetError?.message}
                      currencyError={currencyError?.message}
                    />
                  )}
                />
              )}
            />

            <Controller
              name="targetDecisionDate"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <DatePicker
                  label={t("targetDecisionDate")}
                  value={field.value || ""}
                  onChange={field.onChange}
                  error={error?.message}
                />
              )}
            />
          </div>
        </FormSection>

        {/* Section 3: Next Action */}
        <FormSection title={t("sectionFollowUpAndNextAction")}>
          <div className="flex flex-col gap-4">
            <Controller
              name="nextActionAtUtc"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <DateTimePicker
                  label={t("nextActionAt")}
                  value={field.value || ""}
                  onChange={field.onChange}
                  error={error?.message}
                />
              )}
            />

            <QuickNoteChips
              label={t("quickNotesLabel")}
              templates={quickActionTemplates}
              onSelect={handleApplyQuickNote}
            />

            <Controller
              name="nextActionNote"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <Input
                  label={t("nextActionNote")}
                  {...field}
                  placeholder={t("nextActionNotePlaceholder")}
                  error={error?.message}
                />
              )}
            />
          </div>
        </FormSection>
      </form>
    </Drawer>
  );
}
