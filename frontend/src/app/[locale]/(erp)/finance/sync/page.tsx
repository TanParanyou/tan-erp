"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { FinanceSyncPage } from "@/features/finance/components/finance-sync-page";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface PageProps {
  params: Promise<{ locale: string }>;
}

export default function Page({ params }: PageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("finance");

  return (
    <PermissionGuard permission={PERMISSIONS.FINANCE_SYNC_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <FinanceSyncPage />
    </PermissionGuard>
  );
}
