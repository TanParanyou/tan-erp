"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { OpportunityList } from "@/features/opportunities/components/opportunity-list";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useTranslations } from "next-intl";

interface OpportunitiesPageProps {
  params: Promise<{ locale: string }>;
}

export default function OpportunitiesPage({ params }: OpportunitiesPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const { selectedMembership } = useSelectedMembership();
  const t = useTranslations("opportunities");
  const hasAccess = can(selectedMembership, "opportunities.read");

  if (!hasAccess) {
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
          {t("errors.readAccessDeniedDetail")}
        </p>
      </div>
    );
  }

  return <OpportunityList />;
}
