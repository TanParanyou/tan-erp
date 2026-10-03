"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { BomEditor } from "@/features/production/components/bom-editor";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface PageProps {
  params: Promise<{ locale: string; id: string; revisionId: string }>;
}

export default function Page({ params }: PageProps) {
  const { locale, id, revisionId } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("production");

  return (
    <PermissionGuard permission={PERMISSIONS.BOMS_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <BomEditor bomId={id} revisionId={revisionId} />
    </PermissionGuard>
  );
}
