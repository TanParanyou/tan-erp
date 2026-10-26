"use client";

import React, { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import { getAuthToken } from "@/lib/auth/auth-session";
import type { UserDto } from "@/lib/api/api-client";
import { AvatarUpload } from "@/components/forms/AvatarUpload";
import { useUpdateMyProfile } from "../api/profile-queries";
import { profileErrorKey } from "../profile-errors";

const MAX_NAME_LENGTH = 255;
const MAX_PHONE_LENGTH = 50;

interface PersonalProfileValues {
  displayName: string;
  phoneNumber: string;
  preferredLocale: string;
}

interface PersonalProfileFormProps {
  user: UserDto;
}

export function PersonalProfileForm({ user }: PersonalProfileFormProps) {
  const t = useTranslations("myProfile");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const context = useApiRequestContext();
  const updateMutation = useUpdateMyProfile();
  const { uploadSingleFile, isUploading } = useDeferredFileUpload({ parentType: "user" });

  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const [avatarRemoved, setAvatarRemoved] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const schema = z.object({
    displayName: z.string().trim().min(1, t("validation.displayNameRequired")).max(MAX_NAME_LENGTH, t("validation.displayNameTooLong")),
    phoneNumber: z.string().max(MAX_PHONE_LENGTH, t("validation.phoneTooLong")),
    preferredLocale: z.string(),
  });

  const {
    register,
    handleSubmit,
    watch,
    reset,
    formState: { errors, isDirty },
  } = useForm<PersonalProfileValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      displayName: user.displayName ?? "",
      phoneNumber: user.phoneNumber ?? "",
      preferredLocale: user.preferredLocale ?? "",
    },
  });

  const isBusy = updateMutation.isPending || isUploading;
  const watchedName = watch("displayName");

  const handleAvatarChange = (file: File | null) => {
    setAvatarFile(file);
    setAvatarRemoved(file === null);
  };

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);

    let avatarFileId: string | null = avatarRemoved ? null : user.avatarFileId ?? null;
    if (avatarFile) {
      try {
        const token = await getAuthToken();
        const uploaded = await uploadSingleFile(avatarFile, {
          token: token ?? "",
          membershipId: context.membershipId,
          locale,
        });
        avatarFileId = uploaded?.fileId ?? null;
      } catch (error: unknown) {
        setSubmitError(error instanceof Error ? error.message : t("errors.GENERIC"));
        return;
      }
    }

    try {
      const saved = await updateMutation.mutateAsync({
        displayName: values.displayName.trim(),
        phoneNumber: values.phoneNumber.trim() || null,
        preferredLocale: values.preferredLocale || null,
        avatarFileId,
      });
      toast.success(t("saveSuccess"));
      setAvatarFile(null);
      setAvatarRemoved(false);
      reset({
        displayName: saved.displayName ?? values.displayName,
        phoneNumber: saved.phoneNumber ?? "",
        preferredLocale: saved.preferredLocale ?? "",
      });
      if (saved.preferredLocale && saved.preferredLocale !== locale) {
        router.replace(`/${saved.preferredLocale}/profile`);
      }
    } catch (error: unknown) {
      setSubmitError(t(`errors.${profileErrorKey(error)}`));
    }
  });

  return (
    <form onSubmit={onSubmit} noValidate className="flex flex-col gap-6">
      {submitError && <Alert variant="danger">{submitError}</Alert>}

      <FormSection title={t("tabs.personal")}>
        <div className="flex flex-col gap-4">
          <AvatarUpload
            file={avatarFile}
            existingFileId={avatarRemoved ? null : user.avatarFileId}
            onFileChange={handleAvatarChange}
            displayName={watchedName}
            disabled={isBusy}
            uploadButtonText={t("avatar.upload")}
            deleteButtonText={t("avatar.remove")}
            hintText={t("avatar.hint")}
          />

          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Input
              label={t("fields.displayName")}
              required
              disabled={isBusy}
              placeholder={t("fields.displayNamePlaceholder")}
              error={errors.displayName?.message}
              {...register("displayName")}
            />
            <Input
              label={t("fields.email")}
              type="email"
              disabled
              value={user.email ?? ""}
              readOnly
              placeholder={t("fields.email")}
              helperText={t("fields.emailHint")}
            />
            <Input
              label={t("fields.phoneNumber")}
              type="tel"
              disabled={isBusy}
              placeholder={t("fields.phoneNumberPlaceholder")}
              error={errors.phoneNumber?.message}
              {...register("phoneNumber")}
            />
            <Select
              label={t("fields.preferredLocale")}
              disabled={isBusy}
              options={[
                { value: "", label: t("fields.localeDefault") },
                { value: "th", label: t("fields.localeTh") },
                { value: "en", label: t("fields.localeEn") },
              ]}
              {...register("preferredLocale")}
            />
          </div>
        </div>
      </FormSection>

      <div className="flex justify-end">
        <Button type="submit" isLoading={isBusy} disabled={isBusy || (!isDirty && avatarFile === null && !avatarRemoved)}>
          {t("actions.save")}
        </Button>
      </div>
    </form>
  );
}
