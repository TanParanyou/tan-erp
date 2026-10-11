"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { BranchAdminEditor } from "@/features/settings/organization/components/branch-admin-editor";

interface BranchAdministrationDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function BranchAdministrationDynamicPage({ params }: BranchAdministrationDynamicPageProps) {
  const { locale, id } = use(params);
  const t = useTranslations("organizationAdmin");

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <PermissionGuard permission={PERMISSIONS.BRANCHES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <BranchAdminEditor branchId={id} />
    </PermissionGuard>
  );
}
