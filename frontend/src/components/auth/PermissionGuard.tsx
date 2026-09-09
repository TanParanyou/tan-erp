"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, type PermissionKey } from "@/lib/permissions/can";

export interface PermissionGuardProps {
  /** Single permission key or array of permission keys required */
  permission: PermissionKey | PermissionKey[];
  /** If multiple permissions provided, require all (true) or any (false). Default: true */
  requireAll?: boolean;
  /** Custom fallback component when permission is denied */
  fallback?: React.ReactNode;
  /** Custom title for default Access Denied card */
  title?: string;
  /** Custom detail message for default Access Denied card */
  detail?: string;
  /** Children to render when access is granted */
  children: React.ReactNode;
}

/**
 * Atelier Architectural Navy Sharp - Unified Permission Guard
 * Enforces client-side RBAC authorization for ERP features and views.
 */
export function PermissionGuard({
  permission,
  requireAll = true,
  fallback,
  title,
  detail,
  children,
}: PermissionGuardProps) {
  const { selectedMembership } = useSelectedMembership();
  const t = useTranslations("common.errors");

  const permissions = Array.isArray(permission) ? permission : [permission];
  const hasAccess = requireAll
    ? permissions.every((perm) => can(selectedMembership, perm))
    : permissions.some((perm) => can(selectedMembership, perm));

  if (!hasAccess) {
    if (fallback !== undefined) {
      return <>{fallback}</>;
    }

    return (
      <div
        role="alert"
        aria-live="polite"
        className="erp-card max-w-[480px] mx-auto my-8 p-8 text-center border-[var(--erp-warning-border)] bg-[var(--erp-warning-bg)]"
      >
        <div className="flex items-center justify-center gap-2 mb-2">
          <svg
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="var(--erp-warning)"
            strokeWidth="2"
            strokeLinecap="square"
            aria-hidden="true"
            className="shrink-0"
          >
            <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
            <line x1="12" y1="9" x2="12" y2="13" />
            <line x1="12" y1="17" x2="12.01" y2="17" />
          </svg>
          <h2 className="text-lg font-bold text-[var(--erp-warning)] m-0">
            {title || t("accessDeniedTitle")}
          </h2>
        </div>
        <p className="text-sm text-[var(--erp-warning-text)] m-0 leading-relaxed">
          {detail || t("accessDeniedDetail")}
        </p>
      </div>
    );
  }

  return <>{children}</>;
}
