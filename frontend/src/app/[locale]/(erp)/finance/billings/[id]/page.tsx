"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { BillingDetail } from "@/features/finance/components/billing-detail";
import { BillingEditor } from "@/features/finance/components/billing-editor";
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

  const t = useTranslations("finance");

  return (
    <PermissionGuard permission={PERMISSIONS.BILLINGS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      {id === "create" ? <BillingEditor /> : <BillingDetail billingId={id} />}
    </PermissionGuard>
  );
}
