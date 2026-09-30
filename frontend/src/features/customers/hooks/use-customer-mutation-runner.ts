"use client";

import { useLocale, useTranslations } from "next-intl";
import { useQueryClient } from "@tanstack/react-query";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { customerQueryRootKey } from "../api/customer-queries";

export interface CustomerMutationContext {
  token: string;
  membershipId: string;
  locale: "th" | "en";
}

export type CustomerMutationResult<T> =
  | { succeeded: true; value: T }
  | { succeeded: false };

export function useCustomerMutationRunner() {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const localeValue = useLocale();
  const locale: "th" | "en" = localeValue === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const queryClient = useQueryClient();
  const { toast } = useToast();

  return async function runCustomerMutation<T>(
    operation: (context: CustomerMutationContext) => Promise<T>,
  ): Promise<CustomerMutationResult<T>> {
    const token = await getAuthToken();
    if (!token || !membershipId) {
      toast.error(!token ? t("errors.authenticationRequired") : t("errors.membershipRequired"));
      return { succeeded: false };
    }

    try {
      const value = await operation({ token, membershipId, locale });
      await queryClient.invalidateQueries({ queryKey: customerQueryRootKey(membershipId, locale) });
      toast.success(tCommon("feedback.saveSuccess"));
      return { succeeded: true, value };
    } catch (error: unknown) {
      toast.error(error instanceof ApiError ? error.message : t("errors.saveUnexpected"));
      return { succeeded: false };
    }
  };
}
