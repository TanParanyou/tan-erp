"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import type { User } from "firebase/auth";
import { useQueryClient } from "@tanstack/react-query";
import { subscribeToAuthChanges, signOutSession } from "@/lib/auth/auth-session";
import { useCurrentUser } from "@/features/auth/api/current-user-query";
import { useTranslations, useLocale } from "next-intl";
import { ApiError } from "@/lib/api/api-error";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";

interface AccessGateProps {
  children: (currentUser: CurrentUserResponse) => React.ReactNode;
}

export function AccessGate({ children }: AccessGateProps) {
  const queryClient = useQueryClient();
  const tGate = useTranslations("gate");
  const tAuth = useTranslations("auth");
  const locale = useLocale();
  const router = useRouter();
  const [firebaseUser, setFirebaseUser] = useState<User | null | undefined>(undefined);

  useEffect(() => {
    const unsubscribe = subscribeToAuthChanges(queryClient, (user) => {
      setFirebaseUser(user);
    });
    return () => unsubscribe();
  }, [queryClient]);

  const {
    data: currentUser,
    isLoading: isProfileLoading,
    error,
    refetch,
  } = useCurrentUser(firebaseUser?.uid, locale === "en" ? "en" : "th");

  const apiError = error instanceof ApiError ? error : null;
  const errorCode = apiError?.code;
  const isUnauthorized = apiError?.status === 401 || errorCode === "AUTHENTICATION_INVALID";

  // Effect: Redirect to login when there is no Firebase session
  useEffect(() => {
    if (firebaseUser === null) {
      router.push(`/${locale}/login`);
    }
  }, [firebaseUser, locale, router]);

  // Effect: Sign out and redirect to login on 401 / invalid authentication
  useEffect(() => {
    if (isUnauthorized) {
      signOutSession(queryClient).then(() => {
        router.push(`/${locale}/login`);
      });
    }
  }, [isUnauthorized, locale, router, queryClient]);

  // 1. Firebase session loading
  if (firebaseUser === undefined) {
    return (
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "60vh",
        }}
      >
        <MonoSpinner size="md" label={tGate("authLoading")} aria-busy="true" />
      </div>
    );
  }

  // 2. No Firebase session -> redirect to login (handled in useEffect)
  if (firebaseUser === null) {
    return null;
  }

  // 3. Current User profile loading
  if (isProfileLoading) {
    return (
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "60vh",
        }}
      >
        <MonoSpinner size="lg" label={tGate("profileLoading")} aria-busy="true" />
      </div>
    );
  }

  // 4. Error states
  if (error) {
    // 4a. 401 AUTHENTICATION_INVALID -> sign out and return to login (handled in useEffect)
    if (isUnauthorized) {
      return null;
    }

    // 4b. 403 ACTIVE_MEMBERSHIP_REQUIRED -> no-membership recovery state
    if (errorCode === "ACTIVE_MEMBERSHIP_REQUIRED") {
      return (
        <div style={{ padding: "1rem", display: "flex", justifyContent: "center" }}>
          <div
            role="alert"
            className="erp-card"
            style={{
              maxWidth: "520px",
              width: "100%",
              margin: "2rem auto",
              padding: "2rem",
              borderColor: "var(--erp-warning-border)",
              backgroundColor: "var(--erp-warning-bg)",
              boxShadow: "0 4px 6px -1px rgba(146, 64, 14, 0.08)",
            }}
          >
            <div style={{ display: "flex", alignItems: "center", gap: "0.75rem", marginBottom: "1rem" }}>
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--erp-warning)" strokeWidth="2" strokeLinecap="square">
                <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                <line x1="12" y1="9" x2="12" y2="13" />
                <line x1="12" y1="17" x2="12.01" y2="17" />
              </svg>
              <h2 style={{ color: "var(--erp-warning)", fontSize: "1.25rem", margin: 0, fontWeight: 700 }}>
                {tGate("noMembershipTitle")}
              </h2>
            </div>
            <p style={{ color: "#78350F", lineHeight: 1.6, fontSize: "0.9375rem" }}>
              {tGate("noMembershipDetail")}
            </p>
            <div style={{ marginTop: "1.75rem" }}>
              <Button
                variant="primary"
                size="md"
                onClick={() => signOutSession(queryClient).then(() => router.push(`/${locale}/login`))}
                style={{ minHeight: "44px" }}
              >
                {tAuth("logout")}
              </Button>
            </div>
          </div>
        </div>
      );
    }

    // 4c. 403 USER_ACCESS_DISABLED -> disabled-user state
    if (errorCode === "USER_ACCESS_DISABLED") {
      return (
        <div style={{ padding: "1rem", display: "flex", justifyContent: "center" }}>
          <div
            role="alert"
            className="erp-card"
            style={{
              maxWidth: "520px",
              width: "100%",
              margin: "2rem auto",
              padding: "2rem",
              borderColor: "var(--erp-danger-border)",
              backgroundColor: "var(--erp-danger-bg)",
              boxShadow: "0 4px 6px -1px rgba(153, 27, 27, 0.08)",
            }}
          >
            <div style={{ display: "flex", alignItems: "center", gap: "0.75rem", marginBottom: "1rem" }}>
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--erp-danger)" strokeWidth="2" strokeLinecap="square">
                <circle cx="12" cy="12" r="10" />
                <line x1="4.93" y1="4.93" x2="19.07" y2="19.07" />
              </svg>
              <h2 style={{ color: "var(--erp-danger)", fontSize: "1.25rem", margin: 0, fontWeight: 700 }}>
                {tGate("userDisabledTitle")}
              </h2>
            </div>
            <p style={{ color: "#7F1D1D", lineHeight: 1.6, fontSize: "0.9375rem" }}>
              {tGate("userDisabledDetail")}
            </p>
            <div style={{ marginTop: "1.75rem" }}>
              <Button
                variant="primary"
                size="md"
                onClick={() => signOutSession(queryClient).then(() => router.push(`/${locale}/login`))}
                style={{ minHeight: "44px" }}
              >
                {tAuth("logout")}
              </Button>
            </div>
          </div>
        </div>
      );
    }

    // 4d. Network / Server error -> Retry button + traceId
    return (
      <div style={{ padding: "1rem", display: "flex", justifyContent: "center" }}>
        <div
          role="alert"
          className="erp-card"
          style={{
            maxWidth: "520px",
            width: "100%",
            margin: "2rem auto",
            padding: "2rem",
            borderColor: "var(--erp-danger-border)",
          }}
        >
          <div style={{ display: "flex", alignItems: "center", gap: "0.75rem", marginBottom: "1rem" }}>
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--erp-danger)" strokeWidth="2" strokeLinecap="square">
              <polygon points="7.86 2 16.14 2 22 7.86 22 16.14 16.14 22 7.86 22 2 16.14 2 7.86 7.86 2" />
              <line x1="12" y1="8" x2="12" y2="12" />
              <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <h2 style={{ color: "var(--erp-danger)", fontSize: "1.25rem", margin: 0, fontWeight: 700 }}>
              {tGate("serverErrorTitle")}
            </h2>
          </div>
          <p style={{ color: "var(--erp-text-body)", lineHeight: 1.6, fontSize: "0.9375rem" }}>
            {apiError?.message || tGate("serverErrorDetail")}
          </p>
          {apiError?.traceId && (
            <div style={{ marginTop: "0.75rem", padding: "0.5rem 0.75rem", backgroundColor: "var(--erp-surface-muted)", border: "1px solid var(--erp-border-subtle)", fontSize: "0.8125rem", fontFamily: "monospace", color: "var(--erp-text-muted)" }}>
              {tGate("traceId")}
              <span style={{ color: "var(--erp-navy)", fontWeight: 600 }}>{apiError.traceId}</span>
            </div>
          )}
          <div style={{ marginTop: "1.75rem", display: "flex", flexWrap: "wrap", gap: "1rem" }}>
            <Button
              variant="primary"
              size="md"
              onClick={() => refetch()}
              style={{ minHeight: "44px" }}
            >
              {tGate("retry")}
            </Button>
            <Button
              variant="outline"
              size="md"
              onClick={() => signOutSession(queryClient).then(() => router.push(`/${locale}/login`))}
              style={{ minHeight: "44px" }}
            >
              {tAuth("logout")}
            </Button>
          </div>
        </div>
      </div>
    );
  }

  // 5. Success -> Render children with trusted backend context
  if (currentUser) {
    return <>{children(currentUser)}</>;
  }

  return null;
}
