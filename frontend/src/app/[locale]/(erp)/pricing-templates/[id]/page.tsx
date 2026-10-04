"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PricingTemplateDetail } from "@/features/quick-estimates/components/pricing-template-detail";
import { PricingTemplateEditor } from "@/features/quick-estimates/components/pricing-template-editor";
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

  const t = useTranslations("quickEstimates");

  return (
    <PermissionGuard permission={PERMISSIONS.PRICING_TEMPLATES_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      {id === "create" ? <PricingTemplateEditor /> : <PricingTemplateDetail templateId={id} />}
    </PermissionGuard>
  );
}
