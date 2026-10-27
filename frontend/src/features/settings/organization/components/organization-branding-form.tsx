"use client";

import React, { useState, type CSSProperties } from "react";
import { useLocale, useTranslations } from "next-intl";
import { FormSection } from "@/components/forms/FormSection";
import { AvatarUpload } from "@/components/forms/AvatarUpload";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { useToast } from "@/hooks/useToast";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import { getAuthToken } from "@/lib/auth/auth-session";
import { brandCssVariables, checkBrandContrast, parseHexColor } from "@/lib/color/wcag";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useTheme } from "@/providers/theme-provider";
import { useOrganizationBranding, useUpdateOrganizationBranding } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";

function ContrastRow({ label, ratio, passes, passText, failText }: { label: string; ratio: number; passes: boolean; passText: string; failText: string }) {
  return (
    <li className="flex items-center justify-between gap-4 border-b border-erp-border-subtle py-2 text-sm">
      <span className="text-erp-text-body">{label}</span>
      <span className={passes ? "font-semibold text-erp-success" : "font-semibold text-erp-danger"}>
        {ratio.toFixed(2)}:1 {passes ? passText : failText}
      </span>
    </li>
  );
}

export function OrganizationBrandingForm() {
  const t = useTranslations("organizationAdmin.branding");
  const tErrors = useTranslations("organizationAdmin.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const context = useApiRequestContext();
  const { resolvedTheme } = useTheme();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ORGANIZATIONS_MANAGE);
  const brandingQuery = useOrganizationBranding();
  const updateMutation = useUpdateOrganizationBranding();
  const { uploadSingleFile, isUploading } = useDeferredFileUpload({ parentType: "user" });

  const [draftColor, setDraftColor] = useState<string | null>(null);
  const [logoFile, setLogoFile] = useState<File | null>(null);
  const [logoRemoved, setLogoRemoved] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const branding = brandingQuery.data;
  if (brandingQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (brandingQuery.isError || !branding || !branding.rowVersion) return <Alert variant="danger">{tErrors("GENERIC")}</Alert>;

  const rowVersion = branding.rowVersion;
  const savedColor = branding.primaryColor ?? "";
  const colorText = (draftColor ?? savedColor).trim();
  const rgb = colorText === "" ? null : parseHexColor(colorText);
  const isFormatInvalid = colorText !== "" && rgb === null;
  const contrast = rgb ? checkBrandContrast(rgb) : null;
  const isContrastFailing = contrast !== null && !(contrast.textPasses && contrast.canvasPasses);
  const isBusy = updateMutation.isPending || isUploading;
  const isDirty = colorText.toUpperCase() !== savedColor.toUpperCase() || logoFile !== null || logoRemoved;
  const blockReason = isFormatInvalid ? t("validation.hexFormat") : isContrastFailing ? t("validation.contrastFailed") : null;
  const previewStyle = rgb ? (brandCssVariables(rgb, resolvedTheme) as CSSProperties) : undefined;

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (blockReason !== null || isBusy) return;
    setSubmitError(null);

    let logoFileId: string | null = logoRemoved ? null : branding.logoFileId ?? null;
    if (logoFile) {
      try {
        const token = await getAuthToken();
        const uploaded = await uploadSingleFile(logoFile, { token: token ?? "", membershipId: context.membershipId, locale });
        logoFileId = uploaded?.fileId ?? null;
      } catch (error: unknown) {
        setSubmitError(error instanceof Error ? error.message : tErrors("GENERIC"));
        return;
      }
    }

    try {
      await updateMutation.mutateAsync({
        ifMatch: rowVersion,
        payload: { primaryColor: colorText === "" ? null : colorText.toUpperCase(), logoFileId },
      });
      toast.success(t("saveSuccess"));
      setDraftColor(null);
      setLogoFile(null);
      setLogoRemoved(false);
    } catch (error: unknown) {
      setSubmitError(tErrors(organizationAdminErrorKey(error)));
    }
  };

  return (
    <form onSubmit={onSubmit} noValidate className="mx-auto mt-6 flex w-full max-w-5xl flex-col gap-6">
      {submitError ? <Alert variant="danger">{submitError}</Alert> : null}
      <FormSection title={t("title")} description={t("subtitle")}>
        <div className="flex flex-col gap-6">
          <AvatarUpload
            file={logoFile}
            existingFileId={logoRemoved ? null : branding.logoFileId}
            onFileChange={(file) => {
              setLogoFile(file);
              setLogoRemoved(file === null);
            }}
            disabled={!canManage || isBusy}
            uploadButtonText={t("logo.upload")}
            deleteButtonText={t("logo.remove")}
            hintText={t("logo.hint")}
          />

          <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
            <div className="flex flex-col gap-3">
              <div className="flex items-end gap-2">
                <div className="flex-1">
                  <Input
                    label={t("color.label")}
                    helperText={t("color.hint")}
                    placeholder={t("color.placeholder")}
                    maxLength={7}
                    disabled={!canManage || isBusy}
                    value={draftColor ?? savedColor}
                    onChange={(event) => setDraftColor(event.target.value)}
                    error={isFormatInvalid ? t("validation.hexFormat") : undefined}
                  />
                </div>
                <Button
                  type="button"
                  variant="outline"
                  disabled={!canManage || isBusy || colorText === ""}
                  onClick={() => setDraftColor("")}
                >
                  {t("color.reset")}
                </Button>
              </div>
              {contrast ? (
                <ul aria-live="polite" className="flex flex-col">
                  <ContrastRow label={t("contrast.onText")} ratio={contrast.textRatio} passes={contrast.textPasses} passText={t("contrast.pass")} failText={t("contrast.fail")} />
                  <ContrastRow label={t("contrast.onCanvas")} ratio={contrast.canvasRatio} passes={contrast.canvasPasses} passText={t("contrast.pass")} failText={t("contrast.fail")} />
                </ul>
              ) : (
                <p className="text-sm text-erp-text-muted">{t("color.default")}</p>
              )}
            </div>

            <div className="flex flex-col gap-2">
              <span className="text-sm font-medium text-erp-text-main">{t("preview.label")}</span>
              <div className="flex items-center gap-3 border border-erp-border bg-erp-surface-muted p-4" style={previewStyle}>
                <Button type="button" variant="primary" tabIndex={-1}>
                  {t("preview.button")}
                </Button>
              </div>
            </div>
          </div>

          {blockReason ? <Alert variant="warning">{blockReason}</Alert> : null}
        </div>
      </FormSection>

      {canManage ? (
        <div className="flex justify-end">
          <Button type="submit" isLoading={isBusy} disabled={isBusy || !isDirty || blockReason !== null}>
            {t("save")}
          </Button>
        </div>
      ) : (
        <Alert variant="info">{t("readOnlyNotice")}</Alert>
      )}
    </form>
  );
}
