"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { apiClient, type CustomerContactDetailResponse } from "@/lib/api/api-client";
import { Button } from "@/components/ui/Button";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useCustomerContacts } from "../api/customer-queries";
import { useCustomerMutationRunner } from "../hooks/use-customer-mutation-runner";
import { CustomerContactFields } from "./customer-contact-fields";

const contactSchema = z.object({
  name: z.string().trim().min(1).max(250),
  roleTitle: z.string().trim().max(150),
  phone: z.string().trim().max(30),
  email: z.string().trim().max(150),
  lineId: z.string().trim().max(100),
  preferredChannel: z.enum(["phone", "email", "line", "other"]),
});
type ContactValues = z.infer<typeof contactSchema>;

export function CustomerContactManager({ customerId, canManage, readOnly = false }: { customerId: string; canManage: boolean; readOnly?: boolean }) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const runCustomerMutation = useCustomerMutationRunner();
  const [editing, setEditing] = React.useState<CustomerContactDetailResponse | null>(null);
  const [deactivating, setDeactivating] = React.useState<CustomerContactDetailResponse | null>(null);
  const [isSaving, setIsSaving] = React.useState(false);
  const [isDeactivating, setIsDeactivating] = React.useState(false);
  const contactsQuery = useCustomerContacts(customerId, canManage);
  const form = useForm<ContactValues>({
    resolver: zodResolver(contactSchema),
    defaultValues: { name: "", roleTitle: "", phone: "", email: "", lineId: "", preferredChannel: "phone" },
  });
  const submit = form.handleSubmit(async (values) => {
    const payload = { name: values.name, roleTitle: values.roleTitle || undefined, phone: values.phone || undefined, email: values.email || undefined, lineId: values.lineId || undefined, preferredChannel: values.preferredChannel };
    setIsSaving(true);
    try {
      const result = await runCustomerMutation(async ({ token, membershipId, locale }) => editing
        ? apiClient.updateCustomerContact(customerId, editing.id!, payload, { token, membershipId, ifMatch: editing.rowVersion!, locale })
        : apiClient.createCustomerContact(customerId, payload, { token, membershipId, idempotencyKey: crypto.randomUUID(), locale }));
      if (!result.succeeded) return;
      setEditing(null);
      form.reset();
    } finally {
      setIsSaving(false);
    }
  });
  const runContactAction = async (contact: CustomerContactDetailResponse, action: "primary" | "deactivate") => {
    const result = await runCustomerMutation(({ token, membershipId, locale }) => {
      const options = { token, membershipId, ifMatch: contact.rowVersion!, idempotencyKey: crypto.randomUUID(), locale };
      return action === "primary"
        ? apiClient.setPrimaryCustomerContact(customerId, contact.id!, options)
        : apiClient.deactivateCustomerContact(customerId, contact.id!, options);
    });
    if (result.succeeded) {
      setDeactivating(null);
    }
  };

  if (!canManage) return null;
  if (contactsQuery.isLoading) return <div className="flex min-h-24 items-center justify-center"><MonoSpinner label={tCommon("states.loading")} /></div>;

  return <section className="erp-card flex flex-col gap-5 p-6" aria-labelledby="customer-contacts-title">
    <h2 id="customer-contacts-title" className="m-0 border-b border-erp-border-subtle pb-3 text-lg font-bold text-erp-navy">{t("contactsTab")}</h2>
    <div className="grid gap-3 md:grid-cols-2">
      {(contactsQuery.data?.items ?? []).map((contact) => <article key={contact.id} className="border border-erp-border p-4">
        <p className="m-0 font-semibold">{contact.name} {contact.isPrimary ? <span className="erp-badge erp-badge-success">{t("primaryContact")}</span> : null}</p>
        <p className="mt-2 text-sm text-erp-text-muted">{[contact.roleTitle, contact.phone, contact.email, contact.lineId].filter(Boolean).join(" · ") || "-"}</p>
        {!readOnly && contact.status === "active" && <div className="mt-4 flex flex-wrap gap-2">
          <Button variant="outline" size="sm" onClick={() => { setEditing(contact); form.reset({ name: contact.name ?? "", roleTitle: contact.roleTitle ?? "", phone: contact.phone ?? "", email: contact.email ?? "", lineId: contact.lineId ?? "", preferredChannel: contact.preferredChannel === "email" || contact.preferredChannel === "line" || contact.preferredChannel === "other" ? contact.preferredChannel : "phone" }); }}>{tCommon("actions.edit")}</Button>
          {!contact.isPrimary && <Button variant="outline" size="sm" onClick={() => void runContactAction(contact, "primary")}>{t("makePrimaryContact")}</Button>}
          {!contact.isPrimary && <Button variant="danger" size="sm" onClick={() => setDeactivating(contact)}>{t("deactivateContact")}</Button>}
        </div>}
      </article>)}
    </div>
    {!readOnly && <FormSection title={editing ? t("editContact") : t("addContact")}>
      <form onSubmit={submit} className="flex flex-col gap-4">
        <CustomerContactFields
          values={form.watch()}
          errors={{
            name: form.formState.errors.name?.message,
            roleTitle: form.formState.errors.roleTitle?.message,
            phone: form.formState.errors.phone?.message,
            email: form.formState.errors.email?.message,
            lineId: form.formState.errors.lineId?.message,
            preferredChannel: form.formState.errors.preferredChannel?.message,
          }}
          disabled={isSaving || form.formState.isSubmitting}
          onNameChange={(value) => form.setValue("name", value, { shouldDirty: true, shouldValidate: true })}
          onRoleTitleChange={(value) => form.setValue("roleTitle", value, { shouldDirty: true, shouldValidate: true })}
          onPhoneChange={(value) => form.setValue("phone", value, { shouldDirty: true, shouldValidate: true })}
          onEmailChange={(value) => form.setValue("email", value, { shouldDirty: true, shouldValidate: true })}
          onLineIdChange={(value) => form.setValue("lineId", value, { shouldDirty: true, shouldValidate: true })}
          onPreferredChannelChange={(value) => form.setValue("preferredChannel", value, { shouldDirty: true, shouldValidate: true })}
        />
        <FormActionBar isEditMode={Boolean(editing)} isDirty={form.formState.isDirty} isLoading={isSaving || form.formState.isSubmitting} saveText={tCommon("actions.saveChanges")} onCancel={() => { form.reset(); setEditing(null); }} />
      </form>
    </FormSection>}
    {!readOnly && <ConfirmationModal isOpen={Boolean(deactivating)} onClose={() => setDeactivating(null)} onConfirm={() => { if (deactivating) { setIsDeactivating(true); void runContactAction(deactivating, "deactivate").finally(() => setIsDeactivating(false)); } }} title={t("deactivateContactTitle")} message={t("deactivateContactConfirm")} confirmText={tCommon("actions.confirm")} cancelText={tCommon("actions.cancel")} variant="danger" isLoading={isDeactivating} />}
  </section>;
}
