"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { UserAdminDetail } from "@/features/settings/user-administration/components/user-admin-detail";
import { UserAdminEditor } from "@/features/settings/user-administration/components/user-admin-editor";

interface UserAdministrationDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function UserAdministrationDynamicPage({ params }: UserAdministrationDynamicPageProps) {
  const { locale, id } = use(params);
  const t = useTranslations("userAdmin");

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  if (id === "create" || id === "add") {
    return (
      <PermissionGuard permission={PERMISSIONS.USERS_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
        <UserAdminEditor />
      </PermissionGuard>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.USERS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <UserAdminDetail userId={id} />
    </PermissionGuard>
  );
}
