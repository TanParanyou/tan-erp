"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { Badge } from "@/components/ui/Badge";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { useToast } from "@/hooks/useToast";
import { useOrganizationBranches } from "@/features/item-master/api/item-master-queries";
import { useAdminRoles, useCreateAdminUser } from "../api/user-admin-queries";
import { adminErrorKey } from "../user-admin-errors";

const MAX_NAME_LENGTH = 255;

interface UserAdminFormValues {
  displayName: string;
  email: string;
  branchId: string;
  roleIds: string[];
}

export function UserAdminEditor() {
  const t = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const [submitError, setSubmitError] = useState<string | null>(null);

  const rolesQuery = useAdminRoles();
  const branchesQuery = useOrganizationBranches();
  const createMutation = useCreateAdminUser();

  const schema = z.object({
    displayName: z.string().trim().min(1, t("validation.displayNameRequired")).max(MAX_NAME_LENGTH, t("validation.displayNameTooLong")),
    email: z.string().trim().min(1, t("validation.emailRequired")).email(t("validation.emailInvalid")),
    branchId: z.string(),
    roleIds: z.array(z.string()).min(1, t("validation.roleRequired")),
  });

  const {
    register,
    control,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<UserAdminFormValues>({
    resolver: zodResolver(schema),
    defaultValues: { displayName: "", email: "", branchId: "", roleIds: [] },
  });

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    try {
      const user = await createMutation.mutateAsync({
        displayName: values.displayName.trim(),
        email: values.email.trim(),
        branchId: values.branchId || null,
        roleIds: values.roleIds,
      });
      toast.success(t("createSuccess"));
      router.push(`/${locale}/settings/users/${user.id}`);
    } catch (error: unknown) {
      setSubmitError(t(`errors.${adminErrorKey(error)}`));
    }
  });

  const roles = rolesQuery.data?.items ?? [];
  const branches = branchesQuery.data ?? [];

  return (
    <FormContainer
      asForm
      onSubmit={onSubmit}
      maxWidth="lg"
      header={
        <PageHeader
          title={t("createTitle")}
          subtitle={t("createSubtitle")}
          backHref={`/${locale}/settings/users`}
          backLabel={t("backToList")}
        />
      }
      errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
      actionBar={
        <FormActionBar
          isDirty={isDirty}
          isLoading={createMutation.isPending}
          isSaveDisabled={createMutation.isPending}
          saveText={t("createSubmit")}
          saveButtonType="submit"
          cancelHref={`/${locale}/settings/users`}
          showCancel
          cancelText={tCommon("actions.cancel")}
        />
      }
    >
      <FormSection title={t("sections.identity")} description={t("sections.identityHint")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <Input label={t("fields.displayName")} required error={errors.displayName?.message} {...register("displayName")} />
          <Input
            label={t("fields.email")}
            type="email"
            required
            helperText={t("fields.emailHint")}
            error={errors.email?.message}
            {...register("email")}
          />
        </div>
      </FormSection>

      <FormSection title={t("sections.access")} description={t("sections.accessHint")}>
        <Select
          label={t("fields.branch")}
          options={branches.map((branch) => ({ value: branch.id, label: branch.name }))}
          placeholder={t("organizationWide")}
          {...register("branchId")}
        />

        {rolesQuery.isPending ? (
          <MonoSpinner size="md" label={tCommon("states.loading")} />
        ) : rolesQuery.isError ? (
          <Alert variant="danger">{t("errors.GENERIC")}</Alert>
        ) : (
          <Controller
            control={control}
            name="roleIds"
            render={({ field }) => (
              <fieldset className="flex flex-col gap-1" aria-describedby={errors.roleIds ? "role-ids-error" : undefined}>
                <legend className="erp-label">
                  {t("fields.roles")}
                  <span className="erp-label-required">*</span>
                </legend>
                {roles.map((role) => {
                  const checked = field.value.includes(role.id ?? "");
                  return (
                    <div key={role.id} className="flex items-center gap-2">
                      <Checkbox
                        label={role.name}
                        checked={checked}
                        disabled={!role.assignable}
                        onChange={(event) =>
                          field.onChange(
                            event.target.checked
                              ? [...field.value, role.id ?? ""]
                              : field.value.filter((id) => id !== role.id)
                          )
                        }
                      />
                      {role.requiresApproval ? <Badge variant="warning" size="sm">{t("requiresApproval")}</Badge> : null}
                      {!role.assignable ? <Badge variant="outline" size="sm">{t("notAssignable")}</Badge> : null}
                    </div>
                  );
                })}
                {errors.roleIds ? (
                  <p id="role-ids-error" role="alert" className="text-xs text-erp-danger">
                    {errors.roleIds.message}
                  </p>
                ) : null}
              </fieldset>
            )}
          />
        )}
      </FormSection>
    </FormContainer>
  );
}
