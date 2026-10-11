"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { BranchAdminList } from "@/features/settings/organization/components/branch-admin-list";

export default function BranchSettingsPage() {
  const t = useTranslations("organizationAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.BRANCHES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <BranchAdminList />
    </PermissionGuard>
  );
}
