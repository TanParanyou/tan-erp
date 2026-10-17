"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { AuditEventList } from "@/features/settings/audit/components/audit-event-list";

export default function AuditSettingsPage() {
  const t = useTranslations("auditLog");

  return (
    <PermissionGuard permission={PERMISSIONS.AUDIT_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <AuditEventList />
    </PermissionGuard>
  );
}
