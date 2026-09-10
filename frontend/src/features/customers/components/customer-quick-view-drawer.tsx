"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { IconAlertCircle } from "@/components/common/Icons";
import { useCustomerDetail } from "../api/customer-queries";
import { isAuthenticationRequiredError, isMembershipRequiredError } from "@/lib/api/api-error";
import { getCustomerStatusLabelKey, getCustomerTypeLabelKey } from "../customer-labels";

export interface CustomerQuickViewDrawerProps {
  customerId: string | null;
  isOpen: boolean;
  onClose: () => void;
}

export function CustomerQuickViewDrawer({
  customerId,
  isOpen,
  onClose,
}: CustomerQuickViewDrawerProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const {
    data: customer,
    isLoading,
    isError,
    error,
    refetch,
  } = useCustomerDetail(customerId);

  const resolveDetailErrorMessage = (err: Error | null): string => {
    if (isAuthenticationRequiredError(err)) {
      return t("errors.authenticationRequired");
    }
    if (isMembershipRequiredError(err)) {
      return t("errors.membershipRequired");
    }
    return t("errors.loadDetail");
  };

  const displayName = customer
    ? locale === "en" && customer.displayNameEn
      ? customer.displayNameEn
      : customer.displayNameTh || customer.displayNameEn || "-"
    : "-";

  const statusKey = customer ? getCustomerStatusLabelKey(customer.status) : null;
  const statusLabel = statusKey ? tCommon(`status.${statusKey}`) : "-";
  const typeKey = customer ? getCustomerTypeLabelKey(customer.customerType) : null;
  const typeLabel = typeKey ? t(typeKey) : "-";

  const contact = customer?.primaryContact;

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={t("quickViewTitle")}
      size="md"
      footer={
        <div className="flex justify-between items-center w-full gap-3">
          <Button variant="outline" size="sm" onClick={onClose} className="min-h-[40px]">
            {tCommon("actions.close")}
          </Button>
          {customer && (
            <Button
              href={`/${locale}/customers/${customer.id}`}
              variant="primary"
              size="sm"
              className="min-h-[40px]"
            >
              {t("viewFullDetail")}
            </Button>
          )}
        </div>
      }
    >
      {isLoading ? (
        <div className="flex items-center justify-center min-h-[250px]">
          <MonoSpinner size="md" label={tCommon("states.loading")} aria-busy="true" />
        </div>
      ) : isError || !customer ? (
        <div
          role="alert"
          aria-live="polite"
          className="erp-card p-6 border-erp-danger-border bg-erp-danger-bg text-center flex flex-col items-center gap-3"
        >
          <IconAlertCircle size={28} className="text-erp-danger" />
          <p className="text-sm font-bold text-erp-danger">
            {resolveDetailErrorMessage(error)}
          </p>
          <Button
            variant="outline"
            size="sm"
            onClick={() => refetch()}
            className="min-h-[36px]"
          >
            {tCommon("actions.retry")}
          </Button>
        </div>
      ) : (
      <div className="space-y-6">
        {/* Customer Header Info */}
        <div className="p-4 bg-erp-surface-muted border border-erp-border">
          <div className="flex items-center justify-between gap-2 mb-2">
            <span className="font-mono text-xs font-bold text-erp-navy">
              {customer.code || "-"}
            </span>
            <StatusBadge
              label={statusLabel}
              variant={customer.status === "active" ? "success" : "warning"}
            />
          </div>
          <h2 className="text-base font-bold text-erp-text-main leading-snug">
            {displayName}
          </h2>
          {customer.displayNameEn && customer.displayNameTh && (
            <p className="text-xs text-erp-text-muted mt-0.5">
              {locale === "en" ? customer.displayNameTh : customer.displayNameEn}
            </p>
          )}
        </div>

        {/* Classification */}
        <div className="space-y-2">
          <h3 className="text-xs font-bold uppercase tracking-wider text-erp-navy border-b border-erp-border pb-1">
            {t("generalInfo")}
          </h3>
          <div className="grid grid-cols-2 gap-3 text-xs">
            <div>
              <span className="text-erp-text-muted block">{t("customerType")}</span>
              <span className="font-medium text-erp-text-main mt-0.5 inline-block">
                {typeLabel}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block">{t("preferredLocale")}</span>
              <span className="font-medium text-erp-text-main mt-0.5 inline-block font-mono">
                {customer.preferredLocale ? customer.preferredLocale.toUpperCase() : "-"}
              </span>
            </div>
            {customer.leadSource && (
              <div className="col-span-2">
                <span className="text-erp-text-muted block">{t("leadSource")}</span>
                <span className="font-medium text-erp-text-main mt-0.5 inline-block">
                  {customer.leadSource}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Primary Contact Info */}
        <div className="space-y-2">
          <h3 className="text-xs font-bold uppercase tracking-wider text-erp-navy border-b border-erp-border pb-1">
            {t("primaryContact")}
          </h3>
          {contact ? (
            <div className="space-y-2 text-xs bg-erp-surface p-3 border border-erp-border">
              <div className="font-semibold text-erp-text-main">
                {contact.name || "-"}
                {contact.roleTitle && (
                  <span className="text-erp-text-muted font-normal ml-1">
                    ({contact.roleTitle})
                  </span>
                )}
              </div>
              <div className="flex flex-col gap-1 text-erp-text-muted">
                {contact.phone && (
                  <div>
                    <span className="font-medium text-erp-navy">{t("phone")}: </span>
                    <span className="font-mono">{contact.phone}</span>
                  </div>
                )}
                {contact.email && (
                  <div>
                    <span className="font-medium text-erp-navy">{t("email")}: </span>
                    <span className="font-mono">{contact.email}</span>
                  </div>
                )}
                {contact.preferredChannel && (
                  <div>
                    <span className="font-medium text-erp-navy">{t("preferredChannel")}: </span>
                    <span>{contact.preferredChannel}</span>
                  </div>
                )}
              </div>
            </div>
          ) : (
            <p className="text-xs text-erp-text-muted italic">{t("noPrimaryContact")}</p>
          )}
        </div>
      </div>
      )}
    </Drawer>
  );
}
