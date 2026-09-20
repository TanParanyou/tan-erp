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
  IconChevronLeft,
  IconChevronRight,
  IconLogOut,
  IconGlobe,
  IconBriefcase,
  IconUser,
  IconBuilding,
  IconGitBranch,
  IconFileText,
} from "@/components/common/Icons";
import { LogoIcon } from "@/components/common/Logo";
import { Button } from "@/components/ui/Button";
import { ThemeToggle } from "@/components/common/ThemeToggle";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

interface ErpShellProps {
  currentUser: CurrentUserResponse;
  children?: React.ReactNode;
}

export function ErpShell({ currentUser, children }: ErpShellProps) {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const tShell = useTranslations("shell");
  const tAuth = useTranslations("auth");
  const tApp = useTranslations("app");
  const locale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const queryClient = useQueryClient();

  const { selectedMembership } = useSelectedMembership();

  const user = currentUser.user;
  const memberships = currentUser.memberships || [];
  const activeMembership = selectedMembership || memberships[0];
  const orgName = activeMembership?.organization?.name || "-";
  const branchName = activeMembership?.branch?.name || tShell("noBranch");

  const hasCustomersRead = can(activeMembership, "customers.read");
  const hasOpportunitiesRead = can(activeMembership, "opportunities.read");
  const hasSettingsRead = can(activeMembership, "document-sequences.read") || can(activeMembership, "organizations.read");

  const targetLocale = locale === "th" ? "en" : "th";

  const handleSignOut = async () => {
    await signOutSession(queryClient);
    router.push(`/${locale}/login`);
  };

  const isHomeActive = pathname === `/${locale}` || pathname === `/${locale}/`;
  const isCustomersActive = pathname.startsWith(`/${locale}/customers`);
  const isOpportunitiesActive = pathname.startsWith(`/${locale}/opportunities`);
  const isDocumentNumberingActive = pathname.startsWith(`/${locale}/settings/document-numbering`);

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
            <LogoIcon size={26} color="#FFFFFF" strokeWidth={2} style={{ flexShrink: 0 }} />
            <span className="erp-header-title">
              {tApp("title")}
            </span>
          </div>

          <div className="erp-header-context-info">
            <span style={{ color: "#E5E7EB" }}>
              <strong>{tShell("organization")}:</strong>{" "}
              <span data-testid="org-name">{orgName}</span>
            </span>
            <span style={{ color: "rgba(255, 255, 255, 0.4)" }}>|</span>
            <span style={{ color: "#E5E7EB" }}>
              <strong>{tShell("branch")}:</strong>{" "}
              <span data-testid="branch-name">{branchName}</span>
            </span>
          </div>
        </div>

        <div className="erp-header-right">
          {/* User information (desktop/tablet only) */}
          <div className="erp-header-user-info">
            <span style={{ fontWeight: 600, color: "#FFFFFF", maxWidth: "100%", overflow: "hidden", textOverflow: "ellipsis" }}>
              {user?.displayName || user?.email || tShell("user")}
            </span>
            {user?.displayName && user?.email && (
              <span style={{ color: "#D1D5DB", fontSize: "0.75rem", maxWidth: "100%", overflow: "hidden", textOverflow: "ellipsis" }}>
                {user.email}
              </span>
            )}
          </div>

          {/* Theme switcher toggle */}
          <ThemeToggle />

          {/* Locale switcher link */}
          <Link
            href={`/${targetLocale}`}
            className="erp-header-lang-btn"
            style={{
              minHeight: "44px",
            }}
            aria-label={`Switch to ${targetLocale === "th" ? "Thai" : "English"}`}
            title={`Switch to ${targetLocale === "th" ? "Thai" : "English"}`}
          >
            <IconGlobe size={16} />
            <span className="erp-lang-full">{tShell("switchLanguage")}</span>
            <span className="erp-lang-short">{targetLocale.toUpperCase()}</span>
          </Link>

          {/* Sign out button */}
          <Button
            type="button"
            variant="outline"
            size="md"
            onClick={handleSignOut}
            className="erp-header-logout-btn"
            style={{
              minHeight: "44px",
            }}
            aria-label={tAuth("logout")}
            title={tAuth("logout")}
          >
            <IconLogOut size={16} />
            <span className="erp-header-btn-text">{tAuth("logout")}</span>
          </Button>
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
          role="navigation"
          aria-label="Main Navigation"
        >
          {/* Mobile Drawer Header */}
          <div className="erp-drawer-header">
            <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="#FFFFFF" strokeWidth="2" strokeLinecap="square">
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
                color: "#FFFFFF",
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
            <div style={{ display: "flex", alignItems: "flex-start", gap: "0.75rem" }}>
              <div
                style={{
                  width: "36px",
                  height: "36px",
                  backgroundColor: "var(--erp-surface)",
                  border: "1px solid var(--erp-border)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  color: "var(--erp-navy)",
                  flexShrink: 0,
                }}
                aria-label={tShell("user")}
                title={tShell("user")}
              >
                <IconUser size={20} />
              </div>
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

          <ul style={{ listStyle: "none", padding: "0.5rem 0", margin: 0, flex: 1 }}>
            <li>
              <Link
                href={`/${locale}`}
                onClick={() => setIsMobileMenuOpen(false)}
                className={`erp-nav-link ${isHomeActive ? "erp-nav-link-active" : ""}`}
                title={tShell("home")}
              >
                <IconHome size={20} />
                <span className="erp-nav-text">{tShell("home")}</span>
              </Link>
            </li>
            {hasCustomersRead && (
              <li>
                <Link
                  href={`/${locale}/customers`}
                  onClick={() => setIsMobileMenuOpen(false)}
                  className={`erp-nav-link ${isCustomersActive ? "erp-nav-link-active" : ""}`}
                  title={tShell("customers")}
                >
                  <IconUsers size={20} />
                  <span className="erp-nav-text">{tShell("customers")}</span>
                </Link>
              </li>
            )}
            {hasOpportunitiesRead && (
              <li>
                <Link
                  href={`/${locale}/opportunities`}
                  onClick={() => setIsMobileMenuOpen(false)}
                  className={`erp-nav-link ${isOpportunitiesActive ? "erp-nav-link-active" : ""}`}
                  title={tShell("opportunities")}
                >
                  <IconBriefcase size={20} />
                  <span className="erp-nav-text">{tShell("opportunities")}</span>
                </Link>
              </li>
            )}
            {hasSettingsRead && (
              <li>
                <Link
                  href={`/${locale}/settings/document-numbering`}
                  onClick={() => setIsMobileMenuOpen(false)}
                  className={`erp-nav-link ${isDocumentNumberingActive ? "erp-nav-link-active" : ""}`}
                  title={tShell("documentNumbering")}
                >
                  <IconFileText size={20} />
                  <span className="erp-nav-text">{tShell("documentNumbering")}</span>
                </Link>
              </li>
            )}
          </ul>

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
