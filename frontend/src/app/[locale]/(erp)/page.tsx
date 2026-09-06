"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { AccessGate } from "@/features/auth";
import { ErpShell } from "@/components/layout/erp-shell";
import { isSupportedLocale, type SupportedLocale } from "@/lib/i18n/locales";

interface ErpHomePageProps {
  params: Promise<{ locale: string }>;
}

export default function ErpHomePage({ params }: ErpHomePageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <AccessGate locale={locale as SupportedLocale}>
      {(currentUser) => (
        <ErpShell locale={locale as SupportedLocale} currentUser={currentUser} />
      )}
    </AccessGate>
  );
}
