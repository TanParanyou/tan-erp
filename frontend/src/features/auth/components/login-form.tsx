"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { signInWithEmail } from "@/lib/auth/auth-session";
import { useTranslations, useLocale } from "next-intl";

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
      const firebaseError = err as { code?: string; message?: string };
      if (firebaseError.code === "auth/invalid-credential" || firebaseError.code === "auth/user-not-found" || firebaseError.code === "auth/wrong-password") {
        setErrorMessage(t("invalidCredentials"));
      } else {
        setErrorMessage(firebaseError.message || t("signInFailed"));
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="login-card" style={{ maxWidth: "420px", width: "100%", margin: "0 auto", padding: "2rem", border: "1px solid #0B3056", borderRadius: 0, backgroundColor: "#ffffff" }}>
      <header style={{ marginBottom: "1.5rem" }}>
        <h1 style={{ fontSize: "1.5rem", color: "#0B3056", margin: "0 0 0.5rem 0", fontWeight: 700 }}>
          {t("loginTitle")}
        </h1>
        <p style={{ fontSize: "0.875rem", color: "#4B5563", margin: 0, lineHeight: 1.4 }}>
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
              marginBottom: "1rem",
              backgroundColor: "#FEF2F2",
              border: "1px solid #DC2626",
              color: "#991B1B",
              fontSize: "0.875rem",
              borderRadius: 0,
            }}
          >
            {errorMessage}
          </div>
        )}

        <div style={{ marginBottom: "1.25rem" }}>
          <label
            htmlFor="email"
            style={{ display: "block", marginBottom: "0.375rem", fontSize: "0.875rem", fontWeight: 600, color: "#1F2937" }}
          >
            {t("emailLabel")}
          </label>
          <input
            id="email"
            name="email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            disabled={isSubmitting}
            aria-invalid={Boolean(fieldErrors.email)}
            aria-describedby={fieldErrors.email ? "email-error" : undefined}
            placeholder={t("emailPlaceholder")}
            style={{
              width: "100%",
              height: "44px",
              padding: "0 0.75rem",
              fontSize: "1rem",
              border: `1px solid ${fieldErrors.email ? "#DC2626" : "#0B3056"}`,
              borderRadius: 0,
              outline: "none",
              boxSizing: "border-box",
            }}
          />
          {fieldErrors.email && (
            <p id="email-error" style={{ color: "#DC2626", fontSize: "0.8125rem", margin: "0.25rem 0 0 0" }}>
              {fieldErrors.email}
            </p>
          )}
        </div>

        <div style={{ marginBottom: "1.5rem" }}>
          <label
            htmlFor="password"
            style={{ display: "block", marginBottom: "0.375rem", fontSize: "0.875rem", fontWeight: 600, color: "#1F2937" }}
          >
            {t("passwordLabel")}
          </label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={isSubmitting}
            aria-invalid={Boolean(fieldErrors.password)}
            aria-describedby={fieldErrors.password ? "password-error" : undefined}
            placeholder={t("passwordPlaceholder")}
            style={{
              width: "100%",
              height: "44px",
              padding: "0 0.75rem",
              fontSize: "1rem",
              border: `1px solid ${fieldErrors.password ? "#DC2626" : "#0B3056"}`,
              borderRadius: 0,
              outline: "none",
              boxSizing: "border-box",
            }}
          />
          {fieldErrors.password && (
            <p id="password-error" style={{ color: "#DC2626", fontSize: "0.8125rem", margin: "0.25rem 0 0 0" }}>
              {fieldErrors.password}
            </p>
          )}
        </div>

        <button
          type="submit"
          disabled={isSubmitting}
          style={{
            width: "100%",
            height: "44px",
            backgroundColor: isSubmitting ? "#6B7280" : "#0B3056",
            color: "#ffffff",
            fontSize: "1rem",
            fontWeight: 600,
            border: "none",
            borderRadius: 0,
            cursor: isSubmitting ? "not-allowed" : "pointer",
            transition: "background-color 0.15s ease",
          }}
        >
          {isSubmitting ? t("submitting") : t("submitButton")}
        </button>
      </form>
    </div>
  );
}
