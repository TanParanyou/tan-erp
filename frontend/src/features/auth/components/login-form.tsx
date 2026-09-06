"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { signInWithEmail } from "@/lib/auth/auth-session";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

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
    <div className="erp-card login-card" style={{ maxWidth: "420px", width: "100%", margin: "0 auto", padding: "2rem" }}>
      <header style={{ marginBottom: "1.5rem" }}>
        <h1 style={{ fontSize: "1.5rem", color: "var(--erp-navy)", margin: "0 0 0.5rem 0", fontWeight: 700 }}>
          {t("loginTitle")}
        </h1>
        <p style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)", margin: 0, lineHeight: 1.4 }}>
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
              backgroundColor: "var(--erp-danger-bg)",
              border: "1px solid var(--erp-danger)",
              color: "#991B1B",
              fontSize: "0.875rem",
              borderRadius: 0,
            }}
          >
            {errorMessage}
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
