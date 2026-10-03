"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { WarrantyList } from "@/features/service/components/warranty-list";
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

  const t = useTranslations("service");

  return (
    <PermissionGuard permission={PERMISSIONS.WARRANTIES_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <WarrantyList />
    </PermissionGuard>
  );
}
