"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import type { CustomerLeadSourceKey } from "../customer-labels";

export type CustomerLeadSource = "" | "walk_in" | "facebook_ads" | "referral" | "project_developer" | "website" | "other";
export type CustomerType = "organization" | "person";
export type CustomerLocale = "th" | "en";

export interface CustomerIdentityValues {
  customerType: CustomerType;
  preferredLocale: CustomerLocale;
  displayNameTh: string;
  displayNameEn: string;
  leadSource: CustomerLeadSource;
  leadSourceNote: string;
}

export interface CustomerIdentityErrors {
  displayNameTh?: string;
  displayNameEn?: string;
  leadSource?: string;
  leadSourceNote?: string;
}

export interface CustomerIdentityFieldsProps {
  values: CustomerIdentityValues;
  errors: CustomerIdentityErrors;
  disabled?: boolean;
  customerTypeDisabled?: boolean;
  showPlaceholders?: boolean;
  leadSourceNoteRequired?: boolean;
  onCustomerTypeChange: (value: CustomerType) => void;
  onPreferredLocaleChange: (value: CustomerLocale) => void;
  onDisplayNameThChange: (value: string) => void;
  onDisplayNameEnChange: (value: string) => void;
  onLeadSourceChange: (value: CustomerLeadSource) => void;
  onLeadSourceNoteChange: (value: string) => void;
}

const leadSourceValues: Exclude<CustomerLeadSource, "">[] = [
  "walk_in",
  "facebook_ads",
  "referral",
  "project_developer",
  "website",
  "other",
];

const leadSourceLabelKeys: Record<Exclude<CustomerLeadSource, "">, CustomerLeadSourceKey> = {
  walk_in: "leadSourceWalkIn",
  facebook_ads: "leadSourceFacebookAds",
  referral: "leadSourceReferral",
  project_developer: "leadSourceProjectDeveloper",
  website: "leadSourceWebsite",
  other: "leadSourceOther",
};

export function CustomerIdentityFields({
  values,
  errors,
  disabled = false,
  customerTypeDisabled = false,
  showPlaceholders = false,
  leadSourceNoteRequired = false,
  onCustomerTypeChange,
  onPreferredLocaleChange,
  onDisplayNameThChange,
  onDisplayNameEnChange,
  onLeadSourceChange,
  onLeadSourceNoteChange,
}: CustomerIdentityFieldsProps): React.JSX.Element {
  const t = useTranslations("customers");
  const leadSourceOptions = leadSourceValues.map((value) => ({ value, label: t(leadSourceLabelKeys[value]) }));

  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
      <Select
        id="customerType"
        label={t("customerType")}
        required
        disabled={disabled || customerTypeDisabled}
        value={values.customerType}
        onChange={(event) => onCustomerTypeChange(event.target.value === "person" ? "person" : "organization")}
        options={[{ value: "organization", label: t("organization") }, { value: "person", label: t("person") }]}
      />
      <Select
        id="preferredLocale"
        label={t("preferredLocale")}
        required
        disabled={disabled}
        value={values.preferredLocale}
        onChange={(event) => onPreferredLocaleChange(event.target.value === "en" ? "en" : "th")}
        options={[{ value: "th", label: t("localeThai") }, { value: "en", label: t("localeEnglish") }]}
      />
      <Input
        id="displayNameTh"
        label={t("displayNameTh")}
        placeholder={showPlaceholders ? t("displayNameThPlaceholder") : undefined}
        required
        disabled={disabled}
        error={errors.displayNameTh}
        value={values.displayNameTh}
        onChange={(event) => onDisplayNameThChange(event.target.value)}
      />
      <Input
        id="displayNameEn"
        label={t("displayNameEn")}
        placeholder={showPlaceholders ? t("displayNameEnPlaceholder") : undefined}
        disabled={disabled}
        error={errors.displayNameEn}
        value={values.displayNameEn}
        onChange={(event) => onDisplayNameEnChange(event.target.value)}
      />
      <Select
        id="leadSource"
        label={t("leadSource")}
        placeholder={showPlaceholders ? t("leadSourceSelect") : undefined}
        disabled={disabled}
        error={errors.leadSource}
        value={values.leadSource}
        onChange={(event) => {
          const value = event.target.value;
          onLeadSourceChange(leadSourceValues.find((item) => item === value) ?? "");
        }}
        options={leadSourceOptions}
      />
      {values.leadSource === "other" && (
        <Input
          id="leadSourceNote"
          label={t("leadSourceNote")}
          placeholder={showPlaceholders ? t("leadSourceNotePlaceholder") : undefined}
          required={leadSourceNoteRequired}
          maxLength={200}
          disabled={disabled}
          error={errors.leadSourceNote}
          value={values.leadSourceNote}
          onChange={(event) => onLeadSourceNoteChange(event.target.value)}
        />
      )}
    </div>
  );
}
