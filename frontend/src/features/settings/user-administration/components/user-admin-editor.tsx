"use client";

import React, { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { useToast } from "@/hooks/useToast";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useCreateAdminUser } from "../api/user-admin-queries";
import { adminErrorKey } from "../user-admin-errors";
import { UserAdminFormFields, type UserAdminFormValues } from "./user-admin-form-fields";

const MAX_NAME_LENGTH = 255;

function fromInputValue(value: string): string | null {
  if (!value) return null;
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? null : date.toISOString();
}

export function UserAdminEditor() {
  const t = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const context = useApiRequestContext();

  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const { uploadSingleFile, isUploading } = useDeferredFileUpload({
    parentType: "user",
  });

  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);
  const createMutation = useCreateAdminUser();

  const schema = z.object({
    displayName: z.string().trim().min(1, t("validation.displayNameRequired")).max(MAX_NAME_LENGTH, t("validation.displayNameTooLong")),
    email: z.string().trim().min(1, t("validation.emailRequired")).email(t("validation.emailInvalid")),
    phoneNumber: z.string().max(50, t("validation.phoneTooLong")),
    employeeCode: z.string().max(50, t("validation.codeTooLong")),
    departmentId: z.string(),
    jobTitleId: z.string(),
    notes: z.string().max(1000, t("validation.notesTooLong")),
    branchId: z.string(),
    startsAt: z.string(),
    expiresAt: z.string(),
    roleIds: z.array(z.string()).min(1, t("validation.roleRequired")),
  });

  const {
    register,
    control,
    handleSubmit,
    setValue,
    formState: { errors, isDirty },
  } = useForm<UserAdminFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      displayName: "",
      email: "",
      phoneNumber: "",
      employeeCode: "",
      departmentId: "",
      jobTitleId: "",
      notes: "",
      branchId: "",
      startsAt: "",
      expiresAt: "",
      roleIds: [],
    },
  });

  const watchedDisplayName = useWatch({ control, name: "displayName" });
  const watchedDepartmentId = useWatch({ control, name: "departmentId" });

  const isBusy = createMutation.isPending || isUploading;

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);

    let avatarFileId: string | null = null;
    if (avatarFile) {
      try {
        const token = await getAuthToken();
        const uploadRes = await uploadSingleFile(avatarFile, {
          token: token ?? "",
          membershipId: context.membershipId,
          locale,
        });
        avatarFileId = uploadRes?.fileId ?? null;
      } catch (err: unknown) {
        setSubmitError(err instanceof Error ? err.message : t("errors.GENERIC"));
        return;
      }
    }

    const payload = {
      displayName: values.displayName.trim(),
      email: values.email.trim(),
      phoneNumber: values.phoneNumber.trim() || null,
      employeeCode: values.employeeCode.trim() || null,
      departmentId: values.departmentId || null,
      jobTitleId: values.jobTitleId || null,
      avatarFileId,
      notes: values.notes.trim() || null,
      branchId: values.branchId || null,
      startsAtUtc: fromInputValue(values.startsAt),
      expiresAtUtc: fromInputValue(values.expiresAt),
      roleIds: values.roleIds,
    };

    const fingerprint = JSON.stringify(payload);
    if (intentRef.current?.fingerprint !== fingerprint) {
      intentRef.current = { fingerprint, key: crypto.randomUUID() };
    }

    try {
      const user = await createMutation.mutateAsync({ payload, idempotencyKey: intentRef.current.key });
      intentRef.current = null;
      toast.success(t("createSuccess"));
      router.push(`/${locale}/settings/users/${user.id}`);
    } catch (error: unknown) {
      setSubmitError(t(`errors.${adminErrorKey(error)}`));
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
          title={t("createTitle")}
          subtitle={t("createSubtitle")}
          backHref={`/${locale}/settings/users`}
          backLabel={t("backToList")}
        />
      }
      errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
      actionBar={
        <FormActionBar
          isDirty={isDirty || avatarFile !== null}
          isLoading={isBusy}
          isSaveDisabled={isBusy}
          saveText={t("createSubmit")}
          saveButtonType="submit"
          cancelHref={`/${locale}/settings/users`}
          showCancel
          cancelText={tCommon("actions.cancel")}
        />
      }
    >
      <UserAdminFormFields
        mode="create"
        register={register}
        control={control}
        errors={errors}
        setValue={setValue}
        avatarFile={avatarFile}
        onAvatarFileChange={setAvatarFile}
        currentDisplayName={watchedDisplayName}
        selectedDepartmentId={watchedDepartmentId}
        disabled={isBusy}
      />
    </FormContainer>
  );
}
