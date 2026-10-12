"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { RoleDefinitionEditor } from "@/features/settings/role-management/components/role-definition-editor";

interface RoleDefinitionDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function RoleDefinitionDynamicPage({ params }: RoleDefinitionDynamicPageProps) {
  const { locale, id } = use(params);
  const t = useTranslations("roleAdmin");

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <RoleDefinitionEditor roleId={id} />
    </PermissionGuard>
  );
}
