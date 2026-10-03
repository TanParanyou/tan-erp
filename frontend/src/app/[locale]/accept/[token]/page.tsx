"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PublicAcceptancePage } from "@/features/external-acceptance/components/public-acceptance-page";

interface PageProps {
  params: Promise<{ locale: string; token: string }>;
}

/** Public customer page. Deliberately outside the ERP layout, so it has no sign-in gate and no ERP navigation. */
export default function Page({ params }: PageProps) {
  const { locale, token } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return <PublicAcceptancePage token={token} uiLocale={locale === "en" ? "en" : "th"} />;
}
