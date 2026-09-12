"use client";

import React, { useState, useMemo } from "react";
import { useTranslations } from "next-intl";
import { useCustomerList } from "@/features/customers/api/customer-queries";
import { Button } from "@/components/ui/Button";
import { Avatar } from "@/components/ui/Avatar";
import { cn } from "@/lib/utils/cn";
import { EntityAutocomplete } from "./EntityAutocomplete";
import type { CustomerListItemResponse } from "@/lib/api/api-client";

export interface CustomerAutocompleteProps {
  value?: string | null;
  onChange: (customerId: string) => void;
  onViewDrawer?: (customerId: string) => void;
  label?: string;
  error?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
}

/**
 * CustomerAutocomplete - Atelier Architectural Navy Sharp
 * Consumes global EntityAutocomplete with customer-specific layout and cards.
 */
export function CustomerAutocomplete({
  value,
  onChange,
  onViewDrawer,
  label,
  error,
  placeholder,
  required,
  disabled = false,
  className,
}: CustomerAutocompleteProps) {
  const t = useTranslations("opportunities");
  const [searchQuery, setSearchQuery] = useState("");

  // Fetch active customers
  const { data: customerData, isLoading } = useCustomerList({
    search: searchQuery.trim() || undefined,
    status: "active",
    limit: 20,
  });

  const customerList: CustomerListItemResponse[] = useMemo(
    () => customerData?.items ?? [],
    [customerData?.items]
  );

  // Find currently selected customer details if value is present
  const selectedCustomer = useMemo(() => {
    if (!value) return null;
    return customerList.find((c) => c.id === value) ?? null;
  }, [value, customerList]);

  const resolvedLabel = label || t("customer");
  const resolvedPlaceholder = placeholder || t("customerSearchPlaceholder");

  return (
    <EntityAutocomplete<CustomerListItemResponse>
      value={value}
      onChange={onChange}
      label={resolvedLabel}
      placeholder={resolvedPlaceholder}
      error={error}
      required={required}
      disabled={disabled}
      className={className}
      items={customerList}
      isLoading={isLoading}
      emptyText={t("noCustomerFound")}
      loadingText={t("loadingMore")}
      onSearchChange={setSearchQuery}
      getItemKey={(c) => c.id ?? ""}
      renderSelectedCard={(onClear) => (
        <div
          role="region"
          aria-label={resolvedLabel}
          className="flex flex-col gap-3 border border-erp-navy/40 bg-erp-surface p-3.5 shadow-sm"
        >
          <div className="flex flex-col sm:flex-row sm:items-start justify-between gap-3">
            <div
              onClick={() => {
                if (onViewDrawer && value) {
                  onViewDrawer(value);
                }
              }}
              className={cn(
                "flex items-start gap-3 min-w-0 flex-1",
                onViewDrawer && "cursor-pointer group hover:opacity-90 transition-opacity"
              )}
              title={onViewDrawer ? t("viewCustomerDrawer") : undefined}
              aria-label={onViewDrawer ? t("viewCustomerDrawer") : undefined}
              role={onViewDrawer ? "button" : undefined}
              tabIndex={onViewDrawer ? 0 : undefined}
              onKeyDown={(e) => {
                if (onViewDrawer && value && (e.key === "Enter" || e.key === " ")) {
                  e.preventDefault();
                  onViewDrawer(value);
                }
              }}
            >
              <Avatar
                initial={selectedCustomer?.displayNameTh || selectedCustomer?.displayNameEn || undefined}
                variant={selectedCustomer?.customerType === "organization" ? "navy" : "muted"}
                size="md"
                className="shrink-0 mt-0.5"
              />

              <div className="flex flex-col min-w-0 flex-1 gap-1">
                <div className="flex flex-wrap items-center gap-1.5">
                  {selectedCustomer?.code && (
                    <span className="font-mono text-xs font-bold text-erp-navy bg-erp-surface-subtle px-1.5 py-0.5 border border-erp-border">
                      [{selectedCustomer.code}]
                    </span>
                  )}
                  <span
                    className={cn(
                      "font-bold text-sm text-erp-navy break-words",
                      onViewDrawer && "group-hover:underline"
                    )}
                  >
                    {selectedCustomer
                      ? selectedCustomer.displayNameTh || selectedCustomer.displayNameEn || "-"
                      : `ID: ${value}`}
                  </span>
                </div>

                {selectedCustomer?.displayNameEn && selectedCustomer?.displayNameTh && (
                  <p className="text-xs text-erp-text-muted break-words m-0">
                    {selectedCustomer.displayNameEn}
                  </p>
                )}

                {/* Primary Contact Snapshot if available */}
                {selectedCustomer?.primaryContact && (
                  <div className="flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-erp-text-muted mt-1 pt-1.5 border-t border-erp-border-subtle">
                    {selectedCustomer.primaryContact.name && (
                      <span>
                        <strong className="text-erp-text-main font-semibold">{t("contactPerson")}</strong>{" "}
                        {selectedCustomer.primaryContact.name}
                      </span>
                    )}
                    {selectedCustomer.primaryContact.phone && (
                      <span className="font-mono">
                        <strong className="text-erp-text-main font-semibold">{t("phone")}</strong>{" "}
                        {selectedCustomer.primaryContact.phone}
                      </span>
                    )}
                    {selectedCustomer.primaryContact.email && (
                      <span>
                        <strong className="text-erp-text-main font-semibold">{t("email")}</strong>{" "}
                        {selectedCustomer.primaryContact.email}
                      </span>
                    )}
                  </div>
                )}
              </div>
            </div>

            {/* Action Buttons: Change Customer */}
            <div className="flex items-center gap-2 shrink-0 self-end sm:self-start">
              {!disabled && (
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={onClear}
                  className="shrink-0 text-xs font-semibold"
                >
                  {t("changeCustomer")}
                </Button>
              )}
            </div>
          </div>
        </div>
      )}
      renderListItem={(c, isHighlighted, isMobile) => {
        const displayName = c.displayNameTh || c.displayNameEn || "-";
        return (
          <>
            <div className="flex flex-col min-w-0">
              <span
                className={cn(
                  "font-semibold text-sm truncate",
                  !isMobile && isHighlighted ? "text-white" : "text-erp-text-main"
                )}
              >
                {displayName}
              </span>
              {c.code && (
                <span
                  className={cn(
                    "text-xs font-mono tracking-wider",
                    !isMobile && isHighlighted ? "text-white/80" : "text-erp-text-muted"
                  )}
                >
                  {c.code}
                </span>
              )}
            </div>
            {c.status && (
              <span
                className={cn(
                  "text-xs uppercase px-1.5 py-0.5 shrink-0 font-medium",
                  !isMobile && isHighlighted
                    ? "bg-white/20 text-white"
                    : "bg-erp-surface-subtle text-erp-text-muted"
                )}
              >
                {c.status}
              </span>
            )}
          </>
        );
      }}
    />
  );
}

export default CustomerAutocomplete;
