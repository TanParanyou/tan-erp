"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerSiteList } from "../api/site-queries";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { IconPlus, IconAlertCircle, IconMapPin } from "@/components/common/Icons";

interface SiteListProps {
  customerId: string;
  isCustomerActive: boolean;
}

export function SiteList({ customerId, isCustomerActive }: SiteListProps) {
  const t = useTranslations("sites");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  const hasReadPermission = can(selectedMembership, "sites.read");
  const hasManagePermission = can(selectedMembership, "sites.manage");

  const { data, isLoading, isError, error, refetch } = useCustomerSiteList(customerId);

  if (!hasReadPermission) {
    return null;
  }

  const canCreateSite = isCustomerActive && hasManagePermission;

  return (
    <div className="erp-card p-6 flex flex-col gap-5">
      <div className="flex justify-between items-center flex-wrap gap-4 border-b border-erp-border-subtle pb-3">
        <div className="flex items-center gap-2">
          <h2 className="text-lg font-bold text-erp-navy m-0">
            {t("title")}
          </h2>
          {data?.items && (
            <span className="erp-badge erp-badge-neutral font-mono text-xs">
              {data.items.length}
            </span>
          )}
        </div>

        {canCreateSite && (
          <Button
            href={`/${locale}/customers/${customerId}/sites/create`}
            variant="primary"
            size="sm"
            icon={<IconPlus size={16} />}
            className="min-h-[44px]"
          >
            {t("createSite")}
          </Button>
        )}
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center py-8">
          <MonoSpinner size="md" label={tCommon("states.loading")} aria-busy="true" />
        </div>
      ) : isError ? (
        <div
          role="alert"
          aria-live="polite"
          className="p-4 border border-erp-danger-border bg-erp-danger-bg text-center flex flex-col items-center gap-3"
        >
          <IconAlertCircle size={24} className="text-erp-danger" />
          <p className="text-sm text-erp-danger font-medium m-0">
            {error?.message || t("errors.loadList")}
          </p>
          <Button variant="outline" size="sm" onClick={() => refetch()} className="min-h-[44px]">
            {tCommon("actions.retry")}
          </Button>
        </div>
      ) : !data?.items || data.items.length === 0 ? (
        <div className="text-center py-8 border border-dashed border-erp-border bg-erp-surface-muted p-6">
          <IconMapPin size={32} className="mx-auto text-erp-text-muted mb-2" />
          <h3 className="text-sm font-semibold text-erp-text-main mb-1">
            {t("emptyTitle")}
          </h3>
          <p className="text-xs text-erp-text-muted m-0">
            {t("emptyDetail")}
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="erp-table w-full">
            <thead>
              <tr className="erp-tr">
                <th className="erp-th text-left">{t("label")}</th>
                <th className="erp-th text-left">{t("addressLine1")}</th>
                <th className="erp-th text-left">{t("province")}</th>
                <th className="erp-th text-left">{t("postalCode")}</th>
                <th className="erp-th text-center">{t("status")}</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((site) => (
                <tr key={site.id} className="erp-tr">
                  <td className="erp-td font-semibold text-erp-navy">
                    {site.label}
                  </td>
                  <td className="erp-td">
                    <div>{site.addressLine1}</div>
                  </td>
                  <td className="erp-td">{site.province}</td>
                  <td className="erp-td font-mono">{site.postalCode}</td>
                  <td className="erp-td text-center">
                    <span
                      className={`erp-badge ${
                        site.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
                      }`}
                    >
                      {site.status === "active" ? t("statusActive") : t("statusInactive")}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
