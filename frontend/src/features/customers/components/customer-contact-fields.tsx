"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PhoneInput } from "@/components/forms/PhoneInput";

export type CustomerContactChannel = "phone" | "email" | "line" | "other";

export interface CustomerContactFieldValues {
  name: string;
  roleTitle: string;
  phone: string;
  email: string;
  lineId: string;
  preferredChannel: CustomerContactChannel;
}

export type CustomerContactFieldErrors = Partial<Record<keyof CustomerContactFieldValues, string | undefined>>;

export interface CustomerContactFieldsProps {
  values: CustomerContactFieldValues;
  errors: CustomerContactFieldErrors;
  disabled?: boolean;
  showPlaceholders?: boolean;
  idPrefix?: string;
  onNameChange: (value: string) => void;
  onRoleTitleChange: (value: string) => void;
  onPhoneChange: (value: string) => void;
  onEmailChange: (value: string) => void;
  onLineIdChange: (value: string) => void;
  onPreferredChannelChange: (value: CustomerContactChannel) => void;
}

export function CustomerContactFields({
  values,
  errors,
  disabled = false,
  showPlaceholders = false,
  idPrefix,
  onNameChange,
  onRoleTitleChange,
  onPhoneChange,
  onEmailChange,
  onLineIdChange,
  onPreferredChannelChange,
}: CustomerContactFieldsProps): React.JSX.Element {
  const t = useTranslations("customers");
  const fieldId = (name: string) => idPrefix ? `${idPrefix}${name}` : undefined;

  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
      <Input id={fieldId("Name")} label={t("contactName")} placeholder={showPlaceholders ? t("contactNamePlaceholder") : undefined} required error={errors.name} disabled={disabled} value={values.name} onChange={(event) => onNameChange(event.target.value)} />
      <Input id={fieldId("RoleTitle")} label={t("roleTitle")} placeholder={showPlaceholders ? t("roleTitlePlaceholder") : undefined} error={errors.roleTitle} disabled={disabled} value={values.roleTitle} onChange={(event) => onRoleTitleChange(event.target.value)} />
      <PhoneInput id={fieldId("Phone")} label={t("phone")} placeholder={showPlaceholders ? t("phonePlaceholder") : undefined} error={errors.phone} disabled={disabled} value={values.phone} onValueChange={onPhoneChange} />
      <Input id={fieldId("Email")} label={t("email")} type="email" placeholder={showPlaceholders ? t("emailPlaceholder") : undefined} error={errors.email} disabled={disabled} value={values.email} onChange={(event) => onEmailChange(event.target.value)} />
      <Input id={fieldId("LineId")} label={t("lineId")} placeholder={showPlaceholders ? t("lineIdPlaceholder") : undefined} error={errors.lineId} disabled={disabled} value={values.lineId} onChange={(event) => onLineIdChange(event.target.value)} />
      <Select
        id={fieldId("PreferredChannel")}
        label={t("preferredChannel")}
        error={errors.preferredChannel}
        disabled={disabled}
        value={values.preferredChannel}
        onChange={(event) => {
          const value = event.target.value;
          onPreferredChannelChange(value === "email" || value === "line" || value === "other" ? value : "phone");
        }}
        options={[{ value: "phone", label: t("channelPhone") }, { value: "email", label: t("channelEmail") }, { value: "line", label: t("channelLine") }, { value: "other", label: t("channelOther") }]}
      />
    </div>
  );
}
