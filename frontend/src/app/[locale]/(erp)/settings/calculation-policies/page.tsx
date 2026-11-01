"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { CalculationPolicyList } from "@/features/estimates/components/calculation-policy-list";

export default function CalculationPolicySettingsPage() {
  const t = useTranslations("estimates.calculationPolicies");

  return (
    <PermissionGuard permission={PERMISSIONS.ESTIMATES_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <CalculationPolicyList />
    </PermissionGuard>
  );
}
