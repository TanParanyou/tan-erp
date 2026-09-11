"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { AddressAutocomplete, type SelectedAddress } from "./AddressAutocomplete";
import { IconMapPin } from "@/components/common/Icons";

export interface AddressAreaValue {
  subdistrict?: string | null;
  district?: string | null;
  province?: string | null;
  postalCode?: string | null;
  countryCode?: string | null;
}

export interface AddressAreaFieldProps {
  id?: string;
  label?: string;
  hint?: string;
  required?: boolean;
  disabled?: boolean;
  error?: string;
  value?: AddressAreaValue;
  onSelect: (address: SelectedAddress) => void;
  onClear: () => void;
  className?: string;
}

export function AddressAreaField({
  id = "address-area-field",
  label,
  hint,
  required = false,
  disabled = false,
  error,
  value,
  onSelect,
  onClear,
  className = "",
}: AddressAreaFieldProps) {
  const t = useTranslations("common.addressAutocomplete");

  const subdistrict = value?.subdistrict?.trim();
  const district = value?.district?.trim();
  const province = value?.province?.trim();
  const postalCode = value?.postalCode?.trim();
  const countryCode = value?.countryCode?.trim() || "TH";

  const hasSelectedAddress = Boolean(subdistrict && district && province && postalCode);

  const displayLabel = label || t("areaSelection");

  if (hasSelectedAddress) {
    return (
      <div className={`erp-form-group ${className}`}>
        <label className="erp-label">
          {displayLabel}
          {required && <span className="erp-label-required">*</span>}
        </label>
        <div className="min-h-[44px] px-3 py-2 bg-white border border-erp-border flex flex-col sm:flex-row sm:items-center justify-between gap-3 rounded-none box-border">
          <div className="flex items-center gap-2.5">
            <IconMapPin size={18} className="text-erp-navy shrink-0" />
            <div className="flex flex-wrap items-baseline gap-x-2 gap-y-0.5">
              <span className="text-sm font-semibold text-erp-text-main">
                {subdistrict} » {district} » {province}
              </span>
              <span className="text-xs font-mono text-erp-text-muted">
                {postalCode} • {countryCode}
              </span>
            </div>
          </div>
          <button
            type="button"
            onClick={onClear}
            disabled={disabled}
            className="inline-flex items-center justify-center px-2.5 py-1 text-xs font-semibold text-erp-navy bg-erp-bg-subtle hover:bg-erp-border-subtle border border-erp-border transition-colors rounded-none disabled:opacity-50 cursor-pointer self-start sm:self-auto"
          >
            {t("changeArea")}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={`erp-form-group ${className}`}>
      <AddressAutocomplete
        id={id}
        label={displayLabel}
        hint={hint}
        disabled={disabled}
        onSelect={onSelect}
      />
      {error && (
        <span className="text-xs text-erp-error font-medium mt-1 block" role="alert">
          {error}
        </span>
      )}
    </div>
  );
}
