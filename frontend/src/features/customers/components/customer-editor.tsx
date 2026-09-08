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
import { Button } from "@/components/ui/Button";
import { IconSave, IconAlertCircle } from "@/components/common/Icons";
import { ApiError } from "@/lib/api/api-error";
import { DuplicateCandidateCard } from "./duplicate-candidate-card";
import { cn } from "@/lib/utils/cn";
import type { CustomerResponse } from "@/lib/api/api-client";

export function CustomerEditor() {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const tValidation = useTranslations("common.validation");
  const locale = useLocale();
  const router = useRouter();
  const queryClient = useQueryClient();
  const { selectedMembership } = useSelectedMembership();

  const [submitError, setSubmitError] = useState<string | null>(null);
  const [duplicateCandidates, setDuplicateCandidates] = useState<CustomerResponse["duplicateCandidates"]>(null);
  const [createdCustomerId, setCreatedCustomerId] = useState<string | null>(null);
  const [isCreateComplete, setIsCreateComplete] = useState(false);

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
    formState: { errors, isSubmitting },
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
      setSubmitError(t("errors.authenticationRequired"));
      return;
    }

    const membershipId = selectedMembership?.id;
    if (!membershipId) {
      setSubmitError(t("errors.membershipRequired"));
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

      // Navigate to detail view of created customer
      router.push(`/${locale}/customers/${created.id}`);
    } catch (err: unknown) {
      failedSubmissionRef.current = true;
      if (err instanceof ApiError) {
        setSubmitError(err.message);
      } else {
        setSubmitError(t("errors.saveUnexpected"));
      }
    }
  };

  return (
    <div className="flex flex-col gap-6 max-w-[800px]">
      {/* Header */}
      <div className="border-b border-erp-border pb-5">
        <h1 className="text-2xl font-bold text-erp-navy mb-1 tracking-tight">
          {t("createCustomer")}
        </h1>
        <p className="text-sm text-erp-text-muted m-0">
          {t("subtitle")}
        </p>
      </div>

      {submitError && (
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
      )}

      {/* Duplicate result preserved after create */}
      {duplicateCandidates && duplicateCandidates.length > 0 && (
        <DuplicateCandidateCard
          candidates={duplicateCandidates}
          createdCustomerHref={createdCustomerId ? `/${locale}/customers/${createdCustomerId}` : undefined}
        />
      )}

      {/* Form */}
      <form onChange={handleFormChange} onSubmit={handleSubmit(onSubmit)} noValidate className="flex flex-col gap-6">
        {/* Customer Base Info Section */}
        <div className="erp-card p-6 flex flex-col gap-5">
          <h2 className="text-lg font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3">
            {t("title")}
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Customer Type */}
            <div className="erp-form-group">
              <label htmlFor="customerType" className="erp-label">
                {t("customerType")}
                <span className="erp-label-required">*</span>
              </label>
              <Controller
                name="customerType"
                control={control}
                render={({ field }) => (
                  <select
                    id="customerType"
                    className="erp-input"
                    disabled={isSubmitting || isCreateComplete}
                    {...field}
                  >
                    <option value="organization">{t("organization")}</option>
                    <option value="person">{t("person")}</option>
                  </select>
                )}
              />
            </div>

            {/* Preferred Locale */}
            <div className="erp-form-group">
              <label htmlFor="preferredLocale" className="erp-label">
                {t("preferredLocale")}
                <span className="erp-label-required">*</span>
              </label>
              <Controller
                name="preferredLocale"
                control={control}
                render={({ field }) => (
                  <select
                    id="preferredLocale"
                    className="erp-input"
                    disabled={isSubmitting || isCreateComplete}
                    {...field}
                  >
                    <option value="th">{t("localeThai")}</option>
                    <option value="en">{t("localeEnglish")}</option>
                  </select>
                )}
              />
            </div>
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
          <div className="erp-form-group">
            <label htmlFor="leadSource" className="erp-label">
              {t("leadSource")}
            </label>
            <Controller
              name="leadSource"
              control={control}
              render={({ field }) => (
                <select
                  id="leadSource"
                  className={cn("erp-input", errors.leadSource && "erp-input-error")}
                  aria-invalid={Boolean(errors.leadSource)}
                  disabled={isSubmitting || isCreateComplete}
                  {...field}
                >
                  <option value="">{t("leadSourceSelect")}</option>
                  <option value="walk_in">{t("leadSourceWalkIn")}</option>
                  <option value="facebook_ads">{t("leadSourceFacebookAds")}</option>
                  <option value="referral">{t("leadSourceReferral")}</option>
                  <option value="project_developer">{t("leadSourceProjectDeveloper")}</option>
                  <option value="website">{t("leadSourceWebsite")}</option>
                  <option value="other">{t("leadSourceOther")}</option>
                </select>
              )}
            />
            {errors.leadSource?.message && (
              <p className="erp-error-text" role="alert">
                {errors.leadSource.message}
              </p>
            )}
          </div>
        </div>

        {/* Primary Contact Section */}
        <div className="erp-card p-6 flex flex-col gap-5">
          <h2 className="text-lg font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3">
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
                <div className="erp-form-group">
                  <label htmlFor="preferredChannel" className="erp-label">
                    {t("preferredChannel")}
                  </label>
                  <select
                    id="preferredChannel"
                    className="erp-input"
                    disabled={isSubmitting || isCreateComplete}
                    {...field}
                  >
                    <option value="phone">{t("channelPhone")}</option>
                    <option value="email">{t("channelEmail")}</option>
                    <option value="line">{t("channelLine")}</option>
                    <option value="other">{t("channelOther")}</option>
                  </select>
                </div>
              )}
            />
          </div>
        </div>

        {/* Sticky Action Buttons Bar */}
        <div className="erp-form-actions-sticky">
          <Button
            type="button"
            variant="outline"
            size="md"
            disabled={isSubmitting || isCreateComplete}
            onClick={() => router.push(`/${locale}/customers`)}
            className="min-h-[44px]"
          >
            {tCommon("actions.cancel")}
          </Button>

          <Button
            type="submit"
            variant="primary"
            size="md"
            isLoading={isSubmitting}
            disabled={isCreateComplete}
            icon={<IconSave size={16} />}
            className="min-h-[44px] min-w-[140px]"
          >
            {t("saveCustomer")}
          </Button>
        </div>
      </form>
    </div>
  );
}
