"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerDetail } from "../api/customer-queries";
import { DuplicateCandidateCard } from "./duplicate-candidate-card";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { IconChevronLeft, IconAlertCircle, IconFileText } from "@/components/common/Icons";
import { getContactChannelLabelKey, getCustomerLeadSourceLabelKey, getCustomerStatusLabelKey, getCustomerTypeLabelKey } from "../customer-labels";

interface CustomerDetailProps {
  customerId: string;
}

export function CustomerDetail({ customerId }: CustomerDetailProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const resolveCustomerTypeLabel = (value: string | null | undefined): string => {
    const key = getCustomerTypeLabelKey(value);
    return key ? t(key) : "-";
  };

  const resolveCustomerStatusLabel = (value: string | null | undefined): string => {
    const key = getCustomerStatusLabelKey(value);
    return key ? tCommon(`status.${key}`) : "-";
  };

  const resolveDetailErrorMessage = (error: Error | null): string => {
    if (error?.message === "No authentication token available") {
      return t("errors.authenticationRequired");
    }
    if (error?.message === "No active membership selected") {
      return t("errors.membershipRequired");
    }
    return t("errors.loadDetail");
  };

  const { data: customer, isLoading, isError, error, refetch } = useCustomerDetail(customerId);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[300px] bg-erp-surface border border-erp-border">
        <MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" />
      </div>
    );
  }

  if (isError || !customer) {
    return (
      <div
        role="alert"
        aria-live="polite"
        className="erp-card p-8 border-erp-danger-border bg-erp-danger-bg text-center flex flex-col items-center gap-4"
      >
        <IconAlertCircle size={32} className="text-erp-danger" />
        <div>
          <h2 className="text-lg font-bold text-erp-danger mb-2">
            {resolveDetailErrorMessage(error)}
          </h2>
        </div>
        <div className="flex gap-4">
          <Button variant="outline" size="md" onClick={() => refetch()} className="min-h-[44px]">
            {tCommon("actions.retry")}
          </Button>
          <Button href={`/${locale}/customers`} variant="primary" size="md" className="min-h-[44px]">
            {t("backToList")}
          </Button>
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
    <div className="flex flex-col gap-6 max-w-[800px]">
      {/* Top Header & Back link */}
      <div className="flex flex-col gap-3 border-b border-erp-border pb-5">
        <Link
          href={`/${locale}/customers`}
          className="inline-flex items-center gap-1 text-erp-navy no-underline text-sm font-semibold hover:underline"
        >
          <IconChevronLeft size={16} />
          <span>{t("backToList")}</span>
        </Link>

        <div className="flex justify-between items-center flex-wrap gap-4">
          <div>
            <div className="flex items-center gap-3">
              <h1 className="text-2xl font-bold text-erp-navy m-0">
                {displayName}
              </h1>
              <span
                className={`erp-badge ${
                  customer.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
                }`}
              >
                {resolveCustomerStatusLabel(customer.status)}
              </span>
            </div>
            <span className="font-mono text-erp-text-muted text-sm">
              {customer.code}
            </span>
          </div>
        </div>
      </div>

      {/* Duplicate Candidates reuse shared card; masked values only */}
      {customer.duplicateCandidates && customer.duplicateCandidates.length > 0 && (
        <DuplicateCandidateCard candidates={customer.duplicateCandidates} />
      )}

      {/* Detail Content */}
      <div className="erp-card p-6 flex flex-col gap-5">
        <h2 className="text-lg font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3">
          {t("title")}
        </h2>

        <dl className="erp-dl">
          <dt>{t("code")}:</dt>
          <dd className="font-mono font-semibold">{customer.code || "-"}</dd>

          <dt>{t("customerType")}:</dt>
          <dd>
            <span className="erp-badge erp-badge-neutral">
              {resolveCustomerTypeLabel(customer.customerType)}
            </span>
          </dd>

          <dt>{t("displayNameTh")}:</dt>
          <dd className="font-semibold">{customer.displayNameTh || "-"}</dd>

          <dt>{t("displayNameEn")}:</dt>
          <dd>{customer.displayNameEn || "-"}</dd>

          <dt>{t("preferredLocale")}:</dt>
          <dd>{customer.preferredLocale === "en" ? t("localeEnglish") : t("localeThai")}</dd>

          <dt>{t("leadSource")}:</dt>
          <dd>
            {customer.leadSource ? (
              <span className="erp-badge erp-badge-neutral">
                {(() => {
                  const key = getCustomerLeadSourceLabelKey(customer.leadSource);
                  return key ? t(key) : customer.leadSource;
                })()}
              </span>
            ) : (
              "-"
            )}
          </dd>
        </dl>
      </div>

      {/* Primary Contact Section */}
      <div className="erp-card p-6 flex flex-col gap-5">
        <h2 className="text-lg font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3">
          {t("primaryContact")}
        </h2>

        {contact ? (
          <dl className="erp-dl">
            <dt>{t("contactName")}:</dt>
            <dd className="font-semibold">{contact.name || "-"}</dd>

            <dt>{t("roleTitle")}:</dt>
            <dd>{contact.roleTitle || "-"}</dd>

            <dt>{t("phone")}:</dt>
            <dd className="font-mono">{contact.phone || "-"}</dd>

            <dt>{t("email")}:</dt>
            <dd>{contact.email || "-"}</dd>

            <dt>{t("lineId")}:</dt>
            <dd className="font-mono">{contact.lineId || "-"}</dd>

            <dt>{t("preferredChannel")}:</dt>
            <dd>
              <span className="erp-badge erp-badge-neutral">
                {(() => {
                  const key = getContactChannelLabelKey(contact.preferredChannel);
                  return key ? t(key) : "-";
                })()}
              </span>
            </dd>
          </dl>
        ) : (
          <p className="text-erp-text-muted text-sm m-0">
            {t("noPrimaryContact")}
          </p>
        )}
      </div>
    </div>
  );
}
