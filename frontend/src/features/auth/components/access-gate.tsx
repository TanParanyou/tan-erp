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
        <div
          role="alert"
          className="erp-card"
          style={{
            maxWidth: "500px",
            margin: "4rem auto",
            padding: "2rem",
            borderColor: "var(--erp-warning)",
            backgroundColor: "var(--erp-warning-bg)",
          }}
        >
          <h2 style={{ color: "#B45309", fontSize: "1.25rem", marginTop: 0 }}>
            {tGate("noMembershipTitle")}
          </h2>
          <p style={{ color: "#78350F", lineHeight: 1.5 }}>
            {tGate("noMembershipDetail")}
          </p>
          <div style={{ marginTop: "1.5rem" }}>
            <Button
              variant="primary"
              size="md"
              onClick={() => signOutSession(queryClient).then(() => router.push(`/${locale}/login`))}
            >
              {tAuth("logout")}
            </Button>
          </div>
        </div>
      );
    }

    // 4c. 403 USER_ACCESS_DISABLED -> disabled-user state
    if (errorCode === "USER_ACCESS_DISABLED") {
      return (
        <div
          role="alert"
          className="erp-card"
          style={{
            maxWidth: "500px",
            margin: "4rem auto",
            padding: "2rem",
            borderColor: "var(--erp-danger)",
            backgroundColor: "var(--erp-danger-bg)",
          }}
        >
          <h2 style={{ color: "#991B1B", fontSize: "1.25rem", marginTop: 0 }}>
            {tGate("userDisabledTitle")}
          </h2>
          <p style={{ color: "#7F1D1D", lineHeight: 1.5 }}>
            {tGate("userDisabledDetail")}
          </p>
          <div style={{ marginTop: "1.5rem" }}>
            <Button
              variant="primary"
              size="md"
              onClick={() => signOutSession(queryClient).then(() => router.push(`/${locale}/login`))}
            >
              {tAuth("logout")}
            </Button>
          </div>
        </div>
      );
    }

    // 4d. Network / Server error -> Retry button + traceId
    return (
      <div
        role="alert"
        className="erp-card"
        style={{
          maxWidth: "500px",
          margin: "4rem auto",
          padding: "2rem",
          borderColor: "var(--erp-danger)",
        }}
      >
        <h2 style={{ color: "var(--erp-danger)", fontSize: "1.25rem", marginTop: 0 }}>
          {tGate("serverErrorTitle")}
        </h2>
        <p style={{ color: "var(--erp-text-muted)", lineHeight: 1.5 }}>
          {apiError?.message || tGate("serverErrorDetail")}
        </p>
        {apiError?.traceId && (
          <p style={{ fontSize: "0.8125rem", color: "var(--erp-text-muted)", fontFamily: "monospace", marginTop: "0.5rem" }}>
            {tGate("traceId")}
            {apiError.traceId}
          </p>
        )}
        <div style={{ marginTop: "1.5rem", display: "flex", gap: "1rem" }}>
          <Button
            variant="primary"
            size="md"
            onClick={() => refetch()}
          >
            {tGate("retry")}
          </Button>
          <Button
            variant="outline"
            size="md"
            onClick={() => signOutSession(queryClient).then(() => router.push(`/${locale}/login`))}
          >
            {tAuth("logout")}
          </Button>
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
