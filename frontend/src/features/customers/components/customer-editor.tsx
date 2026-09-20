"use client";

import React, { useState, useRef, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller, useWatch } from "react-hook-form";
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
import { Alert } from "@/components/ui/Alert";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { FormSection } from "@/components/forms/FormSection";
import { PhoneInput } from "@/components/forms/PhoneInput";
import { SelectWithOther } from "@/components/forms/SelectWithOther";
import { ImageUpload } from "@/components/forms/ImageUpload";
import { fileClient } from "@/lib/api/file-client";
import { PageHeader } from "@/components/layout/PageHeader";

import { useToast } from "@/hooks/useToast";
import { useDebounce } from "@/hooks/useDebounce";
import { IconAlertCircle } from "@/components/common/Icons";
import { ApiError } from "@/lib/api/api-error";
import { DuplicateCandidateCard } from "./duplicate-candidate-card";
import { DuplicateConfirmationModal } from "./duplicate-confirmation-modal";
import { CustomerQuickViewDrawer } from "./customer-quick-view-drawer";
import { useCustomerDuplicateCheck } from "../api/customer-queries";
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
  const [showDuplicateConfirmModal, setShowDuplicateConfirmModal] = useState(false);
  const [pendingValues, setPendingValues] = useState<CustomerFormValues | null>(null);
  const [drawerCustomerId, setDrawerCustomerId] = useState<string | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  // One idempotency key per create intent: reuse for retries of the same payload,
  // rotate only after a failed submission followed by a field change.
  const idempotencyKeyRef = useRef<string | null>(null);
  const failedSubmissionRef = useRef(false);
  const hasConfirmedDuplicatesRef = useRef(false);

  const handleFormChange = (): void => {
    hasConfirmedDuplicatesRef.current = false;
    if (failedSubmissionRef.current) {
      idempotencyKeyRef.current = null;
      failedSubmissionRef.current = false;
    }
  };

  const customerFormSchema = useMemo(
    () =>
      createCustomerFormSchema((key) =>
        tValidation(
          key as
            | "required"
            | "invalidEmail"
            | "phoneOrEmailRequired"
            | "invalidFormat"
            | "invalidPhone"
            | "leadSourceNoteRequired"
        ),
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
      leadSourceNote: "",
      imageFile: null,
      imageFileId: "",
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

  // Watched fields for live duplicate detection
  const watchedDisplayNameTh = useWatch({ control, name: "displayNameTh" });
  const watchedPhone = useWatch({ control, name: "primaryContact.phone" });
  const watchedEmail = useWatch({ control, name: "primaryContact.email" });

  const debouncedName = useDebounce(watchedDisplayNameTh || "", 400);
  const debouncedPhone = useDebounce(watchedPhone || "", 400);
  const debouncedEmail = useDebounce(watchedEmail || "", 400);

  const duplicateCheckParams = useMemo(
    () => ({
      name: debouncedName,
      phone: debouncedPhone,
      email: debouncedEmail,
    }),
    [debouncedName, debouncedPhone, debouncedEmail],
  );

  const { data: liveDuplicates = [] } = useCustomerDuplicateCheck(
    duplicateCheckParams,
    !isCreateComplete,
  );

  const handleViewCandidate = (candidateId: string) => {
    setDrawerCustomerId(candidateId);
    setIsDrawerOpen(true);
  };

  const handleSelectExisting = (candidateId: string) => {
    router.push(`/${locale}/customers/${candidateId}`);
  };

  const executeCreate = async (values: CustomerFormValues) => {
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

      let uploadedImageFileId: string | undefined = values.imageFileId || undefined;
      if (values.imageFile && values.imageFile instanceof File) {
        const sessionRes = await fileClient.createSession(
          {
            files: [
              {
                filename: values.imageFile.name,
                mediaType: values.imageFile.type || "image/webp",
                fileSizeBytes: values.imageFile.size,
              },
            ],
          },
          {
            token,
            membershipId,
            idempotencyKey: `file-sess-${Date.now()}-${Math.random().toString(36).substring(2, 10)}`,
            locale: locale === "en" ? "en" : "th",
          }
        );

        if (!sessionRes.sessionId) {
          throw new Error("Failed to create file upload session.");
        }

        const completeRes = await fileClient.completeSession(
          sessionRes.sessionId,
          [values.imageFile],
          {
            token,
            membershipId,
            locale: locale === "en" ? "en" : "th",
          }
        );

        if (completeRes.files && completeRes.files.length > 0 && completeRes.files[0].fileId) {
          uploadedImageFileId = completeRes.files[0].fileId;
        }
      }

      const created = await apiClient.createCustomer(
        {
          customerType: values.customerType,
          displayNameTh: values.displayNameTh,
          displayNameEn: values.displayNameEn || undefined,
          preferredLocale: values.preferredLocale,
          leadSource: values.leadSource || undefined,
          leadSourceNote: values.leadSource === "other" ? values.leadSourceNote || undefined : undefined,
          imageFileId: uploadedImageFileId,
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

      // If backend returned duplicate candidates (and user hadn't confirmed yet or fallback)
      if (created.duplicateCandidates?.length && !hasConfirmedDuplicatesRef.current) {
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

  const onSubmit = async (values: CustomerFormValues) => {
    if (isCreateComplete) {
      return;
    }

    // Approach B: If there are live detected duplicates and user hasn't explicitly confirmed yet
    if (liveDuplicates.length > 0 && !hasConfirmedDuplicatesRef.current) {
      setPendingValues(values);
      setShowDuplicateConfirmModal(true);
      return;
    }

    await executeCreate(values);
  };

  const handleConfirmDuplicateCreate = async () => {
    hasConfirmedDuplicatesRef.current = true;
    setShowDuplicateConfirmModal(false);
    if (pendingValues) {
      await executeCreate(pendingValues);
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
          onBack={handleCancel}
          backLabel={t("backToList")}
          breadcrumbs={[
            { label: tShell("customers"), href: `/${locale}/customers` },
            { label: t("createCustomer") },
          ]}
        />
      }
      errorBanner={
        submitError ? (
          <Alert variant="danger">
            {submitError}
          </Alert>
        ) : null
      }
      topAlert={
        duplicateCandidates && duplicateCandidates.length > 0 ? (
          <DuplicateCandidateCard
            candidates={duplicateCandidates}
            createdCustomerHref={createdCustomerId ? `/${locale}/customers/${createdCustomerId}` : undefined}
            onViewCandidate={handleViewCandidate}
            onSelectExisting={handleSelectExisting}
          />
        ) : liveDuplicates.length > 0 ? (
          <DuplicateCandidateCard
            candidates={liveDuplicates}
            isLiveAlert
            onViewCandidate={handleViewCandidate}
            onSelectExisting={handleSelectExisting}
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
      <div className="flex flex-col gap-6 w-full">
        {/* Card 1: Customer Information */}
        <FormSection title={t("title")}>
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

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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
          </div>

          {/* Lead Source */}
          <Controller
            name="leadSource"
            control={control}
            render={({ field: leadSourceField }) => (
              <Controller
                name="leadSourceNote"
                control={control}
                render={({ field: leadSourceNoteField }) => (
                  <SelectWithOther
                    triggerValue="other"
                    selectProps={{
                      id: "leadSource",
                      label: t("leadSource"),
                      placeholder: t("leadSourceSelect"),
                      error: errors.leadSource?.message,
                      disabled: isSubmitting || isCreateComplete,
                      options: [
                        { value: "walk_in", label: t("leadSourceWalkIn") },
                        { value: "facebook_ads", label: t("leadSourceFacebookAds") },
                        { value: "referral", label: t("leadSourceReferral") },
                        { value: "project_developer", label: t("leadSourceProjectDeveloper") },
                        { value: "website", label: t("leadSourceWebsite") },
                        { value: "other", label: t("leadSourceOther") },
                      ],
                      ...leadSourceField,
                    }}
                    otherProps={{
                      id: "leadSourceNote",
                      label: t("leadSourceNote"),
                      placeholder: t("leadSourceNotePlaceholder"),
                      error: errors.leadSourceNote?.message,
                      disabled: isSubmitting || isCreateComplete,
                      maxLength: 200,
                      ...leadSourceNoteField,
                    }}
                  />
                )}
              />
            )}
          />

          {/* Customer Image / Profile Photo */}
          <div className="mt-2">
            <Controller
              name="imageFile"
              control={control}
              render={({ field }) => (
                <div>
                  <ImageUpload
                    label={t("imageUploadLabel")}
                    value={field.value ?? undefined}
                    onChange={(file) => {
                      field.onChange(file);
                      handleFormChange();
                    }}
                    error={errors.imageFile?.message}
                  />
                  <p className="text-xs text-neutral-500 mt-1">
                    {t("imageUploadHint")}
                  </p>
                </div>
              )}
            />
          </div>
        </FormSection>

        {/* Card 2: Primary Contact Section */}
        <FormSection title={t("primaryContact")}>
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
                <PhoneInput
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
        </FormSection>
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

      {/* Pre-submit Duplicate Confirmation Modal */}
      <DuplicateConfirmationModal
        isOpen={showDuplicateConfirmModal}
        onClose={() => setShowDuplicateConfirmModal(false)}
        onConfirm={handleConfirmDuplicateCreate}
        onViewCandidate={handleViewCandidate}
        onSelectExisting={handleSelectExisting}
        candidates={liveDuplicates}
        isLoading={isSubmitting}
      />

      {/* Customer Quick View Drawer */}
      <CustomerQuickViewDrawer
        customerId={drawerCustomerId}
        isOpen={isDrawerOpen}
        onClose={() => {
          setIsDrawerOpen(false);
          setDrawerCustomerId(null);
        }}
        onSelectExisting={handleSelectExisting}
      />
    </FormContainer>
  );
}
