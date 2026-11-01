"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { useTranslations, useLocale } from "next-intl";
import { Logo } from "@/components/common/Logo";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { OtpInput } from "@/components/ui/OtpInput";
import { QRCodeDisplay } from "@/components/ui/QRCodeDisplay";
import { CopyButton } from "@/components/common/CopyButton";
import { ApiError } from "@/lib/api/api-error";
import {
  signInWithEmailFlow,
  signInWithGoogleFlow,
  verifyTwoFactorLoginFlow,
  type AuthFlowOutcome,
} from "@/lib/auth/auth-session";

declare global {
  interface Window {
    google?: {
      accounts: {
        id: {
          initialize: (config: {
            client_id: string;
            callback: (response: { credential: string }) => void;
            auto_select?: boolean;
          }) => void;
          prompt: () => void;
        };
      };
    };
  }
}

/* eslint-disable no-restricted-syntax -- token-exempt: Google official brand logo SVG colors */
function GoogleMark({ size = 18 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" aria-hidden="true" className="shrink-0">
      <path
        fill="#4285F4"
        d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
      />
      <path
        fill="#34A853"
        d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
      />
      <path
        fill="#FBBC05"
        d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.06H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.94l2.85-2.22.81-.63z"
      />
      <path
        fill="#EA4335"
        d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.06l3.66 2.84c.87-2.6 3.3-4.52 6.16-4.52z"
      />
    </svg>
  );
}
/* eslint-enable no-restricted-syntax */

function getAuthErrorMessage(error: unknown, t: (key: string) => string): string {
  if (error instanceof ApiError && error.code) {
    switch (error.code) {
      case "INVALID_CREDENTIALS":
        return t("invalidCredentials");
      case "ACCOUNT_LOCKED_OUT":
        return t("accountLockedOut");
      case "USER_NOT_INVITED":
        return t("userNotInvited");
      case "USER_ACCESS_DISABLED":
        return t("userAccessDisabled");
      case "GOOGLE_AUTH_NOT_CONFIGURED":
        return t("googleAuthNotConfigured");
      case "GOOGLE_TOKEN_INVALID":
        return t("googleTokenInvalid");
      case "GOOGLE_ACCOUNT_MISMATCH":
        return t("googleAccountMismatch");
      case "TWO_FACTOR_CODE_INVALID":
        return t("twoFactorCodeInvalid");
      case "TWO_FACTOR_CHALLENGE_INVALID":
        return t("twoFactorChallengeInvalid");
      default:
        break;
    }
  }

  const code = error && typeof error === "object" && "code" in error ? (error as { code?: unknown }).code : undefined;
  if (
    code === "auth/invalid-credential" ||
    code === "auth/user-not-found" ||
    code === "auth/wrong-password" ||
    code === "INVALID_CREDENTIALS"
  ) {
    return t("invalidCredentials");
  }

  return t("signInFailed");
}

type LoginStep = "credentials" | "verify_2fa" | "setup_2fa" | "backup_codes";

interface LoginFormProps {
  onSuccess?: () => void;
}

export function LoginForm({ onSuccess }: LoginFormProps = {}) {
  const router = useRouter();
  const t = useTranslations("auth");
  const locale = useLocale();

  const [step, setStep] = useState<LoginStep>("credentials");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [challengeToken, setChallengeToken] = useState<string>("");
  const [totpSetup, setTotpSetup] = useState<{ secretKey: string; otpAuthUri: string } | null>(null);
  const [backupCodes, setBackupCodes] = useState<string[]>([]);
  const [twoFactorCode, setTwoFactorCode] = useState("");
  const [isBackupCodeMode, setIsBackupCodeMode] = useState(false);

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [warningMessage, setWarningMessage] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<{ email?: string; password?: string }>({});

  const validateCredentials = (): boolean => {
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

  const handleOutcome = (outcome: AuthFlowOutcome) => {
    if (outcome.status === "authenticated") {
      if (outcome.backupCodes && outcome.backupCodes.length > 0) {
        setBackupCodes(outcome.backupCodes);
        setStep("backup_codes");
      } else {
        if (onSuccess) {
          onSuccess();
        } else {
          router.push(`/${locale}`);
        }
      }
    } else if (outcome.status === "requires_two_factor") {
      setChallengeToken(outcome.challengeToken);
      setTwoFactorCode("");
      setIsBackupCodeMode(false);
      setStep("verify_2fa");
    } else if (outcome.status === "requires_two_factor_setup") {
      setChallengeToken(outcome.challengeToken);
      setTotpSetup(outcome.totpSetup);
      setTwoFactorCode("");
      setStep("setup_2fa");
    }
  };

  const handleCredentialsSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isSubmitting) return;

    setErrorMessage(null);
    setWarningMessage(null);
    if (!validateCredentials()) return;

    setIsSubmitting(true);
    try {
      const outcome = await signInWithEmailFlow(email, password);
      handleOutcome(outcome);
    } catch (err: unknown) {
      setErrorMessage(getAuthErrorMessage(err, (k) => t(k)));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleGoogleSignIn = () => {
    setErrorMessage(null);
    setWarningMessage(null);

    const clientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;
    if (!clientId) {
      setWarningMessage(t("googleClientNotConfigured"));
      return;
    }

    if (typeof window === "undefined") return;

    const onCredentialReceived = async (credential: string) => {
      setIsSubmitting(true);
      try {
        const outcome = await signInWithGoogleFlow(credential);
        handleOutcome(outcome);
      } catch (err: unknown) {
        setErrorMessage(getAuthErrorMessage(err, (k) => t(k)));
      } finally {
        setIsSubmitting(false);
      }
    };

    if (window.google?.accounts?.id) {
      window.google.accounts.id.initialize({
        client_id: clientId,
        callback: (res) => {
          if (res.credential) {
            void onCredentialReceived(res.credential);
          }
        },
      });
      window.google.accounts.id.prompt();
    } else {
      const script = document.createElement("script");
      script.src = "https://accounts.google.com/gsi/client";
      script.async = true;
      script.onload = () => {
        window.google?.accounts.id.initialize({
          client_id: clientId,
          callback: (res) => {
            if (res.credential) {
              void onCredentialReceived(res.credential);
            }
          },
        });
        window.google?.accounts.id.prompt();
      };
      document.body.appendChild(script);
    }
  };

  const handleTwoFactorSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isSubmitting || !challengeToken || !twoFactorCode.trim()) return;

    setErrorMessage(null);
    setWarningMessage(null);
    setIsSubmitting(true);

    try {
      const outcome = await verifyTwoFactorLoginFlow(challengeToken, twoFactorCode);
      handleOutcome(outcome);
    } catch (err: unknown) {
      setErrorMessage(getAuthErrorMessage(err, (k) => t(k)));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleFinishLogin = () => {
    if (onSuccess) {
      onSuccess();
    } else {
      router.push(`/${locale}`);
    }
  };

  return (
    <div className="login-card border border-erp-border bg-erp-surface p-6 sm:p-8 shadow-sm">
      <header className="mb-6">
        <div className="mb-5">
          <Logo iconSize={32} />
        </div>

        <h1 className="text-xl font-bold text-erp-navy mb-1.5">
          {step === "verify_2fa"
            ? t("twoFactorTitle")
            : step === "setup_2fa"
            ? t("mandatoryTwoFactorTitle")
            : step === "backup_codes"
            ? t("backupCodesTitle")
            : t("loginTitle")}
        </h1>
        <p className="text-sm text-erp-text-muted leading-relaxed">
          {step === "verify_2fa"
            ? t("twoFactorPrompt")
            : step === "setup_2fa"
            ? t("mandatoryTwoFactorDesc")
            : step === "backup_codes"
            ? t("backupCodesNotice")
            : t("loginNotice")}
        </p>
      </header>

      {errorMessage && (
        <div className="mb-5">
          <Alert variant="danger">{errorMessage}</Alert>
        </div>
      )}

      {warningMessage && (
        <div className="mb-5">
          <Alert variant="warning">{warningMessage}</Alert>
        </div>
      )}

      {/* STEP 1: Credentials */}
      {step === "credentials" && (
        <form onSubmit={handleCredentialsSubmit} noValidate aria-busy={isSubmitting}>
          <div className="space-y-4">
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
          </div>

          <div className="mt-6">
            <Button
              type="submit"
              variant="primary"
              size="md"
              isLoading={isSubmitting}
              disabled={isSubmitting}
              className="w-full h-11"
              style={{ height: "44px" }}
            >
              {isSubmitting ? t("submitting") : t("submitButton")}
            </Button>
          </div>

          <div className="relative my-6">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-erp-border" />
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-erp-surface px-3 text-erp-text-muted font-medium">
                {t("orDivider")}
              </span>
            </div>
          </div>

          <Button
            type="button"
            variant="outline"
            size="md"
            onClick={handleGoogleSignIn}
            disabled={isSubmitting}
            className="w-full h-11 flex items-center justify-center gap-2 border border-erp-border"
            style={{ height: "44px" }}
          >
            <GoogleMark size={18} />
            <span>{t("signInWithGoogle")}</span>
          </Button>
        </form>
      )}

      {/* STEP 2: Verify 2FA Challenge */}
      {step === "verify_2fa" && (
        <form onSubmit={handleTwoFactorSubmit} noValidate aria-busy={isSubmitting} className="space-y-5">
          {!isBackupCodeMode ? (
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-erp-text-main block">
                {t("authenticatorCodeLabel")}
              </label>
              <div className="flex justify-center">
                <OtpInput value={twoFactorCode} onChange={setTwoFactorCode} />
              </div>
            </div>
          ) : (
            <Input
              id="backupCode"
              name="backupCode"
              label={t("backupCodeLabel")}
              placeholder={t("backupCodePlaceholder")}
              value={twoFactorCode}
              onChange={(e) => setTwoFactorCode(e.target.value)}
              disabled={isSubmitting}
              required
            />
          )}

          <div className="flex justify-end">
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => {
                setIsBackupCodeMode(!isBackupCodeMode);
                setTwoFactorCode("");
                setErrorMessage(null);
              }}
              className="text-xs text-erp-navy hover:underline"
            >
              {isBackupCodeMode ? t("useAuthenticatorCode") : t("useBackupCode")}
            </Button>
          </div>

          <div className="space-y-3 pt-2">
            <Button
              type="submit"
              variant="primary"
              size="md"
              isLoading={isSubmitting}
              disabled={isSubmitting || !twoFactorCode.trim()}
              className="w-full h-11"
              style={{ height: "44px" }}
            >
              {isSubmitting ? t("verifying") : t("verifyButton")}
            </Button>

            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => {
                setStep("credentials");
                setTwoFactorCode("");
                setErrorMessage(null);
              }}
              className="w-full"
            >
              {t("backToSignIn")}
            </Button>
          </div>
        </form>
      )}

      {/* STEP 3: Mandatory 2FA Setup */}
      {step === "setup_2fa" && totpSetup && (
        <form onSubmit={handleTwoFactorSubmit} noValidate aria-busy={isSubmitting} className="space-y-5">
          <div className="border border-erp-border bg-erp-surface-subtle p-4 space-y-4">
            <div className="flex justify-center">
              <QRCodeDisplay value={totpSetup.otpAuthUri} size={150} />
            </div>
            <div className="text-xs space-y-2 text-center sm:text-left">
              <p className="font-semibold text-erp-text-main">{t("setupQrInstructions")}</p>
              <p className="text-erp-text-muted">{t("secretKeyLabel")}</p>
              <div className="flex items-center justify-center sm:justify-start gap-2">
                <code className="bg-erp-surface border border-erp-border px-2 py-1 font-mono text-xs font-bold text-erp-text-main">
                  {totpSetup.secretKey}
                </code>
                <CopyButton text={totpSetup.secretKey} label={t("copyAllCodes")} />
              </div>
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-erp-text-main block">
              {t("enterOtpLabel")}
            </label>
            <div className="flex justify-center">
              <OtpInput value={twoFactorCode} onChange={setTwoFactorCode} />
            </div>
          </div>

          <div className="space-y-3 pt-2">
            <Button
              type="submit"
              variant="primary"
              size="md"
              isLoading={isSubmitting}
              disabled={isSubmitting || twoFactorCode.length < 6}
              className="w-full h-11"
              style={{ height: "44px" }}
            >
              {isSubmitting ? t("verifying") : t("verifyAndActivate")}
            </Button>

            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => {
                setStep("credentials");
                setTwoFactorCode("");
                setErrorMessage(null);
              }}
              className="w-full"
            >
              {t("backToSignIn")}
            </Button>
          </div>
        </form>
      )}

      {/* STEP 4: One-Time Backup Codes Confirmation */}
      {step === "backup_codes" && (
        <div className="space-y-5">
          <div className="grid grid-cols-2 gap-2.5 p-4 border border-erp-border bg-erp-surface-subtle">
            {backupCodes.map((code, idx) => (
              <code
                key={idx}
                className="font-mono text-sm font-semibold text-erp-text-main text-center py-1.5 bg-erp-surface border border-erp-border tracking-wider"
              >
                {code}
              </code>
            ))}
          </div>

          <div className="flex justify-end">
            <CopyButton text={backupCodes.join("\n")} label={t("copyAllCodes")} />
          </div>

          <div className="pt-2">
            <Button
              type="button"
              variant="primary"
              size="md"
              onClick={handleFinishLogin}
              className="w-full h-11"
              style={{ height: "44px" }}
            >
              {t("backupCodesSavedContinue")}
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
