"use client";

import React from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { signOutSession } from "@/lib/auth/auth-session";
import { getMessages, type SupportedLocale } from "@/lib/i18n/locales";

interface ErpShellProps {
  locale: SupportedLocale;
  currentUser: CurrentUserResponse;
}

export function ErpShell({ locale, currentUser }: ErpShellProps) {
  const m = getMessages(locale);
  const router = useRouter();

  const user = currentUser.user;
  const memberships = currentUser.memberships || [];
  const primaryMembership = memberships[0];
  const orgName = primaryMembership?.organization?.name || "-";
  const branchName = primaryMembership?.branch?.name || m.shell.noBranch;

  const targetLocale: SupportedLocale = locale === "th" ? "en" : "th";

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
          padding: "0 1.5rem",
          height: "64px",
          backgroundColor: "#0B3056",
          color: "#FFFFFF",
          borderBottom: "1px solid #082442",
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: "1.5rem" }}>
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
            <span style={{ fontSize: "1.25rem", fontWeight: 700, letterSpacing: "0.02em" }}>
              {m.app.title}
            </span>
          </div>

          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: "0.75rem",
              fontSize: "0.875rem",
              paddingLeft: "1.5rem",
              borderLeft: "1px solid rgba(255, 255, 255, 0.2)",
            }}
          >
            <span style={{ color: "#E5E7EB" }}>
              <strong>{m.shell.organization}:</strong>{" "}
              <span data-testid="org-name">{orgName}</span>
            </span>
            <span style={{ color: "rgba(255, 255, 255, 0.4)" }}>|</span>
            <span style={{ color: "#E5E7EB" }}>
              <strong>{m.shell.branch}:</strong>{" "}
              <span data-testid="branch-name">{branchName}</span>
            </span>
          </div>
        </div>

        <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
          {/* User information */}
          <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-end", fontSize: "0.8125rem" }}>
            <span style={{ fontWeight: 600, color: "#FFFFFF" }}>
              {user?.displayName || user?.email || m.shell.user}
            </span>
            {user?.displayName && user?.email && (
              <span style={{ color: "#D1D5DB" }}>{user.email}</span>
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
            {m.shell.switchLanguage}
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
            {m.auth.logout}
          </button>
        </div>
      </header>

      {/* Main Workspace Layout */}
      <div style={{ display: "flex", flex: 1 }}>
        {/* Navigation Sidebar */}
        <nav
          role="navigation"
          aria-label="Main Navigation"
          style={{
            width: "220px",
            backgroundColor: "#FFFFFF",
            borderRight: "1px solid #E5E7EB",
            padding: "1rem 0",
          }}
        >
          <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
            <li>
              <Link
                href={`/${locale}`}
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
                {m.shell.home}
              </Link>
            </li>
          </ul>
        </nav>

        {/* Content Area */}
        <main
          id="main-content"
          role="main"
          style={{
            flex: 1,
            padding: "2rem",
            maxWidth: "1200px",
          }}
        >
          <div style={{ marginBottom: "2rem" }}>
            <h1 style={{ fontSize: "1.75rem", fontWeight: 700, color: "#0B3056", margin: "0 0 0.5rem 0" }}>
              {m.app.title}
            </h1>
            <p style={{ fontSize: "1rem", color: "#4B5563", margin: 0 }}>
              {m.app.subtitle}
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
              {m.shell.user}
            </h2>
            <dl style={{ display: "grid", gridTemplateColumns: "140px 1fr", rowGap: "0.75rem", fontSize: "0.9375rem" }}>
              <dt style={{ fontWeight: 600, color: "#4B5563" }}>ID:</dt>
              <dd style={{ fontFamily: "monospace", color: "#111827" }}>{user?.id || "-"}</dd>

              <dt style={{ fontWeight: 600, color: "#4B5563" }}>{m.auth.emailLabel}:</dt>
              <dd style={{ color: "#111827" }}>{user?.email || "-"}</dd>

              <dt style={{ fontWeight: 600, color: "#4B5563" }}>Name:</dt>
              <dd style={{ color: "#111827" }}>{user?.displayName || "-"}</dd>
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
              {m.shell.permissions}
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
                  {membership.organization?.name || "-"} — {membership.branch?.name || m.shell.noBranch}
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
                            ({m.shell.scope}: {perm.scope})
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
