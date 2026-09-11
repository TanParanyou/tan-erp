"use client";

import React from "react";
import { useTranslations, useLocale } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { IconAlertCircle } from "@/components/common/Icons";
import { useCustomerDetail } from "../api/customer-queries";
import { isAuthenticationRequiredError, isMembershipRequiredError } from "@/lib/api/api-error";
import {
  getCustomerStatusLabelKey,
  getCustomerTypeLabelKey,
} from "../customer-labels";
import { formatDateTime } from "@/lib/formatters/formatters";
import { CustomerDetailContent } from "./customer-detail";

export interface CustomerQuickViewDrawerProps {
  customerId: string | null;
  isOpen: boolean;
  onClose: () => void;
  onSelectExisting?: (customerId: string) => void;
}

export function CustomerQuickViewDrawer({
  customerId,
  isOpen,
  onClose,
  onSelectExisting,
}: CustomerQuickViewDrawerProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const {
    data: customer,
    isLoading: isCustomerLoading,
    isError: isCustomerError,
    error: customerError,
    refetch: refetchCustomer,
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

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={t("quickViewTitle")}
      size="lg"
      footer={
        <div className="flex flex-col-reverse sm:flex-row justify-between items-center w-full gap-3">
          <Button variant="outline" size="sm" onClick={onClose} className="w-full sm:w-auto">
            {tCommon("actions.close")}
          </Button>
          <div className="flex items-center gap-2 w-full sm:w-auto justify-end">
            {customer && customer.id && onSelectExisting && (
              <Button
                type="button"
                variant="secondary"
                size="sm"
                onClick={() => {
                  const targetId = customer.id;
                  if (!targetId) return;
                  onClose();
                  onSelectExisting(targetId);
                }}
                className="w-full sm:w-auto"
              >
                {t("useExistingCustomer")}
              </Button>
            )}
            {customer && (
              <Button
                href={`/${locale}/customers/${customer.id}`}
                variant="primary"
                size="sm"
                className="w-full sm:w-auto font-semibold"
              >
                {t("viewFullDetail")}
              </Button>
            )}
          </div>
        </div>
      }
    >
      {isCustomerLoading ? (
        <div className="flex items-center justify-center min-h-[300px]">
          <MonoSpinner size="md" label={tCommon("states.loading")} aria-busy="true" />
        </div>
      ) : isCustomerError || !customer ? (
        <div
          role="alert"
          aria-live="polite"
          className="erp-card p-6 border-erp-danger-border bg-erp-danger-bg text-center flex flex-col items-center gap-3"
        >
          <IconAlertCircle size={28} className="text-erp-danger" />
          <p className="text-sm font-bold text-erp-danger">
            {resolveDetailErrorMessage(customerError)}
          </p>
          <Button
            variant="outline"
            size="sm"
            onClick={() => refetchCustomer()}
          >
            {tCommon("actions.retry")}
          </Button>
        </div>
      ) : (
        <div className="space-y-6">
          {/* Header Card Summary */}
          <div className="p-4 bg-erp-surface-muted border border-erp-border">
            <div className="flex items-center justify-between gap-2 mb-2">
              <div className="flex items-center gap-2">
                <span className="font-mono text-xs font-bold text-erp-navy bg-erp-surface px-2 py-0.5 border border-erp-border">
                  {customer.code || "-"}
                </span>
                <span className="text-xs text-erp-text-muted">
                  {typeLabel}
                </span>
              </div>
              <StatusBadge
                label={statusLabel}
                variant={customer.status === "active" ? "success" : "warning"}
              />
            </div>
            <h2 className="text-lg font-bold text-erp-text-main leading-snug">
              {displayName}
            </h2>
            {customer.displayNameEn && customer.displayNameTh && (
              <p className="text-xs text-erp-text-muted mt-0.5">
                {locale === "en" ? customer.displayNameTh : customer.displayNameEn}
              </p>
            )}
            {customer.createdAtUtc && (
              <div className="mt-2 pt-2 border-t border-erp-border-subtle text-[11px] text-erp-text-muted">
                <span>{t("createdAt")}: </span>
                <span className="font-mono">
                  {formatDateTime(customer.createdAtUtc, locale === "en" ? "en" : "th")}
                </span>
              </div>
            )}
          </div>

          {/* Single Source of Truth: Customer Detail Content (Info Card, Primary Contact Card, SiteList) */}
          <CustomerDetailContent customer={customer} />
        </div>
      )}
    </Drawer>
  );
}
