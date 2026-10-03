"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PurchaseOrderDetail } from "@/features/procurement/components/purchase-order-detail";
import { PurchaseOrderEditor } from "@/features/procurement/components/purchase-order-editor";
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

  const t = useTranslations("procurement");

  return (
    <PermissionGuard permission={PERMISSIONS.PURCHASE_ORDERS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      {id === "create" ? <PurchaseOrderEditor /> : <PurchaseOrderDetail purchaseOrderId={id} />}
    </PermissionGuard>
  );
}
