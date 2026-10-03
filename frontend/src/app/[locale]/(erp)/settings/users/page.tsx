"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { UserAdminList } from "@/features/settings/user-administration/components/user-admin-list";

export default function UserAdministrationPage() {
  const t = useTranslations("userAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.USERS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <UserAdminList />
    </PermissionGuard>
  );
}
