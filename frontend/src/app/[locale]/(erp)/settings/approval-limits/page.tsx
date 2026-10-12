"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ApprovalLimitList } from "@/features/settings/approval-limits/components/approval-limit-list";

export default function ApprovalLimitSettingsPage() {
  const t = useTranslations("approvalLimitAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.APPROVAL_LIMITS_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <ApprovalLimitList />
    </PermissionGuard>
  );
}
