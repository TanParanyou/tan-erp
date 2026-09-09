"use client";

import React, { useState, useRef, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations, useLocale } from "next-intl";
import { createSiteFormSchema, type SiteFormValues } from "../schemas/site-form-schema";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useQueryClient } from "@tanstack/react-query";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { IconAlertCircle } from "@/components/common/Icons";
import { ApiError } from "@/lib/api/api-error";

interface SiteEditorProps {
  customerId: string;
}

export function SiteEditor({ customerId }: SiteEditorProps) {
  const t = useTranslations("sites");
  const tCustomers = useTranslations("customers");
  const tCommon = useTranslations("common");
  const tShell = useTranslations("shell");
  const tValidation = useTranslations("common.validation");
  const locale = useLocale();
  const router = useRouter();
  const queryClient = useQueryClient();
  const { selectedMembership } = useSelectedMembership();
  const { toast } = useToast();

  const [submitError, setSubmitError] = useState<string | null>(null);
  const [showCancelConfirm, setShowCancelConfirm] = useState(false);

  const idempotencyKeyRef = useRef<string | null>(null);
  const failedSubmissionRef = useRef(false);

  const handleFormChange = (): void => {
    if (failedSubmissionRef.current) {
      idempotencyKeyRef.current = null;
      failedSubmissionRef.current = false;
    }
  };

  const siteFormSchema = useMemo(
    () =>
      createSiteFormSchema((key) =>
        tValidation(key as "required" | "invalidFormat" | "invalidNumber")
      ),
    [tValidation]
  );

  const methods = useForm<SiteFormValues>({
    resolver: zodResolver(siteFormSchema),
    defaultValues: {
      label: "",
      addressLine1: "",
      subdistrict: "",
      district: "",
      province: "",
      postalCode: "",
      countryCode: "TH",
      latitude: null,
      longitude: null,
      accessNote: "",
    },
  });

  const {
    control,
    handleSubmit,
    formState: { errors, isSubmitting, isDirty },
  } = methods;

  const onSubmit = async (values: SiteFormValues) => {
    setSubmitError(null);

    const token = await getAuthToken();
    if (!token) {
      const msg = tCustomers("errors.authenticationRequired");
      setSubmitError(msg);
      toast.error(msg);
      return;
    }

    const membershipId = selectedMembership?.id;
    if (!membershipId) {
      const msg = tCustomers("errors.membershipRequired");
      setSubmitError(msg);
      toast.error(msg);
      return;
    }

    try {
      idempotencyKeyRef.current ??= crypto.randomUUID();
      await apiClient.createSite(
        customerId,
        {
          label: values.label,
          addressLine1: values.addressLine1,
          subdistrict: values.subdistrict,
          district: values.district,
          province: values.province,
          postalCode: values.postalCode,
          countryCode: values.countryCode || "TH",
          latitude: values.latitude !== null && values.latitude !== undefined ? Number(values.latitude) : undefined,
          longitude: values.longitude !== null && values.longitude !== undefined ? Number(values.longitude) : undefined,
          accessNote: values.accessNote || undefined,
        },
        {
          token,
          membershipId,
          idempotencyKey: idempotencyKeyRef.current,
          locale: locale === "en" ? "en" : "th",
        }
      );

      await queryClient.invalidateQueries({ queryKey: ["business"] });
      failedSubmissionRef.current = false;

      toast.success(t("saveSuccess"));
      router.push(`/${locale}/customers/${customerId}`);
    } catch (err: unknown) {
      failedSubmissionRef.current = true;
      const message = err instanceof ApiError ? err.message : t("errors.saveUnexpected");
      setSubmitError(message);
      toast.error(message);
    }
  };

  const handleCancel = () => {
    if (isDirty) {
      setShowCancelConfirm(true);
    } else {
      router.push(`/${locale}/customers/${customerId}`);
    }
  };

  return (
    <FormProvider {...methods}>
      <FormContainer
        asForm
        onSubmit={handleSubmit(onSubmit)}
        onChange={handleFormChange}
        noValidate
        header={
          <PageHeader
            title={t("createSite")}
            subtitle={t("subtitle")}
            breadcrumbs={[
              { label: tShell("customers"), href: `/${locale}/customers` },
              { label: tCustomers("customerDetail"), href: `/${locale}/customers/${customerId}` },
              { label: t("createSite") },
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
        actionBar={
          <FormActionBar
            isDirty={isDirty}
            isLoading={isSubmitting}
            saveText={t("saveSite")}
            cancelText={tCommon("actions.cancel")}
            onCancel={handleCancel}
          />
        }
      >
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 items-start">
          {/* Main Column: Site Information & Address */}
          <div className="lg:col-span-8 flex flex-col gap-6">
            <div className="erp-card p-6 flex flex-col gap-5">
              <h2 className="text-base font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3 tracking-wide uppercase">
                {t("title")}
              </h2>

              <div className="flex flex-col gap-4">
                <Controller
                  name="label"
                  control={control}
                  render={({ field }) => (
                    <Input
                      id="label"
                      label={t("label")}
                      required
                      placeholder={t("labelPlaceholder")}
                      error={errors.label?.message}
                      disabled={isSubmitting}
                      {...field}
                    />
                  )}
                />

                <Controller
                  name="addressLine1"
                  control={control}
                  render={({ field }) => (
                    <Input
                      id="addressLine1"
                      label={t("addressLine1")}
                      required
                      placeholder={t("addressLine1Placeholder")}
                      error={errors.addressLine1?.message}
                      disabled={isSubmitting}
                      {...field}
                    />
                  )}
                />

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <Controller
                    name="subdistrict"
                    control={control}
                    render={({ field }) => (
                      <Input
                        id="subdistrict"
                        label={t("subdistrict")}
                        required
                        error={errors.subdistrict?.message}
                        disabled={isSubmitting}
                        {...field}
                      />
                    )}
                  />

                  <Controller
                    name="district"
                    control={control}
                    render={({ field }) => (
                      <Input
                        id="district"
                        label={t("district")}
                        required
                        error={errors.district?.message}
                        disabled={isSubmitting}
                        {...field}
                      />
                    )}
                  />
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                  <Controller
                    name="province"
                    control={control}
                    render={({ field }) => (
                      <Input
                        id="province"
                        label={t("province")}
                        required
                        error={errors.province?.message}
                        disabled={isSubmitting}
                        {...field}
                      />
                    )}
                  />

                  <Controller
                    name="postalCode"
                    control={control}
                    render={({ field }) => (
                      <Input
                        id="postalCode"
                        label={t("postalCode")}
                        required
                        error={errors.postalCode?.message}
                        disabled={isSubmitting}
                        {...field}
                      />
                    )}
                  />

                  <Controller
                    name="countryCode"
                    control={control}
                    render={({ field }) => (
                      <Input
                        id="countryCode"
                        label={t("countryCode")}
                        required
                        error={errors.countryCode?.message}
                        disabled={isSubmitting}
                        {...field}
                      />
                    )}
                  />
                </div>
              </div>
            </div>

            {/* Access Note Card */}
            <div className="erp-card p-6 flex flex-col gap-5">
              <h2 className="text-base font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3 tracking-wide uppercase">
                {t("accessNote")}
              </h2>

              <Controller
                name="accessNote"
                control={control}
                render={({ field }) => (
                  <Textarea
                    id="accessNote"
                    label={t("accessNote")}
                    placeholder={t("accessNotePlaceholder")}
                    rows={3}
                    error={errors.accessNote?.message}
                    disabled={isSubmitting}
                    {...field}
                  />
                )}
              />
            </div>
          </div>

          {/* Right Column: GPS Coordinates */}
          <div className="lg:col-span-4 flex flex-col gap-6">
            <div className="erp-card p-6 flex flex-col gap-5">
              <h2 className="text-base font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3 tracking-wide uppercase">
                {t("coordinates")}
              </h2>

              <div className="flex flex-col gap-4">
                <Controller
                  name="latitude"
                  control={control}
                  render={({ field }) => (
                    <Input
                      id="latitude"
                      label={t("latitude")}
                      type="number"
                      step="any"
                      placeholder="เช่น 13.7563"
                      error={errors.latitude?.message}
                      disabled={isSubmitting}
                      value={field.value ?? ""}
                      onChange={(e) => {
                        const val = e.target.value === "" ? null : parseFloat(e.target.value);
                        field.onChange(val);
                      }}
                    />
                  )}
                />

                <Controller
                  name="longitude"
                  control={control}
                  render={({ field }) => (
                    <Input
                      id="longitude"
                      label={t("longitude")}
                      type="number"
                      step="any"
                      placeholder="เช่น 100.5018"
                      error={errors.longitude?.message}
                      disabled={isSubmitting}
                      value={field.value ?? ""}
                      onChange={(e) => {
                        const val = e.target.value === "" ? null : parseFloat(e.target.value);
                        field.onChange(val);
                      }}
                    />
                  )}
                />
              </div>
            </div>
          </div>
        </div>

        <ConfirmationModal
          isOpen={showCancelConfirm}
          onClose={() => setShowCancelConfirm(false)}
          onConfirm={() => router.push(`/${locale}/customers/${customerId}`)}
          title={tCommon("dialog.confirmCancelTitle")}
          message={tCommon("dialog.confirmCancelDesc")}
          confirmText={tCommon("actions.confirm")}
          cancelText={tCommon("actions.cancel")}
          variant="warning"
        />
      </FormContainer>
    </FormProvider>
  );
}
