"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { RoleDefinitionList } from "@/features/settings/role-management/components/role-definition-list";

export default function RoleSettingsPage() {
  const t = useTranslations("roleAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <RoleDefinitionList />
    </PermissionGuard>
  );
}
