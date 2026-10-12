"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ApprovalLimitEditor } from "@/features/settings/approval-limits/components/approval-limit-editor";

interface ApprovalLimitDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function ApprovalLimitDynamicPage({ params }: ApprovalLimitDynamicPageProps) {
  const { locale, id } = use(params);
  const t = useTranslations("approvalLimitAdmin");

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <PermissionGuard permission={PERMISSIONS.APPROVAL_LIMITS_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <ApprovalLimitEditor limitId={id} />
    </PermissionGuard>
  );
}
