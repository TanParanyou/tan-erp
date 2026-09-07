"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { CustomerList } from "@/features/customers/components/customer-list";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

interface CustomersPageProps {
  params: Promise<{ locale: string }>;
}

export default function CustomersPage({ params }: CustomersPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const { selectedMembership } = useSelectedMembership();
  const hasAccess = can(selectedMembership, "customers.read");

  if (!hasAccess) {
    return (
      <div
        role="alert"
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
          Access Denied
        </h2>
        <p style={{ color: "#78350F", margin: 0, fontSize: "0.875rem" }}>
          You do not have permission to view customers in this organization.
        </p>
      </div>
    );
  }

  return <CustomerList />;
}
