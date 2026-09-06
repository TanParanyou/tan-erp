"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { FirebaseError } from "firebase/app";
import { signInWithEmail } from "@/lib/auth/auth-session";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

function getFirebaseAuthErrorKey(error: unknown): "invalidCredentials" | "signInFailed" {
  if (!(error instanceof FirebaseError)) return "signInFailed";

  return [
    "auth/invalid-credential",
    "auth/user-not-found",
    "auth/wrong-password",
  ].includes(error.code)
    ? "invalidCredentials"
    : "signInFailed";
}

interface LoginFormProps {
  onSuccess?: () => void;
}

export function LoginForm({ onSuccess }: LoginFormProps = {}) {
  const router = useRouter();
  const t = useTranslations("auth");
  const locale = useLocale();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<{ email?: string; password?: string }>({});

  const validate = (): boolean => {
    const errors: { email?: string; password?: string } = {};

    if (!email.trim()) {
      errors.email = t("emailRequired");
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) {
      errors.email = t("emailInvalid");
    }

    if (!password) {
      errors.password = t("passwordRequired");
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isSubmitting) return;

    setErrorMessage(null);
    if (!validate()) return;

    setIsSubmitting(true);
    try {
      await signInWithEmail(email, password);
      if (onSuccess) {
        onSuccess();
      } else {
        router.push(`/${locale}`);
      }
    } catch (err: unknown) {
      setErrorMessage(t(getFirebaseAuthErrorKey(err)));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="login-card">
      <header style={{ marginBottom: "1.5rem" }}>
        <div style={{ display: "flex", alignItems: "center", gap: "0.75rem", marginBottom: "1.25rem" }}>
          <svg
            width="32"
            height="32"
            viewBox="0 0 24 24"
            fill="none"
            stroke="#0B3056"
            strokeWidth="2"
            strokeLinecap="square"
            strokeLinejoin="miter"
            aria-hidden="true"
          >
            <rect x="3" y="3" width="18" height="18" />
            <line x1="3" y1="9" x2="21" y2="9" />
            <line x1="9" y1="21" x2="9" y2="9" />
          </svg>
          <div>
            <div style={{ fontSize: "0.6875rem", fontWeight: 700, color: "var(--erp-text-muted)", letterSpacing: "0.08em", textTransform: "uppercase" }}>
              ATELIER ARCHITECTURAL NAVY
            </div>
            <div style={{ fontSize: "1rem", fontWeight: 800, color: "var(--erp-navy)" }}>
              TAN-ERP
            </div>
          </div>
        </div>

        <h1 style={{ fontSize: "1.375rem", color: "var(--erp-navy)", margin: "0 0 0.5rem 0", fontWeight: 700 }}>
          {t("loginTitle")}
        </h1>
        <p style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)", margin: 0, lineHeight: 1.5 }}>
          {t("loginNotice")}
        </p>
      </header>

      <form onSubmit={handleSubmit} noValidate aria-busy={isSubmitting}>
        {errorMessage && (
          <div
            role="alert"
            aria-live="polite"
            style={{
              padding: "0.75rem 1rem",
              marginBottom: "1.25rem",
              backgroundColor: "var(--erp-danger-bg)",
              border: "1px solid var(--erp-danger-border)",
              color: "var(--erp-danger)",
              fontSize: "0.875rem",
              borderRadius: 0,
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
            }}
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="square">
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="12" />
              <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <span>{errorMessage}</span>
          </div>
        )}

        <Input
          id="email"
          name="email"
          type="email"
          autoComplete="email"
          label={t("emailLabel")}
          placeholder={t("emailPlaceholder")}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          disabled={isSubmitting}
          aria-invalid={Boolean(fieldErrors.email)}
          aria-describedby={fieldErrors.email ? "email-error" : undefined}
          error={fieldErrors.email}
          required
        />

        <Input
          id="password"
          name="password"
          type="password"
          autoComplete="current-password"
          label={t("passwordLabel")}
          placeholder={t("passwordPlaceholder")}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={isSubmitting}
          aria-invalid={Boolean(fieldErrors.password)}
          aria-describedby={fieldErrors.password ? "password-error" : undefined}
          error={fieldErrors.password}
          required
        />

        <div style={{ marginTop: "1.5rem" }}>
          <Button
            type="submit"
            variant="primary"
            size="md"
            isLoading={isSubmitting}
            disabled={isSubmitting}
            style={{ width: "100%", height: "44px" }}
          >
            {isSubmitting ? t("submitting") : t("submitButton")}
          </Button>
        </div>
      </form>
    </div>
  );
}
