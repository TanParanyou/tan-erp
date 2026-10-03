"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { QuotationDocumentPage } from "@/features/estimates/components/quotation-document-page";
import { isSupportedLocale } from "@/lib/i18n/locales";

interface QuotationPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function QuotationPage({ params }: QuotationPageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return <QuotationDocumentPage estimateId={id} uiLocale={locale === "en" ? "en" : "th"} />;
}
