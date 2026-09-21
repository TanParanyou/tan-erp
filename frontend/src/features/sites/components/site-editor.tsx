"use client";

import React, { useState, useRef, useMemo } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller, FormProvider, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations, useLocale } from "next-intl";
import { createSiteFormSchema, type SiteFormValues } from "../schemas/site-form-schema";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useQueryClient } from "@tanstack/react-query";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MapPreview } from "@/components/ui/MapPreview";
import { Alert } from "@/components/ui/Alert";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { FormSection } from "@/components/forms/FormSection";
import { type SelectedAddress } from "@/components/forms/AddressAutocomplete";
import { AddressAreaField } from "@/components/forms/AddressAreaField";
import { QuickNoteChips } from "@/components/forms/QuickNoteChips";
import { PageHeader } from "@/components/layout/PageHeader";
import { MultiImagePicker, type PendingImageItem } from "@/components/forms/MultiImagePicker";
import { fileClient } from "@/lib/api/file-client";
import { useToast } from "@/hooks/useToast";
import { useCurrentLocation } from "@/hooks/useCurrentLocation";
import { IconAlertCircle, IconMapPin } from "@/components/common/Icons";
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
  const [pendingImages, setPendingImages] = useState<PendingImageItem[]>([]);

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
        tValidation(key as "required" | "invalidFormat" | "invalidNumber" | "coordinatePairRequired")
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
    setValue,
    getValues,
    trigger,
    formState: { errors, isSubmitting, isDirty },
  } = methods;

  const [subdistrict, district, province, postalCode, countryCode, latitude, longitude] = useWatch({
    control,
    name: ["subdistrict", "district", "province", "postalCode", "countryCode", "latitude", "longitude"],
  });

  const { getCurrentLocation, isLoading: isLocating } = useCurrentLocation({
    onSuccess: (coords) => {
      setValue("latitude", coords.latitude, { shouldValidate: true, shouldDirty: true });
      setValue("longitude", coords.longitude, { shouldValidate: true, shouldDirty: true });
      void trigger(["latitude", "longitude"]);
    },
  });

  const handleAppendAccessNote = React.useCallback(
    (text: string) => {
      const current = getValues("accessNote") || "";
      const separator = current.trim().length > 0 ? ", " : "";
      const updated = `${current.trim()}${separator}${text}`;
      setValue("accessNote", updated, { shouldValidate: true, shouldDirty: true });
    },
    [getValues, setValue]
  );

  const siteAccessTemplates = useMemo(
    () => [
      { id: "badge", label: tCommon("quickTemplates.siteAccess.badgeRequired") },
      { id: "gate", label: tCommon("quickTemplates.siteAccess.gateCloses1800") },
      { id: "guard", label: tCommon("quickTemplates.siteAccess.contactGuard") },
      { id: "truck", label: tCommon("quickTemplates.siteAccess.truckAccessHours") },
    ],
    [tCommon]
  );

  const handleClearAddress = React.useCallback(() => {
    setValue("subdistrict", "", { shouldValidate: true, shouldDirty: true });
    setValue("district", "", { shouldValidate: true, shouldDirty: true });
    setValue("province", "", { shouldValidate: true, shouldDirty: true });
    setValue("postalCode", "", { shouldValidate: true, shouldDirty: true });
  }, [setValue]);



  const handleAddressSelect = React.useCallback(
    (address: SelectedAddress) => {
      setValue("subdistrict", address.subdistrict, { shouldValidate: true, shouldDirty: true });
      setValue("district", address.district, { shouldValidate: true, shouldDirty: true });
      setValue("province", address.province, { shouldValidate: true, shouldDirty: true });
      setValue("postalCode", address.postalCode, { shouldValidate: true, shouldDirty: true });
      setValue("countryCode", address.countryCode, { shouldValidate: true, shouldDirty: true });

      const currentLat = getValues("latitude");
      const currentLng = getValues("longitude");
      if (
        (currentLat === null || currentLat === undefined) &&
        (currentLng === null || currentLng === undefined) &&
        address.latitude != null &&
        address.longitude != null
      ) {
        setValue("latitude", Number(address.latitude), { shouldValidate: true, shouldDirty: true });
        setValue("longitude", Number(address.longitude), { shouldValidate: true, shouldDirty: true });
        void trigger(["latitude", "longitude"]);
      }
    },
    [setValue, getValues, trigger]
  );

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
      const stillOptimizing = pendingImages.some((i) => i.isOptimizing);
      if (stillOptimizing) {
        toast.warning(tCommon("states.loading"));
        return;
      }

      let uploadedImages: { fileId: string; caption?: string }[] | undefined = undefined;

      if (pendingImages.length > 0) {
        const filesToUpload = pendingImages.map((i) => i.optimizedFile ?? i.originalFile);
        const sessionRes = await fileClient.createSession(
          {
            files: filesToUpload.map((f) => ({
              filename: f.name,
              mediaType: f.type || "image/webp",
              fileSizeBytes: f.size,
            })),
          },
          {
            token,
            membershipId,
            idempotencyKey: crypto.randomUUID(),
            locale: locale === "en" ? "en" : "th",
          }
        );

        if (!sessionRes.sessionId) {
          throw new Error("Failed to create file upload session.");
        }

        const completeRes = await fileClient.completeSession(
          sessionRes.sessionId,
          filesToUpload,
          {
            token,
            membershipId,
            locale: locale === "en" ? "en" : "th",
          }
        );

        if (completeRes.files && completeRes.files.length > 0) {
          uploadedImages = completeRes.files.map((cf, idx) => ({
            fileId: cf.fileId ?? "",
            caption: pendingImages[idx]?.caption?.trim() || undefined,
          })).filter((item) => Boolean(item.fileId));
        }
      }

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
          images: uploadedImages,
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
            onBack={handleCancel}
            backLabel={t("backToCustomer")}
            breadcrumbs={[
              { label: tShell("customers"), href: `/${locale}/customers` },
              { label: tCustomers("customerDetail"), href: `/${locale}/customers/${customerId}` },
              { label: t("createSite") },
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
            <FormSection title={t("title")}>
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

                <AddressAreaField
                  id="smart-address-search"
                  label={t("areaSelection")}
                  hint={t("smartSearchHint")}
                  required
                  disabled={isSubmitting}
                  error={
                    errors.subdistrict?.message ||
                      errors.district?.message ||
                      errors.province?.message ||
                      errors.postalCode?.message
                      ? t("areaRequired")
                      : undefined
                  }
                  value={{ subdistrict, district, province, postalCode, countryCode }}
                  onSelect={handleAddressSelect}
                  onClear={handleClearAddress}
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
              </div>
            </FormSection>

            {/* Access Note Card */}
            <FormSection title={t("accessNote")}>
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

              <QuickNoteChips
                label={tCommon("quickTemplates.label")}
                templates={siteAccessTemplates}
                onSelect={handleAppendAccessNote}
                disabled={isSubmitting}
              />
            </FormSection>

            {/* Site Photos Section */}
            <FormSection title={t("sitePhotos")}>
              <p className="text-xs text-neutral-500 -mt-2 mb-3">
                {t("sitePhotosSubtitle")}
              </p>
              <MultiImagePicker
                items={pendingImages}
                onChange={(items) => {
                  setPendingImages(items);
                  handleFormChange();
                }}
                disabled={isSubmitting}
                maxFiles={20}
                enableCamera={true}
              />
            </FormSection>
          </div>

          {/* Right Column: GPS Coordinates */}
          <div className="lg:col-span-4 flex flex-col gap-6">
            <FormSection title={t("coordinates")}>
              <div className="flex flex-col gap-4">
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  isLoading={isLocating}
                  disabled={isSubmitting || isLocating}
                  onClick={() => getCurrentLocation()}
                  icon={<IconMapPin size={16} className="text-erp-navy" />}
                  className="w-full justify-center"
                >
                  {isLocating ? tCommon("geolocation.gettingLocation") : tCommon("geolocation.getCurrentLocation")}
                </Button>

                <Controller
                  name="latitude"
                  control={control}
                  render={({ field }) => (
                    <Input
                      id="latitude"
                      label={t("latitude")}
                      type="number"
                      step="any"
                      placeholder={t("latitudePlaceholder")}
                      error={errors.latitude?.message}
                      disabled={isSubmitting}
                      value={field.value ?? ""}
                      onChange={(e) => {
                        const val = e.target.value === "" ? null : parseFloat(e.target.value);
                        field.onChange(val);
                        void trigger(["latitude", "longitude"]);
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
                      placeholder={t("longitudePlaceholder")}
                      error={errors.longitude?.message}
                      disabled={isSubmitting}
                      value={field.value ?? ""}
                      onChange={(e) => {
                        const val = e.target.value === "" ? null : parseFloat(e.target.value);
                        field.onChange(val);
                        void trigger(["latitude", "longitude"]);
                      }}
                    />
                  )}
                />

                <MapPreview
                  latitude={latitude}
                  longitude={longitude}
                  showExternalLink={true}
                  className="mt-1"
                />
              </div>
            </FormSection>
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
