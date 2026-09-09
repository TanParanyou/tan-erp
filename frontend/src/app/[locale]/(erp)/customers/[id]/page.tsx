"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { CustomerEditor } from "@/features/customers/components/customer-editor";
import { CustomerDetail } from "@/features/customers/components/customer-detail";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface CustomerDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function CustomerDynamicPage({ params }: CustomerDynamicPageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("customers");

  const isCreateMode = id === "create" || id === "add";

  if (isCreateMode) {
    return (
      <PermissionGuard
        permission={[PERMISSIONS.CUSTOMERS_CREATE, PERMISSIONS.CUSTOMER_CONTACTS_MANAGE]}
        requireAll={true}
        title={t("errors.accessDeniedTitle")}
        detail={t("errors.createAccessDeniedDetail")}
      >
        <CustomerEditor />
      </PermissionGuard>
    );
  }

  // Detail view mode
  return (
    <PermissionGuard
      permission={PERMISSIONS.CUSTOMERS_READ}
      title={t("errors.accessDeniedTitle")}
      detail={t("errors.readAccessDeniedDetail")}
    >
      <CustomerDetail customerId={id} />
    </PermissionGuard>
  );
}
