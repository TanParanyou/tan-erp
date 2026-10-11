"use client";

import React, { useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Textarea } from "@/components/ui/Textarea";
import { useToast } from "@/hooks/useToast";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useAdminBranch, useCreateAdminBranch, useUpdateAdminBranch } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";
import { BranchActivationPanel } from "./branch-activation-panel";

const CODE_PATTERN = /^[A-Za-z0-9_-]+$/;
const TAX_BRANCH_CODE_PATTERN = /^\d{5}$/;

interface BranchFormValues {
  code: string;
  name: string;
  nameEn: string;
  taxBranchCode: string;
  addressTh: string;
  addressEn: string;
  phone: string;
}

const EMPTY: BranchFormValues = { code: "", name: "", nameEn: "", taxBranchCode: "", addressTh: "", addressEn: "", phone: "" };
const orNull = (value: string): string | null => (value.trim() === "" ? null : value.trim());

/** Nullable API fields become empty inputs; blank inputs go back to the API as null. */
function toFormValues(branch: AdminBranchResponse): BranchFormValues {
  return {
    code: branch.code ?? "",
    name: branch.name ?? "",
    nameEn: branch.nameEn ?? "",
    taxBranchCode: branch.taxBranchCode ?? "",
    addressTh: branch.addressTh ?? "",
    addressEn: branch.addressEn ?? "",
    phone: branch.phone ?? "",
  };
}

export function BranchAdminEditor({ branchId }: { branchId: string }) {
  const isCreate = branchId === "create" || branchId === "add";
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BRANCHES_MANAGE);
  const branchQuery = useAdminBranch(branchId, !isCreate);
  const createMutation = useCreateAdminBranch();
  const updateMutation = useUpdateAdminBranch();
  const [submitError, setSubmitError] = useState<string | null>(null);
  // One key per submit intent: a retry after a network failure replays the same request instead of creating twice.
  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);

  const schema = useMemo(
    () =>
      z.object({
        code: isCreate
          ? z.string().trim().min(1, t("branches.validation.codeRequired")).max(50).regex(CODE_PATTERN, t("branches.validation.codeFormat"))
          : z.string(),
        name: z.string().trim().min(1, t("branches.validation.nameRequired")).max(255),
        nameEn: z.string().trim().max(255),
        taxBranchCode: z.string().trim().refine((v) => v === "" || TAX_BRANCH_CODE_PATTERN.test(v), t("branches.validation.taxBranchCodeFormat")),
        addressTh: z.string().trim().max(500),
        addressEn: z.string().trim().max(500),
        phone: z.string().trim().max(30),
      }),
    [isCreate, t]
  );

  const branch = branchQuery.data;
  // Identifiers are optional in the generated DTO; keep the edit target only when both are present.
  const existing = branch?.id && branch.rowVersion ? { id: branch.id, rowVersion: branch.rowVersion } : null;
  const {
    register,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<BranchFormValues>({
    resolver: zodResolver(schema),
    defaultValues: EMPTY,
    values: !isCreate && branch ? toFormValues(branch) : undefined,
  });

  if (!isCreate && branchQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (!isCreate && (branchQuery.isError || !existing)) return <Alert variant="danger">{t("errors.RESOURCE_NOT_FOUND")}</Alert>;

  const backHref = `/${locale}/settings/branches`;
  const isSaving = createMutation.isPending || updateMutation.isPending;

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    const details = {
      name: values.name.trim(),
      nameEn: orNull(values.nameEn),
      taxBranchCode: orNull(values.taxBranchCode),
      addressTh: orNull(values.addressTh),
      addressEn: orNull(values.addressEn),
      phone: orNull(values.phone),
    };
    try {
      if (isCreate) {
        const payload = { code: values.code.trim(), ...details };
        const fingerprint = JSON.stringify(payload);
        if (intentRef.current?.fingerprint !== fingerprint) intentRef.current = { fingerprint, key: crypto.randomUUID() };
        const created = await createMutation.mutateAsync({ payload, idempotencyKey: intentRef.current.key });
        intentRef.current = null;
        toast.success(t("branches.createSuccess"));
        if (created.id) router.push(`${backHref}/${created.id}`);
      } else if (existing) {
        await updateMutation.mutateAsync({ branchId: existing.id, payload: details, ifMatch: existing.rowVersion });
        toast.success(t("branches.saveSuccess"));
      }
    } catch (error: unknown) {
      setSubmitError(t(`errors.${organizationAdminErrorKey(error)}`));
    }
  });

  return (
    <div className="space-y-6">
      <FormContainer
        asForm
        noValidate
        onSubmit={onSubmit}
        maxWidth="lg"
        header={
          <PageHeader
            title={isCreate ? t("branches.createTitle") : t("branches.editTitle")}
            subtitle={isCreate ? t("branches.createSubtitle") : (branch?.name ?? undefined)}
            backHref={backHref}
            backLabel={t("branches.backToList")}
          />
        }
        errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
        actionBar={
          canManage ? (
            <FormActionBar
              isDirty={isDirty}
              isLoading={isSaving}
              isSaveDisabled={isSaving}
              saveText={isCreate ? t("branches.createSubmit") : t("branches.saveSubmit")}
              saveButtonType="submit"
              cancelHref={backHref}
              showCancel
              cancelText={tCommon("actions.cancel")}
            />
          ) : undefined
        }
      >
        <FormSection title={t("branches.title")}>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Input
              label={t("branches.fields.code")}
              required={isCreate}
              disabled={!isCreate}
              helperText={isCreate ? undefined : t("branches.createSubtitle")}
              error={errors.code?.message}
              {...register("code")}
            />
            <Input
              label={t("branches.fields.taxBranchCode")}
              helperText={t("branches.fields.taxBranchCodeHint")}
              inputMode="numeric"
              maxLength={5}
              disabled={!canManage}
              error={errors.taxBranchCode?.message}
              {...register("taxBranchCode")}
            />
            <Input label={t("branches.fields.name")} required disabled={!canManage} error={errors.name?.message} {...register("name")} />
            <Input label={t("branches.fields.nameEn")} disabled={!canManage} error={errors.nameEn?.message} {...register("nameEn")} />
            <Textarea label={t("branches.fields.addressTh")} rows={3} disabled={!canManage} error={errors.addressTh?.message} {...register("addressTh")} />
            <Textarea label={t("branches.fields.addressEn")} rows={3} disabled={!canManage} error={errors.addressEn?.message} {...register("addressEn")} />
            <Input label={t("branches.fields.phone")} disabled={!canManage} error={errors.phone?.message} {...register("phone")} />
          </div>
        </FormSection>
      </FormContainer>
      {!isCreate && branch ? <BranchActivationPanel branch={branch} canManage={canManage} /> : null}
    </div>
  );
}
