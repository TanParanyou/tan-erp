"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { CustomerList } from "@/features/customers/components/customer-list";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";

interface CustomersPageProps {
  params: Promise<{ locale: string }>;
}

export default function CustomersPage({ params }: CustomersPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <PermissionGuard permission={PERMISSIONS.CUSTOMERS_READ}>
      <CustomerList />
    </PermissionGuard>
  );
}

