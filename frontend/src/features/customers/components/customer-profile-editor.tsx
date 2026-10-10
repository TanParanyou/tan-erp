"use client";

import React, { useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormTabs, useFormTabErrors } from "@/components/forms/FormTabs";
import { TaxIdInput } from "@/components/forms/TaxIdInput";
import { PageHeader } from "@/components/layout/PageHeader";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { customerQueryRootKey, useCustomerDetail } from "../api/customer-queries";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useQueryClient } from "@tanstack/react-query";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { can } from "@/lib/permissions/can";
import { createTaxIdSchema } from "@/lib/validation/tax-id";
import { CustomerIdentityFields, type CustomerLeadSource, type CustomerType, type CustomerLocale } from "./customer-identity-fields";

interface CustomerProfileEditorProps {
  customerId: string;
  onCancel: () => void;
  onSaved: () => void;
}

const profileSchema = z.object({
  customerType: z.enum(["person", "organization"]),
  displayNameTh: z.string().trim().min(1).max(250),
  displayNameEn: z.string().trim().max(250),
  preferredLocale: z.enum(["th", "en"]),
  leadSource: z.enum(["", "walk_in", "facebook_ads", "referral", "project_developer", "website", "other"]),
  leadSourceNote: z.string().trim().max(200),
  legalName: z.string().trim().max(250),
  taxIdentifier: createTaxIdSchema((key) => key === "invalidTaxId" ? "common.validation.invalidTaxId" : "common.validation.required"),
  branchCode: z.string().trim().max(5),
  creditTermDays: z.string().regex(/^\d+$/).transform(Number).pipe(z.number().int().min(0).max(365)),
  creditLimit: z.string().trim().refine((value) => value === "" || (Number.isFinite(Number(value)) && Number(value) >= 0), "Invalid credit limit"),
  currencyCode: z.string().trim().length(3),
  billingCycle: z.string().trim().max(50),
  billingDay: z.string().trim().refine((value) => value === "" || (/^\d{1,2}$/.test(value) && Number(value) >= 1 && Number(value) <= 31), "Invalid billing day"),
  paymentConditionNote: z.string().trim().max(500),
});

type CustomerProfileValues = z.infer<typeof profileSchema>;
type CustomerProfileInput = z.input<typeof profileSchema>;

function normalizeLeadSource(value: string | null | undefined): CustomerProfileInput["leadSource"] {
  return value === "walk_in" || value === "facebook_ads" || value === "referral" || value === "project_developer" || value === "website" || value === "other" ? value : "";
}

export function CustomerProfileEditor({ customerId, onCancel, onSaved }: CustomerProfileEditorProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const queryClient = useQueryClient();
  const { selectedMembership } = useSelectedMembership();
  const canReadCredit = can(selectedMembership, "customers.credit.read");
  const canManageCredit = can(selectedMembership, "customers.credit.manage");
  const canManagePii = can(selectedMembership, "customer-contacts.manage");
  const { toast } = useToast();
  const { data: customer, isLoading } = useCustomerDetail(customerId);
  const intentRef = useRef<{ signature: string; key: string } | null>(null);

  if (isLoading || !customer) {
    return <div className="flex min-h-48 items-center justify-center border border-erp-border bg-erp-surface"><MonoSpinner label={tCommon("states.loading")} /></div>;
  }

  return <CustomerProfileForm
    key={`${customer.id}:${customer.rowVersion}`}
    customer={customer}
    canReadCredit={canReadCredit}
    canManageCredit={canManageCredit}
    canManagePii={canManagePii}
    onCancel={onCancel}
    onSubmit={async (values) => {
      const token = await getAuthToken();
      const membershipId = selectedMembership?.id;
      if (!token || !membershipId) {
        const message = !token ? t("errors.authenticationRequired") : t("errors.membershipRequired");
        toast.error(message);
        return;
      }
      if (!customer.id || !customer.rowVersion) {
        toast.error(t("errors.loadDetail"));
        return;
      }
      const signature = `${customer.id}|${customer.rowVersion}|${JSON.stringify(values)}`;
      if (intentRef.current?.signature !== signature) intentRef.current = { signature, key: crypto.randomUUID() };
      const idempotencyKey = intentRef.current.key;
      try {
        await apiClient.updateCustomer(customer.id, {
          customerType: values.customerType,
          displayNameTh: values.displayNameTh,
          displayNameEn: values.displayNameEn || undefined,
          preferredLocale: values.preferredLocale,
          leadSource: values.leadSource || undefined,
          leadSourceNote: values.leadSource === "other" ? values.leadSourceNote || undefined : undefined,
          legalName: values.legalName || undefined,
          branchCode: values.branchCode || undefined,
          ...(canManagePii ? { taxIdentifier: values.taxIdentifier || null, hasTaxIdentifier: true } : {}),
          ...(canManageCredit ? {
            creditTermDays: values.creditTermDays,
            creditLimit: values.creditLimit === "" ? null : Number(values.creditLimit),
            currencyCode: values.currencyCode,
            billingCycle: values.billingCycle || null,
            billingDay: values.billingDay === "" ? null : Number(values.billingDay),
            paymentConditionNote: values.paymentConditionNote || null,
          } : {}),
        }, { token, membershipId, idempotencyKey, ifMatch: customer.rowVersion, locale: locale === "en" ? "en" : "th" });
        await queryClient.invalidateQueries({ queryKey: customerQueryRootKey(membershipId, locale === "en" ? "en" : "th") });
        toast.success(tCommon("feedback.saveSuccess"));
        onSaved();
      } catch (error: unknown) {
        const message = error instanceof ApiError && error.code === "CUSTOMER_VERSION_CONFLICT"
          ? t("errors.activateConflict")
          : error instanceof ApiError ? error.message : t("errors.saveUnexpected");
        toast.error(message);
      }
    }}
  />;
}

function CustomerProfileForm({
  customer,
  canReadCredit,
  canManageCredit,
  canManagePii,
  onCancel,
  onSubmit,
}: Pick<CustomerProfileEditorProps, "onCancel"> & {
  customer: NonNullable<ReturnType<typeof useCustomerDetail>["data"]>;
  canReadCredit: boolean;
  canManageCredit: boolean;
  canManagePii: boolean;
  onSubmit: (values: CustomerProfileValues) => Promise<void>;
}) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const [activeTab, setActiveTab] = useState<"profile" | "commercial">("profile");
  const form = useForm<CustomerProfileInput, unknown, CustomerProfileValues>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      customerType: customer.customerType === "person" ? "person" : "organization",
      displayNameTh: customer.displayNameTh ?? "",
      displayNameEn: customer.displayNameEn ?? "",
      preferredLocale: customer.preferredLocale === "en" ? "en" : "th",
      leadSource: normalizeLeadSource(customer.leadSource),
      leadSourceNote: customer.leadSourceNote ?? "",
      legalName: customer.legalName ?? "",
      taxIdentifier: customer.taxIdentifier ?? "",
      branchCode: customer.branchCode ?? "",
      creditTermDays: String(customer.creditTermDays ?? 0),
      creditLimit: customer.creditLimit === null || customer.creditLimit === undefined ? "" : String(customer.creditLimit),
      currencyCode: customer.currencyCode ?? "THB",
      billingCycle: customer.billingCycle ?? "",
      billingDay: customer.billingDay === null || customer.billingDay === undefined ? "" : String(customer.billingDay),
      paymentConditionNote: customer.paymentConditionNote ?? "",
    },
  });
  const tabFieldsMap = { profile: ["customerType", "displayNameTh", "displayNameEn", "preferredLocale", "leadSource", "leadSourceNote"], commercial: ["legalName", "taxIdentifier", "branchCode", ...(canReadCredit ? ["creditTermDays", "creditLimit", "currencyCode", "billingCycle", "billingDay", "paymentConditionNote"] : [])] };
  const { tabErrorMap, handleFormError } = useFormTabErrors<"profile" | "commercial", CustomerProfileInput>({ tabFieldsMap, errors: form.formState.errors, setActiveTab });
  const save = form.handleSubmit(onSubmit, handleFormError);
  const identityValues = {
    customerType: form.watch("customerType"),
    preferredLocale: form.watch("preferredLocale"),
    displayNameTh: form.watch("displayNameTh"),
    displayNameEn: form.watch("displayNameEn"),
    leadSource: form.watch("leadSource"),
    leadSourceNote: form.watch("leadSourceNote"),
  };

  return <FormContainer>
    <form onSubmit={save} className="flex flex-col gap-6 pb-24">
      <PageHeader title={t("editCustomer")} subtitle={customer.code ?? ""} />
      <FormTabs<"profile" | "commercial"> activeTab={activeTab} onChange={setActiveTab} ariaLabel={t("editCustomer")} tabs={[{ id: "profile", label: t("profileTab"), hasError: tabErrorMap.profile }, { id: "commercial", label: t("commercialTab"), hasError: tabErrorMap.commercial }]} />
      <div id="tabpanel-profile" role="tabpanel" aria-labelledby="tab-profile" hidden={activeTab !== "profile"}>
      <FormSection title={t("generalInfo")}>
        <CustomerIdentityFields
          values={identityValues}
          errors={{
            displayNameTh: form.formState.errors.displayNameTh?.message,
            displayNameEn: form.formState.errors.displayNameEn?.message,
            leadSource: form.formState.errors.leadSource?.message,
            leadSourceNote: form.formState.errors.leadSourceNote?.message,
          }}
          customerTypeDisabled={customer.status !== "draft"}
          onCustomerTypeChange={(value: CustomerType) => form.setValue("customerType", value, { shouldDirty: true, shouldValidate: true })}
          onPreferredLocaleChange={(value: CustomerLocale) => form.setValue("preferredLocale", value, { shouldDirty: true, shouldValidate: true })}
          onDisplayNameThChange={(value) => form.setValue("displayNameTh", value, { shouldDirty: true, shouldValidate: true })}
          onDisplayNameEnChange={(value) => form.setValue("displayNameEn", value, { shouldDirty: true, shouldValidate: true })}
          onLeadSourceChange={(value: CustomerLeadSource) => form.setValue("leadSource", value, { shouldDirty: true, shouldValidate: true })}
          onLeadSourceNoteChange={(value) => form.setValue("leadSourceNote", value, { shouldDirty: true, shouldValidate: true })}
        />
      </FormSection>
      </div>
      <div id="tabpanel-commercial" role="tabpanel" aria-labelledby="tab-commercial" hidden={activeTab !== "commercial"}>
      <FormSection title={t("commercialTab")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <Input label={t("legalName")} placeholder={t("legalNamePlaceholder")} {...form.register("legalName")} />
          {canManagePii && (
            <TaxIdInput
              id="customer-tax-identifier"
              label={t("taxIdentifier")}
              placeholder={t("taxIdentifierPlaceholder")}
              helperText={t("taxIdentifierHint")}
              error={form.formState.errors.taxIdentifier?.message ? tCommon("validation.invalidTaxId") : undefined}
              value={form.watch("taxIdentifier")}
              onValueChange={(val) => form.setValue("taxIdentifier", val, { shouldDirty: true, shouldValidate: true })}
            />
          )}
          <Input label={t("branchCode")} maxLength={5} placeholder={t("branchCodePlaceholder")} {...form.register("branchCode")} />
          {canReadCredit && <>
            <Input label={t("creditTermDays")} type="number" min={0} placeholder={t("creditTermDaysPlaceholder")} disabled={!canManageCredit} {...form.register("creditTermDays")} />
            <Input label={t("creditLimit")} type="number" min={0} step="0.01" placeholder={t("creditLimitPlaceholder")} disabled={!canManageCredit} {...form.register("creditLimit")} />
            <Input label={t("currencyCode")} maxLength={3} placeholder={t("currencyCodePlaceholder")} disabled={!canManageCredit} {...form.register("currencyCode")} />
            <Input label={t("billingCycle")} placeholder={t("billingCyclePlaceholder")} disabled={!canManageCredit} {...form.register("billingCycle")} />
            <Input label={t("billingDay")} type="number" min={1} max={31} placeholder={t("billingDayPlaceholder")} disabled={!canManageCredit} {...form.register("billingDay")} />
            <Input label={t("paymentConditionNote")} placeholder={t("paymentConditionNotePlaceholder")} disabled={!canManageCredit} {...form.register("paymentConditionNote")} />
          </>}
        </div>
      </FormSection>
      </div>
      <FormActionBar isEditMode isDirty={form.formState.isDirty} isLoading={form.formState.isSubmitting} saveText={tCommon("actions.saveChanges")} onCancel={onCancel} />
    </form>
  </FormContainer>;
}
