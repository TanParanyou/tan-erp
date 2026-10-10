"use client";

import React, { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { MapPreview } from "@/components/ui/MapPreview";
import { IconMapPin, IconEye } from "@/components/common/Icons";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { GalleryLightboxModal, type GalleryItemMetadata } from "@/components/common/GalleryLightboxModal";
import type { SiteResponse } from "@/lib/api/api-client";
import { Input } from "@/components/ui/Input";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { AddressAreaField } from "@/components/forms/AddressAreaField";
import type { SelectedAddress } from "@/components/forms/AddressAutocomplete";
import { Controller, useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useQueryClient } from "@tanstack/react-query";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { customerSiteListQueryKey } from "../api/site-queries";

const siteUpdateSchema = z.object({
  label: z.string().trim().min(1), addressLine1: z.string().trim().min(1), subdistrict: z.string().trim().min(1),
  district: z.string().trim().min(1), province: z.string().trim().min(1), postalCode: z.string().regex(/^\d{5}$/),
  countryCode: z.string().regex(/^[A-Za-z]{2}$/), latitude: z.string(), longitude: z.string(), accessNote: z.string(),
});
type SiteUpdateValues = z.infer<typeof siteUpdateSchema>;

export interface SiteDetailDrawerProps {
  site: SiteResponse | null;
  isOpen: boolean;
  onClose: () => void;
  canManage?: boolean;
  onUpdated?: () => void;
}

export function SiteDetailDrawer({
  site,
  isOpen,
  onClose,
  canManage = false,
  onUpdated,
}: SiteDetailDrawerProps) {
  const t = useTranslations("sites");
  const tCommon = useTranslations("common");

  const [lightboxIndex, setLightboxIndex] = useState<number | null>(null);
  const [showEdit, setShowEdit] = useState(false);
  const [showDeactivate, setShowDeactivate] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isDeactivating, setIsDeactivating] = useState(false);
  const localeCode = useLocale();
  const normalizedLocale: "th" | "en" = localeCode === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const form = useForm<SiteUpdateValues>({ resolver: zodResolver(siteUpdateSchema) });
  const [subdistrict, district, province, postalCode, countryCode] = useWatch({
    control: form.control,
    name: ["subdistrict", "district", "province", "postalCode", "countryCode"],
  });

  React.useEffect(() => {
    if (!site) return;
    form.reset({ label: site.label ?? "", addressLine1: site.addressLine1 ?? "", subdistrict: site.subdistrict ?? "", district: site.district ?? "", province: site.province ?? "", postalCode: site.postalCode ?? "", countryCode: site.countryCode ?? "TH", latitude: site.latitude === null || site.latitude === undefined ? "" : String(site.latitude), longitude: site.longitude === null || site.longitude === undefined ? "" : String(site.longitude), accessNote: site.accessNote ?? "" });
  }, [site, form]);

  if (!site) return null;

  const refreshSites = async () => {
    await queryClient.invalidateQueries({ queryKey: customerSiteListQueryKey(selectedMembership?.id, normalizedLocale, site.customerId) });
    onUpdated?.();
  };

  const saveSite = form.handleSubmit(async (values) => {
    const token = await getAuthToken();
    const membershipId = selectedMembership?.id;
    if (!token || !membershipId || !site.id || !site.customerId || !site.rowVersion) {
      toast.error(t("errors.saveUnexpected"));
      return;
    }
    setIsSaving(true);
    try {
      await apiClient.updateSite(site.customerId, site.id, { label: values.label, addressLine1: values.addressLine1, subdistrict: values.subdistrict, district: values.district, province: values.province, postalCode: values.postalCode, countryCode: values.countryCode, latitude: values.latitude === "" ? null : Number(values.latitude), longitude: values.longitude === "" ? null : Number(values.longitude), accessNote: values.accessNote || undefined }, { token, membershipId, ifMatch: site.rowVersion, locale: normalizedLocale });
      setShowEdit(false);
      await refreshSites();
      toast.success(tCommon("feedback.saveSuccess"));
      onClose();
    } catch (error: unknown) {
      toast.error(error instanceof ApiError ? error.message : t("errors.saveUnexpected"));
    } finally {
      setIsSaving(false);
    }
  });

  const handleAddressSelect = (address: SelectedAddress) => {
    form.setValue("subdistrict", address.subdistrict, { shouldDirty: true, shouldValidate: true });
    form.setValue("district", address.district, { shouldDirty: true, shouldValidate: true });
    form.setValue("province", address.province, { shouldDirty: true, shouldValidate: true });
    form.setValue("postalCode", address.postalCode, { shouldDirty: true, shouldValidate: true });
    form.setValue("countryCode", address.countryCode, { shouldDirty: true, shouldValidate: true });
  };

  const handleClearAddress = () => {
    form.setValue("subdistrict", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("district", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("province", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("postalCode", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("countryCode", "", { shouldDirty: true, shouldValidate: true });
  };

  const deactivateSite = async () => {
    const token = await getAuthToken();
    const membershipId = selectedMembership?.id;
    if (!token || !membershipId || !site.id || !site.customerId || !site.rowVersion) {
      toast.error(t("errors.saveUnexpected"));
      return;
    }
    setIsDeactivating(true);
    try {
      await apiClient.deactivateSite(site.customerId, site.id, { token, membershipId, ifMatch: site.rowVersion, locale: normalizedLocale });
      setShowDeactivate(false);
      await refreshSites();
      toast.success(tCommon("feedback.saveSuccess"));
      onClose();
    } catch (error: unknown) {
      toast.error(error instanceof ApiError ? error.message : t("errors.saveUnexpected"));
    } finally {
      setIsDeactivating(false);
    }
  };

  const siteLabel = site.label || t("siteDetail");
  const images = site.images ?? [];
  const galleryItems: GalleryItemMetadata[] = images.map((img, idx) => ({
    id: img.id || `img-${idx}`,
    fileId: img.fileId,
    caption: img.caption,
  }));

  const handleDrawerClose = () => {
    if (showEdit) {
      setShowEdit(false);
      return;
    }
    onClose();
  };

  const footerActions = showEdit ? (
    <div className="flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3 w-full">
      <Button
        type="button"
        variant="outline"
        size="md"
        className="w-full sm:w-auto min-h-11"
        disabled={isSaving}
        onClick={() => setShowEdit(false)}
      >
        {tCommon("actions.cancel")}
      </Button>
      <Button
        type="submit"
        variant="primary"
        size="md"
        className="w-full sm:w-auto min-h-11"
        isLoading={isSaving || form.formState.isSubmitting}
        disabled={isSaving}
        onClick={() => {
          const formEl = document.getElementById("edit-site-form") as HTMLFormElement | null;
          formEl?.requestSubmit();
        }}
      >
        {tCommon("actions.saveChanges")}
      </Button>
    </div>
  ) : (
    <div className="flex flex-col-reverse sm:flex-row justify-between items-center gap-2 w-full">
      {canManage && site.status === "active" ? (
        <div className="flex gap-2 w-full sm:w-auto">
          <Button
            variant="outline"
            size="md"
            className="flex-1 sm:flex-none min-h-11"
            onClick={() => setShowEdit(true)}
          >
            {t("editSite")}
          </Button>
          <Button
            variant="danger"
            size="md"
            className="flex-1 sm:flex-none min-h-11"
            onClick={() => setShowDeactivate(true)}
          >
            {t("deactivateSite")}
          </Button>
        </div>
      ) : (
        <span />
      )}
      <Button
        variant="outline"
        size="md"
        className="w-full sm:w-auto min-h-11"
        onClick={onClose}
      >
        {tCommon("actions.close")}
      </Button>
    </div>
  );

  return (
    <>
      <Drawer
        isOpen={isOpen}
        onClose={handleDrawerClose}
        title={showEdit ? t("editSite") : siteLabel}
        description={showEdit ? undefined : t("siteDetail")}
        size="lg"
        footer={footerActions}
      >
        {showEdit ? (
          <form id="edit-site-form" onSubmit={saveSite} className="flex flex-col gap-4 py-2">
            <div className="grid gap-4 grid-cols-1 sm:grid-cols-2">
              <Input label={t("label")} placeholder={t("labelPlaceholder")} {...form.register("label")} />
              <Input label={t("addressLine1")} placeholder={t("addressLine1Placeholder")} {...form.register("addressLine1")} />
              <div className="col-span-1 sm:col-span-2">
                <Controller
                  control={form.control}
                  name="subdistrict"
                  render={() => (
                    <AddressAreaField
                      id={`site-address-area-${site.id}`}
                      label={tCommon("addressAutocomplete.areaSelection")}
                      hint={tCommon("addressAutocomplete.smartSearchHint")}
                      required
                      disabled={isSaving}
                      error={
                        form.formState.errors.subdistrict ||
                        form.formState.errors.district ||
                        form.formState.errors.province ||
                        form.formState.errors.postalCode ||
                        form.formState.errors.countryCode
                          ? tCommon("addressAutocomplete.areaRequired")
                          : undefined
                      }
                      value={{ subdistrict, district, province, postalCode, countryCode }}
                      onSelect={handleAddressSelect}
                      onClear={handleClearAddress}
                    />
                  )}
                />
              </div>
              <Input label={t("latitude")} type="number" step="any" placeholder={t("latitudePlaceholder")} {...form.register("latitude")} />
              <Input label={t("longitude")} type="number" step="any" placeholder={t("longitudePlaceholder")} {...form.register("longitude")} />
              <div className="col-span-1 sm:col-span-2">
                <Input label={t("accessNote")} placeholder={t("accessNotePlaceholder")} {...form.register("accessNote")} />
              </div>
            </div>
          </form>
        ) : (
          <div className="flex flex-col gap-6 py-2">
            {/* Header Status Bar */}
            <div className="flex items-center justify-between bg-erp-surface-muted p-3 border border-erp-border">
              <div className="flex items-center gap-2">
                <IconMapPin size={18} className="text-erp-navy" />
                <span className="font-semibold text-sm text-erp-navy">{siteLabel}</span>
              </div>
              <Badge
                variant={site.status === "active" ? "success" : "neutral"}
                size="sm"
              >
                {site.status === "active" ? t("statusActive") : t("statusInactive")}
              </Badge>
            </div>

            {/* Address Details */}
            <div className="erp-card p-4 flex flex-col gap-3">
              <h3 className="text-xs font-bold text-erp-navy uppercase tracking-wide border-b border-erp-border-subtle pb-2 m-0">
                {t("address")}
              </h3>
              <dl className="erp-dl text-xs">
                <dt>{t("addressLine1")}:</dt>
                <dd className="font-medium">{site.addressLine1 || "-"}</dd>

                <dt>{t("areaSelection")}:</dt>
                <dd>
                  <span className="font-medium text-erp-text-main">
                    {[site.subdistrict, site.district, site.province].filter(Boolean).join(" » ") || "-"}
                  </span>{" "}
                  {site.postalCode && (
                    <span className="font-mono text-erp-text-muted">
                      ({site.postalCode})
                    </span>
                  )}
                </dd>

                <dt>{t("countryCode")}:</dt>
                <dd className="font-mono">{site.countryCode || "TH"}</dd>

                {site.accessNote && (
                  <>
                    <dt>{t("accessNote")}:</dt>
                    <dd className="text-erp-navy bg-erp-surface-subtle p-2 border border-erp-border-subtle">
                      {site.accessNote}
                    </dd>
                  </>
                )}
              </dl>
            </div>

            {/* GPS Coordinates & Map */}
            <div className="erp-card p-4 flex flex-col gap-3">
              <h3 className="text-xs font-bold text-erp-navy uppercase tracking-wide border-b border-erp-border-subtle pb-2 m-0">
                {t("coordinatesTitle")}
              </h3>
              {site.latitude != null && site.longitude != null ? (
                <div className="flex flex-col gap-2">
                  <div className="text-xs font-mono text-erp-text-muted flex gap-4">
                    <span>
                      <strong>{t("latitude")}:</strong> {site.latitude}
                    </span>
                    <span>
                      <strong>{t("longitude")}:</strong> {site.longitude}
                    </span>
                  </div>
                  <MapPreview
                    latitude={site.latitude}
                    longitude={site.longitude}
                    showExternalLink={true}
                    className="mt-1"
                  />
                </div>
              ) : (
                <p className="text-xs text-erp-text-muted italic m-0">
                  {t("noCoordinates")}
                </p>
              )}
            </div>

            {/* Site Photos Gallery */}
            <div className="erp-card p-4 flex flex-col gap-3">
              <div className="flex items-center justify-between border-b border-erp-border-subtle pb-2">
                <h3 className="text-xs font-bold text-erp-navy uppercase tracking-wide m-0">
                  {t("sitePhotos")}
                </h3>
                <span className="erp-badge erp-badge-navy font-mono text-xs">
                  {t("photoCount", { count: images.length })}
                </span>
              </div>

              {images.length === 0 ? (
                <p className="text-xs text-erp-text-muted italic py-2 m-0">
                  {t("noPhotos")}
                </p>
              ) : (
                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3 pt-1">
                  {images.map((img, idx) => (
                    <div
                      key={img.id || `img-${idx}`}
                      className="border border-erp-border bg-erp-surface group relative flex flex-col overflow-hidden rounded-none hover:border-erp-navy transition-colors cursor-pointer"
                      onClick={() => setLightboxIndex(idx)}
                    >
                      <div className="relative aspect-video w-full bg-erp-surface-muted overflow-hidden">
                        <AuthenticatedFileImage
                          fileId={img.fileId ?? ""}
                          alt={img.caption || `${t("sitePhotos")} ${idx + 1}`}
                          loading="lazy"
                          className="absolute inset-0 h-full w-full object-cover transition-transform group-hover:scale-105"
                        />
                        <div className="absolute inset-0 bg-erp-navy/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                          <span className="p-1.5 bg-erp-surface text-erp-navy rounded-none shadow-md">
                            <IconEye size={16} strokeWidth={2} />
                          </span>
                        </div>
                      </div>
                      {img.caption && (
                        <div className="p-1.5 bg-erp-surface-subtle border-t border-erp-border">
                          <p className="text-[11px] text-erp-text-main truncate m-0" title={img.caption}>
                            {img.caption}
                          </p>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}
      </Drawer>
      <ConfirmationModal isOpen={showDeactivate} onClose={() => setShowDeactivate(false)} onConfirm={deactivateSite} title={t("deactivateSiteTitle")} message={t("deactivateSiteConfirm")} confirmText={tCommon("actions.confirm")} cancelText={tCommon("actions.cancel")} variant="danger" isLoading={isDeactivating} />

      {/* Lightbox Modal for Full View */}
      {lightboxIndex !== null && galleryItems.length > 0 && (
        <GalleryLightboxModal
          isOpen={lightboxIndex !== null}
          onClose={() => setLightboxIndex(null)}
          items={galleryItems}
          currentIndex={lightboxIndex}
          onIndexChange={(newIdx) => setLightboxIndex(newIdx)}
          title={siteLabel}
        />
      )}
    </>
  );
}
