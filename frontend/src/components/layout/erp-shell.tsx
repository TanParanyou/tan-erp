"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useRouter, usePathname } from "next/navigation";
import { useQueryClient } from "@tanstack/react-query";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { signOutSession } from "@/lib/auth/auth-session";
import { useTranslations, useLocale } from "next-intl";
import {
  IconClose,
  IconMenu,
  IconHome,
  IconUsers,
  IconUser,
  IconChevronLeft,
  IconChevronRight,
  IconChevronDown,
  IconLogOut,
  IconGlobe,
  IconBriefcase,
  IconBuilding,
  IconGitBranch,
  IconFileText,
  IconSun,
  IconMoon,
  IconMonitor,
} from "@/components/common/Icons";
import { LogoIcon } from "@/components/common/Logo";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { Button } from "@/components/ui/Button";
import { DropdownMenu, type DropdownMenuItem } from "@/components/ui/DropdownMenu";
import { useTheme, type ThemeMode } from "@/providers/theme-provider";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import { Avatar } from "@/components/ui/Avatar";
import { SidebarNav } from "./SidebarNav";
import { useSaveThemePreferences } from "@/features/profile/api/profile-queries";
import { NotificationBell } from "./NotificationBell";
import { cn } from "@/lib/utils/cn";

interface ErpShellProps {
  currentUser: CurrentUserResponse;
  children?: React.ReactNode;
}

export function ErpShell({ currentUser, children }: ErpShellProps) {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useLocalStorage<boolean>(
    "tan_erp_sidebar_collapsed",
    false
  );
  const tShell = useTranslations("shell");
  const tAuth = useTranslations("auth");
  const tApp = useTranslations("app");
  const tProfile = useTranslations("myProfile");
  const { theme, resolvedTheme, setTheme } = useTheme();
  const { save: saveThemePreferences, isPending: isSavingTheme } = useSaveThemePreferences();
  const locale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const queryClient = useQueryClient();

  const { selectedMembership } = useSelectedMembership();

  const user = currentUser.user;
  const memberships = currentUser.memberships || [];
  const activeMembership = selectedMembership || memberships[0];
  const orgName = activeMembership?.organization?.name || "-";
  const brandLogoFileId = activeMembership?.organization?.branding?.logoFileId ?? null;
  const branchName = activeMembership?.branch?.name || tShell("noBranch");

  const hasCustomersRead = can(activeMembership, "customers.read");
  const hasOpportunitiesRead = can(activeMembership, "opportunities.read");
  const hasSettingsRead = can(activeMembership, "document-sequences.read") || can(activeMembership, "organizations.read");
  const hasItemMasterRead = can(activeMembership, "items.read");
  const hasCostSourcesRead = can(activeMembership, "cost-sources.read");
  const hasCostReviewRead = can(activeMembership, "cost-records.read");

  const targetLocale = locale === "th" ? "en" : "th";

  const handleSelectLocale = async (newLocale: "th" | "en") => {
    if (newLocale === locale || isSavingTheme) return;
    await saveThemePreferences({ preferredLocale: newLocale });
    router.push(`/${newLocale}`);
  };

  const handleSelectTheme = (mode: ThemeMode) => {
    setTheme(mode);
    void saveThemePreferences({ themeMode: mode });
  };

  const handleSignOut = async () => {
    await signOutSession(queryClient);
    router.push(`/${locale}/login`);
  };

  const isHomeActive = pathname === `/${locale}` || pathname === `/${locale}/`;
  const isCustomersActive = pathname.startsWith(`/${locale}/customers`);
  const isOpportunitiesActive = pathname.startsWith(`/${locale}/opportunities`);
  const isDocumentNumberingActive = pathname.startsWith(`/${locale}/settings/document-numbering`);
  const isItemMasterActive = pathname.startsWith(`/${locale}/item-master`);

  return (
    <div style={{ display: "flex", flexDirection: "column", minHeight: "100vh", backgroundColor: "var(--erp-canvas)" }}>
      {/* Architectural Shell Header */}
      <header
        role="banner"
        className="erp-header"
      >
        <div className="erp-header-left">
          {/* Responsive Sidebar/Drawer toggle */}
          <button
            type="button"
            className="erp-header-toggle-btn"
            onClick={() => {
              if (typeof window !== "undefined" && window.innerWidth <= 768) {
                setIsMobileMenuOpen((prev) => !prev);
              } else {
                setIsSidebarCollapsed((prev) => !prev);
              }
            }}
            aria-label={tShell("toggleMenu")}
            aria-expanded={isMobileMenuOpen}
            title={tShell("toggleMenu")}
            style={{ minHeight: "44px", minWidth: "44px" }}
          >
            <IconMenu size={20} />
          </button>

          <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
            {brandLogoFileId ? (
              <span className="flex h-8 shrink-0 items-center bg-erp-surface px-1">
                <AuthenticatedFileImage fileId={brandLogoFileId} alt={orgName} className="h-6 w-auto max-w-[120px] object-contain" />
              </span>
            ) : (
              <LogoIcon size={26} color="var(--erp-text-on-navy)" strokeWidth={2} style={{ flexShrink: 0 }} />
            )}
            <span className="erp-header-title">
              {tApp("title")}
            </span>
          </div>

          <div className="erp-header-context-info">
            {/* eslint-disable-next-line no-restricted-syntax -- token-exempt: no semantic token for gray-200 header text */}
            <span style={{ color: "#E5E7EB" }}>
              <strong>{tShell("organization")}:</strong>{" "}
              <span data-testid="org-name">{orgName}</span>
            </span>
            <span style={{ color: "color-mix(in srgb, var(--erp-text-on-navy) 40%, transparent)" }}>|</span>
            {/* eslint-disable-next-line no-restricted-syntax -- token-exempt: no semantic token for gray-200 header text */}
            <span style={{ color: "#E5E7EB" }}>
              <strong>{tShell("branch")}:</strong>{" "}
              <span data-testid="branch-name">{branchName}</span>
            </span>
          </div>
        </div>

        <div className="erp-header-right">
          {/* In-app notifications (renders nothing without a selected membership) */}
          <NotificationBell />

          {/* Language Selection Dropdown */}
          <DropdownMenu
            align="right"
            menuClassName="w-56 shadow-xl border-erp-border bg-erp-surface"
            trigger={
              <button
                type="button"
                className="erp-header-lang-btn"
                style={{ minHeight: "44px" }}
                aria-label={tShell("language")}
                title={tShell("language")}
              >
                <IconGlobe size={16} />
                <span className="erp-lang-full font-medium">{locale === "th" ? "ไทย" : "EN"}</span>
                <span className="erp-lang-short font-medium">{locale.toUpperCase()}</span>
                <IconChevronDown size={12} className="opacity-70 shrink-0" />
              </button>
            }
            items={[
              {
                key: "lang-th",
                label: "ภาษาไทย (TH)",
                description: "ไทย",
                badge: locale === "th" ? (
                  <span className="text-[10px] text-erp-navy font-bold uppercase bg-erp-navy/10 px-1 py-0.5">
                    ✓
                  </span>
                ) : undefined,
                onClick: () => void handleSelectLocale("th"),
                disabled: isSavingTheme,
              },
              {
                key: "lang-en",
                label: "English (EN)",
                description: "English",
                badge: locale === "en" ? (
                  <span className="text-[10px] text-erp-navy font-bold uppercase bg-erp-navy/10 px-1 py-0.5">
                    ✓
                  </span>
                ) : undefined,
                onClick: () => void handleSelectLocale("en"),
                disabled: isSavingTheme,
              },
              {
                key: "lang-my",
                label: "မြန်မာ (Myanmar)",
                description: "မြန်မာဘာသာ",
                badge: (
                  <span className="text-[10px] text-erp-text-muted border border-erp-border-subtle bg-erp-surface-subtle px-1.5 py-0.5">
                    {tShell("comingSoon")}
                  </span>
                ),
                onClick: () => {},
                disabled: true,
              },
              {
                key: "lang-km",
                label: "ភាសាខ្មែរ (Khmer)",
                description: "ភាសាខ្មែរ",
                badge: (
                  <span className="text-[10px] text-erp-text-muted border border-erp-border-subtle bg-erp-surface-subtle px-1.5 py-0.5">
                    {tShell("comingSoon")}
                  </span>
                ),
                onClick: () => {},
                disabled: true,
              },
            ]}
          />

          {/* User Account Dropdown Menu */}
          <DropdownMenu
            align="right"
            menuClassName="w-80 shadow-xl border-erp-border bg-erp-surface"
            headerContent={({ close }) => (
              <div className="flex flex-col gap-3">
                <Link
                  href={`/${locale}/profile`}
                  onClick={close}
                  className="flex items-center gap-3 p-1.5 -m-1.5 hover:bg-erp-navy/5 transition-colors cursor-pointer group"
                  aria-label={tProfile("openProfile")}
                  title={tProfile("openProfile")}
                >
                  <Avatar
                    size="md"
                    fileId={user?.avatarFileId}
                    initial={user?.displayName || user?.email || undefined}
                    alt={user?.displayName || user?.email || tShell("user")}
                    variant="outline"
                    className="border-erp-border shrink-0 group-hover:border-erp-navy transition-colors"
                  />
                  <div className="flex flex-col min-w-0">
                    <span className="text-xs font-semibold text-erp-text-main group-hover:text-erp-navy transition-colors truncate">
                      {user?.displayName || user?.email || tShell("user")}
                    </span>
                    {user?.displayName && user?.email && (
                      <span className="text-[11px] text-erp-text-muted truncate">
                        {user.email}
                      </span>
                    )}
                    <span className="text-[10px] text-erp-navy font-semibold uppercase tracking-wider mt-0.5">
                      {branchName}
                    </span>
                  </div>
                </Link>

                {/* Theme Mode Segmented Toggle (Light / Dark / System) */}
                <div className="pt-2 border-t border-erp-border-subtle">
                  <div className="text-[11px] font-medium text-erp-text-muted mb-1.5 flex items-center justify-between">
                    <span>{tShell("theme")}</span>
                  </div>
                  <div className="erp-header-theme-segment">
                    <button
                      type="button"
                      disabled={isSavingTheme}
                      onClick={() => handleSelectTheme("light")}
                      className={cn(
                        "erp-header-theme-tab flex-1",
                        theme === "light" && "active"
                      )}
                      title={tShell("lightMode")}
                    >
                      <IconSun size={14} className={cn(theme === "light" ? "text-amber-300" : "text-erp-text-muted")} />
                      <span>{tShell("lightMode")}</span>
                    </button>
                    <button
                      type="button"
                      disabled={isSavingTheme}
                      onClick={() => handleSelectTheme("dark")}
                      className={cn(
                        "erp-header-theme-tab flex-1",
                        theme === "dark" && "active"
                      )}
                      title={tShell("darkMode")}
                    >
                      <IconMoon size={14} className={cn(theme === "dark" ? "text-amber-300" : "text-erp-text-muted")} />
                      <span>{tShell("darkMode")}</span>
                    </button>
                    <button
                      type="button"
                      disabled={isSavingTheme}
                      onClick={() => handleSelectTheme("system")}
                      className={cn(
                        "erp-header-theme-tab flex-1",
                        theme === "system" && "active"
                      )}
                      title={tShell("systemMode")}
                    >
                      <IconMonitor size={14} className={cn(theme === "system" ? "text-white" : "text-erp-text-muted")} />
                      <span>{tShell("systemMode")}</span>
                    </button>
                  </div>
                </div>
              </div>
            )}
            trigger={
              <button
                type="button"
                className="erp-header-user-btn"
                aria-label={tProfile("openProfile")}
                title={tProfile("openProfile")}
              >
                <div className="erp-header-user-info">
                  <div className="flex flex-col items-end text-right min-w-0">
                    <span style={{ fontWeight: 600, color: "var(--erp-text-on-navy)", maxWidth: "100%", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                      {user?.displayName || user?.email || tShell("user")}
                    </span>
                    {user?.displayName && user?.email && (
                      // eslint-disable-next-line no-restricted-syntax -- token-exempt: no semantic token for gray-300 header text
                      <span style={{ color: "#D1D5DB", fontSize: "0.75rem", maxWidth: "100%", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                        {user.email}
                      </span>
                    )}
                  </div>
                  <Avatar
                    size="sm"
                    fileId={user?.avatarFileId}
                    initial={user?.displayName || user?.email || undefined}
                    alt={user?.displayName || user?.email || tShell("user")}
                    variant="outline"
                    className="border-white/25 bg-white/10 text-white shrink-0"
                  />
                </div>
                <IconChevronDown size={14} className="text-white/70 shrink-0" />
              </button>
            }
            items={[
              {
                key: "logout",
                label: tAuth("logout"),
                icon: <IconLogOut size={16} />,
                variant: "danger",
                onClick: handleSignOut,
              },
            ]}
          />
        </div>
      </header>

      {/* Main Workspace Layout */}
      <div style={{ display: "flex", flex: 1, position: "relative" }}>
        {/* Mobile Backdrop Overlay */}
        {isMobileMenuOpen && (
          <div
            className="erp-sidebar-backdrop"
            onClick={() => setIsMobileMenuOpen(false)}
            aria-hidden="true"
          />
        )}

        {/* Navigation Sidebar / Mobile Drawer */}
        <aside
          className={`erp-sidebar ${isMobileMenuOpen ? "erp-sidebar-open" : ""} ${isSidebarCollapsed ? "erp-sidebar-collapsed" : ""}`}
        >
          {/* Mobile Drawer Header */}
          <div className="erp-drawer-header">
            <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="var(--erp-text-on-navy)" strokeWidth="2" strokeLinecap="square">
                <rect x="3" y="3" width="18" height="18" />
                <line x1="3" y1="9" x2="21" y2="9" />
                <line x1="9" y1="21" x2="9" y2="9" />
              </svg>
              <span style={{ fontWeight: 700, fontSize: "1rem" }}>{tApp("title")}</span>
            </div>
            <button
              type="button"
              style={{
                background: "transparent",
                border: "none",
                color: "var(--erp-text-on-navy)",
                cursor: "pointer",
                padding: "0.5rem",
                display: "inline-flex",
                alignItems: "center",
                justifyContent: "center",
                minHeight: "44px",
                minWidth: "44px",
              }}
              onClick={() => setIsMobileMenuOpen(false)}
              aria-label={tShell("closeMenu")}
            >
              <IconClose size={20} />
            </button>
          </div>

          {/* Mobile-only Context & User Info */}
          <div className="erp-drawer-context">
            <div style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
              <Avatar
                size="md"
                fileId={user?.avatarFileId}
                initial={user?.displayName || user?.email || undefined}
                alt={user?.displayName || user?.email || tShell("user")}
                variant="outline"
                className="border-erp-border bg-erp-surface text-erp-navy shrink-0"
              />
              <div style={{ minWidth: 0, flex: 1 }}>
                <div style={{ fontWeight: 600, color: "var(--erp-text-main)", fontSize: "0.875rem", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                  {user?.displayName || user?.email || "-"}
                </div>
                {user?.email && user?.displayName && (
                  <div style={{ color: "var(--erp-text-muted)", fontSize: "0.75rem", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                    {user.email}
                  </div>
                )}
              </div>
            </div>

            <div
              style={{
                borderTop: "1px solid var(--erp-border-subtle)",
                paddingTop: "0.625rem",
                marginTop: "0.125rem",
                display: "flex",
                flexDirection: "column",
                gap: "0.5rem",
              }}
            >
              <div
                style={{ display: "flex", alignItems: "center", gap: "0.625rem", fontSize: "0.8125rem" }}
                title={`${tShell("organization")}: ${orgName}`}
              >
                <span
                  style={{
                    color: "var(--erp-text-muted)",
                    display: "inline-flex",
                    alignItems: "center",
                    flexShrink: 0,
                  }}
                  aria-label={tShell("organization")}
                >
                  <IconBuilding size={16} />
                </span>
                <span
                  style={{
                    color: "var(--erp-text-main)",
                    fontWeight: 500,
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap",
                  }}
                >
                  {orgName}
                </span>
              </div>

              <div
                style={{ display: "flex", alignItems: "center", gap: "0.625rem", fontSize: "0.8125rem" }}
                title={`${tShell("branch")}: ${branchName}`}
              >
                <span
                  style={{
                    color: "var(--erp-text-muted)",
                    display: "inline-flex",
                    alignItems: "center",
                    flexShrink: 0,
                  }}
                  aria-label={tShell("branch")}
                >
                  <IconGitBranch size={16} />
                </span>
                <span
                  style={{
                    color: "var(--erp-text-muted)",
                    fontSize: "0.8125rem",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap",
                  }}
                >
                  {branchName}
                </span>
              </div>
            </div>
          </div>

          <SidebarNav
            currentUser={currentUser}
            isCollapsed={isSidebarCollapsed}
            onLinkClick={() => setIsMobileMenuOpen(false)}
          />

          {/* Desktop Bottom Sidebar Collapse/Expand Toggle */}
          <button
            type="button"
            className="erp-sidebar-bottom-toggle"
            onClick={() => setIsSidebarCollapsed((prev) => !prev)}
            aria-label={isSidebarCollapsed ? tShell("expandMenu") : tShell("collapseMenu")}
            title={isSidebarCollapsed ? tShell("expandMenu") : tShell("collapseMenu")}
          >
            {isSidebarCollapsed ? <IconChevronRight size={18} /> : <IconChevronLeft size={18} />}
            <span>{tShell("collapseMenu")}</span>
          </button>
        </aside>

        {/* Content Area */}
        <main
          id="main-content"
          role="main"
          className="erp-main-content"
        >
          {/* Home route renders the default dashboard: route pages return null
              children, which is still a truthy element, so truthiness alone
              can never select the dashboard. */}
          {isHomeActive ? (
            <>
              <div style={{ marginBottom: "1.75rem", borderBottom: "1px solid var(--erp-border)", paddingBottom: "1.25rem" }}>
                <h1 style={{ fontSize: "1.625rem", fontWeight: 700, color: "var(--erp-navy)", margin: "0 0 0.375rem 0", letterSpacing: "-0.01em" }}>
                  {tApp("title")}
                </h1>
                <p style={{ fontSize: "0.9375rem", color: "var(--erp-text-muted)", margin: 0, lineHeight: 1.4 }}>
                  {tApp("subtitle")}
                </p>
              </div>

              <div className="erp-dashboard-grid">
                {/* User Profile and Context Card */}
                <section
                  aria-labelledby="user-profile-heading"
                  className="erp-card"
                >
                  <div className="erp-card-header">
                    <h2 id="user-profile-heading" className="erp-card-title">
                      {tShell("user")}
                    </h2>
                    <span className="erp-badge erp-badge-success">ACTIVE</span>
                  </div>
                  <dl className="erp-dl">
                    <dt>ID:</dt>
                    <dd style={{ fontFamily: "monospace", fontSize: "0.8125rem" }}>{user?.id || "-"}</dd>

                    <dt>{tAuth("emailLabel")}:</dt>
                    <dd>{user?.email || "-"}</dd>

                    <dt>Name:</dt>
                    <dd style={{ fontWeight: 600 }}>{user?.displayName || "-"}</dd>
                  </dl>
                </section>

                {/* Memberships and Permissions Card */}
                <section
                  aria-labelledby="permissions-heading"
                  className="erp-card"
                >
                  <div className="erp-card-header">
                    <h2 id="permissions-heading" className="erp-card-title">
                      {tShell("permissions")}
                    </h2>
                    <span className="erp-badge erp-badge-info">
                      {memberships.reduce((acc, m) => acc + (m.permissions?.length || 0), 0)} PERMISSIONS
                    </span>
                  </div>

                  {memberships.map((membership, idx) => (
                    <div
                      key={membership.id || idx}
                      style={{
                        padding: "1rem",
                        marginBottom: idx < memberships.length - 1 ? "1rem" : 0,
                        backgroundColor: "var(--erp-surface-muted)",
                        border: "1px solid var(--erp-border)",
                      }}
                    >
                      <div style={{ fontWeight: 600, color: "var(--erp-navy)", marginBottom: "0.75rem", display: "flex", alignItems: "center", gap: "0.5rem", flexWrap: "wrap" }}>
                        <span>{membership.organization?.name || "-"}</span>
                        <span style={{ color: "var(--erp-border)" }}>•</span>
                        <span style={{ color: "var(--erp-text-muted)", fontSize: "0.875rem" }}>
                          {membership.branch?.name || tShell("noBranch")}
                        </span>
                      </div>

                      {membership.permissions && membership.permissions.length > 0 ? (
                        <ul
                          style={{
                            listStyle: "none",
                            padding: 0,
                            margin: 0,
                            display: "flex",
                            flexDirection: "column",
                            gap: "0.5rem",
                          }}
                        >
                          {membership.permissions.map((perm, pIdx) => (
                            <li
                              key={pIdx}
                              style={{
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "space-between",
                                flexWrap: "wrap",
                                gap: "0.5rem",
                                padding: "0.5rem 0.75rem",
                                backgroundColor: "var(--erp-surface)",
                                border: "1px solid var(--erp-border-subtle)",
                                fontSize: "0.875rem",
                              }}
                            >
                              <code style={{ fontFamily: "monospace", fontWeight: 600, color: "var(--erp-navy)" }}>
                                {perm.key}
                              </code>
                              {perm.scope && (
                                <span className="erp-badge erp-badge-neutral" style={{ fontSize: "0.6875rem" }}>
                                  {tShell("scope")}: {perm.scope}
                                </span>
                              )}
                            </li>
                          ))}
                        </ul>
                      ) : (
                        <p style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)", margin: 0 }}>
                          -
                        </p>
                      )}
                    </div>
                  ))}
                </section>
              </div>
            </>
          ) : (
            children
          )}
        </main>
      </div>
    </div>
  );
}
