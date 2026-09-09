"use client";

import React, { useState, useRef, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations, useLocale } from "next-intl";
import {
  createOpportunityFormSchema,
  CANONICAL_WORK_TYPES,
  type OpportunityFormValues,
} from "../schemas/opportunity-form-schema";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useCustomerList } from "@/features/customers/api/customer-queries";
import { useCustomerSiteList } from "@/features/sites/api/site-queries";
import { useQueryClient } from "@tanstack/react-query";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { IconAlertCircle } from "@/components/common/Icons";
import { ApiError } from "@/lib/api/api-error";

export function OpportunityEditor() {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const tValidation = useTranslations("common.validation");
  const locale = useLocale();
  const router = useRouter();
  const queryClient = useQueryClient();
  const { selectedMembership } = useSelectedMembership();
  const { toast } = useToast();

  const [submitError, setSubmitError] = useState<string | null>(null);
  const [showCancelConfirm, setShowCancelConfirm] = useState(false);
  const [selectedCustomerId, setSelectedCustomerId] = useState<string>("");

  const idempotencyKeyRef = useRef<string | null>(null);
  const failedSubmissionRef = useRef(false);

  const handleFormChange = (): void => {
    if (failedSubmissionRef.current) {
      idempotencyKeyRef.current = null;
      failedSubmissionRef.current = false;
    }
  };

  const activeBranch = selectedMembership?.branch;

  // Load Active Customers list
  const { data: customerData } = useCustomerList({
    status: "active",
    limit: 100,
  });
  const customerList = customerData?.pages.flatMap((p) => p.items ?? []) ?? [];

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
    resolver: zodResolver(oppFormSchema as any),
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
    formState: { isSubmitting, isValid, isDirty },
  } = methods;

  const currentWorkTypes = watch("workTypes") ?? [];

  // If selected membership lacks active branch, block editing
  if (!activeBranch?.id) {
    return (
      <div
        role="alert"
        aria-live="polite"
        className="erp-card"
        style={{
          padding: "2rem",
          maxWidth: "520px",
          margin: "2rem auto",
          textAlign: "center",
          borderColor: "var(--erp-border-danger)",
          backgroundColor: "var(--erp-bg-danger-light)",
        }}
      >
        <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-danger)", margin: "0 0 0.5rem 0" }}>
          {t("errors.branchRequiredTitle")}
        </h2>
        <p style={{ color: "var(--erp-text-main)", margin: 0, fontSize: "0.875rem" }}>
          {t("errors.branchRequiredDetail")}
        </p>
      </div>
    );
  }

  const resolveWorkTypeLabel = (wt: string): string => {
    switch (wt) {
      case "built-in":
        return t("workTypeBuiltIn");
      case "interior":
        return t("workTypeInterior");
      case "curtain":
        return t("workTypeCurtain");
      case "wallpaper":
        return t("workTypeWallpaper");
      case "exterior":
        return t("workTypeExterior");
      case "other":
        return t("workTypeOther");
      default:
        return wt;
    }
  };

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
      <div style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
        <PageHeader
          title={t("createOpportunity")}
          subtitle={`${t("branchLabel")}: ${activeBranch.name ?? "-"}`}
        />

        {submitError && (
          <div
            role="alert"
            aria-live="polite"
            className="erp-card"
            style={{
              padding: "1rem",
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              borderColor: "var(--erp-border-danger)",
              backgroundColor: "var(--erp-bg-danger-light)",
              color: "var(--erp-danger)",
              fontSize: "0.875rem",
            }}
          >
            <IconAlertCircle size={20} />
            <span>{submitError}</span>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} onChange={handleFormChange} noValidate>
          <FormContainer>
            {/* Section 1: Customer and Primary Site */}
            <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
              <h2
                style={{
                  fontSize: "1rem",
                  fontWeight: 700,
                  color: "var(--erp-navy)",
                  margin: 0,
                  paddingBottom: "0.5rem",
                  borderBottom: "1px solid var(--erp-border)",
                }}
              >
                1. {t("customer")} &amp; {t("primarySite")}
              </h2>

              <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(280px, 1fr))", gap: "1rem" }}>
                <Controller
                  name="customerId"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <Select
                      {...field}
                      label={t("customer")}
                      required
                      error={error?.message}
                      disabled={isSubmitting}
                      options={[
                        { value: "", label: t("customerPlaceholder") },
                        ...customerList.map((c) => ({
                          value: c.id ?? "",
                          label: `${c.code ? `[${c.code}] ` : ""}${c.displayNameTh || c.displayNameEn || "-"}`,
                        })),
                      ]}
                      onChange={(e) => {
                        const newCustomerId = e.target.value;
                        field.onChange(newCustomerId);
                        setSelectedCustomerId(newCustomerId);
                        // Clear primarySiteId synchronously
                        setValue("primarySiteId", "", { shouldValidate: true, shouldDirty: true });
                      }}
                    />
                  )}
                />

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
                          label: selectedCustomerId && siteList.length === 0
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
              </div>
            </div>

            {/* Section 2: Opportunity Details */}
            <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
              <h2
                style={{
                  fontSize: "1rem",
                  fontWeight: 700,
                  color: "var(--erp-navy)",
                  margin: 0,
                  paddingBottom: "0.5rem",
                  borderBottom: "1px solid var(--erp-border)",
                }}
              >
                2. {t("opportunityDetail")}
              </h2>

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

              {/* Work Types Multi-select Checkboxes */}
              <div>
                <label
                  style={{
                    display: "block",
                    fontWeight: 600,
                    fontSize: "0.875rem",
                    color: "var(--erp-text-main)",
                    marginBottom: "0.5rem",
                  }}
                >
                  {t("workTypes")} <span style={{ color: "var(--erp-danger)" }}>*</span>
                </label>
                <div
                  style={{
                    display: "grid",
                    gridTemplateColumns: "repeat(auto-fill, minmax(180px, 1fr))",
                    gap: "0.5rem",
                    backgroundColor: "var(--erp-surface)",
                    padding: "0.75rem",
                    border: "1px solid var(--erp-border)",
                  }}
                >
                  {CANONICAL_WORK_TYPES.map((wt) => {
                    const isChecked = currentWorkTypes.includes(wt);
                    return (
                      <label
                        key={wt}
                        style={{
                          display: "inline-flex",
                          alignItems: "center",
                          gap: "0.5rem",
                          fontSize: "0.875rem",
                          cursor: isSubmitting ? "not-allowed" : "pointer",
                        }}
                      >
                        <input
                          type="checkbox"
                          value={wt}
                          checked={isChecked}
                          disabled={isSubmitting}
                          onChange={(e) => {
                            if (e.target.checked) {
                              setValue("workTypes", [...currentWorkTypes, wt], {
                                shouldValidate: true,
                                shouldDirty: true,
                              });
                            } else {
                              setValue(
                                "workTypes",
                                currentWorkTypes.filter((x) => x !== wt),
                                { shouldValidate: true, shouldDirty: true }
                              );
                            }
                          }}
                          style={{ width: "18px", height: "18px" }}
                        />
                        <span>{resolveWorkTypeLabel(wt)}</span>
                      </label>
                    );
                  })}
                </div>
              </div>

              <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(240px, 1fr))", gap: "1rem" }}>
                <Controller
                  name="sourceCode"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <Input
                      {...field}
                      value={field.value ?? ""}
                      label={t("sourceCode")}
                      error={error?.message}
                      disabled={isSubmitting}
                      placeholder={t("sourceCodePlaceholder")}
                    />
                  )}
                />

                <Controller
                  name="expectedBudget"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <Input
                      {...field}
                      type="number"
                      step="any"
                      value={field.value === undefined || field.value === null ? "" : field.value}
                      onChange={(e) => {
                        const val = e.target.value === "" ? undefined : Number(e.target.value);
                        field.onChange(val);
                      }}
                      label={t("expectedBudget")}
                      error={error?.message}
                      disabled={isSubmitting}
                      placeholder="0.00"
                    />
                  )}
                />

                <Controller
                  name="currencyCode"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <Input
                      {...field}
                      value={field.value ?? "THB"}
                      label={t("currencyCode")}
                      error={error?.message}
                      disabled={isSubmitting}
                      placeholder="THB"
                    />
                  )}
                />

                <Controller
                  name="targetDecisionDate"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <Input
                      {...field}
                      type="date"
                      value={field.value ?? ""}
                      label={t("targetDecisionDate")}
                      error={error?.message}
                      disabled={isSubmitting}
                    />
                  )}
                />
              </div>

              <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(280px, 1fr))", gap: "1rem" }}>
                <Controller
                  name="nextActionAtUtc"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <Input
                      {...field}
                      type="datetime-local"
                      value={field.value ?? ""}
                      label={t("nextActionAt")}
                      error={error?.message}
                      disabled={isSubmitting}
                    />
                  )}
                />

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
              </div>
            </div>

            <FormActionBar
              isLoading={isSubmitting}
              saveText={t("saveOpportunity")}
              onCancel={() => {
                if (isDirty) {
                  setShowCancelConfirm(true);
                } else {
                  router.push(`/${locale}/opportunities`);
                }
              }}
            />
          </FormContainer>
        </form>

        <ConfirmationModal
          isOpen={showCancelConfirm}
          title={tCommon("dialog.confirmCancelTitle")}
          message={tCommon("dialog.confirmCancelDesc")}
          confirmText={tCommon("actions.confirm")}
          cancelText={tCommon("actions.cancel")}
          variant="warning"
          onConfirm={() => {
            setShowCancelConfirm(false);
            router.push(`/${locale}/opportunities`);
          }}
          onClose={() => setShowCancelConfirm(false)}
        />
      </div>
    </FormProvider>
  );
}
