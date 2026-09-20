"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerSiteList } from "../api/site-queries";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { IconPlus, IconAlertCircle, IconMapPin, IconEye, IconFilter } from "@/components/common/Icons";
import { SiteDetailDrawer } from "./site-detail-drawer";
import type { SiteResponse } from "@/lib/api/api-client";

interface SiteListProps {
  customerId: string;
  isCustomerActive: boolean;
}

export function SiteList({ customerId, isCustomerActive }: SiteListProps) {
  const t = useTranslations("sites");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  const [selectedSite, setSelectedSite] = React.useState<SiteResponse | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = React.useState(false);

  // Search and filter states
  const [isFilterVisible, setIsFilterVisible] = React.useState(false);
  const [searchQuery, setSearchQuery] = React.useState("");
  const [statusFilter, setStatusFilter] = React.useState("");
  const [photoFilter, setPhotoFilter] = React.useState("");

  const handleOpenSiteDrawer = (site: SiteResponse) => {
    setSelectedSite(site);
    setIsDrawerOpen(true);
  };

  const hasReadPermission = can(selectedMembership, "sites.read");
  const hasManagePermission = can(selectedMembership, "sites.manage");

  const { data, isLoading, isError, error, refetch } = useCustomerSiteList(customerId);

  const isFiltered = Boolean(searchQuery.trim() || statusFilter || photoFilter);

  const handleResetFilters = () => {
    setSearchQuery("");
    setStatusFilter("");
    setPhotoFilter("");
  };

  const filteredItems = React.useMemo(() => {
    if (!data?.items) return [];
    return data.items.filter((site) => {
      // Status filter
      if (statusFilter && site.status !== statusFilter) {
        return false;
      }
      // Photo filter
      const hasImages = Boolean(site.images && site.images.length > 0);
      if (photoFilter === "with_photos" && !hasImages) {
        return false;
      }
      if (photoFilter === "without_photos" && hasImages) {
        return false;
      }
      // Search query
      if (searchQuery.trim()) {
        const query = searchQuery.toLowerCase().trim();
        const matchLabel = site.label?.toLowerCase().includes(query) ?? false;
        const matchAddress = site.addressLine1?.toLowerCase().includes(query) ?? false;
        const matchSubdistrict = site.subdistrict?.toLowerCase().includes(query) ?? false;
        const matchDistrict = site.district?.toLowerCase().includes(query) ?? false;
        const matchProvince = site.province?.toLowerCase().includes(query) ?? false;
        const matchPostalCode = site.postalCode?.includes(query) ?? false;

        if (
          !matchLabel &&
          !matchAddress &&
          !matchSubdistrict &&
          !matchDistrict &&
          !matchProvince &&
          !matchPostalCode
        ) {
          return false;
        }
      }
      return true;
    });
  }, [data?.items, searchQuery, statusFilter, photoFilter]);

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
              {isFiltered ? `${filteredItems.length} / ${data.items.length}` : data.items.length}
            </span>
          )}
        </div>

        <div className="flex items-center gap-2">
          {data?.items && data.items.length > 0 && (
            <Button
              type="button"
              variant={isFilterVisible || isFiltered ? "primary" : "outline"}
              size="sm"
              icon={<IconFilter size={15} />}
              onClick={() => setIsFilterVisible((prev) => !prev)}
            >
              {tCommon("actions.filter")}
              {isFiltered && !isFilterVisible && (
                <span className="inline-block w-2 h-2 rounded-full bg-erp-gold ml-1" />
              )}
            </Button>
          )}

          {canCreateSite && (
            <Button
              href={`/${locale}/customers/${customerId}/sites/create`}
              variant="primary"
              size="sm"
              icon={<IconPlus size={16} />}
            >
              {t("createSite")}
            </Button>
          )}
        </div>
      </div>

      {/* Filter toolbar (collapsible, shown when isFilterVisible is true or if filtered) */}
      {data?.items && (data.items.length > 0 || isFiltered) && isFilterVisible && (
        <div className="flex flex-wrap items-end gap-3 p-3 bg-erp-surface-subtle border border-erp-border-subtle">
          <ListSearchInput
            id="site-search-input"
            label={t("searchLabel")}
            placeholder={t("searchPlaceholder")}
            value={searchQuery}
            onChange={setSearchQuery}
            onClear={() => setSearchQuery("")}
            widthClassName="w-full sm:w-64"
          />

          <ListFilterSelect
            id="site-status-filter"
            label={t("statusFilter")}
            value={statusFilter}
            onChange={setStatusFilter}
            options={[
              { value: "active", label: t("statusActive") },
              { value: "inactive", label: t("statusInactive") },
            ]}
            widthClassName="w-36"
          />

          <ListFilterSelect
            id="site-photos-filter"
            label={t("hasPhotosFilter")}
            value={photoFilter}
            onChange={setPhotoFilter}
            options={[
              { value: "with_photos", label: t("withPhotos") },
              { value: "without_photos", label: t("withoutPhotos") },
            ]}
            widthClassName="w-36"
          />

          {isFiltered && (
            <div className="pb-0.5">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleResetFilters}
                className="h-10 text-xs font-medium"
              >
                {t("resetFilters")}
              </Button>
            </div>
          )}
        </div>
      )}

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
          <Button variant="outline" size="sm" onClick={() => refetch()}>
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
      ) : filteredItems.length === 0 ? (
        <div className="text-center py-8 border border-dashed border-erp-border bg-erp-surface-muted p-6 flex flex-col items-center gap-2">
          <IconMapPin size={32} className="text-erp-text-muted mb-1" />
          <h3 className="text-sm font-semibold text-erp-text-main m-0">
            {t("filteredEmptyTitle")}
          </h3>
          <p className="text-xs text-erp-text-muted m-0">
            {t("filteredEmptyDetail")}
          </p>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={handleResetFilters}
            className="mt-2"
          >
            {t("resetFilters")}
          </Button>
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
                <th className="erp-th text-center">{t("sitePhotos")}</th>
                <th className="erp-th text-center">{t("status")}</th>
                <th className="erp-th text-center">{tCommon("actions.view")}</th>
              </tr>
            </thead>
            <tbody>
              {filteredItems.map((site) => (
                <tr
                  key={site.id}
                  className="erp-tr hover:bg-erp-surface-subtle cursor-pointer transition-colors"
                  onClick={() => handleOpenSiteDrawer(site)}
                >
                  <td className="erp-td font-semibold text-erp-navy">
                    <button
                      type="button"
                      className="text-left font-semibold text-erp-navy hover:underline focus:outline-none cursor-pointer"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleOpenSiteDrawer(site);
                      }}
                    >
                      {site.label}
                    </button>
                  </td>
                  <td className="erp-td">
                    <div>{site.addressLine1}</div>
                  </td>
                  <td className="erp-td">{site.province}</td>
                  <td className="erp-td font-mono">{site.postalCode}</td>
                  <td className="erp-td text-center">
                    {site.images && site.images.length > 0 ? (
                      <span className="erp-badge erp-badge-navy font-mono text-xs">
                        {t("photoCount", { count: site.images.length })}
                      </span>
                    ) : (
                      <span className="text-xs text-neutral-400">
                        {t("noPhotos")}
                      </span>
                    )}
                  </td>
                  <td className="erp-td text-center">
                    <span
                      className={`erp-badge ${
                        site.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
                      }`}
                    >
                      {site.status === "active" ? t("statusActive") : t("statusInactive")}
                    </span>
                  </td>
                  <td className="erp-td text-center" onClick={(e) => e.stopPropagation()}>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => handleOpenSiteDrawer(site)}
                      icon={<IconEye size={14} className="text-erp-navy" />}
                      className="inline-flex items-center gap-1 text-xs"
                    >
                      {t("viewDetails")}
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Site Detail Drawer */}
      <SiteDetailDrawer
        site={selectedSite}
        isOpen={isDrawerOpen}
        onClose={() => {
          setIsDrawerOpen(false);
          setSelectedSite(null);
        }}
      />
    </div>
  );
}
