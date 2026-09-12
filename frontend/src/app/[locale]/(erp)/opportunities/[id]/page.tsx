"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { OpportunityEditor } from "@/features/opportunities/components/opportunity-editor";
import { OpportunityDetail } from "@/features/opportunities/components/opportunity-detail";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface OpportunityDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function OpportunityDynamicPage({ params }: OpportunityDynamicPageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("opportunities");
  const isCreateMode = id === "create" || id === "add";

  if (isCreateMode) {
    return (
      <PermissionGuard
        permission={PERMISSIONS.OPPORTUNITIES_CREATE}
        title={t("errors.accessDeniedTitle")}
        detail={t("errors.createAccessDeniedDetail")}
      >
        <OpportunityEditor />
      </PermissionGuard>
    );
  }

  return (
    <PermissionGuard
      permission={PERMISSIONS.OPPORTUNITIES_READ}
      title={t("errors.accessDeniedTitle")}
      detail={t("errors.readAccessDeniedDetail")}
    >
      <OpportunityDetail opportunityId={id} />
    </PermissionGuard>
  );
}

