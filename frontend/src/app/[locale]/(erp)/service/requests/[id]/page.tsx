"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { ServiceRequestDetail } from "@/features/service/components/service-request-detail";
import { ServiceRequestEditor } from "@/features/service/components/service-request-editor";
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

  const t = useTranslations("service");

  return (
    <PermissionGuard permission={PERMISSIONS.SERVICE_REQUESTS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      {id === "create" ? <ServiceRequestEditor /> : <ServiceRequestDetail requestId={id} />}
    </PermissionGuard>
  );
}
