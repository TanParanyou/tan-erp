"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { SiteEditor } from "@/features/sites/components/site-editor";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useTranslations } from "next-intl";

interface SiteDynamicPageProps {
  params: Promise<{ locale: string; id: string; siteId: string }>;
}

export default function SiteDynamicPage({ params }: SiteDynamicPageProps) {
  const { locale, id: customerId, siteId } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  // Slice 2 authorization guard: only create is allowed in this vertical slice
  if (siteId !== "create") {
    notFound();
  }

  const { selectedMembership } = useSelectedMembership();
  const t = useTranslations("sites");

  const hasManageAccess = can(selectedMembership, "sites.manage");
  if (!hasManageAccess) {
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
        <p style={{ color: "var(--erp-warning-text)", margin: 0, fontSize: "0.875rem" }}>
          {t("errors.createAccessDeniedDetail")}
        </p>
      </div>
    );
  }

  return <SiteEditor customerId={customerId} />;
}
