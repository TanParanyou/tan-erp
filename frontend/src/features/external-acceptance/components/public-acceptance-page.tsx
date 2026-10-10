"use client";

import { useMemo, useState } from "react";
import { createTranslator, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { ApiError } from "@/lib/api/api-error";
import { formatDateTime } from "@/lib/formatters/formatters";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { QuotationDocumentView, type QuotationDocumentLabel } from "@/features/estimates/components/quotation-document-view";
import { acceptanceLinkErrorCode } from "@/features/estimates/quotation-lifecycle-status";
import { usePublicAcceptance, usePublicAcceptMutation } from "../api/public-acceptance-queries";
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

  const [signerName, setSignerName] = useState("");
  const [signerRole, setSignerRole] = useState("");
  const [consent, setConsent] = useState(false);
  const [signature, setSignature] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

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

  async function submit(): Promise<void> {
    setError(null);
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
      });
    } catch (err: unknown) {
      const code = err instanceof ApiError ? acceptanceLinkErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <main className="mx-auto flex max-w-4xl flex-col gap-5 px-4 py-6">
      <div role="group" aria-label={t("documentLanguage")} className="flex justify-end print:hidden">
        <Button type="button" size="sm" variant={documentLocale === "th" ? "primary" : "outline"} aria-pressed={documentLocale === "th"} onClick={() => setDocumentLocale("th")}>{t("languageTh")}</Button>
        <Button type="button" size="sm" variant={documentLocale === "en" ? "primary" : "outline"} aria-pressed={documentLocale === "en"} onClick={() => setDocumentLocale("en")}>{t("languageEn")}</Button>
      </div>

      <QuotationDocumentView document={document} documentLocale={documentLocale} label={label} />

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
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <Input label={t("signerName")} required maxLength={200} value={signerName} placeholder={view.signerHint ?? undefined} disabled={mutation.isPending} onChange={(event) => setSignerName(event.target.value)} />
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
      </section>
    </main>
  );
}
