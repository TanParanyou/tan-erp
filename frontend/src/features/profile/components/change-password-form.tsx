"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useQueryClient } from "@tanstack/react-query";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import { changeOwnPassword } from "@/lib/auth/auth-session";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import { passwordErrorKey } from "../profile-errors";
import { securityKeys } from "../api/security-queries";

const MIN_PASSWORD_LENGTH = 8;

interface ChangePasswordValues {
  currentPassword?: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ChangePasswordFormProps {
  hasPassword?: boolean;
}

export function ChangePasswordForm({ hasPassword = true }: ChangePasswordFormProps) {
  const t = useTranslations("myProfile");
  const { toast } = useToast();
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const schema = z
    .object({
      currentPassword: z.string().optional(),
      newPassword: z.string().min(MIN_PASSWORD_LENGTH, t("validation.newPasswordTooShort")),
      confirmPassword: z.string(),
    })
    .refine((v) => !hasPassword || Boolean(v.currentPassword && v.currentPassword.trim().length > 0), {
      path: ["currentPassword"],
      message: t("validation.currentPasswordRequired"),
    })
    .refine((v) => v.newPassword === v.confirmPassword, {
      path: ["confirmPassword"],
      message: t("validation.passwordMismatch"),
    })
    .refine((v) => !hasPassword || !v.currentPassword || v.newPassword !== v.currentPassword, {
      path: ["newPassword"],
      message: t("validation.passwordSameAsCurrent"),
    });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ChangePasswordValues>({
    resolver: zodResolver(schema),
    defaultValues: { currentPassword: "", newPassword: "", confirmPassword: "" },
  });

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    setIsSubmitting(true);
    try {
      await changeOwnPassword(hasPassword ? (values.currentPassword || null) : null, values.newPassword);
      toast.success(t("passwordSuccess"));
      reset();
      void queryClient.invalidateQueries({ queryKey: securityKeys.status(context.membershipId) });
    } catch (error: unknown) {
      setSubmitError(t(`errors.${passwordErrorKey(error)}`));
    } finally {
      setIsSubmitting(false);
    }
  });

  return (
    <form onSubmit={onSubmit} noValidate className="flex flex-col gap-6">
      {submitError && <Alert variant="danger">{submitError}</Alert>}

      <FormSection title={t("securityTitle")} description={t("securityHint")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {hasPassword && (
            <Input
              label={t("fields.currentPassword")}
              type="password"
              autoComplete="current-password"
              required
              disabled={isSubmitting}
              placeholder={t("fields.currentPasswordPlaceholder")}
              error={errors.currentPassword?.message}
              wrapperClassName="md:col-span-2"
              {...register("currentPassword")}
            />
          )}
          <Input
            label={t("fields.newPassword")}
            type="password"
            autoComplete="new-password"
            required
            disabled={isSubmitting}
            placeholder={t("fields.newPasswordPlaceholder")}
            error={errors.newPassword?.message}
            {...register("newPassword")}
          />
          <Input
            label={t("fields.confirmPassword")}
            type="password"
            autoComplete="new-password"
            required
            disabled={isSubmitting}
            placeholder={t("fields.confirmPasswordPlaceholder")}
            error={errors.confirmPassword?.message}
            {...register("confirmPassword")}
          />
        </div>
      </FormSection>

      <div className="flex justify-end">
        <Button type="submit" isLoading={isSubmitting} disabled={isSubmitting}>
          {hasPassword ? t("actions.changePassword") : t("actions.setPassword")}
        </Button>
      </div>
    </form>
  );
}
