"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { RoleRequestQueue } from "@/features/settings/user-administration/components/role-request-queue";
import { RoleChangeRequestQueue } from "@/features/settings/role-management/components/role-change-request-queue";

export default function RoleRequestsPage() {
  const t = useTranslations("userAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES_ASSIGN_APPROVAL} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <div className="space-y-10">
        <RoleRequestQueue />
        <RoleChangeRequestQueue />
      </div>
    </PermissionGuard>
  );
}
