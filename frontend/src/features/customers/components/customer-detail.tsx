"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerDetail } from "../api/customer-queries";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { IconChevronLeft, IconAlertCircle, IconFileText } from "@/components/common/Icons";

interface CustomerDetailProps {
  customerId: string;
}

export function CustomerDetail({ customerId }: CustomerDetailProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const { data: customer, isLoading, isError, error, refetch } = useCustomerDetail(customerId);

  if (isLoading) {
    return (
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "300px",
          backgroundColor: "var(--erp-surface)",
          border: "1px solid var(--erp-border)",
        }}
      >
        <MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" />
      </div>
    );
  }

  if (isError || !customer) {
    return (
      <div
        role="alert"
        className="erp-card"
        style={{
          padding: "2rem",
          borderColor: "var(--erp-danger-border)",
          backgroundColor: "var(--erp-danger-bg)",
          textAlign: "center",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          gap: "1rem",
        }}
      >
        <IconAlertCircle size={32} style={{ color: "var(--erp-danger)" }} />
        <div>
          <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-danger)", margin: "0 0 0.5rem 0" }}>
            {error?.message || "Customer not found"}
          </h2>
        </div>
        <div style={{ display: "flex", gap: "1rem" }}>
          <Button variant="outline" size="md" onClick={() => refetch()} style={{ minHeight: "44px" }}>
            {tCommon("actions.cancel")}
          </Button>
          <Link href={`/${locale}/customers`} style={{ textDecoration: "none" }}>
            <Button variant="primary" size="md" style={{ minHeight: "44px" }}>
              {t("backToList")}
            </Button>
          </Link>
        </div>
      </div>
    );
  }

  const displayName =
    locale === "en" && customer.displayNameEn
      ? customer.displayNameEn
      : customer.displayNameTh || customer.displayNameEn || "-";

  const contact = customer.primaryContact;

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "1.5rem", maxWidth: "800px" }}>
      {/* Top Header & Back link */}
      <div style={{ display: "flex", flexDirection: "column", gap: "0.75rem", borderBottom: "1px solid var(--erp-border)", paddingBottom: "1.25rem" }}>
        <Link
          href={`/${locale}/customers`}
          style={{
            display: "inline-flex",
            alignItems: "center",
            gap: "0.25rem",
            color: "var(--erp-navy)",
            textDecoration: "none",
            fontSize: "0.875rem",
            fontWeight: 600,
          }}
        >
          <IconChevronLeft size={16} />
          <span>{t("backToList")}</span>
        </Link>

        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: "1rem" }}>
          <div>
            <div style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
              <h1 style={{ fontSize: "1.5rem", fontWeight: 700, color: "var(--erp-navy)", margin: 0 }}>
                {displayName}
              </h1>
              <span
                className={`erp-badge ${
                  customer.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
                }`}
              >
                {customer.status === "active" ? t("active") : t("inactive")}
              </span>
            </div>
            <span style={{ fontFamily: "monospace", color: "var(--erp-text-muted)", fontSize: "0.875rem" }}>
              {customer.code}
            </span>
          </div>
        </div>
      </div>

      {/* Duplicate Candidates Warning Banner if present in response */}
      {customer.duplicateCandidates && customer.duplicateCandidates.length > 0 && (
        <div
          role="region"
          aria-label={t("duplicateCandidates")}
          className="erp-card"
          style={{
            padding: "1.25rem",
            borderColor: "var(--erp-warning-border)",
            backgroundColor: "var(--erp-warning-bg)",
          }}
        >
          <h3 style={{ fontSize: "1rem", fontWeight: 700, color: "var(--erp-warning)", margin: "0 0 0.5rem 0" }}>
            {t("duplicateCandidates")}
          </h3>
          <p style={{ fontSize: "0.875rem", color: "#78350F", margin: "0 0 0.75rem 0" }}>
            {t("duplicateNotice")}
          </p>
          <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "flex", flexDirection: "column", gap: "0.5rem" }}>
            {customer.duplicateCandidates.map((dup) => (
              <li
                key={dup.id}
                style={{
                  padding: "0.5rem 0.75rem",
                  backgroundColor: "var(--erp-surface)",
                  border: "1px solid var(--erp-border)",
                  fontSize: "0.8125rem",
                  display: "flex",
                  justifyContent: "space-between",
                }}
              >
                <strong>{dup.code} - {dup.displayNameTh}</strong>
                <span>{dup.maskedPhone || dup.maskedEmail}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Detail Content */}
      <div className="erp-card" style={{ padding: "1.5rem", display: "flex", flexDirection: "column", gap: "1.25rem" }}>
        <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-navy)", margin: 0, borderBottom: "1px solid var(--erp-border-subtle)", paddingBottom: "0.75rem" }}>
          {t("title")}
        </h2>

        <dl className="erp-dl">
          <dt>{t("code")}:</dt>
          <dd style={{ fontFamily: "monospace", fontWeight: 600 }}>{customer.code || "-"}</dd>

          <dt>{t("customerType")}:</dt>
          <dd>
            <span className="erp-badge erp-badge-neutral">
              {customer.customerType === "corporate" ? t("corporate") : t("individual")}
            </span>
          </dd>

          <dt>{t("displayNameTh")}:</dt>
          <dd style={{ fontWeight: 600 }}>{customer.displayNameTh || "-"}</dd>

          <dt>{t("displayNameEn")}:</dt>
          <dd>{customer.displayNameEn || "-"}</dd>

          <dt>{t("preferredLocale")}:</dt>
          <dd>{customer.preferredLocale === "en" ? "English" : "ไทย (Thai)"}</dd>
        </dl>
      </div>

      {/* Primary Contact Section */}
      <div className="erp-card" style={{ padding: "1.5rem", display: "flex", flexDirection: "column", gap: "1.25rem" }}>
        <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-navy)", margin: 0, borderBottom: "1px solid var(--erp-border-subtle)", paddingBottom: "0.75rem" }}>
          {t("primaryContact")}
        </h2>

        {contact ? (
          <dl className="erp-dl">
            <dt>{t("contactName")}:</dt>
            <dd style={{ fontWeight: 600 }}>{contact.name || "-"}</dd>

            <dt>{t("roleTitle")}:</dt>
            <dd>{contact.roleTitle || "-"}</dd>

            <dt>{t("phone")}:</dt>
            <dd style={{ fontFamily: "monospace" }}>{contact.phone || "-"}</dd>

            <dt>{t("email")}:</dt>
            <dd>{contact.email || "-"}</dd>

            <dt>{t("preferredChannel")}:</dt>
            <dd>
              <span className="erp-badge erp-badge-neutral">
                {contact.preferredChannel?.toUpperCase() || "-"}
              </span>
            </dd>
          </dl>
        ) : (
          <p style={{ color: "var(--erp-text-muted)", fontSize: "0.875rem", margin: 0 }}>
            -
          </p>
        )}
      </div>
    </div>
  );
}
