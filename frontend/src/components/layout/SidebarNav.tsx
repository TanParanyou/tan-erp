"use client";

import React, { useState, useMemo } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useTranslations, useLocale } from "next-intl";
import {
  IconHome,
  IconUsers,
  IconBriefcase,
  IconBox3D,
  IconGrid,
  IconBuilding,
  IconFileText,
  IconSearch,
  IconClose,
  IconChevronDown,
  IconChevronsUpDown,
} from "@/components/common/Icons";
import { Tooltip } from "@/components/ui/Tooltip";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { can } from "@/lib/permissions/can";

export interface SidebarNavProps {
  currentUser: CurrentUserResponse;
  isCollapsed: boolean;
  onLinkClick?: () => void;
}

interface NavItemDef {
  id: string;
  href: string;
  label: string;
  icon: React.ReactNode;
  isActive: boolean;
  permission?: boolean;
}

interface NavSubgroupDef {
  id: string;
  label: string;
  items: NavItemDef[];
  permission?: boolean;
}

export function SidebarNav({ currentUser, isCollapsed, onLinkClick }: SidebarNavProps) {
  const tShell = useTranslations("shell");
  const locale = useLocale();
  const pathname = usePathname();

  const memberships = currentUser.memberships || [];
  const activeMembership = memberships[0];

  const hasCustomersRead = can(activeMembership, "customers.read");
  const hasProjectsRead = can(activeMembership, "projects.read");
  const hasSuppliersRead = can(activeMembership, "suppliers.read");
  const hasWarehousesRead = can(activeMembership, "warehouses.read");
  const hasInventoryRead = can(activeMembership, "inventory.read");
  const hasPurchaseOrdersRead = can(activeMembership, "purchase-orders.read");
  const hasOpportunitiesRead = can(activeMembership, "opportunities.read");
  const hasEstimateApproval = can(activeMembership, "estimates.approve");
  const hasSettingsRead =
    can(activeMembership, "document-sequences.read") || can(activeMembership, "organizations.read");
  const hasUsersRead = can(activeMembership, "users.read");
  const hasRoleRequestApproval = can(activeMembership, "roles.assign-approval");
  const hasItemMasterRead = can(activeMembership, "items.read");
  const hasCostSourcesRead = can(activeMembership, "cost-sources.read");
  const hasCostReviewRead = can(activeMembership, "cost-records.read");

  const [searchQuery, setSearchQuery] = useState("");

  const isHomeActive = pathname === `/${locale}` || pathname === `/${locale}/`;
  const isCustomersActive = pathname.startsWith(`/${locale}/customers`);
  const isOpportunitiesActive = pathname.startsWith(`/${locale}/opportunities`);
  const isProjectsActive = pathname.startsWith(`/${locale}/projects`);
  const isProcurementActive = pathname.startsWith(`/${locale}/procurement`);
  const isInventoryActive = pathname.startsWith(`/${locale}/inventory`);
  const isEstimateReviewActive = pathname.startsWith(`/${locale}/estimates/review-queue`);
  const isDocumentNumberingActive = pathname.startsWith(`/${locale}/settings/document-numbering`);
  const isUserAdminActive = pathname.startsWith(`/${locale}/settings/users`);
  const isRoleRequestsActive = pathname.startsWith(`/${locale}/settings/role-requests`);
  const isItemMasterActive = pathname.startsWith(`/${locale}/item-master`);

  const crmItems: NavItemDef[] = useMemo(() => {
    const items: NavItemDef[] = [];
    if (hasCustomersRead) {
      items.push({
        id: "customers",
        href: `/${locale}/customers`,
        label: tShell("customers"),
        icon: <IconUsers size={20} />,
        isActive: isCustomersActive,
        permission: hasCustomersRead,
      });
    }
    if (hasOpportunitiesRead) {
      items.push({
        id: "opportunities",
        href: `/${locale}/opportunities`,
        label: tShell("opportunities"),
        icon: <IconBriefcase size={20} />,
        isActive: isOpportunitiesActive,
        permission: hasOpportunitiesRead,
      });
    }
    if (hasProjectsRead) {
      items.push({
        id: "projects",
        href: `/${locale}/projects`,
        label: tShell("projects"),
        icon: <IconGrid size={20} />,
        isActive: isProjectsActive,
        permission: hasProjectsRead,
      });
    }
    if (hasEstimateApproval) {
      items.push({ id: "estimate-reviews", href: `/${locale}/estimates/review-queue`, label: tShell("estimateReviews"), icon: <IconFileText size={20} />, isActive: isEstimateReviewActive, permission: hasEstimateApproval });
    }
    return items;
  }, [hasCustomersRead, hasOpportunitiesRead, hasProjectsRead, hasEstimateApproval, locale, tShell, isCustomersActive, isOpportunitiesActive, isProjectsActive, isEstimateReviewActive]);

  const productItems: NavItemDef[] = useMemo(() => {
    const items: NavItemDef[] = [];
    if (hasItemMasterRead) {
      items.push({
        id: "item-master",
        href: `/${locale}/item-master`,
        label: tShell("itemMaster"),
        icon: <IconBox3D size={20} />,
        isActive:
          isItemMasterActive &&
          !pathname.includes("reference-data") &&
          !pathname.includes("cost-sources") &&
          !pathname.includes("cost-reviews"),
        permission: hasItemMasterRead,
      });
      items.push({
        id: "item-reference-data",
        href: `/${locale}/item-master/reference-data`,
        label: tShell("itemReferenceData"),
        icon: <IconGrid size={20} />,
        isActive: pathname.includes("reference-data"),
        permission: hasItemMasterRead,
      });
    }
    if (hasCostSourcesRead) {
      items.push({
        id: "item-cost-sources",
        href: `/${locale}/item-master/cost-sources`,
        label: tShell("itemCostSources"),
        icon: <IconBuilding size={20} />,
        isActive: pathname.includes("cost-sources"),
        permission: hasCostSourcesRead,
      });
    }
    if (hasCostReviewRead) {
      items.push({
        id: "item-cost-reviews",
        href: `/${locale}/item-master/cost-reviews`,
        label: tShell("costReviews"),
        icon: <IconFileText size={20} />,
        isActive: pathname.includes("cost-reviews"),
        permission: hasCostReviewRead,
      });
    }
    return items;
  }, [hasItemMasterRead, hasCostSourcesRead, hasCostReviewRead, locale, pathname, isItemMasterActive, tShell]);

  const procurementItems: NavItemDef[] = useMemo(() => {
    const items: NavItemDef[] = [];
    if (hasSuppliersRead) {
      items.push({
        id: "suppliers",
        href: `/${locale}/procurement/suppliers`,
        label: tShell("suppliers"),
        icon: <IconBuilding size={20} />,
        isActive: pathname.startsWith(`/${locale}/procurement/suppliers`),
        permission: hasSuppliersRead,
      });
    }
    if (hasPurchaseOrdersRead) {
      items.push({
        id: "purchase-orders",
        href: `/${locale}/procurement/purchase-orders`,
        label: tShell("purchaseOrders"),
        icon: <IconFileText size={20} />,
        isActive: pathname.startsWith(`/${locale}/procurement/purchase-orders`),
        permission: hasPurchaseOrdersRead,
      });
    }
    return items;
  }, [hasSuppliersRead, hasPurchaseOrdersRead, locale, pathname, tShell]);

  const inventoryItems: NavItemDef[] = useMemo(() => {
    const items: NavItemDef[] = [];
    if (hasInventoryRead) {
      items.push({
        id: "stock",
        href: `/${locale}/inventory/stock`,
        label: tShell("stockBalances"),
        icon: <IconBox3D size={20} />,
        isActive: pathname.startsWith(`/${locale}/inventory/stock`),
        permission: hasInventoryRead,
      });
      items.push({
        id: "stock-movements",
        href: `/${locale}/inventory/movements`,
        label: tShell("stockMovements"),
        icon: <IconFileText size={20} />,
        isActive: pathname.startsWith(`/${locale}/inventory/movements`),
        permission: hasInventoryRead,
      });
    }
    if (hasWarehousesRead) {
      items.push({
        id: "warehouses",
        href: `/${locale}/inventory/warehouses`,
        label: tShell("warehouses"),
        icon: <IconBuilding size={20} />,
        isActive: pathname.startsWith(`/${locale}/inventory/warehouses`),
        permission: hasWarehousesRead,
      });
    }
    return items;
  }, [hasInventoryRead, hasWarehousesRead, locale, pathname, tShell]);

  const subgroups: NavSubgroupDef[] = useMemo(() => {
    const groups: NavSubgroupDef[] = [];
    if (crmItems.length > 0) {
      groups.push({
        id: "crm",
        label: tShell("crmSubgroup"),
        items: crmItems,
      });
    }
    if (procurementItems.length > 0) {
      groups.push({
        id: "procurement",
        label: tShell("procurementSubgroup"),
        items: procurementItems,
      });
    }
    if (inventoryItems.length > 0) {
      groups.push({
        id: "inventory",
        label: tShell("inventorySubgroup"),
        items: inventoryItems,
      });
    }
    if (productItems.length > 0) {
      groups.push({
        id: "products",
        label: tShell("productSubgroup"),
        items: productItems,
      });
    }
    return groups;
  }, [crmItems, procurementItems, inventoryItems, productItems, tShell]);

  // Accordion open/close state tracking
  const [openSubgroups, setOpenSubgroups] = useState<Record<string, boolean>>(() => {
    const initial: Record<string, boolean> = {};
    if (isCustomersActive || isOpportunitiesActive || isProjectsActive) initial["crm"] = true;
    if (isProcurementActive) initial["procurement"] = true;
    if (isInventoryActive) initial["inventory"] = true;
    if (isItemMasterActive) initial["products"] = true;
    // Default open all if none active
    if (Object.keys(initial).length === 0) {
      initial["crm"] = true;
      initial["procurement"] = true;
      initial["inventory"] = true;
      initial["products"] = true;
    }
    return initial;
  });

  const toggleSubgroup = (groupId: string) => {
    setOpenSubgroups((prev) => ({
      ...prev,
      [groupId]: !prev[groupId],
    }));
  };

  const isAnySubgroupOpen = useMemo(() => {
    return subgroups.some((g) => openSubgroups[g.id]);
  }, [subgroups, openSubgroups]);

  const handleToggleAll = () => {
    if (isAnySubgroupOpen) {
      // Collapse all
      const next: Record<string, boolean> = {};
      subgroups.forEach((g) => {
        next[g.id] = false;
      });
      setOpenSubgroups(next);
    } else {
      // Expand all
      const next: Record<string, boolean> = {};
      subgroups.forEach((g) => {
        next[g.id] = true;
      });
      setOpenSubgroups(next);
    }
  };

  // Search filtering
  const trimmedSearch = searchQuery.trim().toLowerCase();

  const isHomeVisible = !trimmedSearch || tShell("home").toLowerCase().includes(trimmedSearch);
  const isDocNumberingVisible =
    hasSettingsRead &&
    (!trimmedSearch || tShell("documentNumbering").toLowerCase().includes(trimmedSearch));

  const isUserAdminVisible =
    hasUsersRead && (!trimmedSearch || tShell("userAdmin").toLowerCase().includes(trimmedSearch));
  const isRoleRequestsVisible =
    hasRoleRequestApproval && (!trimmedSearch || tShell("roleRequests").toLowerCase().includes(trimmedSearch));

  const filteredSubgroups = useMemo(() => {
    if (!trimmedSearch) return subgroups;
    return subgroups
      .map((g) => {
        const matchingItems = g.items.filter((item) =>
          item.label.toLowerCase().includes(trimmedSearch)
        );
        return {
          ...g,
          items: matchingItems,
        };
      })
      .filter((g) => g.items.length > 0);
  }, [subgroups, trimmedSearch]);

  const hasAnyResults =
    isHomeVisible ||
    isDocNumberingVisible ||
    isUserAdminVisible ||
    isRoleRequestsVisible ||
    filteredSubgroups.some((g) => g.items.length > 0);

  const renderLink = (item: NavItemDef) => {
    const linkContent = (
      <Link
        href={item.href}
        onClick={onLinkClick}
        className={`erp-nav-link ${item.isActive ? "erp-nav-link-active" : ""}`}
        title={item.label}
        aria-current={item.isActive ? "page" : undefined}
      >
        {item.icon}
        <span className="erp-nav-text">{item.label}</span>
      </Link>
    );

    if (isCollapsed) {
      return (
        <Tooltip content={item.label} position="right" delayMs={100} key={item.id}>
          {linkContent}
        </Tooltip>
      );
    }

    return <React.Fragment key={item.id}>{linkContent}</React.Fragment>;
  };

  return (
    <>
      {/* Quick Search & Toggle All Controls (Expanded Mode Only) */}
      {!isCollapsed && (
        <div className="erp-nav-controls">
          <div className="erp-nav-search-wrapper">
            <span className="erp-nav-search-icon" aria-hidden="true">
              <IconSearch size={14} />
            </span>
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder={tShell("searchMenuPlaceholder")}
              className="erp-nav-search-input"
              aria-label={tShell("searchMenuPlaceholder")}
            />
            {searchQuery && (
              <button
                type="button"
                onClick={() => setSearchQuery("")}
                className="erp-nav-search-clear"
                aria-label={tShell("clearSearch")}
                title={tShell("clearSearch")}
              >
                <IconClose size={14} />
              </button>
            )}
          </div>

          {subgroups.length > 0 && !trimmedSearch && (
            <button
              type="button"
              onClick={handleToggleAll}
              className="erp-nav-toggle-all-btn"
              aria-label={isAnySubgroupOpen ? tShell("collapseAll") : tShell("expandAll")}
              title={isAnySubgroupOpen ? tShell("collapseAll") : tShell("expandAll")}
            >
              <span>{isAnySubgroupOpen ? tShell("collapseAll") : tShell("expandAll")}</span>
              <IconChevronsUpDown size={14} />
            </button>
          )}
        </div>
      )}

      {/* Main Navigation Tree */}
      <nav className="erp-nav-tree" aria-label={tShell("mainNavigation")}>
        <ul className="erp-nav-root-list">
          {/* Home Link */}
          {isHomeVisible && (
            <li>
              {renderLink({
                id: "home",
                href: `/${locale}`,
                label: tShell("home"),
                icon: <IconHome size={20} />,
                isActive: isHomeActive,
              })}
            </li>
          )}

          {/* Subgroups */}
          {filteredSubgroups.map((group) => {
            const hasActiveChild = group.items.some((item) => item.isActive);
            const isOpen = trimmedSearch ? true : Boolean(openSubgroups[group.id]);

            return (
              <li key={group.id}>
                <div className="erp-nav-divider" />
                {!isCollapsed && (
                  <button
                    type="button"
                    onClick={() => toggleSubgroup(group.id)}
                    className="erp-nav-accordion-header"
                    aria-expanded={isOpen}
                    data-has-active-child={hasActiveChild}
                    aria-label={group.label}
                  >
                    <span>{group.label}</span>
                    <span
                      className="erp-nav-accordion-chevron"
                      data-expanded={isOpen}
                      aria-hidden="true"
                    >
                      <IconChevronDown size={16} />
                    </span>
                  </button>
                )}

                {(isOpen || isCollapsed) && (
                  <div className="erp-nav-subgroup-container">
                    <ul className="erp-nav-subgroup-list">
                      {group.items.map((item) => (
                        <li key={item.id}>{renderLink(item)}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </li>
            );
          })}

          {/* Document Numbering (Settings) */}
          {isDocNumberingVisible && (
            <li>
              <div className="erp-nav-divider" />
              {renderLink({
                id: "doc-numbering",
                href: `/${locale}/settings/document-numbering`,
                label: tShell("documentNumbering"),
                icon: <IconFileText size={20} />,
                isActive: isDocumentNumberingActive,
              })}
            </li>
          )}

          {/* User and role administration (Settings) */}
          {isUserAdminVisible && (
            <li>
              {renderLink({
                id: "user-admin",
                href: `/${locale}/settings/users`,
                label: tShell("userAdmin"),
                icon: <IconUsers size={20} />,
                isActive: isUserAdminActive,
              })}
            </li>
          )}
          {isRoleRequestsVisible && (
            <li>
              {renderLink({
                id: "role-requests",
                href: `/${locale}/settings/role-requests`,
                label: tShell("roleRequests"),
                icon: <IconFileText size={20} />,
                isActive: isRoleRequestsActive,
              })}
            </li>
          )}

          {/* Empty search feedback */}
          {trimmedSearch && !hasAnyResults && (
            <li className="erp-nav-empty-search">
              <p>{tShell("noMenuFound")}</p>
            </li>
          )}
        </ul>
      </nav>
    </>
  );
}
