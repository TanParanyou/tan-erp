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
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { useToast } from "@/hooks/useToast";
import type { CreateApprovalLimitRequest } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { isApprovalDocumentType } from "../approval-limit-document-types";
import { useApprovalLimit, useApprovalLimitOptions, useCreateApprovalLimit, useUpdateApprovalLimit } from "../api/approval-limit-queries";
import { approvalLimitErrorKey } from "../approval-limit-errors";

const CURRENCY = "THB";
const AMOUNT_PATTERN = /^\d{1,13}(\.\d{1,2})?$/;

interface LimitFormValues {
  roleId: string;
  documentType: string;
  branchId: string;
  isUnlimited: boolean;
  maxAmount: string;
  isActive: boolean;
}

export function ApprovalLimitEditor({ limitId }: { limitId: string }) {
  const isCreate = limitId === "create" || limitId === "add";
  const t = useTranslations("approvalLimitAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.APPROVAL_LIMITS_MANAGE);
  const limitQuery = useApprovalLimit(limitId, !isCreate);
  const optionsQuery = useApprovalLimitOptions();
  const createMutation = useCreateApprovalLimit();
  const updateMutation = useUpdateApprovalLimit();
  const [submitError, setSubmitError] = useState<string | null>(null);
  // One key per submit intent: a retry after a network failure replays the same request instead of creating twice.
  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);

  // Create mode never reads an existing limit, even if the query layer still holds data for the route param.
  const limit = isCreate ? undefined : limitQuery.data;
  const existing = limit?.id && limit.rowVersion ? { id: limit.id, rowVersion: limit.rowVersion } : null;

  const schema = useMemo(
    () =>
      z
        .object({
          roleId: z.string(),
          documentType: z.string(),
          branchId: z.string(),
          isUnlimited: z.boolean(),
          maxAmount: z.string().trim(),
          isActive: z.boolean(),
        })
        .superRefine((value, ctx) => {
          if (isCreate && value.roleId === "") ctx.addIssue({ code: "custom", path: ["roleId"], message: t("editor.validation.roleRequired") });
          if (isCreate && value.documentType === "") ctx.addIssue({ code: "custom", path: ["documentType"], message: t("editor.validation.documentTypeRequired") });
          if (!value.isUnlimited && value.maxAmount === "") ctx.addIssue({ code: "custom", path: ["maxAmount"], message: t("editor.validation.amountRequired") });
          if (!value.isUnlimited && value.maxAmount !== "" && !AMOUNT_PATTERN.test(value.maxAmount))
            ctx.addIssue({ code: "custom", path: ["maxAmount"], message: t("editor.validation.amountInvalid") });
        }),
    [isCreate, t]
  );
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isDirty },
  } = useForm<LimitFormValues>({
    resolver: zodResolver(schema),
    defaultValues: { roleId: "", documentType: "", branchId: "", isUnlimited: false, maxAmount: "", isActive: true },
    values:
      !isCreate && limit
        ? {
            roleId: limit.role?.id ?? "",
            documentType: limit.documentType ?? "",
            branchId: limit.branch?.id ?? "",
            isUnlimited: limit.isUnlimited === true,
            maxAmount: limit.maxAmount === null || limit.maxAmount === undefined ? "" : String(limit.maxAmount),
            isActive: limit.isActive === true,
          }
        : undefined,
  });
  const isUnlimited = watch("isUnlimited");

  if (!isCreate && limitQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (!isCreate && (limitQuery.isError || !existing)) return <Alert variant="danger">{t("errors.RESOURCE_NOT_FOUND")}</Alert>;

  const backHref = `/${locale}/settings/approval-limits`;
  const isSaving = createMutation.isPending || updateMutation.isPending;
  const options = optionsQuery.data;

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    const maxAmount = values.isUnlimited ? null : Number(values.maxAmount);
    try {
      if (isCreate) {
        const payload: CreateApprovalLimitRequest = {
          roleId: values.roleId,
          documentType: values.documentType,
          branchId: values.branchId === "" ? null : values.branchId,
          currency: CURRENCY,
          isUnlimited: values.isUnlimited,
          maxAmount,
        };
        const fingerprint = JSON.stringify(payload);
        if (intentRef.current?.fingerprint !== fingerprint) intentRef.current = { fingerprint, key: crypto.randomUUID() };
        const created = await createMutation.mutateAsync({ payload, idempotencyKey: intentRef.current.key });
        intentRef.current = null;
        toast.success(t("editor.createSuccess"));
        if (created.id) router.push(`${backHref}/${created.id}`);
      } else if (existing) {
        await updateMutation.mutateAsync({
          limitId: existing.id,
          payload: { isUnlimited: values.isUnlimited, maxAmount, isActive: values.isActive },
          ifMatch: existing.rowVersion,
        });
        toast.success(t("editor.saveSuccess"));
      }
    } catch (error: unknown) {
      setSubmitError(t(`errors.${approvalLimitErrorKey(error)}`));
    }
  });

  return (
    <FormContainer
      asForm
      noValidate
      onSubmit={onSubmit}
      maxWidth="lg"
      header={
        <PageHeader
          title={isCreate ? t("editor.createTitle") : t("editor.editTitle")}
          subtitle={isCreate ? t("editor.createSubtitle") : (limit?.role?.name ?? undefined)}
          backHref={backHref}
          backLabel={t("editor.backToList")}
        />
      }
      errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
      actionBar={
        canManage ? (
          <FormActionBar
            isDirty={isDirty}
            isLoading={isSaving}
            isSaveDisabled={isSaving}
            saveText={isCreate ? t("editor.createSubmit") : t("editor.saveSubmit")}
            saveButtonType="submit"
            cancelHref={backHref}
            showCancel
            cancelText={tCommon("actions.cancel")}
          />
        ) : undefined
      }
    >
      <FormSection title={isCreate ? t("editor.createTitle") : t("editor.editTitle")}>
        {!isCreate ? <Alert variant="info">{t("editor.identityLocked")}</Alert> : null}
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {isCreate ? (
            <>
              {optionsQuery.isError ? <Alert variant="danger">{t("errors.GENERIC")}</Alert> : null}
              <Select
                label={t("editor.fields.role")}
                required
                placeholder={t("editor.placeholders.role")}
                options={(options?.roles ?? []).flatMap((r) => (r.id ? [{ value: r.id, label: r.name ?? "-" }] : []))}
                error={errors.roleId?.message}
                {...register("roleId")}
              />
              <Select
                label={t("editor.fields.documentType")}
                required
                placeholder={t("editor.placeholders.documentType")}
                options={(options?.documentTypes ?? []).filter(isApprovalDocumentType).map((type) => ({ value: type, label: t(`documentType.${type}`) }))}
                error={errors.documentType?.message}
                {...register("documentType")}
              />
              <Select
                label={t("editor.fields.branch")}
                placeholder={t("editor.placeholders.organizationWide")}
                helperText={t("editor.hints.branch")}
                options={(options?.branches ?? []).flatMap((b) => (b.id ? [{ value: b.id, label: b.name ?? "-" }] : []))}
                {...register("branchId")}
              />
            </>
          ) : (
            <>
              <Input label={t("editor.fields.role")} readOnly disabled placeholder="-" value={limit?.role?.name ?? "-"} />
              <Input
                label={t("editor.fields.documentType")}
                readOnly
                disabled
                placeholder="-"
                value={limit?.documentType && isApprovalDocumentType(limit.documentType) ? t(`documentType.${limit.documentType}`) : "-"}
              />
              <Input
                label={t("editor.fields.branch")}
                readOnly
                disabled
                placeholder={t("editor.placeholders.organizationWide")}
                value={limit?.branch ? (limit.branch.name ?? "-") : t("editor.placeholders.organizationWide")}
              />
            </>
          )}
          <Input label={t("editor.fields.currency")} readOnly disabled placeholder="THB" value={isCreate ? CURRENCY : (limit?.currency ?? "-")} />
        </div>
        <Checkbox label={t("editor.fields.isUnlimited")} description={t("editor.hints.unlimited")} disabled={!canManage} {...register("isUnlimited")} />
        <Input
          label={t("editor.fields.maxAmount")}
          inputMode="decimal"
          placeholder={t("editor.placeholders.maxAmount")}
          disabled={!canManage || isUnlimited}
          error={errors.maxAmount?.message}
          {...register("maxAmount")}
        />
        {!isCreate ? (
          <Checkbox label={t("editor.fields.isActive")} description={t("editor.hints.inactive")} disabled={!canManage} {...register("isActive")} />
        ) : null}
      </FormSection>
    </FormContainer>
  );
}
