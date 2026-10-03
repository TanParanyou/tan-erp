"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { BomDetail } from "@/features/production/components/bom-detail";
import { BomEditor } from "@/features/production/components/bom-editor";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface PageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function Page({ params }: PageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("production");

  return (
    <PermissionGuard permission={PERMISSIONS.BOMS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      {id === "create" ? <BomEditor /> : <BomDetail bomId={id} />}
    </PermissionGuard>
  );
}
