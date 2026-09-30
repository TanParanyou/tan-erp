"use client";

import React, { useMemo, useState } from "react";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerSiteList } from "../api/site-queries";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { Button } from "@/components/ui/Button";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { IconPlus, IconMapPin, IconEye, IconFilter } from "@/components/common/Icons";
import { SiteDetailDrawer } from "./site-detail-drawer";
import type { SiteResponse } from "@/lib/api/api-client";

interface SiteListProps {
  customerId: string;
  isCustomerActive: boolean;
  mode?: "manage" | "view";
}

export function SiteList({ customerId, isCustomerActive, mode = "manage" }: SiteListProps) {
  const t = useTranslations("sites");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  const [selectedSite, setSelectedSite] = useState<SiteResponse | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  // Search and filter states
  const [isFilterVisible, setIsFilterVisible] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [photoFilter, setPhotoFilter] = useState("");

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

  const filteredItems = useMemo(() => {
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

  const columns = useMemo<Column<SiteResponse>[]>(
    () => [
      {
        id: "label",
        header: t("label"),
        className: "min-w-[180px]",
        cell: (_val, site) => (
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
        ),
      },
      {
        id: "addressLine1",
        header: t("addressLine1"),
        className: "min-w-[200px]",
        cell: (_val, site) => <div>{site.addressLine1}</div>,
      },
      {
        id: "province",
        header: t("province"),
        className: "min-w-[120px]",
        cell: (_val, site) => <div>{site.province}</div>,
      },
      {
        id: "postalCode",
        header: t("postalCode"),
        className: "min-w-[100px]",
        cell: (_val, site) => <span className="font-mono">{site.postalCode}</span>,
      },
      {
        id: "sitePhotos",
        header: t("sitePhotos"),
        className: "text-center min-w-[120px]",
        cell: (_val, site) =>
          site.images && site.images.length > 0 ? (
            <span className="erp-badge erp-badge-navy font-mono text-xs">
              {t("photoCount", { count: site.images.length })}
            </span>
          ) : (
            <span className="text-xs text-neutral-400">{t("noPhotos")}</span>
          ),
      },
      {
        id: "status",
        header: t("status"),
        className: "text-center min-w-[100px]",
        cell: (_val, site) => (
          <span
            className={`erp-badge ${
              site.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
            }`}
          >
            {site.status === "active" ? t("statusActive") : t("statusInactive")}
          </span>
        ),
      },
      {
        id: "actions",
        header: tCommon("actions.view"),
        isAction: true,
        className: "text-center min-w-[100px]",
        cell: (_val, site) => (
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
        ),
      },
    ],
    [t, tCommon]
  );

  if (!hasReadPermission) {
    return null;
  }

  const canManageSites = mode === "manage" && hasManagePermission;
  const canCreateSite = isCustomerActive && canManageSites;
  const isZeroSites = !isLoading && !isError && (!data?.items || data.items.length === 0);

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

      {/* Main Content Area: Table-Preserved Architecture */}
      {isZeroSites ? (
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
        <DataTable<SiteResponse>
          columns={columns}
          data={filteredItems}
          isLoading={isLoading}
          isError={isError}
          error={error?.message || t("errors.loadList")}
          onRetry={() => { void refetch(); }}
          emptyTitle={t("filteredEmptyTitle")}
          emptyDescription={t("filteredEmptyDetail")}
          hidePagination={true}
        />
      )}

      {/* Site Detail Drawer */}
      <SiteDetailDrawer
        site={selectedSite}
        isOpen={isDrawerOpen}
        canManage={canManageSites}
        onUpdated={() => { void refetch(); }}
        onClose={() => {
          setIsDrawerOpen(false);
          setSelectedSite(null);
        }}
      />
    </div>
  );
}
