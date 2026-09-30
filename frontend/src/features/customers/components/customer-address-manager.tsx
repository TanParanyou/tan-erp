"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { apiClient, type CustomerAddressResponse } from "@/lib/api/api-client";
import { useCustomerAddresses } from "../api/customer-queries";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { AddressAreaField } from "@/components/forms/AddressAreaField";
import type { SelectedAddress } from "@/components/forms/AddressAutocomplete";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useCustomerMutationRunner } from "../hooks/use-customer-mutation-runner";

const addressSchema = z.object({
  addressType: z.enum(["billing", "contact"]),
  label: z.string().trim().min(1).max(100),
  addressLine1: z.string().trim().min(1).max(250),
  subdistrict: z.string().trim().min(1).max(100),
  district: z.string().trim().min(1).max(100),
  province: z.string().trim().min(1).max(100),
  postalCode: z.string().regex(/^\d{5}$/),
  countryCode: z.string().regex(/^[A-Za-z]{2}$/),
});
type AddressValues = z.infer<typeof addressSchema>;

interface CustomerAddressManagerProps {
  customerId: string;
  canManage: boolean;
  readOnly?: boolean;
}

export function CustomerAddressManager({ customerId, canManage, readOnly = false }: CustomerAddressManagerProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const runCustomerMutation = useCustomerMutationRunner();
  const [editing, setEditing] = React.useState<CustomerAddressResponse | null>(null);
  const [deactivating, setDeactivating] = React.useState<CustomerAddressResponse | null>(null);
  const [isSaving, setIsSaving] = React.useState(false);
  const [isDeactivating, setIsDeactivating] = React.useState(false);
  const addressesQuery = useCustomerAddresses(customerId, canManage);
  const form = useForm<AddressValues>({
    resolver: zodResolver(addressSchema),
    defaultValues: { addressType: "billing", label: "", addressLine1: "", subdistrict: "", district: "", province: "", postalCode: "", countryCode: "TH" },
  });

  const submit = form.handleSubmit(async (values) => {
    setIsSaving(true);
    try {
      const result = await runCustomerMutation(async ({ token, membershipId, locale }) => editing
        ? apiClient.updateCustomerAddress(customerId, editing.id!, values, { token, membershipId, ifMatch: editing.rowVersion!, locale })
        : apiClient.createCustomerAddress(customerId, { ...values, isPrimary: values.addressType === "billing" && !addressesQuery.data?.items?.some((item) => item.addressType === "billing" && item.isPrimary) }, { token, membershipId, idempotencyKey: crypto.randomUUID(), locale }));
      if (!result.succeeded) return;
      form.reset();
      setEditing(null);
    } finally {
      setIsSaving(false);
    }
  });

  const runAddressAction = async (address: CustomerAddressResponse, action: "primary" | "deactivate") => {
    const result = await runCustomerMutation(({ token, membershipId, locale }) => {
      const options = { token, membershipId, ifMatch: address.rowVersion!, idempotencyKey: crypto.randomUUID(), locale };
      return action === "primary"
        ? apiClient.setPrimaryCustomerAddress(customerId, address.id!, options)
        : apiClient.deactivateCustomerAddress(customerId, address.id!, options);
    });
    if (result.succeeded) {
      setDeactivating(null);
    }
  };

  const startEditing = (address: CustomerAddressResponse) => {
    setEditing(address);
    form.reset({ addressType: address.addressType === "contact" ? "contact" : "billing", label: address.label ?? "", addressLine1: address.addressLine1 ?? "", subdistrict: address.subdistrict ?? "", district: address.district ?? "", province: address.province ?? "", postalCode: address.postalCode ?? "", countryCode: address.countryCode ?? "TH" });
  };

  const handleAddressSelect = (address: SelectedAddress) => {
    form.setValue("subdistrict", address.subdistrict, { shouldDirty: true, shouldValidate: true });
    form.setValue("district", address.district, { shouldDirty: true, shouldValidate: true });
    form.setValue("province", address.province, { shouldDirty: true, shouldValidate: true });
    form.setValue("postalCode", address.postalCode, { shouldDirty: true, shouldValidate: true });
    form.setValue("countryCode", address.countryCode, { shouldDirty: true, shouldValidate: true });
  };

  const handleClearAddress = () => {
    form.setValue("subdistrict", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("district", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("province", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("postalCode", "", { shouldDirty: true, shouldValidate: true });
    form.setValue("countryCode", "", { shouldDirty: true, shouldValidate: true });
  };

  if (addressesQuery.isLoading) return <div className="flex min-h-24 items-center justify-center"><MonoSpinner label={tCommon("states.loading")} /></div>;

  return <section className="erp-card flex flex-col gap-5 p-6" aria-labelledby="customer-address-title">
    <h2 id="customer-address-title" className="m-0 border-b border-erp-border-subtle pb-3 text-lg font-bold text-erp-navy">{t("addressesTab")}</h2>
    <div className="grid gap-3 md:grid-cols-2">
      {(addressesQuery.data?.items ?? []).map((address) => <article key={address.id} className="border border-erp-border p-4">
        <div className="flex items-start justify-between gap-3">
          <div>
            <p className="m-0 font-semibold">{address.label} {address.isPrimary ? <span className="erp-badge erp-badge-success">{t("primaryAddress")}</span> : null}</p>
            <p className="mt-2 text-sm text-erp-text-muted">{[address.addressLine1, address.subdistrict, address.district, address.province, address.postalCode, address.countryCode].filter(Boolean).join(", ")}</p>
          </div>
          <span className="erp-badge erp-badge-neutral">{address.addressType === "billing" ? t("billingAddress") : t("contactAddress")}</span>
        </div>
        {canManage && !readOnly && address.status === "active" && <div className="mt-4 flex flex-wrap gap-2">
          <Button variant="outline" size="sm" onClick={() => startEditing(address)}>{tCommon("actions.edit")}</Button>
          {!address.isPrimary && address.addressType === "billing" && <Button variant="outline" size="sm" onClick={() => void runAddressAction(address, "primary")}>{t("makePrimaryAddress")}</Button>}
          <Button variant="danger" size="sm" onClick={() => setDeactivating(address)}>{t("deactivateAddress")}</Button>
        </div>}
      </article>)}
    </div>
    {canManage && !readOnly && <FormSection title={editing ? t("editAddress") : t("addAddress")}>
      <form onSubmit={submit} className="flex flex-col gap-4">
        <div className="grid gap-4 md:grid-cols-2">
          <Select label={t("addressType")} disabled={Boolean(editing)} options={[{ value: "billing", label: t("billingAddress") }, { value: "contact", label: t("contactAddress") }]} {...form.register("addressType")} />
          <Input label={t("addressLabel")} error={form.formState.errors.label?.message} {...form.register("label")} />
          <Input label={t("addressLine1")} error={form.formState.errors.addressLine1?.message} {...form.register("addressLine1")} />
          <div className="md:col-span-2">
            <Controller
              control={form.control}
              name="subdistrict"
              render={({ field }) => {
                const hasAreaError = Boolean(
                  form.formState.errors.subdistrict ||
                  form.formState.errors.district ||
                  form.formState.errors.province ||
                  form.formState.errors.postalCode ||
                  form.formState.errors.countryCode
                );

                return (
                  <AddressAreaField
                    id={`customer-address-area-${customerId}`}
                    label={tCommon("addressAutocomplete.areaSelection")}
                    hint={tCommon("addressAutocomplete.smartSearchHint")}
                    required
                    error={hasAreaError ? tCommon("addressAutocomplete.areaRequired") : undefined}
                    value={{
                      subdistrict: field.value,
                      district: form.getValues("district"),
                      province: form.getValues("province"),
                      postalCode: form.getValues("postalCode"),
                      countryCode: form.getValues("countryCode"),
                    }}
                    onSelect={handleAddressSelect}
                    onClear={handleClearAddress}
                  />
                );
              }}
            />
          </div>
        </div>
        <FormActionBar isEditMode={Boolean(editing)} isDirty={form.formState.isDirty} isLoading={isSaving || form.formState.isSubmitting} saveText={tCommon("actions.saveChanges")} onCancel={() => { form.reset(); setEditing(null); }} />
      </form>
    </FormSection>}
    {canManage && !readOnly && <ConfirmationModal isOpen={Boolean(deactivating)} onClose={() => setDeactivating(null)} onConfirm={() => { if (deactivating) { setIsDeactivating(true); void runAddressAction(deactivating, "deactivate").finally(() => setIsDeactivating(false)); } }} title={t("deactivateAddressTitle")} message={t("deactivateAddressConfirm")} confirmText={tCommon("actions.confirm")} cancelText={tCommon("actions.cancel")} variant="danger" isLoading={isDeactivating} />}
  </section>;
}
