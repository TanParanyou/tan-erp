"use client";

import React, { useState, useRef, useMemo, useCallback } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations, useLocale } from "next-intl";
import {
  createOpportunityFormSchema,
  CANONICAL_WORK_TYPES,
  type OpportunityFormValues,
} from "../schemas/opportunity-form-schema";
import {
  CANONICAL_OPPORTUNITY_LEAD_SOURCES,
  getOpportunityLeadSourceOptions,
  resolveWorkTypeLabel as sharedResolveWorkTypeLabel,
} from "../opportunity-labels";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useCustomerSiteList } from "@/features/sites/api/site-queries";
import { useQueryClient } from "@tanstack/react-query";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { Checkbox } from "@/components/ui/Checkbox";
import { Alert } from "@/components/ui/Alert";
import { DatePicker } from "@/components/ui/DatePicker";
import { DateTimePicker } from "@/components/ui/DateTimePicker";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { CustomerAutocomplete } from "@/components/forms/CustomerAutocomplete";
import { CurrencyAmountInput } from "@/components/forms/CurrencyAmountInput";
import { SelectWithOther } from "@/components/forms/SelectWithOther";
import { QuickNoteChips, type QuickTemplateItem } from "@/components/forms/QuickNoteChips";
import { CustomerQuickViewDrawer } from "@/features/customers/components/customer-quick-view-drawer";
import { PageHeader } from "@/components/layout/PageHeader";
import { useDisclosure } from "@/hooks/useDisclosure";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { cn } from "@/lib/utils/cn";

export function OpportunityEditor() {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const tValidation = useTranslations("common.validation");
  const locale = useLocale();
  const router = useRouter();
  const queryClient = useQueryClient();
  const { currentUser, selectedMembership } = useSelectedMembership();
  const { toast } = useToast();

  const ownerLabel = currentUser?.user?.displayName || currentUser?.user?.email || "-";

  const [submitError, setSubmitError] = useState<string | null>(null);
  const cancelConfirm = useDisclosure();
  const [selectedCustomerId, setSelectedCustomerId] = useState<string>("");

  // Customer Quick View Drawer state
  const customerDrawer = useDisclosure<string>();

  // Lead source master data states
  const [selectedSourceOption, setSelectedSourceOption] = useState<string>("");
  const [otherSourceDetail, setOtherSourceDetail] = useState<string>("");

  const idempotencyKeyRef = useRef<string | null>(null);
  const failedSubmissionRef = useRef(false);

  const handleFormChange = (): void => {
    if (failedSubmissionRef.current) {
      idempotencyKeyRef.current = null;
      failedSubmissionRef.current = false;
    }
  };

  const activeBranch = selectedMembership?.branch;

  // Load Customer Sites for the selected customer
  const { data: siteData } = useCustomerSiteList(selectedCustomerId || undefined);
  const siteList = siteData?.items ?? [];

  const oppFormSchema = useMemo(
    () =>
      createOpportunityFormSchema((key) =>
        tValidation(
          key as "required" | "invalidFormat" | "invalidNumber" | "positiveNumber" | "actionDateNotePairRequired"
        )
      ),
    [tValidation]
  );

  const methods = useForm<OpportunityFormValues>({
    resolver: zodResolver(oppFormSchema),
    defaultValues: {
      customerId: "",
      primarySiteId: "",
      title: "",
      scopeSummary: "",
      workTypes: [],
      sourceCode: "",
      expectedBudget: undefined,
      currencyCode: "THB",
      targetDecisionDate: "",
      nextActionAtUtc: "",
      nextActionNote: "",
    },
    mode: "onBlur",
  });

  const {
    control,
    handleSubmit,
    setValue,
    watch,
    formState: { isSubmitting, isDirty },
  } = methods;

  const watchedPrimarySiteId = watch("primarySiteId");

  const selectedSite = useMemo(() => {
    if (!watchedPrimarySiteId) return null;
    return siteList.find((s) => s.id === watchedPrimarySiteId) ?? null;
  }, [watchedPrimarySiteId, siteList]);

  const formattedSiteAddress = useMemo(() => {
    if (!selectedSite) return "";
    const parts = [
      selectedSite.addressLine1,
      selectedSite.subdistrict,
      selectedSite.district,
      selectedSite.province,
      selectedSite.postalCode,
    ].filter(Boolean);
    return parts.join(" ");
  }, [selectedSite]);

  const leadSourceOptions = useMemo(
    () => getOpportunityLeadSourceOptions(t),
    [t]
  );

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

  const quickNoteTemplates: QuickTemplateItem[] = useMemo(
    () => [
      { id: "followup", label: t("quickNoteFollowUp") },
      { id: "survey", label: t("quickNoteSurvey") },
      { id: "quotation", label: t("quickNoteQuotation") },
      { id: "board", label: t("quickNoteBoardMeeting") },
      { id: "budget", label: t("quickNoteBudgetWait") },
    ],
    [t]
  );

  const resolveWorkTypeLabel = useCallback(
    (wt: string): string => sharedResolveWorkTypeLabel(wt, t),
    [t]
  );

  const handleQuickNoteSelect = useCallback(
    (noteText: string) => {
      const currentNote = watch("nextActionNote") || "";
      const updatedNote = currentNote ? `${currentNote} • ${noteText}` : noteText;
      setValue("nextActionNote", updatedNote, { shouldValidate: true, shouldDirty: true });
    },
    [setValue, watch]
  );

  // If selected membership lacks active branch, block editing
  if (!activeBranch?.id) {
    return (
      <div className="max-w-lg mx-auto my-8">
        <Alert variant="danger" title={t("errors.branchRequiredTitle")}>
          {t("errors.branchRequiredDetail")}
        </Alert>
      </div>
    );
  }

  const onSubmit = async (values: OpportunityFormValues): Promise<void> => {
    setSubmitError(null);

    const token = await getAuthToken();
    if (!token) {
      setSubmitError(tCommon("feedback.operationFailed"));
      return;
    }

    const membershipId = selectedMembership?.id;
    if (!membershipId) {
      setSubmitError(t("errors.branchRequiredDetail"));
      return;
    }

    if (!idempotencyKeyRef.current) {
      idempotencyKeyRef.current = crypto.randomUUID();
    }
    const idempotencyKey = idempotencyKeyRef.current;

    try {
      const created = await apiClient.createOpportunity(
        {
          customerId: values.customerId,
          primarySiteId: values.primarySiteId ? values.primarySiteId : null,
          title: values.title.trim(),
          scopeSummary: values.scopeSummary ? values.scopeSummary.trim() : null,
          workTypes: values.workTypes,
          sourceCode: values.sourceCode ? values.sourceCode.trim() : null,
          expectedBudget: values.expectedBudget ?? null,
          currencyCode: values.expectedBudget ? (values.currencyCode || "THB") : null,
          targetDecisionDate: values.targetDecisionDate || null,
          nextActionAtUtc: values.nextActionAtUtc ? new Date(values.nextActionAtUtc).toISOString() : null,
          nextActionNote: values.nextActionNote ? values.nextActionNote.trim() : null,
        },
        {
          token,
          membershipId,
          idempotencyKey,
          locale: locale === "en" ? "en" : "th",
        }
      );

      toast.success(t("saveSuccess"));
      await queryClient.invalidateQueries({ queryKey: ["business"] });
      router.push(`/${locale}/opportunities/${created.id}`);
    } catch (err: unknown) {
      failedSubmissionRef.current = true;
      if (err instanceof ApiError) {
        setSubmitError(err.message || t("errors.saveUnexpected"));
      } else if (err instanceof Error) {
        setSubmitError(err.message);
      } else {
        setSubmitError(t("errors.saveUnexpected"));
      }
    }
  };

  return (
    <FormProvider {...methods}>
      <div className="flex flex-col gap-6">
        <PageHeader title={t("createOpportunity")} />

        {/* Read-only Identity Context */}
        <div className="erp-card p-4 bg-erp-surface-subtle border border-erp-border">
          <dl className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm m-0">
            <div>
              <dt className="text-erp-text-muted font-medium">{t("branchLabel")}:</dt>
              <dd className="text-erp-text-main font-semibold mt-0.5 m-0">{activeBranch.name ?? "-"}</dd>
            </div>
            <div>
              <dt className="text-erp-text-muted font-medium">{t("ownerLabel")}:</dt>
              <dd className="text-erp-text-main font-semibold mt-0.5 m-0">{ownerLabel}</dd>
            </div>
          </dl>
        </div>

        {submitError && (
          <Alert variant="danger" onClose={() => setSubmitError(null)}>
            {submitError}
          </Alert>
        )}

        <form onSubmit={handleSubmit(onSubmit)} onChange={handleFormChange} noValidate>
          <FormContainer>
            {/* Section 1: Customer and Primary Site */}
            <FormSection title={t("sectionCustomerAndSite")}>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 items-start">
                <Controller
                  name="customerId"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <CustomerAutocomplete
                      value={field.value}
                      onChange={(newCustomerId) => {
                        field.onChange(newCustomerId);
                        setSelectedCustomerId(newCustomerId);
                        setValue("primarySiteId", "", { shouldValidate: true, shouldDirty: true });
                      }}
                      onViewDrawer={(targetCustomerId) => {
                        customerDrawer.open(targetCustomerId);
                      }}
                      error={error?.message}
                      required
                      disabled={isSubmitting}
                    />
                  )}
                />

                <div className="flex flex-col gap-2">
                  <Controller
                    name="primarySiteId"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <Select
                        {...field}
                        value={field.value ?? ""}
                        label={t("primarySite")}
                        error={error?.message}
                        disabled={isSubmitting || !selectedCustomerId}
                        options={[
                          {
                            value: "",
                            label:
                              selectedCustomerId && siteList.length === 0
                                ? t("noSitesForCustomer")
                                : t("primarySitePlaceholder"),
                          },
                          ...siteList.map((s) => ({
                            value: s.id ?? "",
                            label: s.label || s.addressLine1 || "-",
                          })),
                        ]}
                      />
                    )}
                  />

                  {/* Primary Site Full Detail Snapshot Card */}
                  {selectedSite && (
                    <div
                      role="region"
                      aria-label={t("primarySite")}
                      className="border border-erp-border bg-erp-surface-subtle p-3 text-xs flex flex-col gap-1.5 shadow-sm"
                    >
                      <div className="font-bold text-erp-navy flex items-center gap-1.5">
                        <span className="font-semibold text-erp-text-main">{selectedSite.label || "-"}</span>
                      </div>

                      {formattedSiteAddress && (
                        <div className="text-erp-text-muted leading-relaxed">
                          <strong className="text-erp-text-main font-medium">{t("siteAddress")}</strong>{" "}
                          <span>{formattedSiteAddress}</span>
                        </div>
                      )}

                      {selectedSite.accessNote && (
                        <div className="text-erp-text-muted mt-0.5 pt-1 border-t border-erp-border-subtle">
                          <strong className="text-erp-text-main font-medium">{t("siteAccessNote")}</strong>{" "}
                          <span>{selectedSite.accessNote}</span>
                        </div>
                      )}
                    </div>
                  )}
                </div>
              </div>
            </FormSection>

            {/* Section 2: Project & Scope Details */}
            <FormSection title={t("sectionProjectAndScope")}>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4 items-start">
                <div className="md:col-span-2">
                  <Controller
                    name="title"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <Input
                        {...field}
                        label={t("titleField")}
                        required
                        error={error?.message}
                        disabled={isSubmitting}
                        placeholder={t("titlePlaceholder")}
                      />
                    )}
                  />
                </div>

                <div className="md:col-span-1">
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
                          disabled: isSubmitting,
                          onChange: (e) => handleSourceSelect(e.target.value),
                        }}
                        otherProps={{
                          placeholder: t("sourceOtherDetailPlaceholder"),
                          value: otherSourceDetail,
                          disabled: isSubmitting,
                          onChange: (e) => handleOtherSourceChange(e.target.value),
                        }}
                        triggerValue="other"
                      />
                    )}
                  />
                </div>
              </div>

              {/* Work Types Multi-select Checkboxes with Controller & Error */}
              <Controller
                name="workTypes"
                control={control}
                render={({ field, fieldState: { error } }) => {
                  const currentValues = field.value ?? [];
                  return (
                    <div className="flex flex-col gap-2">
                      <span className="erp-label">
                        {t("workTypes")} <span className="erp-label-required">*</span>
                      </span>

                      <div
                        className={cn(
                          "grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-2 bg-erp-surface p-3 border",
                          error ? "border-red-600" : "border-erp-border"
                        )}
                      >
                        {CANONICAL_WORK_TYPES.map((wt) => {
                          const isChecked = currentValues.includes(wt);
                          return (
                            <Checkbox
                              key={wt}
                              id={`work-type-${wt}`}
                              label={resolveWorkTypeLabel(wt)}
                              checked={isChecked}
                              disabled={isSubmitting}
                              onChange={(e) => {
                                if (e.target.checked) {
                                  field.onChange([...currentValues, wt]);
                                } else {
                                  field.onChange(currentValues.filter((x) => x !== wt));
                                }
                              }}
                            />
                          );
                        })}
                      </div>

                      {error && (
                        <p className="erp-error-text" role="alert">
                          {error.message || t("workTypesRequiredError")}
                        </p>
                      )}
                    </div>
                  );
                }}
              />

              <Controller
                name="scopeSummary"
                control={control}
                render={({ field, fieldState: { error } }) => (
                  <Textarea
                    {...field}
                    value={field.value ?? ""}
                    label={t("scopeSummary")}
                    error={error?.message}
                    disabled={isSubmitting}
                    rows={3}
                    placeholder={t("scopeSummaryPlaceholder")}
                  />
                )}
              />
            </FormSection>

            {/* Section 3: Budget & Decision Timeline */}
            <FormSection title={t("sectionCommercialAndTimeline")}>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 items-start">
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
                          onAmountChange={budgetField.onChange}
                          currencyValue={currencyField.value ?? "THB"}
                          onCurrencyChange={currencyField.onChange}
                          amountError={budgetError?.message}
                          currencyError={currencyError?.message}
                          disabled={isSubmitting}
                          placeholder="0.00"
                        />
                      )}
                    />
                  )}
                />

                <div>
                  <Controller
                    name="targetDecisionDate"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <DatePicker
                        value={field.value ?? ""}
                        onChange={field.onChange}
                        label={t("targetDecisionDate")}
                        error={error?.message}
                        disabled={isSubmitting}
                      />
                    )}
                  />
                </div>
              </div>
            </FormSection>

            {/* Section 4: Follow-up & Next Action */}
            <FormSection title={t("sectionFollowUpAndNextAction")}>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 items-start">
                <Controller
                  name="nextActionAtUtc"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <DateTimePicker
                      value={field.value ?? ""}
                      onChange={field.onChange}
                      label={t("nextActionAt")}
                      error={error?.message}
                      disabled={isSubmitting}
                    />
                  )}
                />

                <div className="flex flex-col gap-2">
                  <Controller
                    name="nextActionNote"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <Input
                        {...field}
                        value={field.value ?? ""}
                        label={t("nextActionNote")}
                        error={error?.message}
                        disabled={isSubmitting}
                        placeholder={t("nextActionNotePlaceholder")}
                      />
                    )}
                  />

                  {/* Quick Note Chips */}
                  <QuickNoteChips
                    label={t("quickNotesLabel")}
                    templates={quickNoteTemplates}
                    onSelect={handleQuickNoteSelect}
                    disabled={isSubmitting}
                  />
                </div>
              </div>
            </FormSection>

            <FormActionBar
              isDirty={isDirty}
              isLoading={isSubmitting}
              saveText={t("saveOpportunity")}
              onCancel={() => {
                if (isDirty) {
                  cancelConfirm.open();
                } else {
                  router.push(`/${locale}/opportunities`);
                }
              }}
            />
          </FormContainer>
        </form>

        <ConfirmationModal
          isOpen={cancelConfirm.isOpen}
          title={tCommon("dialog.confirmCancelTitle")}
          message={tCommon("dialog.confirmCancelDesc")}
          confirmText={tCommon("actions.confirm")}
          cancelText={tCommon("actions.cancel")}
          variant="warning"
          onConfirm={() => {
            cancelConfirm.close();
            router.push(`/${locale}/opportunities`);
          }}
          onClose={cancelConfirm.close}
        />

        {/* Customer Quick View Drawer */}
        <CustomerQuickViewDrawer
          customerId={customerDrawer.data}
          isOpen={customerDrawer.isOpen}
          onClose={customerDrawer.close}
        />
      </div>
    </FormProvider>
  );
}

export default OpportunityEditor;
