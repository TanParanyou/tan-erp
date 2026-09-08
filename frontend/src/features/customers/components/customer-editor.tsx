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

  // Maintain stable Idempotency-Key across retries for the same form creation session
  const idempotencyKeyRef = useRef<string>(crypto.randomUUID());

  const customerFormSchema = useMemo(
    () =>
      createCustomerFormSchema((key) =>
        tValidation(key as "required" | "invalidEmail" | "phoneOrEmailRequired"),
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
      primaryContact: {
        name: "",
        roleTitle: "",
        phone: "",
        email: "",
        preferredChannel: "phone",
      },
    },
  });

  const onSubmit = async (values: CustomerFormValues) => {
    setSubmitError(null);
    setDuplicateCandidates(null);

    const token = await getAuthToken();
    if (!token) {
      setSubmitError("No authentication token available");
      return;
    }

    const membershipId = selectedMembership?.id;
    if (!membershipId) {
      setSubmitError("No active membership selected");
      return;
    }

    try {
      const created = await apiClient.createCustomer(
        {
          customerType: values.customerType,
          displayNameTh: values.displayNameTh,
          displayNameEn: values.displayNameEn || undefined,
          preferredLocale: values.preferredLocale,
          primaryContact: {
            name: values.primaryContact.name,
            roleTitle: values.primaryContact.roleTitle || undefined,
            phone: values.primaryContact.phone,
            email: values.primaryContact.email || undefined,
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
      await queryClient.invalidateQueries({ queryKey: ["customers"] });

      // Navigate to detail view of created customer
      router.push(`/${locale}/customers/${created.id}`);
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        setSubmitError(err.message);
      } else if (err instanceof Error) {
        setSubmitError(err.message);
      } else {
        setSubmitError("An unexpected error occurred while saving customer");
      }
    }
  };

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "1.5rem", maxWidth: "800px" }}>
      {/* Header */}
      <div style={{ borderBottom: "1px solid var(--erp-border)", paddingBottom: "1.25rem" }}>
        <h1
          style={{
            fontSize: "1.5rem",
            fontWeight: 700,
            color: "var(--erp-navy)",
            margin: "0 0 0.25rem 0",
            letterSpacing: "-0.01em",
          }}
        >
          {t("createCustomer")}
        </h1>
        <p style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)", margin: 0 }}>
          {t("subtitle")}
        </p>
      </div>

      {submitError && (
        <div
          role="alert"
          className="erp-card"
          style={{
            padding: "1rem 1.25rem",
            borderColor: "var(--erp-danger-border)",
            backgroundColor: "var(--erp-danger-bg)",
            display: "flex",
            alignItems: "center",
            gap: "0.75rem",
          }}
        >
          <IconAlertCircle size={20} style={{ color: "var(--erp-danger)", flexShrink: 0 }} />
          <span style={{ color: "var(--erp-danger)", fontSize: "0.875rem", fontWeight: 500 }}>
            {submitError}
          </span>
        </div>
      )}

      {/* Duplicate warning card if any */}
      {duplicateCandidates && duplicateCandidates.length > 0 && (
        <div
          role="region"
          aria-label={t("duplicateCandidates")}
          className="erp-card"
          style={{
            padding: "1.25rem",
            borderColor: "var(--erp-warning-border)",
            backgroundColor: "var(--erp-warning-bg)",
          }}
        >
          <h3 style={{ fontSize: "1rem", fontWeight: 700, color: "var(--erp-warning)", margin: "0 0 0.5rem 0" }}>
            {t("duplicateCandidates")}
          </h3>
          <p style={{ fontSize: "0.875rem", color: "#78350F", margin: "0 0 0.75rem 0" }}>
            {t("duplicateNotice")}
          </p>
          <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "flex", flexDirection: "column", gap: "0.5rem" }}>
            {duplicateCandidates.map((dup) => (
              <li
                key={dup.id}
                style={{
                  padding: "0.5rem 0.75rem",
                  backgroundColor: "var(--erp-surface)",
                  border: "1px solid var(--erp-border)",
                  fontSize: "0.8125rem",
                  display: "flex",
                  justifyContent: "space-between",
                }}
              >
                <strong>{dup.code} - {dup.displayNameTh}</strong>
                <span>{dup.maskedPhone || dup.maskedEmail}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Form */}
      <form onSubmit={handleSubmit(onSubmit)} noValidate style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
        {/* Customer Base Info Section */}
        <div className="erp-card" style={{ padding: "1.5rem", display: "flex", flexDirection: "column", gap: "1.25rem" }}>
          <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-navy)", margin: 0, borderBottom: "1px solid var(--erp-border-subtle)", paddingBottom: "0.75rem" }}>
            {t("title")}
          </h2>

          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
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
                    disabled={isSubmitting}
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
                    disabled={isSubmitting}
                    {...field}
                  >
                    <option value="th">ไทย (Thai)</option>
                    <option value="en">English</option>
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
                required
                disabled={isSubmitting}
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
                disabled={isSubmitting}
                error={errors.displayNameEn?.message}
                {...field}
              />
            )}
          />
        </div>

        {/* Primary Contact Section */}
        <div className="erp-card" style={{ padding: "1.5rem", display: "flex", flexDirection: "column", gap: "1.25rem" }}>
          <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-navy)", margin: 0, borderBottom: "1px solid var(--erp-border-subtle)", paddingBottom: "0.75rem" }}>
            {t("primaryContact")}
          </h2>

          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
            {/* Contact Name */}
            <Controller
              name="primaryContact.name"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactName"
                  label={t("contactName")}
                  required
                  disabled={isSubmitting}
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
                  disabled={isSubmitting}
                  error={errors.primaryContact?.roleTitle?.message}
                  {...field}
                />
              )}
            />
          </div>

          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
            {/* Phone */}
            <Controller
              name="primaryContact.phone"
              control={control}
              render={({ field }) => (
                <Input
                  id="primaryContactPhone"
                  label={t("phone")}
                  disabled={isSubmitting}
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
                  disabled={isSubmitting}
                  error={errors.primaryContact?.email?.message}
                  {...field}
                />
              )}
            />
          </div>

          {/* Preferred Channel */}
          <div className="erp-form-group">
            <label htmlFor="preferredChannel" className="erp-label">
              {t("preferredChannel")}
            </label>
            <Controller
              name="primaryContact.preferredChannel"
              control={control}
              render={({ field }) => (
                <select
                  id="preferredChannel"
                  className="erp-input"
                  disabled={isSubmitting}
                  {...field}
                >
                  <option value="phone">{t("channelPhone")}</option>
                  <option value="email">{t("channelEmail")}</option>
                  <option value="line">{t("channelLine")}</option>
                  <option value="other">{t("channelOther")}</option>
                </select>
              )}
            />
          </div>
        </div>

        {/* Action Buttons */}
        <div style={{ display: "flex", justifyContent: "flex-end", gap: "1rem", marginTop: "0.5rem" }}>
          <Button
            type="button"
            variant="outline"
            size="md"
            disabled={isSubmitting}
            onClick={() => router.push(`/${locale}/customers`)}
            style={{ minHeight: "44px" }}
          >
            {tCommon("actions.cancel")}
          </Button>

          <Button
            type="submit"
            variant="primary"
            size="md"
            isLoading={isSubmitting}
            icon={<IconSave size={16} />}
            style={{ minHeight: "44px", minWidth: "140px" }}
          >
            {t("saveCustomer")}
          </Button>
        </div>
      </form>
    </div>
  );
}
