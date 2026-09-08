"use client";

import React, { useState, useRef, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations, useLocale } from "next-intl";
import { createCustomerFormSchema, type CustomerFormValues } from "../schemas/customer-form-schema";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useQueryClient } from "@tanstack/react-query";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { IconAlertCircle } from "@/components/common/Icons";
import { ApiError } from "@/lib/api/api-error";
import { DuplicateCandidateCard } from "./duplicate-candidate-card";
import type { CustomerResponse } from "@/lib/api/api-client";

export function CustomerEditor() {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const tShell = useTranslations("shell");
  const tValidation = useTranslations("common.validation");
  const locale = useLocale();
  const router = useRouter();
  const queryClient = useQueryClient();
  const { selectedMembership } = useSelectedMembership();
  const { toast } = useToast();

  const [submitError, setSubmitError] = useState<string | null>(null);
  const [duplicateCandidates, setDuplicateCandidates] = useState<CustomerResponse["duplicateCandidates"]>(null);
  const [createdCustomerId, setCreatedCustomerId] = useState<string | null>(null);
  const [isCreateComplete, setIsCreateComplete] = useState(false);
  const [showCancelConfirm, setShowCancelConfirm] = useState(false);

  // One idempotency key per create intent: reuse for retries of the same payload,
  // rotate only after a failed submission followed by a field change.
  const idempotencyKeyRef = useRef<string | null>(null);
  const failedSubmissionRef = useRef(false);

  const handleFormChange = (): void => {
    if (failedSubmissionRef.current) {
      idempotencyKeyRef.current = null;
      failedSubmissionRef.current = false;
    }
  };

  const customerFormSchema = useMemo(
    () =>
      createCustomerFormSchema((key) =>
        tValidation(key as "required" | "invalidEmail" | "phoneOrEmailRequired" | "invalidFormat"),
      ),
    [tValidation],
  );

  const {
    control,
    handleSubmit,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<CustomerFormValues>({
    resolver: zodResolver(customerFormSchema),
    defaultValues: {
      customerType: "organization",
      displayNameTh: "",
      displayNameEn: "",
      preferredLocale: locale === "en" ? "en" : "th",
      leadSource: "",
      primaryContact: {
        name: "",
        roleTitle: "",
        phone: "",
        email: "",
        lineId: "",
        preferredChannel: "phone",
      },
    },
  });

  const onSubmit = async (values: CustomerFormValues) => {
    if (isCreateComplete) {
      return;
    }

    setSubmitError(null);
    setDuplicateCandidates(null);
    setCreatedCustomerId(null);

    const token = await getAuthToken();
    if (!token) {
      const msg = t("errors.authenticationRequired");
      setSubmitError(msg);
      toast.error(msg);
      return;
    }

    const membershipId = selectedMembership?.id;
    if (!membershipId) {
      const msg = t("errors.membershipRequired");
      setSubmitError(msg);
      toast.error(msg);
      return;
    }

    try {
      idempotencyKeyRef.current ??= crypto.randomUUID();
      const created = await apiClient.createCustomer(
        {
          customerType: values.customerType,
          displayNameTh: values.displayNameTh,
          displayNameEn: values.displayNameEn || undefined,
          preferredLocale: values.preferredLocale,
          leadSource: values.leadSource || undefined,
          primaryContact: {
            name: values.primaryContact.name,
            roleTitle: values.primaryContact.roleTitle || undefined,
            phone: values.primaryContact.phone,
            email: values.primaryContact.email || undefined,
            lineId: values.primaryContact.lineId || undefined,
            preferredChannel: values.primaryContact.preferredChannel || undefined,
          },
        },
        {
          token,
          membershipId,
          idempotencyKey: idempotencyKeyRef.current,
          locale: locale === "en" ? "en" : "th",
        }
      );

      // Invalidate customer lists
      await queryClient.invalidateQueries({ queryKey: ["business"] });
      failedSubmissionRef.current = false;

      if (created.duplicateCandidates?.length) {
        setDuplicateCandidates(created.duplicateCandidates);
        setCreatedCustomerId(created.id ?? null);
        setIsCreateComplete(true);
        return;
      }

      toast.success(tCommon("feedback.createSuccess"));
      // Navigate to detail view of created customer
      router.push(`/${locale}/customers/${created.id}`);
    } catch (err: unknown) {
      failedSubmissionRef.current = true;
      const message = err instanceof ApiError ? err.message : t("errors.saveUnexpected");
      setSubmitError(message);
      toast.error(message);
    }
  };

  const handleCancel = () => {
    if (isDirty && !isCreateComplete) {
      setShowCancelConfirm(true);
    } else {
      router.push(`/${locale}/customers`);
    }
  };

  return (
    <FormContainer
      asForm
      onSubmit={handleSubmit(onSubmit)}
      onChange={handleFormChange}
      noValidate
      header={
        <PageHeader
          title={t("createCustomer")}
          subtitle={t("subtitle")}
          breadcrumbs={[
            { label: tShell("customers"), href: `/${locale}/customers` },
            { label: t("createCustomer") },
          ]}
        />
      }
      errorBanner={
        submitError ? (
          <div
            role="alert"
            aria-live="polite"
            className="erp-card p-4 md:px-5 border-erp-danger-border bg-erp-danger-bg flex items-center gap-3"
          >
            <IconAlertCircle size={20} className="text-erp-danger shrink-0" />
            <span className="text-erp-danger text-sm font-medium">
              {submitError}
            </span>
          </div>
        ) : null
      }
      topAlert={
        duplicateCandidates && duplicateCandidates.length > 0 ? (
          <DuplicateCandidateCard
            candidates={duplicateCandidates}
            createdCustomerHref={createdCustomerId ? `/${locale}/customers/${createdCustomerId}` : undefined}
          />
        ) : null
      }
      actionBar={
        <FormActionBar
          isDirty={isDirty && !isCreateComplete}
          isLoading={isSubmitting}
          isSaveDisabled={isCreateComplete}
          saveText={t("saveCustomer")}
          cancelText={tCommon("actions.cancel")}
          onCancel={handleCancel}
        />
      }
    >
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 items-start">
        {/* Left Column: Customer Information */}
        <div className="lg:col-span-7 xl:col-span-7 flex flex-col gap-6">
          <div className="erp-card p-6 flex flex-col gap-5">
            <h2 className="text-base font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3 tracking-wide uppercase">
              {t("title")}
            </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Customer Type */}
            <Controller
              name="customerType"
              control={control}
              render={({ field }) => (
                <Select
                  id="customerType"
                  label={t("customerType")}
                  required
                  disabled={isSubmitting || isCreateComplete}
                  options={[
                    { value: "organization", label: t("organization") },
                    { value: "person", label: t("person") },
                  ]}
                  {...field}
                />
              )}
            />

            {/* Preferred Locale */}
            <Controller
              name="preferredLocale"
              control={control}
              render={({ field }) => (
                <Select
                  id="preferredLocale"
                  label={t("preferredLocale")}
                  required
                  disabled={isSubmitting || isCreateComplete}
                  options={[
                    { value: "th", label: t("localeThai") },
                    { value: "en", label: t("localeEnglish") },
                  ]}
                  {...field}
                />
              )}
            />
          </div>

          {/* Name TH */}
          <Controller
            name="displayNameTh"
            control={control}
            render={({ field }) => (
              <Input
                id="displayNameTh"
                label={t("displayNameTh")}
                placeholder={t("displayNameThPlaceholder")}
                required
                disabled={isSubmitting || isCreateComplete}
                error={errors.displayNameTh?.message}
                {...field}
              />
            )}
          />

          {/* Name EN */}
          <Controller
            name="displayNameEn"
            control={control}
            render={({ field }) => (
              <Input
                id="displayNameEn"
                label={t("displayNameEn")}
                placeholder={t("displayNameEnPlaceholder")}
                disabled={isSubmitting || isCreateComplete}
                error={errors.displayNameEn?.message}
                {...field}
              />
            )}
          />

          {/* Lead Source */}
          <Controller
            name="leadSource"
            control={control}
            render={({ field }) => (
              <Select
                id="leadSource"
                label={t("leadSource")}
                placeholder={t("leadSourceSelect")}
                error={errors.leadSource?.message}
                disabled={isSubmitting || isCreateComplete}
                options={[
                  { value: "walk_in", label: t("leadSourceWalkIn") },
                  { value: "facebook_ads", label: t("leadSourceFacebookAds") },
                  { value: "referral", label: t("leadSourceReferral") },
                  { value: "project_developer", label: t("leadSourceProjectDeveloper") },
                  { value: "website", label: t("leadSourceWebsite") },
                  { value: "other", label: t("leadSourceOther") },
                ]}
                {...field}
              />
            )}
          />
        </div>
      </div>

      {/* Right Column: Primary Contact Section */}
      <div className="lg:col-span-5 xl:col-span-5 flex flex-col gap-6">
        <div className="erp-card p-6 flex flex-col gap-5">
          <h2 className="text-base font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3 tracking-wide uppercase">
            {t("primaryContact")}
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Contact Name */}
            <Controller
              name="primaryContact.name"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactName"
                  label={t("contactName")}
                  placeholder={t("contactNamePlaceholder")}
                  required
                  disabled={isSubmitting || isCreateComplete}
                  error={errors.primaryContact?.name?.message}
                  {...field}
                />
              )}
            />

            {/* Role Title */}
            <Controller
              name="primaryContact.roleTitle"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactRoleTitle"
                  label={t("roleTitle")}
                  placeholder={t("roleTitlePlaceholder")}
                  disabled={isSubmitting || isCreateComplete}
                  error={errors.primaryContact?.roleTitle?.message}
                  {...field}
                />
              )}
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Phone */}
            <Controller
              name="primaryContact.phone"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactPhone"
                  label={t("phone")}
                  placeholder={t("phonePlaceholder")}
                  disabled={isSubmitting || isCreateComplete}
                  error={errors.primaryContact?.phone?.message}
                  {...field}
                />
              )}
            />

            {/* Email */}
            <Controller
              name="primaryContact.email"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactEmail"
                  label={t("email")}
                  type="email"
                  placeholder={t("emailPlaceholder")}
                  disabled={isSubmitting || isCreateComplete}
                  error={errors.primaryContact?.email?.message}
                  {...field}
                />
              )}
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* LINE ID */}
            <Controller
              name="primaryContact.lineId"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactLineId"
                  label={t("lineId")}
                  placeholder={t("lineIdPlaceholder")}
                  disabled={isSubmitting || isCreateComplete}
                  error={errors.primaryContact?.lineId?.message}
                  {...field}
                />
              )}
            />

            {/* Preferred Channel */}
            <Controller
              name="primaryContact.preferredChannel"
              control={control}
              render={({ field }) => (
                <Select
                  id="preferredChannel"
                  label={t("preferredChannel")}
                  disabled={isSubmitting || isCreateComplete}
                  options={[
                    { value: "phone", label: t("channelPhone") },
                    { value: "email", label: t("channelEmail") },
                    { value: "line", label: t("channelLine") },
                    { value: "other", label: t("channelOther") },
                  ]}
                  {...field}
                />
              )}
            />
          </div>
        </div>
      </div>
    </div>

      {/* Safety Confirmation Modal for Cancel when isDirty */}
      <ConfirmationModal
        isOpen={showCancelConfirm}
        onClose={() => setShowCancelConfirm(false)}
        onConfirm={() => {
          setShowCancelConfirm(false);
          router.push(`/${locale}/customers`);
        }}
        title={tCommon("dialog.confirmCancelTitle")}
        message={tCommon("dialog.confirmCancelDesc")}
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant="warning"
      />
    </FormContainer>
  );
}
