"use client";

import { useMemo, useState } from "react";
import { createTranslator, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { ApiError } from "@/lib/api/api-error";
import { formatDateTime } from "@/lib/formatters/formatters";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { QuotationDocumentView, type QuotationDocumentLabel } from "@/features/estimates/components/quotation-document-view";
import { acceptanceLinkErrorCode } from "@/features/estimates/quotation-lifecycle-status";
import { usePublicAcceptance, usePublicAcceptMutation, usePublicPdfDownload, useRequestOtpMutation, useVerifyOtpMutation } from "../api/public-acceptance-queries";
import { SignaturePad } from "./signature-pad";

interface PublicAcceptancePageProps {
  token: string;
  uiLocale: "th" | "en";
}

/** Customer-facing page: no ERP navigation, no login. It shows the same customer-safe quotation document, then collects consent and a signer. */
export function PublicAcceptancePage({ token, uiLocale }: PublicAcceptancePageProps) {
  const t = useTranslations("externalAcceptance.public");
  const tErrors = useTranslations("externalAcceptance.errors");
  const tCommon = useTranslations("common");
  const [documentLocale, setDocumentLocale] = useState<"th" | "en">(uiLocale);
  const query = usePublicAcceptance(token, documentLocale);
  const mutation = usePublicAcceptMutation(token, documentLocale);
  const pdfDownload = usePublicPdfDownload(token);
  const requestOtp = useRequestOtpMutation(token, documentLocale);
  const verifyOtp = useVerifyOtpMutation(token, documentLocale);

  const [signerName, setSignerName] = useState("");
  const [signerRole, setSignerRole] = useState("");
  const [consent, setConsent] = useState(false);
  const [signature, setSignature] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [otpCode, setOtpCode] = useState("");
  const [otpSent, setOtpSent] = useState(false);
  const [pickedChannel, setPickedChannel] = useState("");
  const [otpToken, setOtpToken] = useState<string | null>(null);

  const label = useMemo<QuotationDocumentLabel>(() => {
    const translator = createTranslator({
      locale: documentLocale,
      messages: documentLocale === "en" ? enMessages : thMessages,
      namespace: "quotationDocument",
    });
    return (key: string) => translator(key as Parameters<typeof translator>[0]);
  }, [documentLocale]);

  if (query.isPending) {
    return <div className="flex justify-center py-16"><MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" /></div>;
  }

  if (query.isError || !query.data?.document) {
    return (
      <div role="alert" className="mx-auto max-w-xl border border-erp-border bg-erp-surface p-6 text-sm">
        <h1 className="mb-2 text-base font-bold text-erp-navy">{t("unavailableTitle")}</h1>
        <p>{t("unavailableDetail")}</p>
      </div>
    );
  }

  const view = query.data;
  const document = query.data.document;
  const accepted = view.status === "accepted" || mutation.isSuccess;

  const otpRequired = view.otpRequired;
  const otpChannels = view.otpChannels ?? [];
  // The customer only chooses when the server offers more than one channel; with one, it is used directly.
  const otpChannel = otpChannels.length === 1 ? otpChannels[0] : pickedChannel;

  function describeError(err: unknown): string {
    const code = err instanceof ApiError ? acceptanceLinkErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function sendCode(): Promise<void> {
    setError(null);
    try {
      await requestOtp.mutateAsync(otpChannel);
      setOtpSent(true);
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  async function verifyCode(): Promise<void> {
    setError(null);
    try {
      const verified = await verifyOtp.mutateAsync(otpCode.trim());
      if (!verified.verificationToken) {
        setError(tErrors("failed"));
        return;
      }

      setOtpToken(verified.verificationToken);
      setOtpCode("");
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  async function submit(): Promise<void> {
    setError(null);
    if (otpRequired && !otpToken) {
      setError(tErrors("ACCEPTANCE_OTP_REQUIRED"));
      return;
    }
    if (signerName.trim().length < 2) {
      setError(t("nameRequired"));
      return;
    }

    if (!consent) {
      setError(t("consentRequired"));
      return;
    }

    try {
      await mutation.mutateAsync({
        signerName: signerName.trim(),
        signerRole: signerRole.trim() || null,
        consentAccepted: true,
        consentVersion: view.consentVersion,
        signatureImage: signature,
        // The hash of the PDF shown above; null only when the quotation has no stored PDF. The server rejects any other value.
        documentSha256: view.pdf?.sha256 ?? null,
        otpVerificationToken: otpRequired ? otpToken : null,
      });
    } catch (err: unknown) {
      if (err instanceof ApiError && (err.code === "ACCEPTANCE_OTP_INVALID" || err.code === "ACCEPTANCE_OTP_EXPIRED")) setOtpToken(null);
      setError(describeError(err));
    }
  }

  return (
    <main className="mx-auto flex max-w-4xl flex-col gap-5 px-4 py-6">
      <div role="group" aria-label={t("documentLanguage")} className="flex justify-end print:hidden">
        <Button type="button" size="sm" variant={documentLocale === "th" ? "primary" : "outline"} aria-pressed={documentLocale === "th"} onClick={() => setDocumentLocale("th")}>{t("languageTh")}</Button>
        <Button type="button" size="sm" variant={documentLocale === "en" ? "primary" : "outline"} aria-pressed={documentLocale === "en"} onClick={() => setDocumentLocale("en")}>{t("languageEn")}</Button>
      </div>

      <QuotationDocumentView document={document} documentLocale={documentLocale} label={label} />

      {view.pdf?.sha256 && document.number && (
        <section className="erp-card space-y-3 p-5 print:hidden" aria-label={t("pdfTitle")}>
          <h2 className="text-base font-bold text-erp-navy">{t("pdfTitle")}</h2>
          <p className="text-xs text-erp-text-muted">{t("pdfDescription")}</p>
          <div className="flex flex-wrap items-center gap-3">
            <Button type="button" variant="outline" isLoading={pdfDownload.isPending} disabled={pdfDownload.isPending}
              onClick={() => void pdfDownload.mutateAsync(`${document.number}.pdf`).catch(() => setError(t("pdfFailed")))}>
              {t("pdfDownload")}
            </Button>
            <span className="break-all font-mono text-xs text-erp-text-muted">{t("pdfHash", { hash: view.pdf.sha256 })}</span>
          </div>
        </section>
      )}

      <section className="erp-card space-y-4 p-5 print:hidden" aria-label={t("acceptTitle")}>
        {accepted ? (
          <Alert variant="success">
            <p className="font-semibold">{t("acceptedTitle")}</p>
            <p className="text-xs">{t("acceptedDetail", { date: view.acceptedAtUtc ? formatDateTime(view.acceptedAtUtc, documentLocale) : "-" })}</p>
          </Alert>
        ) : (
          <>
            <h2 className="text-base font-bold text-erp-navy">{t("acceptTitle")}</h2>
            <p className="text-xs text-erp-text-muted">{t("expires", { date: formatDateTime(view.expiresAtUtc ?? "", documentLocale) })}</p>
            {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
            {otpRequired && !otpToken ? (
              <div className="space-y-3" aria-label={t("otpTitle")}>
                <h3 className="text-sm font-bold text-erp-navy">{t("otpTitle")}</h3>
                <p className="text-xs text-erp-text-muted">{t("otpDescription")}</p>
                {otpChannels.length === 0 && <Alert variant="danger">{tErrors("ACCEPTANCE_OTP_UNAVAILABLE")}</Alert>}
                {otpChannels.length > 1 && (
                  <Select
                    label={t("otpChannel")}
                    placeholder={t("otpChannelPlaceholder")}
                    value={pickedChannel}
                    disabled={otpSent || requestOtp.isPending}
                    options={otpChannels.map((channel) => ({ value: channel, label: t(channel === "sms" ? "otpChannelSms" : "otpChannelEmail") }))}
                    onChange={(event) => setPickedChannel(event.target.value)}
                  />
                )}
                {otpSent && <Alert variant="info">{t("otpSent")}</Alert>}
                <Button type="button" variant="outline" isLoading={requestOtp.isPending} disabled={requestOtp.isPending || !otpChannel} onClick={() => void sendCode()}>
                  {otpSent ? t("otpResend") : t("otpSend")}
                </Button>
                {otpSent && (
                  <div className="flex flex-wrap items-end gap-3">
                    <Input label={t("otpCode")} inputMode="numeric" autoComplete="one-time-code" maxLength={9} value={otpCode} placeholder={t("otpCodePlaceholder")} disabled={verifyOtp.isPending} onChange={(event) => setOtpCode(event.target.value)} />
                    <Button type="button" variant="primary" isLoading={verifyOtp.isPending} disabled={verifyOtp.isPending || otpCode.trim().length === 0} onClick={() => void verifyCode()}>{t("otpVerify")}</Button>
                  </div>
                )}
              </div>
            ) : (
            <>
            {otpRequired && <Alert variant="success">{t("otpVerified")}</Alert>}
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <Input label={t("signerName")} required maxLength={200} value={signerName} placeholder={view.signerHint ?? t("signerNamePlaceholder")} disabled={mutation.isPending} onChange={(event) => setSignerName(event.target.value)} />
              <Input label={t("signerRole")} maxLength={100} value={signerRole} placeholder={t("signerRolePlaceholder")} disabled={mutation.isPending} onChange={(event) => setSignerRole(event.target.value)} />
            </div>
            <SignaturePad label={t("signature")} clearLabel={t("clearSignature")} disabled={mutation.isPending} onChange={setSignature} />
            <Checkbox
              checked={consent}
              disabled={mutation.isPending}
              onChange={(event) => setConsent(event.target.checked)}
              label={t("consentText")}
            />
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutation.isPending} disabled={mutation.isPending} onClick={() => void submit()}>{t("accept")}</Button>
            </>
            )}
          </>
        )}
      </section>
    </main>
  );
}
