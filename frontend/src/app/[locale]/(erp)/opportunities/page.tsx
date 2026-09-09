"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { OpportunityList } from "@/features/opportunities/components/opportunity-list";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface OpportunitiesPageProps {
  params: Promise<{ locale: string }>;
}

export default function OpportunitiesPage({ params }: OpportunitiesPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("opportunities");

  return (
    <PermissionGuard
      permission={PERMISSIONS.OPPORTUNITIES_READ}
      title={t("errors.accessDeniedTitle")}
      detail={t("errors.readAccessDeniedDetail")}
    >
      <OpportunityList />
    </PermissionGuard>
  );
}

