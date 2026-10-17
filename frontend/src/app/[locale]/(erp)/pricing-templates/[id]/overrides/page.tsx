"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PricingTemplateOverrides } from "@/features/quick-estimates/components/pricing-template-overrides";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";

interface PageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function Page({ params }: PageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("quickEstimates");

  return (
    <PermissionGuard permission={PERMISSIONS.PRICING_TEMPLATES_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <PricingTemplateOverrides templateId={id} />
    </PermissionGuard>
  );
}
