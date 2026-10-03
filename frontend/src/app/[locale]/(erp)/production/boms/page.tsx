"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { BomList } from "@/features/production/components/bom-list";
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

  const t = useTranslations("production");

  return (
    <PermissionGuard permission={PERMISSIONS.BOMS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <BomList />
    </PermissionGuard>
  );
}
