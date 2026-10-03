"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { MrpRunDetail } from "@/features/mrp/components/mrp-run-detail";
import { MrpRunEditor } from "@/features/mrp/components/mrp-run-editor";
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

  const t = useTranslations("mrp");

  return (
    <PermissionGuard permission={PERMISSIONS.MRP_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      {id === "create" ? <MrpRunEditor /> : <MrpRunDetail runId={id} />}
    </PermissionGuard>
  );
}
