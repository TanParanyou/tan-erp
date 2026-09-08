"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { CustomerEditor } from "@/features/customers/components/customer-editor";
import { CustomerDetail } from "@/features/customers/components/customer-detail";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useTranslations } from "next-intl";

interface CustomerDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function CustomerDynamicPage({ params }: CustomerDynamicPageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const { selectedMembership } = useSelectedMembership();
  const t = useTranslations("customers");

  const isCreateMode = id === "create" || id === "add";

  if (isCreateMode) {
    const hasCreateAccess =
      can(selectedMembership, "customers.create") &&
      can(selectedMembership, "customer-contacts.manage");
    if (!hasCreateAccess) {
      return (
        <div
          role="alert"
          aria-live="polite"
          className="erp-card"
          style={{
            padding: "2rem",
            maxWidth: "480px",
            margin: "2rem auto",
            textAlign: "center",
            borderColor: "var(--erp-warning-border)",
            backgroundColor: "var(--erp-warning-bg)",
          }}
        >
          <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-warning)", margin: "0 0 0.5rem 0" }}>
            {t("errors.accessDeniedTitle")}
          </h2>
          <p style={{ color: "#78350F", margin: 0, fontSize: "0.875rem" }}>
            {t("errors.createAccessDeniedDetail")}
          </p>
        </div>
      );
    }
    return <CustomerEditor />;
  }

  // Detail view mode
  const hasReadAccess = can(selectedMembership, "customers.read");
  if (!hasReadAccess) {
    return (
      <div
        role="alert"
        aria-live="polite"
        className="erp-card"
        style={{
          padding: "2rem",
          maxWidth: "480px",
          margin: "2rem auto",
          textAlign: "center",
          borderColor: "var(--erp-warning-border)",
          backgroundColor: "var(--erp-warning-bg)",
        }}
      >
        <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-warning)", margin: "0 0 0.5rem 0" }}>
          {t("errors.accessDeniedTitle")}
        </h2>
        <p style={{ color: "#78350F", margin: 0, fontSize: "0.875rem" }}>
          {t("errors.readAccessDeniedDetail")}
        </p>
      </div>
    );
  }

  return <CustomerDetail customerId={id} />;
}
