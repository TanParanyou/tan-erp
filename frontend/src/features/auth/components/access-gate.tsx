"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import type { User } from "firebase/auth";
import { subscribeToAuthChanges, signOutSession } from "@/lib/auth/auth-session";
import { useCurrentUser } from "@/features/auth/api/current-user-query";
import { getMessages, type SupportedLocale } from "@/lib/i18n/locales";
import { ApiError } from "@/lib/api/api-error";
import type { CurrentUserResponse } from "@/lib/api/api-client";

interface AccessGateProps {
  locale: SupportedLocale;
  children: (currentUser: CurrentUserResponse) => React.ReactNode;
}

export function AccessGate({ locale, children }: AccessGateProps) {
  const m = getMessages(locale);
  const router = useRouter();
  const [firebaseUser, setFirebaseUser] = useState<User | null | undefined>(undefined);

  useEffect(() => {
    const unsubscribe = subscribeToAuthChanges((user) => {
      setFirebaseUser(user);
    });
    return () => unsubscribe();
  }, []);

  const {
    data: currentUser,
    isLoading: isProfileLoading,
    error,
    refetch,
  } = useCurrentUser(firebaseUser?.uid, locale);

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
      signOutSession().then(() => {
        router.push(`/${locale}/login`);
      });
    }
  }, [isUnauthorized, locale, router]);

  // 1. Firebase session loading
  if (firebaseUser === undefined) {
    return (
      <div
        role="status"
        aria-busy="true"
        aria-live="polite"
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "60vh",
          fontSize: "1.125rem",
          color: "#0B3056",
          fontWeight: 500,
        }}
      >
        {m.gate.authLoading}
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
        role="status"
        aria-busy="true"
        aria-live="polite"
        style={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "60vh",
          gap: "1rem",
        }}
      >
        <div
          style={{
            width: "200px",
            height: "20px",
            backgroundColor: "#E5E7EB",
            animation: "pulse 1.5s infinite",
          }}
        />
        <span style={{ fontSize: "1rem", color: "#4B5563" }}>
          {m.gate.profileLoading}
        </span>
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
          style={{
            maxWidth: "500px",
            margin: "4rem auto",
            padding: "2rem",
            border: "1px solid #D97706",
            backgroundColor: "#FFFBEB",
            borderRadius: 0,
          }}
        >
          <h2 style={{ color: "#B45309", fontSize: "1.25rem", marginTop: 0 }}>
            {m.gate.noMembershipTitle}
          </h2>
          <p style={{ color: "#78350F", lineHeight: 1.5 }}>
            {m.gate.noMembershipDetail}
          </p>
          <div style={{ marginTop: "1.5rem" }}>
            <button
              onClick={() => signOutSession().then(() => router.push(`/${locale}/login`))}
              style={{
                height: "44px",
                padding: "0 1.5rem",
                backgroundColor: "#0B3056",
                color: "#ffffff",
                border: "none",
                fontWeight: 600,
                cursor: "pointer",
                borderRadius: 0,
              }}
            >
              {m.auth.logout}
            </button>
          </div>
        </div>
      );
    }

    // 4c. 403 USER_ACCESS_DISABLED -> disabled-user state
    if (errorCode === "USER_ACCESS_DISABLED") {
      return (
        <div
          role="alert"
          style={{
            maxWidth: "500px",
            margin: "4rem auto",
            padding: "2rem",
            border: "1px solid #DC2626",
            backgroundColor: "#FEF2F2",
            borderRadius: 0,
          }}
        >
          <h2 style={{ color: "#991B1B", fontSize: "1.25rem", marginTop: 0 }}>
            {m.gate.userDisabledTitle}
          </h2>
          <p style={{ color: "#7F1D1D", lineHeight: 1.5 }}>
            {m.gate.userDisabledDetail}
          </p>
          <div style={{ marginTop: "1.5rem" }}>
            <button
              onClick={() => signOutSession().then(() => router.push(`/${locale}/login`))}
              style={{
                height: "44px",
                padding: "0 1.5rem",
                backgroundColor: "#0B3056",
                color: "#ffffff",
                border: "none",
                fontWeight: 600,
                cursor: "pointer",
                borderRadius: 0,
              }}
            >
              {m.auth.logout}
            </button>
          </div>
        </div>
      );
    }

    // 4d. Network / Server error -> Retry button + traceId
    return (
      <div
        role="alert"
        style={{
          maxWidth: "500px",
          margin: "4rem auto",
          padding: "2rem",
          border: "1px solid #DC2626",
          backgroundColor: "#ffffff",
          borderRadius: 0,
        }}
      >
        <h2 style={{ color: "#DC2626", fontSize: "1.25rem", marginTop: 0 }}>
          {m.gate.serverErrorTitle}
        </h2>
        <p style={{ color: "#4B5563", lineHeight: 1.5 }}>
          {apiError?.message || m.gate.serverErrorDetail}
        </p>
        {apiError?.traceId && (
          <p style={{ fontSize: "0.8125rem", color: "#6B7280", fontFamily: "monospace" }}>
            {m.gate.traceId}
            {apiError.traceId}
          </p>
        )}
        <div style={{ marginTop: "1.5rem", display: "flex", gap: "1rem" }}>
          <button
            onClick={() => refetch()}
            style={{
              height: "44px",
              padding: "0 1.5rem",
              backgroundColor: "#0B3056",
              color: "#ffffff",
              border: "none",
              fontWeight: 600,
              cursor: "pointer",
              borderRadius: 0,
            }}
          >
            {m.gate.retry}
          </button>
          <button
            onClick={() => signOutSession().then(() => router.push(`/${locale}/login`))}
            style={{
              height: "44px",
              padding: "0 1.5rem",
              backgroundColor: "transparent",
              color: "#0B3056",
              border: "1px solid #0B3056",
              fontWeight: 600,
              cursor: "pointer",
              borderRadius: 0,
            }}
          >
            {m.auth.logout}
          </button>
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
