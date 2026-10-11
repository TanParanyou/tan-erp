"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Textarea } from "@/components/ui/Textarea";
import { useToast } from "@/hooks/useToast";
import type { OrganizationProfileResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useOrganizationProfile, useUpdateOrganizationProfile } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";

const TAX_IDENTIFIER_PATTERN = /^\d{13}$/;

interface ProfileFormValues {
  name: string;
  nameEn: string;
  taxIdentifier: string;
  addressTh: string;
  addressEn: string;
  phone: string;
}

const orNull = (value: string): string | null => (value.trim() === "" ? null : value.trim());

/** Nullable API fields become empty inputs; blank inputs go back to the API as null. */
function toFormValues(profile: OrganizationProfileResponse): ProfileFormValues {
  return {
    name: profile.name ?? "",
    nameEn: profile.nameEn ?? "",
    taxIdentifier: profile.taxIdentifier ?? "",
    addressTh: profile.addressTh ?? "",
    addressEn: profile.addressEn ?? "",
    phone: profile.phone ?? "",
  };
}

export function OrganizationProfileForm() {
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ORGANIZATIONS_MANAGE);
  const profileQuery = useOrganizationProfile();
  const updateMutation = useUpdateOrganizationProfile();
  const [submitError, setSubmitError] = useState<string | null>(null);

  const schema = useMemo(
    () =>
      z.object({
        name: z.string().trim().min(1, t("profile.validation.nameRequired")).max(255),
        nameEn: z.string().trim().max(255),
        taxIdentifier: z
          .string()
          .trim()
          .refine((v) => v === "" || TAX_IDENTIFIER_PATTERN.test(v), t("profile.validation.taxIdentifierFormat")),
        addressTh: z.string().trim().max(500),
        addressEn: z.string().trim().max(500),
        phone: z.string().trim().max(30),
      }),
    [t]
  );

  const profile = profileQuery.data;
  const {
    register,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<ProfileFormValues>({ resolver: zodResolver(schema), values: profile ? toFormValues(profile) : undefined });

  if (profileQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (profileQuery.isError || !profile || !profile.rowVersion) return <Alert variant="danger">{t("errors.GENERIC")}</Alert>;

  // Narrowed here so the submit closure keeps a non-optional row version (concurrency token for PUT).
  const profileRowVersion = profile.rowVersion;

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    try {
      await updateMutation.mutateAsync({
        ifMatch: profileRowVersion,
        payload: {
          name: values.name.trim(),
          nameEn: orNull(values.nameEn),
          taxIdentifier: orNull(values.taxIdentifier),
          addressTh: orNull(values.addressTh),
          addressEn: orNull(values.addressEn),
          phone: orNull(values.phone),
        },
      });
      toast.success(t("profile.saveSuccess"));
    } catch (error: unknown) {
      setSubmitError(t(`errors.${organizationAdminErrorKey(error)}`));
    }
  });

  return (
    <FormContainer
      asForm
      noValidate
      onSubmit={onSubmit}
      maxWidth="lg"
      header={<PageHeader title={t("profile.title")} subtitle={t("profile.subtitle")} />}
      errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
      actionBar={
        canManage ? (
          <FormActionBar
            isDirty={isDirty}
            isLoading={updateMutation.isPending}
            isSaveDisabled={updateMutation.isPending}
            saveText={t("profile.save")}
            saveButtonType="submit"
          />
        ) : undefined
      }
    >
      {!canManage ? <Alert variant="info">{t("profile.readOnlyNotice")}</Alert> : null}
      <FormSection title={t("profile.title")} description={t("profile.subtitle")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <Input label={t("profile.fields.name")} required disabled={!canManage} error={errors.name?.message} {...register("name")} />
          <Input label={t("profile.fields.nameEn")} disabled={!canManage} error={errors.nameEn?.message} {...register("nameEn")} />
          <Input
            label={t("profile.fields.taxIdentifier")}
            helperText={t("profile.fields.taxIdentifierHint")}
            inputMode="numeric"
            maxLength={13}
            disabled={!canManage}
            error={errors.taxIdentifier?.message}
            {...register("taxIdentifier")}
          />
          <Input label={t("profile.fields.phone")} disabled={!canManage} error={errors.phone?.message} {...register("phone")} />
          <Textarea label={t("profile.fields.addressTh")} rows={3} disabled={!canManage} error={errors.addressTh?.message} {...register("addressTh")} />
          <Textarea label={t("profile.fields.addressEn")} rows={3} disabled={!canManage} error={errors.addressEn?.message} {...register("addressEn")} />
        </div>
      </FormSection>
    </FormContainer>
  );
}
