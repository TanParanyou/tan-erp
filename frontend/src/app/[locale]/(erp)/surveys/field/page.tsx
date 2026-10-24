"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { FieldSurveyWorkspace } from "@/features/surveys/components/field/field-survey-workspace";

interface FieldSurveyPageProps {
  params: Promise<{ locale: string }>;
  searchParams?: Promise<{ opportunityId?: string }>;
}

export default function FieldSurveyPage({ params, searchParams }: FieldSurveyPageProps) {
  const { locale } = use(params);
  const resolvedSearchParams = searchParams ? use(searchParams) : undefined;
  const initialOpportunityId = resolvedSearchParams?.opportunityId || undefined;

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
      <FieldSurveyWorkspace initialOpportunityId={initialOpportunityId} />
    </PermissionGuard>
  );
}
