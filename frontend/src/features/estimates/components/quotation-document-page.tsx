"use client";

import React, { useMemo, useState } from "react";
import Link from "next/link";
import { createTranslator } from "next-intl";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { useQuotationDocument } from "../api/estimate-queries";
import { QuotationDocumentView, type QuotationDocumentLabel } from "./quotation-document-view";

interface QuotationDocumentPageProps {
  estimateId: string;
  uiLocale: "th" | "en";
}

export function QuotationDocumentPage({ estimateId, uiLocale }: QuotationDocumentPageProps) {
  const t = useTranslations("quotationDocument");
  const tCommon = useTranslations("common");
  const { selectedMembership } = useSelectedMembership();
  const canView = can(selectedMembership, PERMISSIONS.QUOTATIONS_ISSUE);
  const [documentLocale, setDocumentLocale] = useState<"th" | "en">(uiLocale);
  const query = useQuotationDocument(estimateId, documentLocale);

  const label = useMemo<QuotationDocumentLabel>(() => {
    const translator = createTranslator({
      locale: documentLocale,
      messages: documentLocale === "en" ? enMessages : thMessages,
      namespace: "quotationDocument",
    });
    return (key: string) => translator(key as Parameters<typeof translator>[0]);
  }, [documentLocale]);

  if (!canView) {
    return (
      <div role="alert" className="border border-erp-border bg-erp-surface p-6 text-sm">
        {t("noPermission")}
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-wrap items-center justify-between gap-3 print:hidden">
        <Link href={`/${uiLocale}/opportunities`} className="text-sm text-erp-navy underline">
          {t("back")}
        </Link>
        <div className="flex flex-wrap items-center gap-2">
          <div role="group" aria-label={t("documentLanguage")} className="flex">
            <Button
              type="button"
              size="sm"
              variant={documentLocale === "th" ? "primary" : "outline"}
              aria-pressed={documentLocale === "th"}
              onClick={() => setDocumentLocale("th")}
              className="!rounded-none"
            >
              {t("languageTh")}
            </Button>
            <Button
              type="button"
              size="sm"
              variant={documentLocale === "en" ? "primary" : "outline"}
              aria-pressed={documentLocale === "en"}
              onClick={() => setDocumentLocale("en")}
              className="!rounded-none"
            >
              {t("languageEn")}
            </Button>
          </div>
          <Button
            type="button"
            size="sm"
            variant="primary"
            disabled={!query.data}
            onClick={() => window.print()}
            className="!rounded-none bg-erp-navy text-white hover:bg-erp-navy-hover"
          >
            {t("print")}
          </Button>
        </div>
      </div>

      {query.data?.hasIncompleteTranslations && (
        <div role="alert" className="border border-erp-border bg-erp-surface p-3 text-sm print:hidden">
          {t("incompleteTranslations")}
        </div>
      )}

      {query.isPending && (
        <MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" />
      )}

      {query.isError && (
        <div role="alert" className="border border-erp-border bg-erp-surface p-6 text-sm print:hidden">
          {t("loadFailed")}
        </div>
      )}

      {query.data && (
        <QuotationDocumentView document={query.data} documentLocale={documentLocale} label={label} />
      )}
    </div>
  );
}
