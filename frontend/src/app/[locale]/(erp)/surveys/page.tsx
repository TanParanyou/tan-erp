"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { SurveyList } from "@/features/surveys/components/survey-list";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface SurveysPageProps {
  params: Promise<{ locale: string }>;
}

export default function SurveysPage({ params }: SurveysPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const tCommon = useTranslations("common");

  return (
    <PermissionGuard
      permission={PERMISSIONS.OPPORTUNITIES_READ}
      title={tCommon("errors.accessDeniedTitle")}
      detail={tCommon("errors.readAccessDeniedDetail")}
    >
      <SurveyList />
    </PermissionGuard>
  );
}
