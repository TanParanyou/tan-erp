"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { SupplierInvoiceDetail } from "@/features/procurement/components/supplier-invoice-detail";
import { SupplierInvoiceEditor } from "@/features/procurement/components/supplier-invoice-editor";
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
  const isCreate = id === "create" || id === "new";

  return (
    <PermissionGuard
      permission={isCreate ? PERMISSIONS.PURCHASE_ORDERS_CREATE : PERMISSIONS.PURCHASE_ORDERS_READ}
      title={t("accessDeniedTitle")}
      detail={t("accessDeniedDetail")}
    >
      {isCreate ? <SupplierInvoiceEditor /> : <SupplierInvoiceDetail supplierInvoiceId={id} />}
    </PermissionGuard>
  );
}
