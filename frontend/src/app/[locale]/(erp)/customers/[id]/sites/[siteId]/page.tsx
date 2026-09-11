"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { SiteEditor } from "@/features/sites/components/site-editor";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface SiteDynamicPageProps {
  params: Promise<{ locale: string; id: string; siteId: string }>;
}

export default function SiteDynamicPage({ params }: SiteDynamicPageProps) {
  const { locale, id: customerId, siteId } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  // Slice 2 authorization guard: only create is allowed in this vertical slice
  if (siteId !== "create") {
    notFound();
  }

  const t = useTranslations("sites");

  return (
    <PermissionGuard
      permission={PERMISSIONS.SITES_MANAGE}
      title={t("errors.accessDeniedTitle")}
      detail={t("errors.createAccessDeniedDetail")}
    >
      <SiteEditor customerId={customerId} />
    </PermissionGuard>
  );
}
