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
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Alert } from "@/components/ui/Alert";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { FormSection } from "@/components/forms/FormSection";
import { ImageUpload } from "@/components/forms/ImageUpload";
import { PageHeader } from "@/components/layout/PageHeader";

import { useToast } from "@/hooks/useToast";
import { useDebounce } from "@/hooks/useDebounce";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { IconAlertCircle } from "@/components/common/Icons";
import { ApiError } from "@/lib/api/api-error";
import { DuplicateCandidateCard } from "./duplicate-candidate-card";
import { DuplicateConfirmationModal } from "./duplicate-confirmation-modal";
import { CustomerQuickViewDrawer } from "./customer-quick-view-drawer";
import { CustomerIdentityFields, type CustomerLeadSource, type CustomerType, type CustomerLocale } from "./customer-identity-fields";
import { CustomerContactFields, type CustomerContactChannel } from "./customer-contact-fields";
import { customerQueryRootKey, useCustomerDuplicateCheck } from "../api/customer-queries";
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

  const { uploadSingleFile, resetIntent: resetFileUploadIntent } = useDeferredFileUpload({
    parentType: "customer",
  });

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
      resetFileUploadIntent();
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
    setValue,
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
  const contactValues = {
    name: useWatch({ control, name: "primaryContact.name" }) ?? "",
    roleTitle: useWatch({ control, name: "primaryContact.roleTitle" }) ?? "",
    phone: watchedPhone ?? "",
    email: watchedEmail ?? "",
    lineId: useWatch({ control, name: "primaryContact.lineId" }) ?? "",
    preferredChannel: useWatch({ control, name: "primaryContact.preferredChannel" }) ?? "phone",
  };
  const identityValues = {
    customerType: useWatch({ control, name: "customerType" }),
    preferredLocale: useWatch({ control, name: "preferredLocale" }),
    displayNameTh: watchedDisplayNameTh,
    displayNameEn: useWatch({ control, name: "displayNameEn" }) ?? "",
    leadSource: useWatch({ control, name: "leadSource" }) ?? "",
    leadSourceNote: useWatch({ control, name: "leadSourceNote" }) ?? "",
  };

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
      let uploadIntentId: string | undefined = undefined;

      if (values.imageFile && values.imageFile instanceof File) {
        const uploadRes = await uploadSingleFile(values.imageFile, {
          token,
          membershipId,
          locale: locale === "en" ? "en" : "th",
        });
        uploadedImageFileId = uploadRes.fileId;
        uploadIntentId = uploadRes.uploadIntentId ?? undefined;
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
          fileUploadIntentId: uploadIntentId,
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
      await queryClient.invalidateQueries({ queryKey: customerQueryRootKey(membershipId, locale === "en" ? "en" : "th") });
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
          {/* Customer Image / Profile Photo */}
          <div className="">
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

          <CustomerIdentityFields
            values={identityValues}
            errors={{
              displayNameTh: errors.displayNameTh?.message,
              displayNameEn: errors.displayNameEn?.message,
              leadSource: errors.leadSource?.message,
              leadSourceNote: errors.leadSourceNote?.message,
            }}
            disabled={isSubmitting || isCreateComplete}
            showPlaceholders
            leadSourceNoteRequired
            onCustomerTypeChange={(value: CustomerType) => { setValue("customerType", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
            onPreferredLocaleChange={(value: CustomerLocale) => { setValue("preferredLocale", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
            onDisplayNameThChange={(value) => { setValue("displayNameTh", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
            onDisplayNameEnChange={(value) => { setValue("displayNameEn", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
            onLeadSourceChange={(value: CustomerLeadSource) => { setValue("leadSource", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
            onLeadSourceNoteChange={(value) => { setValue("leadSourceNote", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
          />
        </FormSection>

        {/* Card 2: Primary Contact Section */}
        <FormSection title={t("primaryContact")}>
          <CustomerContactFields
            values={contactValues}
            idPrefix="primaryContact"
            showPlaceholders
            disabled={isSubmitting || isCreateComplete}
            errors={{
              name: errors.primaryContact?.name?.message,
              roleTitle: errors.primaryContact?.roleTitle?.message,
              phone: errors.primaryContact?.phone?.message,
              email: errors.primaryContact?.email?.message,
              lineId: errors.primaryContact?.lineId?.message,
              preferredChannel: errors.primaryContact?.preferredChannel?.message,
            }}
            onNameChange={(value) => setValue("primaryContact.name", value, { shouldDirty: true, shouldValidate: true })}
            onRoleTitleChange={(value) => setValue("primaryContact.roleTitle", value, { shouldDirty: true, shouldValidate: true })}
            onPhoneChange={(value) => { setValue("primaryContact.phone", value, { shouldDirty: true, shouldValidate: true }); handleFormChange(); }}
            onEmailChange={(value) => setValue("primaryContact.email", value, { shouldDirty: true, shouldValidate: true })}
            onLineIdChange={(value) => setValue("primaryContact.lineId", value, { shouldDirty: true, shouldValidate: true })}
            onPreferredChannelChange={(value: CustomerContactChannel) => setValue("primaryContact.preferredChannel", value, { shouldDirty: true, shouldValidate: true })}
          />
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
