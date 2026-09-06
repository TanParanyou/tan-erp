"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { signOutSession } from "@/lib/auth/auth-session";
import { useTranslations, useLocale } from "next-intl";
import { IconClose } from "@/components/common/Icons";

interface ErpShellProps {
  currentUser: CurrentUserResponse;
}

export function ErpShell({ currentUser }: ErpShellProps) {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const tShell = useTranslations("shell");
  const tAuth = useTranslations("auth");
  const tApp = useTranslations("app");
  const locale = useLocale();
  const router = useRouter();

  const user = currentUser.user;
  const memberships = currentUser.memberships || [];
  const primaryMembership = memberships[0];
  const orgName = primaryMembership?.organization?.name || "-";
  const branchName = primaryMembership?.branch?.name || tShell("noBranch");

  const targetLocale = locale === "th" ? "en" : "th";

  const handleSignOut = async () => {
    await signOutSession();
    router.push(`/${locale}/login`);
  };

  return (
    <div style={{ display: "flex", flexDirection: "column", minHeight: "100vh", backgroundColor: "#F9FAFB" }}>
      {/* Top Application Bar */}
      <header
        role="banner"
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          padding: "0 1rem",
          height: "64px",
          backgroundColor: "#0B3056",
          color: "#FFFFFF",
          borderBottom: "1px solid #082442",
          position: "sticky",
          top: 0,
          zIndex: 1020,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
          {/* Mobile menu hamburger toggle */}
          <button
            type="button"
            className="erp-mobile-menu-btn"
            onClick={() => setIsMobileMenuOpen((prev) => !prev)}
            aria-label={isMobileMenuOpen ? tShell("closeMenu") : tShell("toggleMenu")}
            aria-expanded={isMobileMenuOpen}
          >
            {isMobileMenuOpen ? (
              <IconClose size={22} />
            ) : (
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="square">
                <line x1="3" y1="6" x2="21" y2="6" />
                <line x1="3" y1="12" x2="21" y2="12" />
                <line x1="3" y1="18" x2="21" y2="18" />
              </svg>
            )}
          </button>

          <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
            {/* Local SVG logo icon */}
            <svg
              width="28"
              height="28"
              viewBox="0 0 24 24"
              fill="none"
              stroke="#FFFFFF"
              strokeWidth="2"
              strokeLinecap="square"
              strokeLinejoin="miter"
              aria-hidden="true"
            >
              <rect x="3" y="3" width="18" height="18" />
              <line x1="3" y1="9" x2="21" y2="9" />
              <line x1="9" y1="21" x2="9" y2="9" />
            </svg>
            <span style={{ fontSize: "1.25rem", fontWeight: 700, letterSpacing: "0.02em", whiteSpace: "nowrap" }}>
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

        <div style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
          {/* User information (desktop/tablet) */}
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "flex-end",
              fontSize: "0.8125rem",
              maxWidth: "180px",
              overflow: "hidden",
              textOverflow: "ellipsis",
              whiteSpace: "nowrap",
            }}
          >
            <span style={{ fontWeight: 600, color: "#FFFFFF", maxWidth: "100%", overflow: "hidden", textOverflow: "ellipsis" }}>
              {user?.displayName || user?.email || tShell("user")}
            </span>
            {user?.displayName && user?.email && (
              <span style={{ color: "#D1D5DB", fontSize: "0.75rem", maxWidth: "100%", overflow: "hidden", textOverflow: "ellipsis" }}>
                {user.email}
              </span>
            )}
          </div>

          {/* Locale switcher link */}
          <Link
            href={`/${targetLocale}`}
            locale={false}
            style={{
              display: "inline-flex",
              alignItems: "center",
              justifyContent: "center",
              minHeight: "44px",
              minWidth: "44px",
              padding: "0 0.75rem",
              color: "#FFFFFF",
              backgroundColor: "transparent",
              border: "1px solid rgba(255, 255, 255, 0.3)",
              fontSize: "0.875rem",
              fontWeight: 500,
            }}
            aria-label={`Switch to ${targetLocale === "th" ? "Thai" : "English"}`}
          >
            {tShell("switchLanguage")}
          </Link>

          {/* Sign out button */}
          <button
            type="button"
            onClick={handleSignOut}
            style={{
              display: "inline-flex",
              alignItems: "center",
              justifyContent: "center",
              minHeight: "44px",
              padding: "0 1rem",
              backgroundColor: "transparent",
              color: "#FFFFFF",
              border: "1px solid rgba(255, 255, 255, 0.4)",
              fontSize: "0.875rem",
              fontWeight: 600,
              cursor: "pointer",
            }}
          >
            {tAuth("logout")}
          </button>
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

        {/* Navigation Sidebar */}
        <aside
          className={`erp-sidebar ${isMobileMenuOpen ? "erp-sidebar-open" : ""}`}
          role="navigation"
          aria-label="Main Navigation"
        >
          {/* Mobile-only header context info inside drawer */}
          <div
            style={{
              padding: "0 1rem 1rem 1rem",
              marginBottom: "1rem",
              borderBottom: "1px solid var(--erp-border-subtle)",
              display: "flex",
              flexDirection: "column",
              gap: "0.375rem",
              fontSize: "0.8125rem",
            }}
          >
            <div>
              <strong style={{ color: "var(--erp-text-muted)" }}>{tShell("organization")}:</strong>{" "}
              <span style={{ color: "var(--erp-text-main)", fontWeight: 600 }}>{orgName}</span>
            </div>
            <div>
              <strong style={{ color: "var(--erp-text-muted)" }}>{tShell("branch")}:</strong>{" "}
              <span style={{ color: "var(--erp-text-main)", fontWeight: 600 }}>{branchName}</span>
            </div>
          </div>

          <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
            <li>
              <Link
                href={`/${locale}`}
                onClick={() => setIsMobileMenuOpen(false)}
                style={{
                  display: "flex",
                  alignItems: "center",
                  minHeight: "44px",
                  padding: "0 1.25rem",
                  color: "#0B3056",
                  fontWeight: 600,
                  fontSize: "0.9375rem",
                  backgroundColor: "#EFF6FF",
                  borderLeft: "4px solid #0B3056",
                }}
              >
                {tShell("home")}
              </Link>
            </li>
          </ul>
        </aside>

        {/* Content Area */}
        <main
          id="main-content"
          role="main"
          className="erp-main-content"
        >
          <div style={{ marginBottom: "2rem" }}>
            <h1 style={{ fontSize: "1.75rem", fontWeight: 700, color: "#0B3056", margin: "0 0 0.5rem 0" }}>
              {tApp("title")}
            </h1>
            <p style={{ fontSize: "1rem", color: "#4B5563", margin: 0 }}>
              {tApp("subtitle")}
            </p>
          </div>

          {/* User Profile and Context Card */}
          <section
            aria-labelledby="user-profile-heading"
            style={{
              backgroundColor: "#FFFFFF",
              border: "1px solid #E5E7EB",
              padding: "1.5rem",
              marginBottom: "1.5rem",
            }}
          >
            <h2
              id="user-profile-heading"
              style={{ fontSize: "1.25rem", fontWeight: 600, color: "#111827", margin: "0 0 1rem 0" }}
            >
              {tShell("user")}
            </h2>
            <dl style={{ display: "grid", gridTemplateColumns: "minmax(80px, 140px) 1fr", rowGap: "0.75rem", fontSize: "0.9375rem" }}>
              <dt style={{ fontWeight: 600, color: "#4B5563" }}>ID:</dt>
              <dd style={{ fontFamily: "monospace", color: "#111827", wordBreak: "break-all" }}>{user?.id || "-"}</dd>

              <dt style={{ fontWeight: 600, color: "#4B5563" }}>{tAuth("emailLabel")}:</dt>
              <dd style={{ color: "#111827", wordBreak: "break-all" }}>{user?.email || "-"}</dd>

              <dt style={{ fontWeight: 600, color: "#4B5563" }}>Name:</dt>
              <dd style={{ color: "#111827", wordBreak: "break-word" }}>{user?.displayName || "-"}</dd>
            </dl>
          </section>

          {/* Memberships and Permissions Card */}
          <section
            aria-labelledby="permissions-heading"
            style={{
              backgroundColor: "#FFFFFF",
              border: "1px solid #E5E7EB",
              padding: "1.5rem",
            }}
          >
            <h2
              id="permissions-heading"
              style={{ fontSize: "1.25rem", fontWeight: 600, color: "#111827", margin: "0 0 1rem 0" }}
            >
              {tShell("permissions")}
            </h2>

            {memberships.map((membership, idx) => (
              <div
                key={membership.id || idx}
                style={{
                  padding: "1rem",
                  marginBottom: "1rem",
                  backgroundColor: "#F9FAFB",
                  border: "1px solid #E5E7EB",
                }}
              >
                <div style={{ fontWeight: 600, color: "#0B3056", marginBottom: "0.5rem" }}>
                  {membership.organization?.name || "-"} — {membership.branch?.name || tShell("noBranch")}
                </div>

                {membership.permissions && membership.permissions.length > 0 ? (
                  <ul
                    style={{
                      listStyle: "disc",
                      paddingLeft: "1.5rem",
                      margin: 0,
                      fontSize: "0.875rem",
                      color: "#374151",
                    }}
                  >
                    {membership.permissions.map((perm, pIdx) => (
                      <li key={pIdx} style={{ margin: "0.25rem 0" }}>
                        <code style={{ fontFamily: "monospace", fontWeight: 600 }}>{perm.key}</code>
                        {perm.scope && (
                          <span style={{ color: "#6B7280", marginLeft: "0.5rem" }}>
                            ({tShell("scope")}: {perm.scope})
                          </span>
                        )}
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p style={{ fontSize: "0.875rem", color: "#6B7280", margin: 0 }}>
                    -
                  </p>
                )}
              </div>
            ))}
          </section>
        </main>
      </div>
    </div>
  );
}
