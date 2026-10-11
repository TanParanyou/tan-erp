"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { OrganizationProfileForm } from "@/features/settings/organization/components/organization-profile-form";

export default function OrganizationSettingsPage() {
  const t = useTranslations("organizationAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.ORGANIZATIONS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <OrganizationProfileForm />
    </PermissionGuard>
  );
}
